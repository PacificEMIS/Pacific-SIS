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

using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.core.User.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace opensis.core.User.Services
{
    public class UserService : IUserService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";
        public IUserRepository userRepository;
        public ICheckLoginSession tokenManager;

        public UserService(IUserRepository userRepository, ICheckLoginSession checkLoginSession)
        {
            this.userRepository = userRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// ValidateUserLogin method is used for authentcatred the login process
        /// </summary>
        /// <param name="ObjModel"></param>
        /// <returns></returns>
        public LoginViewModel ValidateUserLogin(LoginViewModel ObjModel)
        {
            logger.Info("Method ValidateLogin called.");
            LoginViewModel ReturnModel = new LoginViewModel();
            try
            {
                ReturnModel = this.userRepository.ValidateUserLogin(ObjModel);
                if (ReturnModel._failure == false)
                {
                    var existToken = tokenManager.CheckTokenInLogin(ReturnModel.Email, ReturnModel.TenantId, ReturnModel._tenantName + ReturnModel.Name);
                    if (existToken != null)
                    {
                        ReturnModel._token = existToken;
                    }
                    else
                    {
                        var tokenInfo = TokenManager.GenerateTokenWithExpiry(ReturnModel._tenantName + ReturnModel.Name + "|" + ReturnModel.Email + "|" + ReturnModel.TenantId);
                        // ReturnModel._token = TokenManager.GenerateToken(ReturnModel._tenantName);
                        ReturnModel._token = tokenInfo.Token;
                        ReturnModel._tokenExpiry = tokenInfo.Expiry;
                        if (this.userRepository.AddLoginSession(ReturnModel))
                        {
                            logger.Info("Method ValidateLogin end with success.");
                        }
                        else
                        {
                            ReturnModel._token = null;
                            logger.Info("Method ValidateLogin end with error");
                            ReturnModel._failure = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReturnModel._failure = true;
                ReturnModel._message = ex.Message;
                logger.Info("Method ValidateLogin end with error :" + ex.Message);
            }
            return ReturnModel;
        }

        /// <summary>
        /// Check User Login Email
        /// </summary>
        /// <param name="checkUserEmailAddressViewModel"></param>
        /// <returns></returns>
        public CheckUserEmailAddressViewModel CheckUserLoginEmail(CheckUserEmailAddressViewModel checkUserEmailAddressViewModel)
        {
            CheckUserEmailAddressViewModel checkUserLoginEmail = new CheckUserEmailAddressViewModel();
            
            if (tokenManager.CheckToken(checkUserEmailAddressViewModel._tenantName + checkUserEmailAddressViewModel._userName, checkUserEmailAddressViewModel._token))
            {
                checkUserLoginEmail = this.userRepository.CheckUserLoginEmail(checkUserEmailAddressViewModel);
            }
            else
            {
                checkUserLoginEmail._failure = true;
                checkUserLoginEmail._message = TOKENINVALID;
            }
            return checkUserLoginEmail;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="ObjModel"></param>
        /// <returns></returns>
        public LoginViewModel RefreshToken(LoginViewModel ObjModel)
        {
            logger.Info("Method ValidateLogin called.");
            LoginViewModel ReturnModel = new LoginViewModel();
            try
            {
                //var tokenInfo = TokenManager.GenerateTokenWithExpiry(ReturnModel._tenantName + ReturnModel.Name);
                // ReturnModel._token = TokenManager.RefreshToken(ObjModel._token, ObjModel._tenantName + ObjModel._userName);

                ReturnModel._token = tokenManager.RefreshToken(ObjModel._token, ObjModel._tenantName + ObjModel._userName);

                //ReturnModel._token = tokenInfo.Token;
                //ReturnModel._tokenExpiry = tokenInfo.Expiry;
                logger.Info("Method RefreshToken end with success.");

            }
            catch (Exception ex)
            {
                ReturnModel._failure = true;
                ReturnModel._message = ex.Message;
                logger.Info("Method RefreshToken end with error :" + ex.Message);
            }

            return ReturnModel;
        }

        public LoginViewModel LogOutForUser(LoginViewModel loginViewModel)
        {
            LoginViewModel loginView = new LoginViewModel();
            /*if (tokenManager.CheckToken(loginViewModel._tenantName + loginViewModel._userName, loginViewModel._token))
            {
                loginView = this.userRepository.LogOutForUser(loginViewModel);
            }
            else
            {
                loginView._failure = true;
                loginView._message = TOKENINVALID;
            }*/
            loginView = this.userRepository.LogOutForUser(loginViewModel);
            return loginView;
        }

        public UserAccessLogListViewModel GetAllUserAccessLog(PageResult pageResult)
        {
            UserAccessLogListViewModel userAccessLogList = new UserAccessLogListViewModel();

            if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
            {
                userAccessLogList = this.userRepository.GetAllUserAccessLog(pageResult);
            }
            else
            {
                userAccessLogList._failure = true;
                userAccessLogList._message = TOKENINVALID;
            }
            return userAccessLogList;
        }

        public UserAccessLogListViewModel DeleteUserAccessLogs(UserAccessLogListViewModel userAccessLogListViewModel)
        {
            UserAccessLogListViewModel userAccessLogListDelete = new UserAccessLogListViewModel();

            if (tokenManager.CheckToken(userAccessLogListViewModel._tenantName + userAccessLogListViewModel._userName, userAccessLogListViewModel._token))
            {
                userAccessLogListDelete = this.userRepository.DeleteUserAccessLogs(userAccessLogListViewModel);
            }
            else
            {
                userAccessLogListDelete._failure = true;
                userAccessLogListDelete._message = TOKENINVALID;
            }
            return userAccessLogListDelete;
        }
    }
}
