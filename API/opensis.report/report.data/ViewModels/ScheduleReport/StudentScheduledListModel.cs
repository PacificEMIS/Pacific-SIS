using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class StudentScheduledListModel: CommonFields
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public List<CourseIds>? courseIds { get; set; }
        public List<CourseSectionForStaff>? courseSectionForStaffs { get; set; }
    }
    public class CourseIds
    {
        public int? CourseId { get; set; }
        public int? CourseSectionId { get; set; }
    }
}
