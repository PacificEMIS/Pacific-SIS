using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterTableStudentMasterAddTableParentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_primary_custodian",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "is_primary_portal_user",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "is_secondary_custodian",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "is_secondary_portal_user",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_address_line_one",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_address_line_two",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_city",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_country",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_email",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_firstname",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_home_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_lastname",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_mobile",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_relationship",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_state",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_student_address_same",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_work_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_contact_zip",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_portal_user_id",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_address_line_one",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_address_line_two",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_city",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_country",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_email",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_firstname",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_home_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_lastname",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_mobile",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_relationship",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_state",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_student_address_same",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_work_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_contact_zip",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "secondary_portal_user_id",
                table: "student_master");

            migrationBuilder.AddColumn<string>(
                name: "alert_description",
                table: "student_master",
                unicode: false,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "critical_alert",
                table: "student_master",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dentist",
                table: "student_master",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dentist_phone",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "insurance_company",
                table: "student_master",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "insurance_company_phone",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "medical_facility",
                table: "student_master",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "medical_facility_phone",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "policy_holder",
                table: "student_master",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "policy_number",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_care_physician",
                table: "student_master",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_care_physician_phone",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vision",
                table: "student_master",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vision_phone",
                table: "student_master",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "parent_info",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    parent_id = table.Column<int>(nullable: false),
                    relationship = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    firstname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    lastname = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    home_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    work_phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    mobile = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    email = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
                    student_address_same = table.Column<bool>(nullable: true),
                    address_line_one = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    address_line_two = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    city = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    state = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    zip = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    is_custodian = table.Column<bool>(nullable: false),
                    is_portal_user = table.Column<bool>(nullable: false),
                    portal_user_id = table.Column<string>(unicode: false, maxLength: 150, nullable: true, comment: "emailaddress mapped to user_master"),
                    bus_pickup = table.Column<bool>(nullable: true),
                    bus_dropoff = table.Column<bool>(nullable: true),
                    contact_type = table.Column<string>(unicode: false, maxLength: 9, nullable: true, comment: "Primary | Secondary | Other"),
                    associationship = table.Column<string>(unicode: false, nullable: true, comment: "tenantid#schoolid#studentid | tenantid#schoolid#studentid | ...."),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_info", x => new { x.tenant_id, x.school_id, x.student_id, x.parent_id });
                    table.ForeignKey(
                        name: "FK_parent_info_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    comment_id = table.Column<int>(nullable: false),
                    comment = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_comments", x => new { x.tenant_id, x.school_id, x.student_id, x.comment_id });
                    table.ForeignKey(
                        name: "FK_student_comments_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_documents",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    student_id = table.Column<int>(nullable: false),
                    document_id = table.Column<int>(nullable: false),
                    file_uploaded = table.Column<byte[]>(nullable: true),
                    uploaded_on = table.Column<DateTime>(type: "datetime", nullable: true),
                    uploaded_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_documents", x => new { x.tenant_id, x.school_id, x.student_id, x.document_id });
                    table.ForeignKey(
                        name: "FK_student_documents_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_secret_questions",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    emailaddress = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    user_id = table.Column<int>(nullable: true),
                    movie = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    city = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    hero = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    book = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    cartoon = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_secret_questions", x => new { x.tenant_id, x.school_id, x.emailaddress });
                    table.ForeignKey(
                        name: "FK_user_secret_questions_user_master",
                        columns: x => new { x.tenant_id, x.school_id, x.emailaddress },
                        principalTable: "user_master",
                        principalColumns: new[] { "tenant_id", "school_id", "emailaddress" },
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parent_info");

            migrationBuilder.DropTable(
                name: "student_comments");

            migrationBuilder.DropTable(
                name: "student_documents");

            migrationBuilder.DropTable(
                name: "user_secret_questions");

            migrationBuilder.DropColumn(
                name: "alert_description",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "critical_alert",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "dentist",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "dentist_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "insurance_company",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "insurance_company_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "medical_facility",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "medical_facility_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "policy_holder",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "policy_number",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_care_physician",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "primary_care_physician_phone",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "vision",
                table: "student_master");

            migrationBuilder.DropColumn(
                name: "vision_phone",
                table: "student_master");

            migrationBuilder.AddColumn<bool>(
                name: "is_primary_custodian",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_primary_portal_user",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_secondary_custodian",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_secondary_portal_user",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_address_line_one",
                table: "student_master",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_address_line_two",
                table: "student_master",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_city",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_country",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_email",
                table: "student_master",
                type: "varchar(100) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_firstname",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_home_phone",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_lastname",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_mobile",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_relationship",
                table: "student_master",
                type: "varchar(20) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_state",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "primary_contact_student_address_same",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_work_phone",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_contact_zip",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primary_portal_user_id",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "user_master table-->user_id, membership table --> profile:Parent");

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_address_line_one",
                table: "student_master",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_address_line_two",
                table: "student_master",
                type: "varchar(200) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_city",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_country",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_email",
                table: "student_master",
                type: "varchar(100) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_firstname",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_home_phone",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_lastname",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_mobile",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_relationship",
                table: "student_master",
                type: "varchar(20) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_state",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "secondary_contact_student_address_same",
                table: "student_master",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_work_phone",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_contact_zip",
                table: "student_master",
                type: "varchar(15) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secondary_portal_user_id",
                table: "student_master",
                type: "varchar(50) CHARACTER SET utf8mb4",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "user_master table-->user_id, membership table --> profile:Parent");
        }
    }
}
