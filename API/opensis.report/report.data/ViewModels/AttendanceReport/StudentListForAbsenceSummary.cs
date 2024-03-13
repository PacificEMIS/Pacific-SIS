using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.AttendanceReport
{

    public class StudentListForAbsenceSummary : CommonFields
    {
        public StudentListForAbsenceSummary()
        {
            studendAttendanceList = new List<StudentListForAbsence>();
            AttendanceCodeList = new List<AttendanceCode>();
        }

        public List<StudentListForAbsence> studendAttendanceList { get; set; }
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<AttendanceCode> AttendanceCodeList { get; set; }
    }

    public class StudentListForAbsence
    {
        public StudentListForAbsence()
        {
            attendanceDetailsViewModels = new List<AttendanceDetailsViewModel>();
        }
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public Guid? StudentGuid { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? StudentInternalId { get; set; }
        public string? StudentAlternetId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? PrefferedName { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? HomePhone { get; set; }
        public int? AbsentCount { get; set; }
        public int? HalfDayCount { get; set; }
        public int? PresentCount { get; set; }
        public List<AttendanceDetailsViewModel> attendanceDetailsViewModels { get; set; }

    }

    public class AbsenceListByStudent : CommonFields
    {

        public AbsenceListByStudent()
        {
            studendList = new List<AbsenceStudentModel>();
            blockPeriodList = new List<BlockPeriod>();
        }

        public List<AbsenceStudentModel> studendList { get; set; }
        public List<BlockPeriod> blockPeriodList { get; set; }
        public string? StudentInternalId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? GradeLevelTitle { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }

    }

    public class AbsenceStudentModel
    {
        public AbsenceStudentModel()
        {
            attendanceInPeriodDetailsViewModels = new List<AttendanceInPeriodDetailsViewModel>();
        }
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public DateTime? AbsenceDate { get; set; }
        public string? Attendance { get; set; }
        public string? AdminComment { get; set; }
        public string? TeacherComment { get; set; }
        public List<AttendanceInPeriodDetailsViewModel> attendanceInPeriodDetailsViewModels { get; set; }
    }

    public class AttendanceInPeriodDetailsViewModel
    {
        public int? PeriodId { get; set; }
        public string? PeriodTitle { get; set; }
        public string? PeriodShortCode { get; set; }
        public string? AttendanceStateCode { get; set; }
        public string? AttendanceTitle { get; set; }
        public string? AttendanceShortName { get; set; }

    }

    public class AttendanceDetailsViewModel
    {
        public int AttendanceCodeId { get; set; }
        public string? AttendanceStateCode { get; set; }
        public string? AttendanceTitle { get; set; }
        public string? AttendanceShortName { get; set; }
        public int? AttendanceCount { get; set; }
    }
}
