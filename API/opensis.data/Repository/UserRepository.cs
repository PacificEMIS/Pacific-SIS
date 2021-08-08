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

using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.IO;
using opensis.data.Helper;
using opensis.data.ViewModels.RoleBasedAccess;

namespace opensis.data.Repository
{
    public class UserRepository : IUserRepository
    {
        private CRMContext context;
        private static string EMAILMESSAGE = "Email address is not registered in the system";
        private static string PASSWORDMESSAGE = "Email + password combination is incorrect, try again";
        private static string INCORRECTLOGINATTEMPTMESSAGE = "Due to excessive incorrect login attempts your account has been deactivated. Please contact the school administration to reactivate your account";
        private static string INACTIVITYDAYSMESSAGE = "Due to your long absence from the system, as a measure of security your account has been deactivated. Please contact the school administration to reactivate your account";
        public UserRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        ///  ValidateUserLogin method is used for authentcatred the login process
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        public LoginViewModel ValidateUserLogin(LoginViewModel objModel)
        {
            LoginViewModel ReturnModel = new LoginViewModel();
            try
            {
                UserMaster userMaster = new UserMaster();
                var decrypted = Utility.Decrypt(objModel.Password);

                string passwordHash = Utility.GetHashedPassword(decrypted);
                ReturnModel._tenantName = objModel._tenantName;
                //var encryptedPassword = EncodePassword(objModel.Password);
                //var user = this.context?.UserMaster.Include(x => x.Membership).Where(x => x.EmailAddress == objModel.Email && x.PasswordHash == passwordHash && (x.MembershipId==1 || x.MembershipId == 2 || x.MembershipId == 3 || x.MembershipId == 4)).FirstOrDefault();

                var user = this.context?.UserMaster.Include(x => x.Membership).Where(x => x.EmailAddress == objModel.Email && x.PasswordHash == passwordHash && (x.Membership.ProfileType.ToLower() == "Super Administrator".ToLower() || x.Membership.ProfileType.ToLower() == "School Administrator".ToLower() || x.Membership.ProfileType.ToLower() == "Admin Assistant".ToLower() || x.Membership.ProfileType.ToLower() == "Teacher".ToLower())).FirstOrDefault();

                var correctEmailList = this.context?.UserMaster.Where(x => x.EmailAddress.Contains(objModel.Email)).ToList();

                var correctPasswordList = this.context?.UserMaster.Where(x => x.PasswordHash == passwordHash).ToList();

                if (correctEmailList.Count > 0)
                {
                    var schoolPreferenceData = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == correctEmailList.FirstOrDefault().TenantId && x.SchoolId == correctEmailList.FirstOrDefault().SchoolId);

                    if (schoolPreferenceData != null)
                    {
                        if (correctEmailList.FirstOrDefault().LoginFailureCount!=null && schoolPreferenceData.MaxLoginFailure!=null && correctEmailList.FirstOrDefault().LoginFailureCount >= schoolPreferenceData.MaxLoginFailure)
                        {
                            ReturnModel.UserId = null;
                            ReturnModel._failure = true;
                            ReturnModel._message = INCORRECTLOGINATTEMPTMESSAGE;

                            return ReturnModel;
                        }

                        if (correctEmailList.FirstOrDefault().LoginAttemptDate != null && schoolPreferenceData.MaxInactivityDays != null && schoolPreferenceData.MaxInactivityDays != 0)
                        {
                            int numberOfDays = (DateTime.UtcNow - correctEmailList.FirstOrDefault().LoginAttemptDate).Value.Days;

                            if (numberOfDays >= schoolPreferenceData.MaxInactivityDays)
                            {
                                ReturnModel.UserId = null;
                                ReturnModel._failure = true;
                                ReturnModel._message = INACTIVITYDAYSMESSAGE;

                                return ReturnModel;
                            }
                        }
                    } 
                }

                if (user == null && correctEmailList.Count > 0 && correctPasswordList.Count == 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;

                    if (correctEmailList.FirstOrDefault().LoginFailureCount != null)
                    {
                        correctEmailList.FirstOrDefault().LoginFailureCount = correctEmailList.FirstOrDefault().LoginFailureCount + 1;
                    }
                    else
                    {
                        correctEmailList.FirstOrDefault().LoginFailureCount = 1;
                    }
                }
                else if (user == null && correctEmailList.Count == 0 && correctPasswordList.Count > 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = EMAILMESSAGE;
                }
                else if (user == null && correctEmailList.Count == 0 && correctPasswordList.Count == 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;
                }
                else if (user == null && correctEmailList.Count > 0 && correctPasswordList.Count > 0)
                {
                    ReturnModel.UserId = null;
                    ReturnModel._failure = true;
                    ReturnModel._message = PASSWORDMESSAGE;

                    if (correctEmailList.FirstOrDefault().LoginFailureCount != null)
                    {
                        correctEmailList.FirstOrDefault().LoginFailureCount = correctEmailList.FirstOrDefault().LoginFailureCount + 1;
                    }
                    else
                    {
                        correctEmailList.FirstOrDefault().LoginFailureCount = 1;
                    }
                }
                else
                {
                    if (user.Membership.ProfileType.ToLower() == "Teacher".ToLower())
                    {
                        var userPhoto = this.context?.StaffMaster.Where(x => x.TenantId == user.TenantId && x.SchoolId == user.SchoolId && x.StaffId == user.UserId).Select(x => x.StaffPhoto).FirstOrDefault();

                        if (userPhoto != null)
                        {
                            ReturnModel.UserPhoto = userPhoto;
                        }
                    }
                    if (user.Membership.ProfileType.ToLower() == "Student".ToLower())
                    {
                        var userPhoto = this.context?.StudentMaster.Where(x => x.TenantId == user.TenantId && x.SchoolId == user.SchoolId && x.StudentId == user.UserId).Select(x => x.StudentPhoto).FirstOrDefault();

                        if (userPhoto != null)
                        {
                            ReturnModel.UserPhoto = userPhoto;
                        }
                    }
                    if (user.Membership.ProfileType.ToLower() == "Parent".ToLower())
                    {
                        var userPhoto = this.context?.ParentInfo.Where(x => x.TenantId == user.TenantId && x.SchoolId == user.SchoolId && x.ParentId == user.UserId).Select(x => x.ParentPhoto).FirstOrDefault();

                        if (userPhoto != null)
                        {
                            ReturnModel.UserPhoto = userPhoto;
                        }
                    }

                    ReturnModel.UserId = user.UserId;
                    ReturnModel.TenantId = user.TenantId;
                    ReturnModel.Email = user.EmailAddress;
                    ReturnModel.Name = user.Name;
                    ReturnModel.LastUsedSchoolId = user.LastUsedSchoolId;
                    ReturnModel.MembershipName = user.Membership.Profile;
                    ReturnModel.MembershipType = user.Membership.ProfileType;
                    ReturnModel.MembershipId = user.Membership.MembershipId;
                    ReturnModel._failure = false;
                    ReturnModel._message = "";

                    if (user.LastUsedSchoolId != null && user.LastUsedSchoolId !=0)
                    {
                        objModel.SchoolId = user.LastUsedSchoolId;
                    }
                    else
                    {
                        objModel.SchoolId = user.SchoolId;
                    }
                    ReturnModel.SchoolId = objModel.SchoolId;

                    correctEmailList.FirstOrDefault().LoginFailureCount = 0;
                    correctEmailList.FirstOrDefault().LoginAttemptDate = DateTime.UtcNow;
                }
            }

            catch (Exception ex)
            {
                ReturnModel._failure = true;
                ReturnModel._message = ex.Message;
            }

            this.context?.SaveChanges();
            return ReturnModel;
        }

        /// <summary>
        /// Check User Login Email
        /// </summary>
        /// <param name="checkUserEmailAddressViewModel"></param>
        /// <returns></returns>
        public CheckUserEmailAddressViewModel CheckUserLoginEmail(CheckUserEmailAddressViewModel checkUserEmailAddressViewModel)
        {
            var checkEmailAddress = this.context?.UserMaster.Where(x => x.TenantId == checkUserEmailAddressViewModel.TenantId && x.EmailAddress == checkUserEmailAddressViewModel.EmailAddress).ToList();
            if (checkEmailAddress.Count() > 0)
            {
                checkUserEmailAddressViewModel.IsValidEmailAddress = false;
                checkUserEmailAddressViewModel._message = "User Login Email Address Already Exist";
            }
            else
            {
                checkUserEmailAddressViewModel.IsValidEmailAddress = true;
                checkUserEmailAddressViewModel._message = "User Login Email Address Is Valid";
            }
            return checkUserEmailAddressViewModel;
        }

        /// <summary>
        /// LogOut For User
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public LoginViewModel LogOutForUser(LoginViewModel loginViewModel)
        {
            try
            {
                //var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == loginViewModel.TenantId && x.EmailAddress == loginViewModel.Email);
                //if (userMasterData != null)
                //{
                var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == loginViewModel.TenantId && x.EmailAddress == loginViewModel.Email && x.IsExpired == false);

                if (loginSessionData != null)
                {
                    //loginSessionData.ForEach(x => x.IsExpired = true);
                    loginSessionData.IsExpired = true;
                    this.context.SaveChanges();
                }
                loginViewModel._failure = false;
                loginViewModel._message = "User LogOut Successfully";
                // }
                //else
                //{
                //    loginViewModel._failure = true;
                //    loginViewModel._message = "User Login Email Is Not Valid";
                //}
            }
            catch (Exception ex)
            {
                loginViewModel._failure = true;
                loginViewModel._message = ex.Message;
            }
            return loginViewModel;
        }

        public bool AddLoginSession(LoginViewModel returnModel)
        {
            try
            {
                if (returnModel != null)
                {
                    int? ide = 1;
                    var loginSession = new LoginSession();
                    var LoginSessionId = this.context?.LoginSession.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (LoginSessionId != null)
                    {
                        ide = LoginSessionId.Id + 1;
                    }
                    var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == returnModel.TenantId && x.EmailAddress == returnModel.Email && x.IsExpired == false);

                    if (loginSessionData != null)
                    {
                        loginSessionData.IsExpired = true;
                    }
                    loginSession.Id = (int)ide;
                    loginSession.TenantId = (Guid)returnModel.TenantId;
                    loginSession.SchoolId = (int)returnModel.SchoolId;
                    loginSession.EmailAddress = returnModel.Email;
                    loginSession.Token = returnModel._token;
                    loginSession.IsExpired = false;
                    loginSession.LoginTime = DateTime.UtcNow;
                    loginSession.CreatedOn = DateTime.UtcNow;
                    loginSession.CreatedBy = returnModel.CreatedBy;
                    this.context?.LoginSession.Add(loginSession);
                    this.context?.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception es)
            {
                return false;
            }
        }

    }
}
