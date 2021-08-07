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

using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.MarkingPeriods;
using opensis.data.ViewModels.Quarter;
using opensis.data.ViewModels.SchoolYear;
using opensis.data.ViewModels.ProgressPeriod;
using opensis.data.ViewModels.Semester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace opensis.data.Repository
{
    public class MarkingPeriodRepository : IMarkingperiodRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public MarkingPeriodRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get Marking Period
        /// </summary>
        /// <param name="markingPeriod"></param>
        /// <returns></returns>
        public MarkingPeriod GetMarkingPeriod(MarkingPeriod markingPeriod)
        {
            MarkingPeriod markingPeriodModel = new MarkingPeriod();
            try
            {
                var MarkingperiodData = this.context?.SchoolYears.Include(x => x.Semesters).ThenInclude(p => p.Quarters).ThenInclude(a => a.ProgressPeriods).Where(x => x.SchoolId == markingPeriod.SchoolId && x.TenantId == markingPeriod.TenantId && x.AcademicYear == markingPeriod.AcademicYear).OrderBy(x=>x.StartDate).ToList();            
                if (MarkingperiodData.Count > 0)
                {     
                    foreach (var year in MarkingperiodData)
                    {
                        SchoolYearView schoolYearView = new SchoolYearView();
                        schoolYearView.TenantId = year.TenantId;
                        schoolYearView.SchoolId = year.SchoolId;
                        schoolYearView.MarkingPeriodId = year.MarkingPeriodId;
                        schoolYearView.Title = year.Title;
                        schoolYearView.ShortName = year.ShortName;
                        schoolYearView.DoesComments = year.DoesComments;
                        schoolYearView.DoesExam = year.DoesExam;
                        schoolYearView.DoesGrades = year.DoesGrades;
                        schoolYearView.StartDate = year.StartDate;
                        schoolYearView.EndDate = year.EndDate;
                        schoolYearView.PostEndDate = year.PostEndDate;
                        schoolYearView.PostStartDate = year.PostStartDate;
                        schoolYearView.IsParent = true;
                        schoolYearView.UpdatedOn = year.UpdatedOn;
                        schoolYearView.CreatedOn = year.CreatedOn;
                        schoolYearView.CreatedBy = (year.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == year.CreatedBy).Name : null;
                        schoolYearView.UpdatedBy = (year.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == year.UpdatedBy).Name : null;
                        var semesterList = year.Semesters.OrderBy(x=>x.StartDate).ToList();
                        foreach (var semester in semesterList)
                        {                           
                            SchoolSemesterView schoolSemesterView = new SchoolSemesterView();
                            schoolSemesterView.TenantId = semester.TenantId;
                            schoolSemesterView.SchoolId = semester.SchoolId;
                            schoolSemesterView.MarkingPeriodId = semester.MarkingPeriodId;
                            schoolSemesterView.Title = semester.Title;
                            schoolSemesterView.YearId =(int) semester.YearId;
                            schoolSemesterView.ShortName = semester.ShortName;
                            schoolSemesterView.DoesComments = semester.DoesComments;
                            schoolSemesterView.DoesExam = semester.DoesExam;
                            schoolSemesterView.DoesGrades = semester.DoesGrades;
                            schoolSemesterView.StartDate = semester.StartDate;
                            schoolSemesterView.EndDate = semester.EndDate;
                            schoolSemesterView.PostEndDate = semester.PostEndDate;
                            schoolSemesterView.PostStartDate = semester.PostStartDate;
                            schoolSemesterView.IsParent = false;
                            schoolSemesterView.CreatedOn = semester.CreatedOn;
                            schoolSemesterView.UpdatedOn = semester.UpdatedOn;
                            schoolSemesterView.UpdatedBy= (semester.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>  u.TenantId == markingPeriod.TenantId && u.EmailAddress == semester.UpdatedBy).Name : null;
                            schoolSemesterView.CreatedBy = (semester.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == semester.CreatedBy).Name : null;
                            schoolYearView.Children.Add(schoolSemesterView);
                            var quaterList = semester.Quarters.OrderBy(x=>x.StartDate).ToList();
                            foreach (var quarter in quaterList)
                            {                                
                                SchoolQuarterView schoolQuarterView = new SchoolQuarterView();
                                schoolQuarterView.TenantId = quarter.TenantId;
                                schoolQuarterView.SchoolId = quarter.SchoolId;
                                schoolQuarterView.MarkingPeriodId = quarter.MarkingPeriodId;
                                schoolQuarterView.Title = quarter.Title;
                                schoolQuarterView.SemesterId =(int) quarter.SemesterId;
                                schoolQuarterView.ShortName = quarter.ShortName;
                                schoolQuarterView.DoesComments = quarter.DoesComments;
                                schoolQuarterView.DoesExam = quarter.DoesExam;
                                schoolQuarterView.DoesGrades = quarter.DoesGrades;
                                schoolQuarterView.StartDate = quarter.StartDate;
                                schoolQuarterView.EndDate = quarter.EndDate;
                                schoolQuarterView.PostEndDate = quarter.PostEndDate;
                                schoolQuarterView.PostStartDate = quarter.PostStartDate;
                                schoolQuarterView.IsParent = false;
                                schoolQuarterView.UpdatedOn = quarter.UpdatedOn;
                                schoolQuarterView.CreatedOn = quarter.CreatedOn;
                                schoolQuarterView.CreatedBy= (quarter.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == quarter.CreatedBy).Name : null;
                                schoolQuarterView.UpdatedBy = (quarter.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == quarter.UpdatedBy).Name : null;
                                schoolSemesterView.Children.Add(schoolQuarterView);
                                var ProgressPeriodList = quarter.ProgressPeriods.OrderBy(x=>x.StartDate).ToList();
                                foreach (var progressPeriod in ProgressPeriodList)
                                {                                    
                                    SchoolProgressPeriodView schoolProgressPeriodView = new SchoolProgressPeriodView();
                                    schoolProgressPeriodView.TenantId = progressPeriod.TenantId;
                                    schoolProgressPeriodView.SchoolId = progressPeriod.SchoolId;
                                    schoolProgressPeriodView.MarkingPeriodId = progressPeriod.MarkingPeriodId;
                                    schoolProgressPeriodView.Title = progressPeriod.Title;
                                    schoolProgressPeriodView.QuarterId =(int) progressPeriod.QuarterId;
                                    schoolProgressPeriodView.ShortName = progressPeriod.ShortName;
                                    schoolProgressPeriodView.DoesComments = progressPeriod.DoesComments;
                                    schoolProgressPeriodView.DoesExam = progressPeriod.DoesExam;
                                    schoolProgressPeriodView.DoesGrades = progressPeriod.DoesGrades;
                                    schoolProgressPeriodView.StartDate = progressPeriod.StartDate;
                                    schoolProgressPeriodView.EndDate = progressPeriod.EndDate;
                                    schoolProgressPeriodView.PostEndDate = progressPeriod.PostEndDate;
                                    schoolProgressPeriodView.PostStartDate = progressPeriod.PostStartDate;
                                    schoolProgressPeriodView.IsParent = false;
                                    schoolProgressPeriodView.UpdatedOn = progressPeriod.UpdatedOn;
                                    schoolProgressPeriodView.CreatedOn = progressPeriod.CreatedOn;
                                    schoolProgressPeriodView.CreatedBy= (progressPeriod.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == progressPeriod.CreatedBy).Name : null;
                                    schoolProgressPeriodView.UpdatedBy = (progressPeriod.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == markingPeriod.TenantId && u.EmailAddress == progressPeriod.UpdatedBy).Name : null;
                                    schoolQuarterView.Children.Add(schoolProgressPeriodView);
                                }                               
                            }
                        }
                        markingPeriodModel.schoolYearsView.Add(schoolYearView);
                    }                    
                }
                if (markingPeriodModel.schoolYearsView.Count > 0)
                {
                    markingPeriodModel._tenantName = markingPeriod._tenantName;
                    markingPeriodModel._token = markingPeriod._token;
                    markingPeriodModel._failure = false;
                    markingPeriodModel.TenantId = markingPeriod.TenantId;
                    markingPeriodModel.SchoolId = markingPeriod.SchoolId;
                }  
                else
                {
                    markingPeriodModel._tenantName = markingPeriod._tenantName;
                    markingPeriodModel._token = markingPeriod._token;
                    markingPeriodModel._failure = true;
                    markingPeriodModel.TenantId = markingPeriod.TenantId;
                    markingPeriodModel.SchoolId = markingPeriod.SchoolId;
                    markingPeriodModel._message = NORECORDFOUND;
                }              
            }
            catch (Exception es)
            {
                markingPeriodModel.schoolYearsView = null;
                markingPeriodModel._failure = true;
                markingPeriodModel._message = es.Message;
            }
            return markingPeriodModel;
        }

        /// <summary>
        /// Add Semester
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterAddViewModel AddSemester(SemesterAddViewModel semester)
        {
            try
            {
                var SchoolYear = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == semester.tableSemesters.TenantId && x.SchoolId == semester.tableSemesters.SchoolId && x.MarkingPeriodId == semester.tableSemesters.YearId);
                if (SchoolYear.StartDate <= semester.tableSemesters.StartDate && SchoolYear.EndDate >= semester.tableSemesters.EndDate)
                {

                    //int? MasterMarkingPeriodId = Utility.GetMaxPK(this.context, new Func<Semesters, int>(x => x.MarkingPeriodId));

                    int? MarkingPeriodId = 1;

                    var semesterData = this.context?.Semesters.Where(x => x.TenantId == semester.tableSemesters.TenantId && x.SchoolId == semester.tableSemesters.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                    if (semesterData != null)
                    {
                        MarkingPeriodId = semesterData.MarkingPeriodId + 1;
                    }

                    semester.tableSemesters.MarkingPeriodId = (int)MarkingPeriodId;
                    semester.tableSemesters.AcademicYear = SchoolYear.AcademicYear;
                    semester.tableSemesters.CreatedOn = DateTime.UtcNow;
                    this.context?.Semesters.Add(semester.tableSemesters);
                    this.context?.SaveChanges();
                    semester._failure = false;
                    semester._message = "Semester Added Successfully";
                    //semester.tableSemesters.SchoolYears = null;
                }
                else
                {
                    semester._failure = true;
                    semester._message = "Start Date And End Date of Semester Should Be Between Start Date And End Date of SchoolYear";
                }                
            }
            catch (Exception ex)
            {
                semester.tableSemesters = null;
                semester._failure = true;
                semester._message = NORECORDFOUND;                
            }
            return semester;
            
        }

        /// <summary>
        /// Update Semester
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterAddViewModel UpdateSemester(SemesterAddViewModel semester)
        {
            try
            {
                var semesterUpdate = this.context?.Semesters.FirstOrDefault(x => x.TenantId == semester.tableSemesters.TenantId && x.SchoolId == semester.tableSemesters.SchoolId && x.MarkingPeriodId == semester.tableSemesters.MarkingPeriodId);
                var SchoolYear = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == semesterUpdate.TenantId && x.SchoolId == semesterUpdate.SchoolId && x.MarkingPeriodId == semesterUpdate.YearId);

                if(SchoolYear.StartDate <= semester.tableSemesters.StartDate && SchoolYear.EndDate >= semester.tableSemesters.EndDate)
                {                    
                    semester.tableSemesters.UpdatedOn = DateTime.Now;
                    semester.tableSemesters.CreatedOn = semesterUpdate.CreatedOn;
                    semester.tableSemesters.CreatedBy = semesterUpdate.CreatedBy;
                    semester.tableSemesters.AcademicYear = SchoolYear.AcademicYear;
                    this.context.Entry(semesterUpdate).CurrentValues.SetValues(semester.tableSemesters);
                    this.context?.SaveChanges();
                    semester._failure = false;
                    semester._message = "Semester Updated Successfully";
                }
                else
                {
                    semester._failure = true;
                    semester._message = "Start Date And End Date of Semester Should Be Between Start Date And End Date of SchoolYear";
                }
                
            }
            catch (Exception ex)
            {
                semester.tableSemesters = null;
                semester._failure = true;
                semester._message = ex.Message;                
            }
            return semester;

        }

        /// <summary>
        /// Get Semester by Id
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterAddViewModel ViewSemester(SemesterAddViewModel semester)
        {
            SemesterAddViewModel semesterView = new SemesterAddViewModel();
            try
            {                
                var semesterById = this.context?.Semesters.FirstOrDefault(x => x.TenantId == semester.tableSemesters.TenantId && x.SchoolId == semester.tableSemesters.SchoolId && x.MarkingPeriodId == semester.tableSemesters.MarkingPeriodId);
                if (semesterById != null)
                {
                    semesterView.tableSemesters = semesterById;                    
                }
                else
                {
                    semesterView._failure = true;
                    semesterView._message = NORECORDFOUND;
                    
                }
            }
            catch (Exception es)
            {

                semester._failure = true;
                semester._message = es.Message;
            }
            return semesterView;
        }
        
        /// <summary>
        /// Delete Semester
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public SemesterAddViewModel DeleteSemester(SemesterAddViewModel semester)
        {
            try
            {
                var semesterDelete = this.context?.Semesters.FirstOrDefault(x => x.TenantId == semester.tableSemesters.TenantId && x.SchoolId == semester.tableSemesters.SchoolId && x.MarkingPeriodId == semester.tableSemesters.MarkingPeriodId);
               
                var QuatersExist = this.context?.Quarters.FirstOrDefault(x => x.TenantId == semesterDelete.TenantId && x.SchoolId == semesterDelete.SchoolId && x.SemesterId == semesterDelete.MarkingPeriodId);
                if (QuatersExist != null)
                {
                    semester.tableSemesters = null;
                    semester._message = "Semester cannot be deleted because it has its association";
                    semester._failure = true;
                }
                else
                {
                    this.context?.Semesters.Remove(semesterDelete);
                    this.context?.SaveChanges();
                    semester._failure = false;
                    semester._message = "Semester Deleted Successfully";
                }


            }
            catch (Exception es)
            {
                semester._failure = true;
                semester._message = es.Message;
            }
            return semester;
        }

        /// <summary>
        /// Add Progress Period
        /// </summary>
        /// <param name="progressPeriod"></param>
        /// <returns></returns>
        public ProgressPeriodAddViewModel AddProgressPeriod(ProgressPeriodAddViewModel progressPeriod)
        {
            var Quarter = this.context?.Quarters.FirstOrDefault(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId && x.MarkingPeriodId == progressPeriod.tableProgressPeriods.QuarterId);
            if (Quarter.StartDate <= progressPeriod.tableProgressPeriods.StartDate && Quarter.EndDate >= progressPeriod.tableProgressPeriods.EndDate)
            {
                //int? MasterMarkingPeriodId = Utility.GetMaxPK(this.context, new Func<ProgressPeriods, int>(x => x.MarkingPeriodId));

                int? MarkingPeriodId = 1;

                var progressPeriodData = this.context?.ProgressPeriods.Where(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                if (progressPeriodData != null)
                {
                    MarkingPeriodId = progressPeriodData.MarkingPeriodId + 1;
                }

                progressPeriod.tableProgressPeriods.MarkingPeriodId = (int)MarkingPeriodId;
                progressPeriod.tableProgressPeriods.AcademicYear = (decimal)Quarter.AcademicYear;
                progressPeriod.tableProgressPeriods.CreatedOn = DateTime.UtcNow;
                this.context?.ProgressPeriods.Add(progressPeriod.tableProgressPeriods);
                this.context?.SaveChanges();
                progressPeriod._failure = false;
                progressPeriod._message = "Progress Period Added Successfully";
                //progressPeriod.tableProgressPeriods.Quarters = null;
            }
            else
            {
                progressPeriod._failure = true;
                progressPeriod._message = "Start Date and End Date of progress period should be between Start Date and End Date of quarter";
            }
            return progressPeriod;
        }

        /// <summary>
        /// Update Progress Period
        /// </summary>
        /// <param name="progressPeriod"></param>
        /// <returns></returns>
        public ProgressPeriodAddViewModel UpdateProgressPeriod(ProgressPeriodAddViewModel progressPeriod)
        {
            try
            {
                var progressUpdate = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId && x.MarkingPeriodId == progressPeriod.tableProgressPeriods.MarkingPeriodId);
                
                var Quarter = this.context?.Quarters.FirstOrDefault(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId && x.MarkingPeriodId == progressPeriod.tableProgressPeriods.QuarterId);
                if (Quarter.StartDate <= progressPeriod.tableProgressPeriods.StartDate && Quarter.EndDate >= progressPeriod.tableProgressPeriods.EndDate)
                {
                    progressPeriod.tableProgressPeriods.UpdatedOn = DateTime.Now;
                    progressPeriod.tableProgressPeriods.CreatedBy = progressUpdate.CreatedBy;
                    progressPeriod.tableProgressPeriods.CreatedOn = progressUpdate.CreatedOn;
                    progressPeriod.tableProgressPeriods.AcademicYear = (decimal)Quarter.AcademicYear;
                    this.context.Entry(progressUpdate).CurrentValues.SetValues(progressPeriod.tableProgressPeriods);
                    this.context?.SaveChanges();
                    progressPeriod._failure = false;
                    progressPeriod._message = "Progress Period Updated Successfully";
                }
                else
                {
                    progressPeriod._failure = true;
                    progressPeriod._message = "Start Date and End Date of progress period should be between Start Date and End Date of quarter";
                }
                
            }
            catch (Exception ex)
            {
                progressPeriod.tableProgressPeriods = null;
                progressPeriod._failure = true;
                progressPeriod._message = ex.Message;                
            }
            return progressPeriod;
        }

        /// <summary>
        /// Get Progress Period By Id
        /// </summary>
        /// <param name="progressPeriod"></param>
        /// <returns></returns>
        public ProgressPeriodAddViewModel ViewProgressPeriod(ProgressPeriodAddViewModel progressPeriod)
        {
            ProgressPeriodAddViewModel ProgressPeriodView = new ProgressPeriodAddViewModel();
            try
            {                
                var ProgressPeriodById = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId && x.MarkingPeriodId == progressPeriod.tableProgressPeriods.MarkingPeriodId);
                if (ProgressPeriodById != null)
                {
                    ProgressPeriodView.tableProgressPeriods = ProgressPeriodById;                    
                }
                else
                {
                    ProgressPeriodView._failure = true;
                    ProgressPeriodView._message = NORECORDFOUND;
                    
                }
            }
            catch (Exception es)
            {

                progressPeriod._failure = true;
                progressPeriod._message = es.Message;
            }
            return ProgressPeriodView;
        }

        /// <summary>
        /// Delete Progress Period
        /// </summary>
        /// <param name="progressPeriod"></param>
        /// <returns></returns>
        public ProgressPeriodAddViewModel DeleteProgressPeriod(ProgressPeriodAddViewModel progressPeriod)
        {
            try
            {
                var progressPeriodDelete = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == progressPeriod.tableProgressPeriods.TenantId && x.SchoolId == progressPeriod.tableProgressPeriods.SchoolId && x.MarkingPeriodId == progressPeriod.tableProgressPeriods.MarkingPeriodId);

                this.context?.ProgressPeriods.Remove(progressPeriodDelete);
                this.context?.SaveChanges();
                progressPeriod._failure = false;
                progressPeriod._message = "Progress Period Deleted Successfully";
            }

            catch (Exception es)
            {
                progressPeriod._failure = true;
                progressPeriod._message = es.Message;
            }
            return progressPeriod;
        }

        /// <summary>
        /// Add School Year
        /// </summary>
        /// <param name="schoolYears"></param>
        /// <returns></returns>
        public SchoolYearsAddViewModel AddSchoolYear(SchoolYearsAddViewModel schoolYears)
        {
            try
            {
                //int? MarkingPeriodId = Utility.GetMaxPK(this.context, new Func<SchoolYears, int>(x => x.MarkingPeriodId));

                int? MarkingPeriodId = 1;

                var schoolYearData = this.context?.SchoolYears.Where(x => x.TenantId == schoolYears.tableSchoolYears.TenantId && x.SchoolId == schoolYears.tableSchoolYears.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                if (schoolYearData != null)
                {
                    MarkingPeriodId = schoolYearData.MarkingPeriodId + 1;
                }

                schoolYears.tableSchoolYears.MarkingPeriodId = (int)MarkingPeriodId;
                schoolYears.tableSchoolYears.AcademicYear = schoolYears.tableSchoolYears.StartDate.HasValue == true ? Convert.ToDecimal(schoolYears.tableSchoolYears.StartDate.Value.Year) : (decimal?)null;
                schoolYears.tableSchoolYears.CreatedOn = DateTime.UtcNow;
                schoolYears.tableSchoolYears.TenantId = schoolYears.tableSchoolYears.TenantId;
                this.context?.SchoolYears.Add(schoolYears.tableSchoolYears);
                this.context?.SaveChanges();
                schoolYears._failure = false;
                schoolYears._message = "School Year Added Successfully";
            }
            catch (Exception es)
            {
                schoolYears._failure = true;
                schoolYears._message = es.Message;
            }
            return schoolYears;

        }
        
        /// <summary>
        /// Get School Year By Id
        /// </summary>
        /// <param name="schoolYears"></param>
        /// <returns></returns>
        public SchoolYearsAddViewModel ViewSchoolYear(SchoolYearsAddViewModel schoolYears)
        {
            SchoolYearsAddViewModel schoolYearsAddViewModel = new SchoolYearsAddViewModel();
            try
            {
                var schoolYearsMaster = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == schoolYears.tableSchoolYears.TenantId && x.SchoolId == schoolYears.tableSchoolYears.SchoolId && x.MarkingPeriodId == schoolYears.tableSchoolYears.MarkingPeriodId);
                if (schoolYearsMaster != null)
                {
                    schoolYearsAddViewModel.tableSchoolYears = schoolYearsMaster;
                    schoolYearsAddViewModel._tenantName = schoolYears._tenantName;
                    schoolYearsAddViewModel._failure = false;                    
                }
                else
                {
                    schoolYearsAddViewModel._failure = true;
                    schoolYearsAddViewModel._message = NORECORDFOUND;                    
                }
            }
            catch (Exception es)
            {

                schoolYearsAddViewModel._failure = true;
                schoolYearsAddViewModel._message = es.Message;
            }
            return schoolYearsAddViewModel;
        }
        
        /// <summary>
        /// Update School Year
        /// </summary>
        /// <param name="schoolYears"></param>
        /// <returns></returns>
        public SchoolYearsAddViewModel UpdateSchoolYear(SchoolYearsAddViewModel schoolYears)
        {
            try
            {
                var schoolCalendar = this.context?.SchoolCalendars.FirstOrDefault(x => x.SchoolId==schoolYears.tableSchoolYears.SchoolId && x.TenantId== schoolYears.tableSchoolYears.TenantId && x.AcademicYear== schoolYears.tableSchoolYears.AcademicYear && x.DefaultCalender==true);

                if(schoolCalendar!= null && (schoolCalendar.StartDate < schoolYears.tableSchoolYears.StartDate || schoolCalendar.EndDate > schoolYears.tableSchoolYears.EndDate))
                {
                    schoolYears._failure = true;
                    schoolYears._message = "Start date cannot be changed because it has its association.";
                }
                else
                {
                    var schoolYearsMaster = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == schoolYears.tableSchoolYears.TenantId && x.SchoolId == schoolYears.tableSchoolYears.SchoolId && x.MarkingPeriodId == schoolYears.tableSchoolYears.MarkingPeriodId);                    

                    schoolYears.tableSchoolYears.AcademicYear= schoolYears.tableSchoolYears.StartDate.HasValue == true ? Convert.ToDecimal(schoolYears.tableSchoolYears.StartDate.Value.Year) : (decimal?)null;
                    schoolYears.tableSchoolYears.UpdatedOn = DateTime.UtcNow;
                    schoolYears.tableSchoolYears.CreatedOn = schoolYearsMaster.CreatedOn;
                    schoolYears.tableSchoolYears.CreatedBy = schoolYearsMaster.CreatedBy;
                    this.context.Entry(schoolYearsMaster).CurrentValues.SetValues(schoolYears.tableSchoolYears);
                    this.context?.SaveChanges();
                    schoolYears._failure = false;
                    schoolYears._message = "School Year Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                schoolYears.tableSchoolYears = null;
                schoolYears._failure = true;
                schoolYears._message = ex.Message;                
            }
            return schoolYears;
        }
        
        /// <summary>
        /// Delete School Year
        /// </summary>
        /// <param name="schoolYears"></param>
        /// <returns></returns>
        public SchoolYearsAddViewModel DeleteSchoolYear(SchoolYearsAddViewModel schoolYears)
        {
            try
            {
                var deleteSchoolYear = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == schoolYears.tableSchoolYears.TenantId && x.SchoolId == schoolYears.tableSchoolYears.SchoolId && x.MarkingPeriodId == schoolYears.tableSchoolYears.MarkingPeriodId);
                var semester = this.context?.Semesters.FirstOrDefault(z => z.TenantId == deleteSchoolYear.TenantId && z.SchoolId == deleteSchoolYear.SchoolId && z.YearId == deleteSchoolYear.MarkingPeriodId);
                var schoolCalendar = this.context?.SchoolCalendars.FirstOrDefault(x => x.SchoolId == schoolYears.tableSchoolYears.SchoolId);

                if (semester!=null || schoolCalendar!=null)
                {
                    schoolYears.tableSchoolYears = null;
                    schoolYears._message = "School Year cannot be deleted because it has its association";
                    schoolYears._failure = true;
                }
                else
                {
                    this.context?.SchoolYears.Remove(deleteSchoolYear);
                    this.context?.SaveChanges();
                    schoolYears._failure = false;
                    schoolYears._message = "School Year Deleted Successfully";
                }                
            }
            catch (Exception es)
            {
                schoolYears._failure = true;
                schoolYears._message = es.Message;
            }
            return schoolYears;
        }

        /// <summary>
        /// Add Quarter
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public QuarterAddViewModel AddQuarter(QuarterAddViewModel quarters)
        {
            try
            {
                var semester = this.context?.Semesters.FirstOrDefault(x => x.TenantId == quarters.tableQuarter.TenantId && x.SchoolId == quarters.tableQuarter.SchoolId && x.MarkingPeriodId == quarters.tableQuarter.SemesterId);
                if (semester.StartDate <= quarters.tableQuarter.StartDate && semester.EndDate >= quarters.tableQuarter.EndDate)
                {
                    //int? MarkingPeriodId = Utility.GetMaxPK(this.context, new Func<Quarters, int>(x => x.MarkingPeriodId));

                    int? MarkingPeriodId = 1;

                    var quarterData = this.context?.Quarters.Where(x => x.TenantId == quarters.tableQuarter.TenantId && x.SchoolId == quarters.tableQuarter.SchoolId).OrderByDescending(x => x.MarkingPeriodId).FirstOrDefault();

                    if (quarterData != null)
                    {
                        MarkingPeriodId = quarterData.MarkingPeriodId + 1;
                    }

                    quarters.tableQuarter.MarkingPeriodId = (int)MarkingPeriodId;
                    quarters.tableQuarter.AcademicYear = semester.AcademicYear;
                    quarters.tableQuarter.CreatedOn = DateTime.UtcNow;
                    quarters.tableQuarter.TenantId = quarters.tableQuarter.TenantId;
                    this.context?.Quarters.Add(quarters.tableQuarter);
                    this.context?.SaveChanges();
                    quarters._failure = false;
                    quarters._message = "Quarter Added Successfully";
                    //quarters.tableQuarter.Semesters = null;
                }
                else
                {
                    quarters._failure = true;
                    quarters._message = "Start Date And End Date of Quarter Should Be Between Start Date And End Date of Semester";
                }
            }
            catch (Exception es)
            {

                quarters._failure = true;
                quarters._message = es.Message;
            }

            return quarters;
        }
        
        /// <summary>
        /// Get Quarter By Id
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public QuarterAddViewModel ViewQuarter(QuarterAddViewModel quarter)
        {
            QuarterAddViewModel quarterAddViewModel = new QuarterAddViewModel();
            try
            {
                var quarteMaster = this.context?.Quarters.FirstOrDefault(x => x.TenantId == quarter.tableQuarter.TenantId && x.SchoolId == quarter.tableQuarter.SchoolId && x.MarkingPeriodId == quarter.tableQuarter.MarkingPeriodId);
                if (quarteMaster != null)
                {
                    quarterAddViewModel.tableQuarter = quarteMaster;
                    quarterAddViewModel._tenantName = quarter._tenantName;
                    quarterAddViewModel._failure = false;                    
                }
                else
                {
                    quarterAddViewModel._failure = true;
                    quarterAddViewModel._message = NORECORDFOUND;                   
                }
            }
            catch (Exception es)
            {
                quarterAddViewModel._failure = true;
                quarterAddViewModel._message = es.Message;
            }
            return quarterAddViewModel;
        }
        
        /// <summary>
        /// Update Quarter
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public QuarterAddViewModel UpdateQuarter(QuarterAddViewModel quarters)
        {
            try
            {
                var quarteMaster = this.context?.Quarters.FirstOrDefault(x => x.TenantId == quarters.tableQuarter.TenantId && x.SchoolId == quarters.tableQuarter.SchoolId && x.MarkingPeriodId == quarters.tableQuarter.MarkingPeriodId);
                
                var semester = this.context?.Semesters.FirstOrDefault(x => x.TenantId == quarters.tableQuarter.TenantId && x.SchoolId == quarters.tableQuarter.SchoolId && x.MarkingPeriodId == quarters.tableQuarter.SemesterId);
                if(semester.StartDate <= quarters.tableQuarter.StartDate && semester.EndDate >= quarters.tableQuarter.EndDate)
                {
                    quarters.tableQuarter.UpdatedOn = DateTime.Now;
                    quarters.tableQuarter.CreatedBy = quarteMaster.CreatedBy;
                    quarters.tableQuarter.CreatedOn = quarteMaster.CreatedOn;
                    quarters.tableQuarter.AcademicYear = semester.AcademicYear;
                    this.context.Entry(quarteMaster).CurrentValues.SetValues(quarters.tableQuarter);
                    this.context?.SaveChanges();
                    quarters._failure = false;
                    quarters._message = "Quarter Updated Successfully";
                }
                else
                {
                    quarters._failure = true;
                    quarters._message = "Start Date And End Date of Quarter Should Be Between Start Date And End Date of Semester";
                }                
            }
            catch (Exception ex)
            {
                quarters.tableQuarter = null;
                quarters._failure = true;
                quarters._message = ex.Message;                
            }
            return quarters;
        }
        
        /// <summary>
        /// Delete Quarter
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public QuarterAddViewModel DeleteQuarter(QuarterAddViewModel quarter)
        {
            try
            {
                var quarterDelete = this.context?.Quarters.FirstOrDefault(x => x.TenantId == quarter.tableQuarter.TenantId && x.SchoolId == quarter.tableQuarter.SchoolId && x.MarkingPeriodId == quarter.tableQuarter.MarkingPeriodId);
                var progressPeriod = this.context?.ProgressPeriods.FirstOrDefault(z => z.TenantId == quarterDelete.TenantId && z.SchoolId == quarterDelete.SchoolId && z.QuarterId == quarterDelete.MarkingPeriodId);
                if(progressPeriod != null)
                {
                    quarter.tableQuarter = null;
                    quarter._message = "Quarter cannot be deleted because it has its association";
                    quarter._failure = true;
                }
                else
                {
                    this.context?.Quarters.Remove(quarterDelete);
                    this.context?.SaveChanges();
                    quarter._failure = false;
                    quarter._message = "Quarter Deleted Successfully";
                }
            }
            catch (Exception es)
            {
                quarter._failure = true;
                quarter._message = es.Message;
            }
            return quarter;
        }

        /// <summary>
        /// Get Academic Year List
        /// </summary>
        /// <param name="downViewModel"></param>
        /// <returns></returns>
        public DropDownViewModel GetAcademicYearList(DropDownViewModel downViewModel)
        {
            DropDownViewModel dropDownViewModel = new DropDownViewModel();
            var data = this.context?.SchoolYears.Where(x => x.TenantId == downViewModel.TenantId && x.SchoolId == downViewModel.SchoolId).Select(m => new AcademicYear()
            {
                StartDate=m.StartDate,
                EndDate=m.EndDate,
                Year = m.StartDate.HasValue == true && m.StartDate.Value.Year != m.EndDate.Value.Year && m.EndDate.HasValue == true ? m.StartDate.Value.Year.ToString() + "-" + m.EndDate.Value.Year.ToString() :
                      m.StartDate.HasValue == true && m.StartDate.Value.Year == m.EndDate.Value.Year && m.EndDate.HasValue == true ? m.EndDate.Value.Year.ToString() : m.StartDate.HasValue == true && m.EndDate.HasValue == false ? m.StartDate.Value.Year.ToString()
                      : m.StartDate.HasValue == false && m.EndDate.HasValue == true ? m.EndDate.Value.Year.ToString() : null,AcademyYear=(int)m.AcademicYear
            }).ToList();

            dropDownViewModel.AcademicYears = data;
            dropDownViewModel.SchoolId = downViewModel.SchoolId;
            dropDownViewModel.TenantId = downViewModel.TenantId;
            dropDownViewModel._tenantName = downViewModel._tenantName;

            if (data.Count() > 0)
            {
                dropDownViewModel._failure = false;
            }
            else
            {
                dropDownViewModel._failure = true;
                dropDownViewModel._message = NORECORDFOUND;
            }            
            return dropDownViewModel;
        }

        /// <summary>
        /// Get Marking Period Title List
        /// </summary>
        /// <param name="periodViewModel"></param>
        /// <returns></returns>
        public PeriodViewModel GetMarkingPeriodTitleList(PeriodViewModel periodViewModel)
        {
            if (periodViewModel.AcademicYear > 0)
            {
                periodViewModel.period = new List<PeriodView>();

                var SemesterList = this.context?.Semesters.Include(x=>x.Quarters).ThenInclude(x=>x.ProgressPeriods).Where(x => x.AcademicYear == periodViewModel.AcademicYear && x.SchoolId==periodViewModel.SchoolId && x.TenantId==periodViewModel.TenantId).ToList();
                
                if (SemesterList.Count > 0)
                {
                    foreach (var Semester in SemesterList)
                    {
                        var QuarterList = Semester.Quarters.Where(x => x.SemesterId == Semester.MarkingPeriodId && x.AcademicYear== periodViewModel.AcademicYear).ToList();

                        var QuarterListForAnySemester = SemesterList.Where(x => x.Quarters.Count > 0).ToList().SelectMany(x => x.Quarters).ToList();

                        if(QuarterListForAnySemester.Count>0)
                        {
                            if (QuarterList.Count > 0)
                            {
                                var ProgressPeriodListForAnyQuater = QuarterListForAnySemester.Where(m => m.ProgressPeriods.Count > 0).ToList();

                                if (ProgressPeriodListForAnyQuater.Count > 0)
                                {
                                    foreach (var Quarter in QuarterList)
                                    {
                                        var ProgressPeriodList = Quarter.ProgressPeriods.Where(x => x.QuarterId == Quarter.MarkingPeriodId && x.AcademicYear == periodViewModel.AcademicYear).ToList();

                                        if (ProgressPeriodList.Count > 0)
                                        {
                                            foreach (var ProgressPeriod in ProgressPeriodList)
                                            {
                                                var ProgressList = new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = ProgressPeriod.Title, EndDate = ProgressPeriod.EndDate, StartDate = ProgressPeriod.StartDate };
                                                periodViewModel.period.Add(ProgressList);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (periodViewModel.period.Count == 0)
                                    {
                                        var QuaterTitleList = SemesterList.SelectMany(x => x.Quarters).ToList().Select(x => new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).ToList();
                                        periodViewModel.period.AddRange(QuaterTitleList);
                                    }
                                }                               
                            }
                        }           
                        else
                        {
                            if (periodViewModel.period.Count == 0)
                            {
                                var SemesterTitleList = SemesterList.Select(x => new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).ToList();
                                periodViewModel.period.AddRange(SemesterTitleList);
                            }
                        }
                    }
                }
                else
                {
                    var YearTitle = this.context?.SchoolYears.Where(x => x.TenantId == periodViewModel.TenantId && x.SchoolId == periodViewModel.SchoolId && x.AcademicYear == periodViewModel.AcademicYear).Select(x => new PeriodView() {MarkingPeriodId=x.MarkingPeriodId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).FirstOrDefault();
                    periodViewModel.period.Add(YearTitle);
                }
                periodViewModel._failure = false;
            }          

            else
            {
                periodViewModel._message = "Provide valid year id";
                periodViewModel._failure = true;
            }

            if (periodViewModel.period.Count > 1)
            {
                periodViewModel.period = periodViewModel.period.OrderBy(x => x.StartDate).ToList();
            }

            return periodViewModel;
        }

        ///// <summary>
        ///// Get Marking Period Title List
        ///// </summary>
        ///// <param name="periodViewModel"></param>
        ///// <returns></returns>
        //public PeriodViewModel GetMarkingPeriodTitleList(PeriodViewModel periodViewModel)
        //{
        //    if (periodViewModel.AcademicYear > 0)
        //    {
        //        periodViewModel.period = new List<PeriodView>();
        //        var SemesterList = this.context?.Semesters.Include(x => x.Quarters).ThenInclude(x => x.ProgressPeriods).Where(x => x.AcademicYear == periodViewModel.AcademicYear && x.SchoolId == periodViewModel.SchoolId && x.TenantId == periodViewModel.TenantId).ToList();

        //        if (SemesterList.Count > 0)
        //        {
        //            foreach (var Semester in SemesterList)
        //            {
        //                var QuarterList = Semester.Quarters.Where(x => x.SemesterId == Semester.MarkingPeriodId && x.AcademicYear == periodViewModel.AcademicYear).ToList();

        //                if (QuarterList.Count > 0)
        //                {
        //                    foreach (var Quarter in QuarterList)
        //                    {
        //                        var ProgressPeriodList = Quarter.ProgressPeriods.Where(x => x.QuarterId == Quarter.MarkingPeriodId && x.AcademicYear == periodViewModel.AcademicYear).ToList();

        //                        if (ProgressPeriodList.Count > 0)
        //                        {
        //                            foreach (var ProgressPeriod in ProgressPeriodList)
        //                            {
        //                                var ProgressList = new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = ProgressPeriod.Title, EndDate = ProgressPeriod.EndDate, StartDate = ProgressPeriod.StartDate };
        //                                periodViewModel.period.Add(ProgressList);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (periodViewModel.period.Count == 0)
        //                            {
        //                                var QuaterTitleList = SemesterList.SelectMany(x => x.Quarters).ToList().Select(x => new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).ToList();
        //                                periodViewModel.period.AddRange(QuaterTitleList);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (periodViewModel.period.Count == 0)
        //                    {
        //                        var SemesterTitleList = SemesterList.Select(x => new PeriodView() { MarkingPeriodId = Semester.YearId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).ToList();
        //                        periodViewModel.period.AddRange(SemesterTitleList);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var YearTitle = this.context?.SchoolYears.Where(x => x.TenantId == periodViewModel.TenantId && x.SchoolId == periodViewModel.SchoolId && x.AcademicYear == periodViewModel.AcademicYear).Select(x => new PeriodView() { MarkingPeriodId = x.MarkingPeriodId, PeriodTitle = x.Title, EndDate = x.EndDate, StartDate = x.StartDate }).FirstOrDefault();
        //            periodViewModel.period.Add(YearTitle);
        //        }
        //        periodViewModel._failure = false;
        //    }
        //    else
        //    {
        //        periodViewModel._message = "Provide valid year id";
        //        periodViewModel._failure = true;
        //    }
        //    return periodViewModel;
        //}

        /// <summary>
        /// Get All Marking Period List
        /// </summary>
        /// <param name="markingPeriodListViewModel"></param>
        /// <returns></returns>
        public MarkingPeriodListViewModel GetAllMarkingPeriodList(MarkingPeriodListViewModel markingPeriodListViewModel)
        {
            MarkingPeriodListViewModel markingPeriodList = new MarkingPeriodListViewModel();
            try
            {
                var markingPeriodData = this.context?.SchoolYears.Include(x => x.Semesters).ThenInclude(s => s.Quarters).Where(x => x.TenantId == markingPeriodListViewModel.TenantId && x.SchoolId == markingPeriodListViewModel.SchoolId && x.AcademicYear == markingPeriodListViewModel.AcademicYear).ToList();
                if (markingPeriodData.Count > 0)
                {
                    foreach (var markingPeriod in markingPeriodData)
                    {
                        var schoolYear = new GetMarkingPeriodView
                        {
                            Value = "0" + "_" + markingPeriod.MarkingPeriodId,
                            Text = markingPeriod.ShortName,
                            StartDate = markingPeriod.StartDate,
                            EndDate = markingPeriod.EndDate,
                            FullName=markingPeriod.Title
                    };
                        markingPeriodList.getMarkingPeriodView.Add(schoolYear);
                    }

                    var semesterData = markingPeriodData.SelectMany(x=>x.Semesters).ToList();
                    foreach (var semester in semesterData)
                    {
                        var sem = new GetMarkingPeriodView
                        {
                            Value = "1" + "_" + semester.MarkingPeriodId,
                            Text = semester.ShortName,
                            StartDate = semester.StartDate,
                            EndDate = semester.EndDate,
                            FullName = semester.Title
                        };
                        markingPeriodList.getMarkingPeriodView.Add(sem);
                    }
                    var quaterData = markingPeriodData.SelectMany(x => x.Semesters).SelectMany(x=>x.Quarters).ToList();
                    foreach (var quater in quaterData)
                    {
                        var qtr = new GetMarkingPeriodView
                        {
                            Value = "2" + "_" + quater.MarkingPeriodId,
                            Text = quater.ShortName,
                            StartDate = quater.StartDate,
                            EndDate = quater.EndDate,
                            FullName = quater.Title
                        };
                        markingPeriodList.getMarkingPeriodView.Add(qtr);
                    }                      
                    markingPeriodList._failure = false;
                }
                else
                {
                    markingPeriodList._failure = true;
                    markingPeriodList._message = NORECORDFOUND;
                }
                markingPeriodList.TenantId = markingPeriodListViewModel.TenantId;
                markingPeriodList.SchoolId = markingPeriodListViewModel.SchoolId;
                markingPeriodList.AcademicYear = markingPeriodListViewModel.AcademicYear;
                markingPeriodList._tenantName = markingPeriodListViewModel._tenantName;
                markingPeriodList._token = markingPeriodListViewModel._token;
            }
            catch (Exception es)
            {
                markingPeriodList.getMarkingPeriodView = null;
                markingPeriodList._failure = true;
                markingPeriodList._message = es.Message;
            }
            return markingPeriodList;
        }
    }
}
