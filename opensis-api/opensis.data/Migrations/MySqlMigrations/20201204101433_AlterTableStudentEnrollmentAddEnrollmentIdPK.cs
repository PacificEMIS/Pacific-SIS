using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableStudentEnrollmentAddEnrollmentIdPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_student_enrollment",
                table: "student_enrollment");

            migrationBuilder.AlterColumn<int>(
                name: "enrollment_id",
                table: "student_enrollment",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_enrollment",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_id", "enrollment_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_student_enrollment",
                table: "student_enrollment");

            migrationBuilder.AlterColumn<int>(
                name: "enrollment_id",
                table: "student_enrollment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_enrollment",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_id" });
        }
    }
}
