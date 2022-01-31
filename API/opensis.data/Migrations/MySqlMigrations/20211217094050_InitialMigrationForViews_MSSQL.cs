using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class InitialMigrationForViews_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView =
                   @"
                    CREATE VIEW all_course_section_view
                    AS
                    SELECT course.tenant_id, course.school_id, course.course_id, course.course_title, course.course_subject, course.course_program, course_section.academic_year, course_section.course_section_id, 
                                             course_section.course_section_name, course_section.seats, course_section.duration_start_date, course_section.duration_end_date, course_section.yr_marking_period_id, 
                                             course_section.qtr_marking_period_id, course_section.smstr_marking_period_id, course_section.schedule_type, course_section.meeting_days AS fixed_days, 
                                             course_fixed_schedule.room_id AS fixed_room_id, course_fixed_schedule.period_id AS fixed_period_id, course_variable_schedule.day AS var_day, course_variable_schedule.period_id AS var_period_id, 
                                             course_variable_schedule.room_id AS var_room_id, course_calendar_schedule.date AS cal_date, course_calendar_schedule.period_id AS cal_period_id, course_calendar_schedule.room_id AS cal_room_id, 
                                             course_block_schedule.period_id AS block_period_id, course_block_schedule.room_id AS block_room_id, course_section.is_active, course.course_grade_level, course_section.grade_scale_id, 
                                             course_section.allow_teacher_conflict, course_section.allow_student_conflict, course_section.calendar_id, course_section.attendance_taken, DAYNAME(`course_calendar_schedule`.`date`) AS `cal_day`, 
                                             course_variable_schedule.take_attendance AS take_attendance_variable, course_calendar_schedule.take_attendance AS take_attendance_calendar, 
                                             course_block_schedule.take_attendance AS take_attendance_block, course_block_schedule.block_id, course_section.attendance_category_id
                    FROM course INNER JOIN
                                             course_section ON course.tenant_id = course_section.tenant_id AND course.school_id = course_section.school_id AND course.course_id = course_section.course_id LEFT OUTER JOIN
                                             course_fixed_schedule ON course_section.tenant_id = course_fixed_schedule.tenant_id AND course_section.school_id = course_fixed_schedule.school_id AND 
                                             course_section.course_id = course_fixed_schedule.course_id AND course_section.course_section_id = course_fixed_schedule.course_section_id LEFT OUTER JOIN
                                             course_variable_schedule ON course_section.tenant_id = course_variable_schedule.tenant_id AND course_section.school_id = course_variable_schedule.school_id AND 
                                             course_section.course_id = course_variable_schedule.course_id AND course_section.course_section_id = course_variable_schedule.course_section_id LEFT OUTER JOIN
                                             course_calendar_schedule ON course_section.tenant_id = course_calendar_schedule.tenant_id AND course_section.school_id = course_calendar_schedule.school_id AND 
                                             course_section.course_id = course_calendar_schedule.course_id AND course_section.course_section_id = course_calendar_schedule.course_section_id LEFT OUTER JOIN
                                             course_block_schedule ON course_section.tenant_id = course_block_schedule.tenant_id AND course_section.school_id = course_block_schedule.school_id AND 
                                             course_section.course_id = course_block_schedule.course_id AND course_section.course_section_id = course_block_schedule.course_section_id";

            migrationBuilder.Sql(allCourseSectionView);

            string studentListView =
                   @"
                    CREATE VIEW student_list_view
                    AS
                    SELECT student_master.tenant_id, student_master.school_id, student_master.student_id, student_master.alternate_id, student_master.district_id, student_master.state_id, 
		                    student_master.admission_number, student_master.roll_number, student_master.salutation, student_master.first_given_name, student_master.middle_name, student_master.last_family_name, 
		                    student_master.suffix, student_master.preferred_name, student_master.previous_name, student_master.social_security_number, student_master.other_govt_issued_number, student_master.dob, 
		                    student_master.gender, student_master.race, student_master.ethnicity, student_master.marital_status, student_master.country_of_birth, student_master.nationality, 
		                    student_master.first_language_id, student_master.second_language_id, student_master.third_language_id, student_master.home_phone, student_master.mobile_phone, 
		                    student_master.personal_email, student_master.school_email, student_master.twitter, student_master.facebook, student_master.instagram, student_master.youtube, student_master.linkedin, 
		                    student_master.home_address_line_one, student_master.home_address_line_two, student_master.home_address_city, student_master.home_address_state, student_master.home_address_zip, 
		                    student_master.bus_no, student_master.school_bus_pick_up, student_master.school_bus_drop_off, student_master.mailing_address_same_to_home, student_master.mailing_address_line_one, 
		                    student_master.mailing_address_line_two, student_master.mailing_address_city, student_master.mailing_address_state, student_master.mailing_address_zip, student_master.home_address_country, 
		                    student_master.mailing_address_country, student_master.section_id, student_master.student_internal_id, student_master.updated_on, student_master.updated_by, student_master.enrollment_type, 
		                    student_master.is_active, student_master.student_guid, student_enrollment.enrollment_id, student_enrollment.enrollment_date, student_enrollment.enrollment_code, student_enrollment.calender_id, student_enrollment.grade_id,
		                    student_enrollment.grade_level_title, student_enrollment.rolling_option, student_enrollment.school_name, sections.name AS section_name, student_master.created_by, student_master.created_on, student_master.student_portal_id, student_master.eligibility_504, student_master.economic_disadvantage, student_master.free_lunch_eligibility, student_master.special_education_indicator, student_master.lep_indicator, student_master.estimated_grad_date
                    FROM student_enrollment INNER JOIN
		                    student_master ON student_enrollment.tenant_id = student_master.tenant_id AND student_enrollment.school_id = student_master.school_id AND 
		                    student_enrollment.student_id = student_master.student_id LEFT OUTER JOIN
		                    sections ON student_master.tenant_id = sections.tenant_id AND student_master.school_id = sections.school_id AND student_master.section_id = sections.section_id
                    WHERE (student_enrollment.is_active = 1)";

            migrationBuilder.Sql(studentListView);

            string parentListView =
                   @"
                    CREATE VIEW parent_list_view
                    AS
                    SELECT parent_info.tenant_id, parent_associationship.school_id, parent_info.parent_id, parent_info.firstname, parent_info.lastname, parent_info.home_phone, parent_info.work_phone, parent_info.mobile, 
	                    parent_info.is_portal_user, parent_info.bus_pickup, parent_info.bus_dropoff, parent_info.updated_on, parent_info.updated_by, parent_info.bus_No, parent_info.login_email, 
	                    parent_info.middlename, parent_info.personal_email, parent_info.salutation, parent_info.suffix, parent_info.user_profile, parent_info.work_email, parent_info.parent_photo, 
	                    parent_info.parent_guid, parent_associationship.student_id, student_master.first_given_name, student_master.middle_name AS student_middle_name, student_master.last_family_name, 
	                    parent_address.student_address_same, parent_address.address_line_one, parent_address.address_line_two, parent_address.country, parent_address.city, parent_address.state, 
	                    parent_address.zip, parent_associationship.associationship, parent_info.created_by, parent_info.created_on, parent_associationship.relationship
                    FROM parent_associationship INNER JOIN
	                    parent_info ON parent_associationship.tenant_id = parent_info.tenant_id AND 
	                    parent_associationship.parent_id = parent_info.parent_id INNER JOIN
	                    student_master ON parent_associationship.tenant_id = student_master.tenant_id AND parent_associationship.school_id = student_master.school_id AND 
	                    parent_associationship.student_id = student_master.student_id LEFT OUTER JOIN
	                    parent_address ON parent_info.tenant_id = parent_address.tenant_id AND parent_info.school_id = parent_address.school_id AND parent_info.parent_id = parent_address.parent_id";

            migrationBuilder.Sql(parentListView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView = @"DROP VIEW all_course_section_view";
            migrationBuilder.Sql(allCourseSectionView);

            string studentListView = @"DROP VIEW student_list_view";
            migrationBuilder.Sql(studentListView);

            string parentListView = @"DROP VIEW parent_list_view";
            migrationBuilder.Sql(parentListView);
        }
    }
}
