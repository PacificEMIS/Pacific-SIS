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
    public partial class GradebookConfiguration
    {
        

        public GradebookConfiguration()
        {
            GradebookConfigurationGradescale = new HashSet<GradebookConfigurationGradescale>();
            GradebookConfigurationQuarter = new HashSet<GradebookConfigurationQuarter>();
            GradebookConfigurationSemester = new HashSet<GradebookConfigurationSemester>();
            GradebookConfigurationYear = new HashSet<GradebookConfigurationYear>();
            GradebookConfigurationProgressPeriods = new HashSet<GradebookConfigurationProgressPeriod>();
        }

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int CourseId { get; set; }
        public int CourseSectionId { get; set; }
        public decimal AcademicYear { get; set; }
        public int GradebookConfigurationId { get; set; }
        /// <summary>
        /// weight grades,assigned date defaults to today,due date defaults to today - separated by pipe(|)
        /// </summary>
        public string? General { get; set; }
        public string? ScoreRounding { get; set; }
        public string? AssignmentSorting { get; set; }
        public int? MaxAnomalousGrade { get; set; }
        public int? UpgradedAssignmentGradeDays { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? ConfigUpdateFlag { get; set; }
        [ValidateNever]
        public virtual CourseSection CourseSection { get; set; } = null!;
        public virtual ICollection<GradebookConfigurationGradescale> GradebookConfigurationGradescale { get; set; }
        public virtual ICollection<GradebookConfigurationProgressPeriod> GradebookConfigurationProgressPeriods { get; set; }
        public virtual ICollection<GradebookConfigurationQuarter> GradebookConfigurationQuarter { get; set; }
        public virtual ICollection<GradebookConfigurationSemester> GradebookConfigurationSemester { get; set; }
        public virtual ICollection<GradebookConfigurationYear> GradebookConfigurationYear { get; set; }
    }
}
