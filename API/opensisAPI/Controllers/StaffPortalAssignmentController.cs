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
using opensis.core.StaffPortalAssignment.Interfaces;
using opensis.data.ViewModels.StaffPortalAssignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/StaffPortalAssignment")]
    [ApiController]
    public class StaffPortalAssignmentController : ControllerBase
    {
        private IStaffPortalAssignmentService _staffPortalAssignmentService;
        public StaffPortalAssignmentController(IStaffPortalAssignmentService staffPortalAssignmentService)
        {
            _staffPortalAssignmentService = staffPortalAssignmentService;
        }

        [HttpPost("addAssignmentType")]
        public ActionResult<AssignmentTypeAddViewModel> AddAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel aassignmentTypeAdd = new AssignmentTypeAddViewModel();
            try
            {
                aassignmentTypeAdd = _staffPortalAssignmentService.AddAssignmentType(assignmentTypeAddViewModel);
            }
            catch (Exception ex)
            {

                aassignmentTypeAdd._message = ex.Message;
                aassignmentTypeAdd._failure = true;
            }
            return aassignmentTypeAdd;
            ;
        }

        [HttpPut("updateAssignmentType")]
        public ActionResult<AssignmentTypeAddViewModel> UpdateAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel aassignmentTypeUpdate = new AssignmentTypeAddViewModel();
            try
            {
                aassignmentTypeUpdate = _staffPortalAssignmentService.UpdateAssignmentType(assignmentTypeAddViewModel);
            }
            catch (Exception ex)
            {

                aassignmentTypeUpdate._message = ex.Message;
                aassignmentTypeUpdate._failure = true;
            }
            return aassignmentTypeUpdate;
        }

        [HttpPost("deleteAssignmentType")]
        public ActionResult<AssignmentTypeAddViewModel> DeleteAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel aassignmentTypeDelete = new AssignmentTypeAddViewModel();
            try
            {
                aassignmentTypeDelete = _staffPortalAssignmentService.DeleteAssignmentType(assignmentTypeAddViewModel);
            }
            catch (Exception ex)
            {

                aassignmentTypeDelete._message = ex.Message;
                aassignmentTypeDelete._failure = true;
            }
            return aassignmentTypeDelete;
        }

        [HttpPost("addAssignment")]
        public ActionResult<AssignmentAddViewModel> AddAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel assignmentAdd = new AssignmentAddViewModel();
            try
            {
                assignmentAdd = _staffPortalAssignmentService.AddAssignment(assignmentAddViewModel);
            }
            catch (Exception ex)
            {

                assignmentAdd._message = ex.Message;
                assignmentAdd._failure = true;
            }
            return assignmentAdd;
        }

        [HttpPut("updateAssignment")]
        public ActionResult<AssignmentAddViewModel> UpdateAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel assignmentUpdate = new AssignmentAddViewModel();
            try
            {
                assignmentUpdate = _staffPortalAssignmentService.UpdateAssignment(assignmentAddViewModel);
            }
            catch (Exception ex)
            {

                assignmentUpdate._message = ex.Message;
                assignmentUpdate._failure = true;
            }
            return assignmentUpdate;
        }

        [HttpPost("deleteAssignment")]
        public ActionResult<AssignmentAddViewModel> DeleteAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel assignmentDelete = new AssignmentAddViewModel();
            try
            {
                assignmentDelete = _staffPortalAssignmentService.DeleteAssignment(assignmentAddViewModel);
            }
            catch (Exception ex)
            {

                assignmentDelete._message = ex.Message;
                assignmentDelete._failure = true;
            }
            return assignmentDelete;
        }

        [HttpPost("getAllAssignmentType")]
        public ActionResult<AssignmentTypeListViewModel> GetAllAssignment(AssignmentTypeListViewModel assignmentTypeListViewModel)
        {
            AssignmentTypeListViewModel assignmentTypeListView = new AssignmentTypeListViewModel();
            try
            {
                assignmentTypeListView = _staffPortalAssignmentService.GetAllAssignmentType(assignmentTypeListViewModel);
            }
            catch (Exception ex)
            {

                assignmentTypeListView._message = ex.Message;
                assignmentTypeListView._failure = true;
            }
            return assignmentTypeListView;
        }

        [HttpPost("copyAssignmentForCourseSection")]
        public ActionResult<AssignmentAddViewModel> CopyAssignmentForCourseSection(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel assignmentCopy = new AssignmentAddViewModel();
            try
            {
                assignmentCopy = _staffPortalAssignmentService.CopyAssignmentForCourseSection(assignmentAddViewModel);
            }
            catch (Exception ex)
            {

                assignmentCopy._message = ex.Message;
                assignmentCopy._failure = true;
            }
            return assignmentCopy;
        }
    }
}
