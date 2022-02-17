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
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using opensis.core.User.Interfaces;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.User;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/User")]
    [ApiController]    
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly IAntiforgery _antiForgery;
        private readonly IConfiguration _configuration;
        private CRMContext context;
        private CatalogDBContext catdbContext;
        public UserController(IUserService userService,IServiceProvider serviceProvider, IConfiguration configuration, IDbContextFactory dbContextFactory, ICatalogDBContextFactory catdbContextFactory)
        {
            _userService = userService;
            this._configuration = configuration;

            if (this._configuration.GetValue<bool>("AntiForgeryTokenValidationEnabled"))
                this._antiForgery = serviceProvider.GetService<IAntiforgery>();

            this.context = dbContextFactory.Create();
            this.catdbContext = catdbContextFactory.Create();
        }

        /// <summary>
        /// This is used for authentcatred the login process
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>

        [HttpPost("ValidateLogin")]
        public ActionResult<LoginViewModel> ValidateLogin(LoginViewModel objModel)
        {
            var response=  _userService.ValidateUserLogin(objModel);
            if (this._configuration.GetValue<bool>("AntiForgeryTokenValidationEnabled"))
                this.GenerateAntiForgeryToken(response._tokenExpiry);
            return response;
        }

        [HttpPost("checkUserLoginEmail")]
        public ActionResult<CheckUserEmailAddressViewModel> CheckUserLoginEmail(CheckUserEmailAddressViewModel checkUserEmailAddressViewModel)
        {
            CheckUserEmailAddressViewModel checkUserEmailAddress = new CheckUserEmailAddressViewModel();
            try
            {
                checkUserEmailAddress = _userService.CheckUserLoginEmail(checkUserEmailAddressViewModel);

            }
            catch (Exception es)
            {
                checkUserEmailAddress._message = es.Message;
                checkUserEmailAddress._failure = true;
            }
            return checkUserEmailAddress;
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public ActionResult<LoginViewModel> RefreshToken(LoginViewModel objModel)
        {
            var response = _userService.RefreshToken(objModel);

            if (this._configuration.GetValue<bool>("AntiForgeryTokenValidationEnabled"))
                this.GenerateAntiForgeryToken(response._tokenExpiry);

            return response;
        }
        private void GenerateAntiForgeryToken(System.DateTimeOffset tokenExpiry)
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            var cookieOptions = new CookieOptions()
            {
                Domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}",
                Expires = tokenExpiry,
                HttpOnly = this._configuration.GetValue<bool>("AntiForgeryHttpOnly"),
                Secure = this._configuration.GetValue<bool>("AntiForgerySecureCookie")
            };
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, options: cookieOptions);
        }

        [HttpPost("logOutForUser")]
        public ActionResult<LoginViewModel> LogOutForUser(LoginViewModel loginViewModel)
        {
            LoginViewModel loginView = new LoginViewModel();
            try
            {
                loginView = _userService.LogOutForUser(loginViewModel);

            }
            catch (Exception es)
            {
                loginView._message = es.Message;
                loginView._failure = true;
            }
            return loginView;
        }

        [HttpPost("getAllUserAccessLog")]
        public ActionResult<UserAccessLogListViewModel> GetAllUserAccessLog(PageResult pageResult)
        {
            UserAccessLogListViewModel userAccessLogList = new UserAccessLogListViewModel();
            try
            {
                userAccessLogList = _userService.GetAllUserAccessLog(pageResult);

            }
            catch (Exception es)
            {
                userAccessLogList._message = es.Message;
                userAccessLogList._failure = true;
            }
            return userAccessLogList;
        }

        [HttpPost("deleteUserAccessLogs")]
        public ActionResult<UserAccessLogListViewModel> DeleteUserAccessLogs(UserAccessLogListViewModel userAccessLogListViewModel)
        {
            UserAccessLogListViewModel userAccessLogListDelete = new UserAccessLogListViewModel();
            try
            {
                userAccessLogListDelete = _userService.DeleteUserAccessLogs(userAccessLogListViewModel);

            }
            catch (Exception es)
            {
                userAccessLogListDelete._message = es.Message;
                userAccessLogListDelete._failure = true;
            }
            return userAccessLogListDelete;
        }

        [HttpPost("insertInitialDataAtRegistration")]
        public ActionResult<RegistrationViewModel> InsertInitialDataAtRegistration(RegistrationViewModel registrationViewModel)
        {
            try
            {
                var tenantDetail = this.catdbContext?.AvailableTenants.Where(x => x.TenantName == registrationViewModel.TenantName && x.IsActive).FirstOrDefault();

                if (tenantDetail != null)
                {
                    Guid tenantId = (Guid)tenantDetail.TenantId;

                    int? schoolId = Utility.GetMaxPK(this.context, new Func<SchoolMaster, int>(x => x.SchoolId));
                    int? schoolDetailId = Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id));
                    long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));
                    int? gradeId = Utility.GetMaxPK(this.context, new Func<Gradelevels, int>(x => x.GradeId));
                    Guid SchoolGuidId = Guid.NewGuid();

                    var school = new List<SchoolMaster>()
                    { new SchoolMaster() {TenantId=tenantId,SchoolId=(int)schoolId,SchoolInternalId=schoolId.ToString(),SchoolGuid=SchoolGuidId,SchoolName=registrationViewModel.SchoolName,CreatedOn=DateTime.UtcNow,
                        Membership=new List<Membership>()
                        {
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Super Administrator", IsActive= true, IsSuperadmin= true, IsSystem= true, MembershipId= 1, ProfileType= "Super Administrator"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "School Administrator", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 2, ProfileType= "School Administrator"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Admin Assistant", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 3, ProfileType= "Admin Assistant"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 4, ProfileType= "Teacher"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Homeroom Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 5, ProfileType= "Homeroom Teacher"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Parent", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 6, ProfileType= "Parent"},
                            new Membership(){CreatedOn=DateTime.UtcNow, TenantId= tenantId,Profile= "Student", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 7, ProfileType= "Student"}
                        },
                        SchoolDetail =new List<SchoolDetail>()
                        {
                            new SchoolDetail(){Id=(int)schoolDetailId, TenantId=tenantId, SchoolId=(int)schoolId, DateSchoolOpened=DateTime.UtcNow,Status=true}
                        },
                        DpdownValuelist=new List<DpdownValuelist>() {
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="PK",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="K",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+1},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="1",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+2},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="2",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+3},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="3",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+4},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="4",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+5},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="5",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+6},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="6",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+7},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="7",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+8},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="8",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+9},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="9",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+10},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="10",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+11},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="11",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+12},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="12",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+13},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="13",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+14},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="14",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+15},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="15",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+16},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="16",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+17},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="17",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+18},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="18",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+19},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="19",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+20},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Grade Level",LovColumnValue="20",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+21},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="School Gender",LovColumnValue="Boys",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+22},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="School Gender",LovColumnValue="Girls",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+23},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="School Gender",LovColumnValue="Mixed",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+24},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Mr.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+25},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Miss.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+26},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Mrs.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+27},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Ms.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+28},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Dr.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+29},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Rev.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+30},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Prof.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+31},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Sir.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+32},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Salutation",LovColumnValue="Lord ",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+33},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="Jr.",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+34},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="Sr",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+35},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="II",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+37},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="III",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+38},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="IV",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+39},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="V",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+40},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Suffix",LovColumnValue="PhD",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+41},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Gender",LovColumnValue="Male",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+42},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Gender",LovColumnValue="Female",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+43},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Gender",LovColumnValue="Other",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+44},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Marital Status",LovColumnValue="Single",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+45},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Marital Status",LovColumnValue="Married",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+46},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Marital Status",LovColumnValue="Partnered",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+47},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Rolling/Retention Option",LovColumnValue="Next grade at current school",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+48},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Rolling/Retention Option",LovColumnValue="Retain",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+49},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Rolling/Retention Option",LovColumnValue="Do not enroll after this school year",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+50},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Relationship",LovColumnValue="Mother",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+51},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Relationship",LovColumnValue="Father",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+52},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Relationship",LovColumnValue="Legal Guardian",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+53},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Relationship",LovColumnValue="Other",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+54},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Enrollment Type",LovColumnValue="Add",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+55},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Enrollment Type",LovColumnValue="Drop",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+56},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Enrollment Type",LovColumnValue="Rolled Over",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+57},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Enrollment Type",LovColumnValue="Drop (Transfer)",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+58},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Enrollment Type",LovColumnValue="Enroll (Transfer)",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+59},


                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Dropdown",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+60},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Editable Dropdown",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+61},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Text",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+62},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Checkbox",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+63},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Number",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+64},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Multiple SelectBox",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+65},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Date",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+66},
                        new DpdownValuelist(){UpdatedOn=DateTime.UtcNow, TenantId= tenantId,SchoolId=(int)schoolId,LovName="Field Type",LovColumnValue="Textarea",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+67},
                    },
                    FieldsCategory=new List<FieldsCategory>()
                    {
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="General Information",Module="School",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=1},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Wash Information",Module="School",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=2},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Student",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=3},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Enrollment Info",Module="Student",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=4},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Student",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=5},

                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Family Info",Module="Student",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=6},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Medical Info",Module="Student",SortOrder=5,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=7},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Comments",Module="Student",SortOrder=6,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=8},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Documents",Module="Student",SortOrder=7,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=9},

                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Parent",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=10},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Address Info",Module="Parent",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=11},

                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Staff",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=12},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="School Info",Module="Staff",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=13},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Staff",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=14},
                        new FieldsCategory(){ TenantId=tenantId,SchoolId=(int)schoolId,IsSystemCategory=true,Search=true, Title="Certification Info",Module="Staff",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CategoryId=15}
                    },
                    StudentEnrollmentCode= new List<StudentEnrollmentCode>()
                    {
                        new StudentEnrollmentCode(){TenantId=tenantId, SchoolId=(int)schoolId, EnrollmentCode=1, Title="New", ShortName="NEW", SortOrder=1, Type="Add", AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  },
                        new StudentEnrollmentCode(){TenantId=tenantId, SchoolId=(int)schoolId, EnrollmentCode=2, Title="Dropped Out", ShortName="DROP", SortOrder=2, Type="Drop", AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  },
                        new StudentEnrollmentCode(){TenantId=tenantId, SchoolId=(int)schoolId, EnrollmentCode=3, Title="Rolled Over", ShortName="ROLL", SortOrder=3, Type="Rolled Over", AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  },
                        new StudentEnrollmentCode(){TenantId=tenantId, SchoolId=(int)schoolId, EnrollmentCode=4, Title="Transferred In", ShortName="TRAN", SortOrder=4, Type="Enroll (Transfer)", AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  },
                        new StudentEnrollmentCode(){TenantId=tenantId, SchoolId=(int)schoolId, EnrollmentCode=5, Title="Transferred Out", ShortName="TRAN", SortOrder=5, Type="Drop (Transfer)", AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  }
                    },
                    Block=new List<Block>()
                    {
                        new Block(){TenantId=tenantId, SchoolId=(int)schoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year), CreatedOn=DateTime.UtcNow,  }
                    }
                },
                    }.ToList();

                    ReleaseNumber releaseNumber = new ReleaseNumber();
                    {
                        releaseNumber.TenantId = tenantId;
                        releaseNumber.SchoolId = (int)schoolId;
                        releaseNumber.ReleaseNumber1 = "1.0.0";
                        releaseNumber.ReleaseDate = DateTime.UtcNow;
                    }

                    //insert into permission group
                    var dataGroup = System.IO.File.ReadAllText(@"Group.json");
                    JsonSerializerSettings settingGrp = new JsonSerializerSettings();
                    List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp);

                    foreach (PermissionGroup permisionGrp in objGroup)
                    {

                        permisionGrp.TenantId = tenantId;
                        permisionGrp.SchoolId = (int)schoolId;
                        //permisionGrp.IsActive = true;
                        permisionGrp.PermissionCategory = null;
                        this.context?.PermissionGroup.Add(permisionGrp);
                    }

                    //insert into system default custom fields
                    var dataCustomFields = System.IO.File.ReadAllText(@"CustomFields.json");
                    JsonSerializerSettings settingCusFld = new JsonSerializerSettings();
                    List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld);
                    foreach (CustomFields customFields in objCusFld)
                    {
                        customFields.TenantId = tenantId;
                        customFields.SchoolId = (int)schoolId;
                        customFields.UpdatedBy = "poulamibose01@gmail.com";
                        customFields.UpdatedOn = DateTime.UtcNow;
                        this.context?.CustomFields.Add(customFields);
                    }

                    //insert into permission category
                    var dataCategory = System.IO.File.ReadAllText(@"Category.json");
                    JsonSerializerSettings settingCat = new JsonSerializerSettings();
                    List<PermissionCategory> objCat = JsonConvert.DeserializeObject<List<PermissionCategory>>(dataCategory, settingCat);
                    foreach (PermissionCategory permissionCate in objCat)
                    {
                        permissionCate.TenantId = tenantId;
                        permissionCate.SchoolId = (int)schoolId;
                        permissionCate.PermissionGroup = null;
                        permissionCate.RolePermission = null;
                        permissionCate.CreatedOn = DateTime.UtcNow;
                        this.context?.PermissionCategory.Add(permissionCate);
                    }

                    //insert into permission subcategory
                    var dataSubCategory = System.IO.File.ReadAllText(@"SubCategory.json");
                    JsonSerializerSettings settingSubCat = new JsonSerializerSettings();
                    List<PermissionSubcategory> objSubCat = JsonConvert.DeserializeObject<List<PermissionSubcategory>>(dataSubCategory, settingSubCat);
                    foreach (PermissionSubcategory permissionSubCate in objSubCat)
                    {
                        permissionSubCate.TenantId = tenantId;
                        permissionSubCate.SchoolId = (int)schoolId;
                        permissionSubCate.RolePermission = null;
                        permissionSubCate.CreatedOn = DateTime.UtcNow;
                        this.context?.PermissionSubcategory.Add(permissionSubCate);
                    }

                    //insert into role permission
                    var dataRolePermission = System.IO.File.ReadAllText(@"RolePermission.json");
                    JsonSerializerSettings settingRole = new JsonSerializerSettings();
                    List<RolePermission> objRole = JsonConvert.DeserializeObject<List<RolePermission>>(dataRolePermission, settingRole);
                    foreach (RolePermission permissionRole in objRole)
                    {
                        permissionRole.TenantId = tenantId;
                        permissionRole.SchoolId = (int)schoolId;
                        permissionRole.PermissionCategory = null;
                        permissionRole.Membership = null;
                        permissionRole.CreatedOn = DateTime.UtcNow;
                        this.context?.RolePermission.Add(permissionRole);
                    }

                    this.context?.SchoolMaster.AddRange(school);
                    this.context?.ReleaseNumber.Add(releaseNumber);

                    //var dpDownvalueData = this.context?.DpdownValuelist.Where(x => x.SchoolId == null).ToList();
                    var dpDownvalueData = this.context?.DpdownValuelist.ToList();

                    if (dpDownvalueData.Count == 0)
                    {
                        var DpdownValuelist = new List<DpdownValuelist>()
                        {
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Level",LovColumnValue="Pre-K",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+68},
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Level",LovColumnValue="Primary",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+69},
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Level",LovColumnValue="Middle",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+70},
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Level",LovColumnValue="Secondary",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+71},

                            new DpdownValuelist(){TenantId=tenantId,LovName="School Classification",LovColumnValue="Public",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+72},
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Classification",LovColumnValue="Private",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+73},
                            new DpdownValuelist(){TenantId=tenantId,LovName="School Classification",LovColumnValue="Charter",CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+74},
                        };

                        this.context?.DpdownValuelist.AddRange(DpdownValuelist);
                    }

                    var userData = this.context?.UserMaster.FirstOrDefault(x => x.EmailAddress == registrationViewModel.EmailAddress);

                    if (userData == null)
                    {
                        int? staffId = 1;
                        var dataExits = this.context?.StaffMaster.Where(x => x.TenantId == tenantId).Count();

                        if (dataExits > 0)
                        {
                            var staffData = this.context?.StaffMaster.Where(x => x.TenantId == tenantId).Max(x => x.StaffId);
                            if (staffData != null)
                            {
                                staffId = staffData + 1;
                            }
                        }

                        Guid StaffGuidId = Guid.NewGuid();

                        var staffMasterData = new StaffMaster();
                        {
                            staffMasterData.TenantId = tenantId;
                            staffMasterData.SchoolId = (int)schoolId;
                            staffMasterData.StaffId = (int)staffId;
                            staffMasterData.StaffInternalId = staffId.ToString();
                            staffMasterData.FirstGivenName = registrationViewModel.UserName;
                            staffMasterData.StaffGuid = StaffGuidId;
                            staffMasterData.LoginEmailAddress = registrationViewModel.EmailAddress;
                            staffMasterData.PortalAccess = true;
                            staffMasterData.Profile = "Super Administrator";
                            staffMasterData.JobTitle = "Super Administrator";
                            staffMasterData.IsActive = true;
                            staffMasterData.CreatedOn = DateTime.UtcNow;
                        }
                        this.context?.StaffMaster.Add(staffMasterData);

                        string hashPassword = Utility.GetHashedPassword(registrationViewModel.Password);

                        var userMasterData = new UserMaster();
                        {
                            userMasterData.TenantId = tenantId;
                            userMasterData.SchoolId = (int)schoolId;
                            userMasterData.UserId = (int)staffId;
                            userMasterData.Name = registrationViewModel.UserName;
                            userMasterData.EmailAddress = registrationViewModel.EmailAddress;
                            userMasterData.PasswordHash = hashPassword;
                            userMasterData.LangId = 1;
                            userMasterData.MembershipId = 1;
                            userMasterData.IsActive = true;
                            userMasterData.CreatedOn = DateTime.UtcNow;
                        }
                        this.context?.UserMaster.Add(userMasterData);
                    }

                    //Insert Calendar
                    SchoolCalendars schoolCalendars = new();

                    schoolCalendars.TenantId = tenantId;
                    schoolCalendars.SchoolId = (int)schoolId;
                    schoolCalendars.CalenderId = 1;
                    schoolCalendars.Title = "Default Calendar";
                    schoolCalendars.AcademicYear = registrationViewModel.SchoolBeginDate != null ? Convert.ToDecimal(registrationViewModel.SchoolBeginDate.Value.Year) : 0;
                    schoolCalendars.DefaultCalender = true;
                    schoolCalendars.SessionCalendar = true;
                    schoolCalendars.Days = "12345";
                    schoolCalendars.StartDate = registrationViewModel.SchoolBeginDate;
                    schoolCalendars.EndDate = registrationViewModel.SchoolEndDate;
                    schoolCalendars.CreatedOn = DateTime.UtcNow;

                    this.context?.SchoolCalendars.Add(schoolCalendars);

                    //Insert data in APIControllerList table
                    List<ApiControllerList> apiControllerList = new()
                    {
                        new ApiControllerList() { TenantId = tenantId, SchoolId = (int)schoolId, ControllerId = 1, ApiTitle = "GetSchoolDetails", ControllerPath = registrationViewModel.APIDomain + "api/getSchoolDetails/academicYear/{academicYear}", IsActive = true, Module = "School", CreatedOn = DateTime.UtcNow },
                        new ApiControllerList() { TenantId = tenantId, SchoolId = (int)schoolId, ControllerId = 2, ApiTitle = "GetAllStudent", ControllerPath = registrationViewModel.APIDomain + "api/getAllStudent/academicYear/{academicYear}", IsActive = true, Module = "Student", CreatedOn = DateTime.UtcNow },
                        new ApiControllerList() { TenantId = tenantId, SchoolId = (int)schoolId, ControllerId = 3, ApiTitle = "GetAllStaff", ControllerPath = registrationViewModel.APIDomain + "api/getAllStaff/academicYear/{academicYear}", IsActive = true, Module = "Staff", CreatedOn = DateTime.UtcNow }
                    };
                    this.context?.ApiControllerList.AddRange(apiControllerList);

                    this.context?.SaveChanges();

                    registrationViewModel._failure = false;
                    registrationViewModel._message = "Initial registration data added successfully.";
                }
                else
                {
                    registrationViewModel._failure = true;
                    registrationViewModel._message = "Tenant not found.";

                    return registrationViewModel;
                }
            }
            catch (Exception es)
            {
                registrationViewModel._failure = true;
                registrationViewModel._message = es.Message;
            }
            return registrationViewModel;
        }
    }
}
