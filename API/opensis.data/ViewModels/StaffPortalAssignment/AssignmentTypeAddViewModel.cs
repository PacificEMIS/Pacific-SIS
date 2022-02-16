using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalAssignment
{
    public class AssignmentTypeAddViewModel : CommonFields
    {
        public AssignmentType? assignmentType { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
    }
}
