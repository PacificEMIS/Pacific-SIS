﻿/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace opensis.data.Repository
{
    public class SchoolRepository : ISchoolRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public SchoolRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        //Get Full Address
        private static string ToFullAddress(string Address1, string Address2, string City, string State, string Country, string Zip)
        {
            string address = "";
            if (!string.IsNullOrWhiteSpace(Address1))
            {


                return address == null
                      ? null
                      : $"{Address1?.Trim()}{(!string.IsNullOrWhiteSpace(Address2) ? $", {Address2?.Trim()}" : string.Empty)}, {City?.Trim()}, {State?.Trim()} {Zip?.Trim()}";
            }
            return address;
        }
        
        /// <summary>
        /// Get SchoolsList with pagination
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public SchoolListModel GetAllSchoolList(PageResult pageResult)
        {
            int resultData;
            SchoolListModel schoolListModel = new SchoolListModel();
            IQueryable<SchoolMaster> transactionIQ = null;
            IQueryable<SchoolMaster> SchoolMasterList = null;

            var userMasterData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            if (userMasterData != null)
            {
                if (userMasterData.Membership != null && userMasterData.Membership.ProfileType.ToLower() == "Super Administrator".ToLower())
                {
                    SchoolMasterList = this.context?.SchoolMaster
                      .Include(d => d.SchoolDetail)
                      .Where(x => x.TenantId == pageResult.TenantId && (pageResult.IncludeInactive == false ? x.SchoolDetail.FirstOrDefault().Status == true : true));
                }
                else
                {
                    var staffSchoolInfoData = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == userMasterData.UserId).Select(s => s.SchoolAttachedId);

                    if (staffSchoolInfoData != null)
                    {
                        SchoolMasterList = this.context?.SchoolMaster
                                          .Include(d => d.SchoolDetail)
                                          .Where(x => x.TenantId == pageResult.TenantId && staffSchoolInfoData.Contains(x.SchoolId) && (pageResult.IncludeInactive == false ? x.SchoolDetail.FirstOrDefault().Status == true : true));
                    }
                }
            }

            //var SchoolMasterList = this.context?.SchoolMaster
            //         .Include(d => d.SchoolDetail)
            //         .Where(x => x.TenantId == pageResult.TenantId && (pageResult.IncludeInactive == false ? x.SchoolDetail.FirstOrDefault().Status == true : true));


            try
            {
                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {
                    //string sortField = "SchoolName"; string sortOrder = "desc";

                    transactionIQ = SchoolMasterList;
                }

                else
                {
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        transactionIQ = SchoolMasterList.Where(x => x.SchoolName.ToLower().Contains(Columnvalue.ToLower()) || x.StreetAddress1.ToLower().Contains(Columnvalue.ToLower()) || x.StreetAddress2.ToLower().Contains(Columnvalue.ToLower()) || x.Zip.ToLower().Contains(Columnvalue.ToLower()) || x.State.ToLower().Contains(Columnvalue.ToLower()) || x.City.ToLower().Contains(Columnvalue.ToLower()) || x.Country.ToLower().Contains(Columnvalue.ToLower()));

                        var childTelephoneFilter = SchoolMasterList.Where(x => x.SchoolDetail.FirstOrDefault() != null ? x.SchoolDetail.FirstOrDefault().Telephone.ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));

                        if (childTelephoneFilter.ToList().Count > 0)
                        {
                            transactionIQ = transactionIQ.Concat(childTelephoneFilter);
                        }

                        var childNameOfPrincipalFilter = SchoolMasterList.Where(x => x.SchoolDetail.FirstOrDefault() != null ? x.SchoolDetail.FirstOrDefault().NameOfPrincipal.ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));
                        if (childNameOfPrincipalFilter.ToList().Count > 0)
                        {
                            transactionIQ = transactionIQ.Concat(childNameOfPrincipalFilter);
                        }
                        //var countryFilter = this.context?.Country.Where(x => x.Name.ToLower().Contains(Columnvalue.ToLower()));
                        //if (countryFilter.ToList().Count > 0)
                        //{
                        //    foreach (var country in countryFilter.ToList())
                        //    {
                        //        var countrySearch = SchoolMasterList.Where(x => x.Country == country.Id.ToString());

                        //        if (countrySearch.ToList().Count > 0)
                        //        {
                        //            transactionIQ = transactionIQ.Concat(countrySearch);
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        transactionIQ = Utility.FilteredData(pageResult.FilterParams, SchoolMasterList).AsQueryable();
                    }

                    transactionIQ = transactionIQ.Distinct();
                }


                if (pageResult.SortingModel != null)
                {
                    switch (pageResult.SortingModel.SortColumn.ToLower())
                    {
                        case "nameofprincipal":

                            if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                            {

                                transactionIQ = transactionIQ.OrderBy(a => a.SchoolDetail.Count > 0 ? a.SchoolDetail.FirstOrDefault().NameOfPrincipal : null);
                            }
                            else
                            {
                                transactionIQ = transactionIQ.OrderByDescending(a => a.SchoolDetail.Count > 0 ? a.SchoolDetail.FirstOrDefault().NameOfPrincipal : null);
                            }
                            break;

                        default:
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                            break;
                    }

                }

                int totalCount = transactionIQ.Count();
                if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                {
                    transactionIQ = transactionIQ.Select(p => new SchoolMaster
                    {
                        SchoolId = p.SchoolId,
                        TenantId = p.TenantId,
                        SchoolName = p.SchoolName.Trim(),
                        Zip = p.Zip,
                        StreetAddress1 = p.StreetAddress1,
                        StreetAddress2 = p.StreetAddress2,
                        State = p.State,
                        City = p.City,
                        Country = p.Country,
                        CreatedBy = (p.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>  u.TenantId == pageResult.TenantId && u.EmailAddress == p.CreatedBy).Name : null,
                        CreatedOn = p.CreatedOn,
                        UpdatedBy = (p.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>  u.TenantId == pageResult.TenantId && u.EmailAddress == p.UpdatedBy).Name : null,
                        UpdatedOn = p.UpdatedOn,
                        SchoolDetail = p.SchoolDetail.Select(s => new SchoolDetail
                        {
                            Telephone = s.Telephone,
                            NameOfPrincipal = s.NameOfPrincipal,
                            Status = s.Status,
                            CreatedBy= (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                            CreatedOn =s.CreatedOn,
                            UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,
                            UpdatedOn = s.UpdatedOn
                        }).ToList()
                    }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }
                //var schoollist = transactionIQ.AsNoTracking().Select(s => new GetSchoolForView
                //{
                //    SchoolId = s.SchoolId,
                //    SchoolName = s.SchoolName,
                //    TenantId = s.TenantId,
                //    Telephone = s.SchoolDetail.FirstOrDefault() == null ? string.Empty : s.SchoolDetail.FirstOrDefault().Telephone == null ? string.Empty : s.SchoolDetail.FirstOrDefault().Telephone.Trim(),
                //    NameOfPrincipal = s.SchoolDetail.FirstOrDefault() == null ? string.Empty : s.SchoolDetail.FirstOrDefault().NameOfPrincipal == null ? string.Empty : s.SchoolDetail.FirstOrDefault().NameOfPrincipal.Trim(),
                //    StreetAddress1 = s.SchoolDetail.FirstOrDefault() == null ? string.Empty : ToFullAddress(s.StreetAddress1, s.StreetAddress2,
                //    int.TryParse(s.City, out resultData) == true ? this.context.City.Where(x => x.Id == Convert.ToInt32(s.City)).FirstOrDefault().Name : s.City,
                //    int.TryParse(s.State, out resultData) == true ? this.context.State.Where(x => x.Id == Convert.ToInt32(s.State)).FirstOrDefault().Name : s.State,
                //    int.TryParse(s.Country, out resultData) == true ? this.context.Country.Where(x => x.Id == Convert.ToInt32(s.Country)).FirstOrDefault().Name : string.Empty, s.Zip),
                //    Status = s.SchoolDetail.FirstOrDefault() == null ? false : s.SchoolDetail.FirstOrDefault().Status == null ? false : s.SchoolDetail.FirstOrDefault().Status
                //}).ToList();

                schoolListModel.TenantId = pageResult.TenantId;
                //schoolListModel.GetSchoolForView = schoollist;
                schoolListModel.schoolMaster = transactionIQ.ToList();
                schoolListModel.TotalCount = totalCount;
                schoolListModel.PageNumber = pageResult.PageNumber;
                schoolListModel._pageSize = pageResult.PageSize;
                schoolListModel._tenantName = pageResult._tenantName;
                schoolListModel._token = pageResult._token;
                schoolListModel._failure = false;
            }
            catch (Exception es)
            {
                schoolListModel._message = es.Message;
                schoolListModel._failure = true;
                schoolListModel._tenantName = pageResult._tenantName;
                schoolListModel._token = pageResult._token;
            }
            return schoolListModel;

        }

        /// <summary>
        /// Get All school for dropdown
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolListModel GetAllSchools(SchoolListModel school)
        {
            SchoolListModel schoolListModel = new SchoolListModel();
            try
            {
                var userMasterData = this.context?.UserMaster.Include(c=>c.Membership).FirstOrDefault(x => x.TenantId == school.TenantId && x.EmailAddress == school.EmailAddress);

                if (userMasterData != null)
                {
                    if (userMasterData.Membership!=null && userMasterData.Membership.ProfileType.ToLower() == "Super Administrator".ToLower())
                    {
                        var schoolList = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Where(x => x.TenantId == school.TenantId && x.SchoolDetail.FirstOrDefault().Status != false).OrderBy(x => x.SchoolName)
                  .Select(e => new SchoolMaster()
                  {
                      SchoolId = e.SchoolId,
                      TenantId = e.TenantId,
                      SchoolName = e.SchoolName.Trim(),
                      CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                      CreatedOn =e.CreatedOn,
                      UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                      UpdatedOn =e.UpdatedOn,
                      SchoolDetail = e.SchoolDetail.Select(s => new SchoolDetail
                      {
                          DateSchoolOpened = s.DateSchoolOpened,
                          DateSchoolClosed = s.DateSchoolClosed,
                          CreatedBy= (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                          CreatedOn =s.CreatedOn,
                          UpdatedBy= (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,
                          UpdatedOn =s.UpdatedOn
                      }).ToList()
                  }).ToList();

                        schoolListModel.schoolMaster = schoolList;
                    }
                    else
                    {
                        var schoolList = this.context?.SchoolMaster.
                                    Join(this.context?.StaffSchoolInfo,
                                    sm => sm.SchoolId, ssi => ssi.SchoolAttachedId,
                                    (sm, ssi) => new { sm, ssi }).Where(c => c.sm.TenantId == school.TenantId && c.ssi.TenantId == school.TenantId && c.sm.SchoolDetail.FirstOrDefault().Status != false && c.ssi.StaffId == userMasterData.UserId && (c.ssi.EndDate == null || c.ssi.EndDate >= DateTime.UtcNow.Date) && (c.ssi.StartDate <= DateTime.UtcNow.Date)).OrderBy(x => x.sm.SchoolName).Select(e => new SchoolMaster()
                                    {
                                        SchoolId = e.sm.SchoolId,
                                        TenantId = e.sm.TenantId,
                                        SchoolName = e.sm.SchoolName.Trim(),
                                        CreatedBy = (e.sm.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == e.sm.CreatedBy).Name : null,
                                        CreatedOn = e.sm.CreatedOn,
                                        UpdatedBy = (e.sm.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == e.sm.UpdatedBy).Name : null,
                                        UpdatedOn = e.sm.UpdatedOn,
                                        SchoolDetail = e.sm.SchoolDetail.Select(s => new SchoolDetail
                                        {
                                            DateSchoolOpened = s.DateSchoolOpened,
                                            DateSchoolClosed = s.DateSchoolClosed,
                                            CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                                            CreatedOn = s.CreatedOn,
                                            UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,
                                            UpdatedOn = s.UpdatedOn
                                        }).ToList()
                                    }).ToList();
                        schoolListModel.schoolMaster = schoolList;

                    }
                }
                else
                {
                    schoolListModel.schoolMaster = null;
                }

                schoolListModel.PageNumber = null;
                schoolListModel._pageSize = null;
                schoolListModel._tenantName = school._tenantName;
                schoolListModel._token = school._token;
                schoolListModel._failure = false;
            }
            catch (Exception es)
            {
                schoolListModel._message = es.Message;
                schoolListModel._failure = true;
                schoolListModel._tenantName = school._tenantName;
                schoolListModel._token = school._token;
            }
            return schoolListModel;

        }
        
        /// <summary>
        /// Get School by id
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolAddViewModel ViewSchool(SchoolAddViewModel school)
        {
            try
            {
                SchoolAddViewModel SchoolAddViewModel = new SchoolAddViewModel();
                var schoolMaster = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.FieldsCategory).ThenInclude(x => x.CustomFields).ThenInclude(x => x.CustomFieldsValue).Include(x=>x.SchoolYears).FirstOrDefault(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId);

                if (schoolMaster != null)
                {
                    var customFields = schoolMaster.FieldsCategory.Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId && x.Module == "School").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                                        CreatedBy= (y.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == y.CreatedBy).Name : null,
                                        CreatedOn =y.CreatedOn,
                                        UpdatedOn = y.UpdatedOn,
                                        UpdatedBy = (y.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == y.UpdatedBy).Name : null,
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
                                            UpdatedBy = (z.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == z.UpdatedBy).Name : null,
                                            CreatedBy = (z.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == z.CreatedBy).Name : null,
                                            CreatedOn = z.CreatedOn,
                                            CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == school.schoolMaster.SchoolId).Select(e=> new CustomFieldsValue()
                                            {
                                                TenantId=e.TenantId,
                                                SchoolId=e.SchoolId,
                                                CategoryId=e.CategoryId,
                                                FieldId=e.FieldId,
                                                TargetId=e.TargetId,
                                                Module=e.Module,
                                                CustomFieldTitle=e.CustomFieldTitle,
                                                CustomFieldType=e.CustomFieldType,
                                                CustomFieldValue=e.CustomFieldValue,
                                                CreatedOn=e.CreatedOn,
                                                UpdateOn=e.UpdateOn,
                                                UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                                                CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                                            }).ToList()
                                        }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                                    }).ToList();


                    school.schoolMaster = new SchoolMaster()
                    {
                        TenantId= schoolMaster.TenantId,
                        SchoolName=schoolMaster.SchoolName,                        
                        SchoolId= schoolMaster.SchoolId,
                        AlternateName=schoolMaster.AlternateName,
                        SchoolAltId= schoolMaster.SchoolAltId,
                        City= schoolMaster.City,
                        Country= schoolMaster.Country,
                        County= schoolMaster.County,
                        CurrentPeriodEnds= schoolMaster.CurrentPeriodEnds,
                        District= schoolMaster.District,
                        Division= schoolMaster.Division,
                        Features= schoolMaster.Features,
                        Latitude= schoolMaster.Latitude,
                        Longitude= schoolMaster.Longitude,
                        MaxApiChecks= schoolMaster.MaxApiChecks,
                        Zip= schoolMaster.Zip,
                        State= schoolMaster.State,
                        SchoolDistrictId= schoolMaster.SchoolDistrictId,
                        SchoolGuid= schoolMaster.SchoolGuid,
                        SchoolClassification= schoolMaster.SchoolClassification,
                        StreetAddress1= schoolMaster.StreetAddress1,
                        StreetAddress2= schoolMaster.StreetAddress2,
                        PlanId= schoolMaster.PlanId,
                        SchoolStateId= schoolMaster.SchoolStateId,
                        SchoolInternalId= schoolMaster.SchoolInternalId,
                        SchoolLevel= schoolMaster.SchoolLevel,
                        SchoolDetail=schoolMaster.SchoolDetail.Select(v=> new SchoolDetail()
                        { 
                            TenantId=v.TenantId,
                            SchoolId=v.SchoolId,
                            Id=v.Id,
                            Affiliation=v.Affiliation,
                            Associations=v.Associations,
                            CommonToiletAccessibility=v.CommonToiletAccessibility,
                            ComonToiletType=v.ComonToiletType,
                            CurrentlyAvailable=v.CurrentlyAvailable,
                            DateSchoolClosed=v.DateSchoolClosed,
                            DateSchoolOpened=v.DateSchoolOpened,
                            Electricity=v.Electricity,
                            Email=v.Email,
                            Facebook=v.Facebook,
                            Fax=v.Fax,
                            FemaleToiletAccessibility=v.FemaleToiletAccessibility,
                            FemaleToiletType=v.FemaleToiletType,
                            Gender=v.Gender,
                            HandwashingAvailable=v.HandwashingAvailable,
                            HighestGradeLevel=v.HighestGradeLevel,
                            HygeneEducation=v.HygeneEducation,
                            Internet=v.Internet,
                            Instagram=v.Instagram,
                            LinkedIn=v.LinkedIn,
                            Locale=v.Locale,
                            LowestGradeLevel=v.LowestGradeLevel,
                            MainSourceOfDrinkingWater=v.MainSourceOfDrinkingWater,
                            MaleToiletAccessibility=v.MaleToiletAccessibility,
                            NameOfAssistantPrincipal=v.NameOfAssistantPrincipal,
                            MaleToiletType=v.MaleToiletType,
                            NameOfPrincipal=v.NameOfPrincipal,
                            RunningWater=v.RunningWater,
                            SchoolLogo=v.SchoolLogo,
                            SoapAndWaterAvailable=v.SoapAndWaterAvailable,
                            Status=v.Status,
                            Telephone=v.Telephone,
                            TotalCommonToilets=v.TotalCommonToilets,
                            TotalCommonToiletsUsable=v.TotalCommonToiletsUsable,
                            TotalFemaleToilets=v.TotalFemaleToilets,
                            TotalFemaleToiletsUsable=v.TotalFemaleToiletsUsable,
                            TotalMaleToilets=v.TotalMaleToilets,
                            TotalMaleToiletsUsable=v.TotalMaleToiletsUsable,
                            Twitter=v.Twitter,
                            Website=v.Website,
                            Youtube=v.Youtube,
                            CreatedOn=v.CreatedOn,
                            UpdatedOn=v.UpdatedOn,
                            UpdatedBy = (v.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == v.UpdatedBy).Name : null,
                            CreatedBy = (v.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == v.CreatedBy).Name : null,
                        }).ToList(),                        
                        CreatedOn= schoolMaster.CreatedOn,
                        UpdatedOn= schoolMaster.UpdatedOn,
                        UpdatedBy = (schoolMaster.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == schoolMaster.UpdatedBy).Name : null,
                        CreatedBy = (schoolMaster.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == school.schoolMaster.TenantId && u.EmailAddress == schoolMaster.CreatedBy).Name : null,                        
                    };
                    school.schoolMaster.FieldsCategory = customFields;
                    //school.schoolMaster.FieldsCategory.FirstOrDefault().CustomFields = schoolcustomFields;
                    if (school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.FirstOrDefault().SchoolMaster = null;
                    }
                    if (school.schoolMaster.CustomFields.ToList().Count > 0)
                    {
                        school.schoolMaster.CustomFields = null;
                    }

                    if (schoolMaster.SchoolYears.Count > 0)
                    {
                        school.IsMarkingPeriod = true;
                    }
                    else
                    {
                        school.IsMarkingPeriod = false;
                    }

                    school._tenantName = school._tenantName;
                    return school;
                }
                else
                {
                    SchoolAddViewModel._failure = true;
                    SchoolAddViewModel._message = NORECORDFOUND;
                    return SchoolAddViewModel;
                }
            }
            catch (Exception es)
            {

                throw;
            }

        }

        /// <summary>
        /// Update School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolAddViewModel UpdateSchool(SchoolAddViewModel school)
        {
            try
            {
                var schoolMaster = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.FieldsCategory).ThenInclude(x => x.CustomFields).ThenInclude(x => x.CustomFieldsValue).FirstOrDefault(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId);

                var checkInternalId = this.context?.SchoolMaster.Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolInternalId == school.schoolMaster.SchoolInternalId && x.SchoolInternalId != null && x.SchoolId != school.schoolMaster.SchoolId).ToList();

                if (checkInternalId.Count() > 0)
                {
                    school.schoolMaster = null;
                    school._failure = true;
                    school._message = "School InternalID Already Exist";
                }
                else
                {
                    if (string.IsNullOrEmpty(school.schoolMaster.SchoolInternalId))
                    {
                        school.schoolMaster.SchoolInternalId = schoolMaster.SchoolInternalId;
                    }
                    school.schoolMaster.CreatedBy = schoolMaster.CreatedBy;
                    school.schoolMaster.CreatedOn = schoolMaster.CreatedOn;
                    school.schoolMaster.SchoolGuid = schoolMaster.SchoolGuid;
                    school.schoolMaster.PlanId = schoolMaster.PlanId;
                    school.schoolMaster.UpdatedOn = DateTime.UtcNow;
                    this.context.Entry(schoolMaster).CurrentValues.SetValues(school.schoolMaster);

                    if (schoolMaster.SchoolDetail.ToList().Count == 0 && school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.Id = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id)));
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.SchoolId = school.schoolMaster.SchoolId);
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.TenantId = school.schoolMaster.TenantId);
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.UpdatedOn = DateTime.UtcNow);
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.UpdatedBy = school.schoolMaster.UpdatedBy);
                        this.context?.SchoolDetail.AddRange(school.schoolMaster.SchoolDetail);
                    }

                    if (schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        foreach (var detailes in schoolMaster.SchoolDetail.ToList())
                        {
                            var markingPeriodExist = this.context?.SchoolYears.FirstOrDefault(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId);
                            if (markingPeriodExist != null)
                            {
                                school.schoolMaster.SchoolDetail.FirstOrDefault().DateSchoolOpened = schoolMaster.SchoolDetail.FirstOrDefault().DateSchoolOpened;
                            }

                            this.context.Entry(schoolMaster.SchoolDetail.FirstOrDefault()).CurrentValues.SetValues(school.schoolMaster.SchoolDetail.FirstOrDefault());
                        }
                    }

                    //Student Custom Field value With Delete
                    if (school.schoolMaster.FieldsCategory != null && school.schoolMaster.FieldsCategory.ToList().Count > 0)
                    {
                        var fieldsCategory = school.schoolMaster.FieldsCategory.FirstOrDefault(x => x.CategoryId == school.SelectedCategoryId);

                        if (fieldsCategory != null)
                        {
                            foreach (var customFields in fieldsCategory.CustomFields.ToList())
                            {
                                var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "School" && x.TargetId == school.schoolMaster.SchoolId);
                                if (customFieldValueData != null)
                                {
                                    this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                }

                                if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                {
                                    customFields.CustomFieldsValue.FirstOrDefault().Module = "School";
                                    customFields.CustomFieldsValue.FirstOrDefault().CategoryId = customFields.CategoryId;
                                    customFields.CustomFieldsValue.FirstOrDefault().FieldId = customFields.FieldId;
                                    customFields.CustomFieldsValue.FirstOrDefault().CustomFieldTitle = customFields.Title;
                                    customFields.CustomFieldsValue.FirstOrDefault().CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault().SchoolId = school.schoolMaster.SchoolId;
                                    customFields.CustomFieldsValue.FirstOrDefault().TenantId = school.schoolMaster.TenantId;
                                    customFields.CustomFieldsValue.FirstOrDefault().TargetId = school.schoolMaster.SchoolId;
                                    customFields.CustomFieldsValue.FirstOrDefault().CustomFields = null;
                                    this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                }
                            }

                        }
                    }
                    this.context?.SaveChanges();
                    if (school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.FirstOrDefault().SchoolMaster = null;
                    }

                    if (school.schoolMaster.FieldsCategory!=null && school.schoolMaster.FieldsCategory.ToList().Count > 0)
                    {
                        foreach (var FieldsCategory in school.schoolMaster.FieldsCategory)
                        {
                            foreach (var customFields in FieldsCategory.CustomFields)
                            {
                                customFields.CustomFieldsValue.FirstOrDefault().CustomFields = null;
                            }
                        }
                    }

                    school._failure = false;
                    school._message = "School Updated Successfully";
                }
                return school;
            }
            catch (Exception ex)
            {
                school.schoolMaster = null;
                school._failure = true;
                school._message = NORECORDFOUND;
                return school;
            }

        }

        /// <summary>
        /// Add School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolAddViewModel AddSchool(SchoolAddViewModel school)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    //int? MasterSchoolId = Utility.GetMaxPK(this.context, new Func<SchoolMaster, int>(x => x.SchoolId));
                    int? MasterSchoolId = 1;

                    var schoolData = this.context?.SchoolMaster.Where(x => x.TenantId == school.schoolMaster.TenantId).OrderByDescending(x => x.SchoolId).FirstOrDefault();

                    if (schoolData != null)
                    {
                        MasterSchoolId = schoolData.SchoolId + 1;
                    }
                    //int? MemberShipId = Utility.GetMaxPK(this.context, new Func<Membership, int>(x => x.MembershipId));
                    //int? CategoryId = Utility.GetMaxPK(this.context, new Func<FieldsCategory, int>(x => x.CategoryId));
                    school.schoolMaster.SchoolId = (int)MasterSchoolId;
                    Guid GuidId = Guid.NewGuid();
                    var GuidIdExist = this.context?.SchoolMaster.FirstOrDefault(x => x.SchoolGuid == GuidId);
                    if (GuidIdExist != null)
                    {
                        school._failure = true;
                        school._message = "Guid is already exist, Please try again.";
                        return school;
                    }
                    school.schoolMaster.SchoolGuid = GuidId;

                    if (school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.Id = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id)));
                    }
                    school.schoolMaster.CreatedOn = DateTime.UtcNow;
                    school.schoolMaster.TenantId = school.schoolMaster.TenantId;

                    if (!string.IsNullOrEmpty(school.schoolMaster.SchoolInternalId))
                    {
                        bool checkInternalID = CheckInternalID(school.schoolMaster.TenantId, school.schoolMaster.SchoolInternalId);
                        if (checkInternalID == false)
                        {
                            school.schoolMaster = null;
                            school._failure = true;
                            school._message = "School InternalID Already Exist";
                            return school;
                        }
                    }
                    else
                    {
                        school.schoolMaster.SchoolInternalId = MasterSchoolId.ToString();
                    }

                    school.schoolMaster.Membership = new List<Membership>() {
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Super Administrator", IsActive= true, IsSuperadmin= true, IsSystem= true, MembershipId= 1, ProfileType= "Super Administrator"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "School Administrator", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 2, ProfileType= "School Administrator"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Admin Assistant", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 3, ProfileType= "Admin Assistant"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 4, ProfileType= "Teacher"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Homeroom Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 5, ProfileType= "Homeroom Teacher"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Parent", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 6, ProfileType= "Parent"},
                    new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,Profile= "Student", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 7, ProfileType= "Student"},
                };

                    long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));

                    school.schoolMaster.DpdownValuelist = new List<DpdownValuelist>() {
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="PK",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="K",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+1},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="1",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+2},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="2",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+3},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="3",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+4},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="4",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+5},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="5",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+6},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="6",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+7},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="7",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+8},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="8",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+9},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="9",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+10},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="10",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+11},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="11",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+12},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="12",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+13},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="13",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+14},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="14",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+15},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="15",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+16},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="16",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+17},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="17",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+18},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="18",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+19},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="19",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+20},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="20",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+21},


                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Boys",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+22},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Girls",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+23},
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Mixed",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+24},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Mr.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+25},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Miss.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+26},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Mrs.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+27},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Ms.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+28},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Dr.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+29},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Rev.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+30},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Prof.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+31},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Sir.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+32},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Lord ",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+33},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="Jr.",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+34},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="Sr",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+35},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="II",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+37},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="III",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+38},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="IV",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+39},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="V",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+40},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="PhD",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+41},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Male",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+42},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Female",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+43},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Other",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+44},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Single",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+45},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Married",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+46},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Partnered",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+47},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Next grade at current school",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+48},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Retain",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+49},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Do not enroll after this school year",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+50},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Mother",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+51},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Father",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+52},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Legal Guardian",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+53},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Other",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+54},


                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Add",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+55},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Drop",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+56},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Rolled Over",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+57},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Drop (Transfer)",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+58},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Enroll (Transfer)",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+59},


                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Dropdown",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+60},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Editable Dropdown",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+61},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Text",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+62},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Checkbox",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+63},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Number",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+64},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Multiple SelectBox",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+65},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Date",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+66},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Textarea",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+67},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Level",LovColumnValue=school.schoolMaster.SchoolLevel,CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+68},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Classification",LovColumnValue=school.schoolMaster.SchoolClassification,CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+69}
                };

                    school.schoolMaster.FieldsCategory = new List<FieldsCategory>()
                {
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Information",Module="School",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=1},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Wash Information",Module="School",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=2},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Student",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=3},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Enrollment Info",Module="Student",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=4},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Student",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=5},

                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Family Info",Module="Student",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=6},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Medical Info",Module="Student",SortOrder=5,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=7},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Comments",Module="Student",SortOrder=6,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=8},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Documents",Module="Student",SortOrder=7,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=9},

                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Parent",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=10},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address Info",Module="Parent",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=11},

                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Staff",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=12},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="School Info",Module="Staff",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=13},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Staff",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=14},
                    new FieldsCategory(){ TenantId=school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Certification Info",Module="Staff",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=school.schoolMaster.CreatedBy,CategoryId=15}
                };

                    school.schoolMaster.StudentEnrollmentCode = new List<StudentEnrollmentCode>()
                {
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=1, Title="New", ShortName="NEW", SortOrder=1, Type="Add", CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=2, Title="Dropped Out", ShortName="DROP", SortOrder=2, Type="Drop", CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=3, Title="Rolled Over", ShortName="ROLL", SortOrder=3, Type="Rolled Over", CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=4, Title="Transferred In", ShortName="TRAN", SortOrder=4, Type="Enroll (Transfer)", CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=5, Title="Transferred Out", ShortName="TRAN", SortOrder=5, Type="Drop (Transfer)", CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy }
                };

                    school.schoolMaster.Block = new List<Block>()
                {
                     new Block(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy }
                };

                    ReleaseNumber releaseNumber = new ReleaseNumber();
                    {
                        releaseNumber.TenantId = school.schoolMaster.TenantId;
                        releaseNumber.SchoolId = school.schoolMaster.SchoolId;
                        releaseNumber.ReleaseNumber1 = "1.0.0";
                        releaseNumber.ReleaseDate = DateTime.UtcNow;
                        releaseNumber.CreatedBy = school.schoolMaster.CreatedBy;
                        releaseNumber.CreatedOn = DateTime.UtcNow;
                    }

                    //insert into permission group
                    var dataGroup = System.IO.File.ReadAllText(@"Group.json");
                    JsonSerializerSettings settingGrp = new JsonSerializerSettings();
                    List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp);

                    foreach (PermissionGroup permisionGrp in objGroup)
                    {

                        permisionGrp.TenantId = school.schoolMaster.TenantId;
                        permisionGrp.SchoolId = school.schoolMaster.SchoolId;
                        permisionGrp.CreatedBy = school.schoolMaster.CreatedBy;
                        permisionGrp.CreatedOn = school.schoolMaster.CreatedOn;
                        //permisionGrp.IsActive = true;
                        permisionGrp.PermissionCategory = null;
                        this.context?.PermissionGroup.Add(permisionGrp);
                        //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                    }

                    //insert into system default custom fields
                    var dataCustomFields = System.IO.File.ReadAllText(@"CustomFields.json");
                    JsonSerializerSettings settingCusFld = new JsonSerializerSettings();
                    List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld);
                    foreach (CustomFields customFields in objCusFld)
                    {
                        customFields.TenantId = school.schoolMaster.TenantId;
                        customFields.SchoolId = school.schoolMaster.SchoolId;
                        customFields.CreatedBy = school.schoolMaster.CreatedBy;
                        customFields.CreatedOn = DateTime.UtcNow;
                        this.context?.CustomFields.Add(customFields);
                        //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                    }

                    //insert into permission category
                    var dataCategory = System.IO.File.ReadAllText(@"Category.json");
                    JsonSerializerSettings settingCat = new JsonSerializerSettings();
                    List<PermissionCategory> objCat = JsonConvert.DeserializeObject<List<PermissionCategory>>(dataCategory, settingCat);
                    foreach (PermissionCategory permissionCate in objCat)
                    {
                        permissionCate.TenantId = school.schoolMaster.TenantId;
                        permissionCate.SchoolId = school.schoolMaster.SchoolId;
                        permissionCate.PermissionGroup = null;
                        permissionCate.RolePermission = null;
                        permissionCate.CreatedBy = school.schoolMaster.CreatedBy;
                        permissionCate.CreatedOn = DateTime.UtcNow;
                        this.context?.PermissionCategory.Add(permissionCate);
                        //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                    }

                    //insert into permission subcategory
                    var dataSubCategory = System.IO.File.ReadAllText(@"SubCategory.json");
                    JsonSerializerSettings settingSubCat = new JsonSerializerSettings();
                    List<PermissionSubcategory> objSubCat = JsonConvert.DeserializeObject<List<PermissionSubcategory>>(dataSubCategory, settingSubCat);
                    foreach (PermissionSubcategory permissionSubCate in objSubCat)
                    {
                        permissionSubCate.TenantId = school.schoolMaster.TenantId;
                        permissionSubCate.SchoolId = school.schoolMaster.SchoolId;
                        permissionSubCate.RolePermission = null;
                        permissionSubCate.CreatedBy = school.schoolMaster.CreatedBy;
                        permissionSubCate.CreatedOn = DateTime.UtcNow;
                        this.context?.PermissionSubcategory.Add(permissionSubCate);
                        //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                    }

                    //insert into role permission
                    var dataRolePermission = System.IO.File.ReadAllText(@"RolePermission.json");
                    JsonSerializerSettings settingRole = new JsonSerializerSettings();
                    List<RolePermission> objRole = JsonConvert.DeserializeObject<List<RolePermission>>(dataRolePermission, settingRole);
                    foreach (RolePermission permissionRole in objRole)
                    {
                        permissionRole.TenantId = school.schoolMaster.TenantId;
                        permissionRole.SchoolId = school.schoolMaster.SchoolId;
                        permissionRole.PermissionCategory = null;
                        permissionRole.Membership = null;
                        permissionRole.CreatedBy = school.schoolMaster.CreatedBy;
                        permissionRole.CreatedOn = DateTime.UtcNow;
                        this.context?.RolePermission.Add(permissionRole);
                        //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                    }

                    var customFieldCategories = this.context?.FieldsCategory.Include(x => x.CustomFields).Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.SchoolId && x.IsSystemCategory != true).ToList();
                    if (customFieldCategories.Count > 0)
                    {
                        foreach (var customFieldCategoriesData in customFieldCategories)
                        {
                            var customFields = new FieldsCategory() 
                            { 
                                TenantId = school.schoolMaster.TenantId, 
                                SchoolId = school.schoolMaster.SchoolId, 
                                CategoryId = customFieldCategoriesData.CategoryId, 
                                IsSystemCategory = false, 
                                Search = customFieldCategoriesData.Search, 
                                Title = customFieldCategoriesData.Title, 
                                Module = customFieldCategoriesData.Module, 
                                SortOrder = customFieldCategoriesData.SortOrder, 
                                Required = customFieldCategoriesData.Required, 
                                Hide = customFieldCategoriesData.Hide, 
                                IsSystemWideCategory = true, 
                                CreatedBy = school.schoolMaster.CreatedBy, 
                                CreatedOn = DateTime.UtcNow,
                                CustomFields= customFieldCategoriesData.CustomFields.Where(x => x.IsSystemWideField == true).Select(s=>new CustomFields
                                {
                                    TenantId = school.schoolMaster.TenantId,
                                    SchoolId = school.schoolMaster.SchoolId,
                                    FieldId = s.FieldId,
                                    Type = s.Type,
                                    Search = s.Search,
                                    Title = s.Title,
                                    SortOrder = s.SortOrder,
                                    SelectOptions = s.SelectOptions,
                                    CategoryId = s.CategoryId,
                                    SystemField = s.SystemField,
                                    Required = s.Required,
                                    DefaultSelection = s.DefaultSelection,
                                    Hide = s.Hide,
                                    Module = s.Module,
                                    FieldName = s.FieldName,
                                    IsSystemWideField = true,
                                    CreatedBy = school.schoolMaster.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                }).ToList()
                            };

                            school.schoolMaster.FieldsCategory.Add(customFields);
                        }
                    }

                    this.context?.SchoolMaster.Add(school.schoolMaster);
                    this.context?.ReleaseNumber.Add(releaseNumber);

                    //Insert data in staff_school_info table for School Admin & Admin Assistant
                    var userMasterData = this.context?.UserMaster.Include(x=>x.Membership).Where(x => x.TenantId==school.schoolMaster.TenantId && x.EmailAddress==school.EmailAddress).ToList();

                    if (userMasterData.Count > 0)
                    {
                        if (userMasterData.FirstOrDefault().Membership.ProfileType.ToLower() != "Super Administrator".ToLower())
                        {
                            int? Id = 0;
                            Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));

                            StaffSchoolInfo staffSchoolInfo = new StaffSchoolInfo();
                            {
                                staffSchoolInfo.TenantId = school.schoolMaster.TenantId;
                                staffSchoolInfo.SchoolId = userMasterData.FirstOrDefault().SchoolId;
                                staffSchoolInfo.StaffId = userMasterData.FirstOrDefault().UserId;
                                staffSchoolInfo.SchoolAttachedId = school.schoolMaster.SchoolId;
                                staffSchoolInfo.SchoolAttachedName = school.schoolMaster.SchoolName;
                                staffSchoolInfo.Profile = userMasterData.FirstOrDefault().Membership.Profile;
                                staffSchoolInfo.StartDate = DateTime.UtcNow;
                                staffSchoolInfo.CreatedBy = school.schoolMaster.CreatedBy;
                                staffSchoolInfo.CreatedOn = DateTime.UtcNow;
                                staffSchoolInfo.Id = (int)Id;
                            }
                            this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                        }
                    }
                    
                    this.context?.SaveChanges();
                    transaction.Commit();
                    school._failure = false;
                    school._message = "School Added Successfully";

                    school.schoolMaster.Membership = null;
                    school.schoolMaster.DpdownValuelist = null;
                    school.schoolMaster.PermissionGroup = null;
                    school.schoolMaster.CustomFields = null;
                    school.schoolMaster.FieldsCategory = null;

                    //school.schoolMaster.Membership.ToList().ForEach(x=>x.SchoolMaster=null);
                    /*if (school.schoolMaster.SchoolDetail.ToList().Count>0)
                    {
                        school.schoolMaster.SchoolDetail.FirstOrDefault().SchoolMaster = null;
                    }*/
                    //school.schoolMaster.FieldsCategory.ToList().ForEach(x => x.SchoolMaster = null);
                    /*if (school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.FirstOrDefault().SchoolMaster = null;
                    }*/

                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    school._failure = true;
                    school._message = es.Message;
                }
            }
            return school;
        }

        //checking Internal Id
        private bool CheckInternalID(Guid TenantId, string InternalID)
        {
            if (InternalID != null && InternalID != "")
            {
                var checkInternalId = this.context?.SchoolMaster.Where(x => x.TenantId == TenantId && x.SchoolInternalId == InternalID).ToList();
                if (checkInternalId.Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        ///  Check School Internal Id Exist or Not
        /// </summary>
        /// <param name="checkSchoolInternalIdViewModel"></param>
        /// <returns></returns>
        public CheckSchoolInternalIdViewModel CheckSchoolInternalId(CheckSchoolInternalIdViewModel checkSchoolInternalIdViewModel)
        {
            var checkInternalId = this.context?.SchoolMaster.Where(x => x.TenantId == checkSchoolInternalIdViewModel.TenantId && x.SchoolInternalId == checkSchoolInternalIdViewModel.SchoolInternalId).ToList();
            if (checkInternalId.Count() > 0)
            {
                checkSchoolInternalIdViewModel.IsValidInternalId = false;
                checkSchoolInternalIdViewModel._message = "School Internal Id Already Exist";
            }
            else
            {
                checkSchoolInternalIdViewModel.IsValidInternalId = true;
                checkSchoolInternalIdViewModel._message = "School Internal Id Is Valid";
            }
            return checkSchoolInternalIdViewModel;
        }
        
        /// <summary>
        /// Student Enrollment School List
        /// </summary>
        /// <param name="schoolListViewModel"></param>
        /// <returns></returns>
        public SchoolListViewModel StudentEnrollmentSchoolList(SchoolListViewModel schoolListViewModel)
        {
            SchoolListViewModel schoolListView = new SchoolListViewModel();
            try
            {
                var schoolListWithGradeLevel = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.Gradelevels).Include(x => x.StudentEnrollmentCode).Where(x => x.TenantId == schoolListViewModel.TenantId && x.SchoolDetail.FirstOrDefault().Status == true).Select(e=> new SchoolMaster()
                {
                    TenantId = e.TenantId,
                    SchoolId = e.SchoolId,
                    AlternateName = e.AlternateName,
                    SchoolAltId = e.SchoolAltId,
                    City = e.City,
                    Country = e.Country,
                    County = e.County,
                    CurrentPeriodEnds = e.CurrentPeriodEnds,
                    District = e.District,
                    Division = e.Division,
                    Features = e.Features,
                    Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    MaxApiChecks = e.MaxApiChecks,
                    Zip = e.Zip,
                    State = e.State,
                    SchoolDistrictId = e.SchoolDistrictId,
                    SchoolGuid = e.SchoolGuid,
                    SchoolClassification = e.SchoolClassification,
                    StreetAddress1 = e.StreetAddress1,
                    StreetAddress2 = e.StreetAddress2,
                    PlanId = e.PlanId,
                    SchoolStateId = e.SchoolStateId,
                    SchoolInternalId = e.SchoolInternalId,
                    SchoolLevel = e.SchoolLevel,
                    CreatedOn = e.CreatedOn,
                    UpdatedOn = e.UpdatedOn,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == schoolListViewModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == schoolListViewModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                }).ToList();
                schoolListView.schoolMaster = schoolListWithGradeLevel;
                schoolListView._tenantName = schoolListViewModel._tenantName;
                schoolListView._token = schoolListViewModel._token;
                schoolListView._failure = false;
            }
            catch (Exception es)
            {
                schoolListView._message = es.Message;
                schoolListView._failure = true;
                schoolListView._tenantName = schoolListViewModel._tenantName;
                schoolListView._token = schoolListViewModel._token;
            }
            return schoolListView;
        }

        /// <summary>
        /// Add/Update School Logo
        /// </summary>
        /// <param name="schoolAddViewModel"></param>
        /// <returns></returns>
        public SchoolAddViewModel AddUpdateSchoolLogo(SchoolAddViewModel schoolAddViewModel)
        {
            try
            {
                var schoolLogoUpdate = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Where(x => x.TenantId == schoolAddViewModel.schoolMaster.TenantId && x.SchoolId == schoolAddViewModel.schoolMaster.SchoolId).ToList();

                if (schoolLogoUpdate.FirstOrDefault().SchoolDetail.Count > 0)
                {
                    schoolLogoUpdate.FirstOrDefault().SchoolDetail.FirstOrDefault().UpdatedOn = DateTime.UtcNow;
                    schoolLogoUpdate.FirstOrDefault().SchoolDetail.FirstOrDefault().UpdatedBy = schoolAddViewModel.schoolMaster.UpdatedBy;
                    schoolLogoUpdate.FirstOrDefault().UpdatedOn = DateTime.UtcNow;
                    schoolLogoUpdate.FirstOrDefault().UpdatedBy = schoolAddViewModel.schoolMaster.UpdatedBy;
                    schoolLogoUpdate.FirstOrDefault().SchoolDetail.FirstOrDefault().SchoolLogo = schoolAddViewModel.schoolMaster.SchoolDetail.FirstOrDefault().SchoolLogo;
                    this.context?.SaveChanges();
                    schoolAddViewModel._message = "School Logo Updated Successfully";
                }
                else
                {
                    schoolAddViewModel._failure = true;
                    schoolAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                schoolAddViewModel._failure = true;
                schoolAddViewModel._message = es.Message;
            }
            return schoolAddViewModel;
        }

        /// <summary>
        /// Copy School
        /// </summary>
        /// <param name="copySchoolViewModel"></param>
        /// <returns></returns>
        public CopySchoolViewModel CopySchool(CopySchoolViewModel copySchoolViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var copyFromSchool = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == copySchoolViewModel.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId);
                    string schoolName = null;

                    if (copyFromSchool != null)
                    {
                        if (copySchoolViewModel.schoolMaster.SchoolName != null)
                        {
                            schoolName = copySchoolViewModel.schoolMaster.SchoolName;
                        }
                        
                        copySchoolViewModel.schoolMaster = copyFromSchool;
                        int? MasterSchoolId = Utility.GetMaxPK(this.context, new Func<SchoolMaster, int>(x => x.SchoolId));
                        copySchoolViewModel.schoolMaster.SchoolId = (int)MasterSchoolId;

                        Guid GuidId = Guid.NewGuid();
                        var GuidIdExist = this.context?.SchoolMaster.FirstOrDefault(x => x.SchoolGuid == GuidId);
                        if (GuidIdExist != null)
                        {
                            copySchoolViewModel._failure = true;
                            copySchoolViewModel._message = "Guid is already exist, Please try again.";
                            return copySchoolViewModel;
                        }

                        copySchoolViewModel.schoolMaster.SchoolName = schoolName;
                        copySchoolViewModel.schoolMaster.SchoolGuid = GuidId;
                        copySchoolViewModel.schoolMaster.SchoolInternalId = null;
                        this.context?.SchoolMaster.Add(copySchoolViewModel.schoolMaster);
                        this.context.SaveChanges();

                        int? Ide = null;
                        Ide = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id));

                        var schoolDetailsData = this.context?.SchoolDetail.Where(x => x.TenantId == copySchoolViewModel.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId).ToList();

                        if (schoolDetailsData.Count > 0)
                        {
                            schoolDetailsData.ForEach(x => x.SchoolMaster = null);

                            var SchoolDetails = new List<SchoolDetail>(){new SchoolDetail()
                            { TenantId =copySchoolViewModel.TenantId, SchoolId = MasterSchoolId, Id = (int)Ide, Affiliation = schoolDetailsData.FirstOrDefault().Affiliation,Associations =schoolDetailsData.FirstOrDefault().Associations,Locale =schoolDetailsData.FirstOrDefault().Locale, LowestGradeLevel = schoolDetailsData.FirstOrDefault().LowestGradeLevel, HighestGradeLevel = schoolDetailsData.FirstOrDefault().HighestGradeLevel,DateSchoolOpened =schoolDetailsData.FirstOrDefault().DateSchoolOpened,DateSchoolClosed =schoolDetailsData.FirstOrDefault().DateSchoolClosed, Status = schoolDetailsData.FirstOrDefault().Status, Internet = schoolDetailsData.FirstOrDefault().Internet, Gender =schoolDetailsData.FirstOrDefault().Gender,Electricity =schoolDetailsData.FirstOrDefault().Electricity,Telephone =schoolDetailsData.FirstOrDefault().Telephone,Fax =schoolDetailsData.FirstOrDefault().Fax,Website =schoolDetailsData.FirstOrDefault().Website,Email =schoolDetailsData.FirstOrDefault().Email,Twitter =schoolDetailsData.FirstOrDefault().Twitter,Facebook =schoolDetailsData.FirstOrDefault().Facebook,Instagram =schoolDetailsData.FirstOrDefault().Instagram,LinkedIn =schoolDetailsData.FirstOrDefault().LinkedIn,Youtube =schoolDetailsData.FirstOrDefault().Youtube,SchoolLogo =schoolDetailsData.FirstOrDefault().SchoolLogo,NameOfPrincipal = schoolDetailsData.FirstOrDefault().NameOfPrincipal,NameOfAssistantPrincipal = schoolDetailsData.FirstOrDefault().NameOfAssistantPrincipal,RunningWater =schoolDetailsData.FirstOrDefault().RunningWater,MainSourceOfDrinkingWater =schoolDetailsData.FirstOrDefault().MainSourceOfDrinkingWater,CurrentlyAvailable =schoolDetailsData.FirstOrDefault().CurrentlyAvailable,TotalFemaleToilets =schoolDetailsData.FirstOrDefault().TotalFemaleToilets,FemaleToiletType =schoolDetailsData.FirstOrDefault().FemaleToiletType,FemaleToiletAccessibility =schoolDetailsData.FirstOrDefault().FemaleToiletAccessibility,TotalFemaleToiletsUsable =schoolDetailsData.FirstOrDefault().TotalFemaleToiletsUsable,MaleToiletType =schoolDetailsData.FirstOrDefault().MaleToiletType,TotalMaleToilets =schoolDetailsData.FirstOrDefault().TotalMaleToilets,TotalMaleToiletsUsable =schoolDetailsData.FirstOrDefault().TotalMaleToiletsUsable,MaleToiletAccessibility =schoolDetailsData.FirstOrDefault().MaleToiletAccessibility,ComonToiletType =schoolDetailsData.FirstOrDefault().ComonToiletType,TotalCommonToilets =schoolDetailsData.FirstOrDefault().TotalCommonToilets,TotalCommonToiletsUsable =schoolDetailsData.FirstOrDefault().TotalCommonToiletsUsable,CommonToiletAccessibility =schoolDetailsData.FirstOrDefault().CommonToiletAccessibility,HandwashingAvailable =schoolDetailsData.FirstOrDefault().HandwashingAvailable,SoapAndWaterAvailable =schoolDetailsData.FirstOrDefault().SoapAndWaterAvailable,HygeneEducation =schoolDetailsData.FirstOrDefault().HygeneEducation,CreatedBy=schoolDetailsData.FirstOrDefault().CreatedBy, CreatedOn=DateTime.UtcNow }
                            };

                            this.context?.SchoolDetail.AddRange(SchoolDetails);
                        }

                        var Membership = new List<Membership>() {
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "Super Administrator", IsActive= true, IsSuperadmin= true, IsSystem= true, MembershipId= 1, ProfileType= "Super Administrator"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "School Administrator", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 2, ProfileType= "School Administrator"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "Admin Assistant", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 3, ProfileType= "Admin Assistant"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 4, ProfileType= "Teacher"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "Homeroom Teacher", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 5, ProfileType= "Homeroom Teacher"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId =copySchoolViewModel.schoolMaster.SchoolId, Profile= "Parent", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 6, ProfileType= "Parent"},
                            new Membership(){CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, Profile= "Student", IsActive= true, IsSuperadmin= false, IsSystem= true, MembershipId= 7, ProfileType= "Student"},
                        };
                        this.context?.Membership.AddRange(Membership);

                        long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));

                        var DpdownValuelist = new List<DpdownValuelist>() {
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="PK",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="K",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+1},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="1",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+2},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="2",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+3},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="3",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+4},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="4",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+5},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="5",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+6},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="6",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+7},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="7",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+8},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="8",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+9},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="9",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+10},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="10",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+11},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="11",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+12},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="12",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+13},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="13",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+14},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="14",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+15},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="15",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+16},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="16",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+17},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="17",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+18},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="18",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+19},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="19",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+20},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="20",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+21},


                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Boys",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+22},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Girls",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+23},
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="School Gender",LovColumnValue="Mixed",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+24},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Mr.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+25},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Miss.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+26},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Mrs.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+27},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Ms.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+28},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Dr.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+29},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Rev.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+30},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Prof.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+31},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Sir.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+32},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Salutation",LovColumnValue="Lord ",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+33},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="Jr.",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+34},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="Sr",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+35},
                             //new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.ModifiedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="Sr",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+36},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="II",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+37},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="III",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+38},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="IV",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+39},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="V",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+40},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Suffix",LovColumnValue="PhD",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+41},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Male",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+42},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Female",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+43},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Gender",LovColumnValue="Other",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+44},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Single",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+45},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Married",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+46},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Marital Status",LovColumnValue="Partnered",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+47},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Next grade at current school",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+48},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Retain",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+49},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Rolling/Retention Option",LovColumnValue="Do not enroll after this school year",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+50},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Mother",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+51},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Father",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+52},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Legal Guardian",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+53},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Relationship",LovColumnValue="Other",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+54},


                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Add",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+55},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Drop",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+56},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Rolled Over",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+57},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Drop (Transfer)",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+58},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Enrollment Type",LovColumnValue="Enroll (Transfer)",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+59},


                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Dropdown",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+60},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Editable Dropdown",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+61},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Text",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+62},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Checkbox",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+63},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Number",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+64},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Multiple SelectBox",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+65},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Date",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+66},
                             new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Field Type",LovColumnValue="Textarea",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+67},
                                                       
                        };
                        this.context?.DpdownValuelist.AddRange(DpdownValuelist);
                        var dpdownValueIde = dpdownValueId+68;

                        var FieldsCategory = new List<FieldsCategory>()
                        {
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Information",Module="School",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=1},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Wash Information",Module="School",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=2},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Student",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=3},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Enrollment Info",Module="Student",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=4},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Student",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=5},

                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Family Info",Module="Student",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=6},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Medical Info",Module="Student",SortOrder=5,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=7},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Comments",Module="Student",SortOrder=6,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=8},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Documents",Module="Student",SortOrder=7,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=9},

                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Parent",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=10},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address Info",Module="Parent",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=11},

                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="General Info",Module="Staff",SortOrder=1,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=12},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="School Info",Module="Staff",SortOrder=2,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=13},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Address & Contact",Module="Staff",SortOrder=3,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=14},
                            new FieldsCategory(){ TenantId=copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,IsSystemCategory=true,Search=true, Title="Certification Info",Module="Staff",SortOrder=4,Required=true,Hide=false,CreatedOn=DateTime.UtcNow,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CategoryId=15}
                        };
                        this.context?.FieldsCategory.AddRange(FieldsCategory);

                        var StudentEnrollmentCode = new List<StudentEnrollmentCode>()
                        {
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=1, Title="New", ShortName="NEW", SortOrder=1, Type="Add", CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=2, Title="Dropped Out", ShortName="DROP", SortOrder=2, Type="Drop", CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=3, Title="Rolled Over", ShortName="ROLL", SortOrder=3, Type="Rolled Over", CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=4, Title="Transferred In", ShortName="TRAN", SortOrder=4, Type="Enroll (Transfer)", CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=5, Title="Transferred Out", ShortName="TRAN", SortOrder=5, Type="Drop (Transfer)", CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy }
                        };
                        this.context?.StudentEnrollmentCode.AddRange(StudentEnrollmentCode);

                        var Block = new List<Block>()
                        {
                             new Block(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy }
                        };
                        this.context?.Block.AddRange(Block);

                        var releaseNumber = new ReleaseNumber();
                        {
                            releaseNumber.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                            releaseNumber.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                            releaseNumber.ReleaseNumber1 = "v1.0.0";
                            releaseNumber.ReleaseDate = DateTime.UtcNow;
                            releaseNumber.CreatedOn = DateTime.UtcNow;
                            releaseNumber.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                        }

                        //insert into system default custom fields
                        var dataCustomFields = System.IO.File.ReadAllText(@"CustomFields.json");
                        JsonSerializerSettings settingCusFld = new JsonSerializerSettings();
                        List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld);
                        foreach (CustomFields customFields in objCusFld)
                        {
                            customFields.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                            customFields.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                            customFields.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                            customFields.CreatedOn = DateTime.UtcNow;
                            this.context?.CustomFields.Add(customFields);
                            //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                        }

                        var customFieldCategories = this.context?.FieldsCategory.Include(x => x.CustomFields).Where(x => x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId && x.IsSystemCategory != true).ToList();
                        if (customFieldCategories.Count > 0)
                        {
                            foreach (var customFieldCategoriesData in customFieldCategories)
                            {
                                var customFields = new FieldsCategory()
                                {
                                    TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                    SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                    CategoryId = customFieldCategoriesData.CategoryId,
                                    IsSystemCategory = false,
                                    Search = customFieldCategoriesData.Search,
                                    Title = customFieldCategoriesData.Title,
                                    Module = customFieldCategoriesData.Module,
                                    SortOrder = customFieldCategoriesData.SortOrder,
                                    Required = customFieldCategoriesData.Required,
                                    Hide = customFieldCategoriesData.Hide,
                                    IsSystemWideCategory = true,
                                    CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    CustomFields = customFieldCategoriesData.CustomFields.Where(x => x.IsSystemWideField == true).Select(s => new CustomFields
                                    {
                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                        FieldId = s.FieldId,
                                        Type = s.Type,
                                        Search = s.Search,
                                        Title = s.Title,
                                        SortOrder = s.SortOrder,
                                        SelectOptions = s.SelectOptions,
                                        CategoryId = s.CategoryId,
                                        SystemField = s.SystemField,
                                        Required = s.Required,
                                        DefaultSelection = s.DefaultSelection,
                                        Hide = s.Hide,
                                        Module = s.Module,
                                        FieldName = s.FieldName,
                                        IsSystemWideField = true,
                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                        CreatedOn = DateTime.UtcNow,
                                    }).ToList()
                                };

                                this.context?.FieldsCategory.Add(customFields);
                            }
                        }

                        if (copySchoolViewModel.ProfilePermission == true)
                        {
                            var PermissionGroupData = this.context?.PermissionGroup.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (PermissionGroupData.Count > 0)
                            {
                                PermissionGroupData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.PermissionGroup.AddRange(PermissionGroupData);
                            }
                            var PermissionCategoryData = this.context?.PermissionCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (PermissionCategoryData.Count > 0)
                            {
                                PermissionGroupData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.PermissionCategory.AddRange(PermissionCategoryData);
                            }
                            var PermissionSubcategoryData = this.context?.PermissionSubcategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (PermissionSubcategoryData.Count > 0)
                            {
                                PermissionSubcategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.PermissionSubcategory.AddRange(PermissionSubcategoryData);
                            }
                            var RolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (RolePermissionData.Count > 0)
                            {
                                RolePermissionData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.RolePermission.AddRange(RolePermissionData);
                            }
                        }
                        else
                        {
                            //insert into permission group
                            var dataGroup = System.IO.File.ReadAllText(@"Group.json");
                            JsonSerializerSettings settingGrp = new JsonSerializerSettings();
                            List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp);

                            foreach (PermissionGroup permisionGrp in objGroup)
                            {

                                permisionGrp.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permisionGrp.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permisionGrp.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permisionGrp.CreatedOn = DateTime.UtcNow;
                                //permisionGrp.IsActive = true;
                                permisionGrp.PermissionCategory = null;
                                this.context?.PermissionGroup.Add(permisionGrp);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            ////insert into system default custom fields
                            //var dataCustomFields = System.IO.File.ReadAllText(@"CustomFields.json");
                            //JsonSerializerSettings settingCusFld = new JsonSerializerSettings();
                            //List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld);
                            //foreach (CustomFields customFields in objCusFld)
                            //{
                            //    customFields.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                            //    customFields.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                            //    customFields.UpdatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                            //    customFields.LastUpdate = DateTime.UtcNow;
                            //    this.context?.CustomFields.Add(customFields);
                            //    //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            //}

                            //insert into permission category
                            var dataCategory = System.IO.File.ReadAllText(@"Category.json");
                            JsonSerializerSettings settingCat = new JsonSerializerSettings();
                            List<PermissionCategory> objCat = JsonConvert.DeserializeObject<List<PermissionCategory>>(dataCategory, settingCat);
                            foreach (PermissionCategory permissionCate in objCat)
                            {
                                permissionCate.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionCate.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionCate.PermissionGroup = null;
                                permissionCate.RolePermission = null;
                                permissionCate.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionCategory.Add(permissionCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into permission subcategory
                            var dataSubCategory = System.IO.File.ReadAllText(@"SubCategory.json");
                            JsonSerializerSettings settingSubCat = new JsonSerializerSettings();
                            List<PermissionSubcategory> objSubCat = JsonConvert.DeserializeObject<List<PermissionSubcategory>>(dataSubCategory, settingSubCat);
                            foreach (PermissionSubcategory permissionSubCate in objSubCat)
                            {
                                permissionSubCate.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionSubCate.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionSubCate.RolePermission = null;
                                permissionSubCate.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionSubCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionSubcategory.Add(permissionSubCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into role permission
                            var dataRolePermission = System.IO.File.ReadAllText(@"RolePermission.json");
                            JsonSerializerSettings settingRole = new JsonSerializerSettings();
                            List<RolePermission> objRole = JsonConvert.DeserializeObject<List<RolePermission>>(dataRolePermission, settingRole);
                            foreach (RolePermission permissionRole in objRole)
                            {
                                permissionRole.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionRole.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionRole.PermissionCategory = null;
                                permissionRole.Membership = null;
                                permissionRole.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionRole.CreatedOn = DateTime.UtcNow;
                                this.context?.RolePermission.Add(permissionRole);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }
                        }
                        
                        this.context?.ReleaseNumber.Add(releaseNumber);
                        //this.context?.SaveChanges();
                        //transaction.Commit();

                        if (copySchoolViewModel.Periods == true)
                        {
                            var blockData = this.context?.Block.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.BlockId!=1).ToList();

                            if (blockData.Count > 0)
                            {
                                blockData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Block.AddRange(blockData);
                            }

                            var blockPeriodData = this.context?.BlockPeriod.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (blockPeriodData.Count > 0)
                            {
                                blockPeriodData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.BlockPeriod.AddRange(blockPeriodData);
                            }
                        }

                        if (copySchoolViewModel.MarkingPeriods == true)
                        {
                            var schoolYearsData = this.context?.SchoolYears.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (schoolYearsData.Count > 0)
                            {
                                schoolYearsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.SchoolYears.AddRange(schoolYearsData);
                            }

                            var semestersData = this.context?.Semesters.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (semestersData.Count > 0)
                            {
                                semestersData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Semesters.AddRange(semestersData);
                            }

                            var quartersData = this.context?.Quarters.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (quartersData.Count > 0)
                            {
                                quartersData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Quarters.AddRange(quartersData);
                            }

                            var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (progressPeriodsData.Count > 0)
                            {
                                progressPeriodsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.ProgressPeriods.AddRange(progressPeriodsData);
                            }
                        }

                        if (copySchoolViewModel.Calendar == true)
                        {
                            var calenderData = this.context?.SchoolCalendars.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (calenderData.Count > 0)
                            {
                                calenderData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.SchoolCalendars.AddRange(calenderData);
                            }
                        }

                        if (copySchoolViewModel.Sections == true)
                        {
                            var sectionData = this.context?.Sections.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (sectionData.Count > 0)
                            {          
                                sectionData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Sections.AddRange(sectionData);
                            }
                        }

                        if (copySchoolViewModel.Rooms == true)
                        {
                            var roomData = this.context?.Rooms.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (roomData.Count > 0)
                            {
                                roomData.ToList().ForEach(x =>x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Rooms.AddRange(roomData);
                            }
                        }

                        if (copySchoolViewModel.GradeLevels == true)
                        {
                            var gradelevelsData = this.context?.Gradelevels.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (gradelevelsData.Count > 0)
                            {
                                gradelevelsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Gradelevels.AddRange(gradelevelsData);
                            }
                        }

                        if (copySchoolViewModel.SchoolFields == true)
                        {
                            var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "school" && x.IsSystemCategory != true).ToList();

                            if (FieldsCategoryData.Count > 0)
                            {
                                FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.FieldsCategory.AddRange(FieldsCategoryData);
                            }

                            var SchoolFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "school" && x.SystemField != true).ToList();

                            if (SchoolFieldsData.Count > 0)
                            {
                                SchoolFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.CustomFields.AddRange(SchoolFieldsData);
                            }
                        }

                        if (copySchoolViewModel.StudentFields == true)
                        {
                            var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "student" && x.IsSystemCategory != true).ToList();

                            if (FieldsCategoryData.Count > 0)
                            {
                                FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.FieldsCategory.AddRange(FieldsCategoryData);
                            }

                            var StudentFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "student" && x.SystemField != true).ToList();

                            if (StudentFieldsData.Count > 0)
                            {
                                StudentFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.CustomFields.AddRange(StudentFieldsData);
                            }
                        }

                        if (copySchoolViewModel.EnrollmentCodes == true)
                        {
                            var EnrollmentCodesData = this.context?.StudentEnrollmentCode.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Title.ToLower() != "dropped out" && x.Title.ToLower() != "new" && x.Title.ToLower() != "rolled over" && x.Title.ToLower() != "transferred in" && x.Title.ToLower() != "transferred out").ToList();

                            if (EnrollmentCodesData.Count > 0)
                            {
                                EnrollmentCodesData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.StudentEnrollmentCode.AddRange(EnrollmentCodesData);
                            }
                        }

                        if (copySchoolViewModel.StaffFields == true)
                        {
                            var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "staff" && x.IsSystemCategory != true).ToList();

                            if (FieldsCategoryData.Count > 0)
                            {
                                FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.FieldsCategory.AddRange(FieldsCategoryData);
                            }

                            var StaffFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "staff" && x.SystemField != true).ToList();

                            if (StaffFieldsData.Count > 0)
                            {
                                StaffFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.CustomFields.AddRange(StaffFieldsData);
                            }
                        }

                        if (copySchoolViewModel.Subjets == true)
                        {
                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (subjectData.Count > 0)
                            {
                                subjectData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Subject.AddRange(subjectData);
                            }
                        }
                        if (copySchoolViewModel.Programs == true)
                        {
                            var ProgramsData = this.context?.Programs.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (ProgramsData.Count > 0)
                            {
                                ProgramsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Programs.AddRange(ProgramsData);
                            }
                        }

                        if (copySchoolViewModel.Course == true)
                        {
                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (subjectData.Count > 0)
                            {
                                subjectData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Subject.AddRange(subjectData);
                            }

                            var ProgramsData = this.context?.Programs.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (ProgramsData.Count > 0)
                            {
                                ProgramsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Programs.AddRange(ProgramsData);
                            }

                            var courseData = this.context?.Course.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (courseData.Count > 0)
                            {
                                courseData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Course.AddRange(courseData);
                            }
                        }

                        if (copySchoolViewModel.AttendanceCode == true)
                        {
                            var AttendanceCodeCategoriData = this.context?.AttendanceCodeCategories.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (AttendanceCodeCategoriData.Count > 0)
                            {                         
                                AttendanceCodeCategoriData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.AttendanceCodeCategories.AddRange(AttendanceCodeCategoriData);
                            }
                            var attendanceCodeData = this.context?.AttendanceCode.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (attendanceCodeData.Count > 0)
                            {                              
                                attendanceCodeData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.AttendanceCode.AddRange(attendanceCodeData);
                            }
                        }                    

                        if (copySchoolViewModel.ReportCardGrades == true)
                        {
                            
                            var GradeScaleData = this.context?.GradeScale.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (GradeScaleData.Count > 0)
                            {
                                GradeScaleData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.GradeScale.AddRange(GradeScaleData);
                            }

                            var GradeData = this.context?.Grade.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (GradeData.Count > 0)
                            {
                                GradeData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.Grade.AddRange(GradeData);
                            } 
                        }

                        if (copySchoolViewModel.ReportCardComments == true)
                        {
                            var reportCardCommentsData = this.context?.CourseCommentCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (reportCardCommentsData.Count > 0)
                            {
                                reportCardCommentsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.CourseCommentCategory.AddRange(reportCardCommentsData);
                            }
                        }

                        if (copySchoolViewModel.StandardGrades == true)
                        {
                            var standardGradesData = this.context?.GradeUsStandard.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (standardGradesData.Count > 0)
                            {
                                standardGradesData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.GradeUsStandard.AddRange(standardGradesData);
                            }
                        }
                      
                        if (copySchoolViewModel.EffortGrades == true)
                        {
                            var EffortGradeLibraryCategoryData = this.context?.EffortGradeLibraryCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (EffortGradeLibraryCategoryData.Count > 0)
                            {
                                EffortGradeLibraryCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.EffortGradeLibraryCategory.AddRange(EffortGradeLibraryCategoryData);
                            }

                            var EffortGradeLibraryCategoryItemData = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (EffortGradeLibraryCategoryItemData.Count > 0)
                            {
                                EffortGradeLibraryCategoryItemData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.EffortGradeLibraryCategoryItem.AddRange(EffortGradeLibraryCategoryItemData);
                            }

                            var effortGradeScaleData = this.context?.EffortGradeScale.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (effortGradeScaleData.Count > 0)
                            {
                                effortGradeScaleData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.EffortGradeScale.AddRange(effortGradeScaleData);
                            }
                        }

                        if (copySchoolViewModel.HonorRollSetup == true)
                        {
                            var honorRollSetupData = this.context?.HonorRolls.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (honorRollSetupData.Count > 0)
                            {
                                honorRollSetupData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context.HonorRolls.AddRange(honorRollSetupData);
                            }
                        }
                        
                        if (copySchoolViewModel.SchoolLevel == true)
                        {

                            var SchoolLevelData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "school level").ToList();

                            if (SchoolLevelData.Count > 0)
                            {
                                SchoolLevelData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(SchoolLevelData);
                            }
                        }
                        else
                        {
                            var DpdownValueList = new List<DpdownValuelist>() {

                            new DpdownValuelist() { UpdatedOn = DateTime.UtcNow, UpdatedBy = copySchoolViewModel.schoolMaster.CreatedBy, TenantId = copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, LovName = "School Level", LovColumnValue = copySchoolViewModel.schoolMaster.SchoolLevel, CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy, CreatedOn = DateTime.UtcNow, Id = (long)dpdownValueIde ++ }
                            };
                             this.context?.DpdownValuelist.AddRange(DpdownValueList);
                        }

                        if (copySchoolViewModel.SchoolClassification == true)
                        {
                            var SchoolClassificationData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "school classification").ToList();

                            if (SchoolClassificationData.Count > 0)
                            {
                                SchoolClassificationData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(SchoolClassificationData);
                            }
                        }
                        else
                        {
                            var DpdownValueList = new List<DpdownValuelist>() {

                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="School Classification",LovColumnValue=copySchoolViewModel.schoolMaster.SchoolClassification,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueIde++}
                            };
                            this.context?.DpdownValuelist.AddRange(DpdownValueList);
                        }

                        if (copySchoolViewModel.FemaleToiletType == true)
                        {
                            var FemaleToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "female toilet type").ToList();

                            if (FemaleToiletTypeData.Count > 0)
                            {
                                FemaleToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(FemaleToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.FemaleToiletAccessibility == true)
                        {
                            var FemaleToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "female toilet accessibility").ToList();

                            if (FemaleToiletAccessibilityData.Count > 0)
                            {
                                FemaleToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(FemaleToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.MaleToiletType == true)
                        {
                            var MaleToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "male toilet type").ToList();

                            if (MaleToiletTypeData.Count > 0)
                            {
                                MaleToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(MaleToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.MaleToiletAccessibility == true)
                        {
                            var MaleToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "male toilet accessibility").ToList();

                            if (MaleToiletAccessibilityData.Count > 0)
                            {
                                MaleToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(MaleToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.CommonToiletType == true)
                        {
                            var CommonToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "common toilet type").ToList();

                            if (CommonToiletTypeData.Count > 0)
                            {
                                CommonToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(CommonToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.CommonToiletAccessibility == true)
                        {
                            var CommonToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "common toilet accessibility").ToList();

                            if (CommonToiletAccessibilityData.Count > 0)
                            {
                                CommonToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(CommonToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.Race == true)
                        {
                            var RaceData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "race").ToList();

                            if (RaceData.Count > 0)
                            {
                                RaceData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(RaceData);
                            }
                        }
                        if (copySchoolViewModel.Ethnicity == true)
                        {
                            var EthnicityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "ethnicityData").ToList();

                            if (EthnicityData.Count > 0)
                            {
                                EthnicityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context.DpdownValuelist.AddRange(EthnicityData);
                            }
                        }

                        this.context?.SaveChanges();
                        transaction.Commit();
                        copySchoolViewModel._failure = false;
                        copySchoolViewModel._message = "School Copied Successfully";

                        copySchoolViewModel.schoolMaster.Membership = null;
                        copySchoolViewModel.schoolMaster.DpdownValuelist = null;
                        copySchoolViewModel.schoolMaster.PermissionGroup = null;
                    }
                    else
                    {
                        copySchoolViewModel._failure = true;
                        copySchoolViewModel._message = NORECORDFOUND;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    copySchoolViewModel._failure = true;
                    copySchoolViewModel._message = ex.Message;
                }
            }
            return copySchoolViewModel;
        }

        /// <summary>
        /// Update Last Used School Id
        /// </summary>
        /// <param name="userViewModel"></param>
        /// <returns></returns>
        public UserViewModel UpdateLastUsedSchoolId(UserViewModel userViewModel)
        {
            try
            {
                var userMasterData = this.context.UserMaster.FirstOrDefault(c => c.TenantId == userViewModel.TenantId && c.EmailAddress == userViewModel.EmailAddress );

                if (userMasterData !=null)
                {
                    userMasterData.LastUsedSchoolId = userViewModel.LastUsedSchoolId;
                    userMasterData.UpdatedBy = userViewModel.UpdatedBy;
                    userMasterData.UpdatedOn = DateTime.UtcNow;
                    this.context?.SaveChanges();
                    userViewModel._failure = false;
                    userViewModel._message = "Last Used School Updated Successfully";
                }
                else
                {
                    userViewModel._failure = true;
                    userViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                userViewModel._failure = true;
                userViewModel._message = es.Message;
            }
            return userViewModel;
        }
    }
}
