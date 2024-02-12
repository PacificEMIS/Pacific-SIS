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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using opensis.core.ParentPortal.Interface;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.School;
using System;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/ParentPortal")]
    [ApiController]
    public class ParentPortalController : ControllerBase
    {
        private IParentPortalService _parentPortalService;
        public ParentPortalController(IParentPortalService parentPortalService)
        {
            _parentPortalService = parentPortalService;
        }
        [HttpPost("getStudentListForParent")]
        public ActionResult<ParentInfoAddViewModel> GetStudentListForParent(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            ParentInfoAddViewModel parentInfoAdd = new ParentInfoAddViewModel();
            try
            {
                parentInfoAdd = _parentPortalService.GetStudentListForParent(parentInfoAddViewModel);
            }
            catch (Exception ex)
            {
                parentInfoAdd._message = ex.Message;
                parentInfoAdd._failure = true;
            }
            return parentInfoAdd;
        }
        [HttpPost("getAllSchoolsByStudentId")]

        public ActionResult<SchoolListModel> GetAllSchoolsByStudentId(SchoolListModel school)
        {
            SchoolListModel schoolListModel = new SchoolListModel();
            try
            {
                schoolListModel = _parentPortalService.GetAllSchoolsByStudentId(school);
            }
            catch (Exception ex)
            {
                schoolListModel._message = ex.Message;
                schoolListModel._failure = true;
            }
            return schoolListModel;
        }
    }
}
