using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class CreateTableLoginSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "login_session",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    emailaddress = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    ipaddress = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    token = table.Column<string>(unicode: false, nullable: true),
                    is_expired = table.Column<bool>(nullable: true),
                    login_time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_session_1", x => new { x.id, x.tenant_id, x.school_id, x.emailaddress });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "login_session");
        }
    }
}
