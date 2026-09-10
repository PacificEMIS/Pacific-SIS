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

using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using opensis.core.School.Interfaces;
using opensis.data.ViewModels.SuperAdministrator;

namespace opensisAPI.Controllers
{
    /// <summary>
    /// Settings > Administration > Super Administrators. Every endpoint is
    /// restricted server-side to callers whose own login is an active
    /// Super Administrator.
    /// </summary>
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/SuperAdministrator")]
    [ApiController]
    public class SuperAdministratorController : ControllerBase
    {
        private readonly ISuperAdministratorService _service;

        public SuperAdministratorController(ISuperAdministratorService service)
        {
            _service = service;
        }

        [HttpPost("getAll")]
        public ActionResult<SuperAdministratorListViewModel> GetAll(SuperAdministratorListViewModel model)
        {
            try
            {
                return _service.GetSuperAdministrators(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorListViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("add")]
        public ActionResult<SuperAdministratorAddViewModel> Add(SuperAdministratorAddViewModel model)
        {
            try
            {
                return _service.AddSuperAdministrator(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorAddViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("setActiveStatus")]
        public ActionResult<SuperAdministratorActionViewModel> SetActiveStatus(SuperAdministratorActionViewModel model)
        {
            try
            {
                return _service.SetActiveStatus(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorActionViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("delete")]
        public ActionResult<SuperAdministratorActionViewModel> Delete(SuperAdministratorActionViewModel model)
        {
            try
            {
                return _service.DeleteSuperAdministrator(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorActionViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("getPromotionCandidates")]
        public ActionResult<SuperAdministratorCandidateListViewModel> GetPromotionCandidates(SuperAdministratorCandidateListViewModel model)
        {
            try
            {
                return _service.GetPromotionCandidates(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorCandidateListViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("promote")]
        public ActionResult<SuperAdministratorActionViewModel> Promote(SuperAdministratorActionViewModel model)
        {
            try
            {
                return _service.PromoteToSuperAdministrator(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorActionViewModel { _failure = true, _message = es.Message };
            }
        }

        [HttpPost("demote")]
        public ActionResult<SuperAdministratorActionViewModel> Demote(SuperAdministratorActionViewModel model)
        {
            try
            {
                return _service.DemoteSuperAdministrator(model);
            }
            catch (Exception es)
            {
                return new SuperAdministratorActionViewModel { _failure = true, _message = es.Message };
            }
        }
    }
}
