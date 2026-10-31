// Copy of PacificEMIS.UI src/app/dashboard/tile/dashboard-tile.component.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { DashboardSelection } from './dashboard-selection';

/** Height of a tile in pixels for each height step (a chart needs a definite height). */
const HEIGHT_STEP = 150;
const HEIGHT_GAP = 10;

/**
 * One panel of a dashboard: a heading, the filters in force, and a chart or table.
 * It sits on the dashboard's 12 column grid (width), at a fixed height (steps of
 * 150px) or as tall as its content. Its expand button opens it over the whole page, where anything marked
 * tileDetail is shown as well - typically the table behind a chart.
 *
 * <emis-dashboard-tile heading="Schools by State" [filters]="..." [width]="3" [height]="3" #tile>
 *   <mat-select tileOptions ... />          optional controls at the right of the heading
 *   <emis-row-col-chart ... />               the body
 *   <emis-cross-tab tileDetail ... />        shown only when expanded
 * </emis-dashboard-tile>
 */
@Component({
  selector: 'emis-dashboard-tile',
  imports: [MatIconButton, MatIcon],
  templateUrl: './dashboard-tile.component.html',
  styleUrl: './dashboard-tile.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class]': '"width" + width()',
    '[class.min-half]': 'minHalf()',
    '[style.height.px]': 'heightPx()',
    '(document:keydown.escape)': 'expanded() && toggle()',
  },
})
export class DashboardTileComponent {
  private selection = inject(DashboardSelection);
  private id = this.selection.register();

  readonly heading = input.required<string>();
  /** The filters in force, in words. */
  readonly filters = input('');
  /** Columns of the 12 column grid; narrow tiles widen on small screens. */
  readonly width = input(12);
  /** Height in steps of 150px, which a chart needs; null lets the tile take the height of its content (a table). */
  readonly height = input<number | null>(3);
  /** Keep the tile at least half a row wide below 1800px: for content that needs elbow room. */
  readonly minHalf = input(false);

  /** This tile is opened over the whole page. */
  readonly expanded = computed(() => this.selection.expanded() === this.id);
  /** Another tile is expanded. */
  protected receded = computed(() => this.selection.expanded() !== null && !this.expanded());
  protected heightPx = computed(() => {
    const height = this.height();
    return height === null ? null : height * HEIGHT_STEP - HEIGHT_GAP;
  });

  toggle(): void {
    this.selection.toggle(this.id);
  }

  /** Opens the tile over the whole page, if it is not already. */
  expand(): void {
    if (!this.expanded()) {
      this.selection.toggle(this.id);
    }
  }
}
