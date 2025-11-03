using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.GradeReport
{
    public class StudentCgpaViewModel : CommonFields
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<StudentCgpaDetails> studentCgpaDetails { get; set; }=new List<StudentCgpaDetails>();
    }

    public class StudentCgpaDetails
    {
        public int StudentId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? AlternateId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? PreferredName { get; set; }
        public string? GradeLevelTitle { get; set; }
        public int? GradeId { get; set; }
        public string? SectionName { get; set; }
        public string? SchoolEmail { get; set; }
        public string? HomePhone { get; set; }
        public int? Rank { get; set; }
        public decimal? CumulativeGPA { get; set; }
        public decimal? TotalCreditAttempeted { get; set; }
        public decimal? TotalCreditEarned { get; set; }

    }

}
