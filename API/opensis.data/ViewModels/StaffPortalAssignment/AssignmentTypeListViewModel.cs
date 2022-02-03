using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalAssignment
{
    public class AssignmentTypeListViewModel : CommonFields
    {
        public AssignmentTypeListViewModel()
        {
            assignmentTypeList = new List<AssignmentType>();
        }
        public List<AssignmentType> assignmentTypeList { get; set; }
        public Guid TenantId { get; set; }        
        public int SchoolId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? TotalWeightage { get; set; }
        public bool? WeightedGrades { get; set; }
        public decimal? AcademicYear { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
    }
}
