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

using System;
using System.Collections.Generic;

namespace opensis.data.Models
{
    public partial class Course
    {
      
        public Course()
        {
            CourseSection = new HashSet<CourseSection>();
            CourseStandard = new HashSet<CourseStandard>();
        }

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int CourseId { get; set; }
        public decimal? AcademicYear { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseShortName { get; set; }
        public string? CourseGradeLevel { get; set; }
        public string? CourseProgram { get; set; }
        public string? CourseSubject { get; set; }
        /// <summary>
        /// &apos;Core&apos; or &apos;Elective&apos;
        /// </summary>
        public string? CourseCategory { get; set; }
        public double? CreditHours { get; set; }
        /// <summary>
        /// choose between US Common Core library or school specific standards library.
        /// </summary>
        public string? Standard { get; set; }
        public string? StandardRefNo { get; set; }
        public string? CourseDescription { get; set; }
        public bool? IsCourseActive { get; set; }
        public int? RolloverId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual ICollection<CourseSection> CourseSection { get; set; }
        public virtual ICollection<CourseStandard> CourseStandard { get; set; }
    }
}
