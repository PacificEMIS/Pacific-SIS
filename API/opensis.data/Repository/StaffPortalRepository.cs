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
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class StaffPortalRepository : IStaffPortalRepository
    {
        private readonly CRMContext? context;
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
        public ScheduledCourseSectionViewModel MissingAttendanceListForCourseSection_old(PageResult pageResult)
        {

            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            IQueryable<CourseSectionViewList>? transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();

            if (pageResult is null)
            {
                return scheduledCourseSectionView;
            }
            //CourseFixedSchedule courseFixedSchedule = null;
            //List<CourseVariableSchedule> CourseVariableSchedule = new List<CourseVariableSchedule>();
            //List<CourseCalendarSchedule> courseCalendarSchedule = new List<CourseCalendarSchedule>();
            //List<CourseBlockSchedule> CourseBlockSchedule = new List<CourseBlockSchedule>();

            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();
            List<DateTime> holidayList = new List<DateTime>();
            try
            {
                var staffCourseSectionScheduleData = this.context?.StaffCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.StaffId == pageResult.StaffId && e.CourseSectionId == pageResult.CourseSectionId && e.IsDropped != true);


                if (staffCourseSectionScheduleData != null)
                {
                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.CourseSectionId == pageResult.CourseSectionId && (e.AttendanceTaken == true || e.TakeAttendanceCalendar == true || e.TakeAttendanceVariable == true || e.TakeAttendanceBlock == true)).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    if (allCourseSectionVewList != null && allCourseSectionVewList.Any())
                    {
                        var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewList.FirstOrDefault()!.CalendarId && (e.StartDate >= staffCourseSectionScheduleData.DurationStartDate && e.StartDate <= staffCourseSectionScheduleData.DurationEndDate || e.EndDate >= staffCourseSectionScheduleData.DurationStartDate && e.EndDate <= staffCourseSectionScheduleData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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

                        if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            foreach (var allCourseSectionVew in allCourseSectionVewList)
                            {
                                var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= staffCourseSectionScheduleData.DurationStartDate && v.BellScheduleDate <= staffCourseSectionScheduleData.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                if (bellScheduleList != null && bellScheduleList.Any())
                                {
                                    foreach (var bellSchedule in bellScheduleList)
                                    {
                                        var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == pageResult.SchoolId && b.TenantId == pageResult.TenantId && b.EffectiveStartDate!.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == pageResult.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                        if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                        {
                                            CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == pageResult.SchoolId && b.TenantId == pageResult.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == pageResult.CourseSectionId).ToList();

                                            if (staffAttendanceData?.Any() == false)
                                            {
                                                courseSectionBlock.AttendanceDate = bellSchedule.BellScheduleDate;
                                                courseSectionBlock.CourseId = staffCourseSectionScheduleData.CourseId;
                                                courseSectionBlock.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                                courseSectionBlock.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                                courseSectionBlock.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                                courseSectionBlock.AttendanceTaken = staffCourseSectionScheduleData.CourseSection.AttendanceTaken;
                                                courseSectionBlock.PeriodId = allCourseSectionVew.BlockPeriodId;
                                                courseSectionBlock.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVew.BlockPeriodId)!.PeriodTitle : null;
                                                courseSectionBlock.BlockId = allCourseSectionVew.BlockId;
                                                staffCoursesectionSchedule.Add(courseSectionBlock);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            List<DateTime> dateList = new List<DateTime>();
                            List<string> list = new List<string>();
                            //string[] meetingDays = { };
                            string[]? meetingDays = { };

                            DateTime start = (DateTime)staffCourseSectionScheduleData.DurationStartDate!;
                            DateTime end = (DateTime)staffCourseSectionScheduleData.DurationEndDate!;

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

                            meetingDays = staffCourseSectionScheduleData.MeetingDays!.ToLower().Split("|");

                            bool allDays = meetingDays == null || !meetingDays.Any();

                            dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                  .Select(offset => start.AddDays(offset))
                                                  .Where(d => allDays || meetingDays!.Contains(d.DayOfWeek.ToString().ToLower()))
                                                  .ToList();

                            if (dateList.Count > 0)
                            {
                                dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                //Remove Holiday
                                dateList = dateList.Where(x => !holidayList.Contains(x.Date)).ToList();
                            }

                            foreach (var date in dateList)
                            {
                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == pageResult.SchoolId && b.TenantId == pageResult.TenantId && b.EffectiveStartDate!.Value.Date <= date && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                {
                                    if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                    {
                                        CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId);

                                        if (staffAttendanceData?.Any() == false)
                                        {
                                            CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                            CourseSectionFixed.AttendanceDate = date;
                                            CourseSectionFixed.CourseId = staffCourseSectionScheduleData.CourseId;
                                            CourseSectionFixed.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                            CourseSectionFixed.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                            CourseSectionFixed.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                            CourseSectionFixed.AttendanceTaken = staffCourseSectionScheduleData.CourseSection.AttendanceTaken;
                                            CourseSectionFixed.PeriodId = allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId;
                                            CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId)!.PeriodTitle : null;
                                            CourseSectionFixed.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewList.FirstOrDefault()!.FixedPeriodId)!.BlockId : 0;

                                            staffCoursesectionSchedule.Add(CourseSectionFixed);
                                        }
                                    }
                                    if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                    {
                                        //var courseVariableScheduleData = allCourseSectionVewList.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));
                                        var courseVariableScheduleData = allCourseSectionVewList.Where(e => e.VarDay != null && e.VarDay.Contains(date.DayOfWeek.ToString()));

                                        if (courseVariableScheduleData != null)
                                        {
                                            foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                            {
                                                CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                if (staffAttendanceData?.Any() == false)
                                                {
                                                    CourseSectionVariable.AttendanceDate = date;
                                                    CourseSectionVariable.CourseId = staffCourseSectionScheduleData.CourseId;
                                                    CourseSectionVariable.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                                    CourseSectionVariable.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                                    CourseSectionVariable.AttendanceTaken = courseVariableSchedule.TakeAttendanceVariable;
                                                    CourseSectionVariable.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                                    CourseSectionVariable.PeriodId = courseVariableSchedule.VarPeriodId;
                                                    //CourseSectionVariable.PeriodTitle = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId).PeriodTitle : null;
                                                    //CourseSectionVariable.BlockId = (BlockPeriodList.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId).BlockId : 0;
                                                    CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId)!.PeriodTitle : null;
                                                    CourseSectionVariable.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId)!.BlockId : 0;
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
                            var calendarDataList = allCourseSectionVewList.Where(c => c.CalDate <= DateTime.Today.Date && !holidayList.Contains(c.CalDate.Value.Date));

                            if (calendarDataList.Count() > 0)
                            {
                                foreach (var calenderSchedule in calendarDataList)
                                {
                                    var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == pageResult.SchoolId && b.TenantId == pageResult.TenantId && b.EffectiveStartDate!.Value.Date <= calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId).ToList();  //check student's EffectiveStartDate in this course section 

                                    if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                    {
                                        CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionScheduleData.SchoolId && b.TenantId == staffCourseSectionScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                        if (staffAttendanceData?.Any() == false)
                                        {
                                            CourseSectioncalender.AttendanceDate = (DateTime)calenderSchedule.CalDate!;
                                            CourseSectioncalender.CourseId = staffCourseSectionScheduleData.CourseId;
                                            CourseSectioncalender.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                            CourseSectioncalender.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                            CourseSectioncalender.AttendanceTaken = calenderSchedule.TakeAttendanceCalendar;
                                            CourseSectioncalender.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                            CourseSectioncalender.PeriodId = calenderSchedule.CalPeriodId;
                                            CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)!.PeriodTitle : null;
                                            CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)!.BlockId : 0;
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
                        string? Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                        //transactionIQ = staffCoursesectionSchedule.Where(x => x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();

                        transactionIQ = staffCoursesectionSchedule.Where(x => x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || (x.PeriodTitle != null && x.PeriodTitle.Contains(Columnvalue))).AsQueryable();
                    }
                }
                transactionIQ = transactionIQ?.Distinct();

                if (pageResult.SortingModel != null)
                {
                    transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                }

                //int totalCount = transactionIQ.Count();
                int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }

                scheduledCourseSectionView.courseSectionViewList = transactionIQ != null ? transactionIQ.ToList() : new();
                scheduledCourseSectionView.MissingAttendanceCount = totalCount;
                scheduledCourseSectionView._pageSize = pageResult.PageSize;
                scheduledCourseSectionView.PageNumber = pageResult.PageNumber;
                scheduledCourseSectionView.TenantId = pageResult.TenantId;
                scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                scheduledCourseSectionView.StaffId = pageResult.StaffId;
                scheduledCourseSectionView.CourseSectionId = pageResult.CourseSectionId != null ? (int)pageResult.CourseSectionId : 0;
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

        public ScheduledCourseSectionViewModel MissingAttendanceListForCourseSection(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            IQueryable<CourseSectionViewList>? transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();

            if (pageResult is null)
            {
                return scheduledCourseSectionView;
            }

            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();
            List<DateTime> holidayList = new List<DateTime>();
            try
            {
                var staffCourseSectionScheduleData = this.context?.StaffCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.StaffId == pageResult.StaffId && e.CourseSectionId == pageResult.CourseSectionId && e.IsDropped != true);

                if (staffCourseSectionScheduleData != null)
                {
                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.CourseSectionId == pageResult.CourseSectionId && (e.AttendanceTaken == true || e.TakeAttendanceCalendar == true || e.TakeAttendanceVariable == true || e.TakeAttendanceBlock == true)).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    if (allCourseSectionVewList != null && allCourseSectionVewList.Any())
                    {
                        var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionScheduleData.TenantId && x.SchoolId == staffCourseSectionScheduleData.SchoolId && x.CourseSectionId == staffCourseSectionScheduleData.CourseSectionId && x.MissingAttendanceDate >= staffCourseSectionScheduleData.DurationStartDate && x.MissingAttendanceDate <= staffCourseSectionScheduleData.DurationEndDate).ToList();

                        if (studentMissingAttendanceData != null && studentMissingAttendanceData.Any() == true)
                        {
                            foreach (var studentMissingAttendance in studentMissingAttendanceData)
                            {
                                if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                {
                                    CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();
                                    CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                    CourseSectionFixed.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                    CourseSectionFixed.CourseId = staffCourseSectionScheduleData.CourseId;
                                    CourseSectionFixed.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                    CourseSectionFixed.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                    CourseSectionFixed.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                    CourseSectionFixed.AttendanceTaken = staffCourseSectionScheduleData.CourseSection.AttendanceTaken;
                                    CourseSectionFixed.PeriodId = studentMissingAttendance.PeriodId;
                                    CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.PeriodTitle : null;
                                    CourseSectionFixed.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)?.BlockId : 0;

                                    staffCoursesectionSchedule.Add(CourseSectionFixed);
                                }

                                else if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    CourseSectionViewList CourseSectionVariable = new CourseSectionViewList();

                                    CourseSectionVariable.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                    CourseSectionVariable.CourseId = staffCourseSectionScheduleData.CourseId;
                                    CourseSectionVariable.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                    CourseSectionVariable.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                    CourseSectionVariable.AttendanceTaken = allCourseSectionVewList.FirstOrDefault(e => e.VarPeriodId == studentMissingAttendance.PeriodId && e.VarDay!.ToLower().Contains(studentMissingAttendance.MissingAttendanceDate.Value.Date.DayOfWeek.ToString().ToLower()))?.TakeAttendanceVariable;
                                    CourseSectionVariable.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                    CourseSectionVariable.PeriodId = studentMissingAttendance.PeriodId;
                                    CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
                                    CourseSectionVariable.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.BlockId : 0;
                                    staffCoursesectionSchedule.Add(CourseSectionVariable);
                                }
                                else if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                    CourseSectioncalender.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                    CourseSectioncalender.CourseId = staffCourseSectionScheduleData.CourseId;
                                    CourseSectioncalender.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                    CourseSectioncalender.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                    CourseSectioncalender.AttendanceTaken = allCourseSectionVewList.FirstOrDefault(e => e.CalPeriodId == studentMissingAttendance.PeriodId && e.CalDate == studentMissingAttendance.MissingAttendanceDate.Value.Date)?.TakeAttendanceCalendar;
                                    CourseSectioncalender.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                    CourseSectioncalender.PeriodId = studentMissingAttendance.PeriodId;
                                    CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
                                    CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.BlockId : 0;
                                    staffCoursesectionSchedule.Add(CourseSectioncalender);
                                }
                                else if (staffCourseSectionScheduleData.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                    courseSectionBlock.AttendanceDate = (DateTime)studentMissingAttendance.MissingAttendanceDate!;
                                    courseSectionBlock.CourseId = staffCourseSectionScheduleData.CourseId;
                                    courseSectionBlock.CourseSectionId = staffCourseSectionScheduleData.CourseSectionId;
                                    courseSectionBlock.CourseSectionName = staffCourseSectionScheduleData.CourseSectionName;
                                    courseSectionBlock.AttendanceCategoryId = staffCourseSectionScheduleData.CourseSection.AttendanceCategoryId;
                                    courseSectionBlock.AttendanceTaken = allCourseSectionVewList.FirstOrDefault(e => e.BlockPeriodId == studentMissingAttendance.PeriodId && e.BlockId == studentMissingAttendance.BlockId)?.TakeAttendanceBlock;
                                    courseSectionBlock.PeriodId = studentMissingAttendance.PeriodId;
                                    courseSectionBlock.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
                                    courseSectionBlock.BlockId = studentMissingAttendance.BlockId;
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
                        string? Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                        transactionIQ = staffCoursesectionSchedule.Where(x => x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || (x.PeriodTitle != null && x.PeriodTitle.Contains(Columnvalue))).AsQueryable();
                    }
                }
                transactionIQ = transactionIQ?.Distinct();

                if (pageResult.SortingModel != null)
                {
                    transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                }

                //int totalCount = transactionIQ.Count();
                int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;
                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }

                scheduledCourseSectionView.courseSectionViewList = transactionIQ != null ? transactionIQ.ToList() : new();
                scheduledCourseSectionView.MissingAttendanceCount = totalCount;
                scheduledCourseSectionView._pageSize = pageResult.PageSize;
                scheduledCourseSectionView.PageNumber = pageResult.PageNumber;
                scheduledCourseSectionView.TenantId = pageResult.TenantId;
                scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                scheduledCourseSectionView.StaffId = pageResult.StaffId;
                scheduledCourseSectionView.CourseSectionId = pageResult.CourseSectionId != null ? (int)pageResult.CourseSectionId : 0;
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
                if (courseSectionUpdateViewModel.courseSection != null)
                {
                    var courseSectionData = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == courseSectionUpdateViewModel.courseSection.TenantId && x.SchoolId == courseSectionUpdateViewModel.courseSection.SchoolId && x.CourseId == courseSectionUpdateViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionUpdateViewModel.courseSection.CourseSectionId);
                    if (courseSectionData != null)
                    {
                        courseSectionData.OnlineClassroomUrl = courseSectionUpdateViewModel.courseSection.OnlineClassroomUrl;
                        courseSectionData.OnlineClassroomPassword = courseSectionUpdateViewModel.courseSection.OnlineClassroomPassword;
                        courseSectionData.UpdatedBy = courseSectionUpdateViewModel.courseSection.UpdatedBy;
                        courseSectionData.UpdatedOn = DateTime.UtcNow;
                        this.context?.SaveChanges();
                        courseSectionUpdateViewModel._failure = false;
                        courseSectionUpdateViewModel._message = "Updated Successfully";
                    }
                    else
                    {
                        courseSectionUpdateViewModel._failure = true;
                        courseSectionUpdateViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                courseSectionUpdateViewModel._failure = true;
                courseSectionUpdateViewModel._message = es.Message;
            }
            return courseSectionUpdateViewModel;
        }

        /// <summary>
        /// Get All Missing Attendance List For Staff
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetAllMissingAttendanceListForStaff_old(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
           // IQueryable<CourseSectionViewList> transactionIQ = null;
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();
            List<DateTime> holidayList = new List<DateTime>();

            try
            {
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(x => x.Course).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && s.IsDropped != true);

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    staffCourseSectionDataList = staffCourseSectionDataList?.Where(s => (pageResult.DobStartDate.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate.Value.Date) || (pageResult.DobEndDate.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate));
                }

                if (staffCourseSectionDataList != null && staffCourseSectionDataList.Any())
                {

                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.AcademicYear == pageResult.AcademicYear).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {

                            var allCourseSectionVewLists = allCourseSectionVewList?.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists != null && allCourseSectionVewLists.Any())
                            {
                                var CalendarEventsData = this.context?.CalendarEvents.Where(e => e.TenantId == pageResult.TenantId && e.CalendarId == allCourseSectionVewLists.FirstOrDefault()!.CalendarId && (e.StartDate >= staffCourseSectionData.DurationStartDate && e.StartDate <= staffCourseSectionData.DurationEndDate || e.EndDate >= staffCourseSectionData.DurationStartDate && e.EndDate <= staffCourseSectionData.DurationEndDate) && e.IsHoliday == true && (e.SchoolId == pageResult.SchoolId || e.ApplicableToAllSchool == true)).ToList();

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


                                if (staffCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    var blockScheduleData = allCourseSectionVewList?.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList();

                                    if (blockScheduleData != null && blockScheduleData.Any())
                                    {
                                        foreach (var blockSchedule in blockScheduleData)
                                        {
                                            var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.BlockId == blockSchedule.BlockId && v.BellScheduleDate >= staffCourseSectionData.DurationStartDate && v.BellScheduleDate <= staffCourseSectionData.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date && (!holidayList.Contains(v.BellScheduleDate))).ToList();

                                            if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                                            {
                                                bellScheduleList = bellScheduleList?.Where(s => pageResult.DobStartDate != null && s.BellScheduleDate >= pageResult.DobStartDate && s.BellScheduleDate <= pageResult.DobEndDate).ToList();
                                            }

                                            if (bellScheduleList != null && bellScheduleList.Any())
                                            {
                                                foreach (var bellSchedule in bellScheduleList)
                                                {
                                                    var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= bellSchedule.BellScheduleDate.Date && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section

                                                    if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                                    {
                                                        CourseSectionViewList courseSectionBlock = new CourseSectionViewList();

                                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == bellSchedule.BellScheduleDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == blockSchedule.BlockPeriodId).ToList();

                                                        if (staffAttendanceData?.Any() == false)
                                                        {
                                                            courseSectionBlock.AttendanceDate = bellSchedule.BellScheduleDate;
                                                            courseSectionBlock.CourseId = staffCourseSectionData.CourseId;
                                                            courseSectionBlock.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                            courseSectionBlock.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                            courseSectionBlock.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                                            courseSectionBlock.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
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
                                }

                                if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)" || staffCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                {
                                    List<DateTime> dateList = new List<DateTime>();
                                    List<string> list = new List<string>();
                                    string[] meetingDays = { };

                                    DateTime start = (DateTime)(pageResult.DobStartDate != null ? pageResult.DobStartDate : staffCourseSectionData.DurationStartDate!);
                                    DateTime end = (DateTime)(pageResult.DobEndDate != null ? pageResult.DobEndDate : staffCourseSectionData.DurationEndDate!);

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

                                        if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                        {
                                            if (staffCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                            {
                                                CourseSectionViewList CourseSectionFixed = new CourseSectionViewList();

                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId);

                                                if (staffAttendanceData?.Any() == false)
                                                {
                                                    CourseSectionFixed.ScheduleType = "Fixed Schedule";

                                                    CourseSectionFixed.AttendanceDate = date;
                                                    CourseSectionFixed.CourseId = staffCourseSectionData.CourseId;
                                                    CourseSectionFixed.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                    CourseSectionFixed.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                    CourseSectionFixed.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                                    CourseSectionFixed.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                                    CourseSectionFixed.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                                    CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == allCourseSectionVewLists.FirstOrDefault()!.FixedPeriodId)!.PeriodTitle : null;
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

                                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == date && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                        if (staffAttendanceData?.Any() == false)
                                                        {
                                                            CourseSectionVariable.AttendanceDate = date;
                                                            CourseSectionVariable.CourseId = staffCourseSectionData.CourseId;
                                                            CourseSectionVariable.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                            CourseSectionVariable.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                            CourseSectionVariable.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                                            CourseSectionVariable.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                                            CourseSectionVariable.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                                            CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == courseVariableSchedule.VarPeriodId)!.PeriodTitle : null;
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
                                        var calenderScheduleList = allCourseSectionVewLists.Where(c => c.CalDate >= staffCourseSectionData.DurationStartDate && c.CalDate <= staffCourseSectionData.DurationEndDate && c.CalDate <= DateTime.Today.Date && !holidayList.Contains(c.CalDate.Value.Date));

                                        if (calenderScheduleList.ToList().Count > 0)
                                        {
                                            foreach (var calenderSchedule in calenderScheduleList)
                                            {
                                                var StudentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.EffectiveStartDate.Value.Date <= calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId).ToList(); //check student's EffectiveStartDate in this course section 

                                                if (StudentCoursesectionScheduleData != null && StudentCoursesectionScheduleData.Any())
                                                {
                                                    CourseSectionViewList CourseSectioncalender = new CourseSectionViewList();

                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == staffCourseSectionData.SchoolId && b.TenantId == staffCourseSectionData.TenantId && b.AttendanceDate == calenderSchedule.CalDate && b.CourseSectionId == staffCourseSectionData.CourseSectionId && b.PeriodId == calenderSchedule.CalPeriodId);

                                                    if (staffAttendanceData?.Any() == false)
                                                    {
                                                        CourseSectioncalender.AttendanceDate = (DateTime)calenderSchedule.CalDate!;
                                                        CourseSectioncalender.CourseId = staffCourseSectionData.CourseId;
                                                        CourseSectioncalender.CourseSectionId = staffCourseSectionData.CourseSectionId;
                                                        CourseSectioncalender.CourseSectionName = staffCourseSectionData.CourseSectionName;
                                                        CourseSectioncalender.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                                        CourseSectioncalender.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                                        CourseSectioncalender.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                                        CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)!.PeriodTitle : null;
                                                        CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == calenderSchedule.CalPeriodId)!.BlockId : null;
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
                    }

                    //if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    //{
                    //    transactionIQ = staffCoursesectionSchedule.AsQueryable();
                    //}
                    //else
                    //{
                    //    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    //    {
                    //        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                    //        transactionIQ = staffCoursesectionSchedule.Where(x => x.StaffFirstGivenName != null && x.StaffFirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffMiddleName != null && x.StaffMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffLastFamilyName != null && x.StaffLastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.AttendanceDate != null && x.AttendanceDate.Date.ToString("yyyy-MM-dd").Contains(Columnvalue) || x.PeriodTitle.ToLower().Contains(Columnvalue.ToLower())).AsQueryable();
                    //    }
                    //}
                    //transactionIQ = transactionIQ.Distinct();

                    //if (pageResult.SortingModel != null)
                    //{
                    //    transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                    //}

                    //int totalCount = transactionIQ.Count();

                    //if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    //{
                    //    transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    //}

                    //scheduledCourseSectionView.courseSectionViewList = transactionIQ.ToList();               
                    //scheduledCourseSectionView.MissingAttendanceCount = totalCount;
                    //scheduledCourseSectionView._pageSize = pageResult.PageSize;
                    // scheduledCourseSectionView.PageNumber = pageResult.PageNumber;

                    staffCoursesectionSchedule = staffCoursesectionSchedule.OrderBy(x => x.AttendanceDate).ToList();
                    scheduledCourseSectionView.courseSectionViewList = staffCoursesectionSchedule;
                    scheduledCourseSectionView.TenantId = pageResult.TenantId;
                    scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                    scheduledCourseSectionView.StaffId = pageResult.StaffId;
                    scheduledCourseSectionView._failure = false;
                    scheduledCourseSectionView._tenantName = pageResult._tenantName;
                    scheduledCourseSectionView._token = pageResult._token;
                    scheduledCourseSectionView._userName = pageResult._userName;
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        public ScheduledCourseSectionViewModel GetAllMissingAttendanceListForStaff(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            List<CourseSectionViewList> staffCoursesectionSchedule = new List<CourseSectionViewList>();
            List<AllCourseSectionView>? allCourseSectionVewList = new List<AllCourseSectionView>();
            List<BlockPeriod>? BlockPeriodList = new List<BlockPeriod>();

            try
            {
                var staffCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(x => x.Course).Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.StaffId == pageResult.StaffId && s.IsDropped != true);

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    staffCourseSectionDataList = staffCourseSectionDataList?.Where(s => (pageResult.DobStartDate.Value.Date >= s.DurationStartDate!.Value.Date && pageResult.DobStartDate.Value.Date <= s.DurationEndDate!.Value.Date) || (pageResult.DobEndDate.Value.Date >= s.DurationStartDate.Value.Date && pageResult.DobEndDate.Value.Date <= s.DurationEndDate));
                }

                if (staffCourseSectionDataList != null && staffCourseSectionDataList.Any())
                {
                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.AcademicYear == pageResult.AcademicYear).ToList();

                    BlockPeriodList = this.context?.BlockPeriod.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId).ToList();

                    foreach (var staffCourseSectionData in staffCourseSectionDataList.ToList())
                    {
                        if (staffCourseSectionData.CourseSection.AcademicYear == pageResult.AcademicYear)
                        {
                            var allCourseSectionVewLists = allCourseSectionVewList?.Where(v => v.SchoolId == pageResult.SchoolId && v.TenantId == pageResult.TenantId && v.CourseId == staffCourseSectionData.CourseId && v.CourseSectionId == staffCourseSectionData.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (allCourseSectionVewLists != null && allCourseSectionVewLists.Any())
                            {
                                DateTime start = (DateTime)(pageResult.DobStartDate != null ? pageResult.DobStartDate : staffCourseSectionData.DurationStartDate!);
                                DateTime end = (DateTime)(pageResult.DobEndDate != null ? pageResult.DobEndDate : staffCourseSectionData.DurationEndDate!);

                                var studentMissingAttendanceData = this.context?.StudentMissingAttendances.Where(x => x.TenantId == staffCourseSectionData.TenantId && x.SchoolId == staffCourseSectionData.SchoolId && x.CourseSectionId == staffCourseSectionData.CourseSectionId && x.MissingAttendanceDate >= start && x.MissingAttendanceDate <= end).ToList();

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
                                            CourseSectionFixed.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                            CourseSectionFixed.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                            CourseSectionFixed.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            CourseSectionFixed.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
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
                                            CourseSectionVariable.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                            CourseSectionVariable.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                            CourseSectionVariable.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            CourseSectionVariable.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
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
                                            CourseSectioncalender.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                            CourseSectioncalender.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                            CourseSectioncalender.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            CourseSectioncalender.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
                                            CourseSectioncalender.BlockId = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.BlockId : null;
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
                                            courseSectionBlock.CourseTitle = staffCourseSectionData.CourseSection.Course.CourseTitle;
                                            courseSectionBlock.CourseGradeLevel = staffCourseSectionData.CourseSection.Course.CourseGradeLevel;
                                            courseSectionBlock.AttendanceCategoryId = staffCourseSectionData.CourseSection.AttendanceCategoryId != null ? staffCourseSectionData.CourseSection.AttendanceCategoryId : null;
                                            courseSectionBlock.PeriodTitle = (BlockPeriodList?.Count > 0) ? BlockPeriodList.FirstOrDefault(c => c.PeriodId == studentMissingAttendance.PeriodId)!.PeriodTitle : null;
                                            courseSectionBlock.BlockId = studentMissingAttendance.PeriodId;
                                            courseSectionBlock.PeriodId = studentMissingAttendance.PeriodId;
                                            courseSectionBlock.AttendanceTaken = allCourseSectionVewLists.FirstOrDefault(e => e.BlockPeriodId == studentMissingAttendance.PeriodId && e.BlockId == studentMissingAttendance.BlockId)?.TakeAttendanceBlock;

                                            staffCoursesectionSchedule.Add(courseSectionBlock);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    staffCoursesectionSchedule = staffCoursesectionSchedule.OrderBy(x => x.AttendanceDate).ToList();
                    scheduledCourseSectionView.courseSectionViewList = staffCoursesectionSchedule;
                    scheduledCourseSectionView.TenantId = pageResult.TenantId;
                    scheduledCourseSectionView.SchoolId = pageResult.SchoolId;
                    scheduledCourseSectionView.StaffId = pageResult.StaffId;
                    scheduledCourseSectionView._failure = false;
                    scheduledCourseSectionView._tenantName = pageResult._tenantName;
                    scheduledCourseSectionView._token = pageResult._token;
                    scheduledCourseSectionView._userName = pageResult._userName;
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
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
        /// Get Anomalous Grade 
        /// </summary>
        /// <param name="anomalousGradeViewModel"></param>
        /// <returns></returns>
        public AnomalousGradeViewModel GetAnomalousGrade(AnomalousGradeViewModel anomalousGradeViewModel)
        {
            List<StudentAnomalsGrade> transactionIQ = new List<StudentAnomalsGrade>();
            AnomalousGradeViewModel anomalousGrade = new AnomalousGradeViewModel();
            anomalousGrade.TenantId = anomalousGradeViewModel.TenantId;
            anomalousGrade.SchoolId = anomalousGradeViewModel.SchoolId;
            anomalousGrade.StaffId = anomalousGradeViewModel.StaffId;
            anomalousGrade.PageSize = anomalousGradeViewModel.PageSize;
            anomalousGrade.PageNumber = anomalousGradeViewModel.PageNumber;
            anomalousGrade._token = anomalousGradeViewModel._token;
            anomalousGrade._tenantName = anomalousGradeViewModel._tenantName;

            try
            {
                //fetch students whose gradebook grade is not entered yet/missing
                var staffCoursesectionScheduleData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(x => x.StudentCoursesectionSchedule).ThenInclude(s => s.StudentMaster).Where(x => x.TenantId == anomalousGradeViewModel.TenantId && x.SchoolId == anomalousGradeViewModel.SchoolId && x.StaffId == anomalousGradeViewModel.StaffId && x.IsDropped != true && (anomalousGradeViewModel.CourseSectionId == null || x.CourseSectionId == anomalousGradeViewModel.CourseSectionId)).ToList();

                if (staffCoursesectionScheduleData?.Any() == true)
                {
                    //this loop for staff's course sections
                    foreach (var staffCoursesectionSchedule in staffCoursesectionScheduleData)
                    {
                        var studentCoursesectionScheduleData = staffCoursesectionSchedule.CourseSection.StudentCoursesectionSchedule.Where(x => x.IsDropped != true && (anomalousGradeViewModel.StudentId == null || x.StudentId == anomalousGradeViewModel.StudentId) && (anomalousGradeViewModel.IncludeInactive == false || anomalousGradeViewModel.IncludeInactive == null ? x.StudentMaster.IsActive != false : true));

                        //this loop for students schedules in course section
                        foreach (var student in studentCoursesectionScheduleData)
                        {
                            var AssignmentData = this.context?.Assignment.Include(x => x.AssignmentType).Where(x => x.TenantId == anomalousGradeViewModel.TenantId && x.SchoolId == anomalousGradeViewModel.SchoolId && x.CourseSectionId == staffCoursesectionSchedule.CourseSectionId && (anomalousGradeViewModel.AssignmentTypeId == null || x.AssignmentTypeId == anomalousGradeViewModel.AssignmentTypeId) && (anomalousGradeViewModel.AssignmentId == null || x.AssignmentId == anomalousGradeViewModel.AssignmentId)).ToList();

                            if (AssignmentData?.Any() == true)
                            {
                                //this loop for assignments in course section
                                foreach (var assignment in AssignmentData)
                                {
                                    var gradebookData = this.context?.GradebookGrades.Where(x => x.SchoolId == anomalousGradeViewModel.SchoolId && x.TenantId == anomalousGradeViewModel.TenantId && x.AcademicYear == anomalousGradeViewModel.AcademicYear && x.StudentId == student.StudentId && x.CourseSectionId == staffCoursesectionSchedule.CourseSectionId && x.AssignmentId == assignment.AssignmentId);

                                    //check if studet's grade not enterd for this assignment then add in list.
                                    if (gradebookData?.Any() != true)
                                    {
                                        var studentAnomalsGradeData = new StudentAnomalsGrade { FirstGivenName = student.FirstGivenName, MiddleName = student.MiddleName, LastFamilyName = student.LastFamilyName, StudentId = student.StudentId, StudentInternalId = student.StudentInternalId, CourseSectionId = staffCoursesectionSchedule.CourseSectionId, AssignmentTypeId = assignment.AssignmentTypeId, AssignmentId = assignment.AssignmentId, Points = 0, AllowedMarks = "Missing", AssignmentTypeTitle = assignment.AssignmentType.Title, AssignmentTitle = assignment.AssignmentTitle, Comment = null };
                                        transactionIQ.Add(studentAnomalsGradeData);
                                    }
                                }
                            }
                        }
                    }
                }

                //fetch students whose gradebook grade is entered
                var gradebookGradesData = this.context?.GradebookGrades.Include(s => s.StudentMaster).Include(s => s.Assignment).ThenInclude(s => s.AssignmentType).Where(x => x.SchoolId == anomalousGradeViewModel.SchoolId && x.TenantId == anomalousGradeViewModel.TenantId && x.AcademicYear == anomalousGradeViewModel.AcademicYear && (anomalousGradeViewModel.StudentId == null || x.StudentId == anomalousGradeViewModel.StudentId) && (anomalousGradeViewModel.CourseSectionId == null || x.CourseSectionId == anomalousGradeViewModel.CourseSectionId) && (anomalousGradeViewModel.AssignmentTypeId == null || x.AssignmentTypeId == anomalousGradeViewModel.AssignmentTypeId) && (anomalousGradeViewModel.AssignmentId == null || x.AssignmentId == anomalousGradeViewModel.AssignmentId) && (anomalousGradeViewModel.IncludeInactive == false || anomalousGradeViewModel.IncludeInactive == null ? x.StudentMaster.IsActive != false : true))
                    .Select(s => new StudentAnomalsGrade { FirstGivenName = s.StudentMaster.FirstGivenName, MiddleName = s.StudentMaster.MiddleName, LastFamilyName = s.StudentMaster.LastFamilyName, StudentId = s.StudentMaster.StudentId, StudentInternalId = s.StudentMaster.StudentInternalId, Points = s.Assignment.Points, AllowedMarks = s.AllowedMarks, CourseSectionId = s.CourseSectionId, AssignmentTypeId = s.AssignmentTypeId, AssignmentId = s.AssignmentId, AssignmentTypeTitle = s.Assignment.AssignmentType.Title, AssignmentTitle = s.Assignment.AssignmentTitle, Comment = s.Comment }).ToList();

                if (gradebookGradesData?.Any() == true)
                {
                    gradebookGradesData = gradebookGradesData.Where(x => x.AllowedMarks == "*" || (x.AllowedMarks != "*" && Convert.ToDecimal(x.AllowedMarks) > Convert.ToDecimal(x.Points))).ToList(); //fetch those student's AllowedMarks * and got more than assignment point.
                    transactionIQ.AddRange(gradebookGradesData);
                }

                //this block for searching
                if (!string.IsNullOrEmpty(anomalousGradeViewModel.SearchValue))
                {
                    var searchValue = Regex.Replace(anomalousGradeViewModel.SearchValue, @"\s+", "");

                    transactionIQ = transactionIQ.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(searchValue.ToLower()) ||
                             x.MiddleName != null && x.MiddleName.ToLower().Contains(searchValue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(searchValue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(searchValue.ToLower()) || x.AssignmentTypeTitle != null && x.AssignmentTypeTitle.ToLower().Contains(searchValue.ToLower()) || x.AssignmentTitle != null && x.AssignmentTitle.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                if (transactionIQ?.Any() == true)
                {
                    anomalousGrade.TotalCount = transactionIQ.Count();
                    if (anomalousGradeViewModel.PageNumber > 0 && anomalousGradeViewModel.PageSize > 0)
                    {
                        anomalousGrade.studentAnomalsGrades = transactionIQ.Skip((anomalousGradeViewModel.PageNumber - 1) * anomalousGradeViewModel.PageSize).Take(anomalousGradeViewModel.PageSize).ToList();
                    }
                    else
                    {
                        anomalousGrade.studentAnomalsGrades = transactionIQ;
                    }
                }
                else
                {
                    anomalousGrade._failure = true;
                    anomalousGrade._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                anomalousGrade._failure = true;
                anomalousGrade._message = es.Message;
            }
            return anomalousGrade;
        }
    }
}
