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
        private CRMContext context;
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
            try
            {
                var blockTitle = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockTitle.ToLower() == blockAddViewModel.block.BlockTitle.ToLower());

                if (blockTitle == null)
                {
                    int? BlockId = 1;

                    var blockData = this.context?.Block.Where(x => x.SchoolId == blockAddViewModel.block.SchoolId && x.TenantId == blockAddViewModel.block.TenantId).OrderByDescending(x => x.BlockId).FirstOrDefault();

                    if (blockData != null)
                    {
                        BlockId = blockData.BlockId + 1;
                    }

                    blockAddViewModel.block.BlockId = (int)BlockId;
                    blockAddViewModel.block.CreatedOn = DateTime.UtcNow;
                    this.context?.Block.Add(blockAddViewModel.block);
                    this.context?.SaveChanges();
                    blockAddViewModel._failure = false;
                    blockAddViewModel._message = "Block Added Successfully";
                }
                else
                {
                    blockAddViewModel._failure = true;
                    blockAddViewModel._message = "Block Title Already Exists";
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
            try
            {
                var blockUpdate = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockId == blockAddViewModel.block.BlockId);

                if (blockUpdate != null)
                {
                    var blockTitle = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockTitle.ToLower() == blockAddViewModel.block.BlockTitle.ToLower() && x.BlockId != blockAddViewModel.block.BlockId);

                    if (blockTitle == null)
                    {
                        blockAddViewModel.block.CreatedBy = blockUpdate.CreatedBy;
                        blockAddViewModel.block.CreatedOn = blockUpdate.CreatedOn;
                        blockAddViewModel.block.UpdatedOn = DateTime.Now;
                        this.context.Entry(blockUpdate).CurrentValues.SetValues(blockAddViewModel.block);
                        this.context?.SaveChanges();
                        blockAddViewModel._failure = false;
                        blockAddViewModel._message = "Block Updated Successfully";
                    }
                    else
                    {
                        blockAddViewModel._failure = true;
                        blockAddViewModel._message = "Block Title Already Exists";
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
            try
            {
                var blockDelete = this.context?.Block.FirstOrDefault(x => x.TenantId == blockAddViewModel.block.TenantId && x.SchoolId == blockAddViewModel.block.SchoolId && x.BlockId == blockAddViewModel.block.BlockId);

                if (blockDelete != null)
                {
                    var blockPeriodExits = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockDelete.TenantId && x.SchoolId == blockDelete.SchoolId && x.BlockId == blockDelete.BlockId);
                    if (blockPeriodExits != null)
                    {
                        blockAddViewModel._failure = true;
                        blockAddViewModel._message = "Cannot delete because it has association.";
                    }
                    else
                    {
                        this.context?.Block.Remove(blockDelete);
                        this.context?.SaveChanges();
                        blockAddViewModel._failure = false;
                        blockAddViewModel._message = "Block Deleted Successfully";
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
            try
            {
                var periodTitle = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodTitle.ToLower() == blockPeriodAddViewModel.blockPeriod.PeriodTitle.ToLower() && x.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId);

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
                    blockPeriodAddViewModel.blockPeriod.PeriodId = (int)PeriodId;
                    blockPeriodAddViewModel.blockPeriod.PeriodSortOrder = (int)SortOrder;
                    blockPeriodAddViewModel.blockPeriod.CreatedOn = DateTime.UtcNow;
                    this.context?.BlockPeriod.Add(blockPeriodAddViewModel.blockPeriod);
                    this.context?.SaveChanges();

                    var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId).ToList(); 

                    if (blockPeriodList.Count>0)
                    {
                        foreach (var blockPeriod in blockPeriodList)
                        {
                            if (blockPeriod !=null)
                            {
                                TimeSpan span = Convert.ToDateTime( blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                totalMinutes += Convert.ToDecimal(span.TotalMinutes);                                
                            } 
                        }
                        totalMinutes = Math.Round(totalMinutes);
                        decimal halfDayMinutes = totalMinutes / 2;
                        halfDayMinutes= Math.Round(halfDayMinutes);

                        var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId);
                        
                        if (blockData != null)
                        {
                            blockData.FullDayMinutes = Convert.ToInt32( totalMinutes);
                            blockData.HalfDayMinutes = Convert.ToInt32(halfDayMinutes);
                            blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod.CreatedBy;
                            blockData.UpdatedOn = DateTime.UtcNow;
                            this.context?.SaveChanges();
                        }
                    }
                    //this.context?.SaveChanges();
                    blockPeriodAddViewModel._failure = false;
                    blockPeriodAddViewModel._message = "Period Added Successfully";
                }
                else
                {
                    blockPeriodAddViewModel._failure = true;
                    blockPeriodAddViewModel._message = "Period Title Already Exists";
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
            try
            {
                var blockPeriodUpdate = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId);

                if (blockPeriodUpdate != null)
                {
                    decimal totalMinutes = 0;
                    var periodTitle = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.BlockId == blockPeriodAddViewModel.blockPeriod.BlockId && x.PeriodId != blockPeriodAddViewModel.blockPeriod.PeriodId && x.PeriodTitle.ToLower() == blockPeriodAddViewModel.blockPeriod.PeriodTitle.ToLower());

                    if (periodTitle == null)
                    {
                        var startTime = blockPeriodUpdate.PeriodStartTime;
                        var endTime = blockPeriodUpdate.PeriodEndTime;

                        blockPeriodAddViewModel.blockPeriod.CreatedBy = blockPeriodUpdate.CreatedBy;
                        blockPeriodAddViewModel.blockPeriod.CreatedOn = blockPeriodUpdate.CreatedOn;
                        blockPeriodAddViewModel.blockPeriod.PeriodSortOrder = blockPeriodUpdate.PeriodSortOrder;
                        blockPeriodAddViewModel.blockPeriod.UpdatedOn = DateTime.Now;
                        this.context.Entry(blockPeriodUpdate).CurrentValues.SetValues(blockPeriodAddViewModel.blockPeriod);
                        this.context?.SaveChanges();

                        if (startTime != blockPeriodAddViewModel.blockPeriod.PeriodStartTime || endTime != blockPeriodAddViewModel.blockPeriod.PeriodEndTime)
                        {
                            var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodUpdate.BlockId).ToList();

                            if (blockPeriodList.Count > 0)
                            {
                                foreach (var blockPeriod in blockPeriodList)
                                {
                                    if (blockPeriod != null)
                                    {
                                        TimeSpan span = Convert.ToDateTime(blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                        totalMinutes += Convert.ToDecimal(span.TotalMinutes);
                                    }
                                }
                                totalMinutes = Math.Round(totalMinutes);
                                decimal halfDayMinutes = totalMinutes / 2;
                                halfDayMinutes = Math.Round(halfDayMinutes);

                                var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodUpdate.BlockId);

                                if (blockData != null)
                                {
                                    blockData.FullDayMinutes =Convert.ToInt32(totalMinutes);
                                    blockData.HalfDayMinutes = Convert.ToInt32(halfDayMinutes);
                                    blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod.CreatedBy;
                                    blockData.UpdatedOn = DateTime.UtcNow;
                                    this.context?.SaveChanges();
                                }
                            }
                        }                       

                        blockPeriodAddViewModel._failure = false;
                        blockPeriodAddViewModel._message = "Period Updated Successfully";                        
                    }
                    else
                    {
                        blockPeriodAddViewModel._failure = true;
                        blockPeriodAddViewModel._message = "Period Title Already Exists";
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
            try
            {
                var blockPeriodDelete = this.context?.BlockPeriod.FirstOrDefault(x => x.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && x.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && x.PeriodId == blockPeriodAddViewModel.blockPeriod.PeriodId);

                if (blockPeriodDelete != null)
                {
                    this.context?.BlockPeriod.Remove(blockPeriodDelete);
                    this.context?.SaveChanges();
                    
                    decimal totalMinutes = 0;
                    var blockPeriodList = this.context?.BlockPeriod.Where(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodDelete.BlockId).ToList();

                    if (blockPeriodList.Count > 0)
                    {
                        foreach (var blockPeriod in blockPeriodList)
                        {
                            if (blockPeriod != null)
                            {
                                TimeSpan span = Convert.ToDateTime(blockPeriod.PeriodEndTime).Subtract(Convert.ToDateTime(blockPeriod.PeriodStartTime));
                                totalMinutes += Convert.ToInt32(span.TotalMinutes);
                            }
                        }
                        totalMinutes = Math.Round(totalMinutes);
                        decimal halfDayMinutes = totalMinutes / 2;
                        halfDayMinutes = Math.Round(halfDayMinutes);

                        var blockData = this.context?.Block.FirstOrDefault(c => c.TenantId == blockPeriodAddViewModel.blockPeriod.TenantId && c.SchoolId == blockPeriodAddViewModel.blockPeriod.SchoolId && c.BlockId == blockPeriodDelete.BlockId);

                        if (blockData != null)
                        {
                            blockData.FullDayMinutes =Convert.ToInt32(totalMinutes);
                            blockData.HalfDayMinutes =Convert.ToInt32(halfDayMinutes);
                            blockData.UpdatedBy = blockPeriodAddViewModel.blockPeriod.CreatedBy;
                            blockData.UpdatedOn = DateTime.UtcNow;
                            this.context?.SaveChanges();
                        }
                    }
                    //this.context?.BlockPeriod.Remove(blockPeriodDelete);
                    //this.context?.SaveChanges();


                    blockPeriodAddViewModel._failure = false;
                    blockPeriodAddViewModel._message = "Period Deleted Successfully";
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
        /// Get All Block List
        /// </summary>
        /// <param name="blockListViewModel"></param>
        /// <returns></returns>
        public BlockListViewModel GetAllBlockList(BlockListViewModel blockListViewModel)
        {
            BlockListViewModel blockListModel = new BlockListViewModel();
            try
            {
                var blockDataList = this.context?.Block.Where(x => x.TenantId == blockListViewModel.TenantId && x.SchoolId == blockListViewModel.SchoolId).OrderBy(x => x.BlockSortOrder).ToList();
                
                if (blockDataList.Count > 0)
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
                            HalfDayMinutes=block.HalfDayMinutes,
                            FullDayMinutes=block.FullDayMinutes,
                            CreatedBy = (block.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == blockListViewModel.TenantId && u.EmailAddress == block.CreatedBy).Name : null,
                            CreatedOn = block.CreatedOn,
                            UpdatedBy = (block.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == blockListViewModel.TenantId && u.EmailAddress == block.UpdatedBy).Name : null,
                            UpdatedOn = block.UpdatedOn
                        };              
                        var blockPeriodDataList = this.context?.BlockPeriod.Where(x => x.TenantId == block.TenantId && x.SchoolId == block.SchoolId && x.BlockId==block.BlockId).OrderBy(x => x.PeriodSortOrder).Select(e=> new BlockPeriod()
                        { 
                            TenantId=e.TenantId,
                            SchoolId=e.SchoolId,
                            BlockId=e.BlockId,
                            PeriodId=e.PeriodId,
                            PeriodTitle=e.PeriodTitle,
                            PeriodShortName=e.PeriodShortName,
                            PeriodStartTime=e.PeriodStartTime,
                            PeriodEndTime=e.PeriodEndTime,
                            PeriodSortOrder=e.PeriodSortOrder,
                            CalculateAttendance=e.CalculateAttendance,
                            CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == blockListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                            CreatedOn=e.CreatedOn,
                            UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == blockListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                        }).ToList();
                        
                        if(blockPeriodDataList.Count>0)
                        {
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
                blockListModel.getBlockListForView = null;
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
                
                targetBlockPeriod.PeriodSortOrder = blockPeriodSortOrderViewModel.CurrentSortOrder;
                targetBlockPeriod.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy;
                targetBlockPeriod.UpdatedOn = DateTime.UtcNow;

                if (blockPeriodSortOrderViewModel.PreviousSortOrder > blockPeriodSortOrderViewModel.CurrentSortOrder)
                {
                    blockPeriodRecords = this.context?.BlockPeriod.Where(x => x.PeriodSortOrder >= blockPeriodSortOrderViewModel.CurrentSortOrder && x.PeriodSortOrder < blockPeriodSortOrderViewModel.PreviousSortOrder && x.TenantId == blockPeriodSortOrderViewModel.TenantId && x.SchoolId == blockPeriodSortOrderViewModel.SchoolId && x.BlockId == blockPeriodSortOrderViewModel.BlockId).ToList();

                    if (blockPeriodRecords.Count > 0)
                    {
                        blockPeriodRecords.ForEach(x => { x.PeriodSortOrder = x.PeriodSortOrder + 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy; });
                    }
                }
                if (blockPeriodSortOrderViewModel.CurrentSortOrder > blockPeriodSortOrderViewModel.PreviousSortOrder)
                {
                    blockPeriodRecords = this.context?.BlockPeriod.Where(x => x.PeriodSortOrder <= blockPeriodSortOrderViewModel.CurrentSortOrder && x.PeriodSortOrder > blockPeriodSortOrderViewModel.PreviousSortOrder && x.SchoolId == blockPeriodSortOrderViewModel.SchoolId && x.TenantId == blockPeriodSortOrderViewModel.TenantId && x.BlockId == blockPeriodSortOrderViewModel.BlockId).ToList();
                    if (blockPeriodRecords.Count > 0)
                    {
                        blockPeriodRecords.ForEach(x => { x.PeriodSortOrder = x.PeriodSortOrder - 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = blockPeriodSortOrderViewModel.UpdatedBy; });
                    }
                }
                this.context?.SaveChanges();
                blockPeriodSortOrderViewModel._failure = false;
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
                throw;
            }
            return blockAddViewModel;
        }
    }
}
