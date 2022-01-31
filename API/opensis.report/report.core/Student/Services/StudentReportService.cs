using opensis.core.helper.Interfaces;
using opensis.data.Models;
using opensis.data.ViewModels.Student;
using opensis.report.report.core.Student.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.StudentReport;
using System;

namespace opensis.report.report.core.Student.Services
{
    public class StudentReportService : IStudentReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IStudentReportRepository studentReportRepository;
        public ICheckLoginSession tokenManager;
        public StudentReportService(IStudentReportRepository studentReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.studentReportRepository = studentReportRepository;
            this.tokenManager = checkLoginSession;
        }

        /// <summary>
        /// Get Student Info Report
        /// </summary>
        /// <param name="studentInfoReport"></param>
        /// <returns></returns>
        public StudentInfoListForReport GetStudentInfoReport(StudentInfoReport studentInfoReport)
        {
            StudentInfoListForReport studentListModel = new();
            try
            {
                if (tokenManager.CheckToken(studentInfoReport._tenantName + studentInfoReport._userName, studentInfoReport._token))
                {

                    studentListModel = this.studentReportRepository.GetStudentInfoReport(studentInfoReport);
                    return studentListModel;

                }
                else
                {
                    studentListModel._failure = true;
                    studentListModel._message = TOKENINVALID;
                    return studentListModel;
                }
            }
            catch (Exception es)
            {
                studentListModel._failure = true;
                studentListModel._message = es.Message;
            }
            return studentListModel;

        }

        /// <summary>
        /// Student Advanced Report
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        public StudentAdvancedReport StudentAdvancedReport(StudentAdvancedReport reportModel)
        {
            StudentAdvancedReport studentListModel = new();
            try
            {
                if (tokenManager.CheckToken(reportModel._tenantName + reportModel._userName, reportModel._token))
                {

                    studentListModel = this.studentReportRepository.StudentAdvancedReport(reportModel);
                    return studentListModel;

                }
                else
                {
                    studentListModel._failure = true;
                    studentListModel._message = TOKENINVALID;
                    return studentListModel;
                }
            }
            catch (Exception es)
            {
                studentListModel._failure = true;
                studentListModel._message = es.Message;
            }
            return studentListModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentAddDropViewModel"></param>
        /// <returns></returns>
        public StudentAddDropViewModel GetstudentAddDropReport(PageResult pageResult)
        {
            StudentAddDropViewModel studentAddDrop = new StudentAddDropViewModel();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {

                    studentAddDrop = this.studentReportRepository.GetstudentAddDropReport(pageResult);

                }
                else
                {
                    studentAddDrop._failure = true;
                    studentAddDrop._message = TOKENINVALID;

                }
            }
            catch (Exception es)
            {

                studentAddDrop._failure = true;
                studentAddDrop._message = es.Message;
            }
            return studentAddDrop;

        }


        /// <summary>
        /// Get Student Enrollment Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentEnrollmentReport GetStudentEnrollmentReport(PageResult pageResult)
        {
            StudentEnrollmentReport studentEnrolment = new StudentEnrollmentReport();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    studentEnrolment = this.studentReportRepository.GetStudentEnrollmentReport(pageResult);
                }
                else
                {
                    studentEnrolment._failure = true;
                    studentEnrolment._message = TOKENINVALID;

                }
            }
            catch (Exception es)
            {

                studentEnrolment._failure = true;
                studentEnrolment._message = es.Message;
            }
            return studentEnrolment;

        }

    }
}

