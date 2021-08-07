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
using opensis.core.StaffPortalGradebook.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.StaffPortalGradebook;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StaffPortalGradebook.Services
{
    public class StaffPortalGradebookServices: IStaffPortalGradebookServices
    {
        private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStaffPortalGradebookRepository staffPortalGradebookRepository;
        public ICheckLoginSession tokenManager;
        public StaffPortalGradebookServices(IStaffPortalGradebookRepository staffPortalGradebookRepository, ICheckLoginSession checkLoginSession)
        {
            this.staffPortalGradebookRepository = staffPortalGradebookRepository;
            this.tokenManager = checkLoginSession;
        }
        public StaffPortalGradebookServices() { }

        /// <summary>
        /// Add Update Gradebook Configuration
        /// </summary>
        /// <param name="gradebookConfigurationAddViewModel"></param>
        /// <returns></returns>
        public GradebookConfigurationAddViewModel AddUpdateGradebookConfiguration(GradebookConfigurationAddViewModel gradebookConfigurationAddViewModel)
        {
            GradebookConfigurationAddViewModel gradebookConfigurationAdd = new GradebookConfigurationAddViewModel();
            try
            {
                if (tokenManager.CheckToken(gradebookConfigurationAddViewModel._tenantName + gradebookConfigurationAddViewModel._userName, gradebookConfigurationAddViewModel._token))
                {
                    gradebookConfigurationAdd = this.staffPortalGradebookRepository.AddUpdateGradebookConfiguration(gradebookConfigurationAddViewModel);
                }
                else
                {
                    gradebookConfigurationAdd._failure = true;
                    gradebookConfigurationAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradebookConfigurationAdd._failure = true;
                gradebookConfigurationAdd._message = es.Message;
            }
            return gradebookConfigurationAdd;
        }

        /// <summary>
        /// View Gradebook Configuration
        /// </summary>
        /// <param name="gradebookConfigurationAddViewModel"></param>
        /// <returns></returns>
        public GradebookConfigurationAddViewModel ViewGradebookConfiguration(GradebookConfigurationAddViewModel gradebookConfigurationAddViewModel)
        {
            GradebookConfigurationAddViewModel gradebookConfigurationView = new GradebookConfigurationAddViewModel();
            try
            {
                if (tokenManager.CheckToken(gradebookConfigurationAddViewModel._tenantName + gradebookConfigurationAddViewModel._userName, gradebookConfigurationAddViewModel._token))
                {
                    gradebookConfigurationView = this.staffPortalGradebookRepository.ViewGradebookConfiguration(gradebookConfigurationAddViewModel);
                }
                else
                {
                    gradebookConfigurationView._failure = true;
                    gradebookConfigurationView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradebookConfigurationView._failure = true;
                gradebookConfigurationView._message = es.Message;
            }
            return gradebookConfigurationView;
        }

        /// <summary>
        /// Populate Final Grading
        /// </summary>
        /// <param name="finalGradingMarkingPeriodList"></param>
        /// <returns></returns>
        public FinalGradingMarkingPeriodList PopulateFinalGrading(FinalGradingMarkingPeriodList finalGradingMarkingPeriodList)
        {
            FinalGradingMarkingPeriodList finalGradingMarkingPeriod = new FinalGradingMarkingPeriodList();
            try
            {
                if (tokenManager.CheckToken(finalGradingMarkingPeriodList._tenantName + finalGradingMarkingPeriodList._userName, finalGradingMarkingPeriodList._token))
                {
                    finalGradingMarkingPeriod = this.staffPortalGradebookRepository.PopulateFinalGrading(finalGradingMarkingPeriodList);
                }
                else
                {
                    finalGradingMarkingPeriod._failure = true;
                    finalGradingMarkingPeriod._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                finalGradingMarkingPeriod._failure = true;
                finalGradingMarkingPeriod._message = es.Message;
            }
            return finalGradingMarkingPeriod;
        }
    }
}
