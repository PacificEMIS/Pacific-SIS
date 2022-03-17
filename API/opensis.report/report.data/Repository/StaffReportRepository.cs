using Microsoft.EntityFrameworkCore;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.StaffReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.Repository
{
    public class StaffReportRepository : IStaffReportRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffReportRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get All Staff Field List
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>      
        public StaffAdvancedReport StaffAdvancedReport(StaffAdvancedReport reportModel)
        {
            StaffAdvancedReport reportList = new();
            try
            {
                var schoolList = this.context?.SchoolMaster.Where(x => x.TenantId == reportModel.TenantId && x.SchoolId == reportModel.SchoolId);
                var countryLisy = this.context?.Country.ToList();
                if (schoolList?.Any() == true)
                {
                    reportList.schoolListForReport = schoolList.Select(x => new StaffSchoolReport()
                    {
                        TenantId = x.TenantId,
                        SchoolId = x.SchoolId,
                        SchoolGuid = x.SchoolGuid,
                        SchoolName = x.SchoolName,
                        SchoolLogo = x.SchoolDetail.FirstOrDefault().SchoolLogo,
                        SchoolStateId = x.SchoolStateId,
                        SchoolDistrictId = x.SchoolDistrictId,
                        City = x.City,
                        State = x.State,
                        StreetAddress1 = x.StreetAddress1,
                        StreetAddress2 = x.StreetAddress2,
                        Country = x.Country,
                        Division = x.Division,
                        District = x.District,
                        Zip = x.Zip,
                        Longitude = x.Longitude,
                        Latitude = x.Latitude
                    }).ToList();
                    List<StaffSchoolReport> schoolListReport = new();
                    foreach (var school in reportList.schoolListForReport)
                    {
                        StaffSchoolReport schoolReport = new();
                        schoolReport.TenantId = school.TenantId;
                        schoolReport.SchoolId = school.SchoolId;
                        schoolReport.SchoolGuid = school.SchoolGuid;
                        schoolReport.SchoolName = school.SchoolName;
                        schoolReport.SchoolLogo = school.SchoolLogo;
                        schoolReport.SchoolStateId = school.SchoolStateId;
                        schoolReport.SchoolDistrictId = school.SchoolDistrictId;
                        schoolReport.City = school.City;
                        schoolReport.State = school.State;
                        schoolReport.StreetAddress1 = school.StreetAddress1;
                        schoolReport.StreetAddress2 = school.StreetAddress2;
                        schoolReport.Country = school.Country;
                        schoolReport.Division = school.Division;
                        schoolReport.District = school.District;
                        schoolReport.Zip = school.Zip;
                        schoolReport.Longitude = school.Longitude;
                        schoolReport.Latitude = school.Latitude;
                        List<StaffReport> staffListForReport = new();

                        //var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == reportModel.TenantId && reportModel.StaffIds.Contains(x.StaffId.Value) && x.SchoolAttachedId == reportModel.SchoolId).Select(s => s.StaffId).ToList();

                       var staffData = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == reportModel.TenantId && reportModel.StaffIds.Contains(x.StaffId) /*&& (reportModel.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true)*/).ToList();

                        //var staffData = this.context?.StaffMaster.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && reportModel!.StaffGuids!.Contains(x.StaffGuid)).ToList();

                        foreach (var staff in staffData)
                        {
                            staff.StaffPhoto = null;
                            StaffReport staffReport = new();
                            var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
                        .Select(y => new FieldsCategory
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
                            UpdatedOn = y.UpdatedOn,
                            UpdatedBy = y.UpdatedBy,
                            CreatedBy = y.CreatedBy,
                            CreatedOn = y.CreatedOn,
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staff.StaffId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();
                            staffReport.fieldsCategoryList = customFields;
                            staffReport.staffMaster = staff;
                            staffReport.CountryOfBirth = staff.CountryOfBirth != null ? countryLisy.Where(x => x.Id == staff.CountryOfBirth).FirstOrDefault()?.Name : null;
                            staffReport.HomeAddressCountry = staff.HomeAddressCountry != null ? countryLisy.Where(x => x.Id.ToString() == staff.HomeAddressCountry).FirstOrDefault()?.Name : null;
                            staffReport.MailingAddressCountry = staff.MailingAddressCountry != null ? countryLisy.Where(x => x.Id.ToString() == staff.MailingAddressCountry).FirstOrDefault()?.Name : null;
                            staffListForReport.Add(staffReport);
                        }
                        schoolReport.staffListForReport = staffListForReport;
                        schoolListReport.Add(schoolReport);
                    }
                    reportList.schoolListForReport = schoolListReport;
                }
                else
                {
                    reportList._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                reportList._failure = true;
                reportList._message = ex.Message;
            }
            return reportList;
        }
    }
}
