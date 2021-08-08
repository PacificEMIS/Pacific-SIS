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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CourseManager;

namespace opensis.data.Repository
{
    public class CourseManagerRepository : ICourseManagerRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public CourseManagerRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        /// <summary>
        /// Add Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        //public ProgramAddViewModel AddProgram(ProgramAddViewModel programAddViewModel)
        //{
        //    //int? ProgramId = Utility.GetMaxPK(this.context, new Func<Programs, int>(x => x.ProgramId));

        //    int? ProgramId = 0;

        //    var programData = this.context?.Programs.Where(x => x.SchoolId == programAddViewModel.programs.SchoolId && x.TenantId == programAddViewModel.programs.TenantId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

        //    if (programData != null)
        //    {
        //        ProgramId = programData.ProgramId + 1;
        //    }
        //    else
        //    {
        //        ProgramId = 1;
        //    }

        //    programAddViewModel.programs.ProgramId = (int)ProgramId;
        //    programAddViewModel.programs.CreatedOn = DateTime.UtcNow;
        //    this.context?.Programs.Add(programAddViewModel.programs);
        //    this.context?.SaveChanges();
        //    programAddViewModel._failure = false;

        //    return programAddViewModel;
        //}

        /// <summary>
        /// Get All Program
        /// </summary>
        /// <param name="programListViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel GetAllProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel programListModel = new ProgramListViewModel();
            try
            {

                var programList = this.context?.Programs.Where(x => x.TenantId == programListViewModel.TenantId && x.SchoolId == programListViewModel.SchoolId).ToList();

                programListModel.programList = programList;
                programListModel._tenantName = programListViewModel._tenantName;
                programListModel._token = programListViewModel._token;

                if (programList.Count > 0)
                {
                    programListModel._failure = false;
                }
                else
                {
                    programListModel._failure = true;
                    programListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                programListModel._message = es.Message;
                programListModel._failure = true;
                programListModel._tenantName = programListViewModel._tenantName;
                programListModel._token = programListViewModel._token;
            }
            return programListModel;
        }

        /// <summary>
        /// Add/Update Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramListViewModel AddEditProgram(ProgramListViewModel programListViewModel)
        {
            ProgramListViewModel programUpdateModel = new ProgramListViewModel();
            try
            {
                foreach (var programLists in programListViewModel.programList)
                {
                    if (programLists.ProgramId > 0)
                    {
                        var programUpdate = this.context?.Programs.FirstOrDefault(x => x.TenantId == programLists.TenantId && x.SchoolId == programLists.SchoolId && x.ProgramId == programLists.ProgramId);
                        if (programUpdate != null)
                        {
                            var program = this.context?.Programs.FirstOrDefault(x => x.TenantId == programLists.TenantId && x.SchoolId == programLists.SchoolId && x.ProgramId != programLists.ProgramId && x.ProgramName.ToLower() == programLists.ProgramName.ToLower());
                            if (program != null)
                            {
                                programUpdateModel._message = "Program Name Already Exists";
                                programUpdateModel._failure = true;
                                return programUpdateModel;
                            }
                            else
                            {
                                var courseList = this.context?.Course.Where(x => x.TenantId == programUpdate.TenantId && x.SchoolId == programUpdate.SchoolId && x.CourseProgram.ToLower() == programUpdate.ProgramName.ToLower()).ToList();

                                programLists.CreatedBy = programUpdate.CreatedBy;
                                programLists.CreatedOn = programUpdate.CreatedOn;
                                programLists.UpdatedOn = DateTime.UtcNow;
                                this.context.Entry(programUpdate).CurrentValues.SetValues(programLists);
                                this.context?.SaveChanges();

                                if (courseList.Count > 0)
                                {
                                    courseList.ForEach(x => x.CourseProgram = programLists.ProgramName);
                                }

                                programUpdateModel._message = "Program Updated Successfully";
                            }
                        }
                    }
                    else
                    {
                        int? ProgramId = 1;

                        var programData = this.context?.Programs.Where(x => x.SchoolId == programLists.SchoolId && x.TenantId == programLists.TenantId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                        if (programData != null)
                        {
                            ProgramId = programData.ProgramId + 1;
                        }

                        var program = this.context?.Programs.Where(x => x.SchoolId == programLists.SchoolId && x.TenantId == programLists.TenantId && x.ProgramName.ToLower() == programLists.ProgramName.ToLower()).FirstOrDefault();
                        if (program != null)
                        {
                            programUpdateModel._failure = true;
                            programUpdateModel._message = "Program Name Already Exists";
                            return programUpdateModel;
                        }
                        programLists.ProgramId = (int)ProgramId;
                        programLists.CreatedOn = DateTime.UtcNow;
                        this.context?.Programs.AddRange(programLists);
                        programUpdateModel._message = "Program Added Successfully";
                    }
                }
                this.context?.SaveChanges();
                programUpdateModel._failure = false;
            }
            catch (Exception es)
            {

                programUpdateModel._message = es.Message;
                programUpdateModel._failure = true;
                programUpdateModel._tenantName = programListViewModel._tenantName;
                programUpdateModel._token = programListViewModel._token;
            }
            return programUpdateModel;
        }

        /// <summary>
        /// Delete Program
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public ProgramAddViewModel DeleteProgram(ProgramAddViewModel programAddViewModel)
        {
            try
            {
                var programDelete = this.context?.Programs.FirstOrDefault(x => x.TenantId == programAddViewModel.programs.TenantId && x.SchoolId == programAddViewModel.programs.SchoolId && x.ProgramId == programAddViewModel.programs.ProgramId);
                var CourseList = this.context?.Course.Where(e => e.CourseProgram.ToLower() == programDelete.ProgramName.ToLower() && e.SchoolId == programDelete.SchoolId && e.TenantId == programDelete.TenantId).ToList();

                if (CourseList.Count > 0)
                {
                    programAddViewModel._message = "It Has Associationship";
                    programAddViewModel._failure = true;
                }
                else
                {
                    this.context?.Programs.Remove(programDelete);
                    this.context?.SaveChanges();
                    programAddViewModel._failure = false;
                    programAddViewModel._message = "Program Deleted Successfully";
                }
            }
            catch (Exception es)
            {
                programAddViewModel._failure = true;
                programAddViewModel._message = es.Message;
            }
            return programAddViewModel;
        }

        /// <summary>
        /// Add Subject
        /// </summary>
        /// <param name="subjectAddViewModel"></param>
        /// <returns></returns>
        //public SubjectAddViewModel AddSubject(SubjectAddViewModel subjectAddViewModel)
        //{
        //    try
        //    {
        //        //int? MasterSubjectId = Utility.GetMaxPK(this.context, new Func<Subject, int>(x => x.SubjectId));
        //        int? MasterSubjectId = 1;

        //        var subjectData = this.context?.Subject.Where(x => x.SchoolId == subjectAddViewModel.subject.SchoolId && x.TenantId == subjectAddViewModel.subject.TenantId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

        //        if (subjectData != null)
        //        {
        //            MasterSubjectId = subjectData.SubjectId + 1;
        //        }
        //        subjectAddViewModel.subject.SubjectId = (int)MasterSubjectId;
        //        subjectAddViewModel.subject.CreatedOn = DateTime.UtcNow;
        //        this.context?.Subject.Add(subjectAddViewModel.subject);
        //        this.context?.SaveChanges();
        //        subjectAddViewModel._failure = false;
        //    }
        //    catch (Exception es)
        //    {
        //        subjectAddViewModel._failure = true;
        //        subjectAddViewModel._message = es.Message;
        //    }
        //    return subjectAddViewModel;
        //}

        /// <summary>
        /// Add/Update Subject
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel AddEditSubject(SubjectListViewModel subjectListViewModel)
        {
            try
            {
                foreach (var subject in subjectListViewModel.subjectList)
                {
                    if (subject.SubjectId > 0)
                    {
                        var SubjectUpdate = this.context?.Subject.FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && x.SubjectId == subject.SubjectId);

                        if (SubjectUpdate != null)
                        {
                            var subjectName = this.context?.Subject.FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && x.SubjectName.ToLower() == subject.SubjectName.ToLower() && x.SubjectId != subject.SubjectId);

                            if (subjectName == null)
                            {
                                var sameSubjectNameExits = this.context?.Course.Where(x => x.SchoolId == SubjectUpdate.SchoolId && x.TenantId == SubjectUpdate.TenantId && x.CourseSubject.ToLower() == SubjectUpdate.SubjectName.ToLower()).ToList();

                                if (sameSubjectNameExits.Count > 0)
                                {
                                    sameSubjectNameExits.ForEach(x => x.CourseSubject = subject.SubjectName);
                                }

                                var subjectNameUsedInStaff = this.context?.StaffMaster.Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId && x.PrimarySubjectTaught.ToLower() == SubjectUpdate.SubjectName.ToLower()).ToList();

                                if (subjectNameUsedInStaff.Count() > 0)
                                {
                                    subjectNameUsedInStaff.ForEach(x => x.PrimarySubjectTaught = subject.SubjectName);
                                }

                                var StaffData = this.context?.StaffMaster.Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId && x.OtherSubjectTaught.ToLower().Contains(SubjectUpdate.SubjectName.ToLower())).ToList();

                                if (StaffData.Count() > 0)
                                {
                                    foreach (var staff in StaffData)
                                    {
                                        var otherSubjectTaught = staff.OtherSubjectTaught.Split(",");
                                        otherSubjectTaught = otherSubjectTaught.Where(w => w != SubjectUpdate.SubjectName).ToArray();
                                        var newOtherSubjectTaught = string.Join(",", otherSubjectTaught);
                                        newOtherSubjectTaught = newOtherSubjectTaught + "," + subject.SubjectName;
                                        staff.OtherSubjectTaught = newOtherSubjectTaught;
                                    }
                                }

                                subject.CreatedBy = SubjectUpdate.CreatedBy;
                                subject.CreatedOn = SubjectUpdate.CreatedOn;
                                subject.UpdatedOn = DateTime.UtcNow;
                                this.context.Entry(SubjectUpdate).CurrentValues.SetValues(subject);
                                this.context?.SaveChanges();
                                subjectListViewModel._message = "Subject Updated Successfully";
                            }
                            else
                            {
                                subjectListViewModel._failure = true;
                                subjectListViewModel._message = "Subject Name Already Exits";
                                return subjectListViewModel;
                            }
                        }
                        else
                        {
                            subjectListViewModel._failure = true;
                            subjectListViewModel._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        var subjectName = this.context?.Subject.FirstOrDefault(x => x.TenantId == subject.TenantId && x.SchoolId == subject.SchoolId && x.SubjectName.ToLower() == subject.SubjectName.ToLower());

                        if (subjectName == null)
                        {
                            int? SubjectId = 1;

                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == subject.SchoolId && x.TenantId == subject.TenantId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                            if (subjectData != null)
                            {
                                SubjectId = subjectData.SubjectId + 1;
                            }
                            subject.SubjectId = (int)SubjectId;
                            subject.CreatedOn = DateTime.UtcNow;
                            this.context?.Subject.Add(subject);
                        }
                        else
                        {
                            subjectListViewModel._failure = true;
                            subjectListViewModel._message = "Subject Name Already Exits";
                            return subjectListViewModel;
                        }

                        subjectListViewModel._message = "Subject Added Successfully";
                    }
                }
                this.context?.SaveChanges();
                subjectListViewModel._failure = false;
            }
            catch (Exception es)
            {
                subjectListViewModel._failure = true;
                subjectListViewModel._message = es.Message;
            }
            return subjectListViewModel;
        }

        /// <summary>
        /// Get All Subject List
        /// </summary>
        /// <param name="subjectListViewModel"></param>
        /// <returns></returns>
        public SubjectListViewModel GetAllSubjectList(SubjectListViewModel subjectListViewModel)
        {
            SubjectListViewModel subjectList = new SubjectListViewModel();
            try
            {
                var Subjectdata = this.context?.Subject.Where(x => x.TenantId == subjectListViewModel.TenantId && x.SchoolId == subjectListViewModel.SchoolId).Select(e=> new Subject()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    SubjectId=e.SubjectId,
                    SubjectName=e.SubjectName,
                    CreatedOn=e.CreatedOn,
                    UpdatedOn=e.UpdatedOn,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.EmailAddress == e.CreatedBy).Name : null,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == subjectListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null
                }).ToList();
                subjectList.subjectList = Subjectdata;
                subjectList._tenantName = subjectListViewModel._tenantName;
                subjectList._token = subjectListViewModel._token;

                if (Subjectdata.Count > 0)
                {
                    subjectList._failure = false;
                }
                else
                {
                    subjectList._failure = true;
                    subjectList._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                subjectList._message = es.Message;
                subjectList._failure = true;
                subjectList._tenantName = subjectListViewModel._tenantName;
                subjectList._token = subjectListViewModel._token;
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
            try
            {
                var subjectDelete = this.context?.Subject.FirstOrDefault(x => x.TenantId == subjectAddViewModel.subject.TenantId && x.SchoolId == subjectAddViewModel.subject.SchoolId && x.SubjectId == subjectAddViewModel.subject.SubjectId);

                if (subjectDelete != null)
                {
                    var courseExits = this.context?.Course.FirstOrDefault(x => x.TenantId == subjectDelete.TenantId && x.SchoolId == subjectDelete.SchoolId && x.CourseSubject == subjectDelete.SubjectName);
                    if (courseExits != null)
                    {
                        subjectAddViewModel._failure = true;
                        subjectAddViewModel._message = "Cannot delete because it has association.";
                    }
                    else
                    {
                        this.context?.Subject.Remove(subjectDelete);
                        this.context?.SaveChanges();
                        subjectAddViewModel._failure = false;
                        subjectAddViewModel._message = "Subject Deleted Successfully";
                    }
                }
            }
            catch (Exception es)
            {
                subjectAddViewModel._failure = true;
                subjectAddViewModel._message = es.Message;
            }
            return subjectAddViewModel;
        }

        /// <summary>
        /// Add Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel AddCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                if (courseAddViewModel.ProgramId == 0)
                {
                    int? ProgramId = 1;

                    var programData = this.context?.Programs.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                    if (programData != null)
                    {
                        ProgramId = programData.ProgramId + 1;
                    }

                    var programName = this.context?.Programs.FirstOrDefault(x => x.SchoolId == courseAddViewModel.course.SchoolId && x.TenantId == courseAddViewModel.course.TenantId && x.ProgramName.ToLower() == courseAddViewModel.course.CourseProgram.ToLower());
                    if (programName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Program Name Already Exists";
                        return courseAddViewModel;
                    }
                    var programAdd = new Programs() { TenantId = courseAddViewModel.course.TenantId, SchoolId = courseAddViewModel.course.SchoolId, ProgramId = (int)ProgramId, ProgramName = courseAddViewModel.course.CourseProgram, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.course.CreatedBy };
                    this.context?.Programs.Add(programAdd);
                }
                if (courseAddViewModel.SubjectId == 0)
                {
                    int? SubjectId = 1;

                    var subjectData = this.context?.Subject.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                    if (subjectData != null)
                    {
                        SubjectId = subjectData.SubjectId + 1;
                    }

                    var subjectName = this.context?.Subject.FirstOrDefault(x => x.SchoolId == courseAddViewModel.course.SchoolId && x.TenantId == courseAddViewModel.course.TenantId && x.SubjectName.ToLower() == courseAddViewModel.course.CourseSubject.ToLower());
                    if (subjectName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Subject Name Already Exists";
                        return courseAddViewModel;
                    }
                    var subjectAdd = new Subject() { TenantId = courseAddViewModel.course.TenantId, SchoolId = courseAddViewModel.course.SchoolId, SubjectId = (int)SubjectId, SubjectName = courseAddViewModel.course.CourseSubject, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.course.CreatedBy };
                    this.context?.Subject.Add(subjectAdd);
                }

                var courseTitle = this.context?.Course.FirstOrDefault(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId && x.CourseTitle.ToLower() == courseAddViewModel.course.CourseTitle.ToLower());

                if (courseTitle == null)
                {
                    int? CourseId = 1;

                    var courseData = this.context?.Course.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId).OrderByDescending(x => x.CourseId).FirstOrDefault();

                    if (courseData != null)
                    {
                        CourseId = courseData.CourseId + 1;
                    }
                    courseAddViewModel.course.CourseId = (int)CourseId;
                    courseAddViewModel.course.CreatedOn = DateTime.UtcNow;
                    courseAddViewModel.course.IsCourseActive = true;

                    if (courseAddViewModel.course.CourseStandard.ToList().Count > 0)
                    {
                        courseAddViewModel.course.CourseStandard.ToList().ForEach(x => x.CreatedOn = DateTime.UtcNow);
                    }

                    this.context?.Course.Add(courseAddViewModel.course);
                }
                else
                {
                    courseAddViewModel._failure = true;
                    courseAddViewModel._message = "Course Title Already Exits";
                    return courseAddViewModel;
                }
                this.context.SaveChanges();
                courseAddViewModel._failure = false;
                courseAddViewModel._message = "Course Added Successfully";
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Update Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel UpdateCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                if (courseAddViewModel.ProgramId == 0)
                {
                    int? ProgramId = 1;

                    var programData = this.context?.Programs.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId).OrderByDescending(x => x.ProgramId).FirstOrDefault();

                    if (programData != null)
                    {
                        ProgramId = programData.ProgramId + 1;
                    }

                    var programName = this.context?.Programs.FirstOrDefault(x => x.SchoolId == courseAddViewModel.course.SchoolId && x.TenantId == courseAddViewModel.course.TenantId && x.ProgramName.ToLower() == courseAddViewModel.course.CourseProgram.ToLower());

                    if (programName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Program Name Already Exists";
                        return courseAddViewModel;
                    }

                    var programAdd = new Programs() { TenantId = courseAddViewModel.course.TenantId, SchoolId = courseAddViewModel.course.SchoolId, ProgramId = (int)ProgramId, ProgramName = courseAddViewModel.course.CourseProgram, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.course.CreatedBy };
                    this.context?.Programs.Add(programAdd);
                }

                if (courseAddViewModel.SubjectId == 0)
                {
                    int? SubjectId = 1;

                    var subjectData = this.context?.Subject.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId).OrderByDescending(x => x.SubjectId).FirstOrDefault();

                    if (subjectData != null)
                    {
                        SubjectId = subjectData.SubjectId + 1;
                    }

                    var subjectName = this.context?.Subject.FirstOrDefault(x => x.SchoolId == courseAddViewModel.course.SchoolId && x.TenantId == courseAddViewModel.course.TenantId && x.SubjectName.ToLower() == courseAddViewModel.course.CourseSubject.ToLower());
                    if (subjectName != null)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Subject Name Already Exists";
                        return courseAddViewModel;
                    }

                    var subjectAdd = new Subject() { TenantId = courseAddViewModel.course.TenantId, SchoolId = courseAddViewModel.course.SchoolId, SubjectId = (int)SubjectId, SubjectName = courseAddViewModel.course.CourseSubject, CreatedOn = DateTime.UtcNow, CreatedBy = courseAddViewModel.course.CreatedBy };
                    this.context?.Subject.Add(subjectAdd);
                }

                var courseUpdate = this.context?.Course.Include(x => x.CourseStandard).FirstOrDefault(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId && x.CourseId == courseAddViewModel.course.CourseId);

                if (courseUpdate != null)
                {
                    var courseTitle = this.context?.Course.FirstOrDefault(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId && x.CourseTitle.ToLower() == courseAddViewModel.course.CourseTitle.ToLower() && x.CourseId != courseAddViewModel.course.CourseId);

                    if (courseTitle == null)
                    {
                        courseAddViewModel.course.CreatedBy = courseUpdate.CreatedBy;
                        courseAddViewModel.course.CreatedOn = courseUpdate.CreatedOn;
                        courseAddViewModel.course.UpdatedOn = DateTime.UtcNow;
                        this.context.Entry(courseUpdate).CurrentValues.SetValues(courseAddViewModel.course);
                        courseAddViewModel._message = "Course Updated Successfully";

                        if (courseAddViewModel.course.CourseStandard.ToList().Count > 0)
                        {
                            this.context?.CourseStandard.RemoveRange(courseUpdate.CourseStandard);
                            courseAddViewModel.course.CourseStandard.ToList().ForEach(x => x.UpdatedOn = DateTime.UtcNow);
                            this.context?.CourseStandard.AddRange(courseAddViewModel.course.CourseStandard);
                        }
                        if (courseUpdate.CourseStandard.Count > 0 && courseAddViewModel.course.CourseStandard.ToList().Count == 0)
                        {
                            this.context?.CourseStandard.RemoveRange(courseUpdate.CourseStandard);
                        }
                    }
                    else
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Course Title Already Exits";
                        return courseAddViewModel;
                    }
                }
                this.context.SaveChanges();
                courseAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Delete Course
        /// </summary>
        /// <param name="courseAddViewModel"></param>
        /// <returns></returns>
        public CourseAddViewModel DeleteCourse(CourseAddViewModel courseAddViewModel)
        {
            try
            {
                var courseDelete = this.context?.Course.Include(x => x.CourseStandard).Include(x => x.CourseSection).FirstOrDefault(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId && x.CourseId == courseAddViewModel.course.CourseId);
                var courseCommentCategory = this.context?.CourseCommentCategory.Where(x => x.TenantId == courseAddViewModel.course.TenantId && x.SchoolId == courseAddViewModel.course.SchoolId && x.CourseId == courseAddViewModel.course.CourseId).ToList();

                if (courseDelete != null)
                {
                    if (courseDelete.CourseSection.Count > 0 || courseCommentCategory.Count > 0)
                    {
                        courseAddViewModel._failure = true;
                        courseAddViewModel._message = "Course Can't be deleted because it has association";
                        return courseAddViewModel;
                    }
                    this.context?.CourseStandard.RemoveRange(courseDelete.CourseStandard);
                    this.context?.Course.Remove(courseDelete);
                    this.context?.SaveChanges();
                    courseAddViewModel._failure = false;
                    courseAddViewModel._message = "Deleted Successfully";
                }
                else
                {
                    courseAddViewModel._failure = true;
                    courseAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseAddViewModel._failure = true;
                courseAddViewModel._message = es.Message;
            }
            return courseAddViewModel;
        }

        /// <summary>
        /// Get All Course List
        /// </summary>
        /// <param name="courseListViewModel"></param>
        /// <returns></returns>
        public CourseListViewModel GetAllCourseList(CourseListViewModel courseListViewModel)
        {
            CourseListViewModel courseListModel = new CourseListViewModel();
            try
            {
                var courseRecords = this.context?.Course.Include(x => x.CourseSection).Include(e => e.CourseStandard).ThenInclude(c => c.GradeUsStandard).Where(x => x.TenantId == courseListViewModel.TenantId && x.SchoolId == courseListViewModel.SchoolId).Select(e=> new Course()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    CourseId=e.CourseId,
                    CourseTitle=e.CourseTitle,
                    CourseShortName=e.CourseShortName,
                    CourseGradeLevel=e.CourseGradeLevel,
                    CourseProgram=e.CourseProgram,
                    CourseSubject=e.CourseSubject,
                    CourseCategory=e.CourseCategory,
                    CreditHours=e.CreditHours,
                    CourseDescription=e.CourseDescription,
                    IsCourseActive=e.IsCourseActive,
                    Standard=e.Standard,
                    StandardRefNo=e.StandardRefNo,
                    UpdatedOn=e.UpdatedOn,
                    CreatedOn=e.CreatedOn,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    CourseSection= e.CourseSection.Select(w=> new CourseSection()
                    { 
                        TenantId=w.TenantId,
                        SchoolId= w.SchoolId,
                        CourseId=w.CourseId,
                        CourseSectionId=w.CourseSectionId,
                        GradeScaleId=w.GradeScaleId,
                        CourseSectionName=w.CourseSectionName,
                        CalendarId=w.CalendarId,
                        AttendanceCategoryId=w.AttendanceCategoryId,
                        CreditHours=w.CreditHours,
                        Seats=w.Seats,
                        IsWeightedCourse=w.IsWeightedCourse,
                        AffectsClassRank=w.AffectsClassRank,
                        AffectsHonorRoll=w.AffectsHonorRoll,
                        OnlineClassRoom=w.OnlineClassRoom,
                        OnlineClassroomUrl=w.OnlineClassroomUrl,
                        OnlineClassroomPassword=w.OnlineClassroomPassword,
                        UseStandards=w.UseStandards,
                        StandardGradeScaleId=w.StandardGradeScaleId,
                        DurationBasedOnPeriod=w.DurationBasedOnPeriod,
                        DurationStartDate=w.DurationStartDate,
                        DurationEndDate=w.DurationEndDate,
                        ScheduleType=w.ScheduleType,
                        MeetingDays=w.MeetingDays,
                        AttendanceTaken=w.AttendanceTaken,
                        IsActive=w.IsActive,
                        YrMarkingPeriodId=w.YrMarkingPeriodId,
                        SmstrMarkingPeriodId=w.SmstrMarkingPeriodId,
                        QtrMarkingPeriodId=w.QtrMarkingPeriodId,
                        AcademicYear=w.AcademicYear,
                        GradeScaleType=w.GradeScaleType,
                        AllowStudentConflict=w.AllowStudentConflict,
                        AllowTeacherConflict=w.AllowTeacherConflict,
                        CreatedBy= (w.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == w.CreatedBy).Name : null,
                        CreatedOn=w.CreatedOn,
                        UpdatedBy= (w.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == w.UpdatedBy).Name : null,
                        UpdatedOn= w.UpdatedOn
                    }).ToList(),
                    CourseStandard= e.CourseStandard.Select(t=> new CourseStandard()
                    { 
                        TenantId= t.TenantId,
                        SchoolId=t.SchoolId,
                        CourseId=t.CourseId,
                        StandardRefNo=t.StandardRefNo,
                        CreatedBy= (t.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == t.CreatedBy).Name : null,
                        CreatedOn=t.CreatedOn,
                        UpdatedBy= (t.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == t.UpdatedBy).Name : null,
                        UpdatedOn=t.UpdatedOn,
                        GradeUsStandard= new GradeUsStandard()
                        {
                            TenantId=t.GradeUsStandard.TenantId,
                            SchoolId= t.GradeUsStandard.SchoolId,
                            StandardRefNo=t.StandardRefNo,
                            GradeLevel= t.GradeUsStandard.GradeLevel,
                            Domain=t.GradeUsStandard.Domain,
                            Subject=t.GradeUsStandard.Subject,
                            Course=t.GradeUsStandard.Course,
                            Topic=t.GradeUsStandard.Topic,
                            StandardDetails=t.GradeUsStandard.StandardDetails,
                            GradeStandardId=t.GradeUsStandard.GradeStandardId,
                            IsSchoolSpecific=t.GradeUsStandard.IsSchoolSpecific,
                            CreatedBy= (t.GradeUsStandard.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == t.GradeUsStandard.CreatedBy).Name : null,
                            CreatedOn=t.GradeUsStandard.CreatedOn,
                            UpdatedBy= (t.GradeUsStandard.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseListViewModel.TenantId && u.EmailAddress == t.GradeUsStandard.UpdatedBy).Name : null,
                            UpdatedOn= t.GradeUsStandard.UpdatedOn
                        }
                    }).ToList()
                }).ToList();
                if (courseRecords.Count > 0)
                {
                    foreach (var course in courseRecords)
                    {
                        CourseViewModel courseViewModel = new CourseViewModel { course = course, CourseSectionCount = course.CourseSection.Count() };
                        courseListModel.courseViewModelList.Add(courseViewModel);
                    }
                    courseListModel.CourseCount = courseRecords.Count();

                    courseListModel._failure = false;
                }
                else
                {
                    courseListModel.CourseCount = null;
                    courseListModel._failure = true;
                    courseListModel._message = NORECORDFOUND;
                }
                courseListModel._tenantName = courseListViewModel._tenantName;
                courseListModel._token = courseListViewModel._token;
            }
            catch (Exception es)
            {
                courseListModel.courseViewModelList = null;
                courseListModel._message = es.Message;
                courseListModel._failure = true;
                courseListModel._tenantName = courseListViewModel._tenantName;
                courseListModel._token = courseListViewModel._token;
            }
            return courseListModel;
        }

        /// <summary>
        /// Add Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel AddCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    decimal? academicYear = null;

                    var CalenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.CalenderId == courseSectionAddViewModel.courseSection.CalendarId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.TenantId == courseSectionAddViewModel.courseSection.TenantId);

                    if (CalenderData != null)
                    {
                        academicYear = (decimal)CalenderData.AcademicYear;

                        if (!(bool)courseSectionAddViewModel.courseSection.DurationBasedOnPeriod)
                        {
                            if (CalenderData.StartDate > courseSectionAddViewModel.courseSection.DurationStartDate || CalenderData.EndDate < courseSectionAddViewModel.courseSection.DurationEndDate)
                            {
                                courseSectionAddViewModel._message = "Start Date And End Date of Course Section Should Be Between Start Date And End Date of School Calender";
                                courseSectionAddViewModel._failure = true;
                                return courseSectionAddViewModel;
                            }
                        }
                        var checkCourseSectionName = this.context?.CourseSection.Where(x => x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.CourseSectionName.ToLower() == courseSectionAddViewModel.courseSection.CourseSectionName.ToLower() && x.AcademicYear == academicYear).FirstOrDefault();

                        if (checkCourseSectionName != null)
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = "Course Section Name Already Exists";
                            return courseSectionAddViewModel;
                        }

                        int? CourseSectionId = 1;

                        var CourseSectionData = this.context?.CourseSection.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId /*&& x.AcademicYear== academicYear*/).OrderByDescending(x => x.CourseSectionId).FirstOrDefault();


                        if (CourseSectionData != null)
                        {
                            CourseSectionId = CourseSectionData.CourseSectionId + 1;
                        }

                        if (courseSectionAddViewModel.MarkingPeriodId != null)
                        {
                            string[] markingPeriodID = courseSectionAddViewModel.MarkingPeriodId.Split("_");

                            if (markingPeriodID.First() == "0")
                            {
                                courseSectionAddViewModel.courseSection.YrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }

                            if (markingPeriodID.First() == "1")
                            {
                                courseSectionAddViewModel.courseSection.SmstrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }

                            if (markingPeriodID.First() == "2")
                            {
                                courseSectionAddViewModel.courseSection.QtrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }
                        }
                        courseSectionAddViewModel._message = "Course Section Added Successfully.";

                        switch (courseSectionAddViewModel.courseSection.ScheduleType.ToLower())
                        {
                            case "fixedschedule":

                                if (AddFixedSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                                {
                                    return courseSectionAddViewModel;
                                }
                                break;

                            case "variableschedule":

                                if (AddVariableSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                                {
                                    return courseSectionAddViewModel;
                                }
                                break;

                            case "calendarschedule":

                                if (AddCalendarSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                                {
                                    return courseSectionAddViewModel;
                                }
                                break;

                            case "blockschedule":

                                if (AddBlockSchedule(courseSectionAddViewModel, CourseSectionId, academicYear)._failure)
                                {
                                    return courseSectionAddViewModel;
                                }
                                break;

                            default:
                                courseSectionAddViewModel._failure = true;
                                courseSectionAddViewModel._message = "Please Provide a Valid Schedule Type.";
                                return courseSectionAddViewModel;
                        }

                        courseSectionAddViewModel.courseSection.CourseSectionId = (int)CourseSectionId;
                        courseSectionAddViewModel.courseSection.AcademicYear = academicYear;
                        courseSectionAddViewModel.courseSection.CreatedOn = DateTime.UtcNow;
                        this.context?.CourseSection.Add(courseSectionAddViewModel.courseSection);
                        this.context?.SaveChanges();

                        courseSectionAddViewModel._failure = false;
                        transaction.Commit();
                    }
                    else
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "School Calender does not exist";
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    courseSectionAddViewModel._message = es.Message;
                    courseSectionAddViewModel._failure = true;
                }
                return courseSectionAddViewModel;
            }
        }

        /// <summary>
        /// Get All CourseSection
        /// </summary>
        /// <param name="courseSectionViewModel"></param>
        /// <returns></returns>
        public CourseSectionViewModel GetAllCourseSection(CourseSectionViewModel courseSectionViewModel)
        {
            CourseSectionViewModel courseSectionView = new CourseSectionViewModel();
            try
            {
                var courseSectionData = this.context?.CourseSection.Include(x => x.Course).Include(x => x.AttendanceCodeCategories).Include(x => x.GradeScale).Include(x => x.SchoolCalendars).Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Where(x => x.TenantId == courseSectionViewModel.TenantId && x.SchoolId == courseSectionViewModel.SchoolId && x.CourseId == courseSectionViewModel.CourseId && x.AcademicYear == courseSectionViewModel.AcademicYear).ToList().Select(cs => new CourseSection
                {
                    TenantId = cs.TenantId,
                    SchoolId = cs.SchoolId,
                    CourseId = cs.CourseId,
                    CourseSectionId = cs.CourseSectionId,
                    GradeScaleId = cs.GradeScaleId,
                    CourseSectionName = cs.CourseSectionName,
                    CalendarId = cs.CalendarId,
                    AttendanceCategoryId = cs.AttendanceCategoryId,
                    CreditHours = cs.CreditHours,
                    Seats = cs.Seats,
                    IsWeightedCourse = cs.IsWeightedCourse,
                    AffectsClassRank = cs.AffectsClassRank,
                    AffectsHonorRoll = cs.AffectsHonorRoll,
                    OnlineClassRoom = cs.OnlineClassRoom,
                    OnlineClassroomUrl = cs.OnlineClassroomUrl,
                    OnlineClassroomPassword = cs.OnlineClassroomPassword,
                    StandardGradeScaleId = cs.StandardGradeScaleId,
                    UseStandards = cs.UseStandards,
                    DurationBasedOnPeriod = cs.DurationBasedOnPeriod,
                    DurationStartDate = cs.DurationStartDate,
                    DurationEndDate = cs.DurationEndDate,
                    ScheduleType = cs.ScheduleType,
                    MeetingDays = cs.MeetingDays,
                    AttendanceTaken = cs.AttendanceTaken,
                    IsActive = cs.IsActive,
                    CreatedBy = (cs.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == cs.CreatedBy).Name : null,
                    CreatedOn = cs.CreatedOn,
                    UpdatedBy = (cs.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == cs.UpdatedBy).Name : null,
                    UpdatedOn = cs.UpdatedOn,
                    QtrMarkingPeriodId = cs.QtrMarkingPeriodId,
                    SmstrMarkingPeriodId = cs.SmstrMarkingPeriodId,
                    YrMarkingPeriodId = cs.YrMarkingPeriodId,
                    AcademicYear = cs.AcademicYear,
                    GradeScaleType = cs.GradeScaleType,
                    AllowStudentConflict = cs.AllowStudentConflict,
                    AllowTeacherConflict = cs.AllowTeacherConflict,
                    Course = new Course { CourseTitle = cs.Course.CourseTitle, CourseProgram = cs.Course.CourseProgram, CourseSubject = cs.Course.CourseSubject },
                    GradeScale = cs.GradeScale != null ? new GradeScale { GradeScaleName = cs.GradeScale.GradeScaleName } : null,
                    AttendanceCodeCategories = cs.AttendanceCodeCategories != null ? new AttendanceCodeCategories { Title = cs.AttendanceCodeCategories.Title } : null,
                    SchoolCalendars = cs.SchoolCalendars != null ? new SchoolCalendars { TenantId = cs.SchoolCalendars.TenantId, SchoolId = cs.SchoolCalendars.SchoolId, CalenderId = cs.SchoolCalendars.CalenderId, Title = cs.SchoolCalendars.Title, StartDate = cs.SchoolCalendars.StartDate, EndDate = cs.SchoolCalendars.EndDate, AcademicYear = cs.SchoolCalendars.AcademicYear, DefaultCalender = cs.SchoolCalendars.DefaultCalender, Days = cs.SchoolCalendars.Days, RolloverId = cs.SchoolCalendars.RolloverId, VisibleToMembershipId = cs.SchoolCalendars.VisibleToMembershipId, UpdatedOn = cs.SchoolCalendars.UpdatedOn, UpdatedBy = (cs.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == cs.UpdatedBy).Name : null,
                        CreatedBy = (cs.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == cs.CreatedBy).Name : null,
                        CreatedOn = cs.CreatedOn,
                    } : null,
                    Quarters = cs.Quarters != null ? new Quarters { Title = cs.Quarters.Title, StartDate = cs.Quarters.StartDate, EndDate = cs.Quarters.EndDate, } : null,
                    Semesters = cs.Semesters != null ? new Semesters { Title = cs.Semesters.Title, StartDate = cs.Semesters.StartDate, EndDate = cs.Semesters.EndDate, } : null,
                    SchoolYears = cs.SchoolYears != null ? new SchoolYears { Title = cs.SchoolYears.Title, StartDate = cs.SchoolYears.StartDate, EndDate = cs.SchoolYears.EndDate, } : null,

                });

                if (courseSectionData.Count() > 0)
                {
                    string markingId = null;
                    string standardGradeScaleName = null;
                    foreach (var courseSection in courseSectionData)
                    {
                        int? totalStaff = null;
                        int? totalStudent = null;
                        string staffFullName = null;
                        if (courseSection.UseStandards == true)
                        {
                            //var gradeUsStandardData = this.context?.GradeUsStandard.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.GradeStandardId == courseSection.StandardGradeScaleId).FirstOrDefault();
                            //if (gradeUsStandardData != null)
                            //{
                            //    standardRefNo = gradeUsStandardData.StandardRefNo;
                            //}

                            var StandardData = this.context?.GradeScale.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.GradeScaleId == courseSection.StandardGradeScaleId).FirstOrDefault();
                            if (StandardData != null)
                            {
                                standardGradeScaleName = StandardData.GradeScaleName;
                            }
                        }
                        else
                        {
                            standardGradeScaleName = null;
                        }
                        if (courseSection.YrMarkingPeriodId != null)
                        {
                            markingId = "0" + "_" + courseSection.YrMarkingPeriodId;
                        }
                        if (courseSection.SmstrMarkingPeriodId != null)
                        {
                            markingId = "1" + "_" + courseSection.SmstrMarkingPeriodId;
                        }
                        if (courseSection.QtrMarkingPeriodId != null)
                        {
                            markingId = "2" + "_" + courseSection.QtrMarkingPeriodId;
                        }
                        if (courseSection.QtrMarkingPeriodId == null && courseSection.SmstrMarkingPeriodId == null && courseSection.YrMarkingPeriodId == null)
                        {
                            markingId = null;
                        }

                        //totalStaff = this.context.StaffCoursesectionSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != false).ToList().Count();

                        totalStudent = this.context.StudentCoursesectionSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != true).ToList().Count;

                        var staffData = this.context.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId && x.IsDropped != true).ToList();
                        if (staffData.Count > 0)
                        {
                            staffFullName = staffData.FirstOrDefault().StaffMaster.FirstGivenName + " " + staffData.FirstOrDefault().StaffMaster.MiddleName + " " + staffData.FirstOrDefault().StaffMaster.LastFamilyName;
                            totalStaff = staffData.Count;
                        }

                        if (courseSection.ScheduleType.ToLower() == "Fixed Schedule (1)".ToLower())
                        {
                            var fixedScheduleData = this.context?.CourseFixedSchedule.Include(f => f.Rooms).Include(f => f.BlockPeriod).Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Select(s => new CourseFixedSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null, CreatedOn = s.CreatedOn, UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod.PeriodTitle } }).FirstOrDefault();
                            //if (fixedScheduleData != null)
                            //{
                            courseSection.ScheduleType = "Fixed Schedule";
                            GetCourseSectionForView getFixedSchedule = new GetCourseSectionForView
                            {
                                courseFixedSchedule = fixedScheduleData,
                                courseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName
                            };
                            courseSectionView.getCourseSectionForView.Add(getFixedSchedule);
                            //}
                        }

                        if (courseSection.ScheduleType.ToLower() == "Variable Schedule (2)".ToLower())
                        {
                            var variableScheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Select(s => new CourseVariableSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Day = s.Day, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null, CreatedOn = s.CreatedOn, UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod.PeriodTitle } }).ToList();

                            //if (variableScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Variable Schedule";
                            GetCourseSectionForView getVariableSchedule = new GetCourseSectionForView
                            {
                                courseVariableSchedule = variableScheduleData,
                                courseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName
                            };
                            courseSectionView.getCourseSectionForView.Add(getVariableSchedule);
                            //}
                        }

                        if (courseSection.ScheduleType.ToLower() == "Calendar Schedule (3)".ToLower())
                        {
                            var calendarScheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Select(s => new CourseCalendarSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, Date = s.Date, RoomId = s.RoomId, TakeAttendance = s.TakeAttendance, PeriodId = s.PeriodId, BlockId = s.BlockId, CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null, CreatedOn = s.CreatedOn, UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod.PeriodTitle } }).ToList();
                            //if (calendarScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Calendar Schedule";
                            GetCourseSectionForView getCalendarSchedule = new GetCourseSectionForView
                            {
                                courseCalendarSchedule = calendarScheduleData,
                                courseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName
                            };
                            courseSectionView.getCourseSectionForView.Add(getCalendarSchedule);
                            //}
                        }

                        if (courseSection.ScheduleType.ToLower() == "Block Schedule (4)".ToLower())
                        {
                            var blockScheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSection.TenantId && x.SchoolId == courseSection.SchoolId && x.CourseId == courseSection.CourseId && x.CourseSectionId == courseSection.CourseSectionId).Include(f => f.Rooms).Include(f => f.BlockPeriod).Include(f => f.Block).Select(s => new CourseBlockSchedule { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseSectionId = s.CourseSectionId, GradeScaleId = s.GradeScaleId, Serial = s.Serial, RoomId = s.RoomId, PeriodId = s.PeriodId, BlockId = s.BlockId, TakeAttendance = s.TakeAttendance, CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null, CreatedOn = s.CreatedOn, UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseSectionViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null, UpdatedOn = s.UpdatedOn, Rooms = new Rooms { Title = s.Rooms.Title }, BlockPeriod = new BlockPeriod { PeriodTitle = s.BlockPeriod.PeriodTitle }, Block = new Block { BlockTitle = s.Block.BlockTitle } }).ToList();
                            //if (blockScheduleData.Count > 0)
                            //{
                            courseSection.ScheduleType = "Block/Rotating Schedule";
                            GetCourseSectionForView getBlockSchedule = new GetCourseSectionForView
                            {
                                courseBlockSchedule = blockScheduleData,
                                courseSection = courseSection,
                                MarkingPeriod = markingId,
                                StandardGradeScaleName = standardGradeScaleName,
                                TotalStaffSchedule = totalStaff,
                                TotalStudentSchedule = totalStudent,
                                AvailableSeat = courseSection.Seats - totalStudent,
                                StaffName = staffFullName
                            };
                            courseSectionView.getCourseSectionForView.Add(getBlockSchedule);
                            //}
                        }
                    }
                    courseSectionView.TenantId = courseSectionViewModel.TenantId;
                    courseSectionView.SchoolId = courseSectionViewModel.SchoolId;
                    courseSectionView.CourseId = courseSectionViewModel.CourseId;
                    courseSectionView.AcademicYear = courseSectionViewModel.AcademicYear;
                    courseSectionView._tenantName = courseSectionViewModel._tenantName;
                    courseSectionView._token = courseSectionViewModel._token;
                    courseSectionView._failure = false;
                }
                else
                {
                    courseSectionView._failure = true;
                    courseSectionView._message = NORECORDFOUND;
                    courseSectionView._tenantName = courseSectionViewModel._tenantName;
                    courseSectionView._token = courseSectionViewModel._token;
                }
            }
            catch (Exception es)
            {
                courseSectionView.getCourseSectionForView = null;
                courseSectionView._failure = true;
                courseSectionView._message = es.Message;
            }
            return courseSectionView;
        }

        /// <summary>
        /// Add Fixed Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddFixedSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.courseFixedSchedule != null)
                {
                    var courseSectionList = this.context?.AllCourseSectionView.AsEnumerable().Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId &&
                       ((c.FixedPeriodId != null && (c.FixedRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.FixedPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && (Regex.IsMatch(courseSectionAddViewModel.courseSection.MeetingDays.ToLower(), c.FixedDays.ToLower(), RegexOptions.IgnoreCase)))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.VarPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && courseSectionAddViewModel.courseSection.MeetingDays.ToLower().Contains(c.VarDay.ToLower()))) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.CalPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && courseSectionAddViewModel.courseSection.MeetingDays.ToLower().Contains(c.CalDay.ToLower()))))
                       && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                    if (courseSectionList.Count > 0)
                    {
                        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                        courseSectionAddViewModel._failure = true;
                        return courseSectionAddViewModel;
                    }

                    //string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");
                    //string message = null;

                    //foreach (var meetingDay in meetingDays)
                    //{ 
                    //   var courseSectionList = this.context?.CourseSection.
                    //                Join(this.context?.CourseFixedSchedule,
                    //                cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                    //                (cs, cfs) => new { cs, cfs }).Where(c=>c.cs.TenantId==courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId==courseSectionAddViewModel.courseSection.SchoolId && c.cfs.RoomId== courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cfs.PeriodId==courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.MeetingDays.Contains(meetingDay) && c.cs.AcademicYear== academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                    //    if (courseSectionList.Count>0)
                    //    {
                    //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                    //        courseSectionAddViewModel._failure = true;
                    //        return courseSectionAddViewModel;
                    //    }

                    //    Days days = new Days();
                    //    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(meetingDay));

                    //    var variableScheduleDataList = this.context?.CourseSection.
                    //                Join(this.context?.CourseVariableSchedule,
                    //                cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                    //                (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cvs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cvs.Day.ToLower()==Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                    //    if (variableScheduleDataList.Count > 0)
                    //    {
                    //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                    //        courseSectionAddViewModel._failure = true;
                    //        return courseSectionAddViewModel;
                    //    }

                    //    var calenderScheduleList = this.context?.CourseSection.
                    //                Join(this.context?.CourseCalendarSchedule,
                    //                cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                    //                (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.ccs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId  && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                    //    if (calenderScheduleList.Count > 0)
                    //    {
                    //        foreach (var calenderSchedule in calenderScheduleList)
                    //        {
                    //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                    //            if (calenderDay != null)
                    //            {
                    //                int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);                                  

                    //                if (dayValue.ToString() == meetingDay)
                    //                {
                    //                    if (message != null)
                    //                    {
                    //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                    //                    }
                    //                    else
                    //                    {
                    //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                    //                    }
                    //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                    //                    courseSectionAddViewModel._failure = true;
                    //                    return courseSectionAddViewModel;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    var roomCapacity = this.context?.Rooms.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId)?.Capacity;

                    if (roomCapacity == null || roomCapacity < courseSectionAddViewModel.courseSection.Seats)
                    {
                        courseSectionAddViewModel._message = "Invalid Seat Capacity";
                        courseSectionAddViewModel._failure = true;
                        return courseSectionAddViewModel;
                    }
                    else
                    {
                        int? fixedscheduleSerial = 1;

                        var CourseSectionfixedscheduleData = this.context?.CourseFixedSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                        if (CourseSectionfixedscheduleData != null)
                        {
                            fixedscheduleSerial = CourseSectionfixedscheduleData.Serial + 1;
                        }

                        var courseFixedSchedule = new CourseFixedSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)fixedscheduleSerial,
                            RoomId = courseSectionAddViewModel.courseFixedSchedule.RoomId,
                            PeriodId = courseSectionAddViewModel.courseFixedSchedule.PeriodId,
                            CreatedBy = courseSectionAddViewModel.courseFixedSchedule.CreatedBy,
                            CreatedOn = DateTime.UtcNow,
                            BlockId = courseSectionAddViewModel.courseFixedSchedule.BlockId
                        };

                        this.context?.CourseFixedSchedule.Add(courseFixedSchedule);
                        courseSectionAddViewModel.courseSection.ScheduleType = "Fixed Schedule (1)";
                        courseSectionAddViewModel._failure = false;
                    }
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Variable Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddVariableSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseVariableSchedule> courseVariableScheduleList = new List<CourseVariableSchedule>();
                string message = null;

                if (courseSectionAddViewModel.courseVariableScheduleList.Count > 0)
                {
                    int? variablescheduleSerial = 1;

                    var CourseSectionvariablescheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionvariablescheduleData != null)
                    {
                        variablescheduleSerial = CourseSectionvariablescheduleData.Serial + 1;
                        //variablescheduleSerial = CourseSectionvariablescheduleData.cvs.Serial + 1;
                    }

                    foreach (var courseVariableSchedules in courseSectionAddViewModel.courseVariableScheduleList)
                    {

                        var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId &&
                            ((c.FixedPeriodId != null && (c.FixedRoomId == courseVariableSchedules.RoomId && c.FixedPeriodId == courseVariableSchedules.PeriodId && c.FixedDays.Contains(courseVariableSchedules.Day))) ||
                            (c.VarPeriodId != null && (c.VarRoomId == courseVariableSchedules.RoomId && c.VarPeriodId == courseVariableSchedules.PeriodId && c.VarDay == courseVariableSchedules.Day)) ||
                            (c.CalPeriodId != null && (c.CalRoomId == courseVariableSchedules.RoomId && c.CalPeriodId == courseVariableSchedules.PeriodId && c.CalDay == courseVariableSchedules.Day)))
                            && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        if (courseSectionList.Count > 0)
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        //Days days = new Days();

                        //var Day = Enum.GetName(days.GetType(), Convert.ToInt32(courseVariableSchedules.Day));                        

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //            Join(this.context?.CourseVariableSchedule,
                        //            cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //            (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseVariableSchedules.RoomId && c.cvs.PeriodId == courseVariableSchedules.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseVariableSchedules.RoomId && c.cf.PeriodId == courseVariableSchedules.PeriodId && c.cs.MeetingDays.Contains(courseVariableSchedules.Day) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (courseSectionList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //    var calenderScheduleList = this.context?.CourseSection.
                        //            Join(this.context?.CourseCalendarSchedule,
                        //            cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //            (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseVariableSchedules.RoomId && c.ccs.PeriodId == courseVariableSchedules.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();


                        //    if (calenderScheduleList.Count>0)
                        //    {

                        //        foreach (var calenderSchedule in calenderScheduleList)
                        //        {
                        //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //            if (calenderDay != null)
                        //            {
                        //                int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //                if (dayValue.ToString() == courseVariableSchedules.Day)
                        //                {
                        //                    if (message != null)
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                    }
                        //                    else
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                    }
                        //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                    courseSectionAddViewModel._failure = true;
                        //                    return courseSectionAddViewModel;
                        //                }
                        //            }
                        //        }

                        //    }

                        var courseeVariableSchedule = new CourseVariableSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)variablescheduleSerial,
                            Day = courseVariableSchedules.Day,
                            PeriodId = courseVariableSchedules.PeriodId,
                            RoomId = courseVariableSchedules.RoomId,
                            TakeAttendance = courseVariableSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow,
                            BlockId = courseVariableSchedules.BlockId

                        };
                        courseVariableScheduleList.Add(courseeVariableSchedule);
                        variablescheduleSerial++;
                    }
                    this.context?.CourseVariableSchedule.AddRange(courseVariableScheduleList);
                    courseSectionAddViewModel.courseSection.ScheduleType = "Variable Schedule (2)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Calendar Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddCalendarSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseCalendarSchedule> courseCalendarScheduleList = new List<CourseCalendarSchedule>();

                if (courseSectionAddViewModel.courseCalendarScheduleList.Count > 0)
                {
                    int? calendarscheduleSerial = 1;

                    var CourseSectioncalendarscheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectioncalendarscheduleData != null)
                    {
                        calendarscheduleSerial = CourseSectioncalendarscheduleData.Serial + 1;
                        //calendarscheduleSerial = CourseSectioncalendarscheduleData.ccs.Serial + 1;
                    }

                    foreach (var courseCalendarSchedule in courseSectionAddViewModel.courseCalendarScheduleList)
                    {
                        var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId &&
                        ((c.FixedRoomId != null && (c.FixedRoomId == courseCalendarSchedule.RoomId && c.FixedPeriodId == courseCalendarSchedule.PeriodId && c.FixedDays.Contains(calenderDay))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseCalendarSchedule.RoomId && c.VarPeriodId == courseCalendarSchedule.PeriodId && c.VarDay == calenderDay)) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseCalendarSchedule.RoomId && c.CalPeriodId == courseCalendarSchedule.PeriodId && c.CalDay == calenderDay)))
                        && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        if (courseSectionList.Count > 0)
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        //    var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        //    if (calenderDay != null)
                        //    {
                        //        int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //        var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseCalendarSchedule.RoomId && c.cf.PeriodId == courseCalendarSchedule.PeriodId && c.cs.MeetingDays.Contains(dayValue.ToString()) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {                            
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }
                        //}

                        //    var variableScheduleDataList = this.context?.CourseSection.
                        //                Join(this.context?.CourseVariableSchedule,
                        //                cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //                (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseCalendarSchedule.RoomId && c.cvs.PeriodId == courseCalendarSchedule.PeriodId && c.cvs.Day.ToLower() == calenderDay.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (variableScheduleDataList.Count > 0)
                        //        {                            
                        //            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //            courseSectionAddViewModel._failure = true;
                        //            return courseSectionAddViewModel;
                        //        }                        

                        //    var calenderScheduleDataList = this.context?.CourseSection.
                        //                Join(this.context?.CourseCalendarSchedule,
                        //                cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //                (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseCalendarSchedule.RoomId && c.ccs.PeriodId == courseCalendarSchedule.PeriodId && c.ccs.Date == courseCalendarSchedule.Date && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (calenderScheduleDataList.Count > 0)
                        //        {
                        //            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Calendar Scheduling.";
                        //            courseSectionAddViewModel._failure = true;
                        //            return courseSectionAddViewModel;
                        //        }

                        var courseCalenderSchedule = new CourseCalendarSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)calendarscheduleSerial,
                            BlockId = courseCalendarSchedule.BlockId,
                            Date = courseCalendarSchedule.Date,
                            PeriodId = courseCalendarSchedule.PeriodId,
                            RoomId = courseCalendarSchedule.RoomId,
                            TakeAttendance = courseCalendarSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        courseCalendarScheduleList.Add(courseCalenderSchedule);
                        calendarscheduleSerial++;
                    }
                    this.context?.CourseCalendarSchedule.AddRange(courseCalendarScheduleList);
                    courseSectionAddViewModel.courseSection.ScheduleType = "Calendar Schedule (3)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Add Block Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="CourseSectionId"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel AddBlockSchedule(CourseSectionAddViewModel courseSectionAddViewModel, int? CourseSectionId, decimal? academicYear)
        {
            try
            {
                List<CourseBlockSchedule> courseBlockScheduleList = new List<CourseBlockSchedule>();

                if (courseSectionAddViewModel.courseBlockScheduleList.Count > 0)
                {

                    int? blockscheduleSerial = 1;

                    var CourseSectionblockscheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionblockscheduleData != null)
                    {
                        blockscheduleSerial = CourseSectionblockscheduleData.Serial + 1;
                    }

                    foreach (var courseBlockSchedules in courseSectionAddViewModel.courseBlockScheduleList)
                    {

                        var blockScheduleData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.BlockRoomId == courseBlockSchedules.RoomId && c.BlockPeriodId == courseBlockSchedules.PeriodId && c.BlockId == courseBlockSchedules.BlockId && c.AcademicYear == academicYear && c.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        if (blockScheduleData.Count > 0)
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = "Room is not available for this block and period";
                            return courseSectionAddViewModel;
                        }

                        var courseBlockSchedule = new CourseBlockSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = (int)CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)blockscheduleSerial,
                            BlockId = courseBlockSchedules.BlockId,
                            PeriodId = courseBlockSchedules.PeriodId,
                            RoomId = courseBlockSchedules.RoomId,
                            TakeAttendance = courseBlockSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            CreatedOn = DateTime.UtcNow
                        };
                        courseBlockScheduleList.Add(courseBlockSchedule);
                        blockscheduleSerial++;

                    }
                    this.context?.CourseBlockSchedule.AddRange(courseBlockScheduleList);
                    courseSectionAddViewModel.courseSection.ScheduleType = "Block Schedule (4)";
                    courseSectionAddViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel UpdateCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var studentScheduleData = this.context?.StudentCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId && (e.IsDropped == null || e.IsDropped == false));

                    var staffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId);

                    if (studentScheduleData != null || staffScheduleData != null)
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "Could Not Update This Course Section. It Has Association";
                    }
                    else
                    {
                        decimal? academicYear = null;

                        var courseSectionUpdate = this.context?.CourseSection.FirstOrDefault(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.CourseId == courseSectionAddViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId);

                        if (courseSectionUpdate != null)
                        {
                            var CalenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.CalenderId == courseSectionAddViewModel.courseSection.CalendarId && x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId);

                            if (CalenderData != null)
                            {
                                academicYear = CalenderData.AcademicYear;

                                if (!(bool)courseSectionAddViewModel.courseSection.DurationBasedOnPeriod)
                                {
                                    if (CalenderData.StartDate > courseSectionAddViewModel.courseSection.DurationStartDate || CalenderData.EndDate < courseSectionAddViewModel.courseSection.DurationEndDate)
                                    {
                                        courseSectionAddViewModel._message = "Start Date And End Date of Course Section Should Be Between Start Date And End Date of School Calender";
                                        courseSectionAddViewModel._failure = true;
                                        return courseSectionAddViewModel;
                                    }
                                }

                                var courseSectionNameExists = this.context?.CourseSection.FirstOrDefault(x => x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.CourseSectionId != courseSectionAddViewModel.courseSection.CourseSectionId && x.CourseSectionName.ToLower() == courseSectionAddViewModel.courseSection.CourseSectionName.ToLower() && x.AcademicYear == academicYear);

                                if (courseSectionNameExists != null)
                                {
                                    courseSectionAddViewModel._failure = true;
                                    courseSectionAddViewModel._message = "Course Section Name Already Exists";
                                    return courseSectionAddViewModel;
                                }


                                courseSectionAddViewModel._message = "Course Section Updated Successfully";

                                if (courseSectionAddViewModel.courseSection.ScheduleType != null)
                                {
                                    switch (courseSectionAddViewModel.courseSection.ScheduleType.ToLower())
                                    {
                                        case "fixedschedule":
                                            if (UpdateFixedSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "variableschedule":
                                            if (UpdateVariableSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "calendarschedule":
                                            if (UpdateCalendarSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;

                                        case "blockschedule":
                                            if (UpdateBlockSchedule(courseSectionAddViewModel, academicYear)._failure)
                                            {
                                                return courseSectionAddViewModel;
                                            }
                                            break;
                                        default:
                                            courseSectionAddViewModel._failure = true;
                                            courseSectionAddViewModel._message = "Please Provide a Valid Schedule Type.";
                                            return courseSectionAddViewModel;
                                    }

                                    if (courseSectionAddViewModel.MarkingPeriodId != null)
                                    {
                                        var markingPeriodid = courseSectionAddViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                                        if (markingPeriodid.First() == "2")
                                        {
                                            courseSectionAddViewModel.courseSection.QtrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                        if (markingPeriodid.First() == "1")
                                        {
                                            courseSectionAddViewModel.courseSection.SmstrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                        if (markingPeriodid.First() == "0")
                                        {
                                            courseSectionAddViewModel.courseSection.YrMarkingPeriodId = Int32.Parse(markingPeriodid.ElementAt(1));
                                        }
                                    }
                                    courseSectionAddViewModel.courseSection.CreatedBy = courseSectionUpdate.CreatedBy;
                                    courseSectionAddViewModel.courseSection.CreatedOn = courseSectionUpdate.CreatedOn;
                                    courseSectionAddViewModel.courseSection.UpdatedOn = DateTime.UtcNow;
                                    courseSectionAddViewModel.courseSection.AcademicYear = academicYear;
                                    this.context.Entry(courseSectionUpdate).CurrentValues.SetValues(courseSectionAddViewModel.courseSection);
                                    this.context?.SaveChanges();
                                    courseSectionAddViewModel._failure = false;
                                }
                            }
                            else
                            {
                                courseSectionAddViewModel._failure = true;
                                courseSectionAddViewModel._message = "School Calender does not exist";
                                return courseSectionAddViewModel;
                            }
                        }
                        else
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = NORECORDFOUND;
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    courseSectionAddViewModel._failure = true;
                    courseSectionAddViewModel._message = es.Message;
                }
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Fixed Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateFixedSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.courseFixedSchedule != null)
                {
                    var fixedScheduleDataUpdate = this.context?.CourseFixedSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseFixedSchedule.TenantId && x.SchoolId == courseSectionAddViewModel.courseFixedSchedule.SchoolId && x.CourseId == courseSectionAddViewModel.courseFixedSchedule.CourseId && x.CourseSectionId == courseSectionAddViewModel.courseFixedSchedule.CourseSectionId && x.Serial == courseSectionAddViewModel.courseFixedSchedule.Serial).FirstOrDefault();

                    if (fixedScheduleDataUpdate != null)
                    {
                        var roomCapacity = this.context?.Rooms.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && e.IsActive == true)?.Capacity;

                        if (roomCapacity == null || roomCapacity < courseSectionAddViewModel.courseSection.Seats)
                        {
                            courseSectionAddViewModel._message = "Invalid Seat Capacity";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        // string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");

                        //foreach (var meetingDay in meetingDays)
                        //{
                        var courseSectionList = this.context?.AllCourseSectionView.AsEnumerable().Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.CourseSectionId != courseSectionAddViewModel.courseSection.CourseSectionId &&
                        ((c.FixedPeriodId != null && (c.FixedRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.FixedPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && (Regex.IsMatch(courseSectionAddViewModel.courseSection.MeetingDays.ToLower(), c.FixedDays.ToLower(), RegexOptions.IgnoreCase)))) ||
                         (c.VarPeriodId != null && (c.VarRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.VarPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && courseSectionAddViewModel.courseSection.MeetingDays.ToLower().Contains(c.VarDay.ToLower()))) ||
                         (c.CalPeriodId != null && (c.CalRoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.CalPeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && courseSectionAddViewModel.courseSection.MeetingDays.ToLower().Contains(c.CalDay.ToLower()))))
                        && c.AcademicYear == academicYear && ((courseSectionAddViewModel.courseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.courseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                        if (courseSectionList.Count > 0)
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }
                        //}


                        //string[] meetingDays = courseSectionAddViewModel.courseSection.MeetingDays.Split("|");
                        //string message = null;

                        //foreach (var meetingDay in meetingDays)
                        //{
                        //    var courseSectionList = this.context?.CourseSection.
                        //    Join(this.context?.CourseFixedSchedule,
                        //     cs => cs.CourseSectionId, cfs => cfs.CourseSectionId,
                        //    (cs, cfs) => new { cs, cfs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cfs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cfs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.CourseSectionId != courseSectionAddViewModel.courseSection.CourseSectionId && c.cfs.Serial != courseSectionAddViewModel.courseFixedSchedule.Serial && c.cs.MeetingDays.Contains(meetingDay) && c.cs.AcademicYear==academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling.";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }

                        //    Days days = new Days();
                        //    var Day = Enum.GetName(days.GetType(), Convert.ToInt32(meetingDay));

                        //    var variableScheduleDataList = this.context?.CourseSection.
                        //    Join(this.context?.CourseVariableSchedule,
                        //     cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //    (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.cvs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (variableScheduleDataList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling.";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }

                        //    var calenderScheduleList = this.context?.CourseSection.
                        //    Join(this.context?.CourseCalendarSchedule,
                        //     cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //    (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseSectionAddViewModel.courseFixedSchedule.RoomId && c.ccs.PeriodId == courseSectionAddViewModel.courseFixedSchedule.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (calenderScheduleList.Count > 0)
                        //    {
                        //        foreach (var calenderSchedule in calenderScheduleList)
                        //        {
                        //            var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //            if (calenderDay != null)
                        //            {
                        //                int calenderDayValue = (int)Enum.Parse(typeof(Days), calenderDay);
                        //                if (calenderDayValue.ToString() == meetingDay)
                        //                {
                        //                    if (message != null)
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                    }
                        //                    else
                        //                    {
                        //                        message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                    }
                        //                    courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                    courseSectionAddViewModel._failure = true;
                        //                    return courseSectionAddViewModel;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        courseSectionAddViewModel.courseFixedSchedule.CreatedBy = fixedScheduleDataUpdate.CreatedBy;
                        courseSectionAddViewModel.courseFixedSchedule.CreatedOn = fixedScheduleDataUpdate.CreatedOn;
                        courseSectionAddViewModel.courseFixedSchedule.UpdatedOn = DateTime.UtcNow;
                        this.context.Entry(fixedScheduleDataUpdate).CurrentValues.SetValues(courseSectionAddViewModel.courseFixedSchedule);
                    }
                    courseSectionAddViewModel._failure = false;
                    courseSectionAddViewModel.courseSection.ScheduleType = "Fixed Schedule (1)";
                }
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Variable Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateVariableSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.courseVariableScheduleList.Count > 0)
                {
                    var variableScheduleDataUpdate = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.CourseId == courseSectionAddViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId).ToList();

                    if (variableScheduleDataUpdate != null)
                    {
                        this.context?.CourseVariableSchedule.RemoveRange(variableScheduleDataUpdate);
                        this.context?.SaveChanges();
                    }

                    int? variablescheduleSerial = 1;

                    var CourseSectionvariablescheduleData = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionvariablescheduleData != null)
                    {
                        variablescheduleSerial = CourseSectionvariablescheduleData.Serial + 1;
                    }

                    foreach (var courseVariableSchedules in courseSectionAddViewModel.courseVariableScheduleList)
                    {

                        List<CourseVariableSchedule> courseVariableScheduleList = new List<CourseVariableSchedule>();

                        var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId &&
                        ((c.FixedPeriodId != null && (c.FixedRoomId == courseVariableSchedules.RoomId && c.FixedPeriodId == courseVariableSchedules.PeriodId && c.FixedDays.Contains(courseVariableSchedules.Day))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseVariableSchedules.RoomId && c.VarPeriodId == courseVariableSchedules.PeriodId && c.VarDay == courseVariableSchedules.Day)) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseVariableSchedules.RoomId && c.CalPeriodId == courseVariableSchedules.PeriodId && c.CalDay == courseVariableSchedules.Day)))
                        && c.AcademicYear == academicYear && ((courseSectionAddViewModel.courseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.courseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                        if (courseSectionList.Count > 0)
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        //string message = null;

                        //Days days = new Days();

                        //var Day = Enum.GetName(days.GetType(), Convert.ToInt32(courseVariableSchedules.Day));

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //            Join(this.context?.CourseVariableSchedule,
                        //            cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //            (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseVariableSchedules.RoomId && c.cvs.PeriodId == courseVariableSchedules.PeriodId && c.cvs.Day.ToLower() == Day.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var courseSectionList = this.context?.CourseSection.
                        //            Join(this.context?.CourseFixedSchedule,
                        //            cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //            (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseVariableSchedules.RoomId && c.cf.PeriodId == courseVariableSchedules.PeriodId && c.cs.MeetingDays.Contains(courseVariableSchedules.Day) && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (courseSectionList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var calenderScheduleList = this.context?.CourseSection.
                        //        Join(this.context?.CourseCalendarSchedule,
                        //        cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //        (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseVariableSchedules.RoomId && c.ccs.PeriodId == courseVariableSchedules.PeriodId && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (calenderScheduleList.Count > 0)
                        //{
                        //    foreach (var calenderSchedule in calenderScheduleList)
                        //    {
                        //        var calenderDay = calenderSchedule.ccs.Date.Value.DayOfWeek.ToString();

                        //        if (calenderDay != null)
                        //        {
                        //            int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //            if (dayValue.ToString() == courseVariableSchedules.Day)
                        //            {
                        //                if (message != null)
                        //                {
                        //                    message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")" + "," + message;
                        //                }
                        //                else
                        //                {
                        //                    message = calenderSchedule.ccs.Date.Value.Date.ToString("yyyy-MM-dd") + "(" + calenderDay + ")";
                        //                }
                        //                courseSectionAddViewModel._message = message + "is associate with Course Calender Schedule";
                        //                courseSectionAddViewModel._failure = true;
                        //                return courseSectionAddViewModel;
                        //            }
                        //        }
                        //    }
                        //}

                        var courseVariableScheduleAdd = new CourseVariableSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.courseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)variablescheduleSerial,
                            BlockId = courseVariableSchedules.BlockId,
                            Day = courseVariableSchedules.Day,
                            PeriodId = courseVariableSchedules.PeriodId,
                            RoomId = courseVariableSchedules.RoomId,
                            TakeAttendance = courseVariableSchedules.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.courseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };
                        courseVariableScheduleList.Add(courseVariableScheduleAdd);
                        variablescheduleSerial++;
                        this.context?.CourseVariableSchedule.AddRange(courseVariableScheduleList);
                    }
                }
                //}
                courseSectionAddViewModel._failure = false;
                courseSectionAddViewModel.courseSection.ScheduleType = "Variable Schedule (2)";

            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Calendar Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateCalendarSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.courseCalendarScheduleList.Count > 0)
                {
                    var calenderScheduleDataUpdate = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.CourseId == courseSectionAddViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId).ToList();

                    if (calenderScheduleDataUpdate != null)
                    {
                        this.context?.CourseCalendarSchedule.RemoveRange(calenderScheduleDataUpdate);
                        this.context.SaveChanges();
                    }

                    int? calendarscheduleSerial = 1;

                    var CourseSectioncalendarscheduleData = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectioncalendarscheduleData != null)
                    {
                        calendarscheduleSerial = CourseSectioncalendarscheduleData.Serial + 1;
                    }

                    foreach (var courseCalendarSchedule in courseSectionAddViewModel.courseCalendarScheduleList)
                    {
                        List<CourseCalendarSchedule> courseCalendarScheduleList = new List<CourseCalendarSchedule>();

                        var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        var courseSectionList = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId &&
                        ((c.FixedRoomId != null && (c.FixedRoomId == courseCalendarSchedule.RoomId && c.FixedPeriodId == courseCalendarSchedule.PeriodId && c.FixedDays.Contains(calenderDay))) ||
                        (c.VarPeriodId != null && (c.VarRoomId == courseCalendarSchedule.RoomId && c.VarPeriodId == courseCalendarSchedule.PeriodId && c.VarDay == calenderDay)) ||
                        (c.CalPeriodId != null && (c.CalRoomId == courseCalendarSchedule.RoomId && c.CalPeriodId == courseCalendarSchedule.PeriodId && c.CalDay == calenderDay)))
                        && c.AcademicYear == academicYear && ((courseSectionAddViewModel.courseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.courseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                        if (courseSectionList.Count > 0)
                        {
                            courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Scheduling.";
                            courseSectionAddViewModel._failure = true;
                            return courseSectionAddViewModel;
                        }

                        //var calenderDay = courseCalendarSchedule.Date.Value.DayOfWeek.ToString();

                        //if (calenderDay != null)
                        //{
                        //    int dayValue = (int)Enum.Parse(typeof(Days), calenderDay);

                        //    var courseSectionList = this.context?.CourseSection.
                        //        Join(this.context?.CourseFixedSchedule,
                        //        cs => cs.CourseSectionId, cf => cf.CourseSectionId,
                        //        (cs, cf) => new { cs, cf }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cf.RoomId == courseCalendarSchedule.RoomId && c.cf.PeriodId == courseCalendarSchedule.PeriodId && c.cs.MeetingDays.Contains(dayValue.ToString()) && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //    if (courseSectionList.Count > 0)
                        //    {
                        //        courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Fixed Scheduling";
                        //        courseSectionAddViewModel._failure = true;
                        //        return courseSectionAddViewModel;
                        //    }
                        //}

                        //var variableScheduleDataList = this.context?.CourseSection.
                        //        Join(this.context?.CourseVariableSchedule,
                        //        cs => cs.CourseSectionId, cvs => cvs.CourseSectionId,
                        //        (cs, cvs) => new { cs, cvs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.cvs.RoomId == courseCalendarSchedule.RoomId && c.cvs.PeriodId == courseCalendarSchedule.PeriodId && c.cvs.Day.ToLower() == calenderDay.ToLower() && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (variableScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Variable Scheduling";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}

                        //var calenderScheduleDataList = this.context?.CourseSection.
                        //        Join(this.context?.CourseCalendarSchedule,
                        //        cs => cs.CourseSectionId, ccs => ccs.CourseSectionId,
                        //        (cs, ccs) => new { cs, ccs }).Where(c => c.cs.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.cs.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.ccs.RoomId == courseCalendarSchedule.RoomId && c.ccs.PeriodId == courseCalendarSchedule.PeriodId && c.ccs.Date == courseCalendarSchedule.Date && c.cs.AcademicYear == academicYear && c.cs.DurationEndDate > courseSectionAddViewModel.courseSection.DurationStartDate).ToList();

                        //if (calenderScheduleDataList.Count > 0)
                        //{
                        //    courseSectionAddViewModel._message = "Room is not available for this period due to already booked for Calendar Scheduling.";
                        //    courseSectionAddViewModel._failure = true;
                        //    return courseSectionAddViewModel;
                        //}
                        var courseCalenderSchedule = new CourseCalendarSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.courseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)calendarscheduleSerial,
                            BlockId = courseCalendarSchedule.BlockId,
                            Date = courseCalendarSchedule.Date,
                            PeriodId = courseCalendarSchedule.PeriodId,
                            RoomId = courseCalendarSchedule.RoomId,
                            TakeAttendance = courseCalendarSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.courseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };
                        courseCalendarScheduleList.Add(courseCalenderSchedule);
                        calendarscheduleSerial++;
                        this.context?.CourseCalendarSchedule.AddRange(courseCalendarScheduleList);
                    }
                }
                courseSectionAddViewModel.courseSection.ScheduleType = "Calendar Schedule (3)";
                courseSectionAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Update Block Course Section Schedule
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private CourseSectionAddViewModel UpdateBlockSchedule(CourseSectionAddViewModel courseSectionAddViewModel, decimal? academicYear)
        {
            try
            {
                if (courseSectionAddViewModel.courseBlockScheduleList.Count > 0)
                {
                    var blockScheduleDataUpdate = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && x.CourseId == courseSectionAddViewModel.courseSection.CourseId && x.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId).ToList();

                    if (blockScheduleDataUpdate.Count > 0)
                    {
                        this.context?.CourseBlockSchedule.RemoveRange(blockScheduleDataUpdate);
                        this.context.SaveChanges();
                    }

                    int? blockscheduleSerial = 1;

                    var CourseSectionblockscheduleData = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionAddViewModel.courseSection.TenantId && x.SchoolId == courseSectionAddViewModel.courseSection.SchoolId).OrderByDescending(x => x.Serial).FirstOrDefault();

                    if (CourseSectionblockscheduleData != null)
                    {
                        blockscheduleSerial = CourseSectionblockscheduleData.Serial + 1;
                    }

                    foreach (var courseBlockSchedule in courseSectionAddViewModel.courseBlockScheduleList)
                    {
                        List<CourseBlockSchedule> courseBlockScheduleList = new List<CourseBlockSchedule>();

                        var blockScheduleData = this.context?.AllCourseSectionView.Where(c => c.TenantId == courseSectionAddViewModel.courseSection.TenantId && c.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && c.BlockRoomId == courseBlockSchedule.RoomId && c.BlockPeriodId == courseBlockSchedule.PeriodId && c.BlockId == courseBlockSchedule.BlockId && c.AcademicYear == academicYear && ((courseSectionAddViewModel.courseSection.DurationStartDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationStartDate <= c.DurationEndDate) || (courseSectionAddViewModel.courseSection.DurationEndDate >= c.DurationStartDate && courseSectionAddViewModel.courseSection.DurationEndDate <= c.DurationEndDate))).ToList();

                        if (blockScheduleData.Count > 0)
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = "Room is not available for this block and period";
                            return courseSectionAddViewModel;
                        }

                        var courseBlockScheduleAdd = new CourseBlockSchedule()
                        {
                            TenantId = courseSectionAddViewModel.courseSection.TenantId,
                            SchoolId = courseSectionAddViewModel.courseSection.SchoolId,
                            CourseId = courseSectionAddViewModel.courseSection.CourseId,
                            CourseSectionId = courseSectionAddViewModel.courseSection.CourseSectionId,
                            GradeScaleId = courseSectionAddViewModel.courseSection.GradeScaleId,
                            Serial = (int)blockscheduleSerial,
                            BlockId = courseBlockSchedule.BlockId,
                            PeriodId = courseBlockSchedule.PeriodId,
                            RoomId = courseBlockSchedule.RoomId,
                            TakeAttendance = courseBlockSchedule.TakeAttendance,
                            CreatedBy = courseSectionAddViewModel.courseSection.CreatedBy,
                            UpdatedBy = courseSectionAddViewModel.courseSection.UpdatedBy,
                            UpdatedOn = DateTime.UtcNow
                        };

                        courseBlockScheduleList.Add(courseBlockScheduleAdd);
                        blockscheduleSerial++;
                        this.context?.CourseBlockSchedule.AddRange(courseBlockScheduleList);
                    }
                }
                courseSectionAddViewModel.courseSection.ScheduleType = "Block Schedule (4)";
                courseSectionAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                courseSectionAddViewModel._failure = true;
                courseSectionAddViewModel._message = es.Message;
            }
            return courseSectionAddViewModel;
        }

        /// <summary>
        /// Delete Course Section
        /// </summary>
        /// <param name="courseSectionAddViewModel"></param>
        /// <returns></returns>
        public CourseSectionAddViewModel DeleteCourseSection(CourseSectionAddViewModel courseSectionAddViewModel)
        {

            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var studentScheduleData = this.context?.StudentCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId && (e.IsDropped == null || e.IsDropped == false));

                    var staffScheduleData = this.context?.StaffCoursesectionSchedule.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId);

                    if (studentScheduleData != null || staffScheduleData != null)
                    {
                        courseSectionAddViewModel._failure = true;
                        courseSectionAddViewModel._message = "Could Not Delete This Course Section. It Has Association";
                    }
                    else
                    {
                        var courseSectionDelete = this.context?.CourseSection.FirstOrDefault(e => e.TenantId == courseSectionAddViewModel.courseSection.TenantId && e.SchoolId == courseSectionAddViewModel.courseSection.SchoolId && e.CourseSectionId == courseSectionAddViewModel.courseSection.CourseSectionId);

                        if (courseSectionDelete != null)
                        {
                            if (courseSectionDelete.ScheduleType == "Fixed Schedule (1)")
                            {
                                var courseFixedScheduleDelete = this.context?.CourseFixedSchedule.FirstOrDefault(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId);

                                if (courseFixedScheduleDelete != null)
                                {
                                    this.context?.CourseFixedSchedule.Remove(courseFixedScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Variable Schedule (2)")
                            {
                                var courseVariableScheduleDelete = this.context?.CourseVariableSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseVariableScheduleDelete != null)
                                {
                                    this.context?.CourseVariableSchedule.RemoveRange(courseVariableScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Calendar Schedule (3)")
                            {
                                var courseCalendarScheduleDelete = this.context?.CourseCalendarSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseCalendarScheduleDelete != null)
                                {
                                    this.context?.CourseCalendarSchedule.RemoveRange(courseCalendarScheduleDelete);
                                }
                            }

                            if (courseSectionDelete.ScheduleType == "Block Schedule (4)")
                            {
                                var courseBlockScheduleDelete = this.context?.CourseBlockSchedule.Where(x => x.TenantId == courseSectionDelete.TenantId && x.SchoolId == courseSectionDelete.SchoolId && x.CourseSectionId == courseSectionDelete.CourseSectionId).ToList();

                                if (courseBlockScheduleDelete != null)
                                {
                                    this.context?.CourseBlockSchedule.RemoveRange(courseBlockScheduleDelete);
                                }
                            }
                            this.context?.CourseSection.Remove(courseSectionDelete);
                            this.context?.SaveChanges();

                            courseSectionAddViewModel._failure = false;
                            courseSectionAddViewModel._message = "Course Section Deleted Successfully";
                            transaction.Commit();
                        }
                        else
                        {
                            courseSectionAddViewModel._failure = true;
                            courseSectionAddViewModel._message = NORECORDFOUND;
                        }
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    courseSectionAddViewModel._message = es.Message;
                    courseSectionAddViewModel._failure = true;
                }
                return courseSectionAddViewModel;
            }
        }

        /// <summary>
        /// Get All Course Standard For Course
        /// </summary>
        /// <param name="courseStandardForCourseViewModel"></param>
        /// <returns></returns>
        public CourseStandardForCourseViewModel GetAllCourseStandardForCourse(CourseStandardForCourseViewModel courseStandardForCourseViewModel)
        {
            CourseStandardForCourseViewModel courseStandardForCourse = new CourseStandardForCourseViewModel();
            try
            {
                courseStandardForCourse.TenantId = courseStandardForCourseViewModel.TenantId;
                courseStandardForCourse.SchoolId = courseStandardForCourseViewModel.SchoolId;
                courseStandardForCourse.CourseId = courseStandardForCourseViewModel.CourseId;
                courseStandardForCourse._tenantName = courseStandardForCourseViewModel._tenantName;
                courseStandardForCourse._token = courseStandardForCourseViewModel._token;
                courseStandardForCourse._userName = courseStandardForCourseViewModel._userName;

                var courseStandarddata = this.context?.CourseStandard.Include(x => x.GradeUsStandard).Where(x => x.TenantId == courseStandardForCourseViewModel.TenantId && x.SchoolId == courseStandardForCourseViewModel.SchoolId && x.CourseId == courseStandardForCourseViewModel.CourseId).Select(e=> new CourseStandard()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    CourseId=e.CourseId,
                    StandardRefNo=e.StandardRefNo,
                    CreatedOn=e.CreatedOn,
                    UpdatedOn=e.UpdatedOn,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseStandardForCourseViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseStandardForCourseViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    GradeUsStandard= new GradeUsStandard()
                    {
                        TenantId=e.GradeUsStandard.TenantId,
                        SchoolId=e.GradeUsStandard.SchoolId,
                        StandardRefNo=e.GradeUsStandard.StandardRefNo,
                        GradeLevel=e.GradeUsStandard.GradeLevel,
                        Domain= e.GradeUsStandard.Domain,
                        Subject= e.GradeUsStandard.Subject,
                        Course= e.GradeUsStandard.Course,
                        Topic= e.GradeUsStandard.Topic,
                        StandardDetails= e.GradeUsStandard.StandardDetails,
                        GradeStandardId= e.GradeUsStandard.GradeStandardId,
                        IsSchoolSpecific= e.GradeUsStandard.IsSchoolSpecific,
                        CreatedBy= (e.GradeUsStandard.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseStandardForCourseViewModel.TenantId && u.EmailAddress == e.GradeUsStandard.CreatedBy).Name : null,
                        CreatedOn=e.GradeUsStandard.CreatedOn,
                        UpdatedBy= (e.GradeUsStandard.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == courseStandardForCourseViewModel.TenantId && u.EmailAddress == e.GradeUsStandard.UpdatedBy).Name : null,
                        UpdatedOn= e.GradeUsStandard.UpdatedOn
                    }
                }).ToList();
                
                if (courseStandarddata.Count > 0)
                {
                    courseStandardForCourse.courseStandards = courseStandarddata;
                }
                else
                {
                    courseStandardForCourse._failure = true;
                    courseStandardForCourse._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                courseStandardForCourse.courseStandards = null;
                courseStandardForCourse._failure = true;
                courseStandardForCourse._message = es.Message;
            }
            return courseStandardForCourse;
        }

        ///// <summary>
        ///// Delete Course Section  For Specific Schedule Type
        ///// </summary>
        ///// <param name="deleteScheduleViewModel"></param>
        ///// <returns></returns>
        //public DeleteScheduleViewModel DeleteSchedule(DeleteScheduleViewModel deleteScheduleViewModel)
        //{
        //    try
        //    {
        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "variableschedule")
        //        {
        //            var variableScheduleData = this.context?.CourseVariableSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (variableScheduleData != null)
        //            {
        //                this.context?.CourseVariableSchedule.Remove(variableScheduleData);
        //            }
        //        }

        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "blockschedule")
        //        {
        //            var blockScheduleData = this.context?.CourseBlockSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (blockScheduleData != null)
        //            {
        //                this.context?.CourseBlockSchedule.Remove(blockScheduleData);
        //            }
        //        }

        //        if (deleteScheduleViewModel.ScheduleType.ToLower() == "calendarschedule")
        //        {
        //            var calendarScheduleData = this.context?.CourseCalendarSchedule.FirstOrDefault(x => x.TenantId == deleteScheduleViewModel.TenantId && x.SchoolId == deleteScheduleViewModel.SchoolId && x.CourseId == deleteScheduleViewModel.CourseId && x.CourseSectionId == deleteScheduleViewModel.CourseSectionId && x.Serial == deleteScheduleViewModel.Serial);

        //            if (calendarScheduleData != null)
        //            {
        //                this.context?.CourseCalendarSchedule.Remove(calendarScheduleData);
        //            }
        //        }

        //        this.context?.SaveChanges();
        //        deleteScheduleViewModel._message = "Schedule Deleted Successfully";
        //        deleteScheduleViewModel._failure = false;

        //    }
        //    catch (Exception es)
        //    {
        //        deleteScheduleViewModel._message = es.Message;
        //        deleteScheduleViewModel._failure = true;
        //    }
        //    return deleteScheduleViewModel;
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
                searchCourseSection.TenantId = searchCourseSectionViewModel.TenantId;
                searchCourseSection.SchoolId = searchCourseSectionViewModel.SchoolId;
                searchCourseSection._token = searchCourseSectionViewModel._token;
                searchCourseSection._tenantName = searchCourseSectionViewModel._tenantName;

                int? YrmarkingPeriodId = 0;
                int? SmtrmarkingPeriodId = 0;
                int? QtrmarkingPeriodId = 0;
                if (searchCourseSectionViewModel.MarkingPeriodId != null)
                {

                    var markingPeriod = searchCourseSectionViewModel.MarkingPeriodId.Split("_", StringSplitOptions.RemoveEmptyEntries);

                    if (markingPeriod.First() == "0")
                    {
                        YrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                    if (markingPeriod.First() == "1")
                    {
                        SmtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                    if (markingPeriod.First() == "2")
                    {
                        QtrmarkingPeriodId = Int32.Parse(markingPeriod.Last());
                    }
                }
                //var todayDate = DateTime.UtcNow;

                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && (searchCourseSectionViewModel.MarkingPeriodStartDate == null || (x.DurationStartDate >= searchCourseSectionViewModel.MarkingPeriodStartDate || x.DurationEndDate >= searchCourseSectionViewModel.MarkingPeriodStartDate)) && (searchCourseSectionViewModel.CourseId == null || x.CourseId == searchCourseSectionViewModel.CourseId) && (searchCourseSectionViewModel.CourseSubject == null || x.CourseSubject == searchCourseSectionViewModel.CourseSubject) && (searchCourseSectionViewModel.CourseProgram == null || x.CourseProgram == searchCourseSectionViewModel.CourseProgram) && (searchCourseSectionViewModel.MarkingPeriodId == null || x.YrMarkingPeriodId == YrmarkingPeriodId || x.SmstrMarkingPeriodId == SmtrmarkingPeriodId || x.QtrMarkingPeriodId == QtrmarkingPeriodId));

                if (coursedata.Count() > 0)
                {

                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (searchCourseSectionViewModel.ForStaff == true)
                    {
                        foreach (var CourseData in distinctCourseData)
                        {
                            var staffSchedule = this.context.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != false).ToList();
                            if (staffSchedule.Count > 0)
                            {
                                foreach (var staff in staffSchedule)
                                {
                                    var staffName = staff.StaffMaster.FirstGivenName + " " + staff.StaffMaster.MiddleName + " " + staff.StaffMaster.LastFamilyName;
                                    CourseData.StaffName = CourseData.StaffName != null ? CourseData.StaffName + "|" + staffName : staffName;
                                }
                            }
                        }
                    }

                    if (searchCourseSectionViewModel.ForStudent == true)
                    {
                        foreach (var CourseData in distinctCourseData)
                        {
                            int? studentSchedule = null;
                            studentSchedule = this.context.StudentCoursesectionSchedule.Where(x => x.TenantId == searchCourseSectionViewModel.TenantId && x.SchoolId == searchCourseSectionViewModel.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != false).ToList().Count();

                            CourseData.AvailableSeat = CourseData.Seats - studentSchedule;
                        }
                    }

                    searchCourseSection.allCourseSectionViewList = distinctCourseData;
                    searchCourseSectionViewModel._failure = false;
                }
                else
                {
                    searchCourseSection._message = NORECORDFOUND;
                    searchCourseSectionViewModel._failure = true;
                }

            }
            catch (Exception ex)
            {
                searchCourseSection.allCourseSectionViewList = null;
                searchCourseSection._failure = true;
                searchCourseSection._message = ex.Message;
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
                staffListView.TenantId = staffListViewModel.TenantId;
                staffListView.SchoolId = staffListViewModel.SchoolId;
                staffListView.CourseId = staffListViewModel.CourseId;
                staffListView.CourseSectionId = staffListViewModel.CourseSectionId;
                staffListView._token = staffListViewModel._token;
                staffListView._tenantName = staffListViewModel._tenantName;

                var staffSchedule = this.context.CourseSection.Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).Where(x => x.TenantId == staffListViewModel.TenantId && x.SchoolId == staffListViewModel.SchoolId && x.CourseId == staffListViewModel.CourseId && (staffListViewModel.CourseSectionId == null || x.CourseSectionId == staffListViewModel.CourseSectionId)).AsNoTracking().Select(e => new CourseSection
                {
                    TenantId = e.TenantId,
                    SchoolId = e.SchoolId,
                    CourseId = e.CourseId,
                    CourseSectionId = e.CourseSectionId,
                    CourseSectionName = e.CourseSectionName,
                    ScheduleType = e.ScheduleType,
                    YrMarkingPeriodId = e.YrMarkingPeriodId,
                    SmstrMarkingPeriodId = e.SmstrMarkingPeriodId,
                    QtrMarkingPeriodId = e.QtrMarkingPeriodId,
                    DurationStartDate = e.DurationStartDate,
                    DurationEndDate = e.DurationEndDate,
                    Quarters = e.Quarters != null ? new Quarters { Title = e.Quarters.Title, StartDate = e.Quarters.StartDate, EndDate = e.Quarters.EndDate, ShortName = e.Quarters.ShortName } : null,
                    Semesters = e.Semesters != null ? new Semesters { Title = e.Semesters.Title, StartDate = e.Semesters.StartDate, EndDate = e.Semesters.EndDate, ShortName = e.Semesters.ShortName } : null,
                    SchoolYears = e.SchoolYears != null ? new SchoolYears { Title = e.SchoolYears.Title, StartDate = e.SchoolYears.StartDate, EndDate = e.SchoolYears.EndDate, ShortName = e.SchoolYears.ShortName } : null,
                    StaffCoursesectionSchedule = e.StaffCoursesectionSchedule.Where(d => d.IsDropped != true).Select(s => new StaffCoursesectionSchedule
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        StaffId = s.StaffId,
                        StaffGuid = s.StaffGuid,
                        CourseId = s.CourseId,
                        CourseSectionId = s.CourseSectionId,
                        CourseSectionName = s.CourseSectionName,
                        IsDropped = s.IsDropped,
                        MeetingDays = s.MeetingDays,
                        StaffMaster = new StaffMaster
                        {
                            TenantId = s.StaffMaster.TenantId,
                            SchoolId = s.StaffMaster.SchoolId,
                            StaffId = s.StaffMaster.StaffId,
                            FirstGivenName = s.StaffMaster.FirstGivenName,
                            MiddleName = s.StaffMaster.MiddleName,
                            LastFamilyName = s.StaffMaster.LastFamilyName,
                            StaffInternalId = s.StaffMaster.StaffInternalId,
                            StaffPhoto = staffListViewModel.CourseSectionId != null ? s.StaffMaster.StaffPhoto : null,
                            FirstLanguage = s.StaffMaster.FirstLanguage,
                            SecondLanguage = s.StaffMaster.SecondLanguage,
                            ThirdLanguage = s.StaffMaster.ThirdLanguage,
                            FirstLanguageNavigation = s.StaffMaster.FirstLanguageNavigation,
                            SecondLanguageNavigation = s.StaffMaster.SecondLanguageNavigation,
                            ThirdLanguageNavigation = s.StaffMaster.ThirdLanguageNavigation,
                            JobTitle = s.StaffMaster.JobTitle,
                            PrimaryGradeLevelTaught = s.StaffMaster.PrimaryGradeLevelTaught,
                            OtherGradeLevelTaught = s.StaffMaster.OtherGradeLevelTaught,
                            PrimarySubjectTaught = s.StaffMaster.PrimarySubjectTaught,
                            OtherSubjectTaught = s.StaffMaster.OtherSubjectTaught
                        }
                    }).ToList()
                }).ToList();

                if (staffSchedule.Count > 0)
                {
                    staffListView.courseSectionsList = staffSchedule;
                    staffListView._failure = false;
                }
                else
                {
                    staffListView._failure = true;
                    staffListView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffListView.courseSectionsList = null;
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
                searchCourseSchedule.TenantId = searchCourseScheduleViewModel.TenantId;
                searchCourseSchedule.SchoolId = searchCourseScheduleViewModel.SchoolId;
                searchCourseSchedule._token = searchCourseScheduleViewModel._token;
                searchCourseSchedule._tenantName = searchCourseScheduleViewModel._tenantName;



                var courseData = this.context?.Course.Where(x => x.TenantId == searchCourseScheduleViewModel.TenantId && x.SchoolId == searchCourseScheduleViewModel.SchoolId && (searchCourseScheduleViewModel.CourseSubject == null || x.CourseSubject == searchCourseScheduleViewModel.CourseSubject) && (x.CourseProgram == searchCourseScheduleViewModel.CourseProgram || searchCourseScheduleViewModel.CourseProgram == null));



                if (courseData != null)
                {

                    var resultData = courseData.Select(s => new Course
                    {
                        TenantId = s.TenantId,
                        SchoolId = s.SchoolId,
                        CourseId = s.CourseId,
                        CourseProgram = s.CourseProgram,
                        CourseSubject = s.CourseSubject,
                        CourseGradeLevel = s.CourseGradeLevel,
                        CourseTitle = s.CourseTitle,
                        CourseCategory = s.CourseCategory,
                        CreditHours = s.CreditHours
                    }).ToList();

                    searchCourseSchedule.course = resultData;
                    searchCourseScheduleViewModel._failure = false;
                }
                else
                {
                    searchCourseSchedule._message = NORECORDFOUND;
                    searchCourseScheduleViewModel._failure = true;
                }
            }
            catch (Exception ex)
            {
                searchCourseSchedule.course = null;
                searchCourseSchedule._failure = true;
                searchCourseSchedule._message = ex.Message;
            }
            return searchCourseSchedule;
        }

        /// <summary>
        /// Add/Edit BellSchedule
        /// </summary>
        /// <param name="programAddViewModel"></param>
        /// <returns></returns>
        public BellScheduleAddViewModel AddEditBellSchedule(BellScheduleAddViewModel bellScheduleAddViewModel)
        {
            try
            {
                var bellScheduleData = this.context?.BellSchedule.Where(x => x.TenantId == bellScheduleAddViewModel.bellSchedule.TenantId && x.SchoolId == bellScheduleAddViewModel.bellSchedule.SchoolId && x.BellScheduleDate == bellScheduleAddViewModel.bellSchedule.BellScheduleDate).FirstOrDefault();

                if (bellScheduleData != null)
                {
                    bellScheduleAddViewModel.bellSchedule.CreatedBy = bellScheduleData.CreatedBy;
                    bellScheduleAddViewModel.bellSchedule.CreatedOn = bellScheduleData.CreatedOn;
                    bellScheduleAddViewModel.bellSchedule.UpdatedOn = DateTime.UtcNow;
                    this.context.Entry(bellScheduleData).CurrentValues.SetValues(bellScheduleAddViewModel.bellSchedule);

                    bellScheduleAddViewModel._message = "Bell Schedule Updated Successfully";
                }
                else
                {
                    bellScheduleAddViewModel.bellSchedule.CreatedOn = DateTime.UtcNow;
                    this.context?.BellSchedule.Add(bellScheduleAddViewModel.bellSchedule);

                    bellScheduleAddViewModel._message = "Bell Schedule Added Successfully";
                }
                this.context?.SaveChanges();
                bellScheduleAddViewModel._failure = false;
            }
            catch (Exception es)
            {
                bellScheduleAddViewModel._message = es.Message;
                bellScheduleAddViewModel._failure = true;
            }
            return bellScheduleAddViewModel;
        }

        /// <summary>
        /// Get All Bell Schedule
        /// </summary>
        /// <param name="bellScheduleListViewModel"></param>
        /// <returns></returns>
        public BellScheduleListViewModel GetAllBellSchedule(BellScheduleListViewModel bellScheduleListViewModel)
        {
            BellScheduleListViewModel bellScheduleView = new BellScheduleListViewModel();
            try
            {
                bellScheduleView._tenantName = bellScheduleListViewModel._tenantName;
                bellScheduleView._token = bellScheduleListViewModel._token;
                bellScheduleView.TenantId = bellScheduleListViewModel.TenantId;
                bellScheduleView.SchoolId = bellScheduleListViewModel.SchoolId;
                bellScheduleView.AcademicYear = bellScheduleListViewModel.AcademicYear;

                var bellScheduleData = this.context?.BellSchedule.Where(x => x.TenantId == bellScheduleListViewModel.TenantId && x.SchoolId == bellScheduleListViewModel.SchoolId && x.AcademicYear == bellScheduleListViewModel.AcademicYear).Select(e=> new BellSchedule()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    AcademicYear=e.AcademicYear,
                    BellScheduleDate=e.BellScheduleDate,
                    BlockId=e.BlockId,
                    CreatedOn=e.CreatedOn,
                    UpdatedOn=e.UpdatedOn,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == bellScheduleListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == bellScheduleListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null
                }).ToList();

                if (bellScheduleData.Count > 0)
                {
                    bellScheduleView.bellScheduleList = bellScheduleData;
                    bellScheduleView._failure = false;
                }
                else
                {
                    bellScheduleView._failure = true;
                    bellScheduleView._message = NORECORDFOUND;
                }              
            }
            catch (Exception es)
            {
                bellScheduleView._message = es.Message;
                bellScheduleView._failure = true;             
            }
            return bellScheduleView;
        }
    }
}
