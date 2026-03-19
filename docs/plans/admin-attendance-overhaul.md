# Administration Attendance Page Overhaul

**Issue:** The administration attendance page (/school/attendance/administration) is confusing and partially broken.
**Goal:** Transform it from a broken editing tool into a clear oversight/monitoring tool.

---

## Problems Being Fixed

1. **Attendance code dropdown empty** — hardcoded `attendanceCategoryId = 1`; schools using other categories see nothing (FIXED already — sent null to load all categories)
2. **Only students with existing attendance appear** — query only hits `StudentAttendance` table, so students with no records are invisible
3. **No explanation of what the page shows** — even an expert can't tell what the student list represents
4. **Main-table dropdown misleading for multi-period** — binds to `studentAttendanceList[0]` only, showing one period's code
5. **"Update" button cascades dangerously** — overwrites ALL period records with one code (marks all periods Absent even if student attended 5/6)

## Design Decisions

- **Oversight tool, not creation tool** — admin can view and edit existing attendance but cannot create new records for students/periods with no attendance. "No attendance" means "go tell the teacher."
- **Show all enrolled students** — not just those with existing records
- **Show recorded period count** — e.g. "3 periods recorded" or "No attendance". No denominator (total scheduled periods) because computing it requires replicating the full 4-schedule-type logic across 5-7 tables. Can be added later if needed.
- **Remove main-table attendance dropdown** — per-period editing stays in the detail dialog (click student name), which already works correctly
- **Remove main-table "Update" button** — the blunt cascade overwrite is dangerous; edits go through the detail dialog's per-period submit
- **Add page description** — clear note at top explaining what the tool does and doesn't do

---

## Implementation Plan

### Phase 1: Backend — Return all enrolled students with attendance counts

**File:** `API/opensis.data/Repository/StudentAttendanceRepository.cs`
**Method:** `GetAllStudentAttendanceListForAdministration`

Rewrite the query to:
1. Start from all actively enrolled students (`StudentEnrollment` where `IsActive == true`) for the school
2. Left-join to `StudentAttendance` for the selected date (and optional attendance code filter)
3. For each student, count how many `StudentAttendance` records exist for that date = `periodsRecorded`
4. Still load `StudentDailyAttendance` for the "Present" status (Full-Day/Half-Day/Absent) — this is valid info when it exists
5. Still support existing filter/search/pagination logic

**File:** `API/opensis.data/ViewModels/StaffSchedule/` or attendance admin models
- Add `PeriodsRecorded` (int) field to `StudendAttendanceAdministrationViewModel`

### Phase 2: Frontend — Simplify the main table

**File:** `UI/src/app/pages/attendance/administration/administration.component.html`

1. Add descriptive note at the top of the page (inside the card, above the date picker):
   > "View attendance status for all enrolled students on a given date. Click a student name to view or edit their per-period attendance. Students with no attendance recorded have not yet had their attendance taken by a teacher."

2. Replace columns:
   - Keep: Student Name (clickable), Student ID, Grade, Section
   - Replace "Present" + "Attendance" columns with single **"Status"** column showing:
     - "X periods recorded" when X > 0 (with Present status in parentheses if available)
     - "No attendance" when X == 0
   - Keep: Comment column

3. Remove the `<select>` attendance code dropdown from the table
4. Remove the "Update" button at the bottom

**File:** `UI/src/app/pages/attendance/administration/administration.component.ts`

5. Remove `onAttendanceSelected()` method
6. Remove `submitDailyAttendance()` method
7. Remove `studentDailyAttendanceListViewModel` field
8. Update `displayedColumns` array

**File:** `UI/src/app/pages/attendance/administration/administration.component.scss`
- Minor styling for status column (color-code: green for has-attendance, red/grey for none)

### Phase 3: i18n

**File:** `UI/src/assets/i18n/en.json`
- Add translation keys for the page description and status labels

---

## Files Changed

| File | Change |
|---|---|
| `API/opensis.data/Repository/StudentAttendanceRepository.cs` | Rewrite admin query to start from enrolled students |
| `API/opensis.data/ViewModels/` (attendance admin model) | Add `PeriodsRecorded` field |
| `UI/src/app/pages/attendance/administration/administration.component.html` | Add page note, simplify table columns |
| `UI/src/app/pages/attendance/administration/administration.component.ts` | Remove dropdown/update logic, update columns |
| `UI/src/app/pages/attendance/administration/administration.component.scss` | Status styling |
| `UI/src/app/models/attendance-administrative.model.ts` | Add `periodsRecorded` field |
| `UI/src/assets/i18n/en.json` | New translation keys |

## Out of Scope

- Computing total scheduled periods (the denominator) — too complex for now
- Creating attendance records from admin view — admin is oversight, teacher creates
- Changing the detail dialog (student-attendance-comment) — it already works correctly per-period
