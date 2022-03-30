using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.CourseManager
{
    public class CourseSectionAssignmentViewModel : CommonFields
    {
        public CourseSectionAssignmentViewModel()
        {
            courseSectionList = new List<CourseSectionDetails>();
        }
        public Guid? TenantId { get; set; }
        public int? StaffId { get; set; }
        public int? SchoolId { get; set; }
        public decimal? AcademicYear { get; set; }
        public List<CourseSectionDetails> courseSectionList { get; set; }
    }
    public class CourseSectionDetails
    {
        public CourseSectionDetails()
        {
            assignmentTypes = new List<AssignmentType>();
        }
        public string? CourseSectionName { get; set; }
        public int? CourseSectionId { get; set; }
        public List<AssignmentType> assignmentTypes { get; set; }
    }
}
