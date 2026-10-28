using System;
using System.Collections.Generic;
using System.Linq;

namespace opensis.data.Helper
{
    /// <summary>
    /// Pure rules behind cloning course sections from one academic year into another,
    /// kept free of EF so they can be unit tested. The yearly rollover gives courses,
    /// calendars, rooms, periods, grade scales and marking periods new ids, so every
    /// reference is matched on its title and dates are moved forward by whole years.
    /// </summary>
    public static class CourseSectionClone
    {
        public enum ScheduleKind { Unknown, Fixed, Variable, Calendar, Block }

        /// <summary>
        /// Reads the schedule type stored on a course section, e.g. "Fixed Schedule (1)"
        /// or the "fixedschedule" form the UI posts.
        /// </summary>
        public static ScheduleKind GetScheduleKind(string? scheduleType)
        {
            var s = (scheduleType ?? "").ToLowerInvariant();
            if (s.Contains("(1)") || s.Contains("fixed")) return ScheduleKind.Fixed;
            if (s.Contains("(2)") || s.Contains("variable")) return ScheduleKind.Variable;
            if (s.Contains("(3)") || s.Contains("calendar")) return ScheduleKind.Calendar;
            if (s.Contains("(4)") || s.Contains("block")) return ScheduleKind.Block;
            return ScheduleKind.Unknown;
        }

        /// <summary>Key used to match titles across years: trimmed, case-insensitive.</summary>
        public static string NormalizeTitle(string? title) => (title ?? "").Trim().ToLowerInvariant();

        /// <summary>True when both titles are non-empty and equal ignoring case and surrounding spaces.</summary>
        public static bool SameTitle(string? a, string? b)
        {
            var key = NormalizeTitle(a);
            return key.Length > 0 && key == NormalizeTitle(b);
        }

        /// <summary>Splits a "Monday|Wednesday" style day list into lower-case tokens.</summary>
        public static HashSet<string> DayTokens(string? days)
        {
            return new HashSet<string>((days ?? "")
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim().ToLowerInvariant())
                .Where(d => d.Length > 0));
        }

        /// <summary>True when the two day lists share at least one day.</summary>
        public static bool DaysOverlap(string? a, string? b)
        {
            var left = DayTokens(a);
            return left.Count > 0 && DayTokens(b).Any(left.Contains);
        }

        /// <summary>
        /// True when the two date ranges touch. A missing bound is treated as open.
        /// </summary>
        public static bool DurationsOverlap(DateTime? aStart, DateTime? aEnd, DateTime? bStart, DateTime? bEnd)
        {
            var startsBeforeOtherEnds = aStart == null || bEnd == null || aStart.Value.Date <= bEnd.Value.Date;
            var otherStartsBeforeEnds = bStart == null || aEnd == null || bStart.Value.Date <= aEnd.Value.Date;
            return startsBeforeOtherEnds && otherStartsBeforeEnds;
        }

        /// <summary>
        /// Maps a section's duration into the target year. A section that ran the whole
        /// source calendar follows the whole target calendar; any other range is moved
        /// forward by <paramref name="yearShift"/> years and clipped to the target calendar.
        /// <c>Adjusted</c> is true when clipping changed the moved range, which callers
        /// surface as a warning.
        /// </summary>
        public static (DateTime? Start, DateTime? End, bool Adjusted) MapDuration(
            DateTime? start, DateTime? end,
            DateTime? sourceCalendarStart, DateTime? sourceCalendarEnd,
            DateTime? targetCalendarStart, DateTime? targetCalendarEnd,
            int yearShift)
        {
            if (start == null || end == null)
            {
                return (targetCalendarStart?.Date, targetCalendarEnd?.Date, false);
            }

            if (SameDate(start, sourceCalendarStart) && SameDate(end, sourceCalendarEnd))
            {
                return (targetCalendarStart?.Date, targetCalendarEnd?.Date, false);
            }

            var newStart = start.Value.Date.AddYears(yearShift);
            var newEnd = end.Value.Date.AddYears(yearShift);
            var adjusted = false;

            if (targetCalendarStart != null && targetCalendarEnd != null)
            {
                var calStart = targetCalendarStart.Value.Date;
                var calEnd = targetCalendarEnd.Value.Date;

                if (newEnd < calStart || newStart > calEnd)
                {
                    return (calStart, calEnd, true);
                }
                if (newStart < calStart)
                {
                    newStart = calStart;
                    adjusted = true;
                }
                if (newEnd > calEnd)
                {
                    newEnd = calEnd;
                    adjusted = true;
                }
            }

            return (newStart, newEnd, adjusted);
        }

        /// <summary>True when <paramref name="date"/> falls inside the calendar, bounds included.</summary>
        public static bool WithinCalendar(DateTime date, DateTime? calendarStart, DateTime? calendarEnd)
        {
            var d = date.Date;
            return (calendarStart == null || d >= calendarStart.Value.Date)
                && (calendarEnd == null || d <= calendarEnd.Value.Date);
        }

        private static bool SameDate(DateTime? a, DateTime? b) => a != null && b != null && a.Value.Date == b.Value.Date;
    }
}
