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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.data.Models
{
    public partial class GradeScale
    {
       

        public GradeScale()
        {
            CourseSection = new HashSet<CourseSection>();
            Grade = new HashSet<Grade>();
            StudentDailyAttendance = new HashSet<StudentDailyAttendance>();
        }

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int GradeScaleId { get; set; }
        public string? GradeScaleName { get; set; }
        public decimal? GradeScaleValue { get; set; }
        public string? GradeScaleComment { get; set; }
        public bool? CalculateGpa { get; set; }
        public bool? UseAsStandardGradeScale { get; set; }
        public int? SortOrder { get; set; }
        public decimal? AcademicYear { get; set; }
        public int? RolloverId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        
        [ValidateNever]
        public virtual SchoolMaster SchoolMaster { get; set; } = null!;
        public virtual ICollection<CourseSection> CourseSection { get; set; }
        public virtual ICollection<Grade> Grade { get; set; }
        public virtual ICollection<StudentDailyAttendance> StudentDailyAttendance { get; set; }
    }
}
