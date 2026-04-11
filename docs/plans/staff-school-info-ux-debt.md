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

1. Open the staff → School Info → Edit.
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

**Out of scope reason.** UX redesign, not a bug fix. File as its own issue.

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

**Out of scope reason.** Pre-existing. Requires a small product
decision (hard vs soft delete) before coding.

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
-- Staff with multiple "home" rows — the clearest smoking gun
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
ORDER BY row_count DESC;
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

**Out of scope reason.** Per-tenant operational cleanup. Not code.

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

**Out of scope reason.** Depends on item 3 being done first.

### 5. "Add already-attached school as home" edge case

**What's missing.** If a staff is externally attached to School B and
the admin wants to make B the new home, the current UI dropdown on row
1 excludes already-selected schools (via `selectedSchoolId.includes`),
so the admin cannot pick B. The workaround is to remove B as an
external attachment first — but the trash icon is broken (item 2), so
there's no clean UI path.

**What it should do.** The "Change home school" dialog from item 1
should allow picking any school, and if the chosen school is already
an external attachment, perform a merge: update the existing external
row to become the home (set its `school_id` to equal its
`school_attached_id`) and retire the old home row as a tombstone. The
id-match upsert already handles this data shape correctly — only the
UI needs work.

**Out of scope reason.** Depends on item 1 being redesigned.

### 6. Post-save audit tooltip may show missing names

**What happens today.** Immediately after a save, the client echoes the
server response into `staffSchoolInfoModel`. The save endpoint
(`UpdateStaffSchoolInfo`) does not populate the `CreatedByName` /
`UpdatedByName` fields on the response; only the read endpoint
(`ViewStaffSchoolInfo`) does. So the audit tooltip may show "Updated:
<date>" without the "by <name>" part until the page is reloaded or
navigated away and back.

**What it should do.** Either:

- Run the staff-name resolver on the save response before returning,
  so the tooltip is correct immediately after save.
- Have the client call `viewStaffSchoolInfo` after a successful save
  to refresh the list.

Either approach is a small diff. The first is more efficient (one
round trip); the second is more robust (any future server-side
computed fields also end up correct).

**Out of scope reason.** Cosmetic, self-correcting on navigation.

### 7. Missing `audit` translation key

**What's missing.** The column header `{{'audit' | translate}}` used in
the School Info list (and in the grade input audit display that shipped
earlier) has no matching entry in `en.json`, `fr.json`, or `es.json`.
`ngx-translate` falls back to rendering the raw key, so the header
literally reads "audit" in the UI.

**What to do.** Add the key to all three i18n files:
`"audit": "Audit"` / `"audit": "Audit"` / `"audit": "Auditoría"`.
One-line change per file. Cosmetic but easy.

**Out of scope reason.** Also affects the grade pages, so it should be
addressed as a small "add missing translation keys" commit that
covers multiple places at once.

### 8. Misleading French/Spanish translations for `homeSchool`

**What's wrong.** The existing `homeSchool` translations render as
"École à la maison" (fr) and "escuela en casa" (es) — both of which
mean "homeschooling" in the pedagogical sense (children taught at home
by parents), not "the staff member's home/primary school". The English
meaning in this codebase is the latter.

**What to do.** Update the translations to something like:

- fr: `"École principale"` or `"École d'origine"`
- es: `"Escuela principal"` or `"Escuela de origen"`

Verify with a native speaker before committing. The key is used in at
least two places
([student-enrollmentinfo.component.html:515](../../UI/src/app/pages/student/add-student/student-enrollmentinfo/student-enrollmentinfo.component.html#L515)
and the new staff School Info list), so a single fix covers both.

**Out of scope reason.** Requires translation review; low urgency as
the English build renders correctly.

---

## Suggested ordering

1. **Item 7** (audit translation key) — trivial, covers multiple
   pages.
2. **Item 8** (homeSchool translation) — needs a native speaker.
3. **Item 3** (duplication cleanup) — operational, per-tenant. Do
   this before item 4.
4. **Item 4** (unique index migration) — gated on item 3.
5. **Item 6** (audit tooltip refresh) — small code fix, isolated.
6. **Items 1, 2, 5** (UX redesign of the School Info tab) — larger
   design conversation. Best done as a single coherent redesign PR
   rather than three separate tweaks, since they all touch the same
   component and all relate to "how the user manages a staff's
   school attachments".
