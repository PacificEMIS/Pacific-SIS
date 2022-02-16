using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.StaffReport
{
    public class StaffAdvancedReport: CommonFields
    {
        public List<StaffSchoolReport>? schoolListForReport { get; set; }

        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public string? Module { get; set; }
        public decimal? AcademicYear { get; set; }
        public List<int> StaffIds { get; set; }
    }

    public class StaffSchoolReport
    {
        public List<StaffReport>? staffListForReport { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public Guid SchoolGuid { get; set; }
        public string? SchoolInternalId { get; set; }
        public string? SchoolAltId { get; set; }
        public string? SchoolStateId { get; set; }
        public string? SchoolDistrictId { get; set; }
        public string? SchoolLevel { get; set; }
        public string? SchoolClassification { get; set; }
        public string? SchoolName { get; set; }
        public string? AlternateName { get; set; }
        public byte[]? SchoolLogo { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Division { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    public class StaffReport
    {
        public StaffMaster staffMaster { get; set; }
        public List<FieldsCategory>? fieldsCategoryList { get; set; }
    }
}
