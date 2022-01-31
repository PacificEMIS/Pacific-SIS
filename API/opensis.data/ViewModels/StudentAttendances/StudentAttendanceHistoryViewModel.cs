using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class StudentAttendanceHistoryViewModel : CommonFields
    {
        public StudentAttendanceHistoryViewModel()
        {
            attendanceHistoryViewModels = new List<AttendanceHistoryViewModel>();
        }
        public List<AttendanceHistoryViewModel> attendanceHistoryViewModels { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int BlockId { get; set; }
        public int PeriodId { get; set; }
    }
}
