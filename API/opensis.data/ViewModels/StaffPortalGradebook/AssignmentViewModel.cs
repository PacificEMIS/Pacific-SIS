using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
   public class AssignmentViewModel
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int AssignmentTypeId { get; set; }
        public int AssignmentId { get; set; }
        public int? CourseSectionId { get; set; }
        public string? AssignmentTitle { get; set; }
        public int? Points { get; set; }
        public string? AllowedMarks { get; set; }
        public string? Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public string? Comment { get; set; }
    }
}
