using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableCustomFieldsAndAttendanceCodeCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_attendance_code",
                table: "attendance_code");

            migrationBuilder.AddColumn<int>(
                name: "attendance_category_id",
                table: "attendance_code",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_attendance_code",
                table: "attendance_code",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });

            migrationBuilder.CreateTable(
                name: "attendance_code_categories",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    attendance_category_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code_categories", x => new { x.tenant_id, x.school_id, x.attendance_category_id });
                    table.ForeignKey(
                        name: "FK_attendance_code_categories_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "custom_fields",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    field_id = table.Column<int>(nullable: false),
                    type = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    search = table.Column<bool>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    select_options = table.Column<string>(unicode: false, nullable: true),
                    category_id = table.Column<int>(nullable: true),
                    system_field = table.Column<bool>(nullable: true),
                    required = table.Column<bool>(nullable: true),
                    default_selection = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    hide = table.Column<bool>(nullable: true),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields", x => new { x.tenant_id, x.school_id, x.field_id });
                    table.ForeignKey(
                        name: "FK_custom_fields_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_code_attendance_code_categories",
                table: "attendance_code",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                principalTable: "attendance_code_categories",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gradelevels_school_master",
                table: "gradelevels",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_code_attendance_code_categories",
                table: "attendance_code");

            migrationBuilder.DropForeignKey(
                name: "FK_gradelevels_school_master",
                table: "gradelevels");

            migrationBuilder.DropTable(
                name: "attendance_code_categories");

            migrationBuilder.DropTable(
                name: "custom_fields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_attendance_code",
                table: "attendance_code");

            migrationBuilder.DropColumn(
                name: "attendance_category_id",
                table: "attendance_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_attendance_code",
                table: "attendance_code",
                columns: new[] { "tenant_id", "school_id", "attendance_code" });
        }
    }
}
