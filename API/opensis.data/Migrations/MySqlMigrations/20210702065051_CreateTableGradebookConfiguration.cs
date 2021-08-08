using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class CreateTableGradebookConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gradebook_configuration",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    general = table.Column<string>(unicode: false, maxLength: 250, nullable: true, comment: "weight grades,assigned date defaults to today,due date defaults to today - separated by pipe(|)"),
                    score_rounding = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    assignment_sorting = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    max_anomalous_grade = table.Column<int>(nullable: true),
                    upgraded_assignment_grade_days = table.Column<int>(nullable: true),
                    score_breakoff_points_a = table.Column<int>(nullable: true),
                    score_breakoff_points_b = table.Column<int>(nullable: true),
                    score_breakoff_points_c = table.Column<int>(nullable: true),
                    score_breakoff_points_d = table.Column<int>(nullable: true),
                    score_breakoff_points_e = table.Column<int>(nullable: true),
                    score_breakoff_points_f = table.Column<int>(nullable: true),
                    score_breakoff_points_inc = table.Column<int>(nullable: true),
                    final_grading_qtr1 = table.Column<int>(nullable: true),
                    final_grading_qtr2 = table.Column<int>(nullable: true),
                    final_grading_qtr3 = table.Column<int>(nullable: true),
                    final_grading_qtr4 = table.Column<int>(nullable: true),
                    sem1_qtr1 = table.Column<int>(nullable: true),
                    sem1_qtr2 = table.Column<int>(nullable: true),
                    sem2_qtr3 = table.Column<int>(nullable: true),
                    sem2_qtr4 = table.Column<int>(nullable: true),
                    fullyear_sem1 = table.Column<int>(nullable: true),
                    fullyear_sem2 = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, fixedLength: true, maxLength: 150, nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: true),
                    modified_by = table.Column<string>(unicode: false, fixedLength: true, maxLength: 150, nullable: true),
                    date_modifed = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gradebook_configuration");
        }
    }
}
