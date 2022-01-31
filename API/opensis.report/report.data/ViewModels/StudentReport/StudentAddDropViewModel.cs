using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.StudentReport
{
    public class StudentAddDropViewModel : CommonFields
    {
        public StudentAddDropViewModel()
        {
            studentEnrollmentList = new List<StudentEnrollment>();
        }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? GradeLevel { get; set; }
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
        public List<StudentEnrollment> studentEnrollmentList { get; set; }
    }
}
