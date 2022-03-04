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


                var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (progressPeriodsData != null)
                {
                    studentDatas = this.context?.StudentFinalGrade.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.PrgrsprdMarkingPeriodId == progressPeriodsData.MarkingPeriodId && e.IsExamGrade != true).ToList();

                    //markingPeriodId = progressPeriodsData.MarkingPeriodId;

                }
                var quartersData = this.context?.Quarters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (quartersData != null)
                {
                    studentDatas = this.context?.StudentFinalGrade.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.QtrMarkingPeriodId == quartersData.MarkingPeriodId && e.IsExamGrade != true).ToList();

                    //markingPeriodId = quartersData.MarkingPeriodId;
                }
                var semestersData = this.context?.Semesters.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (semestersData != null)
                {
                    studentDatas = this.context?.StudentFinalGrade.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.SmstrMarkingPeriodId == semestersData.MarkingPeriodId && e.IsExamGrade != true).ToList();
                    //markingPeriodId = semestersData.MarkingPeriodId;
                }
                var yearsData = this.context?.SchoolYears.Where(x => x.SchoolId == pageResult.SchoolId && x.TenantId == pageResult.TenantId && x.StartDate == pageResult.MarkingPeriodStartDate && x.EndDate == pageResult.MarkingPeriodEndDate && x.AcademicYear == pageResult.AcademicYear).FirstOrDefault();

                if (yearsData != null)
                {
                    studentDatas = this.context?.StudentFinalGrade.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.YrMarkingPeriodId == yearsData.MarkingPeriodId && e.IsExamGrade != true).ToList();
                    //markingPeriodId = yearsData.MarkingPeriodId;
                }

                if (studentDatas.Any() == true)
                {
                    foreach (var gradedstudentDatas in studentDatas)
                    {
                        var honorData = new HonorRollViewForReport();

                        var studentData = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.StudentId == gradedstudentDatas.StudentId).FirstOrDefault();

                        if (studentData != null)
                        {
                            honorData.Salutation = studentData.Salutation;
                            honorData.FirstGivenName = studentData.FirstGivenName;
                            honorData.MiddleName = studentData.MiddleName;
                            honorData.LastFamilyName = studentData.LastFamilyName;
                            honorData.StudentId = studentData.StudentId;
                            honorData.AlternateId = studentData.AlternateId;
                            honorData.MobilePhone = studentData.MobilePhone;
                            honorData.StudentGuid = studentData.StudentGuid;
                            honorData.StudentInternalId = studentData.StudentInternalId;
                            honorData.DistrictId = studentData.DistrictId;
                            honorData.StateId = studentData.StateId;
                            honorData.AdmissionNumber = studentData.AdmissionNumber;
                            honorData.RollNumber = studentData.RollNumber;
                            honorData.Suffix = studentData.Suffix;
                            honorData.PreferredName = studentData.PreferredName;
                            honorData.PreviousName = studentData.PreviousName;
                            honorData.SocialSecurityNumber = studentData.SocialSecurityNumber;
                            honorData.OtherGovtIssuedNumber = studentData.OtherGovtIssuedNumber;
                            honorData.StudentPhoto = studentData.StudentPhoto;
                            honorData.Dob = studentData.Dob;
                            honorData.StudentPortalId = studentData.StudentPortalId;
                            honorData.Gender = studentData.Gender;
                            honorData.Race = studentData.Race;
                            honorData.MaritalStatus = studentData.MaritalStatus;
                            honorData.Ethnicity = studentData.Ethnicity;
                            honorData.CountryOfBirth = studentData.CountryOfBirth;
                            honorData.Nationality = studentData.Nationality;
                            honorData.FirstLanguageId = studentData.FirstLanguageId;
                            honorData.SecondLanguageId = studentData.SecondLanguageId;
                            honorData.ThirdLanguageId = studentData.ThirdLanguageId;
                            honorData.EstimatedGradDate = studentData.EstimatedGradDate;
                            honorData.Eligibility504 = studentData.Eligibility504;
                            honorData.EconomicDisadvantage = studentData.EconomicDisadvantage;
                            honorData.FreeLunchEligibility = studentData.FreeLunchEligibility;
                            honorData.SpecialEducationIndicator = studentData.SpecialEducationIndicator;
                            honorData.LepIndicator = studentData.LepIndicator;
                            honorData.HomePhone = studentData.HomePhone;
                            honorData.PersonalEmail = studentData.PersonalEmail;
                            honorData.SchoolEmail = studentData.SchoolEmail;
                            honorData.Twitter = studentData.Twitter;
                            honorData.Facebook = studentData.Facebook;
                            honorData.Instagram = studentData.Instagram;
                            honorData.Youtube = studentData.Youtube;
                            honorData.Linkedin = studentData.Linkedin;
                            honorData.HomeAddressLineOne = studentData.HomeAddressLineOne;
                            honorData.HomeAddressLineTwo = studentData.HomeAddressLineTwo;
                            honorData.HomeAddressCity = studentData.HomeAddressCity;
                            honorData.HomeAddressState = studentData.HomeAddressState;
                            honorData.HomeAddressCountry = studentData.HomeAddressCountry;
                            honorData.HomeAddressZip = studentData.HomeAddressZip;
                            honorData.BusNo = studentData.BusNo;
                            honorData.SchoolBusPickUp = studentData.SchoolBusPickUp;
                            honorData.SchoolBusDropOff = studentData.SchoolBusDropOff;
                            honorData.MailingAddressSameToHome = studentData.MailingAddressSameToHome;
                            honorData.MailingAddressLineOne = studentData.MailingAddressLineOne;
                            honorData.MailingAddressLineTwo = studentData.MailingAddressLineTwo;
                            honorData.MailingAddressCity = studentData.MailingAddressCity;
                            honorData.MailingAddressCountry = studentData.MailingAddressCountry;
                            honorData.MailingAddressZip = studentData.MailingAddressZip;
                            honorData.CriticalAlert = studentData.CriticalAlert;
                            honorData.AlertDescription = studentData.AlertDescription;
                            honorData.PrimaryCarePhysician = studentData.PrimaryCarePhysician;
                            honorData.PrimaryCarePhysicianPhone = studentData.PrimaryCarePhysicianPhone;
                            honorData.MedicalFacility = studentData.MedicalFacility;
                            honorData.MedicalFacilityPhone = studentData.MedicalFacilityPhone;
                            honorData.InsuranceCompany = studentData.InsuranceCompany;
                            honorData.InsuranceCompanyPhone = studentData.InsuranceCompanyPhone;
                            honorData.PolicyNumber = studentData.PolicyNumber;
                            honorData.PolicyHolder = studentData.PolicyHolder;
                            honorData.Dentist = studentData.Dentist;
                            honorData.DentistPhone = studentData.DentistPhone;
                            honorData.Vision = studentData.Vision;
                            honorData.VisionPhone = studentData.VisionPhone;

                        }
                        honorData.GradeName = studentData.StudentEnrollment.FirstOrDefault(x => x.IsActive == true)?.GradeLevelTitle;

                        var studentsection = this.context?.Sections.Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.SectionId == studentData.SectionId).FirstOrDefault();
                        if (studentsection != null)
                        {
                            honorData.SectionId = studentsection.SectionId;
                            honorData.SectionName = studentsection.Name;
                        }
                        var honorRoll = this.context?.HonorRolls.Where(h => h.SchoolId == pageResult.SchoolId && h.TenantId == pageResult.TenantId && h.Breakoff <= gradedstudentDatas.PercentMarks).FirstOrDefault();

                        if (honorRoll != null)
                        {
                            honorData.HonorRoll = honorRoll.HonorRoll;
                        }
                        honorRollList.HonorRollViewForReports.Add(honorData);
                    }

                    var StudentHonorRollData = honorRollList.HonorRollViewForReports.AsQueryable();
                    //Filteration ---------//
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
                    }

                }

                else
                {
                    honorRollList._failure = true;
                    honorRollList._message = NORECORDFOUND;
                }

                honorRollList.SchoolId = pageResult.SchoolId;
                honorRollList.AcademicYear = pageResult.AcademicYear;
                honorRollList.TenantId = pageResult.TenantId;
                honorRollList._tenantName = pageResult._tenantName;
                honorRollList._userName = pageResult._userName;
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
