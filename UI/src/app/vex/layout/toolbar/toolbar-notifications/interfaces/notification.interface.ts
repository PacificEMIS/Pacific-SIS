import { DateTime } from 'luxon';

export interface Notification {
  id: string;
  icon: string;
  label: string;
  colorClass: string;
  datetime: DateTime;
  read?: boolean;
}
