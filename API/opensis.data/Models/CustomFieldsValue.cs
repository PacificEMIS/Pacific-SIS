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
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.data.Models
{
   public partial class CustomFieldsValue
    {
        
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int CategoryId { get; set; }
        public int FieldId { get; set; }
        /// <summary>
        /// Target_is school/student/staff id for whom custom field value is entered. For School module it will be always school id.
        /// </summary>
        public int TargetId { get; set; }
        /// <summary>
        /// &apos;Student&apos; | &apos;School&apos; | &apos;Staff&apos;
        /// </summary>
        public string Module { get; set; } = null!;
        public string? CustomFieldTitle { get; set; }
        /// <summary>
        /// &apos;Select&apos; or &apos;Text&apos;
        /// </summary>
        public string? CustomFieldType { get; set; }
        /// <summary>
        /// User input value...Textbox-&gt;textvalue, Select--&gt;Value separated by &apos;|&apos;, Date --&gt; Date in string
        /// </summary>
        public string? CustomFieldValue { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }

        [ValidateNever]
        public virtual CustomFields CustomFields { get; set; } = null!;
    }
}
