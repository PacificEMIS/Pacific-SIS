# GPA / Class Rank List Report — Improvement Plan

**Report location:** Reports > Grades > GPA/Class Rank List
**Issue:** #802

## Current State

The report calculates cumulative GPA and ranks students school-wide for the current
academic year. It displays: student name, ID, alternate ID, grade level, section,
cumulative GPA, total credit attempted, total credit earned, and class rank.

### Problems Identified

1. **Missing Advanced Search** — every other report has the `vex-common-search-student`
   panel; this one only has a text search + grade level dropdown
2. **N+1 query** — `HistoricalCreditTransfer` is fetched per-student inside the loop
3. **No tied ranks** — identical GPAs get sequential ranks (5, 6) instead of tied (5, 5)
4. **Marking period hierarchy check** is O(n) per grade record (repeated `.Any()` calls)
5. **Pagination is post-computation** — all GPAs computed even to show page 1 of 10
6. **Excel export commented out** — button exists in HTML but disabled
7. **No column sorting** — backend supports it but UI doesn't send sort params
8. **Filters are mutually exclusive** — text search clears grade level and vice versa
9. **Rank is school-wide only** — no option to rank within a grade level
10. **Column name mismatch** — `unweightedGpa`/`weightedGpa` columns show credit
    attempted/earned, not actual weighted/unweighted GPA values

---

## Phase 0 — Test Data Population

**Goal:** Populate realistic grades for Lewetik Elementary School so the report
has meaningful data to display and we can verify correctness.

**Script:** `scripts/populate-grades.js` (to be created)

### Lewetik Setup (discovered via API)

- **SchoolId:** 163, **AcademicYear:** 2025, **TenantId:** `1e93c7bf-0fae-42bb-9e09-a1cedc8c0355`
- **73 course sections** across G1-G8 (9 subjects/grade) + test courses
- **65 sections with enrolled students**, **66 with assigned teachers**
- Teachers: Sweeter Lengsi (G1-G2), Gisele Zoya Roland (G3-G4), Curtis Joab (G5-G6), Benson Joel (G7-G8)
- Student counts: G1=4, G2=10, G3=8, G4=7, G5=4, G6=5, G7=3, G8=3
- All sections: `gradeScaleId: 2` (PDOE), `isWeightedCourse: true`, `affectsClassRank: true`, `affectsHonorRoll: true`
- All sections span full Year marking period (`yrMarkingPeriodId: 3`)

### Grade Scale: PDOE Grading System (ID: 2)

| Grade | Breakoff | UnweightedGP | GradeId |
|-------|----------|-------------|---------|
| A+    | 97       | 4           | 15      |
| A     | 94       | 4           | 16      |
| A-    | 90       | 4           | 17      |
| B+    | 87       | 3           | 18      |
| B     | 84       | 3           | 19      |
| B-    | 80       | 3           | 20      |
| C+    | 77       | 2           | 21      |
| C     | 74       | 2           | 22      |
| C-    | 70       | 2           | 23      |
| D+    | 67       | 1           | 24      |
| D     | 64       | 1           | 25      |
| D-    | 60       | 1           | 26      |
| F     | 50       | 0           | 27      |
| Inc   | 0        | 0           | 28      |

WeightedGP is 0 for all grades (unweighted scale only).

### Marking Periods

| Level | ID | DoesGrades | DoesExam |
|-------|----|-----------|----------|
| Year  | 3  | true      | true     |
| Semester 1 | 3 | null  | null     |
| Semester 2 | 4 | null  | null     |
| Quarter 1  | 5 | true  | true     |
| Quarter 2  | 6 | true  | true     |
| Quarter 3  | 7 | true  | true     |
| Quarter 4  | 8 | true  | true     |

### API Endpoint

`POST /{tenant}/InputFinalGrade/addUpdateStudentFinalGrade`

**MarkingPeriodId format:** `{type}_{id}[_E]` where type: 0=Year, 1=Semester, 2=Quarter, 3=ProgressPeriod. `_E` suffix = exam grade.

So for quarters: `2_5` (Q1), `2_6` (Q2), `2_7` (Q3), `2_8` (Q4).
With exams: `2_5_E`, `2_6_E`, `2_7_E`, `2_8_E`.

**No GradebookConfiguration needed** — the `addUpdateStudentFinalGrade` endpoint writes
grades directly without checking gradebook config. The config (Staff Portal > Gradebook >
gear icon) only affects the auto-calculated gradebook flow, not direct final grade entry.
Currently all config percentages are 0 / unconfigured at Lewetik.

### Script Approach

1. Read token from `scripts/.token`
2. For each course with sections that have students:
   - Get enrolled student list via API
   - For each quarter (Q1-Q4), generate random percentage per student (bell-curve 60-99)
   - Map percentage to PDOE grade letter and gradeId
   - Submit via `addUpdateStudentFinalGrade` (regular grades)
   - Submit exam grades too (same quarter + `_E` suffix, slightly different random distribution)
3. Log progress and any failures
4. CalendarId needed — must be discovered (likely from the section's calendarId field)

### What We Still Need to Discover

- **CalendarId** for the sections (available in `courseSection.calendarId` from discovery)
- **Student lists per section** — need actual studentId values (not just counts)
- Both can be obtained in the populate script itself

---

## Phase 1 — Add Advanced Search Panel (UI only)

**Files:**
- `UI/src/app/pages/reports/grades-report/class-rank-list/class-rank-list.component.ts`
- `UI/src/app/pages/reports/grades-report/class-rank-list/class-rank-list.component.html`

**Reference implementation:** Honor Roll component (same module, same folder)
- `UI/src/app/pages/reports/grades-report/honor-roll/honor-roll.component.ts`
- `UI/src/app/pages/reports/grades-report/honor-roll/honor-roll.component.html`

**Note:** `grades-report.module.ts` already imports `SearchStudentModule`.

### Component TS

1. Import `AdvancedSearchExpansionModel` from `src/app/models/common.model`
2. Add properties:
   - `advancedSearchExpansionModel: AdvancedSearchExpansionModel`
   - `showAdvanceSearchPanel = false`
   - `isFromAdvancedSearch = false`
   - `searchValue`, `toggleValues`
3. In constructor, configure expansion model (disable accessInformation,
   enrollmentInformation, searchAllSchools — matching Honor Roll)
4. Add methods:
   - `filterData(res)` — receive filterParams, call `getClassRankList()`
   - `getToggleValues(event)` — store toggle state
   - `hideAdvanceSearch(event)` — close panel
   - `getSearchInput(event)` — capture search value

### Component HTML

1. Add tune icon toggle button next to the search bar
2. Add overlay panel at bottom of template:
   ```html
   <div class="fixed top-0 left-0 w-full h-full z-50" *ngIf="showAdvanceSearchPanel">
     <div class="advance-search-panel absolute bg-white w-full sm:w-4/6 md:w-3/6 lg:w-2/6 h-full z-20" @fadeInRight>
       <vex-common-search-student ...bindings... />
     </div>
     <div class="advance-search-backdrop bg-black opacity-50 ..."></div>
   </div>
   ```
3. Keep existing text search + grade level dropdown (work independently)

---

## Phase 2 — Backend Performance

**File:** `API/opensis.report/report.data/Repository/GradeReportRepository.cs`
(method `GetCGPARankListReport`, lines 506-794)

### 2a. Fix N+1 on HistoricalCreditTransfer (lines 706-712)
- Batch-fetch all records for `studentIdList` in one query before the per-student loop
- Replace per-student query with dictionary lookup

### 2b. Pre-compute marking period flags
- Before iterating grades, build per-student sets of which marking period levels exist
- Replace repeated `.Any()` calls (lines 656-702) with O(1) lookups

---

## Phase 3 — Ranking Improvements

**File:** `API/opensis.report/report.data/Repository/GradeReportRepository.cs`

### 3a. Tied-rank support (lines 756-760)
- Replace sequential `rank++` with competition ranking (same GPA = same rank)

### 3b. Rank within grade level (stretch)
- Add optional parameter to rank per grade level instead of school-wide
- Would require grouping by `GradeId` before ranking

---

## Phase 4 — UI Polish

### 4a. Enable Excel export
- Uncomment export button (HTML lines 25-27)
- Wire up export logic following Honor Roll pattern

### 4b. Column sorting
- Add `mat-sort-header` to table columns
- Pass `sortingModel` to API (backend already handles it, lines 762-767)

### 4c. Allow combining filters
- Remove mutual exclusion between text search and grade level dropdown
- Build filterParams that include both criteria when both are set

---

## Verification

- `cd UI && npm start` — open Reports > Grades > GPA/Class Rank List
- Advanced Search panel opens, filters apply, results update correctly
- Existing text search and grade level dropdown still work
- Pagination works after advanced filtering
- For backend changes: compare GPA values and row counts before/after on a real tenant DB
- Phase 0 data should produce ~44 students with grades across 4 quarters, all with computed GPAs and ranks
