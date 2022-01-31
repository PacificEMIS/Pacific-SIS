using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.catelogdb.Interface;
using opensis.catalogdb.ViewModels;
using opensis.catelogdb.Models;
using opensis.data.ViewModels.ApiAccess;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.Student;
using opensis.data.ViewModels.MarkingPeriods;

namespace opensis.data.Repository
{
    public class ApiAccessRepository : IApiAccessRepository
    {
        private readonly CRMContext? context;
        //private static readonly string NORECORDFOUND = "No Record Found";
        private static string? apiKey;
        private readonly ICatalogDBRepository _catalogDBRepository;
        public IParentInfoRepository _parentInfoRepository;
        public IStudentRepository _studentRepository;
        public IMarkingperiodRepository _markingperiodRepository;
        public ApiAccessRepository(IDbContextFactory dbContextFactory, ICatalogDBRepository catalogDBRepository,
            IParentInfoRepository parentInfoRepository, IStudentRepository studentRepository, IMarkingperiodRepository markingperiodRepository)
        {
            context = dbContextFactory.Create();
            apiKey = dbContextFactory.ApiKeyValue;
            _catalogDBRepository = catalogDBRepository;
            _parentInfoRepository = parentInfoRepository;
            _studentRepository = studentRepository;
            _markingperiodRepository = markingperiodRepository;
        }


        public ApiAccessSchoolListViewModel GetAllSchoolList()
        {
            ApiAccessSchoolListViewModel schoolListModel = new();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                var decryptApiKey = Utility.DecryptString(apiKey);
                string[] sessionInfoArray = decryptApiKey.Split('|');
                if (sessionInfoArray.Length > 0)
                {
                    var response = _catalogDBRepository.CheckIfTenantIsAvailable(new AvailableTenantViewModel
                    {
                        tenant = new AvailableTenants
                        {
                            TenantName = sessionInfoArray[1]
                        }
                    });

                    if (!response.Failure)
                    {
                        if (GetAPIAccessPermission((int)ApiMethodEnum.GetAllSchool, apiKey))
                        {
                            var schoolMasterList = this.context?.SchoolMaster
                         .Include(d => d.SchoolDetail)
                         .Where(x => x.TenantId == response.tenant.TenantId).OrderBy(c => c.SchoolId).Select(e => new ApiAccessSchoolModel()
                         {
                             SchoolYear = this.context.SchoolCalendars.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true).FirstOrDefault()!.StartDate +"-"+ this.context.SchoolCalendars.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true).FirstOrDefault()!.EndDate,
                             State = e.State,
                             SchoolName = e.SchoolName!.Trim(),
                             SchoolId = e.SchoolInternalId,
                             GovtOrNonGovt = e.SchoolClassification,
                             SchoolBudget = null,
                             DateSchoolOpened = e.SchoolDetail.FirstOrDefault()!.DateSchoolOpened,
                             LanguageOfInstruction = null,
                             Buildings = null,
                             ClassRooms = this.context.Rooms.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId).Select(x => x.SchoolId).Count(),
                             ClassRoomsInPoorCondition = null,
                             Electricity = e.SchoolDetail.FirstOrDefault()!.Electricity,
                             ElectricitySource = null,
                             HIVEducation = null,
                             SexualityEducation = null,
                             Covid19Education = null,
                             SpEdInfrastructure = null,
                             Telephone = e.SchoolDetail.FirstOrDefault()!.Telephone,
                             Fax = e.SchoolDetail.FirstOrDefault()!.Fax,
                             Copier = null,
                             Internet = e.SchoolDetail.FirstOrDefault()!.Internet,
                             InternetInTeaching = null,
                             ComputerLab = null,
                             Computers = null,
                             ComputerInTeaching = null,
                             UseOfTechnology = null,
                             SchoolGuid = e.SchoolGuid,
                             SchoolAltId = e.SchoolAltId,
                             SchoolDistrictId = e.SchoolDistrictId,
                             SchoolLevel = e.SchoolLevel,
                             //SchoolClassification = e.SchoolClassification,
                             AlternateName = e.AlternateName,
                             StreetAddress1 = e.StreetAddress1,
                             StreetAddress2 = e.StreetAddress2,
                             City = e.City,
                             County = e.County,
                             Division = e.Division,
                             District = e.District,
                             Zip = e.Zip,
                             Country = e.Country,
                             //CurrentPeriodEnds = e.CurrentPeriodEnds,
                             //MaxApiChecks = e.MaxApiChecks,
                             //Features = e.Features,
                             //CreatedBy = e.CreatedBy,
                             //CreatedOn = e.CreatedOn,
                             //UpdatedBy = e.UpdatedBy,
                             //UpdatedOn = e.UpdatedOn,
                             Longitude = e.Longitude,
                             Latitude = e.Latitude,
                             Affiliation = e.SchoolDetail.FirstOrDefault()!.Affiliation,
                             Associations = e.SchoolDetail.FirstOrDefault()!.Associations,
                             Locale = e.SchoolDetail.FirstOrDefault()!.Locale,
                             LowestGradeLevel = e.SchoolDetail.FirstOrDefault()!.LowestGradeLevel,
                             HighestGradeLevel = e.SchoolDetail.FirstOrDefault()!.HighestGradeLevel,
                             DateSchoolClosed = e.SchoolDetail.FirstOrDefault()!.DateSchoolClosed,
                             Status = e.SchoolDetail.FirstOrDefault()!.Status,
                             Gender = e.SchoolDetail.FirstOrDefault()!.Gender,
                             Website = e.SchoolDetail.FirstOrDefault()!.Website,
                             Email = e.SchoolDetail.FirstOrDefault()!.Email,
                             Twitter = e.SchoolDetail.FirstOrDefault()!.Twitter,
                             Facebook = e.SchoolDetail.FirstOrDefault()!.Facebook,
                             Instagram = e.SchoolDetail.FirstOrDefault()!.Instagram,
                             Youtube = e.SchoolDetail.FirstOrDefault()!.Youtube,
                             LinkedIn = e.SchoolDetail.FirstOrDefault()!.LinkedIn,
                             NameOfPrincipal = e.SchoolDetail.FirstOrDefault()!.NameOfAssistantPrincipal,
                             NameOfAssistantPrincipal = e.SchoolDetail.FirstOrDefault()!.NameOfAssistantPrincipal,
                             RunningWater = e.SchoolDetail.FirstOrDefault()!.RunningWater,
                             MainSourceOfDrinkingWater = e.SchoolDetail.FirstOrDefault()!.MainSourceOfDrinkingWater,
                             CurrentlyAvailable = e.SchoolDetail.FirstOrDefault()!.CurrentlyAvailable,
                             FemaleToiletType = e.SchoolDetail.FirstOrDefault()!.FemaleToiletType,
                             TotalFemaleToilets = e.SchoolDetail.FirstOrDefault()!.TotalFemaleToilets,
                             TotalFemaleToiletsUsable = e.SchoolDetail.FirstOrDefault()!.TotalFemaleToiletsUsable,
                             FemaleToiletAccessibility = e.SchoolDetail.FirstOrDefault()!.FemaleToiletAccessibility,
                             MaleToiletType = e.SchoolDetail.FirstOrDefault()!.MaleToiletType,
                             TotalMaleToilets = e.SchoolDetail.FirstOrDefault()!.TotalMaleToilets,
                             TotalMaleToiletsUsable = e.SchoolDetail.FirstOrDefault()!.TotalMaleToiletsUsable,
                             MaleToiletAccessibility = e.SchoolDetail.FirstOrDefault()!.MaleToiletAccessibility,
                             ComonToiletType = e.SchoolDetail.FirstOrDefault()!.ComonToiletType,
                             TotalCommonToilets = e.SchoolDetail.FirstOrDefault()!.TotalCommonToilets,
                             TotalCommonToiletsUsable = e.SchoolDetail.FirstOrDefault()!.TotalCommonToiletsUsable,
                             CommonToiletAccessibility = e.SchoolDetail.FirstOrDefault()!.CommonToiletAccessibility,
                             HandwashingAvailable = e.SchoolDetail.FirstOrDefault()!.HandwashingAvailable,
                             SoapAndWaterAvailable = e.SchoolDetail.FirstOrDefault()!.SoapAndWaterAvailable,
                             HygeneEducation = e.SchoolDetail.FirstOrDefault()!.HygeneEducation,
                         }).ToList();
                            schoolListModel.ApiAccessSchoolList = schoolMasterList!;

                        }
                        else
                        {
                            schoolListModel._message = "You have no access";
                            schoolListModel.ApiAccessSchoolList = null!;
                        }
                    }


                }
            }

            return schoolListModel;
        }


        public ApiAccessSchoolViewModel GetSchoolDetails(decimal academicYear)
        {
            ApiAccessSchoolViewModel schoolDetails = new();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {


                var decryptApiKey = Utility.DecryptString(apiKey);
                string[] sessionInfoArray = decryptApiKey.Split('|');
                if (sessionInfoArray.Length > 0)
                {
                    var response = this._catalogDBRepository.CheckIfTenantIsAvailable(new AvailableTenantViewModel
                    {
                        tenant = new AvailableTenants { TenantName = sessionInfoArray[1] }
                    });

                    var schoolId = sessionInfoArray[2];

                    if (!response.Failure && !string.IsNullOrEmpty(schoolId))
                    {
                        if (GetAPIAccessPermission((int)ApiMethodEnum.GetSchoolDetails, apiKey))
                        {
                            var schoolMaster = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x=>x.GradeScale).ThenInclude(x=>x.Grade).Where(x => x.TenantId == response.tenant.TenantId && x.SchoolId == Int32.Parse(schoolId)).FirstOrDefault();
                            if (schoolMaster != null)
                            {
                                schoolDetails.SchoolDetails =new ApiAccessSchoolModel()
                                {
                                    SchoolYear = this.context?.SchoolCalendars.Where(x => x.TenantId == response.tenant.TenantId && x.SchoolId == Int32.Parse(schoolId) && x.SessionCalendar==true && x.AcademicYear==academicYear).FirstOrDefault()!.StartDate!.Value.Year + "-" + this.context?.SchoolCalendars.Where(x => x.TenantId == response.tenant.TenantId && x.SchoolId == Int32.Parse(schoolId) && x.SessionCalendar == true && x.AcademicYear == academicYear).FirstOrDefault()!.EndDate!.Value.Year,
                                    State = schoolMaster.State,
                                    SchoolName = schoolMaster.SchoolName!.Trim(),
                                    SchoolId = schoolMaster.SchoolInternalId,
                                    GovtOrNonGovt = schoolMaster.SchoolClassification,
                                    SchoolBudget = null,
                                    DateSchoolOpened = schoolMaster.SchoolDetail.FirstOrDefault()!.DateSchoolOpened,
                                    LanguageOfInstruction = null,
                                    Buildings = null,
                                    ClassRooms = this.context?.Rooms.Where(x => x.TenantId == schoolMaster.TenantId && x.SchoolId == schoolMaster.SchoolId && x.AcademicYear== academicYear).Select(x => x.SchoolId).Count(),
                                    ClassRoomsInPoorCondition = null,
                                    Electricity = schoolMaster.SchoolDetail.FirstOrDefault()!.Electricity,
                                    ElectricitySource = null,
                                    HIVEducation = null,
                                    SexualityEducation = null,
                                    Covid19Education = null,
                                    SpEdInfrastructure = null,
                                    Telephone = schoolMaster.SchoolDetail.FirstOrDefault()!.Telephone,
                                    Fax = schoolMaster.SchoolDetail.FirstOrDefault()!.Fax,
                                    Copier = null,
                                    Internet = schoolMaster.SchoolDetail.FirstOrDefault()!.Internet,
                                    InternetInTeaching = null,
                                    ComputerLab = null,
                                    Computers = null,
                                    ComputerInTeaching = null,
                                    UseOfTechnology = null,
                                    SchoolGuid = schoolMaster.SchoolGuid,
                                    SchoolAltId = schoolMaster.SchoolAltId,
                                    SchoolDistrictId = schoolMaster.SchoolDistrictId,
                                    SchoolLevel = schoolMaster.SchoolLevel,
                                    SchoolClassification = schoolMaster.SchoolClassification,
                                    AlternateName = schoolMaster.AlternateName,
                                    StreetAddress1 = schoolMaster.StreetAddress1,
                                    StreetAddress2 = schoolMaster.StreetAddress2,
                                    City = schoolMaster.City,
                                    County = schoolMaster.County,
                                    Division = schoolMaster.Division,
                                    District = schoolMaster.District,
                                    Zip = schoolMaster.Zip,
                                    Country = schoolMaster.Zip,
                                    CurrentPeriodEnds = schoolMaster.CurrentPeriodEnds,
                                    MaxApiChecks = schoolMaster.MaxApiChecks,
                                    Features = schoolMaster.Features,
                                    Longitude = schoolMaster.Longitude,
                                    Latitude = schoolMaster.Latitude,
                                    Affiliation = schoolMaster.SchoolDetail.FirstOrDefault()!.Affiliation,
                                    Associations = schoolMaster.SchoolDetail.FirstOrDefault()!.Associations,
                                    Locale = schoolMaster.SchoolDetail.FirstOrDefault()!.Locale,
                                    LowestGradeLevel = schoolMaster.SchoolDetail.FirstOrDefault()!.LowestGradeLevel,
                                    HighestGradeLevel = schoolMaster.SchoolDetail.FirstOrDefault()!.HighestGradeLevel,
                                    DateSchoolClosed = schoolMaster.SchoolDetail.FirstOrDefault()!.DateSchoolClosed,
                                    Status = schoolMaster.SchoolDetail.FirstOrDefault()!.Status,
                                    Gender = schoolMaster.SchoolDetail.FirstOrDefault()!.Gender,
                                    Website = schoolMaster.SchoolDetail.FirstOrDefault()!.Website,
                                    Email = schoolMaster.SchoolDetail.FirstOrDefault()!.Email,
                                    Twitter = schoolMaster.SchoolDetail.FirstOrDefault()!.Twitter,
                                    Facebook = schoolMaster.SchoolDetail.FirstOrDefault()!.Facebook,
                                    Instagram = schoolMaster.SchoolDetail.FirstOrDefault()!.Instagram,
                                    Youtube = schoolMaster.SchoolDetail.FirstOrDefault()!.Youtube,
                                    LinkedIn = schoolMaster.SchoolDetail.FirstOrDefault()!.LinkedIn,
                                    NameOfPrincipal = schoolMaster.SchoolDetail.FirstOrDefault()!.NameOfAssistantPrincipal,
                                    NameOfAssistantPrincipal = schoolMaster.SchoolDetail.FirstOrDefault()!.NameOfAssistantPrincipal,
                                    RunningWater = schoolMaster.SchoolDetail.FirstOrDefault()!.RunningWater,
                                    MainSourceOfDrinkingWater = schoolMaster.SchoolDetail.FirstOrDefault()!.MainSourceOfDrinkingWater,
                                    CurrentlyAvailable = schoolMaster.SchoolDetail.FirstOrDefault()!.CurrentlyAvailable,
                                    FemaleToiletType = schoolMaster.SchoolDetail.FirstOrDefault()!.FemaleToiletType,
                                    TotalFemaleToilets = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalFemaleToilets,
                                    TotalFemaleToiletsUsable = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalFemaleToiletsUsable,
                                    FemaleToiletAccessibility = schoolMaster.SchoolDetail.FirstOrDefault()!.FemaleToiletAccessibility,
                                    MaleToiletType = schoolMaster.SchoolDetail.FirstOrDefault()!.MaleToiletType,
                                    TotalMaleToilets = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalMaleToilets,
                                    TotalMaleToiletsUsable = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalMaleToiletsUsable,
                                    MaleToiletAccessibility = schoolMaster.SchoolDetail.FirstOrDefault()!.MaleToiletAccessibility,
                                    ComonToiletType = schoolMaster.SchoolDetail.FirstOrDefault()!.ComonToiletType,
                                    TotalCommonToilets = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalCommonToilets,
                                    TotalCommonToiletsUsable = schoolMaster.SchoolDetail.FirstOrDefault()!.TotalCommonToiletsUsable,
                                    CommonToiletAccessibility = schoolMaster.SchoolDetail.FirstOrDefault()!.CommonToiletAccessibility,
                                    HandwashingAvailable = schoolMaster.SchoolDetail.FirstOrDefault()!.HandwashingAvailable,
                                    SoapAndWaterAvailable = schoolMaster.SchoolDetail.FirstOrDefault()!.SoapAndWaterAvailable,
                                    HygeneEducation = schoolMaster.SchoolDetail.FirstOrDefault()!.HygeneEducation,
                                };
                                schoolDetails.MarkingPeriodList = _markingperiodRepository.GetMarkingPeriod(new MarkingPeriod() { SchoolId = Int32.Parse(schoolId), TenantId = response.tenant.TenantId!.Value, AcademicYear = academicYear })?.schoolYearsView;
                                var blockDataList = this.context?.Block.AsQueryable().Where(x => x.TenantId == schoolMaster.TenantId && x.SchoolId == schoolMaster.SchoolId).ToList();
                                schoolDetails.BlockList = blockDataList?.Select(x => new BlockViewModel() {
                                    BlockTitle = x.BlockTitle,
                                    FullDayMinutes = x.FullDayMinutes,
                                    HalfDayMinutes = x.HalfDayMinutes,
                                    BlockPeriodList = this.context?.BlockPeriod.AsQueryable().Where(b => b.TenantId == schoolMaster.TenantId && b.SchoolId == schoolMaster.SchoolId && b.AcademicYear == academicYear && b.BlockId == x.BlockId).Select(p=> new BlockPeriodViewModel()
                                    {
                                        PeriodTitle=p.PeriodTitle,
                                        PeriodShortName= p.PeriodShortName,
                                        PeriodStartTime= p.PeriodStartTime,
                                        PeriodEndTime= p.PeriodEndTime
                                    }).ToList()
                                }).ToList();
                                schoolDetails.GradeScaleList = schoolMaster.GradeScale.AsQueryable().Where(x => x.AcademicYear == academicYear).Select(e => new GradeScaleViewModel() {
                                    GradeScaleName = e.GradeScaleName,
                                    GradeScaleValue = e.GradeScaleValue,
                                    GradeScaleComment = e.GradeScaleComment,
                                    GradeList = e.Grade.AsQueryable().Select(g => new GradeViewModel()
                                    {
                                        Title = g.Title,
                                        Breakoff = g.Breakoff,
                                        WeightedGpValue = g.WeightedGpValue,
                                        UnweightedGpValue = g.UnweightedGpValue,
                                        Comment = g.Comment
                                    }).ToList()
                                }).ToList();
                            }

                        }
                        else
                        {
                            schoolDetails._message = "You have no access";
                            schoolDetails.SchoolDetails = null!;
                        }
                    }
                }
            }

            return schoolDetails;
        }

        public ApiAccessStaffListViewModel GetAllStaffList()
        {
            ApiAccessStaffListViewModel staffListModel = new();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {


                var decryptApiKey = Utility.DecryptString(apiKey);
                string[] sessionInfoArray = decryptApiKey.Split('|');
                if (sessionInfoArray.Length > 0)
                {
                    var response = this._catalogDBRepository.CheckIfTenantIsAvailable(new AvailableTenantViewModel
                    {
                        tenant = new AvailableTenants { TenantName = sessionInfoArray[1] }
                    });

                    var schoolId = sessionInfoArray[2];

                    if (!response.Failure && !string.IsNullOrEmpty(schoolId))
                    {
                        if (GetAPIAccessPermission((int)ApiMethodEnum.GetAllStaff, apiKey))
                        {
                            var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == response.tenant.TenantId && x.SchoolAttachedId== Int32.Parse(schoolId)).Select(s => s.StaffId).ToList();

                            if (schoolAttachedStaffId?.Any() == true)
                            {
                                var staffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == response.tenant.TenantId && (x.Profile ?? "").ToLower() != "super administrator" && schoolAttachedStaffId.Contains(x.StaffId));

                                staffListModel.ApiStaffList = staffMasterList!.Select(e => new ApiStaffMaster()
                                {
                                    StaffId = e.StaffInternalId,
                                    StaffGuid = e.StaffGuid,
                                    FirstGivenName = e.FirstGivenName,
                                    MiddleName = e.MiddleName,
                                    LastFamilyName = e.LastFamilyName,
                                    Profile = e.StaffSchoolInfo.Any() ? e.StaffSchoolInfo!.Where(x => x.SchoolAttachedId == e.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault()!.Profile : null,
                                    JobTitle = e.JobTitle,
                                    SchoolEmail = e.SchoolEmail,
                                    MobilePhone = e.MobilePhone,
                                    HomeroomTeacher = e.HomeroomTeacher,
                                    PrimaryGradeLevelTaught = e.PrimaryGradeLevelTaught,
                                    OtherGradeLevelTaught = e.OtherGradeLevelTaught,
                                    PrimarySubjectTaught = e.PrimarySubjectTaught,
                                    OtherSubjectTaught = e.OtherSubjectTaught,
                                    FirstLanguage = e.FirstLanguage,
                                    SecondLanguage = e.SecondLanguage,
                                    ThirdLanguage = e.ThirdLanguage,
                                    Dob = e.Dob,
                                    Age = e.Dob != null ? (DateTime.Now.Year - e.Dob.Value.Year) : null,
                                    Gender = e.Gender,
                                    Race = e.Race,
                                    Ethnicity = e.Ethnicity,
                                    MaritalStatus = e.MaritalStatus,
                                    CountryOfBirth = e.CountryOfBirth,
                                    Nationality = e.Nationality,
                                    Twitter = e.Twitter,
                                    Facebook = e.Facebook,
                                    Instagram = e.Instagram,
                                    Youtube = e.Youtube,
                                    Linkedin = e.Linkedin,
                                    HomeAddressLineOne = e.HomeAddressLineOne,
                                    HomeAddressLineTwo = e.HomeAddressLineTwo,
                                    HomeAddressCountry = e.HomeAddressCountry,
                                    HomeAddressCity = e.HomeAddressCity,
                                    HomeAddressState = e.HomeAddressState,
                                    HomeAddressZip = e.HomeAddressZip,
                                    MailingAddressSameToHome = e.MailingAddressSameToHome,
                                    MailingAddressLineOne = e.MailingAddressLineOne,
                                    MailingAddressLineTwo = e.MailingAddressLineTwo,
                                    MailingAddressCountry = e.MailingAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.MailingAddressCountry))!.Name : null,
                                    MailingAddressCity = e.MailingAddressCity,
                                    MailingAddressState = e.MailingAddressState,
                                    MailingAddressZip = e.MailingAddressZip,
                                    IsActive = e.IsActive,
                                    StaffSchoolInfo = !e.StaffSchoolInfo.Any() ? null! : e.StaffSchoolInfo.Where(x => x.SchoolAttachedId == e.SchoolId).Select(f => new ApiStaffSchoolInfo()
                                    {
                                        SchoolAttachedId = f.SchoolAttachedId,
                                        SchoolAttachedName = f.SchoolAttachedName,
                                        Profile = f.Profile,
                                        StartDate = f.StartDate,
                                        EndDate = f.EndDate
                                    }).ToList()
                                }).ToList();
                            }
                        }
                        else
                        {
                            staffListModel._message = "You have no access";
                            staffListModel.ApiStaffList = null!;
                        }


                    }

                }
            }
            return staffListModel;
        }

        public ApiAccessStudentListViewModel GetAllStudentList(decimal? academicYear)
        {
            //ApiAccessStudentListViewModel studentListModel = new ApiAccessStudentListViewModel();
            ApiAccessStudentListViewModel studentListModel = new();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                var decryptApiKey = Utility.DecryptString(apiKey);
                string[] sessionInfoArray = decryptApiKey.Split('|');
                if (sessionInfoArray.Length > 0)
                {
                    var response = _catalogDBRepository.CheckIfTenantIsAvailable(new AvailableTenantViewModel
                    {
                        tenant = new AvailableTenants { TenantName = sessionInfoArray[1] }
                    });

                    var schoolId = sessionInfoArray[2];

                    if (!response.Failure && !string.IsNullOrEmpty(schoolId))
                    {
                        if (GetAPIAccessPermission((int)ApiMethodEnum.GetAllStudent, apiKey))
                        {
                            var studentDataList = this.context?.StudentMaster.Include(x => x.StudentEnrollment).Include(x => x.StudentComments).Include(x => x.StudentDocuments).Include(x => x.StudentMedicalAlert).Include(x => x.StudentMedicalImmunization).Include(x => x.StudentMedicalNote).Include(x => x.StudentMedicalNurseVisit)
                                    .Include(x => x.StudentMedicalProvider).Where(x => x.TenantId == response.tenant.TenantId && x.SchoolId == Int32.Parse(schoolId) && x.StudentEnrollment.Any(se=>se.EnrollmentDate!.Value.Year == academicYear));
                            if (studentDataList != null)
                            {
                                studentListModel.ApiAccessStudentList = studentDataList.Select(e => new ApiStudentMaster()
                                {
                                    StudentId = e.StudentInternalId,
                                    StudentGuid = e.StudentGuid,
                                    AlternateId = e.AlternateId,
                                    DistrictId = e.DistrictId,
                                    StateId = e.StateId,
                                    AdmissionNumber = e.AdmissionNumber,
                                    RollNumber = e.RollNumber,
                                    Salutation = e.Salutation,
                                    FirstGivenName = e.FirstGivenName,
                                    MiddleName = e.MiddleName,
                                    LastFamilyName = e.LastFamilyName,
                                    Suffix = e.Suffix,
                                    PreferredName = e.PreferredName,
                                    PreviousName = e.PreviousName,
                                    SchoolEmail = e.SchoolEmail,
                                    MobilePhone = e.MobilePhone,
                                    SocialSecurityNumber = e.SocialSecurityNumber,
                                    OtherGovtIssuedNumber = e.OtherGovtIssuedNumber,
                                    Dob = e.Dob,
                                    Age = e.Dob != null? (DateTime.Now.Year - e.Dob.Value.Year): null,
                                    Gender = e.Gender,
                                    Race = e.Race,
                                    Ethnicity = e.Ethnicity,
                                    MaritalStatus = e.MaritalStatus,
                                    CountryOfBirth = this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.CountryOfBirth))!.Name ?? null,
                                    Nationality = e.Nationality != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.Nationality))!.Name : null,
                                    FirstLanguage = e.FirstLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.FirstLanguageId))!.Locale : null,
                                    SectionId = e.SectionId,
                                    EstimatedGradDate = e.EstimatedGradDate,
                                    Twitter = e.Twitter,
                                    Facebook = e.Facebook,
                                    Instagram = e.Instagram,
                                    Youtube = e.Youtube,
                                    Linkedin = e.Linkedin,
                                    HomeAddressLineOne = e.HomeAddressLineOne,
                                    HomeAddressLineTwo = e.HomeAddressLineTwo,
                                    HomeAddressCountry = e.HomeAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.HomeAddressCountry))!.Name : null,
                                    HomeAddressCity = e.HomeAddressCity,
                                    HomeAddressState = e.HomeAddressState,
                                    HomeAddressZip = e.HomeAddressZip,
                                    MailingAddressSameToHome= e.MailingAddressSameToHome,
                                    MailingAddressLineOne= e.MailingAddressLineOne,
                                    MailingAddressLineTwo= e.MailingAddressLineTwo,
                                    MailingAddressCountry= e.MailingAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.MailingAddressCountry))!.Name : null,
                                    MailingAddressCity= e.MailingAddressCity,
                                    MailingAddressState= e.MailingAddressState,
                                    MailingAddressZip= e.MailingAddressZip,
                                    IsActive = e.IsActive,
                                    StudentEnrollment = !e.StudentEnrollment!.Any() ? null! : e.StudentEnrollment.Where(x => x.StudentId == e.StudentId && x.EnrollmentDate!.Value.Year == academicYear).Select(f => new ApiStudentEnrollment()
                                    {
                                        EnrollmentId = f.EnrollmentId,
                                        SchoolName = f.SchoolName,
                                        GradeLevelTitle = f.GradeLevelTitle,
                                        EnrollmentDate = f.EnrollmentDate,
                                        EnrollmentCode = f.EnrollmentCode,
                                        ExitDate = f.ExitDate,
                                        ExitCode = f.ExitCode,
                                        SchoolTransferred = f.SchoolTransferred,
                                        TransferredGrade = f.TransferredGrade,
                                    }).ToList(),
                                  /*  StudentDocuments = !e.StudentDocuments!.Any() ? null! : e.StudentDocuments.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).OrderByDescending(x => x.DocumentId).ToList(),
                                    StudentComments = !e.StudentComments!.Any() ? null! : e.StudentComments.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).OrderByDescending(x => x.CommentId).ToList(),*/
                                    StudentMedicalAlert = !e.StudentMedicalAlert!.Any() ? null! : e.StudentMedicalAlert.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList(),
                                    StudentMedicalImmunization = !e.StudentMedicalImmunization!.Any() ? null! : e.StudentMedicalImmunization.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList(),
                                    StudentMedicalNote = !e.StudentMedicalNote!.Any() ? null! : e.StudentMedicalNote.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList(),
                                    StudentMedicalNurseVisit = !e.StudentMedicalNurseVisit!.Any() ? null! : e.StudentMedicalNurseVisit.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList(),
                                    StudentMedicalProvider = !e.StudentMedicalProvider!.Any() ? null! : e.StudentMedicalProvider.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList(),
                                    Contacts = _parentInfoRepository.ViewParentListForStudent(new ParentInfoListModel() { SchoolId = e.SchoolId, StudentId = e.StudentId, TenantId = e.TenantId, IsShowParentPhoto = false }),
                                    Siblings = _studentRepository.ViewAllSibling(new StudentListModel()
                                    {
                                        StudentId = e.StudentId,
                                        SchoolId = e.SchoolId,
                                        TenantId = e.TenantId,
                                        IsShowPicture = false
                                    }),
                                }).ToList();
                            }

                        }
                        else
                        {
                            studentListModel._message = "You have no access";
                            studentListModel.ApiAccessStudentList = null!;
                        }

                    }

                }
            }
            return studentListModel;
        }

        public bool GetAPIAccessPermission(int controllerId, string apiKey)
        {
            bool apiAccess = false;

            var apiKeyMasterData = this.context?.ApiKeysMaster.FirstOrDefault(x => x.ApiKey == apiKey);

            if (apiKeyMasterData != null)
            {
                var apiAccessPermission = this.context?.ApiControllerKeyMapping.FirstOrDefault(x => x.ControllerId == controllerId && x.KeyId == apiKeyMasterData.KeyId && x.SchoolId == apiKeyMasterData.SchoolId && x.IsActive == true);

                if (apiAccessPermission != null)
                {
                    apiAccess = true;
                }
            }

            return apiAccess;
        }
    }
}
