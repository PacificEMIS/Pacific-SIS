/***********************************************************************************
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
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public SchoolRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        //Get Full Address
        private static string ToFullAddress(string Address1, string Address2, string City, string State, string Country, string Zip)
        {
            string? address = "";
            if (!string.IsNullOrWhiteSpace(Address1))
            {


                return address == null ? "" : $"{Address1?.Trim()}{(!string.IsNullOrWhiteSpace(Address2) ? $", {Address2?.Trim()}" : string.Empty)}, {City?.Trim()}, {State?.Trim()} {Zip?.Trim()}";
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
            //int resultData;
            SchoolListModel schoolListModel = new();
            IQueryable<SchoolMaster>? transactionIQ = null;
            IQueryable<SchoolMaster>? SchoolMasterList = null;

            var userMasterData = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.EmailAddress == pageResult.EmailAddress);

            if (userMasterData != null)
            {
                if (userMasterData.Membership != null && String.Compare(userMasterData.Membership.ProfileType, "Super Administrator", true) == 0)
                {
                    SchoolMasterList = this.context?.SchoolMaster
                      .Include(d => d.SchoolDetail)
                      .Where(x => x.TenantId == pageResult.TenantId && (pageResult.IncludeInactive == false ? x.SchoolDetail.FirstOrDefault()!.Status == true : true)).OrderBy(c=>c.SchoolName);
                }
                else
                {
                    var staffSchoolInfoData = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && x.StaffId == userMasterData.UserId).Select(s => s.SchoolAttachedId);

                    if (staffSchoolInfoData != null)
                    {
                        SchoolMasterList = this.context?.SchoolMaster
                                          .Include(d => d.SchoolDetail)
                                          .Where(x => x.TenantId == pageResult.TenantId && staffSchoolInfoData.Contains(x.SchoolId) && (pageResult.IncludeInactive == false ? x.SchoolDetail.FirstOrDefault()!.Status == true : true)).OrderBy(v=>v.SchoolName);
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
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1 && SchoolMasterList != null && SchoolMasterList.Any())
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        //transactionIQ = SchoolMasterList.Where(x => x.SchoolName.ToLower().Contains(Columnvalue.ToLower()) || x.StreetAddress1.ToLower().Contains(Columnvalue.ToLower()) || x.StreetAddress2.ToLower().Contains(Columnvalue.ToLower()) || x.Zip.ToLower().Contains(Columnvalue.ToLower()) || x.State.ToLower().Contains(Columnvalue.ToLower()) || x.City.ToLower().Contains(Columnvalue.ToLower()) || x.Country.ToLower().Contains(Columnvalue.ToLower()));

                        transactionIQ = SchoolMasterList.Where(x => x.SchoolName!.ToLower().Contains(Columnvalue.ToLower()) || (x.StreetAddress1 ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.StreetAddress2 ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.Zip ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.State ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.City ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.Country ?? "").ToLower().Contains(Columnvalue.ToLower()));

                        var childTelephoneFilter = SchoolMasterList.Where(x => x.SchoolDetail.FirstOrDefault() != null ? (x.SchoolDetail.FirstOrDefault()!.Telephone ?? "").ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));

                        if (childTelephoneFilter.ToList().Count > 0)
                        {
                            transactionIQ = transactionIQ.Concat(childTelephoneFilter);
                        }

                        var childNameOfPrincipalFilter = SchoolMasterList.Where(x => x.SchoolDetail.FirstOrDefault() != null ? (x.SchoolDetail.FirstOrDefault()!.NameOfPrincipal ?? "").ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));
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
                        if(SchoolMasterList?.Any() == true && pageResult.FilterParams?.Any() == true)
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams, SchoolMasterList).AsQueryable();
                        }
                    }

                    if(transactionIQ != null && transactionIQ.Any())
                    {
                        transactionIQ = transactionIQ.Distinct();
                    }
                }

                if (transactionIQ != null && transactionIQ.Any())
                {
                    if (pageResult.SortingModel != null)
                    {
                        switch ((pageResult.SortingModel.SortColumn ?? "").ToLower())
                        {
                            case "nameofprincipal":

                                if ((pageResult.SortingModel.SortDirection ?? "").ToLower() == "asc")
                                {

                                    transactionIQ = transactionIQ.OrderBy(a => a.SchoolDetail.Count > 0 ? a.SchoolDetail.FirstOrDefault()!.NameOfPrincipal : null);
                                }
                                else
                                {
                                    transactionIQ = transactionIQ.OrderByDescending(a => a.SchoolDetail.Count > 0 ? a.SchoolDetail.FirstOrDefault()!.NameOfPrincipal : null);
                                }
                                break;

                            default:
                                //transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
                                transactionIQ = Utility.Sort(transactionIQ, (pageResult.SortingModel.SortColumn ?? ""), (pageResult.SortingModel.SortDirection ?? "").ToLower());
                                break;
                        }

                    }
                    else
                    {
                        transactionIQ = transactionIQ.OrderBy(c => c.SchoolName);
                    }

                    int totalCount = transactionIQ.Count();
                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        transactionIQ = transactionIQ.Select(p => new SchoolMaster
                        {
                            SchoolId = p.SchoolId,
                            TenantId = p.TenantId,
                            SchoolName = p.SchoolName!.Trim(),
                            Zip = p.Zip,
                            StreetAddress1 = p.StreetAddress1,
                            StreetAddress2 = p.StreetAddress2,
                            State = p.State,
                            District = p.District,
                            City = p.City,
                            Country = p.Country,
                            CreatedBy = p.CreatedBy,
                            CreatedOn = p.CreatedOn,
                            UpdatedBy = p.UpdatedBy,
                            UpdatedOn = p.UpdatedOn,
                            SchoolDetail = p.SchoolDetail.Select(s => new SchoolDetail
                            {
                                Telephone = s.Telephone,
                                NameOfPrincipal = s.NameOfPrincipal,
                                Status = s.Status,
                                CreatedBy = s.CreatedBy,
                                CreatedOn = s.CreatedOn,
                                UpdatedBy = s.UpdatedBy,
                                UpdatedOn = s.UpdatedOn
                            }).ToList()
                        }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                    }

                    schoolListModel.schoolMaster = transactionIQ.ToList();

                    schoolListModel.schoolMaster.ForEach(e =>
                    {
                        e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.CreatedBy);
                        e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.UpdatedBy);
                    });

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
                    //schoolListModel.schoolMaster = transactionIQ.ToList();
                    schoolListModel.TotalCount = totalCount;
                    schoolListModel.PageNumber = pageResult.PageNumber;
                    schoolListModel._pageSize = pageResult.PageSize;
                    schoolListModel._tenantName = pageResult._tenantName;
                    schoolListModel._token = pageResult._token;
                    schoolListModel._failure = false;
                }
                else
                {
                    schoolListModel._failure = true;
                    schoolListModel._message = NORECORDFOUND;

                }
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
            SchoolListModel schoolListModel = new();
            try
            {
                var userMasterData = this.context?.UserMaster.Include(c=>c.Membership).FirstOrDefault(x => x.TenantId == school.TenantId && x.EmailAddress == school.EmailAddress);

                if (userMasterData != null)
                {
                    if (userMasterData.Membership!=null && String.Compare(userMasterData.Membership.ProfileType, "Super Administrator", true) == 0)
                    {
                        var schoolList = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Where(x => x.TenantId == school.TenantId && x.SchoolDetail.FirstOrDefault()!.Status != false).OrderBy(x => x.SchoolName)
                          .Select(e => new GetSchoolForView()
                          {
                              SchoolId = e.SchoolId,
                              TenantId = e.TenantId,
                              SchoolName = e.SchoolName!.Trim(),
                              DateSchoolOpened = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.StartDate,
                              DateSchoolClosed = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.EndDate,
                              MembershipId = userMasterData.Membership.MembershipId,
                              MembershipType = userMasterData.Membership.ProfileType
                          }).ToList();

                        if (schoolList != null && schoolList.Any())
                        {
                            schoolListModel.getSchoolForView = schoolList;
                        }
                    }
                    else
                    {
                        var schoolList = this.context?.SchoolMaster.
                                    Join(this.context?.StaffSchoolInfo!,
                                    sm => sm.SchoolId, ssi => ssi.SchoolAttachedId,
                                    (sm, ssi) => new { sm, ssi }).Where(c => c.sm.TenantId == school.TenantId && c.ssi.TenantId == school.TenantId && c.sm.SchoolDetail.FirstOrDefault()!.Status != false && c.ssi.StaffId == userMasterData.UserId && (c.ssi.EndDate == null || c.ssi.EndDate >= DateTime.UtcNow.Date) && (c.ssi.StartDate <= DateTime.UtcNow.Date)).OrderBy(x => x.sm.SchoolName).Select(e => new GetSchoolForView()
                                    {
                                        SchoolId = e.sm.SchoolId,
                                        TenantId = e.sm.TenantId,
                                        SchoolName = e.sm.SchoolName!.Trim(),
                                        DateSchoolOpened = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.sm.TenantId && x.SchoolId == e.sm.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.StartDate,
                                        DateSchoolClosed = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.sm.TenantId && x.SchoolId == e.sm.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.EndDate,
                                        MembershipId = e.ssi.MembershipId,
                                        MembershipType = e.ssi.Profile
                                    }).ToList();

                        if (schoolList != null && schoolList.Any())
                        {
                            schoolListModel.getSchoolForView = schoolList;
                        }
                    }
                }
                else
                {
                    schoolListModel.schoolMaster = new();
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
            if (school.schoolMaster is null)
            {
                return school;
            }
            SchoolAddViewModel SchoolAddViewModel = new();
            try
            {
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
                                        CreatedBy= y.CreatedBy,
                                        CreatedOn =y.CreatedOn,
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
                                            CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == school.schoolMaster.SchoolId).ToList()
                                        }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                                    }).ToList();


                    school.schoolMaster = schoolMaster;
                    school.schoolMaster.FieldsCategory = customFields;
                    //school.schoolMaster.FieldsCategory.FirstOrDefault().CustomFields = schoolcustomFields;
                    if (school.schoolMaster.SchoolDetail.ToList().Count > 0)
                    {
                        school.schoolMaster.SchoolDetail.FirstOrDefault()!.SchoolMaster = null;
                    }
                    if (school.schoolMaster.CustomFields.ToList().Count > 0)
                    {
                        school.schoolMaster.CustomFields = new HashSet<CustomFields>();
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
                SchoolAddViewModel._failure = true;
                SchoolAddViewModel._message = es.Message;
                return SchoolAddViewModel;
            }

        }

        /// <summary>
        /// Update School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolAddViewModel UpdateSchool(SchoolAddViewModel school)
        {
            using var transaction = this.context?.Database.BeginTransaction();
            {
                if (school.schoolMaster is null)
                {
                    return school;
                }
                try
                {
                    var schoolMaster = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Include(x => x.FieldsCategory).ThenInclude(x => x.CustomFields).ThenInclude(x => x.CustomFieldsValue).FirstOrDefault(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.schoolMaster.SchoolId);

                    var checkInternalId = this.context?.SchoolMaster.Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolInternalId == school.schoolMaster.SchoolInternalId && x.SchoolInternalId != null && x.SchoolId != school.schoolMaster.SchoolId).ToList();

                    if (checkInternalId?.Any() == true)
                    {
                        school.schoolMaster = null;
                        school._failure = true;
                        school._message = "School InternalID Already Exist";
                    }
                    else
                    {
                        if (schoolMaster != null && string.IsNullOrEmpty(school.schoolMaster.SchoolInternalId))
                        {
                            school.schoolMaster.SchoolInternalId = schoolMaster.SchoolInternalId;
                        }

                        //if school nmae change then change in also student enrollment table
                        if (schoolMaster != null && String.Compare(schoolMaster.SchoolName, school.schoolMaster.SchoolName, true) == 0)
                        {
                            var EnrollmentSchoolNameData = this.context?.StudentEnrollment.AsEnumerable().Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == schoolMaster.SchoolId && String.Compare(x.SchoolName, schoolMaster.SchoolName, true) == 0).ToList();

                            if (EnrollmentSchoolNameData?.Any()==true)
                            {
                                EnrollmentSchoolNameData.ForEach(x => x.SchoolName = school.schoolMaster.SchoolName);
                            }
                            var EnrollmentTransferredSchoolData = this.context?.StudentEnrollment.AsEnumerable().Where(x => x.TenantId == school.schoolMaster.TenantId && x.TransferredSchoolId == schoolMaster.SchoolId && String.Compare(x.SchoolTransferred, schoolMaster.SchoolName, true) == 0).ToList();
                            if (EnrollmentTransferredSchoolData?.Any() == true)
                            {
                                EnrollmentTransferredSchoolData.ForEach(x => x.SchoolTransferred = school.schoolMaster.SchoolName);
                            }
                        }
                        school.schoolMaster.CreatedBy = schoolMaster!.CreatedBy;
                        school.schoolMaster.CreatedOn = schoolMaster!.CreatedOn;
                        school.schoolMaster.SchoolGuid = schoolMaster!.SchoolGuid;
                        school.schoolMaster.PlanId = schoolMaster!.PlanId;
                        school.schoolMaster.UpdatedOn = DateTime.UtcNow;
                        this.context?.Entry(schoolMaster).CurrentValues.SetValues(school.schoolMaster);                     
                        if (schoolMaster.SchoolDetail.ToList().Count == 0 && school.schoolMaster.SchoolDetail.ToList().Count > 0)
                        {
                            school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.Id = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id))!);
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
                                    school.schoolMaster.SchoolDetail.FirstOrDefault()!.DateSchoolOpened = schoolMaster.SchoolDetail.FirstOrDefault()!.DateSchoolOpened;

                                }
                                school.schoolMaster.SchoolDetail.FirstOrDefault()!.CreatedBy = schoolMaster.SchoolDetail.FirstOrDefault()!.CreatedBy;
                                school.schoolMaster.SchoolDetail.FirstOrDefault()!.CreatedOn = schoolMaster.SchoolDetail.FirstOrDefault()!.CreatedOn;
                                school.schoolMaster.SchoolDetail.FirstOrDefault()!.UpdatedOn = DateTime.UtcNow;

                                this.context?.Entry(schoolMaster.SchoolDetail.FirstOrDefault()!).CurrentValues.SetValues(school.schoolMaster.SchoolDetail.FirstOrDefault()!);
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
                                        customFields.CustomFieldsValue.FirstOrDefault()!.Module = "School";
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CategoryId = customFields.CategoryId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.FieldId = customFields.FieldId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldTitle = customFields.Title;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFieldType = customFields.Type;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.SchoolId = school.schoolMaster.SchoolId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TenantId = school.schoolMaster.TenantId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.TargetId = school.schoolMaster.SchoolId;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFields = null!;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CreatedBy = null;
                                        customFields.CustomFieldsValue.FirstOrDefault()!.UpdatedBy = null;
                                        this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                    }
                                }

                            }
                        }
                        this.context?.SaveChanges();
                        if (school.schoolMaster.SchoolDetail.ToList()?.Any()==true)
                        {
                            school.schoolMaster.SchoolDetail.FirstOrDefault()!.SchoolMaster = null;
                        }

                        if (school.schoolMaster.FieldsCategory != null && school.schoolMaster.FieldsCategory.ToList().Count > 0)
                        {
                            foreach (var FieldsCategory in school.schoolMaster.FieldsCategory)
                            {
                                foreach (var customFields in FieldsCategory.CustomFields)
                                {
                                    if (customFields.CustomFieldsValue?.Any() == true)
                                    {
                                        customFields.CustomFieldsValue.FirstOrDefault()!.CustomFields = new();
                                    }
                                }
                            }
                        }

                        transaction?.Commit();
                        school._failure = false;
                        school._message = "School Updated Successfully";
                    }
                    return school;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    school.schoolMaster = null;
                    school._failure = true;
                    school._message = ex.Message;
                    return school;
                }
            }

        }

        /// <summary>
        /// Add School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolAddViewModel AddSchool(SchoolAddViewModel school)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                if (school.schoolMaster is null)
                {
                    return school;
                }
                try
                {
                    if (school.StartDate != null && school.EndDate != null)
                    {
                        if (school.EndDate > school.StartDate?.AddYears(+1))
                        {
                            school._failure = true;
                            school._message = "End date should not be more then one year from start date.";
                        }
                        else
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
                                school.schoolMaster.SchoolDetail.ToList().ForEach(p => p.Id = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id))!);
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
                    new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="PK",CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId!},
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
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Level",LovColumnValue=school.schoolMaster.SchoolLevel!,CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+68},
                     new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=school.schoolMaster.CreatedBy, TenantId= school.schoolMaster.TenantId,SchoolId=school.schoolMaster.SchoolId,LovName="School Classification",LovColumnValue=school.schoolMaster.SchoolClassification!,CreatedBy=school.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId+69}
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
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=1, Title="New", ShortName="NEW", SortOrder=1, Type="Add", AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=2, Title="Dropped Out", ShortName="DROP", SortOrder=2, Type="Drop", AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=3, Title="Rolled Over", ShortName="ROLL", SortOrder=3, Type="Rolled Over", AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=4, Title="Transferred In", ShortName="TRAN", SortOrder=4, Type="Enroll (Transfer)", AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy },
                     new StudentEnrollmentCode(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, EnrollmentCode=5, Title="Transferred Out", ShortName="TRAN", SortOrder=5, Type="Drop (Transfer)", AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy }
                };

                            school.schoolMaster.Block = new List<Block>()
                {
                     new Block(){TenantId=school.schoolMaster.TenantId, SchoolId=school.schoolMaster.SchoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, AcademicYear=Convert.ToDecimal(school.StartDate?.Year), CreatedOn=DateTime.UtcNow, CreatedBy=school.schoolMaster.CreatedBy }
                };

                            ReleaseNumber releaseNumber = new();
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
                            JsonSerializerSettings settingGrp = new();
                            List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp)!;

                            foreach (PermissionGroup permisionGrp in objGroup)
                            {

                                permisionGrp.TenantId = school.schoolMaster.TenantId;
                                permisionGrp.SchoolId = school.schoolMaster.SchoolId;
                                permisionGrp.CreatedBy = school.schoolMaster.CreatedBy;
                                permisionGrp.CreatedOn = school.schoolMaster.CreatedOn;
                                //permisionGrp.IsActive = true;
                                //permisionGrp.PermissionCategory = new HashSet<PermissionCategory>();
                                this.context?.PermissionGroup.Add(permisionGrp);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into system default custom fields
                            var dataCustomFields = System.IO.File.ReadAllText(@"CustomFields.json");
                            JsonSerializerSettings settingCusFld = new();
                            List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld)!;
                            foreach (CustomFields customFields in objCusFld)
                            {
                                customFields.TenantId = school.schoolMaster.TenantId;
                                customFields.SchoolId = school.schoolMaster.SchoolId;
                                customFields.CreatedBy = school.schoolMaster.CreatedBy;
                                customFields.CreatedOn = DateTime.UtcNow;
                                this.context?.CustomFields.Add(customFields);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            int? CusFldId = objCusFld.Max(x => x.FieldId);
                            if (CusFldId != null)
                            {
                                CusFldId++;
                            }

                            //insert into permission category
                            var dataCategory = System.IO.File.ReadAllText(@"Category.json");
                            JsonSerializerSettings settingCat = new();
                            List<PermissionCategory> objCat = JsonConvert.DeserializeObject<List<PermissionCategory>>(dataCategory, settingCat)!;
                            foreach (PermissionCategory permissionCate in objCat)
                            {
                                permissionCate.TenantId = school.schoolMaster.TenantId;
                                permissionCate.SchoolId = school.schoolMaster.SchoolId;
                                //permissionCate.PermissionGroup = new();
                                //permissionCate.RolePermission = new HashSet<RolePermission>();
                                permissionCate.CreatedBy = school.schoolMaster.CreatedBy;
                                permissionCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionCategory.Add(permissionCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into permission subcategory
                            var dataSubCategory = System.IO.File.ReadAllText(@"SubCategory.json");
                            JsonSerializerSettings settingSubCat = new();
                            List<PermissionSubcategory> objSubCat = JsonConvert.DeserializeObject<List<PermissionSubcategory>>(dataSubCategory, settingSubCat)!;
                            foreach (PermissionSubcategory permissionSubCate in objSubCat)
                            {
                                permissionSubCate.TenantId = school.schoolMaster.TenantId;
                                permissionSubCate.SchoolId = school.schoolMaster.SchoolId;
                                //permissionSubCate.RolePermission = new HashSet<RolePermission>();
                                permissionSubCate.CreatedBy = school.schoolMaster.CreatedBy;
                                permissionSubCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionSubcategory.Add(permissionSubCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            int? SubCateId = objSubCat.Max(x => x.PermissionSubcategoryId);
                            if (SubCateId != null)
                            {
                                SubCateId++;
                            }

                            //insert into role permission
                            var dataRolePermission = System.IO.File.ReadAllText(@"RolePermission.json");
                            JsonSerializerSettings settingRole = new();
                            List<RolePermission> objRole = JsonConvert.DeserializeObject<List<RolePermission>>(dataRolePermission, settingRole)!;
                            foreach (RolePermission permissionRole in objRole)
                            {
                                permissionRole.TenantId = school.schoolMaster.TenantId;
                                permissionRole.SchoolId = school.schoolMaster.SchoolId;
                                //permissionRole.PermissionCategory = null;
                                //permissionRole.Membership = null;
                                permissionRole.CreatedBy = school.schoolMaster.CreatedBy;
                                permissionRole.CreatedOn = DateTime.UtcNow;
                                this.context?.RolePermission.Add(permissionRole);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            int? RoleId = objRole.Max(x => x.RolePermissionId);
                            if (RoleId != null)
                            {
                                RoleId++;
                            }

                            var customFieldCategories = this.context?.FieldsCategory.Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.SchoolId && x.IsSystemCategory != true).ToList();

                            if (customFieldCategories?.Any() == true)
                            {
                                foreach (var customFieldCategoriesData in customFieldCategories)
                                {
                                    var fieldsCategory = new FieldsCategory()
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
                                        CreatedOn = DateTime.UtcNow
                                    };

                                    school.schoolMaster.FieldsCategory.Add(fieldsCategory);

                                    var PermissionSubcategoryData = this.context?.PermissionSubcategory.Include(x => x.RolePermission).AsEnumerable().Where(x => x.SchoolId == school.SchoolId && x.TenantId == school.schoolMaster.TenantId && String.Compare(x.PermissionSubcategoryName, customFieldCategoriesData.Title, true) == 0).FirstOrDefault();

                                    if (PermissionSubcategoryData != null)
                                    {
                                        var permissionSubCategory = new PermissionSubcategory()
                                        {
                                            TenantId = school.schoolMaster.TenantId,
                                            SchoolId = school.schoolMaster.SchoolId,
                                            PermissionSubcategoryId = (int)SubCateId!,
                                            PermissionGroupId = PermissionSubcategoryData.PermissionGroupId,
                                            PermissionCategoryId = PermissionSubcategoryData.PermissionCategoryId,
                                            PermissionSubcategoryName = PermissionSubcategoryData.PermissionSubcategoryName,
                                            Path = PermissionSubcategoryData.Path,
                                            Title = PermissionSubcategoryData.Title,
                                            EnableView = PermissionSubcategoryData.EnableView,
                                            EnableAdd = PermissionSubcategoryData.EnableAdd,
                                            EnableEdit = PermissionSubcategoryData.EnableEdit,
                                            EnableDelete = PermissionSubcategoryData.EnableDelete,
                                            CreatedBy = school.schoolMaster.CreatedBy,
                                            CreatedOn = DateTime.UtcNow,
                                            IsActive = true,
                                            SortOrder = PermissionSubcategoryData.SortOrder
                                        };
                                        this.context?.PermissionSubcategory.Add(permissionSubCategory);

                                        var membershipData = this.context?.Membership.Where(x => x.SchoolId == school.SchoolId && x.TenantId == school.schoolMaster.TenantId && x.IsSystem == true).ToList();

                                        foreach (var membership in membershipData!)
                                        {
                                            if (String.Compare(membership.ProfileType, "super administrator", true) == 0 || String.Compare(membership.ProfileType, "school administrator", true) == 0 || String.Compare(membership.ProfileType, "admin assistant", true) == 0)
                                            {
                                                var rolePermission = new RolePermission()
                                                {
                                                    TenantId = school.schoolMaster.TenantId,
                                                    SchoolId = school.schoolMaster.SchoolId,
                                                    RolePermissionId = (int)RoleId!,
                                                    PermissionGroupId = null,
                                                    PermissionCategoryId = null,
                                                    PermissionSubcategoryId = SubCateId!,
                                                    CanView = true,
                                                    CanAdd = true,
                                                    CanEdit = true,
                                                    CanDelete = true,
                                                    CreatedBy = school.schoolMaster.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow,
                                                    MembershipId = membership.MembershipId
                                                };
                                                this.context?.RolePermission.Add(rolePermission);
                                            }
                                            else
                                            {
                                                var rolePermission = new RolePermission()
                                                {
                                                    TenantId = school.schoolMaster.TenantId,
                                                    SchoolId = school.schoolMaster.SchoolId,
                                                    RolePermissionId = (int)RoleId!,
                                                    PermissionGroupId = null,
                                                    PermissionCategoryId = null,
                                                    PermissionSubcategoryId = (int)SubCateId,
                                                    CanView = false,
                                                    CanAdd = false,
                                                    CanEdit = false,
                                                    CanDelete = false,
                                                    CreatedBy = school.schoolMaster.CreatedBy,
                                                    CreatedOn = DateTime.UtcNow,
                                                    MembershipId = membership.MembershipId
                                                };
                                                this.context?.RolePermission.Add(rolePermission);
                                            }
                                            RoleId++;
                                        }
                                        SubCateId++;
                                    }
                                }
                            }

                            var CustomFieldsData = this.context?.CustomFields.Where(x => x.TenantId == school.schoolMaster.TenantId && x.SchoolId == school.SchoolId && x.SystemField != true && x.IsSystemWideField == true).ToList();
                            if (CustomFieldsData?.Any()==true)
                            {
                                CustomFieldsData.ForEach(x => { x.FieldId = (int)CusFldId!++; x.SchoolId = school.schoolMaster.SchoolId; x.CreatedBy = school.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.CustomFields.AddRange(CustomFieldsData);
                            }

                            this.context?.SchoolMaster.Add(school.schoolMaster);
                            this.context?.ReleaseNumber.Add(releaseNumber);

                            //Insert data in staff_school_info table for School Admin & Admin Assistant
                            var userMasterData = this.context?.UserMaster.Include(x => x.Membership).Where(x => x.TenantId == school.schoolMaster.TenantId && x.EmailAddress == school.EmailAddress).ToList();

                            if (userMasterData?.Any()==true)
                            {
                                if (userMasterData.FirstOrDefault()!.Membership.ProfileType != "Super Administrator")
                                {
                                    int? Id = 0;
                                    Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));

                                    StaffSchoolInfo staffSchoolInfo = new();
                                    {
                                        staffSchoolInfo.TenantId = school.schoolMaster.TenantId;
                                        staffSchoolInfo.SchoolId = userMasterData.FirstOrDefault()!.SchoolId;
                                        staffSchoolInfo.StaffId = userMasterData.FirstOrDefault()!.UserId;
                                        staffSchoolInfo.SchoolAttachedId = school.schoolMaster.SchoolId;
                                        staffSchoolInfo.SchoolAttachedName = school.schoolMaster.SchoolName;
                                        staffSchoolInfo.Profile = userMasterData.FirstOrDefault()!.Membership.Profile;
                                        staffSchoolInfo.StartDate = DateTime.UtcNow;
                                        staffSchoolInfo.CreatedBy = school.schoolMaster.CreatedBy;
                                        staffSchoolInfo.CreatedOn = DateTime.UtcNow;
                                        staffSchoolInfo.Id = Id!=null ? (int)Id : 0;
                                    }
                                    this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                                }
                            }

                            AddSchoolYearAndSchoolCalendar(school.schoolMaster.TenantId, school.schoolMaster.SchoolId, school.StartDate, school.EndDate, school.schoolMaster.CreatedBy);

                            this.context?.SaveChanges();
                            transaction?.Commit();
                            school._failure = false;
                            school._message = "School Added Successfully";

                            school.schoolMaster.Membership = new HashSet<Membership>();
                            school.schoolMaster.DpdownValuelist = new HashSet<DpdownValuelist>();
                            school.schoolMaster.PermissionGroup = new HashSet<PermissionGroup>();
                            school.schoolMaster.CustomFields = new HashSet<CustomFields>();
                            //school.schoolMaster.FieldsCategory = null;

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

                    }
                    else
                    {
                        school._failure = true;
                        school._message = "Please provide start date and end date.";
                    }                    
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
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
                if (checkInternalId?.Any()==true)
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
            if (checkInternalId?.Any()==true)
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
            SchoolListViewModel schoolListView = new();
            try
            {
                var schoolListWithGradeLevel = this.context?.SchoolMaster.Include(x => x.Gradelevels).Include(x => x.StudentEnrollmentCode).Where(x => x.TenantId == schoolListViewModel.TenantId && x.SchoolDetail.FirstOrDefault()!.Status == true).ToList();

                if(schoolListWithGradeLevel?.Any()==true)
                {
                    schoolListView.schoolMaster = schoolListWithGradeLevel;
                    schoolListView._tenantName = schoolListViewModel._tenantName;
                    schoolListView._token = schoolListViewModel._token;
                    schoolListView._failure = false;
                }
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
            if (schoolAddViewModel.schoolMaster is null)
            {
                return schoolAddViewModel;
            }
            try
            {
                var schoolLogoUpdate = this.context?.SchoolMaster.Include(x => x.SchoolDetail).Where(x => x.TenantId == schoolAddViewModel.schoolMaster.TenantId && x.SchoolId == schoolAddViewModel.schoolMaster.SchoolId).ToList();

                if (schoolLogoUpdate!=null && schoolLogoUpdate.FirstOrDefault()!.SchoolDetail?.Any()==true)
                {
                    schoolLogoUpdate.FirstOrDefault()!.SchoolDetail.FirstOrDefault()!.UpdatedOn = DateTime.UtcNow;
                    schoolLogoUpdate.FirstOrDefault()!.SchoolDetail.FirstOrDefault()!.UpdatedBy = schoolAddViewModel.schoolMaster.UpdatedBy;
                    schoolLogoUpdate.FirstOrDefault()!.UpdatedOn = DateTime.UtcNow;
                    schoolLogoUpdate.FirstOrDefault()!.UpdatedBy = schoolAddViewModel.schoolMaster.UpdatedBy;
                    schoolLogoUpdate.FirstOrDefault()!.SchoolDetail.FirstOrDefault()!.SchoolLogo = schoolAddViewModel.schoolMaster.SchoolDetail.FirstOrDefault()!.SchoolLogo;
                    schoolLogoUpdate.FirstOrDefault()!.SchoolDetail.FirstOrDefault()!.SchoolThumbnailLogo = schoolAddViewModel.schoolMaster.SchoolDetail.FirstOrDefault()!.SchoolThumbnailLogo;
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
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                if (copySchoolViewModel.schoolMaster is null)
                {
                    return copySchoolViewModel;
                }
                try
                {
                    var academicYear = Utility.GetCurrentAcademicYear(this.context!, copySchoolViewModel.TenantId, copySchoolViewModel.FromSchoolId);

                    var copyFromSchool = this.context?.SchoolMaster.Where(x => x.TenantId == copySchoolViewModel.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId).Select(e => new SchoolMaster
                    {
                        TenantId = e.TenantId,
                        SchoolAltId = e.SchoolAltId,
                        SchoolStateId = e.SchoolStateId,
                        SchoolDistrictId = e.SchoolDistrictId,
                        SchoolLevel = e.SchoolLevel,
                        SchoolClassification = e.SchoolClassification,
                        AlternateName = e.AlternateName,
                        StreetAddress1 = e.StreetAddress1,
                        StreetAddress2 = e.StreetAddress2,
                        City = e.City,
                        County = e.County,
                        Division = e.Division,
                        State = e.State,
                        District = e.District,
                        Zip = e.Zip,
                        Country = e.Country,
                        CurrentPeriodEnds = e.CurrentPeriodEnds,
                        MaxApiChecks = e.MaxApiChecks,
                        Features = e.Features,
                        PlanId = e.PlanId,
                        Longitude = e.Longitude,
                        Latitude = e.Latitude,
                        CreatedBy = e.CreatedBy,
                        CreatedOn = DateTime.UtcNow
                    }).FirstOrDefault();

                    string? schoolName = null;

                    if (copyFromSchool != null)
                    {
                        if (copySchoolViewModel.schoolMaster.SchoolName != null)
                        {
                            schoolName = copySchoolViewModel.schoolMaster.SchoolName;
                        }
                        
                        copySchoolViewModel.schoolMaster = copyFromSchool;
                        int? MasterSchoolId = Utility.GetMaxPK(this.context, new Func<SchoolMaster, int>(x => x.SchoolId));
                        copySchoolViewModel.schoolMaster.SchoolId = (int)MasterSchoolId!;

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
                        copySchoolViewModel.schoolMaster.SchoolInternalId = MasterSchoolId.ToString();
                        this.context?.SchoolMaster.Add(copySchoolViewModel.schoolMaster);
                        this.context?.SaveChanges();

                        int? Ide = null;
                        Ide = (int)Utility.GetMaxPK(this.context, new Func<SchoolDetail, int>(x => x.Id))!;

                        var schoolDetailsData = this.context?.SchoolDetail.Where(x => x.TenantId == copySchoolViewModel.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId).ToList();

                        if (schoolDetailsData?.Any() == true)
                        {
                            //schoolDetailsData.ForEach(x => x.SchoolMaster = null);

                            var SchoolDetails = new List<SchoolDetail>(){new SchoolDetail()
                            { TenantId =copySchoolViewModel.TenantId, SchoolId = MasterSchoolId, Id = (int)Ide, Affiliation = schoolDetailsData.FirstOrDefault()!.Affiliation,Associations =schoolDetailsData.FirstOrDefault()!.Associations,Locale =schoolDetailsData.FirstOrDefault()!.Locale, LowestGradeLevel = schoolDetailsData.FirstOrDefault()!.LowestGradeLevel, HighestGradeLevel = schoolDetailsData.FirstOrDefault()!.HighestGradeLevel,DateSchoolOpened =schoolDetailsData.FirstOrDefault()!.DateSchoolOpened,DateSchoolClosed =schoolDetailsData.FirstOrDefault()!.DateSchoolClosed, Status = schoolDetailsData.FirstOrDefault()!.Status, Internet = schoolDetailsData.FirstOrDefault()!.Internet, Gender =schoolDetailsData.FirstOrDefault()!.Gender,Electricity =schoolDetailsData.FirstOrDefault()!.Electricity,Telephone =schoolDetailsData.FirstOrDefault()!.Telephone,Fax =schoolDetailsData.FirstOrDefault()!.Fax,Website =schoolDetailsData.FirstOrDefault()!.Website,Email =schoolDetailsData.FirstOrDefault()!.Email,Twitter =schoolDetailsData.FirstOrDefault()!.Twitter,Facebook =schoolDetailsData.FirstOrDefault()!.Facebook,Instagram =schoolDetailsData.FirstOrDefault()!.Instagram,LinkedIn =schoolDetailsData.FirstOrDefault()!.LinkedIn,Youtube =schoolDetailsData.FirstOrDefault()!.Youtube,SchoolLogo =schoolDetailsData.FirstOrDefault()!.SchoolLogo,SchoolThumbnailLogo =schoolDetailsData.FirstOrDefault()!.SchoolThumbnailLogo,NameOfPrincipal = schoolDetailsData.FirstOrDefault()!.NameOfPrincipal,NameOfAssistantPrincipal = schoolDetailsData.FirstOrDefault()!.NameOfAssistantPrincipal,RunningWater =schoolDetailsData.FirstOrDefault()!.RunningWater,MainSourceOfDrinkingWater =schoolDetailsData.FirstOrDefault()!.MainSourceOfDrinkingWater,CurrentlyAvailable =schoolDetailsData.FirstOrDefault()!.CurrentlyAvailable,TotalFemaleToilets =schoolDetailsData.FirstOrDefault()!.TotalFemaleToilets,FemaleToiletType =schoolDetailsData.FirstOrDefault()!.FemaleToiletType,FemaleToiletAccessibility =schoolDetailsData.FirstOrDefault()!.FemaleToiletAccessibility,TotalFemaleToiletsUsable =schoolDetailsData.FirstOrDefault()!.TotalFemaleToiletsUsable,MaleToiletType =schoolDetailsData.FirstOrDefault()!.MaleToiletType,TotalMaleToilets =schoolDetailsData.FirstOrDefault()!.TotalMaleToilets,TotalMaleToiletsUsable =schoolDetailsData.FirstOrDefault()!.TotalMaleToiletsUsable,MaleToiletAccessibility =schoolDetailsData.FirstOrDefault()!.MaleToiletAccessibility,ComonToiletType =schoolDetailsData.FirstOrDefault()!.ComonToiletType,TotalCommonToilets =schoolDetailsData.FirstOrDefault()!.TotalCommonToilets,TotalCommonToiletsUsable =schoolDetailsData.FirstOrDefault()!.TotalCommonToiletsUsable,CommonToiletAccessibility =schoolDetailsData.FirstOrDefault()!.CommonToiletAccessibility,HandwashingAvailable =schoolDetailsData.FirstOrDefault()!.HandwashingAvailable,SoapAndWaterAvailable =schoolDetailsData.FirstOrDefault()!.SoapAndWaterAvailable,HygeneEducation =schoolDetailsData.FirstOrDefault()!.HygeneEducation,CreatedBy=schoolDetailsData.FirstOrDefault()!.CreatedBy, CreatedOn=DateTime.UtcNow }
                            };

                            this.context?.SchoolDetail.AddRange(SchoolDetails);
                        }

                        var membershipData = this.context?.Membership.Where(x => x.TenantId == copySchoolViewModel.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId).ToList();

                        if (membershipData?.Any() == true)
                        {
                            membershipData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                            this.context?.Membership.AddRange(membershipData);
                        }

                        long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));

                        var DpdownValuelist = new List<DpdownValuelist>() {
                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="Grade Level",LovColumnValue="PK",CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueId!},
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
                        var dpdownValueIde = dpdownValueId + 68;

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
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=1, Title="New", ShortName="NEW", SortOrder=1, Type="Add", AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=2, Title="Dropped Out", ShortName="DROP", SortOrder=2, Type="Drop", AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=3, Title="Rolled Over", ShortName="ROLL", SortOrder=3, Type="Rolled Over", AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=4, Title="Transferred In", ShortName="TRAN", SortOrder=4, Type="Enroll (Transfer)", AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy },
                             new StudentEnrollmentCode(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, EnrollmentCode=5, Title="Transferred Out", ShortName="TRAN", SortOrder=5, Type="Drop (Transfer)", AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy }
                        };
                        this.context?.StudentEnrollmentCode.AddRange(StudentEnrollmentCode);

                        var Block = new List<Block>()
                        {
                             new Block(){TenantId=copySchoolViewModel.schoolMaster.TenantId, SchoolId=copySchoolViewModel.schoolMaster.SchoolId, BlockId=1, BlockTitle="All Day", BlockSortOrder=1, AcademicYear=academicYear, CreatedOn=DateTime.UtcNow, CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy }
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
                        JsonSerializerSettings settingCusFld = new();
                        List<CustomFields> objCusFld = JsonConvert.DeserializeObject<List<CustomFields>>(dataCustomFields, settingCusFld)!;
                        foreach (CustomFields customFields in objCusFld)
                        {
                            customFields.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                            customFields.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                            customFields.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                            customFields.CreatedOn = DateTime.UtcNow;
                            this.context?.CustomFields.Add(customFields);
                            //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                        }

                        if (copySchoolViewModel.ProfilePermission == true)
                        {
                            var PermissionGroupData = this.context?.PermissionGroup.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();
                            if (PermissionGroupData?.Any() == true)
                            {
                                PermissionGroupData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.PermissionGroup.AddRange(PermissionGroupData);
                            }

                            var PermissionCategoryData = this.context?.PermissionCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();
                            if (PermissionCategoryData?.Any() == true)
                            {
                                PermissionCategoryData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.PermissionCategory.AddRange(PermissionCategoryData);
                            }

                            var PermissionSubcategoryData = this.context?.PermissionSubcategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();
                            if (PermissionSubcategoryData?.Any() == true)
                            {
                                PermissionSubcategoryData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.PermissionSubcategory.AddRange(PermissionSubcategoryData);
                            }

                            var RolePermissionData = this.context?.RolePermission.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();
                            if (RolePermissionData?.Any() == true)
                            {
                                RolePermissionData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.RolePermission.AddRange(RolePermissionData);
                            }

                            var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.IsSystemCategory != true).ToList();
                            if (FieldsCategoryData?.Any() == true)
                            {
                                FieldsCategoryData.ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.FieldsCategory.AddRange(FieldsCategoryData);
                            }

                            var customField = this.context?.CustomFields.Where(x => x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId && x.IsSystemWideField == true && x.SystemField != true).ToList();
                            if (customField?.Any() == true)
                            {
                                customField.ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                this.context?.CustomFields.AddRange(customField);
                            }
                        }
                        else
                        {
                            //insert into permission group
                            var dataGroup = System.IO.File.ReadAllText(@"Group.json");
                            JsonSerializerSettings settingGrp = new();
                            List<PermissionGroup> objGroup = JsonConvert.DeserializeObject<List<PermissionGroup>>(dataGroup, settingGrp)!;

                            foreach (PermissionGroup permisionGrp in objGroup)
                            {
                                permisionGrp.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permisionGrp.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permisionGrp.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permisionGrp.CreatedOn = DateTime.UtcNow;
                                //permisionGrp.IsActive = true;
                                permisionGrp.PermissionCategory = new HashSet<PermissionCategory>();
                                this.context?.PermissionGroup.Add(permisionGrp);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into permission category
                            var dataCategory = System.IO.File.ReadAllText(@"Category.json");
                            JsonSerializerSettings settingCat = new();
                            List<PermissionCategory> objCat = JsonConvert.DeserializeObject<List<PermissionCategory>>(dataCategory, settingCat)!;
                            foreach (PermissionCategory permissionCate in objCat)
                            {
                                permissionCate.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionCate.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionCate.PermissionGroup = new();
                                permissionCate.RolePermission = new HashSet<RolePermission>();
                                permissionCate.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionCategory.Add(permissionCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            //insert into permission subcategory
                            var dataSubCategory = System.IO.File.ReadAllText(@"SubCategory.json");
                            JsonSerializerSettings settingSubCat = new();
                            List<PermissionSubcategory> objSubCat = JsonConvert.DeserializeObject<List<PermissionSubcategory>>(dataSubCategory, settingSubCat)!;
                            foreach (PermissionSubcategory permissionSubCate in objSubCat)
                            {
                                permissionSubCate.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionSubCate.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionSubCate.RolePermission = new HashSet<RolePermission>();
                                permissionSubCate.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionSubCate.CreatedOn = DateTime.UtcNow;
                                this.context?.PermissionSubcategory.Add(permissionSubCate);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }
                            int? SubCateId = objSubCat.Max(x => x.PermissionSubcategoryId);
                            if (SubCateId != null)
                            {
                                SubCateId++;
                            }
                            //insert into role permission
                            var dataRolePermission = System.IO.File.ReadAllText(@"RolePermission.json");
                            JsonSerializerSettings settingRole = new();
                            List<RolePermission> objRole = JsonConvert.DeserializeObject<List<RolePermission>>(dataRolePermission, settingRole)!;
                            foreach (RolePermission permissionRole in objRole)
                            {
                                permissionRole.TenantId = copySchoolViewModel.schoolMaster.TenantId;
                                permissionRole.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                permissionRole.PermissionCategory = new();
                                permissionRole.Membership = new();
                                permissionRole.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy;
                                permissionRole.CreatedOn = DateTime.UtcNow;
                                this.context?.RolePermission.Add(permissionRole);
                                //this.context?.SaveChanges(objModel.UserName, objModel.HostName, objModel.IpAddress, objModel.Page);
                            }

                            int? RoleId = objRole.Max(x => x.RolePermissionId);
                            if (RoleId != null)
                            {
                                RoleId++;
                            }

                            if (copySchoolViewModel.SchoolFields == true)
                            {
                                var FieldsCategoryData = this.context?.FieldsCategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.Module, "school", true) == 0 && x.IsSystemCategory != true).ToList();

                                if (FieldsCategoryData?.Any() == true)
                                {
                                    FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                    this.context?.FieldsCategory.AddRange(FieldsCategoryData);

                                    foreach (var fieldsCategory in FieldsCategoryData)
                                    {
                                        var PermissionSubcategoryData = this.context?.PermissionSubcategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.PermissionSubcategoryName, fieldsCategory.Title, true) == 0).FirstOrDefault();
                                        if (PermissionSubcategoryData != null)
                                        {
                                            var permissionSubCategory = new PermissionSubcategory()
                                            {
                                                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                PermissionSubcategoryId = (int)SubCateId!,
                                                PermissionGroupId = PermissionSubcategoryData.PermissionGroupId,
                                                PermissionCategoryId = PermissionSubcategoryData.PermissionCategoryId,
                                                PermissionSubcategoryName = PermissionSubcategoryData.PermissionSubcategoryName,
                                                Path = PermissionSubcategoryData.Path,
                                                Title = PermissionSubcategoryData.Title,
                                                EnableView = PermissionSubcategoryData.EnableView,
                                                EnableAdd = PermissionSubcategoryData.EnableAdd,
                                                EnableEdit = PermissionSubcategoryData.EnableEdit,
                                                EnableDelete = PermissionSubcategoryData.EnableDelete,
                                                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                IsActive = true
                                            };
                                            this.context?.PermissionSubcategory.Add(permissionSubCategory);

                                            var MembershipData = this.context?.Membership.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.IsSystem == true).ToList();

                                            foreach (var membership in MembershipData!)
                                            {
                                                if (String.Compare(membership.ProfileType, "super administrator", true) == 0 || String.Compare(membership.ProfileType, "school administrator", true) == 0 || String.Compare(membership.ProfileType, "admin assistant", true) == 0)
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = true,
                                                        CanAdd = true,
                                                        CanEdit = true,
                                                        CanDelete = true,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                else
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = false,
                                                        CanAdd = false,
                                                        CanEdit = false,
                                                        CanDelete = false,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                RoleId++;
                                            }
                                            SubCateId++;
                                        }
                                    }
                                }
                                var SchoolFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "school" && x.SystemField != true).ToList();

                                if (SchoolFieldsData?.Any() == true)
                                {
                                    SchoolFieldsData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                    this.context?.CustomFields.AddRange(SchoolFieldsData);
                                }
                            }
                            if (copySchoolViewModel.StudentFields == true)
                            {
                                var FieldsCategoryData = this.context?.FieldsCategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.Module, "student", true) == 0 && x.IsSystemCategory != true).ToList();

                                if (FieldsCategoryData?.Any() == true)
                                {
                                    FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                    this.context?.FieldsCategory.AddRange(FieldsCategoryData);

                                    foreach (var fieldsCategory in FieldsCategoryData)
                                    {
                                        var PermissionSubcategoryData = this.context?.PermissionSubcategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.PermissionSubcategoryName, fieldsCategory.Title, true) == 0).FirstOrDefault();
                                        if (PermissionSubcategoryData != null)
                                        {
                                            var permissionSubCategory = new PermissionSubcategory()
                                            {
                                                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                PermissionSubcategoryId = (int)SubCateId!,
                                                PermissionGroupId = PermissionSubcategoryData.PermissionGroupId,
                                                PermissionCategoryId = PermissionSubcategoryData.PermissionCategoryId,
                                                PermissionSubcategoryName = PermissionSubcategoryData.PermissionSubcategoryName,
                                                Path = PermissionSubcategoryData.Path,
                                                Title = PermissionSubcategoryData.Title,
                                                EnableView = PermissionSubcategoryData.EnableView,
                                                EnableAdd = PermissionSubcategoryData.EnableAdd,
                                                EnableEdit = PermissionSubcategoryData.EnableEdit,
                                                EnableDelete = PermissionSubcategoryData.EnableDelete,
                                                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                IsActive = true
                                            };
                                            this.context?.PermissionSubcategory.Add(permissionSubCategory);

                                            var MembershipData = this.context?.Membership.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.IsSystem == true).ToList();

                                            foreach (var membership in MembershipData!)
                                            {
                                                if (String.Compare(membership.ProfileType, "super administrator", true) == 0 || String.Compare(membership.ProfileType, "school administrator", true) == 0 || String.Compare(membership.ProfileType, "admin assistant", true) == 0)
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = true,
                                                        CanAdd = true,
                                                        CanEdit = true,
                                                        CanDelete = true,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                else
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = false,
                                                        CanAdd = false,
                                                        CanEdit = false,
                                                        CanDelete = false,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                RoleId++;
                                            }
                                            SubCateId++;
                                        }
                                    }
                                }
                                var SchoolFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "student" && x.SystemField != true).ToList();

                                if (SchoolFieldsData?.Any() == true)
                                {
                                    SchoolFieldsData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                    this.context?.CustomFields.AddRange(SchoolFieldsData);
                                }
                            }
                            if (copySchoolViewModel.StaffFields == true)
                            {
                                var FieldsCategoryData = this.context?.FieldsCategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.Module, "staff", true) == 0 && x.IsSystemCategory != true).ToList();

                                if (FieldsCategoryData?.Any() == true)
                                {
                                    FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                    this.context?.FieldsCategory.AddRange(FieldsCategoryData);

                                    foreach (var fieldsCategory in FieldsCategoryData)
                                    {
                                        var PermissionSubcategoryData = this.context?.PermissionSubcategory.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.PermissionSubcategoryName, fieldsCategory.Title, true) == 0).FirstOrDefault();
                                        if (PermissionSubcategoryData != null)
                                        {
                                            var permissionSubCategory = new PermissionSubcategory()
                                            {
                                                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                PermissionSubcategoryId = (int)SubCateId!,
                                                PermissionGroupId = PermissionSubcategoryData.PermissionGroupId,
                                                PermissionCategoryId = PermissionSubcategoryData.PermissionCategoryId,
                                                PermissionSubcategoryName = PermissionSubcategoryData.PermissionSubcategoryName,
                                                Path = PermissionSubcategoryData.Path,
                                                Title = PermissionSubcategoryData.Title,
                                                EnableView = PermissionSubcategoryData.EnableView,
                                                EnableAdd = PermissionSubcategoryData.EnableAdd,
                                                EnableEdit = PermissionSubcategoryData.EnableEdit,
                                                EnableDelete = PermissionSubcategoryData.EnableDelete,
                                                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                CreatedOn = DateTime.UtcNow,
                                                IsActive = true
                                            };
                                            this.context?.PermissionSubcategory.Add(permissionSubCategory);

                                            var MembershipData = this.context?.Membership.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.IsSystem == true).ToList();

                                            foreach (var membership in MembershipData!)
                                            {
                                                if (String.Compare(membership.ProfileType, "super administrator", true) == 0 || String.Compare(membership.ProfileType, "school administrator", true) == 0 || String.Compare(membership.ProfileType, "admin assistant", true) == 0)
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = true,
                                                        CanAdd = true,
                                                        CanEdit = true,
                                                        CanDelete = true,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                else
                                                {
                                                    var rolePermission = new RolePermission()
                                                    {
                                                        TenantId = copySchoolViewModel.schoolMaster.TenantId,
                                                        SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                                                        RolePermissionId = (int)RoleId!,
                                                        PermissionGroupId = null,
                                                        PermissionCategoryId = null,
                                                        PermissionSubcategoryId = (int)SubCateId,
                                                        CanView = false,
                                                        CanAdd = false,
                                                        CanEdit = false,
                                                        CanDelete = false,
                                                        CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                                                        CreatedOn = DateTime.UtcNow,
                                                        MembershipId = membership.MembershipId
                                                    };
                                                    this.context?.RolePermission.Add(rolePermission);
                                                }
                                                RoleId++;
                                            }
                                            SubCateId++;
                                        }
                                    }
                                }
                                var SchoolFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "staff" && x.SystemField != true).ToList();

                                if (SchoolFieldsData?.Any() == true)
                                {
                                    SchoolFieldsData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy; x.CreatedOn = DateTime.UtcNow; });
                                    this.context?.CustomFields.AddRange(SchoolFieldsData);
                                }
                            }
                        }

                        this.context?.ReleaseNumber.Add(releaseNumber);

                        if (copySchoolViewModel.Periods == true)
                        {
                            var blockData = this.context?.Block.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.BlockId != 1).ToList();

                            if (blockData?.Any() == true)
                            {
                                blockData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Block.AddRange(blockData);
                            }

                            var blockPeriodData = this.context?.BlockPeriod.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (blockPeriodData?.Any() == true)
                            {
                                blockPeriodData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.BlockPeriod.AddRange(blockPeriodData);
                            }
                        }

                        if (copySchoolViewModel.MarkingPeriods == true)
                        {
                            var schoolYearsData = this.context?.SchoolYears.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).OrderByDescending(x => x.StartDate).FirstOrDefault();

                            if (schoolYearsData != null)
                            {
                                schoolYearsData.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                this.context?.SchoolYears.Add(schoolYearsData);

                                var semestersData = this.context?.Semesters.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.YearId == schoolYearsData.MarkingPeriodId).ToList();

                                if (semestersData?.Any() == true)
                                {
                                    semestersData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                    this.context?.Semesters.AddRange(semestersData);
                                    var semIds = semestersData.Select(s => s.MarkingPeriodId).ToList();

                                    var quartersData = this.context?.Quarters.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && semIds.Contains((int)x.SemesterId!)).ToList();

                                    if (quartersData?.Any() == true)
                                    {
                                        quartersData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                        this.context?.Quarters.AddRange(quartersData);
                                        var qtrIds = quartersData.Select(s => s.MarkingPeriodId).ToList();

                                        var progressPeriodsData = this.context?.ProgressPeriods.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && qtrIds.Contains((int)x.QuarterId)).ToList();

                                        if (progressPeriodsData?.Any() == true)
                                        {
                                            progressPeriodsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                            this.context?.ProgressPeriods.AddRange(progressPeriodsData);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var schoolYearsData = this.context?.SchoolYears.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).OrderByDescending(x => x.StartDate).FirstOrDefault();

                            if (schoolYearsData != null)
                            {
                                schoolYearsData.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                this.context?.SchoolYears.Add(schoolYearsData);
                            }
                        }

                        if (copySchoolViewModel.Calendar == true)
                        {
                            if (academicYear != null)
                            {
                                var calenderData = this.context?.SchoolCalendars.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.AcademicYear == academicYear).ToList();

                                if (calenderData?.Any() == true)
                                {
                                    calenderData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                    this.context?.SchoolCalendars.AddRange(calenderData);
                                }
                            }
                        }
                        else
                        {
                            if (academicYear != null)
                            {
                                var calenderData = this.context?.SchoolCalendars.FirstOrDefault(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.AcademicYear == academicYear && x.SessionCalendar == true);

                                if (calenderData != null)
                                {
                                    calenderData.SchoolId = copySchoolViewModel.schoolMaster.SchoolId;
                                    calenderData.AcademicYear = (decimal)academicYear;
                                    this.context?.SchoolCalendars.Add(calenderData);
                                }
                            }
                        }

                        if (copySchoolViewModel.Sections == true)
                        {
                            var sectionData = this.context?.Sections.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (sectionData?.Any() == true)
                            {
                                sectionData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Sections.AddRange(sectionData);
                            }
                        }

                        if (copySchoolViewModel.Rooms == true)
                        {
                            var roomData = this.context?.Rooms.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (roomData?.Any() == true)
                            {
                                roomData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Rooms.AddRange(roomData);
                            }
                        }

                        if (copySchoolViewModel.GradeLevels == true)
                        {
                            var gradelevelsData = this.context?.Gradelevels.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (gradelevelsData?.Any() == true)
                            {
                                gradelevelsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Gradelevels.AddRange(gradelevelsData);
                            }
                        }

                        //if (copySchoolViewModel.SchoolFields == true)
                        //{
                        //    var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "school" && x.IsSystemCategory != true).ToList();

                        //    if (FieldsCategoryData.Count > 0)
                        //    {
                        //        FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.FieldsCategory.AddRange(FieldsCategoryData);
                        //    }

                        //    var SchoolFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "school" && x.SystemField != true && x.IsSystemWideField==true).ToList();

                        //    if (SchoolFieldsData.Count > 0)
                        //    {
                        //        SchoolFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.CustomFields.AddRange(SchoolFieldsData);
                        //    }
                        //}
                        //else
                        //{
                        //    var customFieldCategories = this.context?.FieldsCategory.Include(x => x.CustomFields).Where(x => x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId && x.IsSystemCategory != true && x.Module.ToLower() == "school".ToLower()).ToList();
                        //    if (customFieldCategories.Count > 0)
                        //    {
                        //        foreach (var customFieldCategoriesData in customFieldCategories)
                        //        {
                        //            var customFields = new FieldsCategory()
                        //            {
                        //                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                CategoryId = customFieldCategoriesData.CategoryId,
                        //                IsSystemCategory = false,
                        //                Search = customFieldCategoriesData.Search,
                        //                Title = customFieldCategoriesData.Title,
                        //                Module = customFieldCategoriesData.Module,
                        //                SortOrder = customFieldCategoriesData.SortOrder,
                        //                Required = customFieldCategoriesData.Required,
                        //                Hide = customFieldCategoriesData.Hide,
                        //                IsSystemWideCategory = true,
                        //                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                CreatedOn = DateTime.UtcNow,
                        //                CustomFields = customFieldCategoriesData.CustomFields.Where(x => x.IsSystemWideField == true).Select(s => new CustomFields
                        //                {
                        //                    TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                    SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                    FieldId = s.FieldId,
                        //                    Type = s.Type,
                        //                    Search = s.Search,
                        //                    Title = s.Title,
                        //                    SortOrder = s.SortOrder,
                        //                    SelectOptions = s.SelectOptions,
                        //                    CategoryId = s.CategoryId,
                        //                    SystemField = s.SystemField,
                        //                    Required = s.Required,
                        //                    DefaultSelection = s.DefaultSelection,
                        //                    Hide = s.Hide,
                        //                    Module = s.Module,
                        //                    FieldName = s.FieldName,
                        //                    IsSystemWideField = true,
                        //                    CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                    CreatedOn = DateTime.UtcNow,
                        //                }).ToList()
                        //            };

                        //            this.context?.FieldsCategory.Add(customFields);
                        //        }
                        //    }
                        //}

                        //if (copySchoolViewModel.StudentFields == true)
                        //{
                        //    var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "student" && x.IsSystemCategory != true).ToList();

                        //    if (FieldsCategoryData.Count > 0)
                        //    {
                        //        FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.FieldsCategory.AddRange(FieldsCategoryData);
                        //    }

                        //    var StudentFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "student" && x.SystemField != true).ToList();

                        //    if (StudentFieldsData.Count > 0)
                        //    {
                        //        StudentFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.CustomFields.AddRange(StudentFieldsData);
                        //    }
                        //}
                        //else
                        //{
                        //    var customFieldCategories = this.context?.FieldsCategory.Include(x => x.CustomFields).Where(x => x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId && x.IsSystemCategory != true && x.Module.ToLower() == "student".ToLower()).ToList();
                        //    if (customFieldCategories.Count > 0)
                        //    {
                        //        foreach (var customFieldCategoriesData in customFieldCategories)
                        //        {
                        //            var customFields = new FieldsCategory()
                        //            {
                        //                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                CategoryId = customFieldCategoriesData.CategoryId,
                        //                IsSystemCategory = false,
                        //                Search = customFieldCategoriesData.Search,
                        //                Title = customFieldCategoriesData.Title,
                        //                Module = customFieldCategoriesData.Module,
                        //                SortOrder = customFieldCategoriesData.SortOrder,
                        //                Required = customFieldCategoriesData.Required,
                        //                Hide = customFieldCategoriesData.Hide,
                        //                IsSystemWideCategory = true,
                        //                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                CreatedOn = DateTime.UtcNow,
                        //                CustomFields = customFieldCategoriesData.CustomFields.Where(x => x.IsSystemWideField == true).Select(s => new CustomFields
                        //                {
                        //                    TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                    SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                    FieldId = s.FieldId,
                        //                    Type = s.Type,
                        //                    Search = s.Search,
                        //                    Title = s.Title,
                        //                    SortOrder = s.SortOrder,
                        //                    SelectOptions = s.SelectOptions,
                        //                    CategoryId = s.CategoryId,
                        //                    SystemField = s.SystemField,
                        //                    Required = s.Required,
                        //                    DefaultSelection = s.DefaultSelection,
                        //                    Hide = s.Hide,
                        //                    Module = s.Module,
                        //                    FieldName = s.FieldName,
                        //                    IsSystemWideField = true,
                        //                    CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                    CreatedOn = DateTime.UtcNow,
                        //                }).ToList()
                        //            };

                        //            this.context?.FieldsCategory.Add(customFields);
                        //        }
                        //    }
                        //}

                        if (copySchoolViewModel.EnrollmentCodes == true)
                        {
                            var EnrollmentCodesData = this.context?.StudentEnrollmentCode.AsEnumerable().Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && String.Compare(x.Title, "dropped out", true) == 0 && String.Compare(x.Title, "new", true) == 0 && String.Compare(x.Title, "rolled over", true) == 0 && String.Compare(x.Title, "transferred in", true) == 0 && String.Compare(x.Title, "transferred out", true) == 0).ToList();

                            if (EnrollmentCodesData?.Any() == true)
                            {
                                EnrollmentCodesData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.StudentEnrollmentCode.AddRange(EnrollmentCodesData);
                            }
                        }

                        //if (copySchoolViewModel.StaffFields == true)
                        //{
                        //    var FieldsCategoryData = this.context?.FieldsCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "staff" && x.IsSystemCategory != true).ToList();

                        //    if (FieldsCategoryData.Count > 0)
                        //    {
                        //        FieldsCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.FieldsCategory.AddRange(FieldsCategoryData);
                        //    }

                        //    var StaffFieldsData = this.context?.CustomFields.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.Module.ToLower() == "staff" && x.SystemField != true).ToList();

                        //    if (StaffFieldsData.Count > 0)
                        //    {
                        //        StaffFieldsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                        //        this.context.CustomFields.AddRange(StaffFieldsData);
                        //    }
                        //}
                        //else
                        //{
                        //    var customFieldCategories = this.context?.FieldsCategory.Include(x => x.CustomFields).Where(x => x.TenantId == copySchoolViewModel.schoolMaster.TenantId && x.SchoolId == copySchoolViewModel.FromSchoolId && x.IsSystemCategory != true && x.Module.ToLower() == "staff".ToLower()).ToList();
                        //    if (customFieldCategories.Count > 0)
                        //    {
                        //        foreach (var customFieldCategoriesData in customFieldCategories)
                        //        {
                        //            var customFields = new FieldsCategory()
                        //            {
                        //                TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                CategoryId = customFieldCategoriesData.CategoryId,
                        //                IsSystemCategory = false,
                        //                Search = customFieldCategoriesData.Search,
                        //                Title = customFieldCategoriesData.Title,
                        //                Module = customFieldCategoriesData.Module,
                        //                SortOrder = customFieldCategoriesData.SortOrder,
                        //                Required = customFieldCategoriesData.Required,
                        //                Hide = customFieldCategoriesData.Hide,
                        //                IsSystemWideCategory = true,
                        //                CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                CreatedOn = DateTime.UtcNow,
                        //                CustomFields = customFieldCategoriesData.CustomFields.Where(x => x.IsSystemWideField == true).Select(s => new CustomFields
                        //                {
                        //                    TenantId = copySchoolViewModel.schoolMaster.TenantId,
                        //                    SchoolId = copySchoolViewModel.schoolMaster.SchoolId,
                        //                    FieldId = s.FieldId,
                        //                    Type = s.Type,
                        //                    Search = s.Search,
                        //                    Title = s.Title,
                        //                    SortOrder = s.SortOrder,
                        //                    SelectOptions = s.SelectOptions,
                        //                    CategoryId = s.CategoryId,
                        //                    SystemField = s.SystemField,
                        //                    Required = s.Required,
                        //                    DefaultSelection = s.DefaultSelection,
                        //                    Hide = s.Hide,
                        //                    Module = s.Module,
                        //                    FieldName = s.FieldName,
                        //                    IsSystemWideField = true,
                        //                    CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy,
                        //                    CreatedOn = DateTime.UtcNow,
                        //                }).ToList()
                        //            };

                        //            this.context?.FieldsCategory.Add(customFields);
                        //        }
                        //    }
                        //}

                        if (copySchoolViewModel.Subjets == true)
                        {
                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (subjectData?.Any() == true)
                            {
                                subjectData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Subject.AddRange(subjectData);
                            }
                        }
                        if (copySchoolViewModel.Programs == true)
                        {
                            var ProgramsData = this.context?.Programs.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (ProgramsData?.Any() == true)
                            {
                                ProgramsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Programs.AddRange(ProgramsData);
                            }
                        }

                        if (copySchoolViewModel.Course == true)
                        {
                            var subjectData = this.context?.Subject.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (subjectData?.Any() == true)
                            {
                                subjectData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Subject.AddRange(subjectData);
                            }

                            var ProgramsData = this.context?.Programs.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (ProgramsData?.Any() == true)
                            {
                                ProgramsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Programs.AddRange(ProgramsData);
                            }

                            var courseData = this.context?.Course.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (courseData?.Any() == true)
                            {
                                courseData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Course.AddRange(courseData);
                            }
                        }

                        if (copySchoolViewModel.AttendanceCode == true)
                        {
                            var AttendanceCodeCategoriData = this.context?.AttendanceCodeCategories.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (AttendanceCodeCategoriData?.Any() == true)
                            {
                                AttendanceCodeCategoriData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.AttendanceCodeCategories.AddRange(AttendanceCodeCategoriData);
                            }
                            var attendanceCodeData = this.context?.AttendanceCode.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (attendanceCodeData?.Any() == true)
                            {
                                attendanceCodeData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.AttendanceCode.AddRange(attendanceCodeData);
                            }
                        }

                        if (copySchoolViewModel.ReportCardGrades == true)
                        {

                            var GradeScaleData = this.context?.GradeScale.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (GradeScaleData?.Any() == true)
                            {
                                GradeScaleData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.GradeScale.AddRange(GradeScaleData);
                            }

                            var GradeData = this.context?.Grade.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (GradeData?.Any() == true)
                            {
                                GradeData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.Grade.AddRange(GradeData);
                            }
                        }

                        if (copySchoolViewModel.ReportCardComments == true)
                        {
                            var reportCardCommentsData = this.context?.CourseCommentCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (reportCardCommentsData?.Any() == true)
                            {
                                reportCardCommentsData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.CourseCommentCategory.AddRange(reportCardCommentsData);
                            }
                        }

                        if (copySchoolViewModel.StandardGrades == true)
                        {
                            var standardGradesData = this.context?.GradeUsStandard.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (standardGradesData?.Any() == true)
                            {
                                standardGradesData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.GradeUsStandard.AddRange(standardGradesData);
                            }
                        }

                        if (copySchoolViewModel.EffortGrades == true)
                        {
                            var EffortGradeLibraryCategoryData = this.context?.EffortGradeLibraryCategory.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (EffortGradeLibraryCategoryData?.Any() == true)
                            {
                                EffortGradeLibraryCategoryData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.EffortGradeLibraryCategory.AddRange(EffortGradeLibraryCategoryData);
                            }

                            var EffortGradeLibraryCategoryItemData = this.context?.EffortGradeLibraryCategoryItem.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (EffortGradeLibraryCategoryItemData?.Any() == true)
                            {
                                EffortGradeLibraryCategoryItemData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.EffortGradeLibraryCategoryItem.AddRange(EffortGradeLibraryCategoryItemData);
                            }

                            var effortGradeScaleData = this.context?.EffortGradeScale.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (effortGradeScaleData?.Any() == true)
                            {
                                effortGradeScaleData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.EffortGradeScale.AddRange(effortGradeScaleData);
                            }
                        }

                        if (copySchoolViewModel.HonorRollSetup == true)
                        {
                            var honorRollSetupData = this.context?.HonorRolls.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId).ToList();

                            if (honorRollSetupData?.Any() == true)
                            {
                                honorRollSetupData.ToList().ForEach(x => x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId);
                                this.context?.HonorRolls.AddRange(honorRollSetupData);
                            }
                        }

                        if (copySchoolViewModel.SchoolLevel == true)
                        {

                            var SchoolLevelData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "school level").ToList();

                            if (SchoolLevelData?.Any() == true)
                            {
                                SchoolLevelData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(SchoolLevelData);
                            }
                        }
                        else
                        {
                            var DpdownValueList = new List<DpdownValuelist>() {

                            new DpdownValuelist() { UpdatedOn = DateTime.UtcNow, UpdatedBy = copySchoolViewModel.schoolMaster.CreatedBy, TenantId = copySchoolViewModel.schoolMaster.TenantId, SchoolId = copySchoolViewModel.schoolMaster.SchoolId, LovName = "School Level", LovColumnValue = copySchoolViewModel.schoolMaster.SchoolLevel!, CreatedBy = copySchoolViewModel.schoolMaster.CreatedBy, CreatedOn = DateTime.UtcNow, Id = (long)dpdownValueIde ++ }
                            };
                            this.context?.DpdownValuelist.AddRange(DpdownValueList);
                        }

                        if (copySchoolViewModel.SchoolClassification == true)
                        {
                            var SchoolClassificationData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "school classification").ToList();

                            if (SchoolClassificationData?.Any() == true)
                            {
                                SchoolClassificationData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(SchoolClassificationData);
                            }
                        }
                        else
                        {
                            var DpdownValueList = new List<DpdownValuelist>() {

                            new DpdownValuelist(){UpdatedOn=DateTime.UtcNow,UpdatedBy=copySchoolViewModel.schoolMaster.CreatedBy, TenantId= copySchoolViewModel.schoolMaster.TenantId,SchoolId=copySchoolViewModel.schoolMaster.SchoolId,LovName="School Classification",LovColumnValue=copySchoolViewModel.schoolMaster.SchoolClassification!,CreatedBy=copySchoolViewModel.schoolMaster.CreatedBy,CreatedOn=DateTime.UtcNow,Id=(long)dpdownValueIde++}
                            };
                            this.context?.DpdownValuelist.AddRange(DpdownValueList);
                        }

                        if (copySchoolViewModel.FemaleToiletType == true)
                        {
                            var FemaleToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "female toilet type").ToList();

                            if (FemaleToiletTypeData?.Any() == true)
                            {
                                FemaleToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(FemaleToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.FemaleToiletAccessibility == true)
                        {
                            var FemaleToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "female toilet accessibility").ToList();

                            if (FemaleToiletAccessibilityData?.Any() == true)
                            {
                                FemaleToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(FemaleToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.MaleToiletType == true)
                        {
                            var MaleToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "male toilet type").ToList();

                            if (MaleToiletTypeData?.Any() == true)
                            {
                                MaleToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(MaleToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.MaleToiletAccessibility == true)
                        {
                            var MaleToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "male toilet accessibility").ToList();

                            if (MaleToiletAccessibilityData?.Any() == true)
                            {
                                MaleToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(MaleToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.CommonToiletType == true)
                        {
                            var CommonToiletTypeData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "common toilet type").ToList();

                            if (CommonToiletTypeData?.Any() == true)
                            {
                                CommonToiletTypeData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(CommonToiletTypeData);
                            }
                        }
                        if (copySchoolViewModel.CommonToiletAccessibility == true)
                        {
                            var CommonToiletAccessibilityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "common toilet accessibility").ToList();

                            if (CommonToiletAccessibilityData?.Any() == true)
                            {
                                CommonToiletAccessibilityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(CommonToiletAccessibilityData);
                            }
                        }
                        if (copySchoolViewModel.Race == true)
                        {
                            var RaceData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "race").ToList();

                            if (RaceData?.Any() == true)
                            {
                                RaceData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(RaceData);
                            }
                        }
                        if (copySchoolViewModel.Ethnicity == true)
                        {
                            var EthnicityData = this.context?.DpdownValuelist.Where(x => x.SchoolId == copySchoolViewModel.FromSchoolId && x.TenantId == copySchoolViewModel.TenantId && x.LovName.ToLower() == "ethnicityData").ToList();

                            if (EthnicityData?.Any() == true)
                            {
                                EthnicityData.ToList().ForEach(x => { x.SchoolId = copySchoolViewModel.schoolMaster.SchoolId; x.Id = (long)dpdownValueIde++; });
                                this.context?.DpdownValuelist.AddRange(EthnicityData);
                            }
                        }

                        this.context?.SaveChanges();
                        transaction?.Commit();
                        copySchoolViewModel._failure = false;
                        copySchoolViewModel._message = "School Copied Successfully";

                        copySchoolViewModel.schoolMaster.Membership = new HashSet<Membership>();
                        copySchoolViewModel.schoolMaster.DpdownValuelist = new HashSet<DpdownValuelist>();
                        copySchoolViewModel.schoolMaster.PermissionGroup = new HashSet<PermissionGroup>();
                    }
                    else
                    {
                        copySchoolViewModel._failure = true;
                        copySchoolViewModel._message = NORECORDFOUND;
                    }
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
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
                var userMasterData = this.context?.UserMaster.FirstOrDefault(c => c.TenantId == userViewModel.TenantId && c.EmailAddress == userViewModel.EmailAddress );

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

        private void AddSchoolYearAndSchoolCalendar(Guid tenantId, int schoolId, DateTime? startDate, DateTime? endDate, string? createdBy)
        {
            int? calenderId = 1;

            var schoolCalendar = new SchoolCalendars()
            {
                TenantId = tenantId,
                SchoolId = schoolId,
                CalenderId = (int)calenderId,
                Title = "Default Calendar",
                AcademicYear = Convert.ToDecimal(startDate?.Year),
                DefaultCalender = true,
                SessionCalendar = true,
                Days = "12345",
                StartDate = startDate,
                EndDate = endDate,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
            };
            this.context?.SchoolCalendars.Add(schoolCalendar);

            //int? MarkingPeriodId = 1;

            //var schoolYear = new SchoolYears()
            //{
            //    TenantId = tenantId,
            //    SchoolId = schoolId,
            //    MarkingPeriodId = (int)MarkingPeriodId,
            //    AcademicYear = Convert.ToDecimal(startDate.Year),
            //    Title = "Full Year",
            //    ShortName = "FY",
            //    SortOrder = 1,
            //    StartDate = startDate,
            //    EndDate = endDate,
            //    CreatedBy = createdBy,
            //    CreatedOn = DateTime.UtcNow
            //};
            //this.context?.SchoolYears.Add(schoolYear);
            //this.context?.SaveChanges();
        }
    }
}
