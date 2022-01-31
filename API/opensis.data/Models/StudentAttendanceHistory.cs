using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.data.Models
{
    public partial class StudentAttendanceHistory
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
        /// <summary>
        /// should be staff or admin in profile. field is of user_is from usermast.
        /// </summary>
        public int ModifiedBy { get; set; }
        public int? MembershipId { get; set; }
        public DateTime ModificationTimestamp { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ValidateNever]
        public virtual StudentMaster StudentMaster { get; set; } = null!;
    }
}
