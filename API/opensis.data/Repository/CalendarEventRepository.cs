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

using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CalendarEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class CalendarEventRepository : ICalendarEventRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";

        public CalendarEventRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Calendar Event
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        public CalendarEventAddViewModel AddCalendarEvent(CalendarEventAddViewModel calendarEvent)
        {

            //int? eventId = Utility.GetMaxPK(this.context, new Func<CalendarEvents, int>(x => x.EventId));
            int? eventId = 1;

            var eventData = this.context?.CalendarEvents.Where(x => x.TenantId == calendarEvent.schoolCalendarEvent.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/).OrderByDescending(x => x.EventId).FirstOrDefault();

            if (eventData != null)
            {
                eventId = eventData.EventId + 1;
            }

            calendarEvent.schoolCalendarEvent.EventId = (int)eventId;
            calendarEvent.schoolCalendarEvent.CreatedOn = DateTime.UtcNow;
            this.context?.CalendarEvents.Add(calendarEvent.schoolCalendarEvent);
            this.context?.SaveChanges();
            calendarEvent._failure = false;
            calendarEvent._message = "Calendar Event Added Successfully";
            return calendarEvent;
        }

        /// <summary>
        /// Get Calender Event By Id
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        public CalendarEventAddViewModel ViewCalendarEvent(CalendarEventAddViewModel calendarEvent)
        {
            try
            {
                CalendarEventAddViewModel calendarEventAddViewModel = new CalendarEventAddViewModel();
                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.TenantId == calendarEvent.schoolCalendarEvent.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/ && x.EventId == calendarEvent.schoolCalendarEvent.EventId);
                if (calendarEventRepository != null)
                {
                    calendarEventAddViewModel.schoolCalendarEvent = calendarEventRepository;
                    calendarEventAddViewModel._tenantName = calendarEvent._tenantName;
                    calendarEventAddViewModel._failure = false;
                    return calendarEventAddViewModel;
                }
                else
                {
                    calendarEventAddViewModel._failure = true;
                    calendarEventAddViewModel._message = NORECORDFOUND;
                    return calendarEventAddViewModel;
                }
            }
            catch (Exception es)
            {

                throw;
            }
        }

        /// <summary>
        /// Update Calendar Event
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        public CalendarEventAddViewModel UpdateCalendarEvent(CalendarEventAddViewModel calendarEvent)
        {
            try
            {
                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.TenantId == calendarEvent.schoolCalendarEvent.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/ && x.EventId == calendarEvent.schoolCalendarEvent.EventId);

                if (calendarEventRepository!=null)
                {
                    calendarEvent.schoolCalendarEvent.UpdatedOn = DateTime.Now;
                    calendarEvent.schoolCalendarEvent.CreatedBy = calendarEventRepository.CreatedBy;
                    calendarEvent.schoolCalendarEvent.CreatedOn = calendarEventRepository.CreatedOn;
                    calendarEvent.schoolCalendarEvent.CalendarId = calendarEventRepository.CalendarId;
                    calendarEvent.schoolCalendarEvent.SchoolId = calendarEventRepository.SchoolId;
                    this.context.Entry(calendarEventRepository).CurrentValues.SetValues(calendarEvent.schoolCalendarEvent);
                    this.context?.SaveChanges();
                    calendarEvent._failure = false;
                    calendarEvent._message = "Calendar Event Updated Successfully";
                }
                else
                {
                    calendarEvent.schoolCalendarEvent = null;
                    calendarEvent._failure = false;
                    calendarEvent._message = NORECORDFOUND;
                }    
            }
            catch (Exception ex)
            {                
                calendarEvent._failure = true;
                calendarEvent._message = ex.Message;
                //return calendarEvent;
            }
            return calendarEvent;
        }

        /// <summary>
        /// Get All Calendar Event List
        /// </summary>
        /// <param name="calendarEventList"></param>
        /// <returns></returns>
        public CalendarEventListViewModel GetAllCalendarEvent(CalendarEventListViewModel calendarEventList)
        {
            CalendarEventListViewModel calendarEventListViewModel = new CalendarEventListViewModel();
            try
            {
                var membershipData = this.context?.Membership.FirstOrDefault(d => d.TenantId == calendarEventList.TenantId && d.SchoolId == calendarEventList.SchoolId && d.MembershipId == calendarEventList.MembershipId);

                if (membershipData != null)
                {
                    var eventList = this.context?.CalendarEvents.Where(x => ((x.TenantId == calendarEventList.TenantId /*&& x.SchoolId == calendarEventList.SchoolId*/ && x.AcademicYear == calendarEventList.AcademicYear && ((x.CalendarId == calendarEventList.CalendarId /*&& x.SystemWideEvent == false */&& x.SchoolId == calendarEventList.SchoolId) || x.SystemWideEvent == true)) || x.TenantId == calendarEventList.TenantId && x.SystemWideEvent == true && x.AcademicYear == calendarEventList.AcademicYear) && (membershipData.ProfileType.ToLower() == "Super Administrator".ToLower() || membershipData.ProfileType.ToLower() == "School Administrator".ToLower() || membershipData.ProfileType.ToLower() == "Admin Assistant".ToLower() || x.VisibleToMembershipId.Contains(calendarEventList.MembershipId.ToString()))).OrderBy(x => x.Title).Select(w => new CalendarEvents()
                    {
                        TenantId = w.TenantId,
                        SchoolId = w.SchoolId,
                        AcademicYear = w.AcademicYear,
                        CalendarId = w.CalendarId,
                        Description = w.Description,
                        EndDate = w.EndDate,
                        EventColor = w.EventColor,
                        EventId = w.EventId,
                        SchoolDate = w.SchoolDate,
                        StartDate = w.StartDate,
                        SystemWideEvent = w.SystemWideEvent,
                        Title = w.Title,
                        VisibleToMembershipId = w.VisibleToMembershipId,
                        CreatedOn = w.CreatedOn,
                        UpdatedOn = w.UpdatedOn,
                        CreatedBy = (w.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == calendarEventList.TenantId && u.EmailAddress == w.CreatedBy).Name : null,
                        UpdatedBy = (w.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == calendarEventList.TenantId && u.EmailAddress == w.UpdatedBy).Name : null
                    }).ToList();

                    calendarEventListViewModel.calendarEventList = eventList;
                    calendarEventListViewModel._tenantName = calendarEventList._tenantName;
                    calendarEventListViewModel._token = calendarEventList._token;

                    if (eventList.Count > 0)
                    {
                        calendarEventListViewModel._failure = false;
                    }
                    else
                    {
                        calendarEventListViewModel._failure = true;
                        calendarEventListViewModel._message = NORECORDFOUND;
                    }
                }

                
            }
            catch (Exception es)
            {
                calendarEventListViewModel._message = es.Message;
                calendarEventListViewModel._failure = true;
                calendarEventListViewModel._tenantName = calendarEventList._tenantName;
                calendarEventListViewModel._token = calendarEventList._token;
            }
            return calendarEventListViewModel;

        }

        /// <summary>
        /// Delete Calendar Event
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        public CalendarEventAddViewModel DeleteCalendarEvent(CalendarEventAddViewModel calendarEvent)
        {
            try
            {
                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.EventId == calendarEvent.schoolCalendarEvent.EventId && x.TenantId== calendarEvent.schoolCalendarEvent.TenantId);
                if (calendarEventRepository != null)
                {
                    this.context?.CalendarEvents.Remove(calendarEventRepository);
                    this.context?.SaveChanges();
                    calendarEvent._failure = false;
                    calendarEvent._message = "Calendar Event Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                calendarEvent._message = ex.Message;
                calendarEvent._failure = true;
            }
            return calendarEvent;
        }

    }
}
