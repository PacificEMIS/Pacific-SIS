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
using opensis.data.ViewModels.StaffPortalGradebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
   public class StaffPortalGradebookRepository: IStaffPortalGradebookRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffPortalGradebookRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Add Update Gradebook Configuration
        /// </summary>
        /// <param name="gradebookConfigurationAddViewModel"></param>
        /// <returns></returns>
        public GradebookConfigurationAddViewModel AddUpdateGradebookConfiguration(GradebookConfigurationAddViewModel gradebookConfigurationAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (gradebookConfigurationAddViewModel.gradebookConfiguration != null)
                    {
                        gradebookConfigurationAddViewModel.gradebookConfiguration.AcademicYear = (decimal)Utility.GetCurrentAcademicYear(this.context!, gradebookConfigurationAddViewModel.gradebookConfiguration!.TenantId, gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId)!;

                        var GradebookConfigurationData = this.context?.GradebookConfiguration.Include(x => x.GradebookConfigurationGradescale).Include(x => x.GradebookConfigurationYear).Include(x => x.GradebookConfigurationSemester).Include(x => x.GradebookConfigurationQuarter).Include(x => x.GradebookConfigurationProgressPeriods).FirstOrDefault(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId && x.SchoolId == gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId && x.CourseId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseId && x.CourseSectionId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseSectionId && x.AcademicYear == gradebookConfigurationAddViewModel.gradebookConfiguration.AcademicYear);

                        if (GradebookConfigurationData != null)
                        {
                            if (GradebookConfigurationData.GradebookConfigurationGradescale.Count > 0)
                            {
                                this.context?.GradebookConfigurationGradescale.RemoveRange(GradebookConfigurationData.GradebookConfigurationGradescale);
                            }
                            if (GradebookConfigurationData.GradebookConfigurationYear.Count > 0)
                            {
                                this.context?.GradebookConfigurationYear.RemoveRange(GradebookConfigurationData.GradebookConfigurationYear);
                            }
                            if (GradebookConfigurationData.GradebookConfigurationSemester.Count > 0)
                            {
                                this.context?.GradebookConfigurationSemester.RemoveRange(GradebookConfigurationData.GradebookConfigurationSemester);
                            }
                            if (GradebookConfigurationData.GradebookConfigurationQuarter.Count > 0)
                            {
                                this.context?.GradebookConfigurationQuarter.RemoveRange(GradebookConfigurationData.GradebookConfigurationQuarter);
                            }
                            if (GradebookConfigurationData.GradebookConfigurationProgressPeriods.Count > 0)
                            {
                                this.context?.GradebookConfigurationProgressPeriods.RemoveRange(GradebookConfigurationData.GradebookConfigurationProgressPeriods);
                            }
                            this.context?.SaveChanges();

                            gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedBy = GradebookConfigurationData.CreatedBy;
                            gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedOn = GradebookConfigurationData.CreatedOn;
                            gradebookConfigurationAddViewModel.gradebookConfiguration.UpdatedOn = DateTime.UtcNow;
                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId;
                            this.context?.Entry(GradebookConfigurationData).CurrentValues.SetValues(gradebookConfigurationAddViewModel.gradebookConfiguration);


                            int? gcgsId = 1;
                            var GradebookConfigurationGradescaleData = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationGradescaleData != null)
                            {
                                gcgsId = GradebookConfigurationGradescaleData.Id + 1;
                            }

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationGradescale.ToList().ForEach(x => { x.Id = (int)gcgsId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow; });
                            this.context?.GradebookConfigurationGradescale.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationGradescale);

                            int? gcyId = 1;
                            var GradebookConfigurationYearData = this.context?.GradebookConfigurationYear.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationYearData != null)
                            {
                                gcyId = GradebookConfigurationYearData.Id + 1;
                            }
                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationYear.ToList().ForEach(x => { x.Id = (int)gcyId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow; });
                            this.context?.GradebookConfigurationYear.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationYear);

                            int? gcsId = 1;
                            var GradebookConfigurationSemesterData = this.context?.GradebookConfigurationSemester.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationSemesterData != null)
                            {
                                gcsId = GradebookConfigurationSemesterData.Id + 1;
                            }

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationSemester.ToList().ForEach(x => { x.Id = (int)gcsId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow; });
                            this.context?.GradebookConfigurationSemester.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationSemester);

                            int? gcqId = 1;
                            var GradebookConfigurationQuarterData = this.context?.GradebookConfigurationQuarter.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationQuarterData != null)
                            {
                                gcqId = GradebookConfigurationQuarterData.Id + 1;
                            }

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter.ToList().ForEach(x => { x.Id = (int)gcqId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow;});
                            this.context?.GradebookConfigurationQuarter.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter);

                            int? gcppId = 1;
                            var GradebookConfigurationProgressPeriodData = this.context?.GradebookConfigurationProgressPeriods.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationProgressPeriodData != null)
                            {
                                gcppId = GradebookConfigurationProgressPeriodData.Id + 1;
                            }

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationProgressPeriods.ToList().ForEach(x => { x.Id = (int)gcppId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow; });
                            this.context?.GradebookConfigurationProgressPeriods.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationProgressPeriods);


                            this.context?.SaveChanges();
                            gradebookConfigurationAddViewModel._message = "Gradebook Configuration Updated Successfully";
                        }
                        else
                        {
                            int? gcId = 1;
                            int? gcgsId = 1;
                            int? gcyId = 1;
                            int? gcsId = 1;
                            int? gcqId = 1;
                            int? gcppId = 1;

                            var GradebookData = this.context?.GradebookConfiguration.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId && x.SchoolId == gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId).OrderByDescending(x => x.GradebookConfigurationId).FirstOrDefault();
                            if (GradebookData != null)
                            {
                                gcId = GradebookData.GradebookConfigurationId + 1;
                            }

                            var GradebookConfigurationGradescaleData = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationGradescaleData != null)
                            {
                                gcgsId = GradebookConfigurationGradescaleData.Id + 1;
                            }

                            var GradebookConfigurationYearData = this.context?.GradebookConfigurationYear.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationYearData != null)
                            {
                                gcyId = GradebookConfigurationYearData.Id + 1;
                            }

                            var GradebookConfigurationSemesterData = this.context?.GradebookConfigurationSemester.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationSemesterData != null)
                            {
                                gcsId = GradebookConfigurationSemesterData.Id + 1;
                            }

                            var GradebookConfigurationQuarterData = this.context?.GradebookConfigurationQuarter.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationQuarterData != null)
                            {
                                gcqId = GradebookConfigurationQuarterData.Id + 1;
                            }

                            var GradebookConfigurationProgressPeriodData = this.context?.GradebookConfigurationProgressPeriods.Where(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (GradebookConfigurationProgressPeriodData != null)
                            {
                                gcppId = GradebookConfigurationProgressPeriodData.Id + 1;
                            }

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationGradescale.ToList().ForEach(x => { x.Id = (int)gcgsId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationYear.ToList().ForEach(x => { x.Id = (int)gcyId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationSemester.ToList().ForEach(x => { x.Id = (int)gcsId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter.ToList().ForEach(x => { x.Id = (int)gcqId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationProgressPeriods.ToList().ForEach(x => { x.Id = (int)gcppId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                            gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationId = (int)gcId;
                            gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedOn = DateTime.UtcNow;
                            this.context?.GradebookConfiguration.Add(gradebookConfigurationAddViewModel.gradebookConfiguration);
                            this.context?.SaveChanges();
                            gradebookConfigurationAddViewModel._message = "Gradebook Configuration Added Successfully";

                        }
                        transaction?.Commit();
                        gradebookConfigurationAddViewModel._failure = false;
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    gradebookConfigurationAddViewModel._failure = true;
                    gradebookConfigurationAddViewModel._message = es.Message;
                }
            }
            return gradebookConfigurationAddViewModel;
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
                if (gradebookConfigurationAddViewModel.gradebookConfiguration != null)
                {
                    var GradebookConfigurationData = this.context?.GradebookConfiguration.Include(x => x.GradebookConfigurationGradescale).Include(x => x.GradebookConfigurationYear).Include(x => x.GradebookConfigurationSemester).Include(x => x.GradebookConfigurationQuarter).Include(x => x.GradebookConfigurationProgressPeriods).FirstOrDefault(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId && x.SchoolId == gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId && x.CourseId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseId && x.CourseSectionId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseSectionId && x.AcademicYear == gradebookConfigurationAddViewModel.gradebookConfiguration.AcademicYear);
                    if (GradebookConfigurationData != null)
                    {
                        gradebookConfigurationView.gradebookConfiguration = GradebookConfigurationData;
                        gradebookConfigurationView._failure = false;
                    }
                    else
                    {
                        gradebookConfigurationView._message = NORECORDFOUND;
                        gradebookConfigurationView._failure = true;
                    }
                    gradebookConfigurationView._tenantName = gradebookConfigurationAddViewModel._tenantName;
                    gradebookConfigurationView._token = gradebookConfigurationAddViewModel._token;

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
            finalGradingMarkingPeriod.TenantId = finalGradingMarkingPeriodList.TenantId;
            finalGradingMarkingPeriod.SchoolId = finalGradingMarkingPeriodList.SchoolId;
            finalGradingMarkingPeriod._tenantName = finalGradingMarkingPeriodList._tenantName;
            finalGradingMarkingPeriod._token = finalGradingMarkingPeriodList._token;
            finalGradingMarkingPeriod.AcademicYear = finalGradingMarkingPeriodList.AcademicYear;
            finalGradingMarkingPeriod.CourseSectionId = finalGradingMarkingPeriodList.CourseSectionId;
            finalGradingMarkingPeriod.IsConfiguration = finalGradingMarkingPeriodList.IsConfiguration;
            try
            {
                if (finalGradingMarkingPeriodList.IsConfiguration == true)
                {
                    // this block for when this api call from gradebook configuration.
                    var courseSectionData = this.context?.CourseSection.Where(x => x.TenantId == finalGradingMarkingPeriodList.TenantId && x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.CourseSectionId == finalGradingMarkingPeriodList.CourseSectionId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear).FirstOrDefault();
                    if (courseSectionData != null)
                    {
                        if (courseSectionData.YrMarkingPeriodId != null || courseSectionData.DurationBasedOnPeriod == false)
                        {
                            var markingPeriodDataList = this.context?.SchoolYears.Include(x => x.Semesters).ThenInclude(p => p.Quarters).ThenInclude(a => a.ProgressPeriods).Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear).FirstOrDefault();

                            if (markingPeriodDataList != null)
                            {
                                finalGradingMarkingPeriod.schoolYears = markingPeriodDataList;

                                if (markingPeriodDataList.Semesters.Count > 0 || markingPeriodDataList.Semesters != null)
                                {
                                    finalGradingMarkingPeriod.semesters.AddRange(markingPeriodDataList.Semesters);

                                    foreach (var markingperiodData in markingPeriodDataList.Semesters)
                                    {
                                        if (markingperiodData.Quarters?.Count > 0 || markingperiodData.Quarters != null)
                                        {
                                            finalGradingMarkingPeriod.quarters.AddRange(markingperiodData.Quarters.ToList());

                                            foreach (var markingperiod in markingperiodData.Quarters)
                                            {
                                                if (markingperiod.ProgressPeriods?.Count > 0 || markingperiod.ProgressPeriods != null)
                                                {
                                                    finalGradingMarkingPeriod.progressPeriods.AddRange(markingperiod.ProgressPeriods.ToList());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (courseSectionData.SmstrMarkingPeriodId != null)
                        {
                            var markingPeriodDataList = this.context?.Semesters.Include(p => p.Quarters).ThenInclude(a => a.ProgressPeriods).Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear && x.MarkingPeriodId == courseSectionData.SmstrMarkingPeriodId).FirstOrDefault();

                            if (markingPeriodDataList != null)
                            {
                                finalGradingMarkingPeriod.semesters.Add(markingPeriodDataList);

                                if (markingPeriodDataList.Quarters.Count > 0 || markingPeriodDataList.Quarters != null)
                                {
                                    finalGradingMarkingPeriod.quarters.AddRange(markingPeriodDataList.Quarters);

                                    foreach (var quater in markingPeriodDataList.Quarters)
                                    {
                                        if (quater.ProgressPeriods?.Count > 0 || quater.ProgressPeriods != null)
                                        {
                                            finalGradingMarkingPeriod.progressPeriods.AddRange(quater.ProgressPeriods.ToList());
                                        }
                                    }
                                }
                            }
                        }
                        else if (courseSectionData.QtrMarkingPeriodId != null)
                        {
                            var markingPeriodDataList = this.context?.Quarters.Include(a => a.ProgressPeriods).Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear && x.MarkingPeriodId == courseSectionData.QtrMarkingPeriodId).FirstOrDefault();

                            if (markingPeriodDataList != null)
                            {
                                finalGradingMarkingPeriod.quarters.Add(markingPeriodDataList);

                                if (markingPeriodDataList.ProgressPeriods.Count > 0 || markingPeriodDataList.ProgressPeriods != null)
                                {
                                    finalGradingMarkingPeriod.progressPeriods.AddRange(markingPeriodDataList.ProgressPeriods);

                                }
                            }
                        }
                        else if (courseSectionData.PrgrsprdMarkingPeriodId != null)
                        {
                            var markingPeriodDataList = this.context?.ProgressPeriods.Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear && x.MarkingPeriodId == courseSectionData.PrgrsprdMarkingPeriodId).FirstOrDefault();

                            if (markingPeriodDataList != null)
                            {
                                finalGradingMarkingPeriod.progressPeriods.Add(markingPeriodDataList);
                            }
                        }
                    }
                }
                else
                {
                    var markingPeriodDataList = this.context?.SchoolYears.Include(x => x.Semesters).ThenInclude(p => p.Quarters).ThenInclude(a => a.ProgressPeriods).Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear).FirstOrDefault();

                    if (markingPeriodDataList != null)
                    {
                        if (markingPeriodDataList.Semesters.Count > 0 || markingPeriodDataList.Semesters != null)
                        {
                            finalGradingMarkingPeriod.semesters.AddRange(markingPeriodDataList.Semesters);

                            foreach (var markingperiodData in markingPeriodDataList.Semesters)
                            {
                                if (markingperiodData.Quarters?.Count > 0 || markingperiodData.Quarters != null)
                                {
                                    finalGradingMarkingPeriod.quarters.AddRange(markingperiodData.Quarters.ToList());

                                    foreach (var markingperiod in markingperiodData.Quarters)
                                    {
                                        if (markingperiod.ProgressPeriods?.Count > 0 || markingperiod.ProgressPeriods != null)
                                        {
                                            finalGradingMarkingPeriod.progressPeriods.AddRange(markingperiod.ProgressPeriods.ToList());
                                        }
                                    }
                                }
                            }
                        }
                        finalGradingMarkingPeriod.schoolYears = markingPeriodDataList;
                    }
                }
                //remove extra data
                if (finalGradingMarkingPeriod.progressPeriods.Count > 0)
                {
                    finalGradingMarkingPeriod.progressPeriods.ForEach(e => { e.Quarters = new(); e.CourseSections = new List<CourseSection>(); e.StaffCoursesectionSchedules = new List<StaffCoursesectionSchedule>(); e.StudentEffortGradeMasters = new List<StudentEffortGradeMaster>(); e.StudentFinalGrades = new List<StudentFinalGrade>(); });
                }

                if (finalGradingMarkingPeriod.quarters.Count > 0)
                {
                    finalGradingMarkingPeriod.quarters.ForEach(e => { e.Semesters = null; e.CourseSection = new List<CourseSection>(); e.StaffCoursesectionSchedule = new List<StaffCoursesectionSchedule>(); e.StudentEffortGradeMaster = new List<StudentEffortGradeMaster>(); e.StudentFinalGrade = new List<StudentFinalGrade>(); });
                }

                if (finalGradingMarkingPeriod.semesters.Count > 0)
                {
                    finalGradingMarkingPeriod.semesters.ForEach(e => { e.SchoolYears = null; e.CourseSection = new List<CourseSection>(); e.StaffCoursesectionSchedule = new List<StaffCoursesectionSchedule>(); e.StudentEffortGradeMaster = new List<StudentEffortGradeMaster>(); e.StudentFinalGrade = new List<StudentFinalGrade>(); });
                }

                if (finalGradingMarkingPeriod.schoolYears != null)
                {
                    finalGradingMarkingPeriod.schoolYears.CourseSection = new List<CourseSection>();
                    //finalGradingMarkingPeriod.schoolYears.HonorRolls = new List<HonorRolls>();
                    finalGradingMarkingPeriod.schoolYears.StaffCoursesectionSchedule = new List<StaffCoursesectionSchedule>();
                    finalGradingMarkingPeriod.schoolYears.StudentEffortGradeMaster = new List<StudentEffortGradeMaster>();
                    finalGradingMarkingPeriod.schoolYears.StudentFinalGrade = new List<StudentFinalGrade>();
                    //finalGradingMarkingPeriod.schoolYears.Semesters.ToList().ForEach(v => v.Quarters = null);
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
        /// <param name="gradebookGradeListViewModel"></param>
        /// <returns></returns>
        public GradebookGradeListViewModel AddGradebookGrade(GradebookGradeListViewModel gradebookGradeListViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    gradebookGradeListViewModel.AcademicYear = (decimal)Utility.GetCurrentAcademicYear(this.context!, gradebookGradeListViewModel.TenantId, gradebookGradeListViewModel.SchoolId)!;

                    var courseSectionData = this.context?.CourseSection.Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == gradebookGradeListViewModel.AcademicYear);

                    var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == gradebookGradeListViewModel.AcademicYear && x.BreakoffPoints > 0).ToList();

                    var AssignmentTypeIdList = gradebookGradeListViewModel.assignmentsListViewModels.ToList().Select(x => x.AssignmentTypeId).Distinct();

                    var AssignmentTypeData = this.context?.AssignmentType.Where(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == gradebookGradeListViewModel.AcademicYear && AssignmentTypeIdList.Contains(x.AssignmentTypeId));

                    int? YrMarkingPeriodId = null;
                    int? SmstrMarkingPeriodId = null;
                    int? QtrMarkingPeriodId = null;
                    int? PrgrsprdMarkingPeriodId = null;

                    if (AssignmentTypeData?.Any() == true)
                    {
                        YrMarkingPeriodId = AssignmentTypeData.FirstOrDefault()!.YrMarkingPeriodId;
                        SmstrMarkingPeriodId = AssignmentTypeData.FirstOrDefault()!.SmstrMarkingPeriodId;
                        QtrMarkingPeriodId = AssignmentTypeData.FirstOrDefault()!.QtrMarkingPeriodId;
                        PrgrsprdMarkingPeriodId = AssignmentTypeData.FirstOrDefault()!.PrgrsprdMarkingPeriodId;
                    }

                    //var studentIds = gradebookGradeListViewModel.assignmentsListViewModels.FirstOrDefault().studentsListViewModels.Select(x => x.StudentId).ToList();
                    var studentIds = gradebookGradeListViewModel.assignmentsListViewModels.FirstOrDefault()!.studentsListViewModels.Select(x => x.StudentId).ToList();

                    foreach (var studentId in studentIds)
                    {
                        List<GradebookGrades> GradebookGradeList = new();

                        var GradebookGradeData = this.context?.GradebookGrades.Where(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId /*&& x.MarkingPeriodId == gradebookGradeListViewModel.MarkingPeriodId*/ && x.AcademicYear == gradebookGradeListViewModel.AcademicYear && x.StudentId == studentId && (PrgrsprdMarkingPeriodId != null && x.PrgrsprdMarkingPeriodId == PrgrsprdMarkingPeriodId || QtrMarkingPeriodId != null && x.QtrMarkingPeriodId == QtrMarkingPeriodId || SmstrMarkingPeriodId != null && x.SmstrMarkingPeriodId == SmstrMarkingPeriodId || YrMarkingPeriodId != null && x.YrMarkingPeriodId == YrMarkingPeriodId)).ToList();

                        if (GradebookGradeData != null && GradebookGradeData.Any())
                        {
                            this.context?.GradebookGrades.RemoveRange(GradebookGradeData);
                            this.context?.SaveChanges();
                        }

                        var totalWeitage = AssignmentTypeData?.ToList().Sum(x => x.Weightage);

                        decimal? runingAvgSum = 0.0m;

                        decimal? sumOfAssignmentTypeAvg = 0.0m;
                        foreach (var AssignmentTypeId in AssignmentTypeIdList)
                        {
                            decimal? assignmentTypeAvg = 0.0m;
                            int count = 0;
                            var assignmentByType = gradebookGradeListViewModel.assignmentsListViewModels.Where(x => x.AssignmentTypeId == AssignmentTypeId).ToList();

                            foreach (var assignment in assignmentByType)
                            {
                                decimal? weightPersentage = 0.0m;
                                count++;
                                decimal? assignmentPercentage = 0.0m;
                                string assignmentGrade = "";

                                var student = assignment.studentsListViewModels.FirstOrDefault(s => s.StudentId == studentId);

                                if (student != null)
                                {
                                    if (!string.IsNullOrEmpty(student.AllowedMarks))
                                    {
                                        if (student.AllowedMarks != "*")
                                        {
                                            weightPersentage = Math.Round((Convert.ToDecimal(assignment.Weightage) / Convert.ToDecimal(totalWeitage) * 100), 2);

                                            assignmentPercentage = Math.Round(100 * (Convert.ToDecimal(student.AllowedMarks) / Convert.ToDecimal(student.Points)), 2);

                                            var runingAvg = Math.Round((decimal)(assignmentPercentage * weightPersentage / 100), 2);

                                            assignmentTypeAvg = assignmentTypeAvg + runingAvg;

                                            if (courseSectionData?.GradeScale?.Grade != null)
                                            {
                                                if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                                                {
                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(assignmentPercentage));
                                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title ?? "";
                                                }
                                                else
                                                {
                                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= assignmentPercentage)?.Title ?? "";
                                                }
                                            }
                                        }
                                        var gradebookGrade = new GradebookGrades
                                        {
                                            TenantId = gradebookGradeListViewModel.TenantId,
                                            SchoolId = gradebookGradeListViewModel.SchoolId,
                                            StudentId = student.StudentId,
                                            AcademicYear = gradebookGradeListViewModel.AcademicYear,
                                            //MarkingPeriodId = gradebookGradeListViewModel.MarkingPeriodId,
                                            CourseSectionId = gradebookGradeListViewModel.CourseSectionId,
                                            AssignmentTypeId = assignment.AssignmentTypeId,
                                            AssignmentId = assignment.AssignmentId,
                                            AllowedMarks = student.AllowedMarks,
                                            Percentage = assignmentPercentage.ToString(),
                                            LetterGrade = assignmentGrade,
                                            Comment = student.Comment,
                                            CreatedBy = gradebookGradeListViewModel.CreatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                            YrMarkingPeriodId = YrMarkingPeriodId,
                                            SmstrMarkingPeriodId = SmstrMarkingPeriodId,
                                            QtrMarkingPeriodId = QtrMarkingPeriodId,
                                            PrgrsprdMarkingPeriodId = PrgrsprdMarkingPeriodId
                                        };
                                        GradebookGradeList.Add(gradebookGrade);
                                    }
                                }
                            }
                            sumOfAssignmentTypeAvg = Math.Round((decimal)assignmentTypeAvg / Convert.ToDecimal(count), 2);
                            runingAvgSum = runingAvgSum + sumOfAssignmentTypeAvg;
                        }

                        string? runningGrade = null;
                        if (courseSectionData?.GradeScale?.Grade != null)
                        {
                            if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                            {

                                var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(runingAvgSum));
                                if (ConfigurationGrade != null)
                                {
                                    runningGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title ?? "";
                                }
                            }
                            runningGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= runingAvgSum)?.Title ?? "";
                        }
                        GradebookGradeList.ForEach(x => { x.RunningAvg = (Math.Round((decimal)runingAvgSum, 2).ToString()); x.RunningAvgGrade = runningGrade; });
                        this.context?.GradebookGrades.AddRange(GradebookGradeList);
                        this.context?.SaveChanges();
                    }
                    transaction?.Commit();
                    gradebookGradeListViewModel._message = "Data Submitted Successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    gradebookGradeListViewModel._message = es.Message;
                    gradebookGradeListViewModel._failure = true;
                }
            }
            return gradebookGradeListViewModel;
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
                gradebookGradeList.TenantId = gradebookGradeListViewModel.TenantId;
                gradebookGradeList.SchoolId = gradebookGradeListViewModel.SchoolId;
                gradebookGradeList.CourseSectionId = gradebookGradeListViewModel.CourseSectionId;
                gradebookGradeList.AcademicYear = gradebookGradeListViewModel.AcademicYear;
                gradebookGradeList._tenantName = gradebookGradeListViewModel._tenantName;
                gradebookGradeList._token = gradebookGradeListViewModel._token;

                var AssignmentTypeData = this.context?.AssignmentType.Include(x => x.Assignment).Where(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == gradebookGradeListViewModel.AcademicYear).ToList();

                var studentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Include(x => x.StudentMaster).ThenInclude(x => x.GradebookGrades).Where(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == gradebookGradeListViewModel.AcademicYear && (gradebookGradeListViewModel.IncludeInactive == false || gradebookGradeListViewModel.IncludeInactive == null ? x.IsDropped != true : true)).ToList();
                if (studentCoursesectionScheduleData != null && studentCoursesectionScheduleData.Any())
                {
                    //Fetch Assignment Type By Marking Period
                    if (AssignmentTypeData?.Any() == true)
                    {
                        if (AssignmentTypeData.FirstOrDefault()!.PrgrsprdMarkingPeriodId != null)
                        {
                            var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == gradebookGradeListViewModel.SchoolId && x.TenantId == gradebookGradeListViewModel.TenantId && x.StartDate == gradebookGradeListViewModel.MarkingPeriodStartDate && x.EndDate == gradebookGradeListViewModel.MarkingPeriodEndDate && x.AcademicYear == gradebookGradeListViewModel.AcademicYear).FirstOrDefault();

                            if (progressPeriodsData != null)
                            {
                                AssignmentTypeData = AssignmentTypeData.Where(x => x.PrgrsprdMarkingPeriodId == progressPeriodsData.MarkingPeriodId).ToList();
                            }
                        }
                        if (AssignmentTypeData.FirstOrDefault()!.QtrMarkingPeriodId != null)
                        {
                            var quartersData = this.context?.Quarters.Where(x => x.SchoolId == gradebookGradeListViewModel.SchoolId && x.TenantId == gradebookGradeListViewModel.TenantId && x.StartDate == gradebookGradeListViewModel.MarkingPeriodStartDate && x.EndDate == gradebookGradeListViewModel.MarkingPeriodEndDate && x.AcademicYear == gradebookGradeListViewModel.AcademicYear).FirstOrDefault();

                            if (quartersData != null)
                            {
                                AssignmentTypeData = AssignmentTypeData.Where(x => x.QtrMarkingPeriodId == quartersData.MarkingPeriodId).ToList();
                            }
                        }
                        if (AssignmentTypeData.FirstOrDefault()!.SmstrMarkingPeriodId != null)
                        {
                            var semestersData = this.context?.Semesters.Where(x => x.SchoolId == gradebookGradeListViewModel.SchoolId && x.TenantId == gradebookGradeListViewModel.TenantId && x.StartDate == gradebookGradeListViewModel.MarkingPeriodStartDate && x.EndDate == gradebookGradeListViewModel.MarkingPeriodEndDate && x.AcademicYear == gradebookGradeListViewModel.AcademicYear).FirstOrDefault();

                            if (semestersData != null)
                            {
                                AssignmentTypeData = AssignmentTypeData.Where(x => x.SmstrMarkingPeriodId == semestersData.MarkingPeriodId).ToList();
                            }
                        }
                        if (AssignmentTypeData.FirstOrDefault()!.YrMarkingPeriodId != null)
                        {
                            var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == gradebookGradeListViewModel.SchoolId && x.TenantId == gradebookGradeListViewModel.TenantId && x.StartDate == gradebookGradeListViewModel.MarkingPeriodStartDate && x.EndDate == gradebookGradeListViewModel.MarkingPeriodEndDate && x.AcademicYear == gradebookGradeListViewModel.AcademicYear).FirstOrDefault();

                            if (yearsData != null)
                            {
                                AssignmentTypeData = AssignmentTypeData.Where(x => x.YrMarkingPeriodId == yearsData.MarkingPeriodId).ToList();
                            }
                        }

                        foreach (var AssignmentType in AssignmentTypeData)
                        {
                            foreach (var Assignment in AssignmentType.Assignment)
                            {
                                var assignmentsListView = new AssignmentsListViewModel();

                                if (studentCoursesectionScheduleData.Count > 0)
                                {
                                    foreach (var studentCoursesectionSchedule in studentCoursesectionScheduleData)
                                    {
                                        StudentsListViewModel studentsListViewModel = new StudentsListViewModel();

                                        var GradebookGradeData = studentCoursesectionSchedule.StudentMaster.GradebookGrades.FirstOrDefault(x => x.TenantId == gradebookGradeListViewModel.TenantId && x.SchoolId == gradebookGradeListViewModel.SchoolId && x.CourseSectionId == gradebookGradeListViewModel.CourseSectionId && x.AcademicYear == AssignmentType.AcademicYear && x.AssignmentTypeId == Assignment.AssignmentTypeId && x.AssignmentId == Assignment.AssignmentId && x.StudentId == studentCoursesectionSchedule.StudentId);

                                        if (GradebookGradeData != null)
                                        {
                                            studentsListViewModel.AllowedMarks = GradebookGradeData.AllowedMarks;
                                            studentsListViewModel.Comment = GradebookGradeData.Comment;
                                            studentsListViewModel.Percentage = GradebookGradeData.Percentage;
                                            studentsListViewModel.LetterGrade = GradebookGradeData.LetterGrade;
                                        }

                                        studentsListViewModel.StudentInternalId = studentCoursesectionSchedule.StudentInternalId;
                                        studentsListViewModel.StudentPhoto = studentCoursesectionSchedule.StudentMaster.StudentPhoto;
                                        studentsListViewModel.FirstGivenName = studentCoursesectionSchedule.StudentMaster.FirstGivenName;
                                        studentsListViewModel.MiddleName = studentCoursesectionSchedule.StudentMaster.MiddleName;
                                        studentsListViewModel.LastFamilyName = studentCoursesectionSchedule.StudentMaster.LastFamilyName;
                                        studentsListViewModel.RunningAvg = GradebookGradeData != null ? GradebookGradeData.RunningAvg : null;
                                        studentsListViewModel.RunningAvgGrade = GradebookGradeData != null ? GradebookGradeData.RunningAvgGrade : null;
                                        studentsListViewModel.StudentId = studentCoursesectionSchedule.StudentId;
                                        studentsListViewModel.Points = Assignment.Points;
                                        assignmentsListView.studentsListViewModels.Add(studentsListViewModel);
                                    }
                                }
                                assignmentsListView.AssignmentTypeId = AssignmentType.AssignmentTypeId;
                                assignmentsListView.AssignmentId = Assignment.AssignmentId;
                                assignmentsListView.Title = AssignmentType.Title;
                                assignmentsListView.Weightage = AssignmentType.Weightage;
                                assignmentsListView.AssignmentTitle = Assignment.AssignmentTitle;
                                assignmentsListView.AssignmentDate = Assignment.AssignmentDate;
                                assignmentsListView.DueDate = Assignment.DueDate;
                                gradebookGradeList.assignmentsListViewModels.Add(assignmentsListView);

                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(gradebookGradeListViewModel.SearchValue))
                    {
                        var searchValue = Regex.Replace(gradebookGradeListViewModel.SearchValue, @"\s+", "");
                        //var studentData = gradebookGradeList.assignmentsListViewModels.FirstOrDefault().studentsListViewModels;
                        var studentData = gradebookGradeList.assignmentsListViewModels.FirstOrDefault()!.studentsListViewModels;
                        var SearchData = studentData.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(searchValue.ToLower()) ||
                         x.MiddleName != null && x.MiddleName.ToLower().Contains(searchValue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(searchValue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(searchValue.ToLower())).ToList();

                        if (SearchData.Count > 0)
                        {
                            var searchStudentIds = SearchData.Select(s => s.StudentId).ToList();
                            foreach (var assignmentsListViewModel in gradebookGradeList.assignmentsListViewModels)
                            {
                                assignmentsListViewModel.studentsListViewModels = assignmentsListViewModel.studentsListViewModels.Where(x => searchStudentIds.Contains(x.StudentId)).ToList();
                            }
                        }
                        else
                        {
                            gradebookGradeList._failure = true;
                            gradebookGradeList._message = NORECORDFOUND;
                            gradebookGradeList.assignmentsListViewModels = new List<AssignmentsListViewModel>();
                            return gradebookGradeList;
                        }
                    }
                    gradebookGradeList.assignmentsListViewModels = gradebookGradeList.assignmentsListViewModels.OrderBy(x => x.DueDate).ToList();
                    int count = 0;
                    foreach (var assignmentsListViewModel in gradebookGradeList.assignmentsListViewModels)
                    {
                        if (count == 0)
                        {
                            count++;
                            continue;
                        }
                        assignmentsListViewModel.studentsListViewModels.ForEach(x => x.StudentPhoto = null);
                    }
                }
                else
                {
                    gradebookGradeList._failure = true;
                    gradebookGradeList._message = NORECORDFOUND;
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
        /// Gradebook GradeBy Student
        /// </summary>
        /// <param name="assignmentForStudentViewModel"></param>
        /// <returns></returns>
        public AssignmentForStudentViewModel GradebookGradeByStudent(AssignmentForStudentViewModel assignmentForStudentViewModel)
        {
            AssignmentForStudentViewModel assignmentForStudent = new AssignmentForStudentViewModel();
            try
            {
                assignmentForStudent.TenantId = assignmentForStudentViewModel.TenantId;
                assignmentForStudent.SchoolId = assignmentForStudentViewModel.SchoolId;
                assignmentForStudent.StudentId = assignmentForStudentViewModel.StudentId;
                assignmentForStudent.CourseSectionId = assignmentForStudentViewModel.CourseSectionId;
                assignmentForStudent.AcademicYear = assignmentForStudentViewModel.AcademicYear;
                assignmentForStudent._tenantName = assignmentForStudentViewModel._tenantName;
                assignmentForStudent._token = assignmentForStudentViewModel._token;

                var courseSectionData = this.context?.CourseSection.Include(x=>x.GradeScale).ThenInclude(x=>x!.Grade).FirstOrDefault(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear);

                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear).ToList();

                var AssignmentTypeData = this.context?.AssignmentType.Include(x => x.Assignment).ThenInclude(x => x.GradebookGrades).Where(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId).ToList();

                if (AssignmentTypeData!=null&& AssignmentTypeData.Any())
                {
                    foreach (var assignmentType in AssignmentTypeData)
                    {
                        string? assignmentGrade = null;
                        int? allowedMarks = 0;
                        int? totalPoint = 0;
                        int count = 0;
                        AssignmentTypeViewModel assignmentTypeListViewModel = new AssignmentTypeViewModel();
                        foreach (var assignment in assignmentType.Assignment)
                        {
                            count++;
                            AssignmentViewModel assignmentViewModel = new AssignmentViewModel();

                            var gradebookGrade = assignment.GradebookGrades.FirstOrDefault(x => x.StudentId == assignmentForStudentViewModel.StudentId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear /*&& x.MarkingPeriodId == assignmentForStudentViewModel.MarkingPeriodId*/);
                            if (gradebookGrade != null)
                            {
                                assignmentViewModel.AllowedMarks = gradebookGrade.AllowedMarks;
                                assignmentViewModel.Percentage = gradebookGrade.Percentage;
                                assignmentViewModel.LetterGrade = gradebookGrade.LetterGrade;
                                assignmentViewModel.Comment = gradebookGrade.Comment;
                                allowedMarks += Convert.ToInt32(gradebookGrade.AllowedMarks);

                            }
                            assignmentViewModel.TenantId = assignment.TenantId;
                            assignmentViewModel.SchoolId = assignment.SchoolId;
                            assignmentViewModel.CourseSectionId = assignment.CourseSectionId;
                            assignmentViewModel.AssignmentId = assignment.AssignmentId;
                            assignmentViewModel.AssignmentTypeId = assignment.AssignmentTypeId;
                            assignmentViewModel.Points = assignment.Points;
                            assignmentViewModel.AssignmentTitle = assignment.AssignmentTitle;
                            assignmentViewModel.Points = assignment.Points;
                            totalPoint += assignment.Points;
                            assignmentTypeListViewModel.assignmentViewModelList.Add(assignmentViewModel);

                        }
                        var Percentage = Math.Round((Convert.ToDecimal(allowedMarks) / Convert.ToDecimal(totalPoint) * 100), 2);
                        if (Percentage > 0.0m)
                        {
                            if (courseSectionData?.GradeScale?.Grade!=null)
                            {
                                if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                                {
                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(Percentage));
                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title;
                                }
                                else
                                {
                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= Percentage)?.Title;
                                }
                            }
                        }
                        assignmentTypeListViewModel.AssignmentTypeId = assignmentType.AssignmentTypeId;
                        assignmentTypeListViewModel.CourseSectionId = assignmentType.CourseSectionId;
                        assignmentTypeListViewModel.Title = assignmentType.Title;
                        assignmentTypeListViewModel.Weightage = assignmentType.Weightage;
                        assignmentTypeListViewModel.AssignmentTypePoint = totalPoint.ToString();
                        assignmentTypeListViewModel.AssignmentTypeMarks = allowedMarks.ToString() ;
                        assignmentTypeListViewModel.AssignmentTypeLetterGrade = assignmentGrade;
                        assignmentTypeListViewModel.AssignmentTypePercentage = Percentage;
                        assignmentForStudent.assignmentTypeViewModelList.Add(assignmentTypeListViewModel);
                    }
                }
                else
                {
                    assignmentForStudent._failure = true;
                    assignmentForStudent._message = NORECORDFOUND;
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
        /// Add Gradebook GradeBy Student
        /// </summary>
        /// <param name="assignmentForStudentViewModel"></param>
        /// <returns></returns>
        public AssignmentForStudentViewModel AddGradebookGradeByStudent(AssignmentForStudentViewModel assignmentForStudentViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                AssignmentForStudentViewModel assignmentForStudent = new AssignmentForStudentViewModel();
                try
                {
                    assignmentForStudentViewModel.AcademicYear = (decimal)Utility.GetCurrentAcademicYear(this.context!, assignmentForStudentViewModel.TenantId, assignmentForStudentViewModel.SchoolId)!;

                    var courseSectionData = this.context?.CourseSection.Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear);

                    var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear && x.BreakoffPoints > 0).ToList();

                    var AssignmentTypeData = this.context?.AssignmentType.Where(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId && x.AcademicYear == assignmentForStudentViewModel.AcademicYear);

                    var totalWeitage = AssignmentTypeData?.ToList().Sum(x => x.Weightage);

                    var GradebookGradeData = this.context?.GradebookGrades.Where(x => x.TenantId == assignmentForStudentViewModel.TenantId && x.SchoolId == assignmentForStudentViewModel.SchoolId && x.CourseSectionId == assignmentForStudentViewModel.CourseSectionId /*&& x.MarkingPeriodId == assignmentForStudentViewModel.MarkingPeriodId*/ && x.AcademicYear == assignmentForStudentViewModel.AcademicYear && x.StudentId == assignmentForStudentViewModel.StudentId).ToList();

                    if (GradebookGradeData!=null&& GradebookGradeData.Any())
                    {
                        this.context?.GradebookGrades.RemoveRange(GradebookGradeData);
                        this.context?.SaveChanges();
                    }

                    int? YrMarkingPeriodId = null;
                    int? SmstrMarkingPeriodId = null;
                    int? QtrMarkingPeriodId = null;
                    int? PrgrsprdMarkingPeriodId = null;

                    if (assignmentForStudentViewModel.MarkingPeriodId != null)
                    {
                        string[] markingPeriodID = assignmentForStudentViewModel.MarkingPeriodId.Split("_");

                        if (markingPeriodID.First() == "0")
                        {
                            YrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "1")
                        {
                            SmstrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "2")
                        {
                            QtrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }

                        if (markingPeriodID.First() == "3")
                        {
                            PrgrsprdMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                        }
                    }
                    decimal? runingAvgSum = 0.0m;
                    decimal? sumOfAssignmentTypeAvg = 0.0m;
                    var GradebookGradeList = new List<GradebookGrades>();
                    foreach (var assignmentType in assignmentForStudentViewModel.assignmentTypeViewModelList)
                    {
                        int count = 0;
                        decimal? assignmentTypeAvg = 0.0m;
                        AssignmentTypeViewModel assignmentTypeListViewModel = new AssignmentTypeViewModel();
                        foreach (var assignment in assignmentType.assignmentViewModelList)
                        {
                            if (!string.IsNullOrEmpty(assignment.AllowedMarks))
                            {
                                count++;
                                decimal? weightPersentage = 0.0m;
                                decimal? assignmentPercentage = 0.0m;
                                string? assignmentGrade = null;

                                if (assignment.AllowedMarks != "*")
                                {
                                    weightPersentage = Math.Round((Convert.ToDecimal(assignmentType.Weightage) / Convert.ToDecimal(totalWeitage) * 100), 2);

                                    assignmentPercentage = Math.Round(100 * (Convert.ToDecimal(assignment.AllowedMarks) / Convert.ToDecimal(assignment.Points)), 2);

                                    var runingAvg = Math.Round((decimal)(assignmentPercentage * weightPersentage / 100), 2);

                                    assignmentTypeAvg = assignmentTypeAvg + runingAvg;

                                    if (assignmentPercentage > 0.0m)
                                    {
                                        if (courseSectionData?.GradeScale?.Grade != null)
                                        {
                                            if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                                            {
                                                var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(assignmentPercentage));
                                                assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title;
                                            }
                                            else
                                            {
                                                assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= assignmentPercentage)?.Title;
                                            }
                                        }
                                    }
                                }
                                var gradebookGrade = new GradebookGrades
                                {
                                    TenantId = assignmentForStudentViewModel.TenantId,
                                    SchoolId = assignmentForStudentViewModel.SchoolId,
                                    StudentId = assignmentForStudentViewModel.StudentId,
                                    AcademicYear = assignmentForStudentViewModel.AcademicYear,
                                    //MarkingPeriodId = assignmentForStudentViewModel.MarkingPeriodId,
                                    CourseSectionId = assignmentForStudentViewModel.CourseSectionId,
                                    AssignmentTypeId = assignmentType.AssignmentTypeId,
                                    AssignmentId = assignment.AssignmentId,
                                    AllowedMarks = assignment.AllowedMarks,
                                    Percentage = assignmentPercentage.ToString(),
                                    LetterGrade = assignmentGrade,
                                    Comment = assignment.Comment,
                                    CreatedBy = assignmentForStudentViewModel.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    UpdatedBy = assignmentForStudentViewModel.CreatedBy,
                                    UpdatedOn = DateTime.UtcNow,
                                    YrMarkingPeriodId = YrMarkingPeriodId,
                                    SmstrMarkingPeriodId=SmstrMarkingPeriodId,
                                    QtrMarkingPeriodId=QtrMarkingPeriodId,
                                    PrgrsprdMarkingPeriodId=PrgrsprdMarkingPeriodId
                                };

                                GradebookGradeList.Add(gradebookGrade);
                            }
                        }
                        if (assignmentTypeAvg > 0.0m && count > 0)
                        {
                            sumOfAssignmentTypeAvg = Math.Round((decimal)assignmentTypeAvg / Convert.ToDecimal(count), 2);
                            runingAvgSum = runingAvgSum + sumOfAssignmentTypeAvg;
                        }
                    }
                    string? runningGrade = null;
                    if (courseSectionData?.GradeScale?.Grade != null)
                    {
                        if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                        {
                            var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(runingAvgSum));
                            runningGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title;
                        }
                        runningGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= runingAvgSum)?.Title;
                    }
                    GradebookGradeList.ForEach(x => { x.RunningAvg = (Math.Round((decimal)runingAvgSum, 2).ToString()); x.RunningAvgGrade = runningGrade; });
                    this.context?.GradebookGrades.AddRange(GradebookGradeList);
                    this.context?.SaveChanges();
                    transaction?.Commit();
                    assignmentForStudentViewModel._message = "Data Submitted Successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    assignmentForStudentViewModel._failure = true;
                    assignmentForStudentViewModel._message = es.Message;
                }
            }
            return assignmentForStudentViewModel;
        }

        /// <summary>
        /// Gradebook GradeBy Assignment Type
        /// </summary>
        /// <param name="studentListByAssignmentTpyeViewModel"></param>
        /// <returns></returns>
        public StudentListByAssignmentTpyeViewModel GradebookGradeByAssignmentType(StudentListByAssignmentTpyeViewModel studentListByAssignmentTpyeViewModel)
        {
            StudentListByAssignmentTpyeViewModel studentListByAssignmentTpye = new StudentListByAssignmentTpyeViewModel();
            try
            {
                studentListByAssignmentTpye.TenantId = studentListByAssignmentTpyeViewModel.TenantId;
                studentListByAssignmentTpye.SchoolId = studentListByAssignmentTpyeViewModel.SchoolId;
                studentListByAssignmentTpye.AssignmentTpyeId = studentListByAssignmentTpyeViewModel.AssignmentTpyeId;
                studentListByAssignmentTpye.CourseSectionId = studentListByAssignmentTpyeViewModel.CourseSectionId;
                studentListByAssignmentTpye.AcademicYear = studentListByAssignmentTpyeViewModel.AcademicYear;                
                studentListByAssignmentTpye._tenantName = studentListByAssignmentTpyeViewModel._tenantName;
                studentListByAssignmentTpye._token = studentListByAssignmentTpyeViewModel._token;

                var courseSectionData = this.context?.CourseSection.Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear);

                var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear).ToList();

                var AssignmentTypeData = this.context?.AssignmentType.Include(x => x.Assignment).ThenInclude(x => x.GradebookGrades).FirstOrDefault(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AssignmentTypeId == studentListByAssignmentTpyeViewModel.AssignmentTpyeId);

                var studentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Include(x => x.StudentMaster).ThenInclude(x => x.GradebookGrades).Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear&& (studentListByAssignmentTpyeViewModel.IncludeInactive == false || studentListByAssignmentTpyeViewModel.IncludeInactive == null ? x.IsDropped != true : true)).ToList();
                if (studentCoursesectionScheduleData!=null && studentCoursesectionScheduleData.Any())
                {
                    if (AssignmentTypeData != null)
                    {
                        //string assignmentGrade = null;
                        //int? allowedMarks = 0;
                        int? totalPoint = 0;
                        AssignmentTypeViewModel assignmentTypeListViewModel = new AssignmentTypeViewModel();

                        foreach (var Assignment in AssignmentTypeData.Assignment)
                        {
                            var assignmentsListView = new AssignmentsListViewModel();
                            totalPoint = totalPoint + Assignment.Points;
                            if (studentCoursesectionScheduleData.Count > 0)
                            {
                                foreach (var studentCoursesectionSchedule in studentCoursesectionScheduleData)
                                {
                                    StudentsListViewModel studentsListViewModel = new StudentsListViewModel();

                                    var GradebookGradeData = studentCoursesectionSchedule.StudentMaster.GradebookGrades.FirstOrDefault(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == AssignmentTypeData.AcademicYear && x.AssignmentTypeId == Assignment.AssignmentTypeId && x.AssignmentId == Assignment.AssignmentId && x.StudentId == studentCoursesectionSchedule.StudentId);

                                    if (GradebookGradeData != null)
                                    {
                                        studentsListViewModel.AllowedMarks = GradebookGradeData.AllowedMarks;
                                        studentsListViewModel.Comment = GradebookGradeData.Comment;
                                        studentsListViewModel.Percentage = GradebookGradeData.Percentage;
                                        studentsListViewModel.LetterGrade = GradebookGradeData.LetterGrade;
                                    }

                                    studentsListViewModel.StudentInternalId = studentCoursesectionSchedule.StudentInternalId;
                                    studentsListViewModel.StudentPhoto = studentCoursesectionSchedule.StudentMaster.StudentPhoto;
                                    studentsListViewModel.FirstGivenName = studentCoursesectionSchedule.StudentMaster.FirstGivenName;
                                    studentsListViewModel.MiddleName = studentCoursesectionSchedule.StudentMaster.MiddleName;
                                    studentsListViewModel.LastFamilyName = studentCoursesectionSchedule.StudentMaster.LastFamilyName;
                                    studentsListViewModel.RunningAvg = GradebookGradeData != null ? GradebookGradeData.RunningAvg : null;
                                    studentsListViewModel.RunningAvgGrade = GradebookGradeData != null ? GradebookGradeData.RunningAvgGrade : null;
                                    studentsListViewModel.StudentId = studentCoursesectionSchedule.StudentId;
                                    studentsListViewModel.Points = Assignment.Points;
                                    assignmentsListView.studentsListViewModels.Add(studentsListViewModel);
                                }
                            }
                            assignmentsListView.AssignmentTypeId = AssignmentTypeData.AssignmentTypeId;
                            assignmentsListView.AssignmentId = Assignment.AssignmentId;
                            assignmentsListView.Title = AssignmentTypeData.Title;
                            assignmentsListView.Weightage = AssignmentTypeData.Weightage;
                            assignmentsListView.AssignmentTitle = Assignment.AssignmentTitle;
                            assignmentsListView.AssignmentDate = Assignment.AssignmentDate;
                            assignmentsListView.DueDate = Assignment.DueDate;
                            studentListByAssignmentTpye.assignmentsListViewModels.Add(assignmentsListView);
                        }

                        if (!string.IsNullOrEmpty(studentListByAssignmentTpyeViewModel.SearchValue))
                        {
                            var searchValue = Regex.Replace(studentListByAssignmentTpyeViewModel.SearchValue, @"\s+", "");
                            //var studentData = studentListByAssignmentTpye.assignmentsListViewModels.FirstOrDefault().studentsListViewModels;
                            var studentData = studentListByAssignmentTpye.assignmentsListViewModels.First().studentsListViewModels;

                            var SearchData = studentData.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(searchValue.ToLower()) || x.MiddleName != null && x.MiddleName.ToLower().Contains(searchValue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(searchValue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(searchValue.ToLower())).ToList();

                            if (SearchData.Any())
                            {
                                var searchStudentIds = SearchData.Select(s => s.StudentId).ToList();
                                foreach (var assignmentsListViewModel in studentListByAssignmentTpye.assignmentsListViewModels)
                                {
                                    assignmentsListViewModel.studentsListViewModels = assignmentsListViewModel.studentsListViewModels.Where(x => searchStudentIds.Contains(x.StudentId)).ToList();
                                }
                            }
                            else
                            {
                                studentListByAssignmentTpye._failure = true;
                                studentListByAssignmentTpye._message = NORECORDFOUND;
                                studentListByAssignmentTpye.assignmentsListViewModels = new List<AssignmentsListViewModel>();
                                return studentListByAssignmentTpye;
                            }
                        }

                        studentListByAssignmentTpye.assignmentsListViewModels = studentListByAssignmentTpye.assignmentsListViewModels.OrderBy(x => x.DueDate).ToList();

                        int count = 0;
                        foreach (var assignmentsListViewModel in studentListByAssignmentTpye.assignmentsListViewModels)
                        {
                            if (count == 0)
                            {
                                foreach (var student in assignmentsListViewModel.studentsListViewModels)
                                {
                                    var GradebookGradeData = this.context?.GradebookGrades.Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == AssignmentTypeData.AcademicYear && x.AssignmentTypeId == studentListByAssignmentTpyeViewModel.AssignmentTpyeId && x.StudentId == student.StudentId).ToList();

                                    if (GradebookGradeData!=null && GradebookGradeData.Any())
                                    {
                                        int sum = 0;
                                        foreach (var gg in GradebookGradeData)
                                        {
                                            if (gg.AllowedMarks != "*" && !string.IsNullOrEmpty(gg.AllowedMarks))
                                            {
                                                sum = sum + Convert.ToInt32(gg.AllowedMarks);
                                                student.TotalObtainOfAssignmentType = sum;
                                            }
                                        }
                                    }
                                    student.TotalPointOfAssignmentType = totalPoint;

                                    student.PercentageOfAssignmentType = (int?)Math.Round(100 * (Convert.ToDecimal(student.TotalObtainOfAssignmentType) / Convert.ToDecimal(student.TotalPointOfAssignmentType)), 2);
                                    if (GradebookConfigurationGrade!=null && GradebookConfigurationGrade.Any())
                                    {
                                        var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= student.PercentageOfAssignmentType);
                                        student.LetterGradeOfAssignmentType = courseSectionData?.GradeScale?.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title;
                                    }
                                    student.LetterGradeOfAssignmentType = courseSectionData?.GradeScale?.Grade.FirstOrDefault(x => x.Breakoff <= student.PercentageOfAssignmentType)?.Title;

                                }
                                count++;
                                continue;
                            }
                            assignmentsListViewModel.studentsListViewModels.ForEach(x => x.StudentPhoto = null);
                        }
                    }
                }
                else
                {
                    studentListByAssignmentTpye._failure = true;
                    studentListByAssignmentTpye._message = NORECORDFOUND;
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
        /// Add gradebook GradeBy AssignmentType
        /// </summary>
        /// <param name="studentListByAssignmentTpyeViewModel"></param>
        /// <returns></returns>
        public StudentListByAssignmentTpyeViewModel AddgradebookGradeByAssignmentType(StudentListByAssignmentTpyeViewModel studentListByAssignmentTpyeViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    if (studentListByAssignmentTpyeViewModel.assignmentsListViewModels.Count > 0)
                    {
                        studentListByAssignmentTpyeViewModel.AcademicYear = (decimal)Utility.GetCurrentAcademicYear(this.context!, studentListByAssignmentTpyeViewModel.TenantId, studentListByAssignmentTpyeViewModel.SchoolId)!;

                        var courseSectionData = this.context?.CourseSection.Include(x => x.GradeScale).ThenInclude(x => x!.Grade).FirstOrDefault(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear);

                        var AssignmentTypeData = this.context?.AssignmentType.Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear);

                        int? YrMarkingPeriodId = null;
                        int? SmstrMarkingPeriodId = null;
                        int? QtrMarkingPeriodId = null;
                        int? PrgrsprdMarkingPeriodId = null;

                        if (studentListByAssignmentTpyeViewModel.MarkingPeriodId != null)
                        {
                            string[] markingPeriodID = studentListByAssignmentTpyeViewModel.MarkingPeriodId.Split("_");

                            if (markingPeriodID.First() == "0")
                            {
                                YrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }

                            if (markingPeriodID.First() == "1")
                            {
                                SmstrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }

                            if (markingPeriodID.First() == "2")
                            {
                                QtrMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }

                            if (markingPeriodID.First() == "3")
                            {
                                PrgrsprdMarkingPeriodId = Convert.ToInt32(markingPeriodID[1]);
                            }
                        }

                        var totalWeitage = AssignmentTypeData?.ToList().Sum(x => x.Weightage);

                        //var studentIds = studentListByAssignmentTpyeViewModel.assignmentsListViewModels.FirstOrDefault().studentsListViewModels.Select(x => x.StudentId).ToList();
                        var studentIds = studentListByAssignmentTpyeViewModel.assignmentsListViewModels.FirstOrDefault()!.studentsListViewModels.Select(x => x.StudentId).ToList();

                        foreach (var studentId in studentIds)
                        {
                            List<GradebookGrades> GradebookGradeList = new List<GradebookGrades>();

                            var ExixtingGradebookGradeData = this.context?.GradebookGrades.Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId /*&& x.MarkingPeriodId == studentListByAssignmentTpyeViewModel.MarkingPeriodId*/ && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear && x.StudentId == studentId && x.AssignmentTypeId == studentListByAssignmentTpyeViewModel.AssignmentTpyeId).ToList();

                            if (ExixtingGradebookGradeData!=null && ExixtingGradebookGradeData.Any())
                            {
                                var Marks = ExixtingGradebookGradeData.Sum(x => Convert.ToInt32(x.AllowedMarks));

                                this.context?.GradebookGrades.RemoveRange(ExixtingGradebookGradeData);
                                this.context?.SaveChanges();
                            }
                            //decimal? runingAvgSum = 0.0m;
                            decimal? assignmentTypeAvg = 0.0m;
                            foreach (var assignment in studentListByAssignmentTpyeViewModel.assignmentsListViewModels)
                            {
                                var student = assignment.studentsListViewModels.FirstOrDefault(s => s.StudentId == studentId);

                                if (student != null)
                                {
                                    if (!string.IsNullOrEmpty(student.AllowedMarks))
                                    {
                                        decimal? assignmentPercentage = 0.0m;
                                        string? assignmentGrade = null;
                                        if (student.AllowedMarks != "*")
                                        {
                                            assignmentPercentage = Math.Round(100 * (Convert.ToDecimal(student.AllowedMarks) / Convert.ToDecimal(student.Points)), 2);

                                            var weightPersentage = Math.Round((Convert.ToDecimal(assignment.Weightage) / Convert.ToDecimal(totalWeitage) * 100), 2);

                                            var runingAvg = Math.Round((decimal)(assignmentPercentage * weightPersentage / 100), 2);

                                            assignmentTypeAvg = assignmentTypeAvg + runingAvg;

                                            var GradebookConfigurationGrade = this.context?.GradebookConfigurationGradescale.Where(x => x.TenantId == studentListByAssignmentTpyeViewModel.TenantId && x.SchoolId == studentListByAssignmentTpyeViewModel.SchoolId && x.CourseSectionId == studentListByAssignmentTpyeViewModel.CourseSectionId && x.AcademicYear == studentListByAssignmentTpyeViewModel.AcademicYear && x.BreakoffPoints > 0).ToList();
                                            if (courseSectionData?.GradeScale?.Grade != null)
                                            {
                                                if (GradebookConfigurationGrade != null && GradebookConfigurationGrade.Any())
                                                {
                                                    var ConfigurationGrade = GradebookConfigurationGrade.FirstOrDefault(x => x.BreakoffPoints <= Convert.ToInt32(assignmentPercentage));
                                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.GradeId == ConfigurationGrade?.GradeId && x.GradeScaleId == ConfigurationGrade.GradeScaleId)?.Title;
                                                }
                                                else
                                                {
                                                    assignmentGrade = courseSectionData.GradeScale.Grade.FirstOrDefault(x => x.Breakoff <= assignmentPercentage)?.Title;
                                                }
                                            }
                                        }
                                        var gradebookGrade = new GradebookGrades
                                        {
                                            TenantId = studentListByAssignmentTpyeViewModel.TenantId,
                                            SchoolId = studentListByAssignmentTpyeViewModel.SchoolId,
                                            StudentId = student.StudentId,
                                            AcademicYear = studentListByAssignmentTpyeViewModel.AcademicYear,
                                            // MarkingPeriodId = studentListByAssignmentTpyeViewModel.MarkingPeriodId,
                                            CourseSectionId = studentListByAssignmentTpyeViewModel.CourseSectionId,
                                            AssignmentTypeId = assignment.AssignmentTypeId,
                                            AssignmentId = assignment.AssignmentId,
                                            AllowedMarks = student.AllowedMarks,
                                            Percentage = assignmentPercentage.ToString(),
                                            LetterGrade = assignmentGrade,
                                            Comment = student.Comment,
                                            CreatedBy = studentListByAssignmentTpyeViewModel.CreatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                            UpdatedBy = studentListByAssignmentTpyeViewModel.CreatedBy,
                                            UpdatedOn = DateTime.UtcNow,
                                            YrMarkingPeriodId = YrMarkingPeriodId,
                                            SmstrMarkingPeriodId = SmstrMarkingPeriodId,
                                            QtrMarkingPeriodId = QtrMarkingPeriodId,
                                            PrgrsprdMarkingPeriodId = PrgrsprdMarkingPeriodId

                                        };
                                        GradebookGradeList.Add(gradebookGrade);
                                    }
                                }
                            }
                            GradebookGradeList.ForEach(x => x.RunningAvg = assignmentTypeAvg.ToString());
                            this.context?.GradebookGrades.AddRange(GradebookGradeList);
                            this.context?.SaveChanges();
                        }
                        transaction?.Commit();
                        studentListByAssignmentTpyeViewModel._message = "Data Submitted Successfully";
                    }
                    else
                    {
                        studentListByAssignmentTpyeViewModel._failure = true;
                        studentListByAssignmentTpyeViewModel._message = "No Assignment Found";
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    studentListByAssignmentTpyeViewModel._failure = true;
                    studentListByAssignmentTpyeViewModel._message = es.Message;
                }
            }
            return studentListByAssignmentTpyeViewModel;
        }
    }
}
