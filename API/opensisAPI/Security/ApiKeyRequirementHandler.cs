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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using opensis.data.Factory;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace opensisAPI.Security
{
    public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        public const string API_KEY_HEADER_NAME = "X-API-KEY";
        private readonly IHttpContextAccessor httpContextAccessor;
        public ApiKeyRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext authContext, ApiKeyRequirement requirement)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var query = httpContext.Request.Headers;
           
            if (!query.ContainsKey(API_KEY_HEADER_NAME))
            {
                
               httpContext.Response.StatusCode = 401;
               httpContext.Response.ContentType = "application/json";
               httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
               {
                   Msg = "Api Key was not provided"
               }));

                authContext.Fail();
            }
            if (query.ContainsKey(API_KEY_HEADER_NAME))
            {
                var apiKey = query[API_KEY_HEADER_NAME].FirstOrDefault();
                
                if (apiKey != null && IsApiKeyPresentAndValid(apiKey))
                {
                    authContext.Succeed(requirement);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    httpContext.Response.ContentType = "application/json";      
                    httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Msg = "Api Key is not valid"
                    }));
                    authContext.Fail();
                }
            }
            return Task.CompletedTask;
        }
        
        private bool IsApiKeyPresentAndValid(string apiKey)
        {
            bool result = false;
            string message = null;
            try
            {
                var dbContextFactory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IDbContextFactory>();
                var decryptApiKey = Utility.DecryptString(apiKey);
                string[] apiKeyInfoArray = decryptApiKey.Split('|');
                string TenantName = apiKeyInfoArray[1];
                dbContextFactory.TenantName = TenantName;
                var dbContext = dbContextFactory.Create();
                var data = dbContext.ApiKeysMaster.Where(a => a.ApiKey == apiKey && (bool)a.IsActive).FirstOrDefault();
                if (data != null)
                {
                    dbContextFactory.ApiKeyValue = apiKey;
                    result = true;
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
                result = false;
            }

            return result;
        }
    }  
}
