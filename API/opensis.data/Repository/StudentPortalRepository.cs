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
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.StaffSchedule;
using opensis.data.ViewModels.StudentPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class StudentPortalRepository: IStudentPortalRepository
    {
        private readonly CRMContext? context;
        //private readonly CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
        
        public StudentPortalRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get Student Dashboard
        /// </summary>
        /// <param name="studentDashboardViewModel"></param>
        /// <returns></returns>
        public StudentDashboardViewModel GetStudentDashboard(StudentDashboardViewModel studentDashboardViewModel)
        {
            StudentDashboardViewModel scheduledCourseSectionViewForStudent = new();
            List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new();
            //CourseFixedSchedule fixedData = new();
            List<CourseVariableSchedule>? variableData = new();
            List<CourseCalendarSchedule>? calenderData = new();
            List<CourseBlockSchedule>? blockData = new();

            scheduledCourseSectionViewForStudent.TenantId = studentDashboardViewModel.TenantId;
            scheduledCourseSectionViewForStudent._tenantName = studentDashboardViewModel._tenantName;
            scheduledCourseSectionViewForStudent.SchoolId = studentDashboardViewModel.SchoolId;
            scheduledCourseSectionViewForStudent.StudentId = studentDashboardViewModel.StudentId;
            scheduledCourseSectionViewForStudent._token = studentDashboardViewModel._token;
            scheduledCourseSectionViewForStudent._userName = studentDashboardViewModel._userName;
            scheduledCourseSectionViewForStudent.AllCourse = studentDashboardViewModel.AllCourse;

            try
            {
                var todayDate = DateTime.Today;

                var studentMasterData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Include(s => s.Sections).Where(x => x.TenantId == studentDashboardViewModel.TenantId && x.SchoolId == studentDashboardViewModel.SchoolId && x.StudentId == studentDashboardViewModel.StudentId).FirstOrDefault();
                if (studentMasterData != null)
                {
                    //fetch student details
                    scheduledCourseSectionViewForStudent.FirstGivenName = studentMasterData.FirstGivenName;
                    scheduledCourseSectionViewForStudent.MiddleName = studentMasterData.MiddleName;
                    scheduledCourseSectionViewForStudent.LastFamilyName = studentMasterData.LastFamilyName;
                    scheduledCourseSectionViewForStudent.StudentInternalId = studentMasterData.StudentInternalId;
                    scheduledCourseSectionViewForStudent.GradeLevel = studentMasterData.StudentEnrollment.FirstOrDefault(x => x.IsActive == true && x.SchoolId == studentDashboardViewModel.SchoolId)?.GradeLevelTitle;
                    scheduledCourseSectionViewForStudent.Section = studentMasterData.Sections?.Name;
                    scheduledCourseSectionViewForStudent.StudentPhoto = studentMasterData.StudentPhoto;

                    var scheduledCourseSectionData = this.context?.StudentCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Join(this.context.AllCourseSectionView,
                        scs => scs.CourseSectionId, acsv => acsv.CourseSectionId,
                        (scs, acsv) => new { scs, acsv }).Where(x => x.scs.TenantId == studentDashboardViewModel.TenantId && x.acsv.TenantId == studentDashboardViewModel.TenantId && x.scs.SchoolId == studentDashboardViewModel.SchoolId && x.acsv.SchoolId == studentDashboardViewModel.SchoolId && x.scs.StudentId == studentDashboardViewModel.StudentId && x.scs.IsDropped != true).ToList();

                    if (scheduledCourseSectionData != null && scheduledCourseSectionData.Any())
                    {

                        if (studentDashboardViewModel.AllCourse != true)
                        {
                            var coursesectionScheduleList = scheduledCourseSectionData.Where(x => x.acsv.DurationEndDate >= todayDate && ((x.acsv.FixedDays != null
                                && x.acsv.FixedDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower())) || (x.acsv.VarDay != null
                                && x.acsv.VarDay.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower())) || (x.acsv.CalDate != null && x.acsv.CalDate == todayDate) || x.acsv.BlockPeriodId != null)).ToList();

                            studentCoursesectionScheduleList = coursesectionScheduleList.Select(s => s.scs).Distinct().ToList();
                        }
                        else
                        {
                            if (studentDashboardViewModel.MarkingPeriodStartDate != null && studentDashboardViewModel.MarkingPeriodEndDate != null)
                            {
                                scheduledCourseSectionData = scheduledCourseSectionData.Where(x => x.scs.CourseSection.DurationBasedOnPeriod == false || ((studentDashboardViewModel.MarkingPeriodStartDate >= x.acsv.DurationStartDate && studentDashboardViewModel.MarkingPeriodStartDate <= x.acsv.DurationEndDate) && (studentDashboardViewModel.MarkingPeriodEndDate >= x.acsv.DurationStartDate && studentDashboardViewModel.MarkingPeriodEndDate <= x.acsv.DurationEndDate))).ToList();
                            }
                            else
                            {
                                scheduledCourseSectionViewForStudent._failure = true;
                                scheduledCourseSectionViewForStudent._message = "Please send Marking Period Start Date and Marking Period End Date";
                                return scheduledCourseSectionViewForStudent;
                            }
                        }
                    }
                    else
                    {
                        scheduledCourseSectionViewForStudent._failure = true;
                        scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                        return scheduledCourseSectionViewForStudent;
                    }

                    if (scheduledCourseSectionData.Any())
                    {
                        foreach (var scheduledCourseSection in scheduledCourseSectionData)
                        {
                            List<DateTime> holidayList = new();
                            //Calculate Holiday
                            var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == studentDashboardViewModel.TenantId && e.CalendarId == scheduledCourseSection.scs.CourseSection.CalendarId && (e.StartDate >= scheduledCourseSection.acsv.DurationStartDate && e.StartDate <= scheduledCourseSection.acsv.DurationEndDate || e.EndDate >= scheduledCourseSection.acsv.DurationStartDate && e.EndDate <= scheduledCourseSection.acsv.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == studentDashboardViewModel.SchoolId || e.ApplicableToAllSchool == true)).ToList();
                            if (CalendarEventsData != null && CalendarEventsData.Any())
                            {
                                foreach (var calender in CalendarEventsData)
                                {
                                    if (calender.EndDate != null && calender.StartDate != null)
                                    {
                                        if (calender.EndDate.Value.Date > calender.StartDate.Value.Date)
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
                            if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.scs.CourseSection.ScheduleType == "Variable Schedule (2)" || scheduledCourseSection.scs.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSectionViewList CourseSections = new CourseSectionViewList
                                {
                                    CourseTitle = scheduledCourseSection.scs.CourseSection.Course.CourseTitle
                                };

                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                {
                                    CourseSections.ScheduleType = "Fixed Schedule";
                                    CourseSections.MeetingDays = scheduledCourseSection.scs.CourseSection.MeetingDays;

                                    var fixedData = context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId);



                                    if (fixedData != null)
                                    {
                                        CourseSections.courseFixedSchedule = fixedData;
                                        CourseSections.HolidayList = holidayList;
                                    }
                                }
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    CourseSections.ScheduleType = "Variable Schedule";

                                    variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();


                                    if (variableData?.Any() == true)
                                    {
                                        var days = variableData.Select(s => s.Day);
                                        CourseSections.MeetingDays = String.Join("|", days);
                                        CourseSections.courseVariableSchedule = variableData;
                                        CourseSections.HolidayList = holidayList;
                                    }
                                }
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    CourseSections.ScheduleType = "Block Schedule";

                                    blockData = this.context?.CourseBlockSchedule.Include(v => v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();

                                    if (blockData?.Any() == true)
                                    {
                                        CourseSections.MeetingDays = "Block Days";
                                        CourseSections.courseBlockSchedule = blockData;
                                        CourseSections.HolidayList = holidayList;

                                        var bellScheduleList = new List<BellSchedule>();
                                        foreach (var block in blockData)
                                        {
                                            var bellScheduleData = this.context?.BellSchedule.Where(c => c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.BlockId == block.BlockId && c.BellScheduleDate >= scheduledCourseSection.acsv.DurationStartDate && c.BellScheduleDate <= scheduledCourseSection.acsv.DurationEndDate).ToList();
                                            if (bellScheduleData?.Any() == true)
                                            {
                                                bellScheduleList.AddRange(bellScheduleData);
                                            }
                                        }
                                        CourseSections.bellScheduleList = bellScheduleList;
                                    }
                                }
                                CourseSections.CalendarId = scheduledCourseSection.scs.CourseSection.CalendarId;
                                CourseSections.CourseId = scheduledCourseSection.scs.CourseId;
                                CourseSections.CourseGradeLevel = scheduledCourseSection.scs.CourseSection.Course.CourseGradeLevel;
                                CourseSections.CourseSectionId = scheduledCourseSection.scs.CourseSectionId;
                                CourseSections.GradeScaleType = scheduledCourseSection.scs.CourseSection.GradeScaleType;
                                CourseSections.AttendanceCategoryId = scheduledCourseSection.scs.CourseSection.AttendanceCategoryId;
                                CourseSections.GradeScaleId = scheduledCourseSection.scs.CourseSection.GradeScaleId;
                                CourseSections.StandardGradeScaleId = scheduledCourseSection.scs.CourseSection.StandardGradeScaleId;
                                CourseSections.CourseSectionName = scheduledCourseSection.scs.CourseSectionName;
                                CourseSections.YrMarkingPeriodId = scheduledCourseSection.acsv.YrMarkingPeriodId;
                                CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.acsv.SmstrMarkingPeriodId;
                                CourseSections.QtrMarkingPeriodId = scheduledCourseSection.acsv.QtrMarkingPeriodId;
                                CourseSections.PrgrsprdMarkingPeriodId = scheduledCourseSection.acsv.PrgrsprdMarkingPeriodId;
                                CourseSections.DurationStartDate = scheduledCourseSection.acsv.DurationStartDate;
                                CourseSections.DurationEndDate = scheduledCourseSection.acsv.DurationEndDate;
                                CourseSections.MeetingDays = scheduledCourseSection.scs.CourseSection.MeetingDays;
                                CourseSections.AttendanceTaken = scheduledCourseSection.scs.CourseSection.AttendanceTaken;
                                CourseSections.WeekDays = scheduledCourseSection.scs.CourseSection.SchoolCalendars!.Days;

                                scheduledCourseSectionViewForStudent.courseSectionViewList.Add(CourseSections);

                            }
                            else
                            {
                                if (scheduledCourseSection.scs.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    CourseSectionViewList CourseSection = new CourseSectionViewList
                                    {
                                        ScheduleType = "Calendar Schedule"
                                    };

                                    calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (studentDashboardViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.scs.SchoolId && c.TenantId == scheduledCourseSection.scs.TenantId && c.CourseSectionId == scheduledCourseSection.scs.CourseSectionId).ToList();

                                    if (calenderData != null && calenderData.Any())
                                    {
                                        CourseSection.MeetingDays = "Calendar Days";
                                        CourseSection.courseCalendarSchedule = calenderData;
                                        CourseSection.HolidayList = holidayList;

                                        CourseSection.CourseTitle = scheduledCourseSection.scs.CourseSection.Course.CourseTitle;
                                        CourseSection.CalendarId = scheduledCourseSection.scs.CourseSection.CalendarId;
                                        CourseSection.CourseId = scheduledCourseSection.scs.CourseId;
                                        CourseSection.CourseGradeLevel = scheduledCourseSection.scs.CourseSection.Course.CourseGradeLevel;
                                        CourseSection.CourseSectionId = scheduledCourseSection.scs.CourseSectionId;
                                        CourseSection.GradeScaleType = scheduledCourseSection.scs.CourseSection.GradeScaleType;
                                        CourseSection.AttendanceCategoryId = scheduledCourseSection.scs.CourseSection.AttendanceCategoryId;
                                        CourseSection.GradeScaleId = scheduledCourseSection.scs.CourseSection.GradeScaleId;
                                        CourseSection.StandardGradeScaleId = scheduledCourseSection.scs.CourseSection.StandardGradeScaleId;
                                        CourseSection.CourseSectionName = scheduledCourseSection.scs.CourseSectionName;
                                        CourseSection.YrMarkingPeriodId = scheduledCourseSection.acsv.YrMarkingPeriodId;
                                        CourseSection.SmstrMarkingPeriodId = scheduledCourseSection.acsv.SmstrMarkingPeriodId;
                                        CourseSection.PrgrsprdMarkingPeriodId = scheduledCourseSection.acsv.PrgrsprdMarkingPeriodId;
                                        CourseSection.DurationStartDate = scheduledCourseSection.acsv.DurationStartDate;
                                        CourseSection.DurationEndDate = scheduledCourseSection.acsv.DurationEndDate;
                                        //CourseSection.MeetingDays = scheduledCourseSection.scs.CourseSection.MeetingDays;
                                        CourseSection.AttendanceTaken = scheduledCourseSection.scs.CourseSection.AttendanceTaken;
                                        CourseSection.WeekDays = scheduledCourseSection.scs.CourseSection.SchoolCalendars!.Days;

                                        scheduledCourseSectionViewForStudent.courseSectionViewList.Add(CourseSection);


                                    }
                                }
                            }
                        }

                        //this block for assignment details
                        if (studentDashboardViewModel.MarkingPeriodStartDate != null && studentDashboardViewModel.MarkingPeriodEndDate != null)
                        {
                            int? markingPeriodId = null;
                            //var assignmentTypeIds=new List<int>();
                            var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == studentDashboardViewModel.SchoolId && x.TenantId == studentDashboardViewModel.TenantId && x.StartDate == studentDashboardViewModel.MarkingPeriodStartDate && x.EndDate == studentDashboardViewModel.MarkingPeriodEndDate).FirstOrDefault();

                            if (progressPeriodsData != null)
                            {
                                markingPeriodId = progressPeriodsData.MarkingPeriodId;
                            }
                            else
                            {
                                var quarterData = this.context?.Quarters.Where(x => x.SchoolId == studentDashboardViewModel.SchoolId && x.TenantId == studentDashboardViewModel.TenantId && x.StartDate == studentDashboardViewModel.MarkingPeriodStartDate && x.EndDate == studentDashboardViewModel.MarkingPeriodEndDate).FirstOrDefault();

                                if (quarterData != null)
                                {
                                    markingPeriodId = quarterData.MarkingPeriodId;
                                }
                                else
                                {
                                    var semesterData = this.context?.Semesters.Where(x => x.SchoolId == studentDashboardViewModel.SchoolId && x.TenantId == studentDashboardViewModel.TenantId && x.StartDate == studentDashboardViewModel.MarkingPeriodStartDate && x.EndDate == studentDashboardViewModel.MarkingPeriodEndDate).FirstOrDefault();

                                    if (semesterData != null)
                                    {
                                        markingPeriodId = semesterData.MarkingPeriodId;
                                    }
                                    else
                                    {
                                        var yearData = this.context?.SchoolYears.Where(x => x.SchoolId == studentDashboardViewModel.SchoolId && x.TenantId == studentDashboardViewModel.TenantId && x.StartDate == studentDashboardViewModel.MarkingPeriodStartDate && x.EndDate == studentDashboardViewModel.MarkingPeriodEndDate).FirstOrDefault();

                                        if (yearData != null)
                                        {
                                            markingPeriodId = yearData.MarkingPeriodId;
                                        }
                                    }
                                }
                            }

                            foreach (var courseSection in scheduledCourseSectionViewForStudent.courseSectionViewList)
                            {
                                var assignmentData = this.context?.Assignment.Include(s => s.AssignmentType).Where(c => c.SchoolId == studentDashboardViewModel.SchoolId && c.TenantId == studentDashboardViewModel.TenantId && c.CourseSectionId == courseSection.CourseSectionId && ((c.AssignmentType.PrgrsprdMarkingPeriodId != null && c.AssignmentType.PrgrsprdMarkingPeriodId == markingPeriodId) || (c.AssignmentType.QtrMarkingPeriodId != null && c.AssignmentType.QtrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.SmstrMarkingPeriodId != null && c.AssignmentType.SmstrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.YrMarkingPeriodId != null && c.AssignmentType.YrMarkingPeriodId == markingPeriodId) || (c.AssignmentType.PrgrsprdMarkingPeriodId == null && c.AssignmentType.QtrMarkingPeriodId == null && c.AssignmentType.SmstrMarkingPeriodId == null && c.AssignmentType.YrMarkingPeriodId == null))).Select(a => new AssignmentDetails { AssignmentTypeId = a.AssignmentTypeId, AssignmentId = a.AssignmentId, CourseSectionId = a.CourseSectionId, CourseSectionTitle = courseSection.CourseSectionName, AssignmentTypeTitle = a.AssignmentType.Title, AssignmentTitle = a.AssignmentTitle, DueDate = a.DueDate, AssignmentDate = a.AssignmentDate, AssignmentDescription = a.AssignmentDescription }).ToList();

                                if (assignmentData?.Any() == true)
                                {
                                    scheduledCourseSectionViewForStudent.AssignmentList.AddRange(assignmentData);
                                }
                            }
                        }

                        foreach (var courseSection in scheduledCourseSectionViewForStudent.courseSectionViewList)
                        {
                            if (courseSection.courseFixedSchedule != null)
                            {
                                courseSection.courseFixedSchedule.BlockPeriod!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                                courseSection.courseFixedSchedule.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>();
                                courseSection.courseFixedSchedule.BlockPeriod.StudentMissingAttendances = new List<StudentMissingAttendance>();
                                courseSection.courseFixedSchedule.Rooms!.CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
                                courseSection.courseFixedSchedule.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
                            }
                            else if (courseSection.courseVariableSchedule?.Any() == true)
                            {
                                courseSection.courseVariableSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                            else if (courseSection.courseCalendarSchedule?.Any() == true)
                            {
                                courseSection.courseCalendarSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                            else if (courseSection.courseBlockSchedule?.Any() == true)
                            {
                                courseSection.courseBlockSchedule.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.BlockPeriod.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.BlockPeriod.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.BlockPeriod.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); x.BlockPeriod.StudentAttendance = new HashSet<StudentAttendance>(); x.BlockPeriod.StudentMissingAttendances = new HashSet<StudentMissingAttendance>(); x.Rooms.CourseFixedSchedule = new HashSet<CourseFixedSchedule>(); x.Rooms.CourseVariableSchedule = new HashSet<CourseVariableSchedule>(); x.Rooms.CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>(); x.Rooms.CourseBlockSchedule = new HashSet<CourseBlockSchedule>(); });
                            }
                        }
                    }
                    else
                    {
                        scheduledCourseSectionViewForStudent._failure = true;
                        scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                        return scheduledCourseSectionViewForStudent;
                    }

                    if (studentDashboardViewModel.MembershipId != null)
                    {
                        var noticeList = this.context?.Notice.Where(x => x.TenantId == studentDashboardViewModel.TenantId && (x.SchoolId == studentDashboardViewModel.SchoolId || (x.SchoolId != studentDashboardViewModel.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && x.TargetMembershipIds.Contains((studentDashboardViewModel.MembershipId ?? 0).ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

                        if (noticeList?.Any() == true)
                        {
                            scheduledCourseSectionViewForStudent.NoticeList = noticeList;
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionViewForStudent._failure = true;
                    scheduledCourseSectionViewForStudent._message = NORECORDFOUND;
                    return scheduledCourseSectionViewForStudent;
                }
            }
            catch (Exception ex)
            {
                scheduledCourseSectionViewForStudent._failure = true;
                scheduledCourseSectionViewForStudent._message = ex.Message;
            }
            return scheduledCourseSectionViewForStudent;
        }

        /// <summary>
        /// Get Student Gradebook Grades
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentGradebookViewModel GetStudentGradebookGrades(PageResult pageResult)
        {
            StudentGradebookViewModel studentGradebook = new StudentGradebookViewModel();
            List<StudentGradebookGradeDetails> studentGradeList = new();
            IQueryable<StudentGradebookGradeDetails>? transactionIQ = null;

            studentGradebook._tenantName = pageResult._tenantName;
            studentGradebook._token = pageResult._token;
            studentGradebook._userName = pageResult._userName;
            try
            {
                var coursesection = this.context?.StudentCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(y => y.StaffCoursesectionSchedule).ThenInclude(z => z.StaffMaster).Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StudentId == pageResult.StudentId && x.AcademicYear == pageResult.AcademicYear && (x.CourseSection.DurationBasedOnPeriod == false || ((pageResult.MarkingPeriodStartDate >= x.CourseSection.DurationStartDate && pageResult.MarkingPeriodStartDate <= x.CourseSection.DurationEndDate) && (pageResult.MarkingPeriodEndDate >= x.CourseSection.DurationStartDate && pageResult.MarkingPeriodEndDate <= x.CourseSection.DurationEndDate)))).ToList();

                if (coursesection != null)
                {
                    foreach (var section in coursesection)
                    {
                        StudentGradebookGradeDetails studentGrade = new();

                        var AssignmentTypeData = this.context?.AssignmentType.Include(x => x.Assignment).ThenInclude(x => x.GradebookGrades).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId == section.CourseSectionId && x.AcademicYear == pageResult.AcademicYear).ToList();

                        if (AssignmentTypeData?.Any() == true)
                        {
                            if (AssignmentTypeData.FirstOrDefault()!.PrgrsprdMarkingPeriodId != null)
                            {
                                var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                                if (progressPeriodsData != null)
                                {
                                    AssignmentTypeData = AssignmentTypeData?.Where(x => x.PrgrsprdMarkingPeriodId == progressPeriodsData.MarkingPeriodId).ToList();

                                }

                                else if (AssignmentTypeData?.FirstOrDefault()!.QtrMarkingPeriodId != null)
                                {
                                    var quartersData = this.context?.Quarters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                                    if (quartersData != null)
                                    {
                                        AssignmentTypeData = AssignmentTypeData.Where(x => x.QtrMarkingPeriodId == quartersData.MarkingPeriodId).ToList();

                                    }
                                }
                                else if (AssignmentTypeData?.FirstOrDefault()!.SmstrMarkingPeriodId != null)
                                {
                                    var semestersData = this.context?.Semesters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                                    if (semestersData != null)
                                    {
                                        AssignmentTypeData = AssignmentTypeData.Where(x => x.SmstrMarkingPeriodId == semestersData.MarkingPeriodId).ToList();

                                    }
                                }
                                else if (AssignmentTypeData?.FirstOrDefault()!.YrMarkingPeriodId != null)
                                {
                                    var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                                    if (yearsData != null)
                                    {
                                        AssignmentTypeData = AssignmentTypeData.Where(x => x.YrMarkingPeriodId == yearsData.MarkingPeriodId).ToList();
                                    }
                                }
                            }
                        }

                        var staffData = section.CourseSection.StaffCoursesectionSchedule.Where(x => x.IsDropped != true).FirstOrDefault();

                        if (AssignmentTypeData?.SelectMany(x => x.Assignment).Any() == true)
                        {
                            var gradebookData = AssignmentTypeData?.SelectMany(x => x.Assignment).SelectMany(x => x.GradebookGrades).Where(x => x.StudentId == pageResult.StudentId).FirstOrDefault();

                            studentGrade.Ungraded = AssignmentTypeData?.SelectMany(x => x.Assignment)?.Where(x => x.DueDate > DateTime.UtcNow.Date && x.GradebookGrades.Any(x => x.StudentId == pageResult.StudentId && x.AcademicYear == pageResult.AcademicYear) != true).Count();

                            studentGrade.StaffFirstName = staffData?.StaffMaster.FirstGivenName;

                            studentGrade.StaffMiddleName = staffData?.StaffMaster.MiddleName;

                            studentGrade.StaffLastName = staffData?.StaffMaster.LastFamilyName;

                            studentGrade.Percent = gradebookData?.Percentage;

                            studentGrade.Letter = gradebookData?.LetterGrade;

                            studentGrade.CourseSectionName = section.CourseSectionName;

                            studentGrade.CourseSectionId = section.CourseSectionId;

                            studentGrade.LowestGrade = AssignmentTypeData?.SelectMany(x => x.Assignment)?.SelectMany(x => x.GradebookGrades).OrderBy(x => Convert.ToDecimal(x.RunningAvg)).FirstOrDefault()?.RunningAvg;

                            studentGrade.HighestGrade = AssignmentTypeData?.SelectMany(x => x.Assignment)?.SelectMany(x => x.GradebookGrades).OrderByDescending(x => Convert.ToDecimal(x.RunningAvg)).FirstOrDefault()?.RunningAvg;
                        }
                        else
                        {
                            studentGrade.Ungraded = 0;

                            studentGrade.StaffFirstName = staffData?.StaffMaster.FirstGivenName;

                            studentGrade.StaffMiddleName = staffData?.StaffMaster.MiddleName;

                            studentGrade.StaffLastName = staffData?.StaffMaster.LastFamilyName;

                            studentGrade.Percent = "0";

                            studentGrade.Letter = "Not Graded";

                            studentGrade.CourseSectionName = section.CourseSectionName;

                            studentGrade.CourseSectionId = section.CourseSectionId;
                        }

                        studentGradeList.Add(studentGrade);
                    }
                }
                if (studentGradeList.Count() > 0)
                {
                    transactionIQ = studentGradeList.AsQueryable();

                    if (transactionIQ != null)
                    {
                        int? totalCount = transactionIQ.Count();
                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            studentGradebook.PageNumber = pageResult.PageNumber;
                            studentGradebook._pageSize = pageResult.PageSize;
                        }
                        studentGradebook.studentGradebookGradeDetails = transactionIQ.ToList();
                        studentGradebook.TotalCount = totalCount;
                    }
                    else
                    {
                        studentGradebook.TotalCount = 0;
                    }
                }
                else
                {
                    studentGradebook._failure = true;
                    studentGradebook._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                studentGradebook._failure = true;
                studentGradebook._message = ex.Message;
            }
            return studentGradebook;
        }
    }
}
