using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AlterStaffSchoolAndCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_student_master",
                table: "student_enrollment");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "staff_school_info",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "staff_certificate_info",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_staff_school_info",
                table: "staff_school_info",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_staff_certificate_info",
                table: "staff_certificate_info",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_staff_school_info",
                table: "staff_school_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_staff_certificate_info",
                table: "staff_certificate_info");

            migrationBuilder.DropColumn(
                name: "id",
                table: "staff_school_info");

            migrationBuilder.DropColumn(
                name: "id",
                table: "staff_certificate_info");

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_student_master",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
