using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class PrintScheduleReportViewModel: CommonFields
    {
        public PrintScheduleReportViewModel()
        {
            StudentDetailsViewModelList = new List<StudentDetailsViewModel>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int AcademicYear { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public Guid[]? StudentGuids { get; set; }
        public int[]? CourseSectionIds { get; set; }
        public int? StaffId { get; set; }

        public byte[]? SchoolLogo { get; set; }
        public string? SchoolName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public List<StudentDetailsViewModel> StudentDetailsViewModelList { get; set; }
    }

    public class StudentDetailsViewModel
    {
        public StudentDetailsViewModel()
        {
            CourseDetailsViewModelList = new List<CourseDetailsViewModel>();
        }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public string? AlternateId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public List<CourseDetailsViewModel> CourseDetailsViewModelList { get; set; }

    }
    public class CourseDetailsViewModel
    {
        public CourseDetailsViewModel()
        {
            CourseSectionDetailsViewModelList = new List<CourseSectionDetailsViewModel>();
        }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public List<CourseSectionDetailsViewModel> CourseSectionDetailsViewModelList { get; set; }
    }
    public class CourseSectionDetailsViewModel
    {
        public CourseSectionDetailsViewModel()
        {
            DayDetailsViewModelList = new List<DayDetailsViewModel>();
        }
        public int? CourseSectionId { get; set; }
        public string? CourseSectionName { get; set; }
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
        public List<DayDetailsViewModel> DayDetailsViewModelList { get; set; }
    }
    public class DayDetailsViewModel
    {
        public DayDetailsViewModel()
        {
            DatePeriodRoomDetailsViewModelList = new List<DatePeriodRoomDetailsViewModel>();
        }
        public string? DayName { get; set; }
        public List<DatePeriodRoomDetailsViewModel> DatePeriodRoomDetailsViewModelList { get; set; }
    }
    public class DatePeriodRoomDetailsViewModel
    {
        public DateTime? Date { get; set; }
        public int? PeriodId { get; set; }
        public string? PeriodName { get; set; }
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
    }

}
