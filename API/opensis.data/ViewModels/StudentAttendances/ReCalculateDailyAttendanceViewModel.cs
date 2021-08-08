using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class ReCalculateDailyAttendanceViewModel : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
