using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StudentSchedule
{
    public class ScheduledStudentDeleteViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? CourseSectionId { get; set; }
        public List<int>? StudentIds { get; set; }
        public List<int>? StaffIds { get; set; }
        public string? UpdatedBy { get; set; }
        /// <summary>
        /// "Force Delete Transactional Data": delete the attendance records (with their comments
        /// and history) held by the selected students and teachers in this course section.
        /// Without it any row that carries attendance is refused, as before. Honoured only when
        /// the caller is an active Super Administrator (#863).
        /// </summary>
        public bool DeleteAttendance { get; set; }
        /// <summary>
        /// Email of the logged-in caller, taken from the session token by the service layer.
        /// Anything sent by the client is overwritten.
        /// </summary>
        public string? CallerEmail { get; set; }
    }
}
