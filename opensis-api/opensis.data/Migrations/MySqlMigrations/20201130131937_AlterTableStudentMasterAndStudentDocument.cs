using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableStudentMasterAndStudentDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "state_id",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(50) CHARACTER SET utf8mb4",
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "district_id",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(50) CHARACTER SET utf8mb4",
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "economic_disadvantage",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "eligibility_504",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "estimated_grad_date",
                table: "student_master",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "free_lunch_eligibility",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "lep_indicator",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "section_id",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "special_education_indicator",
                table: "student_master",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "student_internal_id",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "filename",
                table: "student_documents",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "filetype",
                table: "student_documents",
                unicode: false,
                fixedLength: true,
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_master_tenant_id_school_id_section_id",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_master_sections",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "section_id" },
                principalTable: "sections",
                principalColumns: new[] { "tenant_id", "school_id", "section_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_master_sections",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_master_tenant_id_school_id_section_id",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "economic_disadvantage",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "eligibility_504",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "estimated_grad_date",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "free_lunch_eligibility",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "lep_indicator",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "section_id",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "special_education_indicator",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "student_internal_id",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "filename",
                table: "student_documents");

            migrationBuilder.DropColumn(
                name: "filetype",
                table: "student_documents");

            migrationBuilder.AlterColumn<string>(
                name: "state_id",
                table: "student_master",
                type: "char(50) CHARACTER SET utf8mb4",
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "district_id",
                table: "student_master",
                type: "char(50) CHARACTER SET utf8mb4",
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
