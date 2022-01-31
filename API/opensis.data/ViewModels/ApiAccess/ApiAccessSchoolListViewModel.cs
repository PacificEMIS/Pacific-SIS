using opensis.data.Models;
using opensis.data.ViewModels.MarkingPeriods;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.ApiAccess
{

    public class ApiAccessSchoolListViewModel : APICommonModel
    {
        public ApiAccessSchoolListViewModel()
        {
            ApiAccessSchoolList = new List<ApiAccessSchoolModel>();
        }
        public List<ApiAccessSchoolModel> ApiAccessSchoolList { get; set; }
    }

    public class ApiAccessSchoolViewModel : APICommonModel
    {
        public ApiAccessSchoolModel? SchoolDetails { get; set; }
        public List<GradeScaleViewModel>? GradeScaleList { get; set; }
        public List<BlockViewModel>? BlockList { get; set; }
        public List<SchoolYearView>? MarkingPeriodList { get; set; }
        public List<AttendanceCode>? AttendanceCodeList { get; set; }
    }

    public class ApiAccessSchoolModel
    {
        public string? SchoolYear { get; set; }
        public string? State { get; set; }
        public string? SchoolName { get; set; }
        public string? SchoolId { get; set; }
        public string? GovtOrNonGovt { get; set; }
        public string? SchoolBudget { get; set; }
        public DateTime? DateSchoolOpened { get; set; }
        public string? LanguageOfInstruction { get; set; }
        public int? Buildings { get; set; }
        public int? ClassRooms { get; set; }
        public int? ClassRoomsInPoorCondition { get; set; }
        public bool? Electricity { get; set; }
        public string? ElectricitySource { get; set; }
        public bool? HIVEducation { get; set; }
        public bool? SexualityEducation { get; set; }
        public bool? Covid19Education { get; set; }
        public bool? SpEdInfrastructure { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public bool? Copier { get; set; }
        public bool? Internet { get; set; }
        public bool? InternetInTeaching { get; set; }
        public bool? ComputerLab { get; set; }
        public int? Computers { get; set; }
        public bool? ComputerInTeaching { get; set; }
        public bool? UseOfTechnology { get; set; }
        public Guid? SchoolGuid { get; set; }
        public string? SchoolAltId { get; set; }
        public string? SchoolDistrictId { get; set; }
        public string? SchoolLevel { get; set; }
        public string? SchoolClassification { get; set; }
        public string? AlternateName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Division { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public DateTime? CurrentPeriodEnds { get; set; }
        public int? MaxApiChecks { get; set; }
        public string? Features { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? Affiliation { get; set; }
        public string? Associations { get; set; }
        public string? Locale { get; set; }
        public string? LowestGradeLevel { get; set; }
        public string? HighestGradeLevel { get; set; }
        public DateTime? DateSchoolClosed { get; set; }
        public bool? Status { get; set; }
        public string? Gender { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? LinkedIn { get; set; }
        public string? NameOfPrincipal { get; set; }
        public string? NameOfAssistantPrincipal { get; set; }
        public bool? RunningWater { get; set; }
        public string? MainSourceOfDrinkingWater { get; set; }
        public bool? CurrentlyAvailable { get; set; }
        public string? FemaleToiletType { get; set; }
        public short? TotalFemaleToilets { get; set; }
        public short? TotalFemaleToiletsUsable { get; set; }
        public string? FemaleToiletAccessibility { get; set; }
        public string? MaleToiletType { get; set; }
        public short? TotalMaleToilets { get; set; }
        public short? TotalMaleToiletsUsable { get; set; }
        public string? MaleToiletAccessibility { get; set; }
        public string? ComonToiletType { get; set; }
        public short? TotalCommonToilets { get; set; }
        public short? TotalCommonToiletsUsable { get; set; }
        public string? CommonToiletAccessibility { get; set; }
        public bool? HandwashingAvailable { get; set; }
        public bool? SoapAndWaterAvailable { get; set; }
        public string? HygeneEducation { get; set; }
    }

    public class HonorRollsViewModel
    {
        public string? HonorRoll { get; set; }
        public int? Breakoff { get; set; }
    }

    public class BlockViewModel
    {
        public string? BlockTitle { get; set; }
        public long? BlockSortOrder { get; set; }
        public int? FullDayMinutes { get; set; }
        public int? HalfDayMinutes { get; set; }

        public List<BlockPeriodViewModel>? BlockPeriodList { get; set; }
    }

    public class BlockPeriodViewModel
    {
        public string? PeriodTitle { get; set; }
        public string? PeriodShortName { get; set; }
        public string? PeriodStartTime { get; set; }
        public string? PeriodEndTime { get; set; }
    }

    public class GradeScaleViewModel
    {
        public string? GradeScaleName { get; set; }
        public decimal? GradeScaleValue { get; set; }
        public string? GradeScaleComment { get; set; }
        public bool? CalculateGpa { get; set; }

        public List<GradeViewModel>? GradeList { get; set; }
    }

    public class GradeViewModel
    {
        public string? Title { get; set; }
        public int? Breakoff { get; set; }
        public decimal? WeightedGpValue { get; set; }
        public decimal? UnweightedGpValue { get; set; }
        public string? Comment { get; set; }
    }
}
