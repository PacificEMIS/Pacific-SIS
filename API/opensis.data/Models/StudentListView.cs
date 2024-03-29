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
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace opensis.data.Models
{
    public partial class StudentListView
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public string? AlternateId { get; set; }
        public string? DistrictId { get; set; }
        public string? StateId { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? RollNumber { get; set; }
        public string? Salutation { get; set; }
        public string? FirstGivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastFamilyName { get; set; }
        public string? Suffix { get; set; }
        public string? PreferredName { get; set; }
        public string? PreviousName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public string? OtherGovtIssuedNumber { get; set; }
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
        public string? HomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? SchoolEmail { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? Linkedin { get; set; }
        public string? HomeAddressLineOne { get; set; }
        public string? HomeAddressLineTwo { get; set; }
        public string? HomeAddressCity { get; set; }
        public string? HomeAddressState { get; set; }
        public string? HomeAddressZip { get; set; }
        public string? BusNo { get; set; }
        public bool? SchoolBusPickUp { get; set; }
        public bool? SchoolBusDropOff { get; set; }
        public bool? MailingAddressSameToHome { get; set; }
        public string? MailingAddressLineOne { get; set; }
        public string? MailingAddressLineTwo { get; set; }
        public string? MailingAddressCity { get; set; }
        public string? MailingAddressState { get; set; }
        public string? MailingAddressZip { get; set; }
        public string? HomeAddressCountry { get; set; }
        public string? MailingAddressCountry { get; set; }
        public int? SectionId { get; set; }
        public string? StudentInternalId { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public string? EnrollmentType { get; set; }
        public bool? IsActive { get; set; }
        public Guid StudentGuid { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string? EnrollmentCode { get; set; }
        public int? CalenderId { get; set; }
        public int? GradeId { get; set; }
        public string? GradeLevelTitle { get; set; }
        public string? RollingOption { get; set; }
        public string? SchoolName { get; set; }
        public string? SectionName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? StudentPortalId { get; set; }
        public bool? Eligibility504 { get; set; }
        public bool? EconomicDisadvantage { get; set; }
        public bool? FreeLunchEligibility { get; set; }
        public bool? SpecialEducationIndicator { get; set; }
        public bool? LepIndicator { get; set; }
        public DateTime? EstimatedGradDate { get; set; }
    }
}
