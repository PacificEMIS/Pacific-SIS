using opensis.core.helper.Interfaces;
using opensis.report.report.core.Staff.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.StaffReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Staff.Services
{
    public class StaffReportService : IStaffReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStaffReportRepository staffReportRepository;
        public ICheckLoginSession tokenManager;
        public StaffReportService(IStaffReportRepository staffReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.staffReportRepository = staffReportRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// Staff Advanced Report
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        public StaffAdvancedReport StaffAdvancedReport(StaffAdvancedReport reportModel)
        {
            StaffAdvancedReport staffListModel = new();
            try
            {
                if (tokenManager.CheckToken(reportModel._tenantName + reportModel._userName, reportModel._token))
                {

                    staffListModel = this.staffReportRepository.StaffAdvancedReport(reportModel);
                    return staffListModel;

                }
                else
                {
                    staffListModel._failure = true;
                    staffListModel._message = TOKENINVALID;
                    return staffListModel;
                }
            }
            catch (Exception es)
            {
                staffListModel._failure = true;
                staffListModel._message = es.Message;
            }
            return staffListModel;

        }
    }
}
