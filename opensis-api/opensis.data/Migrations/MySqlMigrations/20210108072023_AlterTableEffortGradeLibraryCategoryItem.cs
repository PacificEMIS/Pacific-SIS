using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableEffortGradeLibraryCategoryItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "effort_item_title",
                table: "effort_grade_library_category_item",
                unicode: false,
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "effort_item_title",
                table: "effort_grade_library_category_item",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
