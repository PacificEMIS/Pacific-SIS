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
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.Student
{
    public class StudentMedicalInfoViewModel : CommonFields
    {
        public StudentMedicalInfoViewModel()
        {
            studentMedicalAlertList = new List<StudentMedicalAlert>();
            studentMedicalImmunizationList = new List<StudentMedicalImmunization>();
            studentMedicalNoteList = new List<StudentMedicalNote>();
            studentMedicalNurseVisitList = new List<StudentMedicalNurseVisit>();
            studentMedicalProviderList = new List<StudentMedicalProvider>();
            fieldsCategoryList = new List<FieldsCategory>();
        }
        public List<StudentMedicalAlert> studentMedicalAlertList { get; set; }
        public List<StudentMedicalImmunization> studentMedicalImmunizationList { get; set; }
        public List<StudentMedicalNote> studentMedicalNoteList { get; set; }
        public List<StudentMedicalNurseVisit> studentMedicalNurseVisitList { get; set; }
        public List<StudentMedicalProvider> studentMedicalProviderList { get; set; }
        public List<FieldsCategory> fieldsCategoryList { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
