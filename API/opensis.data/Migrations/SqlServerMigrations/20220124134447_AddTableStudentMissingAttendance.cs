using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableStudentMissingAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_missing_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    missing_attendance_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    course_id = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    course_section_id = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    period_id = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    attendance_category_id = table.Column<int>(type: "int", nullable: true),
                    attendance_code = table.Column<int>(type: "int", nullable: true),
                    missing_attendance_date = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "('0001-01-01')"),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_missing_attendance_1", x => new { x.tenant_id, x.school_id, x.missing_attendance_id });
                    table.ForeignKey(
                        name: "FK_missing_attendance_block_period",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "FK_missing_attendance_cs",
                        columns: x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id },
                        principalTable: "staff_coursesection_schedule",
                        principalColumns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "FK_missing_attendance_staff",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_missing_attendance_tenant_id_school_id_block_id_period_id",
                table: "student_missing_attendance",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_missing_attendance_tenant_id_school_id_staff_id_course_id_course_section_id",
                table: "student_missing_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_missing_attendance_tenant_id_staff_id",
                table: "student_missing_attendance",
                columns: new[] { "tenant_id", "staff_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_missing_attendance");
        }
    }
}
