using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Models;
using opensis.data.ViewModels.CourseManager;
using static opensis.data.Helper.CourseSectionClone;

namespace opensis.data.Repository
{
    /// <summary>
    /// Clones the course sections of a past academic year into another year. Courses,
    /// calendars, rooms, periods, grade scales and marking periods are re-created with
    /// new ids by the rollover, so each reference is matched on its title in the target
    /// year. Sections whose course is missing, whose name is taken or whose room is
    /// already booked are skipped and reported; lesser mismatches become warnings.
    /// </summary>
    public partial class CourseManagerRepository
    {
        public const string CLONE_YEAR_REQUIRED = "Select the academic year to clone from.";
        public const string CLONE_SAME_YEAR = "The academic year to clone from must differ from the current one.";
        public const string CLONE_NO_SOURCE_YEAR = "The academic year to clone from does not exist.";
        public const string CLONE_NO_TARGET_CALENDAR = "The current academic year has no session calendar. Run the rollover first.";
        public const string CLONE_COURSE_NOT_FOUND = "Course not found in the current academic year.";
        public const string CLONE_NO_SOURCE_SECTIONS = "No course sections were found in the selected academic year.";
        public const string CLONE_NO_SOURCE_SECTIONS_FOR_COURSE = "The selected academic year has no course sections for a course with this title.";

        public const string SKIP_INACTIVE = "Section is inactive.";
        public const string SKIP_NAME_EXISTS = "A course section with this name already exists.";

        public CloneCourseSectionsViewModel CloneCourseSections(CloneCourseSectionsViewModel vm)
        {
            using var transaction = this.context?.Database.BeginTransaction();
            try
            {
                if (this.context == null)
                {
                    return Fail(vm, "Database context is not available.");
                }
                if (vm.SourceAcademicYear == null || vm.TargetAcademicYear == null)
                {
                    return Fail(vm, CLONE_YEAR_REQUIRED);
                }
                if (vm.SourceAcademicYear == vm.TargetAcademicYear)
                {
                    return Fail(vm, CLONE_SAME_YEAR);
                }

                var sourceYear = vm.SourceAcademicYear.Value;
                var targetYear = vm.TargetAcademicYear.Value;

                var sourceCalendar = SessionCalendar(vm.TenantId, vm.SchoolId, sourceYear);
                if (sourceCalendar == null)
                {
                    return Fail(vm, CLONE_NO_SOURCE_YEAR);
                }
                var targetCalendar = SessionCalendar(vm.TenantId, vm.SchoolId, targetYear);
                if (targetCalendar == null)
                {
                    return Fail(vm, CLONE_NO_TARGET_CALENDAR);
                }

                Course? scopeCourse = null;
                if (vm.CourseId != null)
                {
                    scopeCourse = this.context.Course.FirstOrDefault(c => c.TenantId == vm.TenantId && c.SchoolId == vm.SchoolId && c.CourseId == vm.CourseId);
                    if (scopeCourse == null)
                    {
                        return Fail(vm, CLONE_COURSE_NOT_FOUND);
                    }
                }

                var sourceSections = this.context.CourseSection
                    .Include(x => x.Course)
                    .Where(x => x.TenantId == vm.TenantId && x.SchoolId == vm.SchoolId && x.AcademicYear == sourceYear)
                    .ToList();
                if (scopeCourse != null)
                {
                    sourceSections = sourceSections.Where(s => SameTitle(s.Course?.CourseTitle, scopeCourse.CourseTitle)).ToList();
                }
                if (sourceSections.Count == 0)
                {
                    return Fail(vm, scopeCourse == null ? CLONE_NO_SOURCE_SECTIONS : CLONE_NO_SOURCE_SECTIONS_FOR_COURSE);
                }

                var ctx = new CloneContext(this.context, vm, sourceCalendar, targetCalendar, sourceSections);

                foreach (var source in sourceSections.OrderBy(s => s.Course?.CourseTitle).ThenBy(s => s.CourseSectionName))
                {
                    var result = new CourseSectionCloneResult
                    {
                        CourseTitle = source.Course?.CourseTitle,
                        CourseSectionName = source.CourseSectionName,
                        SourceCourseSectionId = source.CourseSectionId
                    };
                    vm.Results.Add(result);
                    CloneSection(ctx, source, result);
                }

                vm.ClonedCount = vm.Results.Count(r => r.Cloned);
                vm.SkippedCount = vm.Results.Count - vm.ClonedCount;

                if (vm.DryRun)
                {
                    transaction?.Rollback();
                    vm._message = $"Preview: {vm.ClonedCount} course section(s) would be cloned and {vm.SkippedCount} skipped.";
                }
                else
                {
                    this.context.SaveChanges();
                    transaction?.Commit();
                    vm._message = $"{vm.ClonedCount} course section(s) cloned, {vm.SkippedCount} skipped.";
                }
                vm._failure = false;
            }
            catch (Exception es)
            {
                transaction?.Rollback();
                vm._failure = true;
                vm._message = es.Message;
            }
            return vm;
        }

        private static CloneCourseSectionsViewModel Fail(CloneCourseSectionsViewModel vm, string message)
        {
            vm._failure = true;
            vm._message = message;
            return vm;
        }

        private SchoolCalendars? SessionCalendar(Guid tenantId, int schoolId, decimal academicYear)
        {
            return this.context?.SchoolCalendars
                .Where(c => c.TenantId == tenantId && c.SchoolId == schoolId && c.AcademicYear == academicYear && c.SessionCalendar == true)
                .OrderByDescending(c => c.DefaultCalender)
                .FirstOrDefault();
        }

        /// <summary>
        /// Copies one source section into the target year, or records why it was skipped.
        /// Nothing is written when the section is skipped.
        /// </summary>
        private static void CloneSection(CloneContext ctx, CourseSection source, CourseSectionCloneResult result)
        {
            if (source.IsActive == false)
            {
                result.Reason = SKIP_INACTIVE;
                return;
            }

            if (!ctx.TargetCoursesByTitle.TryGetValue(NormalizeTitle(source.Course?.CourseTitle), out var targetCourse))
            {
                result.Reason = $"Course '{source.Course?.CourseTitle}' does not exist in {ctx.TargetYearLabel}. Add the course first.";
                return;
            }

            var nameKey = NormalizeTitle(source.CourseSectionName);
            if (nameKey.Length > 0 && ctx.TargetSectionNames.Contains(nameKey))
            {
                result.Reason = SKIP_NAME_EXISTS;
                return;
            }

            var section = new CourseSection
            {
                TenantId = source.TenantId,
                SchoolId = source.SchoolId,
                CourseId = targetCourse.CourseId,
                CourseSectionId = ctx.NextSectionId,
                AcademicYear = ctx.TargetYear,
                CourseSectionName = source.CourseSectionName,
                GradeScaleType = source.GradeScaleType,
                CreditHours = source.CreditHours,
                Seats = source.Seats,
                AllowStudentConflict = source.AllowStudentConflict,
                AllowTeacherConflict = source.AllowTeacherConflict,
                AllowRoomConflict = source.AllowRoomConflict,
                IsWeightedCourse = source.IsWeightedCourse,
                AffectsClassRank = source.AffectsClassRank,
                AffectsHonorRoll = source.AffectsHonorRoll,
                OnlineClassRoom = source.OnlineClassRoom,
                OnlineClassroomUrl = source.OnlineClassroomUrl,
                OnlineClassroomPassword = source.OnlineClassroomPassword,
                UseStandards = source.UseStandards,
                DurationBasedOnPeriod = source.DurationBasedOnPeriod,
                ScheduleType = source.ScheduleType,
                MeetingDays = source.MeetingDays,
                AttendanceTaken = source.AttendanceTaken,
                IsActive = true,
                IsCustomMarkingPeriod = source.IsCustomMarkingPeriod,
                CreatedBy = ctx.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };

            MapCalendar(ctx, source, section, result);
            MapGradeScales(ctx, source, section, result);
            MapAttendanceCategory(ctx, source, section, result);
            MapMarkingPeriodAndDuration(ctx, source, section, result);

            var rows = MapScheduleRows(ctx, source, section, result);
            if (rows == null)
            {
                return;
            }

            if (ctx.Vm.DryRun == false)
            {
                ctx.Db.CourseSection.Add(section);
                rows.Attach(ctx.Db);
            }
            ctx.CommitCounters(rows);
            ctx.TargetSectionNames.Add(nameKey);

            if (ctx.Vm.IncludeStaff)
            {
                CloneStaff(ctx, source, section, result);
            }

            result.Cloned = true;
            result.NewCourseSectionId = section.CourseSectionId;
        }

        private static void MapCalendar(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            ctx.CalendarsById.TryGetValue(source.CalendarId ?? -1, out var sourceCal);
            SchoolCalendars? target = null;
            if (sourceCal != null)
            {
                ctx.TargetCalendarsByTitle.TryGetValue(NormalizeTitle(sourceCal.Title), out target);
            }
            if (target == null)
            {
                target = ctx.TargetCalendar;
                if (sourceCal != null && !SameTitle(sourceCal.Title, target.Title))
                {
                    result.Warnings.Add($"Calendar '{sourceCal.Title}' not found; using '{target.Title}'.");
                }
            }
            section.CalendarId = target.CalenderId;
        }

        private static void MapGradeScales(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            section.GradeScaleId = MapByTitle(source.GradeScaleId, ctx.GradeScalesById, g => g.GradeScaleName, ctx.TargetGradeScalesByName, g => g.GradeScaleId, "Grade scale", result);
            section.StandardGradeScaleId = MapByTitle(source.StandardGradeScaleId, ctx.GradeScalesById, g => g.GradeScaleName, ctx.TargetGradeScalesByName, g => g.GradeScaleId, "Standard grade scale", result);
        }

        private static void MapAttendanceCategory(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            section.AttendanceCategoryId = MapByTitle(source.AttendanceCategoryId, ctx.AttendanceCategoriesById, a => a.Title, ctx.TargetAttendanceCategoriesByTitle, a => a.AttendanceCategoryId, "Attendance category", result);
        }

        /// <summary>
        /// Resolves a source id to its title, then to the target-year entity with that
        /// title. Returns null, with a warning, when either step fails.
        /// </summary>
        private static int? MapByTitle<T>(int? sourceId, IReadOnlyDictionary<int, T> byId, Func<T, string?> title, IReadOnlyDictionary<string, T> targetByTitle, Func<T, int> id, string label, CourseSectionCloneResult result)
        {
            if (sourceId == null)
            {
                return null;
            }
            if (!byId.TryGetValue(sourceId.Value, out var sourceEntity))
            {
                result.Warnings.Add($"{label} {sourceId} no longer exists; left blank.");
                return null;
            }
            if (targetByTitle.TryGetValue(NormalizeTitle(title(sourceEntity)), out var target))
            {
                return id(target);
            }
            result.Warnings.Add($"{label} '{title(sourceEntity)}' not found; left blank.");
            return null;
        }

        private static void MapMarkingPeriodAndDuration(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            section.YrMarkingPeriodId = null;
            section.SmstrMarkingPeriodId = null;
            section.QtrMarkingPeriodId = null;
            section.PrgrsprdMarkingPeriodId = null;

            MarkingPeriodRef? matched = null;
            string? sourceTitle = null;

            if (source.PrgrsprdMarkingPeriodId != null && ctx.ProgressPeriodsById.TryGetValue(source.PrgrsprdMarkingPeriodId.Value, out var pp))
            {
                sourceTitle = pp.Title;
                if (ctx.MatchProgressPeriod(pp) is { } t)
                    matched = new MarkingPeriodRef(t.MarkingPeriodId, t.Title, t.StartDate, t.EndDate, m => section.PrgrsprdMarkingPeriodId = m);
            }
            else if (source.QtrMarkingPeriodId != null && ctx.QuartersById.TryGetValue(source.QtrMarkingPeriodId.Value, out var q))
            {
                sourceTitle = q.Title;
                if (ctx.MatchQuarter(q) is { } t)
                    matched = new MarkingPeriodRef(t.MarkingPeriodId, t.Title, t.StartDate, t.EndDate, m => section.QtrMarkingPeriodId = m);
            }
            else if (source.SmstrMarkingPeriodId != null && ctx.SemestersById.TryGetValue(source.SmstrMarkingPeriodId.Value, out var s))
            {
                sourceTitle = s.Title;
                if (ctx.MatchSemester(s) is { } t)
                    matched = new MarkingPeriodRef(t.MarkingPeriodId, t.Title, t.StartDate, t.EndDate, m => section.SmstrMarkingPeriodId = m);
            }
            else if (source.YrMarkingPeriodId != null && ctx.YearsById.TryGetValue(source.YrMarkingPeriodId.Value, out var y))
            {
                sourceTitle = y.Title;
                if (ctx.MatchYear(y) is { } t)
                    matched = new MarkingPeriodRef(t.MarkingPeriodId, t.Title, t.StartDate, t.EndDate, m => section.YrMarkingPeriodId = m);
            }

            var hadSourcePeriod = source.PrgrsprdMarkingPeriodId != null || source.QtrMarkingPeriodId != null || source.SmstrMarkingPeriodId != null || source.YrMarkingPeriodId != null;

            if (matched == null && hadSourcePeriod)
            {
                var fallback = ctx.TargetSchoolYear;
                if (source.DurationBasedOnPeriod == true && fallback != null)
                {
                    matched = new MarkingPeriodRef(fallback.MarkingPeriodId, fallback.Title, fallback.StartDate, fallback.EndDate, m => section.YrMarkingPeriodId = m);
                    result.Warnings.Add($"Marking period '{sourceTitle ?? "?"}' not found; using '{fallback.Title}'.");
                }
                else if (source.DurationBasedOnPeriod == true)
                {
                    section.DurationBasedOnPeriod = false;
                    result.Warnings.Add($"Marking period '{sourceTitle ?? "?"}' not found and {ctx.TargetYearLabel} has no school year marking period; duration set to the calendar dates.");
                }
                else
                {
                    result.Warnings.Add($"Marking period '{sourceTitle ?? "?"}' not found; left blank.");
                }
            }

            if (matched != null)
            {
                matched.Assign(matched.Id);
            }

            if (section.DurationBasedOnPeriod == true && matched != null && matched.StartDate != null && matched.EndDate != null)
            {
                section.DurationStartDate = matched.StartDate.Value.Date;
                section.DurationEndDate = matched.EndDate.Value.Date;
                return;
            }

            var (start, end, adjusted) = MapDuration(
                source.DurationStartDate, source.DurationEndDate,
                ctx.SourceCalendar.StartDate, ctx.SourceCalendar.EndDate,
                ctx.TargetCalendar.StartDate, ctx.TargetCalendar.EndDate,
                ctx.YearShift);
            section.DurationStartDate = start;
            section.DurationEndDate = end;
            if (adjusted)
            {
                result.Warnings.Add("Duration dates were clipped to the calendar; review them.");
            }
        }

        private sealed class MarkingPeriodRef
        {
            public MarkingPeriodRef(int id, string? title, DateTime? start, DateTime? end, Action<int?> assign)
            {
                Id = id; Title = title; StartDate = start; EndDate = end; Assign = assign;
            }
            public int Id { get; }
            public string? Title { get; }
            public DateTime? StartDate { get; }
            public DateTime? EndDate { get; }
            public Action<int?> Assign { get; }
        }

        /// <summary>
        /// Builds the schedule rows of the new section. Returns null, with the reason set,
        /// when a room is already booked in the target year and the section does not allow
        /// room conflicts.
        /// </summary>
        private static ScheduleRows? MapScheduleRows(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            var rows = new ScheduleRows();
            var kind = GetScheduleKind(source.ScheduleType);
            if (kind == ScheduleKind.Unknown)
            {
                if (ctx.SourceFixed[source.CourseSectionId].Any()) kind = ScheduleKind.Fixed;
                else if (ctx.SourceVariable[source.CourseSectionId].Any()) kind = ScheduleKind.Variable;
                else if (ctx.SourceCalendarRows[source.CourseSectionId].Any()) kind = ScheduleKind.Calendar;
                else if (ctx.SourceBlock[source.CourseSectionId].Any()) kind = ScheduleKind.Block;
            }

            switch (kind)
            {
                case ScheduleKind.Fixed:
                    {
                        var src = ctx.SourceFixed[source.CourseSectionId].OrderBy(r => r.Serial).FirstOrDefault();
                        if (src == null)
                        {
                            result.Warnings.Add("The source section had no fixed schedule row; add the period and room.");
                            break;
                        }
                        var slot = MapSlot(ctx, src.BlockId, src.PeriodId, src.RoomId, result);
                        if (slot.Period != null)
                        {
                            AlignAttendance(slot.Period, section.AttendanceTaken, v => section.AttendanceTaken = v, result);
                        }
                        var conflict = ctx.FindConflict(false, slot.RoomId, slot.PeriodId, section.MeetingDays, null, section.DurationStartDate, section.DurationEndDate, section.AllowRoomConflict);
                        if (conflict != null)
                        {
                            result.Reason = conflict;
                            return null;
                        }
                        rows.Fixed = new CourseFixedSchedule
                        {
                            TenantId = section.TenantId,
                            SchoolId = section.SchoolId,
                            CourseId = section.CourseId,
                            CourseSectionId = section.CourseSectionId,
                            GradeScaleId = section.GradeScaleId,
                            Serial = ctx.NextFixedSerial,
                            RoomId = slot.RoomId,
                            BlockId = slot.BlockId,
                            PeriodId = slot.PeriodId,
                            CreatedBy = ctx.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        break;
                    }
                case ScheduleKind.Variable:
                    {
                        var serial = ctx.NextVariableSerial;
                        foreach (var src in ctx.SourceVariable[source.CourseSectionId].OrderBy(r => r.Serial))
                        {
                            var slot = MapSlot(ctx, src.BlockId, src.PeriodId, src.RoomId, result);
                            if (slot.Period == null)
                            {
                                result.Warnings.Add($"{src.Day} row dropped: its period was not found.");
                                continue;
                            }
                            var take = src.TakeAttendance;
                            AlignAttendance(slot.Period, take, v => take = v, result, src.Day);
                            var conflict = ctx.FindConflict(false, slot.RoomId, slot.PeriodId, src.Day, null, section.DurationStartDate, section.DurationEndDate, section.AllowRoomConflict);
                            if (conflict != null)
                            {
                                result.Reason = conflict;
                                return null;
                            }
                            rows.Variable.Add(new CourseVariableSchedule
                            {
                                TenantId = section.TenantId,
                                SchoolId = section.SchoolId,
                                CourseId = section.CourseId,
                                CourseSectionId = section.CourseSectionId,
                                GradeScaleId = section.GradeScaleId,
                                Serial = serial++,
                                Day = src.Day,
                                RoomId = slot.RoomId,
                                BlockId = slot.BlockId,
                                PeriodId = slot.PeriodId,
                                TakeAttendance = take,
                                CreatedBy = ctx.CreatedBy,
                                CreatedOn = DateTime.UtcNow
                            });
                        }
                        if (rows.Variable.Count == 0)
                        {
                            result.Warnings.Add("No schedule rows could be carried over; add the days and periods.");
                        }
                        break;
                    }
                case ScheduleKind.Calendar:
                    {
                        var serial = ctx.NextCalendarSerial;
                        foreach (var src in ctx.SourceCalendarRows[source.CourseSectionId].OrderBy(r => r.Serial))
                        {
                            if (src.Date == null)
                            {
                                continue;
                            }
                            var date = src.Date.Value.Date.AddYears(ctx.YearShift);
                            if (!WithinCalendar(date, ctx.TargetCalendar.StartDate, ctx.TargetCalendar.EndDate))
                            {
                                result.Warnings.Add($"{date:yyyy-MM-dd} row dropped: outside the calendar.");
                                continue;
                            }
                            var slot = MapSlot(ctx, src.BlockId, src.PeriodId, src.RoomId, result);
                            if (slot.Period == null)
                            {
                                result.Warnings.Add($"{date:yyyy-MM-dd} row dropped: its period was not found.");
                                continue;
                            }
                            var take = src.TakeAttendance;
                            AlignAttendance(slot.Period, take, v => take = v, result, date.ToString("yyyy-MM-dd"));
                            var conflict = ctx.FindConflict(false, slot.RoomId, slot.PeriodId, date.DayOfWeek.ToString(), null, date, date, section.AllowRoomConflict);
                            if (conflict != null)
                            {
                                result.Reason = conflict;
                                return null;
                            }
                            rows.Calendar.Add(new CourseCalendarSchedule
                            {
                                TenantId = section.TenantId,
                                SchoolId = section.SchoolId,
                                CourseId = section.CourseId,
                                CourseSectionId = section.CourseSectionId,
                                GradeScaleId = section.GradeScaleId,
                                Serial = serial++,
                                Date = date,
                                RoomId = slot.RoomId,
                                BlockId = slot.BlockId,
                                PeriodId = slot.PeriodId,
                                TakeAttendance = take,
                                CreatedBy = ctx.CreatedBy,
                                CreatedOn = DateTime.UtcNow
                            });
                        }
                        if (rows.Calendar.Count == 0)
                        {
                            result.Warnings.Add("No schedule rows could be carried over; add the dates and periods.");
                        }
                        break;
                    }
                case ScheduleKind.Block:
                    {
                        var serial = ctx.NextBlockSerial;
                        foreach (var src in ctx.SourceBlock[source.CourseSectionId].OrderBy(r => r.Serial))
                        {
                            var slot = MapSlot(ctx, src.BlockId, src.PeriodId, src.RoomId, result);
                            if (slot.Period == null)
                            {
                                result.Warnings.Add("A block row was dropped: its period was not found.");
                                continue;
                            }
                            var take = src.TakeAttendance;
                            AlignAttendance(slot.Period, take, v => take = v, result);
                            var conflict = ctx.FindConflict(true, slot.RoomId, slot.PeriodId, null, slot.BlockId, section.DurationStartDate, section.DurationEndDate, section.AllowRoomConflict);
                            if (conflict != null)
                            {
                                result.Reason = conflict;
                                return null;
                            }
                            rows.Block.Add(new CourseBlockSchedule
                            {
                                TenantId = section.TenantId,
                                SchoolId = section.SchoolId,
                                CourseId = section.CourseId,
                                CourseSectionId = section.CourseSectionId,
                                GradeScaleId = section.GradeScaleId,
                                Serial = serial++,
                                RoomId = slot.RoomId,
                                BlockId = slot.BlockId,
                                PeriodId = slot.PeriodId,
                                TakeAttendance = take,
                                CreatedBy = ctx.CreatedBy,
                                CreatedOn = DateTime.UtcNow
                            });
                        }
                        if (rows.Block.Count == 0)
                        {
                            result.Warnings.Add("No schedule rows could be carried over; add the blocks and periods.");
                        }
                        break;
                    }
                default:
                    result.Warnings.Add("The source section has no schedule; add one.");
                    break;
            }
            return rows;
        }

        private sealed class Slot
        {
            public int? RoomId { get; set; }
            public int? BlockId { get; set; }
            public int? PeriodId { get; set; }
            public BlockPeriod? Period { get; set; }
        }

        /// <summary>
        /// Maps a room, block and period onto the target year. The period is looked up
        /// inside the matched block first, then by title anywhere in the target year.
        /// </summary>
        private static Slot MapSlot(CloneContext ctx, int? blockId, int? periodId, int? roomId, CourseSectionCloneResult result)
        {
            var slot = new Slot();

            slot.RoomId = MapByTitle(roomId, ctx.RoomsById, r => r.Title, ctx.TargetRoomsByTitle, r => r.RoomId, "Room", result);

            Block? sourceBlock = null;
            if (blockId != null && ctx.BlocksById.TryGetValue(blockId.Value, out sourceBlock))
            {
                if (ctx.TargetBlocksByTitle.TryGetValue(NormalizeTitle(sourceBlock.BlockTitle), out var targetBlock))
                {
                    slot.BlockId = targetBlock.BlockId;
                }
            }

            if (periodId != null && ctx.PeriodsById.TryGetValue(periodId.Value, out var sourcePeriod))
            {
                var key = NormalizeTitle(sourcePeriod.PeriodTitle);
                var candidates = ctx.TargetPeriods.Where(p => NormalizeTitle(p.PeriodTitle) == key).ToList();
                var period = candidates.FirstOrDefault(p => slot.BlockId != null && p.BlockId == slot.BlockId) ?? candidates.FirstOrDefault();
                if (period != null)
                {
                    slot.Period = period;
                    slot.PeriodId = period.PeriodId;
                    slot.BlockId = period.BlockId;
                }
                else
                {
                    result.Warnings.Add($"Period '{sourcePeriod.PeriodTitle}' not found; left blank.");
                }
            }
            else if (periodId != null)
            {
                result.Warnings.Add($"Period {periodId} no longer exists; left blank.");
            }

            if (slot.BlockId == null && sourceBlock != null)
            {
                result.Warnings.Add($"Block '{sourceBlock.BlockTitle}' not found; left blank.");
            }

            return slot;
        }

        /// <summary>
        /// Keeps the attendance flag consistent with the period's "Calculate Attendance"
        /// setting, as the section editor enforces, and says so when it had to change.
        /// </summary>
        private static void AlignAttendance(BlockPeriod period, bool? current, Action<bool?> set, CourseSectionCloneResult result, string? rowLabel = null)
        {
            var required = period.CalculateAttendance == true;
            if ((current == true) == required)
            {
                return;
            }
            set(required);
            var prefix = rowLabel == null ? "" : rowLabel + ": ";
            result.Warnings.Add(required
                ? $"{prefix}attendance switched on because period '{period.PeriodTitle}' calculates attendance."
                : $"{prefix}attendance switched off because period '{period.PeriodTitle}' does not calculate attendance.");
        }

        private static void CloneStaff(CloneContext ctx, CourseSection source, CourseSection section, CourseSectionCloneResult result)
        {
            foreach (var assignment in ctx.SourceStaff[source.CourseSectionId].Where(a => a.IsDropped != true))
            {
                ctx.StaffById.TryGetValue(assignment.StaffId, out var staff);
                var name = staff == null ? $"staff {assignment.StaffId}" : $"{staff.FirstGivenName} {staff.LastFamilyName}".Trim();
                var active = staff != null && staff.IsActive != false && (staff.EndDate == null || staff.EndDate.Value.Date >= DateTime.UtcNow.Date);
                if (!active)
                {
                    result.Warnings.Add($"Teacher {name} is no longer active and was not assigned.");
                    continue;
                }

                if (ctx.Vm.DryRun)
                {
                    continue;
                }
                ctx.Db.StaffCoursesectionSchedule.Add(new StaffCoursesectionSchedule
                {
                    TenantId = section.TenantId,
                    SchoolId = section.SchoolId,
                    StaffId = assignment.StaffId,
                    StaffGuid = assignment.StaffGuid,
                    CourseId = section.CourseId,
                    CourseSectionId = section.CourseSectionId,
                    CourseSectionName = section.CourseSectionName,
                    YrMarkingPeriodId = section.YrMarkingPeriodId,
                    SmstrMarkingPeriodId = section.SmstrMarkingPeriodId,
                    QtrMarkingPeriodId = section.QtrMarkingPeriodId,
                    PrgrsprdMarkingPeriodId = section.PrgrsprdMarkingPeriodId,
                    AcademicYear = section.AcademicYear,
                    DurationStartDate = section.DurationStartDate,
                    DurationEndDate = section.DurationEndDate,
                    MeetingDays = section.MeetingDays,
                    IsAssigned = true,
                    IsPrimaryStaff = assignment.IsPrimaryStaff,
                    CreatedBy = ctx.CreatedBy,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }

        /// <summary>Schedule rows of one new section, kept aside until the section is accepted.</summary>
        private sealed class ScheduleRows
        {
            public CourseFixedSchedule? Fixed { get; set; }
            public List<CourseVariableSchedule> Variable { get; } = new();
            public List<CourseCalendarSchedule> Calendar { get; } = new();
            public List<CourseBlockSchedule> Block { get; } = new();

            public void Attach(CRMContext db)
            {
                if (Fixed != null) db.CourseFixedSchedule.Add(Fixed);
                if (Variable.Count > 0) db.CourseVariableSchedule.AddRange(Variable);
                if (Calendar.Count > 0) db.CourseCalendarSchedule.AddRange(Calendar);
                if (Block.Count > 0) db.CourseBlockSchedule.AddRange(Block);
            }
        }

        /// <summary>One booked room-and-period in the target year, used to detect clashes.</summary>
        private sealed class Occupancy
        {
            public string? SectionName { get; set; }
            public int? RoomId { get; set; }
            public int? PeriodId { get; set; }
            public int? BlockId { get; set; }
            /// <summary>Day names joined by '|'; null for block rows.</summary>
            public string? Days { get; set; }
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
        }

        /// <summary>
        /// Everything the per-section work needs: lookups by id for the source year,
        /// lookups by title for the target year, the rooms already booked in the target
        /// year and the running id counters.
        /// </summary>
        private sealed class CloneContext
        {
            public CloneContext(CRMContext db, CloneCourseSectionsViewModel vm, SchoolCalendars sourceCalendar, SchoolCalendars targetCalendar, List<CourseSection> sourceSections)
            {
                Db = db;
                Vm = vm;
                SourceCalendar = sourceCalendar;
                TargetCalendar = targetCalendar;
                TargetYear = vm.TargetAcademicYear!.Value;
                YearShift = (int)Math.Round(TargetYear - vm.SourceAcademicYear!.Value);
                CreatedBy = vm.CreatedBy;

                var tenantId = vm.TenantId;
                var schoolId = vm.SchoolId;
                var sourceIds = sourceSections.Select(s => s.CourseSectionId).ToList();

                TargetCoursesByTitle = ByTitle(db.Course.Where(c => c.TenantId == tenantId && c.SchoolId == schoolId && c.AcademicYear == TargetYear).OrderBy(c => c.CourseId).ToList(), c => c.CourseTitle);

                var targetSections = db.CourseSection.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId && s.AcademicYear == TargetYear).ToList();
                TargetSectionNames = new HashSet<string>(targetSections.Select(s => NormalizeTitle(s.CourseSectionName)).Where(n => n.Length > 0));

                var calendars = db.SchoolCalendars.Where(c => c.TenantId == tenantId && c.SchoolId == schoolId).ToList();
                CalendarsById = calendars.GroupBy(c => c.CalenderId).ToDictionary(g => g.Key, g => g.First());
                TargetCalendarsByTitle = ByTitle(calendars.Where(c => c.AcademicYear == TargetYear).OrderBy(c => c.CalenderId), c => c.Title);

                var gradeScales = db.GradeScale.Where(g => g.TenantId == tenantId && g.SchoolId == schoolId).ToList();
                GradeScalesById = gradeScales.GroupBy(g => g.GradeScaleId).ToDictionary(g => g.Key, g => g.First());
                TargetGradeScalesByName = ByTitle(gradeScales.Where(g => g.AcademicYear == TargetYear).OrderBy(g => g.GradeScaleId), g => g.GradeScaleName);

                var attendanceCategories = db.AttendanceCodeCategories.Where(a => a.TenantId == tenantId && a.SchoolId == schoolId).ToList();
                AttendanceCategoriesById = attendanceCategories.GroupBy(a => a.AttendanceCategoryId).ToDictionary(g => g.Key, g => g.First());
                TargetAttendanceCategoriesByTitle = ByTitle(attendanceCategories.Where(a => a.AcademicYear == TargetYear).OrderBy(a => a.AttendanceCategoryId), a => a.Title);

                var rooms = db.Rooms.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId).ToList();
                RoomsById = rooms.GroupBy(r => r.RoomId).ToDictionary(g => g.Key, g => g.First());
                TargetRoomsByTitle = ByTitle(rooms.Where(r => r.AcademicYear == TargetYear).OrderBy(r => r.RoomId), r => r.Title);

                var blocks = db.Block.Where(b => b.TenantId == tenantId && b.SchoolId == schoolId).ToList();
                BlocksById = blocks.GroupBy(b => b.BlockId).ToDictionary(g => g.Key, g => g.First());
                TargetBlocksByTitle = ByTitle(blocks.Where(b => b.AcademicYear == TargetYear).OrderBy(b => b.BlockId), b => b.BlockTitle);

                var periods = db.BlockPeriod.Where(p => p.TenantId == tenantId && p.SchoolId == schoolId).ToList();
                PeriodsById = periods.GroupBy(p => p.PeriodId).ToDictionary(g => g.Key, g => g.First());
                TargetPeriods = periods.Where(p => p.AcademicYear == TargetYear).OrderBy(p => p.BlockId).ThenBy(p => p.PeriodSortOrder).ThenBy(p => p.PeriodId).ToList();

                var years = db.SchoolYears.Where(y => y.TenantId == tenantId && y.SchoolId == schoolId).ToList();
                YearsById = years.GroupBy(y => y.MarkingPeriodId).ToDictionary(g => g.Key, g => g.First());
                Years = years;
                TargetYearsByTitle = ByTitle(years.Where(y => y.AcademicYear == TargetYear).OrderBy(y => y.MarkingPeriodId), y => y.Title);
                TargetSchoolYear = years.Where(y => y.AcademicYear == TargetYear).OrderBy(y => y.StartDate).ThenBy(y => y.MarkingPeriodId).FirstOrDefault();

                var semesters = db.Semesters.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).ToList();
                SemestersById = semesters.GroupBy(s => s.MarkingPeriodId).ToDictionary(g => g.Key, g => g.First());
                SemesterList = semesters;
                TargetSemestersByTitle = ByTitle(semesters.Where(s => s.AcademicYear == TargetYear).OrderBy(s => s.MarkingPeriodId), s => s.Title);

                var quarters = db.Quarters.Where(q => q.TenantId == tenantId && q.SchoolId == schoolId).ToList();
                QuartersById = quarters.GroupBy(q => q.MarkingPeriodId).ToDictionary(g => g.Key, g => g.First());
                QuarterList = quarters;
                TargetQuartersByTitle = ByTitle(quarters.Where(q => q.AcademicYear == TargetYear).OrderBy(q => q.MarkingPeriodId), q => q.Title);

                var progressPeriods = db.ProgressPeriods.Where(p => p.TenantId == tenantId && p.SchoolId == schoolId).ToList();
                ProgressPeriodsById = progressPeriods.GroupBy(p => p.MarkingPeriodId).ToDictionary(g => g.Key, g => g.First());
                ProgressPeriodList = progressPeriods;
                TargetProgressPeriodsByTitle = ByTitle(progressPeriods.Where(p => p.AcademicYear == TargetYear).OrderBy(p => p.MarkingPeriodId), p => p.Title);

                SourceFixed = db.CourseFixedSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && sourceIds.Contains(r.CourseSectionId)).ToList().ToLookup(r => r.CourseSectionId);
                SourceVariable = db.CourseVariableSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && sourceIds.Contains(r.CourseSectionId)).ToList().ToLookup(r => r.CourseSectionId);
                SourceCalendarRows = db.CourseCalendarSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && sourceIds.Contains(r.CourseSectionId)).ToList().ToLookup(r => r.CourseSectionId);
                SourceBlock = db.CourseBlockSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && sourceIds.Contains(r.CourseSectionId)).ToList().ToLookup(r => r.CourseSectionId);

                if (vm.IncludeStaff)
                {
                    var staffRows = db.StaffCoursesectionSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && sourceIds.Contains(r.CourseSectionId)).ToList();
                    SourceStaff = staffRows.ToLookup(r => r.CourseSectionId);
                    var staffIds = staffRows.Select(r => r.StaffId).Distinct().ToList();
                    StaffById = db.StaffMaster.Where(s => s.TenantId == tenantId && staffIds.Contains(s.StaffId)).ToList().GroupBy(s => s.StaffId).ToDictionary(g => g.Key, g => g.First());
                }
                else
                {
                    SourceStaff = Enumerable.Empty<StaffCoursesectionSchedule>().ToLookup(r => r.CourseSectionId);
                    StaffById = new Dictionary<int, StaffMaster>();
                }

                TargetOccupancy = LoadOccupancy(db, tenantId, schoolId, targetSections);

                NextSectionId = (db.CourseSection.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).Max(s => (int?)s.CourseSectionId) ?? 0) + 1;
                NextFixedSerial = (db.CourseFixedSchedule.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).Max(s => (int?)s.Serial) ?? 0) + 1;
                NextVariableSerial = (db.CourseVariableSchedule.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).Max(s => (int?)s.Serial) ?? 0) + 1;
                NextCalendarSerial = (db.CourseCalendarSchedule.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).Max(s => (int?)s.Serial) ?? 0) + 1;
                NextBlockSerial = (db.CourseBlockSchedule.Where(s => s.TenantId == tenantId && s.SchoolId == schoolId).Max(s => (int?)s.Serial) ?? 0) + 1;
            }

            public CRMContext Db { get; }
            public CloneCourseSectionsViewModel Vm { get; }
            public SchoolCalendars SourceCalendar { get; }
            public SchoolCalendars TargetCalendar { get; }
            public decimal TargetYear { get; }
            public string TargetYearLabel => TargetYear.ToString("0");
            public int YearShift { get; }
            public string? CreatedBy { get; }

            public Dictionary<string, Course> TargetCoursesByTitle { get; }
            public HashSet<string> TargetSectionNames { get; }
            public Dictionary<int, SchoolCalendars> CalendarsById { get; }
            public Dictionary<string, SchoolCalendars> TargetCalendarsByTitle { get; }
            public Dictionary<int, GradeScale> GradeScalesById { get; }
            public Dictionary<string, GradeScale> TargetGradeScalesByName { get; }
            public Dictionary<int, AttendanceCodeCategories> AttendanceCategoriesById { get; }
            public Dictionary<string, AttendanceCodeCategories> TargetAttendanceCategoriesByTitle { get; }
            public Dictionary<int, Rooms> RoomsById { get; }
            public Dictionary<string, Rooms> TargetRoomsByTitle { get; }
            public Dictionary<int, Block> BlocksById { get; }
            public Dictionary<string, Block> TargetBlocksByTitle { get; }
            public Dictionary<int, BlockPeriod> PeriodsById { get; }
            public List<BlockPeriod> TargetPeriods { get; }
            public Dictionary<int, SchoolYears> YearsById { get; }
            public Dictionary<string, SchoolYears> TargetYearsByTitle { get; }
            public SchoolYears? TargetSchoolYear { get; }
            public List<SchoolYears> Years { get; }
            public List<Semesters> SemesterList { get; }
            public List<Quarters> QuarterList { get; }
            public List<ProgressPeriods> ProgressPeriodList { get; }

            public SchoolYears? MatchYear(SchoolYears source) => Match(source, TargetYearsByTitle, Years, y => y.Title, y => y.AcademicYear, y => y.StartDate, y => y.MarkingPeriodId);
            public Semesters? MatchSemester(Semesters source) => Match(source, TargetSemestersByTitle, SemesterList, s => s.Title, s => s.AcademicYear, s => s.StartDate, s => s.MarkingPeriodId);
            public Quarters? MatchQuarter(Quarters source) => Match(source, TargetQuartersByTitle, QuarterList, q => q.Title, q => q.AcademicYear, q => q.StartDate, q => q.MarkingPeriodId);
            public ProgressPeriods? MatchProgressPeriod(ProgressPeriods source) => Match(source, TargetProgressPeriodsByTitle, ProgressPeriodList, p => p.Title, p => p.AcademicYear, p => p.StartDate, p => p.MarkingPeriodId);

            /// <summary>
            /// Finds the target-year marking period for a source one: by title first, otherwise the one at the
            /// same position (by start date) when both years have the same number of periods at that level.
            /// Titles often carry the year ("School Year 2025-2026"), so a title miss alone is not a mismatch.
            /// </summary>
            private T? Match<T>(T source, IReadOnlyDictionary<string, T> targetByTitle, List<T> all, Func<T, string?> title, Func<T, decimal?> year, Func<T, DateTime?> start, Func<T, int> id) where T : class
            {
                if (targetByTitle.TryGetValue(NormalizeTitle(title(source)), out var byTitle))
                {
                    return byTitle;
                }
                var sourceYear = year(source);
                var sourceList = all.Where(x => year(x) == sourceYear).OrderBy(start).ThenBy(id).ToList();
                var targetList = all.Where(x => year(x) == TargetYear).OrderBy(start).ThenBy(id).ToList();
                if (sourceList.Count == 0 || sourceList.Count != targetList.Count)
                {
                    return null;
                }
                var index = sourceList.FindIndex(x => id(x) == id(source));
                return index < 0 ? null : targetList[index];
            }
            public Dictionary<int, Semesters> SemestersById { get; }
            public Dictionary<string, Semesters> TargetSemestersByTitle { get; }
            public Dictionary<int, Quarters> QuartersById { get; }
            public Dictionary<string, Quarters> TargetQuartersByTitle { get; }
            public Dictionary<int, ProgressPeriods> ProgressPeriodsById { get; }
            public Dictionary<string, ProgressPeriods> TargetProgressPeriodsByTitle { get; }
            public ILookup<int, CourseFixedSchedule> SourceFixed { get; }
            public ILookup<int, CourseVariableSchedule> SourceVariable { get; }
            public ILookup<int, CourseCalendarSchedule> SourceCalendarRows { get; }
            public ILookup<int, CourseBlockSchedule> SourceBlock { get; }
            public ILookup<int, StaffCoursesectionSchedule> SourceStaff { get; }
            public Dictionary<int, StaffMaster> StaffById { get; }
            public List<Occupancy> TargetOccupancy { get; }

            public int NextSectionId { get; private set; }
            public int NextFixedSerial { get; private set; }
            public int NextVariableSerial { get; private set; }
            public int NextCalendarSerial { get; private set; }
            public int NextBlockSerial { get; private set; }

            /// <summary>Advances the id counters once a section has been accepted.</summary>
            public void CommitCounters(ScheduleRows rows)
            {
                NextSectionId++;
                if (rows.Fixed != null) NextFixedSerial++;
                NextVariableSerial += rows.Variable.Count;
                NextCalendarSerial += rows.Calendar.Count;
                NextBlockSerial += rows.Block.Count;
            }

            /// <summary>
            /// Names the target-year section already using the same room and period at
            /// the same time, or null when the slot is free or the section allows room
            /// conflicts. Block rows only clash with block rows, the other kinds with
            /// each other on a shared day, mirroring the add-section checks.
            /// </summary>
            public string? FindConflict(bool isBlock, int? roomId, int? periodId, string? days, int? blockId, DateTime? start, DateTime? end, bool? allowRoomConflict)
            {
                if (allowRoomConflict == true || roomId == null || periodId == null)
                {
                    return null;
                }
                foreach (var booked in TargetOccupancy)
                {
                    if (booked.RoomId != roomId || booked.PeriodId != periodId)
                    {
                        continue;
                    }
                    if (!DurationsOverlap(start, end, booked.Start, booked.End))
                    {
                        continue;
                    }
                    var bookedIsBlock = booked.Days == null;
                    if (isBlock != bookedIsBlock)
                    {
                        continue;
                    }
                    var clash = isBlock ? booked.BlockId == blockId : DaysOverlap(days, booked.Days);
                    if (clash)
                    {
                        var room = RoomsById.TryGetValue(roomId.Value, out var r) ? r.Title : roomId.ToString();
                        var period = PeriodsById.TryGetValue(periodId.Value, out var p) ? p.PeriodTitle : periodId.ToString();
                        return $"Room '{room}' is already booked for period '{period}' by section '{booked.SectionName}'.";
                    }
                }
                return null;
            }

            private static List<Occupancy> LoadOccupancy(CRMContext db, Guid tenantId, int schoolId, List<CourseSection> targetSections)
            {
                var occupancy = new List<Occupancy>();
                if (targetSections.Count == 0)
                {
                    return occupancy;
                }
                var ids = targetSections.Select(s => s.CourseSectionId).ToList();
                var byId = targetSections.GroupBy(s => s.CourseSectionId).ToDictionary(g => g.Key, g => g.First());

                foreach (var row in db.CourseFixedSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && ids.Contains(r.CourseSectionId)).ToList())
                {
                    var s = byId[row.CourseSectionId];
                    occupancy.Add(new Occupancy { SectionName = s.CourseSectionName, RoomId = row.RoomId, PeriodId = row.PeriodId, BlockId = row.BlockId, Days = s.MeetingDays ?? "", Start = s.DurationStartDate, End = s.DurationEndDate });
                }
                foreach (var row in db.CourseVariableSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && ids.Contains(r.CourseSectionId)).ToList())
                {
                    var s = byId[row.CourseSectionId];
                    occupancy.Add(new Occupancy { SectionName = s.CourseSectionName, RoomId = row.RoomId, PeriodId = row.PeriodId, BlockId = row.BlockId, Days = row.Day ?? "", Start = s.DurationStartDate, End = s.DurationEndDate });
                }
                foreach (var row in db.CourseCalendarSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && ids.Contains(r.CourseSectionId)).ToList())
                {
                    var s = byId[row.CourseSectionId];
                    occupancy.Add(new Occupancy { SectionName = s.CourseSectionName, RoomId = row.RoomId, PeriodId = row.PeriodId, BlockId = row.BlockId, Days = row.Date?.DayOfWeek.ToString() ?? "", Start = row.Date, End = row.Date });
                }
                foreach (var row in db.CourseBlockSchedule.Where(r => r.TenantId == tenantId && r.SchoolId == schoolId && ids.Contains(r.CourseSectionId)).ToList())
                {
                    var s = byId[row.CourseSectionId];
                    occupancy.Add(new Occupancy { SectionName = s.CourseSectionName, RoomId = row.RoomId, PeriodId = row.PeriodId, BlockId = row.BlockId, Days = null, Start = s.DurationStartDate, End = s.DurationEndDate });
                }
                return occupancy;
            }

            private static Dictionary<string, T> ByTitle<T>(IEnumerable<T> items, Func<T, string?> title)
            {
                var result = new Dictionary<string, T>();
                foreach (var item in items)
                {
                    var key = NormalizeTitle(title(item));
                    if (key.Length > 0 && !result.ContainsKey(key))
                    {
                        result[key] = item;
                    }
                }
                return result;
            }
        }
    }
}
