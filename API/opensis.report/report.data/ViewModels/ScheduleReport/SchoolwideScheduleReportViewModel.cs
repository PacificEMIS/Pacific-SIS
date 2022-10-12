using opensis.data.Models;
using opensis.data.ViewModels;
using opensis.data.ViewModels.Period;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.ScheduleReport
{
    public class SchoolwideScheduleReportViewModel: CommonFields
    {
        public SchoolwideScheduleReportViewModel()
        {
            DayWithCourseList = new List<DayWithCourse>();
            BlockListForView = new List<GetBlockListForView>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int AcademicYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DayWithCourse> DayWithCourseList { get; set; }

        public List<GetBlockListForView> BlockListForView { get; set; }
        public CalendarDataView CalendarDataView { get; set; }
    }
    public class DayWithCourse
    {
        public DayWithCourse()
        {
            CourseListModel = new List<ScheduleCourseViewModel>();
        }
        public string DayName { get; set; }
        public DateTime Date { get; set; }
        public bool? IsHoliday { get; set; }
        public List<ScheduleCourseViewModel> CourseListModel { get; set; }
    }

    public class ScheduleCourseViewModel
    {
        public ScheduleCourseViewModel(){
            StaffListModels = new List<ScheduleStaffDetails>();
        }
        public int CourseId { get; set; }
        public string CourseName { get; set;}

        public List<ScheduleStaffDetails> StaffListModels { get; set; }

    }

 public class ScheduleStaffDetails
    {
        public ScheduleStaffDetails()
        {
            CourseSectionListModels = new List<ScheduleCourseSectionViewModel>();
        }
        public string StaffName { get; set; }
        public int StaffId { get; set; }
        public List<ScheduleCourseSectionViewModel> CourseSectionListModels { get; set; }
    }
    public class ScheduleCourseSectionViewModel
    {

        public int CourSectionId { get; set; }
        public string CourseSectionName { get; set; }
        public DateTime? DurationStartDate { get; set; }
        public DateTime? DurationEndDate { get; set; }
        public string? ScheduleType { get; set; }
        public int? PeriodId { get; set; }
    }

    public class DateAndDayNameRange
    {
        public DateTime Date { get; set; }
        public DayOfWeek DayName { get; set; }
       
    }

    public class CalendarDataView
    {
       public CalendarDataView()
        {
            CalendarEvents = new List<CalendarEvents>();
        }
        public int CalendarId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Days { get; set; }

        public List<CalendarEvents> CalendarEvents { get; set; }
    }
}
