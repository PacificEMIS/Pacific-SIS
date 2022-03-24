using opensis.data.Models;
using opensis.report.report.data.ViewModels.AttendanceReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Attendance.Interfaces
{
    public interface IAttendanceReportService
    {
        public StudentAttendanceReport GetStudentAttendanceReport(PageResult pageResult);
        public AverageDailyAttendanceViewModel GetAverageDailyAttendanceReport(PageResult pageResult);
        public AverageDailyAttendanceViewModel GetAverageAttendancebyDayReport(PageResult pageResult);
        public StudentAttendanceReport GetStudentAttendanceExcelReport(PageResult pageResult);
        public StudentListForAbsenceSummary GetAllStudentAbsenceList(PageResult pageResult);
        public AbsenceListByStudent GetAbsenceListByStudent(PageResult pageResult);

  }
}
