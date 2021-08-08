using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class CreateTableAssignmentAndAssignmentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assignment_type",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    assignment_type_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    marking_period_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    weightage = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_type_1", x => new { x.tenant_id, x.school_id, x.assignment_type_id });
                    table.ForeignKey(
                        name: "FK_assignment_type_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assignment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    assignment_type_id = table.Column<int>(nullable: false),
                    assignment_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: true),
                    assignment_title = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    points = table.Column<int>(nullable: true),
                    assignment_date = table.Column<DateTime>(type: "date", nullable: true),
                    due_date = table.Column<DateTime>(type: "date", nullable: true),
                    assignment_description = table.Column<string>(unicode: false, nullable: true),
                    staff_id = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment", x => new { x.tenant_id, x.school_id, x.assignment_type_id, x.assignment_id });
                    table.ForeignKey(
                        name: "FK_assignment_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_assignment_assignment_type",
                        columns: x => new { x.tenant_id, x.school_id, x.assignment_type_id },
                        principalTable: "assignment_type",
                        principalColumns: new[] { "tenant_id", "school_id", "assignment_type_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_staff_id",
                table: "assignment",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_type_tenant_id_school_id_marking_period_id",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assignment");

            migrationBuilder.DropTable(
                name: "assignment_type");
        }
    }
}
