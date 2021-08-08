using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableGradeAndGradeScale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grade_id",
                table: "grade_us_standard");

            migrationBuilder.DropColumn(
                name: "grade_us_standard",
                table: "course");

            migrationBuilder.AddColumn<int>(
                name: "grade_standard_id",
                table: "grade_us_standard",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_school_specific",
                table: "grade_us_standard",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "grade_scale",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "grade",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grade_standard_id",
                table: "grade_us_standard");

            migrationBuilder.DropColumn(
                name: "is_school_specific",
                table: "grade_us_standard");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "grade_scale");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "grade");

            migrationBuilder.AddColumn<int>(
                name: "grade_id",
                table: "grade_us_standard",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade_us_standard",
                table: "course",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }
    }
}
