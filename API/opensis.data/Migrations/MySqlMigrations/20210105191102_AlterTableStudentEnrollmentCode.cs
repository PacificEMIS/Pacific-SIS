using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableStudentEnrollmentCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_code_school_master_SchoolMasterTenantId_S~",
                table: "student_enrollment_code");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_code_SchoolMasterTenantId_SchoolMasterSch~",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "SchoolMasterSchoolId",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "SchoolMasterTenantId",
                table: "student_enrollment_code");

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_code_school_master1",
                table: "student_enrollment_code",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_code_school_master1",
                table: "student_enrollment_code");

            migrationBuilder.AddColumn<int>(
                name: "SchoolMasterSchoolId",
                table: "student_enrollment_code",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolMasterTenantId",
                table: "student_enrollment_code",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_code_SchoolMasterTenantId_SchoolMasterSch~",
                table: "student_enrollment_code",
                columns: new[] { "SchoolMasterTenantId", "SchoolMasterSchoolId" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_code_school_master_SchoolMasterTenantId_S~",
                table: "student_enrollment_code",
                columns: new[] { "SchoolMasterTenantId", "SchoolMasterSchoolId" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
