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
using opensis.data.ViewModels.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public CalendarRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Add calendar
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        public CalendarAddViewModel AddCalendar(CalendarAddViewModel calendar)
        {
            //int? calenderId = Utility.GetMaxPK(this.context, new Func<SchoolCalendars, int>(x => x.CalenderId));
            int? calenderId = 1;

            var calenderData = this.context?.SchoolCalendars.Where(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId).OrderByDescending(x => x.CalenderId).FirstOrDefault();

            if (calenderData != null)
            {
                calenderId = calenderData.CalenderId + 1;
            }

            calendar.SchoolCalendar!.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, calendar.SchoolCalendar.TenantId, calendar.SchoolCalendar.SchoolId) ?? 0;
            calendar.SchoolCalendar!.CalenderId = (int)calenderId;
            calendar.SchoolCalendar.CreatedOn = DateTime.UtcNow;
            var checkDefaultCalendar = this.context?.SchoolCalendars.Where(x => x.AcademicYear == calendar.SchoolCalendar.AcademicYear && x.TenantId == calendar.SchoolCalendar.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId).ToList().Find(x => x.DefaultCalender == true);
            if (checkDefaultCalendar == null)
            {
                calendar.SchoolCalendar.DefaultCalender = true;
            }
            if (calendar.SchoolCalendar.DefaultCalender == true)
            {
                (from p in this.context?.SchoolCalendars
                 where p.TenantId == calendar.SchoolCalendar.TenantId && p.SchoolId == calendar.SchoolCalendar.SchoolId && p.AcademicYear == calendar.SchoolCalendar.AcademicYear
                 select p).ToList().ForEach(x => x.DefaultCalender = false);
            }

            this.context?.SchoolCalendars.Add(calendar.SchoolCalendar);
            //context!.Entry(calendar.SchoolCalendar.SchoolMaster).State = EntityState.Unchanged;
            this.context?.SaveChanges();
            calendar._failure = false;
            calendar._message = "Calendar Added Successfully";

            return calendar;
        }

        /// <summary>
        /// Get Calender By Id
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        public CalendarAddViewModel ViewCalendar(CalendarAddViewModel calendar)
        {
            CalendarAddViewModel calendarAddViewModel = new CalendarAddViewModel();
            try
            {
               
                var calendarRepository = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalenderId == calendar.SchoolCalendar.CalenderId);
                if (calendarRepository != null)
                {
                    calendar.SchoolCalendar = calendarRepository;
                    calendar._tenantName = calendar._tenantName;
                    calendar._failure = false;
                    return calendar;
                }
                else
                {
                    calendarAddViewModel._failure = true;
                    calendarAddViewModel._message = NORECORDFOUND;
                    return calendarAddViewModel;
                }
            }
            catch (Exception es)
            {

                calendarAddViewModel._failure = true;
                calendarAddViewModel._message = es.Message;
                return calendarAddViewModel;
            }
        }

        /// <summary>
        /// Update Calendar
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        public CalendarAddViewModel UpdateCalendar(CalendarAddViewModel calendar)
        {
            try
            {
                var calendarRepository = this.context?.SchoolCalendars.FirstOrDefault(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalenderId == calendar.SchoolCalendar.CalenderId);

                var enrollmentCalendar = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalenderId == calendar.SchoolCalendar.CalenderId && x.IsActive==true);

                if(enrollmentCalendar!= null)
                {
                    calendarRepository!.Title = calendar.SchoolCalendar!.Title;
                    calendar._message = "Calendar Updated Successfully";
                    this.context?.SaveChanges();
                    return calendar;
                }

                var courseSectionCalendar = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalendarId == calendar.SchoolCalendar.CalenderId && x.IsActive==true);

                if(courseSectionCalendar != null)
                {
                    calendarRepository!.Title = calendar.SchoolCalendar!.Title;
                    calendar._message = "Calendar Updated Successfully";
                    this.context?.SaveChanges();
                    return calendar;
                }

                var checkDefaultCalendar = this.context?.SchoolCalendars.Where(x => x.AcademicYear == calendar.SchoolCalendar!.AcademicYear && x.TenantId == calendar.SchoolCalendar.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalenderId != calendar.SchoolCalendar.CalenderId).ToList().Find(x => x.DefaultCalender == true);

                if (checkDefaultCalendar == null)
                {
                    calendar.SchoolCalendar!.DefaultCalender = true;
                }
                else
                {
                    var enrollmentDefaultCalendar = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar.SchoolId && x.CalenderId == checkDefaultCalendar.CalenderId);

                    if (enrollmentDefaultCalendar != null && calendar.SchoolCalendar!.DefaultCalender == true)
                    {
                        calendar.SchoolCalendar = null;
                        calendar._failure = true;
                        calendar._message = "Existing Default Calendar cannot be updated because it has already enrollment.";
                        return calendar;
                    }
                    //calendarRepository.DefaultCalender = calendar.schoolCalendar.DefaultCalender;                        
                }
                
                if (calendar.SchoolCalendar!.DefaultCalender == true )
                {
                    (from p in this.context?.SchoolCalendars
                     where p.CalenderId != calendarRepository!.CalenderId && p.AcademicYear == calendar.SchoolCalendar.AcademicYear && p.TenantId == calendar.SchoolCalendar.TenantId && p.SchoolId == calendar.SchoolCalendar.SchoolId
                     select p).ToList().ForEach(x => x.DefaultCalender = false);

                }
                calendar.SchoolCalendar.AcademicYear = calendarRepository!.AcademicYear;
                calendar.SchoolCalendar.UpdatedOn = DateTime.Now;
                calendar.SchoolCalendar.CreatedOn = calendarRepository!.CreatedOn;
                calendar.SchoolCalendar.CreatedBy = calendarRepository.CreatedBy;
                this.context!.Entry(calendarRepository).CurrentValues.SetValues(calendar.SchoolCalendar);
                this.context?.SaveChanges();

                this.context?.SaveChanges();
                calendar._failure = false;
                calendar._message = "Calendar Updated Successfully";
                return calendar;
            }
            catch (Exception ex)
            {
                calendar.SchoolCalendar = null;
                calendar._failure = true;
                calendar._message = ex.Message;
                return calendar;
            }
        }
        /// <summary>
        /// Get All Calendar
        /// </summary>
        /// <param name="calendarList"></param>
        /// <returns></returns>
        public CalendarListModel GetAllCalendar(CalendarListModel calendarList)
        {
            CalendarListModel calendarListModel = new CalendarListModel();
            try
            {
                var calendarRepository = this.context?.SchoolCalendars.Where(x => x.TenantId == calendarList.TenantId && x.SchoolId == calendarList.SchoolId && x.AcademicYear == calendarList.AcademicYear).OrderBy(x => x.Title).ToList();

               
                calendarListModel._tenantName = calendarList._tenantName;
                calendarListModel._token = calendarList._token;

                if (calendarRepository!=null && calendarRepository.Any())
                {
                    calendarListModel._failure = false;
                    calendarListModel.CalendarList = calendarRepository;
                }
                else
                {
                    calendarListModel._failure = true;
                    calendarListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                calendarListModel._message = es.Message;
                calendarListModel._failure = true;
                calendarListModel._tenantName = calendarList._tenantName;
                calendarListModel._token = calendarList._token;
            }
            return calendarListModel;

        }


        /// <summary>
        /// Delete Calendar
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        public CalendarAddViewModel DeleteCalendar(CalendarAddViewModel calendar)
        {
            try
            {
                var calendarRepository = this.context?.SchoolCalendars.Where(x => x.CalenderId == calendar.SchoolCalendar!.CalenderId && x.TenantId== calendar.SchoolCalendar!.TenantId && x.SchoolId == calendar.SchoolCalendar!.SchoolId).ToList().OrderBy(x => x.CalenderId).LastOrDefault();
                if (calendarRepository != null)
                {
                    var eventsExist = this.context?.CalendarEvents.FirstOrDefault(x => x.TenantId == calendarRepository.TenantId && x.SchoolId == calendarRepository.SchoolId && x.CalendarId == calendarRepository.CalenderId);

                    var enrollmentExist = this.context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == calendarRepository.TenantId && x.SchoolId == calendarRepository.SchoolId && x.CalenderId == calendarRepository.CalenderId);

                    if (eventsExist != null || enrollmentExist != null)
                    {
                        calendar._message = "Calendar cannot be deleted because it has an association.";
                        calendar._failure = true;
                    }
                    else
                    {
                        this.context?.SchoolCalendars.Remove(calendarRepository);
                        this.context?.SaveChanges();
                        calendar._failure = false;
                        calendar._message = "Calendar Deleted Successfully";
                    }
                }

                
            }
            catch (Exception ex)
            {

                calendar._failure = false;
                calendar._message = ex.Message;
            }
            return calendar;
        }
    }
}
