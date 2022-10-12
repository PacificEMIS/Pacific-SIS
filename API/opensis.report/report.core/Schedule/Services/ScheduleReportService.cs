using opensis.core.helper.Interfaces;
using opensis.data.Models;
using opensis.report.report.core.Schedule.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.ScheduleReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Schedule.Services
{
    public class ScheduleReportService : IScheduleReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IScheduleReportRepository scheduleReportRepository;
        public ICheckLoginSession tokenManager;
        public ScheduleReportService(IScheduleReportRepository scheduleReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.scheduleReportRepository = scheduleReportRepository;
            this.tokenManager = checkLoginSession;
        }


        /// <summary>
        /// Get Scheduled Add/Drop Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public AddDropScheduleReport GetScheduledAddDropReport(PageResult pageResult)
        {
            AddDropScheduleReport scheduleAddDrop = new AddDropScheduleReport();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {

                    scheduleAddDrop = this.scheduleReportRepository.GetScheduledAddDropReport(pageResult);

                }
                else
                {
                    scheduleAddDrop._failure = true;
                    scheduleAddDrop._message = TOKENINVALID;

                }
            }
            catch (Exception es)
            {

                scheduleAddDrop._failure = true;
                scheduleAddDrop._message = es.Message;
            }
            return scheduleAddDrop;

        }

        /// <summary>
        /// Scheduled CourseSection List
        /// </summary>
        /// <param name="courseSectionList"></param>
        /// <returns></returns>
        public ScheduleClassList ScheduledCourseSectionList(ScheduleClassList courseSectionList)
        {
            ScheduleClassList scCourseSectionList = new();
            try
            {
                if (tokenManager.CheckToken(courseSectionList._tenantName + courseSectionList._userName, courseSectionList._token))
                {
                    scCourseSectionList = this.scheduleReportRepository.ScheduledCourseSectionList(courseSectionList);
                }
                else
                {
                    scCourseSectionList._failure = true;
                    scCourseSectionList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                scCourseSectionList._failure = true;
                scCourseSectionList._message = es.Message;
            }
            return scCourseSectionList;

        }

        /// <summary>
        /// Get StudentList By CourseSection
        /// </summary>
        /// <param name="studentList"></param>
        /// <returns></returns>
        public StudentScheduledListModel GetStudentListByCourseSection(StudentScheduledListModel studentList)
        {
            StudentScheduledListModel studentScheduledList = new();
            try
            {
                if (tokenManager.CheckToken(studentList._tenantName + studentList._userName, studentList._token))
                {
                    studentScheduledList = this.scheduleReportRepository.GetStudentListByCourseSection(studentList);
                }
                else
                {
                    studentScheduledList._failure = true;
                    studentScheduledList._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                studentScheduledList._failure = true;
                studentScheduledList._message = es.Message;
            }
            return studentScheduledList;
        }
        public SchoolwideScheduleReportViewModel GetSchoolwideScheduleReport(SchoolwideScheduleReportViewModel schoolwideScheduleViewModel)
        {
            SchoolwideScheduleReportViewModel schoolwideScheduleReport = new();
            try
            {
                if (tokenManager.CheckToken(schoolwideScheduleViewModel._tenantName + schoolwideScheduleViewModel._userName, schoolwideScheduleViewModel._token))
                {
                    schoolwideScheduleReport = this.scheduleReportRepository.GetSchoolwideScheduleReport(schoolwideScheduleViewModel);
                }
                else
                {
                    schoolwideScheduleReport._failure = true;
                    schoolwideScheduleReport._message = TOKENINVALID;
                }

            }
            catch (Exception es)
            {
                schoolwideScheduleReport._failure = true;
                schoolwideScheduleReport._message = es.Message;
            }
            return schoolwideScheduleReport;
        }

        /// <summary>
        /// Get Print Schedule Report
        /// </summary>
        /// <param name="printScheduleReportViewModel"></param>
        /// <returns></returns>
        public PrintScheduleReportViewModel GetPrintScheduleReport(PrintScheduleReportViewModel printScheduleReportViewModel)
        {
            PrintScheduleReportViewModel printScheduleReport = new();
            try
            {
                if (tokenManager.CheckToken(printScheduleReportViewModel._tenantName + printScheduleReportViewModel._userName, printScheduleReportViewModel._token))
                {
                    printScheduleReport = this.scheduleReportRepository.GetPrintScheduleReport(printScheduleReportViewModel);
                }
                else
                {
                    printScheduleReport._failure = true;
                    printScheduleReport._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                printScheduleReport._failure = true;
                printScheduleReport._message = es.Message;
            }
            return printScheduleReport;
        }
    }
}
