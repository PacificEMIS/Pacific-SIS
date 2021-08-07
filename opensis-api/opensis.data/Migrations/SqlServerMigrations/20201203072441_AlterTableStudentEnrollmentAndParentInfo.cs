using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AlterTableStudentEnrollmentAndParentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_gradelevels",
                table: "student_enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_sections",
                table: "student_enrollment");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_section_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "calendar_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "drop_code",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "grade_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "last_school",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "next_school",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "section_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "email",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "portal_user_id",
                table: "parent_info");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "student_enrollment",
                newName: "exit_date");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "student_enrollment",
                newName: "enrollment_date");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "student_master",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "student_master",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "enrollment_code",
                table: "student_enrollment",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "calender_id",
                table: "student_enrollment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "exit_code",
                table: "student_enrollment",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade_level_title",
                table: "student_enrollment",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rolling_option",
                table: "student_enrollment",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "LOV of N/A, Transferred In,Rolled Over,New");

            migrationBuilder.AddColumn<string>(
                name: "school_name",
                table: "student_enrollment",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "school_transferred",
                table: "student_enrollment",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transferred_grade",
                table: "student_enrollment",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "login_email",
                table: "parent_info",
                unicode: false,
                maxLength: 150,
                nullable: true,
                comment: "emailaddress mapped to user_master");

            migrationBuilder.AddColumn<string>(
                name: "middlename",
                table: "parent_info",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "personal_email",
                table: "parent_info",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "salutation",
                table: "parent_info",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "suffix",
                table: "parent_info",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_profile",
                table: "parent_info",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_email",
                table: "parent_info",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_calender_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "calender_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_school_calendars",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "calender_id" },
                principalTable: "school_calendars",
                principalColumns: new[] { "tenant_id", "school_id", "calender_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_school_calendars",
                table: "student_enrollment");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_calender_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "calender_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "exit_code",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "grade_level_title",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "rolling_option",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "school_name",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "school_transferred",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "transferred_grade",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "login_email",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "middlename",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "personal_email",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "salutation",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "suffix",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "user_profile",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "work_email",
                table: "parent_info");

            migrationBuilder.RenameColumn(
                name: "exit_date",
                table: "student_enrollment",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "enrollment_date",
                table: "student_enrollment",
                newName: "end_date");

            migrationBuilder.AlterColumn<int>(
                name: "enrollment_code",
                table: "student_enrollment",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "calendar_id",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "drop_code",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "grade_id",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "last_school",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "next_school",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "section_id",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "parent_info",
                type: "varchar(150)",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "portal_user_id",
                table: "parent_info",
                type: "varchar(150)",
                unicode: false,
                maxLength: 150,
                nullable: true,
                comment: "emailaddress mapped to user_master");

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_section_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_gradelevels",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" },
                principalTable: "gradelevels",
                principalColumns: new[] { "tenant_id", "school_id", "grade_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_sections",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "section_id" },
                principalTable: "sections",
                principalColumns: new[] { "tenant_id", "school_id", "section_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
