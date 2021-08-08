﻿using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.StudentAttendances
{
    public class StudendAttendanceAdministrationViewModel
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StudentId { get; set; }
        public Guid StudentGuid { get; set; }
        public string StudentInternalId { get; set; }
        public string FirstGivenName { get; set; }
        public string MiddleName { get; set; }
        public string LastFamilyName { get; set; }
        public string GradeLevelTitle { get; set; }
        public string Section { get; set; }
        public string AttendanceComment { get; set; }
        public string Present { get; set; }
        public int? GradeId { get; set; }
        public int? SectionId { get; set; }
        public List<StudentAttendance> studentAttendanceList { get; set; }

    }
}
