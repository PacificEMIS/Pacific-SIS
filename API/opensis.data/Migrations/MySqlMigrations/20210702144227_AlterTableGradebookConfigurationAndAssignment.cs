using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableGradebookConfigurationAndAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_assignment",
                table: "assignment");

            migrationBuilder.AddColumn<int>(
                name: "sem1_exam",
                table: "gradebook_configuration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem2_exam",
                table: "gradebook_configuration",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_assignment",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type_id",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_type_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_assignment",
                table: "assignment");

            migrationBuilder.DropIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type_id",
                table: "assignment");

            migrationBuilder.DropColumn(
                name: "sem1_exam",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem2_exam",
                table: "gradebook_configuration");

            migrationBuilder.AddPrimaryKey(
                name: "PK_assignment",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_type_id", "assignment_id" });
        }
    }
}
