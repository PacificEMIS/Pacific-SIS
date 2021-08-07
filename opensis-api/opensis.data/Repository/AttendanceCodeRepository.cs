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
using opensis.data.ViewModels.AttendanceCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class AttendanceCodeRepository : IAttendanceCodeRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public AttendanceCodeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
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

            var AttendanceCodeData = this.context?.AttendanceCode.Where(x => x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId).OrderByDescending(x => x.AttendanceCode1).FirstOrDefault();

            if (AttendanceCodeData != null)
            {
                AttendanceCodeId = AttendanceCodeData.AttendanceCode1 + 1;
            }

            var attendanceCodeSortOrder = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId).OrderByDescending(x => x.SortOrder).FirstOrDefault();

            if (attendanceCodeSortOrder != null)
            {
                SortOrder = attendanceCodeSortOrder.SortOrder + 1;
            }


            var checkDefaultAttendanceCode = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId).ToList().Find(x => x.DefaultCode == true);

            if (checkDefaultAttendanceCode == null)
            {
                attendanceCodeAddViewModel.attendanceCode.DefaultCode = true;
            }
            if (attendanceCodeAddViewModel.attendanceCode.DefaultCode == true)
            {
                (from p in this.context?.AttendanceCode
                 where p.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && p.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && p.AttendanceCategoryId == attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId
                 select p).ToList().ForEach(x => x.DefaultCode = false);
            }


            attendanceCodeAddViewModel.attendanceCode.AttendanceCode1 = (int)AttendanceCodeId;
            attendanceCodeAddViewModel.attendanceCode.CreatedOn = DateTime.UtcNow;
            attendanceCodeAddViewModel.attendanceCode.SortOrder = (int)SortOrder;
            this.context?.AttendanceCode.Add(attendanceCodeAddViewModel.attendanceCode);
            
            this.context?.SaveChanges();
            attendanceCodeAddViewModel._failure = false;
            attendanceCodeAddViewModel._message = "Attendance Code Added Successfully";

            return attendanceCodeAddViewModel;
        }
        /// <summary>
        /// Get Attendance Code By Id
        /// </summary>
        /// <param name="attendanceCodeAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeAddViewModel ViewAttendanceCode(AttendanceCodeAddViewModel attendanceCodeAddViewModel)
        {
            AttendanceCodeAddViewModel attendanceCodeViewModel = new AttendanceCodeAddViewModel();
            try
            {                
                var attendanceCodeView = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.attendanceCode.AttendanceCode1);
                if (attendanceCodeView != null)
                {
                    attendanceCodeViewModel.attendanceCode = attendanceCodeView;
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
            AttendanceCodeAddViewModel attendanceCodeUpdateModel = new AttendanceCodeAddViewModel();
            try
            {
                var attendanceCodeUpdate = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.attendanceCode.AttendanceCode1);

                var checkDefaultAttendanceCode = this.context?.AttendanceCode.Where(x => x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.AttendanceCategoryId == attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId&&x.AttendanceCode1!= attendanceCodeAddViewModel.attendanceCode.AttendanceCode1).ToList().Find(x => x.DefaultCode == true);

                if (checkDefaultAttendanceCode == null)
                {
                    attendanceCodeAddViewModel.attendanceCode.DefaultCode = true;
                }
                if (attendanceCodeAddViewModel.attendanceCode.DefaultCode == true)
                {
                    (from p in this.context?.AttendanceCode
                     where p.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && p.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && p.AttendanceCategoryId == attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId
                     select p).ToList().ForEach(x => x.DefaultCode = false);
                }

                attendanceCodeAddViewModel.attendanceCode.UpdatedOn = DateTime.Now;
                attendanceCodeAddViewModel.attendanceCode.CreatedOn = attendanceCodeUpdate.CreatedOn;
                attendanceCodeAddViewModel.attendanceCode.CreatedBy = attendanceCodeUpdate.CreatedBy;
                attendanceCodeAddViewModel.attendanceCode.AttendanceCategoryId = attendanceCodeUpdate.AttendanceCategoryId;
                attendanceCodeAddViewModel.attendanceCode.SortOrder = attendanceCodeUpdate.SortOrder;
                this.context.Entry(attendanceCodeUpdate).CurrentValues.SetValues(attendanceCodeAddViewModel.attendanceCode);
                this.context?.SaveChanges();
                attendanceCodeAddViewModel._failure = false;
                attendanceCodeAddViewModel._message = "Attendance Code Updated Successfully";
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
            AttendanceCodeListViewModel attendanceCodeListModel = new AttendanceCodeListViewModel();
            try
            {

                var attendanceCodeList = this.context?.AttendanceCode.Where(x => x.TenantId == attendanceCodeListViewModel.TenantId && x.SchoolId == attendanceCodeListViewModel.SchoolId && x.AttendanceCategoryId== attendanceCodeListViewModel.AttendanceCategoryId).OrderBy(x => x.SortOrder).Select(e=> new AttendanceCode()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    AttendanceCode1=e.AttendanceCode1,
                    AcademicYear=e.AcademicYear,
                    AllowEntryBy=e.AllowEntryBy,
                    AttendanceCategoryId=e.AttendanceCategoryId,
                    DefaultCode=e.DefaultCode,
                    ShortName=e.ShortName,
                    SortOrder=e.SortOrder,
                    StateCode=e.StateCode,
                    Title=e.Title,
                    Type=e.Type,
                    CreatedOn=e.CreatedOn,
                    UpdatedOn=e.UpdatedOn,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>u.TenantId == attendanceCodeListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>u.TenantId == attendanceCodeListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                }).ToList();

                attendanceCodeListModel.attendanceCodeList = attendanceCodeList;
                attendanceCodeListModel._tenantName = attendanceCodeListViewModel._tenantName;
                attendanceCodeListModel._token = attendanceCodeListViewModel._token;

                if (attendanceCodeList.Count > 0)
                {
                    attendanceCodeListModel._failure = false;
                }
                else
                {
                    attendanceCodeListModel._failure = true;
                    attendanceCodeListModel._message = NORECORDFOUND;
                }
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
                var attendanceCodeDelete = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.AttendanceCode1 == attendanceCodeAddViewModel.attendanceCode.AttendanceCode1);

                if (attendanceCodeDelete != null)
                {
                    var studentAttendanceData = this.context?.StudentAttendance.FirstOrDefault(x => x.TenantId == attendanceCodeAddViewModel.attendanceCode.TenantId && x.SchoolId == attendanceCodeAddViewModel.attendanceCode.SchoolId && x.AttendanceCode == attendanceCodeAddViewModel.attendanceCode.AttendanceCode1);

                    if (studentAttendanceData != null)
                    {
                        attendanceCodeAddViewModel._failure = true;
                        attendanceCodeAddViewModel._message = "Cannot Delete Because It Has Association.";
                    }
                    else
                    {
                        this.context?.AttendanceCode.Remove(attendanceCodeDelete);
                        this.context?.SaveChanges();
                        attendanceCodeAddViewModel._failure = false;
                        attendanceCodeAddViewModel._message = "Attendance Code Deleted Successfully";
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

            var AttendanceCodeCategoriesData = this.context?.AttendanceCodeCategories.Where(x => x.TenantId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.SchoolId).OrderByDescending(x => x.AttendanceCategoryId).FirstOrDefault();

            if (AttendanceCodeCategoriesData != null)
            {
                AttendanceCodeCategoryId = AttendanceCodeCategoriesData.AttendanceCategoryId + 1;
            }

            attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.AttendanceCategoryId = (int)AttendanceCodeCategoryId;
            attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.CreatedOn = DateTime.UtcNow;
            this.context?.AttendanceCodeCategories.Add(attendanceCodeCategoriesAddViewModel.attendanceCodeCategories);
            this.context?.SaveChanges();
            attendanceCodeCategoriesAddViewModel._failure = false;
            attendanceCodeCategoriesAddViewModel._message = "Attendance Category Added Successfully";

            return attendanceCodeCategoriesAddViewModel;
        }
        /// <summary>
        /// Get Attendance Code Categories By Id
        /// </summary>
        /// <param name="attendanceCodeCategoriesAddViewModel"></param>
        /// <returns></returns>
        public AttendanceCodeCategoriesAddViewModel ViewAttendanceCodeCategories(AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesAddViewModel)
        {
            AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesViewModel = new AttendanceCodeCategoriesAddViewModel();
            try
            {
                var attendanceCodeCategoriesView = this.context?.AttendanceCodeCategories.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.AttendanceCategoryId);
                if (attendanceCodeCategoriesView != null)
                {
                    attendanceCodeCategoriesViewModel.attendanceCodeCategories = attendanceCodeCategoriesView;
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
            AttendanceCodeCategoriesAddViewModel attendanceCodeCategoriesUpdateModel = new AttendanceCodeCategoriesAddViewModel();
            try
            {
                var attendanceCodeCategoriesUpdate = this.context?.AttendanceCodeCategories.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.AttendanceCategoryId);
                
                attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.UpdatedOn = DateTime.Now;
                attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.CreatedOn = attendanceCodeCategoriesUpdate.CreatedOn;
                attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.CreatedBy = attendanceCodeCategoriesUpdate.CreatedBy;
                this.context.Entry(attendanceCodeCategoriesUpdate).CurrentValues.SetValues(attendanceCodeCategoriesAddViewModel.attendanceCodeCategories);
                this.context?.SaveChanges();
                attendanceCodeCategoriesAddViewModel._failure = false;
                attendanceCodeCategoriesAddViewModel._message = "Attendance Category Updated Successfully";
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
            AttendanceCodeCategoriesListViewModel attendanceCodeCategoriesListModel = new AttendanceCodeCategoriesListViewModel();
            try
            {

                var attendanceCodeCategoriesList = this.context?.AttendanceCodeCategories.Where(x => x.TenantId == attendanceCodeCategoriesListViewModel.TenantId && x.SchoolId == attendanceCodeCategoriesListViewModel.SchoolId).Select(r=> new AttendanceCodeCategories()
                { 
                    TenantId=r.TenantId,
                    SchoolId= r.SchoolId,
                    AcademicYear= r.AcademicYear,
                    AttendanceCategoryId=r.AttendanceCategoryId,
                    Title=r.Title,
                    CreatedOn=r.CreatedOn,
                    UpdatedOn=r.UpdatedOn,
                    CreatedBy = (r.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == attendanceCodeCategoriesListViewModel.TenantId && u.EmailAddress == r.CreatedBy).Name : null,
                    UpdatedBy = (r.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == attendanceCodeCategoriesListViewModel.TenantId && u.EmailAddress == r.UpdatedBy).Name : null
                }).ToList();

                attendanceCodeCategoriesListModel.attendanceCodeCategoriesList = attendanceCodeCategoriesList;
                attendanceCodeCategoriesListModel._tenantName = attendanceCodeCategoriesListViewModel._tenantName;
                attendanceCodeCategoriesListModel._token = attendanceCodeCategoriesListViewModel._token;

                if (attendanceCodeCategoriesList.Count > 0)
                {
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
                var attendanceCodeCategoriesDelete = this.context?.AttendanceCodeCategories.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.TenantId && x.SchoolId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesAddViewModel.attendanceCodeCategories.AttendanceCategoryId);
                var AttendanceCodeExist = this.context?.AttendanceCode.FirstOrDefault(x => x.TenantId == attendanceCodeCategoriesDelete.TenantId && x.SchoolId == attendanceCodeCategoriesDelete.SchoolId && x.AttendanceCategoryId == attendanceCodeCategoriesDelete.AttendanceCategoryId);
                if (AttendanceCodeExist != null)
                {
                    attendanceCodeCategoriesAddViewModel.attendanceCodeCategories = null;
                    attendanceCodeCategoriesAddViewModel._message = "Attendance Category cannot be deleted because it has its association";
                    attendanceCodeCategoriesAddViewModel._failure = true;
                }
                else
                {
                    this.context?.AttendanceCodeCategories.Remove(attendanceCodeCategoriesDelete);
                    this.context?.SaveChanges();
                    attendanceCodeCategoriesAddViewModel._failure = false;
                    attendanceCodeCategoriesAddViewModel._message = "Attendance Category Deleted Successfully";
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

                            if (AttendanceCodesSortOrderItem.Count > 0)
                            {
                                AttendanceCodesSortOrderItem.ForEach( x => {x.SortOrder = x.SortOrder + 1; x.UpdatedOn = DateTime.UtcNow;x.UpdatedBy = attendanceCodeSortOrderModel.UpdatedBy; });
                            }
                        }
                        if (attendanceCodeSortOrderModel.CurrentSortOrder > attendanceCodeSortOrderModel.PreviousSortOrder)
                        {
                            AttendanceCodesSortOrderItem = this.context?.AttendanceCode.Where(x => x.SortOrder <= attendanceCodeSortOrderModel.CurrentSortOrder && x.SortOrder > attendanceCodeSortOrderModel.PreviousSortOrder && x.SchoolId == attendanceCodeSortOrderModel.SchoolId && x.TenantId == attendanceCodeSortOrderModel.TenantId && x.AttendanceCategoryId == attendanceCodeSortOrderModel.AttendanceCategoryId).ToList();
                            if (AttendanceCodesSortOrderItem.Count > 0)
                            {
                                AttendanceCodesSortOrderItem.ForEach(x => { x.SortOrder = x.SortOrder - 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = attendanceCodeSortOrderModel.UpdatedBy; });
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
