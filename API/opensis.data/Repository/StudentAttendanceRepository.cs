/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Period;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using opensis.data.ViewModels.StudentAttendances;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StudentAttendanceRepository : IStudentAttendanceRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentAttendanceRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Student Attendance Add/Update
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel AddUpdateStudentAttendance(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<StudentAttendance> studentAttendance = new List<StudentAttendance>();
                    List<StudentDailyAttendance> studentDailyAttendances = new List<StudentDailyAttendance>();
                    List<StudentAttendanceHistory> studentAttendanceHistories = new List<StudentAttendanceHistory>();
                    List<StudentMissingAttendance> studentMissingAttendances = new List<StudentMissingAttendance>();

                    if (studentAttendanceAddViewModel.studentAttendance?.Any() == true)
                    {
                        var checkAttendanceDate = CheckAttendanceDate(studentAttendanceAddViewModel.TenantId, studentAttendanceAddViewModel.SchoolId, studentAttendanceAddViewModel.CourseSectionId, studentAttendanceAddViewModel.AttendanceDate);//check attendance date is valid or not.

                        if (checkAttendanceDate == true)
                        {
                            var attendanceDataExist = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId).ToList();

                            long? StudentAttendanceId = 1;

                            var maxAttendanceId = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).Max(x => (long?)x.StudentAttendanceId);

                            if (maxAttendanceId != null)
                            {
                                StudentAttendanceId = maxAttendanceId + 1;
                            }

                            int? membershipID = null;

                            var staffSchoolInfoData = this.context?.StaffSchoolInfo.Include(x => x.StaffMaster).FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId && c.SchoolId == studentAttendanceAddViewModel.SchoolId && c.StaffId == studentAttendanceAddViewModel.StaffId);

                            if (staffSchoolInfoData != null)
                            {
                                membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && (v.Profile ?? "").ToLower() == (staffSchoolInfoData.Profile ?? "").ToLower())?.MembershipId;
                                //membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && String.Compare(v.Profile, staffSchoolInfoData.Profile, true) == 0)?.MembershipId;
                            }

                            long? CommentId = Utility.GetMaxLongPK<StudentAttendanceComments>(this.context, x => x.CommentId);

                            foreach (var studentAttendances in studentAttendanceAddViewModel.studentAttendance)
                            {
                                if (studentAttendances.StudentAttendanceComments.Count() > 0)
                                {
                                    foreach (var StudentAttendanceComment in studentAttendances.StudentAttendanceComments)
                                    {
                                        StudentAttendanceComment.CommentId = (long)CommentId!;
                                        CommentId++;
                                    }
                                }
                            }

                            long? HistoryCommentId = Utility.GetMaxLongPK<StudentAttendanceHistory>(this.context, x => x.AttendanceHistoryId);

                            if (attendanceDataExist?.Any() == true)
                            {
                                //this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                                var studentAttendanceIDs = attendanceDataExist.Select(v => v.StudentAttendanceId).ToList();

                                var studentAttendanceCommentData = this.context?.StudentAttendanceComments.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && (studentAttendanceIDs == null || (studentAttendanceIDs.Contains(x.StudentAttendanceId))));

                                if (studentAttendanceCommentData?.Any() == true)
                                {
                                    this.context?.StudentAttendanceComments.RemoveRange(studentAttendanceCommentData);
                                }
                                this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                                this.context?.SaveChanges();

                                foreach (var studentAttendancedata in studentAttendanceAddViewModel.studentAttendance.ToList())
                                {
                                    if (studentAttendancedata.AttendanceCode > 0)
                                    {
                                        var studentAttendanceUpdate = new StudentAttendance()
                                        {
                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                            StudentId = studentAttendancedata.StudentId,
                                            StaffId = studentAttendanceAddViewModel.StaffId,
                                            CourseId = studentAttendanceAddViewModel.CourseId,
                                            CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                            AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                            AttendanceCode = studentAttendancedata.AttendanceCode,
                                            AttendanceDate = studentAttendanceAddViewModel.AttendanceDate,
                                            //Comments = studentAttendancedata.Comments,
                                            UpdatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                            UpdatedOn = DateTime.UtcNow,
                                            BlockId = studentAttendancedata.BlockId,
                                            PeriodId = studentAttendanceAddViewModel.PeriodId,
                                            StudentAttendanceId = (int)StudentAttendanceId,
                                            MembershipId = membershipID,
                                            StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                            {
                                                c.UpdatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                                c.UpdatedOn = DateTime.UtcNow;
                                                c.CommentTimestamp = DateTime.UtcNow;
                                                c.MembershipId = membershipID;
                                                return c;
                                            }).ToList()
                                        };
                                        studentAttendance.Add(studentAttendanceUpdate);
                                        //StudentAttendanceId++;

                                        var studentAttendanceHistoryUpdate = new StudentAttendanceHistory()
                                        {
                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                            StudentId = studentAttendancedata.StudentId,
                                            AttendanceHistoryId = (long)HistoryCommentId!,
                                            CourseId = studentAttendanceAddViewModel.CourseId,
                                            CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                            AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                            AttendanceCode = studentAttendancedata.AttendanceCode,
                                            AttendanceDate = studentAttendanceAddViewModel.AttendanceDate,
                                            BlockId = studentAttendancedata.BlockId,
                                            PeriodId = studentAttendanceAddViewModel.PeriodId,
                                            ModifiedBy = studentAttendanceAddViewModel.StaffId,
                                            ModificationTimestamp = DateTime.UtcNow,
                                            MembershipId = membershipID,
                                            UpdatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                            UpdatedOn = DateTime.UtcNow,
                                        };
                                        studentAttendanceHistories.Add(studentAttendanceHistoryUpdate);
                                        StudentAttendanceId++;
                                        HistoryCommentId++;
                                    }
                                }

                                //remove data from StudentMissingAttendances table.
                                var dataExitsInMA = this.context?.StudentMissingAttendances.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.PeriodId == studentAttendanceAddViewModel.PeriodId && x.MissingAttendanceDate == studentAttendanceAddViewModel.AttendanceDate).FirstOrDefault();
                                if (dataExitsInMA != null)
                                {
                                    studentMissingAttendances.Add(dataExitsInMA);
                                }

                                studentAttendanceAddViewModel._message = "Student attendance updated successfully.";
                            }
                            else
                            {
                                foreach (var studentAttendancedata in studentAttendanceAddViewModel.studentAttendance.ToList())
                                {
                                    if (studentAttendancedata.AttendanceCode > 0)
                                    {
                                        var studentAttendanceAdd = new StudentAttendance()
                                        {
                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                            StudentId = studentAttendancedata.StudentId,
                                            StaffId = studentAttendanceAddViewModel.StaffId,
                                            CourseId = studentAttendanceAddViewModel.CourseId,
                                            CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                            AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                            AttendanceCode = studentAttendancedata.AttendanceCode,
                                            AttendanceDate = studentAttendanceAddViewModel.AttendanceDate,
                                            //Comments = studentAttendancedata.Comments,
                                            CreatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                            BlockId = studentAttendancedata.BlockId,
                                            PeriodId = studentAttendanceAddViewModel.PeriodId,
                                            StudentAttendanceId = (int)StudentAttendanceId,
                                            MembershipId = membershipID,
                                            StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                            {
                                                c.UpdatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                                c.UpdatedOn = DateTime.UtcNow;
                                                c.CommentTimestamp = DateTime.UtcNow;
                                                c.MembershipId = membershipID;
                                                return c;
                                            }).ToList()
                                        };
                                        studentAttendance.Add(studentAttendanceAdd);

                                        var studentAttendanceHistoryAdd = new StudentAttendanceHistory()
                                        {
                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                            StudentId = studentAttendancedata.StudentId,
                                            AttendanceHistoryId = (long)HistoryCommentId!,
                                            CourseId = studentAttendanceAddViewModel.CourseId,
                                            CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                            AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                            AttendanceCode = studentAttendancedata.AttendanceCode,
                                            AttendanceDate = studentAttendanceAddViewModel.AttendanceDate,
                                            BlockId = studentAttendancedata.BlockId,
                                            PeriodId = studentAttendanceAddViewModel.PeriodId,
                                            ModifiedBy = studentAttendanceAddViewModel.StaffId,
                                            ModificationTimestamp = DateTime.UtcNow,
                                            MembershipId = membershipID,
                                            CreatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                        };
                                        studentAttendanceHistories.Add(studentAttendanceHistoryAdd);
                                        StudentAttendanceId++;
                                        HistoryCommentId++;
                                    }
                                }

                                //check for remove data from StudentMissingAttendances table.
                                var dataExitsInMA = this.context?.StudentMissingAttendances.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.PeriodId == studentAttendanceAddViewModel.PeriodId && x.MissingAttendanceDate == studentAttendanceAddViewModel.AttendanceDate).FirstOrDefault();
                                if (dataExitsInMA != null)
                                {
                                    studentMissingAttendances.Add(dataExitsInMA);
                                }

                                studentAttendanceAddViewModel._message = "Student attendance added successfully.";
                            }
                            this.context?.StudentAttendance.AddRange(studentAttendance);
                            this.context?.StudentAttendanceHistory.AddRange(studentAttendanceHistories);
                            this.context?.StudentMissingAttendances.RemoveRange(studentMissingAttendances);
                            this.context?.SaveChanges();

                            var studentIdList = studentAttendanceAddViewModel.studentAttendance.Where(s => s.AttendanceCode > 0).Select(x => x.StudentId).ToList();

                            // Batch-load all lookup data before the loop (eliminates N+1 queries)
                            var allAttendanceForDate = this.context?.StudentAttendance
                                .AsNoTracking()
                                .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && studentIdList.Contains(x.StudentId) && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate)
                                .ToList()
                                .GroupBy(x => x.StudentId)
                                .ToDictionary(g => g.Key, g => g.ToList());

                            var blockPeriodLookup = this.context?.BlockPeriod
                                .AsNoTracking()
                                .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                                .ToDictionary(x => (x.BlockId, x.PeriodId));

                            var attendanceCodeList = this.context?.AttendanceCode
                                .AsNoTracking()
                                .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                                .ToList();

                            var existingDailyAttendance = this.context?.StudentDailyAttendance
                                .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && studentIdList.Contains(x.StudentId) && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate)
                                .ToDictionary(x => x.StudentId);

                            foreach (var studentId in studentIdList)
                            {
                                int totalAttendanceMin = 0;
                                if (allAttendanceForDate != null && allAttendanceForDate.TryGetValue(studentId, out var attendanceData))
                                {
                                    foreach (var attendance in attendanceData)
                                    {
                                        if (blockPeriodLookup != null && blockPeriodLookup.TryGetValue((attendance.BlockId, attendance.PeriodId), out var BlockPeriodData))
                                        {
                                            // Only count periods flagged as calculating attendance
                                            if (BlockPeriodData.CalculateAttendance != true)
                                                continue;

                                            var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                            var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                            TimeSpan? periodTime = periodEndTime - periodStartTime;
                                            var hour = Convert.ToInt32(periodTime.Value.Hours);
                                            var min = Convert.ToInt32(periodTime.Value.Minutes);
                                            var classMin = hour > 0 ? (hour * 60 + min) : min;

                                            var AttendanceCodeData = attendanceCodeList?.FirstOrDefault(x => x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                            if (AttendanceCodeData != null)
                                            {
                                                if (AttendanceCodeData.StateCode!.ToLower() != "absent".ToLower())
                                                {
                                                    totalAttendanceMin = totalAttendanceMin + classMin;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (existingDailyAttendance != null && existingDailyAttendance.TryGetValue(studentId, out var studentDailyAttendanceData))
                                {
                                    studentDailyAttendanceData.AttendanceMinutes = totalAttendanceMin;
                                }
                                else
                                {
                                    var studentDailyAttendance = new StudentDailyAttendance { TenantId = studentAttendanceAddViewModel.TenantId, SchoolId = studentAttendanceAddViewModel.SchoolId, StudentId = studentId, AttendanceDate = studentAttendanceAddViewModel.AttendanceDate, CreatedBy = studentAttendanceAddViewModel.CreatedBy, AttendanceMinutes = totalAttendanceMin, CreatedOn = DateTime.UtcNow };
                                    studentDailyAttendances.Add(studentDailyAttendance);
                                }
                            }
                            this.context?.StudentDailyAttendance.AddRange(studentDailyAttendances);
                            this.context?.SaveChanges();
                            transaction?.Commit();
                            studentAttendanceAddViewModel._failure = false;
                            studentAttendanceAddViewModel.studentAttendance.ForEach(x => { x.StudentAttendanceComments.FirstOrDefault()!.Membership = null; x.StudentAttendanceComments.FirstOrDefault()!.StudentAttendance = new(); });
                        }
                        else
                        {
                            studentAttendanceAddViewModel._failure = true;
                            studentAttendanceAddViewModel._message = "Your selected date is not in course section date range";
                        }
                    }

                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentAttendanceAddViewModel._failure = true;
                    studentAttendanceAddViewModel._message = es.Message;
                }
            }
            return studentAttendanceAddViewModel;
        }

        /// <summary>
        /// Get All Student Attendance List
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel GetAllStudentAttendanceList(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            try
            {
                var studentAttendanceData = this.context?.StudentAttendance.AsNoTracking().Include(k => k.Membership).Include(c => c.StudentCoursesectionSchedule).Include(v => v.StudentAttendanceComments).ThenInclude(y => y.Membership).Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId).ToList();

                if (studentAttendanceData?.Any() == true)
                {
                    studentAttendanceAddViewModel.studentAttendance = studentAttendanceData;
                    studentAttendanceAddViewModel._failure = false;
                }
                else
                {
                    studentAttendanceAddViewModel._failure = true;
                    studentAttendanceAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentAttendanceAddViewModel._message = es.Message;
                studentAttendanceAddViewModel._failure = true;
            }
            return studentAttendanceAddViewModel;

        }

        /// <summary>
        /// Search Course Section For Student Attendance
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel SearchCourseSectionForStudentAttendance(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.AsNoTracking().Include(s => s.StaffMaster).Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData?.Any() == true)
                {

                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (scheduledCourseSection.CourseSection.AcademicYear == scheduledCourseSectionViewModel.AcademicYear)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            var courseSectionData = this.context?.CourseSection.AsNoTracking().FirstOrDefault(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId);

                            if (courseSectionData != null)
                            {
                                var CalendarEventsData = this.context?.CalendarEvents.AsNoTracking().Where(e => e.TenantId == scheduledCourseSection.TenantId && e.CalendarId == courseSectionData.CalendarId && (e.StartDate >= courseSectionData.DurationStartDate && e.StartDate <= courseSectionData.DurationEndDate || e.EndDate >= courseSectionData.DurationStartDate && e.EndDate <= courseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSection.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                if (CalendarEventsData?.Any() == true)
                                {
                                    foreach (var calender in CalendarEventsData)
                                    {
                                        if (calender.EndDate!.Value.Date > calender.StartDate!.Value.Date)
                                        {
                                            var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                               .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                               .ToList();
                                            CourseSections.HolidayList.AddRange(date);
                                        }
                                        CourseSections.HolidayList.Add(calender.StartDate.Value.Date);
                                    }
                                    CourseSections.HolidayList.Distinct();
                                }
                            }

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";

                                var courseFixedScheduleData = this.context?.CourseFixedSchedule.AsNoTracking().Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId);
                                if (courseFixedScheduleData != null)
                                {
                                    if (courseFixedScheduleData.BlockPeriod != null)
                                    {
                                        courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                        courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();

                                    }
                                    CourseSections.courseFixedSchedule = courseFixedScheduleData;

                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (courseVariableScheduleData?.Any() == true)
                                {
                                    courseVariableScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = courseVariableScheduleData;
                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (courseCalenderScheduleData?.Any() == true)
                                {
                                    courseCalenderScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (courseBlockScheduleData?.Any() == true)
                                {
                                    courseBlockScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseBlockSchedule = courseBlockScheduleData;
                                }
                            }

                            CourseSections.CourseId = scheduledCourseSection.CourseId;
                            CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                            CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                            CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.PrgrsprdMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                            CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                            CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;

                            scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
                        }
                    }

                    // Bulk-load which (courseSectionId, periodId, date) combos already have attendance
                    if (scheduledCourseSectionView.courseSectionViewList.Any())
                    {
                        var csIds = scheduledCourseSectionView.courseSectionViewList.Select(c => c.CourseSectionId).ToList();

                        var enrolledCounts = this.context?.StudentCoursesectionSchedule
                            .AsNoTracking()
                            .Where(ss => ss.TenantId == scheduledCourseSectionViewModel.TenantId
                                && ss.SchoolId == scheduledCourseSectionViewModel.SchoolId
                                && csIds.Contains(ss.CourseSectionId)
                                && ss.IsDropped != true)
                            .GroupBy(ss => ss.CourseSectionId)
                            .Select(g => new { CourseSectionId = g.Key, Count = g.Count() })
                            .ToDictionary(x => x.CourseSectionId, x => x.Count)
                            ?? new Dictionary<int, int>();

                        scheduledCourseSectionView.AttendanceTakenList = this.context?.StudentAttendance
                            .AsNoTracking()
                            .Where(sa => sa.TenantId == scheduledCourseSectionViewModel.TenantId
                                && sa.SchoolId == scheduledCourseSectionViewModel.SchoolId
                                && csIds.Contains(sa.CourseSectionId))
                            .GroupBy(sa => new { sa.CourseSectionId, sa.PeriodId, sa.AttendanceDate })
                            .Select(g => new AttendanceTakenRecord
                            {
                                CourseSectionId = g.Key.CourseSectionId,
                                PeriodId = g.Key.PeriodId,
                                AttendanceDate = g.Key.AttendanceDate,
                                AttendanceCount = g.Select(sa => sa.StudentId).Distinct().Count()
                            })
                            .ToList() ?? new List<AttendanceTakenRecord>();

                        foreach (var record in scheduledCourseSectionView.AttendanceTakenList)
                        {
                            record.EnrolledCount = enrolledCounts.GetValueOrDefault(record.CourseSectionId ?? 0, 0);
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView.courseSectionViewList = new();
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Add/Update Student Attendance For Student360
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel AddUpdateStudentAttendanceForStudent360(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<StudentAttendance> studentAttendance = new List<StudentAttendance>();
                    List<StudentDailyAttendance> studentDailyAttendances = new List<StudentDailyAttendance>();
                    List<StudentAttendanceHistory> studentAttendanceHistories = new List<StudentAttendanceHistory>();
                    List<StudentMissingAttendance> studentMissingAttendances = new List<StudentMissingAttendance>();

                    if (studentAttendanceAddViewModel.studentAttendance?.Any() == true)
                    {
                        var courseSectionIds = studentAttendanceAddViewModel.studentAttendance.Select(v => v.CourseSectionId).ToList();
                        var attendanceDates = studentAttendanceAddViewModel.studentAttendance.Select(v => v.AttendanceDate).ToList();
                        var periodIds = studentAttendanceAddViewModel.studentAttendance.Select(v => v.PeriodId).ToList();

                        var attendanceDataExist = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId /*&& x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId*/ /*&& x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId*/ && (courseSectionIds == null || (courseSectionIds.Contains(x.CourseSectionId))) && (attendanceDates == null || (attendanceDates.Contains(x.AttendanceDate))) && (periodIds == null || (periodIds.Contains(x.PeriodId)))).ToList();

                        long? StudentAttendanceId = 1;

                        var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                        if (studentAttendanceData != null)
                        {
                            StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                        }

                        int? membershipID = null;
                        var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId && c.SchoolId == studentAttendanceAddViewModel.SchoolId && c.StaffId == studentAttendanceAddViewModel.StaffId);

                        if (staffSchoolInfoData != null)
                        {
                            //membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && v.Profile.ToLower() == staffSchoolInfoData.Profile.ToLower())?.MembershipId;
                            membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && (v.Profile ?? "").ToLower() == (staffSchoolInfoData.Profile ?? "").ToLower())?.MembershipId;
                        }
                        else
                        {
                            var staffMasterData = this.context?.StaffMaster.FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId /*&& c.SchoolId == studentAttendanceAddViewModel.SchoolId */&& c.StaffId == studentAttendanceAddViewModel.StaffId);
                            if (staffMasterData != null)
                            {
                                //membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && v.Profile.ToLower() == staffMasterData.Profile.ToLower())?.MembershipId;
                                membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && (v.Profile ?? "").ToLower() == (staffMasterData.Profile ?? "").ToLower())?.MembershipId;
                            }
                        }

                        long? CommentId = Utility.GetMaxLongPK<StudentAttendanceComments>(this.context, x => x.CommentId);

                        foreach (var studentAttendances in studentAttendanceAddViewModel.studentAttendance)
                        {
                            if (studentAttendances.StudentAttendanceComments.Count() > 0)
                            {
                                foreach (var StudentAttendanceComment in studentAttendances.StudentAttendanceComments)
                                {
                                    StudentAttendanceComment.CommentId = (long)CommentId!;
                                    CommentId++;
                                }
                            }
                        }

                        long? HistoryCommentId = Utility.GetMaxLongPK<StudentAttendanceHistory>(this.context, x => x.AttendanceHistoryId);

                        if (attendanceDataExist?.Any() == true)
                        {
                            //this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                            var studentAttendanceIDs = attendanceDataExist.Select(v => v.StudentAttendanceId).ToList();

                            var studentAttendanceCommentData = this.context?.StudentAttendanceComments.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && (studentAttendanceIDs == null || (studentAttendanceIDs.Contains(x.StudentAttendanceId))));

                            if (studentAttendanceCommentData?.Any() == true)
                            {
                                this.context?.StudentAttendanceComments.RemoveRange(studentAttendanceCommentData);
                            }
                            this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                            this.context?.SaveChanges();

                            foreach (var studentAttendancedata in studentAttendanceAddViewModel.studentAttendance.ToList())
                            {
                                var studentAttendanceUpdate = new StudentAttendance()
                                {
                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                    StudentId = studentAttendanceAddViewModel.StudentId,
                                    StaffId = studentAttendancedata.StaffId,
                                    CourseId = studentAttendancedata.CourseId,
                                    CourseSectionId = studentAttendancedata.CourseSectionId,
                                    AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                    AttendanceCode = studentAttendancedata.AttendanceCode,
                                    AttendanceDate = studentAttendancedata.AttendanceDate,
                                    //Comments = studentAttendancedata.Comments,
                                    UpdatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                    UpdatedOn = DateTime.UtcNow,
                                    BlockId = studentAttendancedata.BlockId,
                                    PeriodId = studentAttendancedata.PeriodId,
                                    StudentAttendanceId = (int)StudentAttendanceId,
                                    MembershipId = studentAttendanceAddViewModel.MembershipId != null ? studentAttendanceAddViewModel.MembershipId : membershipID,
                                    StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                    {
                                        c.UpdatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                        c.UpdatedOn = DateTime.UtcNow;
                                        c.CommentTimestamp = DateTime.UtcNow;
                                        c.MembershipId = c.MembershipId != null ? c.MembershipId : membershipID;
                                        return c;
                                    }).ToList()
                                };

                                studentAttendance.Add(studentAttendanceUpdate);

                                var studentAttendanceHistoryUpdate = new StudentAttendanceHistory()
                                {
                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                    StudentId = studentAttendanceAddViewModel.StudentId,
                                    CourseId = studentAttendancedata.CourseId,
                                    CourseSectionId = studentAttendancedata.CourseSectionId,
                                    AttendanceHistoryId = (long)HistoryCommentId!,
                                    AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                    AttendanceCode = studentAttendancedata.AttendanceCode,
                                    AttendanceDate = studentAttendancedata.AttendanceDate,
                                    BlockId = studentAttendancedata.BlockId,
                                    PeriodId = studentAttendancedata.PeriodId,
                                    ModifiedBy = studentAttendanceAddViewModel.UserId != null ? (int)studentAttendanceAddViewModel.UserId : studentAttendancedata.StaffId, //this will be login userId when admin login(for attendance administration screan) other time it will be staffId
                                    ModificationTimestamp = DateTime.UtcNow,
                                    MembershipId = studentAttendanceAddViewModel.MembershipId != null ? studentAttendanceAddViewModel.MembershipId : membershipID,
                                    UpdatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                    UpdatedOn = DateTime.UtcNow,
                                };
                                studentAttendanceHistories.Add(studentAttendanceHistoryUpdate);
                                StudentAttendanceId++;
                                HistoryCommentId++;

                                //check for remove data from StudentMissingAttendances table.
                                var dataExitsInMA = this.context?.StudentMissingAttendances.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendancedata.CourseSectionId && x.PeriodId == studentAttendancedata.PeriodId && x.MissingAttendanceDate == studentAttendancedata.AttendanceDate).FirstOrDefault();
                                if (dataExitsInMA != null)
                                {
                                    studentMissingAttendances.Add(dataExitsInMA);
                                }
                            }
                            studentAttendanceAddViewModel._message = "Student attendance updated successfully.";
                        }
                        else
                        {
                            foreach (var studentAttendancedata in studentAttendanceAddViewModel.studentAttendance.ToList())
                            {
                                var studentAttendanceAdd = new StudentAttendance()
                                {
                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                    StudentId = studentAttendanceAddViewModel.StudentId,
                                    StaffId = studentAttendancedata.StaffId,
                                    CourseId = studentAttendancedata.CourseId,
                                    CourseSectionId = studentAttendancedata.CourseSectionId,
                                    AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                    AttendanceCode = studentAttendancedata.AttendanceCode,
                                    AttendanceDate = studentAttendancedata.AttendanceDate,
                                    //Comments = studentAttendancedata.Comments,
                                    CreatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    BlockId = studentAttendancedata.BlockId,
                                    PeriodId = studentAttendancedata.PeriodId,
                                    StudentAttendanceId = (int)StudentAttendanceId,
                                    MembershipId = studentAttendanceAddViewModel.MembershipId != null ? studentAttendanceAddViewModel.MembershipId : membershipID,
                                    StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                    {
                                        c.CreatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                        c.CreatedOn = DateTime.UtcNow;
                                        c.CommentTimestamp = DateTime.UtcNow;
                                        c.MembershipId = c.MembershipId != null ? c.MembershipId : membershipID;
                                        return c;
                                    }).ToList()
                                };
                                studentAttendance.Add(studentAttendanceAdd);

                                var studentAttendanceHistoryAdd = new StudentAttendanceHistory()
                                {
                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                    StudentId = studentAttendanceAddViewModel.StudentId,
                                    CourseId = studentAttendancedata.CourseId,
                                    CourseSectionId = studentAttendancedata.CourseSectionId,
                                    AttendanceHistoryId = (long)HistoryCommentId!,
                                    AttendanceCategoryId = studentAttendancedata.AttendanceCategoryId,
                                    AttendanceCode = studentAttendancedata.AttendanceCode,
                                    AttendanceDate = studentAttendancedata.AttendanceDate,
                                    BlockId = studentAttendancedata.BlockId,
                                    PeriodId = studentAttendancedata.PeriodId,
                                    ModifiedBy = studentAttendanceAddViewModel.UserId != null ? (int)studentAttendanceAddViewModel.UserId : studentAttendancedata.StaffId, //this will be login userId when admin login(for attendance administration screan) other time it will be staffId
                                    ModificationTimestamp = DateTime.UtcNow,
                                    MembershipId = studentAttendanceAddViewModel.MembershipId != null ? studentAttendanceAddViewModel.MembershipId : membershipID,
                                    CreatedBy = studentAttendanceAddViewModel.UpdatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                };
                                studentAttendanceHistories.Add(studentAttendanceHistoryAdd);
                                StudentAttendanceId++;
                                HistoryCommentId++;

                                //check for remove data from StudentMissingAttendances table.
                                var dataExitsInMA = this.context?.StudentMissingAttendances.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendancedata.CourseSectionId && x.PeriodId == studentAttendancedata.PeriodId && x.MissingAttendanceDate == studentAttendancedata.AttendanceDate).FirstOrDefault();
                                if (dataExitsInMA != null)
                                {
                                    studentMissingAttendances.Add(dataExitsInMA);
                                }
                            }
                            studentAttendanceAddViewModel._message = "Student attendance added successfully.";
                        }

                        studentAttendance.ToList().ForEach(x => x.StudentAttendanceComments.ToList().ForEach(x => { x.StudentAttendance = new(); x.Membership = null; }));

                        this.context?.StudentAttendance.AddRange(studentAttendance);
                        this.context?.StudentAttendanceHistory.AddRange(studentAttendanceHistories);
                        this.context?.StudentMissingAttendances.RemoveRange(studentMissingAttendances);
                        this.context?.SaveChanges();

                        attendanceDates = attendanceDates.Distinct().ToList();

                        // Batch-load all lookup data before the loop (eliminates N+1 queries)
                        var allAttendanceForDates = this.context?.StudentAttendance
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId && attendanceDates.Contains(x.AttendanceDate))
                            .ToList()
                            .GroupBy(x => x.AttendanceDate)
                            .ToDictionary(g => g.Key, g => g.ToList());

                        var blockPeriodLookup = this.context?.BlockPeriod
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                            .ToDictionary(x => (x.BlockId, x.PeriodId));

                        var attendanceCodeList = this.context?.AttendanceCode
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                            .ToList();

                        var existingDailyAttendance = this.context?.StudentDailyAttendance
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId && attendanceDates.Contains(x.AttendanceDate))
                            .ToDictionary(x => x.AttendanceDate);

                        foreach (var date in attendanceDates)
                        {
                            int totalAttendanceMin = 0;
                            if (allAttendanceForDates != null && allAttendanceForDates.TryGetValue(date, out var attendanceData))
                            {
                                foreach (var attendance in attendanceData)
                                {
                                    if (blockPeriodLookup != null && blockPeriodLookup.TryGetValue((attendance.BlockId, attendance.PeriodId), out var BlockPeriodData))
                                    {
                                        // Only count periods flagged as calculating attendance
                                        if (BlockPeriodData.CalculateAttendance != true)
                                            continue;

                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = attendanceCodeList?.FirstOrDefault(x => x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            if (AttendanceCodeData.StateCode!.ToLower() != "absent")
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                            }
                            if (existingDailyAttendance != null && existingDailyAttendance.TryGetValue(date, out var studentDailyAttendanceData))
                            {
                                studentDailyAttendanceData.AttendanceMinutes = totalAttendanceMin;
                            }
                            else
                            {
                                var studentDailyAttendance = new StudentDailyAttendance { TenantId = studentAttendanceAddViewModel.TenantId, SchoolId = studentAttendanceAddViewModel.SchoolId, StudentId = studentAttendanceAddViewModel.StudentId, AttendanceDate = date, CreatedBy = studentAttendanceAddViewModel.CreatedBy, AttendanceMinutes = totalAttendanceMin, CreatedOn = DateTime.UtcNow };
                                studentDailyAttendances.Add(studentDailyAttendance);
                            }
                        }
                        this.context?.StudentDailyAttendance.AddRange(studentDailyAttendances);
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        studentAttendanceAddViewModel._failure = false;
                        studentAttendanceAddViewModel.studentAttendance.ToList().ForEach(x => x.StudentAttendanceComments.ToList().ForEach(x => x.StudentAttendance = new()));
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentAttendanceAddViewModel._failure = true;
                    studentAttendanceAddViewModel._message = es.Message;
                }
            }
            return studentAttendanceAddViewModel;
        }

        /// <summary>
        /// Staff List For Missing Attendance
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel StaffListForMissingAttendance_old(PageResult pageResult)
        {
            StaffListModel staffListViewModel = new StaffListModel();
            IQueryable<StaffMaster>? transactionIQ = null;
            List<StaffMaster> staffCoursesectionSchedule = new List<StaffMaster>();
            IQueryable<StaffCoursesectionSchedule>? staffScheduleDataList = null;
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<DateTime> holidayList = new List<DateTime>();
            try
            {
                staffScheduleDataList = this.context?.StaffCoursesectionSchedule.AsNoTracking().Include(d => d.StaffMaster).Include(d => d.StudentAttendance).Include(b => b.CourseSection).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
                {
                    SchoolId = v.SchoolId,
                    TenantId = v.TenantId,
                    CourseSectionId = v.CourseSectionId,
                    CourseId = v.CourseId,
                    IsDropped = v.IsDropped,
                    DurationStartDate = v.DurationStartDate,
                    DurationEndDate = v.DurationEndDate,
                    MeetingDays = v.MeetingDays,
                    CourseSection = v.CourseSection,
                    StaffMaster = new StaffMaster()
                    {
                        SchoolId = v.StaffMaster.SchoolId,
                        TenantId = v.StaffMaster.TenantId,
                        StaffId = v.StaffMaster.StaffId,
                        FirstGivenName = v.StaffMaster.FirstGivenName,
                        MiddleName = v.StaffMaster.MiddleName,
                        LastFamilyName = v.StaffMaster.LastFamilyName,
                        StaffInternalId = v.StaffMaster.StaffInternalId,
                        Profile = v.StaffMaster.Profile,
                        JobTitle = v.StaffMaster.JobTitle,
                        SchoolEmail = v.StaffMaster.SchoolEmail,
                        MobilePhone = v.StaffMaster.MobilePhone
                    }
                });

                //var calendarData = this.context?.CourseCalendarSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();
                allCourseSectionVewList = this.context?.AllCourseSectionView.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();
                if (allCourseSectionVewList is null)
                {
                    return staffListViewModel;
                }
                if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                {
                    staffScheduleDataList = staffScheduleDataList?.Where(e => ((pageResult.DobStartDate!.Value.Date >= e.DurationStartDate!.Value.Date && pageResult.DobStartDate!.Value.Date <= e.DurationEndDate!.Value.Date) || (pageResult.DobEndDate!.Value.Date >= e.DurationStartDate!.Value.Date && pageResult.DobEndDate!.Value.Date <= e.DurationEndDate)));

                    //Calculate Holiday
                    var CalendarEventsData = this.context?.CalendarEvents.AsNoTracking().Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewList.FirstOrDefault()!.CalendarId && (e.StartDate >= pageResult.DobStartDate && e.StartDate <= pageResult.DobEndDate || e.EndDate >= pageResult.DobStartDate && e.EndDate <= pageResult.DobEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                    if (CalendarEventsData?.Any() == true)
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
                }

                List<int> ID = new List<int>();
                List<DateTime> missingAttendanceDatelist = new List<DateTime>();
                if (staffScheduleDataList?.Any() == true)
                {
                    foreach (var staffScheduleData in staffScheduleDataList.ToList())
                    {
                        if (staffScheduleData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffScheduleData.CourseId && v.CourseSectionId == staffScheduleData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();


                            if (allCourseSectionVewLists.Count > 0)
                            {
                                DateTime start;
                                DateTime end;
                                List<DateTime> dateList = new List<DateTime>();

                                if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                                {
                                    start = (DateTime)pageResult.DobStartDate;
                                    end = (DateTime)pageResult.DobEndDate;
                                }
                                else
                                {
                                    start = (DateTime)staffScheduleData.DurationStartDate!;
                                    end = (DateTime)staffScheduleData.DurationEndDate!;
                                }
                                if (staffScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    foreach (var allCourseSectionVew in allCourseSectionVewList)
                                    {

                                        var bellScheduleList = this.context?.BellSchedule.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= start && v.BellScheduleDate <= end && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                        if (bellScheduleList?.Any() == true)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {

                                                var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == staffScheduleData.CourseSectionId && b.CourseId == staffScheduleData.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

                                                if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                {
                                                    missingAttendanceDatelist.Add(bellSchedule.BellScheduleDate);

                                                    if (!ID.Contains(staffScheduleData.StaffMaster.StaffId))
                                                    {
                                                        ID.Add(staffScheduleData.StaffMaster.StaffId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (staffScheduleData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    var courseCalenderScheduleDateList = allCourseSectionVewLists.Where(c => c.CourseId == staffScheduleData.CourseId && c.CourseSectionId == staffScheduleData.CourseSectionId && c.CalDate >= start && c.CalDate <= end /*&& c.TakeAttendanceCalendar != false*/ && c.CalDate <= DateTime.Today.Date).ToList();

                                    if (courseCalenderScheduleDateList.Count > 0)
                                    {
                                        foreach (var courseCalenderScheduleDate in courseCalenderScheduleDateList)
                                        {
                                            var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffScheduleData.StaffId*/ && b.AttendanceDate.Date == courseCalenderScheduleDate.CalDate && b.CourseSectionId == staffScheduleData.CourseSectionId && b.CourseId == staffScheduleData.CourseId && b.PeriodId == courseCalenderScheduleDate.CalPeriodId);

                                            if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                            {
                                                missingAttendanceDatelist.Add((DateTime)courseCalenderScheduleDate.CalDate!);

                                                if (!ID.Contains(staffScheduleData.StaffMaster.StaffId))
                                                {
                                                    //staffCoursesectionSchedule.Add(staffScheduleData.StaffMaster);
                                                    ID.Add(staffScheduleData.StaffMaster.StaffId);
                                                    //break;
                                                }
                                                //else
                                                //{
                                                //    break;
                                                //}
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    List<string> list = new List<string>();

                                    string[] meetingDays = { };
                                    meetingDays = staffScheduleData.MeetingDays!.ToLower().Split("|");

                                    //if (staffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                    //{
                                    //    list = allCourseSectionVewLists.FirstOrDefault().FixedDays.Split("|").ToList();

                                    //    if (list.Count > 0)
                                    //    {
                                    //        meetingDays = list.ToArray();
                                    //    }
                                    //}
                                    //if (staffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                    //{
                                    //    meetingDays = allCourseSectionVewLists.Select(c => c.VarDay).ToArray();
                                    //}

                                    bool allDays = meetingDays == null || !meetingDays.Any();

                                    dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                          .Select(offset => start.AddDays(offset))
                                                          .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                          .ToList();

                                    if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                                    {
                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date >= staffScheduleData.DurationStartDate && s.Date <= staffScheduleData.DurationEndDate)).ToList();
                                    }

                                    if (dateList.Count > 0)
                                    {
                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                    }

                                    foreach (var date in dateList)
                                    {
                                        if (staffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                        {
                                            CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                            var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == staffScheduleData.CourseId && b.CourseSectionId == staffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId);

                                            if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                            {
                                                missingAttendanceDatelist.Add(date.Date);

                                                if (!ID.Contains(staffScheduleData.StaffMaster.StaffId))
                                                {
                                                    //staffCoursesectionSchedule.Add(staffScheduleData.StaffMaster);
                                                    ID.Add(staffScheduleData.StaffMaster.StaffId);
                                                    //break;
                                                }
                                                //else
                                                //{
                                                //    break;
                                                //}
                                            }

                                        }
                                        if (staffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                        {
                                            var courseVariableScheduleData = allCourseSectionVewLists.Where(e => e.VarDay != null && e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                            if (courseVariableScheduleData != null)
                                            {
                                                foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                {
                                                    CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == staffScheduleData.CourseId && b.CourseSectionId == staffScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                    if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                    {
                                                        missingAttendanceDatelist.Add(date.Date);

                                                        if (!ID.Contains(staffScheduleData.StaffMaster.StaffId))
                                                        {
                                                            //staffCoursesectionSchedule.Add(staffScheduleData.StaffMaster);
                                                            ID.Add(staffScheduleData.StaffMaster.StaffId);
                                                            //break;
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
                if (staffScheduleDataList?.Any() == true)
                {
                    var staffList = staffScheduleDataList.Select(b => b.StaffMaster).Where(x => (ID == null || (ID.Contains(x.StaffId)))).ToList();

                    if (staffList.Count > 0)
                    {
                        staffCoursesectionSchedule.AddRange(staffList);
                    }
                    staffCoursesectionSchedule = staffCoursesectionSchedule.GroupBy(c => c.StaffId).Select(c => c.FirstOrDefault()!).ToList();
                    missingAttendanceDatelist = missingAttendanceDatelist.GroupBy(b => b.Date).Select(c => c.FirstOrDefault()).ToList();
                    //Remove Holiday
                    missingAttendanceDatelist = missingAttendanceDatelist.Where(x => !holidayList.Contains(x.Date)).ToList();

                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = staffCoursesectionSchedule.AsQueryable();
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            transactionIQ = staffCoursesectionSchedule.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffInternalId != null && x.StaffInternalId.ToLower().Contains(Columnvalue.ToLower()) || x.Profile != null && x.Profile.ToLower().Contains(Columnvalue.ToLower()) || x.JobTitle != null && x.JobTitle.ToLower().Contains(Columnvalue.ToLower()) || x.SchoolEmail != null && x.SchoolEmail.ToLower().Contains(Columnvalue.ToLower()) || x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue)).AsQueryable();
                        }
                    }
                    //transactionIQ = transactionIQ.Distinct();

                    if (pageResult.SortingModel != null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                    }

                    int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                    if (totalCount > 0)
                    {
                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ!.Select(p => new StaffMaster
                            {
                                SchoolId = p.SchoolId,
                                TenantId = p.TenantId,
                                StaffId = p.StaffId,
                                StaffInternalId = p.StaffInternalId,
                                FirstGivenName = p.FirstGivenName,
                                MiddleName = p.MiddleName,
                                LastFamilyName = p.LastFamilyName,
                                Profile = p.Profile,
                                JobTitle = p.JobTitle,
                                SchoolEmail = p.SchoolEmail,
                                MobilePhone = p.MobilePhone
                            }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }
                    }
                    staffListViewModel.staffMaster = transactionIQ != null ? transactionIQ.ToList() : new();
                    staffListViewModel.missingAttendanceDateList = missingAttendanceDatelist;
                    staffListViewModel.TotalCount = totalCount;
                    staffListViewModel.PageNumber = pageResult.PageNumber;
                    staffListViewModel._pageSize = pageResult.PageSize;
                    staffListViewModel._failure = false;
                    staffListViewModel.TenantId = pageResult.TenantId;
                    staffListViewModel._tenantName = pageResult._tenantName;
                    staffListViewModel._token = pageResult._token;
                    staffListViewModel._userName = pageResult._userName;
                }
                else
                {
                    staffListViewModel._failure = true;
                    staffListViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffListViewModel._failure = true;
                staffListViewModel._message = es.Message;
            }
            return staffListViewModel;
        }

        public StaffListModel StaffListForMissingAttendance(PageResult pageResult)
        {
            StaffListModel staffListViewModel = new StaffListModel();
            IQueryable<StaffMaster>? transactionIQ = null;
            List<StaffMaster> staffCoursesectionSchedule = new List<StaffMaster>();
            IQueryable<StaffCoursesectionSchedule>? staffScheduleDataList = null;
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();

            try
            {
                staffScheduleDataList = this.context?.StaffCoursesectionSchedule.AsNoTracking().Include(b => b.CourseSection).Include(d => d.StaffMaster).ThenInclude(a => a.StaffSchoolInfo).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
                {
                    SchoolId = v.SchoolId,
                    TenantId = v.TenantId,
                    CourseSectionId = v.CourseSectionId,
                    CourseId = v.CourseId,
                    IsDropped = v.IsDropped,
                    DurationStartDate = v.DurationStartDate,
                    DurationEndDate = v.DurationEndDate,
                    MeetingDays = v.MeetingDays,
                    CourseSection = v.CourseSection,
                    StaffMaster = new StaffMaster()
                    {
                        SchoolId = v.StaffMaster.SchoolId,
                        TenantId = v.StaffMaster.TenantId,
                        StaffId = v.StaffMaster.StaffId,
                        FirstGivenName = v.StaffMaster.FirstGivenName,
                        MiddleName = v.StaffMaster.MiddleName,
                        LastFamilyName = v.StaffMaster.LastFamilyName,
                        StaffInternalId = v.StaffMaster.StaffInternalId,
                        Profile = v.StaffMaster.Profile,
                        JobTitle = v.StaffMaster.JobTitle,
                        SchoolEmail = v.StaffMaster.SchoolEmail,
                        MobilePhone = v.StaffMaster.MobilePhone,
                        AlternateId = v.StaffMaster.AlternateId,
                        SocialSecurityNumber = v.StaffMaster.SocialSecurityNumber,
                        LoginEmailAddress = v.StaffMaster.LoginEmailAddress,
                        Dob = v.StaffMaster.Dob,
                        Race = v.StaffMaster.Race,
                        Ethnicity = v.StaffMaster.Ethnicity,
                        Gender = v.StaffMaster.Gender,
                        CountryOfBirth = v.StaffMaster.CountryOfBirth,
                        MaritalStatus = v.StaffMaster.MaritalStatus,
                        Nationality = v.StaffMaster.Nationality,
                        FirstLanguage = v.StaffMaster.FirstLanguage,
                        SecondLanguage = v.StaffMaster.SecondLanguage,
                        ThirdLanguage = v.StaffMaster.ThirdLanguage,
                        HomePhone = v.StaffMaster.HomePhone,
                        JoiningDate = v.StaffMaster.JoiningDate,
                        EndDate = v.StaffMaster.EndDate,
                        HomeAddressLineOne = v.StaffMaster.HomeAddressLineOne,
                        HomeAddressLineTwo = v.StaffMaster.HomeAddressLineTwo,
                        HomeAddressCity = v.StaffMaster.HomeAddressCity,
                        HomeAddressCountry = v.StaffMaster.HomeAddressCountry,
                        HomeAddressState = v.StaffMaster.HomeAddressState,
                        HomeAddressZip = v.StaffMaster.HomeAddressZip,
                        BusNo = v.StaffMaster.BusNo,
                        PersonalEmail = v.StaffMaster.PersonalEmail,
                        StaffSchoolInfo = v.StaffMaster.StaffSchoolInfo.ToList(),
                    }
                });

                allCourseSectionVewList = this.context?.AllCourseSectionView.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                if (allCourseSectionVewList != null)
                {
                    if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                    {
                        staffScheduleDataList = staffScheduleDataList?.Where(e => ((pageResult.DobStartDate.Value.Date >= e.DurationStartDate!.Value.Date && pageResult.DobStartDate.Value.Date <= e.DurationEndDate!.Value.Date) || (pageResult.DobEndDate.Value.Date >= e.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= e.DurationEndDate)));
                    }

                    List<int> ID = new List<int>();
                    List<DateTime> missingAttendanceDatelist = new List<DateTime>();
                    if (staffScheduleDataList?.Any() == true)
                    {
                        var staffScheduleDataMaterialized = staffScheduleDataList.ToList();

                        // Batch-load all missing attendance for this school in one query (eliminates N+1)
                        var courseSectionIds = staffScheduleDataMaterialized
                            .Where(s => s.CourseSection.AcademicYear == pageResult.AcademicYear)
                            .Select(s => s.CourseSectionId).Distinct().ToList();

                        var allMissingAttendance = this.context?.StudentMissingAttendances.AsNoTracking()
                            .Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId.HasValue && courseSectionIds.Contains(x.CourseSectionId.Value))
                            .ToList();

                        var missingAttendanceByCourseSection = allMissingAttendance?
                            .Where(x => x.CourseSectionId.HasValue)
                            .GroupBy(x => x.CourseSectionId!.Value)
                            .ToDictionary(g => g.Key, g => g.ToList())
                            ?? new Dictionary<int, List<StudentMissingAttendance>>();

                        foreach (var staffScheduleData in staffScheduleDataMaterialized)
                        {
                            if (staffScheduleData.CourseSection.AcademicYear == pageResult.AcademicYear)
                            {
                                var allCourseSectionVewLists = allCourseSectionVewList.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffScheduleData.CourseId && v.CourseSectionId == staffScheduleData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                                if (allCourseSectionVewLists.Count > 0)
                                {
                                    DateTime start;
                                    DateTime end;

                                    if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                                    {
                                        start = (DateTime)pageResult.DobStartDate;
                                        end = (DateTime)pageResult.DobEndDate;
                                    }
                                    else
                                    {
                                        start = (DateTime)staffScheduleData.DurationStartDate!;
                                        end = (DateTime)staffScheduleData.DurationEndDate!;
                                    }

                                    var studentMissingAttendanceData = missingAttendanceByCourseSection.TryGetValue(staffScheduleData.CourseSectionId, out var csData)
                                        ? csData.Where(x => x.MissingAttendanceDate >= start && x.MissingAttendanceDate <= end).ToList()
                                        : null;

                                    if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                    {
                                        var dateList = studentMissingAttendanceData.Select(s => s.MissingAttendanceDate!.Value.Date).ToList();
                                        missingAttendanceDatelist.AddRange(dateList);
                                        if (!ID.Contains(staffScheduleData.StaffMaster.StaffId))
                                        {
                                            ID.Add(staffScheduleData.StaffMaster.StaffId);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (staffScheduleDataList?.Any() == true)
                    {
                        var staffList = staffScheduleDataList.Select(b => b.StaffMaster).Where(x => (ID == null || (ID.Contains(x.StaffId)))).ToList();

                        if (staffList.Count > 0)
                        {
                            staffCoursesectionSchedule.AddRange(staffList);
                        }
                        staffCoursesectionSchedule = staffCoursesectionSchedule.GroupBy(c => c.StaffId).Select(c => c.FirstOrDefault()!).ToList();
                        missingAttendanceDatelist = missingAttendanceDatelist.GroupBy(b => b.Date).Select(c => c.FirstOrDefault()).ToList();

                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = staffCoursesectionSchedule.AsQueryable();
                        }
                        else
                        {
                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                                transactionIQ = staffCoursesectionSchedule.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffInternalId != null && x.StaffInternalId.ToLower().Contains(Columnvalue.ToLower()) || x.Profile != null && x.Profile.ToLower().Contains(Columnvalue.ToLower()) || x.JobTitle != null && x.JobTitle.ToLower().Contains(Columnvalue.ToLower()) || x.SchoolEmail != null && x.SchoolEmail.ToLower().Contains(Columnvalue.ToLower()) || x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue)).AsQueryable();
                            }
                            else
                            {
                                transactionIQ = Utility.FilteredData(pageResult.FilterParams!, staffCoursesectionSchedule!).AsQueryable();
                            }
                        }
                        //transactionIQ = transactionIQ.Distinct();

                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                        }

                        int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                        if (totalCount > 0)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ!.Select(p => new StaffMaster
                                {
                                    SchoolId = p.SchoolId,
                                    TenantId = p.TenantId,
                                    StaffId = p.StaffId,
                                    StaffInternalId = p.StaffInternalId,
                                    FirstGivenName = p.FirstGivenName,
                                    MiddleName = p.MiddleName,
                                    LastFamilyName = p.LastFamilyName,
                                    Profile = p.Profile,
                                    JobTitle = p.JobTitle,
                                    SchoolEmail = p.SchoolEmail,
                                    MobilePhone = p.MobilePhone,
                                    StaffSchoolInfo = p.StaffSchoolInfo.ToList(),
                                }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }
                        }
                        staffListViewModel.staffMaster = transactionIQ != null ? transactionIQ.ToList() : new();
                        staffListViewModel.missingAttendanceDateList = missingAttendanceDatelist;
                        staffListViewModel.TotalCount = totalCount;
                        staffListViewModel.PageNumber = pageResult.PageNumber;
                        staffListViewModel._pageSize = pageResult.PageSize;
                        staffListViewModel._failure = false;
                        staffListViewModel.TenantId = pageResult.TenantId;
                        staffListViewModel._tenantName = pageResult._tenantName;
                        staffListViewModel._token = pageResult._token;
                        staffListViewModel._userName = pageResult._userName;
                    }
                    else
                    {
                        staffListViewModel._failure = true;
                        staffListViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                staffListViewModel._failure = true;
                staffListViewModel._message = es.Message;
            }
            return staffListViewModel;
        }

        /// <summary>
        /// Missing Attendance List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel MissingAttendanceList_old(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            IQueryable<CourseSectionViewList>? transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();
            //CourseFixedSchedule courseFixedSchedule = null;
            //List<CourseVariableSchedule> CourseVariableSchedule = new List<CourseVariableSchedule>();
            //List<CourseCalendarSchedule> courseCalendarSchedule = new List<CourseCalendarSchedule>();
            //List<CourseBlockSchedule> CourseBlockSchedule = new List<CourseBlockSchedule>();
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();
            List<DateTime> holidayList = new List<DateTime>();

            try
            {
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.AsNoTracking().Include(x => x.CourseSection).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && ((pageResult.DobStartDate!.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate!.Value.Date) || (pageResult.DobEndDate!.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate)) && s.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
                {
                    SchoolId = v.SchoolId,
                    TenantId = v.TenantId,
                    CourseSectionId = v.CourseSectionId,
                    CourseId = v.CourseId,
                    IsDropped = v.IsDropped,
                    DurationStartDate = v.DurationStartDate,
                    DurationEndDate = v.DurationEndDate,
                    StaffId = v.StaffId,
                    MeetingDays = v.MeetingDays,
                    CourseSectionName = v.CourseSectionName,
                    CourseSection = v.CourseSection,
                    StaffMaster = new StaffMaster()
                    {
                        SchoolId = v.StaffMaster.SchoolId,
                        TenantId = v.StaffMaster.TenantId,
                        StaffId = v.StaffMaster.StaffId,
                        FirstGivenName = v.StaffMaster.FirstGivenName,
                        MiddleName = v.StaffMaster.MiddleName,
                        LastFamilyName = v.StaffMaster.LastFamilyName,
                        StaffInternalId = v.StaffMaster.StaffInternalId,
                        Profile = v.StaffMaster.Profile,
                        JobTitle = v.StaffMaster.JobTitle,
                        SchoolEmail = v.StaffMaster.SchoolEmail,
                        MobilePhone = v.StaffMaster.MobilePhone
                    }
                });

                if (staffCourseSectionDataList?.Any() == true)
                {

                    allCourseSectionVewList = this.context?.AllCourseSectionView.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    // Dictionary for O(1) BlockPeriod lookups instead of linear scans
                    var blockPeriodByPeriodId = BlockPeriodList?.GroupBy(x => x.PeriodId).ToDictionary(g => g.Key, g => g.First());

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList!.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists.Count > 0)
                            {
                                var CalendarEventsData = this.context?.CalendarEvents.AsNoTracking().Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewLists.FirstOrDefault()!.CalendarId && (e.StartDate >= staffCourseSectionData.DurationStartDate && e.StartDate <= staffCourseSectionData.DurationEndDate || e.EndDate >= staffCourseSectionData.DurationStartDate && e.EndDate <= staffCourseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                                if (CalendarEventsData?.Any() == true)
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


                                if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    var blockScheduleData = allCourseSectionVewList!.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();
                                    foreach (var blockSchedule in blockScheduleData)
                                    {
                                        var bellScheduleList = this.context?.BellSchedule.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == blockSchedule.BlockId && v.BellScheduleDate >= pageResult.DobStartDate && v.BellScheduleDate <= pageResult.DobEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                        if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                                        {
                                            bellScheduleList = bellScheduleList?.Where(s => pageResult.DobStartDate != null && s.BellScheduleDate >= pageResult.DobStartDate && s.BellScheduleDate <= pageResult.DobEndDate).ToList();
                                        }

                                        if (bellScheduleList?.Any() == true)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate!.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                if (StudentCoursesectionScheduleData?.Any() == true)
                                                {
                                                    CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == blockSchedule.BlockPeriodId).ToList();

                                                    if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                    {
                                                        courseSectionBlock.AttendanceDate = bellSchedule.BellScheduleDate;
                                                        courseSectionBlock.CourseId = staffCourseSectionData.CourseId;
                                                        courseSectionBlock.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                        courseSectionBlock.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                        courseSectionBlock.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                        courseSectionBlock.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                        courseSectionBlock.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                        courseSectionBlock.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                        courseSectionBlock.PeriodTitle = blockPeriodByPeriodId != null && blockSchedule.BlockPeriodId != null && blockPeriodByPeriodId.TryGetValue((int)blockSchedule.BlockPeriodId, out var bpBlk) ? bpBlk.PeriodTitle : null;
                                                        courseSectionBlock.BlockId = blockSchedule.BlockId;
                                                        courseSectionBlock.PeriodId = blockSchedule.BlockPeriodId;
                                                        courseSectionBlock.AttendanceTaken = blockSchedule.TakeAttendanceBlock;

                                                        staffCoursesectionSchedule.Add(courseSectionBlock);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    List<DateTime> dateList = new List<DateTime>();
                                    List<string> list = new List<string>();
                                    string[] meetingDays = { };


                                    DateTime start = (DateTime)pageResult.DobStartDate!;
                                    DateTime end = (DateTime)pageResult.DobEndDate!;

                                    //if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                    //{
                                    //    list = allCourseSectionVewLists.FirstOrDefault().FixedDays.Split("|").ToList();

                                    //    if (list.Count > 0)
                                    //    {
                                    //        meetingDays = list.ToArray();
                                    //    }
                                    //}
                                    //if (staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                    //{
                                    //    meetingDays = allCourseSectionVewLists.Select(c => c.VarDay).ToArray();
                                    //}

                                    meetingDays = staffCourseSectionData.MeetingDays!.ToLower().Split("|");

                                    bool allDays = meetingDays == null || !meetingDays.Any();

                                    dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                         .Select(offset => start.AddDays(offset))
                                                         .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                         .ToList();

                                    dateList = dateList.Where(s => dateList.Any(secL => s.Date >= staffCourseSectionData.DurationStartDate && s.Date <= staffCourseSectionData.DurationEndDate)).ToList();

                                    if (dateList.Count > 0)
                                    {
                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                        //Remove Holiday
                                        dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
                                    }

                                    foreach (var date in dateList)
                                    {
                                        var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate!.Value.Date <= date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section

                                        if (StudentCoursesectionScheduleData?.Any() == true)
                                        {
                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                            {
                                                CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                                var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId);

                                                if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                {
                                                    CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                                    CourseSectionFixed.AttendanceDate = date;
                                                    CourseSectionFixed.CourseId = staffCourseSectionData.CourseId;
                                                    CourseSectionFixed.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                    CourseSectionFixed.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                    CourseSectionFixed.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                    CourseSectionFixed.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                    CourseSectionFixed.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                    CourseSectionFixed.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                    var fixedPeriodId = allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId;
                                                    var bpFixedOld = fixedPeriodId != null && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue((int)fixedPeriodId, out var bpf) ? bpf : null;
                                                    CourseSectionFixed.PeriodTitle = bpFixedOld?.PeriodTitle;
                                                    CourseSectionFixed.BlockId = bpFixedOld?.BlockId;
                                                    CourseSectionFixed.PeriodId = fixedPeriodId;
                                                    CourseSectionFixed.AttendanceTaken = staffCourseSectionData.CourseSection.AttendanceTaken;

                                                    staffCoursesectionSchedule.Add(CourseSectionFixed);
                                                }
                                            }
                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                            {
                                                var courseVariableScheduleData = allCourseSectionVewLists.Where(e => e.VarDay != null && e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                                if (courseVariableScheduleData != null)
                                                {
                                                    foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                    {
                                                        CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                                        var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                        if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                        {
                                                            CourseSectionVariable.AttendanceDate = date;
                                                            CourseSectionVariable.CourseId = staffCourseSectionData.CourseId;
                                                            CourseSectionVariable.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                            CourseSectionVariable.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                            CourseSectionVariable.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                            CourseSectionVariable.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                            CourseSectionVariable.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                            CourseSectionVariable.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                            var bpVarOld = courseVariableSchedule.VarPeriodId != null && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue((int)courseVariableSchedule.VarPeriodId, out var bpv) ? bpv : null;
                                                            CourseSectionVariable.PeriodTitle = bpVarOld?.PeriodTitle;
                                                            CourseSectionVariable.BlockId = bpVarOld?.BlockId;
                                                            CourseSectionVariable.PeriodId = courseVariableSchedule.VarPeriodId;
                                                            CourseSectionVariable.AttendanceTaken = courseVariableSchedule.TakeAttendanceVariable;

                                                            staffCoursesectionSchedule.Add(CourseSectionVariable);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (allCourseSectionVewLists.Count > 0)
                                    {
                                        var calenderScheduleList = allCourseSectionVewLists.Where(c => c.CalDate >= pageResult.DobStartDate && c.CalDate <= pageResult.DobEndDate && c.CalDate <= DateTime.Today.Date && !holidayList.Contains(c.CalDate.Value.Date));

                                        if (calenderScheduleList.ToList().Count > 0)
                                        {
                                            foreach (var calenderSchedule in calenderScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate!.Value.Date <= calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                if (StudentCoursesectionScheduleData?.Any() == true)
                                                {
                                                    CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.AsNoTracking().Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                                    if (staffAttendanceData?.Any() == false/*.Count() == 0*/)
                                                    {
                                                        CourseSectioncalender.AttendanceDate = (DateTime)calenderSchedule.CalDate!;
                                                        CourseSectioncalender.CourseId = staffCourseSectionData.CourseId;
                                                        CourseSectioncalender.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                        CourseSectioncalender.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                        CourseSectioncalender.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                        CourseSectioncalender.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                        CourseSectioncalender.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                        CourseSectioncalender.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                        var bpCalOld = calenderSchedule.CalPeriodId != null && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue((int)calenderSchedule.CalPeriodId, out var bpc) ? bpc : null;
                                                        CourseSectioncalender.PeriodTitle = bpCalOld?.PeriodTitle;
                                                        CourseSectioncalender.BlockId = bpCalOld?.BlockId;
                                                        CourseSectioncalender.PeriodId = calenderSchedule.CalPeriodId;
                                                        CourseSectioncalender.AttendanceTaken = calenderSchedule.TakeAttendanceCalendar;

                                                        staffCoursesectionSchedule.Add(CourseSectioncalender);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = staffCoursesectionSchedule.AsQueryable();
                        }
                        else
                        {
                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                                transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) ||/* x.AttendanceDate != null && */x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle != null && x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
                            }
                        }
                        transactionIQ = transactionIQ != null ? transactionIQ.Distinct() : null;

                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                        }

                        int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;

                        if (totalCount > 0 && transactionIQ != null)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }

                            scheduledCourseSectionView.courseSectionViewList = transactionIQ.ToList();
                            scheduledCourseSectionView.MissingAttendanceCount = totalCount;
                            scheduledCourseSectionView._pageSize = pageResult.PageSize;
                            scheduledCourseSectionView.PageNumber = pageResult.PageNumber;
                            scheduledCourseSectionView.TenantId = pageResult.TenantId;
                            scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                            scheduledCourseSectionView.StaffId = pageResult.StaffId;
                            scheduledCourseSectionView._failure = false;
                            scheduledCourseSectionView._tenantName = pageResult._tenantName;
                            scheduledCourseSectionView._token = pageResult._token;
                            scheduledCourseSectionView._userName = pageResult._userName;
                        }
                    }
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        public ScheduledCourseSectionViewModel MissingAttendanceList(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            IQueryable<CourseSectionViewList>? transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();

            try
            {
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.AsNoTracking().Include(x => x.CourseSection).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && ((pageResult.DobStartDate!.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate!.Value.Date) || (pageResult.DobEndDate!.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate)) && s.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
                {
                    SchoolId = v.SchoolId,
                    TenantId = v.TenantId,
                    CourseSectionId = v.CourseSectionId,
                    CourseId = v.CourseId,
                    IsDropped = v.IsDropped,
                    DurationStartDate = v.DurationStartDate,
                    DurationEndDate = v.DurationEndDate,
                    StaffId = v.StaffId,
                    MeetingDays = v.MeetingDays,
                    CourseSectionName = v.CourseSectionName,
                    CourseSection = v.CourseSection,
                    StaffMaster = new StaffMaster()
                    {
                        SchoolId = v.StaffMaster.SchoolId,
                        TenantId = v.StaffMaster.TenantId,
                        StaffId = v.StaffMaster.StaffId,
                        FirstGivenName = v.StaffMaster.FirstGivenName,
                        MiddleName = v.StaffMaster.MiddleName,
                        LastFamilyName = v.StaffMaster.LastFamilyName,
                        StaffInternalId = v.StaffMaster.StaffInternalId,
                        Profile = v.StaffMaster.Profile,
                        JobTitle = v.StaffMaster.JobTitle,
                        SchoolEmail = v.StaffMaster.SchoolEmail,
                        MobilePhone = v.StaffMaster.MobilePhone
                    }
                });

                if (staffCourseSectionDataList?.Any() == true)
                {

                    allCourseSectionVewList = this.context?.AllCourseSectionView.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.AsNoTracking().Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    // Dictionary for O(1) BlockPeriod lookups instead of linear scans
                    var blockPeriodByPeriodId = BlockPeriodList?.GroupBy(x => x.PeriodId).ToDictionary(g => g.Key, g => g.First());

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList!.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists.Count > 0)
                            {
                                DateTime start = (DateTime)pageResult.DobStartDate!;
                                DateTime end = (DateTime)pageResult.DobEndDate!;

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.AsNoTracking().Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.MissingAttendanceDate >= start && x.MissingAttendanceDate <= end).ToList();

                                if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                                {
                                    foreach (var studentMissingAttendance in studentMissingAttendanceData)
                                    {
                                        if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                        {
                                            CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                            CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                            CourseSectionFixed.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                            CourseSectionFixed.CourseId = staffCourseSectionData.CourseId;
                                            CourseSectionFixed.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                            CourseSectionFixed.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                            CourseSectionFixed.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                            CourseSectionFixed.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                            CourseSectionFixed.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                            CourseSectionFixed.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            var bpFixed = studentMissingAttendance.PeriodId.HasValue && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue(studentMissingAttendance.PeriodId.Value, out var bpFixedVal) ? bpFixedVal : null;
                                            CourseSectionFixed.PeriodTitle = bpFixed?.PeriodTitle;
                                            CourseSectionFixed.BlockId = bpFixed?.BlockId;
                                            CourseSectionFixed.PeriodId = studentMissingAttendance.PeriodId;
                                            CourseSectionFixed.AttendanceTaken = staffCourseSectionData.CourseSection.AttendanceTaken;

                                            staffCoursesectionSchedule.Add(CourseSectionFixed);
                                        }

                                        else if (staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                        {
                                            CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                            CourseSectionVariable.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                            CourseSectionVariable.CourseId = staffCourseSectionData.CourseId;
                                            CourseSectionVariable.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                            CourseSectionVariable.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                            CourseSectionVariable.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                            CourseSectionVariable.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                            CourseSectionVariable.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                            CourseSectionVariable.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            var bpVarNew = studentMissingAttendance.PeriodId.HasValue && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue(studentMissingAttendance.PeriodId.Value, out var bpVarVal) ? bpVarVal : null;
                                            CourseSectionVariable.PeriodTitle = bpVarNew?.PeriodTitle;
                                            CourseSectionVariable.BlockId = bpVarNew?.BlockId;
                                            CourseSectionVariable.PeriodId = studentMissingAttendance.PeriodId;
                                            CourseSectionVariable.AttendanceTaken = allCourseSectionVewLists.FirstOrDefault(e => e.VarPeriodId == studentMissingAttendance.PeriodId && e.VarDay!.ToLower().Contains(studentMissingAttendance.MissingAttendanceDate.Value.Date.DayOfWeek.ToString().ToLower()))?.TakeAttendanceVariable;

                                            staffCoursesectionSchedule.Add(CourseSectionVariable);
                                        }
                                        else if (staffCourseSectionData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                        {
                                            CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                            CourseSectioncalender.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                            CourseSectioncalender.CourseId = staffCourseSectionData.CourseId;
                                            CourseSectioncalender.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                            CourseSectioncalender.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                            CourseSectioncalender.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                            CourseSectioncalender.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                            CourseSectioncalender.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                            CourseSectioncalender.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            var bpCalNew = studentMissingAttendance.PeriodId.HasValue && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue(studentMissingAttendance.PeriodId.Value, out var bpCalVal) ? bpCalVal : null;
                                            CourseSectioncalender.PeriodTitle = bpCalNew?.PeriodTitle;
                                            CourseSectioncalender.BlockId = bpCalNew?.BlockId;
                                            CourseSectioncalender.PeriodId = studentMissingAttendance.PeriodId;
                                            CourseSectioncalender.AttendanceTaken = allCourseSectionVewLists.FirstOrDefault(e => e.CalPeriodId == studentMissingAttendance.PeriodId && e.CalDate == studentMissingAttendance.MissingAttendanceDate.Value.Date)?.TakeAttendanceCalendar;

                                            staffCoursesectionSchedule.Add(CourseSectioncalender);
                                        }
                                        else if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                                        {
                                            CourseSectionViewList courseSectionBlock = new CourseSectionViewList();


                                            courseSectionBlock.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                            courseSectionBlock.CourseId = staffCourseSectionData.CourseId;
                                            courseSectionBlock.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                            courseSectionBlock.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                            courseSectionBlock.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                            courseSectionBlock.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                            courseSectionBlock.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                            courseSectionBlock.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            courseSectionBlock.PeriodTitle = studentMissingAttendance.PeriodId.HasValue && blockPeriodByPeriodId != null && blockPeriodByPeriodId.TryGetValue(studentMissingAttendance.PeriodId.Value, out var bpBlockNew) ? bpBlockNew.PeriodTitle : null;
                                            courseSectionBlock.BlockId = studentMissingAttendance.BlockId;
                                            courseSectionBlock.PeriodId = studentMissingAttendance.PeriodId;
                                            courseSectionBlock.AttendanceTaken = allCourseSectionVewLists.FirstOrDefault(e => e.BlockPeriodId == studentMissingAttendance.PeriodId && e.BlockId == studentMissingAttendance.BlockId)?.TakeAttendanceBlock;

                                            staffCoursesectionSchedule.Add(courseSectionBlock);
                                        }
                                    }
                                }
                            }
                        }


                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = staffCoursesectionSchedule.AsQueryable();
                        }
                        else
                        {
                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                                transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) ||/* x.AttendanceDate != null && */x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle != null && x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
                            }
                        }
                        transactionIQ = transactionIQ != null ? transactionIQ.Distinct() : null;

                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                        }

                        int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;

                        if (totalCount > 0 && transactionIQ != null)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }

                            scheduledCourseSectionView.courseSectionViewList = transactionIQ.ToList();
                            scheduledCourseSectionView.MissingAttendanceCount = totalCount;
                           
                        }
                    }
                }
                scheduledCourseSectionView._pageSize = pageResult.PageSize;
                scheduledCourseSectionView.PageNumber = pageResult.PageNumber;
                scheduledCourseSectionView.TenantId = pageResult.TenantId;
                scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                scheduledCourseSectionView.StaffId = pageResult.StaffId;
                scheduledCourseSectionView._failure = false;
                scheduledCourseSectionView._tenantName = pageResult._tenantName;
                scheduledCourseSectionView._token = pageResult._token;
                scheduledCourseSectionView._userName = pageResult._userName;
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Get All Student Attendance List Administration
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentAttendanceListViewModel GetAllStudentAttendanceListForAdministration(PageResult pageResult)
        {
            StudentAttendanceListViewModel studentAttendanceList = new StudentAttendanceListViewModel();
            studentAttendanceList.TenantId = pageResult.TenantId;
            studentAttendanceList.SchoolId = pageResult.SchoolId;
            studentAttendanceList._userName = pageResult._userName;
            studentAttendanceList._tenantName = pageResult._tenantName;
            IQueryable<StudendAttendanceAdministrationViewModel>? transactionIQ = null;
            List<StudendAttendanceAdministrationViewModel> attendanceData = new List<StudendAttendanceAdministrationViewModel>();
            try
            {
                // Get the latest enrollment per student (mirrors StudentListView logic)
                var latestEnrollmentIds = this.context?.StudentEnrollment
                    .AsNoTracking()
                    .Where(e => e.TenantId == pageResult.TenantId && e.SchoolId == pageResult.SchoolId)
                    .GroupBy(e => e.StudentId)
                    .Select(g => g.Max(x => x.EnrollmentId))
                    .ToList() ?? new List<int>();

                // Start from students whose latest enrollment is active
                var enrolledStudents = this.context?.StudentEnrollment
                    .AsNoTracking()
                    .Include(e => e.StudentMaster)
                        .ThenInclude(s => s.Sections)
                    .Where(e => e.TenantId == pageResult.TenantId
                        && e.SchoolId == pageResult.SchoolId
                        && e.IsActive == true
                        && e.StudentMaster.IsActive != false
                        && latestEnrollmentIds.Contains(e.EnrollmentId))
                    .ToList();

                if (enrolledStudents != null && enrolledStudents.Any())
                {
                    var studentIds = enrolledStudents.Select(e => e.StudentId).Distinct().ToList();

                    // Batch-load all attendance records for the selected date
                    var studentAttendanceData = pageResult.AttendanceDate != null
                        ? this.context?.StudentAttendance.AsNoTracking()
                            .Include(s => s.StudentAttendanceComments).ThenInclude(s => s.Membership)
                            .Include(s => s.BlockPeriod)
                            .Include(s => s.AttendanceCodeNavigation)
                            .Include(s => s.StudentCoursesectionSchedule)
                            .Where(x => x.TenantId == pageResult.TenantId
                                && x.SchoolId == pageResult.SchoolId
                                && studentIds.Contains(x.StudentId)
                                && x.AttendanceDate == pageResult.AttendanceDate
                                && (pageResult.AttendanceCode == null || x.AttendanceCode == pageResult.AttendanceCode))
                            .ToList()
                        : new List<StudentAttendance>();

                    // Group attendance by student for O(1) lookup
                    var attendanceByStudent = studentAttendanceData!
                        .GroupBy(a => a.StudentId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Batch-load daily attendance for present status
                    var dailyAttendanceLookup = pageResult.AttendanceDate != null
                        ? this.context?.StudentDailyAttendance
                            .AsNoTracking()
                            .Where(x => x.TenantId == pageResult.TenantId
                                && x.SchoolId == pageResult.SchoolId
                                && studentIds.Contains(x.StudentId)
                                && x.AttendanceDate == pageResult.AttendanceDate)
                            .ToDictionary(x => x.StudentId)
                        : new Dictionary<int, StudentDailyAttendance>();

                    // Load block data for present status calculation
                    var firstAttendance = studentAttendanceData!.FirstOrDefault();
                    var blockData = firstAttendance != null
                        ? this.context?.Block.AsNoTracking().FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.BlockId == firstAttendance.BlockId)
                        : null;

                    // Deduplicate students (one enrollment row per student)
                    var processedStudentIds = new HashSet<int>();

                    foreach (var enrollment in enrolledStudents)
                    {
                        if (!processedStudentIds.Add(enrollment.StudentId))
                            continue;

                        var student = enrollment.StudentMaster;
                        if (student == null) continue;

                        var administrationViewModel = new StudendAttendanceAdministrationViewModel
                        {
                            TenantId = enrollment.TenantId,
                            SchoolId = enrollment.SchoolId,
                            StudentId = enrollment.StudentId,
                            StudentGuid = student.StudentGuid,
                            StudentInternalId = student.StudentInternalId,
                            FirstGivenName = student.FirstGivenName,
                            MiddleName = student.MiddleName,
                            LastFamilyName = student.LastFamilyName,
                            GradeLevelTitle = enrollment.GradeLevelTitle,
                            GradeId = enrollment.GradeId,
                            Section = student.Sections?.Name,
                            SectionId = student.SectionId
                        };

                        // Attach attendance records if any exist for this student
                        if (attendanceByStudent.TryGetValue(enrollment.StudentId, out var records))
                        {
                            administrationViewModel.PeriodsRecorded = records.Count;

                            // Break circular references for serialization
                            records.ForEach(x =>
                            {
                                x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                x.AttendanceCodeNavigation.StudentAttendance = new HashSet<StudentAttendance>();
                                x.StudentCoursesectionSchedule.StudentMaster = new();
                                x.Membership = null;
                                x.StudentAttendanceComments.ToList().ForEach(c =>
                                {
                                    c.Membership!.StudentAttendanceComments = new HashSet<StudentAttendanceComments>();
                                    c.Membership.StudentAttendance = new HashSet<StudentAttendance>();
                                });
                            });
                            administrationViewModel.studentAttendanceList = records;
                        }

                        // Calculate present status from daily attendance
                        if (dailyAttendanceLookup != null && dailyAttendanceLookup.TryGetValue(enrollment.StudentId, out var dailyAtt))
                        {
                            if (dailyAtt.AttendanceMinutes >= blockData?.FullDayMinutes)
                            {
                                administrationViewModel.Present = "Full-Day";
                            }
                            else if (dailyAtt.AttendanceMinutes >= blockData?.HalfDayMinutes)
                            {
                                administrationViewModel.Present = "Half-Day";
                            }
                            else
                            {
                                administrationViewModel.Present = "Absent";
                            }
                            administrationViewModel.AttendanceComment = dailyAtt.AttendanceComment;
                        }

                        attendanceData.Add(administrationViewModel);
                    }

                    // Apply filters, search, sorting, and pagination
                    if (attendanceData.Count > 0)
                    {
                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = attendanceData.AsQueryable();
                        }
                        else
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                transactionIQ = attendanceData.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                x.GradeLevelTitle != null && x.GradeLevelTitle.ToLower().Contains(Columnvalue.ToLower()) ||
                                x.Section != null && x.Section.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
                            }
                            else
                            {
                                transactionIQ = Utility.FilteredData(pageResult.FilterParams!, attendanceData).AsQueryable();

                                //medical advance search
                                var studentGuids = transactionIQ.Select(s => s.StudentGuid).ToList();
                                if (studentGuids.Count > 0)
                                {
                                    var filterStudentIds = Utility.MedicalAdvancedSearch(this.context!, pageResult.FilterParams!, pageResult.TenantId, pageResult.SchoolId, studentGuids);

                                    if (filterStudentIds?.Count > 0)
                                    {
                                        transactionIQ = transactionIQ.Where(x => filterStudentIds.Contains(x.StudentGuid));
                                    }
                                    else
                                    {
                                        transactionIQ = null;
                                    }
                                }
                            }
                        }

                        if (transactionIQ != null)
                        {
                            if (pageResult.SortingModel != null)
                            {
                                switch (pageResult.SortingModel.SortColumn!.ToLower())
                                {
                                    default:
                                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection!.ToLower());
                                        break;
                                }
                            }

                            int? totalCount = transactionIQ.Count();
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                                studentAttendanceList.PageNumber = pageResult.PageNumber;
                                studentAttendanceList._pageSize = pageResult.PageSize;
                            }
                            studentAttendanceList.studendAttendanceAdministrationList = transactionIQ.ToList();
                            studentAttendanceList.TotalCount = totalCount;
                        }
                        else
                        {
                            studentAttendanceList.TotalCount = 0;
                            studentAttendanceList._failure = true;
                            studentAttendanceList._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        studentAttendanceList._failure = true;
                        studentAttendanceList._message = NORECORDFOUND;
                    }
                }
                else
                {
                    studentAttendanceList._failure = true;
                    studentAttendanceList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentAttendanceList._failure = true;
                studentAttendanceList._message = es.Message;
            }
            return studentAttendanceList;
        }

        /// <summary>
        /// Course Section List For Attendance Administration
        /// </summary>
        /// <param name="courseSectionForAttendanceViewModel"></param>
        /// <returns></returns>
        public CourseSectionForAttendanceViewModel CourseSectionListForAttendanceAdministration(CourseSectionForAttendanceViewModel courseSectionForAttendanceViewModel)
        {
            CourseSectionForAttendanceViewModel courseSectionList = new CourseSectionForAttendanceViewModel();

            courseSectionList.TenantId = courseSectionForAttendanceViewModel.TenantId;
            courseSectionList.SchoolId = courseSectionForAttendanceViewModel.SchoolId;
            courseSectionList.AcademicYear = courseSectionForAttendanceViewModel.AcademicYear;
            courseSectionList._tenantName = courseSectionForAttendanceViewModel._tenantName;
            courseSectionList._token = courseSectionForAttendanceViewModel._token;
            try
            {
                var CourseSectionData = this.context?.CourseSection.AsNoTracking().Include(x => x.StudentCoursesectionSchedule).Include(x => x.Course).Include(x => x.SchoolCalendars).Include(x => x.StaffCoursesectionSchedule).Where(x => x.TenantId == courseSectionForAttendanceViewModel.TenantId && x.SchoolId == courseSectionForAttendanceViewModel.SchoolId && x.AcademicYear == courseSectionForAttendanceViewModel.AcademicYear && x.StaffCoursesectionSchedule.Any() && x.StudentCoursesectionSchedule.Any(s => s.IsDropped != true)).ToList();

                if (CourseSectionData?.Any() == true)
                {
                    foreach (var courseSection in CourseSectionData)
                    {
                        var studentExistInCS = courseSection.StudentCoursesectionSchedule.Where(x => x.IsDropped != true).ToList();

                        if (studentExistInCS.Count > 0)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            var CalendarEventsData = this.context?.CalendarEvents.AsNoTracking().Where(e => e.TenantId == courseSectionForAttendanceViewModel.TenantId && e.CalendarId == courseSection.CalendarId && (e.StartDate >= courseSection.DurationStartDate && e.StartDate <= courseSection.DurationEndDate || e.EndDate >= courseSection.DurationStartDate && e.EndDate <= courseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == courseSectionForAttendanceViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

                            if (CalendarEventsData?.Any() == true)
                            {
                                foreach (var calender in CalendarEventsData)
                                {
                                    if (calender.EndDate!.Value.Date > calender.StartDate!.Value.Date)
                                    {
                                        var date = Enumerable.Range(0, 1 + (calender.EndDate.Value.Date - calender.StartDate.Value.Date).Days)
                                           .Select(i => calender.StartDate.Value.Date.AddDays(i))
                                           .ToList();
                                        if (date.Count > 0)
                                        {
                                            CourseSections.HolidayList.AddRange(date);
                                        }
                                    }
                                    CourseSections.HolidayList.Add(calender.StartDate.Value.Date);
                                }
                                CourseSections.HolidayList.Select(x => x.Date).Distinct();
                            }

                            if (courseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                if (courseSection.AttendanceTaken == true)
                                {
                                    CourseSections.ScheduleType = "Fixed Schedule";

                                    var courseFixedScheduleData = this.context?.CourseFixedSchedule.AsNoTracking().Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId);
                                    if (courseFixedScheduleData != null)
                                    {
                                        if (courseFixedScheduleData.BlockPeriod != null)
                                        {
                                            courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                            courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                            //courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = null;
                                            //courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = null;
                                            //courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = null;
                                            //courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = null;
                                        }

                                        CourseSections.courseFixedSchedule = courseFixedScheduleData;
                                        CourseSections.MeetingDays = courseSection.StaffCoursesectionSchedule.Count > 0 ? courseSection.StaffCoursesectionSchedule.FirstOrDefault()?.MeetingDays : null;
                                        CourseSections.CourseId = courseSection.CourseId;
                                        CourseSections.CourseSectionId = courseSection.CourseSectionId;
                                        CourseSections.CourseTitle = courseSection.Course.CourseTitle;
                                        CourseSections.CourseSectionName = courseSection.CourseSectionName;
                                        CourseSections.DurationStartDate = courseSection.DurationStartDate;
                                        CourseSections.DurationEndDate = courseSection.DurationEndDate;
                                        CourseSections.AttendanceCategoryId = courseSection.AttendanceCategoryId;

                                        courseSectionList.courseSectionViewList.Add(CourseSections);
                                    }
                                }
                            }

                            if (courseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseVariableScheduleData?.Any() == true)
                                {
                                    courseVariableScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = courseVariableScheduleData;
                                    CourseSections.MeetingDays = courseSection.StaffCoursesectionSchedule.Count > 0 ? courseSection.StaffCoursesectionSchedule.FirstOrDefault()?.MeetingDays : null;
                                    CourseSections.CourseId = courseSection.CourseId;
                                    CourseSections.CourseSectionId = courseSection.CourseSectionId;
                                    CourseSections.CourseTitle = courseSection.Course.CourseTitle;
                                    CourseSections.CourseSectionName = courseSection.CourseSectionName;
                                    CourseSections.DurationStartDate = courseSection.DurationStartDate;
                                    CourseSections.DurationEndDate = courseSection.DurationEndDate;
                                    CourseSections.AttendanceCategoryId = courseSection.AttendanceCategoryId;

                                    courseSectionList.courseSectionViewList.Add(CourseSections);
                                }
                            }

                            if (courseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseCalenderScheduleData?.Any() == true)
                                {
                                    courseCalenderScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                                    CourseSections.MeetingDays = courseSection.StaffCoursesectionSchedule.Count > 0 ? courseSection.StaffCoursesectionSchedule.FirstOrDefault()?.MeetingDays : null;
                                    CourseSections.CourseId = courseSection.CourseId;
                                    CourseSections.CourseSectionId = courseSection.CourseSectionId;
                                    CourseSections.CourseTitle = courseSection.Course.CourseTitle;
                                    CourseSections.CourseSectionName = courseSection.CourseSectionName;
                                    CourseSections.DurationStartDate = courseSection.DurationStartDate;
                                    CourseSections.DurationEndDate = courseSection.DurationEndDate;
                                    CourseSections.AttendanceCategoryId = courseSection.AttendanceCategoryId;

                                    courseSectionList.courseSectionViewList.Add(CourseSections);
                                }
                            }

                            if (courseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.AsNoTracking().Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseBlockScheduleData?.Any() == true)
                                {
                                    courseBlockScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseBlockSchedule = courseBlockScheduleData;
                                    CourseSections.MeetingDays = courseSection.StaffCoursesectionSchedule.Count > 0 ? courseSection.StaffCoursesectionSchedule.FirstOrDefault()?.MeetingDays : null;
                                    CourseSections.CourseId = courseSection.CourseId;
                                    CourseSections.CourseSectionId = courseSection.CourseSectionId;
                                    CourseSections.CourseTitle = courseSection.Course.CourseTitle;
                                    CourseSections.CourseSectionName = courseSection.CourseSectionName;
                                    CourseSections.DurationStartDate = courseSection.DurationStartDate;
                                    CourseSections.DurationEndDate = courseSection.DurationEndDate;
                                    CourseSections.AttendanceCategoryId = courseSection.AttendanceCategoryId;

                                    //for bellSchedule list return — batch-load for all blocks instead of per-block N+1
                                    var blockIds = courseBlockScheduleData.Select(b => b.BlockId).Distinct().ToList();
                                    var bellScheduleList = this.context?.BellSchedule.AsNoTracking().Where(c => c.SchoolId == courseSection.SchoolId && c.TenantId == courseSection.TenantId && blockIds.Contains(c.BlockId) && c.BellScheduleDate >= courseSection.DurationStartDate && c.BellScheduleDate <= courseSection.DurationEndDate).ToList() ?? new List<BellSchedule>();

                                    CourseSections.bellScheduleList = bellScheduleList;
                                    courseSectionList.courseSectionViewList.Add(CourseSections);
                                }
                            }
                        }
                    }
                }
                else
                {
                    courseSectionList._failure = true;
                    courseSectionList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseSectionList._failure = true;
                courseSectionList._message = es.Message;
            }
            return courseSectionList;
        }

        /// <summary>
        /// Add Absences
        /// </summary>
        /// <param name="studentAttendanceListViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel AddAbsences(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (studentAttendanceAddViewModel.studentAttendance?.Any() == true)
                    {
                        List<StudentAttendance> studentAttendanceList = new List<StudentAttendance>();
                        List<StudentAttendanceComments> studentAttendanceCommentsList = new List<StudentAttendanceComments>();
                        List<StudentDailyAttendance> studentDailyAttendances = new List<StudentDailyAttendance>();

                        long? StudentAttendanceId = 1;

                        var maxAbsAttendanceId = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).Max(x => (long?)x.StudentAttendanceId);

                        if (maxAbsAttendanceId != null)
                        {
                            StudentAttendanceId = maxAbsAttendanceId + 1;
                        }

                        long? CommentId = Utility.GetMaxLongPK<StudentAttendanceComments>(this.context, x => x.CommentId);

                        var allCsData = this.context?.AllCourseSectionView.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId).ToList();

                        var staffId = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.IsDropped != true)?.StaffId;
                        if (allCsData?.Any() == true)
                        {
                            // Batch-load data used inside the per-student loop (eliminates N+1 queries)
                            int? blockIde = this.context?.Block.Where(b => b.TenantId == studentAttendanceAddViewModel.TenantId && b.SchoolId == studentAttendanceAddViewModel.SchoolId && b.AcademicYear == Utility.GetCurrentAcademicYear(this.context!, studentAttendanceAddViewModel.TenantId, studentAttendanceAddViewModel.SchoolId)).FirstOrDefault()!.BlockId;

                            var absStudentIds = studentAttendanceAddViewModel.studentAttendance.Select(s => s.StudentId).ToList();
                            var absAttendanceDates = studentAttendanceAddViewModel.studentAttendance.Select(s => s.AttendanceDate).Distinct().ToList();

                            var existingAttendanceLookup = this.context?.StudentAttendance
                                .Include(x => x.StudentAttendanceComments)
                                .Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && absStudentIds.Contains(x.StudentId) && absAttendanceDates.Contains(x.AttendanceDate))
                                .ToList();

                            var bellScheduleLookup = this.context?.BellSchedule
                                .Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && absAttendanceDates.Contains(x.BellScheduleDate))
                                .ToDictionary(x => x.BellScheduleDate);

                            foreach (var studentAttendance in studentAttendanceAddViewModel.studentAttendance)
                            {
                                int? periodIde = null;
                                if (allCsData.FirstOrDefault()!.ScheduleType == "Fixed Schedule (1)")
                                {
                                    periodIde = allCsData.FirstOrDefault()!.FixedPeriodId;

                                    var attendanceDataForStudent = existingAttendanceLookup?.FirstOrDefault(x => x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId);

                                    if (attendanceDataForStudent != null)
                                    {
                                        if (attendanceDataForStudent.StudentAttendanceComments?.Any() == true)
                                        {
                                            var attendanceCommentDataForStudent = attendanceDataForStudent.StudentAttendanceComments.FirstOrDefault(x => x.MembershipId == 1);

                                            if (attendanceCommentDataForStudent != null)
                                            {
                                                if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                {
                                                    attendanceCommentDataForStudent.Comment = studentAttendanceAddViewModel.AbsencesReason;
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                {
                                                    var StudentAttendanceComments = new StudentAttendanceComments
                                                    {
                                                        TenantId = studentAttendanceAddViewModel.TenantId,
                                                        SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                        StudentId = studentAttendance.StudentId,
                                                        StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                        CommentId = (long)CommentId!,
                                                        Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                        CommentTimestamp = DateTime.UtcNow,
                                                        CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = studentAttendanceAddViewModel.MembershipId
                                                    };
                                                    studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                    CommentId++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                            {
                                                var StudentAttendanceComments = new StudentAttendanceComments
                                                {
                                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                    StudentId = studentAttendance.StudentId,
                                                    StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                    CommentId = (long)CommentId!,
                                                    Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                    CommentTimestamp = DateTime.UtcNow,
                                                    CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow,
                                                    MembershipId = studentAttendanceAddViewModel.MembershipId
                                                };
                                                studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                CommentId++;
                                            }
                                        }

                                        attendanceDataForStudent.AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!;
                                    }
                                    else
                                    {
                                        if (allCsData.FirstOrDefault()!.AttendanceTaken == true)
                                        {
                                            var studentAttendanceAdd = new StudentAttendance()
                                            {
                                                TenantId = studentAttendanceAddViewModel.TenantId,
                                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                StudentId = studentAttendance.StudentId,
                                                StaffId = (int)staffId!,
                                                CourseId = studentAttendanceAddViewModel.CourseId,
                                                CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                                AttendanceCategoryId = (int)studentAttendanceAddViewModel.AttendanceCategoryId!,
                                                AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!,
                                                AttendanceDate = studentAttendance.AttendanceDate,
                                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                BlockId = (int)blockIde!,
                                                PeriodId = (int)periodIde!,
                                                StudentAttendanceId = (int)StudentAttendanceId,
                                                MembershipId = studentAttendanceAddViewModel.MembershipId
                                            };
                                            studentAttendanceList.Add(studentAttendanceAdd);

                                            if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                            {
                                                var StudentAttendanceComments = new StudentAttendanceComments
                                                {
                                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                    StudentId = studentAttendance.StudentId,
                                                    StudentAttendanceId = (int)StudentAttendanceId,
                                                    CommentId = (long)CommentId!,
                                                    Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                    CommentTimestamp = DateTime.UtcNow,
                                                    CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow,
                                                    MembershipId = studentAttendanceAddViewModel.MembershipId
                                                };
                                                studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                CommentId++;
                                            }

                                            StudentAttendanceId++;
                                        }
                                    }
                                }
                                if (allCsData.FirstOrDefault()!.ScheduleType == "Variable Schedule (2)")
                                {
                                    //var day = studentAttendance.AttendanceDate.DayOfWeek.ToString();
                                    //periodIde = allCsData.Where(x => x.VarDay.ToLower() == day.ToLower()).Select(s => s.VarPeriodId).FirstOrDefault();
                                    var day = studentAttendance.AttendanceDate.DayOfWeek.ToString();
                                    var periodIds = allCsData.AsEnumerable().Where(x => String.Compare(x.VarDay, day, true) == 0).ToList();

                                    if (periodIds?.Any() == true)
                                    {
                                        foreach (var periodId in periodIds)
                                        {
                                            var attendanceDataForStudent = existingAttendanceLookup?.FirstOrDefault(x => x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.PeriodId == periodId.VarPeriodId);

                                            if (attendanceDataForStudent != null)
                                            {
                                                if (attendanceDataForStudent.StudentAttendanceComments?.Any() == true)
                                                {
                                                    var attendanceCommentDataForStudent = attendanceDataForStudent.StudentAttendanceComments.FirstOrDefault(x => x.MembershipId == 1);

                                                    if (attendanceCommentDataForStudent != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                        {
                                                            attendanceCommentDataForStudent.Comment = studentAttendanceAddViewModel.AbsencesReason;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                        {
                                                            var StudentAttendanceComments = new StudentAttendanceComments
                                                            {
                                                                TenantId = studentAttendanceAddViewModel.TenantId,
                                                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                                StudentId = studentAttendance.StudentId,
                                                                StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                                CommentId = (long)CommentId!,
                                                                Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                                CommentTimestamp = DateTime.UtcNow,
                                                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                                CreatedOn = DateTime.UtcNow,
                                                                MembershipId = studentAttendanceAddViewModel.MembershipId
                                                            };
                                                            studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                            CommentId++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                    {
                                                        var StudentAttendanceComments = new StudentAttendanceComments
                                                        {
                                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                            StudentId = studentAttendance.StudentId,
                                                            StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                            CommentId = (long)CommentId!,
                                                            Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                            CommentTimestamp = DateTime.UtcNow,
                                                            CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                            CreatedOn = DateTime.UtcNow,
                                                            MembershipId = studentAttendanceAddViewModel.MembershipId
                                                        };
                                                        studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                        CommentId++;
                                                    }
                                                }

                                                attendanceDataForStudent.AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!;
                                            }
                                            else
                                            {
                                                if (periodId.TakeAttendanceVariable == true)
                                                {
                                                    var studentAttendanceAdd = new StudentAttendance()
                                                    {
                                                        TenantId = studentAttendanceAddViewModel.TenantId,
                                                        SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                        StudentId = studentAttendance.StudentId,
                                                        StaffId = (int)staffId!,
                                                        CourseId = studentAttendanceAddViewModel.CourseId,
                                                        CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                                        AttendanceCategoryId = (int)studentAttendanceAddViewModel.AttendanceCategoryId!,
                                                        AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!,
                                                        AttendanceDate = studentAttendance.AttendanceDate,
                                                        CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        BlockId = (int)blockIde!,
                                                        PeriodId = (int)periodId.VarPeriodId!,
                                                        StudentAttendanceId = (int)StudentAttendanceId,
                                                        MembershipId = studentAttendanceAddViewModel.MembershipId
                                                    };
                                                    studentAttendanceList.Add(studentAttendanceAdd);

                                                    if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                    {
                                                        var StudentAttendanceComments = new StudentAttendanceComments
                                                        {
                                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                            StudentId = studentAttendance.StudentId,
                                                            StudentAttendanceId = (int)StudentAttendanceId,
                                                            CommentId = (long)CommentId!,
                                                            Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                            CommentTimestamp = DateTime.UtcNow,
                                                            CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                            CreatedOn = DateTime.UtcNow,
                                                            MembershipId = studentAttendanceAddViewModel.MembershipId
                                                        };
                                                        studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                        CommentId++;
                                                    }

                                                    StudentAttendanceId++;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (allCsData.FirstOrDefault()!.ScheduleType == "Calendar Schedule (3)")
                                {
                                    var periodIds = allCsData.Where(x => x.CalDate == studentAttendance.AttendanceDate).ToList();

                                    if (periodIds?.Any() == true)
                                    {
                                        foreach (var periodId in periodIds)
                                        {
                                            var attendanceDataForStudent = existingAttendanceLookup?.FirstOrDefault(x => x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.PeriodId == periodId.CalPeriodId);

                                            if (attendanceDataForStudent != null)
                                            {
                                                if (attendanceDataForStudent.StudentAttendanceComments?.Any() == true)
                                                {
                                                    var attendanceCommentDataForStudent = attendanceDataForStudent.StudentAttendanceComments.FirstOrDefault(x => x.MembershipId == 1);

                                                    if (attendanceCommentDataForStudent != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                        {
                                                            attendanceCommentDataForStudent.Comment = studentAttendanceAddViewModel.AbsencesReason;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                        {
                                                            var StudentAttendanceComments = new StudentAttendanceComments
                                                            {
                                                                TenantId = studentAttendanceAddViewModel.TenantId,
                                                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                                StudentId = studentAttendance.StudentId,
                                                                StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                                CommentId = (long)CommentId!,
                                                                Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                                CommentTimestamp = DateTime.UtcNow,
                                                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                                CreatedOn = DateTime.UtcNow,
                                                                MembershipId = studentAttendanceAddViewModel.MembershipId
                                                            };
                                                            studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                            CommentId++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                    {
                                                        var StudentAttendanceComments = new StudentAttendanceComments
                                                        {
                                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                            StudentId = studentAttendance.StudentId,
                                                            StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                            CommentId = (long)CommentId!,
                                                            Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                            CommentTimestamp = DateTime.UtcNow,
                                                            CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                            CreatedOn = DateTime.UtcNow,
                                                            MembershipId = studentAttendanceAddViewModel.MembershipId
                                                        };
                                                        studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                        CommentId++;
                                                    }
                                                }

                                                attendanceDataForStudent.AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!;
                                            }
                                            else
                                            {
                                                if (periodId.TakeAttendanceCalendar == true)
                                                {
                                                    var studentAttendanceAdd = new StudentAttendance()
                                                    {
                                                        TenantId = studentAttendanceAddViewModel.TenantId,
                                                        SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                        StudentId = studentAttendance.StudentId,
                                                        StaffId = (int)staffId!,
                                                        CourseId = studentAttendanceAddViewModel.CourseId,
                                                        CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                                        AttendanceCategoryId = (int)studentAttendanceAddViewModel.AttendanceCategoryId!,
                                                        AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!,
                                                        AttendanceDate = studentAttendance.AttendanceDate,
                                                        CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        BlockId = (int)blockIde!,
                                                        PeriodId = (int)periodId.CalPeriodId!,
                                                        StudentAttendanceId = (int)StudentAttendanceId,
                                                        MembershipId = studentAttendanceAddViewModel.MembershipId
                                                    };
                                                    studentAttendanceList.Add(studentAttendanceAdd);

                                                    if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                    {
                                                        var StudentAttendanceComments = new StudentAttendanceComments
                                                        {
                                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                            StudentId = studentAttendance.StudentId,
                                                            StudentAttendanceId = (int)StudentAttendanceId,
                                                            CommentId = (long)CommentId!,
                                                            Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                            CommentTimestamp = DateTime.UtcNow,
                                                            CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                            CreatedOn = DateTime.UtcNow,
                                                            MembershipId = studentAttendanceAddViewModel.MembershipId
                                                        };
                                                        studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                        CommentId++;
                                                    }

                                                    StudentAttendanceId++;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (allCsData.FirstOrDefault()!.ScheduleType == "Block Schedule (4)")
                                {
                                    BellSchedule? BellScheduleData = null;
                                    bellScheduleLookup?.TryGetValue(studentAttendance.AttendanceDate, out BellScheduleData);
                                    if (BellScheduleData != null)
                                    {
                                        blockIde = BellScheduleData.BlockId;
                                        periodIde = allCsData.Where(x => x.BlockId == BellScheduleData.BlockId).Select(s => s.BlockPeriodId).FirstOrDefault();
                                    }

                                    var attendanceDataForStudent = existingAttendanceLookup?.FirstOrDefault(x => x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId);

                                    if (attendanceDataForStudent != null)
                                    {
                                        if (attendanceDataForStudent.StudentAttendanceComments?.Any() == true)
                                        {
                                            var attendanceCommentDataForStudent = attendanceDataForStudent.StudentAttendanceComments.FirstOrDefault(x => x.MembershipId == 1);

                                            if (attendanceCommentDataForStudent != null)
                                            {
                                                if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                {
                                                    attendanceCommentDataForStudent.Comment = studentAttendanceAddViewModel.AbsencesReason;
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                                {
                                                    var StudentAttendanceComments = new StudentAttendanceComments
                                                    {
                                                        TenantId = studentAttendanceAddViewModel.TenantId,
                                                        SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                        StudentId = studentAttendance.StudentId,
                                                        StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                        CommentId = (long)CommentId!,
                                                        Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                        CommentTimestamp = DateTime.UtcNow,
                                                        CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = studentAttendanceAddViewModel.MembershipId
                                                    };
                                                    studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                    CommentId++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                            {
                                                var StudentAttendanceComments = new StudentAttendanceComments
                                                {
                                                    TenantId = studentAttendanceAddViewModel.TenantId,
                                                    SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                    StudentId = studentAttendance.StudentId,
                                                    StudentAttendanceId = attendanceDataForStudent.StudentAttendanceId,
                                                    CommentId = (long)CommentId!,
                                                    Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                    CommentTimestamp = DateTime.UtcNow,
                                                    CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow,
                                                    MembershipId = studentAttendanceAddViewModel.MembershipId
                                                };
                                                studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                                CommentId++;
                                            }
                                        }

                                        attendanceDataForStudent.AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!;
                                    }
                                    else
                                    {
                                        var studentAttendanceAdd = new StudentAttendance()
                                        {
                                            TenantId = studentAttendanceAddViewModel.TenantId,
                                            SchoolId = studentAttendanceAddViewModel.SchoolId,
                                            StudentId = studentAttendance.StudentId,
                                            StaffId = (int)staffId!,
                                            CourseId = studentAttendanceAddViewModel.CourseId,
                                            CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                            AttendanceCategoryId = (int)studentAttendanceAddViewModel.AttendanceCategoryId!,
                                            AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode!,
                                            AttendanceDate = studentAttendance.AttendanceDate,
                                            CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                            BlockId = (int)blockIde!,
                                            PeriodId = (int)periodIde!,
                                            StudentAttendanceId = (int)StudentAttendanceId,
                                            MembershipId = studentAttendanceAddViewModel.MembershipId
                                        };
                                        studentAttendanceList.Add(studentAttendanceAdd);

                                        if (!string.IsNullOrEmpty(studentAttendanceAddViewModel.AbsencesReason))
                                        {
                                            var StudentAttendanceComments = new StudentAttendanceComments
                                            {
                                                TenantId = studentAttendanceAddViewModel.TenantId,
                                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                                StudentId = studentAttendance.StudentId,
                                                StudentAttendanceId = (int)StudentAttendanceId,
                                                CommentId = (long)CommentId!,
                                                Comment = studentAttendanceAddViewModel.AbsencesReason,
                                                CommentTimestamp = DateTime.UtcNow,
                                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                MembershipId = studentAttendanceAddViewModel.MembershipId
                                            };
                                            studentAttendanceCommentsList.Add(StudentAttendanceComments);
                                            CommentId++;
                                        }

                                        StudentAttendanceId++;
                                    }
                                }
                            }
                        }

                        this.context?.StudentAttendance.AddRange(studentAttendanceList);
                        this.context?.StudentAttendanceComments.AddRange(studentAttendanceCommentsList);
                        this.context?.SaveChanges();

                        //Insert into daily attendance table — batch-load lookups to eliminate N+1 queries
                        var dailyStudentIds = studentAttendanceAddViewModel.studentAttendance.Select(s => s.StudentId).ToList();
                        var dailyAttendanceDates = studentAttendanceAddViewModel.studentAttendance.Select(s => s.AttendanceDate).Distinct().ToList();

                        var allAttendanceForDaily = this.context?.StudentAttendance
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && dailyStudentIds.Contains(x.StudentId) && dailyAttendanceDates.Contains(x.AttendanceDate))
                            .ToList()
                            .GroupBy(x => (x.StudentId, x.AttendanceDate))
                            .ToDictionary(g => g.Key, g => g.ToList());

                        var blockPeriodLookup = this.context?.BlockPeriod
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                            .ToDictionary(x => (x.BlockId, x.PeriodId));

                        var attendanceCodeList = this.context?.AttendanceCode
                            .AsNoTracking()
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId)
                            .ToList();

                        var existingDailyAttendance = this.context?.StudentDailyAttendance
                            .Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && dailyStudentIds.Contains(x.StudentId) && dailyAttendanceDates.Contains(x.AttendanceDate))
                            .ToList()
                            .GroupBy(x => (x.StudentId, x.AttendanceDate))
                            .ToDictionary(g => g.Key, g => g.First());

                        foreach (var studentId in studentAttendanceAddViewModel.studentAttendance)
                        {
                            int totalAttendanceMin = 0;
                            if (allAttendanceForDaily != null && allAttendanceForDaily.TryGetValue((studentId.StudentId, studentId.AttendanceDate), out var attendanceData))
                            {
                                foreach (var attendance in attendanceData)
                                {
                                    if (blockPeriodLookup != null && blockPeriodLookup.TryGetValue((attendance.BlockId, attendance.PeriodId), out var BlockPeriodData))
                                    {
                                        // Only count periods flagged as calculating attendance
                                        if (BlockPeriodData.CalculateAttendance != true)
                                            continue;

                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = attendanceCodeList?.FirstOrDefault(x => x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            if (AttendanceCodeData.StateCode!.ToLower() != "absent".ToLower())
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                            }
                            if (existingDailyAttendance != null && existingDailyAttendance.TryGetValue((studentId.StudentId, studentId.AttendanceDate), out var studentDailyAttendanceData))
                            {
                                studentDailyAttendanceData.AttendanceMinutes = totalAttendanceMin;
                            }
                            else
                            {
                                var studentDailyAttendance = new StudentDailyAttendance { TenantId = studentAttendanceAddViewModel.TenantId, SchoolId = studentAttendanceAddViewModel.SchoolId, StudentId = studentId.StudentId, AttendanceDate = studentId.AttendanceDate, CreatedBy = studentAttendanceAddViewModel.CreatedBy, AttendanceMinutes = totalAttendanceMin, CreatedOn = DateTime.UtcNow };

                                studentDailyAttendances.Add(studentDailyAttendance);
                            }
                        }
                        this.context?.StudentDailyAttendance.AddRange(studentDailyAttendances);
                        this.context?.SaveChanges();

                        transaction?.Commit();
                        studentAttendanceAddViewModel._message = "Attendance records saved successfully";
                    }
                    else
                    {
                        studentAttendanceAddViewModel._failure = true;
                        studentAttendanceAddViewModel._message = "Please select student";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentAttendanceAddViewModel._failure = true;
                    studentAttendanceAddViewModel._message = es.InnerException?.Message ?? es.Message;
                }
            }
            return studentAttendanceAddViewModel;
        }

        /// <summary>
        /// Fill Attendance As Present
        /// For all course sections (or a specific one), fills missing attendance
        /// records with the "Present" attendance code from DurationStartDate
        /// to min(today, DurationEndDate).
        /// </summary>
        /// <param name="fillAttendanceViewModel"></param>
        /// <returns></returns>
        public FillAttendanceViewModel FillAttendanceAsPresent(FillAttendanceViewModel fillAttendanceViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var academicYear = Utility.GetCurrentAcademicYear(this.context!, fillAttendanceViewModel.TenantId, fillAttendanceViewModel.SchoolId);

                    // Get course sections: all or specific one
                    var courseSectionsQuery = this.context?.CourseSection
                        .Include(cs => cs.StaffCoursesectionSchedule)
                        .Include(cs => cs.Course)
                        .Where(cs => cs.TenantId == fillAttendanceViewModel.TenantId
                            && cs.SchoolId == fillAttendanceViewModel.SchoolId
                            && cs.AcademicYear == academicYear
                            && cs.IsActive == true
                            && cs.DurationStartDate != null
                            && cs.DurationEndDate != null);

                    if (fillAttendanceViewModel.CourseSectionId != null)
                    {
                        courseSectionsQuery = courseSectionsQuery?.Where(cs => cs.CourseSectionId == fillAttendanceViewModel.CourseSectionId);
                    }

                    var courseSections = courseSectionsQuery?.ToList();

                    if (courseSections == null || !courseSections.Any())
                    {
                        fillAttendanceViewModel._failure = true;
                        fillAttendanceViewModel._message = "No active course sections found";
                        return fillAttendanceViewModel;
                    }

                    // Batch-load shared lookups
                    int? blockId = this.context?.Block.Where(b => b.TenantId == fillAttendanceViewModel.TenantId && b.SchoolId == fillAttendanceViewModel.SchoolId && b.AcademicYear == academicYear).FirstOrDefault()?.BlockId;

                    var allCourseSectionView = this.context?.AllCourseSectionView.AsNoTracking()
                        .Where(v => v.SchoolId == fillAttendanceViewModel.SchoolId && v.TenantId == fillAttendanceViewModel.TenantId && v.AcademicYear == academicYear)
                        .ToList();

                    var blockPeriodLookup = this.context?.BlockPeriod.AsNoTracking()
                        .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId)
                        .ToDictionary(x => (x.BlockId, x.PeriodId));

                    var attendanceCodeList = this.context?.AttendanceCode.AsNoTracking()
                        .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId && x.AcademicYear == academicYear)
                        .ToList();

                    // Batch-load ALL existing attendance for this school/year to avoid per-section queries
                    var csIds = courseSections.Select(cs => cs.CourseSectionId).ToList();

                    var existingAttendanceSet = new HashSet<(int StudentId, int CourseSectionId, DateTime Date, int PeriodId)>(
                        this.context?.StudentAttendance.AsNoTracking()
                            .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId && csIds.Contains(x.CourseSectionId))
                            .Select(x => new { x.StudentId, x.CourseSectionId, x.AttendanceDate, x.PeriodId })
                            .ToList()
                            .Select(x => (x.StudentId, x.CourseSectionId, x.AttendanceDate, x.PeriodId))
                        ?? Enumerable.Empty<(int, int, DateTime, int)>()
                    );

                    // Batch-load all enrolled students per course section
                    var enrolledStudents = this.context?.StudentCoursesectionSchedule.AsNoTracking()
                        .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId && csIds.Contains(x.CourseSectionId) && x.IsDropped != true)
                        .ToList()
                        .GroupBy(x => x.CourseSectionId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Batch-load holidays per calendar
                    var calendarIds = courseSections.Select(cs => cs.CalendarId).Where(id => id != null).Distinct().ToList();
                    var holidayEvents = this.context?.CalendarEvents.AsNoTracking()
                        .Where(e => e.TenantId == fillAttendanceViewModel.TenantId && calendarIds.Contains(e.CalendarId) && e.IsHoliday == true && (e.SchoolId == fillAttendanceViewModel.SchoolId || e.ApplicableToAllSchool == true))
                        .ToList();

                    var holidaysByCalendar = new Dictionary<int, HashSet<DateTime>>();
                    foreach (var calId in calendarIds)
                    {
                        var holidays = new HashSet<DateTime>();
                        var eventsForCal = holidayEvents?.Where(e => e.CalendarId == calId).ToList();
                        if (eventsForCal != null)
                        {
                            foreach (var evt in eventsForCal)
                            {
                                if (evt.StartDate != null)
                                {
                                    holidays.Add(evt.StartDate.Value.Date);
                                    if (evt.EndDate != null && evt.EndDate.Value.Date > evt.StartDate.Value.Date)
                                    {
                                        var range = Enumerable.Range(0, 1 + (evt.EndDate.Value.Date - evt.StartDate.Value.Date).Days)
                                            .Select(i => evt.StartDate.Value.Date.AddDays(i));
                                        foreach (var d in range) holidays.Add(d);
                                    }
                                }
                            }
                        }
                        holidaysByCalendar[(int)calId!] = holidays;
                    }

                    // Batch-load bell schedules for block schedule sections
                    var blockCsSections = courseSections.Where(cs => cs.ScheduleType == "Block Schedule (4)").ToList();
                    var bellScheduleLookup = new Dictionary<int, List<BellSchedule>>();
                    if (blockCsSections.Any())
                    {
                        var blockCsIds = blockCsSections.Select(cs => cs.CourseSectionId).ToList();
                        var blockIdsForBell = allCourseSectionView?.Where(v => blockCsIds.Contains(v.CourseSectionId) && v.BlockId.HasValue).Select(v => v.BlockId!.Value).Distinct().ToList();
                        if (blockIdsForBell?.Any() == true)
                        {
                            var allBellSchedules = this.context?.BellSchedule.AsNoTracking()
                                .Where(b => b.TenantId == fillAttendanceViewModel.TenantId && b.SchoolId == fillAttendanceViewModel.SchoolId && b.BlockId.HasValue && blockIdsForBell.Contains(b.BlockId.Value))
                                .ToList();
                            if (allBellSchedules != null)
                            {
                                bellScheduleLookup = allBellSchedules.Where(b => b.BlockId.HasValue).GroupBy(b => b.BlockId!.Value).ToDictionary(g => g.Key, g => g.ToList());
                            }
                        }
                    }

                    // Batch-load existing daily attendance
                    var existingDailyLookup = this.context?.StudentDailyAttendance
                        .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId)
                        .ToList()
                        .GroupBy(x => (x.StudentId, x.AttendanceDate))
                        .ToDictionary(g => g.Key, g => g.First());

                    long? StudentAttendanceId = 1;
                    var maxAttendanceId = this.context?.StudentAttendance.Where(x => x.SchoolId == fillAttendanceViewModel.SchoolId && x.TenantId == fillAttendanceViewModel.TenantId).Max(x => (long?)x.StudentAttendanceId);
                    if (maxAttendanceId != null)
                    {
                        StudentAttendanceId = maxAttendanceId + 1;
                    }

                    long? CommentId = Utility.GetMaxLongPK<StudentAttendanceComments>(this.context, x => x.CommentId);

                    DateTime today = DateTime.Today.Date;
                    List<StudentAttendance> newAttendanceRecords = new List<StudentAttendance>();
                    List<StudentAttendanceComments> newCommentRecords = new List<StudentAttendanceComments>();
                    // Track (studentId, date) for daily attendance update
                    HashSet<(int StudentId, DateTime Date)> affectedStudentDates = new HashSet<(int, DateTime)>();
                    int sectionsProcessed = 0;

                    foreach (var courseSection in courseSections)
                    {
                        var csViewData = allCourseSectionView?.Where(v => v.CourseId == courseSection.CourseId && v.CourseSectionId == courseSection.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                        if (csViewData == null || !csViewData.Any()) continue;

                        var holidays = courseSection.CalendarId != null && holidaysByCalendar.TryGetValue((int)courseSection.CalendarId, out var h) ? h : new HashSet<DateTime>();

                        // Get the "Present" attendance code for this section's category
                        // Pick the Present code for this section's category.
                        // Prefer the one marked as DefaultCode, then by ShortName "P", then lowest code number for deterministic results.
                        var presentCode = attendanceCodeList?
                            .Where(x => x.AttendanceCategoryId == courseSection.AttendanceCategoryId && x.StateCode != null && x.StateCode.ToLower() == "present")
                            .OrderByDescending(x => x.DefaultCode == true)
                            .ThenBy(x => x.ShortName != null && x.ShortName.ToLower() == "p" ? 0 : 1)
                            .ThenBy(x => x.AttendanceCode1)
                            .FirstOrDefault();
                        if (presentCode == null) continue;

                        var staffId = courseSection.StaffCoursesectionSchedule?.FirstOrDefault(x => x.IsDropped != true)?.StaffId ?? 0;

                        var sectionStudents = enrolledStudents != null && enrolledStudents.TryGetValue(courseSection.CourseSectionId, out var enrolled) ? enrolled : null;
                        if (sectionStudents == null || !sectionStudents.Any()) continue;

                        DateTime startDate = courseSection.DurationStartDate!.Value.Date;
                        DateTime endDate = courseSection.DurationEndDate!.Value.Date;
                        if (endDate > today) endDate = today;
                        if (startDate > endDate) continue;

                        // Determine date-period pairs based on schedule type
                        var datePeriodPairs = new List<(DateTime date, int periodId, int effectiveBlockId)>();

                        if (courseSection.ScheduleType == "Fixed Schedule (1)")
                        {
                            var fixedPeriodId = csViewData.FirstOrDefault()?.FixedPeriodId;
                            if (fixedPeriodId == null || csViewData.FirstOrDefault()?.AttendanceTaken != true) continue;

                            var meetingDays = courseSection.StaffCoursesectionSchedule?.FirstOrDefault(x => x.IsDropped != true)?.MeetingDays?.ToLower().Split("|");
                            bool allDays = meetingDays == null || !meetingDays.Any();

                            var dateList = Enumerable.Range(0, 1 + (endDate - startDate).Days)
                                .Select(offset => startDate.AddDays(offset))
                                .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                .Where(d => !holidays.Contains(d.Date))
                                .ToList();

                            foreach (var date in dateList)
                            {
                                datePeriodPairs.Add((date, (int)fixedPeriodId, blockId ?? 0));
                            }
                        }
                        else if (courseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            var meetingDays = courseSection.StaffCoursesectionSchedule?.FirstOrDefault(x => x.IsDropped != true)?.MeetingDays?.ToLower().Split("|");
                            bool allDays = meetingDays == null || !meetingDays.Any();

                            var dateList = Enumerable.Range(0, 1 + (endDate - startDate).Days)
                                .Select(offset => startDate.AddDays(offset))
                                .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                .Where(d => !holidays.Contains(d.Date))
                                .ToList();

                            foreach (var date in dateList)
                            {
                                var dayName = date.DayOfWeek.ToString();
                                var periodsForDay = csViewData.Where(x => x.VarDay != null && String.Compare(x.VarDay, dayName, true) == 0 && x.TakeAttendanceVariable == true).ToList();
                                foreach (var p in periodsForDay)
                                {
                                    if (p.VarPeriodId != null)
                                    {
                                        datePeriodPairs.Add((date, (int)p.VarPeriodId, blockId ?? 0));
                                    }
                                }
                            }
                        }
                        else if (courseSection.ScheduleType == "Calendar Schedule (3)")
                        {
                            var calEntries = csViewData.Where(c => c.CalDate != null && c.CalDate.Value.Date >= startDate && c.CalDate.Value.Date <= endDate && !holidays.Contains(c.CalDate.Value.Date) && c.TakeAttendanceCalendar == true).ToList();

                            foreach (var cal in calEntries)
                            {
                                if (cal.CalPeriodId != null)
                                {
                                    datePeriodPairs.Add((cal.CalDate!.Value.Date, (int)cal.CalPeriodId, blockId ?? 0));
                                }
                            }
                        }
                        else if (courseSection.ScheduleType == "Block Schedule (4)")
                        {
                            var blockViewData = csViewData.Where(v => v.BlockId.HasValue && v.BlockPeriodId.HasValue && v.TakeAttendanceBlock == true).ToList();

                            foreach (var blockView in blockViewData)
                            {
                                if (bellScheduleLookup.TryGetValue(blockView.BlockId!.Value, out var bells))
                                {
                                    var validBells = bells.Where(b => b.BellScheduleDate >= startDate && b.BellScheduleDate <= endDate && !holidays.Contains(b.BellScheduleDate)).ToList();
                                    foreach (var bell in validBells)
                                    {
                                        datePeriodPairs.Add((bell.BellScheduleDate, blockView.BlockPeriodId!.Value, bell.BlockId ?? 0));
                                    }
                                }
                            }
                        }

                        if (!datePeriodPairs.Any()) continue;

                        sectionsProcessed++;

                        // For each student x date-period, check if attendance exists
                        foreach (var student in sectionStudents)
                        {
                            foreach (var (date, periodId, effectiveBlockId) in datePeriodPairs)
                            {
                                // Check student was enrolled on this date
                                if (student.EffectiveStartDate != null && student.EffectiveStartDate.Value.Date > date) continue;

                                // Skip if attendance already exists
                                if (existingAttendanceSet.Contains((student.StudentId, courseSection.CourseSectionId, date, periodId))) continue;

                                var attendanceRecord = new StudentAttendance()
                                {
                                    TenantId = fillAttendanceViewModel.TenantId,
                                    SchoolId = fillAttendanceViewModel.SchoolId,
                                    StudentId = student.StudentId,
                                    StaffId = staffId,
                                    CourseId = courseSection.CourseId,
                                    CourseSectionId = courseSection.CourseSectionId,
                                    AttendanceCategoryId = (int)courseSection.AttendanceCategoryId!,
                                    AttendanceCode = (int)presentCode.AttendanceCode1,
                                    AttendanceDate = date,
                                    CreatedBy = fillAttendanceViewModel.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    BlockId = effectiveBlockId,
                                    PeriodId = periodId,
                                    StudentAttendanceId = (int)StudentAttendanceId!,
                                    MembershipId = fillAttendanceViewModel.MembershipId
                                };

                                newAttendanceRecords.Add(attendanceRecord);

                                var commentRecord = new StudentAttendanceComments
                                {
                                    TenantId = fillAttendanceViewModel.TenantId,
                                    SchoolId = fillAttendanceViewModel.SchoolId,
                                    StudentId = student.StudentId,
                                    StudentAttendanceId = (int)StudentAttendanceId!,
                                    CommentId = (long)CommentId!,
                                    Comment = "",
                                    CommentTimestamp = DateTime.UtcNow,
                                    CreatedBy = fillAttendanceViewModel.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    MembershipId = fillAttendanceViewModel.MembershipId
                                };
                                newCommentRecords.Add(commentRecord);
                                CommentId++;

                                existingAttendanceSet.Add((student.StudentId, courseSection.CourseSectionId, date, periodId));
                                affectedStudentDates.Add((student.StudentId, date));
                                StudentAttendanceId++;
                            }
                        }
                    }

                    if (newAttendanceRecords.Any())
                    {
                        this.context?.StudentAttendance.AddRange(newAttendanceRecords);
                        this.context?.StudentAttendanceComments.AddRange(newCommentRecords);
                        this.context?.SaveChanges();

                        // Update StudentDailyAttendance for affected student-date combos
                        var affectedStudentIds = affectedStudentDates.Select(x => x.StudentId).Distinct().ToList();
                        var affectedDates = affectedStudentDates.Select(x => x.Date).Distinct().ToList();

                        var allAttendanceForDaily = this.context?.StudentAttendance.AsNoTracking()
                            .Where(x => x.TenantId == fillAttendanceViewModel.TenantId && x.SchoolId == fillAttendanceViewModel.SchoolId && affectedStudentIds.Contains(x.StudentId) && affectedDates.Contains(x.AttendanceDate))
                            .ToList()
                            .GroupBy(x => (x.StudentId, x.AttendanceDate))
                            .ToDictionary(g => g.Key, g => g.ToList());

                        List<StudentDailyAttendance> newDailyRecords = new List<StudentDailyAttendance>();

                        foreach (var (studentId, date) in affectedStudentDates)
                        {
                            int totalAttendanceMin = 0;
                            if (allAttendanceForDaily != null && allAttendanceForDaily.TryGetValue((studentId, date), out var attendanceData))
                            {
                                foreach (var attendance in attendanceData)
                                {
                                    if (blockPeriodLookup != null && blockPeriodLookup.TryGetValue((attendance.BlockId, attendance.PeriodId), out var BlockPeriodData))
                                    {
                                        if (BlockPeriodData.CalculateAttendance != true) continue;

                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = attendanceCodeList?.FirstOrDefault(x => x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            if (AttendanceCodeData.StateCode!.ToLower() != "absent")
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                            }

                            if (existingDailyLookup != null && existingDailyLookup.TryGetValue((studentId, date), out var existingDaily))
                            {
                                existingDaily.AttendanceMinutes = totalAttendanceMin;
                            }
                            else
                            {
                                var dailyRecord = new StudentDailyAttendance
                                {
                                    TenantId = fillAttendanceViewModel.TenantId,
                                    SchoolId = fillAttendanceViewModel.SchoolId,
                                    StudentId = studentId,
                                    AttendanceDate = date,
                                    CreatedBy = fillAttendanceViewModel.CreatedBy,
                                    AttendanceMinutes = totalAttendanceMin,
                                    CreatedOn = DateTime.UtcNow
                                };
                                newDailyRecords.Add(dailyRecord);
                                // Add to lookup to avoid duplicates within the same batch
                                existingDailyLookup![(studentId, date)] = dailyRecord;
                            }
                        }

                        if (newDailyRecords.Any())
                        {
                            this.context?.StudentDailyAttendance.AddRange(newDailyRecords);
                        }
                        this.context?.SaveChanges();

                        // Clean up matching StudentMissingAttendance records
                        var missingRecords = this.context?.StudentMissingAttendances
                            .Where(m => m.TenantId == fillAttendanceViewModel.TenantId && m.SchoolId == fillAttendanceViewModel.SchoolId && m.CourseSectionId.HasValue && csIds.Contains(m.CourseSectionId.Value))
                            .ToList();

                        if (missingRecords?.Any() == true)
                        {
                            var filledCombos = new HashSet<(int CourseSectionId, DateTime Date, int PeriodId)>(
                                newAttendanceRecords.Select(r => (r.CourseSectionId, r.AttendanceDate, r.PeriodId)).Distinct()
                            );
                            var toRemove = missingRecords.Where(m => m.CourseSectionId.HasValue && m.MissingAttendanceDate.HasValue && m.PeriodId.HasValue && filledCombos.Contains((m.CourseSectionId.Value, m.MissingAttendanceDate.Value, m.PeriodId.Value))).ToList();
                            if (toRemove.Any())
                            {
                                this.context?.StudentMissingAttendances.RemoveRange(toRemove);
                                this.context?.SaveChanges();
                            }
                        }
                    }

                    transaction?.Commit();
                    fillAttendanceViewModel.TotalRecordsCreated = newAttendanceRecords.Count;
                    fillAttendanceViewModel.CourseSectionsProcessed = sectionsProcessed;
                    fillAttendanceViewModel._failure = false;
                    fillAttendanceViewModel._message = $"Successfully created {newAttendanceRecords.Count} attendance records across {sectionsProcessed} course sections";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    fillAttendanceViewModel._failure = true;
                    fillAttendanceViewModel._message = es.InnerException?.Message ?? es.Message;
                }
            }
            return fillAttendanceViewModel;
        }

        /// <summary>
        /// Update Student Daily Attendance
        /// </summary>
        /// <param name="studentDailyAttendanceListViewModel"></param>
        /// <returns></returns>
        public StudentDailyAttendanceListViewModel UpdateStudentDailyAttendance(StudentDailyAttendanceListViewModel studentDailyAttendanceListViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (studentDailyAttendanceListViewModel.studentDailyAttendanceList.Count > 0)
                    {
                        var StudentAttendanceAddList = new List<StudentAttendance>();
                        foreach (var studentDailyAttendance in studentDailyAttendanceListViewModel.studentDailyAttendanceList)
                        {
                            var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.StudentId == studentDailyAttendance.StudentId && x.AttendanceDate == studentDailyAttendanceListViewModel.AttendanceDate);

                            if (studentDailyAttendanceData != null)
                            {
                                studentDailyAttendanceData.AttendanceCode = studentDailyAttendance.AttendanceCode;
                                studentDailyAttendanceData.AttendanceComment = studentDailyAttendance.AttendanceComment;

                                if (studentDailyAttendance.AttendanceCode != null)
                                {
                                    var AttendanceCodeData = this.context?.AttendanceCode.AsEnumerable().FirstOrDefault(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && String.Compare(x.Title, studentDailyAttendance.AttendanceCode, true) == 0);
                                    if (AttendanceCodeData != null)
                                    {
                                        if (AttendanceCodeData.StateCode != "Absent")
                                        {
                                            var StudentAttendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.StudentId == studentDailyAttendance.StudentId && x.AttendanceDate == studentDailyAttendanceListViewModel.AttendanceDate).ToList();

                                            if (StudentAttendanceData?.Any() == true)
                                            {
                                                var blockData = this.context?.Block.FirstOrDefault(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.BlockId == StudentAttendanceData.FirstOrDefault()!.BlockId);
                                                StudentAttendanceData.ToList().ForEach(x => x.AttendanceCode = (int)AttendanceCodeData.AttendanceCode1);

                                                if (AttendanceCodeData.StateCode != "Present")
                                                {
                                                    //this block for half day
                                                    var halfDatMin = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(blockData?.FullDayMinutes) / 2));
                                                    studentDailyAttendanceData.AttendanceMinutes = halfDatMin;
                                                }
                                                else
                                                {
                                                    //this block for present
                                                    studentDailyAttendanceData.AttendanceMinutes = blockData?.FullDayMinutes;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //this block for absent
                                            var StudentAttendanceData = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).Where(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.StudentId == studentDailyAttendance.StudentId && x.AttendanceDate == studentDailyAttendanceListViewModel.AttendanceDate).ToList();

                                            if (StudentAttendanceData?.Any() == true)
                                            {
                                                StudentAttendanceData.ToList().ForEach(x => x.AttendanceCode = (int)AttendanceCodeData.AttendanceCode1);

                                                studentDailyAttendanceData.AttendanceMinutes = 0;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                studentDailyAttendanceListViewModel._failure = true;
                                studentDailyAttendanceListViewModel._message = NORECORDFOUND;
                            }
                        }
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        studentDailyAttendanceListViewModel._failure = false;
                        studentDailyAttendanceListViewModel._message = "Student daily attendance updated successfully";
                    }
                    else
                    {
                        studentDailyAttendanceListViewModel._failure = true;
                        studentDailyAttendanceListViewModel._message = "Please select student";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentDailyAttendanceListViewModel._failure = true;
                    studentDailyAttendanceListViewModel._message = es.Message;
                }
            }
            return studentDailyAttendanceListViewModel;
        }

        /// <summary>
        /// Add/Update Student Attendance Comments
        /// </summary>
        /// <param name="studentAttendanceCommentsAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceCommentsAddViewModel AddUpdateStudentAttendanceComments(StudentAttendanceCommentsAddViewModel studentAttendanceCommentsAddViewModel)
        {
            if (studentAttendanceCommentsAddViewModel.studentAttendanceComments is null)
            {
                return studentAttendanceCommentsAddViewModel;
            }
            try
            {
                if (studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId > 0)
                {
                    var studentAttendanceCommentUpdate = this.context?.StudentAttendanceComments.FirstOrDefault(x => x.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && x.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && x.CommentId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId && x.StudentId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.StudentId && x.StudentAttendanceId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.StudentAttendanceId);

                    if (studentAttendanceCommentUpdate != null)
                    {
                        int? membershipID = null;

                        //var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);

                        //if (staffSchoolInfoData != null)
                        //{
                        //    membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffSchoolInfoData.Profile??"").ToLower())?.MembershipId;
                        //    //membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && String.Compare(v.Profile, staffSchoolInfoData.Profile, true) == 0)?.MembershipId;
                        //}
                        var staffMasterData = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && /*c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId &&*/ c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);
                        if (staffMasterData != null)
                        {
                            if (staffMasterData.StaffSchoolInfo != null && staffMasterData.StaffSchoolInfo.Any())
                            {
                                var staffSchoolInfoData = staffMasterData.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);
                                if (staffSchoolInfoData != null)
                                {
                                    membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffSchoolInfoData.Profile ?? "").ToLower())?.MembershipId;
                                }
                            }
                            else
                            {
                                membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffMasterData.Profile ?? "").ToLower())?.MembershipId;
                            }
                        }

                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId = studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId != null ? studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId : membershipID;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.UpdatedOn = DateTime.UtcNow;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentTimestamp = DateTime.UtcNow;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedOn = studentAttendanceCommentUpdate.CreatedOn;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedBy = studentAttendanceCommentUpdate.CreatedBy;

                        this.context?.Entry(studentAttendanceCommentUpdate).CurrentValues.SetValues(studentAttendanceCommentsAddViewModel.studentAttendanceComments);
                        this.context?.SaveChanges();
                        studentAttendanceCommentsAddViewModel._failure = false;
                        studentAttendanceCommentsAddViewModel._message = "Student attendance comment updated successfully";
                    }
                    else
                    {
                        studentAttendanceCommentsAddViewModel._failure = true;
                        studentAttendanceCommentsAddViewModel._message = NORECORDFOUND;
                    }
                }
                else
                {
                    long? CommentId = Utility.GetMaxLongPK<StudentAttendanceComments>(this.context, x => x.CommentId);

                    int? membershipID = null;
                    //var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);

                    //if (staffSchoolInfoData != null)
                    //{
                    //    membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffSchoolInfoData.Profile??"").ToLower())?.MembershipId;
                    //    //membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && String.Compare(v.Profile, staffSchoolInfoData.Profile, true) == 0)?.MembershipId;
                    //}

                    var staffMasterData = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && /*c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId &&*/ c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);
                    if (staffMasterData != null)
                    {
                        if (staffMasterData.StaffSchoolInfo != null && staffMasterData.StaffSchoolInfo.Any())
                        {
                            var staffSchoolInfoData = staffMasterData.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && c.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && c.StaffId == studentAttendanceCommentsAddViewModel.StaffId);
                            if (staffSchoolInfoData != null)
                            {
                                membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffSchoolInfoData.Profile ?? "").ToLower())?.MembershipId;
                            }
                        }
                        else
                        {
                            membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.Profile.ToLower() == (staffMasterData.Profile ?? "").ToLower())?.MembershipId;
                        }
                    }

                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId = studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId != null ? studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId : membershipID;
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId = (long)CommentId!;
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedOn = DateTime.UtcNow;
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentTimestamp = DateTime.UtcNow;
                    this.context?.StudentAttendanceComments.Add(studentAttendanceCommentsAddViewModel.studentAttendanceComments);
                    this.context?.SaveChanges();
                    studentAttendanceCommentsAddViewModel._failure = false;
                    studentAttendanceCommentsAddViewModel._message = "Student attendance comment added successfully";
                }

                if (studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId != null)
                {
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.Membership = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && v.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && v.MembershipId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.MembershipId);
                }
            }
            catch (Exception es)
            {
                studentAttendanceCommentsAddViewModel._failure = true;
                studentAttendanceCommentsAddViewModel._message = es.Message;
            }
            return studentAttendanceCommentsAddViewModel;
        }

        /// <summary>
        /// Re-Calculate Daily Attendance
        /// </summary>
        /// <param name="reCalculateDailyAttendanceViewModel"></param>
        /// <returns></returns>
        public ReCalculateDailyAttendanceViewModel ReCalculateDailyAttendance_old(ReCalculateDailyAttendanceViewModel reCalculateDailyAttendanceViewModel)
        {
            try
            {
                var studentDailyAttendanceData = this.context?.StudentDailyAttendance.Where(x => x.TenantId == reCalculateDailyAttendanceViewModel.TenantId && x.SchoolId == reCalculateDailyAttendanceViewModel.SchoolId && x.AttendanceDate >= reCalculateDailyAttendanceViewModel.FromDate && x.AttendanceDate <= reCalculateDailyAttendanceViewModel.ToDate).ToList();

                if (studentDailyAttendanceData?.Any() == true)
                {
                    var AttendanceDateData = studentDailyAttendanceData.Select(s => s.AttendanceDate).Distinct().ToList();
                    foreach (var AttendanceDate in AttendanceDateData)
                    {
                        //all class in a day have only one block 
                        var AttendanceDataInaDay = this.context?.StudentAttendance.Where(x => x.TenantId == reCalculateDailyAttendanceViewModel.TenantId && x.SchoolId == reCalculateDailyAttendanceViewModel.SchoolId && x.AttendanceDate == AttendanceDate).ToList();
                        if (AttendanceDataInaDay?.Any() == true)
                        {
                            var studentId = AttendanceDataInaDay.Select(s => s.StudentId).Distinct().ToList();

                            foreach (var student in studentId)
                            {
                                int totalAttendanceMin = 0;
                                var StudentAttendanceData = AttendanceDataInaDay.Where(x => x.StudentId == student).ToList();

                                foreach (var StudentAttendance in StudentAttendanceData)
                                {
                                    var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == StudentAttendance.TenantId && x.SchoolId == StudentAttendance.SchoolId && x.BlockId == StudentAttendance.BlockId && x.PeriodId == StudentAttendance.PeriodId);

                                    if (BlockPeriodData != null)
                                    {
                                        // Only count periods flagged as calculating attendance
                                        if (BlockPeriodData.CalculateAttendance != true)
                                            continue;

                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == StudentAttendance.TenantId && x.SchoolId == StudentAttendance.SchoolId && x.AttendanceCode1 == StudentAttendance.AttendanceCode && x.AttendanceCategoryId == StudentAttendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            //if (AttendanceCodeData.Title.ToLower() != "absent")
                                            if (String.Compare(AttendanceCodeData.Title, "absent", true) == 0)
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                                var studentDailyAttendanceDataUpdate = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == reCalculateDailyAttendanceViewModel.TenantId && x.SchoolId == reCalculateDailyAttendanceViewModel.SchoolId && x.StudentId == student && x.AttendanceDate == AttendanceDate);

                                if (studentDailyAttendanceDataUpdate != null)
                                {
                                    studentDailyAttendanceDataUpdate.AttendanceMinutes = totalAttendanceMin;
                                }
                            }
                        }
                    }
                    this.context?.SaveChanges();
                    reCalculateDailyAttendanceViewModel._message = "The daily attendance between given timeframe has been recalculated";
                    reCalculateDailyAttendanceViewModel._failure = false;
                }
                else
                {
                    reCalculateDailyAttendanceViewModel._message = NORECORDFOUND;
                    reCalculateDailyAttendanceViewModel._failure = false;
                }
            }
            catch (Exception ex)
            {
                reCalculateDailyAttendanceViewModel._message = ex.Message;
                reCalculateDailyAttendanceViewModel._failure = true;
            }
            return reCalculateDailyAttendanceViewModel;
        }

        public ReCalculateDailyAttendanceViewModel ReCalculateDailyAttendance(ReCalculateDailyAttendanceViewModel reCalculateDailyAttendanceViewModel)
        {
            try
            {
                var tenantId = reCalculateDailyAttendanceViewModel.TenantId;
                var schoolId = reCalculateDailyAttendanceViewModel.SchoolId;
                var fromDate = reCalculateDailyAttendanceViewModel.FromDate;
                var toDate = reCalculateDailyAttendanceViewModel.ToDate;
                var academicYear = reCalculateDailyAttendanceViewModel._academicYear;

                var studentDailyAttendanceMasterData = this.context?.StudentDailyAttendance
                    .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId && x.AttendanceDate >= fromDate && x.AttendanceDate <= toDate)
                    .ToList();

                var studentAttendanceMasterData = this.context?.StudentAttendance
                    .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId && x.AttendanceDate >= fromDate && x.AttendanceDate <= toDate)
                    .ToList();

                if (studentAttendanceMasterData?.Any() != true)
                {
                    reCalculateDailyAttendanceViewModel._message = NORECORDFOUND;
                    reCalculateDailyAttendanceViewModel._failure = false;
                    return reCalculateDailyAttendanceViewModel;
                }

                // Load all master data up-front
                var blockPeriodLookup = this.context?.BlockPeriod
                    .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId)
                    .ToLookup(x => (x.BlockId, x.PeriodId));

                var attendanceCodeLookup = this.context?.AttendanceCode
                    .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId)
                    .ToDictionary(x => (x.AttendanceCode1, x.AttendanceCategoryId));

                var blockLookup = this.context?.Block
                    .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId)
                    .ToDictionary(x => x.BlockId);

                var attendanceDates = studentAttendanceMasterData.Select(s => s.AttendanceDate).Distinct();

                foreach (var attendanceDate in attendanceDates)
                {
                    var attendanceDataInADay = studentAttendanceMasterData.Where(x => x.AttendanceDate == attendanceDate).ToList();
                    var studentIds = attendanceDataInADay.Select(s => s.StudentId).Distinct();

                    foreach (var studentId in studentIds)
                    {
                        int totalAttendanceMin = 0;
                        string attendanceCode = string.Empty;

                        var studentAttendanceData = attendanceDataInADay.Where(x => x.StudentId == studentId).ToList();

                        foreach (var attendance in studentAttendanceData)
                        {
                            var blockPeriod = blockPeriodLookup?[(attendance.BlockId, attendance.PeriodId)].FirstOrDefault();

                            if (blockPeriod is null) continue;

                            TimeSpan start = TimeSpan.Parse(blockPeriod.PeriodStartTime!);
                            TimeSpan end = TimeSpan.Parse(blockPeriod.PeriodEndTime!);
                            int classMin = (int)(end - start).TotalMinutes;

                            if (attendanceCodeLookup != null && attendanceCodeLookup.TryGetValue((attendance.AttendanceCode, attendance.AttendanceCategoryId), out var code))
                            {
                                switch (code.StateCode?.ToLower())
                                {
                                    case "present":
                                        totalAttendanceMin += classMin;
                                        break;
                                    case "half day":
                                        totalAttendanceMin += (int)Math.Ceiling(classMin / 2.0);
                                        break;
                                }
                            }
                        }

                        var blockId = studentAttendanceData.FirstOrDefault()?.BlockId;
                        var block = blockId != null && blockLookup?.ContainsKey(blockId.Value) == true ? blockLookup[blockId.Value] : null;

                        if (block != null)
                        {
                            if (totalAttendanceMin >= block.FullDayMinutes)
                                attendanceCode = "Present";
                            else if (totalAttendanceMin >= block.HalfDayMinutes)
                                attendanceCode = "Half Day";
                            else
                                attendanceCode = "Absent";
                        }

                        var existingRecord = studentDailyAttendanceMasterData?.FirstOrDefault(x =>
                            x.TenantId == tenantId && x.SchoolId == schoolId &&
                            x.StudentId == studentId && x.AttendanceDate == attendanceDate);

                        if (existingRecord != null)
                        {
                            existingRecord.AttendanceMinutes = totalAttendanceMin;
                            existingRecord.AttendanceCode = attendanceCode;
                            existingRecord.UpdatedOn = DateTime.UtcNow;
                            existingRecord.UpdatedBy = reCalculateDailyAttendanceViewModel.UpdatedBy;
                        }
                        else
                        {
                            this.context?.StudentDailyAttendance.Add(new StudentDailyAttendance
                            {
                                TenantId = tenantId,
                                SchoolId = schoolId,
                                StudentId = studentId,
                                AttendanceDate = attendanceDate,
                                CreatedBy = reCalculateDailyAttendanceViewModel.UpdatedBy,
                                CreatedOn = DateTime.UtcNow,
                                AttendanceMinutes = totalAttendanceMin,
                                AttendanceCode = attendanceCode
                            });
                        }
                    }
                }

                this.context?.SaveChanges();

                reCalculateDailyAttendanceViewModel._message = "The daily attendance between given timeframe has been recalculated";
                reCalculateDailyAttendanceViewModel._failure = false;
            }
            catch (Exception ex)
            {
                reCalculateDailyAttendanceViewModel._message = ex.Message;
                reCalculateDailyAttendanceViewModel._failure = true;
            }
            return reCalculateDailyAttendanceViewModel;
        }

        /// <summary>
        /// Get Student Attendance History
        /// </summary>
        /// <param name="studentAttendanceHistoryViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceHistoryViewModel GetStudentAttendanceHistory(StudentAttendanceHistoryViewModel studentAttendanceHistoryViewModel)
        {
            StudentAttendanceHistoryViewModel studentAttendanceHistory = new StudentAttendanceHistoryViewModel();
            studentAttendanceHistory.TenantId = studentAttendanceHistoryViewModel.TenantId;
            studentAttendanceHistory._token = studentAttendanceHistoryViewModel._token;
            studentAttendanceHistory._tenantName = studentAttendanceHistoryViewModel._tenantName;
            studentAttendanceHistory.SchoolId = studentAttendanceHistoryViewModel.SchoolId;
            try
            {
                var StudentAttendanceHistoryData = this.context?.StudentAttendanceHistory.AsNoTracking().Where(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.SchoolId == studentAttendanceHistoryViewModel.SchoolId && x.StudentId == studentAttendanceHistoryViewModel.StudentId && x.CourseId == studentAttendanceHistoryViewModel.CourseId && x.CourseSectionId == studentAttendanceHistoryViewModel.CourseSectionId && x.BlockId == studentAttendanceHistoryViewModel.BlockId && x.PeriodId == studentAttendanceHistoryViewModel.PeriodId && x.AttendanceDate == studentAttendanceHistoryViewModel.AttendanceDate).OrderBy(x => x.ModificationTimestamp).ToList();

                if (StudentAttendanceHistoryData?.Any() == true)
                {
                    var UserIds = StudentAttendanceHistoryData.Select(x => x.ModifiedBy).Distinct().ToList();
                    foreach (var UserId in UserIds)

                    {
                        var AttendanceHistory = StudentAttendanceHistoryData.Where(x => x.ModifiedBy == UserId).OrderByDescending(x => x.ModificationTimestamp).FirstOrDefault();
                        if (AttendanceHistory != null)
                        {
                            AttendanceHistoryViewModel attendanceHistory = new AttendanceHistoryViewModel
                            {
                                TenantId = AttendanceHistory.TenantId,
                                SchoolId = AttendanceHistory.SchoolId,
                                StudentId = AttendanceHistory.StudentId,
                                AttendanceHistoryId = AttendanceHistory.AttendanceHistoryId,
                                CourseId = AttendanceHistory.CourseId,
                                CourseSectionId = AttendanceHistory.CourseSectionId,
                                AttendanceCategoryId = AttendanceHistory.AttendanceCategoryId,
                                AttendanceCode = AttendanceHistory.AttendanceCode,
                                AttendanceDate = AttendanceHistory.AttendanceDate,
                                BlockId = AttendanceHistory.BlockId,
                                PeriodId = AttendanceHistory.PeriodId,
                                MembershipId = AttendanceHistory.MembershipId,
                                ModifiedBy = AttendanceHistory.ModifiedBy,
                                ModificationTimestamp = AttendanceHistory.ModificationTimestamp,
                                ProfileType = AttendanceHistory.MembershipId == 1 ? "Super Administrator" : "Teacher",
                                AttendanceCodeTitle = this.context?.AttendanceCode.AsNoTracking().FirstOrDefault(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.SchoolId == studentAttendanceHistoryViewModel.SchoolId && x.AttendanceCategoryId == AttendanceHistory.AttendanceCategoryId && x.AttendanceCode1 == AttendanceHistory.AttendanceCode)?.Title
                            };

                            var StaffMasterData = this.context?.StaffMaster.AsNoTracking().FirstOrDefault(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.StaffId == UserId);

                            if (StaffMasterData != null)
                            {
                                attendanceHistory.UserName = StaffMasterData.FirstGivenName + " " + StaffMasterData.LastFamilyName;
                            }
                            studentAttendanceHistory.attendanceHistoryViewModels.Add(attendanceHistory);
                        }
                    }
                }
                else
                {
                    studentAttendanceHistory._failure = true;
                    studentAttendanceHistory._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentAttendanceHistory._failure = true;
                studentAttendanceHistory._message = es.Message;
            }
            return studentAttendanceHistory;
        }

        private bool CheckAttendanceDate(Guid TenantId, int SchoolId, int CourseSectionID, DateTime AttendanceDate)
        {
            bool IsVaild = false;

            var CourseSectionData = this.context?.CourseSection.AsNoTracking().Where(x => x.TenantId == TenantId && x.SchoolId == SchoolId && x.CourseSectionId == CourseSectionID && x.DurationStartDate <= AttendanceDate && x.DurationEndDate >= AttendanceDate).FirstOrDefault();

            if (CourseSectionData != null)
            {
                IsVaild = true;
            }

            return IsVaild;
        }
    }
}