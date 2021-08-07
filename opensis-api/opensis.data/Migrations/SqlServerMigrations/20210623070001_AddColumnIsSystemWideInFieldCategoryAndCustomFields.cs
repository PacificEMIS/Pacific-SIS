using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddColumnIsSystemWideInFieldCategoryAndCustomFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "permission_subcategory",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "permission_category",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_system_wide_category",
                table: "fields_category",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_system_wide_field",
                table: "custom_fields",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "permission_subcategory");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "permission_category");

            migrationBuilder.DropColumn(
                name: "is_system_wide_category",
                table: "fields_category");

            migrationBuilder.DropColumn(
                name: "is_system_wide_field",
                table: "custom_fields");
        }
    }
}
