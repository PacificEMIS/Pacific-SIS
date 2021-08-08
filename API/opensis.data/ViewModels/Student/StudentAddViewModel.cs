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

using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.Student
{
    public class StudentAddViewModel : CommonFields
    {
        public StudentMaster studentMaster { get; set; }
        public List<FieldsCategory> fieldsCategoryList { get; set; }
        public List<int> studentIds { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string AcademicYear { get; set; }
        public string LoginEmail { get; set; }
        public string PasswordHash { get; set; }
        public bool? PortalAccess { get; set; }
        public string CurrentGradeLevel { get; set; }
        public string EnrollmentDate { get; set; }
        public string Dob { get; set; }
        public string EstimatedGradDate { get; set; }
        public string CreatedOrUpdatedBy { get; set; }
        public string CountryOfBirthName { get; set; }
        public string NationalityName { get; set; }
        public string FirstLanguageName { get; set; }
        public string SecondLanguageName { get; set; }
        public string ThirdLanguageName { get; set; }
        public string SectionName { get; set; }
    }
}
