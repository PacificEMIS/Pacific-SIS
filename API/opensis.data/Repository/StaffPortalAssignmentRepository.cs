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
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.StaffPortalAssignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StaffPortalAssignmentRepository : IStaffPortalAssignmentRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffPortalAssignmentRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel AddAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            try
            {
                var checkAssignmentTypeTitle = this.context?.AssignmentType.Where(x => x.SchoolId == assignmentTypeAddViewModel.assignmentType.SchoolId && x.TenantId == assignmentTypeAddViewModel.assignmentType.TenantId && x.CourseSectionId == assignmentTypeAddViewModel.assignmentType.CourseSectionId && x.Title.ToLower() == assignmentTypeAddViewModel.assignmentType.Title.ToLower()).FirstOrDefault();

                if (checkAssignmentTypeTitle != null)
                {
                    assignmentTypeAddViewModel._failure = true;
                    assignmentTypeAddViewModel._message = "Title Already Exists";
                }
                else
                {
                    int? assignmentTypeId = 1;

                    var assignmentTypeData = this.context?.AssignmentType.Where(x => x.SchoolId == assignmentTypeAddViewModel.assignmentType.SchoolId && x.TenantId == assignmentTypeAddViewModel.assignmentType.TenantId).OrderByDescending(x => x.AssignmentTypeId).FirstOrDefault();

                    if (assignmentTypeData != null)
                    {
                        assignmentTypeId = assignmentTypeData.AssignmentTypeId + 1;
                    }
                    assignmentTypeAddViewModel.assignmentType.AssignmentTypeId = (int)assignmentTypeId;
                    assignmentTypeAddViewModel.assignmentType.CreatedOn = DateTime.UtcNow;
                    this.context?.AssignmentType.Add(assignmentTypeAddViewModel.assignmentType);
                    this.context?.SaveChanges();
                    assignmentTypeAddViewModel._failure = false;

                    assignmentTypeAddViewModel._message = "Assignment Type Added Successfully";
                }

            }
            catch (Exception es)
            {
                assignmentTypeAddViewModel._failure = true;
                assignmentTypeAddViewModel._message = es.Message;
            }
            return assignmentTypeAddViewModel;
        }

        /// <summary>
        /// Update Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel UpdateAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            try
            {
                var checkAssignmentTypeTitle = this.context?.AssignmentType.Where(x => x.SchoolId == assignmentTypeAddViewModel.assignmentType.SchoolId && x.TenantId == assignmentTypeAddViewModel.assignmentType.TenantId && x.CourseSectionId == assignmentTypeAddViewModel.assignmentType.CourseSectionId && x.Title.ToLower() == assignmentTypeAddViewModel.assignmentType.Title.ToLower() && x.AssignmentTypeId != assignmentTypeAddViewModel.assignmentType.AssignmentTypeId).FirstOrDefault();

                if (checkAssignmentTypeTitle != null)
                {
                    assignmentTypeAddViewModel._failure = true;
                    assignmentTypeAddViewModel._message = "Title Already Exists";
                }
                else
                {
                    var AssignmentTypeData = this.context?.AssignmentType.FirstOrDefault(c => c.SchoolId == assignmentTypeAddViewModel.assignmentType.SchoolId && c.TenantId == assignmentTypeAddViewModel.assignmentType.TenantId && c.AssignmentTypeId == assignmentTypeAddViewModel.assignmentType.AssignmentTypeId);

                    if (AssignmentTypeData != null)
                    {
                        AssignmentTypeData.Title = assignmentTypeAddViewModel.assignmentType.Title;
                        AssignmentTypeData.Weightage = assignmentTypeAddViewModel.assignmentType.Weightage;
                        AssignmentTypeData.UpdatedBy = assignmentTypeAddViewModel.assignmentType.UpdatedBy;
                        AssignmentTypeData.UpdatedOn = DateTime.UtcNow;
                        this.context?.SaveChanges();
                        assignmentTypeAddViewModel._failure = false;
                        assignmentTypeAddViewModel._message = "Assignment Type Updated Successfully";
                    }
                    else
                    {
                        assignmentTypeAddViewModel._failure = true;
                        assignmentTypeAddViewModel._message = NORECORDFOUND;
                    }
                }
            }
            catch (Exception es)
            {
                assignmentTypeAddViewModel._failure = true;
                assignmentTypeAddViewModel._message = es.Message;
            }
            return assignmentTypeAddViewModel;
        }

        /// <summary>
        /// Delete Assignment Type
        /// </summary>
        /// <param name="assignmentTypeAddViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeAddViewModel DeleteAssignmentType(AssignmentTypeAddViewModel assignmentTypeAddViewModel)
        {
            try
            {
                var AssignmentTypeData = this.context?.AssignmentType.Include(b => b.Assignment).FirstOrDefault(c => c.SchoolId == assignmentTypeAddViewModel.assignmentType.SchoolId && c.TenantId == assignmentTypeAddViewModel.assignmentType.TenantId /*&& c.CourseSectionId == assignmentTypeAddViewModel.assignmentType.CourseSectionId*/ && c.AssignmentTypeId == assignmentTypeAddViewModel.assignmentType.AssignmentTypeId);

                if (AssignmentTypeData != null)
                {
                    if (AssignmentTypeData.Assignment == null || AssignmentTypeData.Assignment.Count == 0)
                    {
                        this.context?.AssignmentType.Remove(AssignmentTypeData);
                        this.context?.SaveChanges();
                        assignmentTypeAddViewModel._failure = false;
                        assignmentTypeAddViewModel._message = "Assignment Type Deleted Succesfully";
                    }
                    else
                    {
                        assignmentTypeAddViewModel._failure = true;
                        assignmentTypeAddViewModel._message = "Can Not Be Deleted.Because It Has Assignments";
                    }
                }
                else
                {
                    assignmentTypeAddViewModel._failure = true;
                    assignmentTypeAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                assignmentTypeAddViewModel._failure = true;
                assignmentTypeAddViewModel._message = es.Message;
            }
            return assignmentTypeAddViewModel;
        }

        /// <summary>
        /// Add Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel AddAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            try
            {
                var checkAssignmentTitle = this.context?.Assignment.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId && x.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId && x.AssignmentTitle.ToLower() == assignmentAddViewModel.assignment.AssignmentTitle.ToLower() && x.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId).FirstOrDefault();

                if (checkAssignmentTitle != null)
                {
                    assignmentAddViewModel._failure = true;
                    assignmentAddViewModel._message = "Assignment Title Already Exists";
                }
                else
                {
                    var courseSectionData = this.context?.CourseSection.FirstOrDefault(e => e.SchoolId == assignmentAddViewModel.assignment.SchoolId && e.SchoolId == assignmentAddViewModel.assignment.SchoolId && e.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId);

                    if (courseSectionData != null)
                    {
                        if ((assignmentAddViewModel.assignment.AssignmentDate >= courseSectionData.DurationStartDate && assignmentAddViewModel.assignment.AssignmentDate <= courseSectionData.DurationEndDate) && (assignmentAddViewModel.assignment.DueDate >= courseSectionData.DurationStartDate && assignmentAddViewModel.assignment.DueDate <= courseSectionData.DurationEndDate))
                        {
                            if (assignmentAddViewModel.assignment.DueDate >= assignmentAddViewModel.assignment.AssignmentDate)
                            {
                                int? assignmentId = 1;

                                var assignmentData = this.context?.Assignment.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId /*&& x.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId*/).OrderByDescending(x => x.AssignmentId).FirstOrDefault();

                                if (assignmentData != null)
                                {
                                    assignmentId = assignmentData.AssignmentId + 1;
                                }
                                assignmentAddViewModel.assignment.AssignmentId = (int)assignmentId;
                                assignmentAddViewModel.assignment.CreatedOn = DateTime.UtcNow;
                                this.context?.Assignment.Add(assignmentAddViewModel.assignment);
                                this.context?.SaveChanges();
                                assignmentAddViewModel._failure = false;
                                assignmentAddViewModel._message = "Assignment Added Successfully";
                            }
                            else
                            {
                                assignmentAddViewModel._failure = true;
                                assignmentAddViewModel._message = "Due Date Should Be Same Or Greater Than Assign Date";
                            }
                        }
                        else
                        {
                            assignmentAddViewModel._failure = true;
                            assignmentAddViewModel._message = "Assigned Date And Due Date Must Be Within The Course Section Start Date And End Date.";
                        }
                    }
                }
            }
            catch (Exception es)
            {
                assignmentAddViewModel._failure = true;
                assignmentAddViewModel._message = es.Message;
            }
            return assignmentAddViewModel;
        }

        /// <summary>
        /// Update Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel UpdateAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            try
            {
                var checkAssignmentTitle = this.context?.Assignment.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId && x.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId && x.AssignmentTitle.ToLower() == assignmentAddViewModel.assignment.AssignmentTitle.ToLower() && x.AssignmentId != assignmentAddViewModel.assignment.AssignmentId && x.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId).FirstOrDefault();

                if (checkAssignmentTitle != null)
                {
                    assignmentAddViewModel._failure = true;
                    assignmentAddViewModel._message = "Assignment Title Already Exists";
                }
                else
                {
                    var courseSectionData = this.context?.CourseSection.FirstOrDefault(e => e.SchoolId == assignmentAddViewModel.assignment.SchoolId && e.SchoolId == assignmentAddViewModel.assignment.SchoolId && e.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId);

                    if (courseSectionData != null)
                    {
                        if ((assignmentAddViewModel.assignment.AssignmentDate >= courseSectionData.DurationStartDate && assignmentAddViewModel.assignment.AssignmentDate <= courseSectionData.DurationEndDate) && (assignmentAddViewModel.assignment.DueDate >= courseSectionData.DurationStartDate && assignmentAddViewModel.assignment.DueDate <= courseSectionData.DurationEndDate))
                        {
                            if (assignmentAddViewModel.assignment.DueDate >= assignmentAddViewModel.assignment.AssignmentDate)
                            {
                                var AssignmentData = this.context?.Assignment.FirstOrDefault(c => c.SchoolId == assignmentAddViewModel.assignment.SchoolId && c.TenantId == assignmentAddViewModel.assignment.TenantId /*&& c.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId*/ && c.AssignmentId == assignmentAddViewModel.assignment.AssignmentId && c.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId);

                                if (AssignmentData != null)
                                {
                                    assignmentAddViewModel.assignment.StaffId = AssignmentData.StaffId;
                                    assignmentAddViewModel.assignment.CreatedBy = AssignmentData.CreatedBy;
                                    assignmentAddViewModel.assignment.CreatedOn = AssignmentData.CreatedOn;
                                    assignmentAddViewModel.assignment.UpdatedOn = DateTime.UtcNow;
                                    this.context.Entry(AssignmentData).CurrentValues.SetValues(assignmentAddViewModel.assignment);
                                    this.context?.SaveChanges();
                                    assignmentAddViewModel._failure = false;
                                    assignmentAddViewModel._message = "Assignment Updated Successfully";
                                }
                                else
                                {
                                    assignmentAddViewModel._failure = true;
                                    assignmentAddViewModel._message = NORECORDFOUND;
                                }
                            }
                            else
                            {
                                assignmentAddViewModel._failure = true;
                                assignmentAddViewModel._message = "Due Date Should Be Same Or Greater Than Assign Date";
                            }
                        }
                        else
                        {
                            assignmentAddViewModel._failure = true;
                            assignmentAddViewModel._message = "Assigned Date And Due Date Must Be Within The Course Section Start Date And End Date.";
                        }
                    }
                }
            }
            catch (Exception es)
            {
                assignmentAddViewModel._failure = true;
                assignmentAddViewModel._message = es.Message;
            }
            return assignmentAddViewModel;
        }

        /// <summary>
        /// Delete Assignment
        /// </summary>
        /// <param name="assignmentAddViewModel"></param>
        /// <returns></returns>
        public AssignmentAddViewModel DeleteAssignment(AssignmentAddViewModel assignmentAddViewModel)
        {
            try
            {
                var AssignmentData = this.context?.Assignment.FirstOrDefault(c => c.SchoolId == assignmentAddViewModel.assignment.SchoolId && c.TenantId == assignmentAddViewModel.assignment.TenantId /*&& c.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId*/ && c.AssignmentId == assignmentAddViewModel.assignment.AssignmentId /*&& c.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId*/);

                if (AssignmentData != null)
                {
                    this.context?.Assignment.Remove(AssignmentData);
                    this.context?.SaveChanges();
                    assignmentAddViewModel._failure = false;
                    assignmentAddViewModel._message = "Assignment Deleted Succesfully";
                }
                else
                {
                    assignmentAddViewModel._failure = true;
                    assignmentAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                assignmentAddViewModel._failure = true;
                assignmentAddViewModel._message = es.Message;
            }
            return assignmentAddViewModel;
        }

        /// <summary>
        /// Get All Assignment Type
        /// </summary>
        /// <param name="assignmentTypeListViewModel"></param>
        /// <returns></returns>
        public AssignmentTypeListViewModel GetAllAssignmentType(AssignmentTypeListViewModel assignmentTypeListViewModel)
        {
            AssignmentTypeListViewModel assignmentTypeListView = new AssignmentTypeListViewModel();
            try
            {
                int? totalWeightPercent = 0;
                bool weightedGrades = false;

                var assignmentTypeList = this.context?.AssignmentType.Include(b => b.Assignment).Where(c => c.SchoolId == assignmentTypeListViewModel.SchoolId && c.TenantId == assignmentTypeListViewModel.TenantId && c.CourseSectionId == assignmentTypeListViewModel.CourseSectionId).Select(e=> new AssignmentType()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    AssignmentTypeId=e.AssignmentTypeId,
                    AcademicYear=e.AcademicYear,
                    MarkingPeriodId=e.MarkingPeriodId,
                    CourseSectionId=e.CourseSectionId,
                    Title=e.Title,
                    Weightage=e.Weightage,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == assignmentTypeListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn=e.CreatedOn,
                    UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == assignmentTypeListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn=e.UpdatedOn,
                    Assignment=e.Assignment.Select(f=>new Assignment()
                    { 
                        TenantId=f.TenantId,
                        SchoolId=f.SchoolId,
                        AssignmentTypeId=f.AssignmentTypeId,
                        AssignmentId=f.AssignmentId,
                        CourseSectionId=f.CourseSectionId,
                        AssignmentTitle=f.AssignmentTitle,
                        Points=f.Points,
                        AssignmentDate=f.AssignmentDate,
                        DueDate=f.DueDate,
                        AssignmentDescription=f.AssignmentDescription,
                        StaffId=f.StaffId,
                        CreatedBy= (f.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == assignmentTypeListViewModel.TenantId && u.EmailAddress == f.CreatedBy).Name : null,
                        CreatedOn=f.CreatedOn,
                        UpdatedBy= (f.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == assignmentTypeListViewModel.TenantId && u.EmailAddress == f.UpdatedBy).Name : null,
                        UpdatedOn=f.UpdatedOn
                    }).ToList()
                }).ToList();

                if (assignmentTypeList.Count > 0)
                {
                    totalWeightPercent = assignmentTypeList.Select(c => c.Weightage).Sum();

                    var checkWeightGradesData = this.context?.GradebookConfiguration.FirstOrDefault(e => e.SchoolId == assignmentTypeListViewModel.SchoolId && e.TenantId == assignmentTypeListViewModel.TenantId && e.General.ToLower().Contains("weightgrades"));

                    if (checkWeightGradesData != null)
                    {
                        weightedGrades = true;
                    }
                    assignmentTypeListView._failure = false;
                }
                else
                {
                    assignmentTypeListView._failure = true;
                    assignmentTypeListView._message = NORECORDFOUND;
                }
                assignmentTypeListView.assignmentTypeList = assignmentTypeList;
                assignmentTypeListView.TotalWeightage = totalWeightPercent;
                assignmentTypeListView.WeightedGrades = weightedGrades;
                assignmentTypeListView.TenantId = assignmentTypeListViewModel.TenantId;
                assignmentTypeListView.SchoolId = assignmentTypeListViewModel.SchoolId;
                assignmentTypeListView.CourseSectionId = assignmentTypeListViewModel.CourseSectionId;
                assignmentTypeListView._tenantName = assignmentTypeListViewModel._tenantName;
                assignmentTypeListView._userName = assignmentTypeListViewModel._userName;
                assignmentTypeListView._token = assignmentTypeListViewModel._token;
            }
            catch (Exception es)
            {
                assignmentTypeListView._failure = true;
                assignmentTypeListView._message = es.Message;
            }
            return assignmentTypeListView;
        }

        public AssignmentAddViewModel CopyAssignmentForCourseSection(AssignmentAddViewModel assignmentAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var assignmentTypeDataList = this.context?.AssignmentType.Where(e => e.SchoolId == assignmentAddViewModel.assignment.SchoolId && e.TenantId == assignmentAddViewModel.assignment.TenantId).ToList();

                    if (assignmentTypeDataList.Count > 0)
                    {
                        // Generate Assignment ID
                        int? assignmentId = 1;

                        var assignmentData = this.context?.Assignment.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId).OrderByDescending(x => x.AssignmentId).FirstOrDefault();

                        if (assignmentData != null)
                        {
                            assignmentId = assignmentData.AssignmentId + 1;
                        }

                        // Generate Assignment Type ID
                        int? assignmentTypeId = 1;

                        var assignmentType = assignmentTypeDataList.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId).OrderByDescending(x => x.AssignmentTypeId).FirstOrDefault();

                        if (assignmentType != null)
                        {
                            assignmentTypeId = assignmentType.AssignmentTypeId + 1;
                        }

                        var assignmentTypeData = assignmentTypeDataList.FirstOrDefault(c => c.CourseSectionId == assignmentAddViewModel.assignment.CourseSectionId && c.AssignmentTypeId == assignmentAddViewModel.assignment.AssignmentTypeId);

                        if (assignmentTypeData != null)
                        {
                            foreach (var courseSectionId in assignmentAddViewModel.courseSectionIds)
                            {
                                var checkAssignmentType = assignmentTypeDataList.FirstOrDefault(x => x.CourseSectionId == courseSectionId && x.Title.ToLower() == assignmentTypeData.Title.ToLower());

                                if (checkAssignmentType != null)
                                {
                                    var checkAssignmentTitle = this.context?.Assignment.Where(x => x.SchoolId == assignmentAddViewModel.assignment.SchoolId && x.TenantId == assignmentAddViewModel.assignment.TenantId && x.CourseSectionId == courseSectionId && x.AssignmentTitle.ToLower() == assignmentAddViewModel.assignment.AssignmentTitle.ToLower() && x.AssignmentTypeId == checkAssignmentType.AssignmentTypeId).FirstOrDefault();

                                    if (checkAssignmentTitle != null)
                                    {
                                        assignmentAddViewModel._failure = true;
                                        assignmentAddViewModel._message = "Assignment Title Already Exists";
                                        return assignmentAddViewModel;
                                    }
                                    else
                                    {
                                        var assignmentAdd = new Assignment()
                                        {
                                            TenantId = assignmentAddViewModel.assignment.TenantId,
                                            SchoolId = assignmentAddViewModel.assignment.SchoolId,
                                            AssignmentTypeId = checkAssignmentType.AssignmentTypeId,
                                            AssignmentId = (int)assignmentId,
                                            CourseSectionId = courseSectionId,
                                            AssignmentTitle = assignmentAddViewModel.assignment.AssignmentTitle,
                                            Points = assignmentAddViewModel.assignment.Points,
                                            AssignmentDate = assignmentAddViewModel.assignment.AssignmentDate,
                                            DueDate = assignmentAddViewModel.assignment.DueDate,
                                            AssignmentDescription = assignmentAddViewModel.assignment.AssignmentDescription,
                                            StaffId = assignmentAddViewModel.assignment.StaffId,
                                            CreatedBy = assignmentAddViewModel.assignment.CreatedBy,
                                            CreatedOn = DateTime.UtcNow
                                        };
                                        this.context?.Assignment.Add(assignmentAdd);
                                        assignmentId++;
                                    }

                                }
                                else
                                {
                                    var assignmentTypeAdd = new AssignmentType()
                                    {
                                        TenantId = assignmentAddViewModel.assignment.TenantId,
                                        SchoolId = assignmentAddViewModel.assignment.SchoolId,
                                        AssignmentTypeId = (int)assignmentTypeId,
                                        AcademicYear = assignmentTypeData.AcademicYear,
                                        MarkingPeriodId = assignmentTypeData.MarkingPeriodId,
                                        CourseSectionId = courseSectionId,
                                        Title = assignmentTypeData.Title,
                                        Weightage = assignmentTypeData.Weightage,
                                        CreatedBy = assignmentAddViewModel.assignment.CreatedBy,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    this.context?.AssignmentType.Add(assignmentTypeAdd);                                    
                                    //this.context?.SaveChanges();

                                    var AssignmentDataAdd = new Assignment()
                                    {
                                        TenantId = assignmentAddViewModel.assignment.TenantId,
                                        SchoolId = assignmentAddViewModel.assignment.SchoolId,
                                        AssignmentTypeId = (int)assignmentTypeId,
                                        AssignmentId = (int)assignmentId,
                                        CourseSectionId = courseSectionId,
                                        AssignmentTitle = assignmentAddViewModel.assignment.AssignmentTitle,
                                        Points = assignmentAddViewModel.assignment.Points,
                                        AssignmentDate = assignmentAddViewModel.assignment.AssignmentDate,
                                        DueDate = assignmentAddViewModel.assignment.DueDate,
                                        AssignmentDescription = assignmentAddViewModel.assignment.AssignmentDescription,
                                        StaffId = assignmentAddViewModel.assignment.StaffId,
                                        CreatedBy = assignmentAddViewModel.assignment.CreatedBy,
                                        CreatedOn = DateTime.UtcNow
                                    };
                                    this.context?.Assignment.Add(AssignmentDataAdd);
                                    assignmentId++;
                                    assignmentTypeId++;
                                }
                            }
                            this.context?.SaveChanges();
                            transaction.Commit();
                            assignmentAddViewModel._failure = false;
                            assignmentAddViewModel._message = "Assignment Copied Successfully";
                        }
                    }
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    assignmentAddViewModel._failure = true;
                    assignmentAddViewModel._message = es.Message;
                }
            }
            return assignmentAddViewModel;
        }
    }
}
