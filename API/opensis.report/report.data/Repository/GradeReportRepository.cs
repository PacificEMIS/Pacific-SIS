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
    }
}
