using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CalendarEventsAndStudentEnrollment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "age_range",
                table: "gradelevels",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "educational_stage",
                table: "gradelevels",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade_level_equivalency",
                table: "gradelevels",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "attendance_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    attendance_code = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    type = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    state_code = table.Column<string>(unicode: false, maxLength: 1, nullable: true),
                    default_code = table.Column<bool>(nullable: true),
                    allow_entry_by = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code", x => new { x.tenant_id, x.school_id, x.attendance_code });
                });

            migrationBuilder.CreateTable(
                name: "calendar_events",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    calendar_id = table.Column<int>(nullable: false),
                    event_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    school_date = table.Column<DateTime>(type: "date", nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    description = table.Column<string>(unicode: false, nullable: true),
                    visible_to_membership_id = table.Column<string>(unicode: false, maxLength: 30, nullable: true, comment: "membershipids separated by comma"),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_events", x => new { x.tenant_id, x.school_id, x.calendar_id, x.event_id });
                });

            migrationBuilder.CreateTable(
                name: "grade_equivalency",
                columns: table => new
                {
                    country = table.Column<string>(unicode: false, fixedLength: true, maxLength: 30, nullable: true),
                    isced_grade_level = table.Column<int>(nullable: true),
                    grade_description = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    age_range = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    educational_stage = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                });
           

            migrationBuilder.CreateTable(
                name: "student_enrollment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    enrollment_id = table.Column<int>(nullable: true),
                    grade_id = table.Column<int>(nullable: true),
                    section_id = table.Column<int>(nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    enrollment_code = table.Column<int>(nullable: true),
                    drop_code = table.Column<int>(nullable: true),
                    next_school = table.Column<int>(nullable: true),
                    calendar_id = table.Column<int>(nullable: true),
                    last_school = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment", x => new { x.tenant_id, x.school_id, x.student_id });
                });

            migrationBuilder.CreateTable(
                name: "student_enrollment_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    enrollment_code = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    short_name = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    type = table.Column<string>(unicode: false, maxLength: 4, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment_codes", x => new { x.tenant_id, x.school_id, x.enrollment_code });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance_code");

            migrationBuilder.DropTable(
                name: "calendar_events");

            migrationBuilder.DropTable(
                name: "grade_equivalency");

            migrationBuilder.DropTable(
                name: "student_enrollment");

            migrationBuilder.DropTable(
                name: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "age_range",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "educational_stage",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "grade_level_equivalency",
                table: "gradelevels");
        }
    }
}
