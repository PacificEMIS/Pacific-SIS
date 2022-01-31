using opensis.report.report.data.ViewModels.StaffReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Staff.Interfaces
{
    public interface IStaffReportService
    {
        public StaffAdvancedReport StaffAdvancedReport(StaffAdvancedReport reportModel);
    }
}
