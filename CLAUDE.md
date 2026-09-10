# CLAUDE.md — Pacific-SIS Context

This file gives Claude persistent context about this project across sessions.

---

## What This Is

Pacific-SIS is a multi-tenant Student Information System for Pacific EMIS. This Student Information System can be used by anyone (i.e. a country, a school, etc.)

Each country/school is a separate **tenant** with its own MySQL database. The system is operated by volunteers under the Pacific EMIS project.

---

## Tech Stack

To be upgraded soon.

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
| Deployment | Ansible |

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

1. Request arrives → `TenantDBMappingMiddleware` takes the tenant name from the **first path segment**, not the subdomain (`Request.Path.Value.Split('/')` → `urlParts[0]`). Controller routes are declared as `[Route("{tenant}/Controller")]`, so `POST /fedsis/StudentSchedule/...` selects tenant `fedsis`.
2. `MySQLContextFactory.Create()` builds a connection string from the template + tenant name
3. It calls `context.Database.Migrate()` — auto-applies any pending EF migrations
4. All queries run against that tenant's isolated database

On the UI side the tenant comes from `assets/config.json` whenever the browser URL contains `localhost` or `lvh.me`; only a real deployed hostname is parsed for a subdomain (`default-values.service.ts` → `setDefaultTenant()`). So `lvh.me` in local dev is just a convenient alias for `127.0.0.1` — it plays no part in tenant resolution.

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
# Run background job (requires DOTNET_ENVIRONMENT — not ASPNETCORE_ENVIRONMENT):
DOTNET_ENVIRONMENT=Development dotnet run --project API/opensis.backgroundjob/opensis.backgroundjob.csproj
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
$env:OPENSIIS_MIGRATION_CONNSTR = "server=localhost;database=mysis;user=myadminuser;password=..."

dotnet ef migrations add MigrationName \
  --project API/opensis.data \
  --startup-project API/opensisAPI \
  --context CRMContextMySQL \
  --output-dir Migrations/MySqlMigrations
```

---

## Publish & Deploy

When the user says "publish":
1. Run the three build commands (see README § Building for Production)
2. Tell the user to run their sync script from WSL to rsync artifacts to the Ansible control node

Deployment is managed via Ansible (see local MEMORY for details).

- Rsyncs **compiled artifacts** (not source) to servers — config files excluded
- Writes `appsettings.json`, `assets/config.json`, `NLog.config` from Jinja2 templates
- systemd manages one service per tenant instance
- Apache reverse-proxies HTTPS → Kestrel localhost port

---

## Development Environment

- **OS:** Windows 10/11, targeting Linux for production and eventual dev migration
- **Preferred workflow:** terminal-first — run the API and UI as two long-running terminal processes, not through the IDE. See README § *Local Development Setup* steps 4-8.
- **Editor:** VS Code (C# Dev Kit installed), used as an editor rather than a host
- **Backend debug:** `F5` → **API: opensisAPI** is available when breakpoints are genuinely needed. Do not build from a terminal while it is attached — the debug adapter locks the output DLLs, the copy step fails while compilation still reports success, and the running API keeps executing the old assembly. Details and the timestamp check are in the README.
- **The API does not auto-rebuild.** After a C# edit, stop and restart it. `dotnet watch` does not stay up under the .NET 10 SDK here.
- **Git:** User handles all commits/PRs
- **Deployment:** ansible (see MEMORY for local path)

---

## Working Conventions

- Always check the Ansible deployment repo before changing config file structure
- See `~/.claude/CLAUDE.md` for global conventions (commit format, "wrap up" keyword, etc.)
- Design notes and plans are **not** kept in this repo (they pick up real data). They live in
  the private Drive folder `repositories-data/Pacific-SIS/docs/plans/`. Code comments that
  cite `docs/plans/<name>.md` refer to that folder. Never recreate `docs/plans` here.

---

## UI Node.js Version Requirement

The Angular 10 frontend requires **Node.js v14.21.3**. Newer Node versions produce build errors. Use nvm to pin the version when working on the UI.

---

## Known Technical Debt

- Angular 10 and .NET 6 are both EOL — upgrade is a near future goal
- 132 build warnings (mostly nullable reference warnings in `opensis.data`)
- `System.IdentityModel.Tokens.Jwt` 6.14.1 has a known moderate vulnerability
- No automated tests running in CI
- Encryption keys are app-wide constants (same across all deployments)
