using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
   public class AssignmentTypeViewModel
    {
        public AssignmentTypeViewModel()
        {
            assignmentViewModelList = new List<AssignmentViewModel>();
        }
        public int AssignmentTypeId { get; set; }
        public int? CourseSectionId { get; set; }
        public string? Title { get; set; }
        public int? Weightage { get; set; }
        public decimal? AssignmentTypePercentage { get; set; }
        public string? AssignmentTypeLetterGrade { get; set; }
        public string? AssignmentTypeMarks { get; set; }
        public string? AssignmentTypePoint { get; set; }
        public List<AssignmentViewModel> assignmentViewModelList { get; set; }
    }
}
  
