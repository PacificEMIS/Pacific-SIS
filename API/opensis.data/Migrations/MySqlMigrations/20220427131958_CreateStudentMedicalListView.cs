using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class CreateStudentMedicalListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			string studentMedicalListView =
				   @"
					DROP VIEW IF EXISTS student_medical_list_view;

                    CREATE VIEW student_medical_list_view
					AS
					SELECT student_master.school_id, student_master.student_id, student_master.tenant_id, student_medical_alert.alert_type, student_medical_alert.alert_description, student_medical_note.note_date, 
					student_medical_note.medical_note, student_medical_immunization.immunization_type, student_medical_immunization.immunization_date, 
					student_medical_immunization.comment AS immunization_comment, student_medical_nurse_visit.nurse_visit_date, student_medical_nurse_visit.reason, student_medical_nurse_visit.result, 
					student_medical_nurse_visit.comment AS nurse_comment
					FROM student_master INNER JOIN
						student_medical_note ON student_master.tenant_id = student_medical_note.tenant_id AND student_master.school_id = student_medical_note.school_id AND 
						student_master.student_id = student_medical_note.student_id INNER JOIN
						student_medical_alert ON student_master.tenant_id = student_medical_alert.tenant_id AND student_master.school_id = student_medical_alert.school_id AND 
						student_master.student_id = student_medical_alert.student_id INNER JOIN
						student_medical_nurse_visit ON student_master.tenant_id = student_medical_nurse_visit.tenant_id AND student_master.school_id = student_medical_nurse_visit.school_id AND 
						student_master.student_id = student_medical_nurse_visit.student_id INNER JOIN
						student_medical_immunization ON student_master.tenant_id = student_medical_immunization.tenant_id AND student_master.school_id = student_medical_immunization.school_id AND 
						student_master.student_id = student_medical_immunization.student_id";

			migrationBuilder.Sql(studentMedicalListView);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string studentMedicalListView = @"DROP VIEW student_medical_list_view";
			migrationBuilder.Sql(studentMedicalListView);
		}
    }
}
