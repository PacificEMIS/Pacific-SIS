using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateTableGradebookGrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_student_attendance",
                table: "student_attendance");

            migrationBuilder.AddColumn<DateTime>(
                name: "login_attempt_date",
                table: "user_master",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "login_failure_count",
                table: "user_master",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_attendance_1",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "staff_id", "course_id", "course_section_id", "attendance_date", "block_id", "period_id" });

            migrationBuilder.CreateTable(
                name: "gradebook_grades",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    assignment_type_id = table.Column<int>(nullable: false),
                    assignment_id = table.Column<int>(nullable: false),
                    allowed_marks = table.Column<string>(unicode: false, fixedLength: true, maxLength: 5, nullable: true, comment: "* (Asterisk) is also allowed, hence char datatype"),
                    percentage = table.Column<int>(nullable: true),
                    letter_grade = table.Column<string>(unicode: false, fixedLength: true, maxLength: 5, nullable: true),
                    comment = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    running_avg = table.Column<string>(unicode: false, fixedLength: true, maxLength: 5, nullable: true),
                    running_avg_grade = table.Column<string>(unicode: false, fixedLength: true, maxLength: 5, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_grades", x => new { x.tenant_id, x.school_id, x.student_id, x.academic_year, x.marking_period_id, x.course_section_id, x.assignment_type_id, x.assignment_id });
                    table.ForeignKey(
                        name: "FK_gradebook_grades_assignment",
                        columns: x => new { x.tenant_id, x.school_id, x.assignment_id },
                        principalTable: "assignment",
                        principalColumns: new[] { "tenant_id", "school_id", "assignment_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_grades_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment_id",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gradebook_grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_attendance_1",
                table: "student_attendance");

            migrationBuilder.DropColumn(
                name: "login_attempt_date",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "login_failure_count",
                table: "user_master");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_attendance",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "staff_id", "course_id", "course_section_id", "attendance_category_id", "attendance_code", "attendance_date", "block_id", "period_id" });
        }
    }
}
