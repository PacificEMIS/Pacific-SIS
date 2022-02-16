using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
    public class AssignmentForStudentViewModel : CommonFields
    {
        public AssignmentForStudentViewModel()
        {
            assignmentTypeViewModelList = new List<AssignmentTypeViewModel>();
        }
        public List<AssignmentTypeViewModel> assignmentTypeViewModelList { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public decimal AcademicYear { get; set; }
        public int CourseSectionId { get; set; }
        public int StudentId { get; set; }
        public string? CreatedBy { get; set; }
        public string? MarkingPeriodId { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
    }
}
