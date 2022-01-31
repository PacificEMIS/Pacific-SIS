using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddPrgrsPrdMarkingPeriodIdInCourseSectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "gradebook_grades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prgrsprd_marking_period_id",
                table: "course_section",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "prgrsprd_marking_period_id", "academic_year", "qtr_marking_period_id" },
                principalTable: "progress_periods",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id", "academic_year", "quarter_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_course_section_progress_periods",
                table: "course_section");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_prgrsprd_marking_period_id_academic_year_qtr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "gradebook_grades");

            migrationBuilder.DropColumn(
                name: "prgrsprd_marking_period_id",
                table: "course_section");
        }
    }
}
