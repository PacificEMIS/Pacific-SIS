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

                            var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                            if (studentAttendanceData != null)
                            {
                                StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                            }

                            int? membershipID = null;

                            var staffSchoolInfoData = this.context?.StaffSchoolInfo.Include(x => x.StaffMaster).FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId && c.SchoolId == studentAttendanceAddViewModel.SchoolId && c.StaffId == studentAttendanceAddViewModel.StaffId);

                            if (staffSchoolInfoData != null)
                            {
                                membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && (v.Profile ?? "").ToLower() == (staffSchoolInfoData.Profile ?? "").ToLower())?.MembershipId;
                                //membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && String.Compare(v.Profile, staffSchoolInfoData.Profile, true) == 0)?.MembershipId;
                            }

                            long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

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

                            long? HistoryCommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceHistory, long>(x => x.AttendanceHistoryId));

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

                            foreach (var studentId in studentIdList)
                            {
                                int totalAttendanceMin = 0;
                                var attendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate).ToList();
                                if (attendanceData != null)
                                {
                                    foreach (var attendance in attendanceData)
                                    {
                                        var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.BlockId == attendance.BlockId && x.PeriodId == attendance.PeriodId);

                                        if (BlockPeriodData != null)
                                        {
                                            var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                            var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                            TimeSpan? periodTime = periodEndTime - periodStartTime;
                                            var hour = Convert.ToInt32(periodTime.Value.Hours);
                                            var min = Convert.ToInt32(periodTime.Value.Minutes);
                                            var classMin = hour > 0 ? (hour * 60 + min) : min;

                                            var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
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
                                var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate);

                                if (studentDailyAttendanceData != null)
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
                var studentAttendanceData = this.context?.StudentAttendance.Include(k => k.Membership).Include(c => c.StudentCoursesectionSchedule).Include(v => v.StudentAttendanceComments).ThenInclude(y => y.Membership).Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId).ToList();

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

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(s => s.StaffMaster).Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData?.Any() == true)
                {

                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (scheduledCourseSection.CourseSection.AcademicYear == scheduledCourseSectionViewModel.AcademicYear)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            var courseSectionData = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId);

                            if (courseSectionData != null)
                            {
                                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == scheduledCourseSection.TenantId && e.CalendarId == courseSectionData.CalendarId && (e.StartDate >= courseSectionData.DurationStartDate && e.StartDate <= courseSectionData.DurationEndDate || e.EndDate >= courseSectionData.DurationStartDate && e.EndDate <= courseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == scheduledCourseSection.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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

                                var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId);
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

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (courseVariableScheduleData?.Any() == true)
                                {
                                    courseVariableScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseVariableSchedule = courseVariableScheduleData;
                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (courseCalenderScheduleData?.Any() == true)
                                {
                                    courseCalenderScheduleData.ForEach(x => { x.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });

                                    CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

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
                            membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && String.Compare(v.Profile, staffSchoolInfoData.Profile, true) == 0)?.MembershipId;
                        }
                        else
                        {
                            var staffMasterData = this.context?.StaffMaster.FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId /*&& c.SchoolId == studentAttendanceAddViewModel.SchoolId */&& c.StaffId == studentAttendanceAddViewModel.StaffId);
                            if (staffMasterData != null)
                            {
                                //membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && v.Profile.ToLower() == staffMasterData.Profile.ToLower())?.MembershipId;
                                membershipID = this.context?.Membership.AsEnumerable().FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && String.Compare(v.Profile, staffMasterData.Profile, true) == 0)?.MembershipId;
                            }
                        }

                        long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

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

                        long? HistoryCommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceHistory, long>(x => x.AttendanceHistoryId));

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
                        foreach (var date in attendanceDates)
                        {
                            int totalAttendanceMin = 0;
                            var attendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId && x.AttendanceDate == date).ToList();
                            if (attendanceData != null && attendanceData.Any())
                            {
                                foreach (var attendance in attendanceData)
                                {
                                    var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.BlockId == attendance.BlockId && x.PeriodId == attendance.PeriodId);

                                    if (BlockPeriodData != null)
                                    {
                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            //if (String.Compare(AttendanceCodeData.Title, "absent", true) == 0)
                                            if (AttendanceCodeData.StateCode!.ToLower() != "absent")
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                            }
                            var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId && x.AttendanceDate == date);

                            if (studentDailyAttendanceData != null)
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
                staffScheduleDataList = this.context?.StaffCoursesectionSchedule.Include(d => d.StaffMaster).Include(d => d.StudentAttendance).Include(b => b.CourseSection).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
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
                allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();
                if (allCourseSectionVewList is null)
                {
                    return staffListViewModel;
                }
                if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                {
                    staffScheduleDataList = staffScheduleDataList?.Where(e => ((pageResult.DobStartDate.Value.Date >= e.DurationStartDate.Value.Date && pageResult.DobStartDate.Value.Date <= e.DurationEndDate.Value.Date) || (pageResult.DobEndDate.Value.Date >= e.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= e.DurationEndDate)));

                    //Calculate Holiday
                    var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewList.FirstOrDefault()!.CalendarId && (e.StartDate >= pageResult.DobStartDate && e.StartDate <= pageResult.DobEndDate || e.EndDate >= pageResult.DobStartDate && e.EndDate <= pageResult.DobEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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

                                        var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= start && v.BellScheduleDate <= end && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                        if (bellScheduleList?.Any() == true)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {

                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == staffScheduleData.CourseSectionId && b.CourseId == staffScheduleData.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

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
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffScheduleData.StaffId*/ && b.AttendanceDate.Date == courseCalenderScheduleDate.CalDate && b.CourseSectionId == staffScheduleData.CourseSectionId && b.CourseId == staffScheduleData.CourseId && b.PeriodId == courseCalenderScheduleDate.CalPeriodId);

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

                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == staffScheduleData.CourseId && b.CourseSectionId == staffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId);

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
                                            var courseVariableScheduleData = allCourseSectionVewLists.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                            if (courseVariableScheduleData != null)
                                            {
                                                foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                {
                                                    CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == staffScheduleData.CourseId && b.CourseSectionId == staffScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

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
                staffScheduleDataList = this.context?.StaffCoursesectionSchedule.Include(d => d.StudentAttendance).Include(b => b.CourseSection).Include(d => d.StaffMaster).ThenInclude(a => a.StaffSchoolInfo).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
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

                allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

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
                        foreach (var staffScheduleData in staffScheduleDataList.ToList())
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

                                    var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId == staffScheduleData.CourseSectionId && x.MissingAttendanceDate >= start && x.MissingAttendanceDate <= end).ToList();

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
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && ((pageResult.DobStartDate!.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate.Value.Date) || (pageResult.DobEndDate!.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate)) && s.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
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

                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList!.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists.Count > 0)
                            {
                                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewLists.FirstOrDefault()!.CalendarId && (e.StartDate >= staffCourseSectionData.DurationStartDate && e.StartDate <= staffCourseSectionData.DurationEndDate || e.EndDate >= staffCourseSectionData.DurationStartDate && e.EndDate <= staffCourseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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
                                        var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == blockSchedule.BlockId && v.BellScheduleDate >= pageResult.DobStartDate && v.BellScheduleDate <= pageResult.DobEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                        if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                                        {
                                            bellScheduleList = bellScheduleList?.Where(s => pageResult.DobStartDate != null && s.BellScheduleDate >= pageResult.DobStartDate && s.BellScheduleDate <= pageResult.DobEndDate).ToList();
                                        }

                                        if (bellScheduleList?.Any() == true)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                if (StudentCoursesectionScheduleData?.Any() == true)
                                                {
                                                    CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == blockSchedule.BlockPeriodId).ToList();

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


                                                        courseSectionBlock.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == blockSchedule.BlockPeriodId)!.PeriodTitle : null;
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
                                        var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                        if (StudentCoursesectionScheduleData?.Any() == true)
                                        {
                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                            {
                                                CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId);

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


                                                    CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId)?.PeriodTitle : null;
                                                    CourseSectionFixed.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId)?.BlockId : null;
                                                    CourseSectionFixed.PeriodId = allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId;
                                                    CourseSectionFixed.AttendanceTaken = staffCourseSectionData.CourseSection.AttendanceTaken;

                                                    staffCoursesectionSchedule.Add(CourseSectionFixed);
                                                }
                                            }
                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                            {
                                                var courseVariableScheduleData = allCourseSectionVewLists.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                                if (courseVariableScheduleData != null)
                                                {
                                                    foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                    {
                                                        CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

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


                                                            CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId)?.PeriodTitle : null;
                                                            CourseSectionVariable.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == courseVariableSchedule.VarPeriodId)?.BlockId : null;
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
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section 

                                                if (StudentCoursesectionScheduleData?.Any() == true)
                                                {
                                                    CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

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


                                                        CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)?.PeriodTitle : null;
                                                        CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)?.BlockId : null;
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

                                transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) ||/* x.AttendanceDate != null && */x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
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
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && ((pageResult.DobStartDate!.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate!.Value.Date) || (pageResult.DobEndDate!.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate)) && s.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
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

                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList!.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists.Count > 0)
                            {
                                DateTime start = (DateTime)pageResult.DobStartDate!;
                                DateTime end = (DateTime)pageResult.DobEndDate!;

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.MissingAttendanceDate >= start && x.MissingAttendanceDate <= end).ToList();

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
                                            CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.PeriodTitle : null;
                                            CourseSectionFixed.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == studentMissingAttendance.PeriodId)?.BlockId : null;
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
                                            CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.PeriodTitle : null;
                                            CourseSectionVariable.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == studentMissingAttendance.PeriodId)?.BlockId : null;
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
                                            CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.PeriodTitle : null;
                                            CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.BlockId : null;
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
                                            courseSectionBlock.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.PeriodTitle : null;
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

                                transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) ||/* x.AttendanceDate != null && */x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
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
            studentAttendanceList._userName = pageResult._userName;
            IQueryable<StudendAttendanceAdministrationViewModel>? transactionIQ = null;
            List<StudendAttendanceAdministrationViewModel> attendanceData = new List<StudendAttendanceAdministrationViewModel>();
            try
            {
                var studentAttendanceData = this.context?.StudentAttendance.Include(s => s.StudentAttendanceComments).ThenInclude(s => s.Membership).Include(s => s.BlockPeriod).Include(s => s.AttendanceCodeNavigation).Include(s => s.StudentCoursesectionSchedule).ThenInclude(s => s.StudentMaster).ThenInclude(s => s.StudentEnrollment).Include(s => s.StudentCoursesectionSchedule.StudentMaster.Sections).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.AttendanceDate == pageResult.AttendanceDate && (pageResult.AttendanceCode == null || x.AttendanceCode == pageResult.AttendanceCode)).ToList();
                if (studentAttendanceData != null && studentAttendanceData.Any())
                {
                    var studentIds = studentAttendanceData.Select(a => a.StudentId).Distinct().ToList();
                    var blockId = studentAttendanceData.FirstOrDefault()!.BlockId;
                    foreach (var ide in studentIds)
                    {
                        StudendAttendanceAdministrationViewModel administrationViewModel = new StudendAttendanceAdministrationViewModel();

                        var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.StudentId == ide && x.AttendanceDate == pageResult.AttendanceDate);

                        if (studentDailyAttendanceData != null)
                        {
                            var blockData = this.context?.Block.FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.BlockId == blockId);
                            if (studentDailyAttendanceData.AttendanceMinutes >= blockData?.FullDayMinutes)
                            {
                                administrationViewModel.Present = "Full-Day";
                            }
                            if (studentDailyAttendanceData.AttendanceMinutes >= blockData?.HalfDayMinutes && studentDailyAttendanceData.AttendanceMinutes < blockData?.FullDayMinutes)
                            {
                                administrationViewModel.Present = "Half-Day";
                            }
                            if (studentDailyAttendanceData.AttendanceMinutes < blockData?.HalfDayMinutes)
                            {
                                administrationViewModel.Present = "Absent";
                            }
                            administrationViewModel.AttendanceComment = studentDailyAttendanceData.AttendanceComment;
                        }

                        var studentAttendance = studentAttendanceData.Where(x => x.StudentId == ide);

                        var attendance = studentAttendance.FirstOrDefault();
                        if (attendance != null)
                        {
                            administrationViewModel.TenantId = attendance.TenantId;
                            administrationViewModel.SchoolId = attendance.SchoolId;
                            administrationViewModel.StudentId = attendance.StudentId;
                            administrationViewModel.StudentInternalId = attendance.StudentCoursesectionSchedule.StudentMaster.StudentInternalId;
                            administrationViewModel.StudentGuid = attendance.StudentCoursesectionSchedule.StudentMaster.StudentGuid;
                            administrationViewModel.FirstGivenName = attendance.StudentCoursesectionSchedule.StudentMaster.FirstGivenName;
                            administrationViewModel.MiddleName = attendance.StudentCoursesectionSchedule.StudentMaster.MiddleName;
                            administrationViewModel.LastFamilyName = attendance.StudentCoursesectionSchedule.StudentMaster.LastFamilyName;
                            administrationViewModel.GradeLevelTitle = attendance.StudentCoursesectionSchedule.StudentMaster.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeLevelTitle).FirstOrDefault();
                            administrationViewModel.Section = attendance.StudentCoursesectionSchedule.StudentMaster.Sections?.Name;
                            administrationViewModel.GradeId = attendance.StudentCoursesectionSchedule.StudentMaster.StudentEnrollment.Where(x => x.IsActive == true).Select(s => s.GradeId).FirstOrDefault();
                            administrationViewModel.SectionId = attendance.StudentCoursesectionSchedule.StudentMaster.SectionId;
                        }

                        studentAttendance.ToList().ForEach(x => { x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.AttendanceCodeNavigation.StudentAttendance = new HashSet<StudentAttendance>(); x.StudentCoursesectionSchedule.StudentMaster = new(); x.Membership = null; x.StudentAttendanceComments.ToList().ForEach(c => { c.Membership!.StudentAttendanceComments = new HashSet<StudentAttendanceComments>(); c.Membership.StudentAttendance = new HashSet<StudentAttendance>(); }); });
                        administrationViewModel.studentAttendanceList = studentAttendance.ToList();

                        attendanceData.Add(administrationViewModel);
                    }

                    if (attendanceData.Count() > 0)
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
                var CourseSectionData = this.context?.CourseSection.Include(x => x.StudentCoursesectionSchedule).Include(x => x.Course).Include(x => x.SchoolCalendars).Include(x => x.StaffCoursesectionSchedule).Where(x => x.TenantId == courseSectionForAttendanceViewModel.TenantId && x.SchoolId == courseSectionForAttendanceViewModel.SchoolId && x.AcademicYear == courseSectionForAttendanceViewModel.AcademicYear).ToList();

                if (CourseSectionData?.Any() == true)
                {
                    CourseSectionData = CourseSectionData.Where(x => x.StaffCoursesectionSchedule.Count > 0).ToList();
                    foreach (var courseSection in CourseSectionData)
                    {
                        var studentExistInCS = courseSection.StudentCoursesectionSchedule.Where(x => x.IsDropped != true).ToList();

                        if (studentExistInCS.Count > 0)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == courseSectionForAttendanceViewModel.TenantId && e.CalendarId == courseSection.CalendarId && (e.StartDate >= courseSection.DurationStartDate && e.StartDate <= courseSection.DurationEndDate || e.EndDate >= courseSection.DurationStartDate && e.EndDate <= courseSection.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == courseSectionForAttendanceViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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

                                    var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId);
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

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

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

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

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

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

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

                                    //for bellSchedule list return.
                                    var bellScheduleList = new List<BellSchedule>();
                                    foreach (var block in courseBlockScheduleData)
                                    {
                                        var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == courseSection.SchoolId && c.TenantId == courseSection.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= courseSection.DurationStartDate && c.BellScheduleDate <= courseSection.DurationEndDate).ToList();
                                        if (bellScheduleData?.Any() == true)
                                        {
                                            bellScheduleList.AddRange(bellScheduleData);
                                        }
                                    }

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

                        var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                        if (studentAttendanceData != null)
                        {
                            StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                        }

                        long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

                        var allCsData = this.context?.AllCourseSectionView.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId).ToList();

                        var staffId = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.IsDropped != true)?.StaffId;
                        if (allCsData?.Any() == true)
                        {
                            foreach (var studentAttendance in studentAttendanceAddViewModel.studentAttendance)
                            {
                                int? blockIde = this.context?.Block.Where(b=>b.TenantId == studentAttendanceAddViewModel.TenantId && b.SchoolId == studentAttendanceAddViewModel.SchoolId && b.AcademicYear == Utility.GetCurrentAcademicYear(this.context!, studentAttendanceAddViewModel.TenantId, studentAttendanceAddViewModel.SchoolId)).FirstOrDefault()!.BlockId;
                               /* blockPeriodAddViewModel.blockPeriod.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, blockPeriodAddViewModel.blockPeriod.TenantId, blockPeriodAddViewModel.blockPeriod.SchoolId);*/
                                int? periodIde = null;
                                if (allCsData.FirstOrDefault()!.ScheduleType == "Fixed Schedule (1)")
                                {
                                    periodIde = allCsData.FirstOrDefault()!.FixedPeriodId;

                                    var attendanceDataForStudent = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.MembershipId == 1);

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
                                            var attendanceDataForStudent = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.MembershipId == 1 && x.PeriodId == periodId.VarPeriodId);

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
                                            var attendanceDataForStudent = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.MembershipId == 1 && x.PeriodId == periodId.CalPeriodId);

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
                                    var BellScheduleData = this.context?.BellSchedule.FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.BellScheduleDate == studentAttendance.AttendanceDate);
                                    if (BellScheduleData != null)
                                    {
                                        blockIde = BellScheduleData.BlockId;
                                        periodIde = allCsData.Where(x => x.BlockId == BellScheduleData.BlockId).Select(s => s.BlockPeriodId).FirstOrDefault();
                                    }

                                    var attendanceDataForStudent = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendance.AttendanceDate && x.StudentId == studentAttendance.StudentId && x.MembershipId == 1);

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

                        //Insert into daily attendance table
                        foreach (var studentId in studentAttendanceAddViewModel.studentAttendance)
                        {
                            int totalAttendanceMin = 0;
                            var attendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentId.StudentId && x.AttendanceDate == studentId.AttendanceDate).ToList();
                            if (attendanceData != null)
                            {
                                foreach (var attendance in attendanceData)
                                {
                                    var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.BlockId == attendance.BlockId && x.PeriodId == attendance.PeriodId);

                                    if (BlockPeriodData != null)
                                    {
                                        var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime!);
                                        var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime!);
                                        TimeSpan? periodTime = periodEndTime - periodStartTime;
                                        var hour = Convert.ToInt32(periodTime.Value.Hours);
                                        var min = Convert.ToInt32(periodTime.Value.Minutes);
                                        var classMin = hour > 0 ? (hour * 60 + min) : min;

                                        var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                        if (AttendanceCodeData != null)
                                        {
                                            if (AttendanceCodeData.Title!.ToLower() != "absent".ToLower())
                                            {
                                                totalAttendanceMin = totalAttendanceMin + classMin;
                                            }
                                        }
                                    }
                                }
                            }
                            var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentId.StudentId && x.AttendanceDate == studentId.AttendanceDate);

                            if (studentDailyAttendanceData != null)
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
                        studentAttendanceAddViewModel._message = "Add absences added successfully";
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
                    studentAttendanceAddViewModel._message = es.Message;
                }
            }
            return studentAttendanceAddViewModel;
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
                    long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

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
        public ReCalculateDailyAttendanceViewModel ReCalculateDailyAttendance(ReCalculateDailyAttendanceViewModel reCalculateDailyAttendanceViewModel)
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
                var StudentAttendanceHistoryData = this.context?.StudentAttendanceHistory.Where(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.SchoolId == studentAttendanceHistoryViewModel.SchoolId && x.StudentId == studentAttendanceHistoryViewModel.StudentId && x.CourseId == studentAttendanceHistoryViewModel.CourseId && x.CourseSectionId == studentAttendanceHistoryViewModel.CourseSectionId && x.BlockId == studentAttendanceHistoryViewModel.BlockId && x.PeriodId == studentAttendanceHistoryViewModel.PeriodId && x.AttendanceDate == studentAttendanceHistoryViewModel.AttendanceDate).OrderBy(x => x.ModificationTimestamp).ToList();

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
                                AttendanceCodeTitle = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.SchoolId == studentAttendanceHistoryViewModel.SchoolId && x.AttendanceCategoryId == AttendanceHistory.AttendanceCategoryId && x.AttendanceCode1 == AttendanceHistory.AttendanceCode)?.Title
                            };

                            var StaffMasterData = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == studentAttendanceHistoryViewModel.TenantId && x.StaffId == UserId);

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

            var CourseSectionData = this.context?.CourseSection.Where(x => x.TenantId == TenantId && x.SchoolId == SchoolId && x.CourseSectionId == CourseSectionID && x.DurationStartDate <= AttendanceDate && x.DurationEndDate >= AttendanceDate).FirstOrDefault();

            if (CourseSectionData != null)
            {
                IsVaild = true;
            }

            return IsVaild;
        }
    }
}