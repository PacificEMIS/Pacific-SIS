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
using opensis.data.ViewModels.StaffPortal;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StaffPortalRepository : IStaffPortalRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffPortalRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Missing Attendance List For Course Section
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel MissingAttendanceListForCourseSection(PageResult pageResult)
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
                var staffCourseSectionScheduleData = this.context?.StaffCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.StaffId == pageResult.StaffId && e.CourseSectionId == pageResult.CourseSectionId && e.IsDropped != true);


                if (staffCourseSectionScheduleData != null)
                {
                    allCourseSectionVewList = this.context.AllCourseSectionView.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.CourseSectionId == pageResult.CourseSectionId && (e.AttendanceTaken == true || e.TakeAttendanceCalendar == true || e.TakeAttendanceVariable == true || e.TakeAttendanceBlock == true)).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    if (allCourseSectionVewList.Count > 0)
                    {
                        if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            foreach (var allCourseSectionVew in allCourseSectionVewList)
                            {
                                var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= pageResult.DobStartDate && v.BellScheduleDate <= pageResult.DobEndDate && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                if (bellScheduleList.Count > 0)
                                {
                                    foreach (var bellSchedule in bellScheduleList)
                                    {
                                        CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == pageResult.SchoolId && b.TenantId == pageResult.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == pageResult.CourseSectionId).ToList();

                                        if (staffAttendanceData.Count() == 0)
                                        {
                                            courseSectionBlock.AttendanceDate = bellSchedule.BellScheduleDate;
                                            courseSectionBlock.CourseId = staffCourseSectionScheduleData.CourseId;
                                            courseSectionBlock.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                            courseSectionBlock.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                            courseSectionBlock.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                            courseSectionBlock.AttendanceTaken = staffCourseSectionScheduleData.CourseSection.AttendanceTaken;
                                            courseSectionBlock.PeriodId = allCourseSectionVew.BlockPeriodId;
                                            courseSectionBlock.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVew.BlockPeriodId).PeriodTitle : null;
                                            courseSectionBlock.BlockId = allCourseSectionVew.BlockId;
                                            staffCoursesectionSchedule.Add(courseSectionBlock);
                                        }
                                    }
                                }
                            }
                        }

                        if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            List<DateTime> dateList = new List<DateTime>();
                            List<string> list = new List<string>();
                            string[] meetingDays = { };


                            DateTime start = (DateTime)staffCourseSectionScheduleData.DurationStartDate;
                            DateTime end = (DateTime)staffCourseSectionScheduleData.DurationEndDate;

                            //if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            //{
                            //    list = allCourseSectionVewList.FirstOrDefault().FixedDays.Split("|").ToList();

                            //    if (list.Count > 0)
                            //    {
                            //        meetingDays = list.ToArray();
                            //    }
                            //}
                            //if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                            //{
                            //    meetingDays = allCourseSectionVewList.Select(c => c.VarDay).ToArray();
                            //}

                            meetingDays = staffCourseSectionScheduleData.MeetingDays.ToLower().Split("|");

                            bool allDays = meetingDays == null || !meetingDays.Any();

                            dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                  .Select(offset => start.AddDays(offset))
                                                  .Where(d => allDays || meetingDays.Contains(d.DayOfWeek.ToString().ToLower()))
                                                  .ToList();

                            if (dateList.Count > 0)
                            {
                                dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                            }

                            foreach (var date in dateList)
                            {
                                if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                {
                                    CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == allCourseSectionVewList.FirstOrDefault().FixedPeriodId);

                                    if (staffAttendanceData.Count() == 0)
                                    {
                                        CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                        CourseSectionFixed.AttendanceDate = date;
                                        CourseSectionFixed.CourseId = staffCourseSectionScheduleData.CourseId;
                                        CourseSectionFixed.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                        CourseSectionFixed.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                        CourseSectionFixed.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                        CourseSectionFixed.AttendanceTaken = staffCourseSectionScheduleData.CourseSection.AttendanceTaken;
                                        CourseSectionFixed.PeriodId = allCourseSectionVewList.FirstOrDefault().FixedPeriodId;
                                        CourseSectionFixed.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewList.FirstOrDefault().FixedPeriodId).PeriodTitle : null;

                                        staffCoursesectionSchedule.Add(CourseSectionFixed);
                                    }
                                }
                                if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    var courseVariableScheduleData = allCourseSectionVewList.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                    if (courseVariableScheduleData != null)
                                    {
                                        foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                        {
                                            CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                CourseSectionVariable.AttendanceDate = date;
                                                CourseSectionVariable.CourseId = staffCourseSectionScheduleData.CourseId;
                                                CourseSectionVariable.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                                CourseSectionVariable.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                                CourseSectionVariable.AttendanceTaken = courseVariableSchedule.TakeAttendanceVariable;
                                                CourseSectionVariable.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                                CourseSectionVariable.PeriodId = courseVariableSchedule.VarPeriodId;
                                                CourseSectionVariable.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId).PeriodTitle : null;

                                                staffCoursesectionSchedule.Add(CourseSectionVariable);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            var calendarDataList = allCourseSectionVewList.Where(c => c.CalDate <= DateTime.Today.Date);

                            if (calendarDataList.Count() > 0)
                            {
                                foreach (var calenderSchedule in calendarDataList)
                                {
                                    CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                    if (staffAttendanceData.Count() == 0)
                                    {
                                        CourseSectioncalender.AttendanceDate = (DateTime)calenderSchedule.CalDate;
                                        CourseSectioncalender.CourseId = staffCourseSectionScheduleData.CourseId;
                                        CourseSectioncalender.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                        CourseSectioncalender.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                        CourseSectioncalender.AttendanceTaken = calenderSchedule.TakeAttendanceCalendar;
                                        CourseSectioncalender.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                        CourseSectioncalender.PeriodId = calenderSchedule.CalPeriodId;
                                        CourseSectioncalender.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId).PeriodTitle : null;

                                        staffCoursesectionSchedule.Add(CourseSectioncalender);
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

                        transactionIQ = staffCoursesectionSchedule.Where(x => x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate != null && x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
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
                scheduledCourseSectionView.CourseSectionId = (int)pageResult.CourseSectionId;
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
        /// Update Online Class Room URL & Password In Course Section
        /// </summary>
        /// <param name="courseSectionUpdateViewModel"></param>
        /// <returns></returns>
        public CourseSectionUpdateViewModel UpdateOnlineClassRoomURLInCourseSection(CourseSectionUpdateViewModel courseSectionUpdateViewModel)
        {
            try
            {
                var courseSectionData = this.context.CourseSection.FirstOrDefault(x => x.TenantId == courseSectionUpdateViewModel.courseSection.TenantId && x.SchoolId == courseSectionUpdateViewModel.courseSection.SchoolId && x.CourseId == courseSectionUpdateViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionUpdateViewModel.courseSection.CourseSectionId);
                if (courseSectionData != null)
                {
                    courseSectionData.OnlineClassroomUrl = courseSectionUpdateViewModel.courseSection.OnlineClassroomUrl;
                    courseSectionData.OnlineClassroomPassword = courseSectionUpdateViewModel.courseSection.OnlineClassroomPassword;
                    courseSectionData.UpdatedBy = courseSectionUpdateViewModel.courseSection.UpdatedBy;
                    courseSectionData.UpdatedOn = DateTime.UtcNow;
                    this.context.SaveChanges();
                    courseSectionUpdateViewModel._failure = false;
                    courseSectionUpdateViewModel._message = "Updated Successfully";
                }
                else
                {
                    courseSectionUpdateViewModel._failure = true;
                    courseSectionUpdateViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseSectionUpdateViewModel._failure = true;
                courseSectionUpdateViewModel._message = es.Message;
            }
            return courseSectionUpdateViewModel;
        }        
    }
}
