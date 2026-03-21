using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.backgroundjob.Models;
using opensis.backgroundjob.ViewModels;
using System;

namespace opensis.backgroundjob
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("process started.");
            UpdateStudentCoursesectionScheduleDropDate();
            UpdateStudentEnrollmmentDropDate();
            AddMissingAttendance();
            UpdateReenrollmentDateForStudent();
            Console.WriteLine("process completed.");
        }


        private static void UpdateStudentCoursesectionScheduleDropDate()
        {
            using (var context = new TableContext())
            {
                int i = 0;
                int? Id = 1;

                var scheduledJobsData = context.ScheduledJobs.Where(x => x.IsActive == true && x.JobScheduleDate!.Value.Date <= DateTime.UtcNow.Date && x.ApiTitle == "GroupDropForScheduledStudent").ToList();

                foreach (var scheduledJob in scheduledJobsData)
                {
                    using (var transaction = context?.Database.BeginTransaction())
                    {
                        var scheduledStudentDropModel = JsonConvert.DeserializeObject<ScheduledStudentDropModel>(scheduledJob.TaskJson!);

                        if (scheduledStudentDropModel != null)
                        {
                            if (i == 0)
                            {
                                var dataExits = context?.ScheduledJobHistories.Where(x => x.TenantId == scheduledStudentDropModel.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = context?.ScheduledJobHistories.Where(x => x.TenantId == scheduledStudentDropModel.TenantId).Max(x => x.JobRunId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }
                                i++;
                            }

                            try
                            {
                                List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new List<StudentCoursesectionSchedule>();

                                if (!string.IsNullOrEmpty(scheduledStudentDropModel.StudentId.ToString()) && scheduledStudentDropModel.StudentId > 0)
                                {
                                    foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                                    {
                                        var studentData = context?.StudentCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId == scheduledStudentDropModel.StudentId && x.CourseSectionId == scheduledStudent.CourseSectionId);

                                        if (studentData != null)
                                        {
                                            studentData.IsDropped = true;

                                            studentCoursesectionScheduleList.Add(studentData);

                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                                    {
                                        var studentData = context?.StudentCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudent.SchoolId && x.TenantId == scheduledStudent.TenantId && x.StudentId == scheduledStudent.StudentId && x.CourseSectionId == scheduledStudentDropModel.CourseSectionId);

                                        if (studentData != null)
                                        {
                                            studentData.IsDropped = true;

                                            studentCoursesectionScheduleList.Add(studentData);
                                        }
                                    }
                                }

                                context?.StudentCoursesectionSchedule.UpdateRange(studentCoursesectionScheduleList);
                                context?.SaveChanges();
                                transaction?.Commit();

                                //update job status
                                scheduledJob.LastRunStatus = true;
                                scheduledJob.LastRunTime = DateTime.UtcNow;
                                scheduledJob.IsActive = false;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = true,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //update job status
                                scheduledJob.LastRunStatus = false;
                                scheduledJob.LastRunTime = DateTime.UtcNow;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = false,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        private static void UpdateStudentEnrollmmentDropDate()
        {
            using (var context = new TableContext())
            {
                int i = 0;
                int? Id = 1;

                var scheduledJobsData = context.ScheduledJobs.Where(x => x.IsActive == true && x.JobScheduleDate!.Value.Date <= DateTime.UtcNow.Date && x.ApiTitle == "UpdateStudentEnrollment").ToList();

                foreach (var scheduledJob in scheduledJobsData)
                {
                    using (var transaction = context?.Database.BeginTransaction())
                    {
                        var studentEnrollmentListModel = JsonConvert.DeserializeObject<StudentEnrollmentListModel>(scheduledJob.TaskJson!);

                        if (studentEnrollmentListModel != null)
                        {
                            if (i == 0)
                            {
                                var dataExits = context?.ScheduledJobHistories.Where(x => x.TenantId == studentEnrollmentListModel.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = context?.ScheduledJobHistories.Where(x => x.TenantId == studentEnrollmentListModel.TenantId).Max(x => x.JobRunId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }
                                i++;
                            }

                            try
                            {
                                foreach (var studentEnrollmentList in studentEnrollmentListModel.studentEnrollments)
                                {
                                    if (studentEnrollmentList.EnrollmentId > 0)
                                    {
                                        if (studentEnrollmentList.ExitCode != null)
                                        {
                                            var studentExitCode = context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.ExitCode); //fetching enrollemnt code type 

                                            if (studentExitCode!.Type!.ToLower() == "Drop (Transfer)".ToLower())
                                            {
                                                var StudentMasterData = context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentList.TenantId && x.StudentGuid == studentEnrollmentListModel.StudentGuid).ToList();

                                                if (StudentMasterData?.Any() == true)
                                                {
                                                    var studentCurrentSchool = StudentMasterData.FirstOrDefault(x => x.SchoolId == studentEnrollmentList.TransferredSchoolId);
                                                    if (studentCurrentSchool != null)
                                                    {
                                                        studentCurrentSchool.IsActive = true;

                                                        var studentEnrollmentUpdate = context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.StudentId == studentEnrollmentList.StudentId && x.EnrollmentId == studentEnrollmentList.EnrollmentId);
                                                        if (studentEnrollmentUpdate != null)
                                                        {
                                                            studentEnrollmentUpdate.IsActive = false;
                                                        }


                                                        if (studentCurrentSchool.StudentPortalId != null)
                                                        {
                                                            var userMasterData = context?.UserMaster.FirstOrDefault(x => x.EmailAddress == studentCurrentSchool.StudentPortalId && x.TenantId == studentCurrentSchool.TenantId);
                                                            if (userMasterData != null)
                                                            {
                                                                context?.UserMaster.Remove(userMasterData);

                                                                UserMaster userMaster = new UserMaster();
                                                                userMaster.TenantId = studentCurrentSchool.TenantId;
                                                                userMaster.SchoolId = studentCurrentSchool.SchoolId;
                                                                userMaster.UserId = studentCurrentSchool.StudentId;
                                                                userMaster.Name = userMasterData.Name;
                                                                userMaster.EmailAddress = userMasterData.EmailAddress;
                                                                userMaster.PasswordHash = userMasterData.PasswordHash;
                                                                userMaster.LangId = userMasterData.LangId;
                                                                var membershipsId = context?.Membership.Where(x => x.SchoolId == studentCurrentSchool.SchoolId && x.TenantId == studentEnrollmentList.TenantId && x.Profile == "Student").Select(x => x.MembershipId).FirstOrDefault();
                                                                userMaster.MembershipId = (int)membershipsId!;
                                                                userMaster.UpdatedOn = DateTime.UtcNow;
                                                                userMaster.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                                                userMaster.IsActive = true;
                                                                context?.UserMaster.Add(userMaster);

                                                            }
                                                        }
                                                        //context?.SaveChanges();
                                                    }

                                                    var studentOldSchool = StudentMasterData.Where(x => x.SchoolId != studentEnrollmentList.TransferredSchoolId).ToList();
                                                    if (studentOldSchool?.Any() == true)
                                                    {
                                                        studentOldSchool.ForEach(x => x.IsActive = false);

                                                        //this foreach for drop student from his scheduled course section
                                                        foreach (var studentSchool in studentOldSchool)
                                                        {
                                                            var studentCourseSection = context?.StudentCoursesectionSchedule.Where(x => x.SchoolId == studentSchool.SchoolId && x.TenantId == studentSchool.TenantId && x.StudentId == studentSchool.StudentId && x.IsDropped != true).ToList();
                                                            if (studentCourseSection?.Any() == true)
                                                            {
                                                                studentCourseSection.ForEach(s => { s.IsDropped = true; s.EffectiveDropDate = studentEnrollmentList.ExitDate; });
                                                            }
                                                        }
                                                    }

                                                    context?.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                if (studentExitCode.Type.ToLower() == "Drop".ToLower())
                                                {
                                                    context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentList.StudentGuid && x.SchoolId == studentEnrollmentList.SchoolId).ToList().ForEach(x => x.IsActive = false);

                                                    var studentCourseSection = context?.StudentCoursesectionSchedule.Where(x => x.SchoolId == studentEnrollmentList.SchoolId && x.TenantId == studentEnrollmentList.TenantId && x.StudentGuid == studentEnrollmentList.StudentGuid && x.IsDropped != true).ToList();
                                                    if (studentCourseSection?.Any() == true)
                                                    {
                                                        studentCourseSection.ForEach(s => { s.IsDropped = true; s.EffectiveDropDate = studentEnrollmentList.ExitDate; });
                                                    }

                                                    context?.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                                transaction?.Commit();

                                //update job status
                                scheduledJob.LastRunStatus = true;
                                scheduledJob.LastRunTime = DateTime.UtcNow;
                                scheduledJob.IsActive = false;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = true,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //update job status
                                scheduledJob.LastRunStatus = false;
                                scheduledJob.LastRunTime = DateTime.UtcNow;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = false,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        private static void AddMissingAttendance()
        {
            using (var context = new TableContext())
            {
                try
                {
                    List<StudentMissingAttendance> studentMissingAttendances = new List<StudentMissingAttendance>();

                    // Upper bound is yesterday; but date generation (below) starts from
                    // DurationStartDate, so the job safely back-fills any days it missed.
                    // This is intentional — do NOT change it to generate only for yesterday.
                    var yesterdayDate = DateTime.Today.AddDays(-1).Date;

                    int? missingAttendanceId = 1;
                    var dataExits = context?.StudentMissingAttendances.Count();

                    if (dataExits > 0)
                    {
                        var maxIde = context?.StudentMissingAttendances.Max(x => x.MissingAttendanceId);
                        if (maxIde != null)
                        {
                            missingAttendanceId = maxIde + 1;
                        }
                    }

                    // Load staff-course sections whose duration includes yesterday
                    var staffCourseSectionScheduleData = context?.StaffCoursesectionSchedule.AsNoTracking().Include(x => x.CourseSection).Where(x => x.IsDropped != true && yesterdayDate >= x.DurationStartDate && yesterdayDate <= x.DurationEndDate).ToList();

                    if (staffCourseSectionScheduleData == null || !staffCourseSectionScheduleData.Any())
                        return;

                    // Collect all relevant tenant/school/courseSectionIds for batch loading
                    var schoolWiseCourseSectionData = staffCourseSectionScheduleData.Select(s => new { s.TenantId, s.SchoolId, s.CourseSectionId }).Distinct().ToList();
                    var allCourseSectionIds = schoolWiseCourseSectionData.Select(x => x.CourseSectionId).Distinct().ToList();
                    var allSchoolIds = schoolWiseCourseSectionData.Select(x => x.SchoolId).Distinct().ToList();

                    // Batch-load AllCourseSectionView for relevant course sections only
                    var allCourseSectionVewListData = context?.AllCourseSectionView.AsNoTracking()
                        .Where(v => allCourseSectionIds.Contains(v.CourseSectionId))
                        .ToList() ?? new List<AllCourseSectionView>();

                    // Batch-load BlockPeriod for relevant schools
                    var blockPeriodList = context?.BlockPeriod.AsNoTracking()
                        .Where(bp => allSchoolIds.Contains(bp.SchoolId))
                        .ToList() ?? new List<BlockPeriod>();
                    var blockPeriodLookup = blockPeriodList
                        .GroupBy(bp => (bp.SchoolId, (int?)bp.PeriodId))
                        .ToDictionary(g => g.Key, g => g.First().BlockId);

                    // Build set of (SchoolId, PeriodId) where CalculateAttendance is true.
                    // Only generate missing attendance for these periods — periods not flagged
                    // for attendance calculation should not prompt teachers to take attendance.
                    var calculateAttendanceSet = new HashSet<(int, int?)>(
                        blockPeriodList
                            .Where(bp => bp.CalculateAttendance == true)
                            .Select(bp => (bp.SchoolId, (int?)bp.PeriodId)));

                    // Batch-load ALL StudentAttendance for relevant course sections (full duration range)
                    var allStudentAttendance = context?.StudentAttendance.AsNoTracking()
                        .Where(a => allCourseSectionIds.Contains(a.CourseSectionId))
                        .Select(a => new { a.SchoolId, a.CourseSectionId, a.PeriodId, a.AttendanceDate })
                        .ToList();
                    // Key: (SchoolId, CourseSectionId, PeriodId, Date) -> exists
                    var attendanceSet = new HashSet<(int, int, int?, DateTime)>(
                        allStudentAttendance?.Select(a => (a.SchoolId, a.CourseSectionId, (int?)a.PeriodId, a.AttendanceDate))
                        ?? Enumerable.Empty<(int, int, int?, DateTime)>());

                    // Batch-load ALL existing StudentMissingAttendances for relevant course sections
                    var allExistingMissing = context?.StudentMissingAttendances.AsNoTracking()
                        .Where(m => m.CourseSectionId != null && allCourseSectionIds.Contains(m.CourseSectionId.Value))
                        .Select(m => new { m.SchoolId, CourseSectionId = m.CourseSectionId!.Value, m.PeriodId, m.MissingAttendanceDate })
                        .ToList();
                    var missingSet = new HashSet<(int, int, int?, DateTime)>(
                        allExistingMissing?.Select(m => (m.SchoolId, m.CourseSectionId, m.PeriodId, m.MissingAttendanceDate ?? DateTime.MinValue))
                        ?? Enumerable.Empty<(int, int, int?, DateTime)>());

                    // Batch-load student enrollments for relevant course sections
                    // Key: (SchoolId, CourseSectionId) -> earliest EffectiveStartDate
                    var allStudentEnrollments = context?.StudentCoursesectionSchedule.AsNoTracking()
                        .Where(b => allCourseSectionIds.Contains(b.CourseSectionId) && b.EffectiveStartDate != null)
                        .GroupBy(b => new { b.SchoolId, b.CourseSectionId })
                        .Select(g => new { g.Key.SchoolId, g.Key.CourseSectionId, EarliestStart = g.Min(x => x.EffectiveStartDate) })
                        .ToList();
                    var enrollmentLookup = allStudentEnrollments?
                        .ToDictionary(e => (e.SchoolId, e.CourseSectionId), e => e.EarliestStart)
                        ?? new Dictionary<(int, int), DateTime?>();

                    // Batch-load holidays for all relevant calendars
                    var calendarIds = allCourseSectionVewListData.Where(v => v.CalendarId != null).Select(v => v.CalendarId).Distinct().ToList();
                    var allCalendarEvents = context?.CalendarEvents.AsNoTracking()
                        .Where(e => calendarIds.Contains(e.CalendarId) && e.IsHoliday == true)
                        .ToList() ?? new List<CalendarEvents>();

                    // Batch-load BellSchedule for block schedule types (full duration range up to yesterday)
                    var blockIds = allCourseSectionVewListData.Where(v => v.BlockId != null).Select(v => v.BlockId).Distinct().ToList();
                    var allBellSchedules = blockIds.Any()
                        ? context?.BellSchedule.AsNoTracking()
                            .Where(bs => blockIds.Contains(bs.BlockId) && bs.BellScheduleDate <= yesterdayDate)
                            .ToList() ?? new List<BellSchedule>()
                        : new List<BellSchedule>();

                    // Batch-load marking period tables to exclude dates outside active marking periods.
                    // A course section is assigned to one level (Year/Semester/Quarter/ProgressPeriod).
                    // We walk down to the leaf-level children to find the actual instructional date ranges,
                    // so gaps between semesters, quarters, etc. are automatically excluded.
                    var allSchoolYears = context?.SchoolYears.AsNoTracking()
                        .Where(y => allSchoolIds.Contains(y.SchoolId))
                        .ToList() ?? new List<SchoolYears>();
                    var allSemesters = context?.Semesters.AsNoTracking()
                        .Where(s => allSchoolIds.Contains(s.SchoolId))
                        .ToList() ?? new List<Semesters>();
                    var allQuarters = context?.Quarters.AsNoTracking()
                        .Where(q => allSchoolIds.Contains(q.SchoolId))
                        .ToList() ?? new List<Quarters>();
                    var allProgressPeriods = context?.ProgressPeriods.AsNoTracking()
                        .Where(p => allSchoolIds.Contains(p.SchoolId))
                        .ToList() ?? new List<ProgressPeriods>();

                    // Build a lookup: (SchoolId, CourseSectionId) -> list of valid (StartDate, EndDate) ranges.
                    // Each course section gets its marking period resolved to leaf-level date ranges.
                    var markingPeriodRangesLookup = new Dictionary<(int SchoolId, int CourseSectionId), List<(DateTime Start, DateTime End)>>();

                    foreach (var csv in allCourseSectionVewListData.GroupBy(v => new { v.SchoolId, v.CourseSectionId }).Select(g => g.First()))
                    {
                        var ranges = GetLeafMarkingPeriodRanges(
                            csv.SchoolId, csv.YrMarkingPeriodId, csv.SmstrMarkingPeriodId,
                            csv.QtrMarkingPeriodId, csv.PrgrsprdMarkingPeriodId,
                            allSchoolYears, allSemesters, allQuarters, allProgressPeriods);
                        if (ranges.Any())
                            markingPeriodRangesLookup[(csv.SchoolId, csv.CourseSectionId)] = ranges;
                    }

                    foreach (var schoolWiseCourseSection in schoolWiseCourseSectionData)
                    {
                        var staffCourseSectionData = staffCourseSectionScheduleData.FirstOrDefault(x => x.SchoolId == schoolWiseCourseSection.SchoolId && x.CourseSectionId == schoolWiseCourseSection.CourseSectionId);

                        if (staffCourseSectionData == null)
                            continue;

                        var allCourseSectionVewList = allCourseSectionVewListData.Where(e => e.SchoolId == staffCourseSectionData.SchoolId && e.CourseSectionId == staffCourseSectionData.CourseSectionId && (e.AttendanceTaken == true || e.TakeAttendanceCalendar == true || e.TakeAttendanceVariable == true || e.TakeAttendanceBlock == true)).ToList();

                        if (!allCourseSectionVewList.Any())
                            continue;

                        // Build holiday set for this course section's calendar
                        var calendarId = allCourseSectionVewList.First().CalendarId;
                        var relevantHolidays = allCalendarEvents
                            .Where(e => e.CalendarId == calendarId && (e.SchoolId == staffCourseSectionData.SchoolId || e.ApplicableToAllSchool == true))
                            .ToList();

                        var holidaySet = new HashSet<DateTime>();
                        foreach (var calEvent in relevantHolidays)
                        {
                            if (calEvent.StartDate != null)
                            {
                                if (calEvent.EndDate != null && calEvent.EndDate.Value.Date > calEvent.StartDate.Value.Date)
                                {
                                    for (var d = calEvent.StartDate.Value.Date; d <= calEvent.EndDate.Value.Date; d = d.AddDays(1))
                                        holidaySet.Add(d);
                                }
                                holidaySet.Add(calEvent.StartDate.Value.Date);
                            }
                        }

                        // Check students are enrolled in this section
                        if (!enrollmentLookup.ContainsKey((staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId)))
                            continue;

                        // Get marking period date ranges for this course section.
                        // If ranges exist, only generate missing attendance for dates within them.
                        // This excludes gaps between semesters, quarters, etc.
                        markingPeriodRangesLookup.TryGetValue(
                            (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId),
                            out var mpRanges);

                        if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            // Build date list from DurationStartDate to yesterday, matching meeting days
                            DateTime start = (DateTime)staffCourseSectionData.DurationStartDate!;
                            DateTime end = (DateTime)staffCourseSectionData.DurationEndDate!;
                            if (end > yesterdayDate) end = yesterdayDate;

                            var meetingDays = staffCourseSectionData.MeetingDays?.ToLower().Split("|");
                            bool allDays = meetingDays == null || !meetingDays.Any();

                            var dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                .Select(offset => start.AddDays(offset))
                                .Where(d => (allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                    && !holidaySet.Contains(d)
                                    && IsWithinMarkingPeriod(d, mpRanges))
                                .ToList();

                            foreach (var date in dateList)
                            {
                                // Check if students were enrolled by this date
                                var earliestStart = enrollmentLookup[(staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId)];
                                if (earliestStart != null && earliestStart.Value.Date > date)
                                    continue;

                                if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                {
                                    var fixedPeriodId = allCourseSectionVewList.First().FixedPeriodId;

                                    // Skip periods not flagged for attendance calculation
                                    if (!calculateAttendanceSet.Contains((staffCourseSectionData.SchoolId, fixedPeriodId)))
                                        continue;

                                    var attendanceKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, fixedPeriodId, date);
                                    var missingKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, fixedPeriodId, date);

                                    if (!attendanceSet.Contains(attendanceKey) && !missingSet.Contains(missingKey))
                                    {
                                        blockPeriodLookup.TryGetValue((staffCourseSectionData.SchoolId, fixedPeriodId), out var blockId);
                                        studentMissingAttendances.Add(new StudentMissingAttendance
                                        {
                                            TenantId = staffCourseSectionData.TenantId,
                                            SchoolId = staffCourseSectionData.SchoolId,
                                            MissingAttendanceId = (int)missingAttendanceId!,
                                            StaffId = staffCourseSectionData.StaffId,
                                            BlockId = blockId,
                                            CourseId = staffCourseSectionData.CourseId,
                                            CourseSectionId = staffCourseSectionData.CourseSectionId,
                                            PeriodId = fixedPeriodId,
                                            AttendanceCategoryId = null,
                                            AttendanceCode = null,
                                            MissingAttendanceDate = date,
                                        });
                                        missingAttendanceId++;
                                    }
                                }
                                else // Variable Schedule (2)
                                {
                                    var courseVariableScheduleData = allCourseSectionVewList.Where(e => e.VarDay != null && e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                    foreach (var courseVariableSchedule in courseVariableScheduleData)
                                    {
                                        // Skip periods not flagged for attendance calculation
                                        if (!calculateAttendanceSet.Contains((staffCourseSectionData.SchoolId, courseVariableSchedule.VarPeriodId)))
                                            continue;

                                        var attendanceKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, courseVariableSchedule.VarPeriodId, date);
                                        var missingKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, courseVariableSchedule.VarPeriodId, date);

                                        if (!attendanceSet.Contains(attendanceKey) && !missingSet.Contains(missingKey))
                                        {
                                            blockPeriodLookup.TryGetValue((staffCourseSectionData.SchoolId, courseVariableSchedule.VarPeriodId), out var blockId);
                                            studentMissingAttendances.Add(new StudentMissingAttendance
                                            {
                                                TenantId = staffCourseSectionData.TenantId,
                                                SchoolId = staffCourseSectionData.SchoolId,
                                                MissingAttendanceId = (int)missingAttendanceId!,
                                                StaffId = staffCourseSectionData.StaffId,
                                                BlockId = blockId,
                                                CourseId = staffCourseSectionData.CourseId,
                                                CourseSectionId = staffCourseSectionData.CourseSectionId,
                                                PeriodId = courseVariableSchedule.VarPeriodId,
                                                AttendanceCategoryId = null,
                                                AttendanceCode = null,
                                                MissingAttendanceDate = date,
                                            });
                                            missingAttendanceId++;
                                        }
                                    }
                                }
                            }
                        }
                        else if (staffCourseSectionData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                        {
                            var calenderScheduleList = allCourseSectionVewList.Where(c => c.CalDate != null && c.CalDate.Value.Date >= staffCourseSectionData.DurationStartDate && c.CalDate.Value.Date <= staffCourseSectionData.DurationEndDate && c.CalDate.Value.Date <= yesterdayDate && !holidaySet.Contains(c.CalDate.Value.Date) && IsWithinMarkingPeriod(c.CalDate.Value.Date, mpRanges));

                            foreach (var calenderSchedule in calenderScheduleList)
                            {
                                // Skip periods not flagged for attendance calculation
                                if (!calculateAttendanceSet.Contains((staffCourseSectionData.SchoolId, calenderSchedule.CalPeriodId)))
                                    continue;

                                // Check if students were enrolled by this date
                                var earliestStart = enrollmentLookup[(staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId)];
                                if (earliestStart != null && earliestStart.Value.Date > calenderSchedule.CalDate!.Value.Date)
                                    continue;

                                var attendanceKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, calenderSchedule.CalPeriodId, calenderSchedule.CalDate!.Value.Date);
                                var missingKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, calenderSchedule.CalPeriodId, calenderSchedule.CalDate!.Value.Date);

                                if (!attendanceSet.Contains(attendanceKey) && !missingSet.Contains(missingKey))
                                {
                                    blockPeriodLookup.TryGetValue((staffCourseSectionData.SchoolId, calenderSchedule.CalPeriodId), out var blockId);
                                    studentMissingAttendances.Add(new StudentMissingAttendance
                                    {
                                        TenantId = staffCourseSectionData.TenantId,
                                        SchoolId = staffCourseSectionData.SchoolId,
                                        MissingAttendanceId = (int)missingAttendanceId!,
                                        StaffId = staffCourseSectionData.StaffId,
                                        BlockId = blockId,
                                        CourseId = staffCourseSectionData.CourseId,
                                        CourseSectionId = staffCourseSectionData.CourseSectionId,
                                        PeriodId = calenderSchedule.CalPeriodId,
                                        AttendanceCategoryId = null,
                                        AttendanceCode = null,
                                        MissingAttendanceDate = calenderSchedule.CalDate,
                                    });
                                    missingAttendanceId++;
                                }
                            }
                        }
                        else if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            var blockScheduleData = allCourseSectionVewList.Where(v => v.SchoolId == staffCourseSectionData.SchoolId && v.TenantId == staffCourseSectionData.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();

                            foreach (var blockSchedule in blockScheduleData)
                            {
                                // Get all bell schedule dates for this block within the duration range
                                var bellScheduleDates = allBellSchedules.Where(bs => bs.SchoolId == staffCourseSectionData.SchoolId && bs.TenantId == staffCourseSectionData.TenantId && bs.BlockId == blockSchedule.BlockId && bs.BellScheduleDate >= staffCourseSectionData.DurationStartDate && bs.BellScheduleDate <= staffCourseSectionData.DurationEndDate && !holidaySet.Contains(bs.BellScheduleDate) && IsWithinMarkingPeriod(bs.BellScheduleDate, mpRanges)).ToList();

                                foreach (var bellSchedule in bellScheduleDates)
                                {
                                    // Skip periods not flagged for attendance calculation
                                    if (!calculateAttendanceSet.Contains((staffCourseSectionData.SchoolId, blockSchedule.BlockPeriodId)))
                                        continue;

                                    // Check if students were enrolled by this date
                                    var earliestStart = enrollmentLookup[(staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId)];
                                    if (earliestStart != null && earliestStart.Value.Date > bellSchedule.BellScheduleDate.Date)
                                        continue;

                                    var attendanceKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, blockSchedule.BlockPeriodId, bellSchedule.BellScheduleDate);
                                    var missingKey = (staffCourseSectionData.SchoolId, staffCourseSectionData.CourseSectionId, blockSchedule.BlockPeriodId, bellSchedule.BellScheduleDate);

                                    if (!attendanceSet.Contains(attendanceKey) && !missingSet.Contains(missingKey))
                                    {
                                        blockPeriodLookup.TryGetValue((staffCourseSectionData.SchoolId, blockSchedule.BlockPeriodId), out var blockId);
                                        studentMissingAttendances.Add(new StudentMissingAttendance
                                        {
                                            TenantId = staffCourseSectionData.TenantId,
                                            SchoolId = staffCourseSectionData.SchoolId,
                                            MissingAttendanceId = (int)missingAttendanceId!,
                                            StaffId = staffCourseSectionData.StaffId,
                                            BlockId = blockId,
                                            CourseId = staffCourseSectionData.CourseId,
                                            CourseSectionId = staffCourseSectionData.CourseSectionId,
                                            PeriodId = blockSchedule.BlockPeriodId,
                                            AttendanceCategoryId = null,
                                            AttendanceCode = null,
                                            MissingAttendanceDate = bellSchedule.BellScheduleDate,
                                        });
                                        missingAttendanceId++;
                                    }
                                }
                            }
                        }
                    }

                    if (studentMissingAttendances.Any())
                    {
                        context?.StudentMissingAttendances.AddRange(studentMissingAttendances);
                        context?.SaveChanges();
                    }
                }
                catch (Exception es)
                {
                    var msg = es.Message;
                }
            }
        }

        /// <summary>
        /// Resolves the valid instructional date ranges for a course section by walking
        /// the marking period hierarchy down to the leaf level.
        ///
        /// A course section is assigned to exactly one marking period level via one of
        /// YrMarkingPeriodId, SmstrMarkingPeriodId, QtrMarkingPeriodId, or PrgrsprdMarkingPeriodId.
        /// If that level has children (e.g. a Year has Semesters, which have Quarters),
        /// the leaf-level children define the actual instructional date ranges. Gaps between
        /// leaves (e.g. semester breaks, quarter breaks) are excluded.
        /// If the assigned level has no children, its own dates are used.
        /// </summary>
        private static List<(DateTime Start, DateTime End)> GetLeafMarkingPeriodRanges(
            int schoolId, int? yrId, int? smstrId, int? qtrId, int? prgrsprdId,
            List<SchoolYears> allYears, List<Semesters> allSemesters,
            List<Quarters> allQuarters, List<ProgressPeriods> allProgressPeriods)
        {
            var ranges = new List<(DateTime Start, DateTime End)>();

            if (prgrsprdId != null)
            {
                // Assigned at progress period level — this IS the leaf
                var pp = allProgressPeriods.FirstOrDefault(p => p.SchoolId == schoolId && p.MarkingPeriodId == prgrsprdId);
                if (pp?.StartDate != null && pp?.EndDate != null)
                    ranges.Add((pp.StartDate.Value, pp.EndDate.Value));
                return ranges;
            }

            if (qtrId != null)
            {
                // Assigned at quarter level — check for child progress periods
                var childPPs = allProgressPeriods
                    .Where(p => p.SchoolId == schoolId && p.QuarterId == qtrId
                        && p.StartDate != null && p.EndDate != null)
                    .ToList();
                if (childPPs.Any())
                {
                    ranges.AddRange(childPPs.Select(p => (p.StartDate!.Value, p.EndDate!.Value)));
                    return ranges;
                }
                // No children — use the quarter's own dates
                var qtr = allQuarters.FirstOrDefault(q => q.SchoolId == schoolId && q.MarkingPeriodId == qtrId);
                if (qtr?.StartDate != null && qtr?.EndDate != null)
                    ranges.Add((qtr.StartDate.Value, qtr.EndDate.Value));
                return ranges;
            }

            if (smstrId != null)
            {
                // Assigned at semester level — check for child quarters
                var childQtrs = allQuarters
                    .Where(q => q.SchoolId == schoolId && q.SemesterId == smstrId)
                    .ToList();
                if (childQtrs.Any())
                {
                    // Recurse into each quarter to find its leaves
                    foreach (var cq in childQtrs)
                    {
                        var qtrRanges = GetLeafMarkingPeriodRanges(
                            schoolId, null, null, cq.MarkingPeriodId, null,
                            allYears, allSemesters, allQuarters, allProgressPeriods);
                        ranges.AddRange(qtrRanges);
                    }
                    return ranges;
                }
                // No children — use the semester's own dates
                var sem = allSemesters.FirstOrDefault(s => s.SchoolId == schoolId && s.MarkingPeriodId == smstrId);
                if (sem?.StartDate != null && sem?.EndDate != null)
                    ranges.Add((sem.StartDate.Value, sem.EndDate.Value));
                return ranges;
            }

            if (yrId != null)
            {
                // Assigned at year level — check for child semesters
                var childSems = allSemesters
                    .Where(s => s.SchoolId == schoolId && s.YearId == yrId)
                    .ToList();
                if (childSems.Any())
                {
                    // Recurse into each semester to find its leaves
                    foreach (var cs in childSems)
                    {
                        var semRanges = GetLeafMarkingPeriodRanges(
                            schoolId, null, cs.MarkingPeriodId, null, null,
                            allYears, allSemesters, allQuarters, allProgressPeriods);
                        ranges.AddRange(semRanges);
                    }
                    return ranges;
                }
                // No children — use the year's own dates
                var yr = allYears.FirstOrDefault(y => y.SchoolId == schoolId && y.MarkingPeriodId == yrId);
                if (yr?.StartDate != null && yr?.EndDate != null)
                    ranges.Add((yr.StartDate.Value, yr.EndDate.Value));
                return ranges;
            }

            // No marking period assigned — no filtering (return empty = allow all dates)
            return ranges;
        }

        /// <summary>
        /// Returns true if the date falls within at least one of the marking period ranges,
        /// or if no ranges were resolved (no marking period assigned — allow all dates).
        /// </summary>
        private static bool IsWithinMarkingPeriod(DateTime date, List<(DateTime Start, DateTime End)>? ranges)
        {
            if (ranges == null || !ranges.Any())
                return true; // No marking period constraint — allow all dates
            return ranges.Any(r => date >= r.Start && date <= r.End);
        }

        private static void UpdateReenrollmentDateForStudent()
        {
            using (var context = new TableContext())
            {
                int i = 0;
                int? Id = 1;

                var scheduledJobsData = context.ScheduledJobs.Where(x => x.IsActive == true && x.JobScheduleDate!.Value.Date <= DateTime.UtcNow.Date && x.ApiTitle == "ReenrollmentForStudent").ToList();

                foreach (var scheduledJob in scheduledJobsData)
                {
                    using (var transaction = context?.Database.BeginTransaction())
                    {
                        var studentListModel = JsonConvert.DeserializeObject<StudentListModel>(scheduledJob.TaskJson!);

                        if (studentListModel != null)
                        {
                            if (i == 0)
                            {
                                var dataExits = context?.ScheduledJobHistories.Where(x => x.TenantId == studentListModel.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = context?.ScheduledJobHistories.Where(x => x.TenantId == studentListModel.TenantId).Max(x => x.JobRunId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }
                                i++;
                            }

                            try
                            {
                                foreach (var studentData in studentListModel.studentMaster)
                                {
                                    var StudentData = context?.StudentMaster.FirstOrDefault(s => s.TenantId == studentListModel.TenantId && s.SchoolId == studentListModel.SchoolId && s.StudentGuid == studentData.StudentGuid);

                                    if (StudentData != null)
                                    {
                                        StudentData.IsActive = true;

                                        //Student Protal Access
                                        if (studentData.StudentPortalId != null)
                                        {
                                            var userMasterData = context?.UserMaster.FirstOrDefault(x => x.EmailAddress == studentData.StudentPortalId && x.TenantId == studentData.TenantId);
                                            if (userMasterData != null)
                                            {
                                                context?.UserMaster.Remove(userMasterData);

                                                UserMaster userMaster = new UserMaster();
                                                userMaster.TenantId = studentData.TenantId;
                                                userMaster.SchoolId = (int)studentListModel.SchoolId!;
                                                userMaster.UserId = studentData.StudentId;
                                                userMaster.Name = userMasterData.Name;
                                                userMaster.EmailAddress = userMasterData.EmailAddress;
                                                userMaster.PasswordHash = userMasterData.PasswordHash;
                                                userMaster.LangId = userMasterData.LangId;
                                                var membershipsId = context?.Membership.Where(x => x.SchoolId == (int)studentListModel.SchoolId && x.TenantId == studentListModel.TenantId && x.Profile == "Student").Select(x => x.MembershipId).FirstOrDefault();
                                                userMaster.MembershipId = (int)membershipsId!;
                                                userMaster.UpdatedOn = DateTime.UtcNow;
                                                userMaster.UpdatedBy = studentListModel.UpdatedBy;
                                                userMaster.IsActive = true;
                                                context?.UserMaster.Add(userMaster);
                                            }
                                        }
                                    }
                                    context?.SaveChanges();
                                }

                                transaction?.Commit();

                                //update job status
                                scheduledJob.LastRunStatus = true;
                                scheduledJob.LastRunTime = DateTime.UtcNow;
                                scheduledJob.IsActive = false;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = true,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //update job status
                                scheduledJob.LastRunStatus = false;
                                scheduledJob.LastRunTime = DateTime.UtcNow;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = false,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

    }
}
