// Copy of PacificEMIS.UI src/app/dashboard/tile/dashboard-selection.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import { Injectable, signal } from '@angular/core';

/**
 * Which tile of a dashboard, if any, is expanded to fill the page.
 * Provided by the dashboard page, so the tiles of one dashboard share it.
 */
@Injectable()
export class DashboardSelection {
  private nextId = 0;
  readonly expanded = signal<number | null>(null);

  register(): number {
    return ++this.nextId;
  }

  toggle(id: number): void {
    this.expanded.update((current) => (current === id ? null : id));
  }
}
