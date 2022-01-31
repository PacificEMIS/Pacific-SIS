using opensis.data.ViewModels;
using System;

namespace opensis.report.report.data.ViewModels.StudentReport
{
    public class StudentInfoReport : CommonFields
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public bool IsGeneralInfo { get; set; }
        public bool IsEnrollmentInfo { get; set; }
        public bool IsAddressInfo { get; set; }
        public bool IsFamilyInfo { get; set; }
        public bool IsMedicalInfo { get; set; }
        public Guid[]? StudentGuids { get; set; }
    }
}
