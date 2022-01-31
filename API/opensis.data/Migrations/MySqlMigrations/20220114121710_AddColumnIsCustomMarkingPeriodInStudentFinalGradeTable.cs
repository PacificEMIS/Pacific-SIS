using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddColumnIsCustomMarkingPeriodInStudentFinalGradeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_conf_progressPeriod",
                table: "gradebook_configuration_progressPeriod");

            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_configuration_quarter_progress_periods",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "progress_periods$FK_progress_periods_quarters",
                table: "progress_periods");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_cs_sch_periods",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_student_effort_grade_periods",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "FK_student_final_grade_periods",
                table: "student_final_grade");

            migrationBuilder.DropIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_per~",
                table: "student_final_grade");

            migrationBuilder.DropIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_mar~",
                table: "student_effort_grade_master");

            migrationBuilder.DropIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_ma~",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_progress_periods_tenant_id",
                table: "progress_periods");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_prgrsprd~",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressPeriod");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_i~",
                table: "course_section");

            migrationBuilder.AddColumn<bool>(
                name: "is_custom_marking_period",
                table: "student_final_grade",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_exam_grade",
                table: "student_final_grade",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_custom_marking_period",
                table: "student_effort_grade_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_exam_grade",
                table: "student_effort_grade_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "quarter_id",
                table: "progress_periods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "academic_year",
                table: "progress_periods",
                type: "decimal(4,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,0)");

            migrationBuilder.AddColumn<bool>(
                name: "is_custom_marking_period",
                table: "course_section",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_progress_periods_tenant_id",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_per~",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_mar~",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_ma~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_conf_progress_periods",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "progress_periods$FK_quarters",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "quarter_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_staff_cs_sch_progress_periods",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_effort_grade_master_progress_periods",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_final_grade_progress_periods",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_conf_progress_periods",
                table: "gradebook_configuration_progressPeriod");

            migrationBuilder.DropForeignKey(
                name: "progress_periods$FK_quarters",
                table: "progress_periods");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_cs_sch_progress_periods",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_student_effort_grade_master_progress_periods",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "FK_student_final_grade_progress_periods",
                table: "student_final_grade");

            migrationBuilder.DropIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_per~",
                table: "student_final_grade");

            migrationBuilder.DropIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_mar~",
                table: "student_effort_grade_master");

            migrationBuilder.DropIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_ma~",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_progress_periods_tenant_id",
                table: "progress_periods");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressPeriod");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_id",
                table: "course_section");

            migrationBuilder.DropColumn(
                name: "is_custom_marking_period",
                table: "student_final_grade");

            migrationBuilder.DropColumn(
                name: "is_exam_grade",
                table: "student_final_grade");

            migrationBuilder.DropColumn(
                name: "is_custom_marking_period",
                table: "student_effort_grade_master");

            migrationBuilder.DropColumn(
                name: "is_exam_grade",
                table: "student_effort_grade_master");

            migrationBuilder.DropColumn(
                name: "is_custom_marking_period",
                table: "course_section");

            migrationBuilder.AlterColumn<int>(
                name: "quarter_id",
                table: "progress_periods",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "academic_year",
                table: "progress_periods",
                type: "decimal(4,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,0)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_progress_periods_tenant_id",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_prgrsprd_marking_per~",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_prgrsprd_mar~",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_prgrsprd_ma~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_prgrsprd~",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_i~",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_conf_progressPeriod",
                table: "gradebook_configuration_progressPeriod",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_configuration_quarter_progress_periods",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });

            migrationBuilder.AddForeignKey(
                name: "progress_periods$FK_progress_periods_quarters",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "quarter_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

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
    }
}
