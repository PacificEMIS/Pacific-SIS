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
  "encryptionKey": "oPen$!$.b14Ca5898a4e4133b!",
  "dataEncryptionKey": "oPen$!$.b14Ca58!",
  "tenant": "your-tenant-name"
}
```

Set `tenant` to the database name you want to develop against (e.g. `kisis`).

### 4. Run the API

Open the repo root in VS Code. Press `F5` → select **API: opensisAPI**.

The API builds, starts with the debugger attached, and auto-migrates the tenant database on first request. Verify at `https://localhost:5001/swagger`.

### 5. Run the UI

> **Node.js version matters.** Use Node.js v14.21.3. Newer versions have build errors with this codebase. If you use nvm: `nvm use 14.21.3` (or `nvm install 14.21.3` first).

```bash
cd UI
npm install
npm start
```

UI will be at `http://localhost:4200`. (`ng serve` is equivalent for development.)

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

**API:**
```bash
dotnet publish API/opensisAPI/opensisAPI.csproj -c Release
```

**Background job (Linux):**
```bash
dotnet publish API/opensis.backgroundjob/opensis.backgroundjob.csproj -c Release -r linux-x64
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

Managed by Ansible from the `purltek-systems` repository using the `dotnet` role. The role:

1. Rsyncs compiled artifacts to the server (config files explicitly excluded)
2. Writes `appsettings.json`, `assets/config.json`, and `NLog.config` from Jinja2 templates with server-specific values
3. Manages systemd services (one per tenant instance)
4. Apache handles SSL termination and reverse-proxies to the .NET Kestrel port

No manual migration steps during deployment — the API auto-migrates each tenant on startup.

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
