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

                var honorRollData = this.context?.HonorRolls.Where(h => h.SchoolId == pageResult.SchoolId && h.TenantId == pageResult.TenantId);

                if (studentDatas.Any() == true)
                {
                    foreach (var gradedstudentDatas in studentDatas)
                    {
                        var honorData = new HonorRollViewForReport();

                        //var studentData = gradedstudentDatas.StudentMaster.Where(e => e.SchoolId == pageResult.SchoolId && e.TenantId == pageResult.TenantId && e.StudentId == gradedstudentDatas.StudentId).FirstOrDefault();

                        if (gradedstudentDatas.StudentMaster != null)
                        {
                            honorData.Salutation = gradedstudentDatas.StudentMaster.Salutation;
                            honorData.FirstGivenName = gradedstudentDatas.StudentMaster.FirstGivenName;
                            honorData.MiddleName = gradedstudentDatas.StudentMaster.MiddleName;
                            honorData.LastFamilyName = gradedstudentDatas.StudentMaster.LastFamilyName;
                            honorData.StudentId = gradedstudentDatas.StudentMaster.StudentId;
                            honorData.AlternateId = gradedstudentDatas.StudentMaster.AlternateId;
                            honorData.MobilePhone = gradedstudentDatas.StudentMaster.MobilePhone;
                            honorData.StudentGuid = gradedstudentDatas.StudentMaster.StudentGuid;
                            honorData.StudentInternalId = gradedstudentDatas.StudentMaster.StudentInternalId;
                            honorData.DistrictId = gradedstudentDatas.StudentMaster.DistrictId;
                            honorData.StateId = gradedstudentDatas.StudentMaster.StateId;
                            honorData.AdmissionNumber = gradedstudentDatas.StudentMaster.AdmissionNumber;
                            honorData.RollNumber = gradedstudentDatas.StudentMaster.RollNumber;
                            honorData.Suffix = gradedstudentDatas.StudentMaster.Suffix;
                            honorData.PreferredName = gradedstudentDatas.StudentMaster.PreferredName;
                            honorData.PreviousName = gradedstudentDatas.StudentMaster.PreviousName;
                            honorData.SocialSecurityNumber = gradedstudentDatas.StudentMaster.SocialSecurityNumber;
                            honorData.OtherGovtIssuedNumber = gradedstudentDatas.StudentMaster.OtherGovtIssuedNumber;
                            honorData.StudentPhoto = gradedstudentDatas.StudentMaster.StudentPhoto;
                            honorData.Dob = gradedstudentDatas.StudentMaster.Dob;
                            honorData.StudentPortalId = gradedstudentDatas.StudentMaster.StudentPortalId;
                            honorData.Gender = gradedstudentDatas.StudentMaster.Gender;
                            honorData.Race = gradedstudentDatas.StudentMaster.Race;
                            honorData.MaritalStatus = gradedstudentDatas.StudentMaster.MaritalStatus;
                            honorData.Ethnicity = gradedstudentDatas.StudentMaster.Ethnicity;
                            honorData.CountryOfBirth = gradedstudentDatas.StudentMaster.CountryOfBirth;
                            honorData.Nationality = gradedstudentDatas.StudentMaster.Nationality;
                            honorData.FirstLanguageId = gradedstudentDatas.StudentMaster.FirstLanguageId;
                            honorData.SecondLanguageId = gradedstudentDatas.StudentMaster.SecondLanguageId;
                            honorData.ThirdLanguageId = gradedstudentDatas.StudentMaster.ThirdLanguageId;
                            honorData.EstimatedGradDate = gradedstudentDatas.StudentMaster.EstimatedGradDate;
                            honorData.Eligibility504 = gradedstudentDatas.StudentMaster.Eligibility504;
                            honorData.EconomicDisadvantage = gradedstudentDatas.StudentMaster.EconomicDisadvantage;
                            honorData.FreeLunchEligibility = gradedstudentDatas.StudentMaster.FreeLunchEligibility;
                            honorData.SpecialEducationIndicator = gradedstudentDatas.StudentMaster.SpecialEducationIndicator;
                            honorData.LepIndicator = gradedstudentDatas.StudentMaster.LepIndicator;
                            honorData.HomePhone = gradedstudentDatas.StudentMaster.HomePhone;
                            honorData.PersonalEmail = gradedstudentDatas.StudentMaster.PersonalEmail;
                            honorData.SchoolEmail = gradedstudentDatas.StudentMaster.SchoolEmail;
                            honorData.Twitter = gradedstudentDatas.StudentMaster.Twitter;
                            honorData.Facebook = gradedstudentDatas.StudentMaster.Facebook;
                            honorData.Instagram = gradedstudentDatas.StudentMaster.Instagram;
                            honorData.Youtube = gradedstudentDatas.StudentMaster.Youtube;
                            honorData.Linkedin = gradedstudentDatas.StudentMaster.Linkedin;
                            honorData.HomeAddressLineOne = gradedstudentDatas.StudentMaster.HomeAddressLineOne;
                            honorData.HomeAddressLineTwo = gradedstudentDatas.StudentMaster.HomeAddressLineTwo;
                            honorData.HomeAddressCity = gradedstudentDatas.StudentMaster.HomeAddressCity;
                            honorData.HomeAddressState = gradedstudentDatas.StudentMaster.HomeAddressState;
                            honorData.HomeAddressCountry = gradedstudentDatas.StudentMaster.HomeAddressCountry;
                            honorData.HomeAddressZip = gradedstudentDatas.StudentMaster.HomeAddressZip;
                            honorData.BusNo = gradedstudentDatas.StudentMaster.BusNo;
                            honorData.SchoolBusPickUp = gradedstudentDatas.StudentMaster.SchoolBusPickUp;
                            honorData.SchoolBusDropOff = gradedstudentDatas.StudentMaster.SchoolBusDropOff;
                            honorData.MailingAddressSameToHome = gradedstudentDatas.StudentMaster.MailingAddressSameToHome;
                            honorData.MailingAddressLineOne = gradedstudentDatas.StudentMaster.MailingAddressLineOne;
                            honorData.MailingAddressLineTwo = gradedstudentDatas.StudentMaster.MailingAddressLineTwo;
                            honorData.MailingAddressCity = gradedstudentDatas.StudentMaster.MailingAddressCity;
                            honorData.MailingAddressCountry = gradedstudentDatas.StudentMaster.MailingAddressCountry;
                            honorData.MailingAddressZip = gradedstudentDatas.StudentMaster.MailingAddressZip;
                            honorData.CriticalAlert = gradedstudentDatas.StudentMaster.CriticalAlert;
                            honorData.AlertDescription = gradedstudentDatas.StudentMaster.AlertDescription;
                            honorData.PrimaryCarePhysician = gradedstudentDatas.StudentMaster.PrimaryCarePhysician;
                            honorData.PrimaryCarePhysicianPhone = gradedstudentDatas.StudentMaster.PrimaryCarePhysicianPhone;
                            honorData.MedicalFacility = gradedstudentDatas.StudentMaster.MedicalFacility;
                            honorData.MedicalFacilityPhone = gradedstudentDatas.StudentMaster.MedicalFacilityPhone;
                            honorData.InsuranceCompany = gradedstudentDatas.StudentMaster.InsuranceCompany;
                            honorData.InsuranceCompanyPhone = gradedstudentDatas.StudentMaster.InsuranceCompanyPhone;
                            honorData.PolicyNumber = gradedstudentDatas.StudentMaster.PolicyNumber;
                            honorData.PolicyHolder = gradedstudentDatas.StudentMaster.PolicyHolder;
                            honorData.Dentist = gradedstudentDatas.StudentMaster.Dentist;
                            honorData.DentistPhone = gradedstudentDatas.StudentMaster.DentistPhone;
                            honorData.Vision = gradedstudentDatas.StudentMaster.Vision;
                            honorData.VisionPhone = gradedstudentDatas.StudentMaster.VisionPhone;

                        }
                        honorData.GradeName = gradedstudentDatas.StudentMaster.StudentEnrollment.FirstOrDefault(x => x.IsActive == true)?.GradeLevelTitle;

                        var studentsection = this.context?.Sections.Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId && s.SectionId == gradedstudentDatas.StudentMaster.SectionId).FirstOrDefault();

                        if (studentsection != null)
                        {
                            honorData.SectionId = studentsection.SectionId;
                            honorData.SectionName = studentsection.Name;
                        }
                        var honor = honorRollData.Where(h => h.Breakoff <= gradedstudentDatas.PercentMarks).FirstOrDefault();

                        if (honor != null)
                        {
                            honorData.HonorRoll = honor.HonorRoll;
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
                        honorRollList.HonorRollViewForReports = new();
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
