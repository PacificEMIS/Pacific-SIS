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
using opensis.data.ViewModels.CustomField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class CustomFieldRepository : ICustomFieldRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public CustomFieldRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Custom Field
        /// </summary>
        /// <param name="customFieldAddViewModel"></param>
        /// <returns></returns>
        public CustomFieldAddViewModel AddCustomField(CustomFieldAddViewModel customFieldAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(customFieldAddViewModel.customFields.Type) && !string.IsNullOrWhiteSpace(customFieldAddViewModel.customFields.Module))
                    {
                        var checkCustomFieldTitle = this.context?.CustomFields.Where(x => (customFieldAddViewModel.customFields.IsSystemWideField == true) ? x.TenantId == customFieldAddViewModel.customFields.TenantId && x.Title.ToLower() == customFieldAddViewModel.customFields.Title.ToLower() : x.TenantId == customFieldAddViewModel.customFields.TenantId && x.SchoolId == customFieldAddViewModel.customFields.SchoolId && x.Title.ToLower() == customFieldAddViewModel.customFields.Title.ToLower()).FirstOrDefault();

                        if (checkCustomFieldTitle != null)
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "Custom Field Title Already Exists";
                        }
                        else
                        {
                            int? MasterFieldId = 1;
                            int? SortOrder = 1;

                            var customFieldAllDataList = this.context?.CustomFields.Where(c => c.TenantId == customFieldAddViewModel.customFields.TenantId && c.SchoolId == customFieldAddViewModel.customFields.SchoolId).ToList();
                            var CustomFieldData = customFieldAllDataList.OrderByDescending(x => x.FieldId).FirstOrDefault();

                            if (CustomFieldData != null)
                            {
                                MasterFieldId = CustomFieldData.FieldId + 1;
                            }

                            var SortOrderData = customFieldAllDataList.Where(x => x.CategoryId == customFieldAddViewModel.customFields.CategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                            if (SortOrderData != null)
                            {
                                SortOrder = SortOrderData.SortOrder + 1;
                            }

                            string fieldName = Regex.Replace(customFieldAddViewModel.customFields.Title, @"\s+", "");
                            customFieldAddViewModel.customFields.FieldId = (int)MasterFieldId;
                            customFieldAddViewModel.customFields.SortOrder = (int)SortOrder;
                            customFieldAddViewModel.customFields.CreatedOn = DateTime.UtcNow;
                            customFieldAddViewModel.customFields.CreatedBy = customFieldAddViewModel.customFields.CreatedBy;
                            customFieldAddViewModel.customFields.FieldName = fieldName;
                            customFieldAddViewModel.customFields.SystemField = false;
                            this.context?.CustomFields.Add(customFieldAddViewModel.customFields);
                            this.context?.SaveChanges();

                            if ((bool)customFieldAddViewModel.customFields.IsSystemWideField)
                            {
                                CustomFieldAddViewModel customFieldAdd = new CustomFieldAddViewModel();

                                var schoolDataList = this.context?.SchoolMaster.Where(x => x.TenantId == customFieldAddViewModel.customFields.TenantId && x.SchoolId != customFieldAddViewModel.customFields.SchoolId).ToList();

                                var fieldCategoryData = this.context?.FieldsCategory.Where(c => c.TenantId == customFieldAddViewModel.customFields.TenantId).ToList();

                                var customFieldDataList = this.context?.CustomFields.Where(c => c.TenantId == customFieldAddViewModel.customFields.TenantId).ToList();

                                if (schoolDataList.Count > 0)
                                {
                                    var fieldCategoryTitle = fieldCategoryData.FirstOrDefault(c => c.SchoolId == customFieldAddViewModel.customFields.SchoolId && c.CategoryId == customFieldAddViewModel.customFields.CategoryId)?.Title;

                                    if (fieldCategoryTitle != null)
                                    {
                                        foreach (var schoolData in schoolDataList)
                                        {
                                            var checkFieldCategory = fieldCategoryData.FirstOrDefault(c => c.SchoolId == schoolData.SchoolId && c.Title.ToLower() == fieldCategoryTitle.ToLower());

                                            if (checkFieldCategory != null)
                                            {
                                                int? FieldId = 1;
                                                int? sortOrder = 1;

                                                var CustomFieldsData = customFieldDataList.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.FieldId).FirstOrDefault();

                                                if (CustomFieldsData != null)
                                                {
                                                    FieldId = CustomFieldsData.FieldId + 1;
                                                }

                                                var sortOrderData = customFieldDataList.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId && x.CategoryId == checkFieldCategory.CategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                                                if (sortOrderData != null)
                                                {
                                                    sortOrder = sortOrderData.SortOrder + 1;
                                                }

                                                string FieldName = Regex.Replace(customFieldAddViewModel.customFields.Title, @"\s+", "");
                                                customFieldAdd.customFields.SchoolId = schoolData.SchoolId;
                                                customFieldAdd.customFields.TenantId = schoolData.TenantId;
                                                customFieldAdd.customFields.FieldId = (int)FieldId;
                                                customFieldAdd.customFields.Type = customFieldAddViewModel.customFields.Type;
                                                customFieldAdd.customFields.Search = customFieldAddViewModel.customFields.Search;
                                                customFieldAdd.customFields.Title = customFieldAddViewModel.customFields.Title;
                                                customFieldAdd.customFields.SortOrder = (int)sortOrder;
                                                customFieldAdd.customFields.SelectOptions = customFieldAddViewModel.customFields.SelectOptions;
                                                customFieldAdd.customFields.CategoryId = checkFieldCategory.CategoryId;
                                                customFieldAdd.customFields.SystemField = false;
                                                customFieldAdd.customFields.Required = customFieldAddViewModel.customFields.Required;
                                                customFieldAdd.customFields.DefaultSelection = customFieldAddViewModel.customFields.DefaultSelection;
                                                customFieldAdd.customFields.Hide = customFieldAddViewModel.customFields.Hide;
                                                customFieldAdd.customFields.CreatedOn = DateTime.UtcNow;
                                                customFieldAdd.customFields.CreatedBy = customFieldAddViewModel.customFields.CreatedBy;
                                                customFieldAdd.customFields.Module = customFieldAddViewModel.customFields.Module;
                                                customFieldAdd.customFields.IsSystemWideField = customFieldAddViewModel.customFields.IsSystemWideField;
                                                customFieldAdd.customFields.FieldName = FieldName;
                                                this.context?.CustomFields.Add(customFieldAdd.customFields);
                                                this.context?.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            customFieldAddViewModel._failure = false;
                            customFieldAddViewModel._message = "Custom Field Added Successfully";
                        }
                    }
                    else
                    {
                        customFieldAddViewModel.customFields = null;
                        customFieldAddViewModel._failure = true;
                        customFieldAddViewModel._message = "Please enter Type and Module";
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    customFieldAddViewModel._failure = true;
                    customFieldAddViewModel._message = es.Message;
                }
            }
            return customFieldAddViewModel;
        }

        /// <summary>
        /// Update Custom Field
        /// </summary>
        /// <param name="customFieldAddViewModel"></param>
        /// <returns></returns>
        public CustomFieldAddViewModel UpdateCustomField(CustomFieldAddViewModel customFieldAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    List<SchoolMaster> schoolMasterList = new List<SchoolMaster>();

                    if (!string.IsNullOrWhiteSpace(customFieldAddViewModel.customFields.Type) && !string.IsNullOrWhiteSpace(customFieldAddViewModel.customFields.Module))
                    {
                        var checkCustomFieldTitle = this.context?.CustomFields.Where(x => x.SchoolId == customFieldAddViewModel.customFields.SchoolId && x.TenantId == customFieldAddViewModel.customFields.TenantId && x.Title.ToLower() == customFieldAddViewModel.customFields.Title.ToLower() && x.FieldId != customFieldAddViewModel.customFields.FieldId).FirstOrDefault();

                        if (checkCustomFieldTitle != null)
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "Custom Field Title Already Exists";
                        }
                        else
                        {
                            var customFieldUpdate = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == customFieldAddViewModel.customFields.TenantId && x.SchoolId == customFieldAddViewModel.customFields.SchoolId && x.FieldId == customFieldAddViewModel.customFields.FieldId);

                            if (customFieldUpdate != null)
                            {
                                string FieldName = Regex.Replace(customFieldAddViewModel.customFields.Title, @"\s+", "");

                                if (customFieldUpdate.IsSystemWideField == true)
                                {
                                    schoolMasterList = this.context?.SchoolMaster.Include(x => x.CustomFields).Where(c => c.TenantId == customFieldAddViewModel.customFields.TenantId && c.SchoolId != customFieldAddViewModel.customFields.SchoolId).ToList();

                                    foreach (var schoolMaster in schoolMasterList)
                                    {
                                        var SchoolCustomField = schoolMaster.CustomFields.Where(x => x.SystemField != true).ToList();

                                        var SchoolCustomFieldUpdate = SchoolCustomField.FirstOrDefault(x => x.SchoolId == schoolMaster.SchoolId && x.Title.ToLower() == customFieldUpdate.Title.ToLower() && x.CategoryId == customFieldUpdate.CategoryId);

                                        if (SchoolCustomFieldUpdate != null)
                                        {
                                            var CheckCustomFieldTitle = SchoolCustomField.Where(x => x.SchoolId == schoolMaster.SchoolId && x.TenantId == customFieldAddViewModel.customFields.TenantId && x.Title.ToLower() == customFieldAddViewModel.customFields.Title.ToLower() && x.FieldId != SchoolCustomFieldUpdate.FieldId).FirstOrDefault();

                                            if (CheckCustomFieldTitle != null)
                                            {
                                                customFieldAddViewModel._failure = true;
                                                customFieldAddViewModel._message = "Custom Field Title Already Exists";
                                                return customFieldAddViewModel;
                                            }

                                            var customFieldValue = this.context?.CustomFieldsValue.FirstOrDefault(c => c.SchoolId == SchoolCustomFieldUpdate.SchoolId && c.TenantId == SchoolCustomFieldUpdate.TenantId && c.FieldId == SchoolCustomFieldUpdate.FieldId && c.CategoryId == SchoolCustomFieldUpdate.CategoryId);

                                            if (customFieldValue != null)
                                            {
                                                var CustomFieldDataUpdate = SchoolCustomField.FirstOrDefault(x => x.SchoolId == schoolMaster.SchoolId && x.Title.ToLower() == customFieldUpdate.Title.ToLower() && x.CategoryId == customFieldUpdate.CategoryId);

                                                if (CustomFieldDataUpdate != null)
                                                {
                                                    CustomFieldDataUpdate.Title = customFieldAddViewModel.customFields.Title;
                                                    CustomFieldDataUpdate.FieldName = FieldName;
                                                    CustomFieldDataUpdate.UpdatedBy = customFieldAddViewModel.customFields.UpdatedBy;
                                                    CustomFieldDataUpdate.UpdatedOn = DateTime.UtcNow;
                                                    CustomFieldDataUpdate.Required = customFieldAddViewModel.customFields.Required;
                                                    CustomFieldDataUpdate.Hide = customFieldAddViewModel.customFields.Hide;
                                                }
                                            }
                                            else
                                            {
                                                SchoolCustomFieldUpdate.Title = customFieldAddViewModel.customFields.Title;
                                                SchoolCustomFieldUpdate.FieldName = FieldName;
                                                SchoolCustomFieldUpdate.Type = customFieldAddViewModel.customFields.Type;
                                                SchoolCustomFieldUpdate.DefaultSelection = customFieldAddViewModel.customFields.DefaultSelection;
                                                SchoolCustomFieldUpdate.Required = customFieldAddViewModel.customFields.Required;
                                                SchoolCustomFieldUpdate.Hide = customFieldAddViewModel.customFields.Hide;
                                                SchoolCustomFieldUpdate.UpdatedBy = customFieldAddViewModel.customFields.UpdatedBy;
                                                SchoolCustomFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                            }
                                            this.context.SaveChanges();
                                        }
                                    }
                                }
                                var customFieldsValueExists = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldUpdate.TenantId && x.SchoolId == customFieldUpdate.SchoolId && x.CategoryId == customFieldUpdate.CategoryId && x.FieldId == customFieldUpdate.FieldId);

                                if (customFieldsValueExists != null)
                                {
                                    customFieldUpdate.Title = customFieldAddViewModel.customFields.Title;
                                    customFieldUpdate.FieldName = FieldName;
                                    customFieldUpdate.UpdatedBy = customFieldAddViewModel.customFields.UpdatedBy;
                                    customFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                    customFieldUpdate.Required = customFieldAddViewModel.customFields.Required;
                                    customFieldUpdate.Hide = customFieldAddViewModel.customFields.Hide;
                                }
                                else
                                {
                                    customFieldUpdate.Title = customFieldAddViewModel.customFields.Title;
                                    customFieldUpdate.FieldName = FieldName;
                                    customFieldUpdate.Type = customFieldAddViewModel.customFields.Type;
                                    customFieldUpdate.DefaultSelection = customFieldAddViewModel.customFields.DefaultSelection;
                                    customFieldUpdate.Required = customFieldAddViewModel.customFields.Required;
                                    customFieldUpdate.Hide = customFieldAddViewModel.customFields.Hide;
                                    customFieldUpdate.UpdatedBy = customFieldAddViewModel.customFields.UpdatedBy;
                                    customFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                }
                                this.context?.SaveChanges();
                                transaction.Commit();
                                customFieldAddViewModel._failure = false;
                                customFieldAddViewModel._message = "Custom Field Updated Successfully";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    customFieldAddViewModel.customFields = null;
                    customFieldAddViewModel._failure = true;
                    customFieldAddViewModel._message = ex.Message;
                }
            }
            return customFieldAddViewModel;
        }

        /// <summary>
        /// Delete Custom Field
        /// </summary>
        /// <param name="customFieldAddViewModel"></param>
        /// <returns></returns>
        public CustomFieldAddViewModel DeleteCustomField(CustomFieldAddViewModel customFieldAddViewModel)
        {
            try
            {
                List<CustomFields> customFieldList = new List<CustomFields>();
                var customFieldDelete = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == customFieldAddViewModel.customFields.TenantId && x.SchoolId == customFieldAddViewModel.customFields.SchoolId && x.FieldId == customFieldAddViewModel.customFields.FieldId);

                if (customFieldDelete != null)
                {
                    var customFieldsValueExists = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == customFieldDelete.SchoolId && x.CategoryId == customFieldDelete.CategoryId && x.FieldId == customFieldDelete.FieldId);

                    if (customFieldsValueExists != null)
                    {
                        if (!string.IsNullOrWhiteSpace(customFieldsValueExists.CustomFieldValue))
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "It Can't Be Deleted Because It Has Association";
                            return customFieldAddViewModel;
                        }

                    }
                    else
                    {
                        customFieldList.Add(customFieldDelete);
                    }

                    if (customFieldDelete.IsSystemWideField == true)
                    {
                        var schoolMasterList = this.context?.SchoolMaster.Include(x => x.CustomFields).Where(c => c.TenantId == customFieldAddViewModel.customFields.TenantId && c.SchoolId != customFieldAddViewModel.customFields.SchoolId).ToList();

                        foreach (var schoolMaster in schoolMasterList)
                        {
                            var SchoolCustomField = schoolMaster.CustomFields.Where(x => x.SystemField != true).ToList();

                            var CustomFieldSchoolWise = SchoolCustomField.FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == schoolMaster.SchoolId && x.CategoryId == customFieldDelete.CategoryId && x.Title.ToLower() == customFieldDelete.Title.ToLower());
                            if (CustomFieldSchoolWise != null)
                            {
                                var customFieldsValueExistsSchoolWise = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == CustomFieldSchoolWise.SchoolId && x.CategoryId == CustomFieldSchoolWise.CategoryId && x.FieldId == CustomFieldSchoolWise.FieldId);
                                if (customFieldsValueExistsSchoolWise != null)
                                {
                                    customFieldAddViewModel._failure = true;
                                    customFieldAddViewModel._message = "It Can't Be Deleted Because It Has Association";
                                    return customFieldAddViewModel;
                                }
                                else
                                {
                                    customFieldList.Add(CustomFieldSchoolWise);
                                }
                            }

                        }
                    }

                    this.context?.CustomFields.RemoveRange(customFieldList);
                    this.context?.SaveChanges();
                    customFieldAddViewModel._failure = false;
                    customFieldAddViewModel._message = "Custom Field Deleted Successfully";
                }
            }
            catch (Exception es)
            {
                customFieldAddViewModel._failure = true;
                customFieldAddViewModel._message = es.Message;
            }
            return customFieldAddViewModel;
        }

        /// <summary>
        /// Add Field Category
        /// </summary>
        /// <param name="fieldsCategoryAddViewModel"></param>
        /// <returns></returns>
        public FieldsCategoryAddViewModel AddFieldsCategory(FieldsCategoryAddViewModel fieldsCategoryAddViewModel)
        {
            List<FieldsCategory> fieldsCategoryList = new List<FieldsCategory>();
            List<PermissionSubcategory> permissionSubcategoryList = new List<PermissionSubcategory>();
            List<RolePermission> rolePermissionList = new List<RolePermission>();

            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var checkFieldCategoryTitle = this.context?.FieldsCategory.Where(x => /*x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId &&*/ x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.Title.ToLower() == fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower() && x.IsSystemWideCategory == true).FirstOrDefault();

                    if (checkFieldCategoryTitle != null)
                    {
                        fieldsCategoryAddViewModel._failure = true;
                        fieldsCategoryAddViewModel._message = "Field Category Title Already Exists";
                    }
                    else
                    {
                        //int? CategoryId = Utility.GetMaxPK(this.context, new Func<FieldsCategory, int>(x => x.CategoryId));
                        //int? CategoryId = 1;

                        //var FieldCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId).OrderByDescending(x => x.CategoryId).FirstOrDefault();

                        //if (FieldCategoryData != null)
                        //{
                        //    CategoryId = FieldCategoryData.CategoryId + 1;
                        //}

                        //fieldsCategoryAddViewModel.fieldsCategory.IsSystemWideCategory = true;
                        //fieldsCategoryAddViewModel.fieldsCategory.CategoryId = (int)CategoryId;
                        //fieldsCategoryAddViewModel.fieldsCategory.LastUpdate = DateTime.UtcNow;
                        //this.context?.FieldsCategory.Add(fieldsCategoryAddViewModel.fieldsCategory);


                        var schoolDataList = this.context?.SchoolMaster.Where(c => c.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId).ToList();

                        if (schoolDataList.Count > 0)
                        {
                            foreach (var schoolData in schoolDataList)
                            {
                                int? CategoryId = 1;

                                var FieldCategory = this.context?.FieldsCategory.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.CategoryId).FirstOrDefault();

                                if (FieldCategory != null)
                                {
                                    CategoryId = FieldCategory.CategoryId + 1;
                                }

                                //fieldsCategoryAddViewModel.fieldsCategory.IsSystemWideCategory = true;
                                //fieldsCategoryAddViewModel.fieldsCategory.CategoryId = (int)CategoryId;
                                //fieldsCategoryAddViewModel.fieldsCategory.LastUpdate = DateTime.UtcNow;
                                //this.context?.FieldsCategory.Add(fieldsCategoryAddViewModel.fieldsCategory);

                                var fieldCategory = new FieldsCategory()
                                {
                                    TenantId = schoolData.TenantId,
                                    SchoolId = schoolData.SchoolId,
                                    CategoryId = (int)CategoryId,
                                    Hide = fieldsCategoryAddViewModel.fieldsCategory.Hide,
                                    IsSystemCategory = false,
                                    IsSystemWideCategory = true,
                                    Search = fieldsCategoryAddViewModel.fieldsCategory.Search,
                                    Module = fieldsCategoryAddViewModel.fieldsCategory.Module,
                                    Required = fieldsCategoryAddViewModel.fieldsCategory.Required,
                                    SortOrder = fieldsCategoryAddViewModel.fieldsCategory.SortOrder,
                                    Title = fieldsCategoryAddViewModel.fieldsCategory.Title,
                                    CreatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy,
                                    CreatedOn = DateTime.UtcNow
                                };

                                fieldsCategoryList.Add(fieldCategory);


                                var permissionGroupData = this.context?.PermissionGroup.FirstOrDefault(x => x.TenantId == schoolData.TenantId && x.SchoolId == schoolData.SchoolId && x.PermissionGroupName.ToLower().Contains(fieldsCategoryAddViewModel.fieldsCategory.Module.ToLower()));

                                if (permissionGroupData != null)
                                {
                                    var permissionCategory = this.context?.PermissionCategory.FirstOrDefault(e => e.PermissionGroupId == permissionGroupData.PermissionGroupId && e.TenantId == permissionGroupData.TenantId && e.SchoolId == permissionGroupData.SchoolId && e.PermissionGroupId == permissionGroupData.PermissionGroupId);

                                    if (permissionCategory != null)
                                    {
                                        var checkPermissionSubCategoryName = this.context?.PermissionSubcategory.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId && x.PermissionSubcategoryName.ToLower() == fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower()).FirstOrDefault();

                                        if (checkPermissionSubCategoryName != null)
                                        {
                                            fieldsCategoryAddViewModel._failure = true;
                                            fieldsCategoryAddViewModel._message = "Permission Subcategory Name Already Exists";
                                        }
                                        else
                                        {
                                            int? PermissionSubCategoryId = 1;

                                            var permissionSubCategoryData = this.context?.PermissionSubcategory.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.PermissionSubcategoryId).FirstOrDefault();

                                            if (permissionSubCategoryData != null)
                                            {
                                                PermissionSubCategoryId = permissionSubCategoryData.PermissionSubcategoryId + 1;
                                            }

                                            string path = null;

                                            string[] titleArray = fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower().Split(" ");

                                            var module = fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower() == "school" ? "schoolinfo" : fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower();

                                            if (module == "student")
                                            {
                                                module = "students";
                                            }

                                            path = "/school/" + module + "/custom/" + string.Join("-", titleArray);

                                            var permissionSubCategory = new PermissionSubcategory()
                                            {
                                                TenantId = schoolData.TenantId,
                                                SchoolId = schoolData.SchoolId,
                                                PermissionSubcategoryId = (int)PermissionSubCategoryId,
                                                PermissionGroupId = permissionCategory.PermissionGroupId,
                                                PermissionCategoryId = permissionCategory.PermissionCategoryId,
                                                PermissionSubcategoryName = fieldsCategoryAddViewModel.fieldsCategory.Title,
                                                Path = path,
                                                Title = fieldsCategoryAddViewModel.fieldsCategory.Title,
                                                EnableView = true,
                                                EnableAdd = true,
                                                EnableEdit = true,
                                                EnableDelete = true,
                                                CreatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                IsActive = true
                                            };
                                            //this.context?.PermissionSubcategory.Add(permissionSubCategory);
                                            permissionSubcategoryList.Add(permissionSubCategory);

                                            int? rolePermissionId = 1;

                                            var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                                            if (rolePermissionData != null)
                                            {
                                                rolePermissionId = rolePermissionData.RolePermissionId + 1;
                                            }
                                            var rolePermission = new RolePermission()
                                            {
                                                TenantId = schoolData.TenantId,
                                                SchoolId = schoolData.SchoolId,
                                                RolePermissionId = (int)rolePermissionId,
                                                PermissionGroupId = null,
                                                PermissionCategoryId = null,
                                                PermissionSubcategoryId = (int)PermissionSubCategoryId,
                                                CanView = true,
                                                CanAdd = true,
                                                CanEdit = true,
                                                CanDelete = true,
                                                CreatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                MembershipId = this.context?.Membership.FirstOrDefault(e => e.SchoolId == schoolData.SchoolId && e.TenantId == schoolData.TenantId)?.MembershipId
                                            };
                                            //this.context?.RolePermission.Add(rolePermission);
                                            rolePermissionList.Add(rolePermission);
                                        }
                                    }
                                }
                            }
                        }
                        this.context?.FieldsCategory.AddRange(fieldsCategoryList);
                        this.context?.PermissionSubcategory.AddRange(permissionSubcategoryList);
                        this.context?.RolePermission.AddRange(rolePermissionList);
                        this.context?.SaveChanges();
                        transaction.Commit();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field Category Added Successfully";
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    fieldsCategoryAddViewModel._failure = false;
                    fieldsCategoryAddViewModel._message = es.Message;
                }
            }
            return fieldsCategoryAddViewModel;
        }

        /// <summary>
        /// Update Field Category 
        /// </summary>
        /// <param name="fieldsCategoryAddViewModel"></param>
        /// <returns></returns>
        public FieldsCategoryAddViewModel UpdateFieldsCategory(FieldsCategoryAddViewModel fieldsCategoryAddViewModel)
        {
            FieldsCategoryAddViewModel fieldsCategoryUpdateModel = new FieldsCategoryAddViewModel();
            try
            {
                var fieldCategoryData = this.context?.FieldsCategory.Where(c => c.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId).ToList();


                var checkFieldCategoryTitle = fieldCategoryData.Where(x => /*x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId &&*/ x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.Title.ToLower() == fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower() && x.IsSystemWideCategory == true && x.CategoryId != fieldsCategoryAddViewModel.fieldsCategory.CategoryId).FirstOrDefault();

                if (checkFieldCategoryTitle != null)
                {
                    fieldsCategoryAddViewModel._failure = true;
                    fieldsCategoryAddViewModel._message = "Field Category Title Already Exists";
                }
                else
                {
                    if (fieldsCategoryAddViewModel.fieldsCategory.IsSystemWideCategory == true)
                    {
                        var fieldsCategoryUpdate = fieldCategoryData.FirstOrDefault(x => /*x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId &&*/ x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.CategoryId == fieldsCategoryAddViewModel.fieldsCategory.CategoryId);

                        if (fieldsCategoryUpdate != null)
                        {
                            var permissionSubCategoryData = this.context?.PermissionSubcategory.FirstOrDefault(c => c.SchoolId == fieldsCategoryUpdate.SchoolId && c.TenantId == fieldsCategoryUpdate.TenantId && c.PermissionSubcategoryName.ToLower() == fieldsCategoryUpdate.Title.ToLower() && c.Title.ToLower() == fieldsCategoryUpdate.Title.ToLower());

                            if (permissionSubCategoryData != null)
                            {
                                permissionSubCategoryData.Title = fieldsCategoryAddViewModel.fieldsCategory.Title;
                                permissionSubCategoryData.PermissionSubcategoryName = fieldsCategoryAddViewModel.fieldsCategory.Title;
                                permissionSubCategoryData.UpdatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy;
                                permissionSubCategoryData.UpdatedOn = DateTime.UtcNow;

                                string path = null;

                                string[] titleArray = fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower().Split(" ");

                                var module = fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower() == "school" ? "schoolinfo" : fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower();

                                if (module == "student")
                                {
                                    module = "students";
                                }
                                path = "/school/" + module + "/custom/" + string.Join("-", titleArray);
                                permissionSubCategoryData.Path = path;
                            }

                            var schoolDataList = this.context?.SchoolMaster.Where(v => v.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && v.SchoolId != fieldsCategoryAddViewModel.fieldsCategory.SchoolId).ToList();

                            if (schoolDataList.Count > 0)
                            {
                                foreach (var schoolData in schoolDataList)
                                {
                                    var fieldUpdate = fieldCategoryData.FirstOrDefault(x => /*x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId &&*/ x.SchoolId == schoolData.SchoolId && x.Title.ToLower() == fieldsCategoryUpdate.Title.ToLower());

                                    if (fieldUpdate != null)
                                    {
                                        var permissionSubCategory = this.context?.PermissionSubcategory.FirstOrDefault(c => c.SchoolId == fieldUpdate.SchoolId && c.TenantId == fieldUpdate.TenantId && c.PermissionSubcategoryName.ToLower() == fieldUpdate.Title.ToLower() && c.Title.ToLower() == fieldUpdate.Title.ToLower());

                                        if (permissionSubCategory != null)
                                        {
                                            permissionSubCategory.Title = fieldsCategoryAddViewModel.fieldsCategory.Title;
                                            permissionSubCategory.PermissionSubcategoryName = fieldsCategoryAddViewModel.fieldsCategory.Title;
                                            permissionSubCategory.UpdatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy;
                                            permissionSubCategory.UpdatedOn = DateTime.UtcNow;

                                            string path = null;

                                            string[] titleArray = fieldsCategoryAddViewModel.fieldsCategory.Title.ToLower().Split(" ");

                                            var module = fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower() == "school" ? "schoolinfo" : fieldsCategoryAddViewModel.fieldsCategory.Module.ToString().ToLower();

                                            path = "/school/" + module + "/custom/" + string.Join("-", titleArray);
                                            permissionSubCategory.Path = path;
                                        }

                                        fieldUpdate.Title = fieldsCategoryAddViewModel.fieldsCategory.Title;
                                        fieldUpdate.SortOrder = fieldsCategoryAddViewModel.fieldsCategory.SortOrder;
                                        fieldUpdate.UpdatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy;
                                        fieldUpdate.UpdatedOn = DateTime.UtcNow;

                                    }
                                }
                            }

                            fieldsCategoryUpdate.Title = fieldsCategoryAddViewModel.fieldsCategory.Title;
                            fieldsCategoryUpdate.SortOrder = fieldsCategoryAddViewModel.fieldsCategory.SortOrder;
                            fieldsCategoryUpdate.UpdatedBy = fieldsCategoryAddViewModel.fieldsCategory.UpdatedBy;
                            fieldsCategoryUpdate.UpdatedOn = DateTime.UtcNow;

                        }
                        this.context?.SaveChanges();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field Category Updated Successfully";
                    }
                }
            }
            catch (Exception es)
            {
                fieldsCategoryAddViewModel._failure = true;
                fieldsCategoryAddViewModel._message = es.Message;
            }
            return fieldsCategoryAddViewModel;
        }

        /// <summary>
        /// Delete Field Category
        /// </summary>
        /// <param name="fieldsCategoryAddViewModel"></param>
        /// <returns></returns>
        public FieldsCategoryAddViewModel DeleteFieldsCategory(FieldsCategoryAddViewModel fieldsCategoryAddViewModel)
        {
            try
            {
                List<FieldsCategory> fieldsCategories = new List<FieldsCategory>();
                List<PermissionSubcategory> permissionSubcategories = new List<PermissionSubcategory>();
                List<RolePermission> rolePermissions = new List<RolePermission>();

                var fieldsCategoryDelete = this.context?.FieldsCategory.FirstOrDefault(x => x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.CategoryId == fieldsCategoryAddViewModel.fieldsCategory.CategoryId);

                if (fieldsCategoryDelete != null)
                {
                    var customFieldsExists = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == fieldsCategoryDelete.TenantId && x.SchoolId == fieldsCategoryDelete.SchoolId && x.CategoryId == fieldsCategoryDelete.CategoryId);

                    if (customFieldsExists != null)
                    {
                        fieldsCategoryAddViewModel._failure = true;
                        fieldsCategoryAddViewModel._message = "It Can't Be Deleted Because It Has Association";
                        return fieldsCategoryAddViewModel;
                    }
                    else
                    {
                        fieldsCategories.Add(fieldsCategoryDelete);

                        var permissionSubCategory = this.context?.PermissionSubcategory.FirstOrDefault(c => c.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && c.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && c.PermissionSubcategoryName.ToLower() == fieldsCategoryDelete.Title.ToLower() && c.Title.ToLower() == fieldsCategoryDelete.Title.ToLower());

                        if (permissionSubCategory != null)
                        {
                            var rolePermissionData = this.context?.RolePermission.FirstOrDefault(x => x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.PermissionSubcategoryId == permissionSubCategory.PermissionSubcategoryId);

                            if (rolePermissionData != null)
                            {
                                rolePermissions.Add(rolePermissionData);
                            }

                            permissionSubcategories.Add(permissionSubCategory);
                        }

                        var schoolMasterList = this.context?.SchoolMaster.Where(v => v.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && v.SchoolId != fieldsCategoryAddViewModel.fieldsCategory.SchoolId).ToList();

                        if (schoolMasterList.Count > 0)
                        {
                            foreach (var schoolMaster in schoolMasterList)
                            {
                                var fieldCategoryData = this.context?.FieldsCategory.Include(x => x.CustomFields).FirstOrDefault(c => c.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && c.SchoolId == schoolMaster.SchoolId && c.Title.ToLower() == fieldsCategoryDelete.Title.ToLower());

                                if (fieldCategoryData.CustomFields.Count() > 0)
                                {
                                    fieldsCategoryAddViewModel._failure = true;
                                    fieldsCategoryAddViewModel._message = "It Can't Be Deleted Because It Has Association";
                                    return fieldsCategoryAddViewModel;
                                }
                                else
                                {
                                    var permissionSubCategoryData = this.context?.PermissionSubcategory.FirstOrDefault(c => c.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && c.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && c.PermissionSubcategoryName.ToLower() == fieldsCategoryDelete.Title.ToLower() && c.Title.ToLower() == fieldsCategoryDelete.Title.ToLower());

                                    if (permissionSubCategoryData != null)
                                    {
                                        var rolePermissionData = this.context?.RolePermission.FirstOrDefault(x => x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.PermissionSubcategoryId == permissionSubCategoryData.PermissionSubcategoryId);

                                        if (rolePermissionData != null)
                                        {
                                            rolePermissions.Add(rolePermissionData);
                                        }

                                        permissionSubcategories.Add(permissionSubCategoryData);
                                    }
                                    fieldsCategories.Add(fieldCategoryData);
                                }
                            }
                        }
                        this.context?.RolePermission.RemoveRange(rolePermissions);
                        this.context?.PermissionSubcategory.RemoveRange(permissionSubcategories);
                        this.context?.FieldsCategory.RemoveRange(fieldsCategories);
                        this.context?.SaveChanges();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field Category Deleted Successfully";
                    }
                }
            }
            catch (Exception es)
            {
                fieldsCategoryAddViewModel._failure = true;
                fieldsCategoryAddViewModel._message = es.Message;
            }
            return fieldsCategoryAddViewModel;
        }

        /// <summary>
        /// Get All Field Category
        /// </summary>
        /// <param name="fieldsCategoryListViewModel"></param>
        /// <returns></returns>
        public FieldsCategoryListViewModel GetAllFieldsCategory(FieldsCategoryListViewModel fieldsCategoryListViewModel)
        {
            FieldsCategoryListViewModel fieldsCategoryListModel = new FieldsCategoryListViewModel();
            try
            {

                var fieldsCategoryList = this.context?.FieldsCategory
                    .Include(x => x.CustomFields)
                    .Where(x => x.TenantId == fieldsCategoryListViewModel.TenantId &&
                                x.SchoolId == fieldsCategoryListViewModel.SchoolId &&
                                x.Module == fieldsCategoryListViewModel.Module)
                    .OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder).Select(e => new FieldsCategory()
                    {
                        TenantId = e.TenantId,
                        SchoolId = e.SchoolId,
                        CategoryId = e.CategoryId,
                        IsSystemCategory = e.IsSystemCategory,
                        Search = e.Search,
                        Title = e.Title,
                        Module = e.Module,
                        SortOrder = e.SortOrder,
                        Required = e.Required,
                        Hide = e.Hide,
                        IsSystemWideCategory = e.IsSystemWideCategory,
                        CreatedOn = e.CreatedOn,
                        UpdatedOn = e.UpdatedOn,
                        CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == fieldsCategoryListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                        UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == fieldsCategoryListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                        CustomFields = e.CustomFields.Select(e => new CustomFields()
                        {
                            TenantId = e.TenantId,
                            SchoolId = e.SchoolId,
                            FieldId = e.FieldId,
                            Type = e.Type,
                            Search = e.Search,
                            Title = e.Title,
                            SortOrder = e.SortOrder,
                            SelectOptions = e.SelectOptions,
                            CategoryId = e.CategoryId,
                            SystemField = e.SystemField,
                            Required = e.Required,
                            DefaultSelection = e.DefaultSelection,
                            Hide = e.Hide,
                            Module = e.Module,
                            FieldName = e.FieldName,
                            IsSystemWideField = e.IsSystemWideField,
                            CreatedOn = e.CreatedOn,
                            UpdatedOn = e.UpdatedOn,
                            CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == fieldsCategoryListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                            UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == fieldsCategoryListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null
                        }).ToList()
                    }).ToList();

                if (fieldsCategoryList.Count > 0)
                {
                    foreach (var fieldsCategory in fieldsCategoryList)
                    {
                        fieldsCategory.CustomFields = fieldsCategory.CustomFields.OrderByDescending(y => y.SystemField).ThenBy(y => y.SortOrder).ToList();
                    }
                    fieldsCategoryListModel._failure = false;
                }
                else
                {
                    fieldsCategoryListModel._failure = true;
                    fieldsCategoryListModel._message = NORECORDFOUND;
                }
                fieldsCategoryListModel.fieldsCategoryList = fieldsCategoryList;
                fieldsCategoryListModel._tenantName = fieldsCategoryListViewModel._tenantName;
                fieldsCategoryListModel._token = fieldsCategoryListViewModel._token;

            }
            catch (Exception es)
            {
                fieldsCategoryListModel._message = es.Message;
                fieldsCategoryListModel._failure = true;
                fieldsCategoryListModel._tenantName = fieldsCategoryListViewModel._tenantName;
                fieldsCategoryListModel._token = fieldsCategoryListViewModel._token;
            }
            //fieldsCategoryListModel.fieldsCategoryList.ToList().ForEach(x => x.CustomFields.ToList().ForEach(y => y.FieldsCategory = null));
            return fieldsCategoryListModel;
        }

        /// <summary>
        /// Update Custom Field Sort Order
        /// </summary>
        /// <param name="customFieldSortOrderModel"></param>
        /// <returns></returns>

        public CustomFieldSortOrderModel UpdateCustomFieldSortOrder(CustomFieldSortOrderModel customFieldSortOrderModel)
        {
            try
            {
                var customFieldRecords = new List<CustomFields>();

                var targetCustomField = this.context?.CustomFields.FirstOrDefault(x => x.SortOrder == customFieldSortOrderModel.PreviousSortOrder && x.SchoolId == customFieldSortOrderModel.SchoolId && x.CategoryId == customFieldSortOrderModel.CategoryId);
                targetCustomField.SortOrder = customFieldSortOrderModel.CurrentSortOrder;
                targetCustomField.UpdatedBy = customFieldSortOrderModel.UpdatedBy;
                targetCustomField.UpdatedOn = DateTime.UtcNow;

                if (customFieldSortOrderModel.PreviousSortOrder > customFieldSortOrderModel.CurrentSortOrder)
                {
                    customFieldRecords = this.context?.CustomFields.Where(x => x.SortOrder >= customFieldSortOrderModel.CurrentSortOrder && x.SortOrder < customFieldSortOrderModel.PreviousSortOrder && x.SchoolId == customFieldSortOrderModel.SchoolId && x.CategoryId == customFieldSortOrderModel.CategoryId).ToList();

                    if (customFieldRecords.Count > 0)
                    {
                        customFieldRecords.ForEach(x => { x.SortOrder = x.SortOrder + 1; x.UpdatedBy = customFieldSortOrderModel.UpdatedBy; x.UpdatedOn = DateTime.UtcNow; });
                    }
                }
                if (customFieldSortOrderModel.CurrentSortOrder > customFieldSortOrderModel.PreviousSortOrder)
                {
                    customFieldRecords = this.context?.CustomFields.Where(x => x.SortOrder <= customFieldSortOrderModel.CurrentSortOrder && x.SortOrder > customFieldSortOrderModel.PreviousSortOrder && x.SchoolId == customFieldSortOrderModel.SchoolId && x.CategoryId == customFieldSortOrderModel.CategoryId).ToList();
                    if (customFieldRecords.Count > 0)
                    {
                        customFieldRecords.ForEach(x => { x.SortOrder = x.SortOrder - 1; x.UpdatedBy = customFieldSortOrderModel.UpdatedBy; x.UpdatedOn = DateTime.UtcNow; });
                    }
                }
                this.context?.SaveChanges();
                customFieldSortOrderModel._failure = false;
                customFieldSortOrderModel._message = "Custom Field Sort Order Updated Successfully";
            }
            catch (Exception es)
            {
                customFieldSortOrderModel._message = es.Message;
                customFieldSortOrderModel._failure = true;
            }
            return customFieldSortOrderModel;
        }
    }
}
