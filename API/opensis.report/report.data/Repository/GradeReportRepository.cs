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

                var honorRollData = this.context?.HonorRolls.Where(h => h.SchoolId == pageResult.SchoolId && h.TenantId == pageResult.TenantId).ToList();

                var studentsection = this.context?.Sections.Where(s => s.SchoolId == pageResult.SchoolId && s.TenantId == pageResult.TenantId).ToList();

                if (studentDatas.Any() == true)
                {
                    var StudentHonorRollData = studentDatas.Select(x => new HonorRollViewForReport
                    {
                        FirstGivenName = x.StudentMaster.FirstGivenName,
                        MiddleName = x.StudentMaster.MiddleName,
                        LastFamilyName = x.StudentMaster.LastFamilyName,
                        StudentInternalId = x.StudentMaster.StudentInternalId,
                        AlternateId = x.StudentMaster.AlternateId,
                        MobilePhone = x.StudentMaster.MobilePhone,
                        StudentGuid = x.StudentMaster.StudentGuid,
                        DistrictId = x.StudentMaster.DistrictId,
                        StateId = x.StudentMaster.StateId,
                        AdmissionNumber = x.StudentMaster.AdmissionNumber,
                        RollNumber = x.StudentMaster.RollNumber,
                        Suffix = x.StudentMaster.Suffix,
                        PreferredName = x.StudentMaster.PreferredName,
                        PreviousName = x.StudentMaster.PreviousName,
                        SocialSecurityNumber = x.StudentMaster.SocialSecurityNumber,
                        OtherGovtIssuedNumber = x.StudentMaster.OtherGovtIssuedNumber,
                        //StudentPhoto = x.StudentMaster.StudentPhoto,
                        Dob = x.StudentMaster.Dob,
                        StudentPortalId = x.StudentMaster.StudentPortalId,
                        Gender = x.StudentMaster.Gender,
                        Race = x.StudentMaster.Race,
                        MaritalStatus = x.StudentMaster.MaritalStatus,
                        Ethnicity = x.StudentMaster.Ethnicity,
                        CountryOfBirth = x.StudentMaster.CountryOfBirth,
                        Nationality = x.StudentMaster.Nationality,
                        FirstLanguageId = x.StudentMaster.FirstLanguageId,
                        SecondLanguageId = x.StudentMaster.SecondLanguageId,
                        ThirdLanguageId = x.StudentMaster.ThirdLanguageId,
                        EstimatedGradDate = x.StudentMaster.EstimatedGradDate,
                        Eligibility504 = x.StudentMaster.Eligibility504,
                        EconomicDisadvantage = x.StudentMaster.EconomicDisadvantage,
                        FreeLunchEligibility = x.StudentMaster.FreeLunchEligibility,
                        SpecialEducationIndicator = x.StudentMaster.SpecialEducationIndicator,
                        LepIndicator = x.StudentMaster.LepIndicator,
                        HomePhone = x.StudentMaster.HomePhone,
                        PersonalEmail = x.StudentMaster.PersonalEmail,
                        SchoolEmail = x.StudentMaster.SchoolEmail,
                        Twitter = x.StudentMaster.Twitter,
                        Facebook = x.StudentMaster.Facebook,
                        Instagram = x.StudentMaster.Instagram,
                        Youtube = x.StudentMaster.Youtube,
                        Linkedin = x.StudentMaster.Linkedin,
                        HomeAddressLineOne = x.StudentMaster.HomeAddressLineOne,
                        HomeAddressLineTwo = x.StudentMaster.HomeAddressLineTwo,
                        HomeAddressCity = x.StudentMaster.HomeAddressCity,
                        HomeAddressState = x.StudentMaster.HomeAddressState,
                        HomeAddressCountry = x.StudentMaster.HomeAddressCountry,
                        HomeAddressZip = x.StudentMaster.HomeAddressZip,
                        BusNo = x.StudentMaster.BusNo,
                        SchoolBusPickUp = x.StudentMaster.SchoolBusPickUp,
                        SchoolBusDropOff = x.StudentMaster.SchoolBusDropOff,
                        MailingAddressSameToHome = x.StudentMaster.MailingAddressSameToHome,
                        MailingAddressLineOne = x.StudentMaster.MailingAddressLineOne,
                        MailingAddressLineTwo = x.StudentMaster.MailingAddressLineTwo,
                        MailingAddressCity = x.StudentMaster.MailingAddressCity,
                        MailingAddressCountry = x.StudentMaster.MailingAddressCountry,
                        MailingAddressZip = x.StudentMaster.MailingAddressZip,
                        CriticalAlert = x.StudentMaster.CriticalAlert,
                        AlertDescription = x.StudentMaster.AlertDescription,
                        PrimaryCarePhysician = x.StudentMaster.PrimaryCarePhysician,
                        PrimaryCarePhysicianPhone = x.StudentMaster.PrimaryCarePhysicianPhone,
                        MedicalFacility = x.StudentMaster.MedicalFacility,
                        MedicalFacilityPhone = x.StudentMaster.MedicalFacilityPhone,
                        InsuranceCompany = x.StudentMaster.InsuranceCompany,
                        InsuranceCompanyPhone = x.StudentMaster.InsuranceCompanyPhone,
                        PolicyNumber = x.StudentMaster.PolicyNumber,
                        PolicyHolder = x.StudentMaster.PolicyHolder,
                        Dentist = x.StudentMaster.Dentist,
                        DentistPhone = x.StudentMaster.DentistPhone,
                        Vision = x.StudentMaster.Vision,
                        IsActive = x.StudentMaster.IsActive,
                        VisionPhone = x.StudentMaster.VisionPhone,
                        GradeName = x.StudentMaster.StudentEnrollment.FirstOrDefault(x => x.IsActive == true)?.GradeLevelTitle,
                        SectionName = studentsection.FirstOrDefault(y => y.SectionId == x.StudentMaster.SectionId)?.Name,
                        HonorRoll = honorRollData.FirstOrDefault(h => h.Breakoff <= x.PercentMarks)?.HonorRoll

                    }).AsQueryable();

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
