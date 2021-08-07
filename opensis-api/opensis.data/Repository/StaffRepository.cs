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
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public StaffRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Staff
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddStaff(StaffAddViewModel staffAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    //int? staffId = Utility.GetMaxPK(this.context, new Func<StaffMaster, int>(x => x.StaffId));
                    int? staffId = 1;
                    var dataExits = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId).Count();

                    if (dataExits > 0)
                    {
                        var staffData = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId).Max(x => x.StaffId);
                        if (staffData != null)
                        {
                            staffId = staffData + 1;
                        }
                    }
                    staffAddViewModel.staffMaster.StaffId = (int)staffId;
                    Guid GuidId = Guid.NewGuid();
                    var GuidIdExist = this.context?.StaffMaster.FirstOrDefault(x => x.StaffGuid == GuidId);
                    if (GuidIdExist != null)
                    {
                        staffAddViewModel._failure = true;
                        staffAddViewModel._message = "Guid is already exist, Please try again.";
                        return staffAddViewModel;
                    }
                    staffAddViewModel.staffMaster.StaffGuid = GuidId;
                    staffAddViewModel.staffMaster.CreatedOn = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(staffAddViewModel.staffMaster.StaffInternalId))
                    {
                        bool checkInternalID = CheckInternalID(staffAddViewModel.staffMaster.TenantId, staffAddViewModel.staffMaster.StaffInternalId);
                        if (checkInternalID == false)
                        {
                            staffAddViewModel.staffMaster = null;
                            staffAddViewModel.fieldsCategoryList = null;
                            staffAddViewModel._failure = true;
                            staffAddViewModel._message = "Staff InternalID Already Exist";
                            return staffAddViewModel;
                        }
                    }
                    else
                    {
                        staffAddViewModel.staffMaster.StaffInternalId = staffAddViewModel.staffMaster.StaffId.ToString();
                    }
                    //bool checkInternalID = CheckInternalID(staffAddViewModel.staffMaster.TenantId, staffAddViewModel.staffMaster.StaffInternalId);
                    //if (checkInternalID == true)
                    //{                       
                        //Add Staff Portal Access
                        if (!string.IsNullOrWhiteSpace(staffAddViewModel.PasswordHash) && !string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress))
                        {
                            UserMaster userMaster = new UserMaster();

                            var decrypted = Utility.Decrypt(staffAddViewModel.PasswordHash);
                            string passwordHash = Utility.GetHashedPassword(decrypted);

                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                            if (loginInfo == null)
                            {
                                var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.Profile == "Teacher");

                                userMaster.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                userMaster.TenantId = staffAddViewModel.staffMaster.TenantId;
                                userMaster.UserId = staffAddViewModel.staffMaster.StaffId;
                                userMaster.LangId = 1;
                                userMaster.MembershipId = membership.MembershipId;
                                userMaster.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                userMaster.PasswordHash = passwordHash;
                                userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName;
                                userMaster.IsActive = staffAddViewModel.staffMaster.PortalAccess;

                                this.context?.UserMaster.Add(userMaster);
                                this.context?.SaveChanges();
                            }
                            else
                            {
                                staffAddViewModel.staffMaster = null;
                                staffAddViewModel._failure = true;
                                staffAddViewModel._message = "Staff Login Email Already Exist";
                                return staffAddViewModel;
                            }
                        }

                        this.context?.StaffMaster.Add(staffAddViewModel.staffMaster);
                        this.context?.SaveChanges();

                        //Insert data into StaffSchoolInfo table            
                        int? Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => (int)x.Id));
                        var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();
                        var StaffSchoolInfoData = new StaffSchoolInfo() { TenantId = staffAddViewModel.staffMaster.TenantId, SchoolId = staffAddViewModel.staffMaster.SchoolId, StaffId = staffAddViewModel.staffMaster.StaffId, SchoolAttachedId = staffAddViewModel.staffMaster.SchoolId, Id = (int)Id, SchoolAttachedName = schoolName, StartDate = DateTime.UtcNow, CreatedOn = DateTime.UtcNow, CreatedBy = staffAddViewModel.staffMaster.CreatedBy,Profile= "Teacher" };
                        this.context?.StaffSchoolInfo.Add(StaffSchoolInfoData);
                        this.context?.SaveChanges();

                        if (staffAddViewModel.fieldsCategoryList != null && staffAddViewModel.fieldsCategoryList.ToList().Count > 0)
                        {
                            var fieldsCategory = staffAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffAddViewModel.SelectedCategoryId);
                            if (fieldsCategory != null)
                            {
                                foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                {
                                    if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                    {
                                        customFields.CustomFieldsValue.FirstOrDefault().Module = "Staff";
                                        customFields.CustomFieldsValue.FirstOrDefault().CategoryId = customFields.CategoryId;
                                        customFields.CustomFieldsValue.FirstOrDefault().FieldId = customFields.FieldId;
                                        customFields.CustomFieldsValue.FirstOrDefault().CustomFieldTitle = customFields.Title;
                                        customFields.CustomFieldsValue.FirstOrDefault().CustomFieldType = customFields.Type;
                                    customFields.CustomFieldsValue.FirstOrDefault().TenantId = staffAddViewModel.staffMaster.TenantId;
                                    customFields.CustomFieldsValue.FirstOrDefault().SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                        customFields.CustomFieldsValue.FirstOrDefault().TargetId = staffAddViewModel.staffMaster.StaffId;
                                        this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                        this.context?.SaveChanges();
                                    }
                                }

                            }
                        }

                        staffAddViewModel._failure = false;
                        staffAddViewModel._message = "Staff Added Successfully";
                    //}
                    //else
                    //{
                    //    staffAddViewModel.staffMaster = null;
                    //    staffAddViewModel.fieldsCategoryList = null;
                    //    staffAddViewModel._failure = true;
                    //    staffAddViewModel._message = "Staff InternalID Already Exist";
                    //}
                    transaction.Commit();
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    staffAddViewModel._message = es.Message;
                    staffAddViewModel._failure = true;
                    staffAddViewModel._tenantName = staffAddViewModel._tenantName;
                    staffAddViewModel._token = staffAddViewModel._token;
                }
            }
            return staffAddViewModel;
        }

        //Checking Internal ID
        private bool CheckInternalID(Guid TenantId, string InternalID)
        {
            if (InternalID != null && InternalID != "")
            {
                var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == TenantId && x.StaffInternalId == InternalID).ToList();
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
        /// Get All Staff List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StaffListModel GetAllStaffList(PageResult pageResult)
        {
            StaffListModel staffListModel = new StaffListModel();
            IQueryable<StaffMaster> transactionIQ = null;

            var schoolAttachedStaffId = this.context?.StaffSchoolInfo.Where(x => x.TenantId == pageResult.TenantId && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolAttachedId == pageResult.SchoolId : true) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date : true)).Select(s => s.StaffId).ToList();

            var StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && x.Profile.ToLower() != "super administrator" && schoolAttachedStaffId.Contains(x.StaffId));

            //var StaffMasterList = this.context?.StaffMaster.Include(x => x.StaffSchoolInfo).Where(x => x.TenantId == pageResult.TenantId && x.Profile.ToLower() != "super administrator" && (pageResult.SearchAllSchool == false || pageResult.SearchAllSchool == null ? x.SchoolId == pageResult.SchoolId : true) && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.StaffSchoolInfo.FirstOrDefault().EndDate == null || x.StaffSchoolInfo.FirstOrDefault().EndDate >= DateTime.UtcNow : true));

            try
            {
                if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                {

                    transactionIQ = StaffMasterList;
                }
                else
                {
                    if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        transactionIQ = StaffMasterList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.StaffInternalId != null && x.StaffInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                    x.JobTitle != null && x.JobTitle.Contains(Columnvalue) ||
                                                                    x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue) ||
                                                                    x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue));
                        //var ProfileFilter = StaffMasterList.Where(x => x.StaffSchoolInfo.FirstOrDefault() != null ? x.StaffSchoolInfo.FirstOrDefault().Profile.ToLower().Contains(Columnvalue.ToLower()) : string.Empty.Contains(Columnvalue));

                        var ProfileFilter = StaffMasterList.Where(x => x.StaffSchoolInfo.Count > 0 ? x.StaffSchoolInfo.Any(x => x.Profile.ToLower().Contains(Columnvalue.ToLower()) && x.SchoolAttachedId==pageResult.SchoolId) : string.Empty.Contains(Columnvalue));

                        if (ProfileFilter.ToList().Count() > 0)
                        {
                            transactionIQ = transactionIQ.Concat(ProfileFilter);
                        }
                    }
                    else
                    {
                        if (pageResult.FilterParams.Any(x => x.ColumnName.ToLower() == "profile"))
                        {
                            var filterValue = pageResult.FilterParams.Where(x => x.ColumnName.ToLower() == "profile").Select(x => x.FilterValue).FirstOrDefault();
                            //var profileFilterData = StaffMasterList.Where(x => x.StaffSchoolInfo.FirstOrDefault().Profile.ToLower() == filterValue.ToLower());
                            var profileFilterData = StaffMasterList.Where(x => x.StaffSchoolInfo.Any(x => x.Profile.ToLower() == filterValue.ToLower() && x.SchoolAttachedId == pageResult.SchoolId));

                            if (profileFilterData.ToList().Count() > 0)
                            {
                                //transactionIQ = transactionIQ.Concat(a);
                                StaffMasterList = profileFilterData;
                                var indexValue = pageResult.FilterParams.FindIndex(x => x.ColumnName.ToLower() == "profile");
                                pageResult.FilterParams.RemoveAt(indexValue);
                            }
                        }   

                        transactionIQ = Utility.FilteredData(pageResult.FilterParams, StaffMasterList).AsQueryable();
                    }
                    //transactionIQ = transactionIQ.Distinct();
                }

                if (pageResult.DobStartDate != null && pageResult.DobEndDate != null)
                {
                    var filterInDateRange = transactionIQ.Where(x => x.Dob >= pageResult.DobStartDate && x.Dob <= pageResult.DobEndDate).AsQueryable();

                    if (filterInDateRange.ToList().Count() > 0)
                    {
                        transactionIQ = filterInDateRange;
                    }
                }

                if (pageResult.FullName != null)
                {
                    var staffName = pageResult.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (staffName.Length > 1)
                    {
                        var firstName = staffName.First();
                        var lastName = staffName.Last();
                        pageResult.FullName = null;

                        if (pageResult.FullName == null)
                        {
                            var nameSearch = transactionIQ.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.FirstGivenName.StartsWith(firstName.ToString()) && x.LastFamilyName.StartsWith(lastName.ToString()));

                            //transactionIQ = transactionIQ.Concat(nameSearch);
                            transactionIQ = nameSearch;
                        }
                    }
                    else
                    {
                        var nameSearch = transactionIQ.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (x.FirstGivenName.StartsWith(pageResult.FullName) || x.LastFamilyName.StartsWith(pageResult.FullName)));

                        //transactionIQ = transactionIQ.Concat(nameSearch);
                        transactionIQ = nameSearch;
                    }
                }

                if (pageResult.SortingModel != null)
                {
                    switch (pageResult.SortingModel.SortColumn.ToLower())
                    {
                        case "profile":

                            if (pageResult.SortingModel.SortDirection.ToLower() == "asc")
                            {
                                transactionIQ = transactionIQ.OrderBy(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault().Profile : null);
                            }
                            else
                            {
                                transactionIQ = transactionIQ.OrderByDescending(a => a.StaffSchoolInfo.Count > 0 ? a.StaffSchoolInfo.FirstOrDefault().Profile : null);
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
                    transactionIQ = transactionIQ.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        StaffInternalId = e.StaffInternalId,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        //Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => x.SchoolAttachedId == pageResult.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault().Profile : null,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x =>(pageResult.SearchAllSchool !=true)? x.SchoolAttachedId == pageResult.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date) : x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date).FirstOrDefault().Profile : null,
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
                        StaffPhoto = pageResult.ProfilePhoto != null ? e.StaffPhoto : null,
                        CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                        UpdatedOn = e.UpdatedOn,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate }).ToList() : null
                    }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                }
                else
                {
                    transactionIQ = transactionIQ.Select(e => new StaffMaster
                    {
                        TenantId = e.TenantId,
                        StaffId = e.StaffId,
                        StaffInternalId = e.StaffInternalId,
                        FirstGivenName = e.FirstGivenName,
                        MiddleName = e.MiddleName,
                        LastFamilyName = e.LastFamilyName,
                        //Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => x.SchoolAttachedId == pageResult.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).FirstOrDefault().Profile : null,
                        Profile = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Where(x => (pageResult.SearchAllSchool != true) ? x.SchoolAttachedId == pageResult.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date) : x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date).FirstOrDefault().Profile : null,
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
                        StaffPhoto = pageResult.ProfilePhoto != null ? e.StaffPhoto : null,
                        CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                        CreatedOn = e.CreatedOn,
                        UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == pageResult.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                        UpdatedOn = e.UpdatedOn,
                        StaffSchoolInfo = e.StaffSchoolInfo.Count > 0 ? e.StaffSchoolInfo.Select(s => new StaffSchoolInfo { StartDate = s.StartDate, EndDate = s.EndDate }).ToList() : null
                    });
                }
                
                //var staffList = transactionIQ.Select(s => new GetStaffListForView
                //{
                //    TenantId = s.TenantId,
                //    StaffId = s.StaffId,
                //    StaffInternalId = s.StaffInternalId,
                //    FirstGivenName = s.FirstGivenName,
                //    MiddleName = s.MiddleName,
                //    LastFamilyName = s.LastFamilyName,
                //    Profile = s.Profile,
                //    JobTitle = s.JobTitle,
                //    SchoolEmail = s.SchoolEmail,
                //    MobilePhone = s.MobilePhone,
                //}).ToList();

                staffListModel.TenantId = pageResult.TenantId;
                //staffListModel.getStaffListForView = staffList;
                staffListModel.staffMaster = transactionIQ.ToList();
                staffListModel.TotalCount = totalCount;
                staffListModel.PageNumber = pageResult.PageNumber;
                staffListModel._pageSize = pageResult.PageSize;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
                staffListModel._failure = false;
            }
            catch (Exception es)
            {
                staffListModel._message = es.Message;
                staffListModel._failure = true;
                staffListModel._tenantName = pageResult._tenantName;
                staffListModel._token = pageResult._token;
            }
            return staffListModel;
        }

        /// <summary>
        ///  Get Staff By Id
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel ViewStaff(StaffAddViewModel staffAddViewModel)
        {
            StaffAddViewModel staffView = new StaffAddViewModel();
            try
            {
                var staffData = this.context?.StaffMaster.Include(x=>x.StaffSchoolInfo).FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId /*&& x.SchoolId==staffAddViewModel.staffMaster.SchoolId*/ &&  x.StaffId == staffAddViewModel.staffMaster.StaffId);
                if (staffData != null)
                {
                    if (staffData.StaffSchoolInfo.Count() > 0)
                    {
                        staffData.Profile = staffData.StaffSchoolInfo.FirstOrDefault().Profile;
                    }
                    staffView.staffMaster = new StaffMaster()
                    {
                        TenantId= staffData.TenantId,
                        SchoolId= staffData.SchoolId,
                        AlternateId= staffData.AlternateId,
                        BusDropoff= staffData.BusDropoff,
                        BusNo= staffData.BusNo,
                        BusPickup= staffData.BusPickup,
                        CountryOfBirth= staffData.CountryOfBirth,
                        DisabilityDescription= staffData.DisabilityDescription,
                        DistrictId= staffData.DistrictId,
                        Dob= staffData.Dob,
                        EmergencyEmail= staffData.EmergencyEmail,
                        EmergencyFirstName= staffData.EmergencyFirstName,
                        EmergencyHomePhone= staffData.EmergencyHomePhone,
                        EmergencyLastName= staffData.EmergencyLastName,
                        EmergencyMobilePhone= staffData.EmergencyMobilePhone,
                        EmergencyWorkPhone= staffData.EmergencyWorkPhone,
                        EndDate= staffData.EndDate,
                        Ethnicity= staffData.Ethnicity,
                        Facebook= staffData.Facebook,
                        FirstGivenName= staffData.FirstGivenName,
                        FirstLanguage= staffData.FirstLanguage,
                        FirstLanguageNavigation= staffData.FirstLanguageNavigation,
                        Gender= staffData.Gender,
                        HomeAddressCity= staffData.HomeAddressCity,
                        HomeAddressCountry= staffData.HomeAddressCountry,
                        HomeAddressLineOne= staffData.HomeAddressLineOne,
                        HomeAddressLineTwo= staffData.HomeAddressLineTwo,
                        HomeAddressState= staffData.HomeAddressState,
                        HomeAddressZip= staffData.HomeAddressZip,
                        HomePhone= staffData.HomePhone,
                        HomeroomTeacher= staffData.HomeroomTeacher,
                        Instagram= staffData.Instagram,
                        JobTitle= staffData.JobTitle,
                        JoiningDate= staffData.JoiningDate,
                        LastFamilyName= staffData.LastFamilyName,
                        Linkedin= staffData.Linkedin,
                        LoginEmailAddress= staffData.LoginEmailAddress,
                        MailingAddressCity= staffData.MailingAddressCity,
                        MailingAddressCountry= staffData.MailingAddressCountry,
                        MailingAddressLineOne= staffData.MailingAddressLineOne,
                        MailingAddressLineTwo= staffData.MailingAddressLineTwo,
                        MailingAddressSameToHome= staffData.MailingAddressSameToHome,
                        MailingAddressState= staffData.MailingAddressState,
                        MailingAddressZip= staffData.MailingAddressZip,
                        MaritalStatus= staffData.MaritalStatus,
                        MiddleName= staffData.MiddleName,
                        MobilePhone= staffData.MobilePhone,
                        OfficePhone= staffData.OfficePhone,
                        Nationality= staffData.Nationality,
                        OtherGovtIssuedNumber= staffData.OtherGovtIssuedNumber,
                        OtherGradeLevelTaught= staffData.OtherGradeLevelTaught,
                        OtherSubjectTaught= staffData.OtherSubjectTaught,
                        PersonalEmail= staffData.PersonalEmail,
                        PhysicalDisability= staffData.PhysicalDisability,
                        PortalAccess= staffData.PortalAccess,
                        PreferredName= staffData.PreferredName,
                        PreviousName= staffData.PreviousName,
                        PrimaryGradeLevelTaught= staffData.PrimaryGradeLevelTaught,
                        PrimarySubjectTaught= staffData.PrimarySubjectTaught,
                        Profile= staffData.Profile,
                        Race= staffData.Race,
                        RelationshipToStaff= staffData.RelationshipToStaff,
                        Salutation= staffData.Salutation,
                        SchoolEmail= staffData.SchoolEmail,
                        SecondLanguage= staffData.SecondLanguage,
                        SecondLanguageNavigation= staffData.SecondLanguageNavigation,
                        SocialSecurityNumber= staffData.SocialSecurityNumber,
                        StaffGuid= staffData.StaffGuid,
                        StaffId= staffData.StaffId,
                        StaffInternalId= staffData.StaffInternalId,
                        StaffPhoto= staffData.StaffPhoto,
                        StateId= staffData.StateId,
                        Suffix= staffData.Suffix,
                        ThirdLanguage= staffData.ThirdLanguage,
                        Twitter= staffData.Twitter,
                        Youtube=staffData.Youtube,
                        CreatedOn= staffData.CreatedOn,
                        UpdatedOn= staffData.UpdatedOn,
                        UpdatedBy = (staffData.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == staffData.UpdatedBy).Name : null,
                        CreatedBy = (staffData.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == staffData.CreatedBy).Name : null,
                        StaffSchoolInfo= staffData.StaffSchoolInfo.ToList()
                    };

                    var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.Module == "Staff").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                            CreatedBy= (y.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == y.CreatedBy).Name : null,
                            CreatedOn =DateTime.UtcNow,
                            UpdatedOn = y.UpdatedOn,
                            UpdatedBy = (y.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u =>  u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == y.UpdatedBy).Name : null,
                            CustomFields = y.CustomFields.Select(z => new CustomFields
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
                                UpdatedBy = (z.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == z.UpdatedBy).Name : null,
                                CreatedBy = (z.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == z.CreatedBy).Name : null,
                                CreatedOn =z.CreatedOn,
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == staffAddViewModel.staffMaster.StaffId).Select(g=>new CustomFieldsValue()
                                {
                                    TenantId=g.TenantId,
                                    SchoolId=g.SchoolId,
                                    CategoryId=g.CategoryId,
                                    FieldId=g.FieldId,
                                    TargetId=g.TargetId,
                                    Module=g.Module,
                                    CustomFieldTitle=g.CustomFieldTitle,
                                    CustomFieldType=g.CustomFieldType,
                                    CustomFieldValue=g.CustomFieldValue,
                                    UpdateOn = g.UpdateOn,
                                    UpdatedBy = (g.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == g.UpdatedBy).Name : null,
                                    CreatedBy = (g.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffAddViewModel.staffMaster.TenantId && u.EmailAddress == g.CreatedBy).Name : null,
                                    CreatedOn = g.CreatedOn,
                                }).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();
                    staffView.fieldsCategoryList = customFields;
                    staffView._tenantName = staffAddViewModel._tenantName;
                    staffView._token = staffAddViewModel._token;
                }
                else
                {
                    staffView._tenantName = staffAddViewModel._tenantName;
                    staffView._token = staffAddViewModel._token;
                    staffView._failure = true;
                    staffView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffView._tenantName = staffAddViewModel._tenantName;
                staffView._token = staffAddViewModel._token;
                staffView._failure = true;
                staffView._message = es.Message;
            }
            return staffView;
        }

        /// <summary>
        /// Update Staff
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel UpdateStaff(StaffAddViewModel staffAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffInternalId == staffAddViewModel.staffMaster.StaffInternalId && x.StaffInternalId != null && x.StaffId != staffAddViewModel.staffMaster.StaffId).ToList();
                    if (checkInternalId.Count() > 0)
                    {
                        staffAddViewModel.staffMaster = null;
                        staffAddViewModel.fieldsCategoryList = null;
                        staffAddViewModel._failure = true;
                        staffAddViewModel._message = "Staff InternalID Already Exist";
                    }
                    else
                    {
                        var staffUpdate = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffId == staffAddViewModel.staffMaster.StaffId);

                        if (string.IsNullOrEmpty(staffAddViewModel.staffMaster.StaffInternalId))
                        {
                            staffAddViewModel.staffMaster.StaffInternalId = staffUpdate.StaffInternalId;
                        }

                        //Add or Update staff portal access
                        if (staffUpdate.LoginEmailAddress != null)
                        {
                            if (!string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress))
                            {
                                if (staffUpdate.LoginEmailAddress != staffAddViewModel.staffMaster.LoginEmailAddress)
                                {
                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                                    if (loginInfo != null)
                                    {
                                        staffAddViewModel.staffMaster = null;
                                        staffAddViewModel._failure = true;
                                        staffAddViewModel._message = "Staff Login Email Already Exist";
                                        return staffAddViewModel;
                                    }
                                    else
                                    {
                                        var loginInfoData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffUpdate.LoginEmailAddress);

                                        loginInfoData.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                        loginInfoData.IsActive = staffAddViewModel.staffMaster.PortalAccess;

                                        this.context?.UserMaster.Add(loginInfoData);
                                        this.context?.SaveChanges();

                                        //Update Parent Login in ParentInfo table.
                                        //staffUpdate.LoginEmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                    }
                                }
                                else
                                {
                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffUpdate.LoginEmailAddress);

                                    loginInfo.IsActive = staffAddViewModel.staffMaster.PortalAccess;

                                    this.context?.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(staffAddViewModel.staffMaster.LoginEmailAddress) && !string.IsNullOrWhiteSpace(staffAddViewModel.PasswordHash))
                            {
                                var decrypted = Utility.Decrypt(staffAddViewModel.PasswordHash);
                                string passwordHash = Utility.GetHashedPassword(decrypted);

                                UserMaster userMaster = new UserMaster();

                                var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.EmailAddress == staffAddViewModel.staffMaster.LoginEmailAddress);

                                if (loginInfo == null)
                                {
                                    var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.Profile == "Teacher");

                                    userMaster.SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                    userMaster.TenantId = staffAddViewModel.staffMaster.TenantId;
                                    userMaster.UserId = staffAddViewModel.staffMaster.StaffId;
                                    userMaster.LangId = 1;
                                    userMaster.MembershipId = membership.MembershipId;
                                    userMaster.EmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                    userMaster.PasswordHash = passwordHash;
                                    userMaster.Name = staffAddViewModel.staffMaster.FirstGivenName;
                                    userMaster.UpdatedOn = DateTime.UtcNow;
                                    userMaster.IsActive = staffAddViewModel.staffMaster.PortalAccess;

                                    this.context?.UserMaster.Add(userMaster);
                                    this.context?.SaveChanges();


                                    //Update LoginEmailAddress in StaffMaster table.
                                    //staffUpdate.LoginEmailAddress = staffAddViewModel.staffMaster.LoginEmailAddress;
                                }
                                else
                                {
                                    staffAddViewModel.staffMaster = null;
                                    staffAddViewModel._failure = true;
                                    staffAddViewModel._message = "Staff Login Email Already Exist";
                                    return staffAddViewModel;
                                }
                            }
                        }

                        staffAddViewModel.staffMaster.StaffGuid = staffUpdate.StaffGuid;
                        staffAddViewModel.staffMaster.UpdatedOn = DateTime.UtcNow;
                        staffAddViewModel.staffMaster.CreatedOn = staffUpdate.CreatedOn;
                        staffAddViewModel.staffMaster.CreatedBy = staffUpdate.CreatedBy;
                        this.context.Entry(staffUpdate).CurrentValues.SetValues(staffAddViewModel.staffMaster);
                        this.context?.SaveChanges();

                        if (staffAddViewModel.fieldsCategoryList != null && staffAddViewModel.fieldsCategoryList.ToList().Count > 0)
                        {
                            var fieldsCategory = staffAddViewModel.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staffAddViewModel.SelectedCategoryId);
                            if (fieldsCategory != null)
                            {
                                foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                {
                                    var customFieldValueData = this.context?.CustomFieldsValue.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.SchoolId == staffAddViewModel.staffMaster.SchoolId && x.CategoryId == customFields.CategoryId && x.FieldId == customFields.FieldId && x.Module == "Staff" && x.TargetId == staffAddViewModel.staffMaster.StaffId);
                                    if (customFieldValueData != null)
                                    {
                                        this.context?.CustomFieldsValue.RemoveRange(customFieldValueData);
                                    }
                                    if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                    {
                                        customFields.CustomFieldsValue.FirstOrDefault().Module = "Staff";
                                        customFields.CustomFieldsValue.FirstOrDefault().CategoryId = customFields.CategoryId;
                                        customFields.CustomFieldsValue.FirstOrDefault().FieldId = customFields.FieldId;
                                        customFields.CustomFieldsValue.FirstOrDefault().CustomFieldTitle = customFields.Title;
                                        customFields.CustomFieldsValue.FirstOrDefault().CustomFieldType = customFields.Type;
                                        customFields.CustomFieldsValue.FirstOrDefault().SchoolId = staffAddViewModel.staffMaster.SchoolId;
                                        customFields.CustomFieldsValue.FirstOrDefault().TenantId = staffAddViewModel.staffMaster.TenantId;
                                        customFields.CustomFieldsValue.FirstOrDefault().TargetId = staffAddViewModel.staffMaster.StaffId;
                                        this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                        this.context?.SaveChanges();
                                    }
                                }
                            }
                        }
                        staffAddViewModel._failure = false;
                        staffAddViewModel._message = "Staff Updated Successfully";
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    staffAddViewModel.staffMaster = null;
                    staffAddViewModel._failure = true;
                    staffAddViewModel._message = ex.Message;
                }
            }
            return staffAddViewModel;
        }

        /// <summary>
        /// Check Staff Internal Id
        /// </summary>
        /// <param name="checkStaffInternalIdViewModel"></param>
        /// <returns></returns>
        public CheckStaffInternalIdViewModel CheckStaffInternalId(CheckStaffInternalIdViewModel checkStaffInternalIdViewModel)
        {
            var checkInternalId = this.context?.StaffMaster.Where(x => x.TenantId == checkStaffInternalIdViewModel.TenantId && x.StaffInternalId == checkStaffInternalIdViewModel.StaffInternalId).ToList();
            if (checkInternalId.Count() > 0)
            {
                checkStaffInternalIdViewModel.IsValidInternalId = false;
                checkStaffInternalIdViewModel._message = "Staff Internal Id Already Exist";
            }
            else
            {
                checkStaffInternalIdViewModel.IsValidInternalId = true;
                checkStaffInternalIdViewModel._message = "Staff Internal Id Is Valid";
            }
            return checkStaffInternalIdViewModel;
        }

        /// <summary>
        /// Add Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel AddStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);
                    if (staffMaster != null)
                    {
                        //staffMaster.Profile = staffSchoolInfoAddViewModel.Profile;
                        staffMaster.JobTitle = staffSchoolInfoAddViewModel.JobTitle;
                        staffMaster.JoiningDate = staffSchoolInfoAddViewModel.JoiningDate;
                        staffMaster.EndDate = staffSchoolInfoAddViewModel.EndDate;
                        staffMaster.HomeroomTeacher = staffSchoolInfoAddViewModel.HomeroomTeacher;
                        staffMaster.PrimaryGradeLevelTaught = staffSchoolInfoAddViewModel.PrimaryGradeLevelTaught;
                        staffMaster.OtherGradeLevelTaught = staffSchoolInfoAddViewModel.OtherGradeLevelTaught;
                        staffMaster.PrimarySubjectTaught = staffSchoolInfoAddViewModel.PrimarySubjectTaught;
                        staffMaster.OtherSubjectTaught = staffSchoolInfoAddViewModel.OtherSubjectTaught;
                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.staffSchoolInfoList != null && staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList().Count > 0)
                    {
                        int? Id = 0;
                        Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));
                        foreach (var staffSchoolInfo in staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList())
                        {
                            staffSchoolInfo.Id = (int)Id;
                            staffSchoolInfo.CreatedOn = DateTime.UtcNow;
                            this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                            Id++;
                        }
                        this.context?.SaveChanges();
                    }

                    transaction.Commit();
                    staffSchoolInfoAddViewModel._failure = false;
                    staffSchoolInfoAddViewModel._message = "Staff School Info Added Successfully";
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    staffSchoolInfoAddViewModel._failure = true;
                    staffSchoolInfoAddViewModel._message = es.Message;
                }
            }
            return staffSchoolInfoAddViewModel;
        }

        /// <summary>
        /// Get Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel ViewStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            StaffSchoolInfoAddViewModel staffSchoolInfoView = new StaffSchoolInfoAddViewModel();
            try
            {              
                var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault().TenantId && x.StaffId == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault().StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);
                if (staffMaster != null)
                {
                    staffSchoolInfoView.Profile = staffMaster.Profile;
                    staffSchoolInfoView.JobTitle = staffMaster.JobTitle;
                    staffSchoolInfoView.JoiningDate = staffMaster.JoiningDate;
                    staffSchoolInfoView.EndDate = staffMaster.EndDate;
                    staffSchoolInfoView.HomeroomTeacher = staffMaster.HomeroomTeacher;
                    staffSchoolInfoView.PrimaryGradeLevelTaught = staffMaster.PrimaryGradeLevelTaught;
                    staffSchoolInfoView.PrimarySubjectTaught = staffMaster.PrimarySubjectTaught;
                    staffSchoolInfoView.OtherGradeLevelTaught = staffMaster.OtherGradeLevelTaught;
                    staffSchoolInfoView.OtherSubjectTaught = staffMaster.OtherSubjectTaught;
                    staffSchoolInfoView.TenantId = staffMaster.TenantId;
                    staffSchoolInfoView.SchoolId = staffMaster.SchoolId;
                    staffSchoolInfoView.StaffId = staffMaster.StaffId;
                    staffSchoolInfoView._failure = false;
                    
                }
                else
                {
                    staffSchoolInfoView._failure = true;
                    staffSchoolInfoView._message = NORECORDFOUND;
                }
                var staffSchoolInfo = this.context?.StaffSchoolInfo.Where(x => x.TenantId == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault().TenantId && x.StaffId == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault().StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId).ToList();
                if(staffSchoolInfo.Count() > 0)
                {
                    staffSchoolInfoView.staffSchoolInfoList= staffSchoolInfo;
                }
            }
            catch (Exception es)
            {
                staffSchoolInfoView._failure = true;
                staffSchoolInfoView._message = es.Message;
            }
            return staffSchoolInfoView;
        }

        /// <summary>
        /// Update Staff School Info
        /// </summary>
        /// <param name="staffSchoolInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffSchoolInfoAddViewModel UpdateStaffSchoolInfo(StaffSchoolInfoAddViewModel staffSchoolInfoAddViewModel)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var staffMaster = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);
                    if (staffMaster != null)
                    {
                        staffMaster.Profile = staffSchoolInfoAddViewModel.Profile;
                        staffMaster.JobTitle = staffSchoolInfoAddViewModel.JobTitle;
                        staffMaster.JoiningDate = staffSchoolInfoAddViewModel.JoiningDate;
                        staffMaster.EndDate = staffSchoolInfoAddViewModel.EndDate;
                        staffMaster.HomeroomTeacher = staffSchoolInfoAddViewModel.HomeroomTeacher;
                        staffMaster.PrimaryGradeLevelTaught = staffSchoolInfoAddViewModel.PrimaryGradeLevelTaught;
                        staffMaster.OtherGradeLevelTaught = staffSchoolInfoAddViewModel.OtherGradeLevelTaught;
                        staffMaster.PrimarySubjectTaught = staffSchoolInfoAddViewModel.PrimarySubjectTaught;
                        staffMaster.OtherSubjectTaught = staffSchoolInfoAddViewModel.OtherSubjectTaught;

                        var userMaster = this.context?.UserMaster.Include(x => x.Membership).FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.UserId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId);

                        if (userMaster != null)
                        {
                            var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId && x.Profile.ToLower() == staffSchoolInfoAddViewModel.staffSchoolInfoList.FirstOrDefault().Profile.ToLower());

                            if (membership != null)
                            {
                                userMaster.MembershipId = membership.MembershipId;                                
                            }
                            else
                            {
                                userMaster.MembershipId = 4;
                            }

                            userMaster.UpdatedOn = DateTime.Now;
                        }

                        this.context?.SaveChanges();
                    }

                    if (staffSchoolInfoAddViewModel.staffSchoolInfoList != null && staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList().Count > 0)
                    {
                        var staffSchoolInfoData = this.context?.StaffSchoolInfo.Where(x => x.TenantId == staffSchoolInfoAddViewModel.TenantId && x.StaffId == staffSchoolInfoAddViewModel.StaffId && x.SchoolId == staffSchoolInfoAddViewModel.SchoolId).ToList();
                        if (staffSchoolInfoData.Count() > 0)
                        {
                            this.context?.StaffSchoolInfo.RemoveRange(staffSchoolInfoData);
                            this.context?.SaveChanges();
                        }
                        int? Id = 0;
                        Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => x.Id));
                        foreach (var staffSchoolInfo in staffSchoolInfoAddViewModel.staffSchoolInfoList.ToList())
                        {
                            staffSchoolInfo.Id = (int)Id;
                            staffSchoolInfo.UpdatedOn = DateTime.UtcNow;
                            staffSchoolInfo.StaffMaster = null;
                            this.context?.StaffSchoolInfo.Add(staffSchoolInfo);
                            Id++;
                        }
                        this.context?.SaveChanges();
                    }
                    transaction.Commit();
                    staffSchoolInfoAddViewModel._failure = false;
                    staffSchoolInfoAddViewModel._message = "Staff School Info Updated Successfully";
                }
                catch (Exception es)
                {
                    transaction.Rollback();
                    staffSchoolInfoAddViewModel._failure = true;
                    staffSchoolInfoAddViewModel._message = es.Message;
                }
            }
            return staffSchoolInfoAddViewModel;
        }
        
        /// <summary>
        /// Add Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel AddStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            try
            {
                int? Id = Utility.GetMaxPK(this.context, new Func<StaffCertificateInfo, int>(x => x.Id));
                staffCertificateInfoAddViewModel.staffCertificateInfo.Id = (int)Id;
                staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedOn = DateTime.UtcNow;
                this.context?.StaffCertificateInfo.Add(staffCertificateInfoAddViewModel.staffCertificateInfo);
                this.context?.SaveChanges();
                staffCertificateInfoAddViewModel._failure = false;
                staffCertificateInfoAddViewModel._message = "Staff Certificate Added Successfully";
            }
            catch (Exception es)
            {
                staffCertificateInfoAddViewModel._message = es.Message;
                staffCertificateInfoAddViewModel._failure = true;
                staffCertificateInfoAddViewModel._tenantName = staffCertificateInfoAddViewModel._tenantName;
                staffCertificateInfoAddViewModel._token = staffCertificateInfoAddViewModel._token;
            }

            return staffCertificateInfoAddViewModel;
        }
        
        /// <summary>
        /// Get All Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoListModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoListModel GetAllStaffCertificateInfo(StaffCertificateInfoListModel staffCertificateInfoListModel)
        {
            StaffCertificateInfoListModel staffCertificateInfoListView = new StaffCertificateInfoListModel();          
            //IQueryable<StaffCertificateInfo> transactionIQ = null;           

            try
            {
                var staffCertificateInfoList = this.context?.StaffCertificateInfo.Where(x => x.TenantId == staffCertificateInfoListModel.TenantId && x.SchoolId == staffCertificateInfoListModel.SchoolId && x.StaffId == staffCertificateInfoListModel.StaffId).Select(e=>new StaffCertificateInfo()
                { 
                    TenantId=e.TenantId,
                    SchoolId=e.SchoolId,
                    StaffId=e.StaffId,
                    CertificationName=e.CertificationName,
                    ShortName=e.ShortName,
                    CertificationCode=e.CertificationCode,
                    PrimaryCertification=e.PrimaryCertification,
                    CertificationDate=e.CertificationDate,
                    CertificationExpiryDate=e.CertificationExpiryDate,
                    CertificationDescription=e.CertificationDescription,
                    Id=e.Id,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffCertificateInfoListModel.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn=e.CreatedOn,
                    UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == staffCertificateInfoListModel.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn=e.UpdatedOn
                }).ToList();

                staffCertificateInfoListView.staffCertificateInfoList = staffCertificateInfoList;
                staffCertificateInfoListView._tenantName = staffCertificateInfoListModel._tenantName;
                staffCertificateInfoListView.TenantId = staffCertificateInfoListModel.TenantId;
                staffCertificateInfoListView.SchoolId = staffCertificateInfoListModel.SchoolId;
                staffCertificateInfoListView.StaffId = staffCertificateInfoListModel.StaffId;
                staffCertificateInfoListView._token = staffCertificateInfoListModel._token;

                if (staffCertificateInfoList.Count>0)
                {   
                    staffCertificateInfoListView._failure = false;
                }
                else
                {
                    staffCertificateInfoListView._failure = true;
                    staffCertificateInfoListView._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoListView._message = es.Message;
                staffCertificateInfoListView._failure = true;
                staffCertificateInfoListView._tenantName = staffCertificateInfoListModel._tenantName;
                staffCertificateInfoListView._token = staffCertificateInfoListModel._token;
            }
            return staffCertificateInfoListView;
        }
        
        /// <summary>
        /// Update Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel UpdateStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            try
            {
                var staffCertificateInfoUpdate = this.context?.StaffCertificateInfo.FirstOrDefault(x => x.TenantId == staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId && x.SchoolId == staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId && x.StaffId == staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId && x.Id == staffCertificateInfoAddViewModel.staffCertificateInfo.Id);
                if (staffCertificateInfoUpdate != null)
                {
                    staffCertificateInfoAddViewModel.staffCertificateInfo.UpdatedOn = DateTime.UtcNow;
                    staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedBy = staffCertificateInfoUpdate.CreatedBy;
                    staffCertificateInfoAddViewModel.staffCertificateInfo.CreatedOn = staffCertificateInfoUpdate.CreatedOn;
                    this.context.Entry(staffCertificateInfoUpdate).CurrentValues.SetValues(staffCertificateInfoAddViewModel.staffCertificateInfo);
                    this.context?.SaveChanges();
                    staffCertificateInfoAddViewModel._failure = false;
                    staffCertificateInfoAddViewModel._message= "Staff Certificate Updated Successfully";
                }
                else
                {
                    staffCertificateInfoAddViewModel.staffCertificateInfo = null;
                    staffCertificateInfoAddViewModel._failure = true;
                    staffCertificateInfoAddViewModel._message = NORECORDFOUND;
                }
            }
            catch(Exception es)
            {
                staffCertificateInfoAddViewModel._failure = true;
                staffCertificateInfoAddViewModel._message = es.Message;
            }
            return staffCertificateInfoAddViewModel;
        }
        
        /// <summary>
        /// Delete Staff Certificate Info
        /// </summary>
        /// <param name="staffCertificateInfoAddViewModel"></param>
        /// <returns></returns>
        public StaffCertificateInfoAddViewModel DeleteStaffCertificateInfo(StaffCertificateInfoAddViewModel staffCertificateInfoAddViewModel)
        {
            try
            {
                var staffCertificateInfoDelete = this.context?.StaffCertificateInfo.FirstOrDefault(x => x.TenantId == staffCertificateInfoAddViewModel.staffCertificateInfo.TenantId && x.SchoolId == staffCertificateInfoAddViewModel.staffCertificateInfo.SchoolId && x.StaffId == staffCertificateInfoAddViewModel.staffCertificateInfo.StaffId && x.Id == staffCertificateInfoAddViewModel.staffCertificateInfo.Id);
                if (staffCertificateInfoDelete!=null)
                {
                    this.context?.StaffCertificateInfo.Remove(staffCertificateInfoDelete);
                    this.context?.SaveChanges();
                    staffCertificateInfoAddViewModel._failure = false;
                    staffCertificateInfoAddViewModel._message = "Staff Certificate Deleted Successfully";
                }
                else
                {
                    staffCertificateInfoAddViewModel.staffCertificateInfo = null;
                    staffCertificateInfoAddViewModel._failure = true;
                    staffCertificateInfoAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                staffCertificateInfoAddViewModel._failure = true;
                staffCertificateInfoAddViewModel._message = es.Message;
            }
            return staffCertificateInfoAddViewModel;
        }

        /// <summary>
        /// Add or Update Staff Photo
        /// </summary>
        /// <param name="staffAddViewModel"></param>
        /// <returns></returns>
        public StaffAddViewModel AddUpdateStaffPhoto(StaffAddViewModel staffAddViewModel)
        {
            try
            {
                var staffUpdate = this.context?.StaffMaster.FirstOrDefault(x => x.TenantId == staffAddViewModel.staffMaster.TenantId && x.StaffId == staffAddViewModel.staffMaster.StaffId);
                if(staffUpdate != null)
                {
                    staffUpdate.UpdatedOn = DateTime.UtcNow;
                    staffUpdate.StaffPhoto = staffAddViewModel.staffMaster.StaffPhoto;
                    staffUpdate.UpdatedBy = staffAddViewModel.staffMaster.UpdatedBy;
                    this.context?.SaveChanges();
                    staffAddViewModel._message = "Staff Photo Updated Successfully";
                }
                else
                {
                    staffAddViewModel._failure = true;
                    staffAddViewModel._message = NORECORDFOUND;
                }
            }
            catch(Exception es)
            {
                staffAddViewModel._failure = true;
                staffAddViewModel._message = es.Message;
            }
            return staffAddViewModel;
        }

        /// <summary>
        /// Add Staff List
        /// </summary>
        /// <param name="staffListAddViewModel"></param>
        /// <returns></returns>
        public StaffListAddViewModel AddStaffList(StaffListAddViewModel staffListAddViewModel)
        {
            StaffListAddViewModel staffListAdd = new StaffListAddViewModel();
            staffListAdd._tenantName = staffListAddViewModel._tenantName;
            staffListAdd._token = staffListAddViewModel._token;
            staffListAdd._userName = staffListAddViewModel._userName;
            int number;
            if (staffListAddViewModel.StaffAddViewModelList.Count > 0)
            {
                staffListAdd._failure = false;
                staffListAdd._message = "Staff Added Successfully";

                //int? staffId = Utility.GetMaxPK(this.context, new Func<StaffMaster, int>(x => x.StaffId));
                int? staffId = 1;

                var dataExits = this.context?.StaffMaster.Where(x => x.TenantId == staffListAddViewModel.TenantId).Count();

                if (dataExits > 0)
                {
                    var staffData = this.context?.StaffMaster.Where(x => x.TenantId == staffListAddViewModel.TenantId).Max(x => x.StaffId);

                    if (staffData != null)
                    {
                        staffId = staffData + 1;
                    }
                }             
               
                int? indexNo = -1;

                foreach (var staff in staffListAddViewModel.StaffAddViewModelList)
                {
                    indexNo++;

                     //UserMaster userMaster = new UserMaster();
                    var StaffSchoolInfoData = new StaffSchoolInfo();
                    
                    using (var transaction = this.context.Database.BeginTransaction())
                    {
                        try
                        {
                            staff.staffMaster.TenantId = staffListAddViewModel.TenantId;
                            staff.staffMaster.SchoolId = staffListAddViewModel.SchoolId;
                            staff.staffMaster.StaffId = (int)staffId;
                            staff.staffMaster.CreatedOn = DateTime.UtcNow;
                            staff.staffMaster.CreatedBy = staffListAddViewModel.CreatedBy;
                            Guid GuidId = Guid.NewGuid();
                            var GuidIdExist = this.context?.StaffMaster.FirstOrDefault(x => x.StaffGuid == GuidId);

                            if (GuidIdExist != null)
                            {
                                staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                staffListAdd.StaffAddViewModelList.Add(staff);
                                staffListAdd._failure = true;
                                staffListAdd._message = "Staff Rejected Due to Data Error";
                                continue;
                            }

                            staff.staffMaster.StaffGuid = GuidId;

                            if (!string.IsNullOrEmpty(staff.staffMaster.StaffInternalId))
                            {
                                bool checkInternalID = CheckInternalID(staff.staffMaster.TenantId, staff.staffMaster.StaffInternalId);
                                if (checkInternalID == false)
                                {
                                    staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    staffListAdd.StaffAddViewModelList.Add(staff);
                                    staffListAdd._failure = true;
                                    staffListAdd._message = "Staff Rejected Due to Data Error";
                                    continue;
                                }
                            }
                            else
                            {
                                staff.staffMaster.StaffInternalId = staff.staffMaster.StaffId.ToString();
                            }

                            if (staff.FirstLanguageName != null)
                            {
                                var checkItsId = Int32.TryParse(staff.FirstLanguageName, out number);
                                if (checkItsId)
                                {
                                    var firstLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.FirstLanguageName));
                                    staff.staffMaster.FirstLanguage = firstLanguageData != null ? Convert.ToInt32(staff.FirstLanguageName) : (int?)null;
                                }
                                else
                                {
                                    var firstLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.FirstLanguageName.ToLower())?.LangId;
                                    staff.staffMaster.FirstLanguage = firstLanguageId != null ? firstLanguageId : null;
                                }
                            }

                            if (staff.SecondLanguageName != null)
                            {
                                var checkItsId = Int32.TryParse(staff.SecondLanguageName, out number);
                                if (checkItsId)
                                {
                                    var SecondLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.SecondLanguageName));
                                    staff.staffMaster.SecondLanguage = SecondLanguageData != null ? Convert.ToInt32(staff.SecondLanguageName) : (int?)null;
                                }
                                else
                                {
                                    var secondLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.SecondLanguageName.ToLower())?.LangId;
                                    staff.staffMaster.SecondLanguage = secondLanguageId != null ? secondLanguageId : null;
                                }
                            }

                            if (staff.ThirdLanguageName != null)
                            {
                                var checkItsId = Int32.TryParse(staff.ThirdLanguageName, out number);
                                if (checkItsId)
                                {
                                    var ThirdLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(staff.ThirdLanguageName));
                                    staff.staffMaster.ThirdLanguage = ThirdLanguageData != null ? Convert.ToInt32(staff.ThirdLanguageName) : (int?)null;
                                }
                                else
                                {
                                    var thirdLanguageId = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == staff.ThirdLanguageName.ToLower())?.LangId;
                                    staff.staffMaster.ThirdLanguage = thirdLanguageId != null ? thirdLanguageId : null;
                                }
                            }

                            if (staff.CountryOfBirthName != null)
                            {
                                var checkItsId = Int32.TryParse(staff.CountryOfBirthName, out number);
                                if (checkItsId)
                                {
                                    var CountryOfBirthData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.CountryOfBirthName));
                                    staff.staffMaster.CountryOfBirth = CountryOfBirthData != null ? Convert.ToInt32(staff.CountryOfBirthName) : (int?)null;
                                }
                                else
                                {
                                    var countryOfBirthId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.CountryOfBirthName.ToLower())?.Id;
                                    staff.staffMaster.CountryOfBirth = countryOfBirthId != null ? countryOfBirthId : null;
                                }
                            }

                            if (staff.NationalityName != null)
                            {
                                var checkItsId = Int32.TryParse(staff.NationalityName, out number);
                                if (checkItsId)
                                {
                                    var NationalityData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.NationalityName));
                                    staff.staffMaster.Nationality = NationalityData != null ? Convert.ToInt32(staff.NationalityName) : (int?)null;
                                }
                                else
                                {
                                    var nationalityId = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.NationalityName.ToLower())?.Id;
                                    staff.staffMaster.Nationality = nationalityId != null ? nationalityId : null;
                                }
                            }


                            if (staff.staffMaster.HomeAddressCountry != null)
                            {
                                var checkItsId = Int32.TryParse(staff.staffMaster.HomeAddressCountry, out number);
                                if (checkItsId)
                                {
                                    var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.staffMaster.HomeAddressCountry));
                                    staff.staffMaster.HomeAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                }
                                else
                                {
                                    var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.staffMaster.HomeAddressCountry.ToLower());
                                    staff.staffMaster.HomeAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                }
                            }

                            if (staff.staffMaster.MailingAddressCountry != null)
                            {
                                var checkItsId = Int32.TryParse(staff.staffMaster.MailingAddressCountry, out number);
                                if (checkItsId)
                                {
                                    var CountryData = this.context?.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(staff.staffMaster.MailingAddressCountry));
                                    staff.staffMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                }
                                else
                                {
                                    var CountryData = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == staff.staffMaster.MailingAddressCountry.ToLower());
                                    staff.staffMaster.MailingAddressCountry = CountryData != null ? CountryData.Id.ToString() : null;
                                }
                            }


                            //Add Staff Portal Access
                            if (!string.IsNullOrWhiteSpace(staff.PasswordHash) && !string.IsNullOrWhiteSpace(staff.LoginEmail))
                            {
                                UserMaster userMaster = new UserMaster();

                                //var decrypted = Utility.Decrypt(staff.PasswordHash);
                                string passwordHash = Utility.GetHashedPassword(staff.PasswordHash);

                                var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == staff.staffMaster.TenantId && x.EmailAddress == staff.LoginEmail);

                                if (loginInfo == null)
                                {
                                    var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == staff.staffMaster.TenantId && x.SchoolId == staff.staffMaster.SchoolId && x.Profile == "Teacher");

                                    userMaster.SchoolId = staff.staffMaster.SchoolId;
                                    userMaster.TenantId = staff.staffMaster.TenantId;
                                    userMaster.UserId = staff.staffMaster.StaffId;
                                    userMaster.LangId = 1;
                                    userMaster.MembershipId = membership.MembershipId;
                                    userMaster.EmailAddress = staff.LoginEmail;
                                    userMaster.PasswordHash = passwordHash;
                                    userMaster.Name = staff.staffMaster.FirstGivenName;
                                    userMaster.IsActive = true;

                                    this.context?.UserMaster.Add(userMaster);
                                    this.context?.SaveChanges();
                                    staff.staffMaster.PortalAccess = true;
                                    staff.staffMaster.LoginEmailAddress = staff.LoginEmail;
                                }
                                else
                                {
                                    staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                    staffListAdd.StaffAddViewModelList.Add(staff);
                                    staffListAdd._failure = true;
                                    staffListAdd._message = "Staff Rejected Due to Data Error";
                                    continue;
                                }
                            }

                            if (staff.staffMaster.FirstGivenName == null || staff.staffMaster.LastFamilyName == null)
                            {
                                staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                                staffListAdd.StaffAddViewModelList.Add(staff);
                                staffListAdd._failure = true;
                                staffListAdd._message = "Staff Rejected Due to Data Error";
                                continue;
                            }

                            if (staff.Dob != null)
                            {
                                staff.staffMaster.Dob = Convert.ToDateTime(staff.Dob);
                            }
                            if (staff.JoiningDate != null)
                            {
                                staff.staffMaster.JoiningDate = Convert.ToDateTime(staff.JoiningDate);
                            }

                            this.context?.StaffMaster.Add(staff.staffMaster);
                            this.context?.SaveChanges();

                            //Insert data into StaffSchoolInfo table            
                            int? Id = Utility.GetMaxPK(this.context, new Func<StaffSchoolInfo, int>(x => (int)x.Id));
                            var schoolName = this.context?.SchoolMaster.Where(x => x.TenantId == staff.staffMaster.TenantId && x.SchoolId == staff.staffMaster.SchoolId).Select(s => s.SchoolName).FirstOrDefault();

                            var startDate = DateTime.UtcNow;
                            if (staff.StartDate != null)
                            {
                               startDate = Convert.ToDateTime(staff.StartDate);
                            }

                            StaffSchoolInfoData = new StaffSchoolInfo() { TenantId = staff.staffMaster.TenantId, SchoolId = staff.staffMaster.SchoolId, StaffId = staff.staffMaster.StaffId, SchoolAttachedId = staff.staffMaster.SchoolId, Id = (int)Id, SchoolAttachedName = schoolName, StartDate = startDate, CreatedOn = DateTime.UtcNow, CreatedBy = staff.staffMaster.CreatedBy, Profile = staff.Profile ?? "Teacher" };
                            this.context?.StaffSchoolInfo.Add(StaffSchoolInfoData);
                            this.context?.SaveChanges();

                            if (staff.fieldsCategoryList != null && staff.fieldsCategoryList.ToList().Count > 0)
                            {
                                //var fieldsCategory = staff.fieldsCategoryList.FirstOrDefault(x => x.CategoryId == staff.SelectedCategoryId);
                                //if (fieldsCategory != null)
                                //{
                                foreach (var fieldsCategory in staff.fieldsCategoryList.ToList())
                                {
                                    foreach (var customFields in fieldsCategory.CustomFields.ToList())
                                    {
                                        if (customFields.CustomFieldsValue != null && customFields.CustomFieldsValue.ToList().Count > 0)
                                        {
                                            customFields.CustomFieldsValue.FirstOrDefault().Module = "Staff";
                                            customFields.CustomFieldsValue.FirstOrDefault().CategoryId = customFields.CategoryId;
                                            customFields.CustomFieldsValue.FirstOrDefault().TenantId = staffListAddViewModel.TenantId;
                                            customFields.CustomFieldsValue.FirstOrDefault().FieldId = customFields.FieldId;
                                            customFields.CustomFieldsValue.FirstOrDefault().CustomFieldTitle = customFields.Title;
                                            customFields.CustomFieldsValue.FirstOrDefault().CustomFieldType = customFields.Type;
                                            customFields.CustomFieldsValue.FirstOrDefault().SchoolId = staff.staffMaster.SchoolId;
                                            customFields.CustomFieldsValue.FirstOrDefault().TargetId = staff.staffMaster.StaffId;
                                            this.context?.CustomFieldsValue.AddRange(customFields.CustomFieldsValue);
                                            this.context?.SaveChanges();
                                        }
                                    }
                                }
                                //}
                            }
                            transaction.Commit();
                            staffId++;
                        }
                        catch (Exception es)
                        {
                            transaction.Rollback();
                            staffListAdd.StaffAddViewModelList.Add(staff);
                            staffListAdd.ConflictIndexNo = staffListAdd.ConflictIndexNo != null ? staffListAdd.ConflictIndexNo + "," + indexNo.ToString() : indexNo.ToString();
                            this.context?.StaffMaster.Remove(staff.staffMaster);
                            this.context?.StaffSchoolInfo.Remove(StaffSchoolInfoData);
                            //this.context?.UserMaster.Remove(userMaster);
                            staffListAdd._failure = true;
                            staffListAdd._message = "Staff Rejected Due to Data Error";
                            
                            //staffId--;
                            continue;
                        }
                    }
                }
            }
            else
            {
                staffListAdd._failure = true;
                staffListAdd._message = "Please Import Staff";
            }
            return staffListAdd;
        }

        /// <summary>
        /// Get Scheduled Course Sections For Staff
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetScheduledCourseSectionsForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;

                var scheduledCourseSectionDataList = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).ThenInclude(y => y.SchoolYears).ThenInclude(s => s.Semesters).ThenInclude(e => e.Quarters).Include(n=>n.CourseSection.SchoolCalendars).Include(b=>b.CourseSection.Course).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionDataList.Count>0)
                {
                    foreach (var scheduledCourseSectionData in scheduledCourseSectionDataList)
                    {
                        CourseSectionViewList CourseSections = new CourseSectionViewList();

                        if (scheduledCourseSectionData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                        {
                            CourseSections.ScheduleType = "Fixed Schedule";

                            var courseFixedScheduleData = this.context?.CourseFixedSchedule.Include(c => c.BlockPeriod).Include(b=>b.Rooms).FirstOrDefault(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId);
                            if (courseFixedScheduleData != null)
                            {
                                courseFixedScheduleData.BlockPeriod.CourseFixedSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseVariableSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseCalendarSchedule = null;
                                courseFixedScheduleData.BlockPeriod.CourseBlockSchedule = null;
                                courseFixedScheduleData.Rooms.CourseFixedSchedule = null;
                                courseFixedScheduleData.Rooms.CourseVariableSchedule = null;
                                courseFixedScheduleData.Rooms.CourseCalendarSchedule = null;
                                courseFixedScheduleData.Rooms.CourseBlockSchedule = null;
                                CourseSections.courseFixedSchedule = courseFixedScheduleData;

                            }
                        }
                        if (scheduledCourseSectionData.CourseSection.ScheduleType == "Variable Schedule (2)")
                        {
                            CourseSections.ScheduleType = "Variable Schedule";

                            var courseVariableScheduleData = this.context?.CourseVariableSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                            if (courseVariableScheduleData.Count > 0)
                            {
                                courseVariableScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                CourseSections.courseVariableSchedule = courseVariableScheduleData;
                            }
                        }
                        if (scheduledCourseSectionData.CourseSection.ScheduleType == "Calendar Schedule (3)")
                        {
                            CourseSections.ScheduleType = "Calendar Schedule";

                            var courseCalenderScheduleData = this.context?.CourseCalendarSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                            if (courseCalenderScheduleData.Count > 0)
                            {
                                courseCalenderScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null;x.Rooms.CourseFixedSchedule=null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                CourseSections.courseCalendarSchedule = courseCalenderScheduleData;
                            }
                        }
                        if (scheduledCourseSectionData.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            CourseSections.ScheduleType = "Block Schedule";

                            var courseBlockScheduleData = this.context?.CourseBlockSchedule.Include(c => c.BlockPeriod).Include(b => b.Rooms).Where(x => x.TenantId == scheduledCourseSectionData.TenantId && x.SchoolId == scheduledCourseSectionData.SchoolId && x.CourseSectionId == scheduledCourseSectionData.CourseSectionId).ToList();

                            if (courseBlockScheduleData.Count > 0)
                            {
                                courseBlockScheduleData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                CourseSections.courseBlockSchedule = courseBlockScheduleData;
                            }
                        }

                        CourseSections.CourseId = scheduledCourseSectionData.CourseId;
                        CourseSections.CourseSectionId = scheduledCourseSectionData.CourseSectionId;
                        CourseSections.CalendarId = scheduledCourseSectionData.CourseSection.CalendarId;
                        CourseSections.CourseSectionName = scheduledCourseSectionData.CourseSectionName;
                        CourseSections.CourseTitle = scheduledCourseSectionData.CourseSection.Course.CourseTitle;
                        CourseSections.CourseGradeLevel = scheduledCourseSectionData.CourseSection.Course.CourseGradeLevel;
                        CourseSections.YrMarkingPeriodId = scheduledCourseSectionData.YrMarkingPeriodId;
                        CourseSections.SmstrMarkingPeriodId = scheduledCourseSectionData.SmstrMarkingPeriodId;
                        CourseSections.QtrMarkingPeriodId = scheduledCourseSectionData.QtrMarkingPeriodId;
                        CourseSections.DurationStartDate = scheduledCourseSectionData.DurationStartDate;
                        CourseSections.DurationEndDate = scheduledCourseSectionData.DurationEndDate;
                        CourseSections.MeetingDays = scheduledCourseSectionData.MeetingDays;
                        CourseSections.AttendanceCategoryId = scheduledCourseSectionData.CourseSection.AttendanceCategoryId;
                        CourseSections.AttendanceTaken = scheduledCourseSectionData.CourseSection.AttendanceTaken;
                        CourseSections.WeekDays = scheduledCourseSectionData.CourseSection.SchoolCalendars.Days;
                        CourseSections.SchoolYears = scheduledCourseSectionData.CourseSection.SchoolYears;
                        CourseSections.Quarters = scheduledCourseSectionData.CourseSection.Quarters;
                        CourseSections.Semesters = scheduledCourseSectionData.CourseSection.Semesters;
                        CourseSections.DurationBasedOnPeriod = scheduledCourseSectionData.CourseSection.DurationBasedOnPeriod;

                        if (CourseSections!=null)
                        {
                            if (CourseSections.SchoolYears != null)
                            {
                                CourseSections.SchoolYears.CourseSection = null;
                                CourseSections.SchoolYears.HonorRolls = null;
                                CourseSections.SchoolYears.Semesters = null;
                                CourseSections.SchoolYears.StaffCoursesectionSchedule = null;
                                CourseSections.SchoolYears.StudentEffortGradeMaster = null;
                                CourseSections.SchoolYears.StudentFinalGrade = null;
                            }
                            if (CourseSections.Quarters != null)
                            {
                                CourseSections.Quarters.CourseSection = null;
                                CourseSections.Quarters.Semesters = null;
                                CourseSections.Quarters.StaffCoursesectionSchedule = null;
                                CourseSections.Quarters.StudentEffortGradeMaster = null;
                                CourseSections.Quarters.StudentFinalGrade = null;
                                CourseSections.Quarters.ProgressPeriods = null;
                            }
                            if (CourseSections.Semesters != null)
                            {
                                CourseSections.Semesters.CourseSection = null;
                                CourseSections.Semesters.Quarters = null;
                                CourseSections.Semesters.StaffCoursesectionSchedule = null;
                                CourseSections.Semesters.StudentEffortGradeMaster = null;
                                CourseSections.Semesters.StudentFinalGrade = null;
                                CourseSections.Semesters.SchoolYears = null;
                            }
                        }
                        scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
                        scheduledCourseSectionView._failure = false;
                    }
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }
    }
}
