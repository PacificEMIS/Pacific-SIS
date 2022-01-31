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

using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Grades;
using opensis.data.ViewModels.InputFinalGrade;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Convert;
namespace opensis.data.Repository
{
    public class InputFinalGradeRepository : IInputFinalGradeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public InputFinalGradeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add/Update Student Final Grade
        /// </summary>
        /// <param name="studentFinalGradeListModel"></param>
        /// <returns></returns>
        public StudentFinalGradeListModel AddUpdateStudentFinalGrade(StudentFinalGradeListModel studentFinalGradeListModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<StudentFinalGrade> studentFinalGradeList = new List<StudentFinalGrade>();
                    //List<StudentFinalGradeStandard> studentFinalGradeStandardList = new List<StudentFinalGradeStandard>();
                    List<StudentFinalGradeComments> studentFinalGradeCommentList = new List<StudentFinalGradeComments>();

                    int Id = 1;
                    var StudentFinalStandard = this.context?.StudentFinalGradeStandard.ToList();

                    if (StudentFinalStandard != null && StudentFinalStandard.Any())
                    {
                        Id = ToInt32(value: StudentFinalStandard!.OrderByDescending(s => s.Id).FirstOrDefault()!.Id + 1);
                    }

                    int? YrMarkingPeriodId = 0;
                    int? SmstrMarkingPeriodId = 0;
                    int? QtrMarkingPeriodId = 0;
                    int? PrgrsprdMarkingPeriodId = 0;

                    if (studentFinalGradeListModel.MarkingPeriodId != null)
                    {
                        var markingPeriodid = studentFinalGradeListModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                        if (markingPeriodid.First() == "3")
                        {
                            PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                        }
                        if (markingPeriodid.First() == "2")
                        {
                            QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                        }
                        if (markingPeriodid.First() == "1")
                        {
                            SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                        }
                        if (markingPeriodid.First() == "0")
                        {
                            YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                        }
                    }
                    foreach (var studentFinalGrade in studentFinalGradeListModel.StudentFinalGradeList)
                    {
                        foreach (var studentFinalGradeStandarddata in studentFinalGrade.StudentFinalGradeStandard)
                        {
                            studentFinalGradeStandarddata.Id = Id;
                            studentFinalGradeStandarddata.PrgrsprdMarkingPeriodId = (PrgrsprdMarkingPeriodId > 0) ? PrgrsprdMarkingPeriodId : null;
                            studentFinalGradeStandarddata.QtrMarkingPeriodId = (QtrMarkingPeriodId > 0) ? QtrMarkingPeriodId : null;
                            studentFinalGradeStandarddata.SmstrMarkingPeriodId = (SmstrMarkingPeriodId > 0) ? SmstrMarkingPeriodId : null;
                            studentFinalGradeStandarddata.YrMarkingPeriodId = (YrMarkingPeriodId > 0) ? YrMarkingPeriodId : null;
                            studentFinalGradeStandarddata.CalendarId = studentFinalGradeListModel.CalendarId;
                            studentFinalGradeStandarddata.AcademicYear = studentFinalGradeListModel.AcademicYear;
                            studentFinalGradeStandarddata.TeacherComment = studentFinalGrade.TeacherComment;
                            Id++;
                        }
                    }

                    if (studentFinalGradeListModel.StudentFinalGradeList.Count > 0)
                    {
                        var studentFinalGradeData = new List<StudentFinalGrade>();

                        //if (studentFinalGradeListModel.MarkingPeriodId != null)
                        //{
                            studentFinalGradeData = this.context?.StudentFinalGrade.Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && e.CourseId == studentFinalGradeListModel.CourseId && e.CourseSectionId == studentFinalGradeListModel.CourseSectionId && e.CalendarId == studentFinalGradeListModel.CalendarId /*&& (YrMarkingPeriodId > 0 && e.YrMarkingPeriodId == YrMarkingPeriodId || SmstrMarkingPeriodId > 0 && e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || QtrMarkingPeriodId > 0 && e.QtrMarkingPeriodId == QtrMarkingPeriodId)*/).ToList();
                        //}
                        //else
                        //{
                        //    studentFinalGradeData = this.context?.StudentFinalGrade.Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && e.CourseId == studentFinalGradeListModel.CourseId && e.CourseSectionId == studentFinalGradeListModel.CourseSectionId && e.CalendarId == studentFinalGradeListModel.CalendarId && e.YrMarkingPeriodId == null && e.SmstrMarkingPeriodId == null && e.QtrMarkingPeriodId == null).ToList();
                        //}

                        if (studentFinalGradeData!=null && studentFinalGradeData.Any())
                        {

                            var containStudentFinalGradeSrlno = studentFinalGradeData.Select(x => x.StudentFinalGradeSrlno).Distinct().ToList();

                            List<long> studentFinalGradeSrlnos = new List<long> { };
                            studentFinalGradeSrlnos = containStudentFinalGradeSrlno;

                            var studentFinalGradeStandardData = this.context?.StudentFinalGradeStandard.Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && e.CalendarId == studentFinalGradeListModel.CalendarId && /*(YrMarkingPeriodId > 0 && e.YrMarkingPeriodId == YrMarkingPeriodId || SmstrMarkingPeriodId > 0 && e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || QtrMarkingPeriodId > 0 && e.QtrMarkingPeriodId == QtrMarkingPeriodId) &&*/ (studentFinalGradeSrlnos == null || (studentFinalGradeSrlnos.Contains(e.StudentFinalGradeSrlno)))).ToList();

                            if (studentFinalGradeStandardData!=null && studentFinalGradeStandardData.Any())
                            {

                                this.context?.StudentFinalGradeStandard.RemoveRange(studentFinalGradeStandardData);
                            }

                            var studentFinalGradeCommentsData = this.context?.StudentFinalGradeComments.Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && (studentFinalGradeSrlnos == null || (studentFinalGradeSrlnos.Contains(e.StudentFinalGradeSrlno)))).ToList();

                            if (studentFinalGradeCommentsData !=null &&studentFinalGradeCommentsData.Any())
                            {
                                this.context?.StudentFinalGradeComments.RemoveRange(studentFinalGradeCommentsData);
                            }
                            this.context?.StudentFinalGrade.RemoveRange(studentFinalGradeData);
                            this.context?.SaveChanges();


                            long? studentFinalGradeSrlno = 1;

                            var studentFinalGradeSrlnoData = this.context?.StudentFinalGrade.Where(x => x.SchoolId == studentFinalGradeListModel.SchoolId && x.TenantId == studentFinalGradeListModel.TenantId).OrderByDescending(x => x.StudentFinalGradeSrlno).FirstOrDefault();

                            if (studentFinalGradeSrlnoData != null)
                            {
                                studentFinalGradeSrlno = studentFinalGradeSrlnoData.StudentFinalGradeSrlno + 1;
                            }

                            decimal? academicYear = Utility.GetCurrentAcademicYear(this.context!, studentFinalGradeListModel.TenantId, studentFinalGradeListModel.SchoolId);

                            foreach (var studentFinalGrade in studentFinalGradeListModel.StudentFinalGradeList)
                            {
                                var studentFinalGradeUpdate = new StudentFinalGrade()
                                {
                                    TenantId = studentFinalGradeListModel.TenantId,
                                    SchoolId = studentFinalGradeListModel.SchoolId,
                                    StudentId = studentFinalGrade.StudentId,
                                    CourseId = studentFinalGradeListModel.CourseId,
                                    CourseSectionId = studentFinalGradeListModel.CourseSectionId,
                                    GradeId = studentFinalGrade.GradeId,
                                    GradeScaleId = studentFinalGrade.GradeScaleId,
                                    AcademicYear = academicYear,
                                    CalendarId = studentFinalGradeListModel.CalendarId,
                                    YrMarkingPeriodId = (YrMarkingPeriodId > 0) ? YrMarkingPeriodId : null,
                                    SmstrMarkingPeriodId = (SmstrMarkingPeriodId > 0) ? SmstrMarkingPeriodId : null,
                                    QtrMarkingPeriodId = (QtrMarkingPeriodId > 0) ? QtrMarkingPeriodId : null,
                                    PrgrsprdMarkingPeriodId = (PrgrsprdMarkingPeriodId > 0) ? PrgrsprdMarkingPeriodId : null,
                                    IsPercent = studentFinalGradeListModel.IsPercent,
                                    PercentMarks = studentFinalGrade.PercentMarks,
                                    GradeObtained = studentFinalGrade.GradeObtained,
                                    UpdatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy,
                                    UpdatedOn = DateTime.UtcNow,
                                    StudentFinalGradeSrlno = (long)studentFinalGradeSrlno,
                                    BasedOnStandardGrade = studentFinalGrade.BasedOnStandardGrade,
                                    TeacherComment = studentFinalGrade.TeacherComment,
                                    IsCustomMarkingPeriod = studentFinalGradeListModel.IsCustomMarkingPeriod,
                                    IsExamGrade = studentFinalGradeListModel.IsExamGrade,
                                    //StudentFinalGradeComments = studentFinalGrade.StudentFinalGradeComments?.Select(c =>
                                    //{
                                    //    c.UpdatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy;
                                    //    c.UpdatedOn = DateTime.UtcNow;
                                    //    return c;
                                    //}).ToList(),
                                    StudentFinalGradeComments = studentFinalGrade.StudentFinalGradeComments.Select(c => { c.UpdatedOn = DateTime.UtcNow;
                                        c.UpdatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy;
                                        return c;
                                    }).ToList(),
                                    StudentFinalGradeStandard = studentFinalGrade.StudentFinalGradeStandard.Select(c =>
                                     {
                                         c.AcademicYear = academicYear;
                                         c.UpdatedBy =  studentFinalGradeListModel.CreatedOrUpdatedBy ; 
                                         c.UpdatedOn = DateTime.UtcNow;
                                         return c;
                                     }).ToList()
                                };
                                studentFinalGradeList.Add(studentFinalGradeUpdate);
                                studentFinalGradeSrlno++;
                            }
                            studentFinalGradeListModel._message = "Student Final Grade Updated Succsesfully.";
                        }
                        else
                        {
                            long? studentFinalGradeSrlno = 1;

                            var studentFinalGradeSrlnoData = this.context?.StudentFinalGrade.Where(x => x.SchoolId == studentFinalGradeListModel.SchoolId && x.TenantId == studentFinalGradeListModel.TenantId).OrderByDescending(x => x.StudentFinalGradeSrlno).FirstOrDefault();

                            if (studentFinalGradeSrlnoData != null)
                            {
                                studentFinalGradeSrlno = studentFinalGradeSrlnoData.StudentFinalGradeSrlno + 1;
                            }

                            decimal? academicYear = Utility.GetCurrentAcademicYear(this.context!, studentFinalGradeListModel.TenantId, studentFinalGradeListModel.SchoolId);

                            foreach (var studentFinalGrade in studentFinalGradeListModel.StudentFinalGradeList)
                            {
                                var studentFinalGradeAdd = new StudentFinalGrade()
                                {
                                    TenantId = studentFinalGradeListModel.TenantId,
                                    SchoolId = studentFinalGradeListModel.SchoolId,
                                    StudentId = studentFinalGrade.StudentId,
                                    CourseId = studentFinalGradeListModel.CourseId,
                                    CourseSectionId = studentFinalGradeListModel.CourseSectionId,
                                    GradeId = studentFinalGrade.GradeId,
                                    GradeScaleId = studentFinalGrade.GradeScaleId,
                                    AcademicYear = academicYear,
                                    CalendarId = studentFinalGradeListModel.CalendarId,
                                    YrMarkingPeriodId = (YrMarkingPeriodId > 0) ? YrMarkingPeriodId : null,
                                    SmstrMarkingPeriodId = (SmstrMarkingPeriodId > 0) ? SmstrMarkingPeriodId : null,
                                    QtrMarkingPeriodId = (QtrMarkingPeriodId > 0) ? QtrMarkingPeriodId : null,
                                    PrgrsprdMarkingPeriodId = (PrgrsprdMarkingPeriodId > 0) ? PrgrsprdMarkingPeriodId : null,
                                    IsPercent = studentFinalGradeListModel.IsPercent,
                                    PercentMarks = studentFinalGrade.PercentMarks,
                                    GradeObtained = studentFinalGrade.GradeObtained,
                                    CreatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    StudentFinalGradeSrlno = (long)studentFinalGradeSrlno,
                                    BasedOnStandardGrade = studentFinalGrade.BasedOnStandardGrade,
                                    TeacherComment = studentFinalGrade.TeacherComment,
                                    IsCustomMarkingPeriod= studentFinalGradeListModel.IsCustomMarkingPeriod,
                                    IsExamGrade= studentFinalGradeListModel.IsExamGrade,
                                    StudentFinalGradeComments = studentFinalGrade!.StudentFinalGradeComments.Select(c => { c.CreatedBy = studentFinalGradeListModel?.CreatedOrUpdatedBy; c.CreatedOn = DateTime.UtcNow; return c; }).ToList(),
                                    StudentFinalGradeStandard = studentFinalGrade.StudentFinalGradeStandard.Select(c =>
                                    {
                                        c.CreatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy;
                                        c.CreatedOn = DateTime.UtcNow;
                                        return c;
                                    }).ToList()
                                };
                                studentFinalGradeList.Add(studentFinalGradeAdd);
                                studentFinalGradeSrlno++;
                            }
                            studentFinalGradeListModel._message = "Student Final Grade Added succsesfully.";
                        }
                        this.context?.StudentFinalGrade.AddRange(studentFinalGradeList);
                        //this.context?.StudentFinalGradeStandard.AddRange(studentFinalGradeStandardList);
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        studentFinalGradeListModel._failure = false;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentFinalGradeListModel._failure = true;
                    studentFinalGradeListModel._message = es.Message;
                }
            }
            return studentFinalGradeListModel;
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
                //int? YrMarkingPeriodId = 0;
                //int? SmstrMarkingPeriodId = 0;
                //int? QtrMarkingPeriodId = 0;

                //if (studentFinalGradeListModel.MarkingPeriodId != null)
                //{

                //    var markingPeriodid = studentFinalGradeListModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                //    if (markingPeriodid.First() == "2")
                //    {
                //        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                //    }
                //    if (markingPeriodid.First() == "1")
                //    {
                //        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                //    }
                //    if (markingPeriodid.First() == "0")
                //    {
                //        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                //    }
                //}

                var studentFinalGradeData = new List<StudentFinalGrade>();

                //if (studentFinalGradeListModel.MarkingPeriodId != null)
                //{
                    studentFinalGradeData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeStandard).Include(d => d.StudentFinalGradeComments).ThenInclude(y => y.CourseCommentCategory).Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && e.CourseId == studentFinalGradeListModel.CourseId && e.CourseSectionId == studentFinalGradeListModel.CourseSectionId && e.CalendarId == studentFinalGradeListModel.CalendarId && e.AcademicYear== studentFinalGradeListModel.AcademicYear /*&& (YrMarkingPeriodId > 0 && e.YrMarkingPeriodId == YrMarkingPeriodId || SmstrMarkingPeriodId > 0 && e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || QtrMarkingPeriodId > 0 && e.QtrMarkingPeriodId == QtrMarkingPeriodId)*/).ToList();
                //}
                //else
                //{
                //    studentFinalGradeData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeStandard).Include(d => d.StudentFinalGradeComments).ThenInclude(y => y.CourseCommentCategory).Where(e => e.SchoolId == studentFinalGradeListModel.SchoolId && e.TenantId == studentFinalGradeListModel.TenantId && e.CourseId == studentFinalGradeListModel.CourseId && e.CourseSectionId == studentFinalGradeListModel.CourseSectionId && e.CalendarId == studentFinalGradeListModel.CalendarId && e.YrMarkingPeriodId == null && e.SmstrMarkingPeriodId == null && e.QtrMarkingPeriodId == null).ToList();
                //}

                if (studentFinalGradeData!=null && studentFinalGradeData.Any())
                {
                    studentFinalGradeList.StudentFinalGradeList = studentFinalGradeData;
                    studentFinalGradeList.TenantId = studentFinalGradeListModel.TenantId;
                    studentFinalGradeList.SchoolId = studentFinalGradeListModel.SchoolId;
                    studentFinalGradeList.CalendarId = studentFinalGradeListModel.CalendarId;
                    studentFinalGradeList.CourseId = studentFinalGradeListModel.CourseId;
                    studentFinalGradeList.CourseSectionId = studentFinalGradeListModel.CourseSectionId;
                    studentFinalGradeList.StandardGradeScaleId = studentFinalGradeListModel.StandardGradeScaleId;
                    studentFinalGradeList.MarkingPeriodId = studentFinalGradeListModel.MarkingPeriodId;
                    studentFinalGradeList.AcademicYear = studentFinalGradeListModel.AcademicYear;
                    studentFinalGradeList.IsPercent = studentFinalGradeListModel.IsPercent;
                    studentFinalGradeList.CreatedOrUpdatedBy = studentFinalGradeListModel.CreatedOrUpdatedBy;
                    studentFinalGradeList._userName = studentFinalGradeListModel._userName;
                    studentFinalGradeList._tenantName = studentFinalGradeListModel._tenantName;
                    studentFinalGradeList._token = studentFinalGradeListModel._token;
                    studentFinalGradeList._failure = false;

                    if (studentFinalGradeList.StudentFinalGradeList.Count > 0)
                    {
                        //foreach (var studentFinalGrade in studentFinalGradeList.studentFinalGradeList)
                        //{
                        //    foreach (var FinalGrade in studentFinalGrade.StudentFinalGradeComments)
                        //    {
                        //        FinalGrade.CourseCommentCategory.StudentFinalGradeComments = null;
                        //    }
                        //}
                        studentFinalGradeList.StudentFinalGradeList.ForEach(f => f.StudentFinalGradeComments.ToList().ForEach(r => r.CourseCommentCategory.StudentFinalGradeComments = null! ));
                    }
                }
                else
                {
                    studentFinalGradeList._failure = true;
                    studentFinalGradeList._message = NORECORDFOUND;
                }

            }
            catch (Exception es)
            {
                studentFinalGradeList._message = es.Message;
                studentFinalGradeList._failure = true;
            }
            return studentFinalGradeList;
        }

        //public ReportCardCommentListViewModel GetReportCardCommentsForInputFinalGrade(ReportCardCommentListViewModel reportCardCommentListViewModel)
        //{
        //    try
        //    {
        //        var ReportCardCommentsData = this.context?.ReportCardComments.Where(x => x.TenantId == reportCardCommentListViewModel.TenantId && x.SchoolId == reportCardCommentListViewModel.SchoolId && (x.CourseSectionId == reportCardCommentListViewModel.CourseSectionId || x.ApplicableAllCourses == true) && x.CourseId == reportCardCommentListViewModel.CourseId).ToList();

        //        if (ReportCardCommentsData.Count > 0)
        //        {
        //            reportCardCommentListViewModel.reportCardCommentList = ReportCardCommentsData;
        //            reportCardCommentListViewModel._failure = false;
        //        }
        //        else
        //        {
        //            reportCardCommentListViewModel._failure = true;
        //            reportCardCommentListViewModel._message = NORECORDFOUND;
        //        }
        //    }
        //    catch (Exception es)
        //    {
        //        reportCardCommentListViewModel._message = es.Message;
        //        reportCardCommentListViewModel._failure = true;
        //    }
        //    return reportCardCommentListViewModel;
        //}


        /// <summary>
        /// Get Student Report Card Grades
        /// </summary>
        /// <param name="studentReportCardGradesViewModel"></param>
        /// <returns></returns>
        public StudentReportCardGradesViewModel GetStudentReportCardGrades(StudentReportCardGradesViewModel studentReportCardGradesViewModel)
        {
            StudentReportCardGradesViewModel studentReportCardGrades = new StudentReportCardGradesViewModel();
            studentReportCardGrades.TenantId = studentReportCardGradesViewModel.TenantId;
            studentReportCardGrades.SchoolId = studentReportCardGradesViewModel.SchoolId;
            studentReportCardGrades.StudentId = studentReportCardGradesViewModel.StudentId;
            studentReportCardGrades.AcademicYear = studentReportCardGradesViewModel.AcademicYear;
            studentReportCardGrades.MarkingPeriodId = studentReportCardGradesViewModel.MarkingPeriodId;
            studentReportCardGrades._tenantName = studentReportCardGradesViewModel._tenantName;
            studentReportCardGrades._token = studentReportCardGradesViewModel._token;
            try
            {
                int? YrMarkingPeriodId = 0;
                int? SmstrMarkingPeriodId = 0;
                int? QtrMarkingPeriodId = 0;
                int? PrgrsprdMarkingPeriodId = 0;

                if (studentReportCardGradesViewModel.MarkingPeriodId != null)
                {
                    var markingPeriodid = studentReportCardGradesViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                    if (markingPeriodid.First() == "3")
                    {
                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "2")
                    {
                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "1")
                    {
                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "0")
                    {
                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                }
                var studentFinalGradeAllData = this.context?.StudentFinalGrade.Include(x => x.StudentMaster).Where(e => e.SchoolId == studentReportCardGradesViewModel.SchoolId && e.TenantId == studentReportCardGradesViewModel.TenantId && e.StudentId == studentReportCardGradesViewModel.StudentId).ToList();

                if (studentFinalGradeAllData!=null && studentFinalGradeAllData.Any())
                {
                    decimal? Wgpa = 0.0m;
                    decimal? UNWgpa = 0.0m;

                    var GradeLevelData = this.context?.Gradelevels.Where(x => x.TenantId == studentReportCardGradesViewModel.TenantId && x.SchoolId == studentReportCardGradesViewModel.SchoolId).ToList();

                    var studentFinalGradeData = studentFinalGradeAllData.Where(e => e.AcademicYear == studentReportCardGradesViewModel.AcademicYear && (YrMarkingPeriodId > 0 && e.YrMarkingPeriodId == YrMarkingPeriodId || SmstrMarkingPeriodId > 0 && e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || QtrMarkingPeriodId > 0 && e.QtrMarkingPeriodId == QtrMarkingPeriodId || PrgrsprdMarkingPeriodId > 0 && e.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)).ToList();

                    if (studentFinalGradeData.Count > 0)
                    {
                        decimal? sumOfWGPValue = 0.0m;
                        decimal? sumOfUNWGPValue = 0.0m;
                        int WCSCount = 0;
                        int UNWCSCount = 0;
                        decimal? WCScreditEarned = 0.0m;
                        decimal? UNWCScreditEarned = 0.0m;

                        foreach (var studentFinalGrade in studentFinalGradeData)
                        {
                            decimal? WGPValue = 0.0m;
                            decimal? UNWGPValue = 0.0m;
                            CourseSectionWithGradesViewModel courseSectionWithGrades = new CourseSectionWithGradesViewModel();

                            var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == studentReportCardGradesViewModel.TenantId && x.SchoolId == studentReportCardGradesViewModel.SchoolId && x.CourseId == studentFinalGrade.CourseId && x.CourseSectionId == studentFinalGrade.CourseSectionId);

                            if (courseSectionData != null)
                            {
                                if (courseSectionData.GradeScale != null)
                                {
                                    var gradeData = courseSectionData.GradeScale.Grade.AsEnumerable().FirstOrDefault(x => x.TenantId == studentReportCardGradesViewModel.TenantId && x.SchoolId == studentReportCardGradesViewModel.SchoolId && String.Compare(x.Title, studentFinalGrade?.GradeObtained, true) == 0 && x.GradeScaleId == studentFinalGrade?.GradeScaleId);
                                    if (gradeData != null)
                                    {
                                        if (courseSectionData.IsWeightedCourse == true)
                                        {
                                            WGPValue = studentFinalGrade.CreditEarned == null ? courseSectionData.CreditHours * gradeData.WeightedGpValue : studentFinalGrade.CreditEarned * gradeData.WeightedGpValue;
                                            sumOfWGPValue += WGPValue;
                                            WCScreditEarned += studentFinalGrade.CreditEarned == null ? courseSectionData.CreditHours : studentFinalGrade.CreditEarned;
                                            WCSCount = WCSCount + 1;
                                        }
                                        else
                                        {
                                            UNWGPValue = studentFinalGrade.CreditEarned == null ? courseSectionData.CreditHours * gradeData.UnweightedGpValue : studentFinalGrade.CreditEarned * gradeData.UnweightedGpValue;
                                            sumOfUNWGPValue += UNWGPValue;
                                            UNWCScreditEarned += studentFinalGrade.CreditEarned == null ? courseSectionData.CreditHours : studentFinalGrade.CreditEarned;
                                            UNWCSCount = UNWCSCount + 1;
                                        }
                                    }

                                    courseSectionWithGrades.CourseSectionName = courseSectionData.CourseSectionName;
                                    courseSectionWithGrades.GPValue = courseSectionData.IsWeightedCourse == true ? WGPValue : UNWGPValue;
                                    courseSectionWithGrades.WeightedGP = courseSectionData.IsWeightedCourse == true ? "Yes" : "NO";
                                    courseSectionWithGrades.GradeScaleName = courseSectionData.GradeScale.GradeScaleName;
                                    courseSectionWithGrades.GradeScaleType = courseSectionData.GradeScaleType;
                                    courseSectionWithGrades.GradeScaleValue = courseSectionData.GradeScale.GradeScaleValue;
                                    courseSectionWithGrades.CreditAttempted = studentFinalGrade.CreditAttempted == null ? courseSectionData.CreditHours : studentFinalGrade.CreditAttempted;
                                    courseSectionWithGrades.CreditEarned = studentFinalGrade.CreditEarned == null ? courseSectionData.CreditHours : studentFinalGrade.CreditEarned;
                                    courseSectionWithGrades.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, studentReportCardGradesViewModel.TenantId, studentFinalGrade.CreatedBy);
                                    courseSectionWithGrades.CreatedOn = studentFinalGrade.CreatedOn;
                                    courseSectionWithGrades.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, studentReportCardGradesViewModel.TenantId, studentFinalGrade.UpdatedBy);
                                    courseSectionWithGrades.UpdatedOn = studentFinalGrade.UpdatedOn;
                                }
                            }

                            courseSectionWithGrades.CourseId = studentFinalGrade.CourseId;
                            courseSectionWithGrades.CourseSectionId = studentFinalGrade.CourseSectionId;
                            courseSectionWithGrades.StudentFinalGradeSrlno = studentFinalGrade.StudentFinalGradeSrlno;
                            courseSectionWithGrades.PercentMarks = studentFinalGrade.PercentMarks;
                            courseSectionWithGrades.GradeId = studentFinalGrade.GradeId;
                            courseSectionWithGrades.GradeScaleId = studentFinalGrade.GradeScaleId;
                            courseSectionWithGrades.GradeObtained = studentFinalGrade.GradeObtained;
                            courseSectionWithGrades.CourseId = studentFinalGrade.CourseId;
                            studentReportCardGrades.CourseSectionWithGradesViewModelList.Add(courseSectionWithGrades);
                        }
                        //decimal? Wgpa = 0.0m;
                        //decimal? UNWgpa = 0.0m;
                        if (WCScreditEarned > 0 && sumOfWGPValue > 0)
                        {
                            Wgpa = sumOfWGPValue / WCScreditEarned;
                        }
                        if (UNWCScreditEarned > 0 && sumOfUNWGPValue > 0)
                        {
                            UNWgpa = sumOfUNWGPValue / UNWCScreditEarned;
                        }
                    }

                    studentReportCardGrades.FirstGivenName =  studentFinalGradeAllData?.FirstOrDefault()?.StudentMaster.FirstGivenName;
                    studentReportCardGrades.MiddleName = studentFinalGradeAllData?.FirstOrDefault()?.StudentMaster.MiddleName;
                    studentReportCardGrades.LastFamilyName = studentFinalGradeAllData?.FirstOrDefault()?.StudentMaster.LastFamilyName;
                    studentReportCardGrades.StudentInternalId = studentFinalGradeAllData?.FirstOrDefault()?.StudentMaster.StudentInternalId;
                    studentReportCardGrades.StudentPhoto = studentFinalGradeAllData?.FirstOrDefault()?.StudentMaster.StudentPhoto;
                    studentReportCardGrades.WeightedGPA = Wgpa;
                    studentReportCardGrades.UnWeightedGPA = UNWgpa;
                    studentReportCardGrades.GredeLavel = (GradeLevelData!=null && GradeLevelData.Any()) ? GradeLevelData.FirstOrDefault(x => x.GradeId == studentFinalGradeAllData?.FirstOrDefault()?.GradeId)?.Title : null;
                }
                else
                {
                    studentReportCardGrades._failure = true;
                    studentReportCardGrades._message = NORECORDFOUND;
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
        /// Update Student Report Card Grades
        /// </summary>
        /// <param name="studentReportCardGradesViewModel"></param>
        /// <returns></returns>
        public StudentReportCardGradesViewModel UpdateStudentReportCardGrades(StudentReportCardGradesViewModel studentReportCardGradesViewModel)
        {
            try
            {
                int? YrMarkingPeriodId = 0;
                int? SmstrMarkingPeriodId = 0;
                int? QtrMarkingPeriodId = 0;
                int? PrgrsprdMarkingPeriodId = 0;

                if (studentReportCardGradesViewModel.MarkingPeriodId != null)
                {
                    var markingPeriodid = studentReportCardGradesViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                    if (markingPeriodid.First() == "3")
                    {
                        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "2")
                    {
                        QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "1")
                    {
                        SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                    if (markingPeriodid.First() == "0")
                    {
                        YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                    }
                }
                var studentFinalGradeData = this.context?.StudentFinalGrade.Where(e => e.SchoolId == studentReportCardGradesViewModel.SchoolId && e.TenantId == studentReportCardGradesViewModel.TenantId && e.StudentId == studentReportCardGradesViewModel.StudentId && e.AcademicYear == studentReportCardGradesViewModel.AcademicYear && (YrMarkingPeriodId > 0 && e.YrMarkingPeriodId == YrMarkingPeriodId || SmstrMarkingPeriodId > 0 && e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || QtrMarkingPeriodId > 0 && e.QtrMarkingPeriodId == QtrMarkingPeriodId || PrgrsprdMarkingPeriodId > 0 && e.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId)).ToList();

                if (studentFinalGradeData!=null && studentFinalGradeData.Any())
                {
                    foreach (var studentFinalGrade in studentFinalGradeData)
                    {
                        foreach (var CourseSectionWithGrades in studentReportCardGradesViewModel.CourseSectionWithGradesViewModelList)
                        {
                            if (studentFinalGrade.StudentFinalGradeSrlno == CourseSectionWithGrades.StudentFinalGradeSrlno)
                            {
                                studentFinalGrade.PercentMarks = CourseSectionWithGrades.PercentMarks;
                                studentFinalGrade.GradeObtained = CourseSectionWithGrades.GradeObtained;
                                studentFinalGrade.CreditAttempted = CourseSectionWithGrades.CreditAttempted;
                                studentFinalGrade.CreditEarned = CourseSectionWithGrades.CreditEarned;
                                studentFinalGrade.UpdatedBy = studentReportCardGradesViewModel.UpdatedBy;
                                studentFinalGrade.UpdatedOn = DateTime.UtcNow;
                            }
                        }
                    }
                    this.context?.SaveChanges();
                    studentReportCardGradesViewModel._failure = false;
                    studentReportCardGradesViewModel._message = "Updated successfully";
                }
                else
                {
                    studentReportCardGradesViewModel._failure = true;
                    studentReportCardGradesViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentReportCardGradesViewModel._failure = true;
                studentReportCardGradesViewModel._message = es.Message;
            }
            return studentReportCardGradesViewModel;
        }

        /// <summary>
        /// Get All Student For Final Grade
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentListModel GetAllStudentListForFinalGrade(PageResult pageResult)
        {
            StudentListModel studentListModel = new ();
            IQueryable<StudentListView>? transactionIQ = null;
            IQueryable<StudentListView>? studentDataList = null;
            List<int> studentIds = new ();

            var studentFinalGradeData = this.context?.StudentFinalGrade.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.AcademicYear==pageResult.AcademicYear);

            if (studentFinalGradeData !=null && studentFinalGradeData.Any())
            {
                studentIds = studentFinalGradeData.Select(x => x.StudentId).Distinct().ToList();
            }

            studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && studentIds.Contains(x.StudentId) && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : true) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true));

            try
            {
                if (studentDataList != null) {
                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {
                    transactionIQ = studentDataList;
                }
                else
                {
                    string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                    Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");

                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                            

                        transactionIQ = studentDataList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || ($"{x.FirstGivenName}{ x.MiddleName}{x.LastFamilyName}").Contains(Columnvalue.ToLower()) || (x.FirstGivenName + x.MiddleName).Contains(Columnvalue.ToLower()) || (x.FirstGivenName + x.LastFamilyName).Contains(Columnvalue.ToLower()) || (x.MiddleName + x.LastFamilyName).Contains(Columnvalue.ToLower()) ||
                                                                    x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.AlternateId != null && x.AlternateId.Contains(Columnvalue) ||
                                                                    x.HomePhone != null && x.HomePhone.Contains(Columnvalue) ||
                                                                    x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue) ||
                                                                    x.PersonalEmail != null && x.PersonalEmail.Contains(Columnvalue) ||
                                                                    x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue) ||
                                                                    x.GradeLevelTitle != null && x.GradeLevelTitle.Contains(Columnvalue) ||
                                                                    x.SectionName != null && x.SectionName.Contains(Columnvalue));
                    }
                    else
                    {
                        if (pageResult.FilterParams!.Any(x => x.ColumnName.ToLower() == "coursesection"))
                        {
                            var filterValue = pageResult.FilterParams!.AsEnumerable().Where(x => String.Compare(x.ColumnName, "coursesection", true) == 0).Select(x => x.FilterValue).FirstOrDefault();

                            var studentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId.ToString() == filterValue && x.IsDropped != true);

                            if (studentCoursesectionScheduleData !=null && studentCoursesectionScheduleData.ToList().Any())
                            {
                                var studentGuid = studentCoursesectionScheduleData.Select(s => s.StudentGuid).ToList();
                                transactionIQ = studentDataList.Where(x => studentGuid.Contains(x.StudentGuid));

                                var indexValue = pageResult.FilterParams!.FindIndex(x => x.ColumnName.ToLower() == "coursesection");
                                pageResult.FilterParams.RemoveAt(indexValue);

                                if (pageResult.FilterParams.Count > 0)
                                {
                                    transactionIQ = Utility.FilteredData(pageResult.FilterParams, transactionIQ).AsQueryable();
                                }
                            }
                            else
                            {
                                studentListModel._message = NORECORDFOUND;
                                studentListModel._failure = true;
                                return studentListModel;
                            }
                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, studentDataList).AsQueryable();
                        }                       
                    }
                }

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    var filterInDateRange = transactionIQ.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate);
                    if (filterInDateRange.Any())
                    {
                        transactionIQ = filterInDateRange;
                    }
                    else
                    {
                        transactionIQ = null;
                    }
                }

                if (pageResult.SortingModel != null)
                {
                    transactionIQ = Utility.Sort(transactionIQ!, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                }
                else
                {
                    transactionIQ = transactionIQ?.OrderBy(s => s.LastFamilyName).ThenBy(c => c.FirstGivenName);
                }

                int? totalCount = transactionIQ?.Count();
                if (totalCount > 0)
                {
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ?.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    studentListModel.studentListViews = transactionIQ!.ToList();

                    studentListModel.studentListViews.ForEach(c =>
                    {
                        c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                        c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                    });

                    studentListModel.TotalCount = totalCount;
                    studentListModel._message = "success";
                    studentListModel._failure = false;
                }
                else
                {
                    studentListModel._message = NORECORDFOUND;
                    studentListModel._failure = true;
                }

                studentListModel.TenantId = pageResult.TenantId;
                studentListModel.SchoolId = pageResult.SchoolId;
                studentListModel.PageNumber = pageResult.PageNumber;
                studentListModel._pageSize = pageResult.PageSize;
                studentListModel._tenantName = pageResult._tenantName;
                studentListModel._token = pageResult._token;

            }
            }
            catch (Exception es)
            {
                studentListModel._message = es.Message;
                studentListModel._failure = true;
            }
            return studentListModel;
        }
    }
}
