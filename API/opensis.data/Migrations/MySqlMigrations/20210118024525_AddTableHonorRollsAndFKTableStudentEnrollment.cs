using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableHonorRollsAndFKTableStudentEnrollment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parent_address_student_master",
                table: "parent_address");

            migrationBuilder.DropForeignKey(
               name: "FK_student_documents_student_master",
               table: "student_documents");

            migrationBuilder.DropForeignKey(
               name: "FK_student_comments_student_master",
               table: "student_comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_master",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_parent_address_tenant_id_school_id_student_id",
                table: "parent_address");

            //migrationBuilder.RenameColumn(
            //    name: "bus_No",
            //    table: "student_master",
            //    newName: "bus_no");

            migrationBuilder.Sql("ALTER TABLE `student_master` CHANGE `bus_No` `bus_no` VARCHAR(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL");

            migrationBuilder.AlterColumn<double>(
                name: "credit_hours",
                table: "course",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(5) CHARACTER SET utf8mb4",
                oldFixedLength: true,
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "course_category",
                table: "course",
                unicode: false,
                maxLength: 8,
                nullable: true,
                comment: "'Core' or 'Elective'",
                oldClrType: typeof(string),
                oldType: "varchar(100) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "standard",
                table: "course",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "choose between US Common Core library or school specific standards library.");

            migrationBuilder.AddColumn<string>(
                name: "standard_ref_no",
                table: "course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_student_master_tenant_id_school_id_student_guid",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_master_1",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.CreateTable(
                name: "honor_rolls",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    honor_roll_id = table.Column<int>(nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    honor_roll = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    breakoff = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_honor_rolls", x => new { x.tenant_id, x.school_id, x.honor_roll_id });
                    table.ForeignKey(
                        name: "FK_honor_rolls_honor_rolls",
                        columns: x => new { x.tenant_id, x.school_id, x.marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_master",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_guid",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_guid" });

            migrationBuilder.CreateIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_id",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_student_master",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_guid" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                      name: "FK_student_documents_student_master",
                      table: "student_documents",
                      columns: new[] { "tenant_id", "school_id", "student_id" },
                      principalTable: "student_master",
                      principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                      onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                      name: "FK_student_comments_student_master",
                      table: "student_comments",
                      columns: new[] { "tenant_id", "school_id", "student_id" },
                      principalTable: "student_master",
                      principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                      onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_enrollment_student_master",
                table: "student_enrollment");

            migrationBuilder.DropTable(
                name: "honor_rolls");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_student_master_tenant_id_school_id_student_guid",
                table: "student_master");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_master_1",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_master",
                table: "student_master");

            migrationBuilder.DropIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_guid",
                table: "student_enrollment");

            migrationBuilder.DropColumn(
                name: "standard",
                table: "course");

            migrationBuilder.DropColumn(
                name: "standard_ref_no",
                table: "course");

            migrationBuilder.RenameColumn(
                name: "bus_no",
                table: "student_master",
                newName: "bus_No");

            migrationBuilder.AlterColumn<string>(
                name: "credit_hours",
                table: "course",
                type: "char(5) CHARACTER SET utf8mb4",
                fixedLength: true,
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "course_category",
                table: "course",
                type: "varchar(100) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 8,
                oldNullable: true,
                oldComment: "'Core' or 'Elective'");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_master",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "IX_parent_address_tenant_id_school_id_student_id",
                table: "parent_address",
                columns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_parent_address_student_master",
                table: "parent_address",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
