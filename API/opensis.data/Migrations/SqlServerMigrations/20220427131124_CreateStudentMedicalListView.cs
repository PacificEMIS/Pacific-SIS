using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateStudentMedicalListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string studentMedicalListView =
                  @"
                    DROP VIEW IF EXISTS [dbo].[student_medical_list_view];  
                    GO

                    CREATE VIEW [dbo].[student_medical_list_view]
                    AS
                    SELECT dbo.student_master.school_id, dbo.student_master.student_id, dbo.student_master.tenant_id, dbo.student_medical_alert.alert_type, dbo.student_medical_alert.alert_description, dbo.student_medical_note.note_date, 
                         dbo.student_medical_note.medical_note, dbo.student_medical_immunization.immunization_type, dbo.student_medical_immunization.immunization_date, 
                         dbo.student_medical_immunization.comment AS immunization_comment, dbo.student_medical_nurse_visit.nurse_visit_date, dbo.student_medical_nurse_visit.reason, dbo.student_medical_nurse_visit.result, 
                         dbo.student_medical_nurse_visit.comment AS nurse_comment
                    FROM dbo.student_master INNER JOIN
                        dbo.student_medical_note ON dbo.student_master.tenant_id = dbo.student_medical_note.tenant_id AND dbo.student_master.school_id = dbo.student_medical_note.school_id AND 
                        dbo.student_master.student_id = dbo.student_medical_note.student_id INNER JOIN
                        dbo.student_medical_alert ON dbo.student_master.tenant_id = dbo.student_medical_alert.tenant_id AND dbo.student_master.school_id = dbo.student_medical_alert.school_id AND 
                        dbo.student_master.student_id = dbo.student_medical_alert.student_id INNER JOIN
                        dbo.student_medical_nurse_visit ON dbo.student_master.tenant_id = dbo.student_medical_nurse_visit.tenant_id AND dbo.student_master.school_id = dbo.student_medical_nurse_visit.school_id AND 
                        dbo.student_master.student_id = dbo.student_medical_nurse_visit.student_id INNER JOIN
                        dbo.student_medical_immunization ON dbo.student_master.tenant_id = dbo.student_medical_immunization.tenant_id AND dbo.student_master.school_id = dbo.student_medical_immunization.school_id AND 
                        dbo.student_master.student_id = dbo.student_medical_immunization.student_id
                    GO";

            migrationBuilder.Sql(studentMedicalListView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string studentMedicalListView = @"DROP VIEW dbo.student_medical_list_view";
            migrationBuilder.Sql(studentMedicalListView);
        }
    }
}
