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
using Newtonsoft.Json;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.StudentEffortGrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StudentEffortGradeRepository : IStudentEffortGradeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StudentEffortGradeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add/Update Student Effort Grade
        /// </summary>
        /// <param name="studentEffortGradeListModel"></param>
        /// <returns></returns>
        public HomeRoomStaffByStudentListModel AddUpdateStudentEffortGrade(HomeRoomStaffByStudentListModel studentEffortGradeListModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    List<StudentEffortGradeMaster> studentEffortGradeList = new List<StudentEffortGradeMaster>();

                    long Id = 1;
                    var StudentEffortDetails = this.context?.StudentEffortGradeDetail.ToList();

                    if (StudentEffortDetails != null && StudentEffortDetails.Any())
                    {
                        // Id = StudentEffortDetails.OrderByDescending(s => s.Id).FirstOrDefault().Id + 1;
                        Id = StudentEffortDetails.OrderByDescending(s => s.Id).FirstOrDefault()!.Id + 1;
                    }
                    studentEffortGradeListModel.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, studentEffortGradeListModel.TenantId, studentEffortGradeListModel.SchoolId);

                    int? YrMarkingPeriodId = 0;
                    int? SmstrMarkingPeriodId = 0;
                    int? QtrMarkingPeriodId = 0;
                    int? PrgrsprdMarkingPeriodId = 0;

                    var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                    if (progressPeriodsData != null)
                    {
                        PrgrsprdMarkingPeriodId = progressPeriodsData.MarkingPeriodId;
                    }
                    else
                    {
                        var quartersData = this.context?.Quarters.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                        if (quartersData != null)
                        {
                            QtrMarkingPeriodId = quartersData.MarkingPeriodId;

                        }
                        else
                        {
                            var semestersData = this.context?.Semesters.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                            if (semestersData != null)
                            {
                                SmstrMarkingPeriodId = semestersData.MarkingPeriodId;
                            }
                            else
                            {
                                var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                                if (yearsData != null)
                                {
                                    YrMarkingPeriodId = yearsData.MarkingPeriodId;

                                }
                            }
                        }
                    }

                    if (studentEffortGradeListModel.studentsByHomeRoomStaffView != null && studentEffortGradeListModel.studentsByHomeRoomStaffView.Any())
                    {
                        foreach (var studentEffortGrade in studentEffortGradeListModel.studentsByHomeRoomStaffView)
                        {
                            //if (studentEffortGrade.StudentEffortGradeDetail.ToList().Count() > 0 || studentEffortGrade.StudentEffortGradeDetail != null)
                            if (studentEffortGrade.StudentEffortGradeDetail != null && studentEffortGrade.StudentEffortGradeDetail.Any())
                            {
                                foreach (var studentEffortGradeDetaildata in studentEffortGrade.StudentEffortGradeDetail)
                                {
                                    studentEffortGradeDetaildata.Id = Id;
                                    Id++;
                                }
                            }
                        }

                        var studentEffortGradeData = new List<StudentEffortGradeMaster>();

                        var studentIds = studentEffortGradeListModel.studentsByHomeRoomStaffView.Select(s => s.StudentId).ToList();
                        studentEffortGradeData = this.context?.StudentEffortGradeMaster.Where(e => e.SchoolId == studentEffortGradeListModel.SchoolId && e.TenantId == studentEffortGradeListModel.TenantId && e.AcademicYear == studentEffortGradeListModel.AcademicYear && (e.YrMarkingPeriodId == YrMarkingPeriodId || e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || e.QtrMarkingPeriodId == QtrMarkingPeriodId || e.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId) && studentIds.Contains(e.StudentId)).ToList();

                        if (studentEffortGradeData != null && studentEffortGradeData.Any())
                        {
                            var containStudentEffortGradeSrlno = studentEffortGradeData.Select(x => x.StudentEffortGradeSrlno).Distinct().ToList();

                            List<long> studentEffortGradeSrlnos = new List<long> { };
                            studentEffortGradeSrlnos = containStudentEffortGradeSrlno;

                            var studentEffortGradeDetailsData = this.context?.StudentEffortGradeDetail.Where(e => e.SchoolId == studentEffortGradeListModel.SchoolId && e.TenantId == studentEffortGradeListModel.TenantId && (studentEffortGradeSrlnos == null || (studentEffortGradeSrlnos.Contains(e.StudentEffortGradeSrlno)))).ToList();

                            if (studentEffortGradeDetailsData != null && studentEffortGradeDetailsData.Any())
                            {
                                this.context?.StudentEffortGradeDetail.RemoveRange(studentEffortGradeDetailsData);
                            }
                            this.context?.StudentEffortGradeMaster.RemoveRange(studentEffortGradeData);
                            this.context?.SaveChanges();

                            long? studentEffortGradeSrlno = 1;

                            var studentFinalGradeSrlnoData = this.context?.StudentEffortGradeMaster.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId).OrderByDescending(x => x.StudentEffortGradeSrlno).FirstOrDefault();

                            if (studentFinalGradeSrlnoData != null)
                            {
                                studentEffortGradeSrlno = studentFinalGradeSrlnoData.StudentEffortGradeSrlno + 1;
                            }

                            foreach (var studentEffortGrade in studentEffortGradeListModel.studentsByHomeRoomStaffView)
                            {
                                var studentEffortGradeUpdate = new StudentEffortGradeMaster()
                                {
                                    TenantId = (Guid)studentEffortGradeListModel.TenantId!,
                                    SchoolId = (int)studentEffortGradeListModel.SchoolId!,
                                    StudentId = studentEffortGrade.StudentId,
                                    CourseId = 0,
                                    CourseSectionId = 0,
                                    AcademicYear = studentEffortGradeListModel.AcademicYear,
                                    CalendarId = null,
                                    YrMarkingPeriodId = (YrMarkingPeriodId > 0) ? YrMarkingPeriodId : null,
                                    SmstrMarkingPeriodId = (SmstrMarkingPeriodId > 0) ? SmstrMarkingPeriodId : null,
                                    QtrMarkingPeriodId = (QtrMarkingPeriodId > 0) ? QtrMarkingPeriodId : null,
                                    PrgrsprdMarkingPeriodId = (PrgrsprdMarkingPeriodId > 0) ? PrgrsprdMarkingPeriodId : null,
                                    UpdatedBy = studentEffortGradeListModel.CreatedOrUpdatedBy,
                                    UpdatedOn = DateTime.UtcNow,
                                    StudentEffortGradeSrlno = (long)studentEffortGradeSrlno,
                                    TeacherComment = studentEffortGrade.TeacherComment,
                                    StudentEffortGradeDetail = studentEffortGrade.StudentEffortGradeDetail.Select(c =>
                                    {
                                        c.UpdatedBy = studentEffortGradeListModel.CreatedOrUpdatedBy;
                                        c.UpdatedOn = DateTime.UtcNow;
                                        return c;
                                    }).ToList()
                                };
                                studentEffortGradeList.Add(studentEffortGradeUpdate);
                                studentEffortGradeSrlno++;
                            }
                            studentEffortGradeListModel._message = "Student Effort Grade Updated Succsesfully.";
                        }
                        else
                        {
                            long? studentEffortGradeSrlno = 1;

                            var studentEffortGradeSrlnoData = this.context?.StudentEffortGradeMaster.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId).OrderByDescending(x => x.StudentEffortGradeSrlno).FirstOrDefault();

                            if (studentEffortGradeSrlnoData != null)
                            {
                                studentEffortGradeSrlno = studentEffortGradeSrlnoData.StudentEffortGradeSrlno + 1;
                            }

                            foreach (var studentEffortGrade in studentEffortGradeListModel.studentsByHomeRoomStaffView)
                            {
                                var studentEffortGradeAdd = new StudentEffortGradeMaster()
                                {
                                    TenantId = (Guid)studentEffortGradeListModel.TenantId!,
                                    SchoolId = (int)studentEffortGradeListModel.SchoolId!,
                                    StudentId = studentEffortGrade.StudentId,
                                    CourseId = 0,
                                    CourseSectionId = 0,
                                    AcademicYear = studentEffortGradeListModel.AcademicYear,
                                    CalendarId = null,
                                    YrMarkingPeriodId = (YrMarkingPeriodId > 0) ? YrMarkingPeriodId : null,
                                    SmstrMarkingPeriodId = (SmstrMarkingPeriodId > 0) ? SmstrMarkingPeriodId : null,
                                    QtrMarkingPeriodId = (QtrMarkingPeriodId > 0) ? QtrMarkingPeriodId : null,
                                    PrgrsprdMarkingPeriodId = (PrgrsprdMarkingPeriodId > 0) ? PrgrsprdMarkingPeriodId : null,
                                    CreatedBy = studentEffortGradeListModel.CreatedOrUpdatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    StudentEffortGradeSrlno = (long)studentEffortGradeSrlno,
                                    TeacherComment = studentEffortGrade.TeacherComment,
                                    StudentEffortGradeDetail = studentEffortGrade.StudentEffortGradeDetail.Select(c =>
                                    {
                                        c.CreatedBy = studentEffortGradeListModel.CreatedOrUpdatedBy;
                                        c.CreatedOn = DateTime.UtcNow;
                                        return c;
                                    }).ToList()
                                };
                                studentEffortGradeList.Add(studentEffortGradeAdd);
                                studentEffortGradeSrlno++;
                            }
                            studentEffortGradeListModel._message = "Student Effort Grade Added succsesfully.";
                        }
                        this.context?.StudentEffortGradeMaster.AddRange(studentEffortGradeList);
                        this.context?.SaveChanges();
                        transaction?.Commit();
                        studentEffortGradeListModel._failure = false;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentEffortGradeListModel._failure = true;
                    studentEffortGradeListModel._message = es.Message;
                }
            }
            return studentEffortGradeListModel;
        }

        /// <summary>
        /// Get All Student Effort Grade List
        /// </summary>
        /// <param name="studentEffortGradeListModel"></param>
        /// <returns></returns>
        public StudentEffortGradeListModel GetAllStudentEffortGradeList(StudentEffortGradeListModel studentEffortGradeListModel)
        {
            StudentEffortGradeListModel studentEffortGradeList = new StudentEffortGradeListModel();
            try
            {
                int? YrMarkingPeriodId = 0;
                int? SmstrMarkingPeriodId = 0;
                int? QtrMarkingPeriodId = 0;
                int? PrgrsprdMarkingPeriodId = 0;

                var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                if (progressPeriodsData != null)
                {
                    PrgrsprdMarkingPeriodId = progressPeriodsData.MarkingPeriodId;
                }
                else
                {
                    var quartersData = this.context?.Quarters.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                    if (quartersData != null)
                    {
                        QtrMarkingPeriodId = quartersData.MarkingPeriodId;

                    }
                    else
                    {
                        var semestersData = this.context?.Semesters.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                        if (semestersData != null)
                        {
                            SmstrMarkingPeriodId = semestersData.MarkingPeriodId;
                        }
                        else
                        {
                            var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == studentEffortGradeListModel.SchoolId && x.TenantId == studentEffortGradeListModel.TenantId && x.StartDate == studentEffortGradeListModel.MarkingPeriodStartDate && x.EndDate == studentEffortGradeListModel.MarkingPeriodEndDate && x.AcademicYear == studentEffortGradeListModel.AcademicYear).FirstOrDefault();

                            if (yearsData != null)
                            {
                                YrMarkingPeriodId = yearsData.MarkingPeriodId;

                            }
                        }
                    }
                }

                //if (studentEffortGradeListModel.MarkingPeriodId != null)
                //{

                //    var markingPeriodid = studentEffortGradeListModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                //    if (markingPeriodid.First() == "3")
                //    {
                //        PrgrsprdMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                //    }
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

                var studentEffortGradeData = new List<StudentEffortGradeMaster>();

                studentEffortGradeData = this.context?.StudentEffortGradeMaster.Include(x => x.StudentEffortGradeDetail.OrderBy(x => x.Id)).Where(e => e.SchoolId == studentEffortGradeListModel.SchoolId && e.TenantId == studentEffortGradeListModel.TenantId && (e.YrMarkingPeriodId == YrMarkingPeriodId || e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || e.QtrMarkingPeriodId == QtrMarkingPeriodId || e.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId) && e.AcademicYear == studentEffortGradeListModel.AcademicYear).ToList();

                if (studentEffortGradeData != null && studentEffortGradeData.Any())
                {
                    studentEffortGradeList.studentEffortGradeList = studentEffortGradeData;
                    studentEffortGradeList.TenantId = studentEffortGradeListModel.TenantId;
                    studentEffortGradeList.SchoolId = studentEffortGradeListModel.SchoolId;
                    //studentEffortGradeList.CourseId = studentEffortGradeListModel.CourseId;
                    //studentEffortGradeList.CourseSectionId = studentEffortGradeListModel.CourseSectionId;
                    //studentEffortGradeList.CalendarId = studentEffortGradeListModel.CalendarId;
                    //studentEffortGradeList.MarkingPeriodId = studentEffortGradeListModel.MarkingPeriodId;
                    studentEffortGradeList.AcademicYear = studentEffortGradeListModel.AcademicYear;
                    studentEffortGradeList.CreatedOrUpdatedBy = studentEffortGradeListModel.CreatedOrUpdatedBy;
                    studentEffortGradeList._userName = studentEffortGradeListModel._userName;
                    studentEffortGradeList._tenantName = studentEffortGradeListModel._tenantName;
                    studentEffortGradeList._token = studentEffortGradeListModel._token;
                    studentEffortGradeList._failure = false;
                }
                else
                {
                    studentEffortGradeList.studentEffortGradeList = studentEffortGradeData ?? new();
                    studentEffortGradeList._failure = true;
                    studentEffortGradeList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentEffortGradeList._message = es.Message;
                studentEffortGradeList._failure = true;
            }
            return studentEffortGradeList;
        }

        //Start GetStudentListByHomeRoomStaff //
        public HomeRoomStaffByStudentListModel GetStudentListByHomeRoomStaff(PageResult pageResult)
        {
            HomeRoomStaffByStudentListModel studentListByHomeRoomStaff = new HomeRoomStaffByStudentListModel();

            IQueryable<StudentsByHomeRoomStaffView>? transactionIQ = null;

            var studentListByHomeRoomStaffView = new List<StudentsByHomeRoomStaffView>();

            try
            {
                int? YrMarkingPeriodId = 0;
                int? SmstrMarkingPeriodId = 0;
                int? QtrMarkingPeriodId = 0;
                int? PrgrsprdMarkingPeriodId = 0;

                var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (progressPeriodsData != null)
                {
                    PrgrsprdMarkingPeriodId = progressPeriodsData.MarkingPeriodId;
                }
                else
                {
                    var quartersData = this.context?.Quarters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                    if (quartersData != null)
                    {
                        QtrMarkingPeriodId = quartersData.MarkingPeriodId;

                    }
                    else
                    {
                        var semestersData = this.context?.Semesters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                        if (semestersData != null)
                        {
                            SmstrMarkingPeriodId = semestersData.MarkingPeriodId;
                        }
                        else
                        {
                            var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                            if (yearsData != null)
                            {
                                YrMarkingPeriodId = yearsData.MarkingPeriodId;

                            }
                        }
                    }
                }

                var staffCourseSection = this.context?.StaffCoursesectionSchedule.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.StaffId == pageResult.StaffId && x.IsDropped != true && (pageResult.MarkingPeriodStartDate >= x.DurationStartDate && pageResult.MarkingPeriodStartDate <= x.DurationEndDate) && (pageResult.MarkingPeriodEndDate >= x.DurationStartDate && pageResult.MarkingPeriodEndDate <= x.DurationEndDate) /*&& x.AcademicYear == pageResult.AcademicYear*/).ToList();

                var staffcsids = staffCourseSection?.Select(x => x.CourseSectionId).ToList();

                if (staffcsids?.Any() == true)
                {
                    var scheduledData = this.context?.StudentCoursesectionSchedule.
                                       Join(this.context.StudentMaster,
                                       scs => scs.StudentId, sm => sm.StudentId,
                                       (scs, sm) => new { scs, sm }).Where(c => c.scs.TenantId == pageResult.TenantId && c.scs.SchoolId == pageResult.SchoolId && c.sm.SchoolId == pageResult.SchoolId && c.sm.TenantId == pageResult.TenantId && staffcsids!.Contains(c.scs.CourseSectionId) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? c.sm.IsActive != false : true) && (c.scs.IsDropped != true)).ToList();
                    if (scheduledData != null && scheduledData.Any())
                    {
                        var StudentEffortGradeMasterData = this.context?.StudentEffortGradeMaster.Include(x => x.StudentEffortGradeDetail.OrderBy(x => x.Id)).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && (e.YrMarkingPeriodId == YrMarkingPeriodId || e.SmstrMarkingPeriodId == SmstrMarkingPeriodId || e.QtrMarkingPeriodId == QtrMarkingPeriodId || e.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId) && e.AcademicYear == pageResult.AcademicYear).ToList();

                        studentListByHomeRoomStaffView = scheduledData?.Select(ssv => new StudentsByHomeRoomStaffView
                        {
                            TenantId = ssv.sm.TenantId,
                            SchoolId = ssv.sm.SchoolId,
                            StudentId = ssv.sm.StudentId,
                            StudentGuid = ssv.sm.StudentGuid,
                            FirstGivenName = ssv.sm.FirstGivenName,
                            MiddleName = ssv.sm.MiddleName,
                            LastFamilyName = ssv.sm.LastFamilyName,
                            AlternateId = ssv.sm.AlternateId,
                            StudentInternalId = ssv.sm.StudentInternalId,
                            GradeLevel = this.context?.Gradelevels.FirstOrDefault(c => c.TenantId == ssv.sm.TenantId && c.SchoolId == ssv.sm.SchoolId && c.GradeId == ssv.scs.GradeId)?.Title,
                            StudentPhoto = (pageResult.ProfilePhoto == true) ? ssv.sm.StudentThumbnailPhoto : null,
                            IsDropped = ssv.scs.IsDropped,
                            StudentEffortGradeSrlno = StudentEffortGradeMasterData?.Where(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId).Any() == true ? StudentEffortGradeMasterData.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)!.StudentEffortGradeSrlno : 0,
                            AcademicYear = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.AcademicYear,
                            TeacherComment = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.TeacherComment,
                            YrMarkingPeriodId = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.YrMarkingPeriodId,
                            SmstrMarkingPeriodId = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.SmstrMarkingPeriodId,
                            QtrMarkingPeriodId = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.QtrMarkingPeriodId,
                            PrgrsprdMarkingPeriodId = StudentEffortGradeMasterData?.FirstOrDefault(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId)?.PrgrsprdMarkingPeriodId,
                            StudentEffortGradeDetail = StudentEffortGradeMasterData.Where(x => x.SchoolId == ssv.sm.SchoolId && x.StudentId == ssv.sm.StudentId).SelectMany(s => s.StudentEffortGradeDetail).ToList(),

                        }).GroupBy(f => f.StudentId).Select(g => g.First()).ToList();
                    }

                    if (studentListByHomeRoomStaffView != null && studentListByHomeRoomStaffView.Any())
                    {
                        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                        {
                            transactionIQ = studentListByHomeRoomStaffView.AsQueryable();
                        }
                        else
                        {
                            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                            {
                                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;

                                transactionIQ = studentListByHomeRoomStaffView.Where(x => x.FirstGivenName != null && x.FirstGivenName.Contains(Columnvalue) ||
                                x.LastFamilyName != null && x.LastFamilyName.Contains(Columnvalue) ||
                                (x.GradeLevel != null && x.GradeLevel.Contains(Columnvalue)) ||
                                (x.StudentInternalId != null && x.StudentInternalId.Contains(Columnvalue))).AsQueryable();
                            }
                            else
                            {
                                transactionIQ = Utility.FilteredData(pageResult.FilterParams!, studentListByHomeRoomStaffView).AsQueryable();
                            }
                            transactionIQ = transactionIQ.Distinct();
                        }

                        if (pageResult.SortingModel != null)
                        {
                            switch (pageResult!.SortingModel!.SortColumn)
                            {
                                default:
                                    transactionIQ = Utility.Sort(transactionIQ, pageResult?.SortingModel?.SortColumn!, pageResult?.SortingModel?.SortDirection!);
                                    break;
                            }
                        }

                        studentListByHomeRoomStaff.studentsByHomeRoomStaffView = transactionIQ!.ToList() ?? new();
                        studentListByHomeRoomStaff._failure = false;
                        studentListByHomeRoomStaff.studentsByHomeRoomStaffView.ForEach(y => { y.ProgressPeriod = null; y.Quarters = null; y.Semesters = null; y.SchoolYears = null; y.StudentEffortGradeDetail.ToList().ForEach(x => x.StudentEffortGradeMaster = new()); });
                    }
                    else
                    {
                        studentListByHomeRoomStaff.studentsByHomeRoomStaffView = studentListByHomeRoomStaffView ?? new();
                        studentListByHomeRoomStaff._failure = true;
                        studentListByHomeRoomStaff._message = NORECORDFOUND;
                    }

                    studentListByHomeRoomStaff.TenantId = pageResult?.TenantId;
                    studentListByHomeRoomStaff.SchoolId = pageResult?.SchoolId;
                    studentListByHomeRoomStaff.StaffId = pageResult?.StaffId;
                    studentListByHomeRoomStaff.AcademicYear = pageResult?.AcademicYear;
                    studentListByHomeRoomStaff._tenantName = pageResult?._tenantName;
                    studentListByHomeRoomStaff._token = pageResult?._token;
                }
                else
                {
                    studentListByHomeRoomStaff._failure = true;
                    studentListByHomeRoomStaff._message = "Staff is not sheduled in Any Coursesections";
                }
            }
            catch (Exception es)
            {
                studentListByHomeRoomStaff._failure = true;
                studentListByHomeRoomStaff._message = es.Message; ;
            }
            return studentListByHomeRoomStaff;
        }
        //End GetStudentListByHomeRoomStaff API//
    }
}
