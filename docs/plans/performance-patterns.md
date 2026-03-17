# Performance Patterns — Standing Plan

## Context

Database access patterns are the primary source of system-wide CPU spikes, causing near-
unresponsiveness even with very few users. Performance is a first-class concern — not an
afterthought. Every time repository or service code is touched, check the items below.

---

## CRITICAL RULE

Before committing any query optimization, run both the old and new query against a real
tenant DB and compare output (row counts, shape, values). Never assume a rewrite is
equivalent — verify it.

---

## Checklist

### EF Core / Repository Layer

- [ ] **N+1 queries** — loading related data inside a loop instead of using `.Include()` or a JOIN
- [ ] **Missing `.AsNoTracking()`** — all read-only queries (lists, reports, lookups) should use it
- [ ] **Unbounded queries** — no `.Take()` or pagination on queries that could return large sets
- [ ] **Repeated DB round-trips** — multiple queries where a single JOIN or subquery would do
- [ ] **`.ToList()` before filtering** — materializing a large set in memory then filtering in C#
- [ ] **`Select *` instead of projection** — loading full entities when only a few columns are needed
- [ ] **Missing indexes** — frequently filtered or joined columns without an index (check via EXPLAIN)

### Async / Threading

- [ ] **Synchronous blocking on async** — `.Result`, `.Wait()`, or `GetAwaiter().GetResult()` on async calls
- [ ] **`async void`** — fire-and-forget methods that swallow exceptions; use `async Task` instead
- [ ] **Unnecessary `await` in a passthrough** — `return await Foo()` where `return Foo()` suffices

### General

- [ ] **Unbounded background jobs** — nightly jobs that process all tenants in sequence without limits
- [ ] **Logging in hot paths** — verbose log statements inside tight loops or high-frequency calls
- [ ] **Missing caching** — reference data (e.g. school year, grading periods) fetched per-request

---

## Index Review Process

When adding a query that filters or joins on a column not currently indexed:

1. Run `EXPLAIN SELECT ...` against a real tenant DB
2. Check for `type: ALL` (full table scan) on large tables
3. If found, add an EF migration to create the index
4. Re-run EXPLAIN to confirm index is used

---

## Running Log

| Date | File / Area | Issue Found | Fix Applied |
|------|-------------|-------------|-------------|
| —    | —           | —           | —           |
