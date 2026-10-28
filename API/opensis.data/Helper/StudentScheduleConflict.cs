using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using opensis.data.Models;

namespace opensis.data.Helper
{
    /// <summary>
    /// Period-conflict rules used when scheduling a student into a course section,
    /// kept free of EF so they can be unit tested.
    /// </summary>
    public static class StudentScheduleConflict
    {
        /// <summary>
        /// Hard limit of the student_schedule_view.conflict_comment column.
        /// </summary>
        public const int MaxCommentLength = 300;

        private const string Ellipsis = "...";

        /// <summary>
        /// True when <paramref name="target"/> meets in the same period on at least one
        /// common day as <paramref name="existing"/>. Each argument is one row of
        /// all_course_section_view, i.e. one schedule row (fixed, variable, calendar or block)
        /// of a course section. Date overlap, dropped rows and the allow-conflict flag are
        /// checked by the caller.
        /// </summary>
        public static bool MeetsAtSameTime(AllCourseSectionView target, AllCourseSectionView existing)
        {
            return
                target.FixedPeriodId != null && ((existing.FixedPeriodId == target.FixedPeriodId || existing.VarPeriodId == target.FixedPeriodId || existing.CalPeriodId == target.FixedPeriodId) && ((existing.FixedDays != null && (Regex.IsMatch((target.FixedDays ?? "").ToLower(), existing.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (existing.VarDay != null && ((target.FixedDays ?? "").ToLower().Contains(existing.VarDay.ToLower()))) || (existing.CalDay != null && ((target.FixedDays ?? "").ToLower().Contains(existing.CalDay.ToLower())))))
                ||
                target.VarPeriodId != null && ((existing.FixedPeriodId == target.VarPeriodId || existing.VarPeriodId == target.VarPeriodId || existing.CalPeriodId == target.VarPeriodId) && ((existing.FixedDays != null && (Regex.IsMatch((target.VarDay ?? "").ToLower(), existing.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (existing.VarDay != null && ((target.VarDay ?? "").ToLower().Contains(existing.VarDay.ToLower()))) || (existing.CalDay != null && ((target.VarDay ?? "").ToLower().Contains(existing.CalDay.ToLower())))))
                ||
                target.CalPeriodId != null && ((existing.FixedPeriodId == target.CalPeriodId || existing.VarPeriodId == target.CalPeriodId || existing.CalPeriodId == target.CalPeriodId) && ((existing.FixedDays != null && (Regex.IsMatch((target.CalDay ?? "").ToLower(), existing.FixedDays.ToLower(), RegexOptions.IgnoreCase))) || (existing.VarDay != null && ((target.CalDay ?? "").ToLower().Contains(existing.VarDay.ToLower()))) || (existing.CalDay != null && ((target.CalDay ?? "").ToLower().Contains(existing.CalDay.ToLower())))))
                ||
                target.BlockPeriodId != null && (existing.BlockPeriodId == target.BlockPeriodId && existing.BlockRoomId == target.BlockRoomId && existing.BlockId == target.BlockId);
        }

        /// <summary>
        /// Builds the conflict comment shown in the scheduling report, naming every course
        /// section the student is already scheduled in at the same time, e.g.
        /// "Period conflict with English Reading G1 (1-2 Period 1, Mon-Fri, Room#1- G1)".
        /// Result is trimmed to <see cref="MaxCommentLength"/>.
        /// </summary>
        /// <param name="conflicts">Schedule rows of the sections already on the student's timetable.</param>
        /// <param name="periodTitles">Period id to title, for the student's school.</param>
        /// <param name="roomTitles">Room id to title, for the student's school.</param>
        public static string BuildPeriodConflictComment(IEnumerable<AllCourseSectionView> conflicts, IReadOnlyDictionary<int, string> periodTitles, IReadOnlyDictionary<int, string> roomTitles)
        {
            var descriptions = conflicts
                .GroupBy(c => c.CourseSectionId)
                .Select(g => DescribeSection(g.First(), periodTitles, roomTitles))
                .ToList();

            if (descriptions.Count == 0)
            {
                return "There is a period conflict";
            }

            var comment = "Period conflict with " + string.Join("; ", descriptions);
            if (comment.Length > MaxCommentLength)
            {
                comment = comment.Substring(0, MaxCommentLength - Ellipsis.Length) + Ellipsis;
            }
            return comment;
        }

        /// <summary>
        /// "Section name (Period, Days, Room)". The course title is left out because section
        /// names already carry it in practice. Missing titles fall back to ids.
        /// </summary>
        public static string DescribeSection(AllCourseSectionView section, IReadOnlyDictionary<int, string> periodTitles, IReadOnlyDictionary<int, string> roomTitles)
        {
            var name = string.IsNullOrWhiteSpace(section.CourseSectionName)
                ? (section.CourseTitle ?? "").Trim()
                : section.CourseSectionName.Trim();

            var details = new List<string>();

            var periodId = section.FixedPeriodId ?? section.VarPeriodId ?? section.CalPeriodId ?? section.BlockPeriodId;
            if (periodId != null)
            {
                details.Add(periodTitles.TryGetValue(periodId.Value, out var periodTitle) && !string.IsNullOrWhiteSpace(periodTitle)
                    ? periodTitle.Trim()
                    : "Period " + periodId.Value);
            }

            var days = ShortDays(section.FixedDays ?? section.VarDay ?? section.CalDay);
            if (days != null)
            {
                details.Add(days);
            }

            var roomId = section.FixedRoomId ?? section.VarRoomId ?? section.CalRoomId ?? section.BlockRoomId;
            if (roomId != null)
            {
                details.Add(roomTitles.TryGetValue(roomId.Value, out var roomTitle) && !string.IsNullOrWhiteSpace(roomTitle)
                    ? roomTitle.Trim()
                    : "Room " + roomId.Value);
            }

            return details.Count == 0 ? name : name + " (" + string.Join(", ", details) + ")";
        }

        private static readonly string[] WeekDays = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
        private static readonly string[] WeekDaysShort = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        /// <summary>
        /// "Monday|Tuesday|Wednesday|Thursday|Friday" becomes "Mon-Fri"; any other set of
        /// days is listed in week order, e.g. "Tue, Thu". Unknown tokens are kept as given.
        /// </summary>
        public static string? ShortDays(string? days)
        {
            if (string.IsNullOrWhiteSpace(days))
            {
                return null;
            }

            var tokens = days.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()).Where(d => d.Length > 0).ToList();
            var known = tokens.Select(t => Array.IndexOf(WeekDays, t.ToLower())).Where(i => i >= 0).Distinct().OrderBy(i => i).ToList();
            var unknown = tokens.Where(t => Array.IndexOf(WeekDays, t.ToLower()) < 0).ToList();

            if (known.Count == 5 && unknown.Count == 0 && known.SequenceEqual(new[] { 0, 1, 2, 3, 4 }))
            {
                return "Mon-Fri";
            }

            var parts = known.Select(i => WeekDaysShort[i]).Concat(unknown).ToList();
            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }
}
