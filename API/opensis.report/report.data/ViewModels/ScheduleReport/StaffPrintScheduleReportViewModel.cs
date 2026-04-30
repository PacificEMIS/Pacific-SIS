using opensis.data.ViewModels;
using System;
using System.Collections.Generic;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class StaffPrintScheduleReportViewModel : CommonFields
    {
        public StaffPrintScheduleReportViewModel()
        {
            StaffDetails = new StaffDetailsViewModel();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int AcademicYear { get; set; }
        public int StaffId { get; set; }
        public int[]? CourseSectionIds { get; set; }

        public byte[]? SchoolLogo { get; set; }
        public string? SchoolName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public StaffDetailsViewModel StaffDetails { get; set; }
    }

    public class StaffDetailsViewModel
    {
        public StaffDetailsViewModel()
        {
            CourseDetailsViewModelList = new List<CourseDetailsViewModel>();
        }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public byte[]? StaffPhoto { get; set; }
        public string? StaffInternalId { get; set; }
        public string? JobTitle { get; set; }
        public string? Profile { get; set; }
        public string? PersonalEmail { get; set; }
        public string? MobilePhone { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public List<CourseDetailsViewModel> CourseDetailsViewModelList { get; set; }
    }
}