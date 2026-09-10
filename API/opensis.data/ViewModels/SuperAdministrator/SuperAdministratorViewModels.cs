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

using System;
using System.Collections.Generic;

namespace opensis.data.ViewModels.SuperAdministrator
{
    /// <summary>
    /// One Super Administrator as shown on the Settings > Administration >
    /// Super Administrators page. A Super Administrator is a staff record
    /// whose login carries the tenant-wide "Super Administrator" membership.
    /// </summary>
    public class SuperAdministratorViewModel
    {
        public int StaffId { get; set; }
        public int SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public Guid? StaffGuid { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? LoginEmailAddress { get; set; }
        /// <summary>False when the staff record carries the profile but has no login row.</summary>
        public bool HasLogin { get; set; }
        /// <summary>Whether the login can sign in (user_master.is_active).</summary>
        public bool IsActive { get; set; }
        /// <summary>The staff row's own disable flag (staff_master.is_active = false); does not block login.</summary>
        public bool StaffRecordDisabled { get; set; }
        /// <summary>True for the caller's own account; self-service actions are refused.</summary>
        public bool IsSelf { get; set; }
        /// <summary>True when staff_school_info rows exist (typically a promoted staff).</summary>
        public bool HasSchoolAttachments { get; set; }
        /// <summary>Current home school attachment, used to prefill the demote dialog.</summary>
        public int? PreviousSchoolId { get; set; }
        public string? PreviousProfile { get; set; }
        public DateTime? LastLoginOn { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class SuperAdministratorListViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        /// <summary>Set by the service from the validated token. Never trusted from the client.</summary>
        public string? CallerEmail { get; set; }
        public List<SuperAdministratorViewModel> SuperAdministrators { get; set; } = new();
    }

    public class SuperAdministratorAddViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        /// <summary>Home school recorded on the new staff row; the caller's current school.</summary>
        public int? SchoolId { get; set; }
        public string? CallerEmail { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? LoginEmailAddress { get; set; }
        /// <summary>AES-encrypted by the UI exactly like staff portal passwords.</summary>
        public string? PasswordHash { get; set; }
        public string? CreatedBy { get; set; }
        /// <summary>Populated on success.</summary>
        public int? StaffId { get; set; }
    }

    /// <summary>
    /// Request for the single-target actions: activate/deactivate, delete,
    /// promote and demote.
    /// </summary>
    public class SuperAdministratorActionViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public string? CallerEmail { get; set; }
        public int? StaffId { get; set; }
        /// <summary>Desired status for setActiveStatus.</summary>
        public bool? IsActive { get; set; }
        /// <summary>Demote only: the school the demoted staff will be attached to.</summary>
        public int? TargetSchoolId { get; set; }
        /// <summary>Demote only: one of the four school profile types.</summary>
        public string? TargetProfileType { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class SuperAdministratorCandidateViewModel
    {
        public int StaffId { get; set; }
        public int SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? LoginEmailAddress { get; set; }
        public string? Profile { get; set; }
    }

    public class SuperAdministratorCandidateListViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public string? CallerEmail { get; set; }
        public string? SearchText { get; set; }
        public List<SuperAdministratorCandidateViewModel> Candidates { get; set; } = new();
    }
}
