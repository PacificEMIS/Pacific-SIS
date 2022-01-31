using opensis.data.Models;
using opensis.data.ViewModels.StudentHistoricalGrade;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.StudentHistoricalGrade.Interfaces
{
    public interface IStudentHistoricalGradeService
    {
        public HistoricalMarkingPeriodAddViewModel AddHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod);
        public HistoricalMarkingPeriodAddViewModel UpdateHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod);
        public HistoricalMarkingPeriodListModel GetAllHistoricalMarkingPeriodList(PageResult pageResult);
        public HistoricalMarkingPeriodAddViewModel DeleteHistoricalMarkingPeriod(HistoricalMarkingPeriodAddViewModel historicalMarkingPeriod);
        public HistoricalGradeAddViewModel AddUpdateHistoricalGrade(HistoricalGradeAddViewModel historicalGradeList);
        public HistoricalGradeAddViewModel GetAllHistoricalGradeList(HistoricalGradeAddViewModel historicalGradeList);
    }
}
