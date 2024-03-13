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

namespace opensis.data.ViewModels.StudentPortal
{
    public class StudentGradebookViewModel : CommonFields
    {
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<StudentGradebookGradeDetails>? studentGradebookGradeDetails { get; set; }
    }

    public class StudentGradebookGradeDetails
    {
        public int? CourseSectionId { get; set; }
        public string? CourseSectionName { get; set; }
        public string? StaffFirstName { get; set; }
        public string? StaffMiddleName { get; set; }
        public string? StaffLastName { get; set; }
        public string? Percent { get; set; }
        public string? Letter { get; set; }
        public int? Ungraded { get; set; }
        public string? LowestGrade { get; set; }
        public string? HighestGrade { get; set; }
    }
    public class StudentGradebookGradesByCourseSection : CommonFields
    {
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
        public List<StudentGradebookDetails>? studentGradebookDetails { get; set; }
    }
    public class StudentGradebookDetails
    {
        public string? AssignmentTitle { get; set; }
        public string? AssignmentType { get; set; }
        public string? AssignmentDescription { get; set; }
        public string? Points { get; set; }
        public int? Possible { get; set; }
        public string? Percent { get; set; }
        public string? Letter { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? LowestGrade { get; set; }
        public string? HighestGrade { get; set; }
    }
}
