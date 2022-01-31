﻿/***********************************************************************************
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

using System;
using System.Collections.Generic;
using System.Text;
using opensis.core.CourseManager.Interfaces;
using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.CourseManager;

namespace opensis.core.CourseManager.Services
{
    public class CourseManagerService : ICourseManagerService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public ICourseManagerRepository courseManagerRepository;
        public ICheckLoginSession tokenManager;

        public CourseManagerService(ICourseManagerRepository courseManagerRepository, ICheckLoginSession checkLoginSession)
        {
            this.courseManagerRepository = courseManagerRepository;
            this.tokenManager = checkLoginSession;
        }

        //Required for Unit Testing
        public CourseManagerService() { }
        /// <summary>
        /// Add Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        //public ProgramAddViewModel AddProgram(ProgramAddViewModel programAddViewModel)
        //{
        //    ProgramAddViewModel ProgramAddModel = new ProgramAddViewModel();
        //    try
        //    {
        //        if (tokenManager.CheckToken(programAddViewModel._tenantName, programAddViewModel._token))
        //        {
        //            ProgramAddModel = this.courseManagerRepository.AddProgram(programAddViewModel);
        //        }
        //        else
        //        {
        //            ProgramAddModel._failure = true;
        //            ProgramAddModel._message = TOKENINVALID;
        //        }
        //    }
        //    catch (Exception es)
        //    {

        //        ProgramAddModel._failure = true;
        //        ProgramAddModel._message = es.Message;
        //    }
        //    return ProgramAddModel;
        //}
        /// <summary>
        /// Get All Program
        /// </summary>
        /// <param name="programListViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel GetAllProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel ProgramListModel = new ProgramListViewModel();
            try
            {
                if (tokenManager.CheckToken(programListViewModel._tenantName + programListViewModel._userName, programListViewModel._token))
                {
                    ProgramListModel = this.courseManagerRepository.GetAllProgram(programListViewModel);
                }
                else
                {
                    ProgramListModel._failure = true;
                    ProgramListModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                ProgramListModel._failure = true;
                ProgramListModel._message = es.Message;
            }

            return ProgramListModel;
        }

        /// <summary>
        /// Add/Update Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel AddEditProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel ProgramUpdateModel = new ProgramListViewModel();
            try
            {
                if (tokenManager.CheckToken(programListViewModel._tenantName + programListViewModel._userName, programListViewModel._token))
                {
                    ProgramUpdateModel = this.courseManagerRepository.AddEditProgram(programListViewModel);
                }
                else
                {
                    ProgramUpdateModel._failure = true;
                    ProgramUpdateModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {

                ProgramUpdateModel._failure = true;
                ProgramUpdateModel._message = es.Message;
            }
            return ProgramUpdateModel;
        }
        
        /// <summary>
        /// Delete Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramAddViewModel DeleteProgram(ProgramAddViewModel programAddViewModel)
        {
            ProgramAddViewModel programDeleteModel = new ProgramAddViewModel();
            try
            {
                if (tokenManager.CheckToken(programAddViewModel._tenantName + programAddViewModel._userName, programAddViewModel._token))
                {
                    programDeleteModel = this.courseManagerRepository.DeleteProgram(programAddViewModel);
                }
                else
                {
                    programDeleteModel._failure = true;
                    programDeleteModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                programDeleteModel._failure = true;
                programDeleteModel._message = es.Message;
            }

            return programDeleteModel;
        }
        
        /// <summary>
        /// Add Subject
        /// </summary>
        /// <param name="subjectAddViewModel"></param>
        /// <returns></returns>
        //public SubjectAddViewModel AddSubject(SubjectAddViewModel subjectAddViewModel)
        //{
        //    SubjectAddViewModel subjectAdd = new SubjectAddViewModel();

        //    if (tokenManager.CheckToken(subjectAddViewModel._tenantName, subjectAddViewModel._token))
        //    {
        //        subjectAdd = this.courseManagerRepository.AddSubject(subjectAddViewModel);
        //    }
        //    else
        //    {
        //        subjectAdd._failure = true;
        //        subjectAdd._message = TOKENINVALID;
        //    }
        //    return subjectAdd;
        //}

        /// <summary>
        /// Add/Update Subject
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel AddEditSubject(SubjectListViewModel subjectListViewModel)
        {
            SubjectListViewModel subjectAddUpdate = new SubjectListViewModel();
            if (tokenManager.CheckToken(subjectListViewModel._tenantName + subjectListViewModel._userName, subjectListViewModel._token))
            {
                subjectAddUpdate = this.courseManagerRepository.AddEditSubject(subjectListViewModel);
            }
            else
            {
                subjectAddUpdate._failure = true;
                subjectAddUpdate._message = TOKENINVALID;
            }
            return subjectAddUpdate;
        }

        /// <summary>
        /// Get All Subject List
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel GetAllSubjectList(SubjectListViewModel subjectListViewModel)
        {
            SubjectListViewModel subjectList = new SubjectListViewModel();
            if (tokenManager.CheckToken(subjectListViewModel._tenantName + subjectListViewModel._userName, subjectListViewModel._token))
            {
                subjectList = this.courseManagerRepository.GetAllSubjectList(subjectListViewModel);
            }
            else
            {
                subjectList._failure = true;
                subjectList._message = TOKENINVALID;
            }
            return subjectList;
        }

        /// <summary>
        /// Delete Subject
        /// </summary>
        /// <param name="subjectAddViewModel"></param>
        /// <returns></returns>
        public SubjectAddViewModel DeleteSubject(SubjectAddViewModel subjectAddViewModel)
        {
            SubjectAddViewModel subjectDelete = new SubjectAddViewModel();
            if (tokenManager.CheckToken(subjectAddViewModel._tenantName + subjectAddViewModel._userName, subjectAddViewModel._token))
            {
                subjectDelete = this.courseManagerRepository.DeleteSubject(subjectAddViewModel);
            }
            else
            {
                subjectDelete._failure = true;
                subjectDelete._message = TOKENINVALID;
            }
            return subjectDelete;
        }

        /// <summary>
        /// Add Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel AddCourse(CourseAddViewModel courseAddViewModel)
        {
            CourseAddViewModel courseAdd = new CourseAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseAddViewModel._tenantName + courseAddViewModel._userName, courseAddViewModel._token))
                {
                    courseAdd = this.courseManagerRepository.AddCourse(courseAddViewModel);
                }
                else
                {
                    courseAdd._failure = true;
                    courseAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseAdd._failure = true;
                courseAdd._message = es.Message;
            }
            return courseAdd;
        }

        /// <summary>
        /// Update Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel UpdateCourse(CourseAddViewModel courseAddViewModel)
        {
            CourseAddViewModel courseUpdate = new CourseAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseAddViewModel._tenantName + courseAddViewModel._userName, courseAddViewModel._token))
                {
                    courseUpdate = this.courseManagerRepository.UpdateCourse(courseAddViewModel);
                }
                else
                {
                    courseUpdate._failure = true;
                    courseUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseUpdate._failure = true;
                courseUpdate._message = es.Message;
            }
            return courseUpdate;
        }

        /// <summary>
        /// Delete Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel DeleteCourse(CourseAddViewModel courseAddViewModel)
        {
            CourseAddViewModel courseDelete = new CourseAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseAddViewModel._tenantName + courseAddViewModel._userName, courseAddViewModel._token))
                {
                    courseDelete = this.courseManagerRepository.DeleteCourse(courseAddViewModel);
                }
                else
                {
                    courseDelete._failure = true;
                    courseDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseDelete._failure = true;
                courseDelete._message = es.Message;
            }
            return courseDelete;
        }

        /// <summary>
        /// Get All Course List
        /// </summary>
        /// <param name="courseListViewModel"></param>
        /// <returns></returns>
        public CourseListViewModel GetAllCourseList(CourseListViewModel courseListViewModel)
        {
            CourseListViewModel CourseListModel = new CourseListViewModel();
            try
            {
                if (tokenManager.CheckToken(courseListViewModel._tenantName + courseListViewModel._userName, courseListViewModel._token))
                {
                    CourseListModel = this.courseManagerRepository.GetAllCourseList(courseListViewModel);
                }
                else
                {
                    CourseListModel._failure = true;
                    CourseListModel._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                CourseListModel._failure = true;
                CourseListModel._message = es.Message;
            }

            return CourseListModel;
        }
        
        /// <summary>
        /// Add Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel AddCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            CourseSectionAddViewModel courseSectionAdd = new CourseSectionAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseSectionAddViewModel._tenantName + courseSectionAddViewModel._userName, courseSectionAddViewModel._token))
                {
                    courseSectionAdd = this.courseManagerRepository.AddCourseSection(courseSectionAddViewModel);
                }
                else
                {
                    courseSectionAdd._failure = true;
                    courseSectionAdd._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseSectionAdd._failure = true;
                courseSectionAdd._message = es.Message;
            }
            return courseSectionAdd;
        }

        /// <summary>
        /// Get All Course Section
        /// </summary>
        /// <param name="courseSectionViewModel"></param>
        /// <returns></returns>
        public CourseSectionViewModel GetAllCourseSection(CourseSectionViewModel courseSectionViewModel)
        {
            CourseSectionViewModel courseSectionView = new CourseSectionViewModel();
            try
            {
                if (tokenManager.CheckToken(courseSectionViewModel._tenantName + courseSectionViewModel._userName, courseSectionViewModel._token))
                {
                    courseSectionView = this.courseManagerRepository.GetAllCourseSection(courseSectionViewModel);
                }
                else
                {
                    courseSectionView._failure = true;
                    courseSectionView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseSectionView._failure = true;
                courseSectionView._message = es.Message;
            }
            return courseSectionView;
        }

        /// <summary>
        /// Update Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel UpdateCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            CourseSectionAddViewModel courseSectionUpdate = new CourseSectionAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseSectionAddViewModel._tenantName + courseSectionAddViewModel._userName, courseSectionAddViewModel._token))
                {
                    courseSectionUpdate = this.courseManagerRepository.UpdateCourseSection(courseSectionAddViewModel);
                }
                else
                {
                    courseSectionUpdate._failure = true;
                    courseSectionUpdate._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseSectionUpdate._failure = true;
                courseSectionUpdate._message = es.Message;
            }
            return courseSectionUpdate;
        }

        /// <summary>
        /// Delete Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel DeleteCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            CourseSectionAddViewModel courseSectionDelete = new CourseSectionAddViewModel();
            try
            {
                if (tokenManager.CheckToken(courseSectionAddViewModel._tenantName + courseSectionAddViewModel._userName, courseSectionAddViewModel._token))
                {
                    courseSectionDelete = this.courseManagerRepository.DeleteCourseSection(courseSectionAddViewModel);
                }
                else
                {
                    courseSectionDelete._failure = true;
                    courseSectionDelete._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseSectionDelete._failure = true;
                courseSectionDelete._message = es.Message;
            }
            return courseSectionDelete;
        }

        /// <summary>
        /// Get All Course Standard For Course
        /// </summary>
        /// <param name="courseStandardForCourseViewModel"></param>
        /// <returns></returns>
        public CourseStandardForCourseViewModel GetAllCourseStandardForCourse(CourseStandardForCourseViewModel courseStandardForCourseViewModel)
        {
            CourseStandardForCourseViewModel courseStandardForCourseView = new CourseStandardForCourseViewModel();
            try
            {
                if (tokenManager.CheckToken(courseStandardForCourseViewModel._tenantName + courseStandardForCourseViewModel._userName, courseStandardForCourseViewModel._token))
                {
                    courseStandardForCourseView = this.courseManagerRepository.GetAllCourseStandardForCourse(courseStandardForCourseViewModel);
                }
                else
                {
                    courseStandardForCourseView._failure = true;
                    courseStandardForCourseView._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseStandardForCourseView._failure = true;
                courseStandardForCourseView._message = es.Message;
            }
            return courseStandardForCourseView;
        }

        ///// <summary>
        ///// Delete Course Section  For Specific Schedule Type
        ///// </summary>
        ///// <param name="deleteScheduleViewModel"></param>
        ///// <returns></returns>
        //public DeleteScheduleViewModel DeleteSchedule(DeleteScheduleViewModel deleteScheduleViewModel)
        //{
        //    DeleteScheduleViewModel deleteSchedule = new DeleteScheduleViewModel();
        //    try
        //    {
        //        if (tokenManager.CheckToken(deleteScheduleViewModel._tenantName + deleteScheduleViewModel._userName, deleteScheduleViewModel._token))
        //        {
        //            deleteSchedule = this.courseManagerRepository.DeleteSchedule(deleteScheduleViewModel);
        //        }
        //        else
        //        {
        //            deleteSchedule._failure = true;
        //            deleteSchedule._message = TOKENINVALID;
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        deleteSchedule._failure = true;
        //        deleteSchedule._message = es.Message;
        //    }
        //    return deleteSchedule;
        //}

        /// <summary>
        /// Search Course Section For Schedule
        /// </summary>
        /// <param name="searchCourseSectionViewModel"></param>
        /// <returns></returns>
        public SearchCourseSectionViewModel SearchCourseSectionForSchedule(SearchCourseSectionViewModel searchCourseSectionViewModel)
        {
            SearchCourseSectionViewModel searchCourseSection = new SearchCourseSectionViewModel();
            try
            {
                if (tokenManager.CheckToken(searchCourseSectionViewModel._tenantName + searchCourseSectionViewModel._userName, searchCourseSectionViewModel._token))
                {
                    searchCourseSection = this.courseManagerRepository.SearchCourseSectionForSchedule(searchCourseSectionViewModel);
                }
                else
                {
                    searchCourseSection._failure = true;
                    searchCourseSection._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                searchCourseSection._failure = true;
                searchCourseSection._message = es.Message;
            }
            return searchCourseSection;
        }

        /// <summary>
        /// Get All Staff Schedule In Course Section & Course
        /// </summary>
        /// <param name="staffListViewModel"></param>
        /// <returns></returns>
        public StaffListViewModel GetAllStaffScheduleInCourseSection(StaffListViewModel staffListViewModel)
        {
            StaffListViewModel staffListView = new StaffListViewModel();
            try
            {
                if (tokenManager.CheckToken(staffListViewModel._tenantName + staffListViewModel._userName, staffListViewModel._token))
                {
                    staffListView = this.courseManagerRepository.GetAllStaffScheduleInCourseSection(staffListViewModel);
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
        /// Search Course For Schedule 
        /// </summary>
        /// <param name="searchCourseScheduleViewModel"></param>
        /// <returns></returns>
        public SearchCourseScheduleViewModel SearchCourseForSchedule(SearchCourseScheduleViewModel searchCourseScheduleViewModel)
        {
            SearchCourseScheduleViewModel searchCourseSchedule = new SearchCourseScheduleViewModel();

            try
            {
                if (tokenManager.CheckToken(searchCourseScheduleViewModel._tenantName + searchCourseScheduleViewModel._userName, searchCourseScheduleViewModel._token))
                {
                    searchCourseSchedule = this.courseManagerRepository.SearchCourseForSchedule(searchCourseScheduleViewModel);
                }
                else
                {
                    searchCourseSchedule._failure = true;
                    searchCourseSchedule._message = TOKENINVALID;
                }
            }

            catch (Exception es)
            {
                searchCourseSchedule._failure = true;
                searchCourseSchedule._message = es.Message;
            }
            return searchCourseSchedule;
        }

        /// <summary>
        /// Add/Edit BellSchedule
        /// </summary>
        /// <param name="BellScheduleAddViewModel"></param>
        /// <returns></returns>
        public BellScheduleAddViewModel AddEditBellSchedule(BellScheduleAddViewModel bellScheduleAddViewModel)
        {
            BellScheduleAddViewModel bellSchedule = new BellScheduleAddViewModel();
            try
            {
                if (tokenManager.CheckToken(bellScheduleAddViewModel._tenantName + bellScheduleAddViewModel._userName, bellScheduleAddViewModel._token))
                {
                    bellSchedule = this.courseManagerRepository.AddEditBellSchedule(bellScheduleAddViewModel);
                }
                else
                {
                    bellSchedule._failure = true;
                    bellSchedule._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {

                bellSchedule._failure = true;
                bellSchedule._message = es.Message;
            }
            return bellSchedule;
        }

        /// <summary>
        ///  Get All BellSchedule
        /// </summary>
        /// <param name="bellScheduleListViewModel"></param>
        /// <returns></returns>
        public BellScheduleListViewModel GetAllBellSchedule(BellScheduleListViewModel bellScheduleListViewModel)
        {
            BellScheduleListViewModel bellScheduleList = new BellScheduleListViewModel();
            try
            {
                if (tokenManager.CheckToken(bellScheduleListViewModel._tenantName + bellScheduleListViewModel._userName, bellScheduleListViewModel._token))
                {
                    bellScheduleList = this.courseManagerRepository.GetAllBellSchedule(bellScheduleListViewModel);
                }
                else
                {
                    bellScheduleList._failure = true;
                    bellScheduleList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                bellScheduleList._failure = true;
                bellScheduleList._message = es.Message;
            }
            return bellScheduleList;
        }

        /// <summary>
        /// Get Course Catelog
        /// </summary>
        /// <param name="courseCatelogViewModel"></param>
        /// <returns></returns>
        public CourseCatelogViewModel GetCourseCatelog(CourseCatelogViewModel courseCatelogViewModel)
        {
            CourseCatelogViewModel courseCatelog = new CourseCatelogViewModel();
            try
            {
                if (tokenManager.CheckToken(courseCatelogViewModel._tenantName + courseCatelogViewModel._userName, courseCatelogViewModel._token))
                {
                    courseCatelog = this.courseManagerRepository.GetCourseCatelog(courseCatelogViewModel);
                }
                else
                {
                    courseCatelog._failure = true;
                    courseCatelog._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                courseCatelog._failure = true;
                courseCatelog._message = es.Message;
            }
            return courseCatelog;
        }
    }
}
 

