using opensis.report.report.data.ViewModels.StaffReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.Interface
{
    public interface IStaffReportRepository
    {
        public StaffAdvancedReport StaffAdvancedReport(StaffAdvancedReport reportModel);
    }
}
