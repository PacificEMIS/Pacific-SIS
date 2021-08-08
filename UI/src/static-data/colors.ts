export interface Color {
  label: string;
  backgroundColor: string;
  backgroundContrastColor: 'text-contrast-black' | 'text-contrast-white';
  textColor: string;
}

export const colors: Color[] = [
  {
    label: 'red',
    backgroundColor: 'bg-red',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-red'
  },
  {
    label: 'green',
    backgroundColor: 'bg-green',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-green'
  },
  {
    label: 'amber',
    backgroundColor: 'bg-amber',
    backgroundContrastColor: 'text-contrast-black',
    textColor: 'text-amber'
  },
  {
    label: 'orange',
    backgroundColor: 'bg-orange',
    backgroundContrastColor: 'text-contrast-black',
    textColor: 'text-orange'
  },
  {
    label: 'deep-orange',
    backgroundColor: 'bg-deep-orange',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-deep-orange'
  },
  {
    label: 'purple',
    backgroundColor: 'bg-purple',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-purple'
  },
  {
    label: 'deep-purple',
    backgroundColor: 'bg-deep-purple',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-deep-purple'
  },
  {
    label: 'cyan',
    backgroundColor: 'bg-cyan',
    backgroundContrastColor: 'text-contrast-black',
    textColor: 'text-cyan'
  },
  {
    label: 'teal',
    backgroundColor: 'bg-teal',
    backgroundContrastColor: 'text-contrast-white',
    textColor: 'text-teal'
  },
  {
    label: 'gray',
    backgroundColor: 'bg-gray',
    backgroundContrastColor: 'text-contrast-black',
    textColor: 'text-gray'
  }
];
