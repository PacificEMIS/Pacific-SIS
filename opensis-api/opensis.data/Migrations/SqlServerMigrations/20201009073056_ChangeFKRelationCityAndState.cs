using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class ChangeFKRelationCityAndState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_city_state",
                table: "city");

            migrationBuilder.CreateIndex(
                name: "IX_city_stateid",
                table: "city",
                column: "stateid");

            migrationBuilder.AddForeignKey(
                name: "FK_city_state",
                table: "city",
                column: "stateid",
                principalTable: "state",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_city_state",
                table: "city");

            migrationBuilder.DropIndex(
                name: "IX_city_stateid",
                table: "city");

            migrationBuilder.AddForeignKey(
                name: "FK_city_state",
                table: "city",
                column: "id",
                principalTable: "state",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
