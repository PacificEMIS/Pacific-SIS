using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddStudentDailyAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "membership_id",
                table: "student_attendance_comments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "membership_id",
                table: "student_attendance",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "student_daily_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    attendance_date = table.Column<DateTime>(type: "date", nullable: false),
                    grade_id = table.Column<int>(nullable: true),
                    grade_scale_id = table.Column<int>(nullable: true),
                    section_id = table.Column<int>(nullable: true),
                    attendance_code = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    attendance_comment = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_daily_attendance", x => new { x.tenant_id, x.school_id, x.student_id, x.attendance_date });
                    table.ForeignKey(
                        name: "FK_student_daily_attendance_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_daily_attendance_sections",
                        columns: x => new { x.tenant_id, x.school_id, x.section_id },
                        principalTable: "sections",
                        principalColumns: new[] { "tenant_id", "school_id", "section_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_daily_attendance_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id_membership_id",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_membership_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_grade_scale_id",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_section_id",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_attendance_membership",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_attendance_comments_membership",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_attendance_membership",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_student_attendance_comments_membership",
                table: "student_attendance_comments");

            migrationBuilder.DropTable(
                name: "student_daily_attendance");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id_membership_id",
                table: "student_attendance_comments");

            migrationBuilder.DropIndex(
                name: "IX_student_attendance_tenant_id_school_id_membership_id",
                table: "student_attendance");

            migrationBuilder.DropColumn(
                name: "membership_id",
                table: "student_attendance_comments");

            migrationBuilder.DropColumn(
                name: "membership_id",
                table: "student_attendance");
        }
    }
}
