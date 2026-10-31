// Copy of PacificEMIS.UI src/app/dashboard/chart/chart-theme.ts at its commit 24de41f8. Shared between the two applications: change it there first and mirror it here (AGENTS.md).
import type { ECOption } from 'src/app/shared/echart/echart';

/**
 * The look of every chart in the application, in one place.
 *
 * The categorical palette is ordered so that neighbouring colours stay apart for
 * colour-blind readers; the order is part of the palette, do not shuffle it.
 * A chart with more series than colours repeats them, so keep the legend on.
 */
export const CHART_PALETTE = [
  '#2a78d6', // blue
  '#eb6834', // orange
  '#1baf7a', // aqua
  '#eda100', // yellow
  '#e87ba4', // magenta
  '#008300', // green
  '#4a3aa7', // violet
  '#e34948', // red
];

/**
 * The four levels of a rating (accreditation, exam achievement), worst to best.
 * Not for anything else: these colours say "bad" and "good".
 */
export const LEVEL_PALETTE = ['#d03b3b', '#f0a000', '#8bc34a', '#0c8a2c'];

/**
 * The colours of a scale of so many rating levels, worst to best. Four levels take
 * the level palette as it is; fewer take its ends, more are shaded between its colours.
 */
export function levelColours(count: number): string[] {
  const last = LEVEL_PALETTE.length - 1;
  switch (count) {
    case 0:
      return [];
    case 1:
      return [LEVEL_PALETTE[last]];
    case 2:
      return [LEVEL_PALETTE[0], LEVEL_PALETTE[last]];
    case 3:
      return [LEVEL_PALETTE[0], LEVEL_PALETTE[1], LEVEL_PALETTE[last]];
    case 4:
      return [...LEVEL_PALETTE];
  }
  const channels = (colour: string) => [1, 3, 5].map((at) => parseInt(colour.slice(at, at + 2), 16));
  return Array.from({ length: count }, (_, i) => {
    const position = (i * last) / (count - 1);
    const from = Math.floor(position);
    const to = Math.min(from + 1, last);
    const a = channels(LEVEL_PALETTE[from]);
    const b = channels(LEVEL_PALETTE[to]);
    const mixed = a.map((channel, c) => Math.round(channel + (b[c] - channel) * (position - from)));
    return '#' + mixed.map((channel) => channel.toString(16).padStart(2, '0')).join('');
  });
}

/** One hue, light to dark: for the steps of a quantity that has an order but no "bad" and "good". */
const SEQUENTIAL_RAMP = ['#dcebfa', '#bcd8f5', '#98c2ee', '#6aa6e6', '#4a8fde', '#2a78d6', '#1f62b5', '#174f96', '#0f3a70'];

/**
 * The colours of so many steps of an ordered quantity (levels of attainment, age
 * bands), lowest to highest: spread along the ramp, so the highest is always the darkest.
 */
export function sequentialColours(count: number): string[] {
  const last = SEQUENTIAL_RAMP.length - 1;
  return Array.from(
    { length: count },
    (_, i) => SEQUENTIAL_RAMP[Math.min(last, Math.round(((i + 1) * last) / Math.max(count, 1)))],
  );
}

export const CHART_INK = {
  /** The tile a chart sits on; also the gap between touching marks. */
  surface: '#ffffff',
  text: '#0b0b0b',
  textSecondary: '#52514e',
  textMuted: '#898781',
  gridline: '#e1e0d9',
  /** A mark that is not part of the current selection. */
  dimmed: '#d9d8d2',
  /** A series or slice with no identity (missing code). */
  unknown: '#898781',
};

export const CHART_FONT = 'Roboto, "Helvetica Neue", sans-serif';

/** Thickest a bar is drawn, however much room its slot has. */
export const BAR_MAX_WIDTH = 24;

/**
 * How much of its colour a mark outside the current selection keeps (a row not
 * picked on a dashboard, the other schools when one is picked): faded, still itself.
 */
export const FADED_OPACITY = 0.4;

/**
 * What the option of every chart starts from: the application's font and ink,
 * and a description for screen readers.
 * @param saveButton adds the button that saves the chart as an image
 */
export function chartBase(saveButton = false): ECOption {
  return {
    textStyle: { fontFamily: CHART_FONT, color: CHART_INK.textSecondary },
    aria: { enabled: true },
    ...(saveButton
      ? {
          toolbox: {
            top: 0,
            right: 0,
            itemSize: 14,
            iconStyle: { borderColor: CHART_INK.textMuted },
            feature: { saveAsImage: { title: 'Save as image' } },
          },
        }
      : {}),
  };
}
