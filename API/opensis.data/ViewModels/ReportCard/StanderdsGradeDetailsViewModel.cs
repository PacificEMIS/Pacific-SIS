using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.ReportCard
{
    public class StanderdsGradeDetailsViewModel
    {
        public StanderdsGradeDetailsViewModel()
        {
            standerdsGradeDetails = new List<StanderdsGradeDetails>();
        }
        public string? CourseSectionName { get; set; }
        public string? StaffName { get; set; }
        public string? MarkingPeriodName { get; set; }
        public string? StandardRefNo { get; set; }
        public string? StandardDetails { get; set; }
        public string? value { get; set; }
        public List<StanderdsGradeDetails> standerdsGradeDetails { get; set; }
    }
    public class StanderdsGradeDetails
    {
        public StanderdsGradeDetails()
        {
            markingPeriodDetailsforStanderdsGrades = new List<MarkingPeriodDetailsforStanderdsGrade>();
        }
        public string? StandardRefNo { get; set; }
        public string? StandardDetails { get; set; }
        public List<MarkingPeriodDetailsforStanderdsGrade> markingPeriodDetailsforStanderdsGrades { get; set; }

    }

    public class MarkingPeriodDetailsforStanderdsGrade
    {
        public string? MarkingPeriodName { get; set; }
        public string? value { get; set; }
    }


}
