using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentHistoricalGrade
{
    public class HistoricalGradeListViewModel : CommonFields
    {
        public HistoricalGradeListViewModel()
        {
            HistoricalGradeList = new List<HistoricalGrade>();
        }
        public List<HistoricalGrade> HistoricalGradeList { get; set; }
    }
}
