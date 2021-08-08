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

using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.core.RoleBasedAccess.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.RoleBasedAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.RoleBasedAccess.Services
{
    public class RoleBasedAccessService : IRoleBasedAccessService
    {
        private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IRoleBasedAccessRepository roleBasedAccessRepository;
        public ICheckLoginSession tokenManager;
        public RoleBasedAccessService(IRoleBasedAccessRepository roleBasedAccessRepository, ICheckLoginSession checkLoginSession)
        {
            this.roleBasedAccessRepository = roleBasedAccessRepository;
            this.tokenManager = checkLoginSession;
        }

        //Required for Unit Testing
        public RoleBasedAccessService() { }

        /// <summary>
        /// Get All Permission Group
        /// </summary>
        /// <param name="permissionGroupListViewModel"></param>
        /// <returns></returns>
        public PermissionGroupListViewModel GetAllPermissionGroup(PermissionGroupListViewModel permissionGroupListViewModel)
        {
            PermissionGroupListViewModel permissionGroupListView = new PermissionGroupListViewModel();
            try
            {
                if (tokenManager.CheckToken(permissionGroupListViewModel._tenantName + permissionGroupListViewModel._userName, permissionGroupListViewModel._token))
                {
                    permissionGroupListView = this.roleBasedAccessRepository.GetAllPermissionGroup(permissionGroupListViewModel);
                }
                else
                {
                    permissionGroupListView._failure = true;
                    permissionGroupListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                permissionGroupListView._failure = true;
                permissionGroupListView._message = es.Message;
            }
            return permissionGroupListView;
        }

        /// <summary>
        /// Get All Role Permission
        /// </summary>
        /// <param name="rolePermissionListViewModel"></param>
        /// <returns></returns>
        public RolePermissionListViewModel GetAllRolePermission(RolePermissionListViewModel rolePermissionListViewModel)
        {
            RolePermissionListViewModel rolePermissionListView = new RolePermissionListViewModel();
            try
            {
                if (tokenManager.CheckToken(rolePermissionListViewModel._tenantName + rolePermissionListViewModel._userName, rolePermissionListViewModel._token))
                {
                    rolePermissionListView = this.roleBasedAccessRepository.GetAllRolePermission(rolePermissionListViewModel);
                }
                else
                {
                    rolePermissionListView._failure = true;
                    rolePermissionListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                rolePermissionListView._failure = true;
                rolePermissionListView._message = es.Message;
            }
            return rolePermissionListView;
        }

        /// <summary>
        /// Update Permission Group
        /// </summary>
        /// <param name="permissionGroupAddViewModel"></param>
        /// <returns></returns>
        public PermissionGroupAddViewModel UpdatePermissionGroup(PermissionGroupAddViewModel permissionGroupAddViewModel)
        {
            PermissionGroupAddViewModel permissionGroupUpdate = new PermissionGroupAddViewModel();
            try
            {
                if (tokenManager.CheckToken(permissionGroupAddViewModel._tenantName + permissionGroupAddViewModel._userName, permissionGroupAddViewModel._token))
                {
                    permissionGroupUpdate = this.roleBasedAccessRepository.UpdatePermissionGroup(permissionGroupAddViewModel);
                }
                else
                {
                    permissionGroupUpdate._failure = true;
                    permissionGroupUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                permissionGroupUpdate._failure = true;
                permissionGroupUpdate._message = es.Message;
            }
            return permissionGroupUpdate;
        }

        /// <summary>
        /// Update Role Permission
        /// </summary>
        /// <param name="permissionGroupListViewModel"></param>
        /// <returns></returns>
        public PermissionGroupListViewModel UpdateRolePermission(PermissionGroupListViewModel permissionGroupListViewModel)
        {
            PermissionGroupListViewModel rolePermissionUpdate = new PermissionGroupListViewModel();
            try
            {
                if (tokenManager.CheckToken(permissionGroupListViewModel._tenantName + permissionGroupListViewModel._userName, permissionGroupListViewModel._token))
                {
                    rolePermissionUpdate = this.roleBasedAccessRepository.UpdateRolePermission(permissionGroupListViewModel);
                }
                else
                {
                    rolePermissionUpdate._failure = true;
                    rolePermissionUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                rolePermissionUpdate._failure = true;
                rolePermissionUpdate._message = es.Message;
            }
            return rolePermissionUpdate;
        }
    }
}
