using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableParentAddressAndAlterParentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parent_info_student_master",
                table: "parent_info");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "address_line_one",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "address_line_two",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "city",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "country",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "is_custodian",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "relationship",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "state",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "student_address_same",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "zip",
                table: "parent_info");

            migrationBuilder.AddColumn<bool>(
                name: "is_custodian",
                table: "parent_associationship",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "relationship",
                table: "parent_associationship",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_parent_info_1",
                table: "parent_info",
                columns: new[] { "tenant_id", "school_id", "parent_id" });

            migrationBuilder.CreateTable(
                name: "dpdown_valuelist",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    lov_name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    lov_column_value = table.Column<string>(unicode: false, nullable: false),
                    created_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dpdown_valuelist", x => x.id);
                    table.ForeignKey(
                        name: "FK_dpdown_valuelist_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parent_address",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    parent_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    student_address_same = table.Column<bool>(nullable: false),
                    address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_address_1", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                    table.ForeignKey(
                        name: "FK_parent_address_parent_info",
                        columns: x => new { x.tenant_id, x.school_id, x.parent_id },
                        principalTable: "parent_info",
                        principalColumns: new[] { "tenant_id", "school_id", "parent_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parent_address_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dpdown_valuelist_tenant_id_school_id",
                table: "dpdown_valuelist",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_parent_address_tenant_id_school_id_student_id",
                table: "parent_address",
                columns: new[] { "tenant_id", "school_id", "student_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dpdown_valuelist");

            migrationBuilder.DropTable(
                name: "parent_address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_parent_info_1",
                table: "parent_info");

            migrationBuilder.DropColumn(
                name: "is_custodian",
                table: "parent_associationship");

            migrationBuilder.DropColumn(
                name: "relationship",
                table: "parent_associationship");

            migrationBuilder.AddColumn<int>(
                name: "student_id",
                table: "parent_info",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "address_line_one",
                table: "parent_info",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_line_two",
                table: "parent_info",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "parent_info",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "parent_info",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_custodian",
                table: "parent_info",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "relationship",
                table: "parent_info",
                type: "varchar(20) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "parent_info",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "student_address_same",
                table: "parent_info",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zip",
                table: "parent_info",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_parent_info",
                table: "parent_info",
                columns: new[] { "tenant_id", "school_id", "student_id", "parent_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_parent_info_student_master",
                table: "parent_info",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
