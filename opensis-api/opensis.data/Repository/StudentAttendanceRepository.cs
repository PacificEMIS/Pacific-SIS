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
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using opensis.data.ViewModels.StudentAttendances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StudentAttendanceRepository : IStudentAttendanceRepository
    {
        private CRMContext context;
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
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    List<StudentAttendance> studentAttendance = new List<StudentAttendance>();
                    List<StudentDailyAttendance> studentDailyAttendances = new List<StudentDailyAttendance>();
                    

                    if (studentAttendanceAddViewModel.studentAttendance.Count > 0)
                    {
                        var attendanceDataExist = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId).ToList();

                        int? StudentAttendanceId = 1;

                        var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                        if (studentAttendanceData != null)
                        {
                            StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                        }

                        int? membershipID=null;
                        var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId && c.SchoolId == studentAttendanceAddViewModel.SchoolId && c.StaffId == studentAttendanceAddViewModel.StaffId);

                        if (staffSchoolInfoData != null)
                        {
                            membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && v.Profile.ToLower() == staffSchoolInfoData.Profile.ToLower())?.MembershipId;
                        }

                        long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

                        foreach (var studentAttendances in studentAttendanceAddViewModel.studentAttendance)
                        {
                            if (studentAttendances.StudentAttendanceComments.Count() > 0)
                            {
                                foreach (var StudentAttendanceComment in studentAttendances.StudentAttendanceComments)
                                {
                                    StudentAttendanceComment.CommentId = (long)CommentId;
                                    CommentId++;
                                }
                            }
                        }

                        if (attendanceDataExist.Count > 0)
                        {
                            //this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                            var studentAttendanceIDs = attendanceDataExist.Select(v => v.StudentAttendanceId).ToList();

                            var studentAttendanceCommentData = this.context?.StudentAttendanceComments.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && (studentAttendanceIDs == null || (studentAttendanceIDs.Contains(x.StudentAttendanceId))));

                            if (studentAttendanceCommentData.Count() > 0)
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
                                    MembershipId= membershipID,
                                    StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                     {
                                         c.UpdatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                         c.UpdatedOn = DateTime.UtcNow;
                                         c.CommentTimestamp= DateTime.UtcNow;
                                         c.MembershipId = membershipID;
                                         return c;
                                     }).ToList()
                                };
                                studentAttendance.Add(studentAttendanceUpdate);
                                StudentAttendanceId++;
                            }
                            studentAttendanceAddViewModel._message = "Student Attendance Updated Succsesfully.";
                        }
                        else
                        {
                            foreach (var studentAttendancedata in studentAttendanceAddViewModel.studentAttendance.ToList())
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
                                    MembershipId= membershipID,
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
                                StudentAttendanceId++;
                            }
                            studentAttendanceAddViewModel._message = "Student Attendance Added Succsesfully.";
                        }
                        this.context?.StudentAttendance.AddRange(studentAttendance);
                        this.context?.SaveChanges();

                        var studentIdList = studentAttendanceAddViewModel.studentAttendance.Select(x => x.StudentId).ToList();

                        foreach (var studentId in studentIdList)
                        {
                            int totalAttendanceMin = 0;
                            var attendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate).ToList();

                            foreach (var attendance in attendanceData)
                            {
                                var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.BlockId == attendance.BlockId && x.PeriodId == attendance.PeriodId);

                                if (BlockPeriodData != null)
                                {
                                    var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime);
                                    var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime);
                                    TimeSpan? periodTime = periodEndTime - periodStartTime;
                                    var hour = Convert.ToInt32(periodTime.Value.Hours);
                                    var min = Convert.ToInt32(periodTime.Value.Minutes);
                                    var classMin = hour > 0 ? (hour * 60 + min) : min;

                                    var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                    if (AttendanceCodeData != null)
                                    {
                                        if (AttendanceCodeData.Title.ToLower() != "absent")
                                        {
                                            totalAttendanceMin = totalAttendanceMin + classMin;
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
                        transaction.Commit();
                        studentAttendanceAddViewModel._failure = false;
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
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
                var studentAttendanceData = this.context?.StudentAttendance.Include(k=>k.Membership).Include(c => c.StudentCoursesectionSchedule).Include(v => v.StudentAttendanceComments).ThenInclude(y => y.Membership).Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId).ToList();    

                if (studentAttendanceData.Count > 0)
                {
                    studentAttendanceAddViewModel.studentAttendance = studentAttendanceData.Select(e => new StudentAttendance()
                    {
                        TenantId = e.TenantId,
                        SchoolId = e.SchoolId,
                        StudentId = e.StudentId,
                        StaffId = e.StaffId,
                        CourseId = e.CourseId,
                        CourseSectionId = e.CourseSectionId,
                        AttendanceCategoryId = e.AttendanceCategoryId,
                        AttendanceDate = e.AttendanceDate,
                        AttendanceCode = e.AttendanceCode,
                        BlockId = e.BlockId,
                        PeriodId = e.PeriodId,
                        StudentAttendanceId = e.StudentAttendanceId,
                        MembershipId = e.MembershipId,
                        CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                        UpdatedOn = e.UpdatedOn,
                        Membership = ( e.Membership!=null) ? new Membership()
                        {
                            TenantId = e.Membership.TenantId,
                            SchoolId = e.Membership.SchoolId,
                            MembershipId = e.Membership.MembershipId,
                            Profile = e.Membership.Profile,
                            IsActive = e.Membership.IsActive,
                            IsSuperadmin = e.Membership.IsSuperadmin,
                            IsSystem = e.Membership.IsSystem,
                            Description = e.Membership.Description,
                            ProfileType = e.Membership.ProfileType,
                            CreatedBy = (e.Membership.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.Membership.CreatedBy).Name : null,
                            CreatedOn = e.Membership.CreatedOn,
                            UpdatedBy = (e.Membership.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.Membership.UpdatedBy).Name : null,
                            UpdatedOn = e.Membership.UpdatedOn
                        } : null,
                        StudentCoursesectionSchedule = new StudentCoursesectionSchedule()
                        {
                            TenantId = e.StudentCoursesectionSchedule.TenantId,
                            SchoolId = e.StudentCoursesectionSchedule.SchoolId,
                            StudentId = e.StudentCoursesectionSchedule.StudentId,
                            CourseId = e.StudentCoursesectionSchedule.CourseId,
                            CourseSectionId = e.StudentCoursesectionSchedule.CourseSectionId,
                            StudentGuid = e.StudentCoursesectionSchedule.StudentGuid,
                            AlternateId = e.StudentCoursesectionSchedule.AlternateId,
                            StudentInternalId = e.StudentCoursesectionSchedule.StudentInternalId,
                            FirstGivenName = e.StudentCoursesectionSchedule.FirstGivenName,
                            MiddleName = e.StudentCoursesectionSchedule.MiddleName,
                            LastFamilyName = e.StudentCoursesectionSchedule.LastFamilyName,
                            FirstLanguageId = e.StudentCoursesectionSchedule.FirstLanguageId,
                            GradeId = e.StudentCoursesectionSchedule.GradeId,
                            AcademicYear = e.StudentCoursesectionSchedule.AcademicYear,
                            GradeScaleId = e.StudentCoursesectionSchedule.GradeScaleId,
                            CourseSectionName = e.StudentCoursesectionSchedule.CourseSectionName,
                            CalendarId = e.StudentCoursesectionSchedule.CalendarId,
                            EffectiveDropDate = e.StudentCoursesectionSchedule.EffectiveDropDate,
                            IsDropped = e.StudentCoursesectionSchedule.IsDropped,
                            CreatedBy = (e.StudentCoursesectionSchedule.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.StudentCoursesectionSchedule.CreatedBy).Name : null,
                            CreatedOn = e.StudentCoursesectionSchedule.CreatedOn,
                            UpdatedBy = (e.StudentCoursesectionSchedule.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == e.StudentCoursesectionSchedule.UpdatedBy).Name : null,
                            UpdatedOn = e.StudentCoursesectionSchedule.UpdatedOn
                        },
                        StudentAttendanceComments = e.StudentAttendanceComments.ToList().Count > 0 ? e.StudentAttendanceComments.Select(n => new StudentAttendanceComments()
                        {
                            TenantId = n.TenantId,
                            SchoolId = n.SchoolId,
                            StudentId = n.StudentId,
                            StudentAttendanceId = n.StudentAttendanceId,
                            CommentId = n.CommentId,
                            Comment = n.Comment,
                            CommentTimestamp = n.CommentTimestamp,
                            CreatedBy = (n.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == n.CreatedBy).Name : null,
                            CreatedOn = n.CreatedOn,
                            UpdatedBy = (n.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == n.UpdatedBy).Name : null,
                            UpdatedOn = n.UpdatedOn,
                            MembershipId = n.MembershipId,
                            Membership = (n.Membership != null) ? new Membership()
                            {
                                TenantId = n.Membership.TenantId,
                                SchoolId = n.Membership.SchoolId,
                                MembershipId = n.Membership.MembershipId,
                                Profile = n.Membership.Profile,
                                IsActive = n.Membership.IsActive,
                                IsSuperadmin = n.Membership.IsSuperadmin,
                                IsSystem = n.Membership.IsSystem,
                                Description = n.Membership.Description,
                                ProfileType = n.Membership.ProfileType,
                                CreatedBy = (n.Membership.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == n.Membership.CreatedBy).Name : null,
                                CreatedOn = n.Membership.CreatedOn,
                                UpdatedBy = (n.Membership.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == studentAttendanceAddViewModel.TenantId && u.EmailAddress == n.Membership.UpdatedBy).Name : null,
                                UpdatedOn = n.Membership.UpdatedOn
                            } : null
                        }).ToList() : null
                    }).ToList();
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

                if (scheduledCourseSectionData.Count() > 0)
                {

                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        CourseSectionViewList CourseSections = new CourseSectionViewList();

                        if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                        {
                            CourseSections.ScheduleType = "Fixed Schedule";

                            var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId);
                            if (courseFixedScheduleData != null)
                            {
                                courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = null;
                                CourseSections.courseFixedSchedule = courseFixedScheduleData;

                            }
                        }
                        if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            CourseSections.ScheduleType = "Variable Schedule";

                            var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                            if (courseVariableScheduleData.Count > 0)
                            {
                                courseVariableScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                CourseSections.courseVariableSchedule = courseVariableScheduleData;
                            }
                        }
                        if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                        {
                            CourseSections.ScheduleType = "Calendar Schedule";

                            var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                            if (courseCalenderScheduleData.Count > 0)
                            {
                                courseCalenderScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                            }
                        }
                        if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            CourseSections.ScheduleType = "Block Schedule";

                            var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == scheduledCourseSection.TenantId && x.SchoolId == scheduledCourseSection.SchoolId && x.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                            if (courseBlockScheduleData.Count > 0)
                            {
                                courseBlockScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                CourseSections.courseBlockSchedule = courseBlockScheduleData;
                            }
                        }

                        CourseSections.CourseId = scheduledCourseSection.CourseId;
                        CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                        CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                        CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                        CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                        CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                        CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                        CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                        CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                        CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                        CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;

                        scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
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
                scheduledCourseSectionView.courseSectionViewList = null;
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
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    List<StudentAttendance> studentAttendance = new List<StudentAttendance>();
                    List<StudentDailyAttendance> studentDailyAttendances = new List<StudentDailyAttendance>();

                    if (studentAttendanceAddViewModel.studentAttendance.Count > 0)
                    {
                        var courseSectionIds = studentAttendanceAddViewModel.studentAttendance.Select(v => v.CourseSectionId).ToList();
                        var attendanceDates = studentAttendanceAddViewModel.studentAttendance.Select(v => v.AttendanceDate).ToList();
                        var periodIds = studentAttendanceAddViewModel.studentAttendance.Select(v => v.PeriodId).ToList();

                        var attendanceDataExist = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId /*&& x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId*/ /*&& x.AttendanceDate == studentAttendanceAddViewModel.AttendanceDate && x.PeriodId == studentAttendanceAddViewModel.PeriodId*/ && (courseSectionIds == null || (courseSectionIds.Contains(x.CourseSectionId))) && (attendanceDates == null || (attendanceDates.Contains(x.AttendanceDate))) && (periodIds == null || (periodIds.Contains(x.PeriodId)))).ToList();

                        int? StudentAttendanceId = 1;

                        var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                        if (studentAttendanceData != null)
                        {
                            StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                        }

                        int? membershipID = null;
                        var staffSchoolInfoData = this.context?.StaffSchoolInfo.FirstOrDefault(c => c.TenantId == studentAttendanceAddViewModel.TenantId && c.SchoolId == studentAttendanceAddViewModel.SchoolId && c.StaffId == studentAttendanceAddViewModel.StaffId);

                        if (staffSchoolInfoData != null)
                        {
                            membershipID = this.context?.Membership.FirstOrDefault(v => v.TenantId == studentAttendanceAddViewModel.TenantId && v.SchoolId == studentAttendanceAddViewModel.SchoolId && v.Profile.ToLower() == staffSchoolInfoData.Profile.ToLower())?.MembershipId;
                        }

                        long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

                        foreach (var studentAttendances in studentAttendanceAddViewModel.studentAttendance)
                        {
                            if (studentAttendances.StudentAttendanceComments.Count() > 0)
                            {
                                foreach (var StudentAttendanceComment in studentAttendances.StudentAttendanceComments)
                                {
                                    StudentAttendanceComment.CommentId = (long)CommentId;
                                    CommentId++;
                                }
                            }
                        }

                        if (attendanceDataExist.Count > 0)
                        {
                            //this.context?.StudentAttendance.RemoveRange(attendanceDataExist);
                            var studentAttendanceIDs = attendanceDataExist.Select(v => v.StudentAttendanceId).ToList();

                            var studentAttendanceCommentData = this.context?.StudentAttendanceComments.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && (studentAttendanceIDs == null || (studentAttendanceIDs.Contains(x.StudentAttendanceId))));

                            if (studentAttendanceCommentData.Count() > 0)
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
                                    MembershipId= membershipID,
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
                                StudentAttendanceId++;
                            }
                            studentAttendanceAddViewModel._message = "Student Attendance updated succsesfully.";
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
                                    MembershipId = membershipID,
                                    StudentAttendanceComments = studentAttendancedata.StudentAttendanceComments.Select(c =>
                                    {
                                        c.CreatedBy = studentAttendanceAddViewModel.UpdatedBy;
                                        c.CreatedOn = DateTime.UtcNow;
                                        c.CommentTimestamp = DateTime.UtcNow;
                                        c.MembershipId = membershipID;
                                        return c;
                                    }).ToList()
                                };
                                studentAttendance.Add(studentAttendanceAdd);
                                StudentAttendanceId++;
                            }
                            studentAttendanceAddViewModel._message = "Student Attendance added succsesfully.";
                        }
                        this.context?.StudentAttendance.AddRange(studentAttendance);
                        this.context?.SaveChanges();

                        attendanceDates = attendanceDates.Distinct().ToList();
                        foreach (var date in attendanceDates)
                        {
                            int totalAttendanceMin = 0;
                            var attendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentAttendanceAddViewModel.TenantId && x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.StudentId == studentAttendanceAddViewModel.StudentId && x.AttendanceDate == date).ToList();
                            foreach (var attendance in attendanceData)
                            {
                                var BlockPeriodData = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.BlockId == attendance.BlockId && x.PeriodId == attendance.PeriodId);

                                if (BlockPeriodData != null)
                                {
                                    var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime);
                                    var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime);
                                    TimeSpan? periodTime = periodEndTime - periodStartTime;
                                    var hour = Convert.ToInt32(periodTime.Value.Hours);
                                    var min = Convert.ToInt32(periodTime.Value.Minutes);
                                    var classMin = hour > 0 ? (hour * 60 + min) : min;

                                    var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendance.TenantId && x.SchoolId == attendance.SchoolId && x.AttendanceCode1 == attendance.AttendanceCode && x.AttendanceCategoryId == attendance.AttendanceCategoryId);
                                    if (AttendanceCodeData != null)
                                    {
                                        if (AttendanceCodeData.Title.ToLower() != "absent")
                                        {
                                            totalAttendanceMin = totalAttendanceMin + classMin;
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
                        this.context.SaveChanges();
                        transaction.Commit();
                        studentAttendanceAddViewModel._failure = false;
                        studentAttendanceAddViewModel.studentAttendance.ToList().ForEach(x => x.StudentAttendanceComments.ToList().ForEach(x => x.StudentAttendance = null));
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
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
        public StaffListModel StaffListForMissingAttendance(PageResult pageResult)
        {
            StaffListModel staffListViewModel = new StaffListModel();
            IQueryable<StaffMaster> transactionIQ = null;
            List<StaffMaster> staffCoursesectionSchedule = new List<StaffMaster>();
            IQueryable<StaffCoursesectionSchedule> staffScheduleDataList = null;
            List<AllCourseSectionView> allCourseSectionVewList = new List<AllCourseSectionView>();
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

                if (pageResult.DobStartDate.HasValue && pageResult.DobEndDate.HasValue)
                {
                    staffScheduleDataList = staffScheduleDataList.Where(e => ((pageResult.DobStartDate.Value.Date >= e.DurationStartDate.Value.Date && pageResult.DobStartDate.Value.Date <= e.DurationEndDate.Value.Date) || (pageResult.DobEndDate.Value.Date >= e.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= e.DurationEndDate)));
                }

                List<int> ID = new List<int>();
                List<DateTime> missingAttendanceDatelist = new List<DateTime>();

                foreach (var staffScheduleData in staffScheduleDataList.ToList())
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
                            start = (DateTime)staffScheduleData.DurationStartDate;
                            end = (DateTime)staffScheduleData.DurationEndDate;
                        }
                        if (staffScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            foreach (var allCourseSectionVew in allCourseSectionVewList)
                            {

                                var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= start && v.BellScheduleDate <= end && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                if (bellScheduleList.Count > 0)
                                {
                                    foreach (var bellSchedule in bellScheduleList)
                                    {

                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == staffScheduleData.CourseSectionId && b.CourseId == staffScheduleData.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

                                        if (staffAttendanceData.Count() == 0)
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

                                    if (staffAttendanceData.Count() == 0)
                                    {
                                        missingAttendanceDatelist.Add((DateTime)courseCalenderScheduleDate.CalDate);

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
                            meetingDays = staffScheduleData.MeetingDays.ToLower().Split("|");

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
                                                  .Where(d => allDays || meetingDays.Contains(d.DayOfWeek.ToString().ToLower()))
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

                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffScheduleData.SchoolId && b.TenantId == staffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == staffScheduleData.CourseId && b.CourseSectionId == staffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault().FixedPeriodId);

                                    if (staffAttendanceData.Count() == 0)
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

                                            if (staffAttendanceData.Count() == 0)
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
                var staffList = staffScheduleDataList.Select(b => b.StaffMaster).Where(x => (ID == null || (ID.Contains(x.StaffId)))).ToList();
                
                if (staffList.Count > 0)
                {
                    staffCoursesectionSchedule.AddRange(staffList);
                }
                staffCoursesectionSchedule = staffCoursesectionSchedule.GroupBy(c => c.StaffId).Select(c => c.FirstOrDefault()).ToList();
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
                }
                //transactionIQ = transactionIQ.Distinct();

                if (pageResult.SortingModel != null)
                {
                    transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                }

                int totalCount = transactionIQ.Count();

                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ.Select(p => new StaffMaster
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

                staffListViewModel.staffMaster = transactionIQ.ToList();
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
        public ScheduledCourseSectionViewModel MissingAttendanceList(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            IQueryable<CourseSectionViewList> transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();
            //CourseFixedSchedule courseFixedSchedule = null;
            //List<CourseVariableSchedule> CourseVariableSchedule = new List<CourseVariableSchedule>();
            //List<CourseCalendarSchedule> courseCalendarSchedule = new List<CourseCalendarSchedule>();
            //List<CourseBlockSchedule> CourseBlockSchedule = new List<CourseBlockSchedule>();
            List<AllCourseSectionView> allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod> BlockPeriodList = new List<BlockPeriod>();

            try
            {
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && ((pageResult.DobStartDate.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate.Value.Date) || (pageResult.DobEndDate.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate)) && s.IsDropped != true).Select(v => new StaffCoursesectionSchedule()
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

                if (staffCourseSectionDataList.Count() > 0)
                {

                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {

                        var allCourseSectionVewLists = allCourseSectionVewList.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                        if (allCourseSectionVewLists.Count > 0)
                        {
                            if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                var blockScheduleData = allCourseSectionVewList.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();
                                foreach(var blockSchedule in blockScheduleData)
                                {
                                    var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == blockSchedule.BlockId && v.BellScheduleDate >= pageResult.DobStartDate && v.BellScheduleDate <= pageResult.DobEndDate && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                    if (bellScheduleList.Count > 0)
                                    {
                                        foreach (var bellSchedule in bellScheduleList)
                                        {
                                            CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == blockSchedule.BlockPeriodId).ToList();

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                courseSectionBlock.AttendanceDate = bellSchedule.BellScheduleDate;
                                                courseSectionBlock.CourseId = staffCourseSectionData.CourseId;
                                                courseSectionBlock.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                courseSectionBlock.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                courseSectionBlock.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                courseSectionBlock.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                courseSectionBlock.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                courseSectionBlock.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                courseSectionBlock.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == blockSchedule.BlockPeriodId).PeriodTitle : null;
                                                courseSectionBlock.BlockId = blockSchedule.BlockId;
                                                courseSectionBlock.PeriodId = blockSchedule.BlockPeriodId;
                                                courseSectionBlock.AttendanceTaken = blockSchedule.TakeAttendanceBlock;

                                                staffCoursesectionSchedule.Add(courseSectionBlock);
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


                                DateTime start = (DateTime)pageResult.DobStartDate;
                                DateTime end = (DateTime)pageResult.DobEndDate;

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

                                 meetingDays = staffCourseSectionData.MeetingDays.ToLower().Split("|");

                                bool allDays = meetingDays == null || !meetingDays.Any();

                                 dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                      .Select(offset => start.AddDays(offset))
                                                      .Where(d => allDays || meetingDays.Contains(d.DayOfWeek.ToString().ToLower()))
                                                      .ToList();

                                dateList = dateList.Where(s => dateList.Any(secL => s.Date >= staffCourseSectionData.DurationStartDate && s.Date <= staffCourseSectionData.DurationEndDate)).ToList();

                                if (dateList.Count > 0)
                                {
                                    dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                }

                                foreach (var date in dateList)
                                {
                                    //CourseSectionViewList CourseSections = new CourseSectionViewList();

                                    //var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId);

                                    if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                    {                                        
                                        CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault().FixedPeriodId);

                                        if (staffAttendanceData.Count() == 0)
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


                                            CourseSectionFixed.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewLists.FirstOrDefault().FixedPeriodId).PeriodTitle : null;
                                            CourseSectionFixed.BlockId = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == allCourseSectionVewLists.FirstOrDefault().FixedPeriodId)?.BlockId : null;
                                            CourseSectionFixed.PeriodId = allCourseSectionVewLists.FirstOrDefault().FixedPeriodId;
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

                                                if (staffAttendanceData.Count() == 0)
                                                {
                                                    CourseSectionVariable.AttendanceDate = date;
                                                    CourseSectionVariable.CourseId = staffCourseSectionData.CourseId;
                                                    CourseSectionVariable.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                    CourseSectionVariable.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                    CourseSectionVariable.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                    CourseSectionVariable.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                    CourseSectionVariable.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                    CourseSectionVariable.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                    CourseSectionVariable.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId).PeriodTitle : null;
                                                    CourseSectionVariable.BlockId = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(e => e.PeriodId == courseVariableSchedule.VarPeriodId)?.BlockId : null;
                                                    CourseSectionVariable.PeriodId = courseVariableSchedule.VarPeriodId;
                                                    CourseSectionVariable.AttendanceTaken = courseVariableSchedule.TakeAttendanceVariable;

                                                    staffCoursesectionSchedule.Add(CourseSectionVariable);
                                                }
                                            }
                                        }
                                        //if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                                        //{
                                        //    CourseSections.ScheduleType = "Block Schedule";

                                        //    //var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();

                                        //    if (CourseBlockSchedule.Count > 0)
                                        //    {
                                        //        var data = CourseBlockSchedule.Where(e => e.Date == date).ToList();
                                        //        CourseSections.courseBlockSchedule = data;
                                        //    }
                                        //}

                                        /*staffCoursesectionSchedule.Add(CourseSections);*/
                                    }
                                    //staffCoursesectionSchedule.Add(CourseSections);


                                }

                            }
                            else
                            {
                                if (allCourseSectionVewLists.Count > 0)
                                {
                                    var calenderScheduleList = allCourseSectionVewLists.Where(c => c.CalDate >= pageResult.DobStartDate && c.CalDate <= pageResult.DobEndDate && c.CalDate<=DateTime.Today.Date);

                                    if (calenderScheduleList.ToList().Count > 0)
                                    {
                                        foreach (var calenderSchedule in calenderScheduleList)
                                        {
                                            CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                CourseSectioncalender.AttendanceDate = (DateTime)calenderSchedule.CalDate;
                                                CourseSectioncalender.CourseId = staffCourseSectionData.CourseId;
                                                CourseSectioncalender.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                CourseSectioncalender.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                CourseSectioncalender.StaffFirstGivenName = staffCourseSectionData.StaffMaster.FirstGivenName;
                                                CourseSectioncalender.StaffMiddleName = staffCourseSectionData.StaffMaster.MiddleName;
                                                CourseSectioncalender.StaffLastFamilyName = staffCourseSectionData.StaffMaster.LastFamilyName;
                                                CourseSectioncalender.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;


                                                CourseSectioncalender.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId).PeriodTitle : null;
                                                CourseSectioncalender.BlockId = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)?.BlockId : null;
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

                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = staffCoursesectionSchedule.AsQueryable();
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                            transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate != null && x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
                        }
                    }
                    transactionIQ = transactionIQ.Distinct();

                    if (pageResult.SortingModel != null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                    }

                    int totalCount = transactionIQ.Count();

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
            IQueryable<StudendAttendanceAdministrationViewModel> transactionIQ = null;
           List< StudendAttendanceAdministrationViewModel> attendanceData = new List<StudendAttendanceAdministrationViewModel>();
            try
            {
                var studentAttendanceData = this.context?.StudentAttendance.Include(s => s.StudentAttendanceComments).Include(s => s.BlockPeriod).Include(s => s.AttendanceCodeNavigation).Include(s => s.StudentCoursesectionSchedule).ThenInclude(s => s.StudentMaster).ThenInclude(s => s.StudentEnrollment).Include(s => s.StudentCoursesectionSchedule.StudentMaster.Sections).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.AttendanceDate == pageResult.AttendanceDate && (pageResult.AttendanceCode == null || x.AttendanceCode == pageResult.AttendanceCode)).ToList();

                var studentIds = studentAttendanceData.Select(a => a.StudentId).Distinct().ToList();
                var blockId = studentAttendanceData.FirstOrDefault().BlockId;
                foreach (var ide in studentIds)
                {
                    StudendAttendanceAdministrationViewModel administrationViewModel = new StudendAttendanceAdministrationViewModel();

                    var studentDailyAttendanceData = this.context?.StudentDailyAttendance.FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.StudentId == ide && x.AttendanceDate == pageResult.AttendanceDate);

                    if (studentDailyAttendanceData != null)
                    {
                        var blockData = this.context?.Block.FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.BlockId == blockId);
                        if (studentDailyAttendanceData.AttendanceMinutes >= blockData.FullDayMinutes)
                        {
                            administrationViewModel.Present = "Full-Day";
                        }
                        if (studentDailyAttendanceData.AttendanceMinutes >= blockData.HalfDayMinutes && studentDailyAttendanceData.AttendanceMinutes < blockData.FullDayMinutes)
                        {
                            administrationViewModel.Present = "Half-Day";
                        }
                    }

                    var studentAttendance = studentAttendanceData.Where(x => x.StudentId == ide);

                    var attendance = studentAttendance.FirstOrDefault();

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

                    studentAttendance.ToList().ForEach(x => { x.BlockPeriod.StudentAttendance = null; x.AttendanceCodeNavigation.StudentAttendance = null;x.StudentCoursesectionSchedule.StudentMaster = null; });
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
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams, attendanceData).AsQueryable();
                        }
                    }

                    if (pageResult.SortingModel != null)
                    {
                        switch (pageResult.SortingModel.SortColumn.ToLower())
                        {
                            default:
                                transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                                break;
                        }
                    }

                    if (transactionIQ != null)
                    {
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
                var CourseSectionData = this.context?.CourseSection.Include(x => x.StudentCoursesectionSchedule).Include(x => x.Course).Include(x => x.SchoolCalendars).Where(x => x.TenantId == courseSectionForAttendanceViewModel.TenantId && x.SchoolId == courseSectionForAttendanceViewModel.SchoolId && x.AcademicYear == courseSectionForAttendanceViewModel.AcademicYear).ToList();

                if (CourseSectionData.Count() > 0)
                {
                    foreach (var courseSection in CourseSectionData)
                    {
                        var studentExistInCS = courseSection.StudentCoursesectionSchedule.Where(x => x.IsDropped != true).ToList();
                        if (studentExistInCS.Count > 0)
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();
                            if (courseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                if (courseSection.AttendanceTaken == true)
                                {
                                    CourseSections.ScheduleType = "Fixed Schedule";

                                    var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).FirstOrDefault(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId);
                                    if (courseFixedScheduleData != null)
                                    {
                                        courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = null;
                                        courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = null;
                                        courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = null;
                                        courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = null;
                                        CourseSections.courseFixedSchedule = courseFixedScheduleData;
                                    }
                                }
                            }

                            if (courseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseVariableScheduleData.Count > 0)
                                {
                                    courseVariableScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                    CourseSections.courseVariableSchedule = courseVariableScheduleData;
                                }
                            }

                            if (courseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSections.ScheduleType = "Calendar Schedule";

                                var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseCalenderScheduleData.Count > 0)
                                {
                                    courseCalenderScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                    CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                                }
                            }

                            if (courseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseSectionId == courseSection.CourseSectionId && x.TakeAttendance == true).ToList();

                                if (courseBlockScheduleData.Count > 0)
                                {
                                    courseBlockScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; });

                                    CourseSections.courseBlockSchedule = courseBlockScheduleData;
                                }
                            }
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
            try
            {
                if (studentAttendanceAddViewModel.studentAttendance.Count > 0)
                {
                    List<StudentAttendance> studentAttendanceList = new List<StudentAttendance>();
                    List<StudentAttendanceComments> studentAttendanceCommentsList = new List<StudentAttendanceComments>();
                    int? StudentAttendanceId = 1;

                    var studentAttendanceData = this.context?.StudentAttendance.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId).OrderByDescending(x => x.StudentAttendanceId).FirstOrDefault();

                    if (studentAttendanceData != null)
                    {
                        StudentAttendanceId = studentAttendanceData.StudentAttendanceId + 1;
                    }

                    long? CommentId = Utility.GetMaxLongPK(this.context, new Func<StudentAttendanceComments, long>(x => x.CommentId));

                    var allCsData = this.context?.AllCourseSectionView.Where(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId).ToList();

                    var staffId = this.context?.StaffCoursesectionSchedule.FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.CourseSectionId == studentAttendanceAddViewModel.CourseSectionId && x.IsDropped != true)?.StaffId;
                    if (allCsData.Count > 0)
                    {
                        foreach (var studentAttendance in studentAttendanceAddViewModel.studentAttendance)
                        {
                            int? blockIde = 1;
                            int? periodIde = null;
                            CommentId++;

                            if (allCsData.FirstOrDefault().ScheduleType == "Fixed Schedule(1)")
                            {
                                periodIde = allCsData.FirstOrDefault().FixedPeriodId;
                            }
                            if (allCsData.FirstOrDefault().ScheduleType == "Variable Schedule (2)")
                            {
                                var day = studentAttendance.AttendanceDate.DayOfWeek.ToString();
                                periodIde = allCsData.Where(x => x.VarDay.ToLower() == day.ToLower()).Select(s => s.VarPeriodId).FirstOrDefault();
                            }
                            if (allCsData.FirstOrDefault().ScheduleType == "Calendar Schedule (3)")
                            {
                                periodIde = allCsData.Where(x => x.CalDate == studentAttendance.AttendanceDate).Select(s => s.CalPeriodId).FirstOrDefault();
                            }
                            if (allCsData.FirstOrDefault().ScheduleType == "Block Schedule (4)")
                            {
                                var BellScheduleData = this.context?.BellSchedule.FirstOrDefault(x => x.SchoolId == studentAttendanceAddViewModel.SchoolId && x.TenantId == studentAttendanceAddViewModel.TenantId && x.BellScheduleDate == studentAttendance.AttendanceDate);
                                if (BellScheduleData != null)
                                {
                                    blockIde = BellScheduleData.BlockId;
                                    periodIde = allCsData.Where(x => x.BlockId == BellScheduleData.BlockId).Select(s => s.BlockPeriodId).FirstOrDefault();
                                }
                            }

                            var studentAttendanceAdd = new StudentAttendance()
                            {
                                TenantId = studentAttendanceAddViewModel.TenantId,
                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                StudentId = studentAttendance.StudentId,
                                StaffId = (int)staffId,
                                CourseId = studentAttendanceAddViewModel.CourseId,
                                CourseSectionId = studentAttendanceAddViewModel.CourseSectionId,
                                AttendanceCategoryId = (int)studentAttendanceAddViewModel.AttendanceCategoryId,
                                AttendanceCode = (int)studentAttendanceAddViewModel.AttendanceCode,
                                AttendanceDate = studentAttendance.AttendanceDate,
                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                CreatedOn = DateTime.UtcNow,
                                BlockId = (int)blockIde,
                                PeriodId = (int)periodIde,
                                StudentAttendanceId = (int)StudentAttendanceId,
                                MembershipId = studentAttendanceAddViewModel.MembershipId
                            };
                            var StudentAttendanceComments = new StudentAttendanceComments
                            {
                                TenantId = studentAttendanceAddViewModel.TenantId,
                                SchoolId = studentAttendanceAddViewModel.SchoolId,
                                StudentId = studentAttendance.StudentId,
                                StudentAttendanceId = (int)StudentAttendanceId,
                                CommentId = (long)CommentId,
                                Comment = studentAttendanceAddViewModel.AbsencesReason,
                                CommentTimestamp = DateTime.UtcNow,
                                CreatedBy = studentAttendanceAddViewModel.CreatedBy,
                                CreatedOn = DateTime.UtcNow,
                                MembershipId = studentAttendanceAddViewModel.MembershipId
                            };
                            studentAttendanceList.Add(studentAttendanceAdd);
                            studentAttendanceCommentsList.Add(StudentAttendanceComments);
                            StudentAttendanceId++;
                        }
                    }
                    this.context?.StudentAttendance.AddRange(studentAttendanceList);
                    this.context?.StudentAttendanceComments.AddRange(studentAttendanceCommentsList);
                    this.context?.SaveChanges();
                    studentAttendanceAddViewModel._message = "Add Absences Added Successfully";
                }
                else
                {
                    studentAttendanceAddViewModel._failure = true;
                    studentAttendanceAddViewModel._message = "Please Select Student";
                }
            }
            catch (Exception es)
            {
                studentAttendanceAddViewModel._failure = true;
                studentAttendanceAddViewModel._message = es.Message;
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
            using (var transaction = this.context.Database.BeginTransaction())
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
                                    var AttendanceCodeId = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.Title.ToLower() == studentDailyAttendance.AttendanceCode.ToLower())?.AttendanceCode1;
                                    if (AttendanceCodeId != null)
                                    {
                                        if (studentDailyAttendance.AttendanceCode.ToLower() != "absent")
                                        {
                                            var StudentAttendanceData = this.context?.StudentAttendance.Where(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.StudentId == studentDailyAttendance.StudentId && x.AttendanceDate == studentDailyAttendanceListViewModel.AttendanceDate).ToList();

                                            if (StudentAttendanceData.Count > 0)
                                            {
                                                var blockData = this.context?.Block.FirstOrDefault(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.BlockId == StudentAttendanceData.FirstOrDefault().BlockId);
                                                StudentAttendanceData.ToList().ForEach(x => x.AttendanceCode = (int)AttendanceCodeId);

                                                studentDailyAttendanceData.AttendanceMinutes = blockData.FullDayMinutes;
                                            }

                                        }
                                        else
                                        {
                                            var StudentAttendanceData = this.context?.StudentAttendance.Include(x => x.StudentAttendanceComments).Where(x => x.TenantId == studentDailyAttendanceListViewModel.TenantId && x.SchoolId == studentDailyAttendanceListViewModel.SchoolId && x.StudentId == studentDailyAttendance.StudentId && x.AttendanceDate == studentDailyAttendanceListViewModel.AttendanceDate).ToList();

                                            if (StudentAttendanceData.Count > 0)
                                            {
                                                StudentAttendanceData.ToList().ForEach(x => x.AttendanceCode = (int)AttendanceCodeId);

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
                        transaction.Commit();
                        studentDailyAttendanceListViewModel._failure = false;
                        studentDailyAttendanceListViewModel._message = "Student Daily Attendance Updated Successfully";
                    }
                    else
                    {
                        studentDailyAttendanceListViewModel._failure = true;
                        studentDailyAttendanceListViewModel._message = "Please Select Student";
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
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
            try
            {
                if (studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId>0)
                {
                    var studentAttendanceCommentUpdate = this.context?.StudentAttendanceComments.FirstOrDefault(x => x.TenantId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.TenantId && x.SchoolId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.SchoolId && x.CommentId == studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId && x.StudentId== studentAttendanceCommentsAddViewModel.studentAttendanceComments.StudentId && x.StudentAttendanceId== studentAttendanceCommentsAddViewModel.studentAttendanceComments.StudentAttendanceId);

                    if (studentAttendanceCommentUpdate != null)
                    {
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.UpdatedOn = DateTime.UtcNow;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentTimestamp = DateTime.UtcNow;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedOn = studentAttendanceCommentUpdate.CreatedOn;
                        studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedBy = studentAttendanceCommentUpdate.CreatedBy;

                        this.context.Entry(studentAttendanceCommentUpdate).CurrentValues.SetValues(studentAttendanceCommentsAddViewModel.studentAttendanceComments);
                        this.context?.SaveChanges();
                        studentAttendanceCommentsAddViewModel._failure = false;
                        studentAttendanceCommentsAddViewModel._message = "Student Attendance Comment Updated Successfully";
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
                    
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentId = (long)CommentId;
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CreatedOn = DateTime.UtcNow;
                    studentAttendanceCommentsAddViewModel.studentAttendanceComments.CommentTimestamp = DateTime.UtcNow;
                    this.context?.StudentAttendanceComments.Add(studentAttendanceCommentsAddViewModel.studentAttendanceComments);
                    this.context?.SaveChanges();
                    studentAttendanceCommentsAddViewModel._failure = false;
                    studentAttendanceCommentsAddViewModel._message = "Student Attendance Comment Added Successfully";
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

                if (studentDailyAttendanceData.Count > 0)
                {
                    var AttendanceDateData = studentDailyAttendanceData.Select(s => s.AttendanceDate).Distinct().ToList();
                    foreach (var AttendanceDate in AttendanceDateData)
                    {
                        //all class in a day have only one block 
                        var AttendanceDataInaDay = this.context?.StudentAttendance.Where(x => x.TenantId == reCalculateDailyAttendanceViewModel.TenantId && x.SchoolId == reCalculateDailyAttendanceViewModel.SchoolId && x.AttendanceDate == AttendanceDate).ToList();
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
                                    var periodEndTime = TimeSpan.Parse(BlockPeriodData.PeriodEndTime);
                                    var periodStartTime = TimeSpan.Parse(BlockPeriodData.PeriodStartTime);
                                    TimeSpan? periodTime = periodEndTime - periodStartTime;
                                    var hour = Convert.ToInt32(periodTime.Value.Hours);
                                    var min = Convert.ToInt32(periodTime.Value.Minutes);
                                    var classMin = hour > 0 ? (hour * 60 + min) : min;

                                    var AttendanceCodeData = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == StudentAttendance.TenantId && x.SchoolId == StudentAttendance.SchoolId && x.AttendanceCode1 == StudentAttendance.AttendanceCode && x.AttendanceCategoryId == StudentAttendance.AttendanceCategoryId);
                                    if (AttendanceCodeData != null)
                                    {
                                        if (AttendanceCodeData.Title.ToLower() != "absent")
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
                    this.context.SaveChanges();
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
    }
}