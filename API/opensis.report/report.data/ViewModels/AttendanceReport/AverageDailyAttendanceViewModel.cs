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

namespace opensis.report.report.data.ViewModels.AttendanceReport
{
    public class AverageDailyAttendanceViewModel : CommonFields
    {
        public AverageDailyAttendanceViewModel()
        {
            averageDailyAttendanceReport = new List<AverageDailyAttendanceReport>();
        }
        public List<AverageDailyAttendanceReport> averageDailyAttendanceReport { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? AcademicYear { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
    }

    public class AverageDailyAttendanceReport
    {
        public DateTime? Date { get; set; }
        public string? GradeLevel { get; set; }
        public int? GradeLevelSortOrder { get; set; }
        public int? Students { get; set; }
        public int? DaysPossible { get; set; }
        public int? AttendancePossible { get; set; }
        public int? Present { get; set; }
        public int? Absent { get; set; }
        public int? Other { get; set; }
        public int? NotTaken { get; set; }
        public decimal? ADA { get; set; }
        public decimal? AvgAttendance { get; set; }
        public decimal? AvgAbsent { get; set; }
    }

}
