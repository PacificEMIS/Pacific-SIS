using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class FillAttendanceViewModel : CommonFields
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? MembershipId { get; set; }
        public string? CreatedBy { get; set; }
        public int TotalRecordsCreated { get; set; }
        public int CourseSectionsProcessed { get; set; }
    }
}
