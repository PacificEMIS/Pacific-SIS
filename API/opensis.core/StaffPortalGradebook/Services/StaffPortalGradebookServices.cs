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
        //private static string SUCCESS = "success";
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

        /// <summary>
        /// Add Gradebook Grade
        /// </summary>
        /// <param name="gradebookGradeAddViewModel"></param>
        /// <returns></returns>
        public GradebookGradeListViewModel AddGradebookGrade(GradebookGradeListViewModel gradebookGradeListViewModel)
        {
            GradebookGradeListViewModel gradebookGradeAdd = new GradebookGradeListViewModel();
            try
            {
                if (tokenManager.CheckToken(gradebookGradeListViewModel._tenantName + gradebookGradeListViewModel._userName, gradebookGradeListViewModel._token))
                {
                    gradebookGradeAdd = this.staffPortalGradebookRepository.AddGradebookGrade(gradebookGradeListViewModel);
                }
                else
                {
                    gradebookGradeAdd._failure = true;
                    gradebookGradeAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradebookGradeAdd._failure = true;
                gradebookGradeAdd._message = es.Message;
            }
            return gradebookGradeAdd;
        }

        /// <summary>
        /// Get Gradebook Grade
        /// </summary>
        /// <param name="gradebookGradeListViewModel"></param>
        /// <returns></returns>
        public GradebookGradeListViewModel GetGradebookGrade(GradebookGradeListViewModel gradebookGradeListViewModel)
        {
            GradebookGradeListViewModel gradebookGradeList = new GradebookGradeListViewModel();
            try
            {
                if (tokenManager.CheckToken(gradebookGradeListViewModel._tenantName + gradebookGradeListViewModel._userName, gradebookGradeListViewModel._token))
                {
                    gradebookGradeList = this.staffPortalGradebookRepository.GetGradebookGrade(gradebookGradeListViewModel);
                }
                else
                {
                    gradebookGradeList._failure = true;
                    gradebookGradeList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gradebookGradeList._failure = true;
                gradebookGradeList._message = es.Message;
            }
            return gradebookGradeList;
        }

        /// <summary>
        /// Gradebook Grade By Student
        /// </summary>
        /// <param name="assignmentForStudentViewModel"></param>
        /// <returns></returns>
        public AssignmentForStudentViewModel GradebookGradeByStudent(AssignmentForStudentViewModel assignmentForStudentViewModel)
        {
            AssignmentForStudentViewModel assignmentForStudent = new AssignmentForStudentViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentForStudentViewModel._tenantName + assignmentForStudentViewModel._userName, assignmentForStudentViewModel._token))
                {
                    assignmentForStudent = this.staffPortalGradebookRepository.GradebookGradeByStudent(assignmentForStudentViewModel);
                }
                else
                {
                    assignmentForStudent._failure = true;
                    assignmentForStudent._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                assignmentForStudent._failure = true;
                assignmentForStudent._message = es.Message;
            }
            return assignmentForStudent;
        }

        /// <summary>
        /// Add Gradebook Grade By Student
        /// </summary>
        /// <param name="assignmentForStudentViewModel"></param>
        /// <returns></returns>
        public AssignmentForStudentViewModel AddGradebookGradeByStudent(AssignmentForStudentViewModel assignmentForStudentViewModel)
        {
            AssignmentForStudentViewModel assignmentForStudent = new AssignmentForStudentViewModel();
            try
            {
                if (tokenManager.CheckToken(assignmentForStudentViewModel._tenantName + assignmentForStudentViewModel._userName, assignmentForStudentViewModel._token))
                {
                    assignmentForStudent = this.staffPortalGradebookRepository.AddGradebookGradeByStudent(assignmentForStudentViewModel);
                }
                else
                {
                    assignmentForStudent._failure = true;
                    assignmentForStudent._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                assignmentForStudent._failure = true;
                assignmentForStudent._message = es.Message;
            }
            return assignmentForStudent;
        }

        /// <summary>
        /// Gradebook Grade By Assignment Type
        /// </summary>
        /// <param name="studentListByAssignmentTpyeViewModel"></param>
        /// <returns></returns>
        public StudentListByAssignmentTpyeViewModel GradebookGradeByAssignmentType(StudentListByAssignmentTpyeViewModel studentListByAssignmentTpyeViewModel)
        {
            StudentListByAssignmentTpyeViewModel studentListByAssignmentTpye = new StudentListByAssignmentTpyeViewModel();
            try
            {
                if (tokenManager.CheckToken(studentListByAssignmentTpyeViewModel._tenantName + studentListByAssignmentTpyeViewModel._userName, studentListByAssignmentTpyeViewModel._token))
                {
                    studentListByAssignmentTpye = this.staffPortalGradebookRepository.GradebookGradeByAssignmentType(studentListByAssignmentTpyeViewModel);
                }
                else
                {
                    studentListByAssignmentTpye._failure = true;
                    studentListByAssignmentTpye._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentListByAssignmentTpye._failure = true;
                studentListByAssignmentTpye._message = es.Message;
            }
            return studentListByAssignmentTpye;
        }

        /// <summary>
        /// Add gradebook Grade By Assignment Type
        /// </summary>
        /// <param name="studentListByAssignmentTpyeViewModel"></param>
        /// <returns></returns>
        public StudentListByAssignmentTpyeViewModel AddgradebookGradeByAssignmentType(StudentListByAssignmentTpyeViewModel studentListByAssignmentTpyeViewModel)
        {
            StudentListByAssignmentTpyeViewModel studentListByAssignmentTpye = new StudentListByAssignmentTpyeViewModel();
            try
            {
                if (tokenManager.CheckToken(studentListByAssignmentTpyeViewModel._tenantName + studentListByAssignmentTpyeViewModel._userName, studentListByAssignmentTpyeViewModel._token))
                {
                    studentListByAssignmentTpye = this.staffPortalGradebookRepository.AddgradebookGradeByAssignmentType(studentListByAssignmentTpyeViewModel);
                }
                else
                {
                    studentListByAssignmentTpye._failure = true;
                    studentListByAssignmentTpye._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentListByAssignmentTpye._failure = true;
                studentListByAssignmentTpye._message = es.Message;
            }
            return studentListByAssignmentTpye;
        }
    }
}
