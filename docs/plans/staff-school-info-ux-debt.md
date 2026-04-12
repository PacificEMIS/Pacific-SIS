# Staff School Info — UX Debt

## Status

**Follow-up to [staff-school-info-duplication.md](staff-school-info-duplication.md).** The
duplication bug is fixed and the data layer is now sane. This document
catalogues the UX and pre-existing-data items that were deliberately kept
out of the hotfix PR but should be addressed in a focused follow-up pass.

---

## Context

The School Info tab on the staff edit page is a multi-school attachment
editor with a delete-then-reinsert backend that this project has now
replaced with an id-matched upsert. The rewrite surfaced several UX and
schema irritations that are pre-existing, user-facing, and worth fixing,
but were out of scope for a hotfix focused on stopping silent data
corruption.

The underlying data model uses a single table (`staff_school_info`) to
encode three semantically distinct things:

1. **Home school** — the one row where `school_id = school_attached_id`,
   cascaded to `staff_master.school_id` and referenced by
   `staff_certificate_info.school_id`.
2. **External attachments** — additional schools the staff has a
   role at, `school_id` (home) != `school_attached_id` (attached).
3. **Historical records** — rows with a past `end_date`, kept around
   as tombstones.

None of the three is made explicit in the schema or UI; users have to
infer them from column values. Every UX pain point in this doc traces
back to that conflation.

---

## Items to address

### 1. Clunky two-step home-school change

**What happens today.** To change a staff's home school, the admin has
to:

1. Open the staff -> School Info -> Edit.
2. Set the current home row's `end_date` to a date in the past.
3. Save. (Nothing visually changes in the list.)
4. Edit again. The home row's school dropdown is now enabled because
   `end_date < today`.
5. Pick the new school.
6. Save again.

**Why.** The `disabled` expression on the school dropdown in the edit
form ([staff-schoolinfo.component.html:100](../../UI/src/app/pages/staff/add-staff/staff-schoolinfo/staff-schoolinfo.component.html#L100))
requires the row to have a past end_date before the dropdown becomes
editable. The logic is a pre-existing artefact, not a deliberate design.

**What it should do.** Offer a single-button "Change home school" action
that opens a small dialog: pick the new school, optionally set the
transition date, and confirm. The backend cascade (update
`staff_master.school_id`, migrate `staff_certificate_info.school_id`) is
already in place and idempotent.

**Edge case to design for.** If the admin picks a school that's already
an external attachment, the dialog should offer to merge: make the
existing external row the new home, retire the old home row as a
historical tombstone. The id-match upsert supports this shape today; the
UI just needs to send the right payload.

### 2. Trash icon doesn't work

**What happens today.** In edit mode, each school attachment row has a
delete (trash/clear) icon at its right edge
([staff-schoolinfo.component.html:142](../../UI/src/app/pages/staff/add-staff/staff-schoolinfo/staff-schoolinfo.component.html#L142)).
The icon is grayed out for any persisted row — the condition is
`*ngIf="staffSchoolInfoList[i].id == null"`, so it's only active for
unsaved rows that haven't been persisted yet.

**Consequence.** A persisted external attachment cannot be removed
through the UI. The workaround is to set an end_date and leave the row
as a tombstone, which is semantically different and clutters the view.

**What it should do.** Allow removing any non-home row. Removal should
either:

- **Hard delete** the row (simple, works with the current id-match
  upsert: drop the row from the payload and the server deletes it), or
- **Soft delete** by setting `end_date = today`, converting it to a
  tombstone. Keeps the history but requires the UI to filter out
  tombstones in the "active attachments" view.

The hard-delete path is cleanest and matches the existing upsert
semantics (payload-dropped rows get deleted unless they are already
tombstones). The UI change is making the `*ngIf` on the trash icon
permissive for any non-home row, and ensuring the removed row is
actually stripped from `staffSchoolInfoList` before the submit call.

### 3. Pre-existing duplication corruption

**What happens today.** Any tenant that was running a version of
Pacific-SIS that contained the School Info bug (every build before the
hotfix) may have `staff_school_info` rows duplicated from non-home
saves. The hotfix stops new corruption from occurring but does NOT
heal existing bad rows. A previous iteration of the upsert included an
opportunistic "heal on save" step; it was removed from the final
hotfix because it made the save path reason about two different
concerns (legitimate edits vs corruption healing) and grew edge cases.

**Diagnostic SQL** (from the duplication plan doc, reproduced here for
convenience). Run against each tenant to see the scope:

```sql
-- Staff with multiple "home" rows -- the clearest smoking gun
SELECT
    staff_id,
    COUNT(*) AS home_row_count
FROM staff_school_info
WHERE school_id = school_attached_id
GROUP BY staff_id
HAVING COUNT(*) > 1
ORDER BY home_row_count DESC;
```

```sql
-- Staff with duplicate (staff_id, school_attached_id) pairs
SELECT
    staff_id,
    school_attached_id,
    COUNT(*) AS row_count,
    GROUP_CONCAT(id ORDER BY id) AS row_ids,
    GROUP_CONCAT(DISTINCT school_id) AS distinct_home_school_ids
FROM staff_school_info
GROUP BY staff_id, school_attached_id
HAVING COUNT(*) > 1
ORDER BY row_count DESC, staff_id;
```

**Cleanup strategy.** Per-staff and manual, not automated. Once you
have the list of affected staff:

1. Dump the full row set for each affected staff
   (see diagnostic #4 in [staff-school-info-duplication.md](staff-school-info-duplication.md)).
2. Identify the real home school (usually the oldest row with
   `school_id = school_attached_id`).
3. Identify the genuine external attachments (distinct
   `school_attached_id` values excluding the home).
4. For each genuine attachment, keep the oldest row and delete the
   rest.
5. Fix any drifted `school_id` values on surviving rows to equal the
   real home school.
6. Verify `staff_master.school_id` equals the real home school; fix
   if it drifted.

Do this as dry-run-first SQL per tenant, with a backup before writing.
Ideally done as support-ticket work with the tenant admin reviewing
which rows are "real".

### 4. Missing database unique constraint

**What's missing.** `staff_school_info` has a surrogate primary key
`id` but no unique constraint on the natural business key
`(tenant_id, staff_id, school_attached_id)`. That's why the original
bug was possible: nothing at the database layer prevented duplicate
`(staff, school)` pairs from being inserted.

**What to do.** After running the cleanup pass from item 3 and
confirming zero duplicates across all tenants, add a migration that
creates a UNIQUE index on
`(tenant_id, staff_id, school_attached_id)`. This prevents any future
code regression from re-introducing duplicates.

**Gotcha.** The migration cannot be applied to a tenant that still has
corrupted data — the index creation will fail. Per the deploy
mechanism (`MySQLContextFactory` auto-migrates on each tenant
connection), this would fail startup on any corrupted tenant. So the
cleanup MUST be completed on all tenants before the migration can
safely ship.

Safer alternative: add a non-unique index for performance now, and
defer the UNIQUE upgrade until every tenant is verified clean.

### 5. Can't re-assign a staff to a previously-attached school

**What happens today.** The school dropdown in edit mode disables any
school that already appears in `selectedSchoolId` (i.e. any school
the staff is currently or historically attached to). This means a
real-world reassignment cycle like:

- Teacher at School A (2020-2022)
- Teacher moves to School B (2022-2024)
- Teacher returns to School A (2024-now)

...cannot be represented. The admin can't pick School A in the
dropdown because it's already in the list (as a tombstone with a past
end_date). This is a legitimate pattern in Pacific Island schools
where teachers rotate between schools on multi-year cycles.

**Root cause.** The `[disabled]` binding on each `mat-option`:
```html
[disabled]="selectedSchoolId.includes(+school.schoolId)"
```
treats every row in `selectedSchoolId` as "taken" regardless of
whether the row is active or retired. The `selectedSchoolId` array
is populated in `manipulateArray()` which skips past-end-date rows
via `splice(i, 1)` — but the splice logic has bugs (it mutates the
array while iterating, causing index drift) and may not actually
remove tombstone school IDs reliably.

**What it should do.** Only exclude actively-attached schools from
the dropdown. Tombstone rows (past end_date) should NOT reserve
their school ID in `selectedSchoolId`, because the attachment has
ended and the school is available for reassignment. The splice
logic in `manipulateArray()` should be fixed or replaced.

**Edge case:** if the admin re-adds School A as a new row while the
tombstone for School A still exists, the backend should handle it
correctly: the tombstone has a different `id` from the new row, so
the id-match upsert would see the new row as `id=0` (insert) and
the tombstone as handled/kept. The result would be two rows for
School A: the tombstone (2020-2022) and the new active row
(2024-now). This is semantically correct and represents the full
history. The "one row per school" invariant no longer holds, but
that's the right trade-off for supporting reassignment.

**Impact on a future UNIQUE constraint (item 4).** If we support
multiple rows per `(staff_id, school_attached_id)` for the
reassignment case, the UNIQUE index from item 4 would need to be
scoped differently — perhaps
`(tenant_id, staff_id, school_attached_id)` WHERE `end_date IS NULL`
(a partial unique index on active rows only). MySQL 8 doesn't
support partial unique indexes natively, but the constraint can be
enforced at the application layer via the upsert logic. Worth
designing items 4 and 5 together.

### 6. Post-save audit tooltip may show missing names

**What happens today.** Immediately after a save, the server now
returns a refreshed list with audit names resolved (fixed in this PR).
However, in edge cases where the `CreatedBy`/`UpdatedBy` GUID doesn't
match any `StaffMaster.StaffGuid` (e.g. the creating user was later
deleted, or the GUID was entered manually), the name field will be
blank and the tooltip shows just the date without "by ...".

This is a minor display issue that affects all audit tooltips across
the app (grade input, effort grades, and now school info). No fix
needed unless a user reports it.

### 7. Missing `audit` translation key

**What's missing.** The column header `{{'audit' | translate}}` used in
the School Info list (and in the grade input audit display that shipped
earlier) has no matching entry in `en.json`, `fr.json`, or `es.json`.
`ngx-translate` falls back to rendering the raw key, so the header
literally reads "audit" in the UI.

**What to do.** Add the key to all three i18n files:
`"audit": "Audit"` / `"audit": "Audit"` / `"audit": "Auditoria"`.
One-line change per file. Cosmetic but easy.

### 8. Misleading French/Spanish translations for `homeSchool`

**What's wrong.** The existing `homeSchool` translations render as
"Ecole a la maison" (fr) and "escuela en casa" (es) — both of which
mean "homeschooling" in the pedagogical sense (children taught at home
by parents), not "the staff member's home/primary school". The English
meaning in this codebase is the latter.

**What to do.** Update the translations to something like:

- fr: `"Ecole principale"` or `"Ecole d'origine"`
- es: `"Escuela principal"` or `"Escuela de origen"`

Verify with a native speaker before committing. The key is used in at
least two places
([student-enrollmentinfo.component.html:515](../../UI/src/app/pages/student/add-student/student-enrollmentinfo/student-enrollmentinfo.component.html#L515)
and the new staff School Info list), so a single fix covers both.

---

## Suggested ordering

1. **Item 7** (audit translation key) — trivial, covers multiple
   pages.
2. **Item 8** (homeSchool translation) — needs a native speaker.
3. **Item 3** (duplication cleanup) — operational, per-tenant. Do
   this before item 4.
4. **Items 4+5** (unique index + reassignment support) — design
   together since they interact. Item 5 may relax the constraint
   from item 4.
5. **Item 6** (audit tooltip edge case) — cosmetic, low priority.
6. **Items 1, 2** (UX redesign of the School Info tab) — larger
   design conversation. Best done as a single coherent redesign PR
   since they touch the same component and relate to "how the user
   manages a staff's school attachments".
