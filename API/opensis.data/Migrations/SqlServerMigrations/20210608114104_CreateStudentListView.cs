using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateStudentListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string script =
                   @"
                    DROP VIEW IF EXISTS [dbo].[student_list_view] ;  
                    GO

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
	                     dbo.student_master.mailing_address_country, dbo.student_master.section_id, dbo.student_master.student_internal_id, dbo.student_master.last_updated, dbo.student_master.updated_by, dbo.student_master.enrollment_type, 
	                     dbo.student_master.is_active, dbo.student_master.student_guid, dbo.student_enrollment.enrollment_id, dbo.student_enrollment.enrollment_date, dbo.student_enrollment.enrollment_code, dbo.student_enrollment.calender_id, dbo.student_enrollment.grade_id,
	                     dbo.student_enrollment.grade_level_title, dbo.student_enrollment.rolling_option, dbo.student_enrollment.school_name, dbo.sections.name AS section_name
                    FROM dbo.student_enrollment INNER JOIN
	                     dbo.student_master ON dbo.student_enrollment.tenant_id = dbo.student_master.tenant_id AND dbo.student_enrollment.school_id = dbo.student_master.school_id AND 
	                     dbo.student_enrollment.student_id = dbo.student_master.student_id LEFT OUTER JOIN
	                     dbo.sections ON dbo.student_master.tenant_id = dbo.sections.tenant_id AND dbo.student_master.school_id = dbo.sections.school_id AND dbo.student_master.section_id = dbo.sections.section_id
                    WHERE (dbo.student_enrollment.is_active = 1)
                    GO";

            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string script = @"DROP VIEW dbo.student_list_view";
            migrationBuilder.Sql(script);
        }
    }
}
