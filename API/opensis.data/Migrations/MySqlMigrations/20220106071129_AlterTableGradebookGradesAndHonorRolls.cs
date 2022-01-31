using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableGradebookGradesAndHonorRolls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "honor_rolls$FK_honor_rolls_honor_rolls",
                table: "honor_rolls");

            migrationBuilder.DropIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_",
                table: "honor_rolls");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_assignment",
                table: "gradebook_grades");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_studmast",
                table: "gradebook_grades");

            migrationBuilder.DropIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment",
                table: "gradebook_grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_grades_tenant_id",
                table: "gradebook_grades");

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_assignment",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" },
                principalTable: "assignment",
                principalColumns: new[] { "tenant_id", "school_id", "assignment_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_studmast",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" });            

            migrationBuilder.DropColumn(
                name: "marking_period_id",
                table: "honor_rolls");

            migrationBuilder.DropColumn(
                name: "marking_period_id",
                table: "gradebook_grades");

            migrationBuilder.AddColumn<int>(
                name: "qtr_marking_period_id",
                table: "gradebook_grades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "smstr_marking_period_id",
                table: "gradebook_grades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "yr_marking_period_id",
                table: "gradebook_grades",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_grades",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "student_id", "academic_year", "course_section_id", "assignment_type_id", "assignment_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_grades",
                table: "gradebook_grades");

            migrationBuilder.DropColumn(
                name: "qtr_marking_period_id",
                table: "gradebook_grades");

            migrationBuilder.DropColumn(
                name: "smstr_marking_period_id",
                table: "gradebook_grades");

            migrationBuilder.DropColumn(
                name: "yr_marking_period_id",
                table: "gradebook_grades");

            migrationBuilder.AddColumn<int>(
                name: "marking_period_id",
                table: "honor_rolls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "marking_period_id",
                table: "gradebook_grades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_grades_tenant_id",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "student_id", "academic_year", "marking_period_id", "course_section_id", "assignment_type_id", "assignment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "honor_rolls$FK_honor_rolls_honor_rolls",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
        }
    }
}
