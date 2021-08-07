using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AlterColumnGenderInSchoolDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "gender",
                table: "school_detail",
                fixedLength: true,
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(6)",
                oldFixedLength: true,
                oldMaxLength: 6,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "gender",
                table: "school_detail",
                type: "nchar(6)",
                fixedLength: true,
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 15,
                oldNullable: true);
        }
    }
}
