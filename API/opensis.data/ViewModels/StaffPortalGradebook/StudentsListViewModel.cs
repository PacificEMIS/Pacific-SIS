using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StaffPortalGradebook
{
    public class StudentsListViewModel
    {
        public int StudentId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public string? RunningAvg { get; set; }
        public string? RunningAvgGrade { get; set; }
        public int? Points { get; set; }
        public string? AllowedMarks { get; set; }
        public string? Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public string? Comment { get; set; }
        public int? TotalPointOfAssignmentType { get; set; }
        public int? TotalObtainOfAssignmentType { get; set; }
        public int? PercentageOfAssignmentType { get; set; }
        public string? LetterGradeOfAssignmentType { get; set; }
    }
}
