using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddCustomFieldsValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custom_fields_custom_category",
                table: "custom_fields");

            migrationBuilder.AlterColumn<int>(
                name: "third_language_id",
                table: "student_master",
                nullable: true,
                comment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "second_language_id",
                table: "student_master",
                nullable: true,
                comment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "first_language_id",
                table: "student_master",
                nullable: true,
                comment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "student_portal_id",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "system_wide_event",
                table: "calendar_events",
                nullable: true,
                comment: "event applicable to all calenders within academic year");

            migrationBuilder.CreateTable(
                name: "custom_fields_value",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    category_id = table.Column<int>(nullable: false),
                    field_id = table.Column<int>(nullable: false),
                    target_id = table.Column<int>(nullable: false, comment: "Target_is school/student/staff id for whom custom field value is entered. For School module it will be always school id."),
                    module = table.Column<string>(unicode: false, fixedLength: true, maxLength: 10, nullable: false, comment: "'Student' | 'School' | 'Staff'"),
                    custom_field_title = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    custom_field_type = table.Column<string>(unicode: false, maxLength: 50, nullable: true, comment: "'Select' or 'Text'"),
                    custom_field_value = table.Column<string>(unicode: false, nullable: true, comment: "User input value...Textbox->textvalue, Select-->Value separated by '|', Date --> Date in string"),
                    last_update = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields_value", x => new { x.tenant_id, x.school_id, x.category_id, x.field_id, x.target_id, x.module });
                });

            migrationBuilder.AddForeignKey(
                name: "FK_custom_fields_fields_category",
                table: "custom_fields",
                columns: new[] { "tenant_id", "school_id", "category_id" },
                principalTable: "fields_category",
                principalColumns: new[] { "tenant_id", "school_id", "category_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custom_fields_fields_category",
                table: "custom_fields");

            migrationBuilder.DropTable(
                name: "custom_fields_value");

            migrationBuilder.DropColumn(
                name: "student_portal_id",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "system_wide_event",
                table: "calendar_events");

            migrationBuilder.AlterColumn<int>(
                name: "third_language_id",
                table: "student_master",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldComment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.");

            migrationBuilder.AlterColumn<int>(
                name: "second_language_id",
                table: "student_master",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldComment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.");

            migrationBuilder.AlterColumn<int>(
                name: "first_language_id",
                table: "student_master",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldComment: "Plan is language will be displayed in dropdown from language table and selected corresponding id will be stored into table.");

            migrationBuilder.AddForeignKey(
                name: "FK_custom_fields_custom_category",
                table: "custom_fields",
                columns: new[] { "tenant_id", "school_id", "category_id" },
                principalTable: "fields_category",
                principalColumns: new[] { "tenant_id", "school_id", "category_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
