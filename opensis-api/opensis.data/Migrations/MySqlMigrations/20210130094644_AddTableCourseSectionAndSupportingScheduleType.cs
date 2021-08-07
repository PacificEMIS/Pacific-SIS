using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableCourseSectionAndSupportingScheduleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "relationship",
                table: "parent_associationship",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "course_block_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    serial = table.Column<int>(nullable: false),
                    block_id = table.Column<int>(nullable: true),
                    period_id = table.Column<int>(nullable: true),
                    room_id = table.Column<int>(nullable: true),
                    take_attendance = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_block_schedule_1", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "FK_course_block_schedule_block",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id },
                        principalTable: "block",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_block_schedule_school_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.period_id },
                        principalTable: "school_periods",
                        principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_block_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_calendar_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    serial = table.Column<int>(nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: true),
                    period_id = table.Column<int>(nullable: true),
                    room_id = table.Column<int>(nullable: true),
                    take_attendance = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_calendar_schedule_1", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "FK_course_calendar_schedule_school_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.period_id },
                        principalTable: "school_periods",
                        principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_calendar_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_fixed_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    serial = table.Column<int>(nullable: false),
                    room_id = table.Column<int>(nullable: true),
                    period_id = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_fixed_schedule_1", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "FK_course_fixed_schedule_school_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.period_id },
                        principalTable: "school_periods",
                        principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_fixed_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_section",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    course_section_name = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    calendar_id = table.Column<int>(nullable: true),
                    attendance_category_id = table.Column<int>(nullable: true),
                    credit_hours = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    seats = table.Column<int>(nullable: true),
                    is_weighted_course = table.Column<bool>(nullable: true),
                    affects_class_rank = table.Column<bool>(nullable: true),
                    affects_honor_roll = table.Column<bool>(nullable: true),
                    online_class_room = table.Column<bool>(nullable: true),
                    online_classroom_url = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    online_classroom_password = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    use_standards = table.Column<bool>(nullable: true),
                    standard_grade_scale_id = table.Column<int>(nullable: true),
                    duration_based_on_period = table.Column<bool>(nullable: true),
                    marking_period_id = table.Column<int>(nullable: true),
                    duration_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    duration_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    schedule_type = table.Column<string>(unicode: false, maxLength: 25, nullable: true, comment: "Fixed Schedule (1) / Variable Schedule (2) / Calendar Days (3) / Bell schedule (4)"),
                    meeting_days = table.Column<string>(unicode: false, maxLength: 13, nullable: true, comment: "Starting Sunday as 0, 0|1|2|3|4|5|6"),
                    attendance_taken = table.Column<bool>(nullable: true),
                    is_active = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_section_1", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "FK_course_section_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_section_attendance_code_categories",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id },
                        principalTable: "attendance_code_categories",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_section_school_calendars",
                        columns: x => new { x.tenant_id, x.school_id, x.calendar_id },
                        principalTable: "school_calendars",
                        principalColumns: new[] { "tenant_id", "school_id", "calender_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_section_course",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id },
                        principalTable: "course",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_section_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_variable_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    serial = table.Column<int>(nullable: false),
                    day = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    period_id = table.Column<int>(nullable: true),
                    room_id = table.Column<int>(nullable: true),
                    take_attendance = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_variable_schedule_1", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "FK_course_variable_schedule_school_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.period_id },
                        principalTable: "school_periods",
                        principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_variable_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_period_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_room_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_period_id",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_room_id",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_period_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_room_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_attendance_category_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_calendar_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "calendar_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_period_id",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_room_id",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_block_schedule");

            migrationBuilder.DropTable(
                name: "course_calendar_schedule");

            migrationBuilder.DropTable(
                name: "course_fixed_schedule");

            migrationBuilder.DropTable(
                name: "course_section");

            migrationBuilder.DropTable(
                name: "course_variable_schedule");

            migrationBuilder.AlterColumn<string>(
                name: "relationship",
                table: "parent_associationship",
                type: "varchar(10) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
