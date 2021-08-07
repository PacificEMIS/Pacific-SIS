using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableParentInfoAndStudentComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "state_id",
                table: "student_master",
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "district_id",
                table: "student_master",
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "student_comments",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "student_comments",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bus_No",
                table: "parent_info",
                unicode: false,
                maxLength: 15,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "student_comments");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "student_comments");

            migrationBuilder.DropColumn(
                name: "bus_No",
                table: "parent_info");

            migrationBuilder.AlterColumn<int>(
                name: "state_id",
                table: "student_master",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "district_id",
                table: "student_master",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
