# Enrollment Codes Overhaul

## Problem

The current enrollment code system reuses the same "Dropped Out" label for fundamentally
different student outcomes. This creates data ambiguity and makes reporting unreliable
without fragile workaround logic.

## How it works today

`StudentEnrollment` has two code fields:

| Field | Set when | Purpose |
|---|---|---|
| `EnrollmentCode` | Student enters a school (rollover, transfer, new) | How the student arrived |
| `ExitCode` | Student leaves (drop, transfer, rollover) | How the student left |

Both are free-text strings matched against `StudentEnrollmentCode.Title`.

`StudentEnrollmentCode.Type` categorizes codes: `"Rolled Over"`, `"Drop"`,
`"Add"`, `"Enroll (Transfer)"`, `"Drop (Transfer)"`.

### Retained/repeated students

During rollover, when `RollingOption = "Retain"`:
1. OLD enrollment: `IsActive = false`, `ExitCode` = title of first code with `Type = "Drop"`
   (typically "Dropped Out"), `RollingOption` stays "Retain"
2. NEW enrollment: `IsActive = true`, `EnrollmentCode` = same "Dropped Out" title,
   `RollingOption = "Next grade at current school"`, same grade as before

The UI works around this at `student-enrollmentinfo.component.html:608` by displaying
"Retained" when `enrollmentCode === 'Dropped Out'`. But the underlying data says "Dropped Out".

### Graduating / completing students

Students finishing their final grade (e.g. Grade 12, Grade 8 at elementary) use
`RollingOption = "Do not enroll after this school year"`, which also results in
`ExitCode = "Dropped Out"`. These students are completers, not dropouts.

### Identifying repeaters for reporting

Repeaters can be identified on active enrollments: their `EnrollmentCode` matches a
`StudentEnrollmentCode` with `Type = "Drop"` (they arrived via "Dropped Out" because
the rollover Retain path uses the Drop code for the new enrollment). This works but is
semantically misleading.

### Identifying dropouts for reporting — currently broken

Dropouts cannot be reliably identified with the current data model. The obstacles:

1. **`StudentEnrollment.IsActive` is unreliable**: when a student is manually dropped
   via the enrollment UI, `exit_code` and `exit_date` are set and `student_master.is_active`
   becomes false, but `student_enrollment.is_active` may stay true. Some drop flows set it
   to false, others don't.

2. **`ExitCode = "Dropped Out"` is shared across unrelated outcomes**: all of the following
   produce `ExitCode = "Dropped Out"`:
   - Actual mid-year dropouts (manual drop via UI)
   - Completers/graduates via rollover (`RollingOption = "Do not enroll after this school year"`)
   - Retained students' old enrollment (rollover Retain path)

3. **Completers dominate the count**: real-world data from Ohmine Elementary (school 170,
   calendar 4 / academic year 2025) shows 133 records with `ExitCode = "Dropped Out"`.
   Breakdown by origin:
   - ~100 from rollover (`rollover_id IS NOT NULL`) — completers who finished Grade 8
   - ~32 manual (`rollover_id IS NULL`) — mix of actual dropouts and manually processed
     completers
   - 1 confirmed manual dropout

4. **`RollingOption` does not help**: all 133 records have
   `rolling_option = "Do not enroll after this school year"`, whether they are completers
   or actual dropouts.

5. **`RolloverId` is a partial signal**: enrollments exited by the rollover process have
   `rollover_id` set, but manually dropped students before rollover runs don't. This can
   separate rollover-generated exits from mid-year drops but not completers from dropouts
   within either group.

**Conclusion**: the dashboard "Dropouts by Grade" chart requires the enrollment codes
overhaul before it can show accurate data. The repeaters chart works with current data.

## What needs to change

### New enrollment code types needed

| Current state | Should be | Affected flow |
|---|---|---|
| Retained student gets `EnrollmentCode = "Dropped Out"` | Dedicated "Retained" / "Repeated" code | Rollover "Retain" path |
| Completing student gets `ExitCode = "Dropped Out"` | Dedicated "Completed" / "Graduated" code | Rollover "Do not enroll" path |
| Re-enrolled student has no distinct code | Dedicated "Re-enrolled" code | Re-enrollment flow |

### Additional exit codes

Missing exit code types (some addressed by #663):
- Completed / Graduated
- Expelled
- Deceased
- Suspension

### Terminology flexibility

Different countries use different terms (e.g. "promoted" vs "rolled over",
"retained" vs "repeated"). The system should support renaming codes per tenant.

## Related GitHub issues

- [#634](https://github.com/PacificEMIS/Pacific-SIS/issues/634) — Incorrect enrollment
  code "Dropped Out" for retained students (core problem)
- [#607](https://github.com/PacificEMIS/Pacific-SIS/issues/607) — "Do not enroll" students
  incorrectly classified as dropouts (completers vs dropouts)
- [#524](https://github.com/PacificEMIS/Pacific-SIS/issues/524) — Grade 12 students on
  rollover should be "Graduated" not "Dropped Out"
- [#663](https://github.com/PacificEMIS/Pacific-SIS/issues/663) — Add missing exit codes
  (Completed, Expelled, Deceased, Suspension) — development done
- [#642](https://github.com/PacificEMIS/Pacific-SIS/issues/642) — Improve enrollment code
  approach (renaming, custom codes)
- [#533](https://github.com/PacificEMIS/Pacific-SIS/issues/533) — Re-enroll enrollment code
  question
- [#572](https://github.com/PacificEMIS/Pacific-SIS/issues/572) — Dropped out and re-enrolled
  student only has single editable enrollment record

## Implementation considerations

- **Data migration**: existing enrollment records with "Dropped Out" need to be reclassified.
  This requires identifying which "Dropped Out" records are actually retentions vs completions
  vs real dropouts. The `RollingOption` field on old enrollments can help distinguish them.
- **Rollover code changes**: `RolloverRepository.cs` lines 962-987 (Retain path) and
  996-1041 (Do not enroll path) need to use the new code types.
- **UI updates**: enrollment info component already has workaround display logic that
  can be simplified once codes are correct.
- **Reporting impact**: any dashboard or report query that identifies repeaters/dropouts
  by enrollment code will need updating. Current workaround logic in `CommonRepository.cs`
  (dashboard) should be replaced with cleaner type-based queries.
- **Testing**: rollover is the most critical flow. All rollover paths (promote, retain,
  do-not-enroll, transfer) need end-to-end testing after changes.
- **Backward compatibility**: existing reports and exports that reference "Dropped Out"
  may need migration or mapping.

### Promoted students (for reference)

Promoted students' old enrollment gets `ExitCode` = title of `StudentEnrollmentCode`
with `Type = "Rolled Over"` (typically "Rolled Over"). This is the one rollover path
that uses a distinct exit code, so promoted exits can be identified reliably.

### Additional data integrity issues found

- **Soft-disable vs hard deactivation mismatch**: the UI "Disable Student" toggle sets
  `student_master.is_active = false` but leaves all `student_enrollment.is_active = true`.
  The formal drop/transfer/rollover paths set both. Queries must check both fields or use
  `exit_code`/`exit_date` instead. See `CommonRepository.ActiveDeactiveUser()` remarks.

- **Duplicate enrollment codes per school**: school 170 has multiple `StudentEnrollmentCode`
  rows with identical titles (e.g. four separate "Dropped Out" entries with different
  `enrollment_code` IDs). Queries using `Title` matching work but the duplicates add noise.

- **Typos in exit codes**: real data contains both `"Dudplicate "` and `"Duplicate "`
  (with trailing space) as exit codes. Any code-matching logic must account for this.

## Status

**Not started** — requires careful planning, data migration, and testing.

Dashboard currently shows "Repeaters by Grade" using workaround logic in
`CommonRepository.cs`. "Dropouts by Grade" chart is implemented but will show
inaccurate data until enrollment codes are overhauled — it is held back from
deployment pending this work.
