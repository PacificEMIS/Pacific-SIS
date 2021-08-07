using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddColumnCreatedByAndCreatedOnInMasterTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "student_master",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "student_master",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "staff_master",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "staff_master",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                table: "school_master",
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
                name: "created_by",
                table: "school_master",
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

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "parent_info",
                unicode: false,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "parent_info",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "created_on",
                table: "parent_info");

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                table: "school_master",
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
                name: "created_by",
                table: "school_master",
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
        }
    }
}
