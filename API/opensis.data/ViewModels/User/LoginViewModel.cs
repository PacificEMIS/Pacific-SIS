﻿/***********************************************************************************
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

using opensis.data.Models;
using opensis.data.ViewModels.RoleBasedAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace opensis.data.ViewModels.User
{
    public class LoginViewModel : CommonFields
    {

        public LoginViewModel()
        {
            userAccessLog = new UserAccessLog();
        }

        [Required]
        public string? Email { get; set; }
       
        public string? Password { get; set; }

        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? MembershipName { get; set; }
        public string? MembershipType { get; set; }
        public string? CreatedBy { get; set; }
        public int? MembershipId { get; set; }
        public byte[]? UserPhoto { get; set; }
        public string? FirstGivenName { get; set; }
        public int? LastUsedSchoolId { get; set; }
        public List<RolePermissionViewModel>? PermissionList { get; set; }
        public UserAccessLog? userAccessLog { get; set; }
        public string? UserGuid { get; set; }
    }
}
