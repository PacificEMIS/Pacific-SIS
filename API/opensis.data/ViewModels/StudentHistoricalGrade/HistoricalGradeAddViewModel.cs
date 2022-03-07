using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentHistoricalGrade
{
    public class HistoricalGradeAddViewModel : CommonFields
    {
        public HistoricalGradeAddViewModel()
        {
            HistoricalGradeList=new List<HistoricalGrade>();
            gradeEquivalencies = new List<GradeEquivalency>();
        }
        public List<HistoricalGrade> HistoricalGradeList { get; set; }
        public List<GradeEquivalency> gradeEquivalencies { get; set; }
        public int? SchoolId { get; set; }
        public Guid TenantId { get; set; }
        public int? StudentId { get; set; }
        public string? CreatedBy { get; set; }

    }
}
