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
using opensis.data.ViewModels.SuperAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace opensis.data.Repository
{
    /// <summary>
    /// Manages the tenant-wide Super Administrator role.
    ///
    /// A Super Administrator is a staff_master row whose profile is
    /// "Super Administrator" and whose user_master login points at the
    /// membership flagged is_superadmin (membership_id 1 in every school).
    /// The membership is what grants access (every school visible, every
    /// permission); the staff profile is what the staff module keys on.
    /// Every write in this repository keeps the two in sync.
    ///
    /// Super Administrators normally have no staff_school_info rows: they
    /// are tenant-wide, not attached to a school. A staff promoted from a
    /// school role keeps their attachment rows as history.
    ///
    /// Callers are identified by CallerEmail, which the service layer fills
    /// from the validated token, never from the request body.
    /// </summary>
    public class SuperAdministratorRepository : ISuperAdministratorRepository
    {
        private readonly CRMContext? context;

        public const string SUPER_ADMINISTRATOR = "Super Administrator";
        public static readonly string[] DemotionProfileTypes = { "School Administrator", "Admin Assistant", "Teacher", "Homeroom Teacher" };

        public const string NOT_AUTHORISED = "Only an active Super Administrator can manage Super Administrators.";
        public const string NOT_SUPER_ADMIN = "This staff member is not a Super Administrator.";
        public const string SELF_ACTION = "You cannot change your own Super Administrator account. Ask another Super Administrator.";
        public const string LAST_ACTIVE = "At least one other active Super Administrator must remain.";

        public SuperAdministratorRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        #region helpers

        private static bool IsSuperAdminMembership(Membership? membership) =>
            membership != null && (membership.IsSuperadmin || string.Equals(membership.ProfileType, SUPER_ADMINISTRATOR, StringComparison.OrdinalIgnoreCase));

        private static bool IsSuperAdminProfile(string? profile) =>
            string.Equals((profile ?? "").Trim(), SUPER_ADMINISTRATOR, StringComparison.OrdinalIgnoreCase);

        private static bool SameEmail(string? a, string? b) =>
            !string.IsNullOrWhiteSpace(a) && string.Equals(a.Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

        private static string FullName(StaffMaster staff) =>
            string.Join(" ", new[] { staff.FirstGivenName, staff.MiddleName, staff.LastFamilyName }.Where(s => !string.IsNullOrWhiteSpace(s)));

        private static void Fail(ViewModels.CommonFields model, string message)
        {
            model._failure = true;
            model._message = message;
        }

        /// <summary>Every login row (one per attached school) that carries the super admin membership.</summary>
        private List<UserMaster> SuperAdminUsers(Guid tenantId) =>
            this.context!.UserMaster.Include(x => x.Membership)
                .Where(x => x.TenantId == tenantId && (x.Membership.IsSuperadmin || x.Membership.ProfileType == SUPER_ADMINISTRATOR))
                .ToList();

        private bool CallerIsActiveSuperAdmin(Guid tenantId, string? callerEmail)
        {
            if (string.IsNullOrWhiteSpace(callerEmail))
            {
                return false;
            }
            return this.context!.UserMaster.Include(x => x.Membership)
                .Where(x => x.TenantId == tenantId && x.EmailAddress == callerEmail && x.IsActive != false)
                .AsEnumerable()
                .Any(x => IsSuperAdminMembership(x.Membership));
        }

        private List<UserMaster> UsersByEmail(Guid tenantId, string? email) =>
            string.IsNullOrWhiteSpace(email)
                ? new List<UserMaster>()
                : this.context!.UserMaster.Include(x => x.Membership).Where(x => x.TenantId == tenantId && x.EmailAddress == email).ToList();

        /// <summary>The staff row when it is a Super Administrator by profile or by login membership, else null.</summary>
        private StaffMaster? FindSuperAdminStaff(Guid tenantId, int staffId)
        {
            var staff = this.context!.StaffMaster.FirstOrDefault(x => x.TenantId == tenantId && x.StaffId == staffId);
            if (staff == null)
            {
                return null;
            }
            if (IsSuperAdminProfile(staff.Profile))
            {
                return staff;
            }
            return UsersByEmail(tenantId, staff.LoginEmailAddress).Any(u => IsSuperAdminMembership(u.Membership)) ? staff : null;
        }

        /// <summary>
        /// True when at least one Super Administrator login other than
        /// <paramref name="excludeStaffId"/> can still sign in. Defensive: the
        /// caller is always such a login and never the target, so through the
        /// API this cannot fail, but it keeps the invariant explicit.
        /// </summary>
        private bool AnotherActiveSuperAdminRemains(Guid tenantId, int excludeStaffId) =>
            SuperAdminUsers(tenantId).Any(u => u.IsActive != false && u.UserId != excludeStaffId);

        private int? SuperAdminMembershipId(Guid tenantId, int schoolId) =>
            this.context!.Membership
                .Where(x => x.TenantId == tenantId && x.SchoolId == schoolId && (x.IsSuperadmin || x.ProfileType == SUPER_ADMINISTRATOR))
                .OrderBy(x => x.MembershipId)
                .Select(x => (int?)x.MembershipId)
                .FirstOrDefault();

        private int NextStaffId(Guid tenantId)
        {
            var staff = this.context!.StaffMaster.Where(x => x.TenantId == tenantId);
            return staff.Any() ? staff.Max(x => x.StaffId) + 1 : 1;
        }

        private void ExpireSessions(Guid tenantId, string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }
            foreach (var session in this.context!.LoginSession.Where(x => x.TenantId == tenantId && x.EmailAddress == email && x.IsExpired != true).ToList())
            {
                session.IsExpired = true;
            }
        }

        #endregion

        public SuperAdministratorListViewModel GetSuperAdministrators(SuperAdministratorListViewModel model)
        {
            var result = new SuperAdministratorListViewModel
            {
                TenantId = model.TenantId,
                SchoolId = model.SchoolId,
                _tenantName = model._tenantName,
                _token = model._token,
                _userName = model._userName
            };
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(result, NOT_AUTHORISED);
                    return result;
                }

                // Union of "profile says super admin" and "login says super admin",
                // so a record that drifted out of sync still shows up.
                var superAdminUsers = SuperAdminUsers(tenantId);
                var staffById = this.context!.StaffMaster
                    .Where(x => x.TenantId == tenantId && x.Profile != null && x.Profile.ToLower() == "super administrator")
                    .ToList()
                    .ToDictionary(x => x.StaffId);
                var missingIds = superAdminUsers.Select(u => u.UserId).Distinct().Where(id => !staffById.ContainsKey(id)).ToList();
                if (missingIds.Any())
                {
                    foreach (var staff in this.context.StaffMaster.Where(x => x.TenantId == tenantId && missingIds.Contains(x.StaffId)).ToList())
                    {
                        staffById[staff.StaffId] = staff;
                    }
                }

                var staffIds = staffById.Keys.ToList();
                var schools = this.context.SchoolMaster.Where(x => x.TenantId == tenantId)
                    .Select(x => new { x.SchoolId, x.SchoolName }).ToList()
                    .ToDictionary(x => x.SchoolId, x => x.SchoolName);
                var schoolInfos = this.context.StaffSchoolInfo
                    .Where(x => x.TenantId == tenantId && x.StaffId != null && staffIds.Contains(x.StaffId.Value)).ToList();
                var emails = staffById.Values.Select(s => s.LoginEmailAddress).Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e!).ToList();
                var usersByEmail = this.context.UserMaster.Where(x => x.TenantId == tenantId && emails.Contains(x.EmailAddress)).ToList()
                    .GroupBy(x => x.EmailAddress, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
                var lastLogins = this.context.UserAccessLog
                    .Where(x => x.TenantId == tenantId && x.LoginStatus == true && emails.Contains(x.Emailaddress))
                    .GroupBy(x => x.Emailaddress)
                    .Select(g => new { Email = g.Key, Last = g.Max(x => x.CreatedOn) }).ToList()
                    .ToDictionary(x => x.Email, x => x.Last, StringComparer.OrdinalIgnoreCase);
                var today = DateTime.UtcNow.Date;

                foreach (var staff in staffById.Values.OrderBy(s => s.LastFamilyName).ThenBy(s => s.FirstGivenName))
                {
                    usersByEmail.TryGetValue(staff.LoginEmailAddress ?? "", out var users);
                    var hasLogin = users != null && users.Any();
                    var infos = schoolInfos.Where(i => i.StaffId == staff.StaffId).ToList();
                    var current = infos.Where(i => i.EndDate == null || i.EndDate >= today).ToList();
                    var home = current.FirstOrDefault(i => i.SchoolId == i.SchoolAttachedId) ?? current.FirstOrDefault();
                    schools.TryGetValue(staff.SchoolId, out var schoolName);
                    lastLogins.TryGetValue(staff.LoginEmailAddress ?? "", out var lastLogin);

                    result.SuperAdministrators.Add(new SuperAdministratorViewModel
                    {
                        StaffId = staff.StaffId,
                        SchoolId = staff.SchoolId,
                        SchoolName = schoolName,
                        StaffGuid = staff.StaffGuid,
                        FirstGivenName = staff.FirstGivenName,
                        MiddleName = staff.MiddleName,
                        LastFamilyName = staff.LastFamilyName,
                        LoginEmailAddress = staff.LoginEmailAddress,
                        HasLogin = hasLogin,
                        // Status means "can sign in": the login row decides that. The
                        // staff row's own disable flag is reported separately because
                        // seeded data has staff rows disabled under enabled logins.
                        IsActive = hasLogin ? users!.All(u => u.IsActive != false) : staff.IsActive != false,
                        StaffRecordDisabled = staff.IsActive == false,
                        IsSelf = SameEmail(staff.LoginEmailAddress, model.CallerEmail),
                        HasSchoolAttachments = infos.Any(),
                        PreviousSchoolId = home?.SchoolAttachedId,
                        PreviousProfile = home?.Profile,
                        LastLoginOn = lastLogin,
                        CreatedOn = staff.CreatedOn
                    });
                }
                result._failure = false;
            }
            catch (Exception ex)
            {
                Fail(result, ex.Message);
            }
            return result;
        }

        public SuperAdministratorAddViewModel AddSuperAdministrator(SuperAdministratorAddViewModel model)
        {
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(model, NOT_AUTHORISED);
                    return model;
                }

                var first = (model.FirstGivenName ?? "").Trim();
                var middle = (model.MiddleName ?? "").Trim();
                var last = (model.LastFamilyName ?? "").Trim();
                var email = (model.LoginEmailAddress ?? "").Trim();
                if (first.Length == 0 || last.Length == 0)
                {
                    Fail(model, "First name and last name are required.");
                    return model;
                }
                if (email.Length == 0 || !email.Contains('@'))
                {
                    Fail(model, "A valid login email address is required.");
                    return model;
                }
                if (string.IsNullOrWhiteSpace(model.PasswordHash))
                {
                    Fail(model, "A password is required.");
                    return model;
                }
                string password;
                try
                {
                    password = Utility.Decrypt(model.PasswordHash);
                }
                catch
                {
                    Fail(model, "The password could not be read.");
                    return model;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    Fail(model, "A password is required.");
                    return model;
                }
                if (this.context!.UserMaster.Any(x => x.TenantId == tenantId && x.EmailAddress == email)
                    || this.context.StaffMaster.Any(x => x.TenantId == tenantId && x.LoginEmailAddress == email))
                {
                    Fail(model, "A login with this email address already exists.");
                    return model;
                }

                var schoolId = model.SchoolId ?? 0;
                var school = this.context.SchoolMaster.FirstOrDefault(x => x.TenantId == tenantId && x.SchoolId == schoolId);
                if (school == null)
                {
                    Fail(model, "The current school could not be found.");
                    return model;
                }
                var membershipId = SuperAdminMembershipId(tenantId, schoolId);
                if (membershipId == null)
                {
                    Fail(model, "No Super Administrator membership exists for the current school.");
                    return model;
                }

                var now = DateTime.UtcNow;
                var staffId = NextStaffId(tenantId);
                var staff = new StaffMaster
                {
                    TenantId = tenantId,
                    SchoolId = schoolId,
                    StaffId = staffId,
                    StaffInternalId = staffId.ToString(),
                    StaffGuid = Guid.NewGuid(),
                    FirstGivenName = first,
                    MiddleName = middle.Length == 0 ? null : middle,
                    LastFamilyName = last,
                    LoginEmailAddress = email,
                    PortalAccess = true,
                    Profile = SUPER_ADMINISTRATOR,
                    JobTitle = SUPER_ADMINISTRATOR,
                    IsActive = true,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = now
                };
                var user = new UserMaster
                {
                    TenantId = tenantId,
                    SchoolId = schoolId,
                    UserId = staffId,
                    Name = first,
                    EmailAddress = email,
                    PasswordHash = Utility.GetHashedPassword(password),
                    LangId = 1,
                    MembershipId = membershipId.Value,
                    IsActive = true,
                    LastUsedSchoolId = schoolId,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = now
                };

                using (var transaction = this.context.Database.BeginTransaction())
                {
                    this.context.StaffMaster.Add(staff);
                    this.context.UserMaster.Add(user);
                    this.context.SaveChanges();
                    transaction.Commit();
                }

                model.StaffId = staffId;
                model._failure = false;
                model._message = "Super Administrator added successfully.";
            }
            catch (Exception ex)
            {
                Fail(model, ex.Message);
            }
            finally
            {
                model.PasswordHash = null;
            }
            return model;
        }

        public SuperAdministratorActionViewModel SetActiveStatus(SuperAdministratorActionViewModel model)
        {
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(model, NOT_AUTHORISED);
                    return model;
                }
                if (model.StaffId == null || model.IsActive == null)
                {
                    Fail(model, "Staff and status are required.");
                    return model;
                }
                var staff = FindSuperAdminStaff(tenantId, model.StaffId.Value);
                if (staff == null)
                {
                    Fail(model, NOT_SUPER_ADMIN);
                    return model;
                }
                if (SameEmail(staff.LoginEmailAddress, model.CallerEmail))
                {
                    Fail(model, SELF_ACTION);
                    return model;
                }
                var activate = model.IsActive.Value;
                if (!activate && !AnotherActiveSuperAdminRemains(tenantId, staff.StaffId))
                {
                    Fail(model, LAST_ACTIVE);
                    return model;
                }

                var now = DateTime.UtcNow;
                staff.IsActive = activate;
                staff.UpdatedBy = model.UpdatedBy;
                staff.UpdatedOn = now;
                foreach (var user in UsersByEmail(tenantId, staff.LoginEmailAddress))
                {
                    user.IsActive = activate;
                    user.UpdatedBy = model.UpdatedBy;
                    user.UpdatedOn = now;
                }
                if (!activate)
                {
                    ExpireSessions(tenantId, staff.LoginEmailAddress);
                }
                this.context!.SaveChanges();

                model._failure = false;
                model._message = FullName(staff) + (activate ? " activated." : " deactivated.");
            }
            catch (Exception ex)
            {
                Fail(model, ex.Message);
            }
            return model;
        }

        public SuperAdministratorActionViewModel DeleteSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(model, NOT_AUTHORISED);
                    return model;
                }
                if (model.StaffId == null)
                {
                    Fail(model, "Staff is required.");
                    return model;
                }
                var staffId = model.StaffId.Value;
                var staff = FindSuperAdminStaff(tenantId, staffId);
                if (staff == null)
                {
                    Fail(model, NOT_SUPER_ADMIN);
                    return model;
                }
                if (SameEmail(staff.LoginEmailAddress, model.CallerEmail))
                {
                    Fail(model, SELF_ACTION);
                    return model;
                }
                if (!AnotherActiveSuperAdminRemains(tenantId, staffId))
                {
                    Fail(model, LAST_ACTIVE);
                    return model;
                }

                // Delete is an undo for accounts created by mistake. Every table
                // records created_by/updated_by as the staff GUID without a
                // foreign key, so deleting someone who has ever signed in would
                // leave their trail unresolvable. A successful login in the
                // access log is the cheap, complete signal that they may have
                // written something; such accounts keep their row and are
                // deactivated instead.
                var name = FullName(staff);
                string? blocker = null;
                var email = staff.LoginEmailAddress;
                if (!string.IsNullOrWhiteSpace(email)
                    && this.context!.UserAccessLog.Any(x => x.TenantId == tenantId && x.Emailaddress == email && x.LoginStatus == true))
                {
                    blocker = name + " has signed in before, so their name must stay resolvable in audit trails. Deactivate instead.";
                }
                else if (this.context!.StaffSchoolInfo.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " has school attachments. Demote to a school profile instead, or deactivate.";
                }
                else if (this.context.StaffCoursesectionSchedule.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " is scheduled on course sections. Deactivate instead.";
                }
                else if (this.context.Assignment.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " has assignments. Deactivate instead.";
                }
                else if (this.context.StudentAttendance.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " has attendance records. Deactivate instead.";
                }
                else if (this.context.StudentMissingAttendances.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " has missing attendance records. Deactivate instead.";
                }
                else if (this.context.StaffCertificateInfo.Any(x => x.TenantId == tenantId && x.StaffId == staffId))
                {
                    blocker = name + " has certificate records. Deactivate instead.";
                }
                if (blocker != null)
                {
                    Fail(model, blocker);
                    return model;
                }

                using (var transaction = this.context.Database.BeginTransaction())
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        this.context.LoginSession.RemoveRange(this.context.LoginSession.Where(x => x.TenantId == tenantId && x.EmailAddress == email));
                        this.context.UserMaster.RemoveRange(this.context.UserMaster.Where(x => x.TenantId == tenantId && x.EmailAddress == email));
                    }
                    this.context.CustomFieldsValue.RemoveRange(this.context.CustomFieldsValue.Where(x => x.TenantId == tenantId && x.Module == "Staff" && x.TargetId == staffId));
                    this.context.StaffMaster.Remove(staff);
                    this.context.SaveChanges();
                    transaction.Commit();
                }

                model._failure = false;
                model._message = name + " deleted.";
            }
            catch (Exception ex)
            {
                Fail(model, ex.Message);
            }
            return model;
        }

        public SuperAdministratorCandidateListViewModel GetPromotionCandidates(SuperAdministratorCandidateListViewModel model)
        {
            var result = new SuperAdministratorCandidateListViewModel
            {
                TenantId = model.TenantId,
                SchoolId = model.SchoolId,
                SearchText = model.SearchText,
                _tenantName = model._tenantName,
                _token = model._token,
                _userName = model._userName
            };
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(result, NOT_AUTHORISED);
                    return result;
                }
                var search = (model.SearchText ?? "").Trim().ToLower();
                var superAdminIds = SuperAdminUsers(tenantId).Select(u => u.UserId).ToHashSet();

                var query = this.context!.StaffMaster.Where(x => x.TenantId == tenantId
                    && x.LoginEmailAddress != null && x.LoginEmailAddress != ""
                    && x.PortalAccess == true && x.IsActive != false
                    && (x.Profile == null || x.Profile.ToLower() != "super administrator"));
                if (search.Length > 0)
                {
                    query = query.Where(x => (x.FirstGivenName ?? "").ToLower().Contains(search)
                        || (x.LastFamilyName ?? "").ToLower().Contains(search)
                        || x.LoginEmailAddress!.ToLower().Contains(search));
                }
                var staff = query.OrderBy(x => x.LastFamilyName).ThenBy(x => x.FirstGivenName).Take(60).ToList()
                    .Where(s => !superAdminIds.Contains(s.StaffId)).Take(20).ToList();
                var schoolIds = staff.Select(s => s.SchoolId).Distinct().ToList();
                var schools = this.context.SchoolMaster.Where(x => x.TenantId == tenantId && schoolIds.Contains(x.SchoolId))
                    .Select(x => new { x.SchoolId, x.SchoolName }).ToList().ToDictionary(x => x.SchoolId, x => x.SchoolName);

                foreach (var s in staff)
                {
                    schools.TryGetValue(s.SchoolId, out var schoolName);
                    result.Candidates.Add(new SuperAdministratorCandidateViewModel
                    {
                        StaffId = s.StaffId,
                        SchoolId = s.SchoolId,
                        SchoolName = schoolName,
                        FirstGivenName = s.FirstGivenName,
                        MiddleName = s.MiddleName,
                        LastFamilyName = s.LastFamilyName,
                        LoginEmailAddress = s.LoginEmailAddress,
                        Profile = string.IsNullOrWhiteSpace(s.Profile) ? s.JobTitle : s.Profile
                    });
                }
                result._failure = false;
            }
            catch (Exception ex)
            {
                Fail(result, ex.Message);
            }
            return result;
        }

        public SuperAdministratorActionViewModel PromoteToSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(model, NOT_AUTHORISED);
                    return model;
                }
                if (model.StaffId == null)
                {
                    Fail(model, "Staff is required.");
                    return model;
                }
                var staff = this.context!.StaffMaster.FirstOrDefault(x => x.TenantId == tenantId && x.StaffId == model.StaffId.Value);
                if (staff == null)
                {
                    Fail(model, "Staff member not found.");
                    return model;
                }
                var users = UsersByEmail(tenantId, staff.LoginEmailAddress);
                if (IsSuperAdminProfile(staff.Profile) || users.Any(u => IsSuperAdminMembership(u.Membership)))
                {
                    Fail(model, FullName(staff) + " is already a Super Administrator.");
                    return model;
                }
                if (!users.Any())
                {
                    Fail(model, FullName(staff) + " has no portal login. Give them portal access with a login email first.");
                    return model;
                }

                var now = DateTime.UtcNow;
                foreach (var user in users)
                {
                    var membershipId = SuperAdminMembershipId(tenantId, user.SchoolId);
                    if (membershipId == null)
                    {
                        Fail(model, "No Super Administrator membership exists for school " + user.SchoolId + ".");
                        return model;
                    }
                    user.MembershipId = membershipId.Value;
                    user.UpdatedBy = model.UpdatedBy;
                    user.UpdatedOn = now;
                }
                staff.Profile = SUPER_ADMINISTRATOR;
                staff.UpdatedBy = model.UpdatedBy;
                staff.UpdatedOn = now;
                // Their current token still carries the old permissions; make them sign in again.
                ExpireSessions(tenantId, staff.LoginEmailAddress);
                this.context.SaveChanges();

                model._failure = false;
                model._message = FullName(staff) + " is now a Super Administrator.";
            }
            catch (Exception ex)
            {
                Fail(model, ex.Message);
            }
            return model;
        }

        public SuperAdministratorActionViewModel DemoteSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            try
            {
                var tenantId = model.TenantId ?? Guid.Empty;
                if (!CallerIsActiveSuperAdmin(tenantId, model.CallerEmail))
                {
                    Fail(model, NOT_AUTHORISED);
                    return model;
                }
                if (model.StaffId == null)
                {
                    Fail(model, "Staff is required.");
                    return model;
                }
                var staffId = model.StaffId.Value;
                var staff = FindSuperAdminStaff(tenantId, staffId);
                if (staff == null)
                {
                    Fail(model, NOT_SUPER_ADMIN);
                    return model;
                }
                if (SameEmail(staff.LoginEmailAddress, model.CallerEmail))
                {
                    Fail(model, SELF_ACTION);
                    return model;
                }
                if (!AnotherActiveSuperAdminRemains(tenantId, staffId))
                {
                    Fail(model, LAST_ACTIVE);
                    return model;
                }

                var profileType = (model.TargetProfileType ?? "").Trim();
                if (!DemotionProfileTypes.Contains(profileType, StringComparer.OrdinalIgnoreCase))
                {
                    Fail(model, "Choose a school profile: School Administrator, Admin Assistant, Teacher or Homeroom Teacher.");
                    return model;
                }
                var targetSchoolId = model.TargetSchoolId ?? 0;
                var school = this.context!.SchoolMaster.FirstOrDefault(x => x.TenantId == tenantId && x.SchoolId == targetSchoolId);
                if (school == null)
                {
                    Fail(model, "Choose the school the staff member will belong to.");
                    return model;
                }
                var membership = this.context.Membership
                    .Where(x => x.TenantId == tenantId && x.SchoolId == targetSchoolId && x.IsSystem == true && x.ProfileType == profileType)
                    .OrderBy(x => x.MembershipId).FirstOrDefault();
                if (membership == null)
                {
                    Fail(model, "No " + profileType + " membership exists for " + school.SchoolName + ".");
                    return model;
                }

                var now = DateTime.UtcNow;
                var today = now.Date;
                var infos = this.context.StaffSchoolInfo.Where(x => x.TenantId == tenantId && x.StaffId == staffId).ToList();
                var users = UsersByEmail(tenantId, staff.LoginEmailAddress);

                using (var transaction = this.context.Database.BeginTransaction())
                {
                    if (!infos.Any())
                    {
                        // A tenant-wide account with no attachment history: the
                        // chosen school becomes its home school.
                        staff.SchoolId = targetSchoolId;
                    }
                    var homeSchoolId = staff.SchoolId;

                    var row = infos.FirstOrDefault(i => i.SchoolAttachedId == targetSchoolId && (i.EndDate == null || i.EndDate >= today));
                    if (row != null)
                    {
                        row.Profile = membership.Profile;
                        row.MembershipId = membership.MembershipId;
                        row.UpdatedBy = model.UpdatedBy;
                        row.UpdatedOn = now;
                    }
                    else
                    {
                        this.context.StaffSchoolInfo.Add(new StaffSchoolInfo
                        {
                            TenantId = tenantId,
                            SchoolId = homeSchoolId,
                            StaffId = staffId,
                            SchoolAttachedId = targetSchoolId,
                            SchoolAttachedName = school.SchoolName,
                            Profile = membership.Profile,
                            MembershipId = membership.MembershipId,
                            StartDate = today,
                            CreatedBy = model.UpdatedBy,
                            CreatedOn = now
                        });
                    }

                    staff.Profile = membership.Profile;
                    if (IsSuperAdminProfile(staff.JobTitle))
                    {
                        staff.JobTitle = null;
                    }
                    staff.UpdatedBy = model.UpdatedBy;
                    staff.UpdatedOn = now;

                    // Logins: one row per school. The row at the target school
                    // takes the chosen membership; rows at other attached schools
                    // take that attachment's profile; a lone row at a school the
                    // staff is not attached to is moved to the target school
                    // (school_id is part of the key, so it is re-created).
                    var hasTargetLogin = users.Any(u => u.SchoolId == targetSchoolId);
                    foreach (var user in users)
                    {
                        if (user.SchoolId == targetSchoolId)
                        {
                            user.MembershipId = membership.MembershipId;
                        }
                        else
                        {
                            var attachment = infos.FirstOrDefault(i => i.SchoolAttachedId == user.SchoolId && (i.EndDate == null || i.EndDate >= today));
                            if (attachment == null && !hasTargetLogin)
                            {
                                this.context.UserMaster.Remove(user);
                                this.context.SaveChanges();
                                this.context.UserMaster.Add(new UserMaster
                                {
                                    TenantId = user.TenantId,
                                    SchoolId = targetSchoolId,
                                    UserId = user.UserId,
                                    Name = user.Name,
                                    EmailAddress = user.EmailAddress,
                                    PasswordHash = user.PasswordHash,
                                    LangId = user.LangId,
                                    MembershipId = membership.MembershipId,
                                    IsTenantadmin = user.IsTenantadmin,
                                    IsActive = user.IsActive,
                                    Description = user.Description,
                                    LastUsedSchoolId = targetSchoolId,
                                    CreatedBy = user.CreatedBy,
                                    CreatedOn = user.CreatedOn,
                                    UpdatedBy = model.UpdatedBy,
                                    UpdatedOn = now
                                });
                                hasTargetLogin = true;
                                continue;
                            }
                            var schoolMembership = attachment == null ? null : this.context.Membership
                                .FirstOrDefault(x => x.TenantId == tenantId && x.SchoolId == user.SchoolId && x.Profile == attachment.Profile);
                            user.MembershipId = schoolMembership?.MembershipId
                                ?? this.context.Membership.Where(x => x.TenantId == tenantId && x.SchoolId == user.SchoolId && x.IsSystem == true && x.ProfileType == "Teacher")
                                    .Select(x => (int?)x.MembershipId).FirstOrDefault()
                                ?? 4;
                        }
                        user.LastUsedSchoolId = targetSchoolId;
                        user.UpdatedBy = model.UpdatedBy;
                        user.UpdatedOn = now;
                    }

                    ExpireSessions(tenantId, staff.LoginEmailAddress);
                    this.context.SaveChanges();
                    transaction.Commit();
                }

                model._failure = false;
                model._message = FullName(staff) + " is now " + profileType + " at " + school.SchoolName + ".";
            }
            catch (Exception ex)
            {
                Fail(model, ex.Message);
            }
            return model;
        }
    }
}
