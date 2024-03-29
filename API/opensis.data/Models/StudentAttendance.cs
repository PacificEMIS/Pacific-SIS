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

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace opensis.data.Models
{
    public partial class StudentAttendance
    {
        

        public StudentAttendance()
        {
            StudentAttendanceComments = new HashSet<StudentAttendanceComments>();
        }

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public int StaffId { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int AttendanceCategoryId { get; set; }
        public int AttendanceCode { get; set; }
        public int BlockId { get; set; }
        public int PeriodId { get; set; }
        public long StudentAttendanceId { get; set; }
        public int? MembershipId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [ValidateNever]
        public virtual AttendanceCode AttendanceCodeNavigation { get; set; } = null!;
        [ValidateNever]
        public virtual BlockPeriod BlockPeriod { get; set; } = null!;

        public virtual Membership? Membership { get; set; }
        [ValidateNever]
        public virtual StaffCoursesectionSchedule StaffCoursesectionSchedule { get; set; } = null!;
        [ValidateNever]
        public virtual StudentCoursesectionSchedule StudentCoursesectionSchedule { get; set; } = null!;
        public virtual ICollection<StudentAttendanceComments> StudentAttendanceComments { get; set; }

    }
}
