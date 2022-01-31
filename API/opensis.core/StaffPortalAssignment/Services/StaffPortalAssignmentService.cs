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
using opensis.core.StaffPortalAssignment.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.StaffPortalAssignment;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StaffPortalAssignment.Services
{
    public class StaffPortalAssignmentService : IStaffPortalAssignmentService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStaffPortalAssignmentRepository staffPortalAssignmentRepository;
        public ICheckLoginSession tokenManager;
        public StaffPortalAssignmentService(IStaffPortalAssignmentRepository staffPortalAssignmentRepository, ICheckLoginSession checkLoginSession)
        {
            this.staffPortalAssignmentRepository = staffPortalAssignmentRepository;
            this.tokenManager = checkLoginSession;
        }
        public StaffPortalAssignmentService() { }

        /// <summary>
        /// Add Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel AddAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel aassignmentTypeAdd = new AssignmentTypeAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentTypeAddViewModel._tenantName + assignmentTypeAddViewModel._userName, assignmentTypeAddViewModel._token))
                {
                    aassignmentTypeAdd = this.staffPortalAssignmentRepository.AddAssignmentType(assignmentTypeAddViewModel);
                }
                else
                {
                    aassignmentTypeAdd._failure = true;
                    aassignmentTypeAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentTypeAdd._failure = true;
                aassignmentTypeAdd._message = es.Message;
            }
            return aassignmentTypeAdd;
        }

        /// <summary>
        /// Update Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel UpdateAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel aassignmentTypeUpdate = new AssignmentTypeAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentTypeAddViewModel._tenantName + assignmentTypeAddViewModel._userName, assignmentTypeAddViewModel._token))
                {
                    aassignmentTypeUpdate = this.staffPortalAssignmentRepository.UpdateAssignmentType(assignmentTypeAddViewModel);
                }
                else
                {
                    aassignmentTypeUpdate._failure = true;
                    aassignmentTypeUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentTypeUpdate._failure = true;
                aassignmentTypeUpdate._message = es.Message;
            }
            return aassignmentTypeUpdate;
        }

        /// <summary>
        /// Delete Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel DeleteAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            AssignmentTypeAddViewModel assignmentTypeDelete = new AssignmentTypeAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentTypeAddViewModel._tenantName + assignmentTypeAddViewModel._userName, assignmentTypeAddViewModel._token))
                {
                    assignmentTypeDelete = this.staffPortalAssignmentRepository.DeleteAssignmentType(assignmentTypeAddViewModel);
                }
                else
                {
                    assignmentTypeDelete._failure = true;
                    assignmentTypeDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                assignmentTypeDelete._failure = true;
                assignmentTypeDelete._message = es.Message;
            }
            return assignmentTypeDelete;
        }

        /// <summary>
        /// Add Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel AddAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel aassignmentAdd = new AssignmentAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentAddViewModel._tenantName + assignmentAddViewModel._userName, assignmentAddViewModel._token))
                {
                    aassignmentAdd = this.staffPortalAssignmentRepository.AddAssignment(assignmentAddViewModel);
                }
                else
                {
                    aassignmentAdd._failure = true;
                    aassignmentAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentAdd._failure = true;
                aassignmentAdd._message = es.Message;
            }
            return aassignmentAdd;
        }

        /// <summary>
        /// Update Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel UpdateAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel aassignmentUpdate = new AssignmentAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentAddViewModel._tenantName + assignmentAddViewModel._userName, assignmentAddViewModel._token))
                {
                    aassignmentUpdate = this.staffPortalAssignmentRepository.UpdateAssignment(assignmentAddViewModel);
                }
                else
                {
                    aassignmentUpdate._failure = true;
                    aassignmentUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentUpdate._failure = true;
                aassignmentUpdate._message = es.Message;
            }
            return aassignmentUpdate;
        }

        /// <summary>
        /// Delete Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel DeleteAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel aassignmentDelete = new AssignmentAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentAddViewModel._tenantName + assignmentAddViewModel._userName, assignmentAddViewModel._token))
                {
                    aassignmentDelete = this.staffPortalAssignmentRepository.DeleteAssignment(assignmentAddViewModel);
                }
                else
                {
                    aassignmentDelete._failure = true;
                    aassignmentDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentDelete._failure = true;
                aassignmentDelete._message = es.Message;
            }
            return aassignmentDelete;
        }

        /// <summary>
        /// Get All Assignment Type
        /// </summary>
        /// <param name="assignmentTypeListViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeListViewModel GetAllAssignmentType(AssignmentTypeListViewModel assignmentTypeListViewModel)
        {
            AssignmentTypeListViewModel aassignmentListView = new AssignmentTypeListViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentTypeListViewModel._tenantName + assignmentTypeListViewModel._userName, assignmentTypeListViewModel._token))
                {
                    aassignmentListView = this.staffPortalAssignmentRepository.GetAllAssignmentType(assignmentTypeListViewModel);
                }
                else
                {
                    aassignmentListView._failure = true;
                    aassignmentListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentListView._failure = true;
                aassignmentListView._message = es.Message;
            }
            return aassignmentListView;
        }

        public AssignmentAddViewModel CopyAssignmentForCourseSection(AssignmentAddViewModel assignmentAddViewModel)
        {
            AssignmentAddViewModel aassignmentCopy = new AssignmentAddViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentAddViewModel._tenantName + assignmentAddViewModel._userName, assignmentAddViewModel._token))
                {
                    aassignmentCopy = this.staffPortalAssignmentRepository.CopyAssignmentForCourseSection(assignmentAddViewModel);
                }
                else
                {
                    aassignmentCopy._failure = true;
                    aassignmentCopy._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                aassignmentCopy._failure = true;
                aassignmentCopy._message = es.Message;
            }
            return aassignmentCopy;
        }
    }
}
