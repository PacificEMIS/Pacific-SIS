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
using opensis.data.ViewModels.Period;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class PeriodRepository: IPeriodRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public PeriodRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel AddBlock(BlockAddViewModel blockAddViewModel)
        {
            if (blockAddViewModel.block is null)
            {
                return blockAddViewModel;
            }
            try
            {
                blockAddViewModel.block.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, blockAddViewModel.block.TenantId, blockAddViewModel.block.SchoolId);

                var blockTitle = this.context?.Block.AsEnumerable().Where(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.AcademicYear == blockAddViewModel.block.AcademicYear && String.Compare(x.BlockTitle, blockAddViewModel.block.BlockTitle, true) == 0).FirstOrDefault();

                if (blockTitle == null)
                {
                    int? BlockId = 1;

                    var blockData = this.context?.Block.Where(x => x.SchoolId == blockAddViewModel.block.SchoolId && x.TenantId == blockAddViewModel.block.TenantId).OrderByDescending(x => x.BlockId).FirstOrDefault();

                    if (blockData != null)
                    {
                        BlockId = blockData.BlockId + 1;
                    }

                    if (blockAddViewModel.block != null)
                    {
                        blockAddViewModel.block.BlockId = (int)BlockId;
                        blockAddViewModel.block.CreatedOn = DateTime.UtcNow;
                        this.context?.Block.Add(blockAddViewModel.block);
                        this.context?.SaveChanges();
                        blockAddViewModel._failure = false;
                        blockAddViewModel._message = "Block added successfully";
                    }
                }
                else
                {
                    blockAddViewModel._failure = true;
                    blockAddViewModel._message = "Block Title already exists";
                }
            }
            catch (Exception es)
            {
                blockAddViewModel._failure = true;
                blockAddViewModel._message = es.Message;
            }
            return blockAddViewModel;
        }

        /// <summary>
        /// Update Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel UpdateBlock(BlockAddViewModel blockAddViewModel)
        {
            if (blockAddViewModel.block is null)
            {
                return blockAddViewModel;
            }
            try
            {
                var blockUpdate = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockId == blockAddViewModel.block.BlockId);

                if (blockUpdate != null)
                {
                    var blockTitle = this.context?.Block.AsEnumerable().Where(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && String.Compare(x.BlockTitle, blockAddViewModel.block.BlockTitle, true) == 0 && x.BlockId != blockAddViewModel.block.BlockId && x.AcademicYear == blockUpdate.AcademicYear).FirstOrDefault();

                    if (blockTitle == null)
                    {
                        if (blockAddViewModel.block != null && blockUpdate != null)
                        {
                            blockAddViewModel.block.AcademicYear = blockUpdate.AcademicYear;
                            blockAddViewModel.block.CreatedBy = blockUpdate.CreatedBy;
                            blockAddViewModel.block.CreatedOn = blockUpdate.CreatedOn;
                            blockAddViewModel.block.UpdatedOn = DateTime.Now;
                            this.context?.Entry(blockUpdate).CurrentValues.SetValues(blockAddViewModel.block);
                            this.context?.SaveChanges();
                            blockAddViewModel._failure = false;
                            blockAddViewModel._message = "Block Updated Successfully";
                        }
                    }
                    else
                    {
                        blockAddViewModel._failure = true;
                        blockAddViewModel._message = "Block Title already exists";
                    }
                }
                else
                {
                    blockAddViewModel._failure = true;
                    blockAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                blockAddViewModel._failure = true;
                blockAddViewModel._message = es.Message;
            }
            return blockAddViewModel;
        }

        /// <summary>
        /// Delete Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel DeleteBlock(BlockAddViewModel blockAddViewModel)
        {
            if (blockAddViewModel.block is null)
            {
                return blockAddViewModel;
            }
            try
            {
                var blockDelete = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockId == blockAddViewModel.block.BlockId);

                if (blockDelete != null)
                {
                    var blockPeriodExits = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockDelete.TenantId && x.SchoolId == blockDelete.SchoolId && x.BlockId == blockDelete.BlockId);

                    var bellScheduleExits = this.context?.BellSchedule.FirstOrDefault(x => x.TenantId == blockDelete.TenantId && x.SchoolId == blockDelete.SchoolId && x.BlockId == blockDelete.BlockId);

                    if (blockPeriodExits != null || bellScheduleExits != null)
                    {
                        blockAddViewModel._failure = true;
                        blockAddViewModel._message = "Cannot delete because it has association.";
                    }
                    else
                    {
                        this.context?.Block.Remove(blockDelete);
                        this.context?.SaveChanges();
                        blockAddViewModel._failure = false;
                        blockAddViewModel._message = "Block deleted successfullyy";
                    }
                }
                else
                {
                    blockAddViewModel._failure = true;
                    blockAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                blockAddViewModel._failure = true;
                blockAddViewModel._message = es.Message;
            }
            return blockAddViewModel;
        }

        /// <summary>
        /// Add Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel AddBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            if (blockPeriodAddViewModel.blockPeriod is null)
            {
                return blockPeriodAddViewModel;
            }
            try
            {
                blockPeriodAddViewModel.blockPeriod.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, blockPeriodAddViewModel.blockPeriod.TenantId, blockPeriodAddViewModel.blockPeriod.SchoolId);

                var periodTitle = this.context?.BlockPeriod.AsEnumerable().Where(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && String.Compare( x.PeriodTitle, blockPeriodAddViewModel.blockPeriod.PeriodTitle,true)==0 && x.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId && x.AcademicYear== blockPeriodAddViewModel.blockPeriod.AcademicYear).FirstOrDefault();

                if (periodTitle == null)
                {
                    int? PeriodId = 1;
                    int? SortOrder = 1;
                    decimal totalMinutes = 0;

                    var blockPeriodData = this.context?.BlockPeriod.Where(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId).OrderByDescending(x => x.PeriodId).FirstOrDefault();

                    if (blockPeriodData != null)
                    {
                        PeriodId = blockPeriodData.PeriodId + 1;
                    }

                    var sortOrderData = this.context?.BlockPeriod.Where(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId).OrderByDescending(x => x.PeriodSortOrder).FirstOrDefault();

                    if (sortOrderData != null)
                    {
                        SortOrder = sortOrderData.PeriodSortOrder + 1;
                    }

                    if(blockPeriodAddViewModel.blockPeriod != null)
                    {
                        blockPeriodAddViewModel.blockPeriod.PeriodId = (int)PeriodId;
                        blockPeriodAddViewModel.blockPeriod.PeriodSortOrder = SortOrder;
                        blockPeriodAddViewModel.blockPeriod.CreatedOn = DateTime.UtcNow;
                        this.context?.BlockPeriod.Add(blockPeriodAddViewModel.blockPeriod);
                        //context!.Entry(blockPeriodAddViewModel.blockPeriod.SchoolMaster).State = EntityState.Unchanged;
                        this.context?.SaveChanges();
                    }
                    

                    var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod!.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId && c.CalculateAttendance==true).ToList(); 

                    if (blockPeriodList != null && blockPeriodList.Any())
                    {
                        foreach (var blockPeriod in blockPeriodList)
                        {
                            if (blockPeriod != null)
                            {
                                TimeSpan span = Convert.ToDateTime(blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                if (span.TotalMinutes >= 0)
                                {
                                    totalMinutes += Convert.ToDecimal(span.TotalMinutes);
                                }
                                else
                                {
                                    totalMinutes += Convert.ToDecimal((24 * 60) + span.TotalMinutes);
                                }

                            } 
                        }
                        totalMinutes = Math.Round(totalMinutes);
                        decimal halfDayMinutes = totalMinutes / 2;
                        halfDayMinutes= Math.Round(halfDayMinutes);

                        var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod!.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId);
                        
                        if (blockData != null)
                        {
                            blockData.FullDayMinutes = Convert.ToInt32( totalMinutes);
                            blockData.HalfDayMinutes = Convert.ToInt32(halfDayMinutes);
                            blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod?.CreatedBy;
                            blockData.UpdatedOn = DateTime.UtcNow;
                            this.context?.SaveChanges();
                        }
                    }
                    //this.context?.SaveChanges();
                    blockPeriodAddViewModel._failure = false;
                    blockPeriodAddViewModel._message = "Period added successfully";
                }
                else
                {
                    blockPeriodAddViewModel._failure = true;
                    blockPeriodAddViewModel._message = "Period Title already exists";
                }
            }
            catch (Exception es)
            {
                blockPeriodAddViewModel._failure = true;
                blockPeriodAddViewModel._message = es.Message;
            }
            return blockPeriodAddViewModel;
        }

        /// <summary>
        /// Update Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel UpdateBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            if (blockPeriodAddViewModel.blockPeriod is null)
            {
                return blockPeriodAddViewModel;
            }
            try
            {
                var blockPeriodUpdate = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId);

                if (blockPeriodUpdate != null)
                {
                    decimal totalMinutes = 0;
                    var periodTitle = this.context?.BlockPeriod.AsEnumerable().Where(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId && x.PeriodId != blockPeriodAddViewModel.blockPeriod.PeriodId && String.Compare(x.PeriodTitle, blockPeriodAddViewModel.blockPeriod.PeriodTitle, true) == 0 && x.AcademicYear == blockPeriodUpdate.AcademicYear).FirstOrDefault();

                    if (periodTitle == null)
                    {
                        if(blockPeriodAddViewModel.blockPeriod != null)
                        {
                            blockPeriodAddViewModel.blockPeriod.AcademicYear = blockPeriodUpdate.AcademicYear;
                            blockPeriodAddViewModel.blockPeriod.CreatedBy = blockPeriodUpdate.CreatedBy;
                            blockPeriodAddViewModel.blockPeriod.CreatedOn = blockPeriodUpdate.CreatedOn;
                            blockPeriodAddViewModel.blockPeriod.PeriodSortOrder = blockPeriodUpdate.PeriodSortOrder;
                            blockPeriodAddViewModel.blockPeriod.UpdatedOn = DateTime.Now;
                            this.context?.Entry(blockPeriodUpdate).CurrentValues.SetValues(blockPeriodAddViewModel.blockPeriod);
                            this.context?.SaveChanges();
                        }

                        var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod!.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodUpdate.BlockId && c.CalculateAttendance == true).ToList();

                        if (blockPeriodList != null && blockPeriodList.Any())
                        {
                            foreach (var blockPeriod in blockPeriodList)
                            {
                                if (blockPeriod != null)
                                {
                                    TimeSpan span = Convert.ToDateTime(blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                    //totalMinutes += Convert.ToDecimal(span.TotalMinutes);
                                    if (span.TotalMinutes >= 0)
                                    {
                                        totalMinutes += Convert.ToDecimal(span.TotalMinutes);
                                    }
                                    else
                                    {
                                        totalMinutes += Convert.ToDecimal((24 * 60) + span.TotalMinutes);
                                    }
                                }
                            }
                            totalMinutes = Math.Round(totalMinutes);
                            decimal halfDayMinutes = totalMinutes / 2;
                            halfDayMinutes = Math.Round(halfDayMinutes);

                            var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod!.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodUpdate.BlockId);

                            if (blockData != null)
                            {
                                blockData.FullDayMinutes =Convert.ToInt32(totalMinutes);
                                blockData.HalfDayMinutes = Convert.ToInt32(halfDayMinutes);
                                blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod?.CreatedBy;
                                blockData.UpdatedOn = DateTime.UtcNow;
                                this.context?.SaveChanges();
                            }
                        }                                               

                        blockPeriodAddViewModel._failure = false;
                        blockPeriodAddViewModel._message = "Period Updated Successfully";                        
                    }
                    else
                    {
                        blockPeriodAddViewModel._failure = true;
                        blockPeriodAddViewModel._message = "Period Title already exists";
                    }
                }
                else
                {
                    blockPeriodAddViewModel._failure = true;
                    blockPeriodAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                blockPeriodAddViewModel._failure = true;
                blockPeriodAddViewModel._message = es.Message;
            }
            return blockPeriodAddViewModel;
        }

        /// <summary>
        /// Delete Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel DeleteBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            if (blockPeriodAddViewModel.blockPeriod is null)
            {
                return blockPeriodAddViewModel;
            }
            try
            {
                //var blockPeriodDelete = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId);

                var courseSectionData = this.context?.AllCourseSectionView.FirstOrDefault(b => b.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && b.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && (b.CalPeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId || b.VarPeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId || b.FixedPeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId) && b.IsActive == true && b.DurationEndDate.Value.Date >= DateTime.Today.Date);

                if (courseSectionData != null)
                {
                    blockPeriodAddViewModel._failure = true;
                    blockPeriodAddViewModel._message = "Period Can Not Be Deleted. Because It Has Association";
                }
                else
                {
                    var blockPeriodDelete = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId);

                    if (blockPeriodDelete != null)
                    {
                        this.context?.BlockPeriod.Remove(blockPeriodDelete);
                        this.context?.SaveChanges();

                        var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodDelete.BlockId);

                        decimal totalMinutes = 0;
                        var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodDelete.BlockId && c.CalculateAttendance == true).ToList();

                        if (blockPeriodList != null && blockPeriodList.Any())
                        {
                            foreach (var blockPeriod in blockPeriodList)
                            {
                                if (blockPeriod != null)
                                {
                                    TimeSpan span = Convert.ToDateTime(blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                    //totalMinutes += Convert.ToInt32(span.TotalMinutes);
                                    if (span.TotalMinutes >= 0)
                                    {
                                        totalMinutes += Convert.ToDecimal(span.TotalMinutes);
                                    }
                                    else
                                    {
                                        totalMinutes += Convert.ToDecimal((24 * 60) + span.TotalMinutes);
                                    }
                                }
                            }
                            totalMinutes = Math.Round(totalMinutes);
                            decimal halfDayMinutes = totalMinutes / 2;
                            halfDayMinutes = Math.Round(halfDayMinutes);

                            //var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodDelete.BlockId);

                            if (blockData != null)
                            {
                                blockData.FullDayMinutes = Convert.ToInt32(totalMinutes);
                                blockData.HalfDayMinutes = Convert.ToInt32(halfDayMinutes);
                                blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod?.UpdatedBy;
                                blockData.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            if (blockData != null)
                            {
                                blockData.FullDayMinutes = null;
                                blockData.HalfDayMinutes = null;
                                blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod?.UpdatedBy;
                                blockData.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                        //this.context?.BlockPeriod.Remove(blockPeriodDelete);
                        this.context?.SaveChanges();
                        blockPeriodAddViewModel._failure = false;
                        blockPeriodAddViewModel._message = "Period deleted successfullyy";
                    }
                    else
                    {
                        blockPeriodAddViewModel._failure = true;
                        blockPeriodAddViewModel._message = NORECORDFOUND;
                    }                    
                }
            }
            catch (Exception es)
            {
                blockPeriodAddViewModel._failure = true;
                blockPeriodAddViewModel._message = es.Message;
            }
            return blockPeriodAddViewModel;
        }

        /// <summary>
        /// Get All Block List
        /// </summary>
        /// <param name="blockListViewModel"></param>
        /// <returns></returns>
        public BlockListViewModel GetAllBlockList(BlockListViewModel blockListViewModel)
        {
            BlockListViewModel blockListModel = new BlockListViewModel();
            try
            {
                var blockDataList = this.context?.Block.Where(x => x.TenantId == blockListViewModel.TenantId && x.SchoolId == blockListViewModel.SchoolId && x.AcademicYear == blockListViewModel.AcademicYear).OrderBy(x => x.BlockSortOrder).ToList();

                if (blockDataList != null && blockDataList.Any())
                {
                    foreach (var block in blockDataList)
                    {
                        var blockList = new GetBlockListForView()
                        {
                            TenantId = block.TenantId,
                            SchoolId = block.SchoolId,
                            BlockId = block.BlockId,
                            BlockTitle = block.BlockTitle,
                            BlockSortOrder = block.BlockSortOrder,
                            HalfDayMinutes = block.HalfDayMinutes,
                            FullDayMinutes = block.FullDayMinutes,
                            CreatedBy = block.CreatedBy,
                            CreatedOn = block.CreatedOn,
                            UpdatedBy = block.UpdatedBy,
                            UpdatedOn = block.UpdatedOn
                        };
                        var blockPeriodDataList = this.context?.BlockPeriod.Where(x => x.TenantId == block.TenantId && x.SchoolId == block.SchoolId && x.BlockId == block.BlockId && x.AcademicYear == blockListViewModel.AcademicYear).OrderBy(x => x.PeriodSortOrder).ToList();

                        if (blockPeriodDataList != null && blockPeriodDataList.Any())
                        {
                            if (blockListViewModel.IsListView == true)
                            {
                                blockPeriodDataList.ForEach(c =>
                                {
                                    c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, block.TenantId, c.CreatedBy);
                                    c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, block.TenantId, c.UpdatedBy);
                                });
                            }
                            blockList.BlockPeriod = blockPeriodDataList;
                        }
                        //block.BlockPeriod = block.BlockPeriod.OrderBy(x => x.PeriodSortOrder).ToList();
                        blockListModel.getBlockListForView.Add(blockList);
                    }
                    blockListModel._failure = false;
                }
                else
                {
                    blockListModel._message = NORECORDFOUND;
                    blockListModel._failure = true;
                }

                blockListModel.TenantId = blockListViewModel.TenantId;
                blockListModel.SchoolId = blockListViewModel.SchoolId;
                blockListModel._tenantName = blockListViewModel._tenantName;
                blockListModel._token = blockListViewModel._token;

            }
            catch (Exception es)
            {
                blockListModel.getBlockListForView = null!;
                blockListModel._message = es.Message;
                blockListModel._failure = true;
                blockListModel._tenantName = blockListViewModel._tenantName;
                blockListModel._token = blockListViewModel._token;
            }
            return blockListModel;
        }

        /// <summary>
        /// Update Block Period Sort Order
        /// </summary>
        /// <param name="blockPeriodSortOrderViewModel"></param>
        /// <returns></returns>
        public BlockPeriodSortOrderViewModel UpdateBlockPeriodSortOrder(BlockPeriodSortOrderViewModel blockPeriodSortOrderViewModel)
        {
            try
            {
                var blockPeriodRecords = new List<BlockPeriod>();

                var targetBlockPeriod = this.context?.BlockPeriod.FirstOrDefault(x => x.PeriodSortOrder == blockPeriodSortOrderViewModel.PreviousSortOrder && x.SchoolId == blockPeriodSortOrderViewModel.SchoolId && x.TenantId == blockPeriodSortOrderViewModel.TenantId && x.BlockId == blockPeriodSortOrderViewModel.BlockId);
                
                if(targetBlockPeriod != null)
                {
                    targetBlockPeriod.PeriodSortOrder = blockPeriodSortOrderViewModel.CurrentSortOrder;
                    targetBlockPeriod.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy;
                    targetBlockPeriod.UpdatedOn = DateTime.UtcNow;

                    if (blockPeriodSortOrderViewModel.PreviousSortOrder > blockPeriodSortOrderViewModel.CurrentSortOrder)
                    {
                        blockPeriodRecords = this.context?.BlockPeriod.Where(x => x.PeriodSortOrder >= blockPeriodSortOrderViewModel.CurrentSortOrder && x.PeriodSortOrder < blockPeriodSortOrderViewModel.PreviousSortOrder && x.TenantId == blockPeriodSortOrderViewModel.TenantId && x.SchoolId == blockPeriodSortOrderViewModel.SchoolId && x.BlockId == blockPeriodSortOrderViewModel.BlockId).ToList();

                        if (blockPeriodRecords != null && blockPeriodRecords.Any())
                        {
                            blockPeriodRecords.ForEach(x => { x.PeriodSortOrder = x.PeriodSortOrder + 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy; });
                        }
                    }
                    if (blockPeriodSortOrderViewModel.CurrentSortOrder > blockPeriodSortOrderViewModel.PreviousSortOrder)
                    {
                        blockPeriodRecords = this.context?.BlockPeriod.Where(x => x.PeriodSortOrder <= blockPeriodSortOrderViewModel.CurrentSortOrder && x.PeriodSortOrder > blockPeriodSortOrderViewModel.PreviousSortOrder && x.SchoolId == blockPeriodSortOrderViewModel.SchoolId && x.TenantId == blockPeriodSortOrderViewModel.TenantId && x.BlockId == blockPeriodSortOrderViewModel.BlockId).ToList();
                        if (blockPeriodRecords != null && blockPeriodRecords.Any())
                        {
                            blockPeriodRecords.ForEach(x => { x.PeriodSortOrder = x.PeriodSortOrder - 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy; });
                        }
                    }
                    this.context?.SaveChanges();
                    blockPeriodSortOrderViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                blockPeriodSortOrderViewModel._message = es.Message;
                blockPeriodSortOrderViewModel._failure = true;
            }
            return blockPeriodSortOrderViewModel;
        }

        /// <summary>
        /// Update Half Day Full Day Minutes For Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel UpdateHalfDayFullDayMinutesForBlock(BlockAddViewModel blockAddViewModel)
        {
            if (blockAddViewModel.block is null)
            {
                return blockAddViewModel;
            }
            try
            {
                var blockData = this.context?.Block.Where(e => e.TenantId == blockAddViewModel.block.TenantId && e.SchoolId == blockAddViewModel.block.SchoolId && e.BlockId== blockAddViewModel.block.BlockId).FirstOrDefault();

                if (blockData != null)
                {
                    blockData.HalfDayMinutes = blockAddViewModel.block.HalfDayMinutes;
                    blockData.FullDayMinutes = blockAddViewModel.block.FullDayMinutes;
                    blockData.UpdatedBy = blockAddViewModel.block.UpdatedBy;
                    blockData.UpdatedOn = DateTime.UtcNow;
                    this.context?.SaveChanges();
                    blockAddViewModel._failure = false;
                    blockAddViewModel._message = "Full Day and Half Day Minutes Updated Succesfully";
                }
                else
                {
                    blockAddViewModel._failure = true;
                    blockAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                blockAddViewModel._failure = true;
                blockAddViewModel._message = es.Message;
            }
            return blockAddViewModel;
        }
    }
}
