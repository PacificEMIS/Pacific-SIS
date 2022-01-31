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

using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public MembershipRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get All Members For Notice
        /// </summary>
        /// <returns></returns>
        public GetAllMembersList GetAllMemberList(GetAllMembersList membersList)
        {
            GetAllMembersList getAllMembersList = new GetAllMembersList();
            try
            {
                var membershipRepository = this.context?.Membership.Where(x => x.TenantId == membersList.TenantId && x.SchoolId == membersList.SchoolId && x.IsActive == true).ToList();

                if(membershipRepository != null && membershipRepository.Any())
                {
                    getAllMembersList.GetAllMemberList = membershipRepository;
                    getAllMembersList.TenantId = membersList.TenantId;
                    getAllMembersList.SchoolId = membersList.SchoolId;
                    getAllMembersList._tenantName = membersList._tenantName;
                    getAllMembersList._token = membersList._token;
                }
                

                return getAllMembersList;
            }
            catch (Exception ex)
            {
                getAllMembersList = null!;
                getAllMembersList._failure = true;
                getAllMembersList._message = ex.Message;
                return getAllMembersList;
            }
        }

        /// <summary>
        /// Add Member
        /// </summary>
        /// <param name="membershipAddViewModel"></param>
        /// <returns></returns>
        public MembershipAddViewModel AddMembership(MembershipAddViewModel membershipAddViewModel)
        {
            if (membershipAddViewModel.membership?.ProfileType is null)
            {
                return membershipAddViewModel;
            }
            List<RolePermission> rolePermissionList = new List<RolePermission>();
            try
            {
                //var memberShipData = this.context?.Membership.FirstOrDefault(c => c.SchoolId == membershipAddViewModel.membership.SchoolId && c.TenantId == membershipAddViewModel.membership.TenantId && c.ProfileType == membershipAddViewModel.membership.ProfileType);

                var memberShipData = this.context?.Membership.AsEnumerable().Where(c => c.SchoolId == membershipAddViewModel.membership.SchoolId && c.TenantId == membershipAddViewModel.membership.TenantId && String.Compare(c.ProfileType, membershipAddViewModel.membership.ProfileType, true) == 0).FirstOrDefault();

                if (memberShipData != null)
                {

                    //var checkProfile = this.context?.Membership.Where(x => x.SchoolId == membershipAddViewModel.membership.SchoolId && x.TenantId == membershipAddViewModel.membership.TenantId && x.Profile.ToLower() == membershipAddViewModel.membership.Profile.ToLower() && x.IsActive==true).FirstOrDefault();
                    var checkProfile = this.context?.Membership.AsEnumerable().Where(x => x.SchoolId == membershipAddViewModel.membership.SchoolId && x.TenantId == membershipAddViewModel.membership.TenantId && String.Compare(x.Profile, membershipAddViewModel.membership.Profile, true) == 0 && x.IsActive == true).FirstOrDefault();

                    if (checkProfile != null)
                    {
                        membershipAddViewModel._failure = true;
                        membershipAddViewModel._message = "Profile Already Exists";
                    }
                    else
                    {
                        int? MembershipId = 1;

                        var MembershipData = this.context?.Membership.Where(x => x.SchoolId == membershipAddViewModel.membership.SchoolId && x.TenantId == membershipAddViewModel.membership.TenantId).OrderByDescending(x => x.MembershipId).FirstOrDefault();

                        if (MembershipData != null)
                        {
                            MembershipId = MembershipData.MembershipId + 1;
                        }

                        //if (membershipAddViewModel.membership?.ProfileType?.ToLower().ToString() == "Super Administrator".ToLower())
                        if (String.Compare(membershipAddViewModel.membership.ProfileType, "Super Administrator", true) == 0)
                        {
                            membershipAddViewModel.membership.IsSuperadmin = true;
                        }
                        else
                        {
                            if(membershipAddViewModel.membership != null)
                            {
                                membershipAddViewModel.membership.IsSuperadmin = false;
                            }
                        }


                        var rollPermissionData = this.context?.RolePermission.Where(e => e.TenantId == memberShipData.TenantId && e.SchoolId == memberShipData.SchoolId && e.MembershipId == memberShipData.MembershipId).ToList();

                        if (rollPermissionData != null)
                        {
                            int? rolePermissionId = 1;

                            var rolePermissionIdData = this.context?.RolePermission.Where(x => x.TenantId == memberShipData.TenantId && x.SchoolId == memberShipData.SchoolId).OrderByDescending(x => x.RolePermissionId).FirstOrDefault();

                            if (rolePermissionIdData != null)
                            {
                                rolePermissionId = rolePermissionIdData.RolePermissionId + 1;
                            }

                            foreach (var rolePermission in rollPermissionData)
                            {
                                var rolePermissions = new RolePermission
                                {
                                    TenantId = rolePermission.TenantId,
                                    SchoolId = rolePermission.SchoolId,
                                    RolePermissionId = (int)rolePermissionId,
                                    PermissionCategoryId = rolePermission.PermissionCategoryId,
                                    CanView = rolePermission.CanView,
                                    CanEdit = rolePermission.CanEdit,
                                    CanAdd = rolePermission.CanAdd,
                                    CanDelete = rolePermission.CanDelete,
                                    CreatedBy = membershipAddViewModel.membership?.CreatedBy,
                                    CreatedOn = DateTime.UtcNow,
                                    MembershipId = MembershipId,
                                    PermissionSubcategoryId = rolePermission.PermissionSubcategoryId,
                                    PermissionGroupId = rolePermission.PermissionGroupId,
                                };
                                this.context?.RolePermission.Add(rolePermissions);
                                rolePermissionId++;
                            }
                        }
                        if(membershipAddViewModel.membership != null)
                        {
                            membershipAddViewModel.membership.IsActive = true;
                            membershipAddViewModel.membership.IsSystem = false;
                            membershipAddViewModel.membership.MembershipId = (int)MembershipId;
                            membershipAddViewModel.membership.CreatedOn = DateTime.UtcNow;
                            this.context?.Membership.Add(membershipAddViewModel.membership);
                            this.context?.SaveChanges();
                            membershipAddViewModel._failure = false;
                            membershipAddViewModel._message = "Membership Added Successfully";
                        }
                    }
                }
                else
                {
                    membershipAddViewModel._failure = true;
                    membershipAddViewModel._message = "Membership Type Not Found";               
                }
            }
            catch (Exception es)
            {
                membershipAddViewModel._message = es.Message;
                membershipAddViewModel._failure = true;
            }
            return membershipAddViewModel;
        }

        /// <summary>
        /// Update Member
        /// </summary>
        /// <param name="membershipAddViewModel"></param>
        /// <returns></returns>
        public MembershipAddViewModel UpdateMembership(MembershipAddViewModel membershipAddViewModel)
        {
            if (membershipAddViewModel.membership is null)
            {
                return membershipAddViewModel;
            }
            try
            {
                var memeberShipData = this.context?.Membership.FirstOrDefault(x => x.TenantId == membershipAddViewModel.membership.TenantId && x.SchoolId == membershipAddViewModel.membership.SchoolId && x.MembershipId == membershipAddViewModel.membership.MembershipId);
                
                if (memeberShipData != null)
                {
                    //var checkProfile = this.context?.Membership.Where(x => x.SchoolId == membershipAddViewModel.membership.SchoolId && x.TenantId == membershipAddViewModel.membership.TenantId && x.MembershipId != membershipAddViewModel.membership.MembershipId && x.Profile == membershipAddViewModel.membership.Profile && x.IsActive==true).FirstOrDefault();
                    var checkProfile = this.context?.Membership.AsEnumerable().Where(x => x.SchoolId == membershipAddViewModel.membership.SchoolId && x.TenantId == membershipAddViewModel.membership.TenantId && x.MembershipId != membershipAddViewModel.membership.MembershipId && String.Compare(x.Profile, membershipAddViewModel.membership.Profile, true) == 0 && x.IsActive == true).FirstOrDefault();

                    if (checkProfile != null)
                    {
                        membershipAddViewModel._failure = true;
                        membershipAddViewModel._message = "Profile Title Already Exists";
                    }
                    else
                    {
                        //if (membershipAddViewModel.membership.ProfileType.ToLower().ToString() == "Super Administrator")
                        //{
                        //    membershipAddViewModel.membership.IsSuperadmin = true;
                        //}
                        //else
                        //{
                        //    membershipAddViewModel.membership.IsSuperadmin = false;
                        //}
                        if(membershipAddViewModel.membership != null)
                        {
                            membershipAddViewModel.membership.ProfileType = memeberShipData.ProfileType;
                            membershipAddViewModel.membership.IsActive = true;
                            membershipAddViewModel.membership.IsSystem = false;
                            membershipAddViewModel.membership.CreatedBy = memeberShipData.CreatedBy;
                            membershipAddViewModel.membership.CreatedOn = memeberShipData.CreatedOn;
                            membershipAddViewModel.membership.UpdatedOn = DateTime.UtcNow;
                            this.context?.Entry(memeberShipData).CurrentValues.SetValues(membershipAddViewModel.membership);
                            this.context?.SaveChanges();
                            membershipAddViewModel._failure = false;
                            membershipAddViewModel._message = "Membership Updated Successfully";
                        }
                    }
                }
                else
                {
                    membershipAddViewModel.membership = null;
                    membershipAddViewModel._failure = true;
                    membershipAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                membershipAddViewModel._failure = true;
                membershipAddViewModel._message = ex.Message;
            }
            return membershipAddViewModel;
        }

        /// <summary>
        /// Delete Member
        /// </summary>
        /// <param name="membershipAddViewModel"></param>
        /// <returns></returns>
        public MembershipAddViewModel DeleteMembership(MembershipAddViewModel membershipAddViewModel)
        {
            if (membershipAddViewModel.membership is null)
            {
                return membershipAddViewModel;
            }
            try
            {
                var memeberShipData = this.context?.Membership.FirstOrDefault(x => x.TenantId == membershipAddViewModel.membership.TenantId && x.SchoolId == membershipAddViewModel.membership.SchoolId && x.MembershipId == membershipAddViewModel.membership.MembershipId);

                if (memeberShipData !=null)
                {
                    var userData = this.context?.UserMaster.Where(x => x.TenantId == membershipAddViewModel.membership.TenantId && x.SchoolId == membershipAddViewModel.membership.SchoolId && x.MembershipId == membershipAddViewModel.membership.MembershipId && x.IsActive == true).ToList();

                    if (userData != null && userData.Any())
                    {
                        membershipAddViewModel._failure = true;
                        membershipAddViewModel._message = "Membership Cannot Be Deleted, Because It Has Association";
                    }
                    else
                    {
                        memeberShipData.IsActive = false;
                        this.context?.SaveChanges();
                        membershipAddViewModel._failure = false;
                        membershipAddViewModel._message = "Membership Deleted Successfully";
                    }
                }
                else
                {
                    membershipAddViewModel._message = NORECORDFOUND;
                    membershipAddViewModel._failure = true;
                }
            }
            catch (Exception es)
            {
                membershipAddViewModel._failure = true;
                membershipAddViewModel._message = es.Message;
            }
            return membershipAddViewModel;
        }
    }
}
