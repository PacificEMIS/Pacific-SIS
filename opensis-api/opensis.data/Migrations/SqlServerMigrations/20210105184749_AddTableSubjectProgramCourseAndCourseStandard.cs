using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableSubjectProgramCourseAndCourseStandard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolMasterSchoolId",
                table: "student_enrollment_code",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolMasterTenantId",
                table: "student_enrollment_code",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "block",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    block_id = table.Column<int>(nullable: false),
                    block_title = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    block_sort_order = table.Column<long>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block", x => new { x.tenant_id, x.school_id, x.block_id });
                    table.ForeignKey(
                        name: "FK_block_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_title = table.Column<string>(unicode: false, nullable: true),
                    course_short_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    course_grade_level = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    course_program = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    course_subject = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    course_category = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    credit_hours = table.Column<string>(fixedLength: true, maxLength: 5, nullable: true),
                    grade_us_standard = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    course_description = table.Column<string>(unicode: false, nullable: true),
                    is_course_active = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => new { x.tenant_id, x.school_id, x.course_id });
                });

            migrationBuilder.CreateTable(
                name: "grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    grade_scale_name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    grade_scale_value = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    grade_scale_comment = table.Column<string>(unicode: false, nullable: true),
                    calculate_gpa = table.Column<bool>(nullable: true),
                    use_as_standard_grade_scale = table.Column<bool>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_scale", x => new { x.tenant_id, x.school_id, x.grade_scale_id });
                    table.ForeignKey(
                        name: "FK_grade_scale_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grade_us_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    standard_ref_no = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    grade_id = table.Column<int>(nullable: true),
                    grade_level = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    domain = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    subject = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    course = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    topic = table.Column<string>(unicode: false, nullable: true),
                    standard_details = table.Column<string>(unicode: false, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_us_standard", x => new { x.tenant_id, x.school_id, x.standard_ref_no });
                });

            migrationBuilder.CreateTable(
                name: "programs",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    program_id = table.Column<int>(nullable: false),
                    program_name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programs", x => new { x.tenant_id, x.school_id, x.program_id });
                });

            migrationBuilder.CreateTable(
                name: "subject",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    subject_id = table.Column<int>(nullable: false),
                    subject_name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject", x => new { x.tenant_id, x.school_id, x.subject_id });
                });

            migrationBuilder.CreateTable(
                name: "block_period",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    block_id = table.Column<int>(nullable: false),
                    period_id = table.Column<int>(nullable: false),
                    period_title = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    period_short_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    period_start_time = table.Column<string>(unicode: false, fixedLength: true, maxLength: 8, nullable: true),
                    period_end_time = table.Column<string>(unicode: false, fixedLength: true, maxLength: 8, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_period", x => new { x.tenant_id, x.school_id, x.block_id, x.period_id });
                    table.ForeignKey(
                        name: "FK_block_period_block",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id },
                        principalTable: "block",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    grade_id = table.Column<int>(nullable: false),
                    tite = table.Column<string>(unicode: false, nullable: true),
                    breakoff = table.Column<int>(nullable: true),
                    weighted_gp_value = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    unweighted_gp_value = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    comment = table.Column<string>(unicode: false, nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade", x => new { x.tenant_id, x.school_id, x.grade_scale_id, x.grade_id });
                    table.ForeignKey(
                        name: "FK_grade_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    standard_ref_no = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_standard", x => new { x.tenant_id, x.school_id, x.course_id, x.standard_ref_no });
                    table.ForeignKey(
                        name: "FK_course_standard_grade_us_standard",
                        columns: x => new { x.tenant_id, x.school_id, x.standard_ref_no },
                        principalTable: "grade_us_standard",
                        principalColumns: new[] { "tenant_id", "school_id", "standard_ref_no" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_code_SchoolMasterTenantId_SchoolMasterSchoolId",
                table: "student_enrollment_code",
                columns: new[] { "SchoolMasterTenantId", "SchoolMasterSchoolId" });

            migrationBuilder.CreateIndex(
                name: "IX_course_standard_tenant_id_school_id_standard_ref_no",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "standard_ref_no" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_code_school_master_SchoolMasterTenantId_SchoolMasterSchoolId",
                table: "student_enrollment_code",
                columns: new[] { "SchoolMasterTenantId", "SchoolMasterSchoolId" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_code_school_master_SchoolMasterTenantId_SchoolMasterSchoolId",
                table: "student_enrollment_code");

            migrationBuilder.DropTable(
                name: "block_period");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropTable(
                name: "course_standard");

            migrationBuilder.DropTable(
                name: "grade");

            migrationBuilder.DropTable(
                name: "programs");

            migrationBuilder.DropTable(
                name: "subject");

            migrationBuilder.DropTable(
                name: "block");

            migrationBuilder.DropTable(
                name: "grade_us_standard");

            migrationBuilder.DropTable(
                name: "grade_scale");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_code_SchoolMasterTenantId_SchoolMasterSchoolId",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "SchoolMasterSchoolId",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "SchoolMasterTenantId",
                table: "student_enrollment_code");
        }
    }
}
