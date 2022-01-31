using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AlterTableAssignmentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "assignment_type",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "qtr_marking_period_id",
                table: "assignment_type",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "smstr_marking_period_id",
                table: "assignment_type",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "yr_marking_period_id",
                table: "assignment_type",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_assignment_type_tenant_id_school_id_prgrsprd_marking_period_id",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_type_tenant_id_school_id_qtr_marking_period_id",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_type_tenant_id_school_id_smstr_marking_period_id",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_type_tenant_id_school_id_yr_marking_period_id",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_type_progress_periods",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_type_quarters",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_type_school_years",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_assignment_type_semesters",
                table: "assignment_type",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assignment_type_progress_periods",
                table: "assignment_type");

            migrationBuilder.DropForeignKey(
                name: "FK_assignment_type_quarters",
                table: "assignment_type");

            migrationBuilder.DropForeignKey(
                name: "FK_assignment_type_school_years",
                table: "assignment_type");

            migrationBuilder.DropForeignKey(
                name: "FK_assignment_type_semesters",
                table: "assignment_type");

            migrationBuilder.DropIndex(
                name: "IX_assignment_type_tenant_id_school_id_prgrsprd_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropIndex(
                name: "IX_assignment_type_tenant_id_school_id_qtr_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropIndex(
                name: "IX_assignment_type_tenant_id_school_id_smstr_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropIndex(
                name: "IX_assignment_type_tenant_id_school_id_yr_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropColumn(
                name: "qtr_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropColumn(
                name: "smstr_marking_period_id",
                table: "assignment_type");

            migrationBuilder.DropColumn(
                name: "yr_marking_period_id",
                table: "assignment_type");
        }
    }
}
