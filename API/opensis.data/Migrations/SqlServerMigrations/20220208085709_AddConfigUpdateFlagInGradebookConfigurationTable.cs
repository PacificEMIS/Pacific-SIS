using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddConfigUpdateFlagInGradebookConfigurationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weighted_gp",
                table: "historical_credit_transfer");

            migrationBuilder.AddColumn<string>(
                name: "course_type",
                table: "historical_credit_transfer",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "config_update_flag",
                table: "gradebook_configuration",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "course_type",
                table: "historical_credit_transfer");

            migrationBuilder.DropColumn(
                name: "config_update_flag",
                table: "gradebook_configuration");

            migrationBuilder.AddColumn<bool>(
                name: "weighted_gp",
                table: "historical_credit_transfer",
                type: "bit",
                nullable: true);
        }
    }
}
