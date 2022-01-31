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
using opensis.core.Period.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.Period;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.Period.Services
{
    public class PeriodService : IPeriodService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IPeriodRepository periodRepository;
        public ICheckLoginSession tokenManager;
        public PeriodService(IPeriodRepository periodRepository, ICheckLoginSession checkLoginSession)
        {
            this.periodRepository = periodRepository;
            this.tokenManager = checkLoginSession;
        }

        //Required for Unit Testing
        public PeriodService() { }

        /// <summary>
        /// Add Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel AddBlock(BlockAddViewModel blockAddViewModel)
        {
            BlockAddViewModel blockAdd = new BlockAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockAddViewModel._tenantName + blockAddViewModel._userName, blockAddViewModel._token))
                {
                    blockAdd = this.periodRepository.AddBlock(blockAddViewModel);
                }
                else
                {
                    blockAdd._failure = true;
                    blockAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockAdd._failure = true;
                blockAdd._message = es.Message;
            }
            return blockAdd;
        }

        /// <summary>
        /// Update Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel UpdateBlock(BlockAddViewModel blockAddViewModel)
        {
            BlockAddViewModel blockUpdate = new BlockAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockAddViewModel._tenantName + blockAddViewModel._userName, blockAddViewModel._token))
                {
                    blockUpdate = this.periodRepository.UpdateBlock(blockAddViewModel);
                }
                else
                {
                    blockUpdate._failure = true;
                    blockUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockUpdate._failure = true;
                blockUpdate._message = es.Message;
            }
            return blockUpdate;
        }

        /// <summary>
        /// Delete Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel DeleteBlock(BlockAddViewModel blockAddViewModel)
        {
            BlockAddViewModel blockDelete = new BlockAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockAddViewModel._tenantName + blockAddViewModel._userName, blockAddViewModel._token))
                {
                    blockDelete = this.periodRepository.DeleteBlock(blockAddViewModel);
                }
                else
                {
                    blockDelete._failure = true;
                    blockDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockDelete._failure = true;
                blockDelete._message = es.Message;
            }
            return blockDelete;
        }

        /// <summary>
        /// Add Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel AddBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            BlockPeriodAddViewModel blockPeriodAdd = new BlockPeriodAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockPeriodAddViewModel._tenantName + blockPeriodAddViewModel._userName, blockPeriodAddViewModel._token))
                {
                    blockPeriodAdd = this.periodRepository.AddBlockPeriod(blockPeriodAddViewModel);
                }
                else
                {
                    blockPeriodAdd._failure = true;
                    blockPeriodAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockPeriodAdd._failure = true;
                blockPeriodAdd._message = es.Message;
            }
            return blockPeriodAdd;
        }

        /// <summary>
        /// Update Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel UpdateBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            BlockPeriodAddViewModel blockPeriodUpdatet = new BlockPeriodAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockPeriodAddViewModel._tenantName + blockPeriodAddViewModel._userName, blockPeriodAddViewModel._token))
                {
                    blockPeriodUpdatet = this.periodRepository.UpdateBlockPeriod(blockPeriodAddViewModel);
                }
                else
                {
                    blockPeriodUpdatet._failure = true;
                    blockPeriodUpdatet._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockPeriodUpdatet._failure = true;
                blockPeriodUpdatet._message = es.Message;
            }
            return blockPeriodUpdatet;
        }

        /// <summary>
        /// Delete Block Period
        /// </summary>
        /// <param name="blockPeriodAddViewModel"></param>
        /// <returns></returns>
        public BlockPeriodAddViewModel DeleteBlockPeriod(BlockPeriodAddViewModel blockPeriodAddViewModel)
        {
            BlockPeriodAddViewModel blockPeriodDelete = new BlockPeriodAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockPeriodAddViewModel._tenantName + blockPeriodAddViewModel._userName, blockPeriodAddViewModel._token))
                {
                    blockPeriodDelete = this.periodRepository.DeleteBlockPeriod(blockPeriodAddViewModel);
                }
                else
                {
                    blockPeriodDelete._failure = true;
                    blockPeriodDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockPeriodDelete._failure = true;
                blockPeriodDelete._message = es.Message;
            }
            return blockPeriodDelete;
        }

        /// <summary>
        /// Get All Block List
        /// </summary>
        /// <param name="blockListViewModel"></param>
        /// <returns></returns>
        public BlockListViewModel GetAllBlockList(BlockListViewModel blockListViewModel)
        {
            BlockListViewModel blockList = new BlockListViewModel();
            try
            {
                if (tokenManager.CheckToken(blockListViewModel._tenantName + blockListViewModel._userName, blockListViewModel._token))
                {
                    blockList = this.periodRepository.GetAllBlockList(blockListViewModel);
                }
                else
                {
                    blockList._failure = true;
                    blockList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockList._failure = true;
                blockList._message = es.Message;
            }
            return blockList;
        }

        /// <summary>
        /// Update Block Period Sort Order
        /// </summary>
        /// <param name="blockPeriodSortOrderViewModel"></param>
        /// <returns></returns>
        public BlockPeriodSortOrderViewModel UpdateBlockPeriodSortOrder(BlockPeriodSortOrderViewModel blockPeriodSortOrderViewModel)
        {
            BlockPeriodSortOrderViewModel blockPeriodSortOrderUpdate = new BlockPeriodSortOrderViewModel();
            try
            {
                if (tokenManager.CheckToken(blockPeriodSortOrderViewModel._tenantName + blockPeriodSortOrderViewModel._userName, blockPeriodSortOrderViewModel._token))
                {
                    blockPeriodSortOrderUpdate = this.periodRepository.UpdateBlockPeriodSortOrder(blockPeriodSortOrderViewModel);
                }
                else
                {
                    blockPeriodSortOrderUpdate._failure = true;
                    blockPeriodSortOrderUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockPeriodSortOrderUpdate._failure = true;
                blockPeriodSortOrderUpdate._message = es.Message;
            }
            return blockPeriodSortOrderUpdate;
        }

        /// <summary>
        /// Update Half Day Full Day Minutes For Block
        /// </summary>
        /// <param name="blockAddViewModel"></param>
        /// <returns></returns>
        public BlockAddViewModel UpdateHalfDayFullDayMinutesForBlock(BlockAddViewModel blockAddViewModel)
        {
            BlockAddViewModel blockUpdate = new BlockAddViewModel();
            try
            {
                if (tokenManager.CheckToken(blockAddViewModel._tenantName + blockAddViewModel._userName, blockAddViewModel._token))
                {
                    blockUpdate = this.periodRepository.UpdateHalfDayFullDayMinutesForBlock(blockAddViewModel);
                }
                else
                {
                    blockUpdate._failure = true;
                    blockUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                blockUpdate._failure = true;
                blockUpdate._message = es.Message;
            }
            return blockUpdate;
        }
    }
}
