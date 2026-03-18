# Scheduling & Attendance Performance Overhaul

Audit date: 2026-03-18. See `performance-patterns.md` for the general checklist.

---

## CRITICAL RULE (repeated from performance-patterns.md)

Before committing any query optimization, run both the old and new query against a real
tenant DB and compare output (row counts, shape, values). Never assume a rewrite is
equivalent — verify it.

---

## Scope

Four repository files account for nearly all the damage, plus one related file:

| File | Lines | Area |
|---|---|---|
| `API/opensis.data/Repository/StudentScheduleRepository.cs` | ~2,400 | Student scheduling |
| `API/opensis.data/Repository/StaffScheduleRepository.cs` | ~400 | Staff/teacher scheduling |
| `API/opensis.data/Repository/StudentAttendanceRepository.cs` | ~3,260 | Attendance taking & admin |
| `API/opensis.data/Repository/AttendanceCodeRepository.cs` | ~475 | Attendance code lookups |
| `API/opensis.data/Repository/CourseManagerRepository.cs` | ~500 | Course/program management (B7 only) |

---

## Anti-Pattern Categories

### A. N+1 Queries in Loops

DB queries executed inside `foreach` loops — often nested 2-3 deep.

| # | File | Method | Lines | What happens | Est. queries |
|---|---|---|---|---|---|
| A1 | StudentScheduleRepo | AddStudentCourseSectionSchedule_old | 90-400 | Triple-nested: sections x students x schedule views | 5,000+ |
| A2 | StudentScheduleRepo | GetStudentListByCourseSection | 1292-1321 | FirstOrDefault for GradeLevel, Section, SchoolName inside .Select() | 3 per student |
| A3 | StudentScheduleRepo | ScheduleCoursesForStudent360 | 2108-2114 | BellSchedule query per block per student | students x blocks |
| A4 | StudentScheduleRepo | ScheduleCourseSectionListForStudent360 | 2391-2398 | Identical bell schedule loop | students x blocks |
| A5 | StaffScheduleRepo | StaffScheduleViewForCourseSection | 61-185 | Triple-nested: teachers x courses x schedule types | 150-200 |
| A6 | StaffScheduleRepo | CheckAvailabilityStaffCourseSectionSchedule | 397-408 | AllCourseSectionView.Join per course section in loop | 50+ |
| A7 | StudentAttendanceRepo | AddUpdateStudentAttendance | 268-311 | Per-student: StudentAttendance, then per-record: BlockPeriod + AttendanceCode | 1,100+ for 100 students |
| A8 | StudentAttendanceRepo | AddUpdateStudentAttendanceForStudent360 | 725-767 | Same pattern as A7 | 1,100+ |
| A9 | AttendanceCodeRepo | GetAllAttendanceCode | 192-196 | Utility.CreatedOrUpdatedBy inside ForEach (2 DB calls each) | 2 per code |

### B. AsEnumerable / ToList Before Filtering

Loads entire tables into memory, then filters in C#.

| # | File | Method | Lines | What loads |
|---|---|---|---|---|
| B1 | StudentScheduleRepo | AddStudentCourseSectionSchedule_old | 361-382 | AllCourseSectionView.Join().AsEnumerable() + Regex per student |
| B2 | StudentScheduleRepo | AddStudentCourseSectionSchedule | 906-919 | Same as B1, newer method |
| B3 | StudentScheduleRepo | AddStudentCourseSectionSchedule | 622-632 | 4 school-wide tables: all schedules, views, enrollments, schedule views |
| B4 | StudentAttendanceRepo | AddUpdateStudentAttendanceForStudent360 | 545 | Membership.AsEnumerable().FirstOrDefault() |
| B5 | StudentAttendanceRepo | CourseSectionListForAttendanceAdministration | 2024 | CourseSection + 4 Includes, entire year, no filter beyond tenant/school/year |
| B6 | AttendanceCodeRepo | DeleteAttendanceCode | 232 | StudentDailyAttendance.AsEnumerable() — entire table |
| B7 | CourseManagerRepo | AddEditProgram | 133, 142 | Programs.AsEnumerable(), Course.AsEnumerable() for case-insensitive compare |

### C. Massive Include Chains / Cartesian Products

Deep Include chains that multiply row counts.

| # | File | Method | Lines | Include depth | Risk |
|---|---|---|---|---|---|
| C1 | StudentScheduleRepo | ScheduleCoursesForStudent360 | 2002-2008 | Section->Years->Semesters->Quarters->StaffSchedule + StudentAttendance | Cartesian: 5 courses x 2 staff x 100 attendance = 1,000 rows |
| C2 | StudentScheduleRepo | ScheduleCourseSectionListForStudent360 | 2294 | Attendance->Comments + CourseSection->Calendars | Comments multiply rows |
| C3 | StudentAttendanceRepo | GetAllStudentAttendanceListForAdministration | 1872 | Attendance->Comments->Membership + BlockPeriod + AttendanceCode + Schedule->Student->Enrollment+Sections | Deep chain |
| C4 | StudentAttendanceRepo | CourseSectionListForAttendanceAdministration | 2024-2028 | CourseSection + StudentSchedule + Course + Calendars + StaffSchedule, then client-side filter | All sections for year |

### D. Missing .AsNoTracking()

**Every single read-only query** across all four files lacks `.AsNoTracking()`. Zero exceptions.
EF Core tracks every returned entity for change detection — roughly doubles memory and CPU.

Estimated 50+ read-only queries need `.AsNoTracking()` added.

### E. Client-Side Regex in Hot Paths

Schedule conflict checking uses `Regex.IsMatch()` and `.ToLower().Contains()` on loaded data.

| # | File | Method | Lines | Context |
|---|---|---|---|---|
| E1 | StudentScheduleRepo | AddStudentCourseSectionSchedule_old | 361-382 | Inside student loop — regex per student per section |
| E2 | StudentScheduleRepo | AddStudentCourseSectionSchedule | 906-919 | Same, newer method |
| E3 | StaffScheduleRepo | CheckAvailabilityStaffCourseSectionSchedule | 316-352 | Per course section being checked |
| E4 | StaffScheduleRepo | CheckAvailabilityStaffCourseSectionSchedule | 397-408 | AsEnumerable join + regex per section |

### F. Linear Scans in Loops

`List.FirstOrDefault()` called repeatedly where a dictionary lookup would be O(1).

| # | File | Method | Lines |
|---|---|---|---|
| F1 | StudentAttendanceRepo | MissingAttendanceList | 1750, 1769, 1789 | BlockPeriodList.FirstOrDefault per record |
| F2 | StudentAttendanceRepo | MissingAttendanceList_old | 1430 | Same pattern |

### G. Unbounded School-Wide Loads

Queries that load all rows for a school with no additional filtering.

| # | File | Method | Lines | Table |
|---|---|---|---|---|
| G1 | StudentScheduleRepo | GroupDropForScheduledStudent | 1628 | ALL StudentMissingAttendances for school |
| G2 | StudentScheduleRepo | GroupDropForScheduledStudent | 1633-1635 | ALL StudentAttendance + comments + history for student |
| G3 | StudentScheduleRepo | ScheduleCourseSectionListForStudent360 | 2296 | ALL Blocks for school |
| G4 | StudentScheduleRepo | ScheduleCourseSectionListForStudent360 | 2300 | ALL AttendanceCodeCategories + codes for school |

---

## Fix Order

Work in phases. Each phase is a commit (or small set of commits). Test after each phase.

### Phase 1 — Quick wins: .AsNoTracking() on all read-only queries (all 4 files)

- [x] StudentScheduleRepository.cs — add .AsNoTracking() to every read-only query
- [x] StaffScheduleRepository.cs — add .AsNoTracking() to every read-only query
- [x] StudentAttendanceRepository.cs — add .AsNoTracking() to every read-only query
- [x] AttendanceCodeRepository.cs — add .AsNoTracking() to every read-only query

Risk: near-zero. Read-only queries are not modified and saved back.
Verify: run the same UI flows before/after, confirm identical results.

### Phase 2 — Remove AsEnumerable / fix case-insensitive comparisons (B4, B6, B7)

Replace `.AsEnumerable()` + `String.Compare(..., true)` with database-side
`EF.Functions.Like()` or `.ToLower()` that Pomelo can translate.

- [x] B4 — Membership lookup in AddUpdateStudentAttendanceForStudent360
- [x] B6 — StudentDailyAttendance in DeleteAttendanceCode
- [x] B7 — Programs and Course in AddEditProgram

Risk: low. Same logic, just executed in MySQL instead of C#.
Verify: confirm same rows returned.

### Phase 3 — Attendance N+1 fixes (A7, A8, A9)

Rewrite the per-student attendance save loops to batch-load BlockPeriod and
AttendanceCode data before the loop, then look up from dictionaries.

- [x] A7 — AddUpdateStudentAttendance: batch-load, dictionary lookup
- [x] A8 — AddUpdateStudentAttendanceForStudent360: same pattern
- [x] A9 — GetAllAttendanceCode: replace Utility.CreatedOrUpdatedBy loop

Risk: low-medium. Same data, different fetch order.
Verify: save attendance for a class, compare DB state before/after.

### Phase 4 — Attendance admin queries (B5, C3, C4, F1, F2)

- [x] B5/C4 — CourseSectionListForAttendanceAdministration: pushed staff/student filters to DB, batch-loaded BellSchedule
- [x] C3 — GetAllStudentAttendanceListForAdministration: batch-loaded StudentDailyAttendance + Block (was N+1)
- [x] F1/F2 — MissingAttendanceList: replaced BlockPeriodList.FirstOrDefault with dictionary lookups

Risk: medium. These shape the data returned to the UI — verify field-by-field.
Verify: load attendance admin screen, compare course section list and attendance list.

### Phase 5 — Student scheduling N+1 fixes (A1, A2, B1, B2, B3)

The biggest and most complex changes.

- [x] A1 — AddStudentCourseSectionSchedule_old: batch-load before loops, eliminate triple-nested queries
- [x] B1/B2 — Conflict checking: batch-load AllCourseSectionView once, filter in-memory from that single load (the regex stays client-side for now, but fed from one query not N)
- [x] B3 — AddStudentCourseSectionSchedule: added AsNoTracking to read-only loads, narrowed StudentEnrollment to request students, removed redundant .AsEnumerable()
- [x] A2 — GetStudentListByCourseSection: replace FirstOrDefault in Select with pre-loaded dictionaries

Risk: medium-high. Scheduling is complex business logic.
Verify: schedule students into sections, verify no conflicts missed, verify student list matches.

### Phase 6 — Student scheduling Include chains + bell schedule loops (C1, C2, A3, A4)

- [x] C1 — ScheduleCoursesForStudent360: flattened deep Include chain into direct Includes + AsSplitQuery (eliminates Cartesian product)
- [x] C2 — ScheduleCourseSectionListForStudent360: added AsSplitQuery to avoid Cartesian from collection Includes
- [x] A3/A4 — Bell schedule loops: replaced per-block loop with single blockIds.Contains() query in both methods

Risk: medium. Student 360 view is data-heavy.
Verify: open Student 360 scheduling tab, compare all displayed data.

### Phase 7 — Staff scheduling (A5, A6, E3, E4)

- [x] A5 — StaffScheduleViewForCourseSection: batch-loaded AllCourseSectionView, SchoolCalendars, StaffCoursesectionSchedule, and 4 schedule-type tables before loops
- [x] A6/E3/E4 — CheckAvailabilityStaffCourseSectionSchedule: batch-loaded AllCourseSectionView + StaffCoursesectionSchedule, replaced DB Join with in-memory join, removed .AsEnumerable()

Risk: medium. Staff conflict checking is critical.
Verify: schedule a teacher, verify conflicts detected correctly.

### Phase 8 — Unbounded loads + drop operation (G1-G4)

- [x] G1/G2 — GroupDropForScheduledStudent: narrowed StudentMissingAttendances, StudentAttendance, and StudentAttendanceHistory to specific courseSectionIds being dropped
- [x] G3/G4 — ScheduleCourseSectionListForStudent360: converted Block to dictionary lookup, narrowed AttendanceCodeCategories to only categories referenced by student's course sections

Risk: low-medium.
Verify: drop a student from a section, verify attendance records cleaned up correctly.

### Phase 9 (future) — Conflict regex rethink (E1, E2)

The day-matching regex (`"MoTuWe"` pattern matching) is inherently client-side logic.
Long-term options:
- Normalize day data into a junction table (e.g., `CourseSectionDay`) with indexed columns
- Pre-compute conflict flags on schedule save

This is a larger architectural change — defer until phases 1-8 are stable.

---

## Progress Log

| Date | Phase | Items | Status |
|------|-------|-------|--------|
| 2026-03-18 | 1 | D (all 4 files) | Done — 98 .AsNoTracking() additions |
| 2026-03-18 | 2 | B4, B6, B7 | Done — removed .AsEnumerable(), use .ToLower() for DB-side compare |
| 2026-03-18 | 3 | A7, A8, A9 | Done — batch-load + dictionary lookups, replaced Utility.CreatedOrUpdatedBy loop |
| 2026-03-18 | 4 | B5, C3, C4, F1, F2 | Done — DB-side filters, batch-loads, dictionary lookups for attendance admin |
| 2026-03-18 | 5 | A1, A2, B1, B2, B3 | Done — batch-load before loops in _old method, AsNoTracking+narrowed loads in new method, dictionary lookups for student list |
| 2026-03-18 | 6 | C1, C2, A3, A4 | Done — flattened Include chains + AsSplitQuery, batch-loaded BellSchedule with blockIds.Contains() |
| 2026-03-18 | 7 | A5, A6, E3, E4 | Done — batch-loaded 7 tables in A5, batch-loaded + in-memory join in A6 |
| 2026-03-18 | 8 | G1, G2, G3, G4 | Done — narrowed drop queries by courseSectionIds, dictionary Block lookup, filtered AttendanceCodeCategories |
