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
using System.Text;

namespace opensis.data.ViewModels.StudentSchedule
{
    public class ScheduleStudentForView
    {
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public int? StudentId { get; set; }
        public Guid StudentGuid { get; set; }
        public int? GradeId { get; set; }
        public int? GradeScaleId { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? AlternateId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? GradeLevel { get; set; }
        public string? Section { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Action { get; set; }
        public byte[]? StudentPhoto { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int? CourseSectionId { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? RollNumber { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? MaritalStatus { get; set; }
        public int? CountryOfBirth { get; set; }
        public int? Nationality { get; set; }
        public int? FirstLanguageId { get; set; }
        public int? SecondLanguageId { get; set; }
        public int? ThirdLanguageId { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? BusNo { get; set; }
        public string? HomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? SchoolEmail { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsDropped { get; set; }
        public string? SchoolName { get; set; }
    }
}
