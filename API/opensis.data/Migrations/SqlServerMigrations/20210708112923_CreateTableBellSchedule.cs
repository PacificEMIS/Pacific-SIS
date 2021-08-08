using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateTableBellSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_configuration",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "final_grading_qtr1",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "final_grading_qtr2",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "final_grading_qtr3",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "final_grading_qtr4",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "fullyear_sem1",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "fullyear_sem2",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_a",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_b",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_c",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_d",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_e",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_f",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "score_breakoff_points_inc",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem1_exam",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem1_qtr1",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem1_qtr2",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem2_exam",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem2_qtr3",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "sem2_qtr4",
                table: "gradebook_configuration");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "user_secret_questions",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "user_master",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "student_master",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "student_enrollment_code",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "student_enrollment",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "student_comments",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "staff_school_info",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated_by",
                table: "staff_master",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "staff_master",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "staff_certificate_info",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "semesters",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "sections",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "modified_by",
                table: "search_filter",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "date_modifed",
                table: "search_filter",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "search_filter",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "school_years",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "modified_by",
                table: "school_master",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "date_modifed",
                table: "school_master",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "school_master",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "school_calendars",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "rooms",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "quarters",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "progress_periods",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "parent_info",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "parent_associationship",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "parent_address",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "created_time",
                table: "notice",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "gradelevels",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "date_modifed",
                table: "gradebook_configuration",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "gradebook_configuration",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "last_update",
                table: "fields_category",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_update",
                table: "custom_fields_value",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_update",
                table: "custom_fields",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "calendar_events",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "attendance_code_categories",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "last_updated",
                table: "attendance_code",
                newName: "updated_on");

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "user_secret_questions",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "user_secret_questions",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "user_secret_questions",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "user_master",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "user_master",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "user_master",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_schedule_view",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_schedule_view",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "student_schedule_view",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "student_schedule_view",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_enrollment_code",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_enrollment_code",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_enrollment_code",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_enrollment",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_enrollment",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_enrollment",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_documents",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_documents",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "student_documents",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "student_documents",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_comments",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_comments",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_comments",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "staff_school_info",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "staff_school_info",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "staff_school_info",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "staff_schedule_view",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "staff_schedule_view",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "staff_schedule_view",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "staff_schedule_view",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "staff_master",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "staff_certificate_info",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "staff_certificate_info",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "staff_certificate_info",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "semesters",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "semesters",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "semesters",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "sections",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "sections",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "sections",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "search_filter",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(50)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "search_filter",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(50)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "school_years",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "school_years",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "school_years",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "school_master",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(150)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "school_master",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(150)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "school_detail",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "school_detail",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "school_detail",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "school_detail",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "school_calendars",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "school_calendars",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "school_calendars",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "rooms",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "rooms",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "rooms",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "release_number",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "release_number",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "release_number",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "release_number",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "quarters",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "quarters",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "quarters",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "progress_periods",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "progress_periods",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "progress_periods",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "plans",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "plans",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "plans",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "plans",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "permission_group",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "permission_group",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "permission_group",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "permission_group",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_info",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_associationship",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "parent_associationship",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "parent_associationship",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_address",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "parent_address",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "parent_address",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "notice",
                unicode: false,
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "notice",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "notice",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "login_session",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "login_session",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "login_session",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "login_session",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "gradelevels",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "gradelevels",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "gradelevels",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "gradebook_configuration",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(150)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gradebook_configuration_id",
                table: "gradebook_configuration",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "gradebook_configuration",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "grade_equivalency",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "grade_equivalency",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "grade_equivalency",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "grade_equivalency",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "grade_educational_stage",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "grade_educational_stage",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "grade_educational_stage",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "grade_educational_stage",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "grade_age_range",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "grade_age_range",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "grade_age_range",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "grade_age_range",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "fields_category",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "fields_category",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "fields_category",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "custom_fields_value",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "custom_fields_value",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "custom_fields_value",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "custom_fields",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "custom_fields",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "custom_fields",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "calendar_events",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "calendar_events",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "calendar_events",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "attendance_code_categories",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "attendance_code_categories",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "attendance_code_categories",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "attendance_code",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "attendance_code",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "attendance_code",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_configuration",
                table: "gradebook_configuration",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateTable(
                name: "bell_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    bell_schedule_date = table.Column<DateTime>(type: "date", nullable: false),
                    block_id = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bell_schedule", x => new { x.tenant_id, x.school_id, x.academic_year, x.bell_schedule_date });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_final_grading",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(nullable: false),
                    yr_marking_period_id = table.Column<int>(nullable: true),
                    smstr_marking_period_id = table.Column<int>(nullable: true),
                    qtr_marking_period_id = table.Column<int>(nullable: true),
                    grading_percentage = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_final_grading", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_final_grading_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_gradescale",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    course_id = table.Column<int>(nullable: false),
                    course_section_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(nullable: false),
                    grade_scale_id = table.Column<int>(nullable: false),
                    grade_id = table.Column<int>(nullable: false),
                    breakoff_points = table.Column<int>(nullable: false),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_gradescale", x => x.id);
                    table.ForeignKey(
                        name: "FK_gradebook_configuration_gradescale_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_qtr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_smstr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_yr_marking_period_id",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_final_grading_tenant_id_school_id_course_id_course_section_id_academic_year_gradebook_configuration_~",
                table: "gradebook_configuration_final_grading",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_gradescale",
                table: "gradebook_configuration_gradescale",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_gradebook_configuration_course_section",
                table: "gradebook_configuration",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gradebook_configuration_course_section",
                table: "gradebook_configuration");

            migrationBuilder.DropTable(
                name: "bell_schedule");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_final_grading");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_gradescale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gradebook_configuration",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "user_secret_questions");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "user_secret_questions");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "user_master");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_schedule_view");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_schedule_view");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "student_schedule_view");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "student_schedule_view");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_enrollment_code");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_documents");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_documents");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "student_documents");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "student_documents");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_comments");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_comments");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "staff_school_info");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "staff_school_info");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "staff_schedule_view");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "staff_schedule_view");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "staff_schedule_view");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "staff_schedule_view");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "staff_certificate_info");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "staff_certificate_info");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "semesters");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "semesters");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "school_years");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "school_years");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "school_detail");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "school_detail");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "school_detail");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "school_detail");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "school_calendars");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "school_calendars");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "release_number");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "release_number");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "release_number");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "release_number");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "quarters");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "quarters");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "progress_periods");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "progress_periods");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "permission_group");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "permission_group");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "permission_group");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "permission_group");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "parent_associationship");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "parent_associationship");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "parent_address");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "parent_address");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "notice");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "notice");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "login_session");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "login_session");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "login_session");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "login_session");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "gradebook_configuration_id",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "gradebook_configuration");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "grade_equivalency");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "grade_equivalency");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "grade_equivalency");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "grade_equivalency");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "grade_educational_stage");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "grade_educational_stage");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "grade_educational_stage");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "grade_educational_stage");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "grade_age_range");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "grade_age_range");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "grade_age_range");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "grade_age_range");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "fields_category");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "fields_category");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "custom_fields_value");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "custom_fields_value");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "custom_fields");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "custom_fields");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "calendar_events");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "calendar_events");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "attendance_code_categories");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "attendance_code_categories");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "attendance_code");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "attendance_code");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "user_secret_questions",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "user_master",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "student_master",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "student_enrollment_code",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "student_enrollment",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "student_comments",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "staff_school_info",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "staff_master",
                newName: "last_updated_by");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "staff_master",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "staff_certificate_info",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "semesters",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "sections",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "search_filter",
                newName: "modified_by");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "search_filter",
                newName: "date_modifed");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "search_filter",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "school_years",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "school_master",
                newName: "modified_by");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "school_master",
                newName: "date_modifed");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "school_master",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "school_calendars",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "rooms",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "quarters",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "progress_periods",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "parent_info",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "parent_associationship",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "parent_address",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "notice",
                newName: "created_time");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "gradelevels",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "gradebook_configuration",
                newName: "date_modifed");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "gradebook_configuration",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "fields_category",
                newName: "last_update");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "custom_fields_value",
                newName: "last_update");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "custom_fields",
                newName: "last_update");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "calendar_events",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "attendance_code_categories",
                newName: "last_updated");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "attendance_code",
                newName: "last_updated");

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "user_secret_questions",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "user_master",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_enrollment_code",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_enrollment",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "student_comments",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "staff_school_info",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_updated_by",
                table: "staff_master",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "staff_certificate_info",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "semesters",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "sections",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "search_filter",
                type: "char(50)",
                unicode: false,
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                table: "search_filter",
                type: "char(50)",
                unicode: false,
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "school_years",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "school_master",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                table: "school_master",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "school_calendars",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "rooms",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "quarters",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "progress_periods",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_info",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_associationship",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "parent_address",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "notice",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "gradelevels",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "gradebook_configuration",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "final_grading_qtr1",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "final_grading_qtr2",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "final_grading_qtr3",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "final_grading_qtr4",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fullyear_sem1",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fullyear_sem2",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modified_by",
                table: "gradebook_configuration",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_a",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_b",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_c",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_d",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_e",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_f",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "score_breakoff_points_inc",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem1_exam",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem1_qtr1",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem1_qtr2",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem2_exam",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem2_qtr3",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sem2_qtr4",
                table: "gradebook_configuration",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "fields_category",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "custom_fields_value",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "custom_fields",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "calendar_events",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "attendance_code_categories",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "attendance_code",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_gradebook_configuration",
                table: "gradebook_configuration",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year" });
        }
    }
}
