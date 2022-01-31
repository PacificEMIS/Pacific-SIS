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

using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public NoticeRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Add Notice
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public NoticeAddViewModel AddNotice(NoticeAddViewModel notice)
        {
            if (notice.Notice is null)
            {
                return notice;
            }
            try
            {
                //int? noticeId = Utility.GetMaxPK(this.context, new Func<Notice, int>(x => x.NoticeId));

                int? noticeId = 1;

                var NoticeData = this.context?.Notice.Where(x => x.SchoolId == notice.Notice.SchoolId && x.TenantId == notice.Notice.TenantId).OrderByDescending(x => x.NoticeId).FirstOrDefault();

                if (NoticeData != null)
                {
                    noticeId = NoticeData.NoticeId + 1;
                }

                if(notice.Notice != null)
                {
                    notice.Notice.NoticeId = (int)noticeId;
                    notice.Notice.Isactive = true;
                    notice.Notice.CreatedOn = DateTime.UtcNow;
                    this.context?.Notice.Add(notice.Notice);
                    this.context?.SaveChanges();
                    notice._failure = false;
                    notice._message = "Notice Added Successfully";
                }
                
                return notice;
            }
            catch (Exception ex)
            {
                notice._failure = false;
                notice._message = ex.Message;
                return notice;
            }

        }

        /// <summary>
        /// Get Notice By Id
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public NoticeAddViewModel ViewNotice(NoticeAddViewModel notice)
        {
            if (notice.Notice is null)
            {
                return notice;
            }
            NoticeAddViewModel noticeAddViewModel = new NoticeAddViewModel();
            try
            {
                var noticeModel = this.context?.Notice.FirstOrDefault(x => x.TenantId == notice.Notice.TenantId && x.SchoolId == notice.Notice.SchoolId && x.NoticeId == notice.Notice.NoticeId);
                if (noticeModel != null)
                {
                    noticeAddViewModel.Notice = noticeModel;
                }
                else
                {
                    noticeAddViewModel._failure = true;
                    noticeAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                noticeAddViewModel._failure = true;
                noticeAddViewModel._message = ex.Message;
            }
            return noticeAddViewModel;
        }

        /// <summary>
        /// Delete Notice
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public NoticeDeleteModel DeleteNotice(NoticeDeleteModel notice)
        {
            try
            {
                var noticeRepository = this.context?.Notice.FirstOrDefault(x => x.TenantId == notice.TenantId && x.SchoolId == notice.SchoolId && x.NoticeId == notice.NoticeId);

                if (noticeRepository != null)
                {
                    noticeRepository.Isactive = false;
                    this.context?.SaveChanges();
                    notice._failure = false;
                    notice._message = "Notice Deleted Successfully";
                }
                else
                {
                    notice._failure = true;
                    notice._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {

                notice._failure = true;
                notice._message = ex.Message;
            }
            return notice;
        }

        /// <summary>
        /// Update Notice
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public NoticeAddViewModel UpdateNotice(NoticeAddViewModel notice)
        {
            if (notice.Notice is null)
            {
                return notice;
            }
            try
            {
                var noticeRepository = this.context?.Notice.FirstOrDefault(x => x.TenantId == notice.Notice.TenantId && x.SchoolId == notice.Notice.SchoolId && x.NoticeId == notice.Notice.NoticeId);

                if (noticeRepository != null)
                {
                    if(notice.Notice != null)
                    {
                        notice.Notice.Isactive = true;
                        notice.Notice.CreatedBy = noticeRepository.CreatedBy;
                        notice.Notice.CreatedOn = noticeRepository.CreatedOn;
                        notice.Notice.UpdatedOn = DateTime.UtcNow;
                        this.context?.Entry(noticeRepository).CurrentValues.SetValues(notice.Notice);
                        this.context?.SaveChanges();
                        notice._failure = false;
                        notice._message = "Notice Updated Successfully";
                    }
                    
                    return notice;
                }
                else
                {
                    notice._failure = true;
                    notice._message = NORECORDFOUND;
                    return notice;
                }
            }
            catch (Exception ex)
            {
                notice._failure = true;
                notice._message = ex.Message;
                return notice;
            }

        }

        /// <summary>
        /// Get all Notice
        /// </summary>
        /// <returns></returns>
        public NoticeListViewModel GetAllNotice(NoticeListViewModel noticeList)
        {
            NoticeListViewModel getAllNoticeList = new NoticeListViewModel();
            try
            {
                var membershipData = this.context?.Membership.FirstOrDefault(d => d.TenantId == noticeList.TenantId && d.SchoolId == noticeList.SchoolId && d.MembershipId == noticeList.MembershipId);

                if (membershipData != null)
                {
                    //var noticeRepository = this.context?.Notice.OrderBy(x => x.ValidFrom).ThenBy(x => x.SortOrder).Where(x => x.TenantId == noticeList.TenantId && (x.SchoolId == noticeList.SchoolId || (x.SchoolId != noticeList.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && (membershipData.ProfileType == "Super Administrator" || membershipData.ProfileType == "School Administrator"|| membershipData.ProfileType == "Admin Assistant" || x.TargetMembershipIds.Contains(noticeList.MembershipId.ToString()))).ToList();

                    //var noticeRepository = this.context?.Notice.OrderBy(x => x.ValidFrom).ThenBy(x => x.SortOrder).Where(x => x.TenantId == noticeList.TenantId && (x.SchoolId == noticeList.SchoolId || (x.SchoolId != noticeList.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && (membershipData.ProfileType == "Super Administrator" || membershipData.ProfileType == "School Administrator" || membershipData.ProfileType == "Admin Assistant" || (noticeList.MembershipId!=null && x.TargetMembershipIds.Contains(noticeList.MembershipId.ToString()!)))).ToList();
                    var noticeRepository = this.context?.Notice.OrderBy(x => x.ValidFrom).ThenBy(x => x.SortOrder).AsEnumerable().Where(x => x.TenantId == noticeList.TenantId && (x.SchoolId == noticeList.SchoolId || (x.SchoolId != noticeList.SchoolId && x.VisibleToAllSchool == true)) && x.Isactive == true && (String.Compare(membershipData.ProfileType, "Super Administrator", true) == 0 || String.Compare(membershipData.ProfileType, "School Administrator", true) == 0 || String.Compare(membershipData.ProfileType, "Admin Assistant", true) == 0 || (noticeList.MembershipId != null && x.TargetMembershipIds.Contains(noticeList.MembershipId.ToString()!)))).ToList();

                    if (noticeRepository != null && noticeRepository.Any())
                    {
                        foreach (var notice in noticeRepository)
                        {
                            if (!string.IsNullOrEmpty(notice.TargetMembershipIds))
                            {
                                string[] membersList = notice.TargetMembershipIds.Split(",");
                                int[] memberIds = Array.ConvertAll(membersList, s => int.Parse(s));
                                var profiles = this.context?.Membership.Where(t => memberIds.Contains(t.MembershipId) && t.SchoolId == noticeList.SchoolId).Select(t => t.Profile).ToArray();

                                if(profiles != null)
                                {
                                    var mebershipIds = string.Join("," + " ", profiles);
                                    notice.TargetMembershipIds = mebershipIds;
                                }
                            }
                        }

                        getAllNoticeList.NoticeList = noticeRepository;
                        getAllNoticeList._failure = false;
                        return getAllNoticeList;
                    }
                    else
                    {
                        getAllNoticeList._failure = true;
                        getAllNoticeList._message = NORECORDFOUND;
                        return getAllNoticeList;
                    }
                }              

            }
            catch (Exception ex)
            {
                getAllNoticeList.NoticeList = null!;
                getAllNoticeList._failure = true;
                getAllNoticeList._message = ex.Message;
                return getAllNoticeList;
            }
            return getAllNoticeList;
        }
    }
}
