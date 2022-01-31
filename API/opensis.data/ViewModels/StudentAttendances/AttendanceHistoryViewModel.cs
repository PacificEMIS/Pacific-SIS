using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class AttendanceHistoryViewModel
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public long AttendanceHistoryId { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int AttendanceCategoryId { get; set; }
        public int AttendanceCode { get; set; }
        public int BlockId { get; set; }
        public int PeriodId { get; set; }
        public int ModifiedBy { get; set; }
        public int? MembershipId { get; set; }
        public DateTime ModificationTimestamp { get; set; }
        public string? UserName { get; set; }
        public string? ProfileType { get; set; }
        public string? AttendanceCodeTitle { get; set; }

    }
}
