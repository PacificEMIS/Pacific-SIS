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
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.ApiKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace opensis.data.Repository
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly CRMContext? context;
        private readonly CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
        
        public ApiKeyRepository(IDbContextFactory dbContextFactory, ICatalogDBContextFactory catdbContextFactory)
        {
            context = dbContextFactory.Create();
            catdbContext = catdbContextFactory.Create();
        }

        public GetAPIKeyListViewModel GetAPIKey(GetAPIKeyListViewModel GetAPIKeyListViewModel)
        {
            GetAPIKeyListViewModel getAPIKeyList = new();
            try
            {
                List<ApiKeysMaster>? apiKeyMasterData = this.context?.ApiKeysMaster.Where(x => x.TenantId == GetAPIKeyListViewModel.TenantId && x.SchoolId == GetAPIKeyListViewModel.SchoolId && x.IsActive == true).ToList();

                if (apiKeyMasterData != null && apiKeyMasterData.Any())
                {
                    getAPIKeyList.ApiKeysMasterList = apiKeyMasterData;
                }
                else
                {
                    getAPIKeyList._failure = true;
                    getAPIKeyList._message = NORECORDFOUND;
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
            try
            {
                if (addAPIAccessViewModel.ApiControllerKeyMapping.Any())
                {
                    foreach (var data in addAPIAccessViewModel.ApiControllerKeyMapping)
                    {
                        var apiControllerKeyMappingData = this.context?.ApiControllerKeyMapping.FirstOrDefault(x => x.TenantId == addAPIAccessViewModel.TenantId && x.SchoolId == addAPIAccessViewModel.SchoolId && x.KeyId == data.KeyId && x.ControllerId == data.ControllerId);

                        if(apiControllerKeyMappingData != null)
                        {
                            var apiControllerKeyMapping = new ApiControllerKeyMapping();
                            apiControllerKeyMappingData.IsActive = data.IsActive;
                            apiControllerKeyMappingData.UpdatedBy = data.UpdatedBy;
                            apiControllerKeyMapping.UpdatedOn = DateTime.UtcNow;

                            this.context?.SaveChanges();
                        }
                    }

                    addAPIAccessViewModel._failure = false;
                    addAPIAccessViewModel._message = "API access updated successfully";
                }
                else
                {
                    addAPIAccessViewModel._failure = true;
                    addAPIAccessViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                addAPIAccessViewModel._failure = true;
                addAPIAccessViewModel._message = es.Message;
            }
            return addAPIAccessViewModel;
        }

        //public GetAPIAccessViewModel GetAPIAccess(GetAPIAccessViewModel getAPIAccessViewModel)
        //{
        //    GetAPIAccessViewModel getAPIAccess = new GetAPIAccessViewModel();
        //    try
        //    {
        //        var apiControllerListData = this.context?.ApiControllerList.Include(x => x.ApiControllerKeyMapping).Where(x => x.TenantId == getAPIAccessViewModel.TenantId && x.SchoolId == getAPIAccessViewModel.SchoolId && x.ApiControllerKeyMapping.Any(y => y.KeyId == getAPIAccessViewModel.KeyId)).ToList().GroupBy(y => y.Module);

        //        //var result = apiControllerListData
        //        //.GroupBy(c => c.Module, (keyId, calls) =>
        //        //{

        //        //    //var data = calls.ToArray();
        //        //    // getAPIAccess.apiControllerList = data.ToList();

        //        //     new GetAPIAccessViewModel
        //        //    {
        //        //        KeyId = Convert.ToInt32(keyId),
        //        //        apiControllerList = calls.ToArray().ToList()

        //        //    };
        //        //}).ToList();

        //        //var stats = apiControllerListData.GroupBy(c => c.Module, (keyId, calls) => new ApiControllerKeyMapping
        //        //{
        //        //    KeyId = Convert.ToInt32(keyId),
        //        //    ControllerId = calls 
        //        //}).ToArray();


        //        //var result = new GetAPIAccessViewModel { CallTypes = stats, SavedCount = calls.Count(c => c.CallRecordStatus != 1.ToString()), SubmittedCount = calls.Count(c => c.CallRecordStatus == 1.ToString()) };


        //        if (apiControllerListData.ToList().Count > 0)
        //        {
        //            getAPIAccess.apiControllerList = apiControllerListData.ToList();

        //            foreach (var data in getAPIAccess.apiControllerList)
        //            {
        //                if (data.FirstOrDefault().ApiControllerKeyMapping.Count > 0)
        //                {
        //                    data.FirstOrDefault().ApiControllerKeyMapping = null;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            getAPIAccess._failure = true;
        //            getAPIAccess._message = NORECORDFOUND;
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        getAPIAccess._failure = true;
        //        getAPIAccess._message = es.Message;
        //    }
        //    return getAPIAccess;
        //}

        public GetAPIAccessViewModel GetAPIAccess(GetAPIAccessViewModel getAPIAccessViewModel)
        {
            GetAPIAccessViewModel getAPIAccess = new();
            try
            {
                List<ApiControllerList>? apiControllerListData = this.context?.ApiControllerList.Include(x => x.ApiControllerKeyMapping).Where(x => x.TenantId == getAPIAccessViewModel.TenantId && x.ApiControllerKeyMapping.Any(y => y.KeyId == getAPIAccessViewModel.KeyId && y.SchoolId == getAPIAccessViewModel.SchoolId)).ToList();

                if(apiControllerListData!=null && apiControllerListData.Any())
                {
                    var result = apiControllerListData
                .GroupBy(c => c.Module, (Modulename, calls) =>
                {

                    var data = calls.SelectMany(x => x.ApiControllerKeyMapping).Where(a => a.KeyId == getAPIAccessViewModel.KeyId && a.SchoolId == getAPIAccessViewModel.SchoolId).Select(x => new ApiViewAccessData
                    {
                        KeyId = x.KeyId,
                        ControllerId = x.ControllerId,
                        IsActive = x.IsActive,
                        ControllerPath = x.ApiControllerList.ControllerPath

                    }).ToArray();

                    return new ApiViewData
                    {
                        Module = Modulename,
                        ApiViewAccessData = data.ToList()

                    };
                   }).ToList();
                    getAPIAccess.ApiViewAccessData = result;
                    getAPIAccess.KeyId = getAPIAccessViewModel.KeyId;
                    getAPIAccess.SchoolId = getAPIAccessViewModel.SchoolId;
                    getAPIAccess.TenantId = getAPIAccessViewModel.TenantId;                
                }
                else
                {
                    getAPIAccess._failure = true;
                    getAPIAccess._message = NORECORDFOUND;
                }

                



                //if (apiControllerListData.ToList().Count == 0)
                //{
                //    getAPIAccess._failure = true;
                //    getAPIAccess._message = NORECORDFOUND;
                //}
                
            }
            catch (Exception es)
            {
                getAPIAccess._failure = true;
                getAPIAccess._message = es.Message;
            }
            return getAPIAccess;
        }


        /// <summary>
        /// Generate API Key
        /// </summary>
        /// <param name="generateAPIKeyViewModel"></param>
        /// <returns></returns>
        public GenerateAPIKeyViewModel GenerateAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            try
            {
                var APITitle = this.context?.ApiKeysMaster.AsEnumerable().FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId && String.Compare(x.ApiTitle, generateAPIKeyViewModel.ApiKeysMaster.ApiTitle, true) == 0);

                if (APITitle != null)
                {
                    generateAPIKeyViewModel._failure = true;
                    generateAPIKeyViewModel._message = "API title already exist";
                }
                else
                {
                    var tenantData = this.catdbContext?.AvailableTenants.FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId);
                    if(tenantData is not null)
                    {
                        string hashAPIKey = CreateEncryptApiKey(tenantData.TenantName, generateAPIKeyViewModel.ApiKeysMaster!.SchoolId);
                        int? APIKeyId = 1;

                        var APIData = this.context?.ApiKeysMaster.Where(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId).OrderByDescending(x => x.KeyId).FirstOrDefault();

                        if (APIData != null)
                        {
                            APIKeyId = APIData.KeyId + 1;
                        }

                        generateAPIKeyViewModel.ApiKeysMaster.KeyId = (int)APIKeyId;
                        generateAPIKeyViewModel.ApiKeysMaster.ApiKey = hashAPIKey;
                        generateAPIKeyViewModel.ApiKeysMaster.Expires = DateTime.Now.AddYears(+1);
                        generateAPIKeyViewModel.ApiKeysMaster.CreatedOn = DateTime.UtcNow;
                        generateAPIKeyViewModel.ApiKeysMaster.Emailaddress = "";
                        generateAPIKeyViewModel.ApiKeysMaster.IsActive = true;
                        generateAPIKeyViewModel.ApiKeysMaster.Revoked = false;

                        this.context?.Add(generateAPIKeyViewModel.ApiKeysMaster);

                        var controllerListData = context?.ApiControllerList.ToList();

                        if (controllerListData is not null && controllerListData.Any())
                        {
                            //List<ApiControllerKeyMapping> apiControllerKeyMappingList = new List<ApiControllerKeyMapping>();

                            List<ApiControllerKeyMapping> apiControllerKeyMappingList = new ();
                            foreach (var controllerList in controllerListData)
                            {
                                ApiControllerKeyMapping apiControllerKeyMapping = new ();

                                apiControllerKeyMapping.TenantId = generateAPIKeyViewModel.ApiKeysMaster.TenantId;
                                apiControllerKeyMapping.SchoolId = generateAPIKeyViewModel.ApiKeysMaster.SchoolId;
                                apiControllerKeyMapping.KeyId = (int)APIKeyId;
                                apiControllerKeyMapping.ControllerId = controllerList.ControllerId;
                                apiControllerKeyMapping.IsActive = false;
                                apiControllerKeyMapping.CreatedBy = generateAPIKeyViewModel.ApiKeysMaster.CreatedBy;
                                apiControllerKeyMapping.CreatedOn = DateTime.UtcNow;

                                apiControllerKeyMappingList.Add(apiControllerKeyMapping);
                            }
                            this.context?.ApiControllerKeyMapping.AddRange(apiControllerKeyMappingList);
                        }

                        this.context?.SaveChanges();

                        generateAPIKeyViewModel._failure = false;
                        generateAPIKeyViewModel._message = "API key created successfully";
                    }
                    
                }
            }
            catch (Exception es)
            {
                generateAPIKeyViewModel._failure = true;
                generateAPIKeyViewModel._message = es.Message;
            }
            return generateAPIKeyViewModel;
        }

        /// <summary>
        /// Update API Key Title
        /// </summary>
        /// <param name="generateAPIKeyViewModel"></param>
        /// <returns></returns>
        public GenerateAPIKeyViewModel UpdateAPIKeyTitle(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            try
            {
                var APITitle = this.context?.ApiKeysMaster.AsEnumerable().FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId && String.Compare(x.ApiTitle, generateAPIKeyViewModel.ApiKeysMaster.ApiTitle, true) == 0);

                if (APITitle != null)
                {
                    generateAPIKeyViewModel._failure = true;
                    generateAPIKeyViewModel._message = "API title already exist";
                }
                else
                {
                    var APIData = this.context?.ApiKeysMaster.FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId && x.KeyId == generateAPIKeyViewModel.ApiKeysMaster.KeyId);

                    if (APIData != null)
                    {
                        APIData.ApiTitle = generateAPIKeyViewModel.ApiKeysMaster!.ApiTitle;
                        APIData.UpdatedOn = DateTime.UtcNow;
                        APIData.UpdatedBy = generateAPIKeyViewModel.ApiKeysMaster.UpdatedBy;

                        this.context?.SaveChanges();

                        generateAPIKeyViewModel._failure = false;
                        generateAPIKeyViewModel._message = "API title updated successfully";
                    }
                    else
                    {
                        generateAPIKeyViewModel._failure = true;
                        generateAPIKeyViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                generateAPIKeyViewModel._failure = true;
                generateAPIKeyViewModel._message = es.Message;
            }
            return generateAPIKeyViewModel;
        }

        /// <summary>
        /// Refresh API Key
        /// </summary>
        /// <param name="generateAPIKeyViewModel"></param>
        /// <returns></returns>
        public GenerateAPIKeyViewModel RefreshAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            try
            {
                var APIData = this.context?.ApiKeysMaster.FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId && x.KeyId == generateAPIKeyViewModel.ApiKeysMaster.KeyId);

                if (APIData != null)
                {
                    var tenantData = this.catdbContext?.AvailableTenants.FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId);
                    if(tenantData is not null)
                    {
                        string hashAPIKey = CreateEncryptApiKey(tenantData.TenantName, generateAPIKeyViewModel.ApiKeysMaster!.SchoolId);
                        APIData.ApiKey = hashAPIKey;
                        APIData.UpdatedOn = DateTime.UtcNow;
                        APIData.UpdatedBy = generateAPIKeyViewModel.ApiKeysMaster.UpdatedBy;

                        this.context?.SaveChanges();

                        generateAPIKeyViewModel.ApiKeysMaster.ApiKey = hashAPIKey;
                        generateAPIKeyViewModel._failure = false;
                        generateAPIKeyViewModel._message = "API key refreshed successfully";
                    }
                    
                }
                else
                {
                    generateAPIKeyViewModel._failure = true;
                    generateAPIKeyViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                generateAPIKeyViewModel._failure = true;
                generateAPIKeyViewModel._message = es.Message;
            }
            return generateAPIKeyViewModel;
        }

        /// <summary>
        /// Delete API Key
        /// </summary>
        /// <param name="generateAPIKeyViewModel"></param>
        /// <returns></returns>
        public GenerateAPIKeyViewModel DeleteAPIKey(GenerateAPIKeyViewModel generateAPIKeyViewModel)
        {
            try
            {
                var APIData = this.context?.ApiKeysMaster.FirstOrDefault(x => x.TenantId == generateAPIKeyViewModel.ApiKeysMaster!.TenantId && x.SchoolId == generateAPIKeyViewModel.ApiKeysMaster.SchoolId && x.KeyId == generateAPIKeyViewModel.ApiKeysMaster.KeyId);

                if (APIData != null)
                {
                    APIData.IsActive = false;
                    APIData.UpdatedOn = DateTime.UtcNow;
                    APIData.UpdatedBy = generateAPIKeyViewModel.ApiKeysMaster!.UpdatedBy;

                    this.context?.SaveChanges();

                    generateAPIKeyViewModel._failure = false;
                    generateAPIKeyViewModel._message = "API key deleted successfully";
                }
                else
                {
                    generateAPIKeyViewModel._failure = true;
                    generateAPIKeyViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                generateAPIKeyViewModel._failure = true;
                generateAPIKeyViewModel._message = es.Message;
            }
            return generateAPIKeyViewModel;
        }

        private static string CreateEncryptApiKey(string tenantName, int schoolId)
        {
            string key = string.Empty;
            var randomNumber = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                key = Convert.ToBase64String(randomNumber);
            }

            string hashAPIKey = Utility.EncryptString(key + "|" + tenantName + "|" + schoolId);
            return hashAPIKey;
        }

       
    }
}
