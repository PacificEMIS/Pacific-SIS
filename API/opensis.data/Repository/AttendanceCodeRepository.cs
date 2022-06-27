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
using opensis.data.ViewModels.AttendanceCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class AttendanceCodeRepository : IAttendanceCodeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public AttendanceCodeRepository(IDbContextFactory dbContextFactory)
        {
            context = dbContextFactory.Create();
        }
        /// <summary>
        /// Add Attendance Code
        /// </summary>
        /// <param name="attendanceCodeAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeAddViewModel AddAttendanceCode(AttendanceCodeAddViewModel attendanceCodeAddViewModel)
        {
            //int? AttendanceCodeId = Utility.GetMaxPK(this.context, new Func<AttendanceCode, int>(x => x.AttendanceCode1));
           
            int? AttendanceCodeId = 1;
            int? SortOrder = 1;

            var AttendanceCodeData = this.context?.AttendanceCode.Where(x => x.TenantId == attendanceCodeAddViewModel.AttendanceCode!.TenantId && x.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId).OrderByDescending(x => x.AttendanceCode1).FirstOrDefault();

            if (AttendanceCodeData != null)
            {
                AttendanceCodeId = AttendanceCodeData.AttendanceCode1 + 1;
            }

            var attendanceCodeSortOrder = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.AttendanceCode!.SchoolId && x.TenantId == attendanceCodeAddViewModel.AttendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

            if (attendanceCodeSortOrder != null)
            {
                SortOrder = attendanceCodeSortOrder.SortOrder + 1;
            }


            var checkDefaultAttendanceCode = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.AttendanceCode!.SchoolId && x.TenantId == attendanceCodeAddViewModel.AttendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId).ToList().Find(x => x.DefaultCode == true);

            if (checkDefaultAttendanceCode == null)
            {
                attendanceCodeAddViewModel.AttendanceCode!.DefaultCode = true;
            }
            if (attendanceCodeAddViewModel.AttendanceCode!.DefaultCode == true)
            {
                (from p in this.context?.AttendanceCode
                 where p.TenantId == attendanceCodeAddViewModel.AttendanceCode.TenantId && p.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && p.AttendanceCategoryId == attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId
                 select p).ToList().ForEach(x => x.DefaultCode = false);
            }

            attendanceCodeAddViewModel.AttendanceCode.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, attendanceCodeAddViewModel.AttendanceCode.TenantId, attendanceCodeAddViewModel.AttendanceCode.SchoolId);
            attendanceCodeAddViewModel.AttendanceCode.AttendanceCode1 = (int)AttendanceCodeId;
            attendanceCodeAddViewModel.AttendanceCode.CreatedOn = DateTime.UtcNow;
            attendanceCodeAddViewModel.AttendanceCode.SortOrder = SortOrder;
            this.context?.AttendanceCode.Add(attendanceCodeAddViewModel.AttendanceCode);
            
            this.context?.SaveChanges();
            attendanceCodeAddViewModel._failure = false;
            attendanceCodeAddViewModel._message = "Attendance Code added successfully";

            return attendanceCodeAddViewModel;
        }
        /// <summary>
        /// Get Attendance Code By Id
        /// </summary>
        /// <param name="attendanceCodeAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeAddViewModel ViewAttendanceCode(AttendanceCodeAddViewModel attendanceCodeAddViewModel)
        {
            AttendanceCodeAddViewModel attendanceCodeViewModel = new ();
            try
            {                
                var attendanceCodeView = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.AttendanceCode!.TenantId && x.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.AttendanceCode.AttendanceCode1);
                if (attendanceCodeView != null)
                {
                    attendanceCodeViewModel.AttendanceCode = attendanceCodeView;
                    attendanceCodeAddViewModel._failure = false;                    
                }
                else
                {
                    attendanceCodeViewModel._failure = true;
                    attendanceCodeViewModel._message = NORECORDFOUND;                    
                }
            }
            catch (Exception es)
            {

                attendanceCodeViewModel._failure = true;
                attendanceCodeViewModel._message = es.Message;
            }
            return attendanceCodeViewModel;
        }
        /// <summary>
        /// Update Attendance Code
        /// </summary>
        /// <param name="attendanceCodeAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeAddViewModel UpdateAttendanceCode(AttendanceCodeAddViewModel attendanceCodeAddViewModel)
        {
            AttendanceCodeAddViewModel attendanceCodeUpdateModel = new ();
            try
            {
                var attendanceCodeUpdate = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.AttendanceCode!.TenantId && x.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.AttendanceCode.AttendanceCode1);

                if (attendanceCodeUpdate is not null)
                {

                
                var checkDefaultAttendanceCode = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.AttendanceCode!.SchoolId && x.TenantId == attendanceCodeAddViewModel.AttendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId && x.AttendanceCode1 != attendanceCodeAddViewModel.AttendanceCode.AttendanceCode1).ToList().Find(x => x.DefaultCode == true);

                if (checkDefaultAttendanceCode == null)
                {
                    attendanceCodeAddViewModel.AttendanceCode!.DefaultCode = true;
                }
                if (attendanceCodeAddViewModel.AttendanceCode!.DefaultCode == true)
                {
                    (from p in this.context?.AttendanceCode
                     where p.TenantId == attendanceCodeAddViewModel.AttendanceCode.TenantId && p.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && p.AttendanceCategoryId == attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId
                     select p).ToList().ForEach(x => x.DefaultCode = false);
                }

                attendanceCodeAddViewModel.AttendanceCode.AcademicYear = attendanceCodeUpdate.AcademicYear;
                attendanceCodeAddViewModel.AttendanceCode.UpdatedOn = DateTime.Now;
                attendanceCodeAddViewModel.AttendanceCode.CreatedOn = attendanceCodeUpdate!.CreatedOn;
                attendanceCodeAddViewModel.AttendanceCode.CreatedBy = attendanceCodeUpdate.CreatedBy;
                attendanceCodeAddViewModel.AttendanceCode.AttendanceCategoryId = attendanceCodeUpdate.AttendanceCategoryId;
                attendanceCodeAddViewModel.AttendanceCode.SortOrder = attendanceCodeUpdate.SortOrder;
                context!.Entry(attendanceCodeUpdate).CurrentValues.SetValues(attendanceCodeAddViewModel.AttendanceCode);
                this.context?.SaveChanges();
                attendanceCodeAddViewModel._failure = false;
                attendanceCodeAddViewModel._message = "Attendance Code Updated Successfully";
            }
            }
            catch (Exception es)
            {
                attendanceCodeAddViewModel._failure = true;
                attendanceCodeAddViewModel._message = es.Message;
            }
            return attendanceCodeAddViewModel;
        }
        /// <summary>
        /// Get All Attendance Code
        /// </summary>
        /// <param name="attendanceCodeListViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeListViewModel GetAllAttendanceCode(AttendanceCodeListViewModel attendanceCodeListViewModel)
        {
            AttendanceCodeListViewModel attendanceCodeListModel = new();
            try
            {
                var attendanceCodeList = this.context?.AttendanceCode.Where(x => x.TenantId == attendanceCodeListViewModel.TenantId && x.SchoolId == attendanceCodeListViewModel.SchoolId && x.AttendanceCategoryId == attendanceCodeListViewModel.AttendanceCategoryId && x.AcademicYear == attendanceCodeListViewModel.AcademicYear).OrderBy(x => x.SortOrder).ToList();

                if (attendanceCodeList is not null && attendanceCodeList.Any())
                {
                    if (attendanceCodeListViewModel.IsListView == true)
                    {
                        attendanceCodeList.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context!, attendanceCodeListViewModel.TenantId, c.CreatedBy!);
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context!, attendanceCodeListViewModel.TenantId, c.UpdatedBy!);
                        });
                    }
                    attendanceCodeListModel.AttendanceCodeList = attendanceCodeList;
                    attendanceCodeListModel._failure = false;
                }
                else
                {
                    attendanceCodeListModel._failure = true;
                    attendanceCodeListModel._message = NORECORDFOUND;
                }

                attendanceCodeListModel._tenantName = attendanceCodeListViewModel._tenantName;
                attendanceCodeListModel._token = attendanceCodeListViewModel._token;
            }
            catch (Exception es)
            {
                attendanceCodeListModel._message = es.Message;
                attendanceCodeListModel._failure = true;
                attendanceCodeListModel._tenantName = attendanceCodeListViewModel._tenantName;
                attendanceCodeListModel._token = attendanceCodeListViewModel._token;
            }
            return attendanceCodeListModel;
        }
        /// <summary>
        /// Delete Attendance Code
        /// </summary>
        /// <param name="attendanceCodeAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeAddViewModel DeleteAttendanceCode(AttendanceCodeAddViewModel attendanceCodeAddViewModel)
        {
            try
            {
                var attendanceCodeDelete = this.context?.AttendanceCode.Include(c=>c.StudentAttendance).FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.AttendanceCode!.TenantId && x.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.AttendanceCode.AttendanceCode1);

                if (attendanceCodeDelete != null)
                {
                    var studentDailyAttendanceData = this.context?.StudentDailyAttendance.AsEnumerable().FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.AttendanceCode!.TenantId && x.SchoolId == attendanceCodeAddViewModel.AttendanceCode.SchoolId && String.Compare(x.AttendanceCode, attendanceCodeDelete.Title,true) == 0);

                   // (x.AttendanceCode??"").ToLower() == attendanceCodeDelete.Title.ToLower());
                    

                    if (attendanceCodeDelete.StudentAttendance.Count>0 || studentDailyAttendanceData != null)
                    {
                        attendanceCodeAddViewModel._failure = true;
                        attendanceCodeAddViewModel._message = "Cannot Delete Because It Has Association.";
                    }
                    else
                    {
                        this.context?.AttendanceCode.Remove(attendanceCodeDelete);
                        this.context?.SaveChanges();
                        attendanceCodeAddViewModel._failure = false;
                        attendanceCodeAddViewModel._message = "Attendance Code deleted successfullyy";
                    }
                }
                else
                {
                    attendanceCodeAddViewModel._failure = true;
                    attendanceCodeAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                attendanceCodeAddViewModel._failure = true;
                attendanceCodeAddViewModel._message = es.Message;
            }
            return attendanceCodeAddViewModel;
        }
        /// <summary>
        /// Add Attendance Code Categories
        /// </summary>
        /// <param name="attendanceCodeCategoriesAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesAddViewModel AddAttendanceCodeCategories(AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesAddViewModel)
        {
            //int? AttendanceCodeCategoryId = Utility.GetMaxPK(this.context, new Func<AttendanceCodeCategories, int>(x => x.AttendanceCategoryId));

            int? AttendanceCodeCategoryId = 1;

            var AttendanceCodeCategoriesData = this.context?.AttendanceCodeCategories.Where(x => x.TenantId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolId).OrderByDescending(x => x.AttendanceCategoryId).FirstOrDefault();

            if (AttendanceCodeCategoriesData != null)
            {
                AttendanceCodeCategoryId = AttendanceCodeCategoriesData.AttendanceCategoryId + 1;
            }
            attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.TenantId, attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolId);
            attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.AttendanceCategoryId = (int)AttendanceCodeCategoryId;
            attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.CreatedOn = DateTime.UtcNow;
            this.context?.AttendanceCodeCategories.Add(attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories);
            //context!.Entry(attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolMaster).State = EntityState.Unchanged;
            this.context?.SaveChanges();
            attendanceCodeCategoriesAddViewModel._failure = false;
            attendanceCodeCategoriesAddViewModel._message = "Attendance Category added successfully";

            return attendanceCodeCategoriesAddViewModel;
        }
        /// <summary>
        /// Get Attendance Code Categories By Id
        /// </summary>
        /// <param name="attendanceCodeCategoriesAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesAddViewModel ViewAttendanceCodeCategories(AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesAddViewModel)
        {
            AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesViewModel = new ();
            try
            {
                var attendanceCodeCategoriesView = this.context?.AttendanceCodeCategories.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.AttendanceCategoryId);
                if (attendanceCodeCategoriesView != null)
                {
                    attendanceCodeCategoriesViewModel.AttendanceCodeCategories = attendanceCodeCategoriesView;
                    attendanceCodeCategoriesViewModel._failure = false;
                }
                else
                {
                    attendanceCodeCategoriesViewModel._failure = true;
                    attendanceCodeCategoriesViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                attendanceCodeCategoriesViewModel._failure = true;
                attendanceCodeCategoriesViewModel._message = es.Message;
            }
            return attendanceCodeCategoriesViewModel;
        }
        /// <summary>
        /// Update Attendance Code Categories
        /// </summary>
        /// <param name="attendanceCodeCategoriesAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesAddViewModel UpdateAttendanceCodeCategories(AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesAddViewModel)
        {
            AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesUpdateModel = new ();
            try
            {
                var attendanceCodeCategoriesUpdate = this.context?.AttendanceCodeCategories.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.AttendanceCategoryId);
                if(attendanceCodeCategoriesUpdate != null)
                {
                    attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.AcademicYear = attendanceCodeCategoriesUpdate.AcademicYear;
                    attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.UpdatedOn = DateTime.Now;
                    attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.CreatedOn = attendanceCodeCategoriesUpdate!.CreatedOn;
                    attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.CreatedBy = attendanceCodeCategoriesUpdate.CreatedBy;
                    this.context!.Entry(attendanceCodeCategoriesUpdate).CurrentValues.SetValues(attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories);
                    this.context?.SaveChanges();
                    attendanceCodeCategoriesAddViewModel._failure = false;
                    attendanceCodeCategoriesAddViewModel._message = "Attendance Category Updated Successfully";
                }
                
            }
            catch (Exception es)
            {
                attendanceCodeCategoriesAddViewModel._failure = true;
                attendanceCodeCategoriesAddViewModel._message = es.Message;
            }
            return attendanceCodeCategoriesAddViewModel;
        }
        /// <summary>
        /// Get All Attendance Code Categories
        /// </summary>
        /// <param name="attendanceCodeCategoriesListViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesListViewModel GetAllAttendanceCodeCategories(AttendanceCodeCategoriesListViewModel attendanceCodeCategoriesListViewModel)
        {
            AttendanceCodeCategoriesListViewModel attendanceCodeCategoriesListModel = new ();
            try
            {
                var attendanceCodeCategoriesList = this.context?.AttendanceCodeCategories.Where(x => x.TenantId == attendanceCodeCategoriesListViewModel.TenantId && x.SchoolId == attendanceCodeCategoriesListViewModel.SchoolId && x.AcademicYear == attendanceCodeCategoriesListViewModel.AcademicYear).ToList();

                if (attendanceCodeCategoriesList is not null && attendanceCodeCategoriesList.Any())
                {
                    attendanceCodeCategoriesListModel.AttendanceCodeCategoriesList = attendanceCodeCategoriesList;
                    attendanceCodeCategoriesListModel._tenantName = attendanceCodeCategoriesListViewModel._tenantName;
                    attendanceCodeCategoriesListModel._token = attendanceCodeCategoriesListViewModel._token;
                    attendanceCodeCategoriesListModel._failure = false;
                }
                else
                {
                    attendanceCodeCategoriesListModel._failure = true;
                    attendanceCodeCategoriesListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                attendanceCodeCategoriesListModel._message = es.Message;
                attendanceCodeCategoriesListModel._failure = true;
                attendanceCodeCategoriesListModel._tenantName = attendanceCodeCategoriesListViewModel._tenantName;
                attendanceCodeCategoriesListModel._token = attendanceCodeCategoriesListViewModel._token;
            }
            return attendanceCodeCategoriesListModel;
        }
        /// <summary>
        /// Delete Attendance Code Categories
        /// </summary>
        /// <param name="attendanceCodeCategoriesAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesAddViewModel DeleteAttendanceCodeCategories(AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesAddViewModel)
        {
            try
            {
                var attendanceCodeCategoriesDelete = this.context?.AttendanceCodeCategories.Include(c=>c.AttendanceCode).Include(b=>b.CourseSection).FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories!.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories.AttendanceCategoryId);

                if (attendanceCodeCategoriesDelete != null)
                {
                    //var AttendanceCodeExist = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesDelete.TenantId && x.SchoolId == attendanceCodeCategoriesDelete.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesDelete.AttendanceCategoryId);
                    
                    if (attendanceCodeCategoriesDelete.AttendanceCode.Any() || attendanceCodeCategoriesDelete.CourseSection.Any())
                    {
                        attendanceCodeCategoriesAddViewModel.AttendanceCodeCategories = null;
                        attendanceCodeCategoriesAddViewModel._message = "Attendance Category cannot be deleted because it has its association";
                        attendanceCodeCategoriesAddViewModel._failure = true;
                    }
                    else
                    {
                        this.context?.AttendanceCodeCategories.Remove(attendanceCodeCategoriesDelete);
                        this.context?.SaveChanges();
                        attendanceCodeCategoriesAddViewModel._failure = false;
                        attendanceCodeCategoriesAddViewModel._message = "Attendance Category deleted successfullyy";
                    }
                }                            
            }
            catch (Exception es)
            {
                attendanceCodeCategoriesAddViewModel._failure = true;
                attendanceCodeCategoriesAddViewModel._message = es.Message;
            }
            return attendanceCodeCategoriesAddViewModel;
        }

        /// <summary>
        /// Update Attendance Code Sort Order
        /// </summary>
        /// <param name="attendanceCodeSortOrderModel"></param>
        /// <returns></returns>
        public AttendanceCodeSortOrderModel UpdateAttendanceCodeSortOrder(AttendanceCodeSortOrderModel attendanceCodeSortOrderModel)
        {
            try
            {
                if (attendanceCodeSortOrderModel.AttendanceCategoryId > 0)
                {
                    var AttendanceCodesSortOrderItem = new List<AttendanceCode>();

                    var targetAttendanceCodeSortOrderItem = this.context?.AttendanceCode.FirstOrDefault(x => x.SortOrder == attendanceCodeSortOrderModel.PreviousSortOrder && x.SchoolId == attendanceCodeSortOrderModel.SchoolId && x.TenantId == attendanceCodeSortOrderModel.TenantId && x.AttendanceCategoryId == attendanceCodeSortOrderModel.AttendanceCategoryId);

                    if (targetAttendanceCodeSortOrderItem != null)
                    {
                        targetAttendanceCodeSortOrderItem.SortOrder = attendanceCodeSortOrderModel.CurrentSortOrder;
                        targetAttendanceCodeSortOrderItem.UpdatedBy = attendanceCodeSortOrderModel.UpdatedBy;
                        targetAttendanceCodeSortOrderItem.UpdatedOn = DateTime.UtcNow;

                        if (attendanceCodeSortOrderModel.PreviousSortOrder > attendanceCodeSortOrderModel.CurrentSortOrder)
                        {
                            AttendanceCodesSortOrderItem = this.context?.AttendanceCode.Where(x => x.SortOrder >= attendanceCodeSortOrderModel.CurrentSortOrder && x.SortOrder < attendanceCodeSortOrderModel.PreviousSortOrder && x.SchoolId == attendanceCodeSortOrderModel.SchoolId && x.TenantId == attendanceCodeSortOrderModel.TenantId && x.AttendanceCategoryId == attendanceCodeSortOrderModel.AttendanceCategoryId).ToList();

                            if (AttendanceCodesSortOrderItem is not null && AttendanceCodesSortOrderItem.Any())
                            {
                                AttendanceCodesSortOrderItem.ForEach( x => {/*x.SortOrder = x.SortOrder + 1;*/
                                    x.SortOrder += 1; x.UpdatedOn = DateTime.UtcNow;x.UpdatedBy = attendanceCodeSortOrderModel.UpdatedBy; });
                            }
                        }
                        if (attendanceCodeSortOrderModel.CurrentSortOrder > attendanceCodeSortOrderModel.PreviousSortOrder)
                        {
                            AttendanceCodesSortOrderItem = this.context?.AttendanceCode.Where(x => x.SortOrder <= attendanceCodeSortOrderModel.CurrentSortOrder && x.SortOrder > attendanceCodeSortOrderModel.PreviousSortOrder && x.SchoolId == attendanceCodeSortOrderModel.SchoolId && x.TenantId == attendanceCodeSortOrderModel.TenantId && x.AttendanceCategoryId == attendanceCodeSortOrderModel.AttendanceCategoryId).ToList();
                            if (AttendanceCodesSortOrderItem !=null && AttendanceCodesSortOrderItem.Any())
                            {
                                AttendanceCodesSortOrderItem.ForEach(x => { /*x.SortOrder = x.SortOrder - 1;*/
                                    x.SortOrder -= 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = attendanceCodeSortOrderModel.UpdatedBy; });
                            }
                        }
                    }
                }
            
                this.context?.SaveChanges();
                attendanceCodeSortOrderModel._failure = false;
            }
            catch (Exception es)
            {
                attendanceCodeSortOrderModel._message = es.Message;
                attendanceCodeSortOrderModel._failure = true;
            }
            return attendanceCodeSortOrderModel;
        }
    }
}
