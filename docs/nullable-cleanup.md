# Nullable Warning Cleanup — Working Plan

## Why This Matters

`opensis.data` has `<Nullable>enable</Nullable>` in its `.csproj`. This tells the compiler to
track nullability and warn about every place where a null could propagate unchecked. These
warnings are not noise — they are the compiler's static analysis of potential runtime
`NullReferenceException` sites.

Working through them properly:
- Confirms code that is genuinely safe (documents intent with `!`)
- Finds latent bugs where null genuinely can occur and the code would throw or misbehave
- Makes the null contract of the codebase explicit and auditable

The goal is **not** to suppress warnings. It is to read each warning site in context and make
an informed decision.

---

## Current State (as of 2026-03-17)

**Total CS warnings: 220** — all in `opensis.data/Repository/`

| Code   | Count | Meaning |
|--------|-------|---------|
| CS8602 | 158   | Dereference of possibly null reference (`x.Property` where x may be null) |
| CS8629 |  60   | Nullable value type may be null (`(int)x` or `x.Value` where x is `int?`) |
| CS8601 |   2   | Possible null reference assignment |

| File                                  | Warnings | Status  |
|---------------------------------------|----------|---------|
| CommonRepository.cs                   |   52     | pending |
| StudentRepository.cs                  |   28     | pending |
| StaffRepository.cs                    |   26     | pending |
| StudentAttendanceRepository.cs        |   24     | pending |
| ReportCardRepository.cs               |   24     | pending |
| StudentScheduleRepository.cs          |   22     | pending |
| StudentPortalRepository.cs            |   14     | pending |
| StaffPortalRepository.cs              |   12     | pending |
| UserRepository.cs                     |    4     | pending |
| CustomFieldRepository.cs              |    4     | pending |
| MarkingperiodRepository.cs            |    2     | pending |
| SchoolRepository.cs                   |    2     | pending |
| PeriodRepository.cs                   |    2     | pending |
| InputFinalGradeRepository.cs          |    2     | pending |
| StudentHistoricalGradeRepository.cs   |    2     | pending |

**Suggested order:** smaller files first (bottom of table upward) to establish patterns before
tackling CommonRepository.cs (52 warnings) and the other large files.

---

## Decision Rules

For each warning site, read the surrounding code and apply the following logic:

### CS8629 — `(int)x` or `x.Value` where `x` is `T?`

Ask: *Can this value ever be null at this point in the code?*

| Situation | Fix | Rationale |
|-----------|-----|-----------|
| DB column is NOT NULL / model property is `[Required]` / prior `.Where(x => x.Field != null)` filters it | `x!.Value` or `(int)x!` | Compiler false positive — assert non-null explicitly |
| Value comes from user input or optional DB column — null is genuinely possible | `x ?? defaultValue` or add null guard before use | Real null path — handle it properly |
| Unsure | Add null guard + log/comment — don't silently swallow with `?? 0` | Safety first |

### CS8602 — `x.Property` where `x` may be null

Ask: *How was `x` obtained, and is it guaranteed to be non-null here?*

| Situation | Fix | Rationale |
|-----------|-----|-----------|
| Navigation property loaded via `.Include()` | `x!.Property` | Include guarantees load; compiler doesn't know |
| Result of `.First()` / `.Single()` (throws if empty) | `x!.Property` | Safe — already throws on null/empty |
| Result of `.FirstOrDefault()` without a prior null check | Add `if (x != null)` guard | Genuine null path possible |
| `this.context?` where context is always injected | `this.context!` or restructure | Context is always set post-construction |
| Left join / optional association | Add null guard | Could genuinely be null |

### CS8601 — possible null reference assignment

Read the specific site. Usually fixable with null-coalescing or a guard, or `!` if the
assignment source is provably non-null.

---

## Patterns Found (updated as we work)

*This section grows as we process files. Record recurring patterns here to speed up later files.*

---

## Process Per File

1. Run build, note exact warning lines for the file
2. Read each warning site in context (surrounding ~10 lines minimum)
3. Classify: false positive vs genuine null path
4. Apply fix (see decision rules)
5. Rebuild — confirm warning count drops, no new warnings, no errors
6. Update the status table above

---

## Ground Rules

- **Never suppress with `#nullable disable`** — that defeats the whole exercise
- **Never add `!` without reading the site** — `!` is an assertion, not a magic fix
- **One file per commit** (or logical group of small files) — keeps diffs reviewable
- **If a fix is uncertain, leave a `// TODO: verify null safety` comment** rather than
  guessing — flag it for a second look
- Build must be **green after every file** — never carry forward broken state
