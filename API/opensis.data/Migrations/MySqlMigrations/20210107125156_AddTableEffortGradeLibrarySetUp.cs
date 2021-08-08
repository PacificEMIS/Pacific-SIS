using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableEffortGradeLibrarySetUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "period_sort_order",
                table: "block_period",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    effort_category_id = table.Column<int>(nullable: false),
                    category_name = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_category", x => new { x.tenant_id, x.school_id, x.effort_category_id });
                });

            migrationBuilder.CreateTable(
                name: "effort_grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    effort_grade_scale_id = table.Column<int>(nullable: false),
                    grade_scale_value = table.Column<int>(nullable: true),
                    grade_scale_comment = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_scale", x => new { x.tenant_id, x.school_id, x.effort_grade_scale_id });
                });

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category_item",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    effort_category_id = table.Column<int>(nullable: false),
                    effort_item_id = table.Column<int>(nullable: false),
                    effort_item_title = table.Column<int>(nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_category_item", x => new { x.tenant_id, x.school_id, x.effort_category_id, x.effort_item_id });
                    table.ForeignKey(
                        name: "FK_effort_category_item_effort_category",
                        columns: x => new { x.tenant_id, x.school_id, x.effort_category_id },
                        principalTable: "effort_grade_library_category",
                        principalColumns: new[] { "tenant_id", "school_id", "effort_category_id" },
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "effort_grade_library_category_item");

            migrationBuilder.DropTable(
                name: "effort_grade_scale");

            migrationBuilder.DropTable(
                name: "effort_grade_library_category");

            migrationBuilder.DropColumn(
                name: "period_sort_order",
                table: "block_period");
        }
    }
}
