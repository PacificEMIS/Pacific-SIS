using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddGUIDinStudentSchoolParentAndStaffTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "home_school_id",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "home_student_id",
                table: "student_enrollment");

            migrationBuilder.AddColumn<Guid>(
                name: "student_guid",
                table: "student_master",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "student_guid",
                table: "student_enrollment",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "state",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "state",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "state",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "state",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "staff_guid",
                table: "staff_master",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "school_guid",
                table: "school_master",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "parent_guid",
                table: "parent_info",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "language",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "language",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "language",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "language",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "country",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "country",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "country",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "country",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "city",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "city",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "city",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_on",
                table: "city",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "student_guid",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "student_guid",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "state");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "state");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "state");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "state");

            migrationBuilder.DropColumn(
                name: "staff_guid",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "school_guid",
                table: "school_master");

            migrationBuilder.DropColumn(
                name: "parent_guid",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "language");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "language");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "language");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "language");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "country");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "country");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "country");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "country");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "city");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "city");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "city");

            migrationBuilder.DropColumn(
                name: "updated_on",
                table: "city");

            migrationBuilder.AddColumn<int>(
                name: "home_school_id",
                table: "student_enrollment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "home_student_id",
                table: "student_enrollment",
                type: "int",
                nullable: true);
        }
    }
}
