using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentEffortGrade
{

    public class HomeRoomStaffByStudentListModel : CommonFields
    {

        public HomeRoomStaffByStudentListModel()
        {
            studentsByHomeRoomStaffView = new List<StudentsByHomeRoomStaffView>();
        }
        public List<StudentsByHomeRoomStaffView>? studentsByHomeRoomStaffView { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StaffId { get; set; }
        public int? CourseId { get; set; }
        public int? CourseSectionId { get; set; }
        public int? CalendarId { get; set; }
        public string? MarkingPeriodId { get; set; }
        public decimal? AcademicYear { get; set; }
        public string? CreatedOrUpdatedBy { get; set; }

        public class StudentsByHomeRoomStaffView : StudentEffortGradeListModel
        {
            public List<StudentEffortGradeMaster>? studentEffortGradeList { get; set; }
            public Guid? TenantId { get; set; }
            public int SchoolId { get; set; }
            public int? StudentId { get; set; }
            public Guid? StudentGuid { get; set; }
            public int? GradeId { get; set; }
            public int? GradeScaleId { get; set; }
            public string? FirstGivenName { get; set; }
            public string? MiddleName { get; set; }
            public string? LastFamilyName { get; set; }
            public string? AlternateId { get; set; }
            public string? StudentInternalId { get; set; }
            public string? GradeLevel { get; set; }
            public string? Section { get; set; }
            public byte[]? StudentPhoto { get; set; }
            public int? CourseSectionId { get; set; }
            public DateTime? Dob { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDropped { get; set; }
            public string? SchoolName { get; set; }
        }

    }
}
