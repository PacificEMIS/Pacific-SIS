using opensis.data.Models;
using opensis.report.report.data.ViewModels.AttendanceReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.Interface
{
    public interface IAttendanceReportRepository
    {
        public StudentAttendanceReport GetStudentAttendanceReport(PageResult pageResult);
        public AverageDailyAttendanceViewModel GetAverageDailyAttendanceReport(PageResult pageResult);
        public AverageDailyAttendanceViewModel GetAverageAttendancebyDayReport(PageResult pageResult);

    }
}
