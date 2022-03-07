using opensis.core.helper.Interfaces;
using opensis.data.Models;
using opensis.report.report.core.Grade.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.GradeReport;
using System;

namespace opensis.report.report.core.Grade.Services
{
    public class GradeReportService : IGradeReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IGradeReportRepository gradeReportRepository;
        public ICheckLoginSession tokenManager;
        public GradeReportService(IGradeReportRepository gradeReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.gradeReportRepository = gradeReportRepository;
            this.tokenManager = checkLoginSession;
        }

        public HonorRollListForReport GetHonorRollReport(PageResult pageResult)
        {
            HonorRollListForReport honorRollListModel = new();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    honorRollListModel = this.gradeReportRepository.GetHonorRollReport(pageResult);
                    return honorRollListModel;
                }
                else
                {
                    honorRollListModel._failure = true;
                    honorRollListModel._message = TOKENINVALID;
                    return honorRollListModel;
                }
            }
            catch (Exception es)
            {
                honorRollListModel._failure = true;
                honorRollListModel._message = es.Message;
            }
            return honorRollListModel;

        }
    }
}
