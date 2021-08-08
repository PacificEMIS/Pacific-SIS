using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddTableStudentMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "user_master",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "school_calendars",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldFixedLength: true,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "custom_fields",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "student_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    alternate_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    district_id = table.Column<int>(nullable: true),
                    state_id = table.Column<int>(nullable: true),
                    admission_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    roll_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    salutation = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    first_given_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    last_family_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    suffix = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    preferred_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    previous_name = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    social_security_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    other_govt_issued_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    student_photo = table.Column<byte[]>(nullable: true),
                    dob = table.Column<DateTime>(type: "date", nullable: true),
                    display_age = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    gender = table.Column<string>(unicode: false, maxLength: 6, nullable: true),
                    race = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ethnicity = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    marital_status = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    country_of_birth = table.Column<int>(nullable: true),
                    nationality = table.Column<int>(nullable: true),
                    first_language_id = table.Column<int>(nullable: true),
                    second_language_id = table.Column<int>(nullable: true),
                    third_language_id = table.Column<int>(nullable: true),
                    home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    mobile_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
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
                    home_address_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    bus_No = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    school_bus_pick_up = table.Column<bool>(nullable: true),
                    school_bus_drop_off = table.Column<bool>(nullable: true),
                    mailing_address_same_to_home = table.Column<bool>(nullable: true, comment: "if true, home address will be replicated to mailing"),
                    mailing_address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    mailing_address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    mailing_address_city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    mailing_address_state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    mailing_address_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    primary_contact_relationship = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    primary_contact_firstname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    primary_contact_lastname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    primary_contact_home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    primary_contact_work_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    primary_contact_mobile = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    primary_contact_email = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    is_primary_custodian = table.Column<bool>(nullable: true),
                    is_primary_portal_user = table.Column<bool>(nullable: true),
                    primary_portal_user_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true, comment: "user_master table-->user_id, membership table --> profile:Parent"),
                    primary_contact_student_address_same = table.Column<bool>(nullable: true),
                    primary_contact_address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    primary_contact_address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    primary_contact_city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    primary_contact_state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    primary_contact_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    secondary_contact_relationship = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    secondary_contact_firstname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    secondary_contact_lastname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    secondary_contact_home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    secondary_contact_work_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    secondary_contact_mobile = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    secondary_contact_email = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    is_secondary_custodian = table.Column<bool>(nullable: true),
                    is_secondary_portal_user = table.Column<bool>(nullable: true),
                    secondary_portal_user_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true, comment: "user_master table-->user_id, membership table --> profile:Parent"),
                    secondary_contact_student_address_same = table.Column<bool>(nullable: true),
                    secondary_contact_address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    secondary_contact_address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    secondary_contact_city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    secondary_contact_state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    secondary_contact_zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_master", x => new { x.tenant_id, x.school_id, x.student_id });
                    table.ForeignKey(
                        name: "FK_student_master_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_student_enrollment_student_master",
                table: "student_enrollment",
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
                name: "student_master");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "user_master");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "school_calendars",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "custom_fields",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
