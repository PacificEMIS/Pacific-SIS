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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using opensis.core.ApiAccess.Interfaces;
using opensis.data.ViewModels.ApiAccess;
using System;
using System.Net;

namespace opensisAPI.Controllers
{
    [ApiController]
    [EnableCors("AllowOrigin")]
    [Authorize(Policy = "ApiKeyPolicy")]
    //[Route("ApiAccess")]

    public class ApiAccessController : ControllerBase
    {
        //private CRMContext context;
        private IApiAccessService _apiAccessService;
        public ApiAccessController(IApiAccessService apiAccessService)
        {

            _apiAccessService = apiAccessService;
        }


        [HttpPost("api/school/getAllSchool")]
        public ApiAccessSchoolListViewModel GetAllSchoolList()
        {

            ApiAccessSchoolListViewModel schoolList = new ApiAccessSchoolListViewModel();
            try
            {
                schoolList = _apiAccessService.GetAllSchoolList();
            }
            catch (Exception es)
            {
                schoolList._message = es.Message;
                schoolList._failure = true;
            }
            return schoolList;
        }

        [HttpPost("api/getSchoolDetails/academicYear/{academicYear}")]
        public ApiAccessSchoolViewModel GetSchoolDetails(decimal academicYear)
        {
            ApiAccessSchoolViewModel schoolDetails = new();
            try
            {
                schoolDetails = _apiAccessService.GetSchoolDetails(academicYear);
            }
            catch (Exception es)
            {
                schoolDetails._message = es.Message;
                schoolDetails._failure = true;
            }
            return schoolDetails;
        }

        [HttpPost("api/getAllStaff/academicYear/{academicYear}")]
        public ApiAccessStaffListViewModel GetAllStaffList(decimal? academicYear)
        {

            ApiAccessStaffListViewModel staffList = new ApiAccessStaffListViewModel();
            try
            {
                staffList = _apiAccessService.GetAllStaffList();
            }
            catch (Exception es)
            {
                staffList._message = es.Message;
                staffList._failure = true;
            }
            return staffList;
        }

        [HttpPost("api/getAllStudent/academicYear/{academicYear}")]
        public ApiAccessStudentListViewModel GetAllStudentList(decimal? academicYear)
        {
            ApiAccessStudentListViewModel studentListModel = new ApiAccessStudentListViewModel();
            try
            {
                studentListModel = this._apiAccessService.GetAllStudentList(academicYear);
            }
            catch (Exception es)
            {
                studentListModel._failure = true;
                studentListModel._message = es.Message;
            }
            return studentListModel;
        }
    }
}
