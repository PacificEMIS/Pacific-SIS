// Copy of PacificEMIS.UI src/app/dashboard/chart/chart.component.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { EchartDirective } from 'src/app/shared/echart/echart.directive';
import type { ECOption } from 'src/app/shared/echart/echart';

/**
 * A chart of a dashboard tile drawn from an ECharts option of the dashboard's own
 * making, for the charts that are not a table of rows and columns (a population
 * pyramid, say). It fills the tile body. Build the option with the colours and
 * inks of chart-theme, so it looks like the other charts.
 */
@Component({
  selector: 'emis-chart',
  imports: [EchartDirective],
  template: `
    <div class="frame">
      <div class="chart" [emisEchart]="option()" (chartClick)="itemClick.emit($event)"></div>
    </div>
  `,
  styles: `
    :host {
      display: block;
      position: relative;
      // fills a tile body, which is a flex column; a percentage height would resolve to nothing there
      flex: 1 1 auto;
      min-height: 0;
      overflow: hidden;
    }

    // out of the flow, so the canvas can never push the host (see emis-row-col-chart)
    .frame {
      position: absolute;
      inset: 0;
    }

    .chart {
      width: 100%;
      height: 100%;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChartComponent {
  readonly option = input.required<ECOption>();
  /** A chart item was clicked: the event parameters ECharts supplies. */
  readonly itemClick = output<unknown>();
}
