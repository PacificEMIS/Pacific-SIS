using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentHistoricalGrade
{
    public class HistoricalMarkingPeriodListModel: CommonFields
    {
        public HistoricalMarkingPeriodListModel()
        {
            HistoricalMarkingPeriodList = new List<HistoricalMarkingPeriod>();
        }
        public List<HistoricalMarkingPeriod> HistoricalMarkingPeriodList { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
    }
}
