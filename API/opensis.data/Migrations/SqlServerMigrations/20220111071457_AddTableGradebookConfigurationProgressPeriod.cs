using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableGradebookConfigurationProgressPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "student_final_grade_standard",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "student_final_grade",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "student_effort_grade_master",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "staff_coursesection_schedule",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "staff_coursesection_schedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "gradebook_configuration_quarter",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_progressPeriod",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    prgrsprd_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_progressPeriod", x => x.id);
                    table.ForeignKey(
                        name: "FK_gb_conf_progPeriod_conf",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "FK_gradebook_conf_progressPeriod",
                        columns: x => new { x.tenant_id, x.school_id, x.prgrsprd_marking_period_id, x.academic_year, x.qtr_marking_period_id },
                        principalTable: "progress_periods",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration~",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_configuration_quarter_progress_periods",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_staff_cs_sch_periods",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_effort_grade_periods",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_final_grade_periods",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_configuration_quarter_progress_periods",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_cs_sch_periods",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_student_effort_grade_periods",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "FK_student_final_grade_periods",
                table: "student_final_grade");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_progressPeriod");

            migrationBuilder.DropIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "student_final_grade");

            migrationBuilder.DropIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "student_effort_grade_master");

            migrationBuilder.DropIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "student_final_grade_standard");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "student_final_grade");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "student_effort_grade_master");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "gradebook_configuration_quarter");
        }
    }
}
