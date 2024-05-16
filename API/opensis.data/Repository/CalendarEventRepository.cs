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
        private readonly CRMContext? context;
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

            var eventData = this.context?.CalendarEvents.Where(x => x.TenantId == calendarEvent.SchoolCalendarEvent!.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/).OrderByDescending(x => x.EventId).FirstOrDefault();

            if (eventData != null)
            {
                eventId = eventData.EventId + 1;
            }

            calendarEvent.SchoolCalendarEvent!.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, calendarEvent.SchoolCalendarEvent.TenantId, calendarEvent.SchoolCalendarEvent.SchoolId);
            calendarEvent.SchoolCalendarEvent!.EventId = (int)eventId;
            calendarEvent.SchoolCalendarEvent.CreatedOn = DateTime.UtcNow;
            this.context?.CalendarEvents.Add(calendarEvent.SchoolCalendarEvent);
            this.context?.SaveChanges();
            calendarEvent._failure = false;
            calendarEvent._message = "Calendar Event added successfully";
            return calendarEvent;
        }

        /// <summary>
        /// Get Calender Event By Id
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        public CalendarEventAddViewModel ViewCalendarEvent(CalendarEventAddViewModel calendarEvent)
        {
            CalendarEventAddViewModel calendarEventAddViewModel = new CalendarEventAddViewModel();
            try
            {
               
                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.TenantId == calendarEvent.SchoolCalendarEvent!.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/ && x.EventId == calendarEvent.SchoolCalendarEvent.EventId);
                if (calendarEventRepository != null)
                {
                    calendarEventAddViewModel.SchoolCalendarEvent = calendarEventRepository;
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
                calendarEventAddViewModel._failure = true;
                calendarEventAddViewModel._message = es.Message;
                return calendarEventAddViewModel;
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
                if (calendarEvent.ProfileType == null || calendarEvent.ProfileType == "")
                {
                    calendarEvent._failure = true;
                    calendarEvent._message = "Please pass profile type";

                    return calendarEvent;
                }

                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.TenantId == calendarEvent.SchoolCalendarEvent!.TenantId && x.EventId == calendarEvent.SchoolCalendarEvent.EventId);

                if (calendarEventRepository != null)
                {
                    if (calendarEvent.ProfileType != null && calendarEvent.ProfileType.ToLower() != "Super Administrator".ToLower())
                    {
                        calendarEvent.SchoolCalendarEvent!.ApplicableToAllSchool = calendarEventRepository.ApplicableToAllSchool;
                        calendarEvent.SchoolCalendarEvent!.SystemWideEvent = calendarEventRepository.SystemWideEvent;
                    }
                    calendarEvent.SchoolCalendarEvent!.AcademicYear = calendarEventRepository.AcademicYear;
                    calendarEvent.SchoolCalendarEvent!.UpdatedOn = DateTime.Now;
                    calendarEvent.SchoolCalendarEvent.CreatedBy = calendarEventRepository.CreatedBy;
                    calendarEvent.SchoolCalendarEvent.CreatedOn = calendarEventRepository.CreatedOn;
                    calendarEvent.SchoolCalendarEvent.CalendarId = calendarEventRepository.CalendarId;
                    calendarEvent.SchoolCalendarEvent.SchoolId = calendarEventRepository.SchoolId;
                    context?.Entry(calendarEventRepository).CurrentValues.SetValues(calendarEvent.SchoolCalendarEvent);
                    this.context?.SaveChanges();
                    calendarEvent._failure = false;
                    if (calendarEvent.SchoolCalendarEvent.IsHoliday == true)
                    {
                        calendarEvent._message = "Calendar holiday updated successfully";
                    }
                    else
                    {
                        calendarEvent._message = "Calendar event updated successfully";
                    }
                }
                else
                {
                    calendarEvent.SchoolCalendarEvent = null;
                    calendarEvent._failure = false;
                    calendarEvent._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                calendarEvent._failure = true;
                calendarEvent._message = ex.Message;
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
                    var eventList = this.context?.CalendarEvents.AsEnumerable().Where(x => (((x.TenantId == calendarEventList.TenantId /*&& x.SchoolId == calendarEventList.SchoolId*/ && x.AcademicYear == calendarEventList.AcademicYear && ((calendarEventList.CalendarId!.Contains(x.CalendarId) /*&& x.SystemWideEvent == false */&& x.SchoolId == calendarEventList.SchoolId) || x.SystemWideEvent == true)) || x.TenantId == calendarEventList.TenantId && x.SystemWideEvent == true && x.AcademicYear == calendarEventList.AcademicYear) && (String.Compare(membershipData!.ProfileType, "Super Administrator", true) == 0  || String.Compare(membershipData!.ProfileType, "School Administrator", true) == 0 || String.Compare(membershipData!.ProfileType, "Admin Assistant", true) == 0 || (x.VisibleToMembershipId ?? "").Contains((calendarEventList.MembershipId ?? 0).ToString()))) || (x.TenantId == calendarEventList.TenantId && x.IsHoliday==true && (x.SchoolId== calendarEventList.SchoolId||x.ApplicableToAllSchool==true))).OrderBy(x => x.Title).ToList();
                    if(eventList!=null && eventList.Any())
                    {
                        calendarEventListViewModel.CalendarEventList = eventList;
                        calendarEventListViewModel._failure = false;
                    }
                    else
                    {
                        calendarEventListViewModel._failure = true;
                        calendarEventListViewModel._message = NORECORDFOUND;
                    }
                    calendarEventListViewModel._tenantName = calendarEventList._tenantName;
                    calendarEventListViewModel._token = calendarEventList._token;

                   
                   
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
                var calendarEventRepository = this.context?.CalendarEvents.FirstOrDefault(x => x.EventId == calendarEvent.SchoolCalendarEvent!.EventId && x.TenantId== calendarEvent.SchoolCalendarEvent.TenantId);
                if (calendarEventRepository != null)
                {
                    this.context?.CalendarEvents.Remove(calendarEventRepository);
                    this.context?.SaveChanges();
                    calendarEvent._failure = false;
                    calendarEvent._message = "Calendar Event deleted successfullyy";
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
