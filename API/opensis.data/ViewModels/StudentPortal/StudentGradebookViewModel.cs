using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StudentPortal
{
    public class StudentGradebookViewModel : CommonFields
    {
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<StudentGradebookGradeDetails>? studentGradebookGradeDetails { get; set; }
    }

    public class StudentGradebookGradeDetails
    {
        public int? CourseSectionId { get; set; }
        public string? CourseSectionName { get; set; }
        public string? StaffFirstName { get; set; }
        public string? StaffMiddleName { get; set; }
        public string? StaffLastName { get; set; }
        public string? Percent { get; set; }
        public string? Letter { get; set; }
        public int? Ungraded { get; set; }
        public string? LowestGrade { get; set; }
        public string? HighestGrade { get; set; }
    }
}
