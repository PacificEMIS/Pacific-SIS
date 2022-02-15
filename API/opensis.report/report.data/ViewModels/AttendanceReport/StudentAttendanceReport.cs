using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.AttendanceReport
{
    public class StudentAttendanceReport : CommonFields
    {
        public StudentAttendanceReport()
        {
            studendAttendanceAdministrationList = new List<StudendAttendanceViewModelForReport>();
        }
        public List<StudendAttendanceViewModelForReport> studendAttendanceAdministrationList { get; set; }
        public DataTable? StudentAttendanceReportForExcel { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }

    }

    public class StudendAttendanceViewModelForReport
    {
        public StudendAttendanceViewModelForReport()
        {
            studentAttendanceList = new List<StudentAttendance>();
        }
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public Guid StudentGuid { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? StudentInternalId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? PeriodsName { get; set; }
        public List<StudentAttendance> studentAttendanceList { get; set; }
    }

    public class AttendanceExcelReport
    {
        public int StudentId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string? StudentInternalId { get; set; }
        public string? StudentName { get; set; }
        public string? PeriodName { get; set; }
        public string? AttendanceCode { get; set; }
        public string? GradeLevelTitle { get; set; }
    }

    public class StudentAttendanceViewForReport
    {
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int? CourseId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? BlockId { get; set; }
        public int? PeriodId { get; set; }
        public int? PeriodSortOrder { get; set; }
        public string? PeriodName { get; set; }
        public int? AttendanceCategoryId { get; set; }
        public int? AttendanceCode { get; set; }
        public string? AttendanceCodeTitle { get; set; }
        public string? StudentInternalId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? GradeLevelTitle { get; set; }
    }
}

