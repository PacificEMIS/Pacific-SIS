// Copy of PacificEMIS.UI src/app/shared/echart/echart.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import * as echarts from 'echarts/core';
import {
  BarChart,
  BarSeriesOption,
  LineChart,
  LineSeriesOption,
  PieChart,
  PieSeriesOption,
  ScatterChart,
  ScatterSeriesOption,
} from 'echarts/charts';
import {
  AriaComponent,
  AriaComponentOption,
  DatasetComponent,
  DatasetComponentOption,
  DataZoomComponent,
  DataZoomComponentOption,
  GridComponent,
  GridComponentOption,
  LegendComponent,
  LegendComponentOption,
  MarkLineComponent,
  MarkLineComponentOption,
  TitleComponent,
  TitleComponentOption,
  ToolboxComponent,
  ToolboxComponentOption,
  TooltipComponent,
  TooltipComponentOption,
  TransformComponent,
  VisualMapComponent,
  VisualMapComponentOption,
} from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';

// Only the chart types and components registered here are bundled; add to the list as charts need them
echarts.use([
  BarChart,
  LineChart,
  PieChart,
  ScatterChart,
  AriaComponent,
  DatasetComponent,
  DataZoomComponent,
  GridComponent,
  LegendComponent,
  MarkLineComponent,
  TitleComponent,
  ToolboxComponent,
  TooltipComponent,
  TransformComponent,
  VisualMapComponent,
  CanvasRenderer,
]);

export type ECOption = echarts.ComposeOption<
  | BarSeriesOption
  | LineSeriesOption
  | PieSeriesOption
  | ScatterSeriesOption
  | AriaComponentOption
  | DatasetComponentOption
  | DataZoomComponentOption
  | GridComponentOption
  | LegendComponentOption
  | MarkLineComponentOption
  | TitleComponentOption
  | ToolboxComponentOption
  | TooltipComponentOption
  | VisualMapComponentOption
>;

export { echarts };
