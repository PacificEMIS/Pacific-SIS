using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddNewColumnInStudentEnrollmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_type",
                table: "parent_info");

            migrationBuilder.AddColumn<string>(
                name: "enrollment_type",
                table: "student_master",
                unicode: false,
                fixedLength: true,
                maxLength: 8,
                nullable: true,
                comment: "\"Internal\" or \"External\". Default \"Internal\"");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "home_school_id",
                table: "student_enrollment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "home_student_id",
                table: "student_enrollment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_type",
                table: "parent_associationship",
                unicode: false,
                maxLength: 9,
                nullable: true,
                comment: "Primary | Secondary | Other");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "enrollment_type",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "home_school_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "home_student_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "contact_type",
                table: "parent_associationship");

            migrationBuilder.AddColumn<string>(
                name: "contact_type",
                table: "parent_info",
                type: "varchar(9) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 9,
                nullable: true,
                comment: "Primary | Secondary | Other");
        }
    }
}
