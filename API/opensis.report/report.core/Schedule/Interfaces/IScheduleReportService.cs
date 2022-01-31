using opensis.data.Models;
using opensis.report.report.data.ViewModels.ScheduleReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Schedule.Interfaces
{
    public interface IScheduleReportService
    {
        public AddDropScheduleReport GetScheduledAddDropReport(PageResult pageResult);
        public ScheduleClassList ScheduledCourseSectionList(ScheduleClassList courseSectionList);
        public StudentScheduledListModel GetStudentListByCourseSection(StudentScheduledListModel studentList);
    }
}
