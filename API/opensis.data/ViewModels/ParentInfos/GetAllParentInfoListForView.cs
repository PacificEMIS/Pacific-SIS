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

using opensis.data.Models;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.ParentInfos
{
    public class GetAllParentInfoListForView : CommonFields
    {
        public GetAllParentInfoListForView()
        {
            parentInfoForView = new List<GetParentInfoForView>();
            parentInfo = new List<ParentInfo>();
        }
        public List<GetParentInfoForView> parentInfoForView { get; set; }

        public List<ParentInfo> parentInfo { get; set; }
        
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }      
        public int? StudentId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? StreetAddress { get; set; }      
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
    }
}
