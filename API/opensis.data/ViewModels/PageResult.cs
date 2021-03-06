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

using opensis.data.ViewModels;
using opensis.data.ViewModels.CommonModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.Models
{
    public class PageResult: CommonFields
    {
        
        public Guid TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StaffId { get; set; }
        public int? StudentId { get; set; }
        public int? CourseSectionId { get; set; }

        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

       // public string FilterText { get; set; }

        //public string SchoolNameFilter { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public SortingModel? SortingModel { get; set; }

        public List<FilterParams>? FilterParams { get; set; }
        public DateTime? DobStartDate { get; set; }
        public DateTime? DobEndDate { get; set; }
        public string? FullName { get; set; }
        public bool? ProfilePhoto { get; set; }
        public bool? IncludeInactive { get; set; }
        public bool? SearchAllSchool { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int? AttendanceCode { get; set; }
        public string? EmailAddress { get; set; }
        public decimal? AcademicYear { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsListView { get; set; }
        public bool? IsSchoolSpecific { get; set; }
        public bool? AciveStudentInCourseSection { get; set; }
        public DateTime? MarkingPeriodStartDate { get; set; }
        public DateTime? MarkingPeriodEndDate { get; set; }
        public string? GradeLevel { get; set; }
        public bool? IsHomeRoomTeacher { get; set; }
        public int? PeriodId { get; set; }
        public int[]? CourseSectionIds { get; set; }
    }
}
