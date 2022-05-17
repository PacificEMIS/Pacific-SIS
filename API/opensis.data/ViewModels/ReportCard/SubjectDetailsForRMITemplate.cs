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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.ReportCard
{
    public class SubjectDetailsForRMITemplate
    {
        public SubjectDetailsForRMITemplate()
        {
            courseSectionDetailsViewforRMIReports = new List<CourseSectionDetailsViewforRMIReport>();
        }
        public string? SubjectName { get; set; }
        public List<CourseSectionDetailsViewforRMIReport> courseSectionDetailsViewforRMIReports { get; set; }
    }

    public class CourseSectionDetailsViewforRMIReport
    {
        public CourseSectionDetailsViewforRMIReport()
        {
            markingPeriodDetailsViewforRMIReports = new List<MarkingPeriodDetailsViewforRMIReport>();
        }
        public string? CourseSectionName { get; set; }
        public List<MarkingPeriodDetailsViewforRMIReport> markingPeriodDetailsViewforRMIReports { get; set; }

    }
    public class MarkingPeriodDetailsViewforRMIReport
    {
        public string? MarkingPeriodName { get; set; }
        public decimal? Percentage { get; set; }
        public string? Grade { get; set; }
        public string? GPA { get; set; }
        public int? PresentCount { get; set; }
        public int? AbsencesCount { get; set; }
        public string? SortId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
