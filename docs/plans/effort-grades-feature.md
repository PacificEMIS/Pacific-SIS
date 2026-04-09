# Student Effort Grades — Feature Notes & How-To

## Status

Functional but partially incomplete. Roughly 70% delivered by the original
contractor. Data entry works end-to-end and persists correctly (and the audit
preservation fix in branch `issue821` makes it safer to use). However the
feature is gated behind specific staff profile setup, and the entered data is
not currently consumed by any reporting (report cards, transcripts, etc.).

---

## Concept

Effort grades are intended to record a **per-student, per-marking-period
overall assessment** of attitude, behaviour, participation, etc. — separate
from academic grades.

Key design assumption: **one teacher per student per marking period** enters
the effort grade. That teacher is the student's homeroom/primary class teacher
or advisor. The model does NOT support per-subject effort grades — there's no
course section linkage on the stored record.

This works naturally for primary schools (one teacher teaches all subjects to
one class). For secondary schools it requires designating a single advisor per
student.

---

## Data Model

**`student_effort_grade_master`** — one row per student per marking period
- `student_id`, `academic_year`, `marking_period_id` (one of the four)
- `teacher_comment`
- `created_by`/`created_on`, `updated_by`/`updated_on`
- `course_id` and `course_section_id` are columns on the table but are
  **always set to 0** by the save logic — the record is not linked to any
  specific course section.

**`student_effort_grade_detail`** — one row per effort item per record
- Foreign key back to master via `student_effort_grade_srlno`
- Holds the selected `effort_grade_scale_id` for each effort item

**`effort_grade_library_category`** + **`effort_grade_library_category_item`**
- The categories and items being assessed (e.g. "Behaviour" → "Follows
  directions", "Respects others")

**`effort_grade_scale`**
- The scale values used for each item (e.g. 1=Outstanding, 2=Satisfactory,
  3=Needs Improvement)

---

## Configuration Required

Before any teacher can enter effort grades, an admin must configure all of
the following at the school level:

1. **Effort Grade Scale** — Settings → Grade Settings → Effort Grade Setup
   → Effort Grade Scale tab. Add at least one scale value.
2. **Effort Grade Library** — Settings → Grade Settings → Effort Grade Setup
   → Effort Grade Library tab. Add at least one category, with at least one
   item under it.
3. **Designate homeroom teachers** — for each teacher who should enter effort
   grades, edit their staff record and set their **Profile** in the school
   info section to literally `Homeroom Teacher` (case-insensitive but exact
   string match — `Class Teacher`, `Subject Teacher`, etc. will not work).

---

## Who Sees What

### The staff selection screen
The "Input Effort Grade" page lists only staff whose
`staff_school_info.profile = 'Homeroom Teacher'`. This filter is hard-coded
in the frontend at
[common-staff-list.component.ts:200](../../UI/src/app/common/common-staff-list/common-staff-list.component.ts#L200)
and enforced in the backend at
[StaffRepository.cs:253,276](../../API/opensis.data/Repository/StaffRepository.cs#L253).

A non-homeroom teacher who otherwise teaches course sections will NOT appear
in this list and cannot have effort grades entered on their behalf.

### After clicking a teacher
The student list is built by:
1. Find all course sections the teacher is assigned to in
   `staff_coursesection_schedule` whose `duration_start_date`/
   `duration_end_date` **completely contain** the marking period being
   graded (strict containment, not overlap).
2. Find all students enrolled in those course sections via
   `student_coursesection_schedule`.
3. **Deduplicate by `student_id`** — collapse all "this student is in
   Math, Reading, Writing..." into a single entry per student.

The result is one flat list of all distinct students the teacher teaches
across all their course sections. There is no per-course-section breakdown
because the model is per-student, not per-course.

---

## Current Limitations / Incompleteness

1. **No reporting consumes effort grades.** Data is collected but never
   appears on report cards, transcripts, progress reports, or any other
   output. Adding this would be custom development.

2. **Admin entry path is a stub.** The route `/school/grades/input-effort-grades`
   exists in navigation but the component is empty. Admins must use
   Staff → Teacher Functions → Input Effort Grade instead.

3. **Strict date containment for course sections.** A teacher's course
   section must FULLY enclose the marking period dates, not just overlap.
   Teachers added partway through a marking period or whose section ended
   before the marking period ended will not see students.

4. **Confusing terminology.** "Homeroom Teacher" is a literal profile string,
   not a role or relationship. Schools where the concept doesn't apply
   need to either set it artificially or remove the gating.

5. **No per-subject effort grades.** Some schools may want to assess effort
   per subject (e.g. "John works hard in Math but not in PE"). Not supported
   — one score per student per marking period.

6. **Audit display didn't exist** until the issue #821 fix. Now an info
   icon next to each student in the list shows when their effort grade was
   created/updated and by whom.

---

## Recommended Use Cases

### Use case 1 — Primary school with one teacher per class (best fit)
Schools like ESDM Elementary where one teacher (e.g. "the Grade 3 teacher")
teaches all subjects to all Grade 3 students.

**Setup:**
- Set each grade-level lead teacher's profile to `Homeroom Teacher`.
- One homeroom teacher per grade.

**Daily use:**
- Each marking period, the Grade 3 teacher logs in (or admin acts on her
  behalf), navigates to Input Effort Grade, picks herself, and rates each
  Grade 3 student on every effort item.

**Result:** Each student gets one effort grade record per marking period
covering their general attitude/behaviour.

### Use case 2 — Secondary school with advisors
Schools where each student has a designated "advisor" or "form teacher" in
addition to subject teachers.

**Setup:**
- Set each advisor's profile to `Homeroom Teacher`.
- Each advisor must be scheduled to teach at least one course section in
  the marking period (even an "Advisory" period works) so their advisees
  can be looked up.

**Daily use:**
- Advisors enter effort grades each marking period reflecting overall
  observation of their advisees, possibly after consulting subject teachers.

### Use case 3 — Schools that don't fit
If neither model fits — for example, a school where every teacher should
be able to assess effort for the students they personally teach in their
specific subject — the feature as currently built will not work without
code changes. Best to leave it disabled.

---

## Proposed Improvements (future work, not in scope of #821)

### Quick wins
1. **Better empty-state messaging.** Instead of just "No students found" or
   "Staff is not scheduled in any course sections," explain *why*: missing
   homeroom profile, no course section enclosing the marking period,
   missing effort grade library/scale, etc.

2. **Sort the student list alphabetically by last name** in the GET projection
   (currently relies on database insertion order).

3. **Document the homeroom profile requirement** in the UI itself — a
   help tooltip on the Input Effort Grade page explaining what makes a
   teacher eligible.

### Medium effort
4. **Wire effort grades into report cards.** The data is there; the report
   card generation needs to query and render it. This is the biggest
   functional gap.

5. **Loosen the date containment check** to overlap (rather than strict
   containment) so teachers added or dropped mid-marking-period can still
   enter grades.

6. **Remove the admin entry stub** or implement it properly. The dead
   menu item is confusing.

### Larger
7. **Per-course-section effort grades** as an option, for schools that
   want subject-specific assessment. Requires schema change and significant
   UI rework.

8. **Decouple from the "Homeroom Teacher" string profile.** Replace with
   either an explicit boolean flag or a different mechanism (e.g. a
   "Homeroom Class" assignment). The current string-comparison approach
   is brittle.

---

## How to Test (with a properly configured teacher)

Setup precondition: at least one teacher exists with
`staff_school_info.profile = 'Homeroom Teacher'`, the school has effort
grade scale and library configured, and the marking period being tested
falls within at least one of that teacher's course section duration ranges.

### Test 1 — Audit display (admin path acting on behalf of teacher)
1. Log in as an admin
2. Navigate: **Staff → Teacher Functions → Input Effort Grade**
3. Click the homeroom teacher
4. Pick a student in the left list
5. Enter or modify some effort grade values, click Submit
6. Reload the page and click the same student again
7. Hover the info icon next to the student name in the list
8. **Expected:** Tooltip shows "Created: ... by [admin name]" and/or
   "Updated: ... by [admin name]" with date/time in your local timezone

### Test 2 — Audit preservation across re-saves (the core fix)
1. Pick a student that already has an info icon (existing data) — note the
   **Created** timestamp in the tooltip
2. Open the student, change one effort value, click Submit
3. Reload, hover the icon again
4. **Expected:** The **Created** timestamp is unchanged. The **Updated**
   timestamp is now the current time. Previously this would have wiped
   `created_on` because the old code used delete-reinsert.

### Test 3 — New student first save (insert path)
1. Find a student who has no info icon yet (no existing record)
2. Enter effort values for them, Submit
3. Reload, click the student
4. **Expected:** Info icon now appears, tooltip shows
   `Created: <today> by <you>`

### Test 4 — Other students untouched (upsert isolation)
**Note:** This test has a caveat. The frontend submits the **entire roster**
on every Submit click, not just the student you edited. So both A and B
will get the same `Updated` timestamp after step 3 — that's expected
behaviour, not a regression. The upsert isolation guarantee is about
**dropped** students (Test 5), not students still on the roster.

If you want true per-student timestamps, that would require changing the
frontend to track dirty rows and only submit modified ones — out of scope
for this fix.

### Test 5 — Dropped student data preservation (the data loss fix)
This test specifically verifies that re-saving effort grades for the
remaining students does NOT destroy the records of a student who has been
dropped from the roster. Because there is no UI today that displays effort
grades for dropped students, verification has to happen in the database.

1. Pick a homeroom teacher with several students
2. Enter and save effort grades for the entire group, including a specific
   student you'll drop next (note their `student_id` from the page URL or
   page source — or just remember their name and look it up)
3. Verify the record exists:
   ```sql
   SELECT student_id, student_effort_grade_srlno,
          academic_year, qtr_marking_period_id,
          teacher_comment, created_on, created_by, updated_on, updated_by
   FROM student_effort_grade_master
   WHERE school_id = <YOUR_SCHOOL_ID>
     AND student_id = <DROPPED_STUDENT_ID>
   ORDER BY updated_on DESC;
   ```
   You should see one row. Note the `student_effort_grade_srlno`, `created_on`
   and `updated_on`.
4. Also verify the detail rows exist:
   ```sql
   SELECT * FROM student_effort_grade_detail
   WHERE school_id = <YOUR_SCHOOL_ID>
     AND student_effort_grade_srlno = <SRLNO_FROM_STEP_3>;
   ```
5. Now drop the student from one of the teacher's course sections (or fully
   from the school) using the normal drop UI. The student should disappear
   from the effort grade entry list.
6. Re-open Input Effort Grade for the same teacher, change one effort value
   for ANY remaining student, click Submit.
7. Re-run the SQL queries from steps 3 and 4. The dropped student's
   `student_effort_grade_master` row and all their `student_effort_grade_detail`
   rows should still exist, with the same `created_on`, `created_by`,
   `updated_on`, and `updated_by` values from before the drop.
8. **Expected:** Dropped student's data is fully intact. Before the
   issue #821 fix, the entire record would have been wiped because the old
   delete-reinsert pattern blew away everything for the marking period.

If the dropped student's row is gone after step 7, the upsert isn't working
correctly and we have a regression.

### Test 6 — Database verification (optional, general sanity check)
```sql
SELECT student_id, student_effort_grade_srlno,
       created_on, created_by,
       updated_on, updated_by
FROM student_effort_grade_master
WHERE school_id = <YOUR_SCHOOL_ID>
ORDER BY updated_on DESC
LIMIT 20;
```
Records re-saved with the new code should now have **both** `created_on`
AND `updated_on` populated. Records last touched with the old code path
will still have `created_on = NULL`.

### Red flags
- Icon doesn't appear at all → cache issue, hard refresh
- Icon shows GUID instead of name → API didn't restart with new code
- Submit fails or grades disappear → transaction rollback worked, but
  please investigate
- Updated timestamp shows in wrong timezone → date parsing issue,
  separate concern
