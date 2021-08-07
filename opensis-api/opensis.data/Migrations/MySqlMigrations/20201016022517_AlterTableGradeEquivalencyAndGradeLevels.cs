using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableGradeEquivalencyAndGradeLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "grade_equivalency", keyColumn: "country",
           keyValues: new object[]
           {
                 "US"
           });
            migrationBuilder.DropColumn(
                name: "age_range",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "educational_stage",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "grade_level_equivalency",
                table: "gradelevels");

            migrationBuilder.DropColumn(
                name: "country",
                table: "grade_equivalency");

            migrationBuilder.DropColumn(
                name: "educational_stage",
                table: "grade_equivalency");

            migrationBuilder.AddColumn<string>(
                name: "isced_grade_level",
                table: "gradelevels",
                unicode: false,
                maxLength: 8,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "isced_grade_level",
                table: "grade_equivalency",
                unicode: false,
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_grade_equivalency",
                table: "grade_equivalency",
                column: "isced_grade_level");

            migrationBuilder.InsertData(
                table: "grade_equivalency",
                columns: new[] { "isced_grade_level", "age_range", "grade_description" },
                values: new object[,]
                {
                    { "ISCED 01", "0-2", "Early childhood education" },
                    { "ISCED 02", "0-2", "Pre-primary education" },
                    { "ISCED 1", "5-7", "Primary education" },
                    { "ISCED 2", "6-10", "Lower secondary education" },
                    { "ISCED 3", "9-12", "Upper secondary education" },
                    { "ISCED 4", "10-11", "Post-secondary non-tertiary education" },
                    { "ISCED 5", "14-16", "Short-cycle tertiary education" },
                    { "ISCED 6", "17-23", "Bachelor's or equivalent" },
                    { "ISCED 7", "21-25", "Master's or equivalent" },
                    { "ISCED 8", "22-28", "Doctoral or equivalent level" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_gradelevels_isced_grade_level",
                table: "gradelevels",
                column: "isced_grade_level");

            migrationBuilder.AddForeignKey(
                name: "FK_gradelevels_grade_equivalency",
                table: "gradelevels",
                column: "isced_grade_level",
                principalTable: "grade_equivalency",
                principalColumn: "isced_grade_level",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gradelevels_grade_equivalency",
                table: "gradelevels");

            migrationBuilder.DropIndex(
                name: "IX_gradelevels_isced_grade_level",
                table: "gradelevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_grade_equivalency",
                table: "grade_equivalency");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 01");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 02");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 1");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 2");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 3");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 4");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 5");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 6");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 7");

            migrationBuilder.DeleteData(
                table: "grade_equivalency",
                keyColumn: "isced_grade_level",
                keyValue: "ISCED 8");

            migrationBuilder.DropColumn(
                name: "isced_grade_level",
                table: "gradelevels");

            migrationBuilder.AddColumn<string>(
                name: "age_range",
                table: "gradelevels",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "educational_stage",
                table: "gradelevels",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade_level_equivalency",
                table: "gradelevels",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "isced_grade_level",
                table: "grade_equivalency",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 8);

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "grade_equivalency",
                type: "char(30) CHARACTER SET utf8mb4",
                unicode: false,
                fixedLength: true,
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "educational_stage",
                table: "grade_equivalency",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }
    }
}
