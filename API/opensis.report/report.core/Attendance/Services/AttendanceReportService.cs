using opensis.core.helper.Interfaces;
using opensis.data.Models;
using opensis.report.report.core.Attendance.Interfaces;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.AttendanceReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Attendance.Services
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

        public IAttendanceReportRepository attendanceReportRepository;
        public ICheckLoginSession tokenManager;
        public AttendanceReportService(IAttendanceReportRepository attendanceReportRepository, ICheckLoginSession checkLoginSession)
        {
            this.attendanceReportRepository = attendanceReportRepository;
            this.tokenManager = checkLoginSession;
        }


        /// <summary>
        /// Get Student Info Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentAttendanceReport GetStudentAttendanceReport(PageResult pageResult)
        {
            StudentAttendanceReport studentListModel = new();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {

                    studentListModel = this.attendanceReportRepository.GetStudentAttendanceReport(pageResult);
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
        /// Get Student Attendance Excel Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentAttendanceReport GetStudentAttendanceExcelReport(PageResult pageResult)
        {
            StudentAttendanceReport studentListModel = new();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {

                    studentListModel = this.attendanceReportRepository.GetStudentAttendanceExcelReport(pageResult);
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
    /// Get Student Absence List
    /// </summary>
    /// <param name="pageResult"></param>
    /// <returns></returns>
    public StudentListForAbsenceSummary GetAllStudentAbsenceList(PageResult pageResult)
    {
      StudentListForAbsenceSummary studentListModel = new();
      try
      {
        if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
        {

          studentListModel = this.attendanceReportRepository.GetAllStudentAbsenceList(pageResult);
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
    /// Get Absence List by Student
    /// </summary>
    /// <param name="pageResult"></param>
    /// <returns></returns>
    public AbsenceListByStudent GetAbsenceListByStudent(PageResult pageResult)
    {
      AbsenceListByStudent absenceListModel = new();
      try
      {
        if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
        {

          absenceListModel = this.attendanceReportRepository.GetAbsenceListByStudent(pageResult);
          return absenceListModel;

        }
        else
        {
          absenceListModel._failure = true;
          absenceListModel._message = TOKENINVALID;
          return absenceListModel;
        }
      }
      catch (Exception es)
      {
        absenceListModel._failure = true;
        absenceListModel._message = es.Message;
      }
      return absenceListModel;

    }

    /// <summary>
    /// GetAverageDailyAttendanceReport
    /// </summary>
    /// <param name="pageResult"></param>
    /// <returns></returns>
    public AverageDailyAttendanceViewModel GetAverageDailyAttendanceReport(PageResult pageResult)
        {
            AverageDailyAttendanceViewModel averageDailyAttendance = new();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    averageDailyAttendance = this.attendanceReportRepository.GetAverageDailyAttendanceReport(pageResult);
                }
                else
                {
                    averageDailyAttendance._failure = true;
                    averageDailyAttendance._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                averageDailyAttendance._failure = true;
                averageDailyAttendance._message = es.Message;
            }
            return averageDailyAttendance;
        }


        /// <summary>
        /// GetAverageAttendancebyDayReport
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public AverageDailyAttendanceViewModel GetAverageAttendancebyDayReport(PageResult pageResult)
        {
            AverageDailyAttendanceViewModel averageDailyAttendance = new();
            try
            {
                if (tokenManager.CheckToken(pageResult._tenantName + pageResult._userName, pageResult._token))
                {
                    averageDailyAttendance = this.attendanceReportRepository.GetAverageAttendancebyDayReport(pageResult);
                }
                else
                {
                    averageDailyAttendance._failure = true;
                    averageDailyAttendance._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                averageDailyAttendance._failure = true;
                averageDailyAttendance._message = es.Message;
            }
            return averageDailyAttendance;
        }
    }
}
