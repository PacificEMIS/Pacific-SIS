# StaffPortalGradebookRepository — Upsert Refactor & Audit Preservation

## Status

**Deferred** — lower priority. Gradebook is not currently in use by any
Pacific EMIS tenant, so the audit destruction caused by the delete-reinsert
pattern has no active victims. Fixing this properly requires careful work
across four methods with complex composite keys, and the thinking and
testing time needed is not justified while the feature is dormant.

When gradebook becomes actively used, this plan should be revisited.

Related: [delete-reinsert-data-loss.md](delete-reinsert-data-loss.md) —
the broader standing initiative to eliminate delete-reinsert patterns
across the backend repositories.

---

## Why This Is in Its Own Plan

Unlike the first two repositories tackled in the parent plan
(`InputFinalGradeRepository`, `StudentEffortGradeRepository`) which each
had a single save method with a simple business key, this repository has
**four** methods using the pattern, with complex composite keys and
different scopes. It deserves standalone planning rather than being
rushed as part of a broader sweep.

---

## Scope

Four methods in
`API/opensis.data/Repository/StaffPortalGradebookRepository.cs`:

1. `AddUpdateGradebookConfiguration` (lines 54-205) — touches
   `GradebookConfiguration` parent plus 5 child tables:
   - `GradebookConfigurationGradescale`
   - `GradebookConfigurationYear`
   - `GradebookConfigurationSemester`
   - `GradebookConfigurationQuarter`
   - `GradebookConfigurationProgressPeriods`

2. `AddGradebookGrade` (lines 411-637) — saves per-assignment grades for
   **multiple students** at once (the primary teacher view: "here's
   assignment X, enter grades for every student").

3. `AddGradebookGradeByStudent` (lines 966-1196) — saves all assignments
   for **one student** at a time (inverse view).

4. `AddgradebookGradeByAssignmentType` (lines 1355-1494) — saves grades
   scoped to one assignment type for multiple students.

Methods 2, 3, and 4 all write to the same underlying `gradebook_grades`
table — they're three different UI paths into the same data.

---

## The `gradebook_grades` Composite Key

```
(TenantId, SchoolId, StudentId, AcademicYear, CourseSectionId,
 AssignmentTypeId, AssignmentId)
```

This is significantly more complex than the two-level key used by
`StudentFinalGrade` or `StudentEffortGrade`. A single row represents
"this student's grade on this specific assignment in this course
section in this academic year."

---

## Current Behaviour Analysis

### Data loss risk: LOW (but not zero)

Unlike `InputFinalGradeRepository`, the existing save methods all filter
by `StudentId` before deleting. So other students' grades are safe on
every save.

They also filter by `CourseSectionId` and `AcademicYear`, and where
applicable by `MarkingPeriod`. So other course sections and other
marking periods are safe.

**Residual risk**: if the frontend ever submits a **partial** list of
assignments (e.g., "show only ungraded assignments" mode), the
delete-all-then-reinsert approach would wipe the unsubmitted ones. The
current UI appears to always submit the full assignment list, so this
is a latent risk rather than an active one.

### Audit destruction risk: HIGH

Every re-save of any assignment grade wipes `CreatedBy`/`CreatedOn` for
**every assignment grade that was touched**. For a feature where teachers
enter grades incrementally across a marking period, this means the
"when was this grade first entered" history is destroyed on every edit.

### Configuration children

`AddUpdateGradebookConfiguration` parent is updated in place correctly
(via `Entry.SetValues(...)`). Only the child tables use delete-reinsert.
Since children are structural (a config has N gradescale entries with no
external owners), the data loss risk is zero for this method — it's
purely an audit concern.

---

## Proposed Approach

### Recommendation: Option C from earlier discussion

**Fix the three grade-save methods (#2, #3, #4) together** using the
proven pattern from `StudentEffortGradeRepository`:
- Load existing rows with `AsNoTracking()`
- Build a dictionary keyed on the business key (in this case
  `(StudentId, AssignmentTypeId, AssignmentId)` within the already-scoped
  course section + academic year + marking period)
- For matching incoming rows: build a fresh entity copying preserved
  `CreatedBy`/`CreatedOn` from the no-track snapshot, set new values,
  attach as `Modified` via `Update()`
- For non-matching incoming rows: `Add()` as a new insert
- **Do not** delete rows not in the submission (preserves audit and
  protects against the partial-list risk)

**Defer `AddUpdateGradebookConfiguration` to a separate follow-up.**
It's a different shape (parent + 5 children), doesn't share the same
upsert logic, and has the added fragility of manual PK assignment via
`Utility.GetMaxPK<>()`. It deserves its own cleanup pass.

### Alternative: extract a shared helper

The three grade-save methods are structurally near-identical. A private
helper method that takes a pre-built `List<GradebookGrades>` and performs
the upsert would avoid copy-paste. Tempting but potentially invasive —
the three methods differ in how they assemble the list and compute the
running averages. Do this as refactor after the tactical fix is proven.

---

## Business Key for the Grade Upsert

Within the already-filtered scope (course section + academic year +
marking period + student), the key that uniquely identifies a
`gradebook_grades` row is:

```
(AssignmentTypeId, AssignmentId)
```

Dictionary lookup: `existingByKey.TryGetValue((at, aid), out var existing)`

For `AddGradebookGrade` (multi-student): iterate students, then for
each student build their dictionary from the pre-loaded snapshot.

For `AddGradebookGradeByStudent` (single student): one dictionary for
the whole student's scope.

For `AddgradebookGradeByAssignmentType`: scoped further by
`AssignmentTypeId`, so the dictionary can just be keyed on `AssignmentId`
within that type.

---

## What's in the Entity but not the Business Key

Important fields preserved across updates:
- `CreatedBy`, `CreatedOn` — the point of this whole exercise
- Computed fields that are recalculated on every save anyway:
  - `Percentage`
  - `LetterGrade`
  - `RunningAvg`
  - `RunningAvgGrade`

The running-average recalculation is extensive (lines 471-620 in
`AddGradebookGrade`) and needs to stay intact through the refactor.
The new entity's values for these come from the in-method computation,
not from the no-track snapshot.

---

## Audit Display (Frontend)

Consistent with the fix pattern from issues #819/#821: add an info icon
to the gradebook entry views that shows a tooltip with created/updated
timestamps and user names.

The three gradebook views are:
- `pages/staff/teacher-function/gradebook-grades/gradebook-grade-list`
  and the sibling `gradebook-grade-details` component
- `pages/class/grades/gradebook-grades` (admin on behalf of teacher)

Each view has its own template. Same `getAuditTooltip(i)` helper pattern.

The backend GET methods (`GetGradebookGrade`, `GradebookGradeByStudent`,
`GradebookGradeByAssignmentType`) will need to resolve the staff GUIDs to
readable names, same as the fix in
`InputFinalGradeRepository.GetAllStudentFinalGradeList` and
`StudentEffortGradeRepository.GetStudentListByHomeRoomStaff`. A
`[NotMapped]` `CreatedByName`/`UpdatedByName` pair on
`GradebookGrades.cs`, populated from a `StaffMaster` lookup in the GET
projection.

---

## Testing Plan

Once gradebook is being actively used, the same test pattern used for
effort grades applies. At minimum:

1. **Enter some assignment grades**, verify `created_on`/`created_by` in
   the DB.
2. **Re-save the same grades** (no actual changes). Verify `created_on`
   is preserved and `updated_on` is set.
3. **Change one grade value**, re-save. Verify only the touched row's
   fields change; other rows' `updated_on` may still fire (same as
   effort grades — whole-list submit — unless we add dirty-row tracking
   in the frontend, out of scope for this plan).
4. **Partial-list test**: manually craft a request that submits a
   subset of assignments, verify that the non-submitted rows are still
   present in the DB. This is the defensive test for the residual
   data loss risk.

---

## Follow-up: `AddUpdateGradebookConfiguration`

When tackling this method:

- Parent `GradebookConfiguration` update via `Entry.SetValues(...)` is
  already correct and should stay.
- Child tables: switch from delete-reinsert to upsert keyed on the
  natural key of each child (e.g., `GradescaleId` for gradescale rows,
  `MarkingPeriodId` for year/semester/quarter/progress-period rows).
- Fix the manual `Utility.GetMaxPK<>()` PK assignment if possible —
  investigate whether the schema uses auto-increment identity columns,
  which would eliminate the need for manual ID generation. If not,
  wrap the read+max+insert sequence in a lock to avoid race conditions
  (the current code has a latent concurrency bug if two teachers save
  simultaneously).

This follow-up is lower priority than the grade-save methods because
the configuration table is written rarely compared to the grade entry
tables.

---

## Decision Log

- **2026-04-10**: Investigated and scoped. Confirmed all four methods
  are wired to live frontend components, but Pacific EMIS has no tenant
  actively using gradebook, so fixing is deferred. A standalone GitHub
  issue should be created with low priority. The broader
  delete-reinsert plan will reference this plan but mark gradebook as
  deferred.
