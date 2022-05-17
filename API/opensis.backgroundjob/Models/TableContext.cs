using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.backgroundjob.Models
{
    public class TableContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var tenant = "opensis";
                string connectionString = $"server=159.89.143.110;port=3306;database={tenant};user=opensisadmin;password=Opens1s@2021;default command timeout=1200";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
        public virtual DbSet<CourseSection> CourseSection { get; set; } = null!;
        public virtual DbSet<StudentCoursesectionSchedule> StudentCoursesectionSchedule { get; set; } = null!;
        public virtual DbSet<ScheduledJob> ScheduledJobs { get; set; } = null!;
        public virtual DbSet<ScheduledJobHistory> ScheduledJobHistories { get; set; } = null!;
        public virtual DbSet<StudentMaster> StudentMaster { get; set; } = null!;
        public virtual DbSet<StudentEnrollment> StudentEnrollment { get; set; } = null!;
        public virtual DbSet<StudentEnrollmentCode> StudentEnrollmentCode { get; set; } = null!;
        public virtual DbSet<Membership> Membership { get; set; } = null!;
        public virtual DbSet<ParentAssociationship> ParentAssociationship { get; set; } = null!;
        public virtual DbSet<UserMaster> UserMaster { get; set; } = null!;
        public virtual DbSet<StaffCoursesectionSchedule> StaffCoursesectionSchedule { get; set; } = null!;
        public virtual DbSet<StudentAttendance> StudentAttendance { get; set; } = null!;
        public virtual DbSet<StudentMissingAttendance> StudentMissingAttendances { get; set; } = null!;
        public virtual DbSet<AllCourseSectionView> AllCourseSectionView { get; set; } = null!;
        public virtual DbSet<CalendarEvents> CalendarEvents { get; set; } = null!;
        public virtual DbSet<BlockPeriod> BlockPeriod { get; set; } = null!;
        public virtual DbSet<BellSchedule> BellSchedule { get; set; } = null!;
        public virtual DbSet<SchoolMaster> SchoolMaster { get; set; } = null!;
        public virtual DbSet<SchoolCalendars> SchoolCalendars { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<ScheduledJob>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.JobId });

                entity.ToTable("scheduled_job");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.ApiTitle)
                    .HasMaxLength(250)
                    .HasColumnName("api_title");

                entity.Property(e => e.ControllerPath)
                    .HasMaxLength(500)
                    .HasColumnName("controller_path");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.JobScheduleDate)
                    .HasColumnType("date")
                    .HasColumnName("job_schedule_date");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(150)
                    .HasColumnName("job_title");

                entity.Property(e => e.LastRunStatus).HasColumnName("last_run_status");

                entity.Property(e => e.LastRunTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_run_time");

                entity.Property(e => e.TaskJson).HasColumnName("task_json");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<ScheduledJobHistory>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.JobId, e.JobRunId });

                entity.ToTable("scheduled_job_history");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.JobId).HasColumnName("job_id");

                entity.Property(e => e.JobRunId).HasColumnName("job_run_id");

                entity.Property(e => e.JobStatus).HasColumnName("job_status");

                entity.Property(e => e.RunTime)
                    .HasColumnType("datetime")
                    .HasColumnName("run_time");

                entity.Property(e => e.ScheduledDate)
                    .HasColumnType("date")
                    .HasColumnName("scheduled_date");

                entity.HasOne(d => d.ScheduledJob)
                    .WithMany(p => p.ScheduledJobHistories)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.JobId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_job_history_job");
            });

            modelBuilder.Entity<StudentCoursesectionSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_student_coursesection_schedule_tenant_id");

                entity.ToTable("student_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId }, "IX_student_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId }, "IX_student_coursesection_schedule_1");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EffectiveDropDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_drop_date");

                entity.Property(e => e.EffectiveStartDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_start_date");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguageId).HasColumnName("first_language_id");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.IsDropped).HasColumnName("is_dropped");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .IsFixedLength();

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                //entity.HasOne(d => d.SchoolMaster)
                //    .WithMany(p => p.StudentCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("sch_mast_std_cs_sch");


                entity.HasOne(d => d.StudentMaster)
                    .WithMany(p => p.StudentCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_mast_cs_sch");


                entity.HasOne(d => d.CourseSection)
                    .WithMany(p => p.StudentCoursesectionSchedule)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("std_cs_cs_sch");
            });

            modelBuilder.Entity<CourseSection>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_course_section_tenant_id");

                entity.ToTable("course_section");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AffectsClassRank).HasColumnName("affects_class_rank");

                entity.Property(e => e.AffectsHonorRoll).HasColumnName("affects_honor_roll");

                entity.Property(e => e.AllowStudentConflict).HasColumnName("allow_student_conflict");

                entity.Property(e => e.AllowTeacherConflict).HasColumnName("allow_teacher_conflict");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceTaken).HasColumnName("attendance_taken");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CreditHours)
                    .HasColumnType("decimal(8, 3)")
                    .HasColumnName("credit_hours");

                entity.Property(e => e.DurationBasedOnPeriod).HasColumnName("duration_based_on_period");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.GradeScaleType)
                    .HasMaxLength(13)
                    .HasColumnName("grade_scale_type");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsWeightedCourse).HasColumnName("is_weighted_course");

                entity.Property(e => e.MeetingDays)
                    .HasMaxLength(100)
                    .HasColumnName("meeting_days");

                entity.Property(e => e.OnlineClassRoom).HasColumnName("online_class_room");

                entity.Property(e => e.OnlineClassroomPassword)
                    .HasMaxLength(20)
                    .HasColumnName("online_classroom_password");

                entity.Property(e => e.OnlineClassroomUrl)
                    .HasMaxLength(250)
                    .HasColumnName("online_classroom_url");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(25)
                    .HasColumnName("schedule_type");

                entity.Property(e => e.Seats).HasColumnName("seats");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.StandardGradeScaleId).HasColumnName("standard_grade_scale_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UseStandards).HasColumnName("use_standards");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");


            });

            modelBuilder.Entity<StudentMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId })
                    .HasName("PK_student_master_tenant_id");

                entity.ToTable("student_master");

                //entity.HasIndex(e => e.FirstLanguageId, "IX_student_master_first_language_id");

                //entity.HasIndex(e => e.SecondLanguageId, "IX_student_master_second_language_id");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SectionId }, "IX_student_master_tenant_id_school_id_section_id");

                //entity.HasIndex(e => e.ThirdLanguageId, "IX_student_master_third_language_id");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "student_master$AK_student_master_tenant_id_school_")
                //    .IsUnique();

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "student_master$IX_student_master")
                  //  .IsUnique();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.AdmissionNumber)
                    .HasMaxLength(50)
                    .HasColumnName("admission_number");

                entity.Property(e => e.AlertDescription).HasColumnName("alert_description");

                entity.Property(e => e.AlternateId)
                    .HasMaxLength(50)
                    .HasColumnName("alternate_id");

                entity.Property(e => e.Associationship).HasColumnName("associationship");

                entity.Property(e => e.BusNo)
                    .HasMaxLength(15)
                    .HasColumnName("bus_no");

                entity.Property(e => e.CountryOfBirth).HasColumnName("country_of_birth");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CriticalAlert)
                    .HasMaxLength(200)
                    .HasColumnName("critical_alert");

                entity.Property(e => e.Dentist)
                    .HasMaxLength(100)
                    .HasColumnName("dentist");

                entity.Property(e => e.DentistPhone)
                    .HasMaxLength(50)
                    .HasColumnName("dentist_phone");

                entity.Property(e => e.DistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("district_id");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.EconomicDisadvantage).HasColumnName("economic_disadvantage");

                entity.Property(e => e.Eligibility504).HasColumnName("eligibility_504");

                entity.Property(e => e.EnrollmentType)
                    .HasMaxLength(8)
                    .HasColumnName("enrollment_type")
                    .IsFixedLength();

                entity.Property(e => e.EstimatedGradDate)
                    .HasColumnType("date")
                    .HasColumnName("estimated_grad_date");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.Facebook).HasColumnName("facebook");

                entity.Property(e => e.FirstGivenName)
                    .HasMaxLength(50)
                    .HasColumnName("first_given_name");

                entity.Property(e => e.FirstLanguageId).HasColumnName("first_language_id");

                entity.Property(e => e.FreeLunchEligibility).HasColumnName("free_lunch_eligibility");

                entity.Property(e => e.Gender)
                    .HasMaxLength(6)
                    .HasColumnName("gender");

                entity.Property(e => e.HomeAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_city");

                entity.Property(e => e.HomeAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_country");

                entity.Property(e => e.HomeAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_one");

                entity.Property(e => e.HomeAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("home_address_line_two");

                entity.Property(e => e.HomeAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("home_address_state");

                entity.Property(e => e.HomeAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("home_address_zip");

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(30)
                    .HasColumnName("home_phone");

                entity.Property(e => e.Instagram).HasColumnName("instagram");

                entity.Property(e => e.InsuranceCompany)
                    .HasMaxLength(200)
                    .HasColumnName("insurance_company");

                entity.Property(e => e.InsuranceCompanyPhone)
                    .HasMaxLength(50)
                    .HasColumnName("insurance_company_phone");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastFamilyName)
                    .HasMaxLength(50)
                    .HasColumnName("last_family_name");

                entity.Property(e => e.LepIndicator).HasColumnName("lep_indicator");

                entity.Property(e => e.Linkedin).HasColumnName("linkedin");

                entity.Property(e => e.MailingAddressCity)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_city");

                entity.Property(e => e.MailingAddressCountry)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_country");

                entity.Property(e => e.MailingAddressLineOne)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_one");

                entity.Property(e => e.MailingAddressLineTwo)
                    .HasMaxLength(200)
                    .HasColumnName("mailing_address_line_two");

                entity.Property(e => e.MailingAddressSameToHome).HasColumnName("mailing_address_same_to_home");

                entity.Property(e => e.MailingAddressState)
                    .HasMaxLength(50)
                    .HasColumnName("mailing_address_state");

                entity.Property(e => e.MailingAddressZip)
                    .HasMaxLength(20)
                    .HasColumnName("mailing_address_zip");

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(10)
                    .HasColumnName("marital_status");

                entity.Property(e => e.MedicalFacility)
                    .HasMaxLength(100)
                    .HasColumnName("medical_facility");

                entity.Property(e => e.MedicalFacilityPhone)
                    .HasMaxLength(50)
                    .HasColumnName("medical_facility_phone");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("middle_name");

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(30)
                    .HasColumnName("mobile_phone");

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.OtherGovtIssuedNumber)
                    .HasMaxLength(50)
                    .HasColumnName("other_govt_issued_number");

                entity.Property(e => e.PersonalEmail).HasColumnName("personal_email");

                entity.Property(e => e.PolicyHolder)
                    .HasMaxLength(100)
                    .HasColumnName("policy_holder");

                entity.Property(e => e.PolicyNumber)
                    .HasMaxLength(50)
                    .HasColumnName("policy_number");

                entity.Property(e => e.PreferredName)
                    .HasMaxLength(50)
                    .HasColumnName("preferred_name");

                entity.Property(e => e.PreviousName)
                    .HasMaxLength(50)
                    .HasColumnName("previous_name");

                entity.Property(e => e.PrimaryCarePhysician)
                    .HasMaxLength(200)
                    .HasColumnName("primary_care_physician");

                entity.Property(e => e.PrimaryCarePhysicianPhone)
                    .HasMaxLength(50)
                    .HasColumnName("primary_care_physician_phone");

                entity.Property(e => e.Race)
                    .HasMaxLength(50)
                    .HasColumnName("race");

                entity.Property(e => e.RollNumber)
                    .HasMaxLength(50)
                    .HasColumnName("roll_number");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(50)
                    .HasColumnName("salutation");

                entity.Property(e => e.SchoolBusDropOff).HasColumnName("school_bus_drop_off");

                entity.Property(e => e.SchoolBusPickUp).HasColumnName("school_bus_pick_up");

                entity.Property(e => e.SchoolEmail).HasColumnName("school_email");

                entity.Property(e => e.SecondLanguageId).HasColumnName("second_language_id");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.SocialSecurityNumber)
                    .HasMaxLength(50)
                    .HasColumnName("social_security_number");

                entity.Property(e => e.SpecialEducationIndicator).HasColumnName("special_education_indicator");

                entity.Property(e => e.StateId)
                    .HasMaxLength(50)
                    .HasColumnName("state_id");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.StudentInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_internal_id");

                entity.Property(e => e.StudentPhoto).HasColumnName("student_photo");

                entity.Property(e => e.StudentThumbnailPhoto).HasColumnName("student_thumbnail_photo");

                entity.Property(e => e.StudentPortalId)
                    .HasMaxLength(50)
                    .HasColumnName("student_portal_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(50)
                    .HasColumnName("suffix");

                entity.Property(e => e.ThirdLanguageId).HasColumnName("third_language_id");

                entity.Property(e => e.Twitter).HasColumnName("twitter");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Vision)
                    .HasMaxLength(100)
                    .HasColumnName("vision");

                entity.Property(e => e.VisionPhone)
                    .HasMaxLength(50)
                    .HasColumnName("vision_phone");

                entity.Property(e => e.Youtube).HasColumnName("youtube");

                //entity.HasOne(d => d.FirstLanguage)
                //    .WithMany(p => p.StudentMasterFirstLanguage)
                //    .HasForeignKey(d => d.FirstLanguageId)
                //    .HasConstraintName("student_master$FK_student_master_language");

                //entity.HasOne(d => d.SecondLanguage)
                //    .WithMany(p => p.StudentMasterSecondLanguage)
                //    .HasForeignKey(d => d.SecondLanguageId)
                //    .HasConstraintName("student_master$FK_student_master_language1");

                //entity.HasOne(d => d.ThirdLanguage)
                //    .WithMany(p => p.StudentMasterThirdLanguage)
                //    .HasForeignKey(d => d.ThirdLanguageId)
                //    .HasConstraintName("student_master$FK_student_master_language2");

                //entity.HasOne(d => d.SchoolMaster)
                //    .WithMany(p => p.StudentMaster)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("student_master$FK_student_master_school_master");

                //entity.HasOne(d => d.Sections)
                //    .WithMany(p => p.StudentMaster)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SectionId })
                //    .HasConstraintName("student_master$FK_student_master_sections");
            });

            modelBuilder.Entity<StudentEnrollment>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.EnrollmentId })
                    .HasName("PK_student_enrollment_tenant_id");

                entity.ToTable("student_enrollment");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.GradeId }, "IX_student_enrollment_tenant_id_school_id_grade_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentGuid }, "IX_student_enrollment_tenant_id_school_id_student_");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");

                entity.Property(e => e.CalenderId).HasColumnName("calender_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.EnrollmentCode)
                    .HasMaxLength(50)
                    .HasColumnName("enrollment_code");

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnType("date")
                    .HasColumnName("enrollment_date");

                entity.Property(e => e.ExitCode)
                    .HasMaxLength(50)
                    .HasColumnName("exit_code");

                entity.Property(e => e.ExitDate)
                    .HasColumnType("date")
                    .HasColumnName("exit_date");

                entity.Property(e => e.GradeId).HasColumnName("grade_id");

                entity.Property(e => e.GradeLevelTitle)
                    .HasMaxLength(50)
                    .HasColumnName("grade_level_title");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.RollingOption)
                    .HasMaxLength(50)
                    .HasColumnName("rolling_option");

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(200)
                    .HasColumnName("school_name");

                entity.Property(e => e.SchoolTransferred)
                    .HasMaxLength(200)
                    .HasColumnName("school_transferred");

                entity.Property(e => e.StudentGuid)
                    .HasMaxLength(36)
                    .HasColumnName("student_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.TransferredGrade)
                    .HasMaxLength(50)
                    .HasColumnName("transferred_grade");

                entity.Property(e => e.TransferredSchoolId).HasColumnName("transferred_school_id");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                //entity.HasOne(d => d.Gradelevels)
                //    .WithMany(p => p.StudentEnrollment)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.GradeId })
                //    .HasConstraintName("student_enrollment$FK_gradelevels");
            });

            modelBuilder.Entity<StudentEnrollmentCode>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EnrollmentCode })
                    .HasName("PK_student_enrollment_code_tenant_id");

                entity.ToTable("student_enrollment_code");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EnrollmentCode).HasColumnName("enrollment_code");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name")
                    .IsFixedLength();

                entity.Property(e => e.SortOrder).HasColumnName("sort_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");


            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MembershipId })
                    .HasName("PK_membership_tenant_id");

                entity.ToTable("membership");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsSuperadmin).HasColumnName("is_superadmin");

                entity.Property(e => e.IsSystem).HasColumnName("is_system");

                entity.Property(e => e.Profile)
                    .HasMaxLength(30)
                    .HasColumnName("profile");

                entity.Property(e => e.ProfileType)
                    .HasMaxLength(30)
                    .HasColumnName("profile_type");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

            });

            modelBuilder.Entity<ParentAssociationship>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.ParentId, e.StudentId })
                    .HasName("PK_parent_associationship_tenant_id");

                entity.ToTable("parent_associationship");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Associationship).HasColumnName("associationship");

                entity.Property(e => e.ContactType)
                    .HasMaxLength(9)
                    .HasColumnName("contact_type");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.IsCustodian).HasColumnName("is_custodian");

                entity.Property(e => e.Relationship)
                    .HasMaxLength(30)
                    .HasColumnName("relationship");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.EmailAddress })
                    .HasName("PK_user_master_tenant_id");

                entity.ToTable("user_master");

                //entity.HasIndex(e => e.LangId, "IX_user_master_lang_id");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_user_master_tenant_id_school_id_membership_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(150)
                    .HasColumnName("emailaddress");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsTenantadmin).HasColumnName("is_tenantadmin");

                entity.Property(e => e.LangId)
                    .HasColumnName("lang_id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastUsedSchoolId).HasColumnName("last_used_school_id");

                entity.Property(e => e.LoginAttemptDate)
                    .HasColumnType("date")
                    .HasColumnName("login_attempt_date");

                entity.Property(e => e.LoginFailureCount).HasColumnName("login_failure_count");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .HasColumnName("passwordhash");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_master$FK_user_master_membership");
            });

            modelBuilder.Entity<StaffCoursesectionSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StaffId, e.CourseId, e.CourseSectionId })
                    .HasName("PK_staff_coursesection_schedule_tenant_id");

                entity.ToTable("staff_coursesection_schedule");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseId, e.CourseSectionId }, "IX_staff_coursesection_schedule_tenant_id_school_i");

                entity.HasIndex(e => new { e.TenantId, e.StaffId }, "IX_staff_coursesection_schedule_tenant_id_staff_id");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.QtrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_1");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.SmstrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_2");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.YrMarkingPeriodId }, "staff_coursesection_schedule_tenant_id_3");


                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.EffectiveDropDate)
                    .HasPrecision(0)
                    .HasColumnName("effective_drop_date");

                entity.Property(e => e.IsAssigned).HasColumnName("is_assigned");

                entity.Property(e => e.IsDropped).HasColumnName("is_dropped");

                entity.Property(e => e.MeetingDays)
                    .HasMaxLength(100)
                    .HasColumnName("meeting_days");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.StaffGuid)
                    .HasMaxLength(36)
                    .HasColumnName("staff_guid")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                //entity.HasOne(d => d.StaffMaster)
                //    .WithMany(p => p.StaffCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("staff_coursesection_schedule$FK_staff_master");

                //entity.HasOne(d => d.Quarter)
                //    .WithMany(p => p.StaffCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.QtrMarkingPeriodId })
                //    .HasConstraintName("staff_coursesection_schedule$FK_quarters");

                //entity.HasOne(d => d.Semester)
                //    .WithMany(p => p.StaffCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.SmstrMarkingPeriodId })
                //   .HasConstraintName("staff_coursesection_schedule$FK_semesters");

                //entity.HasOne(d => d.SchoolYear)
                //    .WithMany(p => p.StaffCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.YrMarkingPeriodId })
                //    .HasConstraintName("staff_coursesection_schedule$FK_years");

                //entity.HasOne(d => d.CourseSection)
                //    .WithMany(p => p.StaffCoursesectionSchedule)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.CourseId, d.CourseSectionId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("staff_coursesection_schedule$FK_section");

                //entity.HasOne(d => d.ProgressPeriod)
                //         .WithMany(p => p.StaffCoursesectionSchedules)
                //         .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PrgrsprdMarkingPeriodId })
                //         .HasConstraintName("FK_staff_cs_sch_progress_periods");
            });

            modelBuilder.Entity<StudentAttendance>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StaffId, e.CourseId, e.CourseSectionId, e.AttendanceDate, e.BlockId, e.PeriodId })
                    .HasName("PK_student_attendance_tenant_id");

                entity.ToTable("student_attendance");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId }, "IX_student_attendance");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.CourseSectionId }, "IX_student_attendance_1");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StaffId }, "IX_student_attendance_2");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.AttendanceDate }, "IX_student_attendance_3");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.AttendanceCategoryId, e.AttendanceCode }, "IX_student_attendance_tenant_id_school_id_attendan");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId }, "IX_student_attendance_tenant_id_school_id_block_id");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.MembershipId }, "IX_student_attendance_tenant_id_school_id_membersh");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StaffId, e.CourseId, e.CourseSectionId }, "IX_student_attendance_tenant_id_school_id_staff_id");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.CourseId, e.CourseSectionId }, "IX_student_attendance_tenant_id_school_id_student_");

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentAttendanceId }, "student_attendance$AK_student_attendance_tenant_id")
                //    .IsUnique();

                //entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.StudentId, e.StudentAttendanceId }, "student_attendance$student_attendance_id_idx")
                //    .IsUnique();

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.AttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("attendance_date");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode).HasColumnName("attendance_code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MembershipId).HasColumnName("membership_id");

                entity.Property(e => e.StudentAttendanceId).HasColumnName("student_attendance_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.Membership)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.MembershipId })
                    .HasConstraintName("student_attendance$FK_membership");

                //entity.HasOne(d => d.AttendanceCodeNavigation)
                //    .WithMany(p => p.StudentAttendance)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.AttendanceCategoryId, d.AttendanceCode })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("student_attendance$FK_code");

                //entity.HasOne(d => d.BlockPeriod)
                //    .WithMany(p => p.StudentAttendance)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("student_attendance$FKperiod");

                entity.HasOne(d => d.StaffCoursesectionSchedule)
                    .WithMany(p => p.StudentAttendance)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StaffId, d.CourseId, d.CourseSectionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("student_attendance$FK_staff_cs_sch");

                //entity.HasOne(d => d.StudentCoursesectionSchedule)
                //    .WithMany(p => p.StudentAttendance)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StudentId, d.CourseId, d.CourseSectionId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("student_attd_coursesec_sch");
            });

            modelBuilder.Entity<StudentMissingAttendance>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.MissingAttendanceId })
                    .HasName("PK_student_missing_attendance_1");

                entity.ToTable("student_missing_attendance");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.MissingAttendanceId).HasColumnName("missing_attendance_id");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceCode).HasColumnName("attendance_code");

                entity.Property(e => e.BlockId)
                    .HasColumnName("block_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CourseId)
                    .HasColumnName("course_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CourseSectionId)
                    .HasColumnName("course_section_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.MissingAttendanceDate)
                    .HasColumnType("date")
                    .HasColumnName("missing_attendance_date")
                    .HasDefaultValueSql("('0001-01-01')");

                entity.Property(e => e.PeriodId)
                    .HasColumnName("period_id")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                //entity.HasOne(d => d.StaffMaster)
                //    .WithMany(p => p.StudentMissingAttendances)
                //    .HasForeignKey(d => new { d.TenantId, d.StaffId })
                //    .HasConstraintName("FK_missing_attendance_staff");

                entity.HasOne(d => d.BlockPeriod)
                    .WithMany(p => p.StudentMissingAttendances)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.BlockId, d.PeriodId })
                    .HasConstraintName("FK_missing_attendance_block_period");

                entity.HasOne(d => d.StaffCoursesectionSchedule)
                    .WithMany(p => p.StudentMissingAttendances)
                    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.StaffId, d.CourseId, d.CourseSectionId })
                    .HasConstraintName("FK_missing_attendance_cs");
            });
     
            modelBuilder.Entity<AllCourseSectionView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("all_course_section_view");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.AllowStudentConflict).HasColumnName("allow_student_conflict");

                entity.Property(e => e.AllowTeacherConflict).HasColumnName("allow_teacher_conflict");

                entity.Property(e => e.AttendanceCategoryId).HasColumnName("attendance_category_id");

                entity.Property(e => e.AttendanceTaken).HasColumnName("attendance_taken");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.BlockPeriodId).HasColumnName("block_period_id");

                entity.Property(e => e.BlockRoomId).HasColumnName("block_room_id");

                entity.Property(e => e.CalDate)
                    .HasColumnType("date")
                    .HasColumnName("cal_date");

                entity.Property(e => e.CalDay)
                    .HasMaxLength(30)
                    .HasColumnName("cal_day");

                entity.Property(e => e.CalPeriodId).HasColumnName("cal_period_id");

                entity.Property(e => e.CalRoomId).HasColumnName("cal_room_id");

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.CourseGradeLevel)
                    .HasMaxLength(50)
                    .HasColumnName("course_grade_level");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseProgram)
                    .HasMaxLength(100)
                    .HasColumnName("course_program");

                entity.Property(e => e.CourseSectionId).HasColumnName("course_section_id");

                entity.Property(e => e.CourseSectionName)
                    .HasMaxLength(200)
                    .HasColumnName("course_section_name");

                entity.Property(e => e.CourseSubject)
                    .HasMaxLength(100)
                    .HasColumnName("course_subject");

                entity.Property(e => e.CourseTitle).HasColumnName("course_title");

                entity.Property(e => e.DurationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_end_date");

                entity.Property(e => e.DurationStartDate)
                    .HasColumnType("date")
                    .HasColumnName("duration_start_date");

                entity.Property(e => e.FixedDays)
                    .HasMaxLength(100)
                    .HasColumnName("fixed_days");

                entity.Property(e => e.FixedPeriodId).HasColumnName("fixed_period_id");

                entity.Property(e => e.FixedRoomId).HasColumnName("fixed_room_id");

                entity.Property(e => e.GradeScaleId).HasColumnName("grade_scale_id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.QtrMarkingPeriodId).HasColumnName("qtr_marking_period_id");

                entity.Property(e => e.ScheduleType)
                    .HasMaxLength(25)
                    .HasColumnName("schedule_type");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.Seats).HasColumnName("seats");

                entity.Property(e => e.SmstrMarkingPeriodId).HasColumnName("smstr_marking_period_id");

                entity.Property(e => e.TakeAttendanceBlock).HasColumnName("take_attendance_block");

                entity.Property(e => e.TakeAttendanceCalendar).HasColumnName("take_attendance_calendar");

                entity.Property(e => e.TakeAttendanceVariable).HasColumnName("take_attendance_variable");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.VarDay)
                    .HasMaxLength(15)
                    .HasColumnName("var_day");

                entity.Property(e => e.VarPeriodId).HasColumnName("var_period_id");

                entity.Property(e => e.VarRoomId).HasColumnName("var_room_id");

                entity.Property(e => e.YrMarkingPeriodId).HasColumnName("yr_marking_period_id");

                entity.Property(e => e.PrgrsprdMarkingPeriodId).HasColumnName("prgrsprd_marking_period_id");
            });

            modelBuilder.Entity<CalendarEvents>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.CalendarId, e.EventId })
                    .HasName("PK_calendar_events_tenant_id");

                entity.ToTable("calendar_events");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.CalendarId).HasColumnName("calendar_id");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.ApplicableToAllSchool).HasColumnName("applicable_to_all_school");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.EventColor)
                    .HasMaxLength(7)
                    .HasColumnName("event_color");

                entity.Property(e => e.IsHoliday).HasColumnName("is_holiday");

                entity.Property(e => e.SchoolDate)
                    .HasColumnType("date")
                    .HasColumnName("school_date");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.SystemWideEvent).HasColumnName("system_wide_event");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.VisibleToMembershipId)
                    .HasMaxLength(30)
                    .HasColumnName("visible_to_membership_id");
            });

            modelBuilder.Entity<BlockPeriod>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.BlockId, e.PeriodId })
                    .HasName("PK_block_period_tenant_id");

                entity.ToTable("block_period");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.PeriodId).HasColumnName("period_id");
                entity.Property(e => e.AcademicYear)
                   .HasColumnType("decimal(4, 0)")
                   .HasColumnName("academic_year");

                entity.Property(e => e.CalculateAttendance).HasColumnName("calculate_attendance");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.PeriodEndTime)
                    .HasMaxLength(8)
                    .HasColumnName("period_end_time")
                    .IsFixedLength();

                entity.Property(e => e.PeriodShortName)
                    .HasMaxLength(50)
                    .HasColumnName("period_short_name");

                entity.Property(e => e.PeriodSortOrder).HasColumnName("period_sort_order");

                entity.Property(e => e.PeriodStartTime)
                    .HasMaxLength(8)
                    .HasColumnName("period_start_time")
                    .IsFixedLength();

                entity.Property(e => e.PeriodTitle)
                    .HasMaxLength(200)
                    .HasColumnName("period_title");
                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                //entity.HasOne(d => d.SchoolMaster)
                //    .WithMany(p => p.BlockPeriod)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("block_period$FK_block_period_school_master");
            });

            modelBuilder.Entity<BellSchedule>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.AcademicYear, e.BellScheduleDate })
                    .HasName("PK_bell_schedule_tenant_id");

                entity.ToTable("bell_schedule");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.BellScheduleDate)
                    .HasColumnType("date")
                    .HasColumnName("bell_schedule_date");

                entity.Property(e => e.BlockId).HasColumnName("block_id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<SchoolMaster>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId })
                    .HasName("PK_school_master_tenant_id");

                entity.ToTable("school_master");

                entity.HasIndex(e => new { e.TenantId, e.SchoolId, e.PlanId }, "IX_school_master_tenant_id_school_id_plan_id");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.AlternateName)
                    .HasMaxLength(100)
                    .HasColumnName("alternate_name");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("city")
                    .IsFixedLength();

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .HasColumnName("country")
                    .IsFixedLength();

                entity.Property(e => e.County)
                    .HasMaxLength(50)
                    .HasColumnName("county")
                    .IsFixedLength();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.CurrentPeriodEnds)
                    .HasPrecision(0)
                    .HasColumnName("current_period_ends");

                entity.Property(e => e.District)
                    .HasMaxLength(50)
                    .HasColumnName("district")
                    .IsFixedLength();

                entity.Property(e => e.Division)
                    .HasMaxLength(50)
                    .HasColumnName("division")
                    .IsFixedLength();

                entity.Property(e => e.Features).HasColumnName("features");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.MaxApiChecks).HasColumnName("max_api_checks");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.SchoolAltId)
                    .HasMaxLength(50)
                    .HasColumnName("school_alt_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolClassification)
                    .HasMaxLength(50)
                    .HasColumnName("school_classification")
                    .IsFixedLength();

                entity.Property(e => e.SchoolDistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("school_district_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolGuid)
                    .HasMaxLength(36)
                    .HasColumnName("school_guid")
                    .HasDefaultValueSql("(N'00000000-0000-0000-0000-000000000000')")
                    .IsFixedLength();

                entity.Property(e => e.SchoolInternalId)
                    .HasMaxLength(50)
                    .HasColumnName("school_internal_id");

                entity.Property(e => e.SchoolLevel)
                    .HasMaxLength(50)
                    .HasColumnName("school_level")
                    .IsFixedLength();

                entity.Property(e => e.SchoolName)
                    .HasMaxLength(100)
                    .HasColumnName("school_name");

                entity.Property(e => e.SchoolStateId)
                    .HasMaxLength(50)
                    .HasColumnName("school_state_id")
                    .IsFixedLength();

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .HasColumnName("state")
                    .IsFixedLength();

                entity.Property(e => e.StreetAddress1)
                    .HasMaxLength(150)
                    .HasColumnName("street_address_1");

                entity.Property(e => e.StreetAddress2)
                    .HasMaxLength(150)
                    .HasColumnName("street_address_2");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.Zip)
                    .HasMaxLength(20)
                    .HasColumnName("zip")
                    .IsFixedLength();

                //entity.HasOne(d => d.Plans)
                //    .WithMany(p => p.SchoolMaster)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId, d.PlanId })
                //    .HasConstraintName("school_master$FK_school_master_plans");
            });

            modelBuilder.Entity<SchoolCalendars>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SchoolId, e.CalenderId })
                    .HasName("PK_school_calendars_tenant_id");

                entity.ToTable("school_calendars");

                entity.Property(e => e.TenantId)
                    .HasMaxLength(36)
                    .HasColumnName("tenant_id")
                    .IsFixedLength();

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.CalenderId).HasColumnName("calender_id");

                entity.Property(e => e.AcademicYear)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("academic_year");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedOn)
                    .HasPrecision(0)
                    .HasColumnName("created_on");

                entity.Property(e => e.Days)
                    .HasMaxLength(7)
                    .HasColumnName("days");

                entity.Property(e => e.DefaultCalender).HasColumnName("default_calender");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.RolloverId).HasColumnName("rollover_id");

                entity.Property(e => e.SessionCalendar).HasColumnName("session_calendar");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(150)
                    .HasColumnName("updated_by");

                entity.Property(e => e.UpdatedOn)
                    .HasPrecision(0)
                    .HasColumnName("updated_on");

                entity.Property(e => e.VisibleToMembershipId)
                    .HasMaxLength(50)
                    .HasColumnName("visible_to_membership_id");

                //entity.HasOne(d => d.SchoolMaster)
                //    .WithMany(p => p.SchoolCalendars)
                //    .HasForeignKey(d => new { d.TenantId, d.SchoolId })
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("school_calendars$FK_school_calendars_school_master");
            });
        }
    }
}
