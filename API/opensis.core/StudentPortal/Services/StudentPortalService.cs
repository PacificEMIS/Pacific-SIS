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
using opensis.core.StudentPortal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opensis.data.Interface;
using opensis.data.ViewModels.StudentPortal;
using opensis.data.Models;

namespace opensis.core.StudentPortal.Services
{
    public class StudentPortalService: IStudentPortalService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStudentPortalRepository studentPortalRepository;
        public ICheckLoginSession tokenManager;
        public StudentPortalService(IStudentPortalRepository studentPortalRepository, ICheckLoginSession checkLoginSession)
        {
            this.studentPortalRepository = studentPortalRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// Get Student Dashboard
        /// </summary>
        /// <param name="studentDashboardViewModel"></param>
        /// <returns></returns>
        public StudentDashboardViewModel GetStudentDashboard(StudentDashboardViewModel studentDashboardViewModel)
        {
            StudentDashboardViewModel studentDashboard = new StudentDashboardViewModel();
            try
            {
                if (tokenManager.CheckToken(studentDashboardViewModel._tenantName + studentDashboardViewModel._userName, studentDashboardViewModel._token))
                {
                    studentDashboard = this.studentPortalRepository.GetStudentDashboard(studentDashboardViewModel);
                }
                else
                {
                    studentDashboard._failure = true;
                    studentDashboard._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                studentDashboard._failure = true;
                studentDashboard._message = ex.Message;
            }
            return studentDashboard;
        }

        /// <summary>
        /// Get Student Gradebook Grades
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentGradebookViewModel GetStudentGradebookGrades(PageResult pageResult)
        {
            StudentGradebookViewModel studentGradebook = new StudentGradebookViewModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    studentGradebook = this.studentPortalRepository.GetStudentGradebookGrades(pageResult);
                }
                else
                {
                    studentGradebook._failure = true;
                    studentGradebook._message = TOKENINVALID;
                }
            }
            catch (Exception ex)
            {
                studentGradebook._failure = true;
                studentGradebook._message = ex.Message;
            }
            return studentGradebook;
        }
    }
}
