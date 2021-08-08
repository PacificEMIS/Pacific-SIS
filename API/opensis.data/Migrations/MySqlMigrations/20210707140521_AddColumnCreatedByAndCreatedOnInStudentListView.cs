using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddColumnCreatedByAndCreatedOnInStudentListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string script =
                   @"
                    DROP VIEW IF EXISTS student_list_view;

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
	                        student_master.mailing_address_country, student_master.section_id, student_master.student_internal_id, student_master.last_updated, student_master.updated_by, student_master.enrollment_type, 
	                        student_master.is_active, student_master.student_guid, student_enrollment.enrollment_id, student_enrollment.enrollment_date, student_enrollment.enrollment_code, student_enrollment.calender_id, student_enrollment.grade_id,
	                        student_enrollment.grade_level_title, student_enrollment.rolling_option, student_enrollment.school_name, sections.name AS section_name, student_master.created_by, student_master.created_on
                    FROM student_enrollment INNER JOIN
	                        student_master ON student_enrollment.tenant_id = student_master.tenant_id AND student_enrollment.school_id = student_master.school_id AND 
	                        student_enrollment.student_id = student_master.student_id LEFT OUTER JOIN
	                        sections ON student_master.tenant_id = sections.tenant_id AND student_master.school_id = sections.school_id AND student_master.section_id = sections.section_id
                    WHERE (student_enrollment.is_active = 1)";

            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string script = @"DROP VIEW student_list_view";
            migrationBuilder.Sql(script);
        }
    }
}
