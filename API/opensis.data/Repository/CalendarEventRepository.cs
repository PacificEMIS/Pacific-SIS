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

            //int? eventId = Utility.GetMaxPK<CalendarEvents>(this.context, x => x.EventId);
            int? eventId = 1;

            var eventData = this.context?.CalendarEvents.Where(x => x.TenantId == calendarEvent.SchoolCalendarEvent!.TenantId /*&& x.SchoolId == calendarEvent.schoolCalendarEvent.SchoolId*/).OrderByDescending(x => x.EventId).FirstOrDefault();

            if (eventData != null)
            {
                eventId = eventData.EventId + 1;
            }

            calendarEvent.SchoolCalendarEvent!.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, calendarEvent.SchoolCalendarEvent.TenantId, calendarEvent.SchoolCalendarEvent.SchoolId);
            calendarEvent.SchoolCalendarEvent!.EventId = (int)eventId;
            calendarEvent.SchoolCalendarEvent.CreatedOn = DateTime.UtcNow;

            // When "Apply to All Schools" is requested, scope to the creator's authorized schools.
            // Super Admins keep the system-wide flag (visible to all schools).
            // School Admins get individual copies at each school they administer.
            if (calendarEvent.SchoolCalendarEvent.ApplicableToAllSchool == true
                || calendarEvent.SchoolCalendarEvent.SystemWideEvent == true)
            {
                var staffMember = this.context?.StaffMaster
                    .FirstOrDefault(s => s.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                        && s.StaffGuid.ToString() == calendarEvent.SchoolCalendarEvent.CreatedBy);

                if (staffMember != null)
                {
                    var activeAssignments = this.context?.StaffSchoolInfo
                        .Include(s => s.Membership)
                        .Where(s => s.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                            && s.StaffId == staffMember.StaffId
                            && (s.EndDate == null || s.EndDate.Value.Date >= DateTime.UtcNow.Date)
                            && s.StartDate <= DateTime.UtcNow.Date)
                        .ToList();

                    // Check Super Admin via StaffSchoolInfo first, then fall back to UserMaster
                    // (Super Admins may not have StaffSchoolInfo entries — they access all schools via UserMaster.Membership)
                    var isSuperAdmin = activeAssignments?.Any(s =>
                        string.Equals(s.Membership?.ProfileType, "Super Administrator", StringComparison.OrdinalIgnoreCase)) ?? false;

                    if (!isSuperAdmin)
                    {
                        isSuperAdmin = this.context?.UserMaster
                            .Include(u => u.Membership)
                            .Any(u => u.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                                && u.UserId == staffMember.StaffId
                                && (u.Membership!.IsSuperadmin == true
                                    || u.Membership.ProfileType!.ToLower() == "super administrator")) ?? false;
                    }

                    if (!isSuperAdmin)
                    {
                        // Other schools where this user is School Administrator
                        var targetSchoolIds = activeAssignments?
                            .Where(s => string.Equals(s.Membership?.ProfileType, "School Administrator", StringComparison.OrdinalIgnoreCase)
                                && s.SchoolAttachedId != null
                                && s.SchoolAttachedId != calendarEvent.SchoolCalendarEvent.SchoolId)
                            .Select(s => s.SchoolAttachedId!.Value)
                            .Distinct()
                            .ToList() ?? new List<int>();

                        // Resolve source membership profile types for visibility translation (events only)
                        List<string?>? sourceProfileTypes = null;
                        if (!string.IsNullOrEmpty(calendarEvent.SchoolCalendarEvent.VisibleToMembershipId)
                            && calendarEvent.SchoolCalendarEvent.IsHoliday != true)
                        {
                            var sourceMembershipIds = calendarEvent.SchoolCalendarEvent.VisibleToMembershipId
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToList();

                            sourceProfileTypes = this.context?.Membership
                                .Where(m => m.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                                    && m.SchoolId == calendarEvent.SchoolCalendarEvent.SchoolId
                                    && sourceMembershipIds.Contains(m.MembershipId))
                                .Select(m => m.ProfileType)
                                .ToList();
                        }

                        // Copies are standalone records, not system-wide
                        calendarEvent.SchoolCalendarEvent.ApplicableToAllSchool = false;
                        calendarEvent.SchoolCalendarEvent.SystemWideEvent = false;

                        foreach (var targetSchoolId in targetSchoolIds)
                        {
                            // Find the default calendar at the target school for the same academic year
                            var targetCalendar = this.context?.SchoolCalendars
                                .FirstOrDefault(c => c.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                                    && c.SchoolId == targetSchoolId
                                    && c.AcademicYear == calendarEvent.SchoolCalendarEvent.AcademicYear
                                    && c.DefaultCalender == true)
                                ?? this.context?.SchoolCalendars
                                    .FirstOrDefault(c => c.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                                        && c.SchoolId == targetSchoolId
                                        && c.AcademicYear == calendarEvent.SchoolCalendarEvent.AcademicYear);

                            if (targetCalendar == null) continue;

                            // Translate membership visibility to the target school
                            string? targetVisibleToMembershipId = calendarEvent.SchoolCalendarEvent.VisibleToMembershipId;
                            if (sourceProfileTypes != null)
                            {
                                var targetMembershipIds = this.context?.Membership
                                    .Where(m => m.TenantId == calendarEvent.SchoolCalendarEvent.TenantId
                                        && m.SchoolId == targetSchoolId
                                        && sourceProfileTypes.Contains(m.ProfileType))
                                    .Select(m => m.MembershipId)
                                    .ToList() ?? new List<int>();

                                targetVisibleToMembershipId = string.Join(",", targetMembershipIds);
                            }

                            eventId++;
                            var copy = new CalendarEvents
                            {
                                TenantId = calendarEvent.SchoolCalendarEvent.TenantId,
                                SchoolId = targetSchoolId,
                                CalendarId = targetCalendar.CalenderId,
                                EventId = (int)eventId,
                                AcademicYear = calendarEvent.SchoolCalendarEvent.AcademicYear,
                                StartDate = calendarEvent.SchoolCalendarEvent.StartDate,
                                EndDate = calendarEvent.SchoolCalendarEvent.EndDate,
                                SchoolDate = calendarEvent.SchoolCalendarEvent.SchoolDate,
                                Title = calendarEvent.SchoolCalendarEvent.Title,
                                Description = calendarEvent.SchoolCalendarEvent.Description,
                                VisibleToMembershipId = targetVisibleToMembershipId,
                                EventColor = calendarEvent.SchoolCalendarEvent.EventColor,
                                IsHoliday = calendarEvent.SchoolCalendarEvent.IsHoliday,
                                ApplicableToAllSchool = false,
                                SystemWideEvent = false,
                                CreatedBy = calendarEvent.SchoolCalendarEvent.CreatedBy,
                                CreatedOn = DateTime.UtcNow
                            };

                            this.context?.CalendarEvents.Add(copy);
                        }
                    }
                    // Super Admin: flags are left as-is (original behavior)
                }
            }

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
                    var eventList = this.context?.CalendarEvents.AsEnumerable().Where(x => (((x.TenantId == calendarEventList.TenantId /*&& x.SchoolId == calendarEventList.SchoolId*/ && x.AcademicYear == calendarEventList.AcademicYear && x.IsHoliday != true && ((calendarEventList.CalendarId!.Contains(x.CalendarId) /*&& x.SystemWideEvent == false */&& x.SchoolId == calendarEventList.SchoolId) || x.SystemWideEvent == true)) || x.TenantId == calendarEventList.TenantId && x.SystemWideEvent == true && x.IsHoliday != true && x.AcademicYear == calendarEventList.AcademicYear) && (String.Compare(membershipData!.ProfileType, "Super Administrator", true) == 0  || String.Compare(membershipData!.ProfileType, "School Administrator", true) == 0 || String.Compare(membershipData!.ProfileType, "Admin Assistant", true) == 0 || (x.VisibleToMembershipId ?? "").Contains((calendarEventList.MembershipId ?? 0).ToString()))) || (x.TenantId == calendarEventList.TenantId && x.IsHoliday==true && (x.SchoolId== calendarEventList.SchoolId||x.ApplicableToAllSchool==true))).OrderBy(x => x.Title).ToList();
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
