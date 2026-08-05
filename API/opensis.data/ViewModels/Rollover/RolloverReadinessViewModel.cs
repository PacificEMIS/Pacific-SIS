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

namespace opensis.data.ViewModels.Rollover
{
    /// <summary>
    /// Read-only pre-rollover completeness summary shown to the operator before
    /// the irreversible rollover runs. Purely informational — never blocks.
    /// </summary>
    public class RolloverReadinessViewModel : CommonFields
    {
        public RolloverReadinessViewModel()
        {
            StudentsByGradeGender = new List<GradeGenderCount>();
            Dispositions = new List<DispositionGradeGender>();
            TerminalGradeNeedsReview = new List<GradeGenderCount>();
            TerminalGradeTitles = new List<string>();
            ExitsByCode = new List<ExitCodeGradeGender>();
            SectionsMissingAttendance = new List<SectionCompletenessRow>();
            SectionsMissingGrades = new List<SectionCompletenessRow>();
        }

        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public decimal? AcademicYear { get; set; }

        public int TotalStudents { get; set; }
        public List<GradeGenderCount> StudentsByGradeGender { get; set; }

        // Grouped by StudentEnrollment.RollingOption — the disposition the
        // rollover engine will act on. "NotSet" bucket = null/blank/unknown.
        public List<DispositionGradeGender> Dispositions { get; set; }

        // Terminal-grade (Gradelevels.NextGradeId == null) students still on
        // "Next grade at current school" or with no disposition — the rollover
        // will silently treat them as completers.
        public List<GradeGenderCount> TerminalGradeNeedsReview { get; set; }
        public List<string> TerminalGradeTitles { get; set; }

        // Mid-year exits grouped by the operator-selected exit code title.
        public List<ExitCodeGradeGender> ExitsByCode { get; set; }

        public List<SectionCompletenessRow> SectionsMissingAttendance { get; set; }
        public List<SectionCompletenessRow> SectionsMissingGrades { get; set; }

        // Active enrollments with no calendar link — they belong to no school
        // year and are excluded from every table above.
        public int UnlinkedEnrollmentCount { get; set; }
    }

    public class GradeGenderCount
    {
        public int? GradeId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? Gender { get; set; }
        public int Count { get; set; }
        public int? SortOrder { get; set; }
    }

    public class DispositionGradeGender
    {
        // A RollingOptionsEnum value ("Next grade at current school", "Retain",
        // "Do not enroll after this school year", "Enrol to another school")
        // or "NotSet".
        public string? DispositionKey { get; set; }
        public int Total { get; set; }
        public List<GradeGenderCount> Rows { get; set; } = new List<GradeGenderCount>();
    }

    public class ExitCodeGradeGender
    {
        public string? ExitCodeTitle { get; set; }
        public int Total { get; set; }
        public List<GradeGenderCount> Rows { get; set; } = new List<GradeGenderCount>();
    }

    public class SectionCompletenessRow
    {
        public int? CourseSectionId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseSectionName { get; set; }
        public string? TeacherName { get; set; }
        public string? MarkingPeriodTitle { get; set; }
        public int Count { get; set; }
    }
}
