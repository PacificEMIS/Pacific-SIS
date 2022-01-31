using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class InitialMigrationForViews_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView =
                  @"
                    CREATE VIEW [dbo].[all_course_section_view]
                    AS
                    SELECT dbo.course.tenant_id, dbo.course.school_id, dbo.course.course_id, dbo.course.course_title, dbo.course.course_subject, dbo.course.course_program, dbo.course_section.academic_year, dbo.course_section.course_section_id, 
                                             dbo.course_section.course_section_name, dbo.course_section.seats, dbo.course_section.duration_start_date, dbo.course_section.duration_end_date, dbo.course_section.yr_marking_period_id, 
                                             dbo.course_section.qtr_marking_period_id, dbo.course_section.smstr_marking_period_id, dbo.course_section.schedule_type, dbo.course_section.meeting_days AS fixed_days, 
                                             dbo.course_fixed_schedule.room_id AS fixed_room_id, dbo.course_fixed_schedule.period_id AS fixed_period_id, dbo.course_variable_schedule.day AS var_day, dbo.course_variable_schedule.period_id AS var_period_id, 
                                             dbo.course_variable_schedule.room_id AS var_room_id, dbo.course_calendar_schedule.date AS cal_date, dbo.course_calendar_schedule.period_id AS cal_period_id, dbo.course_calendar_schedule.room_id AS cal_room_id, 
                                             dbo.course_block_schedule.period_id AS block_period_id, dbo.course_block_schedule.room_id AS block_room_id, dbo.course_section.is_active, dbo.course.course_grade_level, dbo.course_section.grade_scale_id, 
                                             dbo.course_section.allow_teacher_conflict, dbo.course_section.allow_student_conflict, dbo.course_section.calendar_id, dbo.course_section.attendance_taken, DATENAME(DW, dbo.course_calendar_schedule.date) AS cal_day, 
                                             dbo.course_variable_schedule.take_attendance AS take_attendance_variable, dbo.course_calendar_schedule.take_attendance AS take_attendance_calendar, 
                                             dbo.course_block_schedule.take_attendance AS take_attendeace_block, dbo.course_block_schedule.block_id, dbo.course_section.attendance_category_id
                    FROM  dbo.course INNER JOIN
                                             dbo.course_section ON dbo.course.tenant_id = dbo.course_section.tenant_id AND dbo.course.school_id = dbo.course_section.school_id AND dbo.course.course_id = dbo.course_section.course_id LEFT OUTER JOIN
                                             dbo.course_fixed_schedule ON dbo.course_section.tenant_id = dbo.course_fixed_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_fixed_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_fixed_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_fixed_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_variable_schedule ON dbo.course_section.tenant_id = dbo.course_variable_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_variable_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_variable_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_variable_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_calendar_schedule ON dbo.course_section.tenant_id = dbo.course_calendar_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_calendar_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_calendar_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_calendar_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_block_schedule ON dbo.course_section.tenant_id = dbo.course_block_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_block_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_block_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_block_schedule.course_section_id
                    GO";

            migrationBuilder.Sql(allCourseSectionView);

            string studentListView =
                  @"
                    CREATE VIEW [dbo].[student_list_view]
                    AS
                    SELECT dbo.student_master.tenant_id, dbo.student_master.school_id, dbo.student_master.student_id, dbo.student_master.alternate_id, dbo.student_master.district_id, dbo.student_master.state_id, 
	                     dbo.student_master.admission_number, dbo.student_master.roll_number, dbo.student_master.salutation, dbo.student_master.first_given_name, dbo.student_master.middle_name, dbo.student_master.last_family_name, 
	                     dbo.student_master.suffix, dbo.student_master.preferred_name, dbo.student_master.previous_name, dbo.student_master.social_security_number, dbo.student_master.other_govt_issued_number, dbo.student_master.dob, 
	                     dbo.student_master.gender, dbo.student_master.race, dbo.student_master.ethnicity, dbo.student_master.marital_status, dbo.student_master.country_of_birth, dbo.student_master.nationality, 
	                     dbo.student_master.first_language_id, dbo.student_master.second_language_id, dbo.student_master.third_language_id, dbo.student_master.home_phone, dbo.student_master.mobile_phone, 
	                     dbo.student_master.personal_email, dbo.student_master.school_email, dbo.student_master.twitter, dbo.student_master.facebook, dbo.student_master.instagram, dbo.student_master.youtube, dbo.student_master.linkedin, 
	                     dbo.student_master.home_address_line_one, dbo.student_master.home_address_line_two, dbo.student_master.home_address_city, dbo.student_master.home_address_state, dbo.student_master.home_address_zip, 
	                     dbo.student_master.bus_no, dbo.student_master.school_bus_pick_up, dbo.student_master.school_bus_drop_off, dbo.student_master.mailing_address_same_to_home, dbo.student_master.mailing_address_line_one, 
	                     dbo.student_master.mailing_address_line_two, dbo.student_master.mailing_address_city, dbo.student_master.mailing_address_state, dbo.student_master.mailing_address_zip, dbo.student_master.home_address_country, 
	                     dbo.student_master.mailing_address_country, dbo.student_master.section_id, dbo.student_master.student_internal_id, dbo.student_master.updated_on, dbo.student_master.updated_by, dbo.student_master.enrollment_type, 
	                     dbo.student_master.is_active, dbo.student_master.student_guid, dbo.student_enrollment.enrollment_id, dbo.student_enrollment.enrollment_date, dbo.student_enrollment.enrollment_code, dbo.student_enrollment.calender_id, dbo.student_enrollment.grade_id,
	                     dbo.student_enrollment.grade_level_title, dbo.student_enrollment.rolling_option, dbo.student_enrollment.school_name, dbo.sections.name AS section_name, dbo.student_master.created_on, dbo.student_master.created_by, dbo.student_master.student_portal_id, dbo.student_master.eligibility_504, dbo.student_master.economic_disadvantage, dbo.student_master.free_lunch_eligibility, dbo.student_master.special_education_indicator, dbo.student_master.lep_indicator, dbo.student_master.estimated_grad_date
                    FROM dbo.student_enrollment INNER JOIN
	                     dbo.student_master ON dbo.student_enrollment.tenant_id = dbo.student_master.tenant_id AND dbo.student_enrollment.school_id = dbo.student_master.school_id AND 
	                     dbo.student_enrollment.student_id = dbo.student_master.student_id LEFT OUTER JOIN
	                     dbo.sections ON dbo.student_master.tenant_id = dbo.sections.tenant_id AND dbo.student_master.school_id = dbo.sections.school_id AND dbo.student_master.section_id = dbo.sections.section_id
                    WHERE (dbo.student_enrollment.is_active = 1)
                    GO";

            migrationBuilder.Sql(studentListView);

            string parentListView =
                  @"
                    CREATE VIEW [dbo].[parent_list_view]
                    AS
                    SELECT dbo.parent_info.tenant_id, dbo.parent_associationship.school_id, dbo.parent_info.parent_id, dbo.parent_info.firstname, dbo.parent_info.lastname, dbo.parent_info.home_phone, dbo.parent_info.work_phone, dbo.parent_info.mobile, 
	                    dbo.parent_info.is_portal_user, dbo.parent_info.bus_pickup, dbo.parent_info.bus_dropoff, dbo.parent_info.updated_on, dbo.parent_info.updated_by, dbo.parent_info.bus_No, dbo.parent_info.login_email, 
	                    dbo.parent_info.middlename, dbo.parent_info.personal_email, dbo.parent_info.salutation, dbo.parent_info.suffix, dbo.parent_info.user_profile, dbo.parent_info.work_email, dbo.parent_info.parent_photo, 
	                    dbo.parent_info.parent_guid, dbo.parent_associationship.student_id, dbo.student_master.first_given_name, dbo.student_master.middle_name AS student_middle_name, dbo.student_master.last_family_name, 
	                    dbo.parent_address.student_address_same, dbo.parent_address.address_line_one, dbo.parent_address.address_line_two, dbo.parent_address.country, dbo.parent_address.city, dbo.parent_address.state, 
	                    dbo.parent_address.zip, dbo.parent_associationship.associationship, dbo.parent_info.created_by, dbo.parent_info.created_on, dbo.parent_associationship.relationship
                    FROM dbo.parent_associationship INNER JOIN
	                    dbo.parent_info ON dbo.parent_associationship.tenant_id = dbo.parent_info.tenant_id AND 
	                    dbo.parent_associationship.parent_id = dbo.parent_info.parent_id INNER JOIN
	                    dbo.student_master ON dbo.parent_associationship.tenant_id = dbo.student_master.tenant_id AND dbo.parent_associationship.school_id = dbo.student_master.school_id AND 
	                    dbo.parent_associationship.student_id = dbo.student_master.student_id LEFT OUTER JOIN
	                    dbo.parent_address ON dbo.parent_info.tenant_id = dbo.parent_address.tenant_id AND dbo.parent_info.school_id = dbo.parent_address.school_id AND dbo.parent_info.parent_id = dbo.parent_address.parent_id
                    GO";

            migrationBuilder.Sql(parentListView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView = @"DROP VIEW dbo.all_course_section_view";
            migrationBuilder.Sql(allCourseSectionView);

            string studentListView = @"DROP VIEW dbo.student_list_view";
            migrationBuilder.Sql(studentListView);

            string parentListView = @"DROP VIEW dbo.parent_list_view";
            migrationBuilder.Sql(parentListView);
        }
    }
}
