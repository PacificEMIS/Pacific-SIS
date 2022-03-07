using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.StudentReport
{
    public class StudentProgressReport : CommonFields
    {
        public StudentProgressReport()
        {
            SchoolMasterListData = new List<SchoolMasterListData>();
        }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public Guid[]? StudentGuids { get; set; }
        public string? MarkingPeriodTitle { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public decimal? AcademicYear { get; set; }
        public bool? TotalsOnly { get; set; }
        public List<SchoolMasterListData>? SchoolMasterListData { get; set; }
    }

    public class SchoolMasterListData
    {
        public SchoolMasterListData()
        {
            StudentMasterListData = new List<StudentMasterListData>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public Guid SchoolGuid { get; set; }
        public byte[]? SchoolLogo { get; set; }
        public string? SchoolStateId { get; set; }
        public string? SchoolDistrictId { get; set; }
        public string? SchoolName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? Division { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public List<StudentMasterListData>? StudentMasterListData { get; set; }
    }
    public class StudentMasterListData
    {
        public StudentMasterListData()
        {
            CourseSectionListData = new List<CourseSectionListData>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public Guid StudentGuid { get; set; }
        public int? StudentId { get; set; }
        public string? AlternateId { get; set; }
        public string? StudentInternalId { get; set; }
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
        public string? GradeLevelTitle { get; set; }
        public DateTime? Dob { get; set; }
        public int? Age { get; set; }
        public string? StudentPortalId { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public int? SectionId { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? MarkingPeriodTitle { get; set; }
        public List<CourseSectionListData>? CourseSectionListData { get; set; }
    }

    public class CourseSectionListData
    {
        public CourseSectionListData()
        {
            GradeBookGradeListData = new List<GradeBookGradeListData>();
            GradeData = new List<GradeData>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public decimal? AcademicYear { get; set; }
        public int? GradeScaleId { get; set; }
        /// <summary>
        /// &apos;Ungraded&apos;,&apos;Numeric&apos;,&apos;School_Scale&apos;,&apos;Teacher_Scale&apos;
        /// </summary>
        public string? GradeScaleType { get; set; }
        public string? CourseSectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherMiddleName { get; set; }
        public string? TeacherLastName { get; set; }
        public string? Total { get; set; }
        public string? TotalWeightedGrade { get; set; }
        public List<GradeData>? GradeData { get; set; }
        public List<GradeBookGradeListData>? GradeBookGradeListData { get; set; }
    }

    public class GradeBookGradeListData
    {
        public string? AssignmentTypeTitle { get; set; }
        public int? Weight { get; set; }
        public string? AssignmentTitle { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Points { get; set; }
        public string? AllowedMarks { get; set; }
        public int? AssignmentPoint { get; set; }
        public string? Grade { get; set; }
        public string? Comment { get; set; }
        public string? WieghtedGrade { get; set; }
    }

    public class GradeData
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int GradeScaleId { get; set; }
        public int GradeId { get; set; }
        public string? Title { get; set; }
        public int? Breakoff { get; set; }
        public decimal? WeightedGpValue { get; set; }
        public decimal? UnweightedGpValue { get; set; }
        public string? Comment { get; set; }
    }
}