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
using opensis.core.StudentAttendances.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using opensis.data.ViewModels.StudentAttendances;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StudentAttendances.Services
{
    public class StudentAttendanceService : IStudentAttendanceService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStudentAttendanceRepository studentAttendanceRepository;
        public ICheckLoginSession tokenManager;
        public StudentAttendanceService(IStudentAttendanceRepository studentAttendanceRepository, ICheckLoginSession checkLoginSession)
        {
            this.studentAttendanceRepository = studentAttendanceRepository;
            this.tokenManager = checkLoginSession;
        }
        public StudentAttendanceService() { }

        /// <summary>
        /// Student Attendance Add/Update
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel SaveStudentAttendance(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            StudentAttendanceAddViewModel studentAttendanceAdd = new StudentAttendanceAddViewModel();
            if (tokenManager.CheckToken(studentAttendanceAddViewModel._tenantName + studentAttendanceAddViewModel._userName, studentAttendanceAddViewModel._token))
            {
                studentAttendanceAdd = this.studentAttendanceRepository.AddUpdateStudentAttendance(studentAttendanceAddViewModel);
            }
            else
            {
                studentAttendanceAdd._failure = true;
                studentAttendanceAdd._message = TOKENINVALID;
            }
            return studentAttendanceAdd;
        }
        
        /// <summary>
        /// Get All Student Attendance List
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel GetAllStudentAttendanceList(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            StudentAttendanceAddViewModel studentAttendanceView = new StudentAttendanceAddViewModel();

            if (tokenManager.CheckToken(studentAttendanceAddViewModel._tenantName + studentAttendanceAddViewModel._userName, studentAttendanceAddViewModel._token))
            {
                studentAttendanceView = this.studentAttendanceRepository.GetAllStudentAttendanceList(studentAttendanceAddViewModel);
            }
            else
            {
                studentAttendanceView._failure = true;
                studentAttendanceView._message = TOKENINVALID;
            }
            return studentAttendanceView;
        }

        /// <summary>
        /// Search Course Section For Student Attendance
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel SearchCourseSectionForStudentAttendance(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();

            if (tokenManager.CheckToken(scheduledCourseSectionViewModel._tenantName + scheduledCourseSectionViewModel._userName, scheduledCourseSectionViewModel._token))
            {
                scheduledCourseSectionView = this.studentAttendanceRepository.SearchCourseSectionForStudentAttendance(scheduledCourseSectionViewModel);
            }
            else
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = TOKENINVALID;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Add/Update Student Attendance For Student360
        /// </summary>
        /// <param name="studentAttendanceAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel AddUpdateStudentAttendanceForStudent360(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            StudentAttendanceAddViewModel studentAttendanceAdd = new StudentAttendanceAddViewModel();
            try
            {               
                if (tokenManager.CheckToken(studentAttendanceAddViewModel._tenantName + studentAttendanceAddViewModel._userName, studentAttendanceAddViewModel._token))
                {
                    studentAttendanceAdd = this.studentAttendanceRepository.AddUpdateStudentAttendanceForStudent360(studentAttendanceAddViewModel);
                }
                else
                {
                    studentAttendanceAdd._failure = true;
                    studentAttendanceAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentAttendanceAdd._failure = true;
                studentAttendanceAdd._message = es.Message;
            }
            return studentAttendanceAdd;
        }

        /// <summary>
        /// Staff List For Missing Attendance
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel StaffListForMissingAttendance(PageResult pageResult)
        {
            StaffListModel staffListView = new StaffListModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    staffListView = this.studentAttendanceRepository.StaffListForMissingAttendance(pageResult);
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                staffListView._failure = true;
                staffListView._message = es.Message;
            }
            return staffListView;
        }

        /// <summary>
        /// Missing Attendance List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel MissingAttendanceList(PageResult pageResult)
        {
            ScheduledCourseSectionViewModel missingAttendanceView = new ScheduledCourseSectionViewModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    missingAttendanceView = this.studentAttendanceRepository.MissingAttendanceList(pageResult);
                }
                else
                {
                    missingAttendanceView._failure = true;
                    missingAttendanceView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                missingAttendanceView._failure = true;
                missingAttendanceView._message = es.Message;
            }
            return missingAttendanceView;
        }

        /// <summary>
        /// Get All Student Attendance List For Administration
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentAttendanceListViewModel GetAllStudentAttendanceListForAdministration(PageResult pageResult)
        {
            StudentAttendanceListViewModel studentAttendanceList = new StudentAttendanceListViewModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    studentAttendanceList = this.studentAttendanceRepository.GetAllStudentAttendanceListForAdministration(pageResult);
                }
                else
                {
                    studentAttendanceList._failure = true;
                    studentAttendanceList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentAttendanceList._failure = true;
                studentAttendanceList._message = es.Message;
            }
            return studentAttendanceList;
        }

        /// <summary>
        ///  Course Section List For Attendance Administration
        /// </summary>
        /// <param name="courseSectionForAttendanceViewModel"></param>
        /// <returns></returns>
        public CourseSectionForAttendanceViewModel CourseSectionListForAttendanceAdministration(CourseSectionForAttendanceViewModel courseSectionForAttendanceViewModel)
        {
            CourseSectionForAttendanceViewModel courseSectionList = new CourseSectionForAttendanceViewModel();
            try
            {
                if (tokenManager.CheckToken(courseSectionForAttendanceViewModel._tenantName + courseSectionForAttendanceViewModel._userName, courseSectionForAttendanceViewModel._token))
                {
                    courseSectionList = this.studentAttendanceRepository.CourseSectionListForAttendanceAdministration(courseSectionForAttendanceViewModel);
                }
                else
                {
                    courseSectionList._failure = true;
                    courseSectionList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseSectionList._failure = true;
                courseSectionList._message = es.Message;
            }
            return courseSectionList;
        }

        /// <summary>
        /// Add Absences
        /// </summary>
        /// <param name="studentAttendanceListViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceAddViewModel AddAbsences(StudentAttendanceAddViewModel studentAttendanceAddViewModel)
        {
            StudentAttendanceAddViewModel studentAttendanceList = new StudentAttendanceAddViewModel();
            try
            {
                if (tokenManager.CheckToken(studentAttendanceAddViewModel._tenantName + studentAttendanceAddViewModel._userName, studentAttendanceAddViewModel._token))
                {
                    studentAttendanceList = this.studentAttendanceRepository.AddAbsences(studentAttendanceAddViewModel);
                }
                else
                {
                    studentAttendanceList._failure = true;
                    studentAttendanceList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentAttendanceList._failure = true;
                studentAttendanceList._message = es.Message;
            }
            return studentAttendanceList;
        }

        /// <summary>
        /// Update Student Daily Attendance
        /// </summary>
        /// <param name="studentDailyAttendanceListViewModel"></param>
        /// <returns></returns>
        public StudentDailyAttendanceListViewModel UpdateStudentDailyAttendance(StudentDailyAttendanceListViewModel studentDailyAttendanceListViewModel)
        {
            StudentDailyAttendanceListViewModel studentDailyAttendanceList = new StudentDailyAttendanceListViewModel();
            try
            {
                if (tokenManager.CheckToken(studentDailyAttendanceListViewModel._tenantName + studentDailyAttendanceListViewModel._userName, studentDailyAttendanceListViewModel._token))
                {
                    studentDailyAttendanceList = this.studentAttendanceRepository.UpdateStudentDailyAttendance(studentDailyAttendanceListViewModel);
                }
                else
                {
                    studentDailyAttendanceList._failure = true;
                    studentDailyAttendanceList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentDailyAttendanceList._failure = true;
                studentDailyAttendanceList._message = es.Message;
            }
            return studentDailyAttendanceList;
        }

        /// <summary>
        /// Add/Update Student Attendance Comments
        /// </summary>
        /// <param name="studentAttendanceCommentsAddViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceCommentsAddViewModel AddUpdateStudentAttendanceComments(StudentAttendanceCommentsAddViewModel studentAttendanceCommentsAddViewModel)
        {
            StudentAttendanceCommentsAddViewModel StudentAttendanceCommentsAddUpdate = new StudentAttendanceCommentsAddViewModel();
            try
            {
                if (tokenManager.CheckToken(studentAttendanceCommentsAddViewModel._tenantName + studentAttendanceCommentsAddViewModel._userName, studentAttendanceCommentsAddViewModel._token))
                {
                    StudentAttendanceCommentsAddUpdate = this.studentAttendanceRepository.AddUpdateStudentAttendanceComments(studentAttendanceCommentsAddViewModel);
                }
                else
                {
                    StudentAttendanceCommentsAddUpdate._failure = true;
                    StudentAttendanceCommentsAddUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                StudentAttendanceCommentsAddUpdate._failure = true;
                StudentAttendanceCommentsAddUpdate._message = es.Message;
            }
            return StudentAttendanceCommentsAddUpdate;
        }

        /// <summary>
        /// Re-Calculate Daily Attendance
        /// </summary>
        /// <param name="reCalculateDailyAttendanceViewModel"></param>
        /// <returns></returns>
        public ReCalculateDailyAttendanceViewModel ReCalculateDailyAttendance(ReCalculateDailyAttendanceViewModel reCalculateDailyAttendanceViewModel)
        {
            ReCalculateDailyAttendanceViewModel reCalculateDailyAttendance = new ReCalculateDailyAttendanceViewModel();
            try
            {
                if (tokenManager.CheckToken(reCalculateDailyAttendanceViewModel._tenantName + reCalculateDailyAttendanceViewModel._userName, reCalculateDailyAttendanceViewModel._token))
                {
                    reCalculateDailyAttendance = this.studentAttendanceRepository.ReCalculateDailyAttendance(reCalculateDailyAttendanceViewModel);
                }
                else
                {
                    reCalculateDailyAttendance._failure = true;
                    reCalculateDailyAttendance._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                reCalculateDailyAttendance._failure = true;
                reCalculateDailyAttendance._message = es.Message;
            }
            return reCalculateDailyAttendance;
        }

        /// <summary>
        /// Get Student Attendance History
        /// </summary>
        /// <param name="studentAttendanceHistoryViewModel"></param>
        /// <returns></returns>
        public StudentAttendanceHistoryViewModel GetStudentAttendanceHistory(StudentAttendanceHistoryViewModel studentAttendanceHistoryViewModel)
        {
            StudentAttendanceHistoryViewModel studentAttendanceHistory = new StudentAttendanceHistoryViewModel();
            try
            {
                if (tokenManager.CheckToken(studentAttendanceHistoryViewModel._tenantName + studentAttendanceHistoryViewModel._userName, studentAttendanceHistoryViewModel._token))
                {
                    studentAttendanceHistory = this.studentAttendanceRepository.GetStudentAttendanceHistory(studentAttendanceHistoryViewModel);
                }
                else
                {
                    studentAttendanceHistory._failure = true;
                    studentAttendanceHistory._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentAttendanceHistory._failure = true;
                studentAttendanceHistory._message = es.Message;
            }
            return studentAttendanceHistory;
        }
    }
}
