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

Open the repo root in VS Code. Press `F5` → select **API: opensisAPI**.

The API builds, starts with the debugger attached, and auto-migrates the tenant database on first request. Verify at `https://localhost:5001/swagger`.

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

UI will be at `http://localhost:4200`. (`ng serve` is equivalent for development.)

> **Serving on a real hostname for proxy tooling.** To inspect traffic through a browser proxy plugin (e.g. ZeroOmega) routed to mitmproxy / mitmweb, serve on a resolvable hostname instead of `localhost`:
> ```bash
> npm start -- --host lvh.me
> ```
> `lvh.me` resolves to `127.0.0.1`, so the dev server is reachable at `http://lvh.me:4200` while still being local. This avoids the proxy plugin treating `localhost` as a bypass target.

See [UI Commands Reference](#ui-commands-reference) below for the full list of available commands.

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
