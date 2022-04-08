using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddIndexOnParentInfoAndParentAssociationshipTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_parent_info_tenant_id_parent_id",
                table: "parent_info",
                columns: new[] { "tenant_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "IX_parent_associationship_tenant_id_parent_id_asso",
                table: "parent_associationship",
                columns: new[] { "tenant_id", "parent_id", "associationship" });

            migrationBuilder.CreateIndex(
                name: "IX_parent_associationship_tenant_id_school_id_student_id_asso",
                table: "parent_associationship",
                columns: new[] { "tenant_id", "school_id", "student_id", "associationship" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_parent_info_tenant_id_parent_id",
                table: "parent_info");

            migrationBuilder.DropIndex(
                name: "IX_parent_associationship_tenant_id_parent_id_asso",
                table: "parent_associationship");

            migrationBuilder.DropIndex(
                name: "IX_parent_associationship_tenant_id_school_id_student_id_asso",
                table: "parent_associationship");
        }
    }
}
