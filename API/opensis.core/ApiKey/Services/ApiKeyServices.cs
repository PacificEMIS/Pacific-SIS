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

using opensis.core.ApiKey.Interfaces;
using opensis.core.helper.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.ApiKey;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.ApiKey.Services
{
    public class ApiKeyServices : IApiKeyServices
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IApiKeyRepository apiRepository;
        public ICheckLoginSession tokenManager;
        public ApiKeyServices(IApiKeyRepository apiRepository, ICheckLoginSession checkLoginSession)
        {
            this.apiRepository = apiRepository;
            this.tokenManager = checkLoginSession;
        }

        public GetAPIKeyListViewModel GetAPIKey(GetAPIKeyListViewModel getAPIKeyListViewModel)
        {
            GetAPIKeyListViewModel getAPIKeyList = new GetAPIKeyListViewModel();
            try
            {
                if (tokenManager.CheckToken(getAPIKeyListViewModel._tenantName + getAPIKeyListViewModel._userName, getAPIKeyListViewModel._token))
                {
                    getAPIKeyList = this.apiRepository.GetAPIKey(getAPIKeyListViewModel);
                }
                else
                {
                    getAPIKeyList._failure = true;
                    getAPIKeyList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                getAPIKeyList._failure = true;
                getAPIKeyList._message = es.Message;
            }
            return getAPIKeyList;
        }

        public AddAPIAccessViewModel AddAPIAccess(AddAPIAccessViewModel addAPIAccessViewModel)
        {
            AddAPIAccessViewModel addAPIAccess = new AddAPIAccessViewModel();
            try
            {
                if (tokenManager.CheckToken(addAPIAccessViewModel._tenantName + addAPIAccessViewModel._userName, addAPIAccessViewModel._token))
                {
                    addAPIAccess = this.apiRepository.AddAPIAccess(addAPIAccessViewModel);
                }
                else
                {
                    addAPIAccess._failure = true;
                    addAPIAccess._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                addAPIAccess._failure = true;
                addAPIAccess._message = es.Message;
            }
            return addAPIAccess;
        }

        public GetAPIAccessViewModel GetAPIAccess(GetAPIAccessViewModel getAPIAccessViewModel)
        {
            GetAPIAccessViewModel getAPIAccess = new GetAPIAccessViewModel();
            try
            {
                if (tokenManager.CheckToken(getAPIAccessViewModel._tenantName + getAPIAccessViewModel._userName, getAPIAccessViewModel._token))
                {
                    getAPIAccess = this.apiRepository.GetAPIAccess(getAPIAccessViewModel);
                }
                else
                {
                    getAPIAccess._failure = true;
                    getAPIAccess._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                getAPIAccess._failure = true;
                getAPIAccess._message = es.Message;
            }
            return getAPIAccess;
        }

        public GenerateAPIKeyViewModel GenerateAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            if (tokenManager.CheckToken(generateAPIKeyViewModel._tenantName + generateAPIKeyViewModel._userName, generateAPIKeyViewModel._token))
            {
                generateAPIKey = this.apiRepository.GenerateAPIKey(generateAPIKeyViewModel);
            }
            else
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = TOKENINVALID;
            }
            return generateAPIKey;
        }

        public GenerateAPIKeyViewModel UpdateAPIKeyTitle(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            if (tokenManager.CheckToken(generateAPIKeyViewModel._tenantName + generateAPIKeyViewModel._userName, generateAPIKeyViewModel._token))
            {
                generateAPIKey = this.apiRepository.UpdateAPIKeyTitle(generateAPIKeyViewModel);
            }
            else
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = TOKENINVALID;
            }
            return generateAPIKey;
        }

        public GenerateAPIKeyViewModel RefreshAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            if (tokenManager.CheckToken(generateAPIKeyViewModel._tenantName + generateAPIKeyViewModel._userName, generateAPIKeyViewModel._token))
            {
                generateAPIKey = this.apiRepository.RefreshAPIKey(generateAPIKeyViewModel);
            }
            else
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = TOKENINVALID;
            }
            return generateAPIKey;
        }

        public GenerateAPIKeyViewModel DeleteAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            GenerateAPIKeyViewModel generateAPIKey = new GenerateAPIKeyViewModel();
            if (tokenManager.CheckToken(generateAPIKeyViewModel._tenantName + generateAPIKeyViewModel._userName, generateAPIKeyViewModel._token))
            {
                generateAPIKey = this.apiRepository.DeleteAPIKey(generateAPIKeyViewModel);
            }
            else
            {
                generateAPIKey._failure = true;
                generateAPIKey._message = TOKENINVALID;
            }
            return generateAPIKey;
        }

        //public bool GetAPIAccessPermission(GetAPIAccessPermissionViewModel getAPIAccessPermissionViewModel)
        //{
        //    bool apiAccess=false;
        //    if (tokenManager.CheckToken(getAPIAccessPermissionViewModel._tenantName + getAPIAccessPermissionViewModel._userName, getAPIAccessPermissionViewModel._token))
        //    {
        //        apiAccess = this.apiRepository.GetAPIAccessPermission(getAPIAccessPermissionViewModel);
        //    }

        //    return apiAccess;


        //}
    }
}
