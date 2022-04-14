using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.ReportCard
{
    public class EffortGradeDetailsViewModel
    {
        public EffortGradeDetailsViewModel()
        {
            effortGradeItemDetails = new List<EffortGradeItemDetails>();
        }
        public string? CategoryName { get; set; }
        public string? MarkingPeriodName { get; set; }
        public string? EffortItemTitle { get; set; }
        public int? GradeScaleValue { get; set; }
        public List<EffortGradeItemDetails> effortGradeItemDetails { get; set; }
    }
    public class EffortGradeItemDetails
    {
        public EffortGradeItemDetails()
        {
            effortGradeMarkingPeriodDetails = new List<EffortGradeMarkingPeriodDetails>();
        }
        public string? EffortItemTitle { get; set; }
        public List<EffortGradeMarkingPeriodDetails> effortGradeMarkingPeriodDetails { get; set; }

    }
    public class EffortGradeMarkingPeriodDetails
    {
        public string? MarkingPeriodName { get; set; }
        public int? GradeScaleValue { get; set; }

    }

}
