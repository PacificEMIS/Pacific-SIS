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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.ViewModels.GradeReport
{
    public class StudentFinalGradeViewModel : CommonFields
    {
        public StudentFinalGradeViewModel()
        {
            studentDetailsViews = new List<StudentDetailsView>();
        }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public decimal? AcademicYear { get; set; }
        public string? MarkingPeriods { get; set; }
        public int[]? StudentIds { get; set; }
        public bool? Teacher { get; set; }
        public bool? Comments { get; set; }
        public bool? Parcentage { get; set; }
        public bool? YearToDateDailyAbsences { get; set; }
        public bool? DailyAbsencesThisQuater { get; set; }
        public bool? PeriodByPeriodAbsences { get; set; }
        public bool? OtherAttendanceYearToDate { get; set; }
        public bool? OtherAttendanceThisQuater { get; set; }
        public List<StudentDetailsView> studentDetailsViews { get; set; }

    }

    public class StudentDetailsView
    {
        public StudentDetailsView()
        {
            courseSectionDetailsViews = new List<CourseSectionDetailsView>();
        }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public List<CourseSectionDetailsView> courseSectionDetailsViews { get; set; }

    }

    public class CourseSectionDetailsView
    {
        public CourseSectionDetailsView()
        {
            markingPeriodDetailsViews = new List<MarkingPeriodDetailsView>();
        }
        public string? CourseSectionName { get; set; }
        public string? StaffFirstGivenName { get; set; }
        public string? StaffMiddleName { get; set; }
        public string? StaffLastFamilyName { get; set; }
        public int? AbsYTD { get; set; }
        public string? Comments { get; set; }
        public List<MarkingPeriodDetailsView> markingPeriodDetailsViews { get; set; }

    }
    public class MarkingPeriodDetailsView
    {
        public int? MarkingPeriodId { get; set; }
        public string? MarkingPeriodName { get; set; }
        public decimal? Percentage { get; set; }
        public string? Grade { get; set; }
    }

}
