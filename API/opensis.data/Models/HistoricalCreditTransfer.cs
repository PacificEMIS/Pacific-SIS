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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.data.Models
{
    public partial class HistoricalCreditTransfer
    {
        

        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public int HistGradeId { get; set; }
        public int HistMarkingPeriodId { get; set; }
        public int CreditTransferId { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public decimal? Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public decimal? GpValue { get; set; }
        public bool? CalculateGpa { get; set; }
        public decimal? GradeScale { get; set; }
        public decimal? CreditAttempted { get; set; }
        public decimal? CreditEarned { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? CourseType { get; set; }
        [ValidateNever]
        public virtual HistoricalGrade HistoricalGrade { get; set; } = null!;
    }
}
