using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class AddDropScheduleReport: CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public string? SchoolName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public byte[]? SchoolLogo { get; set; }
        public List<StudentScheduleData>? studentCoursesectionScheduleList { get; set; }
    }

    public class StudentScheduleData 
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public Guid StudentGuid { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? StudentName { get; set; }
        public string? StaffName { get; set; }
        public string? CourseName { get; set; }
        public string? CourseSectionName { get; set; }
        public DateTime? EnrolledDate { get; set; }
        public DateTime? DropDate { get; set; }
    }
}
