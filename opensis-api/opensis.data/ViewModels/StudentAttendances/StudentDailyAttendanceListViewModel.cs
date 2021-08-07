using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class StudentDailyAttendanceListViewModel : CommonFields
    {
        public List<StudentDailyAttendance> studentDailyAttendanceList { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
