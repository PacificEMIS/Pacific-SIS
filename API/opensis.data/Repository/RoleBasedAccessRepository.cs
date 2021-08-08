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

using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.RoleBasedAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class RoleBasedAccessRepository : IRoleBasedAccessRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public RoleBasedAccessRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

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
                var permission = this.context?.PermissionGroup.Where(x => x.TenantId == permissionGroupListViewModel.TenantId && x.SchoolId == permissionGroupListViewModel.SchoolId && x.IsActive == true).Select(e=> new PermissionGroup()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    PermissionGroupId=e.PermissionGroupId,
                    PermissionGroupName=e.PermissionGroupName,
                    ShortName=e.ShortName,
                    IsActive=e.IsActive,
                    IsSystem=e.IsSystem,
                    Title=e.Title,
                    Icon=e.Icon,
                    IconType=e.IconType,
                    SortOrder=e.SortOrder,
                    Type=e.Type,
                    Path=e.Path,
                    BadgeType=e.BadgeType,
                    BadgeValue=e.BadgeValue,
                    Active=e.Active,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == permissionGroupListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn=e.CreatedOn,
                    UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == permissionGroupListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn=e.UpdatedOn
                });

                permissionGroupListView.permissionGroupList = permission.ToList();
                permissionGroupListView._token = permissionGroupListViewModel._token;

                if (permission.Count() > 0)
                {
                    permissionGroupListView._failure = false;
                    permissionGroupListView._message = "Permission Group List Fetched";
                }
                else
                {
                    permissionGroupListView._failure = true;
                    permissionGroupListView._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                permissionGroupListView._message = ex.Message;
                permissionGroupListView._failure = true;
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

                rolePermissionListView.PermissionList = new List<RolePermissionViewModel>();

                //var permissionGroup = this.context?.PermissionGroups.Include(pc => pc.PermissionCategories).ThenInclude(rp => rp.RolePermissions.Where//(a=>a.RoleId==objModel.role.RoleId)).Where(x => x.TenantId == objModel.role.TenantId && x.HospitalId == objModel.role.HospitalId && x.IsActive == true );

                var permissionGroup = this.context?.PermissionGroup.Where(x => x.IsActive == true).Select(pg => new PermissionGroup
                {
                    PermissionGroupId = pg.PermissionGroupId,
                    TenantId = pg.TenantId,
                    SchoolId = pg.SchoolId,
                    PermissionGroupName = pg.PermissionGroupName,
                    ShortName = pg.ShortName,
                    IsActive = pg.IsActive,
                    IsSystem = pg.IsSystem,
                    Title = pg.Title,
                    IconType = pg.IconType,
                    Icon = pg.Icon,
                    SortOrder = pg.SortOrder,
                    Type = pg.Type,
                    Path = pg.Path,
                    BadgeType = pg.BadgeType,
                    BadgeValue = pg.BadgeValue,
                    Active = pg.Active,
                    CreatedBy= (pg.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == pg.CreatedBy).Name : null,
                    CreatedOn =pg.CreatedOn,
                    UpdatedBy= (pg.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == pg.UpdatedBy).Name : null,
                    UpdatedOn =pg.UpdatedOn,
                    PermissionCategory = (ICollection<PermissionCategory>)pg.PermissionCategory.Select(pc => new PermissionCategory
                    {
                        PermissionCategoryId = pc.PermissionCategoryId,
                        TenantId = pc.TenantId,
                        SchoolId = pc.SchoolId,
                        PermissionGroupId = pc.PermissionGroupId,
                        PermissionCategoryName = pc.PermissionCategoryName,
                        ShortCode = pc.ShortCode,
                        Path = pc.Path,
                        Title = pc.Title,
                        Type = pc.Type,
                        IsActive = pc.IsActive,
                        EnableView = pc.EnableView,
                        EnableAdd = pc.EnableAdd,
                        EnableDelete = pc.EnableDelete,
                        EnableEdit = pc.EnableEdit,
                        CreatedBy= (pc.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == pc.CreatedBy).Name : null,
                        CreatedOn =pc.CreatedOn,
                        UpdatedBy= (pc.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == pc.UpdatedBy).Name : null,
                        UpdatedOn =pc.UpdatedOn,
                        PermissionSubcategory = (ICollection<PermissionSubcategory>)pc.PermissionSubcategory.Select(psc => new PermissionSubcategory
                        {
                            TenantId = psc.TenantId,
                            SchoolId = psc.SchoolId,
                            PermissionCategoryId = psc.PermissionCategoryId,
                            PermissionSubcategoryId = psc.PermissionSubcategoryId,
                            PermissionGroupId = psc.PermissionGroupId,
                            PermissionSubcategoryName = psc.PermissionSubcategoryName,
                            ShortCode = psc.ShortCode,
                            Path = psc.Path,
                            Title = psc.Title,
                            Type = psc.Type,
                            IsActive = psc.IsActive,
                            EnableView = psc.EnableView,
                            EnableAdd = psc.EnableAdd,
                            EnableDelete = psc.EnableDelete,
                            EnableEdit = psc.EnableEdit,
                            CreatedBy = (psc.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == psc.CreatedBy).Name : null,
                            CreatedOn = psc.CreatedOn,
                            UpdatedBy = (psc.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == psc.UpdatedBy).Name : null,
                            UpdatedOn = psc.UpdatedOn,
                            RolePermission = (ICollection<RolePermission>)psc.RolePermission.Select(scrp => new RolePermission
                            {
                                RolePermissionId = scrp.RolePermissionId,
                                TenantId = scrp.TenantId,
                                SchoolId = scrp.SchoolId,
                                MembershipId = scrp.MembershipId,
                                PermissionSubcategoryId = scrp.PermissionSubcategoryId,
                                CanView = scrp.CanView,
                                CanAdd = scrp.CanAdd,
                                CanDelete = scrp.CanDelete,
                                CanEdit = scrp.CanEdit,
                                CreatedBy = (scrp.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.SchoolId == rolePermissionListViewModel.SchoolId && u.EmailAddress == scrp.CreatedBy).Name : null,
                                CreatedOn = scrp.CreatedOn,
                                UpdatedBy = (scrp.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == scrp.UpdatedBy).Name : null,
                                UpdatedOn = scrp.UpdatedOn,
                            }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.SchoolId == rolePermissionListViewModel.SchoolId && x.MembershipId == rolePermissionListViewModel.MembershipId)
                        }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.IsActive == true),
                        RolePermission = (ICollection<RolePermission>)pc.RolePermission.Select(crp => new RolePermission
                        {
                            RolePermissionId = crp.RolePermissionId,
                            TenantId = crp.TenantId,
                            SchoolId = crp.SchoolId,
                            MembershipId = crp.MembershipId,
                            PermissionCategoryId = crp.PermissionCategoryId,
                            CanView = crp.CanView,
                            CanAdd = crp.CanAdd,
                            CanDelete = crp.CanDelete,
                            CanEdit = crp.CanEdit,
                            CreatedBy = (crp.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == crp.CreatedBy).Name : null,
                            CreatedOn = crp.CreatedOn,
                            UpdatedBy = (crp.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.EmailAddress == crp.UpdatedBy).Name : null,
                            UpdatedOn = crp.UpdatedOn,
                        }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.SchoolId == rolePermissionListViewModel.SchoolId && x.MembershipId == rolePermissionListViewModel.MembershipId)
                    }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.SchoolId == rolePermissionListViewModel.SchoolId && x.IsActive == true),
                    RolePermission = (ICollection<RolePermission>)pg.RolePermission.Select(grp => new RolePermission
                    {
                        RolePermissionId = grp.RolePermissionId,
                        TenantId = grp.TenantId,
                        SchoolId = grp.SchoolId,
                        MembershipId = grp.MembershipId,
                        PermissionGroupId = grp.PermissionGroupId,
                        CanView = grp.CanView,
                        CanAdd = grp.CanAdd,
                        CanDelete = grp.CanDelete,
                        CanEdit = grp.CanEdit,
                        CreatedBy = (grp.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId && u.SchoolId == rolePermissionListViewModel.SchoolId && u.EmailAddress == grp.CreatedBy).Name : null,
                        CreatedOn = grp.CreatedOn,
                        UpdatedBy = (grp.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == rolePermissionListViewModel.TenantId  && u.EmailAddress == grp.UpdatedBy).Name : null,
                        UpdatedOn = grp.UpdatedOn,
                    }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.MembershipId == rolePermissionListViewModel.MembershipId)
                }).Where(x => x.TenantId == rolePermissionListViewModel.TenantId && x.SchoolId == rolePermissionListViewModel.SchoolId).OrderBy(x => x.SortOrder).ToList();

                if (permissionGroup.Count() > 0)
                {
                    foreach (PermissionGroup pg in permissionGroup.ToList())
                    {
                        if (pg.RolePermission.Count > 0)
                        {
                            RolePermissionViewModel pgvm = new RolePermissionViewModel();
                            pgvm.permissionGroup = new PermissionGroup();

                            if (pg.PermissionCategory.Count > 0)
                            {
                                var permissionCategory = pg.PermissionCategory.Where(x => x.RolePermission.Count > 0);

                                pg.PermissionCategory = permissionCategory.ToList();
                                foreach (var scg in pg.PermissionCategory)
                                {
                                    var permissionSubCategory = scg.PermissionSubcategory.Where(x => x.RolePermission.Count > 0);
                                    scg.PermissionSubcategory = permissionSubCategory.ToList();
                                }
                            }

                            pgvm.permissionGroup = pg;
                            rolePermissionListView.PermissionList.Add(pgvm);
                        }
                    }

                    //foreach (PermissionGroup pg in permissionGroup.ToList())
                    //{
                    //    RolePermissionViewModel pgvm = new RolePermissionViewModel();
                    //    pgvm.permissionGroup = new PermissionGroup();
                    //    pgvm.permissionGroup = pg;

                    //    //Get Role permission 
                    //    //foreach (PermissionCategory pc in pg.PermissionCategory)
                    //    //{
                    //    //    if (pc.RolePermission.Count == 0)
                    //    //    {

                    //    //        RolePermission rp = new RolePermission();
                    //    //        rp.PermissionCategoryId = pc.PermissionCategoryId;
                    //    //        rp.MembershipId = rolePermissionListViewModel.MembershipId;
                    //    //        rp.CanAdd = false;
                    //    //        rp.CanDelete = false;
                    //    //        rp.CanEdit = false;
                    //    //        rp.CanView = false;
                    //    //        rp.TenantId = rolePermissionListViewModel.TenantId;
                    //    //        rp.SchoolId = rolePermissionListViewModel.SchoolId;
                    //    //        pc.RolePermission.Add(rp);
                    //    //    }
                    //    //}

                    //    rolePermissionListView.PermissionList.Add(pgvm);
                    //}
                    //objViewModel.PermissionList = permissionGroup.ToList(); 
                    rolePermissionListView._failure = false;
                    rolePermissionListView._message = "Permission List Fetched";
                }
                else
                {
                    rolePermissionListView._failure = true;
                    rolePermissionListView._message = NORECORDFOUND;
                }
                rolePermissionListView._token = rolePermissionListViewModel._token;
                rolePermissionListView.TenantId = rolePermissionListViewModel.TenantId;
                rolePermissionListView.SchoolId = rolePermissionListViewModel.SchoolId;
                rolePermissionListView.MembershipId = rolePermissionListViewModel.MembershipId;
            }
            catch (Exception ex)
            {
                rolePermissionListView._message = ex.Message;
                rolePermissionListView._failure = true;
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
            PermissionGroupAddViewModel permissionGroupUpdateModel = new PermissionGroupAddViewModel();
            try
            {
                var permissionGroup = this.context?.PermissionGroup.FirstOrDefault(x => x.TenantId == permissionGroupAddViewModel.permissionGroup.TenantId && x.SchoolId == permissionGroupAddViewModel.permissionGroup.SchoolId && x.PermissionGroupId == permissionGroupAddViewModel.permissionGroup.PermissionGroupId);

                if (permissionGroup != null)
                {
                    permissionGroup.IsActive = permissionGroupAddViewModel.permissionGroup.IsActive;
                    permissionGroup.UpdatedOn = DateTime.UtcNow;
                    permissionGroup.UpdatedBy = permissionGroupAddViewModel.permissionGroup.UpdatedBy;

                    this.context?.SaveChanges();
                    permissionGroupUpdateModel.permissionGroup = permissionGroup;
                    permissionGroupUpdateModel._token = permissionGroupAddViewModel._token;
                    permissionGroupUpdateModel._message = "Permission Group Updated Successfully";
                    permissionGroupUpdateModel._failure = false;
                }
                else
                {
                    permissionGroupUpdateModel.permissionGroup = null;
                    permissionGroupUpdateModel._failure = true;
                    permissionGroupUpdateModel._message = "No Permission Group Found";
                }
            }
            catch (Exception ex)
            {
                permissionGroupUpdateModel._failure = true;
                permissionGroupUpdateModel._message = ex.Message;
            }
            return permissionGroupUpdateModel;
        }

        /// <summary>
        /// Update Role Permission
        /// </summary>
        /// <param name="permissionGroupListViewModel"></param>
        /// <returns></returns>
        public PermissionGroupListViewModel UpdateRolePermission(PermissionGroupListViewModel permissionGroupListViewModel)
        {
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            PermissionGroupListViewModel PermissionGroupViewModel = new PermissionGroupListViewModel();
            try
            {
                foreach (PermissionGroup p_group in permissionGroupListViewModel.permissionGroupList)
                {
                    if (p_group.RolePermission.ToList().Count() > 0 && p_group.RolePermission != null)
                    {

                        var PermissionGroup = this.context?.RolePermission.FirstOrDefault(x => x.TenantId == p_group.TenantId && x.SchoolId == p_group.SchoolId && x.PermissionGroupId == p_group.PermissionGroupId && x.RolePermissionId == p_group.RolePermission.FirstOrDefault().RolePermissionId && x.MembershipId == p_group.RolePermission.FirstOrDefault().MembershipId);

                        if (PermissionGroup != null)
                        {
                            PermissionGroup.CanAdd = p_group.RolePermission.FirstOrDefault().CanAdd;
                            PermissionGroup.CanEdit = p_group.RolePermission.FirstOrDefault().CanEdit;
                            PermissionGroup.CanDelete = p_group.RolePermission.FirstOrDefault().CanDelete;
                            PermissionGroup.CanView = p_group.RolePermission.FirstOrDefault().CanView;
                            PermissionGroup.UpdatedBy = p_group.RolePermission.FirstOrDefault().UpdatedBy;
                            PermissionGroup.UpdatedOn = DateTime.UtcNow;
                            this.context?.SaveChanges();
                        }
                        else
                        {
                            int? AutoId = 1;

                            var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == p_group.SchoolId && x.TenantId == p_group.TenantId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                            if (rolePermissionData != null)
                            {
                                AutoId = rolePermissionData.RolePermissionId + 1;
                            }
                            p_group.RolePermission.FirstOrDefault().RolePermissionId = (int)AutoId;
                            p_group.RolePermission.FirstOrDefault().PermissionSubcategoryId = null;
                            p_group.RolePermission.FirstOrDefault().PermissionCategoryId = null;
                            p_group.RolePermission.FirstOrDefault().CreatedBy = permissionGroupListViewModel.CreatedBy;
                            p_group.RolePermission.FirstOrDefault().CreatedOn = DateTime.UtcNow;
                            this.context?.RolePermission.Add(p_group.RolePermission.FirstOrDefault());
                            this.context?.SaveChanges();
                        }
                        if (p_group.PermissionCategory.Count() > 0 && p_group.PermissionCategory != null)
                        {
                            foreach (PermissionCategory p_cat in p_group.PermissionCategory)
                            {
                                if (p_cat.RolePermission.ToList().Count() > 0 && p_cat.RolePermission != null)
                                {
                                    var PermissionCatagoary = this.context?.RolePermission.FirstOrDefault(x => x.TenantId == p_cat.TenantId && x.SchoolId == p_cat.SchoolId && x.PermissionCategoryId == p_cat.PermissionCategoryId && x.RolePermissionId == p_cat.RolePermission.FirstOrDefault().RolePermissionId && x.MembershipId == p_cat.RolePermission.FirstOrDefault().MembershipId);

                                    if (PermissionCatagoary != null)
                                    {
                                        PermissionCatagoary.CanAdd = p_cat.RolePermission.FirstOrDefault().CanAdd;
                                        PermissionCatagoary.CanEdit = p_cat.RolePermission.FirstOrDefault().CanEdit;
                                        PermissionCatagoary.CanDelete = p_cat.RolePermission.FirstOrDefault().CanDelete;
                                        PermissionCatagoary.CanView = p_cat.RolePermission.FirstOrDefault().CanView;
                                        PermissionCatagoary.UpdatedBy = p_cat.RolePermission.FirstOrDefault().UpdatedBy;
                                        PermissionCatagoary.UpdatedOn = DateTime.UtcNow;
                                        this.context?.SaveChanges();
                                    }
                                    else
                                    {
                                        int? AutoId = 1;

                                        var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == p_cat.SchoolId && x.TenantId == p_cat.TenantId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                                        if (rolePermissionData != null)
                                        {
                                            AutoId = rolePermissionData.RolePermissionId + 1;
                                        }
                                        p_cat.RolePermission.FirstOrDefault().RolePermissionId = (int)AutoId;
                                        p_cat.RolePermission.FirstOrDefault().PermissionGroupId = null;
                                        p_cat.RolePermission.FirstOrDefault().PermissionSubcategoryId = null;
                                        p_cat.CreatedBy = permissionGroupListViewModel.CreatedBy;
                                        p_cat.CreatedOn = DateTime.UtcNow;
                                        this.context?.RolePermission.Add(p_cat.RolePermission.FirstOrDefault());
                                        this.context?.SaveChanges();
                                    }
                                    if (p_cat.PermissionSubcategory.Count() > 0 && p_cat.PermissionSubcategory != null)
                                    {

                                        foreach (PermissionSubcategory p_subcat in p_cat.PermissionSubcategory)
                                        {
                                            foreach (RolePermission roleper in p_subcat.RolePermission)
                                            {
                                                var PermissionSubCatagory = this.context?.RolePermission.FirstOrDefault(x => x.TenantId == roleper.TenantId && x.SchoolId == roleper.SchoolId && x.RolePermissionId == roleper.RolePermissionId && x.PermissionSubcategoryId == roleper.PermissionSubcategoryId && x.MembershipId == roleper.MembershipId);

                                                if (PermissionSubCatagory != null)
                                                {
                                                    PermissionSubCatagory.CanAdd = roleper.CanAdd;
                                                    PermissionSubCatagory.CanEdit = roleper.CanEdit;
                                                    PermissionSubCatagory.CanDelete = roleper.CanDelete;
                                                    PermissionSubCatagory.CanView = roleper.CanView;
                                                    PermissionSubCatagory.UpdatedBy = roleper.UpdatedBy;
                                                    PermissionSubCatagory.UpdatedOn = DateTime.UtcNow;
                                                    this.context?.SaveChanges();
                                                }
                                                else
                                                {
                                                    int? AutoId = 1;

                                                    var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == roleper.SchoolId && x.TenantId == roleper.TenantId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                                                    if (rolePermissionData != null)
                                                    {
                                                        AutoId = rolePermissionData.RolePermissionId + 1;
                                                    }
                                                    roleper.RolePermissionId = (int)AutoId;
                                                    roleper.PermissionGroupId = null;
                                                    roleper.PermissionCategoryId = null;
                                                    roleper.CreatedBy = permissionGroupListViewModel.CreatedBy;
                                                    roleper.CreatedOn = DateTime.UtcNow;
                                                    this.context?.RolePermission.Add(roleper);
                                                    this.context?.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                transaction.Commit();
                PermissionGroupViewModel.permissionGroupList = permissionGroupListViewModel.permissionGroupList;
                PermissionGroupViewModel._failure = false;
                PermissionGroupViewModel._message = "Role Permission Updated Successfully";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return PermissionGroupViewModel;
        }
    }
}
