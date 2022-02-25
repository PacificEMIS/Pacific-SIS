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

                    var staffCourseSectionScheduleData = context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(x => x.IsDropped != true && yesterdayDate >= x.DurationStartDate && yesterdayDate <= x.DurationEndDate).ToList();

                    var allCourseSectionVewListData = context?.AllCourseSectionView.ToList();

                    if (staffCourseSectionScheduleData != null && staffCourseSectionScheduleData.Any() == true)
                    {
                        List<DateTime> holidayList = new List<DateTime>();

                        var BlockPeriodList = context?.BlockPeriod.ToList();

                        var schoolWiseCourseSectionData = staffCourseSectionScheduleData.Select(s => new { s.SchoolId, s.CourseSectionId }).Distinct();

                        foreach (var schoolWiseCourseSection in schoolWiseCourseSectionData)
                        {
                            var staffCourseSectionData = staffCourseSectionScheduleData.Where(x => x.SchoolId == schoolWiseCourseSection.SchoolId && x.CourseSectionId == schoolWiseCourseSection.CourseSectionId).FirstOrDefault();

                            if (staffCourseSectionData != null)
                            {
                                var allCourseSectionVewList = allCourseSectionVewListData!.Where(e => e.SchoolId == staffCourseSectionData.SchoolId && e.CourseSectionId == staffCourseSectionData.CourseSectionId && (e.AttendanceTaken == true || e.TakeAttendanceCalendar == true || e.TakeAttendanceVariable == true || e.TakeAttendanceBlock == true)).ToList();//fetch data from view for this cs

                                if (allCourseSectionVewList.Any())
                                {
                                    //fetch event & holiday.
                                    var CalendarEventsData = context?.CalendarEvents.Where(e => e.TenantId == staffCourseSectionData.TenantId && e.CalendarId == allCourseSectionVewList.FirstOrDefault()!.CalendarId && (e.StartDate >= staffCourseSectionData.DurationStartDate && e.StartDate <= staffCourseSectionData.DurationEndDate || e.EndDate >= staffCourseSectionData.DurationStartDate && e.EndDate <= staffCourseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == staffCourseSectionData.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                    if (CalendarEventsData != null && CalendarEventsData.Any())
                                    {
                                        foreach (var calender in CalendarEventsData)
                                        {
                                            if (calender.EndDate!.Value.Date > calender.StartDate!.Value.Date)
                                            {
                                                var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                                   .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                                   .ToList();
                                                holidayList.AddRange(date);
                                            }
                                            holidayList.Add(calender.StartDate.Value.Date);
                                        }
                                    }

                                    if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                    {
                                        List<DateTime> dateList = new List<DateTime>();
                                        List<string> list = new List<string>();
                                        string[] meetingDays = { };

                                        DateTime start = (DateTime)staffCourseSectionData.DurationStartDate!;
                                        DateTime end = (DateTime)staffCourseSectionData.DurationEndDate!;

                                        meetingDays = staffCourseSectionData.MeetingDays!.ToLower().Split("|");

                                        bool allDays = meetingDays == null || !meetingDays.Any();

                                        dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                             .Select(offset => start.AddDays(offset))
                                                             .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                             .ToList();

                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date >= staffCourseSectionData.DurationStartDate && s.Date <= staffCourseSectionData.DurationEndDate)).ToList();

                                        if (dateList.Count > 0)
                                        {
                                            //get datelist upto yesterday date.
                                            dateList = dateList.Where(s => dateList.Any(secL => s.Date <= yesterdayDate)).ToList();
                                            //Remove Holiday
                                            dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
                                        }

                                        foreach (var date in dateList)
                                        {
                                            var StudentCoursesectionScheduleData = context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                            if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                            {
                                                if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                                {
                                                    var staffAttendanceData = context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId);

                                                    if (staffAttendanceData?.Any() == false)
                                                    {
                                                        var dataExitsInMA = context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId && x.MissingAttendanceDate == date).FirstOrDefault();
                                                        if (dataExitsInMA == null)
                                                        {
                                                            var missingAttendance = new StudentMissingAttendance
                                                            {
                                                                TenantId = staffCourseSectionData.TenantId,
                                                                SchoolId = staffCourseSectionData.SchoolId,
                                                                MissingAttendanceId = (int)missingAttendanceId,
                                                                StaffId = staffCourseSectionData.StaffId,
                                                                BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId && e.SchoolId == staffCourseSectionData.SchoolId)?.BlockId : null,
                                                                CourseId = staffCourseSectionData.CourseId,
                                                                CourseSectionId = staffCourseSectionData.CourseSectionId,
                                                                PeriodId = allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId,
                                                                AttendanceCategoryId = null,
                                                                AttendanceCode = null,
                                                                MissingAttendanceDate = date,
                                                            };
                                                            studentMissingAttendances.Add(missingAttendance);
                                                            missingAttendanceId++;
                                                        }
                                                    }
                                                }
                                            }

                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                            {
                                                var courseVariableScheduleData = allCourseSectionVewList.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                                if (courseVariableScheduleData != null)
                                                {
                                                    foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                    {
                                                        var staffAttendanceData = context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                        if (staffAttendanceData?.Any() == false)
                                                        {
                                                            var dataExitsInMA = context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.PeriodId == courseVariableSchedule.VarPeriodId && x.MissingAttendanceDate == date).FirstOrDefault();
                                                            if (dataExitsInMA == null)
                                                            {
                                                                var missingAttendance = new StudentMissingAttendance
                                                                {
                                                                    TenantId = staffCourseSectionData.TenantId,
                                                                    SchoolId = staffCourseSectionData.SchoolId,
                                                                    MissingAttendanceId = (int)missingAttendanceId,
                                                                    StaffId = staffCourseSectionData.StaffId,
                                                                    BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == courseVariableSchedule.VarPeriodId && e.SchoolId == staffCourseSectionData.SchoolId)?.BlockId : null,
                                                                    CourseId = staffCourseSectionData.CourseId,
                                                                    CourseSectionId = staffCourseSectionData.CourseSectionId,
                                                                    PeriodId = courseVariableSchedule.VarPeriodId,
                                                                    AttendanceCategoryId = null,
                                                                    AttendanceCode = null,
                                                                    MissingAttendanceDate = date
                                                                };
                                                                studentMissingAttendances.Add(missingAttendance);
                                                                missingAttendanceId++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    else if (staffCourseSectionData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                    {
                                        if (allCourseSectionVewList.Count > 0)
                                        {
                                            var calenderScheduleList = allCourseSectionVewList.Where(c => c.CalDate >= staffCourseSectionData.DurationStartDate && c.CalDate <= staffCourseSectionData.DurationEndDate && c.CalDate <= yesterdayDate.Date && !holidayList.Contains(c.CalDate.Value.Date));

                                            if (calenderScheduleList.ToList().Count > 0)
                                            {
                                                foreach (var calenderSchedule in calenderScheduleList)
                                                {
                                                    var StudentCoursesectionScheduleData = context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section 

                                                    if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                                    {
                                                        var staffAttendanceData = context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                                        if (staffAttendanceData?.Any() == false)
                                                        {
                                                            var dataExitsInMA = context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.PeriodId == calenderSchedule.CalPeriodId && x.MissingAttendanceDate == calenderSchedule.CalDate).FirstOrDefault();
                                                            if (dataExitsInMA == null)
                                                            {
                                                                var missingAttendance = new StudentMissingAttendance
                                                                {
                                                                    TenantId = staffCourseSectionData.TenantId,
                                                                    SchoolId = staffCourseSectionData.SchoolId,
                                                                    MissingAttendanceId = (int)missingAttendanceId,
                                                                    StaffId = staffCourseSectionData.StaffId,
                                                                    BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == calenderSchedule.CalPeriodId && e.SchoolId == staffCourseSectionData.SchoolId)?.BlockId : null,
                                                                    CourseId = staffCourseSectionData.CourseId,
                                                                    CourseSectionId = staffCourseSectionData.CourseSectionId,
                                                                    PeriodId = calenderSchedule.CalPeriodId,
                                                                    AttendanceCategoryId = null,
                                                                    AttendanceCode = null,
                                                                    MissingAttendanceDate = calenderSchedule.CalDate
                                                                };
                                                                studentMissingAttendances.Add(missingAttendance);
                                                                missingAttendanceId++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                                    {
                                        var blockScheduleData = allCourseSectionVewList?.Where(v => v.SchoolId == staffCourseSectionData.SchoolId && v.TenantId == staffCourseSectionData.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();

                                        if (blockScheduleData != null && blockScheduleData.Any())
                                        {
                                            foreach (var blockSchedule in blockScheduleData)
                                            {
                                                var bellScheduleList = context?.BellSchedule.Where(v => v.SchoolId == staffCourseSectionData.SchoolId && v.TenantId == staffCourseSectionData.TenantId && v.BlockId == blockSchedule.BlockId && v.BellScheduleDate >= staffCourseSectionData.DurationStartDate && v.BellScheduleDate <= staffCourseSectionData.DurationEndDate && v.BellScheduleDate <= yesterdayDate && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                                if (bellScheduleList != null && bellScheduleList.Any())
                                                {
                                                    foreach (var bellSchedule in bellScheduleList)
                                                    {
                                                        var StudentCoursesectionScheduleData = context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                        if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                                        {
                                                            var staffAttendanceData = context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == blockSchedule.BlockPeriodId).ToList();

                                                            if (staffAttendanceData?.Any() == false)
                                                            {
                                                                var dataExitsInMA = context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.PeriodId == blockSchedule.BlockPeriodId && x.MissingAttendanceDate == bellSchedule.BellScheduleDate).FirstOrDefault();
                                                                if (dataExitsInMA == null)
                                                                {
                                                                    var missingAttendance = new StudentMissingAttendance
                                                                    {
                                                                        TenantId = staffCourseSectionData.TenantId,
                                                                        SchoolId = staffCourseSectionData.SchoolId,
                                                                        MissingAttendanceId = (int)missingAttendanceId,
                                                                        StaffId = staffCourseSectionData.StaffId,
                                                                        BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == blockSchedule.BlockPeriodId && e.SchoolId == staffCourseSectionData.SchoolId)?.BlockId : null,
                                                                        CourseId = staffCourseSectionData.CourseId,
                                                                        CourseSectionId = staffCourseSectionData.CourseSectionId,
                                                                        PeriodId = blockSchedule.BlockPeriodId,
                                                                        AttendanceCategoryId = null,
                                                                        AttendanceCode = null,
                                                                        MissingAttendanceDate = bellSchedule.BellScheduleDate
                                                                    };
                                                                    studentMissingAttendances.Add(missingAttendance);
                                                                    missingAttendanceId++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
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
    }
}
