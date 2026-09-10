# Pacific-SIS

The next generation School Management System for Pacific EMIS.

**Vision** — Provide the ability to every school in this world to operate on-line.
**Goal** — Create a global school management system suitable for any school, in any country.

**License:** [GNU AGPLv3](https://www.gnu.org/licenses/agpl-3.0.en.html)

---

## Architecture

```
Pacific-SIS/
├── API/
│   ├── opensisAPI/            # ASP.NET Core 6 — REST API entry point
│   ├── opensis.core/          # Business logic / service interfaces
│   ├── opensis.data/          # EF Core models, DbContext, repositories, migrations
│   ├── opensis.report/        # Report generation
│   ├── opensis.catelogdb/     # Catalog database (tenant registry)
│   └── opensis.backgroundjob/ # Console app — nightly scheduled jobs
└── UI/
    └── src/                   # Angular 10 frontend
```

**Multi-tenancy:** Each school is a separate MySQL database. The API resolves the tenant from the request subdomain and switches connections dynamically via `MySQLContextFactory`. Migrations run automatically on first connection to each tenant (`context.Database.Migrate()`).

### User roles and administrators

Every login (`user_master`) points at a **membership** (`membership`), which is a per-school row: `membership_id` 1 to 7 are seeded in every school as Super Administrator, School Administrator, Admin Assistant, Teacher, Homeroom Teacher, Parent and Student. Permissions (`role_permission`) hang off the membership, and a staff member's profile per school is recorded on their `staff_school_info` row.

| Role | Scope | How it is granted |
|---|---|---|
| **Super Administrator** | Whole tenant: sees every school, holds every permission | Login's membership is the one flagged `is_superadmin` (id 1); staff row has profile `Super Administrator` and no school attachment rows |
| School Administrator, Admin Assistant, Teacher, Homeroom Teacher | One school per attachment row | Staff > School Info tab; the login's membership follows the home school row |
| Parent, Student | Their own records | Created with the parent or student record |

Super Administrators are managed from **Settings > Administration > Super Administrators** (Super Administrators only). That page can add a new one, promote an existing staff member with a portal login, demote one back to a school profile (which asks for the school and profile to attach them to, since a Super Administrator has none), activate or deactivate, and delete accounts created by mistake. Delete is refused for anyone who has ever signed in (their GUID may sit in `created_by`/`updated_by` audit columns, which have no foreign key and would stop resolving to a name) or who has school attachments or records; deactivate those instead. Nobody can change their own account there, and at least one other active Super Administrator must always remain. Because they have no school attachment, Super Administrators do not appear in the staff list; open their record from that page instead.

The first Super Administrator of a tenant is created at registration (`UserController.InsertInitialDataAtRegistration`). Existing tenant databases need `API/opensis.data/Scripts/add-super-administrators-permission.sql` run once to get the page; new schools receive it from `SubCategory.json` / `RolePermission.json`.

**There is no "tenant administrator" role.** `user_master.is_tenantadmin` is a column inherited from openSIS that nothing in the API, UI or background job reads. Treat it as dead; the Super Administrator membership is the only tenant-wide role.

---

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) **v14.21.3 (LTS)** — use [nvm](https://github.com/nvm-sh/nvm) (Linux/macOS) or [nvm-windows](https://github.com/coreybutler/nvm-windows) to manage versions; newer Node versions have known build errors with this codebase
- Angular CLI 10: `npm install -g @angular/cli@10`
- MySQL 8.x
- VS Code with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

---

## Local Development Setup

### 1. Clone

```bash
git clone <repo-url>
cd Pacific-SIS
```

### 2. Backend — configure credentials

Create `appsettings.Development.json` in the API project. This file is gitignored and overrides `appsettings.json` when `ASPNETCORE_ENVIRONMENT=Development`.

```bash
cp API/opensisAPI/appsettings.json API/opensisAPI/appsettings.Development.json
```

Edit the file with your local MySQL credentials:

```json
{
  "ConnectionStringTemplateCatalogDBMySQL": "server=localhost;database=catalogdb;user=USER;password=PASSWORD",
  "ConnectionStringTemplateMySQL": "server=localhost;database={tenant};user=USER;password=PASSWORD"
}
```

> `{tenant}` is substituted at runtime from the request context. Locally it comes from `UI/src/assets/config.json`.

Do the same for the background job:

```bash
cp API/opensis.backgroundjob/appsettings.json API/opensis.backgroundjob/appsettings.Development.json
```

Set the `DefaultConnection` string to your local database.

### 3. Frontend — configure UI

```bash
cp UI/src/assets/config.template.json UI/src/assets/config.json
```

Edit `config.json`:

```json
{
  "apiURLOpensis": "https://localhost:5001/",
  "apiURL": "https://localhost:5001/",
  "encryptionKey": "your-encryption-key",
  "dataEncryptionKey": "your-data-encryption-key",
  "tenant": "your-tenant-name"
}
```

Set `tenant` to the database name you want to develop against (e.g. `kisis`).

### 4. Run the API

```bash
dotnet run --project API/opensisAPI/opensisAPI.csproj
```

`launchSettings.json` already supplies `ASPNETCORE_ENVIRONMENT=Development` and binds `http://lvh.me:5000`, so no environment variables are needed. Verify at `http://lvh.me:5000/swagger`. The API auto-migrates the tenant database on first request.

That profile opens a browser tab on start. To suppress it, bypass the profile and set both values yourself:

```bash
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://lvh.me:5000 \
  dotnet run --no-launch-profile --project API/opensisAPI/opensisAPI.csproj
```

**Windows (PowerShell):**
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://lvh.me:5000"
dotnet run --no-launch-profile --project API/opensisAPI/opensisAPI.csproj
```

**VS Code debugger (when you need breakpoints):** press `F5` → **API: opensisAPI**. Read the lock warning below before mixing this with terminal builds.

> **The API does not auto-rebuild.** After editing any C#, stop the process (`Ctrl+C`) and re-run it. `dotnet watch` is not used here: under the .NET 10 SDK it prints `⌚ Waiting for changes`, never binds the port, and gives no error — so plain `dotnet run` is the dependable option.

> **Never build while the VS Code debugger is attached.** The debug adapter holds open file handles on `API/opensisAPI/bin/Debug/net6.0/*.dll`. A terminal `dotnet build` then fails only its *copy* step with `MSB3021` / `MSB3027` — "the file is locked by: Visual Studio Debug Adapter for .NET" — while still reporting **compilation as successful**. The running API keeps executing the previous assembly, so your change appears to do nothing and you debug a version of the code that is no longer on disk. Stop the debug session before building.
>
> When a change seems to have no effect, verify which build is actually loaded — the DLL must be newer than the source:
> ```powershell
> Get-Item API\opensis.data\Repository\StudentScheduleRepository.cs,
>          API\opensisAPI\bin\Debug\net6.0\opensis.data.dll |
>   Select-Object LastWriteTime, Name
> ```

### 5. Run the background job

The background job is a standalone console app that runs nightly scheduled tasks (drop dates, enrollment updates, missing attendance). Unlike the API, it reads `DOTNET_ENVIRONMENT` (not `ASPNETCORE_ENVIRONMENT`) to locate `appsettings.Development.json`.

```bash
DOTNET_ENVIRONMENT=Development dotnet run --project API/opensis.backgroundjob/opensis.backgroundjob.csproj
```

**Windows (PowerShell):**
```powershell
$env:DOTNET_ENVIRONMENT = "Development"
dotnet run --project API/opensis.backgroundjob/opensis.backgroundjob.csproj
```

> Without `DOTNET_ENVIRONMENT=Development`, the job reads only `appsettings.json` (which has placeholder credentials) and fails to connect to MySQL.

### 6. Run the UI

> **Node.js version matters.** Use Node.js v14.21.3. Newer versions have build errors with this codebase. If you use nvm: `nvm use 14.21.3` (or `nvm install 14.21.3` first).

```bash
cd UI
npm install
npm start
```

UI will be at `http://localhost:4200`. (`ng serve` is equivalent for development.) The dev server watches and rebuilds on save, so unlike the API it needs no restart after edits.

> **Serving on a real hostname for proxy tooling.** To inspect traffic through a browser proxy plugin (e.g. ZeroOmega) routed to mitmproxy / mitmweb, serve on a resolvable hostname instead of `localhost`. `lvh.me` resolves to `127.0.0.1`, so the dev server stays local while the proxy plugin no longer treats it as a bypass target:
> ```bash
> npm start -- --host 0.0.0.0 --disable-host-check
> ```
> Both `http://localhost:4200` and `http://lvh.me:4200` then work.
>
> Without `--disable-host-check`, any hostname other than `localhost` is rejected by the dev server's host check. The failure is easy to misread: the page shows **`Invalid Host header`**, and the server returns that text with **HTTP status 200**, so a status-code-only check (`curl -o /dev/null -w "%{http_code}"`) looks like success. Check the response *body*.
>
> `--host lvh.me` alone also works, but then `localhost:4200` starts failing the same check instead. Binding `0.0.0.0` with the check disabled keeps both usable.

> **`sessionStorage` is per-origin, so `localhost:4200` and `lvh.me:4200` hold separate logins.** Switching between them bounces you to the login screen. Pick one hostname and stay on it. The tenant itself resolves identically for both — `default-values.service.ts` treats any URL containing `localhost` or `lvh.me` as local and reads the tenant from `assets/config.json`; only a real deployed hostname is parsed for a subdomain.

> **Stopping the dev server can orphan the node child.** `npm start` spawns node as a child, and killing the wrapper (or a task runner killing it for you) may leave that child holding the port. You then get a *second* server on the next start, and because Windows routes `127.0.0.1` to the most specific binding, requests can keep hitting the stale one — new flags and code silently ignored. Check for duplicates before debugging anything else:
> ```bash
> netstat -ano | grep -E ":4200\s+.*LISTENING"
> ```
> More than one line means an orphan; kill the older PID.

See [UI Commands Reference](#ui-commands-reference) below for the full list of available commands.

### 7. What runs where

Two long-running terminals is the whole setup. The background job is run on demand, not kept up.

| Process | Command | Reachable at |
|---|---|---|
| API | `dotnet run --project API/opensisAPI/opensisAPI.csproj` | `http://lvh.me:5000` — Swagger at `/swagger` |
| UI | `cd UI && npm start` | `http://localhost:4200` |
| Background job | `DOTNET_ENVIRONMENT=Development dotnet run --project API/opensis.backgroundjob/opensis.backgroundjob.csproj` | one-shot, prints `process completed.` and exits |

Quick liveness checks that distinguish "up" from "bound but broken":

```bash
curl -s -o /dev/null -w "api %{http_code}\n" http://lvh.me:5000/swagger/index.html
curl -s http://localhost:4200/ | head -c 40      # must be HTML, not "Invalid Host header"
```

> The API writes nothing useful to stdout — NLog takes over console logging, so you will **not** see the usual `Now listening on: ...` line. An empty terminal does not mean it failed to start; confirm with the `curl` above or `netstat -ano | grep -E ":5000\s+.*LISTENING"`.

### 8. Logs and diagnostics

NLog targets are configured in `API/opensisAPI/NLog.config` and write to `c:\temp\opensisLogs\`:

| File | Contents |
|---|---|
| `nlog-own-<date>.log` | application logs, enriched with request URL and action — start here |
| `nlog-all-<date>.log` | everything, including framework and EF logs |
| `internal-nlog.txt` | NLog's own startup diagnostics (use if logging itself seems broken) |

Tail the application log while reproducing a problem:

```bash
tail -f "/c/temp/opensisLogs/nlog-own-$(date +%Y-%m-%d).log"
```

> **Repository classes swallow exceptions.** Most methods in `opensis.data/Repository` catch `Exception` and return the text in the response's `_message` field instead of rethrowing, so a failure often leaves **no stack trace anywhere** unless the controller logs it explicitly. When a page reports a vague failure:
> 1. Open the browser DevTools **Network** tab and read `_message` in the JSON response — it usually holds the real exception text.
> 2. Check `nlog-own-<date>.log` for a matching `ERROR` line.
>
> If neither has anything, the controller for that endpoint has no logging yet — worth adding, following the pattern in `StudentScheduleController.AddStudentCourseSectionSchedule`.

---

## EF Migrations

Migrations apply automatically in production (auto-migrate on first tenant connection). To generate a new migration locally:

### 1. Set the migration connection environment variable

**Windows (PowerShell, current session):**
```powershell
$env:OPENSIIS_MIGRATION_CONNSTR = "server=localhost;database=kisis;user=USER;password=PASSWORD"
```

**Linux/macOS:**
```bash
export OPENSIIS_MIGRATION_CONNSTR="server=localhost;database=kisis;user=USER;password=PASSWORD"
```

> Change `database=kisis` to target a different tenant when needed. No code changes required.

### 2. Add the migration

```bash
dotnet ef migrations add YourMigrationName \
  --project API/opensis.data \
  --startup-project API/opensisAPI \
  --context CRMContextMySQL \
  --output-dir Migrations/MySqlMigrations
```

---

## Building for Production

All commands run from the repo root. The `-o` flag outputs directly to the path Ansible expects (avoids the default `publish/` subdirectory).

**API (framework-dependent — requires `aspnetcore-runtime-6.0` on server):**
```bash
dotnet publish API/opensisAPI/opensisAPI.csproj -c Release \
  -o API/opensisAPI/bin/Release/net6.0/
```

**Background job (self-contained Linux binary):**
```bash
dotnet publish API/opensis.backgroundjob/opensis.backgroundjob.csproj -c Release -r linux-x64 \
  -o API/opensis.backgroundjob/bin/Release/net6.0/linux-x64/
```

**UI:**
```bash
cd UI && npm run build
```

> Always use `npm run build`, not `ng build --prod` directly. The npm script sets a 6 GB Node memory limit (`--max_old_space_size=6144`) that prevents heap out-of-memory failures during production builds on this codebase.

---

## UI Commands Reference

All commands run from the `UI/` directory.

| Command | Description |
|---|---|
| `npm start` | Dev server at `http://localhost:4200` with live reload |
| `npm start -- --host lvh.me` | Dev server bound to `lvh.me` (resolves to `127.0.0.1`) for use with browser proxy plugins / mitmproxy |
| `npm run build` | Production build — output to `dist/vex/` |
| `npm test` | Unit tests via Karma |
| `npm run lint` | TypeScript linting via TSLint |
| `npm run e2e` | End-to-end tests via Protractor |

### `ng` directly vs `npm run`

`ng serve`, `ng test`, `ng lint`, and `ng e2e` work fine as direct `ng` commands. The only exception is production builds:

```bash
# Correct — wraps ng with a 6 GB Node heap limit
npm run build

# Avoid — may crash with out-of-memory on this codebase
ng build --prod
```

### Scaffolding with `ng`

```bash
ng g component path/to/component-name   # new component
ng g service   path/to/service-name     # new service
ng g module    path/to/module-name      # new module
```

---

## Deployment

### 1. Sync builds to the Ansible control node

After building for production (see above), rsync the three build outputs to your Ansible control node:

```
API/opensisAPI/bin/Release/net6.0/                          → API artifact
API/opensis.backgroundjob/bin/Release/net6.0/linux-x64/     → Background job artifact
UI/dist/vex/                                                 → Frontend artifact
```

Use `rsync -avz --delete` to keep the target in sync and remove stale files. Wrap in a shell script for convenience.

### 2. Run Ansible

From the Ansible control node, run the appropriate playbook from the `purltek-systems` repo. Ansible:

1. Rsyncs compiled artifacts to target servers (config files explicitly excluded)
2. Writes `appsettings.json`, `assets/config.json`, and `NLog.config` from Jinja2 templates with server-specific values
3. Manages systemd services (one per tenant instance)
4. Apache handles SSL termination and reverse-proxies to the .NET Kestrel port

No manual migration steps during deployment — the API auto-migrates each tenant on startup.

---

## Commit Conventions

Format: `type(scope): brief description` — lowercase, imperative, no period at end.

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

**Scope** = the affected feature area or layer, e.g. `school`, `student`, `attendance`, `grades`, `login`, `auth`, `tenant`, `migration`, `api`, `ui`, `data`, `backgroundjob`, `configuration`.

If the commit resolves a GitHub issue, add `Resolves #<number>` in the commit body (separated from the subject by a blank line).

```
feat(attendance): add bulk-mark present for whole class

Resolves #42

fix(login): restore tenant after session clear

perf(school): replace N+1 queries with single JOIN

Resolves #608
```

---

## Config File Reference

| File | Committed | Purpose |
|------|:---------:|---------|
| `API/*/appsettings.json` | Yes | Structure + non-secret defaults (log levels, dbtype) |
| `API/*/appsettings.Development.json` | **No** | Local credentials — gitignored, never commit |
| `UI/src/assets/config.template.json` | Yes | Template showing all required UI config keys |
| `UI/src/assets/config.json` | **No** | Local API URL + tenant — gitignored, never commit |
| `.vscode/launch.json` | Yes | VS Code debug launcher for API projects |
| `.vscode/tasks.json` | Yes | VS Code build tasks |
