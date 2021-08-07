using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddColumnIsTenantadminInMembershipTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "last_used_school_id",
                table: "user_master",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_tenantadmin",
                table: "membership",
                nullable: true,
                comment: "valid for only tenantwise superadmin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_used_school_id",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "is_tenantadmin",
                table: "membership");
        }
    }
}
