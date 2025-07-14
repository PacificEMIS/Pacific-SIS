using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class RenameTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_configuration_progressPeriod",
                table: "gradebook_configuration_progressPeriod");

            migrationBuilder.RenameTable(
                name: "gradebook_configuration_progressPeriod",
                newName: "gradebook_configuration_progressperiod");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressperiod",
                newName: "IX_gradebook_configuration_progressperiod_tenant_id_school_id_p~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_c~",
                table: "gradebook_configuration_progressperiod",
                newName: "IX_gradebook_configuration_progressperiod_tenant_id_school_id_c~");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_configuration_progressperiod",
                table: "gradebook_configuration_progressperiod",
                column: "id");

            migrationBuilder.RenameTable(
                name: "__EFMigrationHistory",
                newName: "__efmigrationhistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_configuration_progressperiod",
                table: "gradebook_configuration_progressperiod");

            migrationBuilder.RenameTable(
                name: "gradebook_configuration_progressperiod",
                newName: "gradebook_configuration_progressPeriod");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_progressperiod_tenant_id_school_id_p~",
                table: "gradebook_configuration_progressPeriod",
                newName: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_p~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_progressperiod_tenant_id_school_id_c~",
                table: "gradebook_configuration_progressPeriod",
                newName: "IX_gradebook_configuration_progressPeriod_tenant_id_school_id_c~");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_configuration_progressPeriod",
                table: "gradebook_configuration_progressPeriod",
                column: "id");

            migrationBuilder.RenameTable(
                name: "__efmigrationhistory",
                newName: "__EFMigrationHistory");
        }
    }
}
