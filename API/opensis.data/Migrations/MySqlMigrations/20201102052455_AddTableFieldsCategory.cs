using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableFieldsCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_custom_fields_school_master",
                table: "custom_fields");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "custom_fields");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "custom_fields",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "Datatype",
                oldClrType: typeof(string),
                oldType: "varchar(50) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "custom_fields",
                unicode: false,
                maxLength: 30,
                nullable: true,
                comment: "Field Name",
                oldClrType: typeof(string),
                oldType: "varchar(30) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "system_field",
                table: "custom_fields",
                nullable: true,
                comment: "wheher it is applicable throughput all forms",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "select_options",
                table: "custom_fields",
                unicode: false,
                nullable: true,
                comment: "LOV for dropdown separated by | character.",
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "required",
                table: "custom_fields",
                nullable: true,
                comment: "Whether value input is required",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "hide",
                table: "custom_fields",
                nullable: true,
                comment: "hide the custom field on UI",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "default_selection",
                table: "custom_fields",
                unicode: false,
                maxLength: 100,
                nullable: true,
                comment: "default value selection on form load",
                oldClrType: typeof(string),
                oldType: "varchar(100) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "custom_fields",
                nullable: false,
                comment: "Take categoryid from custom_category table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "module",
                table: "custom_fields",
                unicode: false,
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                comment: "module like \"school\", \"student\" etc.");

            migrationBuilder.AddPrimaryKey(
                name: "PK_custom_fields",
                table: "custom_fields",
                columns: new[] { "tenant_id", "school_id", "category_id", "field_id" });

            migrationBuilder.CreateTable(
                name: "fields_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    category_id = table.Column<int>(nullable: false),
                    is_system_category = table.Column<bool>(nullable: true),
                    search = table.Column<bool>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    module = table.Column<string>(unicode: false, fixedLength: true, maxLength: 10, nullable: true, comment: "module like \"school\", \"student\" etc."),
                    sort_order = table.Column<int>(nullable: true),
                    required = table.Column<bool>(nullable: true),
                    hide = table.Column<bool>(nullable: true),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_category", x => new { x.tenant_id, x.school_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_custom_category_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddForeignKey(
               name: "FK_custom_fields_custom_category",
               table: "custom_fields",
               columns: new[] { "tenant_id", "school_id", "category_id" },
               principalTable: "fields_category",
               principalColumns: new[] { "tenant_id", "school_id", "category_id" },
               onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
               name: "FK_custom_fields_school_master",
               table: "custom_fields",
               columns: new[] { "tenant_id", "school_id" },
               principalTable: "school_master",
               principalColumns: new[] { "tenant_id", "school_id" },
               onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custom_fields_custom_category",
                table: "custom_fields");

            migrationBuilder.DropTable(
                name: "fields_category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_custom_fields",
                table: "custom_fields");

            migrationBuilder.DropColumn(
                name: "module",
                table: "custom_fields");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "custom_fields",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Datatype");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "custom_fields",
                type: "varchar(30) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true,
                oldComment: "Field Name");

            migrationBuilder.AlterColumn<bool>(
                name: "system_field",
                table: "custom_fields",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldComment: "wheher it is applicable throughput all forms");

            migrationBuilder.AlterColumn<string>(
                name: "select_options",
                table: "custom_fields",
                type: "longtext CHARACTER SET utf8mb4",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldNullable: true,
                oldComment: "LOV for dropdown separated by | character.");

            migrationBuilder.AlterColumn<bool>(
                name: "required",
                table: "custom_fields",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldComment: "Whether value input is required");

            migrationBuilder.AlterColumn<bool>(
                name: "hide",
                table: "custom_fields",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldComment: "hide the custom field on UI");

            migrationBuilder.AlterColumn<string>(
                name: "default_selection",
                table: "custom_fields",
                type: "varchar(100) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "default value selection on form load");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "custom_fields",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldComment: "Take categoryid from custom_category table");

            migrationBuilder.AddPrimaryKey(
                name: "PK_custom_fields",
                table: "custom_fields",
                columns: new[] { "tenant_id", "school_id", "field_id" });
        }
    }
}
