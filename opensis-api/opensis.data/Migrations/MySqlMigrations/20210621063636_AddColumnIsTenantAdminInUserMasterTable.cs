using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddColumnIsTenantAdminInUserMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_tenantadmin",
                table: "membership");

            migrationBuilder.AddColumn<bool>(
                name: "is_tenantadmin",
                table: "user_master",
                nullable: true,
                comment: "valid for only tenantwise superadmin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_tenantadmin",
                table: "user_master");

            migrationBuilder.AddColumn<bool>(
                name: "is_tenantadmin",
                table: "membership",
                type: "tinyint(1)",
                nullable: true,
                comment: "valid for only tenantwise superadmin");
        }
    }
}
