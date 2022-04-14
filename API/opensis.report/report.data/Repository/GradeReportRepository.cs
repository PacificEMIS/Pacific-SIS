using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.GradeReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace opensis.report.report.data.Repository
{
    public class GradeReportRepository : IGradeReportRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public GradeReportRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Get Grade Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public HonorRollListForReport GetHonorRollReport(PageResult pageResult)
        {
            HonorRollListForReport honorRollList = new();
            try
            {
                var studentDatas = new List<StudentFinalGrade>();
                IQueryable<HonorRollViewForReport>? transactionIQ = null;
                int? totalCount = 0;
                decimal? avgPercentage = 0;

                var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (progressPeriodsData != null)
                {
                    studentDatas = this.context?.StudentFinalGrade.Include(s => s.StudentMaster).ThenInclude(e => e.StudentEnrollment).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.PrgrsprdMarkingPeriodId == progressPeriodsData.MarkingPeriodId && e.IsExamGrade != true).ToList();
                }
                else
                {
                    var quartersData = this.context?.Quarters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                    if (quartersData != null)
                    {
                        studentDatas = this.context?.StudentFinalGrade.Include(s => s.StudentMaster).ThenInclude(e => e.StudentEnrollment).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.QtrMarkingPeriodId == quartersData.MarkingPeriodId && e.IsExamGrade != true).ToList();

                    }
                    else
                    {
                        var semestersData = this.context?.Semesters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                        if (semestersData != null)
                        {
                            studentDatas = this.context?.StudentFinalGrade.Include(s => s.StudentMaster).ThenInclude(e => e.StudentEnrollment).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.SmstrMarkingPeriodId == semestersData.MarkingPeriodId && e.IsExamGrade != true).ToList();
                        }
                        else
                        {
                            var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                            if (yearsData != null)
                            {
                                studentDatas = this.context?.StudentFinalGrade.Include(s => s.StudentMaster).ThenInclude(e => e.StudentEnrollment).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.YrMarkingPeriodId == yearsData.MarkingPeriodId && e.IsExamGrade != true).ToList();

                            }
                        }
                    }
                }
                var honorRollData = this.context?.HonorRolls.Where(h => h.SchoolId == pageResult.SchoolId && h.TenantId == pageResult.TenantId).ToList();

                var SectionData = this.context?.Sections.Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId).ToList();

                var studentIds = studentDatas.Select(x => x.StudentId).Distinct().ToList();

                if (studentIds != null && studentIds.Any())
                {
                    foreach (var studentId in studentIds)
                    {
                        HonorRollViewForReport honorRoll = new();
                        var totalCourseSectionCount = 0;
                        decimal? totalPercent = 0;
                        var studentRecords = studentDatas.Where(y => y.StudentId == studentId).ToList();

                        foreach (var students in studentRecords)
                        {
                            var courseSectionData = this.context?.CourseSection.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.CourseSectionId == students.CourseSectionId).FirstOrDefault();
                            if (courseSectionData.AffectsHonorRoll == true)
                            {
                                totalCourseSectionCount = totalCourseSectionCount + 1;
                                totalPercent = totalPercent + students.PercentMarks;
                            }
                        }
                        if (totalCourseSectionCount > 0 && totalPercent > 0)
                        {
                            avgPercentage = totalPercent / totalCourseSectionCount;

                            var studentData = studentRecords.FirstOrDefault();
                            if (studentData != null)
                            {
                                honorRoll.Salutation = studentData.StudentMaster.Salutation;
                                honorRoll.FirstGivenName = studentData.StudentMaster.FirstGivenName;
                                honorRoll.MiddleName = studentData.StudentMaster.MiddleName;
                                honorRoll.LastFamilyName = studentData.StudentMaster.LastFamilyName;
                                //honorRoll.StudentId = studentData.StudentMaster.StudentId;
                                honorRoll.AlternateId = studentData.StudentMaster.AlternateId;
                                honorRoll.MobilePhone = studentData.StudentMaster.MobilePhone;
                                honorRoll.StudentGuid = studentData.StudentMaster.StudentGuid;
                                honorRoll.StudentInternalId = studentData.StudentMaster.StudentInternalId;
                                honorRoll.DistrictId = studentData.StudentMaster.DistrictId;
                                honorRoll.StateId = studentData.StudentMaster.StateId;
                                honorRoll.AdmissionNumber = studentData.StudentMaster.AdmissionNumber;
                                honorRoll.RollNumber = studentData.StudentMaster.RollNumber;
                                honorRoll.Suffix = studentData.StudentMaster.Suffix;
                                honorRoll.PreferredName = studentData.StudentMaster.PreferredName;
                                honorRoll.PreviousName = studentData.StudentMaster.PreviousName;
                                honorRoll.SocialSecurityNumber = studentData.StudentMaster.SocialSecurityNumber;
                                honorRoll.OtherGovtIssuedNumber = studentData.StudentMaster.OtherGovtIssuedNumber;
                                //honorRoll.StudentPhoto = studentData.StudentMaster.StudentPhoto;
                                honorRoll.Dob = studentData.StudentMaster.Dob;
                                honorRoll.StudentPortalId = studentData.StudentMaster.StudentPortalId;
                                honorRoll.Gender = studentData.StudentMaster.Gender;
                                honorRoll.Race = studentData.StudentMaster.Race;
                                honorRoll.MaritalStatus = studentData.StudentMaster.MaritalStatus;
                                honorRoll.Ethnicity = studentData.StudentMaster.Ethnicity;
                                honorRoll.CountryOfBirth = studentData.StudentMaster.CountryOfBirth;
                                honorRoll.Nationality = studentData.StudentMaster.Nationality;
                                honorRoll.FirstLanguageId = studentData.StudentMaster.FirstLanguageId;
                                honorRoll.SecondLanguageId = studentData.StudentMaster.SecondLanguageId;
                                honorRoll.ThirdLanguageId = studentData.StudentMaster.ThirdLanguageId;
                                honorRoll.EstimatedGradDate = studentData.StudentMaster.EstimatedGradDate;
                                honorRoll.Eligibility504 = studentData.StudentMaster.Eligibility504;
                                honorRoll.EconomicDisadvantage = studentData.StudentMaster.EconomicDisadvantage;
                                honorRoll.FreeLunchEligibility = studentData.StudentMaster.FreeLunchEligibility;
                                honorRoll.SpecialEducationIndicator = studentData.StudentMaster.SpecialEducationIndicator;
                                honorRoll.LepIndicator = studentData.StudentMaster.LepIndicator;
                                honorRoll.HomePhone = studentData.StudentMaster.HomePhone;
                                honorRoll.PersonalEmail = studentData.StudentMaster.PersonalEmail;
                                honorRoll.SchoolEmail = studentData.StudentMaster.SchoolEmail;
                                honorRoll.Twitter = studentData.StudentMaster.Twitter;
                                honorRoll.Facebook = studentData.StudentMaster.Facebook;
                                honorRoll.Instagram = studentData.StudentMaster.Instagram;
                                honorRoll.Youtube = studentData.StudentMaster.Youtube;
                                honorRoll.Linkedin = studentData.StudentMaster.Linkedin;
                                honorRoll.HomeAddressLineOne = studentData.StudentMaster.HomeAddressLineOne;
                                honorRoll.HomeAddressLineTwo = studentData.StudentMaster.HomeAddressLineTwo;
                                honorRoll.HomeAddressCity = studentData.StudentMaster.HomeAddressCity;
                                honorRoll.HomeAddressState = studentData.StudentMaster.HomeAddressState;
                                honorRoll.HomeAddressCountry = studentData.StudentMaster.HomeAddressCountry;
                                honorRoll.HomeAddressZip = studentData.StudentMaster.HomeAddressZip;
                                honorRoll.BusNo = studentData.StudentMaster.BusNo;
                                honorRoll.SchoolBusPickUp = studentData.StudentMaster.SchoolBusPickUp;
                                honorRoll.SchoolBusDropOff = studentData.StudentMaster.SchoolBusDropOff;
                                honorRoll.MailingAddressSameToHome = studentData.StudentMaster.MailingAddressSameToHome;
                                honorRoll.MailingAddressLineOne = studentData.StudentMaster.MailingAddressLineOne;
                                honorRoll.MailingAddressLineTwo = studentData.StudentMaster.MailingAddressLineTwo;
                                honorRoll.MailingAddressCity = studentData.StudentMaster.MailingAddressCity;
                                honorRoll.MailingAddressCountry = studentData.StudentMaster.MailingAddressCountry;
                                honorRoll.MailingAddressZip = studentData.StudentMaster.MailingAddressZip;
                                honorRoll.CriticalAlert = studentData.StudentMaster.CriticalAlert;
                                honorRoll.AlertDescription = studentData.StudentMaster.AlertDescription;
                                honorRoll.PrimaryCarePhysician = studentData.StudentMaster.PrimaryCarePhysician;
                                honorRoll.PrimaryCarePhysicianPhone = studentData.StudentMaster.PrimaryCarePhysicianPhone;
                                honorRoll.MedicalFacility = studentData.StudentMaster.MedicalFacility;
                                honorRoll.MedicalFacilityPhone = studentData.StudentMaster.MedicalFacilityPhone;
                                honorRoll.InsuranceCompany = studentData.StudentMaster.InsuranceCompany;
                                honorRoll.InsuranceCompanyPhone = studentData.StudentMaster.InsuranceCompanyPhone;
                                honorRoll.PolicyNumber = studentData.StudentMaster.PolicyNumber;
                                honorRoll.PolicyHolder = studentData.StudentMaster.PolicyHolder;
                                honorRoll.Dentist = studentData.StudentMaster.Dentist;
                                honorRoll.DentistPhone = studentData.StudentMaster.DentistPhone;
                                honorRoll.Vision = studentData.StudentMaster.Vision;
                                honorRoll.VisionPhone = studentData.StudentMaster.VisionPhone;
                                honorRoll.IsActive = studentData.StudentMaster.IsActive;
                                honorRoll.HonorRoll = honorRollData.FirstOrDefault(h => h.Breakoff <= avgPercentage)?.HonorRoll;
                                honorRoll.GradeName = studentData.StudentMaster.StudentEnrollment.FirstOrDefault(x => x.IsActive == true)?.GradeLevelTitle;
                                honorRoll.SectionName = SectionData.FirstOrDefault(y => y.SectionId == studentData.StudentMaster.SectionId)?.Name;
                            }
                            honorRollList.HonorRollViewForReports.Add(honorRoll);

                        }
                    }
                    var StudentHonorRollData = honorRollList.HonorRollViewForReports.AsQueryable();
                    //Filteration Start.......//

                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = StudentHonorRollData;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = StudentHonorRollData.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) ||
                            x.StudentInternalId != null && x.StudentInternalId.Contains(Columnvalue) ||
                                                                        x.AlternateId != null && x.AlternateId.Contains(Columnvalue) ||
                                                                        x.GradeName != null && x.GradeName.Contains(Columnvalue) ||
                                                                        x.SectionName != null && x.SectionName.Contains(Columnvalue) ||
                                                                        x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue) ||
                                                                        x.HonorRoll != null && x.HonorRoll.Contains(Columnvalue));

                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, StudentHonorRollData).AsQueryable();
                        }
                    }
                    if (pageResult.SortingModel != null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                    }
                    else
                    {
                        transactionIQ = transactionIQ.OrderBy(s => s.LastFamilyName).ThenBy(c => c.FirstGivenName);
                    }

                    totalCount = transactionIQ.Count();


                    if (totalCount > 0)
                    {
                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        honorRollList.HonorRollViewForReports = transactionIQ.ToList();

                        honorRollList.TotalCount = totalCount;
                        honorRollList._message = "success";
                        honorRollList._failure = false;
                    }
                    else
                    {
                        honorRollList._message = NORECORDFOUND;
                        honorRollList._failure = true;
                        honorRollList.HonorRollViewForReports = new();
                    }

                }

                else
                {
                    honorRollList._failure = true;
                    honorRollList._message = NORECORDFOUND;
                }

                var SchoolDetails = this.context?.SchoolMaster.Include(y => y.SchoolDetail).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId).FirstOrDefault();

                honorRollList.SchoolId = pageResult.SchoolId;
                honorRollList.AcademicYear = pageResult.AcademicYear;
                honorRollList.TenantId = pageResult.TenantId;
                honorRollList._tenantName = pageResult._tenantName;
                honorRollList._userName = pageResult._userName;
                honorRollList.PageNumber = pageResult.PageNumber;
                honorRollList.PageSize = pageResult.PageSize;
                honorRollList.SchoolName = SchoolDetails.SchoolName;
                honorRollList.SchoolLogo = SchoolDetails.SchoolDetail.FirstOrDefault()?.SchoolLogo;
                honorRollList.Address1 = SchoolDetails.StreetAddress1;
                honorRollList.Address2 = SchoolDetails.StreetAddress2;
                honorRollList.Country = SchoolDetails.Country;
                honorRollList.State = SchoolDetails.State;
                honorRollList.City = SchoolDetails.City;
                honorRollList.District = SchoolDetails.District;
                honorRollList.Division = SchoolDetails.Division;
                honorRollList.Zip = SchoolDetails.Zip;
                honorRollList._token = pageResult._token;
            }
            catch (Exception ex)
            {
                honorRollList._failure = true;
                honorRollList._message = ex.Message;
            }
            return honorRollList;
        }

        /// <summary>
        /// GetStudentFinalGradeReport
        /// </summary>
        /// <param name="studentFinalGradeViewModel"></param>
        /// <returns></returns>
        public StudentFinalGradeViewModel GetStudentFinalGradeReport(StudentFinalGradeViewModel studentFinalGradeViewModel)
        {
            StudentFinalGradeViewModel studentFinalGrade = new();
            studentFinalGrade.TenantId = studentFinalGradeViewModel.TenantId;
            studentFinalGrade.SchoolId = studentFinalGradeViewModel.SchoolId;
            studentFinalGrade.AcademicYear = studentFinalGradeViewModel.AcademicYear;
            studentFinalGrade._userName = studentFinalGradeViewModel._userName;
            studentFinalGrade._token = studentFinalGradeViewModel._token;
            studentFinalGrade._tenantName = studentFinalGradeViewModel._tenantName;
            try
            {
                List<int?> QtrMarkingPeriodId = new List<int?>();
                List<int?> SmstrMarkingPeriodId = new List<int?>();
                List<int?> YrMarkingPeriodId = new List<int?>();
                List<int?> PrgrsprdMarkingPeriodId = new List<int?>();
                List<int?> QtrMarkingPeriodIdExam = new List<int?>();
                List<int?> SmstrMarkingPeriodIdExam = new List<int?>();
                List<int?> YrMarkingPeriodIdExam = new List<int?>();
                List<int?> PrgrsprdMarkingPeriodIdExam = new List<int?>();
                List<MarkingPeriodDetailsView> markingPeriodList = new List<MarkingPeriodDetailsView>();

                var markingPeriodsData = studentFinalGradeViewModel.MarkingPeriods!.Split(",");

                foreach (var markingPeriod in markingPeriodsData)
                {
                    MarkingPeriodDetailsView markingPeriodListView = new MarkingPeriodDetailsView();
                    if (markingPeriod != null)
                    {
                        var markingPeriodid = markingPeriod.Split("_", StringSplitOptions.RemoveEmptyEntries);

                        if (markingPeriodid.First() == "3")
                        {
                            var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                            var ppData = this.context?.ProgressPeriods.FirstOrDefault(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == studentFinalGradeViewModel.AcademicYear);
                            markingPeriodListView.SortId = "3_" + ppData.MarkingPeriodId;

                            if (markingPeriodid.Last() == "E")
                            {
                                markingPeriodListView.MarkingPeriodName = ppData.Title + " Exam";
                                PrgrsprdMarkingPeriodIdExam.Add(Id);
                            }
                            else
                            {
                                markingPeriodListView.MarkingPeriodName = ppData.Title;
                                PrgrsprdMarkingPeriodId.Add(Id);
                            }
                            markingPeriodList.Add(markingPeriodListView);
                        }

                        else if (markingPeriodid.First() == "2")
                        {
                            var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                            var qtrData = this.context?.Quarters.FirstOrDefault(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == studentFinalGradeViewModel.AcademicYear);
                            markingPeriodListView.SortId = "2_" + qtrData.MarkingPeriodId;

                            if (markingPeriodid.Last() == "E")
                            {
                                markingPeriodListView.MarkingPeriodName = qtrData.Title + " Exam";
                                QtrMarkingPeriodIdExam.Add(Id);
                            }
                            else
                            {
                                markingPeriodListView.MarkingPeriodName = qtrData.Title;
                                QtrMarkingPeriodId.Add(Id);
                            }
                            markingPeriodList.Add(markingPeriodListView);
                        }

                        else if (markingPeriodid.First() == "1")
                        {
                            var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                            var smstrData = this.context?.Semesters.FirstOrDefault(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == studentFinalGradeViewModel.AcademicYear);
                            markingPeriodListView.SortId = "1_" + smstrData.MarkingPeriodId;

                            if (markingPeriodid.Last() == "E")
                            {
                                markingPeriodListView.MarkingPeriodName = smstrData.Title + " Exam";
                                SmstrMarkingPeriodIdExam.Add(Id);
                            }
                            else
                            {
                                markingPeriodListView.MarkingPeriodName = smstrData.Title;
                                SmstrMarkingPeriodId.Add(Id);
                            }
                            markingPeriodList.Add(markingPeriodListView);
                        }

                        else if (markingPeriodid.First() == "0")
                        {
                            var Id = Int32.Parse(markingPeriodid.ElementAt(1));

                            var yrData = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId && x.MarkingPeriodId == Id && x.AcademicYear == studentFinalGradeViewModel.AcademicYear);
                            markingPeriodListView.SortId = "0_" + yrData.MarkingPeriodId;

                            if (markingPeriodid.Last() == "E")
                            {
                                markingPeriodListView.MarkingPeriodName = yrData.Title + " Exam";
                                YrMarkingPeriodIdExam.Add(Id);
                            }
                            else
                            {
                                markingPeriodListView.MarkingPeriodName = yrData.Title;
                                YrMarkingPeriodId.Add(Id);
                            }
                            markingPeriodList.Add(markingPeriodListView);
                        }
                    }
                }

                //all required data fetch
                var studentFinalGradeData = this.context?.StudentFinalGrade.Include(x => x.StudentFinalGradeComments).Include(x => x.StudentMaster).Include(x => x.SchoolYears).Include(x => x.Semesters).Include(x => x.Quarters).Include(x => x.ProgressPeriod).Join(this.context?.CourseSection.Include(c => c.StaffCoursesectionSchedule).ThenInclude(c => c.StaffMaster), sfg => sfg.CourseSectionId, cs => cs.CourseSectionId,
                            (sfg, cs) => new { sfg, cs }).Where(x => x.sfg.TenantId == studentFinalGradeViewModel.TenantId && x.sfg.SchoolId == studentFinalGradeViewModel.SchoolId && x.cs.TenantId == studentFinalGradeViewModel.TenantId && x.cs.SchoolId == studentFinalGradeViewModel.SchoolId && studentFinalGradeViewModel.StudentIds.Contains(x.sfg.StudentId) && x.sfg.AcademicYear == studentFinalGradeViewModel.AcademicYear &&

                            ((((x.sfg.YrMarkingPeriodId != null && YrMarkingPeriodId.Contains(x.sfg.YrMarkingPeriodId)) || (x.sfg.SmstrMarkingPeriodId != null && SmstrMarkingPeriodId.Contains(x.sfg.SmstrMarkingPeriodId)) || (x.sfg.QtrMarkingPeriodId != null && QtrMarkingPeriodId.Contains(x.sfg.QtrMarkingPeriodId)) || (x.sfg.PrgrsprdMarkingPeriodId != null && PrgrsprdMarkingPeriodId.Contains(x.sfg.PrgrsprdMarkingPeriodId))) && x.sfg.IsExamGrade != true) ||
                            (((x.sfg.YrMarkingPeriodId != null && YrMarkingPeriodIdExam.Contains(x.sfg.YrMarkingPeriodId)) || (x.sfg.SmstrMarkingPeriodId != null && SmstrMarkingPeriodIdExam.Contains(x.sfg.SmstrMarkingPeriodId)) || (x.sfg.QtrMarkingPeriodId != null && QtrMarkingPeriodIdExam.Contains(x.sfg.QtrMarkingPeriodId)) || (x.sfg.PrgrsprdMarkingPeriodId != null && PrgrsprdMarkingPeriodIdExam.Contains(x.sfg.PrgrsprdMarkingPeriodId))) && x.sfg.IsExamGrade == true))).ToList();

                if (studentFinalGradeData?.Any() == true)
                {
                    var studentAttendanceMasterData = this.context?.StudentAttendance.Include(x => x.AttendanceCodeNavigation).Where(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId);

                    var studentWiseData = studentFinalGradeData.GroupBy(x => x.sfg.StudentId).ToList();

                    foreach (var student in studentWiseData)
                    {
                        var finalGradeData = student.Select(s => s.sfg).FirstOrDefault();

                        StudentDetailsView studentDetailsView = new StudentDetailsView();

                        studentDetailsView.SchoolId = finalGradeData.StudentMaster.SchoolId;
                        studentDetailsView.StudentId = finalGradeData.StudentMaster.StudentId;
                        studentDetailsView.FirstGivenName = finalGradeData.StudentMaster.FirstGivenName;
                        studentDetailsView.MiddleName = finalGradeData.StudentMaster.MiddleName;
                        studentDetailsView.LastFamilyName = finalGradeData.StudentMaster.LastFamilyName;

                        var courseSectionWiseData = student.GroupBy(x => x.sfg.CourseSectionId).ToList();

                        foreach (var courseSection in courseSectionWiseData)
                        {
                            CourseSectionDetailsView courseSectionDetailsView = new CourseSectionDetailsView();
                            var courseSectionData = courseSection.Select(s => s.cs).FirstOrDefault();

                            courseSectionDetailsView.CourseSectionName = courseSectionData.CourseSectionName;
                            if (studentFinalGradeViewModel.Teacher == true)
                            {
                                courseSectionDetailsView.StaffFirstGivenName = courseSectionData.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.FirstGivenName;
                                courseSectionDetailsView.StaffMiddleName = courseSectionData.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.MiddleName;
                                courseSectionDetailsView.StaffLastFamilyName = courseSectionData.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.LastFamilyName;
                            }
                            if (studentFinalGradeViewModel.Comments == true)
                            {
                                courseSectionDetailsView.Comments = courseSection.First().sfg.TeacherComment;
                            }

                            //fetch markingperiod name & bind in VM where final grade has for this course section.
                            courseSectionDetailsView.markingPeriodDetailsViews = courseSection.Select(s => new MarkingPeriodDetailsView { Percentage = studentFinalGradeViewModel.Parcentage == true ? s.sfg.PercentMarks : 0, Grade = s.sfg.GradeObtained, MarkingPeriodName = s.sfg.IsExamGrade == true ? (s.sfg.ProgressPeriod != null ? s.sfg.ProgressPeriod.Title + " Exam" : s.sfg.Quarters != null ? s.sfg.Quarters.Title + " Exam" : s.sfg.Semesters != null ? s.sfg.Semesters.Title + " Exam" : s.sfg.SchoolYears != null ? s.sfg.SchoolYears.Title + " Exam" : null) : (s.sfg.ProgressPeriod != null ? s.sfg.ProgressPeriod.Title : s.sfg.Quarters != null ? s.sfg.Quarters.Title : s.sfg.Semesters != null ? s.sfg.Semesters.Title : s.sfg.SchoolYears != null ? s.sfg.SchoolYears.Title : null), SortId = s.sfg.ProgressPeriod != null ? "3_" + s.sfg.ProgressPeriod.MarkingPeriodId : s.sfg.Quarters != null ? "2_" + s.sfg.Quarters.MarkingPeriodId : s.sfg.Semesters != null ? "1_" + s.sfg.Semesters.MarkingPeriodId : s.sfg.SchoolYears != null ? "0_" + s.sfg.ProgressPeriod.MarkingPeriodId : null }).ToList();

                            //fetch markingperiod name & bind in VM where final grade has not for this course section.

                            var exceptMarkingPeriod = markingPeriodList.Select(x => new { x.MarkingPeriodName, x.SortId }).Except(courseSectionDetailsView.markingPeriodDetailsViews.Select(x => new { x.MarkingPeriodName, x.SortId })).Select(s => new MarkingPeriodDetailsView { MarkingPeriodName = s.MarkingPeriodName, SortId = s.SortId }).ToList();

                            courseSectionDetailsView.markingPeriodDetailsViews.AddRange(exceptMarkingPeriod);
                            courseSectionDetailsView.markingPeriodDetailsViews = courseSectionDetailsView.markingPeriodDetailsViews.OrderBy(s => s.SortId).ThenBy(s => s.MarkingPeriodName).ToList();
                            //this code for ytd absence
                            if (studentFinalGradeViewModel.YearToDateDailyAbsences == true)
                            {
                                var studentAttendanceData = studentAttendanceMasterData.Where(x => x.TenantId == studentFinalGradeViewModel.TenantId && x.SchoolId == studentFinalGradeViewModel.SchoolId && x.StudentId == student.Key && x.CourseSectionId == courseSection.Key).ToList();

                                if (studentAttendanceData.Count > 0)
                                {
                                    courseSectionDetailsView.AbsYTD = studentAttendanceData.Where(x => x.AttendanceCodeNavigation.StateCode.ToLower() == "absent").Count();
                                }
                            }

                            studentDetailsView.courseSectionDetailsViews.Add(courseSectionDetailsView);
                        }
                        studentFinalGrade.studentDetailsViews.Add(studentDetailsView);
                    }
                }
                else
                {
                    studentFinalGrade._message = NORECORDFOUND;
                    studentFinalGrade._failure = true;
                }
            }
            catch (Exception es)
            {
                studentFinalGrade._message = es.Message;
                studentFinalGrade._failure = true;
            }
            return studentFinalGrade;
        }
    }
}
