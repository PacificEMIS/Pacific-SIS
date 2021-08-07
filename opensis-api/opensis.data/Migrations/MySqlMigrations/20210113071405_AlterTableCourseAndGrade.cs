using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableCourseAndGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tite",
                table: "grade");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "grade",
                unicode: false,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_course_standard_course",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "course_id" },
                principalTable: "course",
                principalColumns: new[] { "tenant_id", "school_id", "course_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_standard_course",
                table: "course_standard");

            migrationBuilder.DropColumn(
                name: "title",
                table: "grade");

            migrationBuilder.AddColumn<string>(
                name: "tite",
                table: "grade",
                type: "longtext CHARACTER SET utf8mb4",
                unicode: false,
                nullable: true);
        }
    }
}
