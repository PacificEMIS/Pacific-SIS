
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

using opensis.core.helper.Interfaces;
using opensis.core.StudentHistoricalGrade.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.StudentHistoricalGrade;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StudentHistoricalGrade.Services
{
    public class StudentHistoricalGradeService : IStudentHistoricalGradeService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStudentHistoricalGradeRepository historicalGradeRepository;
        public ICheckLoginSession tokenManager;
        public StudentHistoricalGradeService(IStudentHistoricalGradeRepository historicalGradeRepository, ICheckLoginSession checkLoginSession)
        {
            this.historicalGradeRepository = historicalGradeRepository;
            this.tokenManager = checkLoginSession;
        }
        public StudentHistoricalGradeService() { }

        /// <summary>
        /// Add HistoricalMarkingPeriod
        /// </summary>
        /// <param name="historicalMarkingPeriod"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodAddViewModel AddHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodAddViewModel = new HistoricalMarkingPeriodAddViewModel();
            if (tokenManager.CheckToken(historicalMarkingPeriod._tenantName + historicalMarkingPeriod._userName, historicalMarkingPeriod._token))
            {
                historicalMarkingPeriodAddViewModel = this.historicalGradeRepository.AddHistoricalMarkingPeriod(historicalMarkingPeriod);
                return historicalMarkingPeriodAddViewModel;
            }
            else
            {
                historicalMarkingPeriodAddViewModel._failure = true;
                historicalMarkingPeriodAddViewModel._message = TOKENINVALID;
                return historicalMarkingPeriodAddViewModel;
            }

        }

        /// <summary>
        /// Update Historical MarkingPeriod
        /// </summary>
        /// <param name="historicalMarkingPeriod"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodAddViewModel UpdateHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodAddViewModel = new HistoricalMarkingPeriodAddViewModel();
            if (tokenManager.CheckToken(historicalMarkingPeriod._tenantName + historicalMarkingPeriod._userName, historicalMarkingPeriod._token))
            {
                historicalMarkingPeriodAddViewModel = this.historicalGradeRepository.UpdateHistoricalMarkingPeriod(historicalMarkingPeriod);
                return historicalMarkingPeriodAddViewModel;
            }
            else
            {
                historicalMarkingPeriodAddViewModel._failure = true;
                historicalMarkingPeriodAddViewModel._message = TOKENINVALID;
                return historicalMarkingPeriodAddViewModel;
            }

        }

        /// <summary>
        /// Get All Historical MarkingPeriod List with pagination
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public HistoricalMarkingPeriodListModel GetAllHistoricalMarkingPeriodList(PageResult pageResult)
        {
            HistoricalMarkingPeriodListModel historicalMarkingPeriodListModel = new HistoricalMarkingPeriodListModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    historicalMarkingPeriodListModel = this.historicalGradeRepository.GetAllHistoricalMarkingPeriodList(pageResult);
                }
                else
                {
                    historicalMarkingPeriodListModel._failure = true;
                    historicalMarkingPeriodListModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodListModel._failure = true;
                historicalMarkingPeriodListModel._message = es.Message;
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
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodAddViewModel = new HistoricalMarkingPeriodAddViewModel();
            try
            {
                if (tokenManager.CheckToken(historicalMarkingPeriod._tenantName + historicalMarkingPeriod._userName, historicalMarkingPeriod._token))
                {
                    historicalMarkingPeriodAddViewModel = this.historicalGradeRepository.DeleteHistoricalMarkingPeriod(historicalMarkingPeriod);
                }
                else
                {
                    historicalMarkingPeriodAddViewModel._failure = true;
                    historicalMarkingPeriodAddViewModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodAddViewModel._failure = true;
                historicalMarkingPeriodAddViewModel._message = es.Message;
            }
            return historicalMarkingPeriodAddViewModel;
        }


        /// <summary>
        /// Add Update Historical Grade
        /// </summary>
        /// <param name="historicalGradeList"></param>
        /// <returns></returns>
        public HistoricalGradeAddViewModel AddUpdateHistoricalGrade(HistoricalGradeAddViewModel historicalGradeList)
        {
            HistoricalGradeAddViewModel historicalGradeAddViewModel = new HistoricalGradeAddViewModel();
            if (tokenManager.CheckToken(historicalGradeList._tenantName + historicalGradeList._userName, historicalGradeList._token))
            {
                historicalGradeAddViewModel = this.historicalGradeRepository.AddUpdateHistoricalGrade(historicalGradeList);
                return historicalGradeAddViewModel;
            }
            else
            {
                historicalGradeAddViewModel._failure = true;
                historicalGradeAddViewModel._message = TOKENINVALID;
                return historicalGradeAddViewModel;
            }

        }

        /// <summary>
        /// Get All Historical Grade List
        /// </summary>
        /// <param name="historicalGradeList"></param>
        /// <returns></returns>
        public HistoricalGradeAddViewModel GetAllHistoricalGradeList(HistoricalGradeAddViewModel historicalGradeList)
        {
            HistoricalGradeAddViewModel historicalGradeAddViewModel = new HistoricalGradeAddViewModel();
            if (tokenManager.CheckToken(historicalGradeList._tenantName + historicalGradeList._userName, historicalGradeList._token))
            {
                historicalGradeAddViewModel = this.historicalGradeRepository.GetAllHistoricalGradeList(historicalGradeList);
                return historicalGradeAddViewModel;
            }
            else
            {
                historicalGradeAddViewModel._failure = true;
                historicalGradeAddViewModel._message = TOKENINVALID;
                return historicalGradeAddViewModel;
            }

        }
    }
}

