// Copy of PacificEMIS.UI src/app/shared/echart/echart.directive.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import {
  afterNextRender,
  DestroyRef,
  Directive,
  effect,
  ElementRef,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { echarts, ECOption } from './echart';

/**
 * Renders an ECharts chart in the host element, which must have a height:
 * <div [emisEchart]="options()" class="chart"></div>
 * The chart follows the size of its element.
 */
@Directive({
  selector: '[emisEchart]',
})
export class EchartDirective {
  readonly options = input.required<ECOption>({ alias: 'emisEchart' });
  /** A click on a chart item, with the event parameters ECharts supplies. */
  readonly chartClick = output<unknown>();

  private host = inject<ElementRef<HTMLElement>>(ElementRef);
  private chart = signal<echarts.EChartsType | undefined>(undefined);

  constructor() {
    const destroyRef = inject(DestroyRef);

    afterNextRender(() => {
      const chart = echarts.init(this.host.nativeElement);
      chart.on('click', (params) => this.chartClick.emit(params));
      // resize on the next frame, once per frame: resizing inside the observer's
      // own callback can change the layout it is observing
      let frame = 0;
      const resizeObserver = new ResizeObserver(() => {
        cancelAnimationFrame(frame);
        frame = requestAnimationFrame(() => chart.resize());
      });
      resizeObserver.observe(this.host.nativeElement);
      destroyRef.onDestroy(() => {
        cancelAnimationFrame(frame);
        resizeObserver.disconnect();
        chart.dispose();
      });
      this.chart.set(chart);
    });

    effect(() => {
      this.chart()?.setOption(this.options(), true);
    });
  }
}
