using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableStaffMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "transferred_school_id",
                table: "student_enrollment",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "filetype",
                table: "student_documents",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(50) CHARACTER SET utf8mb4",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "staff_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    staff_id = table.Column<int>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    salutation = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    suffix = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    first_given_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    last_family_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    staff_internal_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    alternate_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    district_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    state_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    preferred_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    previous_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    social_security_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    other_govt_issued_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    gender = table.Column<string>(unicode: false, maxLength: 6, nullable: true),
                    race = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ethnicity = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    marital_status = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    country_of_birth = table.Column<int>(nullable: true),
                    nationality = table.Column<int>(nullable: true),
                    first_language = table.Column<int>(nullable: true),
                    second_language = table.Column<int>(nullable: true),
                    third_language = table.Column<int>(nullable: true),
                    physical_disability = table.Column<bool>(nullable: true),
                    portal_access = table.Column<bool>(nullable: true),
                    login_email_address = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    profile = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    job_title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    joining_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    homeroom_teacher = table.Column<bool>(nullable: true),
                    primary_grade_level_taught = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    primary_subject_taught = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    other_grade_level_taught = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    other_subject_taught = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    mobile_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    office_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    personal_email = table.Column<string>(unicode: false, nullable: true),
                    school_email = table.Column<string>(unicode: false, nullable: true),
                    twitter = table.Column<string>(unicode: false, nullable: true),
                    facebook = table.Column<string>(unicode: false, nullable: true),
                    instagram = table.Column<string>(unicode: false, nullable: true),
                    youtube = table.Column<string>(unicode: false, nullable: true),
                    linkedin = table.Column<string>(unicode: false, nullable: true),
                    home_address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    home_address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    home_address_city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    home_address_state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    home_address_country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    home_address_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    mailing_address_same_to_home = table.Column<bool>(nullable: true, comment: "if true, home address will be replicated to mailing"),
                    mailing_address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    mailing_address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    mailing_address_city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    mailing_address_state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    mailing_address_country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    mailing_address_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    emergency_first_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    emergency_last_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    relationship_to_staff = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    emergency_home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    emergency_work_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    emergency_mobile_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    emergency_email = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    last_updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_master_1", x => new { x.tenant_id, x.staff_id });
                    table.ForeignKey(
                        name: "FK_staff_master_language",
                        column: x => x.first_language,
                        principalTable: "language",
                        principalColumn: "lang_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_staff_master_language1",
                        column: x => x.second_language,
                        principalTable: "language",
                        principalColumn: "lang_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_staff_master_language2",
                        column: x => x.third_language,
                        principalTable: "language",
                        principalColumn: "lang_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_staff_master_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_certificate_info",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: true),
                    school_id = table.Column<int>(nullable: true),
                    staff_id = table.Column<int>(nullable: true),
                    certification_name = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    certification_code = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    primary_certification = table.Column<bool>(nullable: true),
                    certification_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_description = table.Column<string>(unicode: false, nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_staff_certificate_info_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_school_info",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: true),
                    school_id = table.Column<int>(nullable: true),
                    staff_id = table.Column<int>(nullable: true),
                    school_attached_id = table.Column<int>(nullable: true),
                    school_attached_name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    profile = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_staff_school_info_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_staff_certificate_info_tenant_id_staff_id",
                table: "staff_certificate_info",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_master_first_language",
                table: "staff_master",
                column: "first_language");

            migrationBuilder.CreateIndex(
                name: "IX_staff_master_second_language",
                table: "staff_master",
                column: "second_language");

            migrationBuilder.CreateIndex(
                name: "IX_staff_master_third_language",
                table: "staff_master",
                column: "third_language");

            migrationBuilder.CreateIndex(
                name: "IX_staff_master_tenant_id_school_id",
                table: "staff_master",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_school_info_tenant_id_staff_id",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "staff_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "staff_certificate_info");

            migrationBuilder.DropTable(
                name: "staff_school_info");

            migrationBuilder.DropTable(
                name: "staff_master");

            migrationBuilder.DropColumn(
                name: "transferred_school_id",
                table: "student_enrollment");

            migrationBuilder.AlterColumn<string>(
                name: "filetype",
                table: "student_documents",
                type: "char(50) CHARACTER SET utf8mb4",
                unicode: false,
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
