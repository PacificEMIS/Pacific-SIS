using System;
using System.Collections.Generic;

namespace opensis.data.Models
{
    public partial class StudentMissingAttendance
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int MissingAttendanceId { get; set; }
        public int? StaffId { get; set; }
        public int? BlockId { get; set; }
        public int? CourseId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? PeriodId { get; set; }
        public int? AttendanceCategoryId { get; set; }
        public int? AttendanceCode { get; set; }
        public DateTime? MissingAttendanceDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual BlockPeriod? BlockPeriod { get; set; }
        public virtual StaffCoursesectionSchedule? StaffCoursesectionSchedule { get; set; }
        public virtual StaffMaster? StaffMaster { get; set; }
    }
}
