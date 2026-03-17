# SPA Anti-Patterns — Standing Plan

## Approach

This is an **opportunistic** plan. Do not sweep the codebase proactively. Instead, when
touching any Angular component, service, or module, check relevant items from this list and
fix what you find in the code you are already reading. Log fixes below as they accumulate.

---

## Checklist

### Navigation & Routing

- [ ] Route changes that trigger a full page reload (defeats SPA purpose)
- [ ] State changes that do not update the URL (breaks back/forward and deep links)
- [ ] Single routes overloaded with too much conditional logic — consider child routes

### Data Fetching

- [ ] Repeated API calls on every route visit for data that could be cached
- [ ] Sequential (waterfall) API calls where parallel requests would work
- [ ] Large upfront data loads at app startup that could be lazy-loaded

### State Management

- [ ] Prop drilling across many component layers — consider a shared service or store
- [ ] Local component copies of server data that can go stale (prefer reactive streams)
- [ ] Ephemeral UI state (open/closed, hover, etc.) pushed into a global store unnecessarily

### Performance

- [ ] No code-splitting / lazy-loaded modules for feature areas
- [ ] Missing `trackBy` in `*ngFor` loops — causes full DOM re-renders on list updates
- [ ] Memory leaks from unsubscribed Observables — check `ngOnDestroy` for cleanup
- [ ] Heavy computation in template expressions that re-run on every change detection cycle

### Angular-Specific

- [ ] Subscriptions not cleaned up in `ngOnDestroy` (use `takeUntil`, `async` pipe, or explicit `unsubscribe`)
- [ ] Logic in templates that belongs in the component class
- [ ] Eagerly imported feature modules that could be lazy-loaded
- [ ] `any` types in TypeScript that hide real type errors

---

## Angular Version Note

This project uses Angular 10 (EOL). Some modern patterns (standalone components, `inject()`,
signals) are not available. Solutions must be compatible with Angular 10 / RxJS 6.

---

## Running Log

| Date | File / Area | Issue Found | Fix Applied |
|------|-------------|-------------|-------------|
| —    | —           | —           | —           |
