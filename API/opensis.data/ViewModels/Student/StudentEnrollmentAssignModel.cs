using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.Student
{
    public class StudentEnrollmentAssignModel : CommonFields
    {
        public StudentEnrollment studentEnrollments { get; set; }
        public List<int> studentIds { get; set; }
        public int? SectionId { get; set; }
        public DateTime? EstimatedGradDate { get; set; }
        public bool? Eligibility504 { get; set; }
        public bool? EconomicDisadvantage { get; set; }
        public bool? FreeLunchEligibility { get; set; }
        public bool? SpecialEducationIndicator { get; set; }
        public bool? LepIndicator { get; set; }
    }
}
