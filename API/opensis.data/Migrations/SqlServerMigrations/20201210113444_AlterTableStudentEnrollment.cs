using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AlterTableStudentEnrollment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_school_calendars",
                table: "student_enrollment");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_calender_id",
                table: "student_enrollment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_calender_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "calender_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_school_calendars",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "calender_id" },
                principalTable: "school_calendars",
                principalColumns: new[] { "tenant_id", "school_id", "calender_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
