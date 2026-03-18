# .NET 6 to .NET 8 Upgrade Plan

Status: NOT STARTED
Target: .NET 8.0 LTS (supported until November 2026)
Skip: .NET 7 (STS, already EOL), .NET 9 (STS, near-EOL)

---

## Prerequisites

### Fix existing version mismatches

Before upgrading, standardize all projects to consistent .NET 6 package versions:

- [ ] `opensis.backgroundjob` uses EF Core 7.0.2 + Pomelo 7.0.0 — downgrade to 6.0.0 to match everything else (or confirm it works and upgrade everything together)
- [ ] `opensis.Test` targets `netcoreapp3.1` — update to `net6.0` first
- [ ] NLog versions: 4.7.12 vs 4.7.3 — standardize
- [ ] MySqlBackup.NET: 2.3.7 vs 2.3.6.1 — standardize
- [ ] Remove `Microsoft.VisualStudio.Web.CodeGeneration.Design 3.1.4` (ancient .NET 3.1 tooling package) or update to 6.0.x
- [ ] Remove explicit `Microsoft.AspNetCore.Http.Abstractions 2.2.0` references — the framework provides these automatically in net6.0+

### Set up query verification harness

Before changing any EF Core version, build an automated verification tool to compare query outputs before and after. This ensures no silent behavior changes slip through.

- [ ] Create a C# console script or `dotnet-script` tool that:
  - Connects to the test tenant DB via EF Core
  - Calls each repository method with known parameters
  - Serializes results to JSON (row count, column shape, actual data)
  - Stores snapshots to disk as baseline
- [ ] Run the harness against the current .NET 6 codebase to capture "before" baselines
- [ ] Priority repositories to verify (have complex query logic):
  - `StaffScheduleRepository`
  - `StudentScheduleRepository`
  - `StudentAttendanceRepository`
  - `AttendanceCodeRepository`
  - All repositories touched by recent perf work
- [ ] The harness should diff before/after structurally — not just row counts but actual data values
- [ ] Integrate into the upgrade workflow: run before upgrade, run after, compare, flag mismatches

### Requirements

- A test tenant DB with representative data (confirm which tenant to use)
- Ability to `dotnet build` and `dotnet run` from CLI
- Test DB must be reachable from dev machine during sessions

---

## Phase 1: Update target frameworks and core packages

All 10 csproj files need updating. Do them all in one pass to avoid cross-project version conflicts.

### Target framework changes

| Project | Current | Target |
|---|---|---|
| opensisAPI | net6.0 | net8.0 |
| opensis.data | net6.0 | net8.0 |
| opensis.core | net6.0 | net8.0 |
| opensis.backgroundjob | net6.0 | net8.0 |
| opensis.catelogdb | net6.0 | net8.0 |
| opensis.report | net6.0 | net8.0 |
| JSReport | net6.0 | net8.0 |
| opensis.dbackup | net6.0 | net8.0 |
| opensis.NunitTest | net6.0 | net8.0 |
| opensis.Test | netcoreapp3.1 | net8.0 |

### Package upgrades

| Package | From | To | Risk | Notes |
|---|---|---|---|---|
| Microsoft.EntityFrameworkCore.* | 6.0.0 | 8.0.x | Medium | Query behavior changes possible |
| Pomelo.EntityFrameworkCore.MySql | 6.0.0 | 8.0.x | Medium | Decimal precision, JSON handling |
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | 6.0.0 | 8.0.x | Low | Same API, just version bump |
| Swashbuckle.AspNetCore | 5.5.1 | 6.5+ | Low | Already uses AddSwaggerGen |
| NLog | 4.7.x | 5.x | Low-medium | Config XML schema changes |
| NLog.Web.AspNetCore | 4.9.3 | 5.x | Low | Follows NLog major version |
| System.IdentityModel.Tokens.Jwt | 6.34.0 | 7.x | Low | Verify TokenManager.cs |
| Microsoft.NET.Test.Sdk | 17.0.0 / 16.2.0 | 17.8+ | Low | Test infra only |
| Microsoft.EntityFrameworkCore.DynamicLinq | 6.2.14 | 8.x | Low | Usually just a version bump |

### What does NOT need to change

- **Startup.cs pattern** — `UseStartup<Startup>()` still works in .NET 8. No need to rewrite to minimal APIs. Only becomes mandatory in .NET 9+.
- **Newtonsoft.Json** — `AddNewtonsoftJson()` with `ReferenceLoopHandling.Ignore` works fine in .NET 8. Migration to System.Text.Json is optional and a separate effort.
- **JWT auth flow** — same APIs, just newer package version.

---

## Phase 2: Fix compilation errors

After updating all csproj files, build and fix errors iteratively.

- [ ] `dotnet build` the solution — capture all errors
- [ ] Fix breaking API changes (expect these to be minor)
- [ ] Common .NET 8 breaking changes to watch for:
  - `ILogger` generic constraints tightened
  - Some `HttpContext` extension methods moved
  - EF Core: `HasPrecision()` may be required for decimals that previously defaulted
  - EF Core: `DateOnly`/`TimeOnly` type mapping changes with Pomelo 8
  - Nullable reference type warnings may increase (already cleaned up in opensis.data)
- [ ] Verify all projects compile cleanly

---

## Phase 3: Verify EF Core migrations

Critical step — EF Core migration compatibility must be confirmed.

- [ ] Verify existing migrations still apply cleanly on .NET 8 + EF 8 + Pomelo 8
- [ ] Run `dotnet ef migrations list` to confirm no issues
- [ ] Test `context.Database.Migrate()` against a test tenant (this is the production auto-migrate path)
- [ ] Check that no migration snapshots need regeneration
- [ ] If Pomelo 8 changes column type mappings, a new migration may be needed — test carefully

---

## Phase 4: Run query verification harness

- [ ] Run the "after" snapshot against the same test DB
- [ ] Compare against "before" baselines from Prerequisites
- [ ] Investigate and resolve any data mismatches
- [ ] Priority: scheduling and attendance repos (most complex queries, recently optimized)

---

## Phase 5: Remove dead jsreport code

jsreport is **completely unused**. All UI calls are commented out. Reports use client-side
HTML rendering + browser print instead. Remove it to simplify the upgrade.

- [ ] Remove the entire `API/JSReport/` project directory
- [ ] Remove JSReport project reference from `API/opensis.data/opensis.data.csproj`
- [ ] Remove dead `GenerateReportCard` instantiation in `ReportCardRepository.cs` (~line 1108)
- [ ] Remove dead `GenerateTranscript` instantiation in `StudentRepository.cs` (~line 4753)
- [ ] Optionally clean up commented-out UI calls:
  - `transcripts.component.ts` (~line 582)
  - `student-report-card.component.ts` (~line 885)
  - `student-transcript.component.ts` (~line 292)
- [ ] Remove dead Angular service methods:
  - `report-card.service.ts` `generateReportCard()` (~line 65)
  - `student-transcript.service.ts` `generateTranscriptForStudent()` (~line 40)
- [ ] Remove dead API endpoints (optional — they're harmless but add confusion):
  - `ReportCardController.cs` `generateReportCard` endpoint (~line 188)
  - `StudentController.cs` `generateTranscriptForStudent` endpoint (~line 656)

---

## Phase 6: End-to-end verification

- [ ] Start API with `dotnet run`, confirm it boots and connects to test tenant
- [ ] Test JWT authentication (login flow)
- [ ] Test a few key API endpoints manually (scheduling, attendance, student)
- [ ] Run background job against test tenant
- [ ] Run backup tool against test tenant
- [ ] Run report generation
- [ ] Verify NLog output is correct (config may need updates for NLog 5)

---

## Phase 7: Deployment updates

- [ ] Update .NET SDK version on build machine (need .NET 8 SDK)
- [ ] Update Ansible deployment:
  - Check runtime install tasks (must install `aspnetcore-runtime-8.0` instead of 6.0)
  - Update systemd service files if they reference a specific runtime
  - Verify publish commands still work: `dotnet publish -c Release`
  - Check if `linux-x64` runtime identifier changes are needed for backgroundjob
- [ ] Update `.vscode/launch.json` and `tasks.json` if they reference specific framework versions
- [ ] Update `CLAUDE.md` tech stack table

---

## Risk Summary

| Risk | Severity | Mitigation |
|---|---|---|
| Silent EF Core query behavior changes | High | Query verification harness (before/after comparison) |
| Pomelo decimal/JSON mapping changes | Medium | Verify migrations + query harness |
| jsreport (dead code) | None | Remove entirely in Phase 5 — not used |
| NLog 5 config breaking changes | Low-medium | Update NLog.config; test log output |
| Ansible/deployment runtime mismatch | Low | Update Ansible before deploying |
| JWT token validation edge cases | Low | Test login flow end-to-end |

---

## Estimated effort

- Prerequisites (version fixes + harness): 1-2 days
- Phase 1-2 (csproj + compilation): 1 day
- Phase 3-4 (migrations + query verification): 1 day
- Phase 5 (remove jsreport dead code): 0.5 day
- Phase 6 (end-to-end): 0.5 day
- Phase 7 (deployment): 0.5 day
- **Total: 4-6 days**
