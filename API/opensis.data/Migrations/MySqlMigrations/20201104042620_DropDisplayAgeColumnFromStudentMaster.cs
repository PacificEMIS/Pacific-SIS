using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class DropDisplayAgeColumnFromStudentMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_age",
                table: "student_master");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_age",
                table: "student_master",
                type: "varchar(100) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 100,
                nullable: true);
        }
    }
}
