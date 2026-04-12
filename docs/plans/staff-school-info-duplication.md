# StaffSchoolInfo Duplication — Root Cause & Remediation

## Status

**Active production data corruption. No fix applied yet — investigation only.**

A reporter flagged `staff_school_info` rows being duplicated. The original
suspicion was that "Disable Staff" triggered it. Investigation shows Disable
Staff is not the cause — the cause is the School Info tab's save path
(`UpdateStaffSchoolInfo`), which has been a pre-existing delete-reinsert
footgun that was previously gated to Super Administrators only, and which
commit 53a504ec unintentionally exposed to regular admins in non-home
schools.

This is a **missed case** from the [delete-reinsert-data-loss.md](delete-reinsert-data-loss.md)
inventory. `StaffRepository.UpdateStaffSchoolInfo` is not listed there but
exhibits the same anti-pattern with an additional twist that makes it
actively compounding for multi-school staff.

---

## Verified Mechanism

### The server code

[StaffRepository.cs:1106-1191](../../API/opensis.data/Repository/StaffRepository.cs#L1106-L1191)
`UpdateStaffSchoolInfo`:

```csharp
// 1) DELETE scoped by (TenantId, StaffId, SchoolId == session school)
var staffSchoolInfoData = this.context?.StaffSchoolInfo.Where(x =>
    x.TenantId == staffSchoolInfoAddViewModel.TenantId
 && x.StaffId  == staffSchoolInfoAddViewModel.StaffId
 && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId).ToList();

if (staffSchoolInfoData != null && staffSchoolInfoData.Any())
{
    this.context?.StaffSchoolInfo.RemoveRange(staffSchoolInfoData);
    this.context?.SaveChanges();
}

// 2) INSERT — every row from the client list, Id forced to 0
foreach (var staffSchoolInfo in staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList())
{
    staffSchoolInfo.Id = 0;                                // always new PK
    staffSchoolInfo.UpdatedOn = DateTime.UtcNow;
    staffSchoolInfo.CreatedOn = DateTime.UtcNow;           // wipes audit
    staffSchoolInfo.CreatedBy = staffSchoolInfo.UpdatedBy; // wipes audit
    staffSchoolInfo.StaffMaster = null;
    this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
}
this.context?.SaveChanges();
```

### The client code

[staff-schoolinfo.component.ts:394-402](../../UI/src/app/pages/staff/add-staff/staff-schoolinfo/staff-schoolinfo.component.ts#L394-L402)
`updateSchoolInfo`:

```typescript
this.staffSchoolInfoModel?.staffSchoolInfoList?.map((item) => {
    item.tenantId = this.defaultValuesService.getTenantID();
    item.schoolId = this.defaultSchoolId !== 0
                      ? this.defaultSchoolId
                      : this.defaultValuesService.getSchoolID();   // ← !!
    item.staffId = this.staffService.getStaffId();
    ...
});
```

Before POST, **every row's `school_id` is overwritten** to either the
user-picked home school (`defaultSchoolId`, only set via `onSchoolChange`)
or — the usual case — the **session school** (`getSchoolID()`).

### Schema semantics (verified)

Verified via [StaffRepository.cs:156](../../API/opensis.data/Repository/StaffRepository.cs#L156)
(AddStaff) and [StaffRepository.cs:549-561](../../API/opensis.data/Repository/StaffRepository.cs#L549-L561)
(ViewStaff):

- `school_id` = the staff's **home** school.
- `school_attached_id` = the school where the staff is assigned a profile.
- **Home row:** `school_id == school_attached_id`.
- **External-attachment row:** `school_id != school_attached_id`.
  `school_id` = home school, `school_attached_id` = external school.

Confirmed again by [SchoolRepository.cs:1089-1102](../../API/opensis.data/Repository/SchoolRepository.cs#L1089-L1102),
which auto-creates an external attachment when a new school is added by a
non-Super-Admin user, setting `school_id = user's home, school_attached_id = new school`.

### The compounding-duplication scenario

Take a multi-school staff X whose home school is A, with an external
attachment to B. Initial `staff_school_info`:

| id | staff_id | school_id | school_attached_id | role |
|----|----------|-----------|--------------------|------|
| 1  | X        | A         | A                  | home |
| 2  | X        | A         | B                  | external |

An admin logged into **School B** opens staff X → School Info tab → clicks
**Update** (maybe without changing anything):

1. `viewStaffSchoolInfo` returns both rows (filter is `tenant_id + staff_id`, [StaffRepository.cs:1006](../../API/opensis.data/Repository/StaffRepository.cs#L1006)).
2. `updateSchoolInfo` rewrites both rows' `schoolId = B` (session school).
3. POST body: `[{id:1, school_id:B, school_attached_id:A}, {id:2, school_id:B, school_attached_id:B}]`.
4. Server `UpdateStaffSchoolInfo`:
   - `staffMaster WHERE StaffId=X AND SchoolId=B` → **null** (staffMaster.school_id is A), so the entire scalar-field update block is silently skipped.
   - DELETE WHERE `StaffId=X AND SchoolId=B` → deletes **nothing** (the DB rows still have `school_id=A`).
   - INSERT both rows fresh with `school_id=B`, `id=0` → 2 new PKs.

State after the first save:

| id  | staff_id | school_id | school_attached_id |
|-----|----------|-----------|--------------------|
| 1   | X        | A         | A                  |
| 2   | X        | A         | B                  |
| NEW | X        | B         | A                  |
| NEW | X        | B         | B                  |

Four rows where there should be two.

**Next save from School B** doubles again: viewStaffSchoolInfo returns all
4 rows; the client rewrites all `school_id=B`; DELETE matches the 2 new
rows; INSERT writes 4 fresh rows. Now **6 rows**. Every save from School B
adds 2 more.

**If the home-school admin later saves from School A**, DELETE matches
whichever rows currently have `school_id=A` — but the rows with
`school_id=B` survive and keep multiplying on the next B save. The pattern
is additive and does not self-correct.

### Corruption visible in `ViewStaff`

[StaffRepository.cs:549-561](../../API/opensis.data/Repository/StaffRepository.cs#L549-L561)
classifies home vs external via `SchoolId == SchoolAttachedId`. After
corruption the staff above looks like this to the viewer:

| id  | school_id | school_attached_id | classification |
|-----|-----------|--------------------|----------------|
| 1   | A         | A                  | **home = A** |
| 2   | A         | B                  | external = B |
| NEW | B         | A                  | external = A (wrong — A is home) |
| NEW | B         | B                  | **home = B** (overrides the correct one) |

`DefaultSchoolId` ends up as B (wrong), `ExternalSchoolIds` = `[B, A]`.
This corruption is user-visible via the Staff view once it happens.

---

## Why commit 53a504ec lit this up (but did not create it)

The bug has existed as long as `UpdateStaffSchoolInfo` has. What changed is
**who can trigger it.**

Pre-53a504ec [`checkExternalSchoolId`](../../UI/src/app/services/staff.service.ts#L123)
compared a string (session school id from localStorage) against a number
(`staffMaster.schoolId` from the backend) with strict `!==`. The comparison
was always true for multi-school staff, so the code always fell into the
"not in current school" branch, and the inner `externalSchoolIds.findIndex`
check also compared string-to-number (always `-1`). Net effect for regular
admins: **editing multi-school staff was silently blocked from every
school, including the actual home school.** The only users who could edit
at all were Super Administrators (who skip `checkExternalSchoolId`
entirely) — so the duplication bug was latent but reachable only via the
Super Admin account.

The [53a504ec](https://github.com/PacificEMIS/Pacific-SIS/commit/53a504ec)
fix coerced both sides to `Number` and added the OR-of-associations check,
correctly enabling regular admins to edit multi-school staff from **any**
associated school. That's the right behavior for General Info / Address
Info / Certification tabs. But the School Info tab's save path
(`updateStaffSchoolInfo` → `UpdateStaffSchoolInfo`) was never safe for
multi-school editing, and it's now reachable by every regular admin in
every external school. **The dormant bug became actively compounding the
moment 53a504ec shipped.**

Commit date: 2026-04-10. The user reports this is already in production
and needs immediate attention.

---

## What "Disable Staff" actually does (and doesn't do)

For the record, because this was the original suspicion:

[CommonRepository.cs:3787-3883](../../API/opensis.data/Repository/CommonRepository.cs#L3787-L3883)
`ActiveDeactiveUser` only flips `staff_master.is_active`. It does
`Include(x => x.StaffSchoolInfo)` to read the end-date guard on the first
matching row, but it never Adds, Removes, or mutates any StaffSchoolInfo
entity. `SaveChanges()` writes exactly one column.

`UpdateStaff` ([StaffRepository.cs:675-870](../../API/opensis.data/Repository/StaffRepository.cs#L675-L870))
— the handler for the General Info and Address Info tab saves — also never
touches `staff_school_info`. It loads `StaffMaster` without including the
collection and only calls `SetValues` on scalar properties.

**The only frontend path that can create duplicates is the School Info tab
save → `updateStaffSchoolInfo`.** A reporter who said "it doubled after I
clicked Disable Staff" probably also hit Save/Update on the School Info
tab in the same session.

---

## Diagnostic SQL (for verifying damage before touching anything)

Run against a tenant DB. This does not modify data.

### 1. Staff with multiple home rows — the smoking gun

A sane staff has **exactly one** row where `school_id = school_attached_id`.
Any staff with more than one has been corrupted.

```sql
SELECT
    staff_id,
    COUNT(*) AS home_row_count
FROM staff_school_info
WHERE school_id = school_attached_id
GROUP BY staff_id
HAVING COUNT(*) > 1
ORDER BY home_row_count DESC;
```

### 2. Staff with duplicate (staff_id, school_attached_id) pairs

A staff should have at most one row per attached school. More than one
means a dup was created by the loop above.

```sql
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

The `distinct_home_school_ids` column tells you how many different
"home schools" have been written for the same staff — for a corrupted
staff you'll typically see 2+ values (the real home, plus the external
school the admin was in).

### 3. Staff with inconsistent home schools vs staff_master

```sql
SELECT
    sm.staff_id,
    sm.school_id                AS master_home,
    ssi.school_id               AS ssi_home,
    COUNT(*) OVER (PARTITION BY sm.staff_id) AS ssi_row_count
FROM staff_master sm
JOIN staff_school_info ssi
    ON ssi.tenant_id = sm.tenant_id
    AND ssi.staff_id = sm.staff_id
WHERE ssi.school_id != sm.school_id
  AND ssi.school_id = ssi.school_attached_id
ORDER BY sm.staff_id;
```

### 4. Full row dump for one suspect staff

Once a suspect `staff_id` is identified, dump its rows with timestamps to
see the chronology:

```sql
SELECT
    id,
    staff_id,
    school_id,
    school_attached_id,
    school_attached_name,
    profile,
    membership_id,
    start_date,
    end_date,
    created_on,
    created_by,
    updated_on,
    updated_by
FROM staff_school_info
WHERE staff_id = <suspect_staff_id>
ORDER BY created_on, id;
```

If the duplication theory is correct you should see clustered pairs of
`created_on` timestamps — each admin save from a non-home school creates
a new set of rows at the same second.

---

## Remediation — Staged Approach

The safest approach is **two separate changes**, landed in order, with the
first buying time for the second.

### Step 1 — IMMEDIATE: stop the bleeding

**Gate the School Info tab save path behind a home-school check.**

The School Info tab is the only frontend path that invokes
`UpdateStaffSchoolInfo`. If we block non-home-school admins from saving on
that tab — while leaving General Info / Address Info / Certification
freely editable per 53a504ec — no new duplicates can be created, zero
database risk, and the rest of 53a504ec's fix is preserved.

Two ways to do this:

**Option 1a — client-side gate (smallest):** In `checkExternalSchoolId`,
or in a new sibling method, detect when the editing tab is School Info
and the current school is not `defaultSchoolId`. Reject the edit there
with a "Please go to {homeSchoolName} for School Info edits" message.
General Info, Address Info, Certification continue to use the permissive
check.

- **Pros:** Pure UI change, zero DB risk, reversible instantly.
- **Cons:** A malicious/curious user with API access could still POST
  directly to `updateStaffSchoolInfo` and trigger the bug.

**Option 1b — server-side safety net (small, stronger):** In
`UpdateStaffSchoolInfo`, after loading `staff_master`, compute the real
home school (the `school_id` of the row where
`school_id == school_attached_id`, or fall back to `staff_master.school_id`)
and if the incoming `SchoolId` doesn't match, return an error response
without modifying anything. Combine with 1a so the UI still shows a
helpful message.

- **Pros:** Defense in depth. Applies even to API clients.
- **Cons:** Slightly riskier than UI-only, since it touches the handler.
  But the change is a pre-check + early return — no data mutation path
  changes.

**Recommendation for Step 1: do both.** UI gate for UX, server check for
safety. Both are small and reversible.

### Step 2 — PROPER FIX: rewrite `UpdateStaffSchoolInfo` as an upsert

Following the upsert pattern already proven in
[delete-reinsert-data-loss.md](delete-reinsert-data-loss.md) and used by
`InputFinalGradeRepository` and `StudentEffortGradeRepository`:

**Business key:** `(TenantId, StaffId, SchoolAttachedId)` — each staff
can only have one row per attached school, and that's the natural
uniqueness constraint the schema should have had from the start.

**Load scope:** all rows for `(TenantId, StaffId)`, not just the current
`SchoolId`. The "filter by session school" behavior is exactly what caused
the bug — it left external-attachment rows invisible to the merge and
caused them to be duplicated on insert.

**Algorithm:**

```
existingRows = Load AsNoTracking WHERE tenant=T AND staff=X
existingByKey = existingRows.ToDictionary(r => r.SchoolAttachedId)

for each incoming in payload.staffSchoolInfoList:
    if existingByKey.TryGetValue(incoming.SchoolAttachedId, out existing):
        # UPDATE — preserve CreatedOn/CreatedBy and the original home school_id
        update fields: profile, membership_id, start_date, end_date,
                       school_attached_name, ...
        preserve:      created_on, created_by, id, school_id  ← critical
        set:           updated_on = now, updated_by = submitter
        Update(entity as Modified)
    else:
        # INSERT — a new school attachment genuinely being added
        INSERT a fresh row with school_id = the staff's real home school
                                 (NOT the session school)

# DO NOT delete anything that isn't in the payload unless the UI sent an
# explicit "this attachment has been removed" marker. Silent deletes are
# how the original bug worked.
```

**Key invariant to enforce:** `school_id` is set once, at the time of row
creation, to the staff's home school. It **must not** be overwritten on
subsequent updates. The server should ignore the client's `schoolId` on
existing rows entirely, since the client is rewriting it incorrectly.

**Handling deletions:** the current UI removes a row from its local list
via `deleteSchoolInfo(index)`. Under a pure upsert, this would silently
leak — the removed row stays in the DB. We need either:
- The UI to send a `deletedIds` list, or
- The server to accept a "full list, prune missing" semantic for rows
  whose `school_attached_id` equals the session school (i.e., only the
  current school's attachment is authoritative for removal).

This deserves its own careful discussion before implementing. The goal
should be: non-home-school admins can add an attachment to their own
school or edit the profile on their own school's row, but cannot touch
rows for other schools.

Step 2 should be landed as its own PR with:
- SQL diagnostic output before and after, on a non-production tenant.
- A reproduction test for each of: single-school staff save, multi-school
  home-school save, multi-school external-school save.
- A careful read of the "what about row deletion" semantics above.

### Step 3 — Data cleanup

Once the bleeding is stopped, clean up existing damage per tenant.

**Do not run any automated cleanup script without the tenant admin's
review.** The cleanup strategy depends on how much the data has drifted.

Recommended cleanup approach per affected staff:

1. Run diagnostic #4 above for the staff.
2. Identify the **oldest** row with `school_id == school_attached_id` —
   that's almost certainly the real home row.
3. Identify the set of genuine `school_attached_id` values the staff
   was actually attached to (probably the distinct set of
   `school_attached_id` values excluding the home school).
4. For each genuine attached school, keep the **oldest** row and delete
   the rest.
5. For any surviving external row, set `school_id` = the real home school
   (fixing the `school_id` corruption).
6. Set `staff_master.school_id` = the real home school if it drifted.

This should be done as a dry-run-first SQL script per tenant, with a
backup before writing, and a per-tenant review.

**Alternatively, for tenants where the damage is limited**, the cleanup
can be done manually through a support ticket workflow — identify the
handful of affected staff via diagnostic #1 and fix them by hand in SQL.

---

## Open Questions to Resolve Before Any Code Change

1. **Is 53a504ec actually deployed?** The commit is dated 2026-04-10. If
   it hasn't shipped to the tenants yet, the "data corrupting as we
   speak" scope is limited to whatever was reachable via Super Admin
   before, and Step 1 is less urgent (but still valuable).
2. **Can we get a row dump from the reporter's tenant?** Diagnostic #4 on
   the affected staff would confirm whether the duplication pattern
   matches this theory exactly, or whether there's a second bug we
   haven't accounted for.
3. **How is a new external attachment actually added today?** I found two
   writers (`AddStaff` line 157 and `SchoolRepository` line 1089) plus
   the delete-reinsert in `UpdateStaffSchoolInfo`. I did NOT find any UI
   wiring that calls the dedicated `AddStaffSchoolInfo` endpoint, so new
   attachments are presumably either created by school-creation side
   effects or by the School Info tab's "Add More" button
   ([staff-schoolinfo.component.ts:231](../../UI/src/app/pages/staff/add-staff/staff-schoolinfo/staff-schoolinfo.component.ts#L231))
   which routes through the buggy `updateStaffSchoolInfo`. If the "Add
   More" path is the only way to attach an external school today, any
   fix in Step 2 must preserve that add capability.
4. **Does the "deleted" row behavior matter in practice?** If nobody has
   actually used the remove-row button, we can defer the deletion
   semantics and just do upsert-with-no-delete in Step 2. Worth checking
   usage before designing the deletion flow.

---

## What I Am NOT Touching Yet

- Any code change.
- `staff_master.school_id` on any tenant.
- Any row in `staff_school_info`.
- The original `docs/plans/delete-reinsert-data-loss.md` — this plan will
  be added to its inventory table once the fix is landed.

This document exists so the analysis can be reviewed before any change
is made. Sign off on the diagnosis and the staged approach, then Step 1
can land.
