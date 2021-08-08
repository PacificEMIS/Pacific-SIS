using opensis.data.Models;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class CourseSectionForAttendanceViewModel : CommonFields
    {
        public CourseSectionForAttendanceViewModel()
        {
            courseSectionViewList = new List<CourseSectionViewList>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public decimal? AcademicYear { get; set; }
        public List<CourseSectionViewList> courseSectionViewList { get; set; }

    }
}
