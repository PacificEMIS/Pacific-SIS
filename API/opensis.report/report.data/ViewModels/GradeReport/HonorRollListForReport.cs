using opensis.data.Models;
using opensis.data.ViewModels;
using opensis.data.ViewModels.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.GradeReport
{
    public class HonorRollListForReport : CommonFields
    {
        public HonorRollListForReport()
        {
            HonorRollViewForReports = new List<HonorRollViewForReport>();
        }
        public List<HonorRollViewForReport>? HonorRollViewForReports { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public decimal? AcademicYear { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public int? MarkingPeriodId { get; set; }
    }
    public class HonorRollViewForReport
    {
        public string? RollNumber { get; set; }
        public string? Salutation { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? Suffix { get; set; }
        public string? PreferredName { get; set; }
        public string? PreviousName { get; set; }
        public String? GradeName { get; set; }
        public String? SectionName { get; set; }
        public String? HonorRoll { get; set; }
        public int? StudentId { get; set; }
        public Guid? StudentGuid { get; set; }
        public string? StudentInternalId { get; set; }
        public string? AlternateId { get; set; }
        public string? DistrictId { get; set; }
        public string? StateId { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? OtherGovtIssuedNumber { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public DateTime? Dob { get; set; }
        public string? StudentPortalId { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? MaritalStatus { get; set; }
        public int? CountryOfBirth { get; set; }
        public int? Nationality { get; set; }
        public int? FirstLanguageId { get; set; }
        public int? SecondLanguageId { get; set; }
        public int? ThirdLanguageId { get; set; }
        public int? SectionId { get; set; }
        public DateTime? EstimatedGradDate { get; set; }
        public bool? Eligibility504 { get; set; }
        public bool? EconomicDisadvantage { get; set; }
        public bool? FreeLunchEligibility { get; set; }
        public bool? SpecialEducationIndicator { get; set; }
        public bool? LepIndicator { get; set; }
        public string? HomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? SchoolEmail { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? Linkedin { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? BusNo { get; set; }
        public bool? SchoolBusPickUp { get; set; }
        public bool? SchoolBusDropOff { get; set; }
        public bool? MailingAddressSameToHome { get; set; }
        public string? MailingAddressLineOne { get; set; }
        public string? MailingAddressLineTwo { get; set; }
        public string? MailingAddressCity { get; set; }
        public string? MailingAddressState { get; set; }
        public string? MailingAddressCountry { get; set; }
        public string? MailingAddressZip { get; set; }
        public string? CriticalAlert { get; set; }
        public string? AlertDescription { get; set; }
        public string? PrimaryCarePhysician { get; set; }
        public string? PrimaryCarePhysicianPhone { get; set; }
        public string? MedicalFacility { get; set; }
        public string? MedicalFacilityPhone { get; set; }
        public string? InsuranceCompany { get; set; }
        public string? InsuranceCompanyPhone { get; set; }
        public string? PolicyNumber { get; set; }
        public string? PolicyHolder { get; set; }
        public string? Dentist { get; set; }
        public string? DentistPhone { get; set; }
        public string? Vision { get; set; }
        public string? VisionPhone { get; set; }
    }
}
