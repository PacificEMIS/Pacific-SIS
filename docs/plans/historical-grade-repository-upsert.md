# StudentHistoricalGradeRepository — Upsert Refactor & Audit Preservation

## Status

**Deferred** — feature is not actively used by any Pacific EMIS tenant.

A DB check on 2026-04-10 showed only 3 rows across `historical_grade` and
`historical_credit_transfer`, all entered within an 11-minute window on
2023-11-27, all by one user, all at PICS High School, with `updated_on`
always NULL. This matches the profile of a one-off test session from
over two years ago. The feature has been untouched since.

There is no active data loss or audit destruction happening because
nobody is using the feature. Fix can wait until either (a) a tenant
starts admitting transfer students into the SIS with historical records,
or (b) a scheduled cleanup pass sweeps the remaining delete-reinsert
patterns.

Related: [delete-reinsert-data-loss.md](delete-reinsert-data-loss.md).

---

## Scope

One method in
`API/opensis.data/Repository/StudentHistoricalGradeRepository.cs`:

- `AddUpdateHistoricalGrade` (lines ~230-350)

Touches two tables:
- `historical_grade` — header row per (student, historical grade level,
  historical marking period)
- `historical_credit_transfer` — line items inside each header (courses
  taken, grades earned)

---

## Current Behaviour

Classic delete-reinsert pattern:

1. Find existing `historical_grade` rows for the student in this tenant
   and school
2. Find existing `historical_credit_transfer` rows likewise
3. RemoveRange both, SaveChanges
4. Rebuild from the incoming payload and Add each one fresh, assigning
   new sequential IDs via manual `gradeId++` / `creditId++`

**Data loss**: when saving, ALL historical grades for the student are
wiped — not just the one being edited. So if a student has historical
records from multiple schools or grade levels, editing one regenerates
everything. The frontend probably sends the full list every time, so
this is a latent rather than active risk.

**Audit destruction**: yes, on every re-save.

---

## The Business Key

Composite key on `historical_grade`:
`(TenantId, SchoolId, StudentId, HistGradeId, HistMarkingPeriodId)`

Composite key on `historical_credit_transfer`:
`(TenantId, SchoolId, StudentId, HistGradeId, HistMarkingPeriodId,
  CreditTransferId)`

Upsert key within scope: `(HistGradeId, HistMarkingPeriodId)` for the
header, `CreditTransferId` for the line items.

---

## Proposed Approach

Use the same proven pattern from `StudentEffortGradeRepository`:

1. Load existing rows with `AsNoTracking()`.
2. Build dictionary keyed on `(HistGradeId, HistMarkingPeriodId)`.
3. For each incoming header row:
   - If match exists → build a fresh entity preserving `CreatedBy`/
     `CreatedOn`, set new values, attach as `Modified` via `Update()`.
   - If no match → `Add()` as a new insert.
4. Same approach for the child line items, scoped per header.
5. Do NOT delete rows that aren't in the incoming submission, so
   multi-school or multi-year historical records aren't wiped when
   editing one.
6. Manual ID generation (`gradeId++`/`creditId++`) should be replaced
   with proper next-ID calculation via projected `MAX()` queries —
   same fix pattern used for `Id` in `StudentEffortGradeRepository`.

---

## Audit Display (Frontend)

Follow the same pattern as issues #819/#821:
- Add `[NotMapped]` `CreatedByName`/`UpdatedByName` on
  `HistoricalGrade.cs` (and optionally `HistoricalCreditTransfer.cs`)
- Resolve staff GUIDs to readable names in whatever GET method loads
  historical grade data
- Add an info icon + tooltip in the historical grades UI component
  (find it under student profile / transcript area)

---

## Testing Plan

When the fix is implemented:

1. **First-time save** — enter historical grade records for a student,
   verify `created_on`/`created_by` in the DB.
2. **Re-save** — edit one field, save again. Verify `created_on` is
   preserved, `updated_on` is set.
3. **Multi-record preservation** — give a student two historical grade
   records (e.g., 9th grade from School A, 10th grade from School B).
   Edit one. Verify the other is untouched.
4. **Line items** — same for the credit transfer children.

---

## Decision Log

- **2026-04-10**: Investigated and deferred. DB check showed only 3
  abandoned test rows from Nov 2023. No active users. Creating a
  standalone plan so this can be tackled as a focused fix when needed,
  rather than rushed as part of the broader sweep.
