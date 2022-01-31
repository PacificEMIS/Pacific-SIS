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

namespace opensis.data.ViewModels.ReportCard
{
    public class StudentsReportCardViewModel
    {
        public StudentsReportCardViewModel()
        {
            markingPeriodDetailsForDefaultTemplates = new List<MarkingPeriodDetailsForDefaultTemplate>();
            markingPeriodDetailsForOtherTemplates = new List<MarkingPeriodDetailsForOtherTemplate>();
            gradeList = new List<Grade>();
            teacherCommentList = new List<string>();
            courseCommentCategories = new List<CourseCommentCategory>();
        }    
        public string? SchoolName { get; set; }
        public byte[]? SchoolLogo { get; set; }
        public string? SchoolYear { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public int StudentId { get; set; }
        public Guid? StudentGuid { get; set; }
        public string? AlternateId { get; set; }
        public string? StudentInternalId { get; set; }
        public string? GradeTitle { get; set; }
        public string? Section { get; set; }
        public string? Gender { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? YearToDateAttendencePercent { get; set; }
        public int? YearToDateAbsencesInDays { get; set; }
        public List<MarkingPeriodDetailsForDefaultTemplate> markingPeriodDetailsForDefaultTemplates { get; set; }
        public List<MarkingPeriodDetailsForOtherTemplate> markingPeriodDetailsForOtherTemplates { get; set; }
        public List<Grade> gradeList { get; set; }
        public List<string> teacherCommentList { get; set; }
        public List<CourseCommentCategory> courseCommentCategories { get; set; }
    }
}
