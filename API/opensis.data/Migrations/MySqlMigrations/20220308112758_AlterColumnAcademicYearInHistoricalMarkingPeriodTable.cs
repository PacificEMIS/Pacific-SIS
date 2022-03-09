using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterColumnAcademicYearInHistoricalMarkingPeriodTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "academic_year",
                table: "historical_marking_period",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(decimal),
                oldType: "decimal(4,0)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "academic_year",
                table: "historical_marking_period",
                type: "decimal(4,0)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");
        }
    }
}
