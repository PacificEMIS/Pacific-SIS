using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddEventColorInCalendarEventsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "event_color",
                table: "calendar_events",
                unicode: false,
                maxLength: 7,
                nullable: true,
                comment: "will contain HEX code e.g. #5175bc.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_color",
                table: "calendar_events");
        }
    }
}
