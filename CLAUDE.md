# CLAUDE.md — Pacific-SIS Context

This file gives Claude persistent context about this project across sessions.

---

## What This Is

Pacific-SIS is a multi-tenant Student Information System for Pacific EMIS — a regional education authority managing schools across multiple Pacific Island nations (FSM, Kiribati, Marshall Islands, Solomon Islands, Vanuatu, etc.).

Each country/school is a separate **tenant** with its own MySQL database. The system is operated by Ghislain Hachey, who took over development from a third-party team. We are progressively improving code quality and taking full ownership.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | .NET 6 / C# / ASP.NET Core |
| ORM | Entity Framework Core 6 |
| Database | MySQL 8 (Pomelo provider) |
| Background jobs | .NET 6 console app (scheduled via cron) |
| Frontend | Angular 10 / TypeScript |
| UI framework | Angular Material + Vex theme + Tailwind CSS |
| Auth | JWT |
| Reporting | JSReport |
| Logging | NLog |
| Deployment | Ansible (`purltek-systems` repo) |

---

## Repository Structure

```
Pacific-SIS/
├── API/
│   ├── opensisAPI/            # Controllers — HTTP entry point
│   ├── opensis.core/          # Service interfaces + implementations
│   ├── opensis.data/          # EF Core: DbContext, models, repos, migrations
│   ├── opensis.report/        # Report generation logic
│   ├── opensis.catelogdb/     # Catalog DB — tenant registry
│   └── opensis.backgroundjob/ # Nightly job runner (console app / exe)
├── UI/
│   └── src/app/               # Angular modules, services, components, models
├── .vscode/
│   ├── launch.json            # F5 debug launcher for both API projects
│   └── tasks.json             # Build tasks
├── README.md                  # Developer setup guide
└── CLAUDE.md                  # This file
```

---

## Multi-Tenancy — How It Works

1. Request arrives → `TenantDBMapper` middleware extracts tenant name from subdomain
2. `MySQLContextFactory.Create()` builds a connection string from the template + tenant name
3. It calls `context.Database.Migrate()` — auto-applies any pending EF migrations
4. All queries run against that tenant's isolated database

**The connection string template** (from `appsettings.json`) uses `{tenant}` as a placeholder:
```
server=HOST;database={tenant};user=USER;password=PASSWORD
```

**Catalog DB** (`opensis.catelogdb`) is a separate shared database that holds the tenant registry — which tenants exist and are active.

---

## Key Files

| File | Purpose |
|---|---|
| `API/opensisAPI/Startup.cs` | DI registration, middleware pipeline, auth |
| `API/opensisAPI/appsettings.json` | Committed config with placeholders |
| `API/opensisAPI/appsettings.Development.json` | Gitignored — local credentials |
| `API/opensis.data/Factory/MySQLContextFactory.cs` | Tenant DB switching + auto-migrate |
| `API/opensis.data/Models/CRMContextMySQL.cs` | MySQL DbContext (OnConfiguring for EF tooling) |
| `API/opensis.data/Models/CRMContext.cs` | Base DbContext with all entity DbSets |
| `UI/src/environments/environment.ts` | Loads `assets/config.json` synchronously at startup |
| `UI/src/assets/config.json` | Gitignored — local API URL, encryption keys, tenant |
| `UI/src/assets/config.template.json` | Committed template — copy to config.json to start |
| `UI/src/app/common/default-values.service.ts` | Central state service — tenant, school, token, etc. |

---

## Local Dev Setup (Quick Reference)

```bash
# Backend
cp API/opensisAPI/appsettings.json API/opensisAPI/appsettings.Development.json
# Edit: fill in real MySQL host/user/password

cp API/opensis.backgroundjob/appsettings.json API/opensis.backgroundjob/appsettings.Development.json
# Edit: fill in DefaultConnection

# Frontend
cp UI/src/assets/config.template.json UI/src/assets/config.json
# Edit: set tenant name and confirm API URL

# Run API: VS Code F5 → "API: opensisAPI"
# Run UI (dev):  cd UI && npm install && npm start    (ng serve is equivalent)
# Build UI (prod): cd UI && npm run build             (NOT ng build --prod — needs 6GB memory flag)
```

---

## EF Migrations

- **Production:** fully automatic — `MySQLContextFactory` calls `context.Database.Migrate()` on each tenant connection
- **Local (adding a migration):** requires `OPENSIIS_MIGRATION_CONNSTR` env var set to a real connection string, then run `dotnet ef migrations add`
- **Never edit `CRMContextMySQL.cs` to add credentials** — use the env var

```bash
# PowerShell example
$env:OPENSIIS_MIGRATION_CONNSTR = "server=localhost;database=kisis;user=opensisadmin;password=..."

dotnet ef migrations add MigrationName \
  --project API/opensis.data \
  --startup-project API/opensisAPI \
  --context CRMContextMySQL \
  --output-dir Migrations/MySqlMigrations
```

---

## Deployment Pipeline

Managed from `C:\Users\Ghislain Hachey\Development\Purltek\purltek-systems` (Ansible).

- Role: `roles/dotnet/`
- Rsyncs **compiled artifacts** (not source) to servers — config files excluded
- Writes `appsettings.json`, `assets/config.json`, `NLog.config` from Jinja2 templates
- systemd manages one service per tenant instance
- Apache reverse-proxies HTTPS → Kestrel localhost port

**Key Ansible deployment files:**
- `roles/dotnet/tasks/main.yml` — deployment orchestration
- `roles/dotnet/templates/appsettings.json.j2` — API config template
- `roles/dotnet/templates/assets-config.json.j2` — UI config template
- `purltek/ironhide1.purltek.com-deployment.yml` — main Pacific-SIS deployment

---

## Development Environment

- **OS:** Windows 10, targeting Linux for production and eventual dev migration
- **IDE:** VS Code for both frontend and backend (C# Dev Kit installed)
- **Backend debug:** F5 in VS Code using `.vscode/launch.json`
- **Git:** User handles all commits/PRs — never auto-commit
- **Deployment repo:** `C:\Users\Ghislain Hachey\Development\Purltek\purltek-systems`

---

## Working Conventions

- When the user says "wrap up", propose a commit message following the Commit Conventions below.
- User reviews all code before committing — do not auto-commit or auto-push
- Tackle issues one at a time, starting small and growing in complexity
- Always check the Ansible deployment repo before changing config file structure
- No emojis in code or docs unless explicitly asked
- Keep changes minimal and focused — no speculative improvements

---

## Commit Conventions

Format: `type(scope): brief description` — lowercase, imperative, no period.

| Type | Use for |
|---|---|
| `feat` | new feature |
| `fix` | bug fix |
| `perf` | performance improvement |
| `refactor` | code restructuring (no behaviour change) |
| `docs` | documentation only |
| `style` | formatting, whitespace (no logic change) |
| `test` | adding or fixing tests |
| `chore` | build, tooling, config, dependencies |
| `ci` | CI/CD pipeline changes |

Scope = the affected feature area or layer, e.g. `school`, `student`, `attendance`, `login`, `auth`, `tenant`, `migration`, `api`, `ui`, `data`, `backgroundjob`, `configuration`.

If the commit resolves a GitHub issue, add `Resolves #<number>` in the commit body (blank line after subject).

Examples:
```
feat(attendance): add bulk-mark present for whole class

Resolves #42

fix(login): restore tenant after session clear

perf(school): replace N+1 queries with single JOIN

Resolves #608
```

---

## UI Node.js Version Requirement

The Angular 10 frontend requires **Node.js v14.21.3**. Newer Node versions produce build errors. Use nvm to pin the version when working on the UI.

---

## Known Technical Debt

- Angular 10 and .NET 6 are both EOL — upgrade is a future goal
- 132 build warnings (mostly nullable reference warnings in `opensis.data`)
- `System.IdentityModel.Tokens.Jwt` 6.14.1 has a known moderate vulnerability
- No automated tests running in CI
- Encryption keys are app-wide constants (same across all deployments)
