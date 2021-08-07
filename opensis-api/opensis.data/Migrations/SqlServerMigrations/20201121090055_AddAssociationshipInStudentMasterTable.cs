using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddAssociationshipInStudentMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "associationship",
                table: "student_master",
                unicode: false,
                nullable: true,
                comment: "tenantid#schoolid#studentid | tenantid#schoolid#studentid | ....");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "associationship",
                table: "student_master");
        }
    }
}
