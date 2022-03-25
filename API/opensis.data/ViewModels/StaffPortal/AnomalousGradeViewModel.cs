using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StaffPortal
{
    public class AnomalousGradeViewModel : CommonFields
    {
        public AnomalousGradeViewModel()
        {
            studentAnomalsGrades = new List<StudentAnomalsGrade>();
        }

        public Guid? TenantId { get; set; }
        public int? StaffId { get; set; }
        public int? SchoolId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? AssignmentTypeId { get; set; }
        public int? AssignmentId { get; set; }
        public decimal? AcademicYear { get; set; }
        public string? SearchValue { get; set; }
        public int? TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<StudentAnomalsGrade> studentAnomalsGrades { get; set; }
    }
    public class StudentAnomalsGrade
    {
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public int? StudentId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? AssignmentTypeTitle { get; set; }
        public string? AssignmentTitle { get; set; }
        public int? Points { get; set; }
        public string? AllowedMarks { get; set; }
        public string? Comment { get; set; }
    }

}
