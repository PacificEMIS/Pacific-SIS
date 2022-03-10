using Microsoft.EntityFrameworkCore;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.report.report.data.Interface;
using opensis.report.report.data.ViewModels.ScheduleReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace opensis.report.report.data.Repository
{
    public class ScheduleReportRepository : IScheduleReportRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public ScheduleReportRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }


        /// <summary>
        /// Get Scheduled Add/Drop Report
        /// </summary>
        /// <param name="pageResult"></param>
        /// <returns></returns>
        public AddDropScheduleReport GetScheduledAddDropReport(PageResult pageResult)
        {
            AddDropScheduleReport scheduleAddDrop = new();
            IQueryable<StudentScheduleData>? transactionIQ = null;
            try
            {
                scheduleAddDrop.TenantId = pageResult.TenantId;
                scheduleAddDrop.SchoolId = pageResult.SchoolId;
                scheduleAddDrop.FromDate = pageResult.DobStartDate;
                scheduleAddDrop.ToDate = pageResult.DobEndDate;
                scheduleAddDrop._token = pageResult._token;
                scheduleAddDrop._userName = pageResult._userName;
                scheduleAddDrop._tenantName = pageResult._tenantName;
                scheduleAddDrop.PageNumber = pageResult.PageNumber;
                scheduleAddDrop._pageSize = pageResult.PageSize;

                var schoolData = this.context?.SchoolMaster.Include(x => x.SchoolDetail).FirstOrDefault(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId);
                if (schoolData != null)
                {
                    scheduleAddDrop.SchoolName = schoolData.SchoolName;
                    scheduleAddDrop.StreetAddress1 = schoolData.StreetAddress1;
                    scheduleAddDrop.StreetAddress2 = schoolData.StreetAddress2;
                    scheduleAddDrop.Country = schoolData.Country;
                    scheduleAddDrop.City = schoolData.City;
                    scheduleAddDrop.State = schoolData.State;
                    scheduleAddDrop.District = schoolData.District;
                    scheduleAddDrop.Zip = schoolData.Zip;
                    scheduleAddDrop.SchoolLogo = schoolData.SchoolDetail.FirstOrDefault().SchoolLogo;

                }
                //        var studentScheduleAddDropdata = this.context?.StudentCoursesectionSchedule.
                //Join(this.context?.StaffCoursesectionSchedule, std => std.CourseSectionId, stf => stf.CourseSectionId,
                //(std, stf) => new { std, stf }).
                //Join(this.context?.Course, r => r.std.CourseId, cs => cs.CourseId, (r, cs) => new { r, cs }).Join(this.context?.StaffMaster, s => s.r.stf.StaffId, sm => sm.StaffId, (s, sm) => new { s, sm })
                //.Where(x => x.s.r.std.TenantId == pageResult.TenantId && x.s.r.std.SchoolId == pageResult.SchoolId && x.s.r.stf.TenantId == pageResult.TenantId && x.s.r.stf.SchoolId == pageResult.SchoolId && x.s.cs.TenantId == pageResult.TenantId && x.s.cs.SchoolId == pageResult.SchoolId && x.sm.TenantId == pageResult.TenantId && x.sm.SchoolId == pageResult.SchoolId && ((x.s.r.std.EffectiveStartDate >= pageResult.DobStartDate && x.s.r.std.EffectiveStartDate <= pageResult.DobEndDate) || (x.s.r.std.EffectiveDropDate >= pageResult.DobStartDate && x.s.r.std.EffectiveDropDate <= pageResult.DobEndDate)))
                //.Select(m => new StudentScheduleData
                //{
                //    TenantId = m.s.r.std.TenantId,
                //    SchoolId = m.s.r.std.SchoolId,
                //    StudentId = m.s.r.std.StudentId,
                //    StudentGuid = m.s.r.std.StudentGuid,
                //    StudentInternalId = m.s.r.std.StudentInternalId,
                //    CourseName = m.s.cs.CourseTitle,
                //    CourseSectionName = m.s.r.std.CourseSectionName,
                //    EnrolledDate = m.s.r.std.EffectiveStartDate,
                //    DropDate = m.s.r.std.EffectiveDropDate,
                //    StaffName = m.sm.FirstGivenName + " " + (m.s.r.std.MiddleName != null ? m.s.r.std.MiddleName + " " : "") + m.sm.LastFamilyName,
                //    StudentName = m.s.r.std.FirstGivenName + " " + (m.s.r.std.MiddleName != null ? m.s.r.std.MiddleName + " " : "") + m.s.r.std.LastFamilyName,
                //});

                var studentScheduleData = this.context?.StudentCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.StaffCoursesectionSchedule).ThenInclude(x => x.StaffMaster).Include(x => x.CourseSection.Course).Where(x => x.TenantId == pageResult.TenantId && x.SchoolId == pageResult.SchoolId && ((x.EffectiveStartDate >= pageResult.MarkingPeriodStartDate && x.EffectiveStartDate <= pageResult.MarkingPeriodEndDate) || (x.EffectiveDropDate >= pageResult.MarkingPeriodStartDate && x.EffectiveDropDate <= pageResult.MarkingPeriodEndDate))).Select(m => new StudentScheduleData
                {
                    TenantId = m.TenantId,
                    SchoolId = m.SchoolId,
                    StudentId = m.StudentId,
                    StudentGuid = m.StudentGuid,
                    CourseId = m.CourseSection.Course.CourseId,
                    CourseSectionId = m.CourseSection.CourseSectionId,
                    StudentInternalId = m.StudentInternalId,
                    CourseName = m.CourseSection.Course.CourseTitle,
                    CourseSectionName = m.CourseSectionName,
                    EnrolledDate = m.EffectiveStartDate,
                    DropDate = m.EffectiveDropDate,
                    StudentName = m.FirstGivenName + " " + (m.MiddleName != null ? m.MiddleName + " " : "") + m.LastFamilyName,
                    StaffName = m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.FirstGivenName != null ? (m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.FirstGivenName + " " + (m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.MiddleName != null ? m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.MiddleName + " " : null) + m.CourseSection.StaffCoursesectionSchedule.FirstOrDefault().StaffMaster.LastFamilyName + " ") : null,
                });

                if (studentScheduleData?.Any() == true)
                {
                    if (pageResult.FilterParams == null || pageResult.FilterParams.Count == 0)
                    {
                        transactionIQ = studentScheduleData;
                    }
                    else
                    {
                        string Columnvalue = pageResult.FilterParams.ElementAt(0).FilterValue;
                        //Columnvalue = Regex.Replace(Columnvalue, @"\s+", "");

                        if (pageResult.FilterParams != null && pageResult.FilterParams.ElementAt(0).ColumnName == null && pageResult.FilterParams.Count == 1)
                        {
                            transactionIQ = studentScheduleData.Where(x => x.StudentName != null && x.StudentName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseName != null && x.CourseName.ToLower().Contains(Columnvalue.ToLower()) || x.CourseSectionName != null && x.CourseSectionName.ToLower().Contains(Columnvalue.ToLower()) || x.StaffName != null && x.StaffName.ToLower().Contains(Columnvalue.ToLower()) || x.StudentInternalId != null && x.StudentInternalId.ToLower().Contains(Columnvalue.ToLower()));
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

                        scheduleAddDrop.studentCoursesectionScheduleList = transactionIQ.ToList();
                        scheduleAddDrop.TotalCount = totalCount;
                        scheduleAddDrop._failure = false;
                    }
                    else
                    {
                        scheduleAddDrop._failure = true;
                        scheduleAddDrop.studentCoursesectionScheduleList = new();
                        scheduleAddDrop._message = NORECORDFOUND;
                    }
                }
                else
                {
                    scheduleAddDrop._failure = true;
                    scheduleAddDrop.studentCoursesectionScheduleList = new();
                    scheduleAddDrop._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                scheduleAddDrop._failure = true;
                scheduleAddDrop._message = es.Message;
            }
            return scheduleAddDrop;

        }


        /// <summary>
        /// Scheduled CourseSection List
        /// </summary>
        /// <param name="courseSectionList"></param>
        /// <returns></returns>
        public ScheduleClassList ScheduledCourseSectionList(ScheduleClassList courseSectionList)
        {
            ScheduleClassList scheduleClass = new();
            scheduleClass.TenantId = courseSectionList.TenantId;
            scheduleClass.SchoolId = courseSectionList.SchoolId;
            scheduleClass._token = courseSectionList._token;
            scheduleClass._tenantName = courseSectionList._tenantName;
            try
            {
                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == courseSectionList.TenantId && x.SchoolId == courseSectionList.SchoolId && (courseSectionList.CourseId == null || x.CourseId == courseSectionList.CourseId) && (courseSectionList.CourseSubject == null || x.CourseSubject == courseSectionList.CourseSubject));

                if (coursedata != null && coursedata.Any())
                {
                    List<AllCourseSectionView> filteredPeriodCourseData = new();
                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, PrgrsprdMarkingPeriodId = s.PrgrsprdMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, FixedPeriodId = s.FixedPeriodId, VarPeriodId = s.VarPeriodId, CalPeriodId = s.CalPeriodId, BlockPeriodId = s.BlockPeriodId, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (courseSectionList.BlockPeriodId != null)
                    {
                        filteredPeriodCourseData = distinctCourseData.Where(x => x.FixedPeriodId == courseSectionList.BlockPeriodId || x.VarPeriodId == courseSectionList.BlockPeriodId || x.CalPeriodId == courseSectionList.BlockPeriodId || x.BlockPeriodId == courseSectionList.BlockPeriodId).ToList();
                    }
                    else
                    {
                        filteredPeriodCourseData = distinctCourseData;
                    }


                    if (filteredPeriodCourseData.Any() == true)
                    {
                        List<CourseSectionForStaff> sectionList = new();
                        foreach (var CourseData in filteredPeriodCourseData)
                        {
                            List<string> staffName = new();
                            var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == courseSectionList.TenantId && x.SchoolId == courseSectionList.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.CourseId == CourseData.CourseId && (courseSectionList.StaffId == null || x.StaffId == courseSectionList.StaffId) && x.IsDropped != false).ToList();
                            CourseSectionForStaff section = new();

                            if (staffSchedule.Any() == true && courseSectionList.StaffId != null)
                            {
                                foreach (var scheduleList in staffSchedule)
                                {
                                    staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} { (scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");

                                    var totalStudent = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList().Count;

                                    section.CourseSectionName = CourseData.CourseSectionName;
                                    section.SchoolId = CourseData.SchoolId;
                                    section.TenantId = CourseData.TenantId;
                                    section.ScheduledStudentCount = totalStudent;
                                    section.CourseSectionId = CourseData.CourseSectionId;
                                    section.CourseSubject = CourseData.CourseSubject;
                                    section.CourseTitle = CourseData.CourseTitle;
                                    section.CourseProgram = CourseData.CourseProgram;
                                    section.CourseId = CourseData.CourseId;
                                    section.StaffName = string.Join(", ", staffName);
                                    sectionList.Add(section);
                                }
                            }
                            if(courseSectionList.StaffId == null)
                            {
                                foreach (var scheduleList in staffSchedule)
                                {
                                    staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} { (scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");
                                }

                                var totalStudent = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList().Count;

                                section.CourseSectionName = CourseData.CourseSectionName;
                                section.SchoolId = CourseData.SchoolId;
                                section.TenantId = CourseData.TenantId;
                                section.ScheduledStudentCount = totalStudent;
                                section.CourseSectionId = CourseData.CourseSectionId;
                                section.CourseSubject = CourseData.CourseSubject;
                                section.CourseTitle = CourseData.CourseTitle;
                                section.CourseProgram = CourseData.CourseProgram;
                                section.CourseId = CourseData.CourseId;
                                section.StaffName = string.Join(", ", staffName);
                                sectionList.Add(section);
                            }
                        }

                        foreach (var data in sectionList)
                        {
                            if (!scheduleClass.CourseSectionViewList.Any(p => p.SchoolId == data.SchoolId && p.CourseSectionId == data.CourseSectionId))
                            {
                                scheduleClass.CourseSectionViewList.Add(data);
                            }
                        }
                        //scheduleClass.CourseSectionViewList = sectionList;

                        if (scheduleClass.CourseSectionViewList.Count==0)
                        {
                            scheduleClass._failure = true;
                            scheduleClass._message = NORECORDFOUND;
                        }
                    }
                    else
                    {
                        scheduleClass._failure = true;
                        scheduleClass._message = NORECORDFOUND;
                    }
                }
                else
                {
                    scheduleClass._failure = true;
                    scheduleClass._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                scheduleClass._failure = true;
                scheduleClass._message = ex.Message;
            }

            return scheduleClass;
        }


        /// <summary>
        /// Scheduled CourseSection List
        /// </summary>
        /// <param name="courseSectionList"></param>
        /// <returns></returns>
        public StudentScheduledListModel GetStudentListByCourseSection(StudentScheduledListModel studentList)
        {
            StudentScheduledListModel studentScheduledListModel = new();

            studentScheduledListModel.TenantId = studentList.TenantId;
            studentScheduledListModel.SchoolId = studentList.SchoolId;
            studentScheduledListModel._tenantName = studentList._tenantName;
            studentScheduledListModel._token = studentList._token;
            studentScheduledListModel._tokenExpiry = studentList._tokenExpiry;
            List<CourseSectionForStaff> sectionList = new();
            int? studentCount = 0;
            foreach (var ids in studentList.courseIds)
            {
                var coursedata = this.context?.AllCourseSectionView.Where(x => x.TenantId == studentList.TenantId && x.SchoolId == studentList.SchoolId && x.CourseId == ids.CourseId && x.CourseSectionId == ids.CourseSectionId);

                if (coursedata != null && coursedata.Any())
                {
                    var distinctCourseData = coursedata.Select(s => new AllCourseSectionView { TenantId = s.TenantId, SchoolId = s.SchoolId, CourseId = s.CourseId, CourseTitle = s.CourseTitle, CourseProgram = s.CourseProgram, CourseSubject = s.CourseSubject, AcademicYear = s.AcademicYear, CourseSectionId = s.CourseSectionId, CourseSectionName = s.CourseSectionName, YrMarkingPeriodId = s.YrMarkingPeriodId, SmstrMarkingPeriodId = s.SmstrMarkingPeriodId, QtrMarkingPeriodId = s.QtrMarkingPeriodId, PrgrsprdMarkingPeriodId = s.PrgrsprdMarkingPeriodId, IsActive = s.IsActive, DurationStartDate = s.DurationStartDate, DurationEndDate = s.DurationEndDate, Seats = s.Seats, CourseGradeLevel = s.CourseGradeLevel, FixedPeriodId = s.FixedPeriodId, VarPeriodId = s.VarPeriodId, CalPeriodId = s.CalPeriodId, BlockPeriodId = s.BlockPeriodId, GradeScaleId = s.GradeScaleId, AllowStudentConflict = s.AllowStudentConflict, AllowTeacherConflict = s.AllowTeacherConflict, ScheduleType = s.ScheduleType }).Distinct().ToList();

                    if (distinctCourseData.Any() == true)
                    {
                        CourseSectionForStaff section = new();
                        foreach (var CourseData in distinctCourseData)
                        {
                            List<string> staffName = new();
                            var staffSchedule = this.context?.StaffCoursesectionSchedule.Include(x => x.StaffMaster).Where(x => x.TenantId == studentList.TenantId && x.SchoolId == studentList.SchoolId && x.CourseSectionId == CourseData.CourseSectionId && x.CourseId == CourseData.CourseId && x.IsDropped != false).ToList();

                            foreach (var scheduleList in staffSchedule)
                            {
                                staffName.Add($"{scheduleList.StaffMaster.FirstGivenName} { (scheduleList.StaffMaster.MiddleName == null ? "" : $"{scheduleList.StaffMaster.MiddleName} ")}{scheduleList.StaffMaster.LastFamilyName}");
                            }
                            List<StudentListForCoursection> studentListForCourseSectionReport = new();
                            var seduleStudents = this.context?.StudentCoursesectionSchedule.Include(s => s.StudentMaster).ThenInclude(x => x.StudentEnrollment).Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.CourseId == CourseData.CourseId && x.CourseSectionId == CourseData.CourseSectionId && x.IsDropped != true && x.StudentMaster.IsActive == true).ToList();

                            foreach (var e in seduleStudents)
                            {
                                StudentListForCoursection studentListForCoursection = new();
                                var studentView = new StudentViewForCoursection()
                                {

                                    StudentInternalId = e.StudentInternalId,
                                    StudentGuid = e.StudentGuid,
                                    AlternateId = e.AlternateId,
                                    DistrictId = e.StudentMaster.DistrictId,
                                    StateId = e.StudentMaster.StateId,
                                    AdmissionNumber = e.StudentMaster.AdmissionNumber,
                                    RollNumber = e.StudentMaster.RollNumber,
                                    Salutation = e.StudentMaster.Salutation,
                                    FirstGivenName = e.FirstGivenName,
                                    MiddleName = e.MiddleName,
                                    LastFamilyName = e.LastFamilyName,
                                    Suffix = e.StudentMaster.Suffix,
                                    PreferredName = e.StudentMaster.PreferredName,
                                    PreviousName = e.StudentMaster.PreviousName,
                                    SchoolEmail = e.StudentMaster.SchoolEmail,
                                    MobilePhone = e.StudentMaster.MobilePhone,
                                    SocialSecurityNumber = e.StudentMaster.SocialSecurityNumber,
                                    OtherGovtIssuedNumber = e.StudentMaster.OtherGovtIssuedNumber,
                                    Dob = e.StudentMaster.Dob,
                                    Gender = e.StudentMaster.Gender,
                                    Race = e.StudentMaster.Race,
                                    Ethnicity = e.StudentMaster.Ethnicity,
                                    MaritalStatus = e.StudentMaster.MaritalStatus,
                                    CountryOfBirth = e.StudentMaster.CountryOfBirth != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.CountryOfBirth))!.Name : null,
                                    Nationality = e.StudentMaster.Nationality != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.Nationality))!.Name : null,
                                    FirstLanguage = e.FirstLanguageId != null ? this.context!.Language.FirstOrDefault(x => x.LangId == Convert.ToInt32(e.FirstLanguageId))!.Locale : null,
                                    SectionId = e.StudentMaster.SectionId,
                                    SectionName = e.StudentMaster.SectionId != null ? this.context!.Sections.FirstOrDefault(x => x.SectionId == Convert.ToInt32(e.StudentMaster.SectionId) && x.SchoolId == e.StudentMaster.SchoolId).Name : null,
                                    EstimatedGradDate = e.StudentMaster.EstimatedGradDate,
                                    GradeLevelTitle = e.GradeId != null ? this.context!.Gradelevels.FirstOrDefault(x => x.GradeId == Convert.ToInt32(e.GradeId) && x.SchoolId == e.StudentMaster.SchoolId).Title : null,
                                    Twitter = e.StudentMaster.Twitter,
                                    Facebook = e.StudentMaster.Facebook,
                                    Instagram = e.StudentMaster.Instagram,
                                    Youtube = e.StudentMaster.Youtube,
                                    Linkedin = e.StudentMaster.Linkedin,
                                    HomeAddressLineOne = e.StudentMaster.HomeAddressLineOne,
                                    HomeAddressLineTwo = e.StudentMaster.HomeAddressLineTwo,
                                    HomeAddressCountry = e.StudentMaster.HomeAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.HomeAddressCountry))!.Name : null,
                                    HomeAddressCity = e.StudentMaster.HomeAddressCity,
                                    HomeAddressState = e.StudentMaster.HomeAddressState,
                                    HomeAddressZip = e.StudentMaster.HomeAddressZip,
                                    MailingAddressSameToHome = e.StudentMaster.MailingAddressSameToHome,
                                    MailingAddressLineOne = e.StudentMaster.MailingAddressLineOne,
                                    MailingAddressLineTwo = e.StudentMaster.MailingAddressLineTwo,
                                    MailingAddressCountry = e.StudentMaster.MailingAddressCountry != null ? this.context!.Country.FirstOrDefault(x => x.Id == Convert.ToInt32(e.StudentMaster.MailingAddressCountry))!.Name : null,
                                    MailingAddressCity = e.StudentMaster.MailingAddressCity,
                                    MailingAddressState = e.StudentMaster.MailingAddressState,
                                    MailingAddressZip = e.StudentMaster.MailingAddressZip,
                                    IsActive = e.StudentMaster.IsActive,
                                };

                                var customFields = this.context?.FieldsCategory.Where(x => x.TenantId == CourseData.TenantId && x.SchoolId == CourseData.SchoolId && x.Module == "Student").OrderByDescending(x => x.IsSystemCategory).ThenBy(x => x.SortOrder)
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
                                CustomFieldsValue = z.CustomFieldsValue.Where(w => w.TargetId == e.StudentId).ToList()
                            }).OrderByDescending(x => x.SystemField).ThenBy(x => x.SortOrder).ToList()
                        }).ToList();

                                studentListForCoursection.FieldsCategoryList = customFields;
                                studentListForCoursection.StudentView = studentView;
                                studentListForCourseSectionReport.Add(studentListForCoursection);
                            }

                            int? seduleStudentCount = seduleStudents != null ? seduleStudents.Count() : null;
                            var availableSeat = CourseData.Seats - seduleStudentCount;
                            section.StudentLists = studentListForCourseSectionReport;
                            studentCount = studentCount + studentListForCourseSectionReport.Count;
                            section.CourseSectionName = CourseData.CourseSectionName;
                            section.SchoolId = CourseData.SchoolId;
                            section.TenantId = CourseData.TenantId;
                            section.ScheduledStudentCount = seduleStudentCount;
                            section.CourseSectionId = CourseData.CourseSectionId;
                            section.CourseSubject = CourseData.CourseSubject;
                            section.CourseTitle = CourseData.CourseTitle;
                            section.CourseSubject = CourseData.CourseSubject;
                            section.CourseId = CourseData.CourseId;
                            section.StaffName = string.Join(", ", staffName);
                            section.AvailableSeat = availableSeat;
                            section.TotalSeats = CourseData.Seats;

                            sectionList.Add(section);
                        }
                    }
                }
            }
            studentScheduledListModel.courseSectionForStaffs = sectionList;
            studentScheduledListModel.TotalStudents = studentCount;

            return studentScheduledListModel;
        }
    }
}
