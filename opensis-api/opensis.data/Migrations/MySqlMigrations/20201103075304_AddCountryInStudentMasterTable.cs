using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddCountryInStudentMasterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "home_address_country",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mailing_address_country",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_country",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_country",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "home_address_country",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "mailing_address_country",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_country",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_country",
                table: "student_master");
        }
    }
}
