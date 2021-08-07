using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterPKFKBetweenMultipleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "user_master");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_master_1",
                table: "user_master",
                columns: new[] { "tenant_id", "school_id", "emailaddress" });

            migrationBuilder.CreateIndex(
                name: "IX_student_master_first_language_id",
                table: "student_master",
                column: "first_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_master_second_language_id",
                table: "student_master",
                column: "second_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_master_third_language_id",
                table: "student_master",
                column: "third_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_section_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_custom_fields_value_custom_fields",
                table: "custom_fields_value",
                columns: new[] { "tenant_id", "school_id", "category_id", "field_id" },
                principalTable: "custom_fields",
                principalColumns: new[] { "tenant_id", "school_id", "category_id", "field_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_gradelevels",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" },
                principalTable: "gradelevels",
                principalColumns: new[] { "tenant_id", "school_id", "grade_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_sections",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "section_id" },
                principalTable: "sections",
                principalColumns: new[] { "tenant_id", "school_id", "section_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_master_language",
                table: "student_master",
                column: "first_language_id",
                principalTable: "language",
                principalColumn: "lang_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_master_language1",
                table: "student_master",
                column: "second_language_id",
                principalTable: "language",
                principalColumn: "lang_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_master_language2",
                table: "student_master",
                column: "third_language_id",
                principalTable: "language",
                principalColumn: "lang_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custom_fields_value_custom_fields",
                table: "custom_fields_value");

            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_gradelevels",
                table: "student_enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_sections",
                table: "student_enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_student_master_language",
                table: "student_master");

            migrationBuilder.DropForeignKey(
                name: "FK_student_master_language1",
                table: "student_master");

            migrationBuilder.DropForeignKey(
                name: "FK_student_master_language2",
                table: "student_master");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_master_1",
                table: "user_master");

            migrationBuilder.DropIndex(
                name: "IX_student_master_first_language_id",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_master_second_language_id",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_master_third_language_id",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_section_id",
                table: "student_enrollment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_master",
                table: "user_master",
                columns: new[] { "tenant_id", "school_id", "user_id" });
        }
    }
}
