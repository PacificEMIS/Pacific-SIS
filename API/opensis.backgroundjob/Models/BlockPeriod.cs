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
//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.backgroundjob.Models
{
    public partial class BlockPeriod
    {
        
        public BlockPeriod()
        {
            //CourseBlockSchedule = new HashSet<CourseBlockSchedule>();
            //CourseCalendarSchedule = new HashSet<CourseCalendarSchedule>();
            //CourseFixedSchedule = new HashSet<CourseFixedSchedule>();
            //CourseVariableSchedule = new HashSet<CourseVariableSchedule>();
            StudentAttendance = new HashSet<StudentAttendance>();
            StudentMissingAttendances = new HashSet<StudentMissingAttendance>();
        }

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int BlockId { get; set; }
        public int PeriodId { get; set; }
        public string? PeriodTitle { get; set; }
        public string? PeriodShortName { get; set; }
        public string? PeriodStartTime { get; set; }
        public string? PeriodEndTime { get; set; }
        public int? PeriodSortOrder { get; set; }
        public bool? CalculateAttendance { get; set; }
        public decimal? AcademicYear { get; set; }
        public int? RolloverId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        //[ValidateNever]
        //public virtual SchoolMaster SchoolMaster { get; set; } = null!;
        //public virtual ICollection<CourseBlockSchedule> CourseBlockSchedule { get; set; }
        //public virtual ICollection<CourseCalendarSchedule> CourseCalendarSchedule { get; set; }
        //public virtual ICollection<CourseFixedSchedule> CourseFixedSchedule { get; set; }
        //public virtual ICollection<CourseVariableSchedule> CourseVariableSchedule { get; set; }
        public virtual ICollection<StudentAttendance> StudentAttendance { get; set; }
        public virtual ICollection<StudentMissingAttendance> StudentMissingAttendances { get; set; }
    }
}
