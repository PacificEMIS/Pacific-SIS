using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddAcademicYearInBlockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "subject",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "student_enrollment_code",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "honor_rolls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "grade_scale",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "course_comment_category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "course",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "block_period",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "block",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "block",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "attendance_code_categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rollover_id",
                table: "attendance_code",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "subject");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "honor_rolls");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "grade_scale");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "course_comment_category");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "course");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "block_period");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "block");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "block");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "attendance_code_categories");

            migrationBuilder.DropColumn(
                name: "rollover_id",
                table: "attendance_code");
        }
    }
}
