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
using opensis.core.School.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels;
using opensis.data.ViewModels.SuperAdministrator;
using System;
using System.Linq;

namespace opensis.core.School.Services
{
    /// <summary>
    /// Token gate for the Super Administrator management endpoints. Beyond
    /// the usual session check, every call stamps the caller's email from
    /// the validated token onto the request so the repository can verify
    /// the caller's own membership without trusting the request body.
    /// </summary>
    public class SuperAdministratorService : ISuperAdministratorService
    {
        private static readonly string TOKENINVALID = "Token not Valid";
        public ISuperAdministratorRepository superAdministratorRepository;
        public ICheckLoginSession tokenManager;

        public SuperAdministratorService(ISuperAdministratorRepository superAdministratorRepository, ICheckLoginSession checkLoginSession)
        {
            this.superAdministratorRepository = superAdministratorRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// The email carried in the token, or null when the token is not
        /// valid for this session. Tokens are minted as "name|email|tenant".
        /// </summary>
        private string? CallerEmailFromToken(CommonFields model)
        {
            if (!tokenManager.CheckToken(model._tenantName + model._userName, model._token))
            {
                return null;
            }
            var claim = TokenManager.ValidateToken(model._token!);
            var parts = (claim ?? "").Split('|');
            return parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1] : null;
        }

        private static T Reject<T>(T model) where T : CommonFields
        {
            model._failure = true;
            model._message = TOKENINVALID;
            return model;
        }

        public SuperAdministratorListViewModel GetSuperAdministrators(SuperAdministratorListViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(new SuperAdministratorListViewModel { _tenantName = model._tenantName, _token = model._token, _userName = model._userName });
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.GetSuperAdministrators(model);
        }

        public SuperAdministratorAddViewModel AddSuperAdministrator(SuperAdministratorAddViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                model.PasswordHash = null;
                return Reject(model);
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.AddSuperAdministrator(model);
        }

        public SuperAdministratorActionViewModel SetActiveStatus(SuperAdministratorActionViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(model);
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.SetActiveStatus(model);
        }

        public SuperAdministratorActionViewModel DeleteSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(model);
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.DeleteSuperAdministrator(model);
        }

        public SuperAdministratorCandidateListViewModel GetPromotionCandidates(SuperAdministratorCandidateListViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(new SuperAdministratorCandidateListViewModel { _tenantName = model._tenantName, _token = model._token, _userName = model._userName });
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.GetPromotionCandidates(model);
        }

        public SuperAdministratorActionViewModel PromoteToSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(model);
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.PromoteToSuperAdministrator(model);
        }

        public SuperAdministratorActionViewModel DemoteSuperAdministrator(SuperAdministratorActionViewModel model)
        {
            var caller = CallerEmailFromToken(model);
            if (caller == null)
            {
                return Reject(model);
            }
            model.CallerEmail = caller;
            return superAdministratorRepository.DemoteSuperAdministrator(model);
        }
    }
}
