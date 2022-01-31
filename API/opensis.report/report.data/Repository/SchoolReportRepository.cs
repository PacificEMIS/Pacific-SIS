using Microsoft.EntityFrameworkCore;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.SchoolReport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace opensis.report.report.data.Repository
{
    public class SchoolReportRepository : ISchoolReportRepository
    {

        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public SchoolReportRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Get School Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public SchoolListForReport GetSchoolReport(SchoolListForReport report)
        {
            SchoolListForReport schoolListForReport = new();
            try
            {
                var schoolMasterList = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.FieldsCategory).ThenInclude(x => x.CustomFields).ThenInclude(x => x.CustomFieldsValue).Where(x => x.TenantId == report.TenantId && report.SchoolIds.Contains(x.SchoolId)).Select(p => new SchoolMaster
                {
                    SchoolId = p.SchoolId,
                    TenantId = p.TenantId,
                    SchoolInternalId= p.SchoolInternalId,
                    SchoolAltId =p.SchoolAltId,
                    SchoolStateId=p.SchoolStateId,
                    SchoolDistrictId=p.SchoolDistrictId,
                    SchoolLevel= p.SchoolLevel,
                    SchoolClassification= p.SchoolClassification,
                    AlternateName= p.AlternateName,
                    SchoolName = p.SchoolName!.Trim(),
                    Zip = p.Zip,
                    StreetAddress1 = p.StreetAddress1,
                    StreetAddress2 = p.StreetAddress2,
                    State = p.State,
                    City = p.City,
                    Country = p.Country,
                    CreatedBy = p.CreatedBy,
                    CreatedOn = p.CreatedOn,
                    UpdatedBy = p.UpdatedBy,
                    UpdatedOn = p.UpdatedOn,
                    SchoolDetail = p.SchoolDetail.Select(s => new SchoolDetail
                    {
                        Affiliation= s.Affiliation,
                        Telephone = s.Telephone,
                        Associations= s.Associations,
                        LowestGradeLevel= s.LowestGradeLevel,
                        HighestGradeLevel= s.HighestGradeLevel,
                        SchoolLogo= s.SchoolLogo,
                        MaleToiletAccessibility= s.MaleToiletAccessibility,
                        MaleToiletType= s.MaleToiletType,
                        TotalMaleToilets=s.TotalMaleToilets,
                        TotalMaleToiletsUsable=s.TotalMaleToiletsUsable,
                        FemaleToiletAccessibility = s.FemaleToiletAccessibility,
                        FemaleToiletType= s.FemaleToiletAccessibility,
                        TotalFemaleToilets= s.TotalFemaleToilets,
                        TotalFemaleToiletsUsable= s.TotalFemaleToiletsUsable,
                        CommonToiletAccessibility=s.CommonToiletAccessibility,
                        ComonToiletType= s.ComonToiletType,
                        TotalCommonToilets= s.TotalCommonToilets,
                        TotalCommonToiletsUsable= s.TotalCommonToiletsUsable,
                        HandwashingAvailable= s.HandwashingAvailable,
                        SoapAndWaterAvailable = s.SoapAndWaterAvailable,
                        HygeneEducation= s.HygeneEducation,
                        NameOfPrincipal = s.NameOfPrincipal,
                        Status = s.Status,
                        Gender= s.Gender,
                        Internet=s.Internet,
                        Electricity= s.Electricity,
                        Fax= s.Fax,
                        Website= s.Website,
                        Email= s.Email,
                        Twitter= s.Twitter,
                        Facebook= s.Facebook,
                        Instagram= s.Instagram,
                        Youtube= s.Youtube,
                        LinkedIn= s.LinkedIn,
                        NameOfAssistantPrincipal= s.NameOfAssistantPrincipal,
                        RunningWater= s.RunningWater,
                        MainSourceOfDrinkingWater= s.MainSourceOfDrinkingWater,
                        CurrentlyAvailable= s.CurrentlyAvailable,

                        CreatedBy = s.CreatedBy,
                        CreatedOn = s.CreatedOn,
                        UpdatedBy = s.UpdatedBy,
                        UpdatedOn = s.UpdatedOn
                    }).ToList(),
                    FieldsCategory = p.FieldsCategory.Where(x => x.TenantId == p.TenantId && x.SchoolId == p.SchoolId && x.Module == "School").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder).Select(y => new FieldsCategory
                                    {
                                        TenantId = y.TenantId,
                                        SchoolId = y.SchoolId,
                                        CategoryId = y.CategoryId,
                                        IsSystemCategory = y.IsSystemCategory,
                                        Search = y.Search,
                                        Title = y.Title,
                                        Module = y.Module,
                                        SortOrder = y.SortOrder,
                                        Required = y.Required,
                                        Hide = y.Hide,
                                        CreatedBy = y.CreatedBy,
                                        CreatedOn = y.CreatedOn,
                                        UpdatedOn = y.UpdatedOn,
                                        UpdatedBy = y.UpdatedBy,
                                        CustomFields = y.CustomFields.Where(x => x.SystemField != true).Select(z => new CustomFields
                                        {
                                            TenantId = z.TenantId,
                                            SchoolId = z.SchoolId,
                                            CategoryId = z.CategoryId,
                                            FieldId = z.FieldId,
                                            Module = z.Module,
                                            Type = z.Type,
                                            Search = z.Search,
                                            Title = z.Title,
                                            SortOrder = z.SortOrder,
                                            SelectOptions = z.SelectOptions,
                                            SystemField = z.SystemField,
                                            Required = z.Required,
                                            Hide = z.Hide,
                                            DefaultSelection = z.DefaultSelection,
                                            UpdatedOn = z.UpdatedOn,
                                            UpdatedBy = z.UpdatedBy,
                                            CreatedBy = z.CreatedBy,
                                            CreatedOn = z.CreatedOn,
                                            CustomFieldsValue = z.CustomFieldsValue.Select(cv => new CustomFieldsValue()
                                            {
                                                TenantId = cv.TenantId,
                                                SchoolId = cv.SchoolId,
                                                CategoryId = cv.CategoryId,
                                                FieldId = cv.FieldId,
                                                CustomFieldType = cv.CustomFieldType,
                                                CustomFieldTitle = cv.CustomFieldTitle,
                                                CustomFieldValue = cv.CustomFieldValue,
                                                Module = cv.Module,
                                                TargetId = cv.TargetId,
                                                UpdateOn = cv.UpdateOn,
                                                UpdatedBy = cv.UpdatedBy,
                                                CreatedBy = cv.CreatedBy,
                                                CreatedOn = cv.CreatedOn,
                                            }).Where(w => w.TargetId == z.SchoolId).ToList()
                                        }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                                    }).ToList()
                }).ToList();
                List<SchoolViewForReport> schoolReports = new();
                foreach (var school in schoolMasterList)
                {
                    SchoolViewForReport schoolViewForReport = new();
                    schoolViewForReport.schoolMaster = school;
                    schoolViewForReport.StudentCount = this.context.StudentMaster.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && x.IsActive == true).Count();
                    schoolViewForReport.StaffCount = this.context.StaffSchoolInfo.Where(x => x.TenantId == school.TenantId && x.SchoolAttachedId == school.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).Count();
                    schoolViewForReport.ParentCount = this.context.ParentAssociationship.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && x.Associationship == true).Select(x => x.ParentId).Distinct().Count();
                    schoolReports.Add(schoolViewForReport);
                }

                if (schoolReports != null)
                {
                    schoolListForReport.schoolViewForReports = schoolReports;

                }
                else
                {
                    schoolListForReport.schoolViewForReports = new();
                }
            }
            catch (Exception ex)
            {
                schoolListForReport._failure = true;
                schoolListForReport._message = ex.Message;
            }

            return schoolListForReport;
        }
    }
}
