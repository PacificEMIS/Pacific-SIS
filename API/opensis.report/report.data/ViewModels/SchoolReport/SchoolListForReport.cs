using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.SchoolReport
{
    public class SchoolListForReport : CommonFields
    {
        public List<SchoolViewForReport>? schoolViewForReports { get; set; }
        public Guid? TenantId { get; set; }
        public int[]? SchoolIds { get; set; }
    }

    public class SchoolViewForReport
    {
        public SchoolMaster? schoolMaster { get; set; }
        public int? StudentCount { get; set; }
        public int? StaffCount { get; set; }
        public int? ParentCount { get; set; }
    }
}
