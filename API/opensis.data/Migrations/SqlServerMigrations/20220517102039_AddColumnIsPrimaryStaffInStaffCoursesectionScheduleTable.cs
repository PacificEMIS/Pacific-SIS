using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddColumnIsPrimaryStaffInStaffCoursesectionScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_primary_staff",
                table: "staff_coursesection_schedule",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_primary_staff",
                table: "staff_coursesection_schedule");
        }
    }
}
