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
        private readonly CRMContext? context;
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
            if(customFieldAddViewModel.CustomFields.Title is null)
            {
                return customFieldAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<CustomFields> customFieldList = new List<CustomFields>();
                    if (!string.IsNullOrWhiteSpace(customFieldAddViewModel.CustomFields.Type) && !string.IsNullOrWhiteSpace(customFieldAddViewModel.CustomFields.Module))
                    {
                        var checkCustomFieldTitle = this.context?.CustomFields.AsEnumerable().Where(x => (customFieldAddViewModel.CustomFields.IsSystemWideField == true) ? x.TenantId == customFieldAddViewModel.CustomFields.TenantId && x.Title == customFieldAddViewModel.CustomFields.Title : x.TenantId == customFieldAddViewModel.CustomFields.TenantId && x.SchoolId == customFieldAddViewModel.CustomFields.SchoolId && String.Compare(x.Title,customFieldAddViewModel.CustomFields.Title,true)==0).FirstOrDefault();

                        if (checkCustomFieldTitle != null)
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "Custom Field Title Already Exists";
                        }
                        else
                        {
                            int? MasterFieldId = 1;
                            int? SortOrder = 1;

                            var customFieldAllDataList = this.context?.CustomFields.Where(c => c.TenantId == customFieldAddViewModel.CustomFields.TenantId && c.SchoolId == customFieldAddViewModel.CustomFields.SchoolId).ToList();
                            if (customFieldAllDataList != null)
                            {
                                var CustomFieldData = customFieldAllDataList.OrderByDescending(x => x.FieldId).FirstOrDefault();

                                if (CustomFieldData != null)
                                {
                                    MasterFieldId = CustomFieldData.FieldId + 1;
                                }

                                var SortOrderData = customFieldAllDataList.Where(x => x.CategoryId == customFieldAddViewModel.CustomFields.CategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                                if (SortOrderData != null)
                                {
                                    SortOrder = SortOrderData.SortOrder + 1;
                                }

                                string fieldName = Regex.Replace(customFieldAddViewModel.CustomFields.Title, @"\s+", "");
                                customFieldAddViewModel.CustomFields.FieldId = (int)MasterFieldId;
                                customFieldAddViewModel.CustomFields.SortOrder = (int)SortOrder!;
                                customFieldAddViewModel.CustomFields.CreatedOn = DateTime.UtcNow;
                                customFieldAddViewModel.CustomFields.CreatedBy = customFieldAddViewModel.CustomFields.CreatedBy;
                                customFieldAddViewModel.CustomFields.FieldName = fieldName;
                                customFieldAddViewModel.CustomFields.SystemField = false;
                                customFieldList.Add(customFieldAddViewModel.CustomFields);
                            }
                            
                            //this.context?.CustomFields.Add(customFieldAddViewModel.customFields);
                            //this.context?.SaveChanges();

                            if (customFieldAddViewModel.CustomFields?.IsSystemWideField==true)
                            {
                                //CustomFieldAddViewModel customFieldAdd = new CustomFieldAddViewModel();

                                var schoolDataList = this.context?.SchoolMaster.Where(x => x.TenantId == customFieldAddViewModel.CustomFields.TenantId && x.SchoolId != customFieldAddViewModel.CustomFields.SchoolId).ToList();

                                var fieldCategoryData = this.context?.FieldsCategory.Where(c => c.TenantId == customFieldAddViewModel.CustomFields.TenantId).ToList();

                                var customFieldDataList = this.context?.CustomFields.Where(c => c.TenantId == customFieldAddViewModel.CustomFields.TenantId).ToList();

                                if (schoolDataList!=null && fieldCategoryData!=null && customFieldDataList!=null)
                                {
                                    var fieldCategoryTitle = fieldCategoryData.FirstOrDefault(c => c.SchoolId == customFieldAddViewModel.CustomFields.SchoolId && c.CategoryId == customFieldAddViewModel.CustomFields.CategoryId);

                                    if (fieldCategoryTitle != null)
                                    {
                                        foreach (var schoolData in schoolDataList)
                                        {
                                            CustomFieldAddViewModel customFieldAdd = new CustomFieldAddViewModel();
                                            var checkFieldCategory = fieldCategoryData.FirstOrDefault(c => c.SchoolId == schoolData.SchoolId && String.Compare(c.Title,fieldCategoryTitle.Title,true)==0 && c.Module==fieldCategoryTitle.Module);

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

                                                string FieldName = Regex.Replace(customFieldAddViewModel.CustomFields.Title, @"\s+", "");
                                                customFieldAdd.CustomFields.SchoolId = schoolData.SchoolId;
                                                customFieldAdd.CustomFields.TenantId = schoolData.TenantId;
                                                customFieldAdd.CustomFields.FieldId = (int)FieldId;
                                                customFieldAdd.CustomFields.Type = customFieldAddViewModel.CustomFields.Type;
                                                customFieldAdd.CustomFields.Search = customFieldAddViewModel.CustomFields.Search;
                                                customFieldAdd.CustomFields.Title = customFieldAddViewModel.CustomFields.Title;
                                                customFieldAdd.CustomFields.SortOrder = (int)sortOrder!;
                                                customFieldAdd.CustomFields.SelectOptions = customFieldAddViewModel.CustomFields.SelectOptions;
                                                customFieldAdd.CustomFields.CategoryId = checkFieldCategory.CategoryId;
                                                customFieldAdd.CustomFields.SystemField = false;
                                                customFieldAdd.CustomFields.Required = customFieldAddViewModel.CustomFields.Required;
                                                customFieldAdd.CustomFields.DefaultSelection = customFieldAddViewModel.CustomFields.DefaultSelection;
                                                customFieldAdd.CustomFields.Hide = customFieldAddViewModel.CustomFields.Hide;
                                                customFieldAdd.CustomFields.CreatedOn = DateTime.UtcNow;
                                                customFieldAdd.CustomFields.CreatedBy = customFieldAddViewModel.CustomFields.CreatedBy;
                                                customFieldAdd.CustomFields.Module = customFieldAddViewModel.CustomFields.Module;
                                                customFieldAdd.CustomFields.IsSystemWideField = customFieldAddViewModel.CustomFields.IsSystemWideField;
                                                customFieldAdd.CustomFields.FieldName = FieldName;
                                                customFieldList.Add(customFieldAdd.CustomFields);
                                                //this.context?.CustomFields.Add(customFieldAdd.customFields);
                                                //this.context?.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            this.context?.CustomFields.AddRange(customFieldList);
                            this.context?.SaveChanges();
                            transaction?.Commit();
                            customFieldAddViewModel._failure = false;
                            customFieldAddViewModel._message = "Custom Field Added Successfully";
                        }
                    }
                    else
                    {
                        customFieldAddViewModel.CustomFields = null!;
                        customFieldAddViewModel._failure = true;
                        customFieldAddViewModel._message = "Please enter Type and Module";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
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
            if(customFieldAddViewModel.CustomFields.Title is null)
            {
                return customFieldAddViewModel;
            }
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<SchoolMaster>? schoolMasterList = new();

                    if (!string.IsNullOrWhiteSpace(customFieldAddViewModel.CustomFields.Type) && !string.IsNullOrWhiteSpace(customFieldAddViewModel.CustomFields.Module))
                    {
                        var checkCustomFieldTitle = this.context?.CustomFields.AsEnumerable().Where(x => x.SchoolId == customFieldAddViewModel.CustomFields.SchoolId && x.TenantId == customFieldAddViewModel.CustomFields.TenantId && String.Compare(x.Title , customFieldAddViewModel.CustomFields.Title,true)==0 && x.FieldId != customFieldAddViewModel.CustomFields.FieldId).FirstOrDefault();

                        if (checkCustomFieldTitle != null)
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "Custom Field Title Already Exists";
                        }
                        else
                        {
                            var customFieldUpdate = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == customFieldAddViewModel.CustomFields.TenantId && x.SchoolId == customFieldAddViewModel.CustomFields.SchoolId && x.FieldId == customFieldAddViewModel.CustomFields.FieldId);

                            if (customFieldUpdate != null)
                            {
                                string FieldName = Regex.Replace(customFieldAddViewModel.CustomFields.Title, @"\s+", "");

                                if (customFieldUpdate.IsSystemWideField == true)
                                {
                                    schoolMasterList = this.context?.SchoolMaster.Include(x => x.CustomFields).Where(c => c.TenantId == customFieldAddViewModel.CustomFields.TenantId && c.SchoolId != customFieldAddViewModel.CustomFields.SchoolId).ToList();
                                    if (schoolMasterList?.Any() == true)
                                    {

                                   
                                    foreach (var schoolMaster in schoolMasterList)
                                    {
                                        var SchoolCustomField = schoolMaster.CustomFields.Where(x => x.SystemField != true).ToList();

                                        var SchoolCustomFieldUpdate = SchoolCustomField.AsEnumerable().FirstOrDefault(x => x.SchoolId == schoolMaster.SchoolId && String.Compare(x.Title,customFieldUpdate.Title,true)==0 && x.CategoryId == customFieldUpdate.CategoryId);

                                        if (SchoolCustomFieldUpdate != null)
                                        {
                                            var CheckCustomFieldTitle = SchoolCustomField.AsEnumerable().Where(x => x.SchoolId == schoolMaster.SchoolId && x.TenantId == customFieldAddViewModel.CustomFields.TenantId && String.Compare(x.Title,customFieldAddViewModel.CustomFields.Title,true)==0 && x.FieldId != SchoolCustomFieldUpdate.FieldId).FirstOrDefault();

                                            if (CheckCustomFieldTitle != null)
                                            {
                                                customFieldAddViewModel._failure = true;
                                                customFieldAddViewModel._message = "Custom Field Title Already Exists";
                                                return customFieldAddViewModel;
                                            }

                                            var customFieldValue = this.context?.CustomFieldsValue.FirstOrDefault(c => c.SchoolId == SchoolCustomFieldUpdate.SchoolId && c.TenantId == SchoolCustomFieldUpdate.TenantId && c.FieldId == SchoolCustomFieldUpdate.FieldId && c.CategoryId == SchoolCustomFieldUpdate.CategoryId);

                                            if (customFieldValue != null)
                                            {
                                                var CustomFieldDataUpdate = SchoolCustomField.AsEnumerable().FirstOrDefault(x => x.SchoolId == schoolMaster.SchoolId && String.Compare(x.Title,customFieldUpdate.Title,true)==0 && x.CategoryId == customFieldUpdate.CategoryId);

                                                if (CustomFieldDataUpdate != null)
                                                {
                                                    CustomFieldDataUpdate.Title = customFieldAddViewModel.CustomFields.Title;
                                                    CustomFieldDataUpdate.FieldName = FieldName;
                                                    CustomFieldDataUpdate.UpdatedBy = customFieldAddViewModel.CustomFields.UpdatedBy;
                                                    CustomFieldDataUpdate.UpdatedOn = DateTime.UtcNow;
                                                    CustomFieldDataUpdate.Required = customFieldAddViewModel.CustomFields.Required;
                                                    CustomFieldDataUpdate.Hide = customFieldAddViewModel.CustomFields.Hide;
                                                }
                                            }
                                            else
                                            {
                                                SchoolCustomFieldUpdate.Title = customFieldAddViewModel.CustomFields.Title;
                                                SchoolCustomFieldUpdate.FieldName = FieldName;
                                                SchoolCustomFieldUpdate.Type = customFieldAddViewModel.CustomFields.Type;
                                                SchoolCustomFieldUpdate.DefaultSelection = customFieldAddViewModel.CustomFields.DefaultSelection;
                                                SchoolCustomFieldUpdate.SelectOptions = customFieldAddViewModel.CustomFields.SelectOptions;
                                                SchoolCustomFieldUpdate.Required = customFieldAddViewModel.CustomFields.Required;
                                                SchoolCustomFieldUpdate.Hide = customFieldAddViewModel.CustomFields.Hide;
                                                SchoolCustomFieldUpdate.UpdatedBy = customFieldAddViewModel.CustomFields.UpdatedBy;
                                                SchoolCustomFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                            }
                                            this.context?.SaveChanges();
                                        }
                                    }
                                   }
                                }
                                var customFieldsValueExists = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldUpdate.TenantId && x.SchoolId == customFieldUpdate.SchoolId && x.CategoryId == customFieldUpdate.CategoryId && x.FieldId == customFieldUpdate.FieldId);

                                if (customFieldsValueExists != null)
                                {
                                    customFieldUpdate.Title = customFieldAddViewModel.CustomFields.Title;
                                    customFieldUpdate.FieldName = FieldName;
                                    customFieldUpdate.UpdatedBy = customFieldAddViewModel.CustomFields.UpdatedBy;
                                    customFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                    customFieldUpdate.Required = customFieldAddViewModel.CustomFields.Required;
                                    customFieldUpdate.Hide = customFieldAddViewModel.CustomFields.Hide;
                                }
                                else
                                {
                                    customFieldUpdate.Title = customFieldAddViewModel.CustomFields.Title;
                                    customFieldUpdate.FieldName = FieldName;
                                    customFieldUpdate.Type = customFieldAddViewModel.CustomFields.Type;
                                    customFieldUpdate.DefaultSelection = customFieldAddViewModel.CustomFields.DefaultSelection;
                                    customFieldUpdate.SelectOptions = customFieldAddViewModel.CustomFields.SelectOptions;
                                    customFieldUpdate.Required = customFieldAddViewModel.CustomFields.Required;
                                    customFieldUpdate.Hide = customFieldAddViewModel.CustomFields.Hide;
                                    customFieldUpdate.UpdatedBy = customFieldAddViewModel.CustomFields.UpdatedBy;
                                    customFieldUpdate.UpdatedOn = DateTime.UtcNow;
                                }
                                this.context?.SaveChanges();
                                transaction?.Commit();
                                customFieldAddViewModel._failure = false;
                                customFieldAddViewModel._message = "Custom Field Updated Successfully";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    customFieldAddViewModel.CustomFields = null!;
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
                var customFieldDelete = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == customFieldAddViewModel.CustomFields.TenantId && x.SchoolId == customFieldAddViewModel.CustomFields.SchoolId && x.FieldId == customFieldAddViewModel.CustomFields.FieldId);

                if (customFieldDelete != null)
                {
                    var customFieldsValueExists = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == customFieldDelete.SchoolId && x.CategoryId == customFieldDelete.CategoryId && x.FieldId == customFieldDelete.FieldId);

                    if (customFieldsValueExists != null)
                    {
                        if (!string.IsNullOrWhiteSpace(customFieldsValueExists.CustomFieldValue))
                        {
                            customFieldAddViewModel._failure = true;
                            customFieldAddViewModel._message = "This cannot be deleted because it has data associated with it";
                            return customFieldAddViewModel;
                        }

                    }
                    else
                    {
                        customFieldList.Add(customFieldDelete);
                    }

                    if (customFieldDelete.IsSystemWideField == true)
                    {
                        var schoolMasterList = this.context?.SchoolMaster.Include(x => x.CustomFields).Where(c => c.TenantId == customFieldAddViewModel.CustomFields.TenantId && c.SchoolId != customFieldAddViewModel.CustomFields.SchoolId).ToList();
                        if (schoolMasterList?.Any() == true)
                        {
                        foreach (var schoolMaster in schoolMasterList)
                        {
                            var SchoolCustomField = schoolMaster.CustomFields.Where(x => x.SystemField != true).ToList();

                            var CustomFieldSchoolWise = SchoolCustomField.AsEnumerable().FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == schoolMaster.SchoolId && x.CategoryId == customFieldDelete.CategoryId && String.Compare(x.Title ,customFieldDelete.Title,true)==0);
                            if (CustomFieldSchoolWise != null)
                            {
                                var customFieldsValueExistsSchoolWise = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == customFieldDelete.TenantId && x.SchoolId == CustomFieldSchoolWise.SchoolId && x.CategoryId == CustomFieldSchoolWise.CategoryId && x.FieldId == CustomFieldSchoolWise.FieldId);
                                if (customFieldsValueExistsSchoolWise != null)
                                {
                                    customFieldAddViewModel._failure = true;
                                    customFieldAddViewModel._message = "This cannot be deleted because it has data associated with it";
                                    return customFieldAddViewModel;
                                }
                                else
                                {
                                    customFieldList.Add(CustomFieldSchoolWise);
                                }
                            }
                          }
                        }
                    }

                    this.context?.CustomFields.RemoveRange(customFieldList);
                    this.context?.SaveChanges();
                    customFieldAddViewModel._failure = false;
                    customFieldAddViewModel._message = "Custom field deleted successfully";
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
            if(fieldsCategoryAddViewModel.FieldsCategory is null)
            {
                return fieldsCategoryAddViewModel;
            }
            List<FieldsCategory> fieldsCategoryList = new List<FieldsCategory>();
            List<PermissionSubcategory> permissionSubcategoryList = new List<PermissionSubcategory>();
            List<RolePermission> rolePermissionList = new List<RolePermission>();

            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var checkFieldCategoryTitle = this.context?.FieldsCategory.AsEnumerable().Where(x => /*x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId &&*/ x.TenantId == fieldsCategoryAddViewModel.FieldsCategory.TenantId && String.Compare(x.Title ,fieldsCategoryAddViewModel.FieldsCategory.Title,true)==0 && x.IsSystemWideCategory == true).FirstOrDefault();

                    if (checkFieldCategoryTitle != null)
                    {
                        fieldsCategoryAddViewModel._failure = true;
                        fieldsCategoryAddViewModel._message = "Field category title already exists";
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


                        var schoolDataList = this.context?.SchoolMaster.Where(c => c.TenantId == fieldsCategoryAddViewModel.FieldsCategory!.TenantId).ToList();

                        if (schoolDataList?.Count > 0)
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
                                    Hide = fieldsCategoryAddViewModel?.FieldsCategory?.Hide,
                                    IsSystemCategory = false,
                                    IsSystemWideCategory = true,
                                    Search = fieldsCategoryAddViewModel?.FieldsCategory?.Search,
                                    Module = fieldsCategoryAddViewModel?.FieldsCategory?.Module,
                                    Required = fieldsCategoryAddViewModel?.FieldsCategory?.Required,
                                    SortOrder = fieldsCategoryAddViewModel?.FieldsCategory?.SortOrder,
                                    Title = fieldsCategoryAddViewModel?.FieldsCategory?.Title,
                                    CreatedBy = fieldsCategoryAddViewModel?.FieldsCategory?.UpdatedBy,
                                    CreatedOn = DateTime.UtcNow
                                };

                                fieldsCategoryList.Add(fieldCategory);
                        

                                var permissionGroupData = this.context?.PermissionGroup.FirstOrDefault(x => x.TenantId == schoolData.TenantId && x.SchoolId == schoolData.SchoolId && x.PermissionGroupName.Contains(fieldsCategoryAddViewModel!.FieldsCategory!.Module!));

                                if (permissionGroupData != null)
                                {
                                    var permissionCategory = this.context?.PermissionCategory.FirstOrDefault(e => e.PermissionGroupId == permissionGroupData.PermissionGroupId && e.TenantId == permissionGroupData.TenantId && e.SchoolId == permissionGroupData.SchoolId && e.PermissionGroupId == permissionGroupData.PermissionGroupId);

                                    if (permissionCategory != null)
                                    {
                                        var checkPermissionSubCategoryName = this.context?.PermissionSubcategory.AsEnumerable().Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId && String.Compare(x.PermissionSubcategoryName,fieldsCategoryAddViewModel!.FieldsCategory.Title,true)==0).FirstOrDefault();

                                        if (checkPermissionSubCategoryName != null)
                                        {
                                            fieldsCategoryAddViewModel!._failure = true;
                                            fieldsCategoryAddViewModel._message = "Permission subcategory name already exists";
                                        }
                                        else
                                        {
                                            int? PermissionSubCategoryId = 1;

                                            var permissionSubCategoryData = this.context?.PermissionSubcategory.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.PermissionSubcategoryId).FirstOrDefault();

                                            if (permissionSubCategoryData != null)
                                            {
                                                PermissionSubCategoryId = permissionSubCategoryData.PermissionSubcategoryId + 1;
                                            }

                                            int? PermissionSubCategorySortOrder = 1;

                                            var SubCategoryData = this.context?.PermissionSubcategory.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId && x.PermissionCategoryId == permissionCategory.PermissionCategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                                            if (SubCategoryData != null)
                                            {
                                                PermissionSubCategorySortOrder = SubCategoryData.SortOrder + 1;
                                            }

                                            string? path = null;

                                            string[] titleArray = (fieldsCategoryAddViewModel?.FieldsCategory.Title??"").ToLower().Split(" ");

                                            string? module = (fieldsCategoryAddViewModel?.FieldsCategory.Module ?? "").ToLower() == "school" ? "schoolinfo" : (fieldsCategoryAddViewModel?.FieldsCategory.Module ?? "").ToLower();

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
                                                PermissionSubcategoryName = fieldsCategoryAddViewModel?.FieldsCategory.Title,
                                                Path = path,
                                                Title = fieldsCategoryAddViewModel?.FieldsCategory.Title,
                                                EnableView = true,
                                                EnableAdd = true,
                                                EnableEdit = true,
                                                EnableDelete = true,
                                                CreatedBy = fieldsCategoryAddViewModel?.FieldsCategory.UpdatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                IsActive = true,
                                                SortOrder = PermissionSubCategorySortOrder,
                                                IsSystem = false
                                            };
                                            //this.context?.PermissionSubcategory.Add(permissionSubCategory);
                                            permissionSubcategoryList.Add(permissionSubCategory);

                                            int? rolePermissionId = 1;

                                            var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                                            if (rolePermissionData != null)
                                            {
                                                rolePermissionId = rolePermissionData.RolePermissionId + 1;
                                            }

                                            var membershipData = this.context?.Membership.Where(x => x.SchoolId == schoolData.SchoolId && x.TenantId == schoolData.TenantId).ToList()!;

                                            foreach (var membership in membershipData)
                                            {
                                                if (membership.ProfileType == "super administrator" || membership.ProfileType == "school administrator" || membership.ProfileType == "admin assistant")
                                                {
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
                                                        CreatedBy = fieldsCategoryAddViewModel?.FieldsCategory.UpdatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    rolePermissionList.Add(rolePermission);
                                                }
                                                else
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = schoolData.TenantId,
                                                        SchoolId = schoolData.SchoolId,
                                                        RolePermissionId = (int)rolePermissionId,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)PermissionSubCategoryId,
                                                        CanView = false,
                                                        CanAdd = false,
                                                        CanEdit = false,
                                                        CanDelete = false,
                                                        CreatedBy = fieldsCategoryAddViewModel?.FieldsCategory.UpdatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    rolePermissionList.Add(rolePermission);
                                                }
                                                rolePermissionId++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.context?.FieldsCategory.AddRange(fieldsCategoryList);

                        //for (int i = 0; i < fieldsCategoryList.Count; i++)
                        //{
                        //    context!.Entry(fieldsCategoryList[i].SchoolMaster).State = EntityState.Unchanged;
                        //}

                        this.context?.PermissionSubcategory.AddRange(permissionSubcategoryList);
                        this.context?.RolePermission.AddRange(rolePermissionList);

                        //for(int i=0; i<permissionSubcategoryList.Count; i++)
                        //{
                        //    context!.Entry(permissionSubcategoryList[i].PermissionCategory).State = EntityState.Unchanged;
                        //}
                        
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field category added successfully";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
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
            if(fieldsCategoryAddViewModel.FieldsCategory is null)
            {
                return fieldsCategoryAddViewModel;
            }
            FieldsCategoryAddViewModel fieldsCategoryUpdateModel = new FieldsCategoryAddViewModel();
            try
            {
                var fieldCategoryData = this.context?.FieldsCategory.Where(c => c.TenantId == fieldsCategoryAddViewModel.FieldsCategory!.TenantId).ToList();
                if (fieldCategoryData?.Any() == true)
                {


                    var fieldCategoryDataById = fieldCategoryData.Where(x => x.SchoolId == fieldsCategoryAddViewModel.FieldsCategory.SchoolId && x.TenantId == fieldsCategoryAddViewModel.FieldsCategory.TenantId && x.IsSystemCategory != true && x.CategoryId == fieldsCategoryAddViewModel.FieldsCategory.CategoryId).FirstOrDefault();
                    if (fieldCategoryDataById != null)
                    {
                    if (fieldCategoryDataById.Title != fieldsCategoryAddViewModel.FieldsCategory.Title)
                    {
                        var checkFieldCategoryTitle = fieldCategoryData.AsEnumerable().Where(x => x.TenantId == fieldsCategoryAddViewModel.FieldsCategory.TenantId && String.Compare(x.Title , fieldsCategoryAddViewModel.FieldsCategory.Title,true)==0 && x.IsSystemCategory != true && x.CategoryId != fieldsCategoryAddViewModel.FieldsCategory.CategoryId).FirstOrDefault();

                        if (checkFieldCategoryTitle != null)
                        {
                            fieldsCategoryAddViewModel._failure = true;
                            fieldsCategoryAddViewModel._message = "Field category title already exists";

                            return fieldsCategoryAddViewModel;
                        }
                    }
                }
                //else
                //{
                    if (fieldsCategoryAddViewModel.FieldsCategory.IsSystemCategory != true)
                    {
                        var fieldsCategoryUpdate = fieldCategoryData.FirstOrDefault(x => /*x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId &&*/ x.SchoolId == fieldsCategoryAddViewModel.FieldsCategory.SchoolId && x.CategoryId == fieldsCategoryAddViewModel.FieldsCategory.CategoryId);

                        if (fieldsCategoryUpdate != null)
                        {
                            var permissionSubCategoryData = this.context?.PermissionSubcategory.AsEnumerable().FirstOrDefault(c => c.SchoolId == fieldsCategoryUpdate.SchoolId && c.TenantId == fieldsCategoryUpdate.TenantId && String.Compare(c.PermissionSubcategoryName , fieldsCategoryUpdate.Title,true)==0 && String.Compare(c.Title , fieldsCategoryUpdate.Title,true)==0);

                            if (permissionSubCategoryData != null)
                            {
                                permissionSubCategoryData.Title = fieldsCategoryAddViewModel.FieldsCategory.Title;
                                permissionSubCategoryData.PermissionSubcategoryName = fieldsCategoryAddViewModel.FieldsCategory.Title;
                                permissionSubCategoryData.UpdatedBy = fieldsCategoryAddViewModel.FieldsCategory.UpdatedBy;
                                permissionSubCategoryData.UpdatedOn = DateTime.UtcNow;

                                string? path = null;

                                string[] titleArray = (fieldsCategoryAddViewModel.FieldsCategory.Title??"").ToLower().Split(" ");

                                string? module = (fieldsCategoryAddViewModel.FieldsCategory.Module??"").ToLower() == "school" ? "schoolinfo" : (fieldsCategoryAddViewModel.FieldsCategory.Module??"").ToLower();

                                if (module == "student")
                                {
                                    module = "students";
                                }
                                path = "/school/" + module + "/custom/" + string.Join("-", titleArray);
                                permissionSubCategoryData.Path = path;
                            }

                            var schoolDataList = this.context?.SchoolMaster.Where(v => v.TenantId == fieldsCategoryAddViewModel.FieldsCategory.TenantId && v.SchoolId != fieldsCategoryAddViewModel.FieldsCategory.SchoolId).ToList();

                            if (schoolDataList!=null && schoolDataList.Any())
                            {
                                foreach (var schoolData in schoolDataList)
                                {
                                    var fieldUpdate = fieldCategoryData.AsEnumerable().FirstOrDefault(x => /*x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId &&*/ x.SchoolId == schoolData.SchoolId && String.Compare(x.Title , fieldsCategoryUpdate.Title,true)==0);

                                    if (fieldUpdate != null)
                                    {
                                        var permissionSubCategory = this.context?.PermissionSubcategory.AsEnumerable().FirstOrDefault(c => c.SchoolId == fieldUpdate.SchoolId && c.TenantId == fieldUpdate.TenantId && String.Compare(c.PermissionSubcategoryName,fieldUpdate.Title,true)==0 && String.Compare(c.Title , fieldUpdate.Title,true)==0);

                                        if (permissionSubCategory != null)
                                        {
                                            permissionSubCategory.Title = fieldsCategoryAddViewModel.FieldsCategory.Title;
                                            permissionSubCategory.PermissionSubcategoryName = fieldsCategoryAddViewModel.FieldsCategory.Title;
                                            permissionSubCategory.UpdatedBy = fieldsCategoryAddViewModel.FieldsCategory.UpdatedBy;
                                            permissionSubCategory.UpdatedOn = DateTime.UtcNow;

                                            string? path = null;

                                            string[] titleArray = (fieldsCategoryAddViewModel.FieldsCategory.Title??"").ToLower().Split(" ");

                                            string? module = (fieldsCategoryAddViewModel.FieldsCategory.Module ?? "").ToLower() == "school" ? "schoolinfo" : (fieldsCategoryAddViewModel.FieldsCategory.Module ?? "").ToLower();

                                            path = "/school/" + module + "/custom/" + string.Join("-", titleArray);
                                            permissionSubCategory.Path = path;
                                        }

                                        fieldUpdate.Title = fieldsCategoryAddViewModel.FieldsCategory.Title;
                                        fieldUpdate.SortOrder = fieldsCategoryAddViewModel.FieldsCategory.SortOrder;
                                        fieldUpdate.UpdatedBy = fieldsCategoryAddViewModel.FieldsCategory.UpdatedBy;
                                        fieldUpdate.UpdatedOn = DateTime.UtcNow;

                                    }
                                }
                            }

                            fieldsCategoryUpdate.Title = fieldsCategoryAddViewModel.FieldsCategory.Title;
                            fieldsCategoryUpdate.SortOrder = fieldsCategoryAddViewModel.FieldsCategory.SortOrder;
                            fieldsCategoryUpdate.UpdatedBy = fieldsCategoryAddViewModel.FieldsCategory.UpdatedBy;
                            fieldsCategoryUpdate.UpdatedOn = DateTime.UtcNow;

                        }
                        this.context?.SaveChanges();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field category updated successfully";
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

                var fieldsCategoryDelete = this.context?.FieldsCategory.FirstOrDefault(x => x.TenantId == fieldsCategoryAddViewModel.FieldsCategory!.TenantId && x.SchoolId == fieldsCategoryAddViewModel.FieldsCategory.SchoolId && x.CategoryId == fieldsCategoryAddViewModel.FieldsCategory.CategoryId);

                if (fieldsCategoryDelete != null)
                {
                    var customFieldsExists = this.context?.CustomFields.FirstOrDefault(x => x.TenantId == fieldsCategoryDelete.TenantId && x.SchoolId == fieldsCategoryDelete.SchoolId && x.CategoryId == fieldsCategoryDelete.CategoryId);

                    if (customFieldsExists != null)
                    {
                        fieldsCategoryAddViewModel._failure = true;
                        fieldsCategoryAddViewModel._message = "This cannot be deleted because it has data associated with it";
                        return fieldsCategoryAddViewModel;
                    }
                    else
                    {
                        fieldsCategories.Add(fieldsCategoryDelete);

                        var permissionSubCategory = this.context?.PermissionSubcategory.Include(x => x.RolePermission).AsEnumerable().Where(c => c.SchoolId == fieldsCategoryAddViewModel.FieldsCategory!.SchoolId && c.TenantId == fieldsCategoryAddViewModel.FieldsCategory.TenantId && String.Compare(c.PermissionSubcategoryName , fieldsCategoryDelete.Title,true)==0 && c.Title == fieldsCategoryDelete.Title).ToList();

                        if (permissionSubCategory?.Any()==true)
                        {
                            //var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == fieldsCategoryAddViewModel.fieldsCategory.SchoolId && x.TenantId == fieldsCategoryAddViewModel.fieldsCategory.TenantId && x.PermissionSubcategoryId == permissionSubCategory.PermissionSubcategoryId).ToList();

                            if (permissionSubCategory.FirstOrDefault()!.RolePermission.Any())
                            {
                                rolePermissions.AddRange(permissionSubCategory.FirstOrDefault()!.RolePermission);
                            }

                            permissionSubcategories.AddRange(permissionSubCategory);
                        }

                        var schoolMasterList = this.context?.SchoolMaster.Where(v => v.TenantId == fieldsCategoryAddViewModel.FieldsCategory!.TenantId && v.SchoolId != fieldsCategoryAddViewModel.FieldsCategory.SchoolId).ToList();

                        if (schoolMasterList?.Count > 0)
                        {
                            foreach (var schoolMaster in schoolMasterList)
                            {
                                var fieldCategoryData = this.context?.FieldsCategory.Include(x => x.CustomFields).AsEnumerable().FirstOrDefault(c => c.TenantId == fieldsCategoryAddViewModel.FieldsCategory!.TenantId && c.SchoolId == schoolMaster.SchoolId &&String.Compare( c.Title,fieldsCategoryDelete.Title,true)==0);

                                if (fieldCategoryData?.CustomFields.Count > 0)
                                {
                                    fieldsCategoryAddViewModel._failure = true;
                                    fieldsCategoryAddViewModel._message = "This cannot be deleted because it has data associated with it";
                                    return fieldsCategoryAddViewModel;
                                }
                                else
                                {
                                    var permissionSubCategoryData = this.context?.PermissionSubcategory.Include(x => x.RolePermission).AsEnumerable().Where(c => c.SchoolId == schoolMaster.SchoolId && c.TenantId == schoolMaster.TenantId && String.Compare(c.PermissionSubcategoryName , fieldsCategoryDelete.Title,true)==0 && String.Compare(c.Title , fieldsCategoryDelete.Title,true)==0).ToList();

                                    if (permissionSubCategoryData?.Any()==true)
                                    {
                                        //var rolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == schoolMaster.SchoolId && x.TenantId == schoolMaster.TenantId && x.PermissionSubcategoryId == permissionSubCategoryData.PermissionSubcategoryId).ToList();

                                        if (permissionSubCategoryData.FirstOrDefault()!.RolePermission.Any())
                                        {
                                            rolePermissions.AddRange(permissionSubCategoryData.FirstOrDefault()!.RolePermission);
                                        }

                                        permissionSubcategories.AddRange(permissionSubCategoryData);
                                    }
                                    fieldsCategories.Add(fieldCategoryData??null!);
                                }
                            }
                        }
                        this.context?.RolePermission.RemoveRange(rolePermissions);
                        this.context?.PermissionSubcategory.RemoveRange(permissionSubcategories);
                        this.context?.FieldsCategory.RemoveRange(fieldsCategories);
                        this.context?.SaveChanges();
                        fieldsCategoryAddViewModel._failure = false;
                        fieldsCategoryAddViewModel._message = "Field category deleted successfully";
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
                    .Include(x => x.CustomFields).AsEnumerable()
                    .Where(x => x.TenantId == fieldsCategoryListViewModel.TenantId &&
                                x.SchoolId == fieldsCategoryListViewModel.SchoolId &&
                                String.Compare(x.Module , fieldsCategoryListViewModel.Module,true)==0)
                    .OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder).ToList();

                if (fieldsCategoryList?.Any()==true)
                {
                    foreach (var fieldsCategory in fieldsCategoryList)
                    {
                        //fieldsCategory.CustomFields.ToList().ForEach(c =>
                        //{
                        //    c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, fieldsCategoryListViewModel.TenantId, c.CreatedBy);
                        //    c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, fieldsCategoryListViewModel.TenantId, c.UpdatedBy);
                        //});
                        fieldsCategory.CustomFields = fieldsCategory.CustomFields.OrderByDescending(y => y.SystemField).ThenBy(y => y.SortOrder).ToList();
                    }
                    fieldsCategoryListModel._failure = false;
                    fieldsCategoryListModel.FieldsCategoryList = fieldsCategoryList;
                }
                else
                {
                    fieldsCategoryListModel._failure = true;
                    fieldsCategoryListModel._message = NORECORDFOUND;
                }
               
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
                if(targetCustomField != null)
                {
                targetCustomField.SortOrder = customFieldSortOrderModel.CurrentSortOrder;
                targetCustomField.UpdatedBy = customFieldSortOrderModel.UpdatedBy;
                targetCustomField.UpdatedOn = DateTime.UtcNow;
                }
               

                if (customFieldSortOrderModel.PreviousSortOrder > customFieldSortOrderModel.CurrentSortOrder)
                {
                    customFieldRecords = this.context?.CustomFields.Where(x => x.SortOrder >= customFieldSortOrderModel.CurrentSortOrder && x.SortOrder < customFieldSortOrderModel.PreviousSortOrder && x.SchoolId == customFieldSortOrderModel.SchoolId && x.CategoryId == customFieldSortOrderModel.CategoryId).ToList();

                    if (customFieldRecords?.Any()==true)
                    {
                        customFieldRecords.ForEach(x => { x.SortOrder = x.SortOrder + 1; x.UpdatedBy = customFieldSortOrderModel.UpdatedBy; x.UpdatedOn = DateTime.UtcNow; });
                    }
                }
                if (customFieldSortOrderModel.CurrentSortOrder > customFieldSortOrderModel.PreviousSortOrder)
                {
                    customFieldRecords = this.context?.CustomFields.Where(x => x.SortOrder <= customFieldSortOrderModel.CurrentSortOrder && x.SortOrder > customFieldSortOrderModel.PreviousSortOrder && x.SchoolId == customFieldSortOrderModel.SchoolId && x.CategoryId == customFieldSortOrderModel.CategoryId).ToList();
                    if (customFieldRecords?.Any()==true)
                    {
                        customFieldRecords.ForEach(x => { x.SortOrder = x.SortOrder - 1; x.UpdatedBy = customFieldSortOrderModel.UpdatedBy; x.UpdatedOn = DateTime.UtcNow; });
                    }
                }
                this.context?.SaveChanges();
                customFieldSortOrderModel._failure = false;
                customFieldSortOrderModel._message = "Custom field sort order updated successfully";
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
