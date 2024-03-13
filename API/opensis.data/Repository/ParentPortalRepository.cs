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
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class ParentPortalRepository:IParentPortalRepository
    {
        private readonly CRMContext? context;
        //private readonly CatalogDBContext catdbContext;
        private static readonly string NORECORDFOUND = "No Record Found";
        public ParentPortalRepository(IDbContextFactory dbContextFactory) 
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get Student List For Parent
        /// </summary>
        /// <param name="parentInfoAddViewModel"></param>
        /// <returns></returns>

        public ParentInfoAddViewModel GetStudentListForParent(ParentInfoAddViewModel parentInfo)
        {
            ParentInfoAddViewModel parentInfoViewModel = new();
            try
            {
                var AssociationshipData = this.context?.ParentAssociationship.Where(x => x.TenantId == parentInfo.parentInfo!.TenantId && x.ParentId == parentInfo.parentInfo.ParentId && x.Associationship == true).ToList();

                if (AssociationshipData?.Any() == true)
                {
                    foreach (var studentAssociateWithParent in AssociationshipData)
                    {
                        var student = this.context?.StudentMaster.Include(s => s.StudentEnrollment).Include(s => s.SchoolMaster).Where(x => x.StudentId == studentAssociateWithParent.StudentId && x.SchoolId == studentAssociateWithParent.SchoolId && x.TenantId == studentAssociateWithParent.TenantId && x.IsActive == true).FirstOrDefault();

                        if (student != null)
                        {
                             var studentForView = new GetStudentForView()
                            {
                                TenantId = student.TenantId,
                                SchoolId = student.SchoolId,
                                StudentId = student.StudentId,
                                StudentGuid = student.StudentGuid,
                                StudentInternalId = student.StudentInternalId ?? student.StudentInternalId,
                                FirstGivenName = student.FirstGivenName ?? student.FirstGivenName,
                                MiddleName = student.MiddleName ?? student.MiddleName,
                                LastFamilyName = student.LastFamilyName ?? student.LastFamilyName,
                                StudentPhoto = student.StudentThumbnailPhoto ?? student.StudentThumbnailPhoto
                            };
                            parentInfoViewModel.getStudentForView.Add(studentForView);
                        }
                    }
                    parentInfoViewModel._tenantName = parentInfo._tenantName;
                    parentInfoViewModel._token = parentInfo._token;
                    parentInfoViewModel._failure = false;
                }
                else
                {
                    parentInfoViewModel._failure = true;
                    parentInfoViewModel._message = NORECORDFOUND;
                    return parentInfoViewModel;
                }
            }
            catch (Exception ex)
            {
                parentInfoViewModel._failure = true;
                parentInfoViewModel._message = ex.Message;
                return parentInfoViewModel;
            }
            return parentInfoViewModel;
        }
        /// <summary>
        /// Get All Schools By StudentId
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public SchoolListModel GetAllSchoolsByStudentId(SchoolListModel school)
        {
            SchoolListModel schoolListModel = new();
            schoolListModel.PageNumber = null;
            schoolListModel._pageSize = null;
            schoolListModel._tenantName = school._tenantName;
            schoolListModel._token = school._token;

            try
            {
                var membershipData = this.context?.Membership.FirstOrDefault(x => x.TenantId == school.TenantId && x.MembershipId == school.MembershipId);

                var schoolList = this.context?.StudentMaster.Include(x => x.SchoolMaster).ThenInclude(x => x.SchoolDetail).Where(x => x.TenantId == school.TenantId && x.StudentGuid == school.StudentGuid && x.SchoolMaster.SchoolDetail.FirstOrDefault()!.Status != false).OrderBy(x => x.SchoolMaster.SchoolName).Select(e => new GetSchoolForView()
                {
                    SchoolId = e.SchoolId,
                    TenantId = e.TenantId,
                    StudentId = e.StudentId,
                    SchoolName = e.SchoolMaster.SchoolName!.Trim(),
                    DateSchoolOpened = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.StartDate,
                    DateSchoolClosed = this.context!.SchoolCalendars.FirstOrDefault(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.SessionCalendar == true && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date)!.EndDate,
                    MembershipId = membershipData!.MembershipId,
                    MembershipType = membershipData!.ProfileType
                }).ToList();

                if (schoolList != null && schoolList.Any())
                {
                    schoolListModel.getSchoolForView = schoolList;
                    schoolListModel._failure = false;
                }
                else
                {
                    schoolListModel.schoolMaster = new();
                    schoolListModel._failure = true;
                    schoolListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                schoolListModel._message = es.Message;
                schoolListModel._failure = true;
            }
            return schoolListModel;
        }
    }
}
