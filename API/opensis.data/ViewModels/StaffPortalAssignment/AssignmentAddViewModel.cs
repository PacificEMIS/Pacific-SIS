using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalAssignment
{
    public class AssignmentAddViewModel : CommonFields
    {
        public Assignment assignment { get; set; }
        public List<int> courseSectionIds { get; set; }
    }
}
