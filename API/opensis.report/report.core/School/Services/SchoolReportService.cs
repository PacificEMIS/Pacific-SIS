using opensis.core.helper.Interfaces;
using opensis.report.report.core.School.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.SchoolReport;
using System;

namespace opensis.report.report.core.School.Services
{
    public class SchoolReportService : ISchoolReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public ISchoolReportRepository schoolReportRepository;
        public ICheckLoginSession tokenManager;
        public SchoolReportService(ISchoolReportRepository schoolReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.schoolReportRepository = schoolReportRepository;
            this.tokenManager = checkLoginSession;
        }


        /// <summary>
        /// Get School Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public SchoolListForReport GetSchoolReport(SchoolListForReport report)
        {
            SchoolListForReport schoolListModel = new();
            try
            {
                if (tokenManager.CheckToken(report._tenantName + report._userName, report._token))
                {
                    schoolListModel = this.schoolReportRepository.GetSchoolReport(report);
                    return schoolListModel;
                }
                else
                {
                    schoolListModel._failure = true;
                    schoolListModel._message = TOKENINVALID;
                    return schoolListModel;
                }
            }
            catch (Exception es)
            {
                schoolListModel._failure = true;
                schoolListModel._message = es.Message;
            }
            return schoolListModel;

        }
    }
}
