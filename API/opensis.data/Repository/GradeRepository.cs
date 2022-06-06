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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Grades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace opensis.data.Repository
{
    public class GradeRepository : IGradeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public GradeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        
        /// <summary>
        /// Add Grade Scale
        /// </summary>
        /// <param name="gradeScaleAddViewModel"></param>
        /// <returns></returns>
        public GradeScaleAddViewModel AddGradeScale(GradeScaleAddViewModel gradeScaleAddViewModel)
        {
            if(gradeScaleAddViewModel.gradeScale is null)
            {
                return gradeScaleAddViewModel;
            }
            try
            {
                gradeScaleAddViewModel.gradeScale.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, gradeScaleAddViewModel.gradeScale.TenantId, gradeScaleAddViewModel.gradeScale.SchoolId);

                var checkGradeScaleName = this.context?.GradeScale.AsEnumerable().Where(x => x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.TenantId == gradeScaleAddViewModel.gradeScale.TenantId && String.Compare(x.GradeScaleName, gradeScaleAddViewModel.gradeScale.GradeScaleName,true)==0 && x.AcademicYear == gradeScaleAddViewModel.gradeScale.AcademicYear).FirstOrDefault();
                if (checkGradeScaleName != null)
                {
                    gradeScaleAddViewModel._failure = true;
                    gradeScaleAddViewModel._message = "Grade Scale Name Already Exists";
                }
                else
                {
                    int? GradeScaleId = 1;
                    int? SortOrder = 1;

                    var gradeScaleData = this.context?.GradeScale.Where(x => x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.TenantId == gradeScaleAddViewModel.gradeScale.TenantId).OrderByDescending(x => x.GradeScaleId).FirstOrDefault();

                    if (gradeScaleData != null)
                    {
                        GradeScaleId = gradeScaleData.GradeScaleId + 1;
                    }

                    var gradeScaleSortOrder = this.context?.GradeScale.Where(x => x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.TenantId == gradeScaleAddViewModel.gradeScale.TenantId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                    if (gradeScaleSortOrder != null)
                    {
                        SortOrder = gradeScaleSortOrder.SortOrder + 1;
                    }

                    gradeScaleAddViewModel.gradeScale.GradeScaleId = (int)GradeScaleId;
                    gradeScaleAddViewModel.gradeScale.SortOrder = (int)SortOrder!;
                    gradeScaleAddViewModel.gradeScale.CreatedOn = DateTime.UtcNow;
                    this.context?.GradeScale.Add(gradeScaleAddViewModel.gradeScale);
                    //context!.Entry(gradeScaleAddViewModel.gradeScale.SchoolMaster).State = EntityState.Unchanged;
                    this.context?.SaveChanges();
                    gradeScaleAddViewModel._failure = false;
                    gradeScaleAddViewModel._message = "Grade Scale Added Successfully";
                }                
            }
            catch (Exception es)
            {
                gradeScaleAddViewModel._failure = true;
                gradeScaleAddViewModel._message = es.Message;
            }
            return gradeScaleAddViewModel;
        }
        
        /// <summary>
        /// Update Grade Scale
        /// </summary>
        /// <param name="gradeScaleAddViewModel"></param>
        /// <returns></returns>
        public GradeScaleAddViewModel UpdateGradeScale(GradeScaleAddViewModel gradeScaleAddViewModel)
        {
            if (gradeScaleAddViewModel.gradeScale is null)
            {
                return gradeScaleAddViewModel;
            }
            try
            {
                var gradeScaleUpdate = this.context?.GradeScale.FirstOrDefault(x => x.TenantId == gradeScaleAddViewModel.gradeScale.TenantId && x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.GradeScaleId == gradeScaleAddViewModel.gradeScale.GradeScaleId);
                
                if (gradeScaleUpdate != null)
                {
                    var checkGradeScaleName = this.context?.GradeScale.AsEnumerable().Where(x => x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.TenantId == gradeScaleAddViewModel.gradeScale.TenantId && x.GradeScaleId != gradeScaleAddViewModel.gradeScale.GradeScaleId &&
                    String.Compare(x.GradeScaleName, gradeScaleAddViewModel.gradeScale.GradeScaleName, true) == 0 && x.AcademicYear == gradeScaleUpdate.AcademicYear).FirstOrDefault();

                    if (checkGradeScaleName != null)
                    {
                        gradeScaleAddViewModel._failure = true;
                        gradeScaleAddViewModel._message = "Grade Scale Name Already Exists";
                    }
                    else
                    {
                        gradeScaleAddViewModel.gradeScale.AcademicYear = gradeScaleUpdate.AcademicYear;
                        gradeScaleAddViewModel.gradeScale.CreatedBy = gradeScaleUpdate.CreatedBy;
                        gradeScaleAddViewModel.gradeScale.CreatedOn = gradeScaleUpdate.CreatedOn;
                        gradeScaleAddViewModel.gradeScale.UpdatedOn = DateTime.Now;
                        gradeScaleAddViewModel.gradeScale.SortOrder = gradeScaleUpdate.SortOrder;
                        this.context?.Entry(gradeScaleUpdate).CurrentValues.SetValues(gradeScaleAddViewModel.gradeScale);
                        this.context?.SaveChanges();
                        gradeScaleAddViewModel._failure = false;
                        gradeScaleAddViewModel._message = "Grade Scale Updated Successfully";
                    }                    
                }
                else
                {
                    gradeScaleAddViewModel.gradeScale = null;
                    gradeScaleAddViewModel._failure = true;
                    gradeScaleAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                gradeScaleAddViewModel._failure = true;
                gradeScaleAddViewModel._message = es.Message;
            }
            return gradeScaleAddViewModel;
        }
        
        /// <summary>
        /// Delete Grade Scale
        /// </summary>
        /// <param name="gradeScaleAddViewModel"></param>
        /// <returns></returns>
        public GradeScaleAddViewModel DeleteGradeScale(GradeScaleAddViewModel gradeScaleAddViewModel)
        {
            try
            {
                var gradeScaleDelete = this.context?.GradeScale.FirstOrDefault(x => x.TenantId == gradeScaleAddViewModel.gradeScale!.TenantId && x.SchoolId == gradeScaleAddViewModel.gradeScale.SchoolId && x.GradeScaleId == gradeScaleAddViewModel.gradeScale.GradeScaleId);
                
          
                if (gradeScaleDelete!=null)
                {
                    var gradeList = this.context?.Grade.Where(e => e.GradeScaleId == gradeScaleDelete.GradeScaleId && e.SchoolId == gradeScaleDelete.SchoolId && e.TenantId == gradeScaleDelete.TenantId).ToList();
                    if (gradeList?.Any()==true)
                    {
                        gradeScaleAddViewModel._message = "It Has Associationship";
                        gradeScaleAddViewModel._failure = true;
                    }
                    else
                    {
                        this.context?.GradeScale.Remove(gradeScaleDelete);
                        this.context?.SaveChanges();
                        gradeScaleAddViewModel._failure = false;
                        gradeScaleAddViewModel._message = "Grade Scale Deleted Successfully";
                    }
                }
                else
                {
                    gradeScaleAddViewModel._failure = true;
                    gradeScaleAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                gradeScaleAddViewModel._failure = true;
                gradeScaleAddViewModel._message = es.Message;
            }
            return gradeScaleAddViewModel;
        }
        
        /// <summary>
        /// Add Grade
        /// </summary>
        /// <param name="gradeAddViewModel"></param>
        /// <returns></returns>
        public GradeAddViewModel AddGrade(GradeAddViewModel gradeAddViewModel)
        {
            if(gradeAddViewModel.grade is null)
            {
                return gradeAddViewModel;
            }
            try
            {
                var checkGradeTitle = this.context?.Grade.AsEnumerable().Where(x => x.SchoolId == gradeAddViewModel.grade.SchoolId && x.TenantId == gradeAddViewModel.grade.TenantId && x.GradeScaleId == gradeAddViewModel.grade.GradeScaleId &&
                String.Compare(x.Title, gradeAddViewModel.grade.Title,true)==0
                /*x.Title.ToLower()== gradeAddViewModel.grade.Title.ToLower()*/).FirstOrDefault();
                if (checkGradeTitle != null)
                {
                    gradeAddViewModel._failure = true;
                    gradeAddViewModel._message = "Grade Title Already Exists";
                }
                else
                {
                    int? GradeId = 1;
                    int? SortOrder = 1;

                    var gradeData = this.context?.Grade.Where(x => x.SchoolId == gradeAddViewModel.grade.SchoolId && x.TenantId == gradeAddViewModel.grade.TenantId).OrderByDescending(x => x.GradeId).FirstOrDefault();

                    if (gradeData != null)
                    {
                        GradeId = gradeData.GradeId + 1;
                    }
                    var gradeSortOrder = this.context?.Grade.Where(x => x.SchoolId == gradeAddViewModel.grade.SchoolId && x.TenantId == gradeAddViewModel.grade.TenantId && x.GradeScaleId == gradeAddViewModel.grade.GradeScaleId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                    if (gradeSortOrder != null)
                    {
                        SortOrder = gradeSortOrder.SortOrder + 1;
                    }
                    gradeAddViewModel.grade.GradeId = (int)GradeId!;
                    gradeAddViewModel.grade.SortOrder = (int)SortOrder!;
                    gradeAddViewModel.grade.CreatedOn = DateTime.UtcNow;
                    this.context?.Grade.Add(gradeAddViewModel.grade);
                    this.context?.SaveChanges();
                    gradeAddViewModel._failure = false;
                    gradeAddViewModel._message = "Grade Added Successfully";
                }                
            }
            catch (Exception es)
            {

                gradeAddViewModel._failure = true;
                gradeAddViewModel._message = es.Message;
            }
            return gradeAddViewModel;
        }
        
        /// <summary>
        /// Update Grade
        /// </summary>
        /// <param name="gradeAddViewModel"></param>
        /// <returns></returns>
        public GradeAddViewModel UpdateGrade(GradeAddViewModel gradeAddViewModel)
        {
            if(gradeAddViewModel.grade is null)
            {
                return gradeAddViewModel;
            }
            try
            {
                var gradeUpdate = this.context?.Grade.FirstOrDefault(x => x.TenantId == gradeAddViewModel.grade.TenantId && x.SchoolId == gradeAddViewModel.grade.SchoolId && x.GradeId == gradeAddViewModel.grade.GradeId);
                if (gradeUpdate != null)
                {
                    var checkGradeTitle = this.context?.Grade.AsEnumerable().Where(x => x.SchoolId == gradeAddViewModel.grade.SchoolId && x.TenantId == gradeAddViewModel.grade.TenantId && x.GradeScaleId == gradeAddViewModel.grade.GradeScaleId && x.GradeId != gradeAddViewModel.grade.GradeId && 
                    String.Compare(x.Title, gradeAddViewModel.grade.Title)==0
                    /*x.Title.ToLower() == gradeAddViewModel.grade.Title.ToLower()*/).FirstOrDefault();

                    if (checkGradeTitle != null)
                    {
                        gradeAddViewModel._failure = true;
                        gradeAddViewModel._message = "Grade Title Already Exists";
                    }
                    else
                    {
                        gradeAddViewModel.grade.CreatedBy = gradeUpdate.CreatedBy;
                        gradeAddViewModel.grade.CreatedOn = gradeUpdate.CreatedOn;
                        gradeAddViewModel.grade.SortOrder = gradeUpdate.SortOrder;
                        gradeAddViewModel.grade.UpdatedOn = DateTime.Now;
                        this.context?.Entry(gradeUpdate).CurrentValues.SetValues(gradeAddViewModel.grade);                        
                        this.context?.SaveChanges();
                        gradeAddViewModel._failure = false;
                        gradeAddViewModel._message = "Grade Updated Successfully";
                    }                    
                }
                else
                {
                    gradeAddViewModel.grade = null;
                    gradeAddViewModel._failure = true;
                    gradeAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                gradeAddViewModel._failure = true;
                gradeAddViewModel._message = es.Message;
            }
            return gradeAddViewModel;
        }
        
        /// <summary>
        /// Delete Grade
        /// </summary>
        /// <param name="gradeAddViewModel"></param>
        /// <returns></returns>
        public GradeAddViewModel DeleteGrade(GradeAddViewModel gradeAddViewModel)
        {
            try
            {
                var gradeDelete = this.context?.Grade.FirstOrDefault(x => x.TenantId == gradeAddViewModel.grade!.TenantId && x.SchoolId == gradeAddViewModel.grade.SchoolId && x.GradeId==gradeAddViewModel.grade.GradeId);
                
                if (gradeDelete != null)
                {
                        this.context?.Grade.Remove(gradeDelete);
                        this.context?.SaveChanges();
                        gradeAddViewModel._failure = false;
                        gradeAddViewModel._message = "Grade Deleted Successfully";                    
                }
                else
                {
                        gradeAddViewModel._failure = true;
                        gradeAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeAddViewModel._failure = true;
                gradeAddViewModel._message = es.Message;
            }
            return gradeAddViewModel;
        }
        
        /// <summary>
        /// Get All Grade Scale List
        /// </summary>
        /// <param name="gradeScaleListViewModel"></param>
        /// <returns></returns>
        public GradeScaleListViewModel GetAllGradeScaleList(GradeScaleListViewModel gradeScaleListViewModel)
        {
            GradeScaleListViewModel GradeScaleListModel = new ();
            try
            {

                var GradeScaleList = this.context?.GradeScale.Include(x => x.Grade).Where(e => e.TenantId == gradeScaleListViewModel.TenantId && e.SchoolId == gradeScaleListViewModel.SchoolId && e.AcademicYear == gradeScaleListViewModel.AcademicYear).OrderBy(e => e.SortOrder).ToList();       
                
                if (GradeScaleList?.Any()==true)
                {
                    foreach (var GradeScale in GradeScaleList)
                    {
                        if (gradeScaleListViewModel.IsListView == true)
                        {
                            GradeScale.Grade.ToList().ForEach(c =>
                            {
                                c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, gradeScaleListViewModel.TenantId, c.CreatedBy);
                                c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, gradeScaleListViewModel.TenantId, c.UpdatedBy);
                            });
                        }
                        GradeScale.Grade = GradeScale.Grade.OrderBy(y => y.SortOrder).ToList();
                    }

                    GradeScaleListModel.GradeScaleList = GradeScaleList;
                    GradeScaleListModel._failure = false;
                }
                else
                {
                    GradeScaleListModel.GradeScaleList = new();
                    GradeScaleListModel._message = NORECORDFOUND;
                    GradeScaleListModel._failure = true;
                }
                GradeScaleListModel._tenantName = gradeScaleListViewModel._tenantName;
                GradeScaleListModel._token = gradeScaleListViewModel._token;
            }
            catch (Exception es)
            {
                GradeScaleListModel._message = es.Message;
                GradeScaleListModel._failure = true;
                GradeScaleListModel._tenantName = gradeScaleListViewModel._tenantName;
                GradeScaleListModel._token = gradeScaleListViewModel._token;
            }
            return GradeScaleListModel;
        }
        
        /// <summary>
        /// Update Grade Sort Order
        /// </summary>
        /// <param name="gradeSortOrderModel"></param>
        /// <returns></returns>
        public GradeSortOrderModel UpdateGradeSortOrder(GradeSortOrderModel gradeSortOrderModel)
        {
            try
            {
                var GradeRecords = new List<Grade>();

                var targetGrade = this.context?.Grade.FirstOrDefault(x => x.SortOrder == gradeSortOrderModel.PreviousSortOrder && x.SchoolId == gradeSortOrderModel.SchoolId && x.GradeScaleId == gradeSortOrderModel.GradeScaleId && x.TenantId== gradeSortOrderModel.TenantId);
                if (targetGrade != null)
                 {
                    targetGrade.SortOrder = gradeSortOrderModel.CurrentSortOrder;
                    targetGrade.UpdatedBy = gradeSortOrderModel.UpdatedBy;
                    targetGrade.UpdatedOn = DateTime.UtcNow;
                 }
                
                if (gradeSortOrderModel.PreviousSortOrder > gradeSortOrderModel.CurrentSortOrder)
                {
                    GradeRecords = this.context?.Grade.Where(x => x.SortOrder >= gradeSortOrderModel.CurrentSortOrder && x.SortOrder < gradeSortOrderModel.PreviousSortOrder && x.SchoolId == gradeSortOrderModel.SchoolId && x.GradeScaleId == gradeSortOrderModel.GradeScaleId && x.TenantId == gradeSortOrderModel.TenantId).ToList();

                    if (GradeRecords?.Any()==true)
                    {
                        GradeRecords.ForEach(x => { x.SortOrder++; x.UpdatedOn = DateTime.UtcNow;x.UpdatedBy = gradeSortOrderModel.UpdatedBy; });
                    }
                }
                if (gradeSortOrderModel.CurrentSortOrder > gradeSortOrderModel.PreviousSortOrder)
                {
                    GradeRecords = this.context?.Grade.Where(x => x.SortOrder <= gradeSortOrderModel.CurrentSortOrder && x.SortOrder > gradeSortOrderModel.PreviousSortOrder && x.SchoolId == gradeSortOrderModel.SchoolId && x.GradeScaleId == gradeSortOrderModel.GradeScaleId && x.TenantId == gradeSortOrderModel.TenantId).ToList();
                    if (GradeRecords?.Any() == true)
                    {
                        GradeRecords.ForEach(x => { x.SortOrder--; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = gradeSortOrderModel.UpdatedBy; });
                    }
                }
                this.context?.SaveChanges();
                gradeSortOrderModel._failure = false;
            }
            catch (Exception es)
            {
                gradeSortOrderModel._message = es.Message;
                gradeSortOrderModel._failure = true;
            }
            return gradeSortOrderModel;
        }
        
        /// <summary>
        /// Add Effort Grade Library Category
        /// </summary>
        /// <param name="effortGradeLibraryCategoryAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryAddViewModel AddEffortGradeLibraryCategory(EffortGradeLibraryCategoryAddViewModel effortGradeLibraryCategoryAddViewModel)
        {
            if(effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory is null)
            {
                return effortGradeLibraryCategoryAddViewModel;
            }
            try
            {
                var effortGradeLibraryCategoryList = this.context?.EffortGradeLibraryCategory.AsEnumerable().FirstOrDefault(x => x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.TenantId && /*x.CategoryName.ToLower() == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CategoryName.ToLower()*/
                String.Compare(x.CategoryName, effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CategoryName,true)==0
                );

                if (effortGradeLibraryCategoryList == null)
                {
                    int? EffortCategoryId = 1;
                    int? SortOrder = 1;

                    var effortGradeLibraryCategoryData = this.context?.EffortGradeLibraryCategory.Where(x => x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.TenantId).OrderByDescending(x => x.EffortCategoryId).FirstOrDefault();

                    if (effortGradeLibraryCategoryData != null)
                    {
                        EffortCategoryId = effortGradeLibraryCategoryData.EffortCategoryId + 1;
                    }
                    var effortGradeLibraryCategorySortOrder = this.context?.EffortGradeLibraryCategory.Where(x => x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.TenantId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                    if (effortGradeLibraryCategorySortOrder != null)
                    {
                        SortOrder = effortGradeLibraryCategorySortOrder.SortOrder + 1;
                    }

                    effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.EffortCategoryId = (int)EffortCategoryId;
                    effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SortOrder = (int)SortOrder!;
                    effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CreatedOn = DateTime.UtcNow;
                    this.context?.EffortGradeLibraryCategory.Add(effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory);
                    this.context?.SaveChanges();
                    effortGradeLibraryCategoryAddViewModel._failure = false;
                    effortGradeLibraryCategoryAddViewModel._message = "Effort Grade Library Category Added Successfully";
                }
                else
                {
                    effortGradeLibraryCategoryAddViewModel._failure = true;
                    effortGradeLibraryCategoryAddViewModel._message = "Effort Grade Library Category Name Already Exists";
                }
            }
            catch (Exception es)
            {
                effortGradeLibraryCategoryAddViewModel._failure = true;
                effortGradeLibraryCategoryAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryAddViewModel;
        }
        /// <summary>
        /// Update Effort Grade Library Category
        /// </summary>
        /// <param name="effortGradeLibraryCategoryAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryAddViewModel UpdateEffortGradeLibraryCategory(EffortGradeLibraryCategoryAddViewModel effortGradeLibraryCategoryAddViewModel)
        {
            if(effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory is null)
            {
                return effortGradeLibraryCategoryAddViewModel;
            }
            try
            {
                var EffortGradeLibraryCategoryUpdate = this.context?.EffortGradeLibraryCategory.FirstOrDefault(x => x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.TenantId && x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.EffortCategoryId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.EffortCategoryId);
                
                if (EffortGradeLibraryCategoryUpdate != null)
                {
                    var EffortGradeLibraryCategoryData= this.context?.EffortGradeLibraryCategory.AsEnumerable().FirstOrDefault(x => x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.TenantId && x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.EffortCategoryId != effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.EffortCategoryId && /*x.CategoryName.ToLower() == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CategoryName.ToLower()*/
                    String.Compare(x.CategoryName, effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CategoryName, true) == 0);

                    if (EffortGradeLibraryCategoryData!=null)
                    {
                        effortGradeLibraryCategoryAddViewModel._failure = true;
                        effortGradeLibraryCategoryAddViewModel._message = "Effort Grade Library Category Name Already Exists";
                    }
                    else
                    {
                        effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CreatedBy = EffortGradeLibraryCategoryUpdate.CreatedBy;
                        effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.CreatedOn = EffortGradeLibraryCategoryUpdate.CreatedOn;
                        effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SortOrder = EffortGradeLibraryCategoryUpdate.SortOrder;
                        effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.UpdatedOn = DateTime.Now;
                        this.context?.Entry(EffortGradeLibraryCategoryUpdate).CurrentValues.SetValues(effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory);
                        this.context?.SaveChanges();
                        effortGradeLibraryCategoryAddViewModel._failure = false;
                        effortGradeLibraryCategoryAddViewModel._message = "Effort Grade Library Category Updated Successfully";
                    }                    
                }
                else
                {
                    effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory = null;
                    effortGradeLibraryCategoryAddViewModel._failure = true;
                    effortGradeLibraryCategoryAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                effortGradeLibraryCategoryAddViewModel._failure = true;
                effortGradeLibraryCategoryAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryAddViewModel;
        }
        
        /// <summary>
        /// Delete Effort Grade Library Category
        /// </summary>
        /// <param name="effortGradeLibraryCategoryAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryAddViewModel DeleteEffortGradeLibraryCategory(EffortGradeLibraryCategoryAddViewModel effortGradeLibraryCategoryAddViewModel)
        {
            try
            {
                var EffortGradeLibraryCategoryDelete = this.context?.EffortGradeLibraryCategory.FirstOrDefault(x => x.TenantId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory!.TenantId && x.SchoolId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.SchoolId && x.EffortCategoryId == effortGradeLibraryCategoryAddViewModel.EffortGradeLibraryCategory.EffortCategoryId);

                if (EffortGradeLibraryCategoryDelete != null)
                {
                    var effortGradeLibraryCategoryItemExits = this.context?.EffortGradeLibraryCategoryItem.FirstOrDefault(e => e.TenantId == EffortGradeLibraryCategoryDelete.TenantId && e.SchoolId == EffortGradeLibraryCategoryDelete.SchoolId && e.EffortCategoryId == EffortGradeLibraryCategoryDelete.EffortCategoryId);
                    if (effortGradeLibraryCategoryItemExits!=null)
                    {
                        effortGradeLibraryCategoryAddViewModel._failure = true;
                        effortGradeLibraryCategoryAddViewModel._message = "Cannot be deleted because it has association.";
                    }
                    else
                    {
                        this.context?.EffortGradeLibraryCategory.Remove(EffortGradeLibraryCategoryDelete);
                        this.context?.SaveChanges();
                        effortGradeLibraryCategoryAddViewModel._failure = false;
                        effortGradeLibraryCategoryAddViewModel._message = "Effort Grade Library Category Deleted Successfully";
                    }
                    
                }
                else
                {
                    effortGradeLibraryCategoryAddViewModel._failure = true;
                    effortGradeLibraryCategoryAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                effortGradeLibraryCategoryAddViewModel._failure = true;
                effortGradeLibraryCategoryAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryAddViewModel;
        }
        
        /// <summary>
        /// Add Effort Grade Library Category Item
        /// </summary>
        /// <param name="effortGradeLibraryCategoryItemAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryItemAddViewModel AddEffortGradeLibraryCategoryItem(EffortGradeLibraryCategoryItemAddViewModel effortGradeLibraryCategoryItemAddViewModel)
        {
            if(effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem is null)
            {
                return effortGradeLibraryCategoryItemAddViewModel;
            }
            try
            {
                var effortGradeLibraryCategoryItemList = this.context?.EffortGradeLibraryCategoryItem.AsEnumerable().FirstOrDefault(x => x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.TenantId && x.EffortCategoryId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortCategoryId && /*x.EffortItemTitle.ToLower() == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemTitle.ToLower()*/
                String.Compare(x.EffortItemTitle, effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemTitle, true) == 0
                );

                if (effortGradeLibraryCategoryItemList == null)
                {
                    int? EffortCategoryItemId = 1;
                    int? SortOrder = 1;

                    var effortGradeLibraryCategoryItemData = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.TenantId).OrderByDescending(x => x.EffortItemId).FirstOrDefault();

                    if (effortGradeLibraryCategoryItemData != null)
                    {
                        EffortCategoryItemId = effortGradeLibraryCategoryItemData.EffortItemId + 1;
                    }

                    var effortGradeLibraryCategoryItemSortOrder = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.TenantId && x.EffortCategoryId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortCategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                    if (effortGradeLibraryCategoryItemSortOrder != null)
                    {
                        SortOrder = effortGradeLibraryCategoryItemSortOrder.SortOrder + 1;
                    }

                    effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemId = (int)EffortCategoryItemId;
                    effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SortOrder = (int)SortOrder!;
                    effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.CreatedOn = DateTime.UtcNow;
                    this.context?.EffortGradeLibraryCategoryItem.Add(effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem);
                    this.context?.SaveChanges();
                    effortGradeLibraryCategoryItemAddViewModel._failure = false;
                    effortGradeLibraryCategoryItemAddViewModel._message = "Effort Grade Library Category Item Added Successfully";
                }
                else
                {
                    effortGradeLibraryCategoryItemAddViewModel._failure = true;
                    effortGradeLibraryCategoryItemAddViewModel._message = "Effort Grade Library Category Item Name Already Exists";
                }                
            }
            catch (Exception es)
            {

                effortGradeLibraryCategoryItemAddViewModel._failure = true;
                effortGradeLibraryCategoryItemAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryItemAddViewModel;
        }
        
        /// <summary>
        /// Update Effort Grade Library Category Item
        /// </summary>
        /// <param name="effortGradeLibraryCategoryItemAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryItemAddViewModel UpdateEffortGradeLibraryCategoryItem(EffortGradeLibraryCategoryItemAddViewModel effortGradeLibraryCategoryItemAddViewModel)
        {
            if(effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem is null)
            {
                return effortGradeLibraryCategoryItemAddViewModel;
            }
            try
            {
                var EffortGradeLibraryCategoryItemUpdate = this.context?.EffortGradeLibraryCategoryItem.FirstOrDefault(x => x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.TenantId && x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.EffortItemId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemId);
                if (EffortGradeLibraryCategoryItemUpdate != null)
                {
                    var effortGradeLibraryCategoryItemList = this.context?.EffortGradeLibraryCategoryItem.AsEnumerable().FirstOrDefault(x => x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.TenantId  && x.EffortCategoryId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortCategoryId && x.EffortItemId!= EffortGradeLibraryCategoryItemUpdate.EffortItemId &&/* x.EffortItemTitle.ToLower() == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemTitle.ToLower()*/
                    String.Compare(x.EffortItemTitle, effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemTitle, true) == 0
                    );
                    if (effortGradeLibraryCategoryItemList !=null)
                    {
                        effortGradeLibraryCategoryItemAddViewModel._failure = true;
                        effortGradeLibraryCategoryItemAddViewModel._message = "Effort Grade Library Category Item Name Already Exists";
                    }
                    else
                    {
                        effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.CreatedBy = EffortGradeLibraryCategoryItemUpdate.CreatedBy;
                        effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.CreatedOn = EffortGradeLibraryCategoryItemUpdate.CreatedOn;
                        effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SortOrder = EffortGradeLibraryCategoryItemUpdate.SortOrder;
                        effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.UpdatedOn = DateTime.Now;
                        this.context?.Entry(EffortGradeLibraryCategoryItemUpdate).CurrentValues.SetValues(effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem);

                        this.context?.SaveChanges();
                        effortGradeLibraryCategoryItemAddViewModel._failure = false;
                        effortGradeLibraryCategoryItemAddViewModel._message = "Effort Grade Library Category Item Updated Successfully";
                    }                    
                }
                else
                {
                    effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem = null;
                    effortGradeLibraryCategoryItemAddViewModel._failure = true;
                    effortGradeLibraryCategoryItemAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                effortGradeLibraryCategoryItemAddViewModel._failure = true;
                effortGradeLibraryCategoryItemAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryItemAddViewModel;
        }
        
        /// <summary>
        /// Delete Effort Grade Library Category Item
        /// </summary>
        /// <param name="effortGradeLibraryCategoryItemAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeLibraryCategoryItemAddViewModel DeleteEffortGradeLibraryCategoryItem(EffortGradeLibraryCategoryItemAddViewModel effortGradeLibraryCategoryItemAddViewModel)
        {
            try
            {
                var EffortGradeLibraryCategoryItemDelete = this.context?.EffortGradeLibraryCategoryItem.FirstOrDefault(x => x.TenantId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem!.TenantId && x.SchoolId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.SchoolId && x.EffortItemId == effortGradeLibraryCategoryItemAddViewModel.EffortGradeLibraryCategoryItem.EffortItemId);
                
                if (EffortGradeLibraryCategoryItemDelete != null)
                {
                    this.context?.EffortGradeLibraryCategoryItem.Remove(EffortGradeLibraryCategoryItemDelete);
                    this.context?.SaveChanges();
                    effortGradeLibraryCategoryItemAddViewModel._failure = false;
                    effortGradeLibraryCategoryItemAddViewModel._message = "Effort Grade Library Category Item Deleted Successfully";
                }
                else
                {
                    effortGradeLibraryCategoryItemAddViewModel._failure = true;
                    effortGradeLibraryCategoryItemAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                effortGradeLibraryCategoryItemAddViewModel._failure = true;
                effortGradeLibraryCategoryItemAddViewModel._message = es.Message;
            }
            return effortGradeLibraryCategoryItemAddViewModel;
        }
        /// <summary>
        
        /// Get All Effort Grade Llibrary Category List
        /// </summary>
        /// <param name="effortGradeLlibraryCategoryListViewModel"></param>
        /// <returns></returns>
        public EffortGradeLlibraryCategoryListViewModel GetAllEffortGradeLlibraryCategoryList(EffortGradeLlibraryCategoryListViewModel effortGradeLlibraryCategoryListViewModel)
        {
            EffortGradeLlibraryCategoryListViewModel effortGradeLlibraryCategoryListModel = new ();
            try
            {

                var effortGradeLlibraryCategoryList = this.context?.EffortGradeLibraryCategory.Include(x => x.EffortGradeLibraryCategoryItem).Where(e => e.TenantId == effortGradeLlibraryCategoryListViewModel.TenantId && e.SchoolId == effortGradeLlibraryCategoryListViewModel.SchoolId).OrderBy(e => e.SortOrder)
                    .Select(p => new EffortGradeLibraryCategory()
                    {
                        SchoolId = p.SchoolId,
                        TenantId = p.TenantId,
                        EffortCategoryId = p.EffortCategoryId,
                        CategoryName=p.CategoryName,
                        CreatedBy= p.CreatedBy,
                        CreatedOn =p.CreatedOn,
                        SortOrder = p.SortOrder,
                        UpdatedBy = p.UpdatedBy,
                        UpdatedOn =p.UpdatedOn,
                        EffortGradeLibraryCategoryItem = p.EffortGradeLibraryCategoryItem.OrderBy(c => c.SortOrder).ToList()
                    }).ToList();
                
                if(effortGradeLlibraryCategoryList?.Any()==true)
                {
                    foreach (var effortGradeLlibraryCategory in effortGradeLlibraryCategoryList)
                    {
                        if (effortGradeLlibraryCategory.EffortGradeLibraryCategoryItem.Count>0)
                        {
                            if (effortGradeLlibraryCategoryListViewModel.IsListView == true)
                            {
                                effortGradeLlibraryCategory.EffortGradeLibraryCategoryItem.ToList().ForEach(c =>
                                {
                                    c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, effortGradeLlibraryCategoryListViewModel.TenantId, c.CreatedBy);
                                    c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, effortGradeLlibraryCategoryListViewModel.TenantId, c.UpdatedBy);
                                });
                            }
                        }                        
                    }

                    effortGradeLlibraryCategoryListModel.EffortGradeLibraryCategoryList = effortGradeLlibraryCategoryList;
                    effortGradeLlibraryCategoryListModel._failure = false;
                }
                else
                {
                    effortGradeLlibraryCategoryListModel.EffortGradeLibraryCategoryList = new();
                    effortGradeLlibraryCategoryListModel._failure = true;
                    effortGradeLlibraryCategoryListModel._message = NORECORDFOUND;
                }
                effortGradeLlibraryCategoryListModel._tenantName = effortGradeLlibraryCategoryListViewModel._tenantName;
                effortGradeLlibraryCategoryListModel._token = effortGradeLlibraryCategoryListViewModel._token;
            }
            catch (Exception es)
            {
                effortGradeLlibraryCategoryListModel._message = es.Message;
                effortGradeLlibraryCategoryListModel._failure = true;
                effortGradeLlibraryCategoryListModel._tenantName = effortGradeLlibraryCategoryListViewModel._tenantName;
                effortGradeLlibraryCategoryListModel._token = effortGradeLlibraryCategoryListViewModel._token;
            }
            return effortGradeLlibraryCategoryListModel;
        }
        
        /// <summary>
        /// Update Effort Grade Llibrary Category Sort Order
        /// </summary>
        /// <param name="effortgradeLibraryCategorySortOrderModel"></param>
        /// <returns></returns>
        public EffortgradeLibraryCategorySortOrderModel UpdateEffortGradeLlibraryCategorySortOrder(EffortgradeLibraryCategorySortOrderModel effortgradeLibraryCategorySortOrderModel)
        {
            try
            {
                if( effortgradeLibraryCategorySortOrderModel.EffortCategoryId>0)
                {
                    var EffortGradeLibraryCategoryItemRecords = new List<EffortGradeLibraryCategoryItem>();

                    var targetEffortGradeLibraryCategoryItem = this.context?.EffortGradeLibraryCategoryItem.FirstOrDefault(x => x.SortOrder == effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId && x.EffortCategoryId == effortgradeLibraryCategorySortOrderModel.EffortCategoryId);

                    if (targetEffortGradeLibraryCategoryItem != null)
                    {
                        targetEffortGradeLibraryCategoryItem.SortOrder = effortgradeLibraryCategorySortOrderModel.CurrentSortOrder;

                        if (effortgradeLibraryCategorySortOrderModel.PreviousSortOrder > effortgradeLibraryCategorySortOrderModel.CurrentSortOrder)
                        {
                            EffortGradeLibraryCategoryItemRecords = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SortOrder >= effortgradeLibraryCategorySortOrderModel.CurrentSortOrder && x.SortOrder < effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId && x.EffortCategoryId == effortgradeLibraryCategorySortOrderModel.EffortCategoryId).ToList();

                            if (EffortGradeLibraryCategoryItemRecords?.Any()==true)
                            {
                                EffortGradeLibraryCategoryItemRecords.ForEach(x => ++x.SortOrder);
                            }
                        }
                        if (effortgradeLibraryCategorySortOrderModel.CurrentSortOrder > effortgradeLibraryCategorySortOrderModel.PreviousSortOrder)
                        {
                            EffortGradeLibraryCategoryItemRecords = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SortOrder <= effortgradeLibraryCategorySortOrderModel.CurrentSortOrder && x.SortOrder > effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId && x.EffortCategoryId == effortgradeLibraryCategorySortOrderModel.EffortCategoryId).ToList();
                            if (EffortGradeLibraryCategoryItemRecords?.Any()==true)
                            {
                                EffortGradeLibraryCategoryItemRecords.ForEach(x => --x.SortOrder);
                            }
                        }
                    }

                }                
                else
                {
                    var EffortGradeLibraryCategoryRecords = new List<EffortGradeLibraryCategory>();

                    var targetEffortGradeLibraryCategory = this.context?.EffortGradeLibraryCategory.FirstOrDefault(x => x.SortOrder == effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId);
                    if (targetEffortGradeLibraryCategory != null)
                    {
                        targetEffortGradeLibraryCategory.SortOrder = effortgradeLibraryCategorySortOrderModel.CurrentSortOrder;

                        if (effortgradeLibraryCategorySortOrderModel.PreviousSortOrder > effortgradeLibraryCategorySortOrderModel.CurrentSortOrder)
                        {
                            EffortGradeLibraryCategoryRecords = this.context?.EffortGradeLibraryCategory.Where(x => x.SortOrder >= effortgradeLibraryCategorySortOrderModel.CurrentSortOrder && x.SortOrder < effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId).ToList();

                            if (EffortGradeLibraryCategoryRecords?.Any()==true)
                            {
                                EffortGradeLibraryCategoryRecords.ForEach(x => ++x.SortOrder);
                            }
                        }
                        if (effortgradeLibraryCategorySortOrderModel.CurrentSortOrder > effortgradeLibraryCategorySortOrderModel.PreviousSortOrder)
                        {
                            EffortGradeLibraryCategoryRecords = this.context?.EffortGradeLibraryCategory.Where(x => x.SortOrder <= effortgradeLibraryCategorySortOrderModel.CurrentSortOrder && x.SortOrder > effortgradeLibraryCategorySortOrderModel.PreviousSortOrder && x.SchoolId == effortgradeLibraryCategorySortOrderModel.SchoolId && x.TenantId == effortgradeLibraryCategorySortOrderModel.TenantId).ToList();
                            if (EffortGradeLibraryCategoryRecords?.Any()==true)
                            {
                                EffortGradeLibraryCategoryRecords.ForEach(x => --x.SortOrder);
                            }
                        }
                    }

                }
                this.context?.SaveChanges();
                effortgradeLibraryCategorySortOrderModel._failure = false;
            }
            catch (Exception es)
            {
                effortgradeLibraryCategorySortOrderModel._message = es.Message;
                effortgradeLibraryCategorySortOrderModel._failure = true;
            }
            return effortgradeLibraryCategorySortOrderModel;
        }

        /// <summary>
        /// Add Effort Grade Scale
        /// </summary>
        /// <param name="effortGradeScaleAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeScaleAddViewModel AddEffortGradeScale(EffortGradeScaleAddViewModel effortGradeScaleAddViewModel)
        {
            if (effortGradeScaleAddViewModel.EffortGradeScale is null)
            {
                return effortGradeScaleAddViewModel;
            }
            EffortGradeScaleAddViewModel effortGradeScaleAdd = new ();
            try
            {
               
                var gradeScaleValue = this.context?.EffortGradeScale.FirstOrDefault(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId && x.GradeScaleValue.ToString() == effortGradeScaleAddViewModel.EffortGradeScale.GradeScaleValue.ToString());

                if (gradeScaleValue == null)
                {
                    int? effortGradeScaleId = 1;
                    int? SortOrder = 1;

                    var effortGradeScaleData = this.context?.EffortGradeScale.Where(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId).OrderByDescending(x => x.EffortGradeScaleId).FirstOrDefault();

                    if (effortGradeScaleData != null)
                    {
                        effortGradeScaleId = effortGradeScaleData.EffortGradeScaleId + 1;
                    }
                    var sortOrderData = this.context?.EffortGradeScale.Where(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

                    if (sortOrderData != null)
                    {
                        SortOrder = sortOrderData.SortOrder + 1;
                    }

                    effortGradeScaleAddViewModel.EffortGradeScale.EffortGradeScaleId = (int)effortGradeScaleId;
                    effortGradeScaleAddViewModel.EffortGradeScale.SortOrder = (int)SortOrder!;
                    effortGradeScaleAddViewModel.EffortGradeScale.CreatedOn = DateTime.UtcNow;
                    this.context?.EffortGradeScale.Add(effortGradeScaleAddViewModel.EffortGradeScale);
                    this.context?.SaveChanges();
                    effortGradeScaleAddViewModel._failure = false;
                    effortGradeScaleAddViewModel._message = "Effort Grade Scale Added Successfully";
                }
                else
                {
                    effortGradeScaleAddViewModel._failure = true;
                    effortGradeScaleAddViewModel._message = "GradeScaleValue Already Exits";
                }
            }
            catch (Exception es)
            {
                effortGradeScaleAddViewModel._failure = true;
                effortGradeScaleAddViewModel._message = es.Message;
            }
            return effortGradeScaleAddViewModel;
        }

        /// <summary>
        /// Update Effort Grade Scale
        /// </summary>
        /// <param name="effortGradeScaleAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeScaleAddViewModel UpdateEffortGradeScale(EffortGradeScaleAddViewModel effortGradeScaleAddViewModel)
        {
            if (effortGradeScaleAddViewModel.EffortGradeScale is null)
            {
                return effortGradeScaleAddViewModel;
            }
            try
            {
                var effortGradeScaleUpdate = this.context?.EffortGradeScale.FirstOrDefault(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId && x.EffortGradeScaleId == effortGradeScaleAddViewModel.EffortGradeScale.EffortGradeScaleId);

                if (effortGradeScaleUpdate != null)
                {
                    var gradeScaleValue = this.context?.EffortGradeScale.FirstOrDefault(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId && x.GradeScaleValue.ToString() == effortGradeScaleAddViewModel.EffortGradeScale.GradeScaleValue.ToString() && x.EffortGradeScaleId != effortGradeScaleUpdate.EffortGradeScaleId);

                    if (gradeScaleValue == null)
                    {
                        effortGradeScaleAddViewModel.EffortGradeScale.CreatedBy = effortGradeScaleUpdate.CreatedBy;
                        effortGradeScaleAddViewModel.EffortGradeScale.CreatedOn = effortGradeScaleUpdate.CreatedOn;
                        effortGradeScaleAddViewModel.EffortGradeScale.SortOrder = effortGradeScaleUpdate.SortOrder;
                        effortGradeScaleAddViewModel.EffortGradeScale.UpdatedOn = DateTime.Now;
                        this.context?.Entry(effortGradeScaleUpdate).CurrentValues.SetValues(effortGradeScaleAddViewModel.EffortGradeScale);
                        this.context?.SaveChanges();
                        effortGradeScaleAddViewModel._failure = false;
                        effortGradeScaleAddViewModel._message = "Effort Grade Scale Updated Successfully";
                    }
                    else
                    {
                        effortGradeScaleAddViewModel._failure = true;
                        effortGradeScaleAddViewModel._message = "GradeScaleValue Already Exits";
                    }
                }
                else
                {
                    effortGradeScaleAddViewModel.EffortGradeScale = null;
                    effortGradeScaleAddViewModel._message = NORECORDFOUND;
                    effortGradeScaleAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                effortGradeScaleAddViewModel._failure = true;
                effortGradeScaleAddViewModel._message = es.Message;
            }
            return effortGradeScaleAddViewModel;
        }

        /// <summary>
        /// Delete Effort Grade Scale
        /// </summary>
        /// <param name="effortGradeScaleAddViewModel"></param>
        /// <returns></returns>
        public EffortGradeScaleAddViewModel DeleteEffortGradeScale(EffortGradeScaleAddViewModel effortGradeScaleAddViewModel)
        {
            try
            {
                var effortGradeScaleDelete = this.context?.EffortGradeScale.FirstOrDefault(x => x.TenantId == effortGradeScaleAddViewModel.EffortGradeScale!.TenantId && x.SchoolId == effortGradeScaleAddViewModel.EffortGradeScale.SchoolId && x.EffortGradeScaleId == effortGradeScaleAddViewModel.EffortGradeScale.EffortGradeScaleId);

                if (effortGradeScaleDelete != null)
                {
                    this.context?.EffortGradeScale.Remove(effortGradeScaleDelete);
                    this.context?.SaveChanges();
                    effortGradeScaleAddViewModel._failure = false;
                    effortGradeScaleAddViewModel._message = "Effort Grade Scale Deleted Successfully";
                }
                else
                {
                    effortGradeScaleAddViewModel._message = NORECORDFOUND;
                    effortGradeScaleAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                effortGradeScaleAddViewModel._failure = true;
                effortGradeScaleAddViewModel._message = es.Message;
            }
            return effortGradeScaleAddViewModel;
        }

        /// <summary>
        /// Get All Effort Grade Scale
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public EffortGradeScaleListModel GetAllEffortGradeScale(PageResult pageResult)
        {
            EffortGradeScaleListModel effortGradeScaleList = new ();
            IQueryable<EffortGradeScale>? transactionIQ = null;

            var effortGradeScaleData = this.context?.EffortGradeScale.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId==pageResult.SchoolId);
            try
            {
                int totalCount = 0;
                if (effortGradeScaleData != null)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = effortGradeScaleData;
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                            transactionIQ = effortGradeScaleData.Where(x => x.GradeScaleValue != null && x.GradeScaleValue.ToString() == Columnvalue.ToString() || x.GradeScaleComment != null && x.GradeScaleComment.ToLower().Contains(Columnvalue.ToLower()));
                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, effortGradeScaleData).AsQueryable();
                        }
                    }
                    if (pageResult.SortingModel != null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                    }
                     totalCount = transactionIQ.Count();
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    effortGradeScaleList.EffortGradeScaleList = transactionIQ.ToList();
                    if (pageResult.IsListView == true)
                    {
                        effortGradeScaleList.EffortGradeScaleList.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                        });
                    }
                    //var effortList = transactionIQ.AsNoTracking().Select(s => new GetEffortGradeScaleForView
                    //{
                    //    TenantId = s.TenantId,
                    //    SchoolId = s.SchoolId,
                    //    EffortGradeScaleId = s.EffortGradeScaleId,
                    //    GradeScaleValue = s.GradeScaleValue,
                    //    GradeScaleComment = s.GradeScaleComment,
                    //    SortOrder = s.SortOrder
                    //}).ToList();
                }
                effortGradeScaleList.TenantId = pageResult.TenantId;
                effortGradeScaleList.SchoolId = pageResult.SchoolId;
                //effortGradeScaleList.effortGradeScaleList = transactionIQ.ToList();
                //effortGradeScaleList.getEffortGradeScaleForView = effortList;
                effortGradeScaleList.TotalCount = totalCount;
                effortGradeScaleList.PageNumber = pageResult.PageNumber;
                effortGradeScaleList._pageSize = pageResult.PageSize;
                effortGradeScaleList._tenantName = pageResult._tenantName;
                effortGradeScaleList._token = pageResult._token;
                effortGradeScaleList._failure = false;
            }
            catch (Exception es)
            {
                effortGradeScaleList._message = es.Message;
                effortGradeScaleList._failure = true;
                effortGradeScaleList._tenantName = pageResult._tenantName;
                effortGradeScaleList._token = pageResult._token;
            }
            return effortGradeScaleList;
        }

        /// <summary>
        /// Update Effort Grade Scale Sort Order
        /// </summary>
        /// <param name="effortGradeScaleSortOrderViewModel"></param>
        /// <returns></returns>
        public EffortGradeScaleSortOrderViewModel UpdateEffortGradeScaleSortOrder(EffortGradeScaleSortOrderViewModel effortGradeScaleSortOrderViewModel)
        {
            try
            {
                var effortGradeScaleRecords = new List<EffortGradeScale>();

                var targetEffortGradeScale = this.context?.EffortGradeScale.FirstOrDefault(x => x.SortOrder == effortGradeScaleSortOrderViewModel.PreviousSortOrder && x.SchoolId == effortGradeScaleSortOrderViewModel.SchoolId && x.TenantId == effortGradeScaleSortOrderViewModel.TenantId);
                if (targetEffortGradeScale != null)
                {
                  targetEffortGradeScale.SortOrder = effortGradeScaleSortOrderViewModel.CurrentSortOrder;
                }
                if (effortGradeScaleSortOrderViewModel.PreviousSortOrder > effortGradeScaleSortOrderViewModel.CurrentSortOrder)
                {
                    effortGradeScaleRecords = this.context?.EffortGradeScale.Where(x => x.SortOrder >= effortGradeScaleSortOrderViewModel.CurrentSortOrder && x.SortOrder < effortGradeScaleSortOrderViewModel.PreviousSortOrder && x.TenantId == effortGradeScaleSortOrderViewModel.TenantId && x.SchoolId == effortGradeScaleSortOrderViewModel.SchoolId).ToList();

                    if (effortGradeScaleRecords?.Any()==true)
                    {
                        effortGradeScaleRecords.ForEach(x => ++x.SortOrder);
                    }
                }
                if (effortGradeScaleSortOrderViewModel.CurrentSortOrder > effortGradeScaleSortOrderViewModel.PreviousSortOrder)
                {
                    effortGradeScaleRecords = this.context?.EffortGradeScale.Where(x => x.SortOrder <= effortGradeScaleSortOrderViewModel.CurrentSortOrder && x.SortOrder > effortGradeScaleSortOrderViewModel.PreviousSortOrder && x.SchoolId == effortGradeScaleSortOrderViewModel.SchoolId && x.TenantId == effortGradeScaleSortOrderViewModel.TenantId).ToList();
                    if (effortGradeScaleRecords?.Any()==true)
                    {
                        effortGradeScaleRecords.ForEach(x => --x.SortOrder);
                    }
                }
                this.context?.SaveChanges();
                effortGradeScaleSortOrderViewModel._failure = false;
            }
            catch (Exception es)
            {
                effortGradeScaleSortOrderViewModel._message = es.Message;
                effortGradeScaleSortOrderViewModel._failure = true;
            }
            return effortGradeScaleSortOrderViewModel;
        }

        /// <summary>
        /// Add Grade Us Standard
        /// </summary>
        /// <param name="gradeUsStandardAddViewModel"></param>
        /// <returns></returns>
        public GradeUsStandardAddViewModel AddGradeUsStandard(GradeUsStandardAddViewModel gradeUsStandardAddViewModel)
        {
            try
            {
                bool validStandardRefNo = CheckStandardRefNo(gradeUsStandardAddViewModel.gradeUsStandard!.TenantId, gradeUsStandardAddViewModel.gradeUsStandard.StandardRefNo);
                if (validStandardRefNo == true)
                {
                    int GradeStandardId = 1;
                    var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == gradeUsStandardAddViewModel.gradeUsStandard.TenantId && x.SchoolId == gradeUsStandardAddViewModel.gradeUsStandard.SchoolId).OrderByDescending(x => x.GradeStandardId).FirstOrDefault();

                    if (gradeUsStandardData != null)
                    {
                        GradeStandardId = gradeUsStandardData.GradeStandardId + 1;
                    }
                    gradeUsStandardAddViewModel.gradeUsStandard.GradeStandardId = GradeStandardId;
                    gradeUsStandardAddViewModel.gradeUsStandard.CreatedOn = DateTime.UtcNow;                    
                    gradeUsStandardAddViewModel.gradeUsStandard.IsSchoolSpecific = true;
                    this.context?.GradeUsStandard.Add(gradeUsStandardAddViewModel.gradeUsStandard);
                    this.context?.SaveChanges();
                    gradeUsStandardAddViewModel._failure = false;
                    gradeUsStandardAddViewModel._message = "School Specific Standard Added Successfully";
                }
                else
                {
                    gradeUsStandardAddViewModel._failure = true;
                    gradeUsStandardAddViewModel._message = "StandardRefNo Already Exits";
                }
            }
            catch (Exception es)
            {
                gradeUsStandardAddViewModel._failure = true;
                gradeUsStandardAddViewModel._message = es.Message;
            }
            return gradeUsStandardAddViewModel;
        }

        //Checking StandardRefNo is Exits or not.
        private bool CheckStandardRefNo(Guid TenantId,string StandardRefNo)
        {
            if (StandardRefNo != null && StandardRefNo != "")
            {
                var checkStandardRefNo = this.context?.GradeUsStandard.Where(x => x.TenantId == TenantId && x.StandardRefNo == StandardRefNo).ToList();
                if (checkStandardRefNo?.Any()==true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update Grade Us Standard
        /// </summary>
        /// <param name="gradeUsStandardAddViewModel"></param>
        /// <returns></returns>
        public GradeUsStandardAddViewModel UpdateGradeUsStandard(GradeUsStandardAddViewModel gradeUsStandardAddViewModel)
        {
            if (gradeUsStandardAddViewModel.gradeUsStandard is null)
            {
                return gradeUsStandardAddViewModel;
            }
            try
            {
                var gradeUsStandardUpdate = this.context?.GradeUsStandard.FirstOrDefault(x => x.TenantId == gradeUsStandardAddViewModel.gradeUsStandard.TenantId && x.SchoolId == gradeUsStandardAddViewModel.gradeUsStandard.SchoolId && x.StandardRefNo == gradeUsStandardAddViewModel.gradeUsStandard.StandardRefNo);
                if (gradeUsStandardUpdate != null)
                {
                    gradeUsStandardAddViewModel.gradeUsStandard.CreatedBy = gradeUsStandardUpdate.CreatedBy;
                    gradeUsStandardAddViewModel.gradeUsStandard.CreatedOn = gradeUsStandardUpdate.CreatedOn;
                    gradeUsStandardAddViewModel.gradeUsStandard.UpdatedOn = DateTime.Now;
                    gradeUsStandardAddViewModel.gradeUsStandard.IsSchoolSpecific = true;
                    this.context?.Entry(gradeUsStandardUpdate).CurrentValues.SetValues(gradeUsStandardAddViewModel.gradeUsStandard);
                    this.context?.SaveChanges();
                    gradeUsStandardAddViewModel._failure = false;
                    gradeUsStandardAddViewModel._message = "School Specific Standard Updated Successfully";
                }
                else
                {
                    gradeUsStandardAddViewModel.gradeUsStandard = null;
                    gradeUsStandardAddViewModel._message = NORECORDFOUND;
                    gradeUsStandardAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                gradeUsStandardAddViewModel._failure = true;
                gradeUsStandardAddViewModel._message = es.Message;
            }
            return gradeUsStandardAddViewModel;
        }

        /// <summary>
        /// Delete Grade Us Standard
        /// </summary>
        /// <param name="gradeUsStandardAddViewModel"></param>
        /// <returns></returns>
        public GradeUsStandardAddViewModel DeleteGradeUsStandard(GradeUsStandardAddViewModel gradeUsStandardAddViewModel)
        {
            try
            {
                var gradeUsStandardDelete = this.context?.GradeUsStandard.Include(x => x.CourseStandard).FirstOrDefault(x => x.TenantId == gradeUsStandardAddViewModel.gradeUsStandard!.TenantId && x.SchoolId == gradeUsStandardAddViewModel.gradeUsStandard.SchoolId && x.StandardRefNo == gradeUsStandardAddViewModel.gradeUsStandard.StandardRefNo);

                if (gradeUsStandardDelete != null)
                {
                    if (gradeUsStandardDelete?.CourseStandard.Any() == true)
                    {
                        gradeUsStandardAddViewModel._failure = true;
                        gradeUsStandardAddViewModel._message = "School Specific Standard Can Not Be Deleted. Because It Has Association";
                    }
                    else
                    {
                        this.context?.GradeUsStandard.Remove(gradeUsStandardDelete!);
                        this.context?.SaveChanges();
                        gradeUsStandardAddViewModel._failure = false;
                        gradeUsStandardAddViewModel._message = "School Specific Standard Deleted Successfully";
                    }
                }
                else
                {
                    gradeUsStandardAddViewModel._message = NORECORDFOUND;
                    gradeUsStandardAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                gradeUsStandardAddViewModel._failure = true;
                gradeUsStandardAddViewModel._message = es.Message;
            }
            return gradeUsStandardAddViewModel;
        }

        /// <summary>
        /// Get All Grade Us Standard List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public GradeUsStandardListModel GetAllGradeUsStandardList(PageResult pageResult)
        {
            GradeUsStandardListModel gradeUsStandardList = new ();
            IQueryable<GradeUsStandard>? transactionIQ = null;

            var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.IsSchoolSpecific == pageResult.IsSchoolSpecific);
            try
            {
                int? totalCount = 0;
                if (gradeUsStandardData != null)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = gradeUsStandardData;
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                            transactionIQ = gradeUsStandardData.Where(x => x.StandardRefNo != null && x.StandardRefNo.ToLower().Contains(Columnvalue.ToLower()) || x.GradeLevel != null && x.GradeLevel.ToLower().Contains(Columnvalue.ToLower()) || x.Domain != null && x.Domain.ToLower().Contains(Columnvalue.ToLower()) || x.Subject != null && x.Subject.ToLower().Contains(Columnvalue.ToLower()) || x.Course != null && x.Course.ToLower().Contains(Columnvalue.ToLower()) || x.Topic != null && x.Topic.ToLower().Contains(Columnvalue.ToLower()) || x.StandardDetails != null && x.StandardDetails.ToLower().Contains(Columnvalue.ToLower()));
                        }
                        else
                        {
                            if (pageResult.FilterParams?.Count == 3 && pageResult.FilterParams.ElementAt(0).ColumnName.ToLower() == "subject" && pageResult.FilterParams.ElementAt(1).ColumnName.ToLower() == "course" && pageResult.FilterParams.ElementAt(2).ColumnName.ToLower() == "gradelevel")
                            {
                                transactionIQ = gradeUsStandardData.Where(x => x.TenantId == pageResult.TenantId && (pageResult.FilterParams.ElementAt(0).FilterValue == null || (x.Subject == pageResult.FilterParams.ElementAt(0).FilterValue)) && (pageResult.FilterParams.ElementAt(1).FilterValue == null || (x.Course == pageResult.FilterParams.ElementAt(1).FilterValue)) && (pageResult.FilterParams.ElementAt(2).FilterValue == null || (x.GradeLevel == pageResult.FilterParams.ElementAt(2).FilterValue)));
                            }

                            //if (pageResult.FilterParams.Count == 3 && pageResult.FilterParams.ElementAt(0).ColumnName.ToLower() == "subject" && pageResult.FilterParams.ElementAt(1).ColumnName.ToLower() == "course" && pageResult.FilterParams.ElementAt(2).ColumnName.ToLower() == "gradelevel")
                            //{
                            //    transactionIQ = Utility.FilteredData(pageResult.FilterParams, gradeUsStandardData).AsQueryable();
                            //}
                            //else
                            //{
                            //    gradeUsStandardList._message = NORECORDFOUND;
                            //    gradeUsStandardList._failure = true;
                            //    gradeUsStandardList._tenantName = pageResult._tenantName;
                            //    gradeUsStandardList._token = pageResult._token;
                            //    return gradeUsStandardList;
                            //}

                        }
                    }
                    if (pageResult.SortingModel != null && transactionIQ!=null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                    }
                    totalCount = transactionIQ?.Count();
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0 && transactionIQ != null)
                    {
                        transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    gradeUsStandardList.GradeUsStandardList = transactionIQ != null?transactionIQ.ToList():new();

                    if (pageResult.IsListView == true)
                    {
                        gradeUsStandardList.GradeUsStandardList.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                        });
                    }
                    //var gradeUsList = transactionIQ.AsNoTracking().Select(s => new GetGradeUsStandardForView
                    //{
                    //    TenantId = s.TenantId,
                    //    SchoolId = s.SchoolId,
                    //    StandardRefNo = s.StandardRefNo,
                    //    GradeLevel=s.GradeLevel,
                    //    Domain = s.Domain,
                    //    Subject = s.Subject,
                    //    Course = s.Course,
                    //    StandardDetails = s.StandardDetails,
                    //    GradeStandardId=s.GradeStandardId,
                    //    Topic = s.Topic
                    //}).ToList();
                }
                gradeUsStandardList.TenantId = pageResult.TenantId;
                gradeUsStandardList.SchoolId = pageResult.SchoolId;
                //gradeUsStandardList.getGradeUsStandardView = gradeUsList;
                //gradeUsStandardList.gradeUsStandardList = transactionIQ.ToList();
                gradeUsStandardList.TotalCount = totalCount;
                gradeUsStandardList.PageNumber = pageResult.PageNumber;
                gradeUsStandardList._pageSize = pageResult.PageSize;
                gradeUsStandardList._tenantName = pageResult._tenantName;
                gradeUsStandardList._token = pageResult._token;
                gradeUsStandardList._failure = false;
            }
            catch (Exception es)
            {
                gradeUsStandardList._message = es.Message;
                gradeUsStandardList._failure = true;
                gradeUsStandardList._tenantName = pageResult._tenantName;
                gradeUsStandardList._token = pageResult._token;
            }
            return gradeUsStandardList;
        }

        /// <summary>
        /// Get All Subject Standard List
        /// </summary>
        /// <param name="gradeUsStandardListModel"></param>
        /// <returns></returns>
        public GradeUsStandardListModel GetAllSubjectStandardList(GradeUsStandardListModel gradeUsStandardListModel)
        {
            GradeUsStandardListModel subjectStandardList = new ();
            try
            {
                var subjectStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == gradeUsStandardListModel.TenantId && x.SchoolId == gradeUsStandardListModel.SchoolId && x.IsSchoolSpecific == true).Select(s => new { s.Subject, s.TenantId, s.SchoolId }).Distinct().ToList();

                if (subjectStandardData?.Any()==true)
                {
                    var subjectList = subjectStandardData.Select(s => new GradeUsStandard
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        Subject = s.Subject
                    }).ToList();

                    //subjectStandardList.getGradeUsStandardView = subjectList;
                    subjectStandardList.GradeUsStandardList = subjectList;
                    subjectStandardList._failure = false;
                }
                else
                {
                    subjectStandardList._failure = true;
                    subjectStandardList._message = NORECORDFOUND;
                }
                subjectStandardList._tenantName = gradeUsStandardListModel._tenantName;
                subjectStandardList._token = gradeUsStandardListModel._token;
            }
            catch (Exception es)
            {
                subjectStandardList.GradeUsStandardList = new();
                subjectStandardList._message = es.Message;
                subjectStandardList._failure = true;
                subjectStandardList._tenantName = gradeUsStandardListModel._tenantName;
                subjectStandardList._token = gradeUsStandardListModel._token;
            }
            return subjectStandardList;
        }

        /// <summary>
        /// Get All Course Standard List
        /// </summary>
        /// <param name="gradeUsStandardListModel"></param>
        /// <returns></returns>
        public GradeUsStandardListModel GetAllCourseStandardList(GradeUsStandardListModel gradeUsStandardListModel)
        {
            GradeUsStandardListModel courseStandardList = new ();
            try
            {
                var subjectCourseData = this.context?.GradeUsStandard.Where(x => x.TenantId == gradeUsStandardListModel.TenantId && x.SchoolId == gradeUsStandardListModel.SchoolId && x.IsSchoolSpecific == true).Select(s => new { s.Course, s.TenantId, s.SchoolId }).Distinct().ToList();

                if (subjectCourseData?.Any()==true)
                {
                    var courseList = subjectCourseData.Select(s => new GradeUsStandard
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        Course = s.Course
                    }).ToList();
                    //courseStandardList.getGradeUsStandardView = courseList;
                    courseStandardList.GradeUsStandardList = courseList;
                    courseStandardList._failure = false;
                }
                else
                {
                    courseStandardList._failure = true;
                    courseStandardList._message = NORECORDFOUND;
                }
                courseStandardList._tenantName = gradeUsStandardListModel._tenantName;
                courseStandardList._token = gradeUsStandardListModel._token;
            }
            catch (Exception es)
            {
                courseStandardList.GradeUsStandardList = new();
                courseStandardList._message = es.Message;
                courseStandardList._failure = true;
                courseStandardList._tenantName = gradeUsStandardListModel._tenantName;
                courseStandardList._token = gradeUsStandardListModel._token;
            }
            return courseStandardList;
        }

        /// <summary>
        /// Check Standard RefNo Is Valid Or Not
        /// </summary>
        /// <param name="checkStandardRefNoViewModel"></param>
        /// <returns></returns>
        public CheckStandardRefNoViewModel CheckStandardRefNo(CheckStandardRefNoViewModel checkStandardRefNoViewModel)
        {
            var checkStandardRefNo = this.context?.GradeUsStandard.Where(x => x.TenantId == checkStandardRefNoViewModel.TenantId && x.StandardRefNo == checkStandardRefNoViewModel.StandardRefNo).ToList();
            if (checkStandardRefNo?.Any()==true)
            {
                checkStandardRefNoViewModel.IsValidStandardRefNo = false;
                checkStandardRefNoViewModel._message = "StandardRefNo Already Exist,Please Try Again!!";
            }
            else
            {
                checkStandardRefNoViewModel.IsValidStandardRefNo = true;
                checkStandardRefNoViewModel._message = "StandardRefNo Id Is Valid";
            }
            return checkStandardRefNoViewModel;
        }
        
        /// <summary>
        /// Add Honor Roll
        /// </summary>
        /// <param name="honorRollsAddViewModel"></param>
        /// <returns></returns>
        public HonorRollsAddViewModel AddHonorRoll(HonorRollsAddViewModel honorRollsAddViewModel)
        {
            if(honorRollsAddViewModel.honorRolls is null)
            {
                return honorRollsAddViewModel;
            }
            try
            {
                honorRollsAddViewModel.honorRolls.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, honorRollsAddViewModel.honorRolls.TenantId, honorRollsAddViewModel.honorRolls.SchoolId);

                var checkHonorRoll = this.context?.HonorRolls.AsEnumerable().Where(x => x.SchoolId == honorRollsAddViewModel.honorRolls.SchoolId && x.TenantId == honorRollsAddViewModel.honorRolls.TenantId && String.Compare(x.HonorRoll,honorRollsAddViewModel.honorRolls.HonorRoll,true)==0 && x.AcademicYear == honorRollsAddViewModel.honorRolls.AcademicYear).FirstOrDefault();
                if (checkHonorRoll != null)
                {
                    honorRollsAddViewModel._failure = false;
                    honorRollsAddViewModel._message = "Honor Roll Already Exists";
                }
                else
                {
                    int? HonorRollId = 1;

                    var HonorRollData = this.context?.HonorRolls.Where(x => x.SchoolId == honorRollsAddViewModel.honorRolls.SchoolId && x.TenantId == honorRollsAddViewModel.honorRolls.TenantId).OrderByDescending(x => x.HonorRollId).FirstOrDefault();

                    if (HonorRollData != null)
                    {
                        HonorRollId = HonorRollData.HonorRollId + 1;
                    }

                    honorRollsAddViewModel.honorRolls.HonorRollId = (int)HonorRollId;                    
                    honorRollsAddViewModel.honorRolls.CreatedOn = DateTime.UtcNow;
                    this.context?.HonorRolls.Add(honorRollsAddViewModel.honorRolls);
                    this.context?.SaveChanges();
                    honorRollsAddViewModel._failure = false;
                    honorRollsAddViewModel._message = "Honor Roll Added Successfully";
                }
            }
            catch (Exception es)
            {
                honorRollsAddViewModel._failure = true;
                honorRollsAddViewModel._message = es.Message;
            }
            return honorRollsAddViewModel;
        }
        
        /// <summary>
        /// Update Honor Roll
        /// </summary>
        /// <param name="honorRollsAddViewModel"></param>
        /// <returns></returns>
        public HonorRollsAddViewModel UpdateHonorRoll(HonorRollsAddViewModel honorRollsAddViewModel)
        {
            if(honorRollsAddViewModel.honorRolls is null)
            {
                return honorRollsAddViewModel;
            }
            try
            {
                var honorRollUpdate = this.context?.HonorRolls.FirstOrDefault(x => x.TenantId == honorRollsAddViewModel.honorRolls.TenantId && x.SchoolId == honorRollsAddViewModel.honorRolls.SchoolId && x.HonorRollId == honorRollsAddViewModel.honorRolls.HonorRollId);
                if (honorRollUpdate != null)
                {
                    var checkHonorRoll= this.context?.HonorRolls.AsEnumerable().Where(x => x.SchoolId == honorRollsAddViewModel.honorRolls.SchoolId && x.TenantId == honorRollsAddViewModel.honorRolls.TenantId  && x.HonorRollId != honorRollsAddViewModel.honorRolls.HonorRollId &&
                    String.Compare(x.HonorRoll, honorRollsAddViewModel.honorRolls.HonorRoll)==0 && x.AcademicYear == honorRollUpdate.AcademicYear).FirstOrDefault();

                    if (checkHonorRoll != null)
                    {
                        honorRollsAddViewModel._failure = true;
                        honorRollsAddViewModel._message = "Honor Roll Already Exists";
                    }
                    else
                    {
                        honorRollsAddViewModel.honorRolls.CreatedBy = honorRollUpdate.CreatedBy;
                        honorRollsAddViewModel.honorRolls.CreatedOn = honorRollUpdate.CreatedOn;
                        honorRollsAddViewModel.honorRolls.UpdatedOn = DateTime.Now;
                        honorRollsAddViewModel.honorRolls.AcademicYear = honorRollUpdate.AcademicYear;
                        this.context?.Entry(honorRollUpdate).CurrentValues.SetValues(honorRollsAddViewModel.honorRolls);
                        this.context?.SaveChanges();
                        honorRollsAddViewModel._failure = false;
                        honorRollsAddViewModel._message = "Honor Roll Updated Successfully";
                    }
                }
                else
                {
                    honorRollsAddViewModel.honorRolls = null;
                    honorRollsAddViewModel._failure = true;
                    honorRollsAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                honorRollsAddViewModel._failure = true;
                honorRollsAddViewModel._message = es.Message;
            }
            return honorRollsAddViewModel;
        }
        
        /// <summary>
        /// Get All Honor Roll List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public HonorRollsListViewModel GetAllHonorRollList(PageResult pageResult)
        {
            HonorRollsListViewModel honorRollList = new ();
            IQueryable<HonorRolls>? transactionIQ = null;
            int? totalCount=0;
            var honorRollsData = this.context?.HonorRolls.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.AcademicYear == pageResult.AcademicYear);
            try
            {
                if (honorRollsData != null)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = honorRollsData;
                    }
                    else
                    {
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                            transactionIQ = honorRollsData?.Where(x => x.HonorRoll != null && x.Breakoff.ToString() == Columnvalue.ToString() || x.HonorRoll != null && x.HonorRoll.ToLower().Contains(Columnvalue.ToLower()));
                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, honorRollsData).AsQueryable();
                        }
                    }
                    if (pageResult.SortingModel != null && transactionIQ!=null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                    }
                    totalCount = transactionIQ?.Count();
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0 && transactionIQ!=null)
                    {
                        transactionIQ = transactionIQ.Select(e => new HonorRolls
                        {
                            //MarkingPeriodId = e.MarkingPeriodId,
                            HonorRollId = e.HonorRollId,
                            HonorRoll = e.HonorRoll,
                            Breakoff = e.Breakoff,
                            CreatedOn = e.CreatedOn,
                            UpdatedOn = e.UpdatedOn,
                            CreatedBy = e.CreatedBy,
                            UpdatedBy = e.UpdatedBy
                        }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    honorRollList.HonorRollList = transactionIQ!=null? transactionIQ.ToList():new();
                    if (pageResult.IsListView == true)
                    {
                        honorRollList.HonorRollList.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                        });
                    }
                }
                honorRollList.TenantId = pageResult.TenantId;
                honorRollList.SchoolId = pageResult.SchoolId;
                //honorRollList.honorRollList = transactionIQ.ToList();               
                honorRollList.TotalCount = totalCount;
                honorRollList.PageNumber = pageResult.PageNumber;
                honorRollList._pageSize = pageResult.PageSize;
                honorRollList._tenantName = pageResult._tenantName;
                honorRollList._token = pageResult._token;
                honorRollList._failure = false;
            }
            catch (Exception es)
            {
                honorRollList._message = es.Message;
                honorRollList._failure = true;
                honorRollList._tenantName = pageResult._tenantName;
                honorRollList._token = pageResult._token;
            }
            return honorRollList;
        }
        
        /// <summary>
        /// Delete Honor Roll
        /// </summary>
        /// <param name="honorRollsAddViewModel"></param>
        /// <returns></returns>
        public HonorRollsAddViewModel DeleteHonorRoll(HonorRollsAddViewModel honorRollsAddViewModel)
        {
            try
            {
                var honorRollDelete = this.context?.HonorRolls.FirstOrDefault(x => x.TenantId == honorRollsAddViewModel.honorRolls!.TenantId && x.SchoolId == honorRollsAddViewModel.honorRolls.SchoolId && x.HonorRollId == honorRollsAddViewModel.honorRolls.HonorRollId);

                if (honorRollDelete != null)
                {
                    this.context?.HonorRolls.Remove(honorRollDelete);
                    this.context?.SaveChanges();
                    honorRollsAddViewModel._failure = false;
                    honorRollsAddViewModel._message = "Honor Roll Deleted Successfully";
                }
                else
                {
                    honorRollsAddViewModel._message = NORECORDFOUND;
                    honorRollsAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                honorRollsAddViewModel._failure = true;
                honorRollsAddViewModel._message = es.Message;
            }
            return honorRollsAddViewModel;
        }

        /// <summary>
        /// Add Us Standard Data
        /// </summary>
        /// <param name="jurisdictionByIdListViewModel"></param>
        /// <returns></returns>
        //public JurisdictionByIdListViewModel AddUsStandardData(JurisdictionByIdListViewModel jurisdictionByIdListViewModel)
        //{
        //    JurisdictionByIdListViewModel? jurisdictionByIdListViewModelData = new ();
        //    try
        //    {
        //        //Fetch the JSON string from URL.
        //        string apiUrl = "https://api.commonstandardsproject.com/api/v1/jurisdictions/67810E9EF6944F9383DCC602A3484C23";

        //        HttpClient client = new ();
        //        HttpResponseMessage response = client.GetAsync(apiUrl).Result;
        //        string? jurisdictionData = response.Content.ReadAsStringAsync().Result.ToString();

        //        if (jurisdictionData != null)
        //        {
        //            jurisdictionByIdListViewModelData = JsonConvert.DeserializeObject<JurisdictionByIdListViewModel>(jurisdictionData);
        //            List<GradeUsStandard> gradeUsStandard = new();
        //            if (jurisdictionByIdListViewModelData?.data != null)
        //            {

        //                foreach (var data in jurisdictionByIdListViewModelData.data.standardSets)
        //                {
        //                    JurisdictionByIdListViewModel gradeUsStandardListViewModel = new();
        //                    gradeUsStandardListViewModel = this.GetStandardSets(data.id??"", jurisdictionByIdListViewModel.TenantId, jurisdictionByIdListViewModel.SchoolId, jurisdictionByIdListViewModel.CreatedBy);
        //                    gradeUsStandard.AddRange(gradeUsStandardListViewModel.gradeUsStandardList);
        //                }

        //            }

        //           jurisdictionByIdListViewModelData?.gradeUsStandardList.AddRange(gradeUsStandard.Take(10));
        //           jurisdictionByIdListViewModelData._failure = false;
        //           jurisdictionByIdListViewModelData._message = "US standard data added successfully.";
        //         }

        //       // return jurisdictionByIdListViewModelData;
        //    }
        //    catch (Exception es)
        //    {
        //        jurisdictionByIdListViewModelData._failure = true;
        //        jurisdictionByIdListViewModelData._message = es.Message;
        //    }
        //    return jurisdictionByIdListViewModelData;
        //}

        //private JurisdictionByIdListViewModel GetStandardSets(string standardSetid, Guid tenantId, int schoolId, string? createdBy)
        //{

        //    //Fetch the JSON string from URL.
        //    JurisdictionByIdListViewModel jurisdictionByIdListViewModel = new JurisdictionByIdListViewModel();
        //    StandardSetsListViewModel? standardSetsData = new StandardSetsListViewModel();
        //    List<standards> standards = new List<standards>();

        //    string apiUrl = "https://api.commonstandardsproject.com/api/v1/standard_sets/" + standardSetid;

        //    HttpClient client = new HttpClient();
        //    HttpResponseMessage response = client.GetAsync(apiUrl).Result;

        //    var standardSetsData1 = response.Content.ReadAsStringAsync().Result.ToString();
        //    standardSetsData = JsonConvert.DeserializeObject<StandardSetsListViewModel>(standardSetsData1);


        //    JToken jToken = JToken.Parse(standardSetsData1.ToString());

        //    var educationLevels = jToken.SelectToken("data").SelectToken("educationLevels").Value<object>();

        //    var educationLevelsDataList = (JArray)JsonConvert.DeserializeObject(educationLevels.ToString());

        //    List<GradeUsStandard> gradeUsStandardListData = new List<GradeUsStandard>();

        //    int GradeStandardId = 1;
        //    var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == tenantId && x.SchoolId == schoolId).OrderByDescending(x => x.GradeStandardId).FirstOrDefault();

        //    if (gradeUsStandardData != null)
        //    {
        //        GradeStandardId = gradeUsStandardData.GradeStandardId + 1;
        //    }

        //    foreach (var educationLevelsData in educationLevelsDataList)
        //    {
        //        var data = jToken.SelectToken("data").SelectToken("standards").Value<object>();
        //        var standardsDataList = (JObject)JsonConvert.DeserializeObject(data.ToString());

        //        if (standardSetsData.data.subject == "Common Core Mathematics")
        //        {
        //            foreach (var standardsData in standardsDataList)
        //            {
        //                //GradeUsStandardListViewModel gradeUsStandardList = new GradeUsStandardListViewModel();
        //                if (standardsData.Value.SelectToken("statementLabel") != null && standardsData.Value.SelectToken("statementLabel").Value<string>() == "Domain")
        //                {
        //                    var id = standardsData.Value.SelectToken("id").Value<string>();

        //                    List<Notations> subjects = new List<Notations>();
        //                    string topic = null;
        //                    foreach (var standardsData2 in standardsDataList)
        //                    {
        //                        GradeUsStandard gradeUsStandardList = new GradeUsStandard();
        //                        if (standardsData2.Value.SelectToken("statementLabel") != null && standardsData2.Value.SelectToken("statementLabel").Value<string>() == "Standard")
        //                        {
        //                            foreach (var standardsData3 in standardsDataList)
        //                            {
        //                                if (standardsData3.Value.SelectToken("statementLabel") != null && standardsData3.Value.SelectToken("statementLabel").Value<string>() == "Cluster")
        //                                {
        //                                    var id1 = standardsData3.Value.SelectToken("id").Value<string>();
        //                                    var ancestorIdsList1 = standardsData2.Value.SelectToken("ancestorIds").ToArray();
        //                                    foreach (var ancestorIds1 in ancestorIdsList1)
        //                                    {
        //                                        if (id1 == ancestorIds1.ToString())
        //                                        {
        //                                            topic = standardsData3.Value.SelectToken("description").Value<string>();
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            var ancestorIdsList = standardsData2.Value.SelectToken("ancestorIds").ToArray();

        //                            foreach (var ancestorIds in ancestorIdsList)
        //                            {
        //                                if (id == ancestorIds.ToString())
        //                                {

        //                                    gradeUsStandardList.GradeStandardId = GradeStandardId;
        //                                    gradeUsStandardList.TenantId = tenantId;
        //                                    gradeUsStandardList.SchoolId = schoolId;
        //                                    gradeUsStandardList.Subject = "Mathematics";

        //                                    if (educationLevelsData.ToObject<string>().ToLower() == "K".ToLower() || educationLevelsData.ToObject<string>().ToLower() == "Kindergarten".ToLower())
        //                                    {
        //                                        gradeUsStandardList.GradeLevel = "Kindergarten";
        //                                    }
        //                                    else
        //                                    {
        //                                        gradeUsStandardList.GradeLevel = this.context?.GradeEquivalency.Where(x => x.EquivalencyId == educationLevelsData.ToObject<int>()).Select(y => y.GradeLevelEquivalency).FirstOrDefault();
        //                                    }

        //                                    gradeUsStandardList.Course = jToken.SelectToken("data").SelectToken("subject").Value<string>();
        //                                    gradeUsStandardList.StandardRefNo = standardsData2.Value.SelectToken("statementNotation").Value<string>();
        //                                    gradeUsStandardList.Domain = standardsData.Value.SelectToken("description").Value<string>();
        //                                    gradeUsStandardList.StandardDetails = standardsData2.Value.SelectToken("description").Value<string>();
        //                                    gradeUsStandardList.Topic = topic;
        //                                    gradeUsStandardList.IsSchoolSpecific = false;
        //                                    gradeUsStandardList.CreatedBy = createdBy;
        //                                    gradeUsStandardList.CreatedOn = DateTime.UtcNow;

        //                                    gradeUsStandardListData.Add(gradeUsStandardList);
        //                                    GradeStandardId++;

        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var standardsData in standardsDataList)
        //            {
        //                //GradeUsStandardListViewModel gradeUsStandardList = new GradeUsStandardListViewModel();
        //                if (standardsData.Value.SelectToken("statementLabel") == null)
        //                {
        //                    var id = standardsData.Value.SelectToken("id").Value<string>();

        //                    List<Notations> subjects = new List<Notations>();
        //                    string topic = null;
        //                    foreach (var standardsData2 in standardsDataList)
        //                    {
        //                        GradeUsStandard gradeUsStandardList = new GradeUsStandard();
        //                        if (standardsData2.Value.SelectToken("statementLabel") != null && standardsData2.Value.SelectToken("statementLabel").Value<string>() == "Standard")
        //                        {
        //                            foreach (var standardsData3 in standardsDataList)
        //                            {
        //                                if (standardsData3.Value.SelectToken("statementLabel") == null)
        //                                {
        //                                    var id1 = standardsData3.Value.SelectToken("id").Value<string>();
        //                                    var ancestorIdsList1 = standardsData2.Value.SelectToken("ancestorIds").ToArray();
        //                                    foreach (var ancestorIds1 in ancestorIdsList1)
        //                                    {
        //                                        if (id1 == ancestorIds1.ToString())
        //                                        {
        //                                            topic = standardsData3.Value.SelectToken("description").Value<string>();
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            var ancestorIdsList = standardsData2.Value.SelectToken("ancestorIds").ToArray();

        //                            foreach (var ancestorIds in ancestorIdsList)
        //                            {
        //                                if (id == ancestorIds.ToString())
        //                                {
        //                                    gradeUsStandardList.GradeStandardId = GradeStandardId;
        //                                    gradeUsStandardList.TenantId = tenantId;
        //                                    gradeUsStandardList.SchoolId = schoolId;
        //                                    gradeUsStandardList.Subject = "English";

        //                                    if (educationLevelsData.ToObject<string>().ToLower() == "K".ToLower() || educationLevelsData.ToObject<string>().ToLower() == "Kindergarten".ToLower())
        //                                    {
        //                                        gradeUsStandardList.GradeLevel = "Kindergarten";
        //                                    }
        //                                    else
        //                                    {
        //                                        gradeUsStandardList.GradeLevel = this.context?.GradeEquivalency.Where(x => x.EquivalencyId == educationLevelsData.ToObject<int>()).Select(y => y.GradeLevelEquivalency).FirstOrDefault();
        //                                    }

        //                                    gradeUsStandardList.Course = jToken.SelectToken("data").SelectToken("subject").Value<string>();
        //                                    gradeUsStandardList.StandardRefNo = standardsData2.Value.SelectToken("statementNotation").Value<string>();
        //                                    gradeUsStandardList.Domain = standardsData.Value.SelectToken("description").Value<string>();
        //                                    gradeUsStandardList.StandardDetails = standardsData2.Value.SelectToken("description").Value<string>();
        //                                    gradeUsStandardList.Topic = topic;
        //                                    gradeUsStandardList.IsSchoolSpecific = false;
        //                                    gradeUsStandardList.CreatedBy = createdBy;
        //                                    gradeUsStandardList.CreatedOn = DateTime.UtcNow;

        //                                    gradeUsStandardListData.Add(gradeUsStandardList);
        //                                    GradeStandardId++;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    jurisdictionByIdListViewModel.gradeUsStandardList.AddRange(gradeUsStandardListData);
        //    this.context?.GradeUsStandard.AddRange(jurisdictionByIdListViewModel.gradeUsStandardList);
        //    this.context?.SaveChanges();

        //    return jurisdictionByIdListViewModel;
        //}


        public JurisdictionByIdListViewModel AddUsStandardData(JurisdictionByIdListViewModel jurisdictionByIdListViewModel)
        {
            JurisdictionByIdListViewModel? jurisdictionByIdListViewModelData = new();
            try
            {
                //Fetch the JSON string from URL.
                string apiUrl = "https://api.commonstandardsproject.com/api/v1/jurisdictions/67810E9EF6944F9383DCC602A3484C23";

                HttpClient client = new();
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string? jurisdictionData = response.Content.ReadAsStringAsync().Result.ToString();

                if (jurisdictionData != null)
                {
                    jurisdictionByIdListViewModelData = JsonConvert.DeserializeObject<JurisdictionByIdListViewModel>(jurisdictionData);
                    List<GradeUsStandard> gradeUsStandard = new();

                    if (jurisdictionByIdListViewModelData != null)
                    {
                        if (jurisdictionByIdListViewModelData?.data != null)
                        {

                            foreach (var data in jurisdictionByIdListViewModelData.data.standardSets)
                            {
                                JurisdictionByIdListViewModel gradeUsStandardListViewModel = new();
                                gradeUsStandardListViewModel = this.GetStandardSets(data.id ?? "", jurisdictionByIdListViewModel.TenantId, jurisdictionByIdListViewModel.SchoolId, jurisdictionByIdListViewModel.CreatedBy);
                                gradeUsStandard.AddRange(gradeUsStandardListViewModel.gradeUsStandardList);
                            }
                            jurisdictionByIdListViewModelData.gradeUsStandardList.AddRange(gradeUsStandard.Take(10));
                            jurisdictionByIdListViewModelData._failure = false;
                            jurisdictionByIdListViewModelData._message = "US standard data added successfully.";
                        }
                    }

                    //jurisdictionByIdListViewModelData?.gradeUsStandardList.AddRange(gradeUsStandard.Take(10));
                    //jurisdictionByIdListViewModelData._failure = false;
                    //jurisdictionByIdListViewModelData._message = "US standard data added successfully.";
                }

                // return jurisdictionByIdListViewModelData;
            }
            catch (Exception es)
            {
                jurisdictionByIdListViewModelData!._failure = true;
                jurisdictionByIdListViewModelData._message = es.Message;
            }
            //return jurisdictionByIdListViewModelData;
            return jurisdictionByIdListViewModelData!;
        }

        private JurisdictionByIdListViewModel GetStandardSets(string standardSetid, Guid tenantId, int schoolId, string? createdBy)
        {

            //Fetch the JSON string from URL.
            JurisdictionByIdListViewModel jurisdictionByIdListViewModel = new ();
            StandardSetsListViewModel? standardSetsData = new ();
            List<standards> standards = new ();

            string apiUrl = "https://api.commonstandardsproject.com/api/v1/standard_sets/" + standardSetid;

            HttpClient client = new ();
            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            var standardSetsData1 = response.Content.ReadAsStringAsync().Result.ToString();
            standardSetsData = JsonConvert.DeserializeObject<StandardSetsListViewModel>(standardSetsData1);


            JToken jToken = JToken.Parse(standardSetsData1.ToString());
            if (jToken != null)
            {
                var educationLevels = jToken?.SelectToken("data")?.SelectToken("educationLevels")?.Value<object?>();
                JArray? educationLevelsDataList = JsonConvert.DeserializeObject(educationLevels?.ToString() ?? "") as JArray;

            List<GradeUsStandard> gradeUsStandardListData = new ();

            int GradeStandardId = 1;
            var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == tenantId && x.SchoolId == schoolId).OrderByDescending(x => x.GradeStandardId).FirstOrDefault();

            if (gradeUsStandardData != null)
            {
                GradeStandardId = gradeUsStandardData.GradeStandardId + 1;
            }
                if (educationLevelsDataList != null)
                {
                    foreach (var educationLevelsData in educationLevelsDataList)
                    {
                        var data = jToken?.SelectToken("data")?.SelectToken("standards")?.Value<object>();
                        var standardsDataList = JsonConvert.DeserializeObject(data?.ToString() ?? "") as JObject;
                        if (standardsDataList != null)
                        {
                            if (standardSetsData?.data?.subject == "Common Core Mathematics")
                            {
                               
                                    foreach (var standardsData in standardsDataList)
                                    {
                                        //GradeUsStandardListViewModel gradeUsStandardList = new GradeUsStandardListViewModel();
                                        if (standardsData.Value?.SelectToken("statementLabel") != null && standardsData.Value?.SelectToken("statementLabel")?.Value<string>() == "Domain")
                                        {
                                            var id = standardsData.Value?.SelectToken("id")?.Value<string>();

                                            List<Notations> subjects = new();
                                            string? topic = null;
                                            foreach (var standardsData2 in standardsDataList)
                                            {
                                                GradeUsStandard gradeUsStandardList = new GradeUsStandard();
                                                if (standardsData2.Value?.SelectToken("statementLabel") != null && standardsData2.Value?.SelectToken("statementLabel")?.Value<string>() == "Standard")
                                                {
                                                    foreach (var standardsData3 in standardsDataList)
                                                    {
                                                        if (standardsData3.Value?.SelectToken("statementLabel") != null && standardsData3.Value.SelectToken("statementLabel")?.Value<string>() == "Cluster")
                                                        {
                                                            var id1 = standardsData3.Value.SelectToken("id")?.Value<string>();
                                                            var ancestorIdsList1 = standardsData2.Value.SelectToken("ancestorIds")?.ToArray();
                                                            if (ancestorIdsList1 != null)
                                                            {
                                                                foreach (var ancestorIds1 in ancestorIdsList1)
                                                                {
                                                                    if (id1 == ancestorIds1.ToString())
                                                                    {
                                                                        topic = standardsData3.Value.SelectToken("description")?.Value<string>();
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }

                                                    var ancestorIdsList = standardsData2.Value.SelectToken("ancestorIds")?.ToArray();
                                                    if (ancestorIdsList != null)
                                                    {
                                                        foreach (var ancestorIds in ancestorIdsList)
                                                        {
                                                            if (id == ancestorIds.ToString())
                                                            {

                                                                gradeUsStandardList.GradeStandardId = GradeStandardId;
                                                                gradeUsStandardList.TenantId = tenantId;
                                                                gradeUsStandardList.SchoolId = schoolId;
                                                                gradeUsStandardList.Subject = "Mathematics";
                                                                //if (educationLevelsData.ToObject<string>().ToLower() == "K".ToLower() || educationLevelsData.ToObject<string>().ToLower() == "Kindergarten".ToLower())
                                                                if (String.Compare(educationLevelsData.ToObject<string>(), "K", true) == 0 || String.Compare(educationLevelsData.ToObject<string>(), "Kindergarten", true) == 0)
                                                                {
                                                                    gradeUsStandardList.GradeLevel = "Kindergarten";
                                                                }
                                                                else
                                                                {
                                                                    gradeUsStandardList.GradeLevel = this.context?.GradeEquivalency.Where(x => x.EquivalencyId == educationLevelsData.ToObject<int>()).Select(y => y.GradeLevelEquivalency).FirstOrDefault();
                                                                }

                                                                gradeUsStandardList.Course = jToken!.SelectToken("data")?.SelectToken("subject")?.Value<string>();
                                                            //gradeUsStandardList.StandardRefNo = standardsData2.Value?.SelectToken("statementNotation")?.Value<string>();
                                                            gradeUsStandardList.StandardRefNo = standardsData2.Value?.SelectToken("statementNotation")?.Value<string>()!;
                                                            gradeUsStandardList.Domain = standardsData.Value?.SelectToken("description")?.Value<string>();
                                                            //gradeUsStandardList.StandardDetails = standardsData2.Value.SelectToken("description").Value<string>();
                                                            gradeUsStandardList.StandardDetails = standardsData2.Value?.SelectToken("description")?.Value<string>();
                                                            gradeUsStandardList.Topic = topic;
                                                                gradeUsStandardList.IsSchoolSpecific = false;
                                                                gradeUsStandardList.CreatedBy = createdBy;
                                                                gradeUsStandardList.CreatedOn = DateTime.UtcNow;

                                                                gradeUsStandardListData.Add(gradeUsStandardList);
                                                                GradeStandardId++;

                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                
                            }
                            else
                            {
                                foreach (var standardsData in standardsDataList)
                                {
                                    //GradeUsStandardListViewModel gradeUsStandardList = new GradeUsStandardListViewModel();
                                    if (standardsData.Value?.SelectToken("statementLabel") == null)
                                    {
                                        var id = standardsData.Value?.SelectToken("id")?.Value<string>();

                                        List<Notations> subjects = new List<Notations>();
                                        string? topic = null;
                                        foreach (var standardsData2 in standardsDataList)
                                        {
                                            GradeUsStandard gradeUsStandardList = new GradeUsStandard();
                                            if (standardsData2.Value?.SelectToken("statementLabel") != null && standardsData2.Value?.SelectToken("statementLabel")?.Value<string>() == "Standard")
                                            {
                                                foreach (var standardsData3 in standardsDataList)
                                                {
                                                    if (standardsData3.Value?.SelectToken("statementLabel") == null)
                                                    {
                                                        var id1 = standardsData3.Value?.SelectToken("id")?.Value<string>();
                                                        var ancestorIdsList1 = standardsData2.Value.SelectToken("ancestorIds")?.ToArray();
                                                        if (ancestorIdsList1 != null)
                                                        {
                                                            foreach (var ancestorIds1 in ancestorIdsList1)
                                                            {
                                                                if (id1 == ancestorIds1.ToString())
                                                                {
                                                                    topic = standardsData3.Value?.SelectToken("description")?.Value<string>();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                var ancestorIdsList = standardsData2.Value?.SelectToken("ancestorIds")?.ToArray();
                                                if (ancestorIdsList != null) { 
                                                foreach (var ancestorIds in ancestorIdsList)
                                                {
                                                    if (id == ancestorIds.ToString())
                                                    {
                                                        gradeUsStandardList.GradeStandardId = GradeStandardId;
                                                        gradeUsStandardList.TenantId = tenantId;
                                                        gradeUsStandardList.SchoolId = schoolId;
                                                        gradeUsStandardList.Subject = "English";

                                                        if (String.Compare(educationLevelsData.ToObject<string>(), "K", true) == 0 || String.Compare(educationLevelsData.ToObject<string>(), "Kindergarten", true) == 0)
                                                        {
                                                            gradeUsStandardList.GradeLevel = "Kindergarten";
                                                        }
                                                        else
                                                        {
                                                            gradeUsStandardList.GradeLevel = this.context?.GradeEquivalency.Where(x => x.EquivalencyId == educationLevelsData.ToObject<int>()).Select(y => y.GradeLevelEquivalency).FirstOrDefault();
                                                        }

                                                        gradeUsStandardList.Course = jToken!.SelectToken("data")?.SelectToken("subject")?.Value<string>();
                                                            //gradeUsStandardList.StandardRefNo = standardsData2!.Value!.SelectToken("statementNotation")!.Value<string>();
                                                            gradeUsStandardList.StandardRefNo = standardsData2.Value?.SelectToken("statementNotation")?.Value<string>()!;
                                                            gradeUsStandardList.Domain = standardsData.Value?.SelectToken("description")?.Value<string>();
                                                        gradeUsStandardList.StandardDetails = standardsData2.Value?.SelectToken("description")?.Value<string>();
                                                        gradeUsStandardList.Topic = topic;
                                                        gradeUsStandardList.IsSchoolSpecific = false;
                                                        gradeUsStandardList.CreatedBy = createdBy;
                                                        gradeUsStandardList.CreatedOn = DateTime.UtcNow;

                                                        gradeUsStandardListData.Add(gradeUsStandardList);
                                                        GradeStandardId++;
                                                    }
                                                }
                                              }
                                            
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }


                jurisdictionByIdListViewModel.gradeUsStandardList.AddRange(gradeUsStandardListData);
            this.context?.GradeUsStandard.AddRange(jurisdictionByIdListViewModel.gradeUsStandardList);
            this.context?.SaveChanges();
            }
            return jurisdictionByIdListViewModel;
        }



    }
}
