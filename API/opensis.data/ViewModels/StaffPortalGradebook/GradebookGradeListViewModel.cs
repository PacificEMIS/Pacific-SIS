using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
    public class GradebookGradeListViewModel : CommonFields
    {
        public GradebookGradeListViewModel()
        {
            assignmentsListViewModels = new List<AssignmentsListViewModel>();
        }
        public List<AssignmentsListViewModel> assignmentsListViewModels { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public decimal AcademicYear { get; set; }
        public int CourseSectionId { get; set; }
        public string? CreatedBy { get; set;}
        public bool? IncludeInactive { get; set; }
        public bool? ShowUngraded { get; set; }
        public string? SearchValue { get; set; }
        public string? MarkingPeriodId { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public bool? ConfigUpdateFlag { get; set; }
    }
}
