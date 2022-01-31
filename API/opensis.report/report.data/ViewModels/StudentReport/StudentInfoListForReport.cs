using opensis.data.Models;
using opensis.data.ViewModels;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.StudentReport
{
    public class StudentInfoListForReport : CommonFields
    {
        public List<SchoolMasterData>? SchoolMasterData { get; set; }
    }


    public class SchoolMasterData
    {
        public List<StudentMasterData>? StudentMasterData { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public Guid SchoolGuid { get; set; }
        public string? SchoolInternalId { get; set; }
        public string? SchoolAltId { get; set; }
        public byte[]? SchoolLogo { get; set; }
        public string? SchoolStateId { get; set; }
        public string? SchoolDistrictId { get; set; }
        public string? SchoolLevel { get; set; }
        public string? SchoolClassification { get; set; }
        public string? SchoolName { get; set; }
        public string? AlternateName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Division { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    public class StudentMasterData
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public Guid StudentGuid { get; set; }
        public int? StudentId { get; set; }
        public string? AlternateId { get; set; }
        public string? DistrictId { get; set; }
        public string? StateId { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? RollNumber { get; set; }
        public string? Salutation { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? Suffix { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public string? PreferredName { get; set; }
        public string? PreviousName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? OtherGovtIssuedNumber { get; set; }
        public string? GradeLevelTitle { get; set; }
        public DateTime? Dob { get; set; }
        public int? Age { get; set; }
        public string? StudentPortalId { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? MaritalStatus { get; set; }
        public int? CountryOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? FirstLanguage { get; set; }
        public string? SecondLanguageId { get; set; }
        public string? ThirdLanguageId { get; set; }
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
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? BusNo { get; set; }
        public bool? SchoolBusPickUp { get; set; }
        public bool? SchoolBusDropOff { get; set; }
        public bool? MailingAddressSameToHome { get; set; }
        public string? MailingAddressLineOne { get; set; }
        public string? MailingAddressLineTwo { get; set; }
        public string? MailingAddressCountry { get; set; }
        public string? MailingAddressCity { get; set; }
        public string? MailingAddressState { get; set; }
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
        public string? Associationship { get; set; }
        public string? EnrollmentType { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }

        public List<StudentEnrollmentListForView>? StudentEnrollment { get; set; }
       
        public List<StudentMedicalAlert>? StudentMedicalAlert { get; set; }
        public List<StudentMedicalImmunization>? StudentMedicalImmunization { get; set; }
        public List<StudentMedicalNote>? StudentMedicalNote { get; set; }
        public List<StudentMedicalNurseVisit>? StudentMedicalNurseVisit { get; set; }
        public List<StudentMedicalProvider>? StudentMedicalProvider { get; set; }
        public ParentInfoListModel? Contacts { get; set; }
    }
}
