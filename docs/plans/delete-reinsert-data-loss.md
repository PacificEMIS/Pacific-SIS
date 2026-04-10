# Delete-Reinsert Pattern — Data Loss & Audit Destruction

Related issue: https://github.com/PacificEMIS/Pacific-SIS/issues/561

## Status

**Acute data loss resolved, remaining work deferred.** The two repositories
actively causing damage (`InputFinalGradeRepository` and
`StudentEffortGradeRepository`) have been fixed. Every other repository
flagged by the original inventory has been investigated and categorised
as dead code, not in use, false positive, or low-priority audit-only —
none are actively causing data loss or damage to any Pacific EMIS tenant.

This document is now primarily an **audit-and-resolution record** rather
than an open to-do list. See the per-repository sections below for full
details on each resolution.

---

## The Pattern

Multiple backend repositories use a "delete all, then re-insert" approach when saving
data that could be updated in place:

```csharp
// 1. Delete ALL existing records for the scope
this.context.StudentFinalGrade.RemoveRange(existingRecords);
this.context.SaveChanges();

// 2. Insert fresh records from the frontend payload
this.context.StudentFinalGrade.AddRange(newRecords);
this.context.SaveChanges();
```

This destroys two things:

1. **Audit timestamps** — `created_on`/`created_by` from the original entry are lost,
   replaced with the current save's timestamps. Makes it impossible to know when data
   was first entered or by whom.

2. **Records for entities not in the payload** — if the frontend filters out certain
   records before submission (e.g. dropped students), the backend deletes them anyway
   because it wipes the entire scope before inserting only what was sent.

---

## Active Data Loss: Dropped Students' Grades

This is the most urgent consequence. The current flow when a teacher saves grades:

1. Frontend roster shows dropped students grayed out (inputs disabled)
2. `submitFinalGrade()` filters out dropped students: `filter(val => !val.isDropped)`
3. Backend receives only active students' grades
4. Backend **deletes ALL grades** for the course section + marking period (including
   the dropped student's legitimate grades from when they were enrolled)
5. Backend inserts only what was sent (active students only)

**Result:** A dropped student's real grades are silently destroyed every time any
teacher re-saves grades for that marking period. The dropped student's Grade 1 Q1
grades vanish the next time the Q1 grades are touched — even though the student was
enrolled and earned those grades legitimately.

### No marking-period awareness in the roster

The roster query (`GetStudentListByCourseSection`) does not filter dropped students
by marking period. A student dropped on Oct 15 appears grayed out for ALL marking
periods — Q1 (where they earned grades), Q2, Q3, etc. There is no logic to show them
as active for periods they were enrolled and dropped for periods after their exit.

### Diagnostic query

The following query (run against a tenant database) identifies course sections where
this data loss may have occurred — grades were re-saved after a student was dropped:

```sql
SELECT DISTINCT
    sm_school.school_name,
    cs.course_section_name,
    CONCAT(staff.first_given_name, ' ', staff.last_family_name) AS teacher,
    CONCAT(dropped_student.first_given_name, ' ', dropped_student.last_family_name)
        AS dropped_student_name,
    scs_dropped.effective_drop_date AS student_dropped_on,
    MAX(sfg.updated_on) AS grades_re_saved_on,
    COUNT(DISTINCT sfg.student_id) AS student_grades_affected
FROM student_coursesection_schedule scs_dropped
JOIN student_master dropped_student
    ON dropped_student.tenant_id = scs_dropped.tenant_id
    AND dropped_student.school_id = scs_dropped.school_id
    AND dropped_student.student_id = scs_dropped.student_id
JOIN student_final_grade sfg
    ON sfg.tenant_id = scs_dropped.tenant_id
    AND sfg.school_id = scs_dropped.school_id
    AND sfg.course_section_id = scs_dropped.course_section_id
    AND sfg.updated_on IS NOT NULL
    AND sfg.updated_on > scs_dropped.effective_drop_date
JOIN course_section cs
    ON cs.tenant_id = scs_dropped.tenant_id
    AND cs.school_id = scs_dropped.school_id
    AND cs.course_section_id = scs_dropped.course_section_id
JOIN school_master sm_school
    ON sm_school.tenant_id = scs_dropped.tenant_id
    AND sm_school.school_id = scs_dropped.school_id
LEFT JOIN staff_coursesection_schedule staff_cs
    ON staff_cs.tenant_id = scs_dropped.tenant_id
    AND staff_cs.school_id = scs_dropped.school_id
    AND staff_cs.course_section_id = scs_dropped.course_section_id
    AND staff_cs.is_dropped != 1
LEFT JOIN staff_master staff
    ON staff.tenant_id = staff_cs.tenant_id
    AND staff.school_id = staff_cs.school_id
    AND staff.staff_id = staff_cs.staff_id
WHERE scs_dropped.is_dropped = 1
    AND scs_dropped.effective_drop_date IS NOT NULL
GROUP BY
    sm_school.school_name,
    cs.course_section_name,
    staff.first_given_name,
    staff.last_family_name,
    dropped_student.first_given_name,
    dropped_student.last_family_name,
    scs_dropped.effective_drop_date
ORDER BY grades_re_saved_on DESC;
```

Note: `updated_on IS NOT NULL` confirms the grade went through the re-save path (the
"update" code path sets `updated_on` but not `created_on`; first-time saves do the
opposite). However, because the delete-reinsert pattern destroys the original
`created_on`, we cannot prove the original grades existed before the drop — only that
grades were re-saved after it. This list represents "at risk" course sections that
school admins should verify with teachers.

---

## Affected Repositories (full inventory)

All instances of RemoveRange + AddRange on the same entity scope within a transaction:

### Grade-related (highest priority — student-facing data)

| Repository | Method | Tables | Status |
|---|---|---|---|
| InputFinalGradeRepository | AddUpdateStudentFinalGrade | student_final_grade, _comments, _standard | DONE |
| StudentEffortGradeRepository | AddUpdateStudentEffortGrade | student_effort_grade_master, _detail | DONE |
| ReportCardRepository | 2 methods | student_report_card_master, _detail | DEAD CODE — skip |
| StaffPortalGradebookRepository | 4 methods | gradebook_grades, gradebook_configuration_* | DEFERRED — see [gradebook-repository-upsert.md](gradebook-repository-upsert.md) |
| StudentHistoricalGradeRepository | AddUpdateHistoricalGrade | historical_grade, historical_credit_transfer | DEFERRED — see [historical-grade-repository-upsert.md](historical-grade-repository-upsert.md) |

### Student record data

| Repository | Method | Tables | Status |
|---|---|---|---|
| StudentRepository | Transcript creation | student_transcript_master, _detail | DEFERRED — not in use (zero rows in DB) |
| StudentRepository | Parent association | parent_associationship | NOT APPLICABLE — false positive |

### Configuration/scheduling (lower priority)

| Repository | Method | Tables | Status |
|---|---|---|---|
| CourseManagerRepository | 4 methods | course_standard, course_variable_schedule, course_calendar_schedule, course_block_schedule | DEFERRED — audit-only, low priority |

---

## Upsert Pattern Used for the Fixes

The two repositories that were fixed use a consistent upsert-by-business-key
approach that has been proven in production.

**Business key**: the composite key of the row within the already-filtered
scope. For `InputFinalGradeRepository`, that's `StudentId` within a course
section + marking period. For `StudentEffortGradeRepository`, also
`StudentId` within marking period + academic year.

**Pattern**:

```
// Pseudocode
var existingByKey = existingRecords.ToDictionary(r => r.StudentId);

foreach (incoming in incomingList)
{
    if (existingByKey.TryGetValue(incoming.StudentId, out var existing))
    {
        // UPDATE — preserve CreatedOn/CreatedBy, set UpdatedOn/UpdatedBy
        existing.PercentMarks = incoming.PercentMarks;
        existing.UpdatedOn = DateTime.UtcNow;
        existing.UpdatedBy = submittedBy;
        // ... update child collections (comments, standards)
    }
    else
    {
        // INSERT — new student getting graded for the first time
        var newRecord = new StudentFinalGrade { ... };
        newRecord.CreatedOn = DateTime.UtcNow;
        newRecord.CreatedBy = submittedBy;
        context.StudentFinalGrade.Add(newRecord);
    }
}

// DO NOT delete records for keys not in the incoming list.
// Dropped students' grades must be preserved.
```

**Key principle**: only touch what you're given. Students absent from the
submission (dropped students, transferred students) have their existing
grades left untouched.

**EF tracking subtlety** (learned during the StudentEffortGrade fix): loading
existing entities as tracked and then mixing with `Add()` of new entities
can cause "another instance with the same key is already being tracked"
errors. The safer approach is:
- Load existing records with `AsNoTracking()`.
- For updates, construct a fresh entity copying preserved audit fields
  from the no-track snapshot, then attach as `Modified` via `Update()`.
- For inserts, construct and `Add()` as normal.

Both fixed repositories now use this safer approach.

---

## Resolution Summary (April 2026)

### Issue #819 — Grade misalignment (separate prior fix)

The **grade data misalignment bug** was a separate but related problem in the
frontend. Grade inputs were bound to students by array index instead of by
studentId, causing data corruption when the roster order differed from the grade
list order (e.g. after dropping a student). Fixed by rebuilding the grade array
using a Map keyed by studentId. See branch `issue819`.

### Issue #821 — InputFinalGrade upsert + audit display

- **InputFinalGradeRepository.AddUpdateStudentFinalGrade** rewritten as an
  upsert keyed by `StudentId`. Existing grades for students absent from the
  submission (dropped students) are now left untouched. `CreatedOn`/`CreatedBy`
  are preserved across re-saves; `UpdatedOn`/`UpdatedBy` reflect the last
  modification.
- **GetAllStudentFinalGradeList** now resolves the `CreatedBy`/`UpdatedBy`
  staff GUIDs to human-readable names via a lookup against `StaffMaster`,
  populated into `[NotMapped]` properties on the entity.
- **Frontend audit display** added to both the admin grade input
  (`pages/class/grades/input-final-grades`) and the teacher portal grade input
  (`pages/staff/teacher-function/input-final-grade/grade-details`). An info
  icon in a new "Audit" column shows a tooltip with created/updated timestamps
  and user names. Timestamps are parsed as UTC and rendered in the user's
  local timezone.

### StudentEffortGrade upsert + audit display (this branch, follow-up)

- **StudentEffortGradeRepository.AddUpdateStudentEffortGrade** rewritten as
  an upsert keyed by `StudentId`. Note that the original code in this
  repository was already filtered by `studentIds.Contains(...)`, so dropped
  students' rows weren't being deleted on every save like InputFinalGrade —
  but `CreatedOn`/`CreatedBy` were still being wiped on every re-save. The
  fix here is primarily about preserving audit history.
- During the rewrite, two pre-existing performance/correctness issues were
  also corrected:
  - The original code loaded the **entire** `student_effort_grade_detail`
    table into the EF tracker just to compute the next surrogate `Id`.
    Replaced with a simple `MAX(Id)` projection using `AsNoTracking()`.
  - The marking-period filter used an OR across all four marking-period
    columns including `0` defaults, which could match unrelated records.
    Tightened to compare only the non-zero ID.
- **Approach for the upsert**: load existing records with `AsNoTracking()`
  rather than tracking them. For updates, build a fresh `StudentEffortGradeMaster`
  copying preserved fields (notably `CreatedOn`/`CreatedBy`) from the
  no-track snapshot, then attach as `Modified` via `Update()`. Detail rows
  for updated students are deleted by srlno and re-inserted fresh. New
  students follow a clean insert path. This avoids EF change-tracker
  conflicts that arose from mixing tracked existing entities with new ones
  in the same context.
- **GetStudentListByHomeRoomStaff** now populates `CreatedBy`/`CreatedOn`/
  `UpdatedBy`/`UpdatedOn` in the projection and resolves the staff GUIDs
  to readable names, same approach as InputFinalGrade.
- **Frontend audit display** added to the shared `EffortGradeDetailsComponent`
  (used by both the admin and teacher routes). Info icon next to the student
  name in the left list, with a tooltip showing the audit history.
- **Note on per-student audit timestamps:** the entire roster is submitted
  on every Submit click, so all students in the roster get the same
  `UpdatedOn`. This is by design of the existing UI workflow, not a
  consequence of the upsert rewrite. True per-cell audit would require
  frontend dirty-row tracking — out of scope for this fix.
- **Related companion doc:** [effort-grades-feature.md](effort-grades-feature.md)
  documents the broader effort grades feature: setup requirements, the
  "Homeroom Teacher" profile gate, the strict course-section date
  containment rule, the absence of any reporting consumer, and the
  step-by-step test plan used to verify this fix.

### StaffPortalGradebookRepository — investigated and deferred

Four methods use the delete-reinsert pattern across `gradebook_grades`
and `gradebook_configuration_*` tables:

- `AddUpdateGradebookConfiguration`
- `AddGradebookGrade`
- `AddGradebookGradeByStudent`
- `AddgradebookGradeByAssignmentType`

All four are wired to live frontend components and are technically
active code. However:

- **Data loss risk is low** — the grade-save methods filter by
  `StudentId` + scope before deleting, so dropped-student grade loss
  (the severe issue from InputFinalGrade) does not apply here.
- **Audit destruction risk is high** — every re-save wipes `CreatedBy`/
  `CreatedOn` for the touched rows.
- **No Pacific EMIS tenant is currently using gradebook**, so the
  audit destruction has no active victims.
- **Fix complexity is higher** than the previous two repositories —
  four methods, composite key with `AssignmentTypeId` + `AssignmentId`,
  plus a separate configuration method with different shape and manual
  PK generation fragility.

Given the lower urgency and higher complexity, this work is **deferred
to its own standalone plan** and should be tackled separately when
gradebook becomes actively used or as a scheduled cleanup pass. See
[gradebook-repository-upsert.md](gradebook-repository-upsert.md) for
full analysis, proposed approach, and testing strategy.

### CourseManagerRepository — investigated and deferred

Four methods using delete-reinsert on parent-child configuration
relationships:
- `CourseStandard` children of a course
- `CourseVariableSchedule`, `CourseCalendarSchedule`, `CourseBlockSchedule`
  children of a course section

Data loss risk: **zero**. These are structural parent-child relationships
where the children don't exist independently of the parent and are
always submitted as a complete set. There's no concept of "external
entities to preserve" — children are bound to their parent by design.

Audit destruction risk: yes, but only on **course configuration**
records, which are set up when courses are created and rarely touched
after. Unlike student grade data which is edited throughout the term,
course schedules and standards typically get their `created_on` set
once and left alone.

Deferred as audit-only, low priority. Worth revisiting as part of a
scheduled cleanup pass or if audit history on course configuration
becomes important for any reason.

### StudentRepository transcript / parent association — investigated

**Transcript creation** (`student_transcript_master`/`_detail`): deferred.
DB check on 2026-04-10 showed zero rows in either table. Feature is not
in use. Same pattern of delete-reinsert as the other deferred
repositories but no active users, no active damage.

**Parent association** (`parent_associationship`): **false positive** —
NOT APPLICABLE. The RemoveRange + AddRange in
`StudentRepository.UpdateStudentEnrollment` (around line 1876) is not the
anti-pattern we were cataloguing. It's a legitimate data movement
operation triggered only during a student **transfer to another school**.
Because the composite key on `parent_associationship` includes `SchoolId`,
moving a student to a new school requires deleting the old rows and
creating new rows pointed at the new school's `SchoolId` — there's no
"update in place" option when part of the primary key changes.

Additionally, the copy code at
[StudentRepository.cs:1656-1657](../../API/opensis.data/Repository/StudentRepository.cs#L1656-L1657)
already explicitly preserves the original `CreatedBy`/`CreatedOn` on
the new rows, so there is no audit destruction.

The day-to-day parent-student association flow lives in
`ParentInfoRepository.cs` and correctly uses single-row
`Add`/`Update`/`Remove` operations — no anti-pattern there.

### StudentHistoricalGradeRepository — investigated and deferred

`AddUpdateHistoricalGrade` uses delete-reinsert across `historical_grade`
and `historical_credit_transfer`. Fix is mechanically straightforward
(similar shape to `StudentEffortGradeRepository`), but a DB check on
2026-04-10 showed only 3 rows total — all entered within an 11-minute
window on 2023-11-27 as a one-off test, with `updated_on` always NULL
since then. No active users means no active data loss or audit
destruction.

Deferred to a standalone plan so it can be tackled as a focused fix
when a tenant starts using the feature, or as part of a scheduled
cleanup pass. See
[historical-grade-repository-upsert.md](historical-grade-repository-upsert.md)
for full analysis and proposed approach.

### ReportCardRepository — investigated and skipped (dead code)

Both `AddReportCard` (default and custom template branches) use the
delete-reinsert pattern on `student_report_card_master`/`_detail`. However
investigation showed these are dead code paths in the current frontend:

- The frontend's "Generate Report Card for Selected Students" button
  calls `getReportCardForStudents`, which is a **pure dynamic aggregation**
  that reads from `student_final_grade`, `student_attendance`, etc., and
  never touches `student_report_card_master`.
- The only frontend reference to `addReportCard` is **commented out** at
  `student-report-card.component.ts:140`.
- The other read method `GenerateReportCard` (which DOES read from the
  master tables) is also unused by the frontend.
- Tenant data in `student_report_card_master` is from 2021 only — leftover
  test/seed records from initial system setup, never updated since.

The whole snapshot-based report card flow was apparently designed but
abandoned in favour of dynamic generation. The repository methods,
controller endpoints, JSReport-based PDF flow, and database tables are
all vestigial.

**Recommendation for future work**: rather than fixing these methods,
delete them along with their controller endpoints and unused service
methods, and consider dropping the unused tables in a future migration.
This is a cleanup task, not a data-safety fix — there is no risk of data
loss because the destructive code path is never executed.

Skipping this repository in the current scope and moving on to live code
paths instead.

## Related Open Concerns

- **Roster does not filter dropped students by marking period.** A student
  dropped on Oct 15 still appears (grayed out) in the Q1 grade input page even
  though they were enrolled and earned grades that period. The roster query
  `GetStudentListByCourseSection` is course-section-scoped only. This is
  cosmetic now that the upsert preserves their grades, but ideally a dropped
  student should appear as active for marking periods they were enrolled in
  and only be hidden/disabled for periods after their exit date. Tracked
  separately.

- **DateTime serialization without `Z` suffix.** The API returns UTC times
  but Newtonsoft.Json serializes them without the `Z` marker, so JavaScript
  `new Date()` interprets them as local time. The audit display works around
  this by appending `Z` before parsing. A global serialization fix would
  benefit the whole app. Tracked in a separate issue.
