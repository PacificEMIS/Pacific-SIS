# Delete-Reinsert Pattern — Data Loss & Audit Destruction

Related issue: https://github.com/PacificEMIS/Pacific-SIS/issues/561

## Status

**Critical** — actively causing silent data loss, not just an audit concern.

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
| ReportCardRepository | 2 methods | student_report_card_master, _detail | TODO |
| StaffPortalGradebookRepository | 3 methods | gradebook_grades, gradebook_configuration_* | TODO |
| StudentHistoricalGradeRepository | AddUpdateHistoricalGrade | historical_grade, historical_credit_transfer | TODO |

### Student record data

| Repository | Method | Tables | Status |
|---|---|---|---|
| StudentRepository | Transcript creation | student_transcript_master, _detail | TODO |
| StudentRepository | Parent association | parent_associationship | TODO |

### Configuration/scheduling (lower priority)

| Repository | Method | Tables | Status |
|---|---|---|---|
| CourseManagerRepository | 4 methods | course_standard, course_variable_schedule, course_calendar_schedule, course_block_schedule | TODO |

---

## Proposed Fix: Upsert by Business Key

Replace the delete-reinsert pattern with a proper upsert that matches incoming records
to existing ones by their natural business key.

### Example: InputFinalGradeRepository.AddUpdateStudentFinalGrade

**Business key:** `StudentId` (within the already-scoped course section + marking period)

```
// Pseudocode
var existingByStudentId = existingRecords.ToDictionary(r => r.StudentId);

foreach (incoming in incomingList)
{
    if (existingByStudentId.TryGetValue(incoming.StudentId, out var existing))
    {
        // UPDATE — preserve CreatedOn/CreatedBy, set UpdatedOn/UpdatedBy
        existing.PercentMarks = incoming.PercentMarks;
        existing.GradeObtained = incoming.GradeObtained;
        existing.TeacherComment = incoming.TeacherComment;
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

// DO NOT delete records for students not in the incoming list.
// Dropped students' grades must be preserved.
```

**Key principle:** only touch what you're given. Students absent from the submission
(dropped students, transferred students) have their existing grades left untouched.

### Execution order

1. **InputFinalGradeRepository** — start here (best understood, highest impact,
   already demonstrated data loss)
2. **StudentEffortGradeRepository** — same pattern, same risk for dropped students
3. **ReportCardRepository** — report cards for dropped students also at risk
4. **StaffPortalGradebookRepository** — gradebook assignment grades
5. **Remaining repositories** — lower priority, schedule opportunistically

---

## What Was Already Fixed (April 2026)

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
