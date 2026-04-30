export interface SchedulePrintEntry {
  date: Date;
  key: string;
  periodId: number;
  periodName: string;
  roomName: string;
  courseName: string;
  courseSectionName: string;
  staffName?: string;
}

export interface SchedulePrintCell {
  entries: SchedulePrintEntry[];
}

export interface SchedulePrintRow {
  periodName: string;
  cells: SchedulePrintCell[];
}

export interface SchedulePrintWeekDay {
  date: Date;
  dayName: string;
  dow: number;
  key: string;
}

export interface SchedulePrintWeekPage {
  weekStart: Date;
  weekEnd: Date;
  days: SchedulePrintWeekDay[];
  rows: SchedulePrintRow[];
}

const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const parseLocalDate = (s: any): Date => {
  const m = String(s).match(/^(\d{4})-(\d{2})-(\d{2})/);
  if (m) return new Date(+m[1], +m[2] - 1, +m[3]);
  const d = new Date(s);
  return new Date(d.getFullYear(), d.getMonth(), d.getDate());
};

const ymd = (d: Date) => `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;

export function buildSchedulePrintWeekPages(courseDetailsList: any[]): SchedulePrintWeekPage[] {
  const entries: SchedulePrintEntry[] = [];

  courseDetailsList?.forEach(course => {
    course.courseSectionDetailsViewModelList?.forEach(section => {
      section.dayDetailsViewModelList?.forEach(day => {
        day.datePeriodRoomDetailsViewModelList?.forEach(d => {
          const date = parseLocalDate(d.date);
          entries.push({
            date,
            key: ymd(date),
            periodId: d.periodId,
            periodName: d.periodName,
            roomName: d.roomName,
            courseName: course.courseName,
            courseSectionName: section.courseSectionName,
            staffName: section.staffName,
          });
        });
      });
    });
  });

  if (!entries.length) return [];

  const periodMap = new Map<number, string>();
  entries.forEach(e => { if (!periodMap.has(e.periodId)) periodMap.set(e.periodId, e.periodName); });
  const periods = Array.from(periodMap.entries())
    .map(([id, name]) => ({ id, name }))
    .sort((a, b) => a.id - b.id);

  const dowSet = new Set<number>();
  entries.forEach(e => dowSet.add(e.date.getDay()));
  const daysOfWeek = Array.from(dowSet).sort((a, b) => a - b);

  const weekMap = new Map<string, SchedulePrintEntry[]>();
  entries.forEach(e => {
    const sunday = new Date(e.date);
    sunday.setDate(sunday.getDate() - sunday.getDay());
    const key = ymd(sunday);
    if (!weekMap.has(key)) weekMap.set(key, []);
    weekMap.get(key).push(e);
  });

  return Array.from(weekMap.entries())
    .map(([key, weekEntries]) => {
      const [y, mo, d] = key.split('-').map(Number);
      return { sunday: new Date(y, mo, d), weekEntries };
    })
    .sort((a, b) => a.sunday.getTime() - b.sunday.getTime())
    .map(({ sunday, weekEntries }) => {
      const weekDays: SchedulePrintWeekDay[] = daysOfWeek.map(dow => {
        const dt = new Date(sunday);
        dt.setDate(dt.getDate() + dow);
        return { date: dt, dayName: DAY_NAMES[dow], dow, key: ymd(dt) };
      });
      const rows: SchedulePrintRow[] = periods.map(p => ({
        periodName: p.name,
        cells: weekDays.map(wd => ({
          entries: weekEntries.filter(e => e.periodId === p.id && e.key === wd.key),
        })),
      }));
      const saturday = new Date(sunday);
      saturday.setDate(saturday.getDate() + 6);
      return { weekStart: sunday, weekEnd: saturday, days: weekDays, rows };
    });
}

export function getSchedulePrintStyles(): string {
  return `
    @page { size: landscape; margin: 12mm; }
    h1, h2, h3, h4, h5, h6, p { margin: 0; }
    body { -webkit-print-color-adjust: exact; print-color-adjust: exact; font-family: Arial, sans-serif; background-color: #fff; color: #121212; }
    .week-page { page-break-after: always; }
    .week-page:last-child { page-break-after: auto; }
    .report-header { display: flex; align-items: center; border-bottom: 2px solid #000; padding-bottom: 8px; margin-bottom: 8px; }
    .school-logo { width: 56px; height: 56px; border-radius: 50%; border: 1px solid #cacaca; margin-right: 12px; overflow: hidden; flex-shrink: 0; }
    .school-logo img { width: 100%; height: 100%; object-fit: cover; }
    .header-info { flex: 1; }
    .header-info h4 { font-size: 16px; font-weight: 600; }
    .header-info p { font-size: 12px; color: #555; }
    .header-right { text-align: right; }
    .header-right .title { font-size: 14px; font-weight: 600; }
    .header-right .powered { font-size: 11px; color: #666; }
    .subject-line { display: flex; justify-content: space-between; align-items: center; padding: 6px 0; font-size: 13px; }
    .subject-line .name { font-size: 15px; font-weight: 600; }
    .subject-line .meta { color: #555; }
    .week-range { background: #f0f0f0; padding: 6px 10px; font-size: 13px; font-weight: 600; margin-bottom: 6px; border-left: 4px solid #1976d2; }
    table.timetable-grid { border-collapse: collapse; width: 100%; table-layout: fixed; }
    table.timetable-grid th, table.timetable-grid td { border: 1px solid #888; padding: 6px 8px; vertical-align: top; font-size: 12px; }
    table.timetable-grid thead th { background-color: #e3f2fd; text-align: center; font-weight: 600; }
    table.timetable-grid thead th .day-date { font-size: 10px; color: #555; font-weight: 400; display: block; margin-top: 2px; }
    table.timetable-grid .period-col { width: 110px; background-color: #f0f0f0; font-weight: 600; text-align: center; }
    table.timetable-grid td.day-cell { height: 60px; }
    .cell-entry { padding: 2px 0; }
    .cell-entry + .cell-entry { border-top: 1px dashed #ccc; margin-top: 2px; padding-top: 4px; }
    .cell-entry .course-section { font-weight: 600; }
    .cell-entry .teacher { font-size: 11px; color: #444; }
    .cell-entry .room { font-size: 11px; color: #555; }
  `;
}