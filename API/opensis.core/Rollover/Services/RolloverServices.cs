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
using opensis.core.Rollover.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.Rollover;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.Rollover.Services
{
    public class RolloverServices: IRolloverService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IRolloverRepository rolloverRepository;
        public ICheckLoginSession tokenManager;
        public RolloverServices(IRolloverRepository rolloverRepository, ICheckLoginSession checkLoginSession)
        {
            this.rolloverRepository = rolloverRepository;
            this.tokenManager = checkLoginSession;
        }

        //Required for Unit Testing
        public RolloverServices() { }

      
        public RolloverViewModel Rollover(RolloverViewModel rolloverViewModel)
        {
            RolloverViewModel rollover = new RolloverViewModel();
            try
            {
                if (tokenManager.CheckToken(rolloverViewModel._tenantName + rolloverViewModel._userName, rolloverViewModel._token))
                {
                    rollover = this.rolloverRepository.Rollover(rolloverViewModel);
                }
                else
                {
                    rollover._failure = true;
                    rollover._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                rollover._failure = true;
                rollover._message = es.Message;
            }
            return rollover;
        }
    }
}
