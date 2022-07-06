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
using opensis.data.ViewModels.StudentHistoricalGrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StudentHistoricalGradeRepository : IStudentHistoricalGradeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentHistoricalGradeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Historical MarkingPeriod
        /// </summary>
        /// <param name="historicalMarkingPeriod"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodAddViewModel AddHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            if (historicalMarkingPeriod.HistoricalMarkingPeriod is null)
            {
                return historicalMarkingPeriod;
            }
            try
            {
                int? histMarkingPeriodId = 1;

                var historicalMarkingPeriodData = this.context?.HistoricalMarkingPeriod.Where(x => x.SchoolId == historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId && x.TenantId == historicalMarkingPeriod.HistoricalMarkingPeriod.TenantId).OrderByDescending(x => x.HistMarkingPeriodId).FirstOrDefault();

                if (historicalMarkingPeriodData != null)
                {
                    histMarkingPeriodId = historicalMarkingPeriodData.HistMarkingPeriodId + 1;
                }

                historicalMarkingPeriod.HistoricalMarkingPeriod.HistMarkingPeriodId = (int)histMarkingPeriodId;
                historicalMarkingPeriod.HistoricalMarkingPeriod.CreatedOn = DateTime.UtcNow;
                this.context?.HistoricalMarkingPeriod.Add(historicalMarkingPeriod.HistoricalMarkingPeriod);
                this.context?.SaveChanges();
                historicalMarkingPeriod._failure = false;
                historicalMarkingPeriod._message = "Historical marking period added successfully";
            }
            catch (Exception ex)
            {
                historicalMarkingPeriod._failure = true;
                historicalMarkingPeriod._message = ex.Message;
            }
            return historicalMarkingPeriod;
        }

        /// <summary>
        /// Update Historical MarkingPeriod
        /// </summary>
        /// <param name="historicalMarkingPeriod"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodAddViewModel UpdateHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            if (historicalMarkingPeriod.HistoricalMarkingPeriod is null)
            {
                return historicalMarkingPeriod;
            }
            try
            {
                var historicalMarkingPeriodMaster = this.context?.HistoricalMarkingPeriod.FirstOrDefault(x => x.TenantId == historicalMarkingPeriod.HistoricalMarkingPeriod.TenantId && x.SchoolId == historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId && x.HistMarkingPeriodId == historicalMarkingPeriod.HistoricalMarkingPeriod.HistMarkingPeriodId);
                if (historicalMarkingPeriodMaster != null)
                {
                    this.context?.Entry(historicalMarkingPeriodMaster).CurrentValues.SetValues(historicalMarkingPeriod.HistoricalMarkingPeriod);
                    this.context?.SaveChanges();
                    historicalMarkingPeriod._failure = false;
                    historicalMarkingPeriod._message = "Historical marking period updated successfully";
                }
                else
                {
                    historicalMarkingPeriod.HistoricalMarkingPeriod = null;
                    historicalMarkingPeriod._failure = true;
                    historicalMarkingPeriod._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                historicalMarkingPeriod._failure = true;
                historicalMarkingPeriod._message = ex.Message;
            }
            return historicalMarkingPeriod;
        }


        /// <summary>
        /// Get All Historical MarkingPeriod List with pagination
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodListModel GetAllHistoricalMarkingPeriodList(PageResult pageResult)
        {
            HistoricalMarkingPeriodListModel historicalMarkingPeriodListModel = new HistoricalMarkingPeriodListModel();
            IQueryable<HistoricalMarkingPeriod>? transactionIQ = null;

            try
            {
                var historicalMarkingPeriodList = this.context?.HistoricalMarkingPeriod.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId).OrderBy(x => x.GradePostDate);

                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {
                    //string sortField = "SchoolName"; string sortOrder = "desc";

                    transactionIQ = historicalMarkingPeriodList;
                }
                else
                {
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        //transactionIQ = historicalMarkingPeriodList.Where(x => x.Title.ToLower().Contains(Columnvalue.ToLower()) || x.AcademicYear.ToString().Contains(Columnvalue.ToLower()) || (x.GradePostDate.ToString() == Columnvalue));
                        transactionIQ = historicalMarkingPeriodList?.Where(x =>
 String.Compare(x.Title, Columnvalue, true) == 0 || x.AcademicYear.ToString()!.Contains(Columnvalue.ToLower()) || (x.GradePostDate.ToString() == Columnvalue));
                    }
                }
                transactionIQ = transactionIQ?.OrderBy(s => s.GradePostDate);
                int totalCount = transactionIQ != null ? transactionIQ.Count() : 0;

                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }

                historicalMarkingPeriodListModel.HistoricalMarkingPeriodList = transactionIQ != null ? transactionIQ.ToList() : new();

                if (historicalMarkingPeriodListModel.HistoricalMarkingPeriodList.Count > 0)
                {
                    historicalMarkingPeriodListModel.HistoricalMarkingPeriodList.ForEach(c =>
                     {
                         c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                         c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                     });
                }
                historicalMarkingPeriodListModel.TotalCount = totalCount;
                historicalMarkingPeriodListModel.PageNumber = pageResult.PageNumber;
                historicalMarkingPeriodListModel._pageSize = pageResult.PageSize;
                historicalMarkingPeriodListModel._tenantName = pageResult._tenantName;
                historicalMarkingPeriodListModel._token = pageResult._token;

                if (historicalMarkingPeriodListModel.HistoricalMarkingPeriodList.Count > 0)
                {
                    historicalMarkingPeriodListModel._failure = false;
                }
                else
                {
                    historicalMarkingPeriodListModel._failure = true;
                    historicalMarkingPeriodListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodListModel._message = es.Message;
                historicalMarkingPeriodListModel._failure = true;
                historicalMarkingPeriodListModel._tenantName = pageResult._tenantName;
                historicalMarkingPeriodListModel._token = pageResult._token;
            }
            return historicalMarkingPeriodListModel;

        }

        /// <summary>
        /// Delete HistoricalMarkingPeriod
        /// </summary>
        /// <param name="historicalMarkingPeriod"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodAddViewModel DeleteHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            if (historicalMarkingPeriod.HistoricalMarkingPeriod is null)
            {
                return historicalMarkingPeriod;
            }
            try
            {
                var historicalMarkingPeriodData = this.context?.HistoricalMarkingPeriod.FirstOrDefault(x => x.TenantId == historicalMarkingPeriod.HistoricalMarkingPeriod.TenantId && x.SchoolId == historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId && x.HistMarkingPeriodId == historicalMarkingPeriod.HistoricalMarkingPeriod.HistMarkingPeriodId);

                // if (historicalMarkingPeriod != null)
                if (historicalMarkingPeriodData != null)
                {
                    this.context?.HistoricalMarkingPeriod.Remove(historicalMarkingPeriodData);
                    this.context?.SaveChanges();
                    historicalMarkingPeriod._failure = false;
                    historicalMarkingPeriod._message = "Historical marking period deleted successfullyy";
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriod._failure = true;
                historicalMarkingPeriod._message = es.Message;
            }
            return historicalMarkingPeriod;
        }


        /// <summary>
        /// Add Update Historical Grade
        /// </summary>
        /// <param name="historicalGradeList"></param>
        /// <returns></returns>
        public HistoricalGradeAddViewModel AddUpdateHistoricalGrade(HistoricalGradeAddViewModel historicalGradeList)
        {
            try
            {
                int? gradeId = 1;
                int? creditId = 1;
                var gradeExits = this.context?.HistoricalGrade.Where(x => x.SchoolId == historicalGradeList.SchoolId && x.StudentId == historicalGradeList.StudentId && x.TenantId == historicalGradeList.TenantId).ToList();

                if (gradeExits?.Any() == true)
                {
                    var historicalGradeListData = this.context?.HistoricalCreditTransfer.Where(x => x.TenantId == historicalGradeList.TenantId && x.StudentId == historicalGradeList.StudentId && x.SchoolId == historicalGradeList.SchoolId).ToList();
                    if (historicalGradeListData?.Any() == true)
                    {
                        this.context?.HistoricalCreditTransfer.RemoveRange(historicalGradeListData);
                    }

                    var gradeListData = this.context?.HistoricalGrade.Where(x => x.TenantId == historicalGradeList.TenantId && x.StudentId == historicalGradeList.StudentId && x.SchoolId == historicalGradeList.SchoolId).ToList();
                    if (gradeListData?.Any() == true)
                    {
                        this.context?.HistoricalGrade.RemoveRange(gradeListData);
                    }

                    this.context?.SaveChanges();

                    foreach (var historicalGrade in historicalGradeList.HistoricalGradeList)
                    {
                        foreach (var creditTransfer in historicalGrade.HistoricalCreditTransfer)
                        {
                            creditTransfer.CreditTransferId = (int)creditId;
                            creditTransfer.HistGradeId = (int)gradeId;
                            creditTransfer.CreatedOn = DateTime.UtcNow;
                            creditTransfer.CreatedBy = historicalGradeList.CreatedBy;
                            creditTransfer.StudentId = (int)historicalGradeList.StudentId!;
                            creditTransfer.SchoolId = (int)historicalGradeList.SchoolId!;
                            creditTransfer.TenantId = historicalGradeList.TenantId;
                            this.context?.HistoricalCreditTransfer.Add(creditTransfer);
                            creditId++;
                        }

                        historicalGrade.HistGradeId = (int)gradeId;
                        historicalGrade.CreatedOn = DateTime.UtcNow;
                        historicalGrade.CreatedBy = historicalGradeList.CreatedBy;
                        historicalGrade.StudentId = (int)historicalGradeList.StudentId!;
                        historicalGrade.SchoolId = (int)historicalGradeList.SchoolId!;
                        historicalGrade.TenantId = historicalGradeList.TenantId;
                        this.context?.HistoricalGrade.Add(historicalGrade);
                        gradeId++;
                    }

                    historicalGradeList._failure = false;
                    historicalGradeList._message = "Historical grade updated successfully";
                }
                else
                {

                    var gradeData = this.context?.HistoricalGrade.Where(x => x.SchoolId == historicalGradeList.SchoolId && x.TenantId == historicalGradeList.TenantId).OrderByDescending(x => x.HistGradeId).FirstOrDefault();
                    if (gradeData != null)
                    {
                        gradeId = gradeData.HistGradeId + 1;
                    }

                    var creditTransferData = this.context?.HistoricalCreditTransfer.Where(x => x.SchoolId == historicalGradeList.SchoolId && x.TenantId == historicalGradeList.TenantId).OrderByDescending(x => x.CreditTransferId).FirstOrDefault();

                    if (creditTransferData != null)
                    {
                        creditId = creditTransferData.CreditTransferId + 1;
                    }
                    foreach (var historicalGrade in historicalGradeList.HistoricalGradeList)
                    {
                        foreach (var creditTransfer in historicalGrade.HistoricalCreditTransfer)
                        {
                            creditTransfer.CreditTransferId = (int)creditId;
                            creditTransfer.HistGradeId = (int)gradeId;
                            creditTransfer.CreatedOn = DateTime.UtcNow;
                            creditTransfer.CreatedBy = historicalGradeList.CreatedBy;
                            creditTransfer.StudentId = (int)historicalGradeList.StudentId!;
                            creditTransfer.SchoolId = (int)historicalGradeList.SchoolId!;
                            creditTransfer.TenantId = historicalGradeList.TenantId;
                            this.context?.HistoricalCreditTransfer.Add(creditTransfer);
                            creditId++;
                        }

                        historicalGrade.HistGradeId = (int)gradeId;
                        historicalGrade.CreatedOn = DateTime.UtcNow;
                        historicalGrade.CreatedBy = historicalGradeList.CreatedBy;
                        historicalGrade.StudentId = (int)historicalGradeList.StudentId!;
                        historicalGrade.SchoolId = (int)historicalGradeList.SchoolId!;
                        historicalGrade.TenantId = historicalGradeList.TenantId;
                        this.context?.HistoricalGrade.Add(historicalGrade);
                        gradeId++;
                    }

                    historicalGradeList._failure = false;
                    historicalGradeList._message = "Historical grade added successfully";
                }
                this.context?.SaveChanges();
            }
            catch (Exception ex)
            {
                historicalGradeList._failure = true;
                historicalGradeList._message = ex.Message;
            }
            return historicalGradeList;
        }


        /// <summary>
        /// Get All Historical Grade List
        /// </summary>
        /// <param name="historicalGradeList"></param>
        /// <returns></returns>
        public HistoricalGradeAddViewModel GetAllHistoricalGradeList(HistoricalGradeAddViewModel historicalGradeList)
        {
            HistoricalGradeAddViewModel historicalGradeListModel = new HistoricalGradeAddViewModel();
            try
            {
                if (historicalGradeList.StudentId != null && historicalGradeList.StudentId > 0)
                {
                    var historicalGradeDataList = this.context?.HistoricalGrade.Include(x => x.HistoricalCreditTransfer).Where(x => x.TenantId == historicalGradeList.TenantId && x.StudentId == historicalGradeList.StudentId && x.SchoolId == historicalGradeList.SchoolId).ToList();

                    var studentPhoto = this.context?.StudentMaster.Where(x => x.TenantId == historicalGradeList.TenantId && x.StudentId == historicalGradeList.StudentId && x.SchoolId == historicalGradeList.SchoolId).Select(x => x.StudentThumbnailPhoto).FirstOrDefault();

                    if (historicalGradeDataList?.Any() == true)
                    {
                        historicalGradeListModel.HistoricalGradeList = historicalGradeDataList;
                        historicalGradeListModel.StudentPhoto = studentPhoto;
                        historicalGradeListModel.StudentId = historicalGradeList.StudentId;
                        historicalGradeListModel._failure = false;
                    }
                    else
                    {
                        historicalGradeListModel._failure = true;
                        historicalGradeListModel._message = NORECORDFOUND;
                    }
                }
                else
                {
                    //this blok for only transcript screen

                    var historicalGrade = this.context?.HistoricalGrade.Where(x => x.TenantId == historicalGradeList.TenantId && x.SchoolId == historicalGradeList.SchoolId).Select(s => s.EquivalencyId).Distinct().ToList();

                    if (historicalGrade?.Count > 0)
                    {
                        var gradeEquivalencyData = this.context?.GradeEquivalency.Where(x => historicalGrade.Contains(x.EquivalencyId)).ToList();
                        if (gradeEquivalencyData?.Any() == true)
                        {
                            historicalGradeListModel.gradeEquivalencies = gradeEquivalencyData;
                            historicalGradeListModel._failure = false;
                        }
                    }
                    else
                    {
                        historicalGradeListModel._failure = true;
                        historicalGradeListModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                historicalGradeListModel._message = es.Message;
                historicalGradeListModel._failure = true;
            }
            return historicalGradeListModel;
        }
    }
}
