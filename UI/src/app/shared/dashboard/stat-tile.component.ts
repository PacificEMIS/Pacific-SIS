// Copy of PacificEMIS.UI src/app/dashboard/stat/stat-tile.component.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import { booleanAttribute, ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatTooltip } from '@angular/material/tooltip';

/**
 * A headline figure of a dashboard: what it measures, the figure, and a line or
 * two under it (the element's content).
 *
 * <emis-stat-row>
 *   <emis-stat-tile label="Teachers" [value]="teachers | number">teaching staff, each counted once</emis-stat-tile>
 * </emis-stat-row>
 */
@Component({
  selector: 'emis-stat-tile',
  imports: [MatIcon, MatTooltip],
  template: `
    <p class="label">
      {{ label() }}
      @if (info(); as info) {
        <mat-icon
          class="info"
          [matTooltip]="info"
          matTooltipPosition="above"
          aria-hidden="false"
          [attr.aria-label]="info"
          >info_outline</mat-icon
        >
      }
    </p>
    <p class="value" [class]="tone()" [style.color]="colour()">{{ value() }}</p>
    <p class="caption"><ng-content /></p>
  `,
  styles: `
    :host {
      display: block;
      box-sizing: border-box;
      min-width: 0;
      padding: 10px 12px;
      background: var(--emis-tile-surface);
      border: 1px solid var(--mat-sys-outline-variant);
      border-radius: 8px;
    }

    :host(.accented) {
      border-left-width: 4px;
    }

    // its caption has more to say than the others': twice the room, where there is room
    @media (min-width: 600px) {
      :host(.wide) {
        grid-column: span 2;
      }
    }

    p {
      margin: 0;
    }

    .label,
    .caption {
      font-size: 12px;
      color: var(--mat-sys-on-surface-variant);
    }

    // the other names of the figure, on a tooltip
    .info {
      width: 14px;
      height: 14px;
      margin-left: 2px;
      font-size: 14px;
      vertical-align: -2px;
      cursor: help;
    }

    .value {
      font-size: 26px;
      font-weight: 500;
      line-height: 1.3;
      color: var(--mat-sys-on-surface);
      font-variant-numeric: tabular-nums;

      &.good {
        color: var(--emis-good);
      }

      &.warning {
        // the amber of the rating palette (chart-theme LEVEL_PALETTE)
        color: #f0a000;
      }

      &.bad {
        color: var(--mat-sys-error);
      }
    }
  `,
  host: {
    '[class.accented]': 'accent() !== undefined',
    '[class.wide]': 'wide()',
    '[style.border-left-color]': 'accent()',
  },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatTileComponent {
  readonly label = input.required<string>();
  /** The other names the figure goes by, shown on an info icon beside the label. */
  readonly info = input<string>();
  /** The figure as it is to be shown: format it with the number or percent pipe. */
  readonly value = input.required<string | number | null>();
  /** Says the figure is fine, to be watched, or a problem. */
  readonly tone = input<'good' | 'warning' | 'bad'>();
  /** The colour of the figure where it is that of what it measures on the charts; it overrides the tone. */
  readonly colour = input<string>();
  /** A colour down the left edge: that of the series on the charts the figure belongs to. */
  readonly accent = input<string>();
  /** Twice the width of the others in its row. */
  readonly wide = input(false, { transform: booleanAttribute });
}

/** A row of headline figures across the dashboard's grid, as many to a line as there is room for. */
@Component({
  selector: 'emis-stat-row',
  template: `<ng-content />`,
  styles: `
    :host {
      grid-column: 1 / -1;
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 10px;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatRowComponent {}
