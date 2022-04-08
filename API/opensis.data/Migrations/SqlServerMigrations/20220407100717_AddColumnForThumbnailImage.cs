using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddColumnForThumbnailImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "student_thumbnail_photo",
                table: "student_master",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "staff_thumbnail_photo",
                table: "staff_master",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "school_thumbnail_logo",
                table: "school_detail",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "parent_thumbnail_photo",
                table: "parent_info",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "student_thumbnail_photo",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "staff_thumbnail_photo",
                table: "staff_master");

            migrationBuilder.DropColumn(
                name: "school_thumbnail_logo",
                table: "school_detail");

            migrationBuilder.DropColumn(
                name: "parent_thumbnail_photo",
                table: "parent_info");
        }
    }
}
