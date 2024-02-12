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
using opensis.core.helper.Interfaces;
using opensis.core.ParentPortal.Interface;
using opensis.data.Interface;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.core.ParentPortal.Services
{
    public class ParentPortalService:IParentPortalService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IParentPortalRepository parentPortalRepository;
        public ICheckLoginSession tokenManager;
        public ParentPortalService(IParentPortalRepository parentPortalRepository, ICheckLoginSession checkLoginSession)
        {
            this.parentPortalRepository = parentPortalRepository;
            this.tokenManager = checkLoginSession;
        }


        /// <summary>
        /// Get Parent Dashboard
        /// </summary>
        /// <param name="parentDashboardViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel GetStudentListForParent(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            ParentInfoAddViewModel parentInfoViewModel = new ParentInfoAddViewModel();
            try
            {
                if (tokenManager.CheckToken(parentInfoAddViewModel._tenantName + parentInfoAddViewModel._userName, parentInfoAddViewModel._token))
                {
                    parentInfoViewModel = this.parentPortalRepository.GetStudentListForParent(parentInfoAddViewModel);
                }
                else
                {
                    parentInfoViewModel._failure = true;
                    parentInfoViewModel._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                parentInfoViewModel._failure = true;
                parentInfoViewModel._message = ex.Message;
            }
            return parentInfoViewModel;
        }
        public SchoolListModel GetAllSchoolsByStudentId(SchoolListModel school)
        {
            SchoolListModel schoolListModel = new SchoolListModel();
            try
            {
                if (tokenManager.CheckToken(school._tenantName + school._userName, school._token))
                {
                    schoolListModel = this.parentPortalRepository.GetAllSchoolsByStudentId(school);
                }
                else
                {
                    schoolListModel._failure = true;
                    schoolListModel._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                schoolListModel._failure = true;
                schoolListModel._message = ex.Message;
            }
            return schoolListModel;
        }
    }
}
