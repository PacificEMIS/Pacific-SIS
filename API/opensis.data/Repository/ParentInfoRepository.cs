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
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace opensis.data.Repository
{
    public class ParentInfoRepository : IParentInfoRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public ParentInfoRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Parent For Student
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel AddParentForStudent(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                if (parentInfoAddViewModel.parentAssociationship is null)
                {
                    return parentInfoAddViewModel;
                }
                try
                {
                    if (parentInfoAddViewModel.parentInfo != null && parentInfoAddViewModel.parentInfo.ParentId > 0)
                    {
                        var parentInfo = this.context?.ParentInfo.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.ParentId == parentInfoAddViewModel.parentInfo.ParentId);
                        if (parentInfo != null)
                        {
                            var AssociationshipData = this.context?.ParentAssociationship.FirstOrDefault(x => x.TenantId == parentInfo.TenantId && x.SchoolId == parentInfoAddViewModel.parentAssociationship.SchoolId && x.ParentId == parentInfo.ParentId && x.StudentId == parentInfoAddViewModel.parentAssociationship.StudentId);
                            if (AssociationshipData != null)
                            {
                                if (AssociationshipData.Associationship)
                                {
                                    parentInfoAddViewModel._message = "Parent Already Exists For This Student";
                                    parentInfoAddViewModel._failure = true;
                                    return parentInfoAddViewModel;
                                }
                                else
                                {
                                    AssociationshipData.Associationship = true;
                                }
                            }
                            else
                            {
                                var parentAssociationship = new ParentAssociationship { TenantId = parentInfoAddViewModel.parentInfo.TenantId, SchoolId = parentInfoAddViewModel.parentAssociationship.SchoolId, ParentId = parentInfoAddViewModel.parentInfo.ParentId, StudentId = parentInfoAddViewModel.parentAssociationship.StudentId, Associationship = true, UpdatedOn = DateTime.UtcNow, UpdatedBy = parentInfoAddViewModel.parentInfo.CreatedBy, IsCustodian = parentInfoAddViewModel.parentAssociationship.IsCustodian, Relationship = parentInfoAddViewModel.parentAssociationship.Relationship, ContactType = parentInfoAddViewModel.parentAssociationship.ContactType };
                                this.context?.ParentAssociationship.Add(parentAssociationship);
                            }
                        }
                    }
                    else
                    {
                        var checkDuplicateParent = Utility.checkDuplicate(this.context, parentInfoAddViewModel.parentInfo!.TenantId, null, parentInfoAddViewModel.parentInfo.Salutation, parentInfoAddViewModel.parentInfo.Firstname, parentInfoAddViewModel.parentInfo.Middlename, parentInfoAddViewModel.parentInfo.Lastname, parentInfoAddViewModel.parentInfo.Suffix, null, parentInfoAddViewModel.parentInfo.PersonalEmail, parentInfoAddViewModel.parentInfo.Mobile, "parent", null);

                        if (checkDuplicateParent != null)
                        {
                            parentInfoAddViewModel._message = "Duplicate Entry Found";
                            parentInfoAddViewModel._failure = true;
                            return parentInfoAddViewModel;
                        }

                        //int? ParentId = Utility.GetMaxPK(this.context, new Func<ParentInfo, int>(x => x.ParentId));
                        int? ParentId = 1;
                        var dataExits = this.context?.ParentInfo.Where(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId).Count();

                        if (dataExits > 0)
                        {
                            var parentData = this.context?.ParentInfo.Where(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId).Max(x => x.ParentId);
                            if (parentData != null)
                            {
                                ParentId = parentData + 1;
                            }
                        }
                        parentInfoAddViewModel.parentInfo.ParentId = (int)ParentId;
                        parentInfoAddViewModel.parentInfo.CreatedOn = DateTime.UtcNow;
                        Guid GuidId = Guid.NewGuid();
                        var GuidIdExist = this.context?.ParentInfo.FirstOrDefault(x => x.ParentGuid == GuidId);
                        if (GuidIdExist != null)
                        {
                            parentInfoAddViewModel._failure = true;
                            parentInfoAddViewModel._message = "Guid is already exist, Please try again.";
                            return parentInfoAddViewModel;
                        }
                        parentInfoAddViewModel.parentInfo.ParentGuid = GuidId;

                        //Add Parent Portal Access
                        if (!string.IsNullOrWhiteSpace(parentInfoAddViewModel.PasswordHash) && !string.IsNullOrWhiteSpace(parentInfoAddViewModel.parentInfo.LoginEmail))
                        {
                            UserMaster userMaster = new();

                            var decrypted = Utility.Decrypt(parentInfoAddViewModel.PasswordHash);
                            string passwordHash = Utility.GetHashedPassword(decrypted);

                            var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.EmailAddress == parentInfoAddViewModel.parentInfo.LoginEmail);

                            if (loginInfo == null)
                            {
                                var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.SchoolId == parentInfoAddViewModel.parentInfo.SchoolId && x.Profile == "Parent");

                                userMaster.SchoolId = parentInfoAddViewModel.parentInfo.SchoolId;
                                userMaster.TenantId = parentInfoAddViewModel.parentInfo.TenantId;
                                userMaster.UserId = parentInfoAddViewModel.parentInfo.ParentId;
                                userMaster.LangId = 1;
                                userMaster.MembershipId = membership!.MembershipId;
                                userMaster.EmailAddress = parentInfoAddViewModel.parentInfo.LoginEmail;
                                userMaster.PasswordHash = passwordHash;
                                userMaster.Name = parentInfoAddViewModel.parentInfo.Firstname!;
                                userMaster.IsActive = parentInfoAddViewModel.parentInfo.IsPortalUser;

                                parentInfoAddViewModel.parentInfo.LoginEmail = parentInfoAddViewModel.parentInfo.LoginEmail;

                                this.context?.UserMaster.Add(userMaster);
                                this.context?.SaveChanges();
                            }
                            else
                            {
                                parentInfoAddViewModel.parentInfo = null;
                                parentInfoAddViewModel._failure = true;
                                parentInfoAddViewModel._message = "Parent Login Email Already Exist";
                                return parentInfoAddViewModel;
                            }
                        }


                        if (parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentAddressSame == true)
                        {
                            var studentAddress = this.context?.StudentMaster.Where(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.SchoolId == parentInfoAddViewModel.parentInfo.SchoolId && x.StudentId == parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentId).Select(x => new StudentMaster { HomeAddressLineOne = x.HomeAddressLineOne, HomeAddressLineTwo = x.HomeAddressLineTwo, HomeAddressCountry = x.HomeAddressCountry, HomeAddressCity = x.HomeAddressCity, HomeAddressState = x.HomeAddressState, HomeAddressZip = x.HomeAddressZip }).FirstOrDefault();

                            if (studentAddress != null)
                            {
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineOne = studentAddress.HomeAddressLineOne;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineTwo = studentAddress.HomeAddressLineTwo;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Country = studentAddress.HomeAddressCountry;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.City = studentAddress.HomeAddressCity;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.State = studentAddress.HomeAddressState;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Zip = studentAddress.HomeAddressZip;
                            }
                        }

                        this.context?.ParentInfo.Add(parentInfoAddViewModel.parentInfo);

                        var parentAssociationship = new ParentAssociationship { TenantId = parentInfoAddViewModel.parentInfo.TenantId, SchoolId = parentInfoAddViewModel.parentAssociationship.SchoolId, ParentId = parentInfoAddViewModel.parentInfo.ParentId, StudentId = parentInfoAddViewModel.parentAssociationship.StudentId, Associationship = true, CreatedOn = DateTime.UtcNow, CreatedBy = parentInfoAddViewModel.parentInfo.CreatedBy, IsCustodian = parentInfoAddViewModel.parentAssociationship.IsCustodian, Relationship = parentInfoAddViewModel.parentAssociationship.Relationship, ContactType = parentInfoAddViewModel.parentAssociationship.ContactType };

                        this.context?.ParentAssociationship.Add(parentAssociationship);
                    }
                    this.context?.SaveChanges();
                    transaction?.Commit();
                    parentInfoAddViewModel._failure = false;
                    parentInfoAddViewModel._message = "Parent Added Successfully";
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    parentInfoAddViewModel._message = es.Message;
                    parentInfoAddViewModel._failure = true;
                    parentInfoAddViewModel._tenantName = parentInfoAddViewModel._tenantName;
                    parentInfoAddViewModel._token = parentInfoAddViewModel._token;
                }
            }
            return parentInfoAddViewModel;
        }

        /// <summary>
        /// Get Parent List For Student
        /// </summary>
        /// <param name="parentInfoList"></param>
        /// <returns></returns>
        public ParentInfoListModel ViewParentListForStudent(ParentInfoListModel parentInfoList)
        {
            ParentInfoListModel parentInfoListViewModel = new();
            try
            {
                var parentAssociationship = this.context?.ParentAssociationship.Where(x => x.TenantId == parentInfoList.TenantId && x.SchoolId == parentInfoList.SchoolId && x.StudentId == parentInfoList.StudentId && x.Associationship == true).ToList();
                if (parentAssociationship?.Any() == true)
                {
                    int[] ParentId = parentAssociationship.Select(p => p.ParentId).ToArray();
                    var parentDataList = this.context?.ParentInfo.Include(x => x.ParentAddress).Where(x => x.TenantId == parentInfoList.TenantId && ParentId.Contains(x.ParentId));
                    var studentAddress = this.context?.StudentMaster.Where(x => x.TenantId == parentInfoList.TenantId && x.SchoolId == parentInfoList.SchoolId && x.StudentId == parentInfoList.StudentId).Select(x => new StudentMaster { HomeAddressLineOne = x.HomeAddressLineOne, HomeAddressLineTwo = x.HomeAddressLineTwo, HomeAddressCountry = x.HomeAddressCountry, HomeAddressCity = x.HomeAddressCity, HomeAddressState = x.HomeAddressState, HomeAddressZip = x.HomeAddressZip }).FirstOrDefault();
                    if (parentDataList != null)
                    {
                        foreach (var parent in parentAssociationship)
                        {
                            /* var parentData = this.context?.ParentInfo.Include(x => x.ParentAddress).Where(x => x.TenantId == parentInfoList.TenantId && x.ParentId == parent.ParentId).FirstOrDefault();*/
                            var parentData = parentDataList.Where(x => x.ParentId == parent.ParentId).FirstOrDefault();
                            if (parentData != null)
                            {
                                var parentAddress = parentData.ParentAddress.FirstOrDefault();
                                if (parentAddress!.StudentAddressSame == true)
                                {
                                    //var studentAddress = this.context?.StudentMaster.Where(x => x.TenantId == parentInfoList.TenantId && x.SchoolId == parentInfoList.SchoolId && x.StudentId == parentInfoList.StudentId).FirstOrDefault();

                                    if (studentAddress != null)
                                    {
                                        parentAddress!.AddressLineOne = studentAddress!.HomeAddressLineOne;
                                        parentAddress!.AddressLineTwo = studentAddress.HomeAddressLineTwo;
                                        parentAddress!.Country = parentInfoList.IsReport == true ? studentAddress.HomeAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(studentAddress.HomeAddressCountry))!.Name : null : studentAddress.HomeAddressCountry;
                                        parentAddress!.State = studentAddress.HomeAddressState;
                                        parentAddress!.City = studentAddress.HomeAddressCity;
                                        parentAddress!.Zip = studentAddress.HomeAddressZip;
                                        parentAddress!.ParentInfo = new();
                                    }
                                }

                                var parentInfoListData = new ParentInfoListForView
                                {
                                    TenantId = parentData.TenantId,
                                    SchoolId = parentData.SchoolId,
                                    ParentId = parentData.ParentId,
                                    Firstname = parentData.Firstname,
                                    Lastname = parentData.Lastname,
                                    HomePhone = parentData.HomePhone,
                                    WorkPhone = parentData.WorkPhone,
                                    Mobile = parentData.Mobile,
                                    IsPortalUser = parentData.IsPortalUser,
                                    BusPickup = parentData.BusPickup,
                                    BusDropoff = parentData.BusDropoff,
                                    ContactType = parent.ContactType,
                                    CreatedBy = parentData.CreatedBy,
                                    CreatedOn = parent.CreatedOn,
                                    UpdatedOn = parentData.UpdatedOn,
                                    UpdatedBy = parentData.UpdatedBy,
                                    BusNo = parentData.BusNo,
                                    Middlename = parentData.Middlename,
                                    PersonalEmail = parentData.PersonalEmail,
                                    Salutation = parentData.Salutation,
                                    Suffix = parentData.Suffix,
                                    UserProfile = parentData.UserProfile,
                                    WorkEmail = parentData.WorkEmail,
                                    //ParentPhoto = parentData.ParentPhoto,
                                    ParentPhoto = parentData.ParentThumbnailPhoto,
                                    IsCustodian = parent.IsCustodian,
                                    Relationship = parent.Relationship,
                                    ParentAddress = parentInfoList.IsReport == true && parentAddress!.StudentAddressSame == false ? new ParentAddress
                                    {
                                        AddressLineOne = parentAddress?.AddressLineOne,
                                        AddressLineTwo = parentAddress?.AddressLineTwo,
                                        Country = this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(parentAddress!.Country))!.Name,
                                        State = parentAddress?.State,
                                        City = parentAddress?.City,
                                        Zip = parentAddress?.Zip,
                                        ParentInfo = new()
                                    } : parentAddress,
                                    LoginEmail = parentData.LoginEmail
                                };
                                parentInfoListViewModel.parentInfoListForView.Add(parentInfoListData);
                            }
                            if (!parentInfoList.IsShowParentPhoto)
                            {
                                parentInfoListViewModel.parentInfoListForView.ForEach(x => x.ParentPhoto = null);
                            }
                        }
                    }


                }
                else
                {
                    return parentInfoListViewModel;
                }
            }
            catch (Exception es)
            {
                parentInfoListViewModel._failure = true;
                parentInfoListViewModel._message = es.Message;
            }
            return parentInfoListViewModel;
        }

        /// <summary>
        /// Update Parent Info
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel UpdateParentInfo(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            using (var transaction = this.context?.Database.BeginTransaction())
            {
                try
                {
                    var parentInfoUpdate = this.context?.ParentInfo.Include(x => x.ParentAddress).FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo!.TenantId && x.ParentId == parentInfoAddViewModel.parentInfo.ParentId);

                    var checkDuplicateParent = Utility.checkDuplicate(this.context, parentInfoAddViewModel.parentInfo!.TenantId, null, parentInfoAddViewModel.parentInfo.Salutation, parentInfoAddViewModel.parentInfo.Firstname, parentInfoAddViewModel.parentInfo.Middlename, parentInfoAddViewModel.parentInfo.Lastname, parentInfoAddViewModel.parentInfo.Suffix, null, parentInfoAddViewModel.parentInfo.PersonalEmail, parentInfoAddViewModel.parentInfo.Mobile, "parent", parentInfoUpdate!.ParentGuid);

                    if (checkDuplicateParent != null)
                    {
                        parentInfoAddViewModel._failure = true;
                        parentInfoAddViewModel._message = "Duplicate Entry Found";
                        return parentInfoAddViewModel;
                    }
                    else
                    {
                        //Add or Update parent portal access
                        if (parentInfoUpdate.LoginEmail != null)
                        {
                            if (!string.IsNullOrWhiteSpace(parentInfoAddViewModel.parentInfo.LoginEmail))
                            {
                                if (parentInfoUpdate.LoginEmail != parentInfoAddViewModel.parentInfo.LoginEmail)
                                {
                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.EmailAddress == parentInfoAddViewModel.parentInfo.LoginEmail);

                                    if (loginInfo != null)
                                    {
                                        parentInfoAddViewModel.parentInfo = null;
                                        parentInfoAddViewModel._failure = true;
                                        parentInfoAddViewModel._message = "Parent Login Email Already Exist";
                                        return parentInfoAddViewModel;
                                    }
                                    else
                                    {
                                        var loginInfoData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.EmailAddress == parentInfoUpdate.LoginEmail);

                                        if (loginInfoData != null)
                                        {
                                            loginInfoData.EmailAddress = parentInfoAddViewModel.parentInfo.LoginEmail;
                                            loginInfoData.IsActive = parentInfoAddViewModel.parentInfo.IsPortalUser;

                                            this.context?.UserMaster.Add(loginInfoData);
                                            this.context?.SaveChanges();
                                        }
                                        //Update Parent Login in ParentInfo table.
                                        //parentInfoUpdate.LoginEmail = parentInfoAddViewModel.parentInfo.LoginEmail;
                                    }
                                }
                                else
                                {
                                    var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.EmailAddress == parentInfoUpdate.LoginEmail);

                                    if (loginInfo != null)
                                    {
                                        loginInfo.IsActive = parentInfoAddViewModel.parentInfo.IsPortalUser;

                                        this.context?.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(parentInfoAddViewModel.parentInfo.LoginEmail) && !string.IsNullOrWhiteSpace(parentInfoAddViewModel.PasswordHash))
                            {
                                var decrypted = Utility.Decrypt(parentInfoAddViewModel.PasswordHash);
                                string passwordHash = Utility.GetHashedPassword(decrypted);

                                UserMaster userMaster = new();

                                var loginInfo = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.EmailAddress == parentInfoAddViewModel.parentInfo.LoginEmail);

                                if (loginInfo == null)
                                {
                                    var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.SchoolId == parentInfoAddViewModel.parentInfo.SchoolId && x.Profile == "Parent");

                                    userMaster.SchoolId = parentInfoAddViewModel.parentInfo.SchoolId;
                                    userMaster.TenantId = parentInfoAddViewModel.parentInfo.TenantId;
                                    userMaster.UserId = parentInfoAddViewModel.parentInfo.ParentId;
                                    userMaster.LangId = 1;
                                    userMaster.MembershipId = membership!.MembershipId;
                                    userMaster.EmailAddress = parentInfoAddViewModel.parentInfo.LoginEmail;
                                    userMaster.PasswordHash = passwordHash;
                                    userMaster.Name = parentInfoAddViewModel.parentInfo.Firstname ?? "";
                                    userMaster.UpdatedOn = DateTime.UtcNow;
                                    userMaster.IsActive = parentInfoAddViewModel.parentInfo.IsPortalUser;

                                    this.context?.UserMaster.Add(userMaster);
                                    this.context?.SaveChanges();

                                    //Update LoginEmail in ParentInfo table.
                                    //parentInfoUpdate.LoginEmail = parentInfoAddViewModel.parentInfo.LoginEmail;
                                }
                                else
                                {
                                    parentInfoAddViewModel.parentInfo = null;
                                    parentInfoAddViewModel._failure = true;
                                    parentInfoAddViewModel._message = "Parent Login Email Already Exist";
                                    return parentInfoAddViewModel;
                                }
                            }
                        }

                        //parentInfoAddViewModel.parentInfo.UserProfile = parentInfoUpdate.UserProfile;
                        parentInfoAddViewModel.parentInfo.ParentGuid = parentInfoUpdate.ParentGuid;
                        parentInfoAddViewModel.parentInfo.UpdatedOn = DateTime.UtcNow;
                        parentInfoAddViewModel.parentInfo.CreatedBy = parentInfoUpdate.CreatedBy;
                        parentInfoAddViewModel.parentInfo.CreatedOn = parentInfoUpdate.CreatedOn;
                        this.context?.Entry(parentInfoUpdate).CurrentValues.SetValues(parentInfoAddViewModel.parentInfo);
                        this.context?.SaveChanges();

                        if (parentInfoUpdate.ParentAddress.Count > 0 && parentInfoAddViewModel.parentInfo.ParentAddress.Count > 0)
                        {
                            var parentAddress = parentInfoUpdate.ParentAddress.FirstOrDefault()!;
                            if (parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentAddressSame == true)
                            {
                                //var studentAddress = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == parentInfoUpdate.ParentAddress.FirstOrDefault()!.StudentId && x.SchoolId == parentInfoUpdate.SchoolId);
                                var studentAddress = this.context?.StudentMaster.Where(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.SchoolId == parentInfoAddViewModel.parentInfo.SchoolId && x.StudentId == parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentId).Select(x => new StudentMaster { HomeAddressLineOne = x.HomeAddressLineOne, HomeAddressLineTwo = x.HomeAddressLineTwo, HomeAddressCountry = x.HomeAddressCountry, HomeAddressCity = x.HomeAddressCity, HomeAddressState = x.HomeAddressState, HomeAddressZip = x.HomeAddressZip }).FirstOrDefault();

                                if (studentAddress != null)
                                {
                                    studentAddress.HomeAddressLineOne = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineOne;
                                    studentAddress.HomeAddressLineTwo = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineTwo;
                                    studentAddress.HomeAddressCountry = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Country;
                                    studentAddress.HomeAddressState = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.State;
                                    studentAddress.HomeAddressCity = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.City;
                                    studentAddress.HomeAddressZip = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Zip;

                                    if (studentAddress.MailingAddressSameToHome == true)
                                    {
                                        studentAddress.MailingAddressLineOne = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineOne;
                                        studentAddress.MailingAddressLineTwo = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineTwo;
                                        studentAddress.MailingAddressCountry = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Country;
                                        studentAddress.MailingAddressState = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.State;
                                        studentAddress.MailingAddressCity = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.City;
                                        studentAddress.MailingAddressZip = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Zip;
                                    }
                                }

                                parentAddress.StudentAddressSame = true;
                                parentAddress.AddressLineOne = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineOne;
                                parentAddress.AddressLineTwo = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.AddressLineTwo;
                                parentAddress.Country = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Country;
                                parentAddress.City = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.City;
                                parentAddress.State = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.State;
                                parentAddress.Zip = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.Zip;
                                parentAddress.UpdatedBy = parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.UpdatedBy;
                                parentAddress.UpdatedOn = DateTime.UtcNow;
                                this.context?.SaveChanges();
                            }
                            else
                            {
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentId = parentAddress.StudentId;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.StudentAddressSame = false;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.UpdatedOn = DateTime.UtcNow;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.CreatedOn = parentAddress.CreatedOn;
                                parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!.CreatedBy = parentAddress.CreatedBy;
                                this.context?.Entry(parentAddress).CurrentValues.SetValues(parentInfoAddViewModel.parentInfo.ParentAddress.FirstOrDefault()!);
                                this.context?.SaveChanges();
                            }
                        }

                        if (parentInfoAddViewModel.parentAssociationship != null)
                        {
                            var associationshipData = this.context?.ParentAssociationship.Where(x => x.TenantId == parentInfoAddViewModel.parentAssociationship.TenantId && x.ParentId == parentInfoAddViewModel.parentAssociationship.ParentId && x.StudentId == parentInfoAddViewModel.parentAssociationship.StudentId).Select(x => x.IsCustodian).FirstOrDefault();
                            if (associationshipData != null)
                            {
                                associationshipData = parentInfoAddViewModel.parentAssociationship.IsCustodian;
                            }
                        }

                        this.context?.SaveChanges();
                        parentInfoAddViewModel._message = "Parent Updated Successfully";
                        parentInfoAddViewModel._failure = false;
                        transaction?.Commit();
                    }
                }
                catch (Exception es)
                {
                    transaction?.Rollback();
                    parentInfoAddViewModel._failure = true;
                    parentInfoAddViewModel._message = es.Message;
                }
            }
            return parentInfoAddViewModel;
        }

        //Get Full Address.
        private static string ToFullAddress(string? Address1, string? Address2, string? City, string? State, string? Country, string? Zip)
        {
            string? address = "";
            if (!string.IsNullOrWhiteSpace(Address1))
            {


                return address == null
                      ? ""
                      : $"{Address1?.Trim()}{(!string.IsNullOrWhiteSpace(Address2) ? $", {Address2?.Trim()}" : string.Empty)}, {City?.Trim()}, {State?.Trim()} {Zip?.Trim()}";
            }

            return address;
        }

        /// <summary>
        /// Get All Parent Info List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        //public GetAllParentInfoListForView GetAllParentInfoList_Old(PageResult pageResult)
        //{
        //    GetAllParentInfoListForView parentInfoListModel = new GetAllParentInfoListForView();
        //    IQueryable<ParentListView> transactionIQ = null;

        //    var ParentInfoList = this.context?.ParentListView.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.Associationship == true);

        //    try
        //    {
        //        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
        //        {
        //            transactionIQ = ParentInfoList;

        //            transactionIQ = transactionIQ.Select(y => new ParentListView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                CreatedBy = y.CreatedBy,
        //                CreatedOn = y.CreatedOn,
        //                UpdatedBy = y.UpdatedBy,
        //                UpdatedOn = y.UpdatedOn                        
        //            }).Distinct();
        //        }
        //        else
        //        {
        //            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
        //            {
        //                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
        //                Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");
        //                transactionIQ = ParentInfoList.Where(x => x.Firstname.ToLower().Contains(Columnvalue.ToLower()) || x.Lastname.ToLower().Contains(Columnvalue.ToLower()) || x.Middlename.ToLower().Contains(Columnvalue.ToLower()) || (x.Firstname.ToLower() + x.Middlename.ToLower() + x.Lastname.ToLower()).Contains(Columnvalue.ToLower()) || (x.Firstname.ToLower() + x.Middlename.ToLower()).Contains(Columnvalue.ToLower()) || (x.Firstname.ToLower() + x.Lastname.ToLower()).Contains(Columnvalue.ToLower()) || (x.Middlename.ToLower() + x.Lastname.ToLower()).Contains(Columnvalue.ToLower()) || x.WorkEmail.ToLower().Contains(Columnvalue.ToLower()) || x.Mobile.ToLower().Contains(Columnvalue.ToLower()) || x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StudentMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.UserProfile.ToLower().Contains(Columnvalue.ToLower()));

        //                transactionIQ = transactionIQ.Select(y => new ParentListView
        //                {
        //                    SchoolId = y.SchoolId,
        //                    TenantId = y.TenantId,
        //                    ParentId = y.ParentId,
        //                    Firstname = y.Firstname,
        //                    Middlename = y.Middlename,
        //                    Lastname = y.Lastname,
        //                    Salutation = y.Salutation,
        //                    Suffix = y.Suffix,
        //                    WorkEmail = y.WorkEmail,
        //                    WorkPhone = y.WorkPhone,
        //                    HomePhone = y.HomePhone,
        //                    PersonalEmail = y.PersonalEmail,
        //                    Mobile = y.Mobile,
        //                    UserProfile = y.UserProfile,
        //                    CreatedBy =  y.CreatedBy,
        //                    CreatedOn = y.CreatedOn,
        //                    UpdatedBy = y.UpdatedBy,
        //                    UpdatedOn = y.UpdatedOn
        //                }).Distinct();
        //            }
        //            else
        //            {
        //                transactionIQ = Utility.FilteredData(pageResult.FilterParams, ParentInfoList).AsQueryable();

        //                transactionIQ = transactionIQ.AsNoTracking().Select(y => new ParentListView
        //                {
        //                    SchoolId = y.SchoolId,
        //                    TenantId = y.TenantId,
        //                    ParentId = y.ParentId,
        //                    Firstname = y.Firstname,
        //                    Middlename = y.Middlename,
        //                    Lastname = y.Lastname,
        //                    Salutation = y.Salutation,
        //                    Suffix = y.Suffix,
        //                    WorkEmail = y.WorkEmail,
        //                    WorkPhone = y.WorkPhone,
        //                    HomePhone = y.HomePhone,
        //                    PersonalEmail = y.PersonalEmail,
        //                    Mobile = y.Mobile,
        //                    UserProfile = y.UserProfile,
        //                    CreatedBy =  y.CreatedBy,
        //                    CreatedOn = y.CreatedOn,
        //                    UpdatedBy = y.UpdatedBy,
        //                    UpdatedOn = y.UpdatedOn
        //                }).GroupBy(x => x.ParentId).Select(g => g.First());
        //            }
        //        }

        //        if (pageResult.SortingModel != null)
        //        {
        //            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
        //        }

        //        int totalCount = transactionIQ.Count();
        //        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
        //        {
        //            var parentInfo = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                CreatedBy = y.CreatedBy,
        //                CreatedOn = y.CreatedOn,
        //                UpdatedBy = y.UpdatedBy,
        //                UpdatedOn = y.UpdatedOn
        //            }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);

        //            parentInfoListModel.parentInfoForView = parentInfo.ToList();
        //        }
        //        else
        //        {
        //            parentInfoListModel.parentInfoForView = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                CreatedBy = y.CreatedBy,
        //                CreatedOn = y.CreatedOn,
        //                UpdatedBy = y.UpdatedBy,
        //                UpdatedOn = y.UpdatedOn
        //            }).ToList();
        //        }

        //        parentInfoListModel.parentInfoForView.ForEach(e =>
        //        {
        //            e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.CreatedBy);
        //            e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.UpdatedBy);
        //        });


        //        foreach (var ParentInfo in parentInfoListModel.parentInfoForView)
        //        {
        //            var studentAssociateWithParents = ParentInfoList.Where(x => x.ParentId == ParentInfo.ParentId && x.Associationship == true).ToList();
        //            List<string> studentArray = new List<string>();
        //            if (studentAssociateWithParents.Count() > 0)
        //            {
        //                foreach (var studentAssociateWithParent in studentAssociateWithParents)
        //                {
        //                    studentArray.Add(studentAssociateWithParent.FirstGivenName + "|" + studentAssociateWithParent.StudentMiddleName + "|" + studentAssociateWithParent.LastFamilyName + "|" + studentAssociateWithParent.StudentId);
        //                    ParentInfo.students = studentArray.ToArray();
        //                }
        //            }
        //        }

        //        parentInfoListModel.TenantId = pageResult.TenantId;
        //        parentInfoListModel.TotalCount = totalCount;
        //        parentInfoListModel.PageNumber = pageResult.PageNumber;
        //        parentInfoListModel._pageSize = pageResult.PageSize;
        //        parentInfoListModel._tenantName = pageResult._tenantName;
        //        parentInfoListModel._token = pageResult._token;
        //        parentInfoListModel._failure = false;
        //    }
        //    catch (Exception es)
        //    {
        //        parentInfoListModel.parentInfoForView = null;
        //        parentInfoListModel._message = es.Message;
        //        parentInfoListModel._failure = true;
        //        parentInfoListModel._tenantName = pageResult._tenantName;
        //        parentInfoListModel._token = pageResult._token;
        //    }
        //    return parentInfoListModel;
        //}

        /// <summary>
        /// Get All Parent Info List
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public GetAllParentInfoListForView GetAllParentInfoList(PageResult pageResult)
        {
            GetAllParentInfoListForView parentInfoListModel = new();
            IQueryable<ParentListView>? transactionIQ = null;

            var ParentInfoList = this.context?.ParentListView.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.Associationship == true);

            try
            {
                if (ParentInfoList != null && (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0))
                {
                    transactionIQ = ParentInfoList;
                }
                else
                {
                    if (ParentInfoList != null && pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");
                        transactionIQ = ParentInfoList.Where(x => (x.Firstname ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.Lastname ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.Middlename ?? "").ToLower().Contains(Columnvalue.ToLower()) || ((x.Firstname ?? "").ToLower() + (x.Middlename ?? "").ToLower() + (x.Lastname ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.Firstname ?? "").ToLower() + (x.Middlename ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.Firstname ?? "").ToLower() + (x.Lastname ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.Middlename ?? "").ToLower() + (x.Lastname ?? "").ToLower()).Contains(Columnvalue.ToLower()) || (x.WorkEmail ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.Mobile ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.FirstGivenName ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.StudentMiddleName ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.LastFamilyName ?? "").ToLower().Contains(Columnvalue.ToLower()) || (x.UserProfile ?? "").ToLower().Contains(Columnvalue.ToLower()));
                    }
                    else
                    {
                        if (pageResult.FilterParams?.Any() == true && ParentInfoList?.Any() == true)
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams, ParentInfoList).AsQueryable();
                        }
                    }
                }

                if (transactionIQ?.Any() == true)
                {
                    if (pageResult.SortingModel != null)
                    {
                        transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn!, pageResult.SortingModel.SortDirection!.ToLower());
                    }


                    int totalCount = transactionIQ.Select(x => x.ParentId).Distinct().Count();


                    if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                    {
                        var parentInfo = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
                        {
                            SchoolId = y.SchoolId,
                            TenantId = y.TenantId,
                            ParentId = y.ParentId,
                            Firstname = y.Firstname,
                            Middlename = y.Middlename,
                            Lastname = y.Lastname,
                            Salutation = y.Salutation,
                            Suffix = y.Suffix,
                            WorkEmail = y.WorkEmail,
                            WorkPhone = y.WorkPhone,
                            HomePhone = y.HomePhone,
                            PersonalEmail = y.PersonalEmail,
                            Mobile = y.Mobile,
                            UserProfile = y.UserProfile,
                            CreatedBy = y.CreatedBy,
                            CreatedOn = y.CreatedOn,
                            UpdatedBy = y.UpdatedBy,
                            UpdatedOn = y.UpdatedOn
                        }).Distinct().Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);

                        parentInfoListModel.parentInfoForView = parentInfo.ToList();
                    }
                    else
                    {
                        parentInfoListModel.parentInfoForView = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
                        {
                            SchoolId = y.SchoolId,
                            TenantId = y.TenantId,
                            ParentId = y.ParentId,
                            Firstname = y.Firstname,
                            Middlename = y.Middlename,
                            Lastname = y.Lastname,
                            Salutation = y.Salutation,
                            Suffix = y.Suffix,
                            WorkEmail = y.WorkEmail,
                            WorkPhone = y.WorkPhone,
                            HomePhone = y.HomePhone,
                            PersonalEmail = y.PersonalEmail,
                            Mobile = y.Mobile,
                            UserProfile = y.UserProfile,
                            CreatedBy = y.CreatedBy,
                            CreatedOn = y.CreatedOn,
                            UpdatedBy = y.UpdatedBy,
                            UpdatedOn = y.UpdatedOn
                        }).Distinct().ToList();
                    }

                    parentInfoListModel.parentInfoForView.ForEach(e =>
                    {
                        e.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.CreatedBy);
                        e.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, e.UpdatedBy);
                    });
                    int[] ParentId = parentInfoListModel.parentInfoForView.Select(p => p.ParentId).ToArray();
                    if (ParentInfoList != null && ParentInfoList.Any())
                    {
                        var ParentAssoList = ParentInfoList.Where(x => x.Associationship == true && ParentId.Contains(x.ParentId)).Select(a => new { StudentId = a.StudentId, ParentId = a.ParentId, FirstGivenName = a.FirstGivenName == null ? null : a.FirstGivenName, StudentMiddleName = a.StudentMiddleName == null ? null : a.StudentMiddleName, LastFamilyName = a.LastFamilyName == null ? null : a.LastFamilyName }).ToList();

                        foreach (var ParentInfo in parentInfoListModel.parentInfoForView)
                        {
                            if (ParentAssoList != null)
                            {
                                var studentAssociateWithParents = ParentAssoList.Where(x => x.ParentId == ParentInfo.ParentId).ToList();
                                List<string> studentArray = new List<string>();
                                if (studentAssociateWithParents.Count > 0)
                                {
                                    foreach (var studentAssociateWithParent in studentAssociateWithParents)
                                    {
                                        studentArray.Add(studentAssociateWithParent.FirstGivenName + "|" + studentAssociateWithParent.StudentMiddleName + "|" + studentAssociateWithParent.LastFamilyName + "|" + studentAssociateWithParent.StudentId);
                                        ParentInfo.students = studentArray.ToArray();
                                    }
                                }
                            }
                        }
                    }
                    parentInfoListModel.TenantId = pageResult.TenantId;
                    parentInfoListModel.TotalCount = totalCount;
                    parentInfoListModel.PageNumber = pageResult.PageNumber;
                    parentInfoListModel._pageSize = pageResult.PageSize;
                    parentInfoListModel._tenantName = pageResult._tenantName;
                    parentInfoListModel._token = pageResult._token;
                    parentInfoListModel._failure = false;
                }
            }
            catch (Exception es)
            {
                parentInfoListModel.parentInfoForView = null!;
                parentInfoListModel._message = es.Message;
                parentInfoListModel._failure = true;
                parentInfoListModel._tenantName = pageResult._tenantName;
                parentInfoListModel._token = pageResult._token;
            }
            return parentInfoListModel;
        }

        //public GetAllParentInfoListForView GetAllParentInfoList(PageResult pageResult)
        //{
        //    GetAllParentInfoListForView parentInfoListModel = new GetAllParentInfoListForView();
        //    IQueryable<ParentListView> transactionIQ = null;

        //    var ParentInfoList = this.context?.PaentListView.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && x.Associationship == true);

        //    try
        //    {
        //        if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
        //        {
        //            transactionIQ = ParentInfoList;

        //            transactionIQ = transactionIQ.Select(y => new ParentListView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                Students = y.Students
        //            }).Distinct();
        //        }
        //        else
        //        {
        //            if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
        //            {
        //                string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
        //                transactionIQ = ParentInfoList.Where(x => x.Firstname.ToLower().Contains(Columnvalue.ToLower()) || x.Lastname.ToLower().Contains(Columnvalue.ToLower()) || x.Middlename.ToLower().Contains(Columnvalue.ToLower()) || x.WorkEmail.ToLower().Contains(Columnvalue.ToLower()) || x.Mobile.ToLower().Contains(Columnvalue.ToLower()) || x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) || x.StudentMiddleName.ToLower().Contains(Columnvalue.ToLower()) || x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || x.UserProfile.ToLower().Contains(Columnvalue.ToLower()));

        //                transactionIQ = transactionIQ.Select(y => new ParentListView
        //                {
        //                    SchoolId = y.SchoolId,
        //                    TenantId = y.TenantId,
        //                    ParentId = y.ParentId,
        //                    Firstname = y.Firstname,
        //                    Middlename = y.Middlename,
        //                    Lastname = y.Lastname,
        //                    Salutation = y.Salutation,
        //                    Suffix = y.Suffix,
        //                    WorkEmail = y.WorkEmail,
        //                    WorkPhone = y.WorkPhone,
        //                    HomePhone = y.HomePhone,
        //                    PersonalEmail = y.PersonalEmail,
        //                    Mobile = y.Mobile,
        //                    UserProfile = y.UserProfile,
        //                    Students = y.Students
        //                }).Distinct();
        //            }
        //            else
        //            {
        //                transactionIQ = Utility.FilteredData(pageResult.FilterParams, ParentInfoList).AsQueryable();

        //                transactionIQ = transactionIQ.AsNoTracking().Select(y => new ParentListView
        //                {
        //                    SchoolId = y.SchoolId,
        //                    TenantId = y.TenantId,
        //                    ParentId = y.ParentId,
        //                    Firstname = y.Firstname,
        //                    Middlename = y.Middlename,
        //                    Lastname = y.Lastname,
        //                    Salutation = y.Salutation,
        //                    Suffix = y.Suffix,
        //                    WorkEmail = y.WorkEmail,
        //                    WorkPhone = y.WorkPhone,
        //                    HomePhone = y.HomePhone,
        //                    PersonalEmail = y.PersonalEmail,
        //                    Mobile = y.Mobile,
        //                    UserProfile = y.UserProfile,
        //                    Students = y.Students
        //                }).GroupBy(x => x.ParentId).Select(g => g.First());
        //            }
        //        }


        //        if (pageResult.SortingModel != null)
        //        {
        //            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn, pageResult.SortingModel.SortDirection.ToLower());
        //        }

        //        int totalCount = transactionIQ.Count();

        //        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
        //        {
        //            var parentInfo = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                StudentList = y.Students
        //            }).Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);

        //            parentInfoListModel.parentInfoForView = parentInfo.ToList();
        //        }
        //        else
        //        {
        //            parentInfoListModel.parentInfoForView = transactionIQ.AsNoTracking().Select(y => new GetParentInfoForView
        //            {
        //                SchoolId = y.SchoolId,
        //                TenantId = y.TenantId,
        //                ParentId = y.ParentId,
        //                Firstname = y.Firstname,
        //                Middlename = y.Middlename,
        //                Lastname = y.Lastname,
        //                Salutation = y.Salutation,
        //                Suffix = y.Suffix,
        //                WorkEmail = y.WorkEmail,
        //                WorkPhone = y.WorkPhone,
        //                HomePhone = y.HomePhone,
        //                PersonalEmail = y.PersonalEmail,
        //                Mobile = y.Mobile,
        //                UserProfile = y.UserProfile,
        //                StudentList = y.Students
        //            }).ToList();
        //        }

        //        foreach (var ParentInfo in parentInfoListModel.parentInfoForView)
        //        {

        //            string[] students = ParentInfo.StudentList.Split(",");

        //            List<string> studentArray = new List<string>();
        //            foreach (string s in students)
        //            {
        //                studentArray.Add(s);
        //                ParentInfo.students = studentArray.ToArray();
        //            }
        //        }
        //        parentInfoListModel.TenantId = pageResult.TenantId;
        //        parentInfoListModel.TotalCount = totalCount;
        //        parentInfoListModel.PageNumber = pageResult.PageNumber;
        //        parentInfoListModel._pageSize = pageResult.PageSize;
        //        parentInfoListModel._tenantName = pageResult._tenantName;
        //        parentInfoListModel._token = pageResult._token;
        //        parentInfoListModel._failure = false;
        //    }
        //    catch (Exception es)
        //    {
        //        parentInfoListModel.parentInfoForView = null;
        //        parentInfoListModel._message = es.Message;
        //        parentInfoListModel._failure = true;
        //        parentInfoListModel._tenantName = pageResult._tenantName;
        //        parentInfoListModel._token = pageResult._token;
        //    }
        //    return parentInfoListModel;
        //}

        /// <summary>
        /// Delete Parent Info
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel DeleteParentInfo(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            try
            {
                var ParentInfo = this.context?.ParentInfo.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo!.TenantId && x.ParentId == parentInfoAddViewModel.parentInfo.ParentId);
                this.context?.ParentInfo.Remove(ParentInfo!);
                this.context?.SaveChanges();
                parentInfoAddViewModel._failure = false;
                parentInfoAddViewModel._message = "Parent Deleted Successfully";
            }
            catch (Exception es)
            {
                parentInfoAddViewModel._failure = true;
                parentInfoAddViewModel._message = es.Message;
            }
            return parentInfoAddViewModel;
        }

        /// <summary>
        /// Search Parent Info For Student
        /// </summary>
        /// <param name="getAllParentInfoListForView"></param>
        /// <returns></returns>
        public GetAllParentInfoListForView SearchParentInfoForStudent(GetAllParentInfoListForView getAllParentInfoListForView)
        {
            GetAllParentInfoListForView parentInfoListModel = new();
            try
            {
                var containParentId = this.context?.ParentAssociationship.Where(x => x.TenantId == getAllParentInfoListForView.TenantId && x.SchoolId == getAllParentInfoListForView.SchoolId && x.StudentId == getAllParentInfoListForView.StudentId && x.Associationship == true).Select(x => x.ParentId).ToList();
                //string parentIDs = null;
                //if (containParentId.Count() > 0)
                //{
                //    parentIDs = string.Join(",", containParentId);
                //}

                List<int> parentIDs = new List<int> { };

                if (containParentId?.Any() == true)
                {
                    parentIDs = containParentId;
                }
                else
                {
                    parentIDs = new();
                }

                var ParentInfoList = this.context?.ParentInfo.Include(x => x.ParentAddress).Where(x => x.TenantId == getAllParentInfoListForView.TenantId && (string.IsNullOrEmpty(getAllParentInfoListForView.Firstname) || (x.Firstname == getAllParentInfoListForView.Firstname.Trim())) && (string.IsNullOrEmpty(getAllParentInfoListForView.Lastname) || (x.Lastname == getAllParentInfoListForView.Lastname)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Email) || (x.PersonalEmail == getAllParentInfoListForView.Email)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Mobile) || (x.Mobile == getAllParentInfoListForView.Mobile)) && (string.IsNullOrEmpty(getAllParentInfoListForView.StreetAddress) || (x.ParentAddress.Any(pa => pa.AddressLineOne == getAllParentInfoListForView.StreetAddress))) && (string.IsNullOrEmpty(getAllParentInfoListForView.City) || (x.ParentAddress.Any(pa => pa.City == getAllParentInfoListForView.City))) && (string.IsNullOrEmpty(getAllParentInfoListForView.State) || (x.ParentAddress.Any(pa => pa.State == getAllParentInfoListForView.State))) && (string.IsNullOrEmpty(getAllParentInfoListForView.Zip) || (x.ParentAddress.Any(pa => pa.Zip == getAllParentInfoListForView.Zip))) && (parentIDs == null || (!parentIDs.Contains(x.ParentId)))).ToList();

                /*var ParentInfoList = this.context?.ParentListView.Where(x => x.TenantId == getAllParentInfoListForView.TenantId && (string.IsNullOrEmpty(getAllParentInfoListForView.Firstname) || (x.Firstname == getAllParentInfoListForView.Firstname)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Lastname) || (x.Lastname == getAllParentInfoListForView.Lastname)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Email) || (x.PersonalEmail == getAllParentInfoListForView.Email)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Mobile) || (x.Mobile == getAllParentInfoListForView.Mobile)) && (string.IsNullOrEmpty(getAllParentInfoListForView.StreetAddress) || (x.AddressLineOne == getAllParentInfoListForView.StreetAddress)) && (string.IsNullOrEmpty(getAllParentInfoListForView.City) || (x.City == getAllParentInfoListForView.City)) && (string.IsNullOrEmpty(getAllParentInfoListForView.State) || (x.State == getAllParentInfoListForView.State)) && (string.IsNullOrEmpty(getAllParentInfoListForView.Zip) || (x.Zip == getAllParentInfoListForView.Zip)) && (parentIDs == null || (!parentIDs.Contains(x.ParentId)))).ToList();*/

                //foreach (var parent in ParentInfoList)
                //{
                //    if (parent.ParentAddress.FirstOrDefault().StudentAddressSame == true)
                //    {
                //        var parentAddress = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == parent.ParentAddress.FirstOrDefault().StudentId && x.SchoolId == parent.SchoolId);
                //        parent.ParentAddress.FirstOrDefault().AddressLineOne = parentAddress.HomeAddressLineOne;
                //        parent.ParentAddress.FirstOrDefault().AddressLineTwo = parentAddress.HomeAddressLineTwo;
                //        parent.ParentAddress.FirstOrDefault().Country = parentAddress.HomeAddressCountry;
                //        parent.ParentAddress.FirstOrDefault().State = parentAddress.HomeAddressState;
                //        parent.ParentAddress.FirstOrDefault().City = parentAddress.HomeAddressCity;
                //        parent.ParentAddress.FirstOrDefault().Zip = parentAddress.HomeAddressZip;
                //    }
                //}

                if (ParentInfoList?.Any() == true)
                {
                    parentInfoListModel.TenantId = getAllParentInfoListForView.TenantId;
                    //parentInfoListModel.parentInfoForView = parentInfo;
                    parentInfoListModel.parentInfo = ParentInfoList;
                    parentInfoListModel._tenantName = getAllParentInfoListForView._tenantName;
                    parentInfoListModel._token = getAllParentInfoListForView._token;
                    parentInfoListModel._failure = false;
                }
            }
            catch (Exception es)
            {
                parentInfoListModel._message = es.Message;
                parentInfoListModel._failure = true;
                parentInfoListModel._tenantName = getAllParentInfoListForView._tenantName;
                parentInfoListModel._token = getAllParentInfoListForView._token;
            }
            return parentInfoListModel;
        }

        /// <summary>
        /// Get Parent Info By Id 
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel ViewParentInfo(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            ParentInfoAddViewModel parentInfoViewModel = new();
            try
            {
                var parentInfo = this.context?.ParentInfo.Include(x => x.ParentAddress).Where(x => x.TenantId == parentInfoAddViewModel.parentInfo!.TenantId && x.ParentId == parentInfoAddViewModel.parentInfo.ParentId).FirstOrDefault();
                if (parentInfo != null)
                {
                    var parentAddress = parentInfo.ParentAddress.FirstOrDefault();
                    if (parentAddress != null)
                    {
                        if (parentAddress.StudentAddressSame == true)
                        {
                            var studentAddress = this.context?.StudentMaster.Where(x => x.TenantId == parentInfo.TenantId && x.SchoolId == parentInfo.SchoolId && x.StudentId == parentAddress.StudentId).Select(s => new { HomeAddressLineOne = s.HomeAddressLineOne, HomeAddressLineTwo = s.HomeAddressLineTwo, HomeAddressCountry = s.HomeAddressCountry, HomeAddressState = s.HomeAddressState, HomeAddressCity = s.HomeAddressCity, HomeAddressZip = s.HomeAddressZip }).FirstOrDefault();
                            if (studentAddress != null)
                            {
                                parentAddress.AddressLineOne = studentAddress.HomeAddressLineOne?.TrimEnd(',');
                                parentAddress.AddressLineTwo = studentAddress.HomeAddressLineTwo?.TrimEnd(',');
                                parentAddress.Country = studentAddress.HomeAddressCountry?.TrimEnd(',');
                                parentAddress.State = studentAddress.HomeAddressState?.TrimEnd(',');
                                parentAddress.City = studentAddress.HomeAddressCity?.TrimEnd(',');
                                parentAddress.Zip = studentAddress.HomeAddressZip?.TrimEnd(',');
                            }
                        }
                        else
                        {
                            parentAddress.AddressLineOne = (parentAddress.AddressLineOne != null) ? (parentAddress.AddressLineOne ?? "").TrimEnd(',') : null;
                            parentAddress.AddressLineTwo = (parentAddress.AddressLineTwo != null) ? (parentAddress.AddressLineTwo ?? "").TrimEnd(',') : null;
                            parentAddress.Country = (parentAddress.Country != null) ? (parentAddress.Country ?? "").TrimEnd(',') : null;
                            parentAddress.State = parentAddress.State?.TrimEnd(',');
                            parentAddress.City = (parentAddress.City != null) ? (parentAddress.City ?? "").TrimEnd(',') : null;
                            parentAddress.Zip = (parentAddress.Zip != null) ? (parentAddress.Zip ?? "").TrimEnd(',') : null;
                        }
                    }
                    parentInfoViewModel.parentInfo = parentInfo;
                    var AssociationshipData = this.context?.ParentAssociationship.Where(x => x.TenantId == parentInfo.TenantId && x.ParentId == parentInfo.ParentId && x.Associationship == true).ToList();

                    if (AssociationshipData?.Any() == true)
                    {
                        foreach (var studentAssociateWithParent in AssociationshipData)
                        {
                            var student = this.context?.StudentMaster.Include(s => s.StudentEnrollment).Include(s => s.SchoolMaster).Where(x => x.TenantId == studentAssociateWithParent.TenantId && x.SchoolId == studentAssociateWithParent.SchoolId && x.StudentId == studentAssociateWithParent.StudentId).FirstOrDefault();

                            //var student = this.context?.StudentMaster.FirstOrDefault(x => x.StudentId == studentAssociateWithParent.StudentId && x.SchoolId == studentAssociateWithParent.SchoolId);
                            if (student != null)
                            {
                                var studentForView = new GetStudentForView()
                                {
                                    TenantId = student.TenantId,
                                    SchoolId = student.SchoolId,
                                    StudentId = student.StudentId,
                                    StudentInternalId = student.StudentInternalId ?? student.StudentInternalId,
                                    FirstGivenName = student.FirstGivenName ?? student.FirstGivenName,
                                    MiddleName = student.MiddleName ?? student.MiddleName,
                                    LastFamilyName = student.LastFamilyName ?? student.LastFamilyName,
                                    Dob = student.Dob ?? student.Dob,
                                    Gender = student.Gender ?? student.Gender,
                                    Address = ToFullAddress((student.HomeAddressLineOne != null) ? student.HomeAddressLineOne!.TrimEnd(',') : null, (student.HomeAddressLineTwo != null) ? student.HomeAddressLineTwo!.TrimEnd(',') : null, (student.HomeAddressCity != null) ? student.HomeAddressCity!.TrimEnd(',') : null, (student.HomeAddressState != null) ? student.HomeAddressState!.TrimEnd(',') : null,
                                (student.HomeAddressCountry != null) ? student.HomeAddressCountry!.TrimEnd(',') : null, (student.HomeAddressZip != null) ? student.HomeAddressZip!.TrimEnd(',') : null),
                                    SchoolName = student.SchoolMaster.SchoolName ?? student.SchoolMaster.SchoolName,
                                    GradeLevelTitle = student.StudentEnrollment.Count > 0 ? student.StudentEnrollment.OrderByDescending(x => x.EnrollmentDate).FirstOrDefault()!.GradeLevelTitle : null,
                                    IsCustodian = studentAssociateWithParent.IsCustodian,
                                    Relationship = studentAssociateWithParent.Relationship,
                                    //StudentPhoto = student.StudentPhoto ?? student.StudentPhoto
                                    StudentPhoto = student.StudentThumbnailPhoto ?? student.StudentThumbnailPhoto
                                };
                                //var studentForView = new GetStudentForView()
                                //{
                                //    TenantId = student.TenantId,
                                //    SchoolId = student.SchoolId,
                                //    StudentId = student.StudentId,
                                //    StudentInternalId = student.StudentInternalId,
                                //    FirstGivenName = student.FirstGivenName,
                                //    MiddleName = student.MiddleName,
                                //    LastFamilyName = student.LastFamilyName,
                                //    Dob = student.Dob,
                                //    Gender = student.Gender,
                                //    Address = ToFullAddress(student.HomeAddressLineOne, student.HomeAddressLineTwo, student.HomeAddressCity, student.HomeAddressState,
                                //    student.HomeAddressCountry, student.HomeAddressZip),
                                //    SchoolName = this.context?.SchoolMaster.Where(x => x.SchoolId == student.SchoolId)?.Select(s => s.SchoolName).FirstOrDefault(),
                                //    GradeLevelTitle = this.context?.StudentEnrollment.Where(x => x.TenantId == student.TenantId && x.SchoolId == student.SchoolId && x.StudentId == student.StudentId).OrderByDescending(x => x.EnrollmentDate).LastOrDefault().GradeLevelTitle,
                                //    IsCustodian = studentAssociateWithParent.IsCustodian,
                                //    Relationship = studentAssociateWithParent.Relationship,
                                //    StudentPhoto = student.StudentPhoto
                                //};
                                parentInfoViewModel.getStudentForView.Add(studentForView);
                            }
                        }
                    }
                    parentInfoViewModel._tenantName = parentInfoAddViewModel._tenantName;
                    parentInfoViewModel._token = parentInfoAddViewModel._token;
                    parentInfoViewModel._failure = false;
                }
                else
                {
                    parentInfoViewModel._failure = true;
                    parentInfoViewModel._message = NORECORDFOUND;
                    return parentInfoViewModel;
                }
            }
            catch (Exception es)
            {
                parentInfoViewModel._failure = true;
                parentInfoViewModel._message = es.Message;
            }
            return parentInfoViewModel;
        }

        /// <summary>
        /// Add Parent Info
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel AddParentInfo(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            if (parentInfoAddViewModel.parentInfo is null)
            {
                return parentInfoAddViewModel;
            }
            try
            {
                int? ParentId = Utility.GetMaxPK(this.context, new Func<ParentInfo, int>(x => x.ParentId));
                parentInfoAddViewModel.parentInfo.ParentId = (int)ParentId!;
                parentInfoAddViewModel.parentInfo.CreatedOn = DateTime.UtcNow;
                parentInfoAddViewModel.parentInfo.LoginEmail = parentInfoAddViewModel.parentInfo.LoginEmail;
                this.context?.ParentInfo.Add(parentInfoAddViewModel.parentInfo);
                this.context?.SaveChanges();
                parentInfoAddViewModel._failure = false;
                parentInfoAddViewModel._message = "Parent Added Successfully";
            }
            catch (Exception es)
            {
                parentInfoAddViewModel._failure = true;
                parentInfoAddViewModel._message = es.Message;
            }
            return parentInfoAddViewModel;
        }

        /// <summary>
        /// Remove Associated Parent
        /// </summary>
        /// <param name="parentInfoDeleteViewModel"></param>
        /// <returns></returns>
        public ParentInfoDeleteViewModel RemoveAssociatedParent(ParentInfoDeleteViewModel parentInfoDeleteViewModel)
        {
            try
            {
                var ParentInfo = this.context?.ParentAssociationship.FirstOrDefault(x => x.TenantId == parentInfoDeleteViewModel.parentInfo!.TenantId && x.SchoolId == parentInfoDeleteViewModel.StudentSchoolId && x.ParentId == parentInfoDeleteViewModel.parentInfo.ParentId && x.StudentId == parentInfoDeleteViewModel.StudentId);
                if (ParentInfo != null)
                {
                    ParentInfo.Associationship = false;
                    this.context?.SaveChanges();
                    parentInfoDeleteViewModel._failure = false;
                    parentInfoDeleteViewModel._message = "Parent Associationship Removed Successfully";
                }
                else
                {
                    parentInfoDeleteViewModel._failure = true;
                    parentInfoDeleteViewModel._message = NORECORDFOUND;
                }

            }
            catch (Exception es)
            {
                parentInfoDeleteViewModel._failure = true;
                parentInfoDeleteViewModel._message = es.Message;
            }
            return parentInfoDeleteViewModel;
        }

        /// <summary>
        /// Add/Update Parent Photo
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>
        public ParentInfoAddViewModel AddUpdateParentPhoto(ParentInfoAddViewModel parentInfoAddViewModel)
        {
            if (parentInfoAddViewModel.parentInfo is null)
            {
                return parentInfoAddViewModel;
            }
            try
            {
                var parentPhotoUpdate = this.context?.ParentInfo.FirstOrDefault(x => x.TenantId == parentInfoAddViewModel.parentInfo.TenantId && x.ParentId == parentInfoAddViewModel.parentInfo.ParentId);

                if (parentPhotoUpdate != null)
                {
                    parentPhotoUpdate.UpdatedOn = DateTime.UtcNow;
                    parentPhotoUpdate.ParentPhoto = parentInfoAddViewModel.parentInfo.ParentPhoto;
                    parentPhotoUpdate.ParentThumbnailPhoto = parentInfoAddViewModel.parentInfo.ParentThumbnailPhoto;
                    parentPhotoUpdate.UpdatedBy = parentInfoAddViewModel.parentInfo.UpdatedBy;
                    this.context?.SaveChanges();
                    parentInfoAddViewModel._message = "Parent Photo Updated Successfully";
                }
                else
                {
                    parentInfoAddViewModel._failure = true;
                    parentInfoAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                parentInfoAddViewModel._failure = true;
                parentInfoAddViewModel._message = es.Message;
            }
            return parentInfoAddViewModel;
        }
    }
}
