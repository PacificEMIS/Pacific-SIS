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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using opensis.core.ApiKey.Interfaces;
using opensis.core.Common.Interfaces;
using opensis.data.ViewModels.ApiKey;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/ApiKey")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private IApiKeyServices _apiService;
        public ApiKeyController(IApiKeyServices apiServices)
        {
            _apiService = apiServices;
        }

        [HttpPost("getAPIKey")]
        public ActionResult<GetAPIKeyListViewModel> GetAPIKey(GetAPIKeyListViewModel getAPIKeyListViewModel)
        {
            GetAPIKeyListViewModel getAPIKeyList = new GetAPIKeyListViewModel();
            try
            {
                getAPIKeyList = _apiService.GetAPIKey(getAPIKeyListViewModel);

            }
            catch (Exception es)
            {
                getAPIKeyList._failure = true;
                getAPIKeyList._message = es.Message;
            }
            return getAPIKeyList;
        }

        [HttpPost("addAPIAccess")]
        public ActionResult<AddAPIAccessViewModel> AddAPIAccess(AddAPIAccessViewModel addAPIAccessViewModel)
        {
            AddAPIAccessViewModel addAPIAccess = new AddAPIAccessViewModel();
            try
            {
                addAPIAccess = _apiService.AddAPIAccess(addAPIAccessViewModel);

            }
            catch (Exception es)
            {
                addAPIAccess._failure = true;
                addAPIAccess._message = es.Message;
            }
            return addAPIAccess;
        }

        [HttpPost("getAPIAccess")]
        public ActionResult<GetAPIAccessViewModel> GetAPIAccess(GetAPIAccessViewModel getAPIAccessViewModel)
        {
            GetAPIAccessViewModel getAPIAccess = new GetAPIAccessViewModel();
            try
            {
                getAPIAccess = _apiService.GetAPIAccess(getAPIAccessViewModel);

            }
            catch (Exception es)
            {
                getAPIAccess._failure = true;
                getAPIAccess._message = es.Message;
            }
            return getAPIAccess;
        }

        [HttpPost("generateAPIKey")]
        public ActionResult<GenerateAPIKeyViewModel> GenerateAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            try
            {
                generateAPIKey = _apiService.GenerateAPIKey(generateAPIKeyViewModel);
            }
            catch (Exception es)
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = es.Message;
            }
            return generateAPIKey;
        }

        [HttpPut("updateAPIKeyTitle")]
        public ActionResult<GenerateAPIKeyViewModel> UpdateAPIKeyTitle(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            try
            {
                generateAPIKey = _apiService.UpdateAPIKeyTitle(generateAPIKeyViewModel);
            }
            catch (Exception es)
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = es.Message;
            }
            return generateAPIKey;
        }

        [HttpPut("refreshAPIKey")]
        public ActionResult<GenerateAPIKeyViewModel> RefreshAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            try
            {
                generateAPIKey = _apiService.RefreshAPIKey(generateAPIKeyViewModel);
            }
            catch (Exception es)
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = es.Message;
            }
            return generateAPIKey;
        }

        [HttpPut("deleteAPIKey")]
        public ActionResult<GenerateAPIKeyViewModel> DeleteAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            try
            {
                generateAPIKey = _apiService.DeleteAPIKey(generateAPIKeyViewModel);
            }
            catch (Exception es)
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = es.Message;
            }
            return generateAPIKey;
        }

        //[HttpPost("getAPIAccessPermission")]
        //public ActionResult<bool> GetAPIAccessPermission(GetAPIAccessPermissionViewModel getAPIAccessPermissionViewModel)
        //{
        //    bool apiAccess = _apiService.GetAPIAccessPermission(getAPIAccessPermissionViewModel);

        //    return apiAccess;
        //}
    }
}
