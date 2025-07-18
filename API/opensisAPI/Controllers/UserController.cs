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
using opensis.data.ViewModels.School;
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

                    var school =
                     new SchoolMaster()
                     {
                         TenantId = tenantId,
                         SchoolId = (int)schoolId,
                         SchoolInternalId = schoolId.ToString(),
                         SchoolGuid = SchoolGuidId,
                         SchoolName = registrationViewModel.SchoolName,
                         CreatedOn = DateTime.UtcNow,
                         SchoolDetail = new List<SchoolDetail>()
                        {
                            new SchoolDetail(){Id=(int)schoolDetailId, TenantId=tenantId, SchoolId=(int)schoolId, DateSchoolOpened=DateTime.UtcNow,Status=true}
                        }

                     };

                    //insert seeding
                    string dataSchoolSeedingValue = System.IO.File.ReadAllText(@"SchoolSeedingData.json");
                    JsonSerializerSettings settingSSD = new JsonSerializerSettings();
                    SchoolSeedingDataViewModel objSchoolSeeding = JsonConvert.DeserializeObject<SchoolSeedingDataViewModel>(dataSchoolSeedingValue, settingSSD);
                    if (objSchoolSeeding != null)
                    {
                        var seedingMembershipData = objSchoolSeeding.MemberShip;
                        foreach (var item in seedingMembershipData)
                        {
                            item.TenantId = school.TenantId;
                            item.SchoolId = school.SchoolId;
                            item.CreatedBy = school.CreatedBy;
                            item.CreatedOn = school.CreatedOn;
                            school.Membership.Add(item);
                        }
                        var seedingFieldsCategoryData = objSchoolSeeding.FieldsCategory;
                        foreach (var item in seedingFieldsCategoryData)
                        {
                            item.TenantId = school.TenantId;
                            item.SchoolId = school.SchoolId;
                            item.CreatedBy = school.CreatedBy;
                            item.CreatedOn = school.CreatedOn;
                            school.FieldsCategory.Add(item);
                        }
                        var seedingStudentEnrollmentCodeData = objSchoolSeeding.StudentEnrollmentCode;
                        foreach (var item in seedingStudentEnrollmentCodeData)
                        {
                            item.TenantId = school.TenantId;
                            item.SchoolId = school.SchoolId;
                            item.AcademicYear = Convert.ToDecimal(registrationViewModel.SchoolBeginDate?.Year);
                            item.CreatedBy = school.CreatedBy;
                            item.CreatedOn = school.CreatedOn;
                            school.StudentEnrollmentCode.Add(item);
                        }
                        school.Block = new List<Block>()
                            {
                                    new Block(){TenantId=school.TenantId, SchoolId=school.SchoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate?.Year), CreatedOn=school.CreatedOn, CreatedBy=school.CreatedBy }
                            };

                        school.AttendanceCodeCategories = new List<AttendanceCodeCategories>() {
                     new AttendanceCodeCategories(){CreatedOn=school.CreatedOn,CreatedBy=school.CreatedBy, TenantId= school.TenantId,AttendanceCategoryId= 1, AcademicYear=Convert.ToDecimal(registrationViewModel.SchoolBeginDate?.Year) ,Title= "Student Attendance" }};

                        var seedingAttendanceCodeData = objSchoolSeeding.AttendanceCode;
                        foreach (var item in seedingAttendanceCodeData)
                        {
                            item.TenantId = school.TenantId;
                            item.SchoolId = school.SchoolId;
                            item.AcademicYear = Convert.ToDecimal(registrationViewModel.SchoolBeginDate?.Year);
                            item.CreatedBy = school.CreatedBy;
                            item.CreatedOn = school.CreatedOn;
                            this.context?.AttendanceCode.Add(item);
                        }
                    }

                    var dataDpValue = System.IO.File.ReadAllText(@"DPdownValue.json");
                    JsonSerializerSettings settingdpValue = new();
                    List<DpdownValuelist> objDpValue = JsonConvert.DeserializeObject<List<DpdownValuelist>>(dataDpValue, settingdpValue)!;

                    foreach (DpdownValuelist dpdownValue in objDpValue)
                    {
                        dpdownValue.TenantId = school.TenantId;
                        dpdownValue.SchoolId = school.SchoolId;
                        dpdownValue.Id = (long)dpdownValueId!++;
                        dpdownValue.CreatedBy = school.CreatedBy;
                        dpdownValue.CreatedOn = school.CreatedOn;
                        this.context?.DpdownValuelist.Add(dpdownValue);
                    }

                    //insert into permission group
                    var dataGroup = System.IO.File.ReadAllText(@"Group.json");
                    JsonSerializerSettings settingGrp = new JsonSerializerSettings();
                    List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp);

                    foreach (PermissionGroup permisionGrp in objGroup)
                    {
                        permisionGrp.TenantId = tenantId;
                        permisionGrp.SchoolId = (int)schoolId;
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

                    this.context?.SchoolMaster.Add(school);

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

        [HttpPost("getEncryptPassword")]
        public IActionResult GetEncryptPWD(string password)
        {
            string passwordHash = null;
            try
            {
                passwordHash = Utility.EncryptString(password);
            }
            catch (Exception es)
            {
                return Ok(es.Message);
            }
            return Ok(passwordHash);
        }
    }
}
