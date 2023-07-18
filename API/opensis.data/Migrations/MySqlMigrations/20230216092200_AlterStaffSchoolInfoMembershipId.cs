using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterStaffSchoolInfoMembershipId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info");

            migrationBuilder.DropIndex(
                name: "IX_staff_school_info_tenant_id_school_id_membership_id",
                table: "staff_school_info");

            migrationBuilder.CreateIndex(
                name: "IX_staff_school_info_tenant_id_school_attached_id_membership_id",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_attached_id", "membership_id" });

            migrationBuilder.Sql(@"SET FOREIGN_KEY_CHECKS = 0;");

            migrationBuilder.AddForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_attached_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.Sql(@"SET FOREIGN_KEY_CHECKS = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info");

            migrationBuilder.DropIndex(
                name: "IX_staff_school_info_tenant_id_school_attached_id_membership_id",
                table: "staff_school_info");

            migrationBuilder.CreateIndex(
                name: "IX_staff_school_info_tenant_id_school_id_membership_id",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
        }
    }
}
