using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableParentAssociationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "associationship",
                table: "parent_info");

            migrationBuilder.AddColumn<bool>(
                name: "bus_dropoff",
                table: "staff_master",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bus_no",
                table: "staff_master",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "bus_pickup",
                table: "staff_master",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disability_description",
                table: "staff_master",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "staff_photo",
                table: "staff_master",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "parent_photo",
                table: "parent_info",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "parent_associationship",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    parent_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    associationship = table.Column<bool>(nullable: false, comment: "tenantid#schoolid#studentid | tenantid#schoolid#studentid | ...."),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_associationship", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parent_associationship");

            migrationBuilder.DropColumn(
                name: "bus_dropoff",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "bus_no",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "bus_pickup",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "disability_description",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "staff_photo",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "parent_photo",
                table: "parent_info");

            migrationBuilder.AddColumn<string>(
                name: "associationship",
                table: "parent_info",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                comment: "tenantid#schoolid#studentid | tenantid#schoolid#studentid | ....");
        }
    }
}
