using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateTableGradeBookConfigurationYearSemesterQuarter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gradebook_configuration_final_grading");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_quarter",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(nullable: false),
                    qtr_marking_period_id = table.Column<int>(nullable: true),
                    grading_percentage = table.Column<int>(nullable: true),
                    exam_percentage = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_quarter", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_quarter_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_quarter_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_semester",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(nullable: false),
                    smstr_marking_period_id = table.Column<int>(nullable: true),
                    grading_percentage = table.Column<int>(nullable: true),
                    exam_percentage = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_semester", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_semester_semester",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_semester_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_year",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(nullable: false),
                    yr_marking_period_id = table.Column<int>(nullable: true),
                    grading_percentage = table.Column<int>(nullable: true),
                    exam_percentage = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_year", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_year_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_year_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_qtr_marking_period_id",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration_id",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_smstr_marking_period_id",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration_id",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_yr_marking_period_id",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration_id",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gradebook_configuration_quarter");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_semester");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_year");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_final_grading",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    updated_by = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_final_grading", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_qtr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_smstr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_yr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration_~",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
        }
    }
}
