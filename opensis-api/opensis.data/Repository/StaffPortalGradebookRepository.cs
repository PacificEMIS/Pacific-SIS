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
using opensis.data.ViewModels.StaffPortalGradebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
   public class StaffPortalGradebookRepository: IStaffPortalGradebookRepository
    {
        private CRMContext context;
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
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var GradebookConfigurationData = this.context?.GradebookConfiguration.Include(x => x.GradebookConfigurationGradescale).Include(x => x.GradebookConfigurationYear).Include(x => x.GradebookConfigurationSemester).Include(x => x.GradebookConfigurationQuarter).FirstOrDefault(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId && x.SchoolId == gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId && x.CourseId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseId && x.CourseSectionId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseSectionId && x.AcademicYear == gradebookConfigurationAddViewModel.gradebookConfiguration.AcademicYear);

                    if (GradebookConfigurationData != null)
                    {
                        if (GradebookConfigurationData.GradebookConfigurationGradescale.Count > 0)
                        {
                            this.context.GradebookConfigurationGradescale.RemoveRange(GradebookConfigurationData.GradebookConfigurationGradescale);
                        }
                        if (GradebookConfigurationData.GradebookConfigurationYear.Count > 0)
                        {
                            this.context.GradebookConfigurationYear.RemoveRange(GradebookConfigurationData.GradebookConfigurationYear);
                        }
                        if (GradebookConfigurationData.GradebookConfigurationSemester.Count > 0)
                        {
                            this.context.GradebookConfigurationSemester.RemoveRange(GradebookConfigurationData.GradebookConfigurationSemester);
                        }
                        if (GradebookConfigurationData.GradebookConfigurationQuarter.Count > 0)
                        {
                            this.context.GradebookConfigurationQuarter.RemoveRange(GradebookConfigurationData.GradebookConfigurationQuarter);
                        }
                        this.context.SaveChanges();

                        gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedBy = GradebookConfigurationData.CreatedBy;
                        gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedOn = GradebookConfigurationData.CreatedOn;
                        gradebookConfigurationAddViewModel.gradebookConfiguration.UpdatedOn = DateTime.UtcNow;
                        this.context.Entry(GradebookConfigurationData).CurrentValues.SetValues(gradebookConfigurationAddViewModel.gradebookConfiguration);


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

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter.ToList().ForEach(x => { x.Id = (int)gcqId++; x.GradebookConfigurationId = GradebookConfigurationData.GradebookConfigurationId; x.CreatedBy = GradebookConfigurationData.CreatedBy; x.CreatedOn = GradebookConfigurationData.CreatedOn; x.UpdatedOn = DateTime.UtcNow; });
                        this.context?.GradebookConfigurationQuarter.AddRange(gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter);

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

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationGradescale.ToList().ForEach(x => { x.Id = (int)gcgsId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationYear.ToList().ForEach(x => { x.Id = (int)gcyId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationSemester.ToList().ForEach(x => { x.Id = (int)gcsId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationQuarter.ToList().ForEach(x => { x.Id = (int)gcqId++; x.CreatedOn = DateTime.UtcNow; x.GradebookConfigurationId = (int)gcId; });

                        gradebookConfigurationAddViewModel.gradebookConfiguration.GradebookConfigurationId = (int)gcId;
                        gradebookConfigurationAddViewModel.gradebookConfiguration.CreatedOn = DateTime.UtcNow;
                        this.context?.GradebookConfiguration.Add(gradebookConfigurationAddViewModel.gradebookConfiguration);
                        this.context?.SaveChanges();
                        gradebookConfigurationAddViewModel._message = "Gradebook Configuration Added Successfully";

                    }
                    transaction.Commit();
                    gradebookConfigurationAddViewModel._failure = false;
                }
                catch (Exception es)
                {
                    transaction.Rollback();
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
                var GradebookConfigurationData = this.context?.GradebookConfiguration.Include(x => x.GradebookConfigurationGradescale).Include(x => x.GradebookConfigurationYear).Include(x => x.GradebookConfigurationSemester).Include(x => x.GradebookConfigurationQuarter).FirstOrDefault(x => x.TenantId == gradebookConfigurationAddViewModel.gradebookConfiguration.TenantId && x.SchoolId == gradebookConfigurationAddViewModel.gradebookConfiguration.SchoolId && x.CourseId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseId && x.CourseSectionId == gradebookConfigurationAddViewModel.gradebookConfiguration.CourseSectionId && x.AcademicYear == gradebookConfigurationAddViewModel.gradebookConfiguration.AcademicYear);
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
                var markingPeriodDataList = this.context?.SchoolYears.Include(x => x.Semesters).ThenInclude(p => p.Quarters)/*.ThenInclude(a => a.ProgressPeriods)*/.Where(x => x.SchoolId == finalGradingMarkingPeriodList.SchoolId && x.TenantId == finalGradingMarkingPeriodList.TenantId && x.AcademicYear == finalGradingMarkingPeriodList.AcademicYear).FirstOrDefault();

                if (markingPeriodDataList != null)
                {
                    if (markingPeriodDataList.Semesters.Count > 0 || markingPeriodDataList.Semesters != null)
                    {
                        finalGradingMarkingPeriod.semesters.AddRange(markingPeriodDataList.Semesters);

                        foreach (var markingperiodData in markingPeriodDataList.Semesters)
                        {
                            if (markingperiodData.Quarters.Count > 0 || markingperiodData.Quarters != null)
                            {
                                finalGradingMarkingPeriod.quarters.AddRange(markingperiodData.Quarters.ToList());
                            }
                        }
                    }

                    finalGradingMarkingPeriod.schoolYears = markingPeriodDataList;
                    //markingPeriodDataList.Semesters.ToList().ForEach(b => b.Quarters = null);

                    if (finalGradingMarkingPeriod.quarters.Count > 0)
                    {
                        finalGradingMarkingPeriod.quarters.ForEach(e => { e.Semesters = null; e.CourseSection = null; e.ProgressPeriods = null; e.StaffCoursesectionSchedule = null; e.StudentEffortGradeMaster = null; e.StudentFinalGrade = null; });
                    }

                    if (finalGradingMarkingPeriod.semesters.Count > 0)
                    {
                        finalGradingMarkingPeriod.semesters.ForEach(e => { e.SchoolYears = null; e.CourseSection = null; e.StaffCoursesectionSchedule = null; e.StudentEffortGradeMaster = null; e.StudentFinalGrade = null; });
                    }

                    if (finalGradingMarkingPeriod.schoolYears != null)
                    {
                        finalGradingMarkingPeriod.schoolYears.AssignmentType = null;
                        finalGradingMarkingPeriod.schoolYears.CourseSection = null;
                        finalGradingMarkingPeriod.schoolYears.HonorRolls = null;
                        finalGradingMarkingPeriod.schoolYears.StaffCoursesectionSchedule = null;
                        finalGradingMarkingPeriod.schoolYears.StudentEffortGradeMaster = null;
                        finalGradingMarkingPeriod.schoolYears.StudentFinalGrade = null;
                        //finalGradingMarkingPeriod.schoolYears.Semesters.ToList().ForEach(v => v.Quarters = null);
                    }
                }
            }
            catch (Exception es)
            {
                finalGradingMarkingPeriod._failure = true;
                finalGradingMarkingPeriod._message = es.Message;
            }
            return finalGradingMarkingPeriod;
        }
    }
}
