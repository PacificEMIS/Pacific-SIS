using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableStaffMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "dob",
                table: "staff_master",
                type: "date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dob",
                table: "staff_master");
        }
    }
}
