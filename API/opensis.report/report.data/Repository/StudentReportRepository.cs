using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.ParentInfos;
using opensis.data.ViewModels.Student;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.StudentReport;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace opensis.report.report.data.Repository
{
    public class StudentReportRepository : IStudentReportRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public IParentInfoRepository _parentInfoRepository;
        public StudentReportRepository(IDbContextFactory dbContextFactory, IParentInfoRepository parentInfoRepository)
        {
            this.context = dbContextFactory.Create();
            _parentInfoRepository = parentInfoRepository;
        }

        /// <summary>
        /// Get StudentInfo Report
        /// </summary>
        /// <param name="studentInfoReport"></param>
        /// <returns></returns>
        public StudentInfoListForReport GetStudentInfoReport(StudentInfoReport studentInfoReport)
        {
            StudentInfoListForReport studentListModel = new();

            try
            {
                var schoolList = this.context?.SchoolMaster.Where(x => x.TenantId == studentInfoReport.TenantId && x.StudentMaster.Any(x => x.SchoolId == studentInfoReport.SchoolId && studentInfoReport.StudentGuids.Contains(x.StudentGuid) && x.TenantId == studentInfoReport.TenantId));
                if (schoolList?.Any() == true)
                {
                    studentListModel.SchoolMasterData = schoolList.Select(x => new SchoolMasterData()
                    {
                        TenantId = x.TenantId,
                        SchoolId = x.SchoolId,
                        SchoolGuid = x.SchoolGuid,
                        SchoolName = x.SchoolName,
                        SchoolLogo = x.SchoolDetail.FirstOrDefault().SchoolLogo,
                        SchoolStateId = x.SchoolStateId,
                        SchoolDistrictId = x.SchoolDistrictId,
                        City = x.City,
                        State = x.State,
                        StreetAddress1 = x.StreetAddress1,
                        StreetAddress2 = x.StreetAddress2,
                        Country = x.Country,
                        Division = x.Division,
                        District = x.District,
                        Zip = x.Zip,
                        Longitude = x.Longitude,
                        Latitude = x.Latitude,
                        StudentMasterData = x.StudentMaster.Select(e => new StudentMasterData()
                        {
                            TenantId= e.TenantId,
                            SchoolId= e.SchoolId,
                            StudentId = e.StudentId,
                            StudentGuid = e.StudentGuid,
                            AlternateId = e.AlternateId,
                            DistrictId = e.DistrictId,
                            StateId = e.StateId,
                            GradeLevelTitle = e.StudentEnrollment.Where(x=>x.TenantId== e.TenantId && x.SchoolId== e.SchoolId && x.StudentId== e.StudentId && x.IsActive==true).FirstOrDefault().GradeLevelTitle,
                            AdmissionNumber = e.AdmissionNumber,
                            RollNumber = e.RollNumber,
                            Salutation = e.Salutation,
                            FirstGivenName = e.FirstGivenName,
                            MiddleName = e.MiddleName,
                            LastFamilyName = e.LastFamilyName,
                            Suffix = e.Suffix,
                            PreferredName = e.PreferredName,
                            PreviousName = e.PreviousName,
                            SchoolEmail = e.SchoolEmail,
                            MobilePhone = e.MobilePhone,
                            StudentPhoto = e.StudentPhoto,
                            SocialSecurityNumber = e.SocialSecurityNumber,
                            OtherGovtIssuedNumber = e.OtherGovtIssuedNumber,
                            Dob = e.Dob,
                            Gender = e.Gender,
                            Race = e.Race,
                            Ethnicity = e.Ethnicity,
                            MaritalStatus = e.MaritalStatus,
                            CountryOfBirth = e.CountryOfBirth,
                            Nationality = e.Nationality != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.Nationality))!.Name : null,
                            FirstLanguage = e.FirstLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.FirstLanguageId))!.Locale : null,
                            SecondLanguageId = e.SecondLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.SecondLanguageId))!.Locale : null,
                            ThirdLanguageId = e.ThirdLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.ThirdLanguageId))!.Locale : null,
                            SectionId = e.SectionId,
                            EstimatedGradDate = e.EstimatedGradDate,
                            Twitter = e.Twitter,
                            Facebook = e.Facebook,
                            Instagram = e.Instagram,
                            Youtube = e.Youtube,
                            Linkedin = e.Linkedin,
                            HomeAddressLineOne = e.HomeAddressLineOne,
                            HomeAddressLineTwo = e.HomeAddressLineTwo,
                            HomeAddressCountry = e.HomeAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.HomeAddressCountry))!.Name : null,
                            HomeAddressCity = e.HomeAddressCity,
                            HomeAddressState = e.HomeAddressState,
                            HomeAddressZip = e.HomeAddressZip,
                            MailingAddressSameToHome = e.MailingAddressSameToHome,
                            MailingAddressLineOne = e.MailingAddressLineOne,
                            MailingAddressLineTwo = e.MailingAddressLineTwo,
                            MailingAddressCountry = e.MailingAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.MailingAddressCountry))!.Name : null,
                            MailingAddressCity = e.MailingAddressCity,
                            MailingAddressState = e.MailingAddressState,
                            MailingAddressZip = e.MailingAddressZip,
                            IsActive = e.IsActive,
                            StudentEnrollment = studentInfoReport.IsEnrollmentInfo ? this.context.StudentEnrollment.Where(x => x.StudentGuid == e.StudentGuid && x.TenantId == e.TenantId).Select(y => new StudentEnrollmentListForView()
                            {
                                TenantId = y.TenantId,
                                SchoolId = y.SchoolId,
                                StudentId = y.StudentId,
                                StudentGuid = y.StudentGuid,
                                EnrollmentId = y.EnrollmentId,
                                SchoolName = y.SchoolName,
                                GradeLevelTitle = y.GradeLevelTitle,
                                EnrollmentDate = y.EnrollmentDate,
                                EnrollmentCode = y.EnrollmentCode,
                                ExitDate = y.ExitDate,
                                ExitCode = y.ExitCode,
                                SchoolTransferred = y.SchoolTransferred,
                                TransferredGrade = y.TransferredGrade,
                                StartYear = y.EnrollmentDate == null ? (y.ExitDate != null ? this.context.SchoolYears.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.StartDate <= y.ExitDate && z.EndDate >= y.ExitDate).StartDate.Value.Year.ToString() : null) : this.context.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar == true && z.StartDate <= y.EnrollmentDate && z.EndDate >= y.EnrollmentDate).StartDate!.Value.Year.ToString(),

                                EndYear = y.EnrollmentDate == null ? (y.ExitDate != null ? this.context.SchoolYears.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.StartDate <= y.ExitDate && z.EndDate >= y.ExitDate).EndDate.Value.Year.ToString() : null) : (this.context.SchoolCalendars.FirstOrDefault(z => z.TenantId == y.TenantId && z.SchoolId == y.SchoolId && z.SessionCalendar == true && z.StartDate <= y.EnrollmentDate && z.EndDate >= y.EnrollmentDate).EndDate!.Value.Year.ToString() ?? null),

                                IsActive = y.IsActive,
                            }).ToList():null,
                            StudentMedicalAlert = studentInfoReport.IsMedicalInfo ? !e.StudentMedicalAlert!.Any() ? null! : e.StudentMedicalAlert.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList() : null,
                            StudentMedicalImmunization = studentInfoReport.IsMedicalInfo ? !e.StudentMedicalImmunization!.Any() ? null! : e.StudentMedicalImmunization.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList() : null,
                            StudentMedicalNote = studentInfoReport.IsMedicalInfo ? !e.StudentMedicalNote!.Any() ? null! : e.StudentMedicalNote.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList() : null,
                            StudentMedicalNurseVisit = studentInfoReport.IsMedicalInfo ? !e.StudentMedicalNurseVisit!.Any() ? null! : e.StudentMedicalNurseVisit.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList() : null,
                            StudentMedicalProvider = studentInfoReport.IsMedicalInfo ? !e.StudentMedicalProvider!.Any() ? null! : e.StudentMedicalProvider.Where(x => x.TenantId == e.TenantId && x.SchoolId == e.SchoolId && x.StudentId == e.StudentId).ToList() : null,
                            Contacts = studentInfoReport.IsFamilyInfo ? _parentInfoRepository.ViewParentListForStudent(new ParentInfoListModel() { SchoolId = e.SchoolId, StudentId = e.StudentId, TenantId = e.TenantId, IsShowParentPhoto = true, IsReport = true }) : null,

                        }).Where(st => studentInfoReport!.StudentGuids!.Contains(st.StudentGuid) && st.SchoolId== x.SchoolId && st.TenantId == x.TenantId).ToList()

                    }).ToList();
                }
                else
                {
                    studentListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                studentListModel._failure = true;
                studentListModel._message = ex.Message;
            }
            return studentListModel;
        }

        /// <summary>
        /// Get All Student Field List
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>      
        public StudentAdvancedReport StudentAdvancedReport(StudentAdvancedReport reportModel)
        {
            StudentAdvancedReport reportList = new();
            try
            {
                var schoolList = this.context?.SchoolMaster.Where(x => x.TenantId == reportModel.TenantId && x.StudentMaster.Any(x => x.SchoolId == reportModel.SchoolId && reportModel.StudentGuids.Contains(x.StudentGuid) && x.TenantId == reportModel.TenantId));
                if (schoolList?.Any() == true)
                {
                    reportList.schoolListForReport = schoolList.Select(x => new SchoolReport()
                    {
                        TenantId = x.TenantId,
                        SchoolId = x.SchoolId,
                        SchoolGuid = x.SchoolGuid,
                        SchoolName = x.SchoolName,
                        SchoolLogo = x.SchoolDetail.FirstOrDefault().SchoolLogo,
                        SchoolStateId = x.SchoolStateId,
                        SchoolDistrictId = x.SchoolDistrictId,
                        City = x.City,
                        State = x.State,
                        StreetAddress1 = x.StreetAddress1,
                        StreetAddress2 = x.StreetAddress2,
                        Country = x.Country,
                        Division = x.Division,
                        District = x.District,
                        Zip = x.Zip,
                        Longitude = x.Longitude,
                        Latitude = x.Latitude
                    }).ToList();
                    List<SchoolReport> schoolListReport = new();
                    foreach (var school in reportList.schoolListForReport)
                    {
                        SchoolReport schoolReport = new();
                        schoolReport.TenantId = school.TenantId;
                        schoolReport.SchoolId = school.SchoolId;
                        schoolReport.SchoolGuid = school.SchoolGuid;
                        schoolReport.SchoolName = school.SchoolName;
                        schoolReport.SchoolLogo = school.SchoolLogo;
                        schoolReport.SchoolStateId = school.SchoolStateId;
                        schoolReport.SchoolDistrictId = school.SchoolDistrictId;
                        schoolReport.City = school.City;
                        schoolReport.State = school.State;
                        schoolReport.StreetAddress1 = school.StreetAddress1;
                        schoolReport.StreetAddress2 = school.StreetAddress2;
                        schoolReport.Country = school.Country;
                        schoolReport.Division = school.Division;
                        schoolReport.District = school.District;
                        schoolReport.Zip = school.Zip;
                        schoolReport.Longitude = school.Longitude;
                        schoolReport.Latitude = school.Latitude;
                        List<StudentReport> studentListForReport = new();

                        var studentData = this.context?.StudentMaster.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && reportModel!.StudentGuids!.Contains(x.StudentGuid)).ToList();

                        foreach (var student in studentData)
                        {
                            student.StudentPhoto = null;
                            StudentReport studentReport = new();
                            var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == school.TenantId && x.SchoolId == school.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
                        .Select(y => new FieldsCategory
                        {
                            TenantId = y.TenantId,
                            SchoolId = y.SchoolId,
                            CategoryId = y.CategoryId,
                            IsSystemCategory = y.IsSystemCategory,
                            Search = y.Search,
                            Title = y.Title,
                            Module = y.Module,
                            SortOrder = y.SortOrder,
                            Required = y.Required,
                            Hide = y.Hide,
                            UpdatedOn = y.UpdatedOn,
                            UpdatedBy = y.UpdatedBy,
                            CreatedBy = y.CreatedBy,
                            CreatedOn = y.CreatedOn,
                            CustomFields = y.CustomFields.Where(x => x.SystemField != true).Select(z => new CustomFields
                            {
                                TenantId = z.TenantId,
                                SchoolId = z.SchoolId,
                                CategoryId = z.CategoryId,
                                FieldId = z.FieldId,
                                Module = z.Module,
                                Type = z.Type,
                                Search = z.Search,
                                Title = z.Title,
                                SortOrder = z.SortOrder,
                                SelectOptions = z.SelectOptions,
                                SystemField = z.SystemField,
                                Required = z.Required,
                                Hide = z.Hide,
                                DefaultSelection = z.DefaultSelection,
                                UpdatedOn = z.UpdatedOn,
                                UpdatedBy = z.UpdatedBy,
                                CreatedBy = z.CreatedBy,
                                CreatedOn = z.CreatedOn,
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == student.StudentId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();
                            studentReport.fieldsCategoryList = customFields;
                            studentReport.studentMaster = student;
                            studentListForReport.Add(studentReport);
                        }
                        schoolReport.studentListForReport = studentListForReport;
                        schoolListReport.Add(schoolReport);
                    }
                    reportList.schoolListForReport = schoolListReport;
                }
                else
                {
                    reportList._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                reportList._failure = true;
                reportList._message = ex.Message;
            }
            return reportList;
        }
        /// <summary>
        /// Get student Add/Drop Report
        /// </summary>
        /// <param name="studentAddDropViewModel"></param>
        /// <returns></returns>
        public StudentAddDropViewModel GetstudentAddDropReport(PageResult pageResult)
        {
            StudentAddDropViewModel studentAddDrop = new StudentAddDropViewModel();
            IQueryable<StudentEnrollment>? transactionIQ = null;
            try
            {
                studentAddDrop.TenantId = pageResult.TenantId;
                studentAddDrop.SchoolId = pageResult.SchoolId;
                studentAddDrop.FromDate = pageResult.MarkingPeriodStartDate;
                studentAddDrop.ToDate = pageResult.MarkingPeriodEndDate;
                studentAddDrop.GradeLevel = pageResult.GradeLevel;
                studentAddDrop._token = pageResult._token;
                studentAddDrop._userName = pageResult._userName;
                studentAddDrop._tenantName = pageResult._tenantName;
                studentAddDrop.PageNumber = pageResult.PageNumber;
                studentAddDrop._pageSize = pageResult.PageSize;

                var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId);
                if (schoolData != null)
                {
                    studentAddDrop.SchoolName = schoolData.SchoolName;
                    studentAddDrop.StreetAddress1 = schoolData.StreetAddress1;
                    studentAddDrop.StreetAddress2 = schoolData.StreetAddress2;
                    studentAddDrop.Country = schoolData.Country;
                    studentAddDrop.City = schoolData.City;
                    studentAddDrop.State = schoolData.State;
                    studentAddDrop.District = schoolData.District;
                    studentAddDrop.Zip = schoolData.Zip;
                    studentAddDrop.SchoolLogo = schoolData.SchoolDetail.FirstOrDefault().SchoolLogo;

                }

                var studentEnrollmentData = this.context?.StudentEnrollment.Include(x => x.StudentMaster).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && ((x.EnrollmentDate >= pageResult.MarkingPeriodStartDate && x.EnrollmentDate <= pageResult.MarkingPeriodEndDate) || (x.ExitDate >= pageResult.MarkingPeriodStartDate && x.ExitDate <= pageResult.MarkingPeriodEndDate)) && (string.IsNullOrEmpty(pageResult.GradeLevel) || x.GradeLevelTitle.ToLower() == pageResult.GradeLevel.ToLower()));

                if (studentEnrollmentData?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentEnrollmentData;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");

                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentEnrollmentData.Where(x => x.StudentMaster.FirstGivenName != null && x.StudentMaster.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.StudentMaster.MiddleName != null && x.StudentMaster.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.StudentMaster.LastFamilyName != null && x.StudentMaster.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || ((x.StudentMaster.FirstGivenName ?? "").ToLower() + (x.StudentMaster.MiddleName ?? "").ToLower() + (x.StudentMaster.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.StudentMaster.FirstGivenName ?? "").ToLower() + (x.StudentMaster.MiddleName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.StudentMaster.FirstGivenName ?? "").ToLower() + (x.StudentMaster.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.StudentMaster.MiddleName ?? "").ToLower() + (x.StudentMaster.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) ||
                                                                        x.StudentMaster.StudentInternalId != null && x.StudentMaster.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.SchoolName != null && x.SchoolName.Contains(Columnvalue) ||
                                                                        x.EnrollmentCode != null && x.EnrollmentCode.Contains(Columnvalue) ||
                                                                        x.ExitCode != null && x.ExitCode.Contains(Columnvalue));
                        }
                    }

                    if (transactionIQ != null && transactionIQ.Any() == true)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                        }

                        int totalCount = transactionIQ.Count();

                        if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                        {
                            transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                        }

                        studentAddDrop.studentEnrollmentList = transactionIQ.ToList();
                        studentAddDrop.studentEnrollmentList.ForEach(x => { x.StudentMaster.StudentPhoto = null; x.StudentMaster.SchoolMaster = null; x.StudentMaster.StudentEnrollment = null; });

                        studentAddDrop.TotalCount = totalCount;
                        studentAddDrop._failure = false;
                    }
                    else
                    {
                        studentAddDrop._failure = true;
                        studentAddDrop._message = NORECORDFOUND;
                    }
                }
                else
                {
                    studentAddDrop._failure = true;
                    studentAddDrop._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                studentAddDrop._failure = true;
                studentAddDrop._message = es.Message;
            }
            return studentAddDrop;

        }


        /// <summary>
        /// Get Student Enrollment Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public StudentEnrollmentReport GetStudentEnrollmentReport(PageResult pageResult)
        {
            StudentEnrollmentReport studentListModel = new StudentEnrollmentReport();
            IQueryable<StudentListView>? transactionIQ = null;
            IQueryable<StudentListView>? studentDataList = null;
            int? totalCount = 0;

            try
            {
                studentListModel.TenantId = pageResult.TenantId;
                studentListModel.SchoolId = pageResult.SchoolId;
                studentListModel.FromDate = pageResult.MarkingPeriodStartDate;
                studentListModel.ToDate = pageResult.MarkingPeriodEndDate;
                studentListModel.GradeLevel = pageResult.GradeLevel;
                studentListModel._token = pageResult._token;
                studentListModel._userName = pageResult._userName;
                studentListModel._tenantName = pageResult._tenantName;
                studentListModel.PageNumber = pageResult.PageNumber;
                studentListModel._pageSize = pageResult.PageSize;

                var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId);
                if (schoolData != null)
                {
                    studentListModel.SchoolName = schoolData.SchoolName;
                    studentListModel.StreetAddress1 = schoolData.StreetAddress1;
                    studentListModel.StreetAddress2 = schoolData.StreetAddress2;
                    studentListModel.Country = schoolData.Country;
                    studentListModel.City = schoolData.City;
                    studentListModel.State = schoolData.State;
                    studentListModel.District = schoolData.District;
                    studentListModel.Zip = schoolData.Zip;
                    studentListModel.SchoolLogo = schoolData.SchoolDetail.FirstOrDefault().SchoolLogo;

                }
                studentDataList = this.context?.StudentListView.Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && (pageResult.IncludeInactive == false || pageResult.IncludeInactive == null ? x.IsActive != false : true) && x.EnrollmentDate >= pageResult.MarkingPeriodStartDate && x.EnrollmentDate <= pageResult.MarkingPeriodEndDate && (string.IsNullOrEmpty(pageResult.GradeLevel) || x.GradeLevelTitle.ToLower() == pageResult.GradeLevel.ToLower()) && x.EnrollmentCode.ToLower() == "new");

                if (studentDataList?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentDataList;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");
                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentDataList.Where(x => x.FirstGivenName != null && x.FirstGivenName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.MiddleName != null && x.MiddleName.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.LastFamilyName != null && x.LastFamilyName.ToLower().Contains(Columnvalue.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.MiddleName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.FirstGivenName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) || ((x.MiddleName ?? "").ToLower() + (x.LastFamilyName ?? "").ToLower()).Contains(Columnvalue.ToLower()) ||
                                                                        x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()) ||
                                                                        x.AlternateId != null && x.AlternateId.Contains(Columnvalue) ||
                                                                        x.HomePhone != null && x.HomePhone.Contains(Columnvalue) ||
                                                                        x.MobilePhone != null && x.MobilePhone.Contains(Columnvalue) ||
                                                                        x.PersonalEmail != null && x.PersonalEmail.Contains(Columnvalue) ||
                                                                        x.SchoolEmail != null && x.SchoolEmail.Contains(Columnvalue) ||
                                                                        x.GradeLevelTitle != null && x.GradeLevelTitle.Contains(Columnvalue) || x.HomePhone != null && x.HomePhone.Contains(Columnvalue) || x.SectionName != null && x.SectionName.Contains(Columnvalue));
                        }
                        else
                        {
                            transactionIQ = Utility.FilteredData(pageResult.FilterParams!, studentDataList).AsQueryable();
                        }
                    }
                    if (transactionIQ != null)
                    {
                        if (pageResult.SortingModel != null)
                        {
                            transactionIQ = Utility.Sort(transactionIQ, pageResult.SortingModel.SortColumn ?? "", (pageResult.SortingModel.SortDirection ?? "").ToLower());
                        }
                        else
                        {
                            transactionIQ = transactionIQ.OrderBy(s => s.LastFamilyName).ThenBy(c => c.FirstGivenName);
                        }
                        totalCount = transactionIQ.Count();


                        if (totalCount > 0)
                        {
                            if (pageResult.PageNumber > 0 && pageResult.PageSize > 0)
                            {
                                transactionIQ = transactionIQ.Skip((pageResult.PageNumber - 1) * pageResult.PageSize).Take(pageResult.PageSize);
                            }

                            studentListModel.studentListViews = transactionIQ.ToList();

                            studentListModel.studentListViews.ForEach(c =>
                            {
                                c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.CreatedBy);
                                c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, pageResult.TenantId, c.UpdatedBy);
                            });

                            studentListModel.TotalCount = totalCount;
                            studentListModel._message = "success";
                            studentListModel._failure = false;
                        }
                        else
                        {
                            studentListModel._message = NORECORDFOUND;
                            studentListModel._failure = true;
                        }
                    }
                    studentListModel.TenantId = pageResult.TenantId;
                    studentListModel.SchoolId = pageResult.SchoolId;
                    studentListModel.PageNumber = pageResult.PageNumber;
                    studentListModel._pageSize = pageResult.PageSize;
                    studentListModel._tenantName = pageResult._tenantName;
                    studentListModel._token = pageResult._token;
                }

                else
                {
                    studentListModel._message = NORECORDFOUND;
                    studentListModel._failure = true;
                }
            }
            catch (Exception es)
            {
                studentListModel._message = es.Message;
                studentListModel._failure = true;
            }
            return studentListModel;
        }
    }
}
