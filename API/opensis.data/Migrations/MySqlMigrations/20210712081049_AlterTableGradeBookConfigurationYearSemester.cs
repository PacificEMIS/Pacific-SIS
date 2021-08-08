using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableGradeBookConfigurationYearSemester : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "smstr_marking_period_id",
                table: "gradebook_configuration_year",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "qtr_marking_period_id",
                table: "gradebook_configuration_semester",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_smstr_marki~",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_qtr_mar~",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_configuration_semester_quarters",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_configuration_year_semesters",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_configuration_semester_quarters",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_configuration_year_semesters",
                table: "gradebook_configuration_year");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_smstr_marki~",
                table: "gradebook_configuration_year");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_qtr_mar~",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropColumn(
                name: "smstr_marking_period_id",
                table: "gradebook_configuration_year");

            migrationBuilder.DropColumn(
                name: "qtr_marking_period_id",
                table: "gradebook_configuration_semester");
        }
    }
}
