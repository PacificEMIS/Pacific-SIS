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
    public partial class GradebookGrades
    {
       
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public decimal AcademicYear { get; set; }
        //public int MarkingPeriodId { get; set; }
        public int CourseSectionId { get; set; }
        public int AssignmentTypeId { get; set; }
        public int AssignmentId { get; set; }
        /// <summary>
        /// * (Asterisk) is also allowed, hence char datatype
        /// </summary>
        public string? AllowedMarks { get; set; }
        public string? Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public string? Comment { get; set; }
        public string? RunningAvg { get; set; }
        public string? RunningAvgGrade { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? QtrMarkingPeriodId { get; set; }
        public int? SmstrMarkingPeriodId { get; set; }
        public int? YrMarkingPeriodId { get; set; }
        public int? PrgrsprdMarkingPeriodId { get; set; }
        [ValidateNever]
        public virtual Assignment Assignment { get; set; } = null!;
        [ValidateNever]
        public virtual StudentMaster StudentMaster { get; set; } = null!;
    }
}
