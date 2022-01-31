using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
    public class AssignmentsListViewModel
    {
        public AssignmentsListViewModel()
        {
            studentsListViewModels = new List<StudentsListViewModel>();
        }
        public int AssignmentTypeId { get; set; }
        public string? Title { get; set; }
        public int? Weightage { get; set; }
        public int AssignmentId { get; set; }
        public string? AssignmentTitle { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public List<StudentsListViewModel> studentsListViewModels { get; set; }
    }
}
