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

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using opensis.core.StudentHistoricalGrade.Interfaces;
using opensis.data.Models;
using opensis.data.ViewModels.StudentHistoricalGrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/HistoricalMarkingPeriod")]
    [ApiController]
    public class StudentHistoricalGradeController : ControllerBase
    {
        private IStudentHistoricalGradeService _studentHistoricalGradeService;
        public StudentHistoricalGradeController(IStudentHistoricalGradeService studentHistoricalGradeService)
        {
            _studentHistoricalGradeService = studentHistoricalGradeService;
        }

        [HttpPost("addHistoricalMarkingPeriod")]
        public ActionResult<HistoricalMarkingPeriodAddViewModel> AddHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodAdd = new HistoricalMarkingPeriodAddViewModel();
            try
            {
                if (historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId > 0)
                {
                    historicalMarkingPeriodAdd = _studentHistoricalGradeService.AddHistoricalMarkingPeriod(historicalMarkingPeriod);
                }
                else
                {
                    historicalMarkingPeriodAdd._token = historicalMarkingPeriod._token;
                    historicalMarkingPeriodAdd._tenantName = historicalMarkingPeriod._tenantName;
                    historicalMarkingPeriodAdd._failure = true;
                    historicalMarkingPeriodAdd._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodAdd._failure = true;
                historicalMarkingPeriodAdd._message = es.Message;
            }
            return historicalMarkingPeriodAdd;
        }
        
        [HttpPut("updateHistoricalMarkingPeriod")]

        public ActionResult<HistoricalMarkingPeriodAddViewModel> UpdateHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodUpdate = new HistoricalMarkingPeriodAddViewModel();
            try
            {
                if (historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId > 0)
                {
                    historicalMarkingPeriodUpdate = _studentHistoricalGradeService.UpdateHistoricalMarkingPeriod(historicalMarkingPeriod);
                }
                else
                {
                    historicalMarkingPeriodUpdate._token = historicalMarkingPeriod._token;
                    historicalMarkingPeriodUpdate._tenantName = historicalMarkingPeriod._tenantName;
                    historicalMarkingPeriodUpdate._failure = true;
                    historicalMarkingPeriodUpdate._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodUpdate._failure = true;
                historicalMarkingPeriodUpdate._message = es.Message;
            }
            return historicalMarkingPeriodUpdate;
        }

        [HttpPost("getAllhistoricalMarkingPeriodList")]

        public ActionResult<HistoricalMarkingPeriodListModel> GetAllHistoricalMarkingPeriodList(PageResult pageResult)
        {
            HistoricalMarkingPeriodListModel historicalMarkingPeriodList = new HistoricalMarkingPeriodListModel();
            try
            {
                if (pageResult.SchoolId > 0)
                {
                    historicalMarkingPeriodList = _studentHistoricalGradeService.GetAllHistoricalMarkingPeriodList(pageResult);
                }
                else
                {
                    historicalMarkingPeriodList._token = pageResult._token;
                    historicalMarkingPeriodList._tenantName = pageResult._tenantName;
                    historicalMarkingPeriodList._failure = true;
                    historicalMarkingPeriodList._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodList._message = es.Message;
                historicalMarkingPeriodList._failure = true;
            }
            return historicalMarkingPeriodList;
        }
        [HttpPost("deleteHistoricalMarkingPeriod")]

        public ActionResult<HistoricalMarkingPeriodAddViewModel> DeleteHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod)
        {
            HistoricalMarkingPeriodAddViewModel historicalMarkingPeriodDelete = new HistoricalMarkingPeriodAddViewModel();
            try
            {
                if (historicalMarkingPeriod.HistoricalMarkingPeriod.SchoolId > 0)
                {
                    historicalMarkingPeriodDelete = _studentHistoricalGradeService.DeleteHistoricalMarkingPeriod(historicalMarkingPeriod);
                }
                else
                {
                    historicalMarkingPeriodDelete._token = historicalMarkingPeriod._token;
                    historicalMarkingPeriodDelete._tenantName = historicalMarkingPeriod._tenantName;
                    historicalMarkingPeriodDelete._failure = true;
                    historicalMarkingPeriodDelete._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalMarkingPeriodDelete._failure = true;
                historicalMarkingPeriodDelete._message = es.Message;
            }
            return historicalMarkingPeriodDelete;
        }


        [HttpPost("addUpdateHistoricalGrade")]
        public ActionResult<HistoricalGradeAddViewModel> AddUpdateHistoricalGrade(HistoricalGradeAddViewModel historicalGradeList)
        {
            HistoricalGradeAddViewModel historicalGradeAdd = new HistoricalGradeAddViewModel();
            try
            {
                if (historicalGradeList.SchoolId > 0)
                {
                    historicalGradeAdd = _studentHistoricalGradeService.AddUpdateHistoricalGrade(historicalGradeList);
                }
                else
                {
                    historicalGradeAdd._token = historicalGradeList._token;
                    historicalGradeAdd._tenantName = historicalGradeList._tenantName;
                    historicalGradeAdd._failure = true;
                    historicalGradeAdd._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalGradeAdd._failure = true;
                historicalGradeAdd._message = es.Message;
            }
            return historicalGradeAdd;
        }

        [HttpPost("getAllHistoricalGradeList")]
        public ActionResult<HistoricalGradeAddViewModel> GetAllHistoricalGradeList(HistoricalGradeAddViewModel historicalGradeList)
        {
            HistoricalGradeAddViewModel historicalList = new HistoricalGradeAddViewModel();
            try
            {
                if (historicalGradeList.SchoolId > 0)
                {
                    historicalList = _studentHistoricalGradeService.GetAllHistoricalGradeList(historicalGradeList);
                }
                else
                {
                    historicalList._token = historicalGradeList._token;
                    historicalList._tenantName = historicalGradeList._tenantName;
                    historicalList._failure = true;
                    historicalList._message = "Please enter valid school id";
                }
            }
            catch (Exception es)
            {
                historicalList._failure = true;
                historicalList._message = es.Message;
            }
            return historicalList;
        }
    }
}
