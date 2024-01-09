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
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StudentPortal
{
    public class ScheduledCourseSectionViewModelForStudent:CommonFields
    {
        public ScheduledCourseSectionViewModelForStudent() 
        {
            courseSectionViewList = new List<CourseSectionViewList>();
            NotificationList = new List<string>();
            NoticeList = new List<Models.Notice>();
            AssignmentList = new List<AssignmentDetails>();
        }
        public List<CourseSectionViewList> courseSectionViewList { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public int? MembershipId { get; set; }
        public int CourseSectionId { get; set; }
        public bool? AllCourse { get; set; }
        public int? MissingAttendanceCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<opensis.data.Models.Notice> NoticeList { get; set; }
        public List<string> NotificationList { get; set; }
        public List<AssignmentDetails> AssignmentList { get; set; }
        public decimal? AcademicYear { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }

        public string? StudentInternalId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? GradeLevel { get; set; }
        public string? Section { get; set; }
        public byte[]? StudentPhoto { get; set; }
    }

    public class AssignmentDetails
    {
        public int? AssignmentTypeId { get; set; }
        public int? AssignmentId { get; set; }
        public int? CourseSectionId { get; set; }
        public string? CourseSectionTitle { get; set; }
        public string? AssignmentTypeTitle { get; set; }
        public string? AssignmentTitle { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? AssignmentDescription { get; set; }
    }
}
