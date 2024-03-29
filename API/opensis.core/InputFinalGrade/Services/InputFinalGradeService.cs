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

using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.core.InputFinalGrade.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.InputFinalGrade;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.InputFinalGrade.Services
{
    public class InputFinalGradeService : IInputFinalGradeService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IInputFinalGradeRepository inputFinalGradeRepository;
        public ICheckLoginSession tokenManager;
        public InputFinalGradeService(IInputFinalGradeRepository inputFinalGradeRepository, ICheckLoginSession checkLoginSession)
        {
            this.inputFinalGradeRepository = inputFinalGradeRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// Add/Update Student Final Grade
        /// </summary>
        /// <param name="studentFinalGradeListModel"></param>
        /// <returns></returns>
        public StudentFinalGradeListModel AddUpdateStudentFinalGrade(StudentFinalGradeListModel studentFinalGradeListModel)
        {
            StudentFinalGradeListModel studentFinalGradeList = new StudentFinalGradeListModel();
            try
            {
                if (tokenManager.CheckToken(studentFinalGradeListModel._tenantName + studentFinalGradeListModel._userName, studentFinalGradeListModel._token))
                {
                    studentFinalGradeList = this.inputFinalGradeRepository.AddUpdateStudentFinalGrade(studentFinalGradeListModel);
                }
                else
                {
                    studentFinalGradeList._failure = true;
                    studentFinalGradeList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentFinalGradeList._failure = true;
                studentFinalGradeList._message = es.Message;
            }
            return studentFinalGradeList;
        }

        /// <summary>
        /// Get All Student Final Grade List
        /// </summary>
        /// <param name="studentFinalGradeListModel"></param>
        /// <returns></returns>
        public StudentFinalGradeListModel GetAllStudentFinalGradeList(StudentFinalGradeListModel studentFinalGradeListModel)
        {
            StudentFinalGradeListModel studentFinalGradeList = new StudentFinalGradeListModel();
            try
            {
                if (tokenManager.CheckToken(studentFinalGradeListModel._tenantName + studentFinalGradeListModel._userName, studentFinalGradeListModel._token))
                {
                    studentFinalGradeList = this.inputFinalGradeRepository.GetAllStudentFinalGradeList(studentFinalGradeListModel);
                }
                else
                {
                    studentFinalGradeList._failure = true;
                    studentFinalGradeList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentFinalGradeList._failure = true;
                studentFinalGradeList._message = es.Message;
            }
            return studentFinalGradeList;
        }

        //public ReportCardCommentListViewModel GetReportCardCommentsForInputFinalGrade(ReportCardCommentListViewModel reportCardCommentListViewModel)
        //{
        //    ReportCardCommentListViewModel reportCardCommentListView = new ReportCardCommentListViewModel();
        //    try
        //    {
        //        if (tokenManager.CheckToken(reportCardCommentListViewModel._tenantName + reportCardCommentListViewModel._userName, reportCardCommentListViewModel._token))
        //        {
        //            reportCardCommentListView = this.inputFinalGradeRepository.GetReportCardCommentsForInputFinalGrade(reportCardCommentListViewModel);
        //        }
        //        else
        //        {
        //            reportCardCommentListView._failure = true;
        //            reportCardCommentListView._message = TOKENINVALID;
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        reportCardCommentListView._failure = true;
        //        reportCardCommentListView._message = es.Message;
        //    }
        //    return reportCardCommentListView;
        //}

        /// <summary>
        /// Get Student Report Card Grades
        /// </summary>
        /// <param name="studentReportCardGradesViewModel"></param>
        /// <returns></returns>
        public StudentReportCardGradesViewModel GetStudentReportCardGrades(StudentReportCardGradesViewModel studentReportCardGradesViewModel)
        {
            StudentReportCardGradesViewModel studentReportCardGrades = new StudentReportCardGradesViewModel();
            try
            {
                if (tokenManager.CheckToken(studentReportCardGradesViewModel._tenantName + studentReportCardGradesViewModel._userName, studentReportCardGradesViewModel._token))
                {
                    studentReportCardGrades = this.inputFinalGradeRepository.GetStudentReportCardGrades(studentReportCardGradesViewModel);
                }
                else
                {
                    studentReportCardGrades._failure = true;
                    studentReportCardGrades._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentReportCardGrades._failure = true;
                studentReportCardGrades._message = es.Message;
            }
            return studentReportCardGrades;
        }

        /// <summary>
        /// UpdateStudent Report Card Grades
        /// </summary>
        /// <param name="studentReportCardGradesViewModel"></param>
        /// <returns></returns>
        public StudentReportCardGradesViewModel UpdateStudentReportCardGrades(StudentReportCardGradesViewModel studentReportCardGradesViewModel)
        {
            StudentReportCardGradesViewModel studentReportCardGrades = new StudentReportCardGradesViewModel();
            try
            {
                if (tokenManager.CheckToken(studentReportCardGradesViewModel._tenantName + studentReportCardGradesViewModel._userName, studentReportCardGradesViewModel._token))
                {
                    studentReportCardGrades = this.inputFinalGradeRepository.UpdateStudentReportCardGrades(studentReportCardGradesViewModel);
                }
                else
                {
                    studentReportCardGrades._failure = true;
                    studentReportCardGrades._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentReportCardGrades._failure = true;
                studentReportCardGrades._message = es.Message;
            }
            return studentReportCardGrades;
        }

        /// <summary>
        /// Get All Student List For Final Grade
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentListModel GetAllStudentListForFinalGrade(PageResult pageResult)
        {
            StudentListModel studentList = new StudentListModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    studentList = this.inputFinalGradeRepository.GetAllStudentListForFinalGrade(pageResult);
                }
                else
                {
                    studentList._failure = true;
                    studentList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentList._failure = true;
                studentList._message = es.Message;
            }
            return studentList;
        }

        /// <summary>
        /// GetGradebookGradeinFinalGrade
        /// </summary>
        /// <param name="gardeBookGradeViewModel"></param>
        /// <returns></returns>
        public GardeBookGradeViewModel GetGradebookGradeinFinalGrade(GardeBookGradeViewModel gardeBookGradeViewModel)
        {
            GardeBookGradeViewModel gardeBookGrade = new GardeBookGradeViewModel();
            try
            {
                if (tokenManager.CheckToken(gardeBookGradeViewModel._tenantName + gardeBookGradeViewModel._userName, gardeBookGradeViewModel._token))
                {
                    gardeBookGrade = this.inputFinalGradeRepository.GetGradebookGradeinFinalGrade(gardeBookGradeViewModel);
                }
                else
                {
                    gardeBookGrade._failure = true;
                    gardeBookGrade._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                gardeBookGrade._failure = true;
                gardeBookGrade._message = es.Message;
            }
            return gardeBookGrade;
        }
    }
}
