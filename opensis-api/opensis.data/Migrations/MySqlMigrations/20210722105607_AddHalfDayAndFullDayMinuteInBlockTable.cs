using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddHalfDayAndFullDayMinuteInBlockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_calendar_events",
                table: "calendar_events");

            migrationBuilder.AddColumn<int>(
                name: "attendance_minutes",
                table: "student_daily_attendance",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "full_day_minutes",
                table: "block",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "half_day_minutes",
                table: "block",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_calendar_events_1",
                table: "calendar_events",
                columns: new[] { "tenant_id", "calendar_id", "event_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notice",
                table: "notice",
                columns: new[] { "tenant_id", "school_id", "notice_id", "created_on", "sort_order" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notice",
                table: "notice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_calendar_events_1",
                table: "calendar_events");

            migrationBuilder.DropColumn(
                name: "attendance_minutes",
                table: "student_daily_attendance");

            migrationBuilder.DropColumn(
                name: "full_day_minutes",
                table: "block");

            migrationBuilder.DropColumn(
                name: "half_day_minutes",
                table: "block");

            migrationBuilder.AddPrimaryKey(
                name: "PK_calendar_events",
                table: "calendar_events",
                columns: new[] { "tenant_id", "school_id", "calendar_id", "event_id" });
        }
    }
}
