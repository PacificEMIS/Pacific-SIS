using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddStartDateEndDateInSchoolCalendar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "student_enrollment_code",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "school_calendars",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "school_calendars",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "visible_to_membership_id",
                table: "school_calendars",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "calendar_events",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "calendar_events",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "school_calendars");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "school_calendars");

            migrationBuilder.DropColumn(
                name: "visible_to_membership_id",
                table: "school_calendars");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "calendar_events");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "calendar_events");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "student_enrollment_code",
                type: "varchar(4) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
