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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using opensis.core.StudentPortal.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.StudentPortal;
using System;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/StudentPortal")]
    [ApiController]
    public class StudentPortalController : ControllerBase
    {
        private IStudentPortalService _studentPortalService;
        public StudentPortalController(IStudentPortalService studentPortalService)
        {
            _studentPortalService = studentPortalService;
        }
        [HttpPost("getStudentDashboard")]
        public ActionResult<ScheduledCourseSectionViewModelForStudent> GetStudentDashboard(ScheduledCourseSectionViewModelForStudent scheduledCourseSectionViewModelForStudent)
        {
            ScheduledCourseSectionViewModelForStudent scheduledCourseSectionViewForStudent = scheduledCourseSectionViewModelForStudent;
            try
            {
                scheduledCourseSectionViewModelForStudent = _studentPortalService.GetStudentDashboard(scheduledCourseSectionViewModelForStudent);
            }
            catch (Exception ex)
            {
                scheduledCourseSectionViewModelForStudent._message = ex.Message;
                scheduledCourseSectionViewModelForStudent._failure = true;
            }
            return scheduledCourseSectionViewModelForStudent;
        }
        [HttpPost("getStudentGradebookGrades")]
        public ActionResult<StudentGradebookViewModel> GetStudentGradebookGrades(PageResult pageResult)
        {
            StudentGradebookViewModel studentGradebook = new StudentGradebookViewModel();
            try
            {
                studentGradebook = _studentPortalService.GetStudentGradebookGrades(pageResult);
            }
            catch (Exception ex)
            {
                studentGradebook._message = ex.Message;
                studentGradebook._failure = true;
            }
            return studentGradebook;
        }
    }
}
