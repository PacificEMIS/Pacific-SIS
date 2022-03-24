using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using opensis.data.Models;
using opensis.data.ViewModels.Student;
using opensis.report.report.core.Attendance.Interfaces;
using opensis.report.report.core.Grade.Interfaces;
using opensis.report.report.core.Schedule.Interfaces;
using opensis.report.report.core.School.Interfaces;
using opensis.report.report.core.Staff.Interfaces;
using opensis.report.report.core.Student.Interfaces;
using opensis.report.report.data.ViewModels.AttendanceReport;
using opensis.report.report.data.ViewModels.GradeReport;
using opensis.report.report.data.ViewModels.ScheduleReport;
using opensis.report.report.data.ViewModels.SchoolReport;
using opensis.report.report.data.ViewModels.StaffReport;
using opensis.report.report.data.ViewModels.StudentReport;
using System;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/Report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IStudentReportService _studentReportService;
        private IScheduleReportService _scheduleReportService;
        private IAttendanceReportService _attendanceReportService;
        private IStaffReportService _staffReportService;
        private ISchoolReportService _schoolReportService;
        private IGradeReportService _gradeReportService;
        public ReportController(IStudentReportService studentReportService, IScheduleReportService scheduleReportService, IAttendanceReportService attendanceReportService, IStaffReportService staffReportService, ISchoolReportService schoolReportService, IGradeReportService gradeReportService)
        {
            _studentReportService = studentReportService;
            _scheduleReportService = scheduleReportService;
            _attendanceReportService = attendanceReportService;
            _staffReportService = staffReportService;
            _schoolReportService = schoolReportService;
            _gradeReportService = gradeReportService;
        }

            [HttpPost("getStudentInfoReport")]
        public ActionResult<StudentInfoListForReport> GetStudentInfoReport(StudentInfoReport studentInfoReport)
        {
            StudentInfoListForReport studentList = new();
            try
            {
                studentList = _studentReportService.GetStudentInfoReport(studentInfoReport);

            }
            catch (Exception es)
            {
                studentList._failure = true;
                studentList._message = es.Message;
            }
            return studentList;
        }

        [HttpPost("getstudentAddDropReport")]
        public ActionResult<StudentAddDropViewModel> GetstudentAddDropReport(PageResult pageResult)
        {
            StudentAddDropViewModel studentAddDrop = new();
            try
            {
                studentAddDrop = _studentReportService.GetstudentAddDropReport(pageResult);

            }
            catch (Exception es)
            {
                studentAddDrop._failure = true;
                studentAddDrop._message = es.Message;
            }
            return studentAddDrop;
        }

        [HttpPost("getSheduledAddDropReport")]
        public ActionResult<AddDropScheduleReport> GetSheduledAddDropReport(PageResult pageResult)
        {
            AddDropScheduleReport scheduleAddDrop = new();
            try
            {
                scheduleAddDrop = _scheduleReportService.GetScheduledAddDropReport(pageResult);

            }
            catch (Exception es)
            {
                scheduleAddDrop._failure = true;
                scheduleAddDrop._message = es.Message;
            }
            return scheduleAddDrop;
        }


        [HttpPost("getStudentAdvancedReport")]
        public ActionResult<StudentAdvancedReport> StudentAdvancedReport(StudentAdvancedReport reportModel)
        {
            StudentAdvancedReport studentAdvanced = new();
            try
            {
                studentAdvanced = _studentReportService.StudentAdvancedReport(reportModel);

            }
            catch (Exception es)
            {
                studentAdvanced._failure = true;
                studentAdvanced._message = es.Message;
            }
            return studentAdvanced;
        }


        [HttpPost("getStaffAdvancedReport")]
        public ActionResult<StaffAdvancedReport> StaffAdvancedReport(StaffAdvancedReport reportModel)
        {
            StaffAdvancedReport staffAdvanced = new();
            try
            {
                staffAdvanced = _staffReportService.StaffAdvancedReport(reportModel);

            }
            catch (Exception es)
            {
                staffAdvanced._failure = true;
                staffAdvanced._message = es.Message;
            }
            return staffAdvanced;
        }


        [HttpPost("getStudentEnrollmentReport")]
        public ActionResult<StudentEnrollmentReport> GetStudentEnrollmentReport(PageResult pageResult)
        {
            StudentEnrollmentReport studentEnrollment = new();
            try
            {
                studentEnrollment = _studentReportService.GetStudentEnrollmentReport(pageResult);
            }
            catch (Exception es)
            {
                studentEnrollment._failure = true;
                studentEnrollment._message = es.Message;
            }
            return studentEnrollment;
        }

        [HttpPost("getStudentAttendanceReport")]
        public ActionResult<StudentAttendanceReport> GetStudentAttendancedReport(PageResult pageResult)
        {
            StudentAttendanceReport studentAttendanced = new();
            try
            {
                studentAttendanced = _attendanceReportService.GetStudentAttendanceReport(pageResult);
            }
            catch (Exception es)
            {
                studentAttendanced._failure = true;
                studentAttendanced._message = es.Message;
            }
            return studentAttendanced;
        }

        [HttpPost("getStudentAttendanceExcelReport")]
        public ActionResult<StudentAttendanceReport> GetStudentAttendanceExcelReport(PageResult pageResult)
        {
            StudentAttendanceReport studentAttendanced = new();
            try
            {
                studentAttendanced = _attendanceReportService.GetStudentAttendanceExcelReport(pageResult);
            }
            catch (Exception es)
            {
                studentAttendanced._failure = true;
                studentAttendanced._message = es.Message;
            }
            return studentAttendanced;
        }

    [HttpPost("getAllStudentAbsenceList")]
    public ActionResult<StudentListForAbsenceSummary> GetAllStudentAbsenceList(PageResult pageResult)
    {
      StudentListForAbsenceSummary studentListForAbsence = new();
      try
      {
        studentListForAbsence = _attendanceReportService.GetAllStudentAbsenceList(pageResult);
      }
      catch (Exception es)
      {
        studentListForAbsence._failure = true;
        studentListForAbsence._message = es.Message;
      }
      return studentListForAbsence;
    }

    [HttpPost("getAbsenceListByStudent")]
    public ActionResult<AbsenceListByStudent> GetAbsenceListByStudent(PageResult pageResult)
    {
      AbsenceListByStudent absenceList = new();
      try
      {
        absenceList = _attendanceReportService.GetAbsenceListByStudent(pageResult);
      }
      catch (Exception es)
      {
        absenceList._failure = true;
        absenceList._message = es.Message;
      }
      return absenceList;
    }


    [HttpPost("scheduledCourseSectionList")]
        public ActionResult<ScheduleClassList> ScheduledCourseSectionList(ScheduleClassList courseSectionList)
        {
            ScheduleClassList scheduledCourseSectionList = new();
            try
            {
                scheduledCourseSectionList = _scheduleReportService.ScheduledCourseSectionList(courseSectionList);
            }
            catch (Exception es)
            {
                scheduledCourseSectionList._failure = true;
                scheduledCourseSectionList._message = es.Message;
            }
            return scheduledCourseSectionList;
        }

        [HttpPost("getStudentListByCourseSection")]
        public ActionResult<StudentScheduledListModel> GetStudentListByCourseSection(StudentScheduledListModel courseSectionList)
        {
            StudentScheduledListModel scheduledStudentList = new();
            try
            {
                scheduledStudentList = _scheduleReportService.GetStudentListByCourseSection(courseSectionList);
            }
            catch (Exception es)
            {
                scheduledStudentList._failure = true;
                scheduledStudentList._message = es.Message;
            }
            return scheduledStudentList;
        }
        
        [HttpPost("getAverageDailyAttendanceReport")]
        public ActionResult<AverageDailyAttendanceViewModel> GetAverageDailyAttendanceReport(PageResult pageResult)
        {
            AverageDailyAttendanceViewModel averageDailyAttendance = new();
            try
            {
                averageDailyAttendance = _attendanceReportService.GetAverageDailyAttendanceReport(pageResult);
            }
            catch (Exception es)
            {
                averageDailyAttendance._failure = true;
                averageDailyAttendance._message = es.Message;
            }
            return averageDailyAttendance;
        }

        [HttpPost("getAverageAttendancebyDayReport")]
        public ActionResult<AverageDailyAttendanceViewModel> GetAverageAttendancebyDayReport(PageResult pageResult)
        {
            AverageDailyAttendanceViewModel averageDailyAttendance = new();
            try
            {
                averageDailyAttendance = _attendanceReportService.GetAverageAttendancebyDayReport(pageResult);
            }
            catch (Exception es)
            {
                averageDailyAttendance._failure = true;
                averageDailyAttendance._message = es.Message;
            }
            return averageDailyAttendance;
        }

        [HttpPost("getSchoolReport")]
        public ActionResult<SchoolListForReport> GetSchoolReport(SchoolListForReport report)
        {
            SchoolListForReport schoolList = new();
            try
            {
                schoolList = _schoolReportService.GetSchoolReport(report);
            }
            catch (Exception es)
            {
                schoolList._failure = true;
                schoolList._message = es.Message;
            }
            return schoolList;
        }

        [HttpPost("getStudentProgressReport")]
        public ActionResult<StudentProgressReport> GetStudentProgressReport(StudentProgressReport studentProgressReport)
        {
            StudentProgressReport studentProgressReportData = new();
            try
            {
                studentProgressReportData = _studentReportService.GetStudentProgressReport(studentProgressReport);
            }
            catch (Exception es)
            {
                studentProgressReportData._failure = true;
                studentProgressReportData._message = es.Message;
            }
            return studentProgressReportData;
        }

        [HttpPost("getHonorRollReport")]
        public ActionResult<HonorRollListForReport> GetHonorRollReport(PageResult pageResult)
        {
            HonorRollListForReport honorRollList = new();
            try
            {
                honorRollList = _gradeReportService.GetHonorRollReport(pageResult);
            }
            catch (Exception es)
            {
                honorRollList._failure = true;
                honorRollList._message = es.Message;
            }
            return honorRollList;
        }
    }
}
