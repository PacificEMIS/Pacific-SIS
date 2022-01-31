using opensis.data.Models;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.ApiAccess
{
    public class ApiAccessStudentListViewModel : APICommonModel
    {
        public ApiAccessStudentListViewModel()
        {
            ApiAccessStudentList = new List<ApiStudentMaster>();
        }
        public List<ApiStudentMaster> ApiAccessStudentList { get; set; }
    }

    public class ApiStudentMaster
    {

        public ApiStudentMaster()
        {
            StudentEnrollment = new HashSet<ApiStudentEnrollment>();
           // StudentComments = new HashSet<StudentComments>();
           // StudentDocuments = new HashSet<StudentDocuments>();
            StudentMedicalAlert = new HashSet<StudentMedicalAlert>();
            StudentMedicalImmunization = new HashSet<StudentMedicalImmunization>();
            StudentMedicalNote = new HashSet<StudentMedicalNote>();
            StudentMedicalNurseVisit = new HashSet<StudentMedicalNurseVisit>();
            StudentMedicalProvider = new HashSet<StudentMedicalProvider>();
            Contacts = new ParentInfoListModel();
            Siblings = new StudentListModel();
        }
        public Guid StudentGuid { get; set; }
        public string? StudentId { get; set; }
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
        public string? PreferredName { get; set; }
        public string? PreviousName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? OtherGovtIssuedNumber { get; set; }
        public DateTime? Dob { get; set; }
        public int? Age { get; set; }
        public string? StudentPortalId { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? MaritalStatus { get; set; }
        public string? CountryOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? FirstLanguage { get; set; }
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

        public virtual ICollection<ApiStudentEnrollment> StudentEnrollment { get; set; }
        //public virtual ICollection<StudentComments> StudentComments { get; set; }
      //  public virtual ICollection<StudentDocuments> StudentDocuments { get; set; }
        public virtual ICollection<StudentMedicalAlert> StudentMedicalAlert { get; set; }
        public virtual ICollection<StudentMedicalImmunization> StudentMedicalImmunization { get; set; }
        public virtual ICollection<StudentMedicalNote> StudentMedicalNote { get; set; }
        public virtual ICollection<StudentMedicalNurseVisit> StudentMedicalNurseVisit { get; set; }
        public virtual ICollection<StudentMedicalProvider> StudentMedicalProvider { get; set; }
        public ParentInfoListModel Contacts { get; set; }
        public StudentListModel Siblings { get; set; }
    }

    public class ApiStudentSibling
    {
        public string? FirstGivenName { get; set; }
        public string? LastFamilyName { get; set; }
        public DateTime? Dob { get; set; }
        public int? StudentId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? SchoolName { get; set; }
        public string? Address { get; set; }
    }

    public class ApiStudentEnrollment
    {
        public int EnrollmentId { get; set; }
        public int? CalenderId { get; set; }
        public string? RollingOption { get; set; }
        public string? SchoolName { get; set; }
        public int? GradeId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string? EnrollmentCode { get; set; }
        public DateTime? ExitDate { get; set; }
        public string? ExitCode { get; set; }
        public int? TransferredSchoolId { get; set; }
        public string? SchoolTransferred { get; set; }
        public string? TransferredGrade { get; set; }
    }
}
