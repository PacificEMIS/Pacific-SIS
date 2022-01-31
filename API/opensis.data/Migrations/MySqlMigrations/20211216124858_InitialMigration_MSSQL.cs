using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class InitialMigration_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "api_controller_list",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    controller_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    api_title = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    controller_path = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_controller_list_tenant_id", x => new { x.tenant_id, x.controller_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "api_keys_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    key_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    api_key = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    api_title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    scopes = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    expires = table.Column<DateTime>(type: "date", nullable: true),
                    revoked = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_keys_master_tenant_id", x => new { x.tenant_id, x.school_id, x.key_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "assignment_type",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    course_section_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    weightage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_type_tenant_id", x => new { x.tenant_id, x.school_id, x.assignment_type_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "bell_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    bell_schedule_date = table.Column<DateTime>(type: "date", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bell_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.academic_year, x.bell_schedule_date });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "calendar_events",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    calendar_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_date = table.Column<DateTime>(type: "date", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    visible_to_membership_id = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    event_color = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: true, collation: "utf8mb4_general_ci"),
                    is_holiday = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    applicable_to_all_school = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    system_wide_event = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_events_tenant_id", x => new { x.tenant_id, x.calendar_id, x.event_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    countrycode = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.id);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_title = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    course_short_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course_grade_level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course_program = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    course_subject = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    course_category = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true, collation: "utf8mb4_general_ci"),
                    credit_hours = table.Column<double>(type: "double", nullable: true),
                    standard = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    standard_ref_no = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course_description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    is_course_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_comment_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_comment_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    course_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false, collation: "utf8mb4_general_ci"),
                    applicable_all_courses = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    comments = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, defaultValueSql: "(N'')", collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_comment_category_tenant_id", x => new { x.tenant_id, x.school_id, x.course_comment_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_library_category_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_category_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "effort_grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_value = table.Column<int>(type: "int", nullable: true),
                    grade_scale_comment = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_scale_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_grade_scale_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade_age_range",
                columns: table => new
                {
                    age_range_id = table.Column<int>(type: "int", nullable: false),
                    age_range = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_age_range_age_range_id", x => x.age_range_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade_educational_stage",
                columns: table => new
                {
                    isced_code = table.Column<int>(type: "int", nullable: false),
                    educational_stage = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_educational_stage_isced_code", x => x.isced_code);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade_equivalency",
                columns: table => new
                {
                    equivalency_id = table.Column<int>(type: "int", nullable: false),
                    grade_level_equivalency = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_equivalency_equivalency_id", x => x.equivalency_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade_us_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    standard_ref_no = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    grade_standard_id = table.Column<int>(type: "int", nullable: false),
                    grade_level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    domain = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    subject = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    topic = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    standard_details = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    is_school_specific = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_us_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.standard_ref_no, x.grade_standard_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historical_grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    hist_grade_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    school_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    equivalency_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historical_marking_period",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    grade_post_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_exam = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_comments = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_marking_period_tenant_id", x => new { x.tenant_id, x.school_id, x.hist_marking_period_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    lang_id = table.Column<int>(type: "int", nullable: false),
                    lcid = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    locale = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    language_code = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_lang_id", x => x.lang_id);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "login_session",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    ipaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    token = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    is_expired = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    login_time = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_session_id", x => new { x.id, x.tenant_id, x.school_id, x.emailaddress });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "notice",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    notice_id = table.Column<int>(type: "int", nullable: false),
                    target_membership_ids = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci"),
                    body = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci"),
                    valid_from = table.Column<DateTime>(type: "date", nullable: false),
                    valid_to = table.Column<DateTime>(type: "date", nullable: false),
                    isactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    visible_to_all_school = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: false),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notice_tenant_id", x => new { x.tenant_id, x.school_id, x.notice_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "parent_associationship",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    associationship = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    relationship = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    is_custodian = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    contact_type = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_associationship_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "parent_info",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    parent_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')", collation: "ascii_general_ci"),
                    parent_photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    salutation = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    firstname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    middlename = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    lastname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    work_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    mobile = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    personal_email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    work_email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    user_profile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    is_portal_user = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    login_email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    suffix = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    bus_No = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    bus_pickup = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    bus_dropoff = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_info_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    max_api_checks = table.Column<int>(type: "int", nullable: true),
                    features = table.Column<byte[]>(type: "longblob", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans_tenant_id", x => new { x.tenant_id, x.school_id, x.plan_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "programs",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    program_id = table.Column<int>(type: "int", nullable: false),
                    program_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programs_tenant_id", x => new { x.tenant_id, x.school_id, x.program_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "release_number",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    release_number = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false, collation: "utf8mb4_general_ci"),
                    release_date = table.Column<DateTime>(type: "date", nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release_number_tenant_id", x => new { x.tenant_id, x.school_id, x.release_number });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    isactive = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms_tenant_id", x => new { x.tenant_id, x.school_id, x.room_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    section_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sections_tenant_id", x => new { x.tenant_id, x.school_id, x.section_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "staff_schedule_view",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    course_short_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course_section_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    staff_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    staff_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    scheduled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    conflict_comment = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_schedule_view_tenant_id", x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_schedule_view",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    course_section_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    student_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    student_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    scheduled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    conflict_comment = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_schedule_view_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "subject",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    subject_id = table.Column<int>(type: "int", nullable: false),
                    subject_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject_tenant_id", x => new { x.tenant_id, x.school_id, x.subject_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "user_access_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    login_attempt_date = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: false),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    user_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    profile = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    login_failure_count = table.Column<int>(type: "int", nullable: true),
                    login_status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ipaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_access_log_id", x => new { x.id, x.tenant_id, x.school_id, x.login_attempt_date });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "api_controller_key_mapping",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    key_id = table.Column<int>(type: "int", nullable: false),
                    controller_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_controller_key_mapping_tenant_id", x => new { x.tenant_id, x.school_id, x.key_id, x.controller_id });
                    table.ForeignKey(
                        name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_controller_list",
                        columns: x => new { x.tenant_id, x.controller_id },
                        principalTable: "api_controller_list",
                        principalColumns: new[] { "tenant_id", "controller_id" });
                    table.ForeignKey(
                        name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_keys_master",
                        columns: x => new { x.tenant_id, x.school_id, x.key_id },
                        principalTable: "api_keys_master",
                        principalColumns: new[] { "tenant_id", "school_id", "key_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "state",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    countryid = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", precision: 6, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state", x => x.id);
                    table.ForeignKey(
                        name: "state$FK_state_country",
                        column: x => x.countryid,
                        principalTable: "country",
                        principalColumn: "id");
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category_item",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: false),
                    effort_item_id = table.Column<int>(type: "int", nullable: false),
                    effort_item_title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_library_category_item_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_category_id, x.effort_item_id });
                    table.ForeignKey(
                        name: "effort_grade_library_category_item$FK_effort_category_item_effort_category",
                        columns: x => new { x.tenant_id, x.school_id, x.effort_category_id },
                        principalTable: "effort_grade_library_category",
                        principalColumns: new[] { "tenant_id", "school_id", "effort_category_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    standard_ref_no = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    grade_standard_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.standard_ref_no, x.grade_standard_id });
                    table.ForeignKey(
                        name: "course_standard$FK_course_standard_course",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id },
                        principalTable: "course",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id" });
                    table.ForeignKey(
                        name: "course_standard$FK_course_standard_grade_us_standard",
                        columns: x => new { x.tenant_id, x.school_id, x.standard_ref_no, x.grade_standard_id },
                        principalTable: "grade_us_standard",
                        principalColumns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historical_credit_transfer",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    hist_grade_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    credit_transfer_id = table.Column<int>(type: "int", nullable: false),
                    course_code = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    course_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    letter_grade = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_general_ci"),
                    gp_value = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    calculate_gpa = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    weighted_gp = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    grade_scale = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    credit_attempted = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    credit_earned = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_credit_transfer_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id, x.hist_marking_period_id, x.credit_transfer_id });
                    table.ForeignKey(
                        name: "historical_credit_transfer$FK_credit_transfer_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id },
                        principalTable: "historical_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "parent_address",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_address_same = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    address_line_one = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    address_line_two = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    city = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    zip = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_address_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                    table.ForeignKey(
                        name: "parent_address$FK_parent_address_parent_info",
                        columns: x => new { x.tenant_id, x.school_id, x.parent_id },
                        principalTable: "parent_info",
                        principalColumns: new[] { "tenant_id", "school_id", "parent_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    school_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')", collation: "ascii_general_ci"),
                    school_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_alt_id = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_state_id = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_district_id = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_level = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_classification = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    alternate_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    street_address_1 = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    street_address_2 = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    city = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    county = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    division = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    state = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    district = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    zip = table.Column<string>(type: "char(20)", fixedLength: true, maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    country = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    current_period_ends = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    max_api_checks = table.Column<int>(type: "int", nullable: true),
                    features = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    plan_id = table.Column<int>(type: "int", nullable: true),
                    longitude = table.Column<double>(type: "double", nullable: true),
                    latitude = table.Column<double>(type: "double", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_master_tenant_id", x => new { x.tenant_id, x.school_id });
                    table.ForeignKey(
                        name: "school_master$FK_school_master_plans",
                        columns: x => new { x.tenant_id, x.school_id, x.plan_id },
                        principalTable: "plans",
                        principalColumns: new[] { "tenant_id", "school_id", "plan_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    stateid = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_city", x => x.id);
                    table.ForeignKey(
                        name: "city$FK_city_state",
                        column: x => x.stateid,
                        principalTable: "state",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "attendance_code_categories",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code_categories_tenant_id", x => new { x.tenant_id, x.school_id, x.attendance_category_id });
                    table.ForeignKey(
                        name: "attendance_code_categories$FK_attendance_code_categories_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "block",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    block_title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    block_sort_order = table.Column<long>(type: "bigint", nullable: true),
                    full_day_minutes = table.Column<int>(type: "int", nullable: true),
                    half_day_minutes = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_tenant_id", x => new { x.tenant_id, x.school_id, x.block_id });
                    table.ForeignKey(
                        name: "block$FK_block_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "block_period",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    period_id = table.Column<int>(type: "int", nullable: false),
                    period_title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    period_short_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    period_start_time = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: true, collation: "utf8mb4_general_ci"),
                    period_end_time = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: true, collation: "utf8mb4_general_ci"),
                    period_sort_order = table.Column<int>(type: "int", nullable: true),
                    calculate_attendance = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_period_tenant_id", x => new { x.tenant_id, x.school_id, x.block_id, x.period_id });
                    table.ForeignKey(
                        name: "block_period$FK_block_period_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "dpdown_valuelist",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    lov_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    lov_column_value = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci"),
                    lov_code = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dpdown_valuelist", x => x.id);
                    table.ForeignKey(
                        name: "dpdown_valuelist$FK_dpdown_valuelist_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "fields_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    is_system_category = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    search = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    module = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    required = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    hide = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_system_wide_category = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fields_category_tenant_id", x => new { x.tenant_id, x.school_id, x.category_id });
                    table.ForeignKey(
                        name: "fields_category$FK_custom_category_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    grade_scale_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade_scale_comment = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    calculate_gpa = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    use_as_standard_grade_scale = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_scale_tenant_id", x => new { x.tenant_id, x.school_id, x.grade_scale_id });
                    table.ForeignKey(
                        name: "grade_scale$FK_grade_scale_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradelevels",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    short_name = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    equivalency_id = table.Column<int>(type: "int", nullable: true),
                    age_range_id = table.Column<int>(type: "int", nullable: true),
                    isced_code = table.Column<int>(type: "int", nullable: true),
                    next_grade_id = table.Column<int>(type: "int", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradelevels_tenant_id", x => new { x.tenant_id, x.school_id, x.grade_id });
                    table.ForeignKey(
                        name: "gradelevels$FK_gradelevels_grade_age_range",
                        column: x => x.age_range_id,
                        principalTable: "grade_age_range",
                        principalColumn: "age_range_id");
                    table.ForeignKey(
                        name: "gradelevels$FK_gradelevels_grade_educational_stage",
                        column: x => x.isced_code,
                        principalTable: "grade_educational_stage",
                        principalColumn: "isced_code");
                    table.ForeignKey(
                        name: "gradelevels$FK_gradelevels_grade_equivalency",
                        column: x => x.equivalency_id,
                        principalTable: "grade_equivalency",
                        principalColumn: "equivalency_id");
                    table.ForeignKey(
                        name: "gradelevels$FK_gradelevels_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: false),
                    profile = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_system = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_superadmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    profile_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_tenant_id", x => new { x.tenant_id, x.school_id, x.membership_id });
                    table.ForeignKey(
                        name: "membership$fk_table_membership_table_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "permission_group",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_system = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    icon_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    path = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci"),
                    badgeType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    badgeValue = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_group_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_group_id });
                    table.ForeignKey(
                        name: "permission_group$FK_permission_group_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_calendars",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    calender_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    visible_to_membership_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    default_calender = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    days = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: true, collation: "utf8mb4_general_ci"),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    session_calendar = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_calendars_tenant_id", x => new { x.tenant_id, x.school_id, x.calender_id });
                    table.ForeignKey(
                        name: "school_calendars$FK_school_calendars_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    affiliation = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    associations = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    locale = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    lowest_grade_level = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    highest_grade_level = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    date_school_opened = table.Column<DateTime>(type: "date", nullable: true),
                    date_school_closed = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    gender = table.Column<string>(type: "char(15)", fixedLength: true, maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    internet = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    electricity = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    telephone = table.Column<string>(type: "char(30)", fixedLength: true, maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    fax = table.Column<string>(type: "char(20)", fixedLength: true, maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    website = table.Column<string>(type: "char(150)", fixedLength: true, maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    email = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    twitter = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    facebook = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    instagram = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    youtube = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    linkedin = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    name_of_principal = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    name_of_assistant_principal = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    school_logo = table.Column<byte[]>(type: "longblob", nullable: true),
                    running_water = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    main_source_of_drinking_water = table.Column<string>(type: "char(100)", fixedLength: true, maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    currently_available = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    female_toilet_type = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    total_female_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_female_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    female_toilet_accessibility = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    male_toilet_type = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    total_male_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_male_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    male_toilet_accessibility = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    comon_toilet_type = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    total_common_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_common_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    common_toilet_accessibility = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    handwashing_available = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    soap_and_water_available = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    hygene_education = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_detail", x => x.id);
                    table.ForeignKey(
                        name: "school_detail$FK_school_detail_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_preference",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    school_preference_id = table.Column<long>(type: "bigint", nullable: false),
                    school_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_alt_id = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    full_day_minutes = table.Column<int>(type: "int", nullable: true),
                    half_day_minutes = table.Column<int>(type: "int", nullable: true),
                    max_login_failure = table.Column<int>(type: "int", nullable: true),
                    max_inactivity_days = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_preference_tenant_id", x => new { x.tenant_id, x.school_id, x.school_preference_id });
                    table.ForeignKey(
                        name: "school_preference$FK_school_preference_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_rollover",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    rollover_id = table.Column<int>(type: "int", nullable: false),
                    reenrollment_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_begin_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    rollover_content = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    rollover_status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_rollover_tenant_id", x => new { x.tenant_id, x.school_id, x.rollover_id });
                    table.ForeignKey(
                        name: "school_rollover$FK_school_rollover_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "school_years",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_exam = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_comments = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_years_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "school_years$FK_school_years_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "staff_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')", collation: "ascii_general_ci"),
                    staff_photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    salutation = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    suffix = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    first_given_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    middle_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    last_family_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    staff_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    alternate_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    district_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    state_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    preferred_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    previous_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    social_security_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    other_govt_issued_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    gender = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true, collation: "utf8mb4_general_ci"),
                    race = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    ethnicity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    dob = table.Column<DateTime>(type: "date", nullable: true),
                    marital_status = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    country_of_birth = table.Column<int>(type: "int", nullable: true),
                    nationality = table.Column<int>(type: "int", nullable: true),
                    first_language = table.Column<int>(type: "int", nullable: true),
                    second_language = table.Column<int>(type: "int", nullable: true),
                    third_language = table.Column<int>(type: "int", nullable: true),
                    physical_disability = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    portal_access = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    login_email_address = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    profile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    job_title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    joining_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    homeroom_teacher = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    primary_grade_level_taught = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    primary_subject_taught = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    other_grade_level_taught = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    other_subject_taught = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    mobile_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    office_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    personal_email = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    school_email = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    twitter = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    facebook = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    instagram = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    youtube = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    linkedin = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_line_one = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_line_two = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_city = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_zip = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_same_to_home = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    mailing_address_line_one = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_line_two = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_city = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_zip = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_first_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_last_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    relationship_to_staff = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_home_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_work_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_mobile_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    emergency_email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    disability_description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    bus_no = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    bus_pickup = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    bus_dropoff = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_master_tenant_id", x => new { x.tenant_id, x.staff_id });
                    table.ForeignKey(
                        name: "staff_master$FK_staff_master_language",
                        column: x => x.first_language,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "staff_master$FK_staff_master_language1",
                        column: x => x.second_language,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "staff_master$FK_staff_master_language2",
                        column: x => x.third_language,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "staff_master$FK_staff_master_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_enrollment_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    enrollment_code = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment_code_tenant_id", x => new { x.tenant_id, x.school_id, x.enrollment_code });
                    table.ForeignKey(
                        name: "student_enrollment_code$FK_student_enrollment_code_school_master1",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')", collation: "ascii_general_ci"),
                    student_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    alternate_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    district_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    state_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    admission_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    roll_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    salutation = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    first_given_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    middle_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    last_family_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    suffix = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    preferred_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    previous_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    social_security_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    other_govt_issued_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    student_photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    dob = table.Column<DateTime>(type: "date", nullable: true),
                    student_portal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    gender = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true, collation: "utf8mb4_general_ci"),
                    race = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    ethnicity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    marital_status = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    country_of_birth = table.Column<int>(type: "int", nullable: true),
                    nationality = table.Column<int>(type: "int", nullable: true),
                    first_language_id = table.Column<int>(type: "int", nullable: true),
                    second_language_id = table.Column<int>(type: "int", nullable: true),
                    third_language_id = table.Column<int>(type: "int", nullable: true),
                    section_id = table.Column<int>(type: "int", nullable: true),
                    estimated_grad_date = table.Column<DateTime>(type: "date", nullable: true),
                    eligibility_504 = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    economic_disadvantage = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    free_lunch_eligibility = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    special_education_indicator = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    lep_indicator = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    home_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    mobile_phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_general_ci"),
                    personal_email = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    school_email = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    twitter = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    facebook = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    instagram = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    youtube = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    linkedin = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_line_one = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_line_two = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_city = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    home_address_zip = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    bus_no = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    school_bus_pick_up = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    school_bus_drop_off = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    mailing_address_same_to_home = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    mailing_address_line_one = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_line_two = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_city = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_country = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    mailing_address_zip = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    critical_alert = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    alert_description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    primary_care_physician = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    primary_care_physician_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    medical_facility = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    medical_facility_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    insurance_company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    insurance_company_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    policy_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    policy_holder = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    dentist = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    dentist_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    vision = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    vision_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    associationship = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    enrollment_type = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: true, collation: "utf8mb4_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id });
                    table.ForeignKey(
                        name: "student_master$FK_student_master_language",
                        column: x => x.first_language_id,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "student_master$FK_student_master_language1",
                        column: x => x.second_language_id,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "student_master$FK_student_master_language2",
                        column: x => x.third_language_id,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "student_master$FK_student_master_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "student_master$FK_student_master_sections",
                        columns: x => new { x.tenant_id, x.school_id, x.section_id },
                        principalTable: "sections",
                        principalColumns: new[] { "tenant_id", "school_id", "section_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "attendance_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    attendance_code = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    state_code = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true, collation: "utf8mb4_general_ci"),
                    default_code = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    allow_entry_by = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code_tenant_id", x => new { x.tenant_id, x.school_id, x.attendance_category_id, x.attendance_code });
                    table.ForeignKey(
                        name: "attendance_code$FK_attendance_code_attendance_code_categories",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id },
                        principalTable: "attendance_code_categories",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_block_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_block_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_course_block_schedule_block",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id },
                        principalTable: "block",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id" });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_course_block_schedule_block_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_course_block_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_calendar_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "date", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_calendar_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_calendar_schedule$FK_course_calendar_schedule_block_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_calendar_schedule$FK_course_calendar_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_fixed_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_fixed_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_fixed_schedule$FK_course_fixed_schedule_block_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_fixed_schedule$FK_course_fixed_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_variable_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    day = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_general_ci"),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_variable_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_variable_schedule$FK_course_variable_schedule_block_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_variable_schedule$FK_course_variable_schedule_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "custom_fields",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    field_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "(N'')", collation: "utf8mb4_general_ci"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    search = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    field_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    select_options = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    system_field = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    required = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    default_selection = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    hide = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_system_wide_field = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields_tenant_id", x => new { x.tenant_id, x.school_id, x.category_id, x.field_id });
                    table.ForeignKey(
                        name: "custom_fields$FK_custom_fields_fields_category",
                        columns: x => new { x.tenant_id, x.school_id, x.category_id },
                        principalTable: "fields_category",
                        principalColumns: new[] { "tenant_id", "school_id", "category_id" });
                    table.ForeignKey(
                        name: "custom_fields$FK_custom_fields_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    breakoff = table.Column<int>(type: "int", nullable: true),
                    weighted_gp_value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    unweighted_gp_value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    comment = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.grade_scale_id, x.grade_id });
                    table.ForeignKey(
                        name: "grade$FK_grade_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "user_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    passwordhash = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, collation: "utf8mb4_general_ci"),
                    lang_id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    membership_id = table.Column<int>(type: "int", nullable: false),
                    is_tenantadmin = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    login_attempt_date = table.Column<DateTime>(type: "date", nullable: true),
                    login_failure_count = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    last_used_school_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_master_tenant_id", x => new { x.tenant_id, x.school_id, x.emailaddress });
                    table.ForeignKey(
                        name: "user_master$FK_user_master_language",
                        column: x => x.lang_id,
                        principalTable: "language",
                        principalColumn: "lang_id");
                    table.ForeignKey(
                        name: "user_master$FK_user_master_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "permission_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    path = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci"),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    enable_view = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_add = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_edit = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_delete = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_category_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_category_id });
                    table.ForeignKey(
                        name: "permission_category$FK_permission_category_permission_group",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_group_id },
                        principalTable: "permission_group",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "honor_rolls",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    honor_roll_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    honor_roll = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    breakoff = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_honor_rolls_tenant_id", x => new { x.tenant_id, x.school_id, x.honor_roll_id });
                    table.ForeignKey(
                        name: "honor_rolls$FK_honor_rolls_honor_rolls",
                        columns: x => new { x.tenant_id, x.school_id, x.marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    year_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_exam = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_comments = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    rollover_id = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semesters_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "semesters$FK_semesters_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "semesters$FK_semesters_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.year_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "assignment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    assignment_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: true),
                    assignment_title = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    points = table.Column<int>(type: "int", nullable: true),
                    assignment_date = table.Column<DateTime>(type: "date", nullable: true),
                    due_date = table.Column<DateTime>(type: "date", nullable: true),
                    assignment_description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_tenant_id", x => new { x.tenant_id, x.school_id, x.assignment_id });
                    table.ForeignKey(
                        name: "assignment$FK_assignment_assignment_type",
                        columns: x => new { x.tenant_id, x.school_id, x.assignment_type_id },
                        principalTable: "assignment_type",
                        principalColumns: new[] { "tenant_id", "school_id", "assignment_type_id" });
                    table.ForeignKey(
                        name: "assignment$FK_assignment_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "staff_certificate_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    certification_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    certification_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    primary_certification = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    certification_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_certificate_info", x => x.id);
                    table.ForeignKey(
                        name: "staff_certificate_info$FK_staff_certificate_info_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "staff_school_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: true, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    school_attached_id = table.Column<int>(type: "int", nullable: true),
                    school_attached_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    profile = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_school_info", x => x.id);
                    table.ForeignKey(
                        name: "staff_school_info$FK_staff_school_info_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_attendance_history",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    attendance_history_id = table.Column<long>(type: "bigint", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    attendance_date = table.Column<DateTime>(type: "date", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    attendance_code = table.Column<int>(type: "int", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    period_id = table.Column<int>(type: "int", nullable: false),
                    modified_by = table.Column<int>(type: "int", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    modification_timestamp = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_history_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.attendance_history_id });
                    table.ForeignKey(
                        name: "student_attendance_history$FK_student_attendance_history_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    comment_id = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.comment_id });
                    table.ForeignKey(
                        name: "student_comments$FK_student_comments_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_daily_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    attendance_date = table.Column<DateTime>(type: "date", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    section_id = table.Column<int>(type: "int", nullable: true),
                    attendance_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    attendance_comment = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    attendance_minutes = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_daily_attendance_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.attendance_date });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_student_daily_attendance_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_student_daily_attendance_sections",
                        columns: x => new { x.tenant_id, x.school_id, x.section_id },
                        principalTable: "sections",
                        principalColumns: new[] { "tenant_id", "school_id", "section_id" });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_student_daily_attendance_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_documents",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<int>(type: "int", nullable: false),
                    filename = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    filetype = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    file_uploaded = table.Column<byte[]>(type: "longblob", nullable: true),
                    uploaded_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    uploaded_by = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_documents_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.document_id });
                    table.ForeignKey(
                        name: "student_documents$FK_student_documents_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_enrollment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    enrollment_id = table.Column<int>(type: "int", nullable: false),
                    student_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')", collation: "ascii_general_ci"),
                    calender_id = table.Column<int>(type: "int", nullable: true),
                    rolling_option = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    school_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_level_title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    enrollment_date = table.Column<DateTime>(type: "date", nullable: true),
                    enrollment_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    exit_date = table.Column<DateTime>(type: "date", nullable: true),
                    exit_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    transferred_school_id = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    school_transferred = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    transferred_grade = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.enrollment_id });
                    table.ForeignKey(
                        name: "FK_student_enrollment_student_master_tenant_id_school_id_studen~",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "student_enrollment$FK_student_enrollment_gradelevels",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_id },
                        principalTable: "gradelevels",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_medical_alert",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    alert_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    alert_description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_alert_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_alert$FK_student_medical_alert_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_medical_immunization",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    immunization_type = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    immunization_date = table.Column<DateTime>(type: "date", nullable: true),
                    comment = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_immunization_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_immunization$FK_student_medical_immunization_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_medical_note",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    note_date = table.Column<DateTime>(type: "date", nullable: true),
                    medical_note = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_note_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_note$FK_student_medical_note_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_medical_nurse_visit",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    nurse_visit_date = table.Column<DateTime>(type: "date", nullable: true),
                    time_in = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    time_out = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    reason = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    result = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    comment = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_nurse_visit_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_nurse_visit$FK_student_medical_nurse_visit_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_medical_provider",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    primary_care_physician = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    primary_care_physician_phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    preferred_medical_facility = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    preferred_medical_facility_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    insurance_company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    insurance_company_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    policy_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    policy_holder_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    dentist_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    dentist_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    vision_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    vision_provider_phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_provider_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_provider$FK_student_medical_provider_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_report_card_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    school_year = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false, collation: "utf8mb4_general_ci"),
                    marking_period_title = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: false, defaultValueSql: "(N'')", collation: "utf8mb4_general_ci"),
                    grade_title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci"),
                    student_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    yod_attendance = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true, collation: "utf8mb4_general_ci"),
                    yod_absence = table.Column<int>(type: "int", nullable: true),
                    report_generation_date = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    absences = table.Column<int>(type: "int", nullable: true),
                    excused_absences = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_report_card_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.school_year, x.marking_period_title });
                    table.ForeignKey(
                        name: "student_report_card_master$FK_student_report_card_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_transcript_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    grade_title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_general_ci"),
                    school_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    school_year = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: true, collation: "utf8mb4_general_ci"),
                    credit_attempted = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_grade_credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    student_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    cumulative_gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_credit_attempted = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    generated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_transcript_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.grade_title });
                    table.ForeignKey(
                        name: "student_transcript_master$FK_student_transcript_master_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "custom_fields_value",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    field_id = table.Column<int>(type: "int", nullable: false),
                    target_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: false, collation: "utf8mb4_general_ci"),
                    custom_field_title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    custom_field_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    custom_field_value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields_value_tenant_id", x => new { x.tenant_id, x.school_id, x.category_id, x.field_id, x.target_id, x.module });
                    table.ForeignKey(
                        name: "custom_fields_value$FK_custom_fields_value_custom_fields",
                        columns: x => new { x.tenant_id, x.school_id, x.category_id, x.field_id },
                        principalTable: "custom_fields",
                        principalColumns: new[] { "tenant_id", "school_id", "category_id", "field_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "search_filter",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "char(15)", fixedLength: true, maxLength: 15, nullable: false, collation: "utf8mb4_general_ci"),
                    filter_id = table.Column<int>(type: "int", nullable: false),
                    filter_name = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    json_list = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "char(150)", fixedLength: true, maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "char(150)", fixedLength: true, maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_filter_tenant_id", x => new { x.tenant_id, x.school_id, x.module, x.filter_id });
                    table.ForeignKey(
                        name: "search_filter$FK_search_filter_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "search_filter$FK_search_filter_user_master",
                        columns: x => new { x.tenant_id, x.school_id, x.emailaddress },
                        principalTable: "user_master",
                        principalColumns: new[] { "tenant_id", "school_id", "emailaddress" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "user_secret_questions",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_general_ci"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    movie = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    hero = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    book = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    cartoon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_secret_questions_tenant_id", x => new { x.tenant_id, x.school_id, x.emailaddress });
                    table.ForeignKey(
                        name: "user_secret_questions$FK_user_secret_questions_user_master",
                        columns: x => new { x.tenant_id, x.school_id, x.emailaddress },
                        principalTable: "user_master",
                        principalColumns: new[] { "tenant_id", "school_id", "emailaddress" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "permission_subcategory",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_subcategory_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_subcategory_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    path = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci"),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    enable_view = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_add = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_edit = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    enable_delete = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_system = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_subcategory_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_subcategory_id });
                    table.ForeignKey(
                        name: "permission_subcategory$FK_permission_subcategory_permission_category",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_category_id },
                        principalTable: "permission_category",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "quarters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    semester_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_exam = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_comments = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quarters_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "quarters$FK_quarters_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "quarters$FK_quarters_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.semester_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_grades",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    assignment_id = table.Column<int>(type: "int", nullable: false),
                    allowed_marks = table.Column<string>(type: "char(5)", fixedLength: true, maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    percentage = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    letter_grade = table.Column<string>(type: "char(5)", fixedLength: true, maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    running_avg = table.Column<string>(type: "char(5)", fixedLength: true, maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    running_avg_grade = table.Column<string>(type: "char(5)", fixedLength: true, maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_grades_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.academic_year, x.marking_period_id, x.course_section_id, x.assignment_type_id, x.assignment_id });
                    table.ForeignKey(
                        name: "gradebook_grades$FK_gradebook_grades_assignment",
                        columns: x => new { x.tenant_id, x.school_id, x.assignment_id },
                        principalTable: "assignment",
                        principalColumns: new[] { "tenant_id", "school_id", "assignment_id" });
                    table.ForeignKey(
                        name: "gradebook_grades$FK_gradebook_grades_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_report_card_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    school_year = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false, collation: "utf8mb4_general_ci"),
                    marking_period_title = table.Column<string>(type: "char(50)", fixedLength: true, maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    grade_title = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_general_ci"),
                    course_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    teacher = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    grade = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    comments = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    teacher_comments = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    overall_teacher_comments = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_report_card_detail", x => x.id);
                    table.ForeignKey(
                        name: "student_report_card_detail$FK_student_report_card_detail_student_report_card_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.school_year, x.marking_period_title },
                        principalTable: "student_report_card_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_transcript_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    grade_title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, defaultValueSql: "(N'')", collation: "utf8mb4_general_ci"),
                    course_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    course_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    credit_hours = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    gp_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_transcript_detail_id", x => new { x.id, x.tenant_id, x.school_id, x.student_id });
                    table.ForeignKey(
                        name: "student_transcript_detail$FK_student_transcript_detail_student_transcript_master1",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.grade_title },
                        principalTable: "student_transcript_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "grade_title" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    role_permission_id = table.Column<int>(type: "int", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    permission_group_id = table.Column<int>(type: "int", nullable: true),
                    permission_category_id = table.Column<int>(type: "int", nullable: true),
                    permission_subcategory_id = table.Column<int>(type: "int", nullable: true),
                    can_view = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    can_add = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    can_edit = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    can_delete = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission_tenant_id", x => new { x.tenant_id, x.school_id, x.role_permission_id });
                    table.ForeignKey(
                        name: "role_permission$FK_role_permission_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_role_permission_permission_category",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_category_id },
                        principalTable: "permission_category",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_role_permission_permission_groupId",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_group_id },
                        principalTable: "permission_group",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_role_permission_permission_subcategory",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_subcategory_id },
                        principalTable: "permission_subcategory",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_subcategory_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "course_section",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_type = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: true, collation: "utf8mb4_general_ci"),
                    course_section_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    attendance_category_id = table.Column<int>(type: "int", nullable: true),
                    credit_hours = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    seats = table.Column<int>(type: "int", nullable: true),
                    allow_student_conflict = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    allow_teacher_conflict = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_weighted_course = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    affects_class_rank = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    affects_honor_roll = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    online_class_room = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    online_classroom_url = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    online_classroom_password = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci"),
                    use_standards = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    standard_grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    duration_based_on_period = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    duration_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    duration_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    schedule_type = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true, collation: "utf8mb4_general_ci"),
                    meeting_days = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    attendance_taken = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_section_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_attendance_code_categories",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id },
                        principalTable: "attendance_code_categories",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_course",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id },
                        principalTable: "course",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_school_calendars",
                        columns: x => new { x.tenant_id, x.school_id, x.calendar_id },
                        principalTable: "school_calendars",
                        principalColumns: new[] { "tenant_id", "school_id", "calender_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "course_section$FK_course_section_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "progress_periods",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    quarter_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    short_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_exam = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    does_comments = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_progress_periods_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id, x.academic_year, x.quarter_id });
                    table.ForeignKey(
                        name: "progress_periods$FK_progress_periods_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.quarter_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_effort_grade_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_effort_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    teacher_comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_effort_grade_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno });
                    table.ForeignKey(
                        name: "student_effort_grade_master$FK_student_effort_grade_master_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_master$FK_student_effort_grade_master_school_calendars",
                        columns: x => new { x.tenant_id, x.school_id, x.calendar_id },
                        principalTable: "school_calendars",
                        principalColumns: new[] { "tenant_id", "school_id", "calender_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_master$FK_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_master$FK_student_effort_grade_master_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_final_grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_final_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    based_on_standard_grade = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    is_percent = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    percent_marks = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade_obtained = table.Column<string>(type: "char(5)", fixedLength: true, maxLength: 5, nullable: true, collation: "utf8mb4_general_ci"),
                    teacher_comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    creditattempted = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    creditearned = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno });
                    table.ForeignKey(
                        name: "student_final_grade$FK_student_final_grade_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_student_final_grade_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_student_final_grade_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_student_final_grade_student_final_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    general = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    score_rounding = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    assignment_sorting = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    max_anomalous_grade = table.Column<int>(type: "int", nullable: true),
                    upgraded_assignment_grade_days = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id });
                    table.ForeignKey(
                        name: "gradebook_configuration$FK_gradebook_configuration_course_section",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "staff_coursesection_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    staff_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    course_section_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    duration_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    duration_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    meeting_days = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci"),
                    is_dropped = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    effective_drop_date = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    is_assigned = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: false),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_coursesection_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_course_section",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_coursesection_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    student_guid = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    alternate_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    student_internal_id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    first_given_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    middle_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci"),
                    last_family_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_general_ci"),
                    first_language_id = table.Column<int>(type: "int", nullable: true),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    course_section_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_general_ci"),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    is_dropped = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    effective_drop_date = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    effective_start_date = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_coursesection_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "student_coursesection_schedule$FK_course_section",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "student_coursesection_schedule$FK_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "student_coursesection_schedule$FK_student_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_effort_grade_detail",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_effort_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: true),
                    effort_item_id = table.Column<int>(type: "int", nullable: true),
                    effort_grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_effort_grade_detail_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno, x.id });
                    table.ForeignKey(
                        name: "student_effort_grade_detail$FK_student_effort_grade_detail_student_effort_grade_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno },
                        principalTable: "student_effort_grade_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_final_grade_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_final_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    course_comment_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno, x.course_comment_id });
                    table.ForeignKey(
                        name: "student_final_grade_comments$FK_student_final_grade_comments_course_comment_category",
                        columns: x => new { x.tenant_id, x.school_id, x.course_comment_id },
                        principalTable: "course_comment_category",
                        principalColumns: new[] { "tenant_id", "school_id", "course_comment_id" });
                    table.ForeignKey(
                        name: "student_final_grade_comments$FK_student_final_grade_comments_student_final_grade1",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno },
                        principalTable: "student_final_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_final_grade_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    student_final_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    standard_grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grade_obtained = table.Column<int>(type: "int", nullable: true),
                    teacher_comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_final_grade_standard$FK_student_final_grade_standard_student_final_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno },
                        principalTable: "student_final_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_gradescale",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    breakoff_points = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_gradescale", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_configuration_gradescale$FK_gradebook_configuration_gradescale_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_quarter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_quarter", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_configuration_quarter$FK_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_quarter$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_semester",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_semester", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_configuration_semester$FK_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_semester$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_semester$FK_semester",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_year",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_year", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_configuration_year$FK_gradebook_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_year$FK_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_year$FK_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    attendance_date = table.Column<DateTime>(type: "date", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    period_id = table.Column<int>(type: "int", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    attendance_code = table.Column<int>(type: "int", nullable: false),
                    student_attendance_id = table.Column<long>(type: "bigint", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.staff_id, x.course_id, x.course_section_id, x.attendance_date, x.block_id, x.period_id });
                    table.UniqueConstraint("AK_student_attendance_tenant_id_school_id_student_id_student_at~", x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id });
                    table.ForeignKey(
                        name: "student_attendance$FK_student_attendance_attendance_code",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id, x.attendance_code },
                        principalTable: "attendance_code",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });
                    table.ForeignKey(
                        name: "student_attendance$FK_student_attendance_block_period",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "student_attendance$FK_student_attendance_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                    table.ForeignKey(
                        name: "student_attendance$FK_staff_coursesection_schedule",
                        columns: x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id },
                        principalTable: "staff_coursesection_schedule",
                        principalColumns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "student_attendance$FK_student_coursesection_schedule",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id },
                        principalTable: "student_coursesection_schedule",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "student_attendance_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_attendance_id = table.Column<long>(type: "bigint", nullable: false),
                    comment_id = table.Column<long>(type: "bigint", nullable: false),
                    comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    comment_timestamp = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id, x.comment_id });
                    table.ForeignKey(
                        name: "student_attendance_comments$FK_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                    table.ForeignKey(
                        name: "student_attendance_comments$FK_student_attendance",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id },
                        principalTable: "student_attendance",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { 1, "AF", null, null, "Afghanistan", null, null, null },
                    { 2, "AL", null, null, "Albania", null, null, null },
                    { 3, "DZ", null, null, "Algeria", null, null, null },
                    { 4, "AS", null, null, "American Samoa", null, null, null },
                    { 5, "AD", null, null, "Andorra", null, null, null },
                    { 6, "AO", null, null, "Angola", null, null, null },
                    { 7, "AI", null, null, "Anguilla", null, null, null },
                    { 8, "AQ", null, null, "Antarctica", null, null, null },
                    { 9, "AG", null, null, "Antigua And Barbuda", null, null, null },
                    { 10, "AR", null, null, "Argentina", null, null, null },
                    { 11, "AM", null, null, "Armenia", null, null, null },
                    { 12, "AW", null, null, "Aruba", null, null, null },
                    { 13, "AU", null, null, "Australia", null, null, null },
                    { 14, "AT", null, null, "Austria", null, null, null },
                    { 15, "AZ", null, null, "Azerbaijan", null, null, null },
                    { 16, "BS", null, null, "Bahamas The", null, null, null },
                    { 17, "BH", null, null, "Bahrain", null, null, null },
                    { 18, "BD", null, null, "Bangladesh", null, null, null },
                    { 19, "BB", null, null, "Barbados", null, null, null },
                    { 20, "BY", null, null, "Belarus", null, null, null },
                    { 21, "BE", null, null, "Belgium", null, null, null },
                    { 22, "BZ", null, null, "Belize", null, null, null },
                    { 23, "BJ", null, null, "Benin", null, null, null },
                    { 24, "BM", null, null, "Bermuda", null, null, null },
                    { 25, "BT", null, null, "Bhutan", null, null, null },
                    { 26, "BO", null, null, "Bolivia", null, null, null },
                    { 27, "BA", null, null, "Bosnia and Herzegovina", null, null, null },
                    { 28, "BW", null, null, "Botswana", null, null, null },
                    { 29, "BV", null, null, "Bouvet Island", null, null, null },
                    { 30, "BR", null, null, "Brazil", null, null, null },
                    { 31, "IO", null, null, "British Indian Ocean Territory", null, null, null },
                    { 32, "BN", null, null, "Brunei", null, null, null },
                    { 33, "BG", null, null, "Bulgaria", null, null, null },
                    { 34, "BF", null, null, "Burkina Faso", null, null, null },
                    { 35, "BI", null, null, "Burundi", null, null, null },
                    { 36, "KH", null, null, "Cambodia", null, null, null },
                    { 37, "CM", null, null, "Cameroon", null, null, null },
                    { 38, "CA", null, null, "Canada", null, null, null },
                    { 39, "CV", null, null, "Cape Verde", null, null, null },
                    { 40, "KY", null, null, "Cayman Islands", null, null, null },
                    { 41, "CF", null, null, "Central African Republic", null, null, null },
                    { 42, "TD", null, null, "Chad", null, null, null },
                    { 43, "CL", null, null, "Chile", null, null, null },
                    { 44, "CN", null, null, "China", null, null, null },
                    { 45, "CX", null, null, "Christmas Island", null, null, null },
                    { 46, "CC", null, null, "Cocos (Keeling) Islands", null, null, null },
                    { 47, "CO", null, null, "Colombia", null, null, null },
                    { 48, "KM", null, null, "Comoros", null, null, null },
                    { 49, "CG", null, null, "Congo", null, null, null },
                    { 50, "CD", null, null, "Congo The Democratic Republic Of The", null, null, null },
                    { 51, "CK", null, null, "Cook Islands", null, null, null },
                    { 52, "CR", null, null, "Costa Rica", null, null, null },
                    { 53, "CI", null, null, "Cote D'Ivoire (Ivory Coast)", null, null, null },
                    { 54, "HR", null, null, "Croatia (Hrvatska)", null, null, null },
                    { 55, "CU", null, null, "Cuba", null, null, null },
                    { 56, "CY", null, null, "Cyprus", null, null, null },
                    { 57, "CZ", null, null, "Czech Republic", null, null, null },
                    { 58, "DK", null, null, "Denmark", null, null, null },
                    { 59, "DJ", null, null, "Djibouti", null, null, null },
                    { 60, "DM", null, null, "Dominica", null, null, null },
                    { 61, "DO", null, null, "Dominican Republic", null, null, null },
                    { 62, "TP", null, null, "East Timor", null, null, null },
                    { 63, "EC", null, null, "Ecuador", null, null, null },
                    { 64, "EG", null, null, "Egypt", null, null, null },
                    { 65, "SV", null, null, "El Salvador", null, null, null },
                    { 66, "GQ", null, null, "Equatorial Guinea", null, null, null },
                    { 67, "ER", null, null, "Eritrea", null, null, null },
                    { 68, "EE", null, null, "Estonia", null, null, null },
                    { 69, "ET", null, null, "Ethiopia", null, null, null },
                    { 70, "XA", null, null, "External Territories of Australia", null, null, null },
                    { 71, "FK", null, null, "Falkland Islands", null, null, null },
                    { 72, "FO", null, null, "Faroe Islands", null, null, null },
                    { 73, "FJ", null, null, "Fiji Islands", null, null, null },
                    { 74, "FI", null, null, "Finland", null, null, null },
                    { 75, "FR", null, null, "France", null, null, null },
                    { 76, "GF", null, null, "French Guiana", null, null, null },
                    { 77, "PF", null, null, "French Polynesia", null, null, null },
                    { 78, "TF", null, null, "French Southern Territories", null, null, null },
                    { 79, "GA", null, null, "Gabon", null, null, null },
                    { 80, "GM", null, null, "Gambia The", null, null, null },
                    { 81, "GE", null, null, "Georgia", null, null, null },
                    { 82, "DE", null, null, "Germany", null, null, null },
                    { 83, "GH", null, null, "Ghana", null, null, null },
                    { 84, "GI", null, null, "Gibraltar", null, null, null },
                    { 85, "GR", null, null, "Greece", null, null, null },
                    { 86, "GL", null, null, "Greenland", null, null, null },
                    { 87, "GD", null, null, "Grenada", null, null, null },
                    { 88, "GP", null, null, "Guadeloupe", null, null, null },
                    { 89, "GU", null, null, "Guam", null, null, null },
                    { 90, "GT", null, null, "Guatemala", null, null, null },
                    { 91, "XU", null, null, "Guernsey and Alderney", null, null, null },
                    { 92, "GN", null, null, "Guinea", null, null, null },
                    { 93, "GW", null, null, "Guinea-Bissau", null, null, null },
                    { 94, "GY", null, null, "Guyana", null, null, null },
                    { 95, "HT", null, null, "Haiti", null, null, null },
                    { 96, "HM", null, null, "Heard and McDonald Islands", null, null, null },
                    { 97, "HN", null, null, "Honduras", null, null, null },
                    { 98, "HK", null, null, "Hong Kong S.A.R.", null, null, null },
                    { 99, "HU", null, null, "Hungary", null, null, null },
                    { 100, "IS", null, null, "Iceland", null, null, null },
                    { 101, "IN", null, null, "India", null, null, null },
                    { 102, "ID", null, null, "Indonesia", null, null, null },
                    { 103, "IR", null, null, "Iran", null, null, null },
                    { 104, "IQ", null, null, "Iraq", null, null, null },
                    { 105, "IE", null, null, "Ireland", null, null, null },
                    { 106, "IL", null, null, "Israel", null, null, null },
                    { 107, "IT", null, null, "Italy", null, null, null },
                    { 108, "JM", null, null, "Jamaica", null, null, null },
                    { 109, "JP", null, null, "Japan", null, null, null },
                    { 110, "XJ", null, null, "Jersey", null, null, null },
                    { 111, "JO", null, null, "Jordan", null, null, null },
                    { 112, "KZ", null, null, "Kazakhstan", null, null, null },
                    { 113, "KE", null, null, "Kenya", null, null, null },
                    { 114, "KI", null, null, "Kiribati", null, null, null },
                    { 115, "KP", null, null, "Korea North", null, null, null },
                    { 116, "KR", null, null, "Korea South", null, null, null },
                    { 117, "KW", null, null, "Kuwait", null, null, null },
                    { 118, "KG", null, null, "Kyrgyzstan", null, null, null },
                    { 119, "LA", null, null, "Laos", null, null, null },
                    { 120, "LV", null, null, "Latvia", null, null, null },
                    { 121, "LB", null, null, "Lebanon", null, null, null },
                    { 122, "LS", null, null, "Lesotho", null, null, null },
                    { 123, "LR", null, null, "Liberia", null, null, null },
                    { 124, "LY", null, null, "Libya", null, null, null },
                    { 125, "LI", null, null, "Liechtenstein", null, null, null },
                    { 126, "LT", null, null, "Lithuania", null, null, null },
                    { 127, "LU", null, null, "Luxembourg", null, null, null },
                    { 128, "MO", null, null, "Macau S.A.R.", null, null, null },
                    { 129, "MK", null, null, "Macedonia", null, null, null },
                    { 130, "MG", null, null, "Madagascar", null, null, null },
                    { 131, "MW", null, null, "Malawi", null, null, null },
                    { 132, "MY", null, null, "Malaysia", null, null, null },
                    { 133, "MV", null, null, "Maldives", null, null, null },
                    { 134, "ML", null, null, "Mali", null, null, null },
                    { 135, "MT", null, null, "Malta", null, null, null },
                    { 136, "XM", null, null, "Man (Isle of)", null, null, null },
                    { 137, "MH", null, null, "Marshall Islands", null, null, null },
                    { 138, "MQ", null, null, "Martinique", null, null, null },
                    { 139, "MR", null, null, "Mauritania", null, null, null },
                    { 140, "MU", null, null, "Mauritius", null, null, null },
                    { 141, "YT", null, null, "Mayotte", null, null, null },
                    { 142, "MX", null, null, "Mexico", null, null, null },
                    { 143, "FM", null, null, "Micronesia", null, null, null },
                    { 144, "MD", null, null, "Moldova", null, null, null },
                    { 145, "MC", null, null, "Monaco", null, null, null },
                    { 146, "MN", null, null, "Mongolia", null, null, null },
                    { 147, "MS", null, null, "Montserrat", null, null, null },
                    { 148, "MA", null, null, "Morocco", null, null, null },
                    { 149, "MZ", null, null, "Mozambique", null, null, null },
                    { 150, "MM", null, null, "Myanmar", null, null, null },
                    { 151, "NA", null, null, "Namibia", null, null, null },
                    { 152, "NR", null, null, "Nauru", null, null, null },
                    { 153, "NP", null, null, "Nepal", null, null, null },
                    { 154, "AN", null, null, "Netherlands Antilles", null, null, null },
                    { 155, "NL", null, null, "Netherlands The", null, null, null },
                    { 156, "NC", null, null, "New Caledonia", null, null, null },
                    { 157, "NZ", null, null, "New Zealand", null, null, null },
                    { 158, "NI", null, null, "Nicaragua", null, null, null },
                    { 159, "NE", null, null, "Niger", null, null, null },
                    { 160, "NG", null, null, "Nigeria", null, null, null },
                    { 161, "NU", null, null, "Niue", null, null, null },
                    { 162, "NF", null, null, "Norfolk Island", null, null, null },
                    { 163, "MP", null, null, "Northern Mariana Islands", null, null, null },
                    { 164, "NO", null, null, "Norway", null, null, null },
                    { 165, "OM", null, null, "Oman", null, null, null },
                    { 166, "PK", null, null, "Pakistan", null, null, null },
                    { 167, "PW", null, null, "Palau", null, null, null },
                    { 168, "PS", null, null, "Palestinian Territory Occupied", null, null, null },
                    { 169, "PA", null, null, "Panama", null, null, null },
                    { 170, "PG", null, null, "Papua new Guinea", null, null, null },
                    { 171, "PY", null, null, "Paraguay", null, null, null },
                    { 172, "PE", null, null, "Peru", null, null, null },
                    { 173, "PH", null, null, "Philippines", null, null, null },
                    { 174, "PN", null, null, "Pitcairn Island", null, null, null },
                    { 175, "PL", null, null, "Poland", null, null, null },
                    { 176, "PT", null, null, "Portugal", null, null, null },
                    { 177, "PR", null, null, "Puerto Rico", null, null, null },
                    { 178, "QA", null, null, "Qatar", null, null, null },
                    { 179, "RE", null, null, "Reunion", null, null, null },
                    { 180, "RO", null, null, "Romania", null, null, null },
                    { 181, "RU", null, null, "Russia", null, null, null },
                    { 182, "RW", null, null, "Rwanda", null, null, null },
                    { 183, "SH", null, null, "Saint Helena", null, null, null },
                    { 184, "KN", null, null, "Saint Kitts And Nevis", null, null, null },
                    { 185, "LC", null, null, "Saint Lucia", null, null, null },
                    { 186, "PM", null, null, "Saint Pierre and Miquelon", null, null, null },
                    { 187, "VC", null, null, "Saint Vincent And The Grenadines", null, null, null },
                    { 188, "WS", null, null, "Samoa", null, null, null },
                    { 189, "SM", null, null, "San Marino", null, null, null },
                    { 190, "ST", null, null, "Sao Tome and Principe", null, null, null },
                    { 191, "SA", null, null, "Saudi Arabia", null, null, null },
                    { 192, "SN", null, null, "Senegal", null, null, null },
                    { 193, "RS", null, null, "Serbia", null, null, null },
                    { 194, "SC", null, null, "Seychelles", null, null, null },
                    { 195, "SL", null, null, "Sierra Leone", null, null, null },
                    { 196, "SG", null, null, "Singapore", null, null, null },
                    { 197, "SK", null, null, "Slovakia", null, null, null },
                    { 198, "SI", null, null, "Slovenia", null, null, null },
                    { 199, "XG", null, null, "Smaller Territories of the UK", null, null, null },
                    { 200, "SB", null, null, "Solomon Islands", null, null, null },
                    { 201, "SO", null, null, "Somalia", null, null, null },
                    { 202, "ZA", null, null, "South Africa", null, null, null },
                    { 203, "GS", null, null, "South Georgia", null, null, null },
                    { 204, "SS", null, null, "South Sudan", null, null, null },
                    { 205, "ES", null, null, "Spain", null, null, null },
                    { 206, "LK", null, null, "Sri Lanka", null, null, null },
                    { 207, "SD", null, null, "Sudan", null, null, null },
                    { 208, "SR", null, null, "Suriname", null, null, null },
                    { 209, "SJ", null, null, "Svalbard And Jan Mayen Islands", null, null, null },
                    { 210, "SZ", null, null, "Swaziland", null, null, null },
                    { 211, "SE", null, null, "Sweden", null, null, null },
                    { 212, "CH", null, null, "Switzerland", null, null, null },
                    { 213, "SY", null, null, "Syria", null, null, null },
                    { 214, "TW", null, null, "Taiwan", null, null, null },
                    { 215, "TJ", null, null, "Tajikistan", null, null, null },
                    { 216, "TZ", null, null, "Tanzania", null, null, null },
                    { 217, "TH", null, null, "Thailand", null, null, null },
                    { 218, "TG", null, null, "Togo", null, null, null },
                    { 219, "TK", null, null, "Tokelau", null, null, null },
                    { 220, "TO", null, null, "Tonga", null, null, null },
                    { 221, "TT", null, null, "Trinidad And Tobago", null, null, null },
                    { 222, "TN", null, null, "Tunisia", null, null, null },
                    { 223, "TR", null, null, "Turkey", null, null, null },
                    { 224, "TM", null, null, "Turkmenistan", null, null, null },
                    { 225, "TC", null, null, "Turks And Caicos Islands", null, null, null },
                    { 226, "TV", null, null, "Tuvalu", null, null, null },
                    { 227, "UG", null, null, "Uganda", null, null, null },
                    { 228, "UA", null, null, "Ukraine", null, null, null },
                    { 229, "AE", null, null, "United Arab Emirates", null, null, null },
                    { 230, "GB", null, null, "United Kingdom", null, null, null },
                    { 231, "US", null, null, "United States", null, null, null },
                    { 232, "UM", null, null, "United States Minor Outlying Islands", null, null, null },
                    { 233, "UY", null, null, "Uruguay", null, null, null },
                    { 234, "UZ", null, null, "Uzbekistan", null, null, null },
                    { 235, "VU", null, null, "Vanuatu", null, null, null },
                    { 236, "VA", null, null, "Vatican City State (Holy See)", null, null, null },
                    { 237, "VE", null, null, "Venezuela", null, null, null },
                    { 238, "VN", null, null, "Vietnam", null, null, null },
                    { 239, "VG", null, null, "Virgin Islands (British)", null, null, null },
                    { 240, "VI", null, null, "Virgin Islands (US)", null, null, null },
                    { 241, "WF", null, null, "Wallis And Futuna Islands", null, null, null },
                    { 242, "EH", null, null, "Western Sahara", null, null, null },
                    { 243, "YE", null, null, "Yemen", null, null, null },
                    { 244, "YU", null, null, "Yugoslavia", null, null, null },
                    { 245, "ZM", null, null, "Zambia", null, null, null },
                    { 246, "ZW", null, null, "Zimbabwe", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "grade_age_range",
                columns: new[] { "age_range_id", "age_range", "created_by", "created_on", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { 0, "Below 5", null, null, null, null },
                    { 1, "5–6", null, null, null, null },
                    { 2, "6–7", null, null, null, null },
                    { 3, "7–8", null, null, null, null },
                    { 4, "8–9", null, null, null, null },
                    { 5, "9–10", null, null, null, null },
                    { 6, "10–11", null, null, null, null },
                    { 7, "11–12", null, null, null, null },
                    { 8, "12–13", null, null, null, null },
                    { 9, "13–14", null, null, null, null },
                    { 10, "14–15", null, null, null, null },
                    { 11, "15–16", null, null, null, null },
                    { 12, "16–17", null, null, null, null },
                    { 13, "17–18", null, null, null, null },
                    { 14, "18+", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "grade_educational_stage",
                columns: new[] { "isced_code", "created_by", "created_on", "educational_stage", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { 0, null, null, "Early childhood Education", null, null },
                    { 1, null, null, "Primary education", null, null },
                    { 2, null, null, "Lower secondary education", null, null },
                    { 3, null, null, "Upper secondary education", null, null },
                    { 4, null, null, "Post-secondary non-tertiary education", null, null },
                    { 5, null, null, "Short-cycle tertiary education", null, null }
                });

            migrationBuilder.InsertData(
                table: "grade_educational_stage",
                columns: new[] { "isced_code", "created_by", "created_on", "educational_stage", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { 6, null, null, "Bachelor's degree or equivalent", null, null },
                    { 7, null, null, "Master's degree or equivalent", null, null },
                    { 8, null, null, "Doctoral degree or equivalent", null, null }
                });

            migrationBuilder.InsertData(
                table: "grade_equivalency",
                columns: new[] { "equivalency_id", "created_by", "created_on", "grade_level_equivalency", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { -1, null, null, "Pre-Kindergarten", null, null },
                    { 0, null, null, "Kindergarten", null, null },
                    { 1, null, null, "1st Grade", null, null },
                    { 2, null, null, "2nd Grade", null, null },
                    { 3, null, null, "3rd Grade", null, null },
                    { 4, null, null, "4th Grade", null, null },
                    { 5, null, null, "5th Grade", null, null },
                    { 6, null, null, "6th Grade", null, null },
                    { 7, null, null, "7th Grade", null, null },
                    { 8, null, null, "8th Grade", null, null },
                    { 9, null, null, "9th Grade", null, null },
                    { 10, null, null, "10th Grade", null, null },
                    { 11, null, null, "11th Grade", null, null },
                    { 12, null, null, "12th Grade", null, null },
                    { 13, null, null, "1st Year College", null, null },
                    { 14, null, null, "2nd Year College", null, null },
                    { 15, null, null, "3rd Year College", null, null },
                    { 16, null, null, "4th Year College", null, null },
                    { 17, null, null, "5th Year College", null, null },
                    { 18, null, null, "6th Year College", null, null },
                    { 19, null, null, "7th Year College", null, null },
                    { 20, null, null, "8th Year College", null, null }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "created_by", "created_on", "language_code", "lcid", "locale", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { 1, null, null, "af", "af", "Afrikaans", null, null },
                    { 2, null, null, "sq", "sq", "Albanian", null, null },
                    { 3, null, null, "am", "am", "Amharic", null, null },
                    { 4, null, null, "ar", "ar-dz", "Arabic - Algeria", null, null },
                    { 5, null, null, "ar", "ar-bh", "Arabic - Bahrain", null, null },
                    { 6, null, null, "ar", "ar-eg", "Arabic - Egypt", null, null },
                    { 7, null, null, "ar", "ar-iq", "Arabic - Iraq", null, null },
                    { 8, null, null, "ar", "ar-jo", "Arabic - Jordan", null, null },
                    { 9, null, null, "ar", "ar-kw", "Arabic - Kuwait", null, null },
                    { 10, null, null, "ar", "ar-lb", "Arabic - Lebanon", null, null },
                    { 11, null, null, "ar", "ar-ly", "Arabic - Libya", null, null },
                    { 12, null, null, "ar", "ar-ma", "Arabic - Morocco", null, null },
                    { 13, null, null, "ar", "ar-om", "Arabic - Oman", null, null },
                    { 14, null, null, "ar", "ar-qa", "Arabic - Qatar", null, null },
                    { 15, null, null, "ar", "ar-sa", "Arabic - Saudi Arabia", null, null },
                    { 16, null, null, "ar", "ar-sy", "Arabic - Syria", null, null },
                    { 17, null, null, "ar", "ar-tn", "Arabic - Tunisia", null, null },
                    { 18, null, null, "ar", "ar-ae", "Arabic - United Arab Emirates", null, null },
                    { 19, null, null, "ar", "ar-ye", "Arabic - Yemen", null, null },
                    { 20, null, null, "hy", "hy", "Armenian", null, null },
                    { 21, null, null, "as", "as", "Assamese", null, null },
                    { 22, null, null, "az", "az-az", "Azeri - Cyrillic", null, null },
                    { 23, null, null, "az", "az-az", "Azeri - Latin", null, null },
                    { 24, null, null, "eu", "eu", "Basque", null, null },
                    { 25, null, null, "be", "be", "Belarusian", null, null },
                    { 26, null, null, "bn", "bn", "Bengali - Bangladesh", null, null },
                    { 27, null, null, "bn", "bn", "Bengali - India", null, null },
                    { 28, null, null, "bs", "bs", "Bosnian", null, null },
                    { 29, null, null, "bg", "bg", "Bulgarian", null, null },
                    { 30, null, null, "my", "my", "Burmese", null, null },
                    { 31, null, null, "ca", "ca", "Catalan", null, null },
                    { 32, null, null, "zh", "zh-cn", "Chinese - China", null, null },
                    { 33, null, null, "zh", "zh-hk", "Chinese - Hong Kong SAR", null, null },
                    { 34, null, null, "zh", "zh-mo", "Chinese - Macau SAR", null, null },
                    { 35, null, null, "zh", "zh-sg", "Chinese - Singapore", null, null },
                    { 36, null, null, "zh", "zh-tw", "Chinese - Taiwan", null, null },
                    { 37, null, null, "hr", "hr", "Croatian", null, null },
                    { 38, null, null, "cs", "cs", "Czech", null, null },
                    { 39, null, null, "da", "da", "Danish", null, null },
                    { 40, null, null, "Dhivehi", "Maldivian", "Divehi", null, null },
                    { 41, null, null, "nl", "nl-be", "Dutch - Belgium", null, null },
                    { 42, null, null, "nl", "nl-nl", "Dutch - Netherlands", null, null },
                    { 43, null, null, "en", "en-au", "English - Australia", null, null },
                    { 44, null, null, "en", "en-bz", "English - Belize", null, null },
                    { 45, null, null, "en", "en-ca", "English - Canada", null, null },
                    { 46, null, null, "en", "en-cb", "English - Caribbean", null, null },
                    { 47, null, null, "en", "en-gb", "English - Great Britain", null, null },
                    { 48, null, null, "en", "en-in", "English - India", null, null },
                    { 49, null, null, "en", "en-ie", "English - Ireland", null, null },
                    { 50, null, null, "en", "en-jm", "English - Jamaica", null, null },
                    { 51, null, null, "en", "en-nz", "English - New Zealand", null, null },
                    { 52, null, null, "en", "en-ph", "English - Philippines", null, null },
                    { 53, null, null, "en", "en-za", "English - Southern Africa", null, null },
                    { 54, null, null, "en", "en-tt", "English - Trinidad", null, null },
                    { 55, null, null, "en", "en-us", "English - United States", null, null },
                    { 56, null, null, "et", "et", "Estonian", null, null },
                    { 57, null, null, "mk", "mk", "FYRO Macedonia", null, null },
                    { 58, null, null, "fo", "fo", "Faroese", null, null },
                    { 59, null, null, "fa", "fa", "Farsi - Persian", null, null },
                    { 60, null, null, "fi", "fi", "Finnish", null, null },
                    { 61, null, null, "fr", "fr-be", "French - Belgium", null, null },
                    { 62, null, null, "fr", "fr-ca", "French - Canada", null, null },
                    { 63, null, null, "fr", "fr-fr", "French - France", null, null },
                    { 64, null, null, "fr", "fr-lu", "French - Luxembourg", null, null },
                    { 65, null, null, "fr", "fr-ch", "French - Switzerland", null, null },
                    { 66, null, null, "gd", "gd-ie", "Gaelic - Ireland", null, null },
                    { 67, null, null, "gd", "gd", "Gaelic - Scotland", null, null },
                    { 68, null, null, "de", "de-at", "German - Austria", null, null },
                    { 69, null, null, "de", "de-de", "German - Germany", null, null },
                    { 70, null, null, "de", "de-li", "German - Liechtenstein", null, null },
                    { 71, null, null, "de", "de-lu", "German - Luxembourg", null, null },
                    { 72, null, null, "de", "de-ch", "German - Switzerland", null, null },
                    { 73, null, null, "el", "el", "Greek", null, null },
                    { 74, null, null, "gn", "gn", "Guarani - Paraguay", null, null },
                    { 75, null, null, "gu", "gu", "Gujarati", null, null },
                    { 76, null, null, "he", "he", "Hebrew", null, null },
                    { 77, null, null, "hi", "hi", "Hindi", null, null },
                    { 78, null, null, "hu", "hu", "Hungarian", null, null },
                    { 79, null, null, "is", "is", "Icelandic", null, null },
                    { 80, null, null, "id", "id", "Indonesian", null, null },
                    { 81, null, null, "it", "it-it", "Italian - Italy", null, null },
                    { 82, null, null, "it", "it-ch", "Italian - Switzerland", null, null },
                    { 83, null, null, "ja", "ja", "Japanese", null, null },
                    { 84, null, null, "kn", "kn", "Kannada", null, null },
                    { 85, null, null, "ks", "ks", "Kashmiri", null, null },
                    { 86, null, null, "kk", "kk", "Kazakh", null, null },
                    { 87, null, null, "km", "km", "Khmer", null, null },
                    { 88, null, null, "ko", "ko", "Korean", null, null },
                    { 89, null, null, "lo", "lo", "Lao", null, null },
                    { 90, null, null, "la", "la", "Latin", null, null },
                    { 91, null, null, "lv", "lv", "Latvian", null, null },
                    { 92, null, null, "lt", "lt", "Lithuanian", null, null },
                    { 93, null, null, "ms", "ms-bn", "Malay - Brunei", null, null },
                    { 94, null, null, "ms", "ms-my", "Malay - Malaysia", null, null },
                    { 95, null, null, "ml", "ml", "Malayalam", null, null },
                    { 96, null, null, "mt", "mt", "Maltese", null, null },
                    { 97, null, null, "mi", "mi", "Maori", null, null },
                    { 98, null, null, "mr", "mr", "Marathi", null, null },
                    { 99, null, null, "mn", "mn", "Mongolian", null, null },
                    { 100, null, null, "mn", "mn", "Mongolian", null, null },
                    { 101, null, null, "ne", "ne", "Nepali", null, null },
                    { 102, null, null, "nb", "no-no", "Norwegian - Bokml", null, null },
                    { 103, null, null, "nn", "no-no", "Norwegian - Nynorsk", null, null },
                    { 104, null, null, "or", "or", "Oriya", null, null },
                    { 105, null, null, "pl", "pl", "Polish", null, null },
                    { 106, null, null, "pt", "pt-br", "Portuguese - Brazil", null, null },
                    { 107, null, null, "pt", "pt-pt", "Portuguese - Portugal", null, null },
                    { 108, null, null, "pa", "pa", "Punjabi", null, null },
                    { 109, null, null, "rm", "rm", "Raeto-Romance", null, null },
                    { 110, null, null, "ro", "ro-mo", "Romanian - Moldova", null, null },
                    { 111, null, null, "ro", "ro", "Romanian - Romania", null, null },
                    { 112, null, null, "ru", "ru", "Russian", null, null },
                    { 113, null, null, "ru", "ru-mo", "Russian - Moldova", null, null },
                    { 114, null, null, "sa", "sa", "Sanskrit", null, null },
                    { 115, null, null, "sr", "sr-sp", "Serbian - Cyrillic", null, null },
                    { 116, null, null, "sr", "sr-sp", "Serbian - Latin", null, null },
                    { 117, null, null, "tn", "tn", "Setsuana", null, null },
                    { 118, null, null, "sd", "sd", "Sindhi", null, null },
                    { 119, null, null, "Sinhalese", "si", "Sinhala", null, null },
                    { 120, null, null, "sk", "sk", "Slovak", null, null },
                    { 121, null, null, "sl", "sl", "Slovenian", null, null },
                    { 122, null, null, "so", "so", "Somali", null, null },
                    { 123, null, null, "sb", "sb", "Sorbian", null, null },
                    { 124, null, null, "es", "es-ar", "Spanish - Argentina", null, null },
                    { 125, null, null, "es", "es-bo", "Spanish - Bolivia", null, null },
                    { 126, null, null, "es", "es-cl", "Spanish - Chile", null, null },
                    { 127, null, null, "es", "es-co", "Spanish - Colombia", null, null },
                    { 128, null, null, "es", "es-cr", "Spanish - Costa Rica", null, null },
                    { 129, null, null, "es", "es-do", "Spanish - Dominican Republic", null, null },
                    { 130, null, null, "es", "es-ec", "Spanish - Ecuador", null, null },
                    { 131, null, null, "es", "es-sv", "Spanish - El Salvador", null, null },
                    { 132, null, null, "es", "es-gt", "Spanish - Guatemala", null, null },
                    { 133, null, null, "es", "es-hn", "Spanish - Honduras", null, null },
                    { 134, null, null, "es", "es-mx", "Spanish - Mexico", null, null },
                    { 135, null, null, "es", "es-ni", "Spanish - Nicaragua", null, null },
                    { 136, null, null, "es", "es-pa", "Spanish - Panama", null, null },
                    { 137, null, null, "es", "es-py", "Spanish - Paraguay", null, null },
                    { 138, null, null, "es", "es-pe", "Spanish - Peru", null, null },
                    { 139, null, null, "es", "es-pr", "Spanish - Puerto Rico", null, null },
                    { 140, null, null, "es", "es-es", "Spanish - Spain (Traditional)", null, null },
                    { 141, null, null, "es", "es-uy", "Spanish - Uruguay", null, null },
                    { 142, null, null, "es", "es-ve", "Spanish - Venezuela", null, null },
                    { 143, null, null, "sw", "sw", "Swahili", null, null },
                    { 144, null, null, "sv", "sv-fi", "Swedish - Finland", null, null },
                    { 145, null, null, "sv", "sv-se", "Swedish - Sweden", null, null },
                    { 146, null, null, "tg", "tg", "Tajik", null, null },
                    { 147, null, null, "ta", "ta", "Tamil", null, null },
                    { 148, null, null, "tt", "tt", "Tatar", null, null },
                    { 149, null, null, "te", "te", "Telugu", null, null },
                    { 150, null, null, "th", "th", "Thai", null, null },
                    { 151, null, null, "bo", "bo", "Tibetan", null, null },
                    { 152, null, null, "ts", "ts", "Tsonga", null, null },
                    { 153, null, null, "tr", "tr", "Turkish", null, null },
                    { 154, null, null, "tk", "tk", "Turkmen", null, null },
                    { 155, null, null, "uk", "uk", "Ukrainian", null, null },
                    { 157, null, null, "ur", "ur", "Urdu", null, null },
                    { 158, null, null, "uz", "uz-uz", "Uzbek - Cyrillic", null, null },
                    { 159, null, null, "uz", "uz-uz", "Uzbek - Latin", null, null },
                    { 160, null, null, "vi", "vi", "Vietnamese", null, null },
                    { 161, null, null, "cy", "cy", "Welsh", null, null },
                    { 162, null, null, "xh", "xh", "Xhosa", null, null },
                    { 163, null, null, "yi", "yi", "Yiddish", null, null },
                    { 164, null, null, "zu", "zu", "Zulu", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_api_controller_key_mapping_tenant_id_controller_id",
                table: "api_controller_key_mapping",
                columns: new[] { "tenant_id", "controller_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type_id",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_type_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_staff_id",
                table: "assignment",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_city_stateid",
                table: "city",
                column: "stateid");

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_room_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_block_id_period~",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_room_id",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_room_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_attendance_category_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_calendar_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "calendar_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_period_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_marking_period_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_period_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_standard_tenant_id_school_id_standard_ref_no_grade_st~",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_block_id_period~",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_room_id",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_custom_fields",
                table: "custom_fields",
                columns: new[] { "tenant_id", "school_id", "title" });

            migrationBuilder.CreateIndex(
                name: "IX_dpdown_valuelist_tenant_id_school_id",
                table: "dpdown_valuelist",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_gradescale",
                table: "gradebook_configuration_gradescale",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_course_i~",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_qtr_mark~",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_course_~",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_qtr_mar~",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_smstr_m~",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_course_id_c~",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_smstr_marki~",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_yr_marking_~",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment_id",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradelevels_age_range_id",
                table: "gradelevels",
                column: "age_range_id");

            migrationBuilder.CreateIndex(
                name: "IX_gradelevels_equivalency_id",
                table: "gradelevels",
                column: "equivalency_id");

            migrationBuilder.CreateIndex(
                name: "IX_gradelevels_isced_code",
                table: "gradelevels",
                column: "isced_code");

            migrationBuilder.CreateIndex(
                name: "historical_marking_period$IX_historical_marking_period",
                table: "historical_marking_period",
                columns: new[] { "tenant_id", "school_id", "hist_marking_period_id", "academic_year", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_id",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notice",
                table: "notice",
                columns: new[] { "tenant_id", "school_id", "notice_id", "created_on", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_permission_category_tenant_id_school_id_permission_group_id",
                table: "permission_category",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.CreateIndex(
                name: "IX_permission_subcategory_tenant_id_school_id_permission_catego~",
                table: "permission_subcategory",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_progress_periods_tenant_id_school_id_quarter_id",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "quarter_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quarters_tenant_id_school_id_semester_id",
                table: "quarters",
                columns: new[] { "tenant_id", "school_id", "semester_id" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_tenant_id_school_id_membership_id",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_category_id",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_group_id",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_subcategory_id",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_subcategory_id" });

            migrationBuilder.CreateIndex(
                name: "IX_school_detail_tenant_id_school_id",
                table: "school_detail",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_school_master_tenant_id_school_id_plan_id",
                table: "school_master",
                columns: new[] { "tenant_id", "school_id", "plan_id" });

            migrationBuilder.CreateIndex(
                name: "IX_search_filter",
                table: "search_filter",
                columns: new[] { "tenant_id", "school_id", "filter_name" });

            migrationBuilder.CreateIndex(
                name: "IX_search_filter_tenant_id_school_id_emailaddress",
                table: "search_filter",
                columns: new[] { "tenant_id", "school_id", "emailaddress" });

            migrationBuilder.CreateIndex(
                name: "IX_semesters_tenant_id_school_id_year_id",
                table: "semesters",
                columns: new[] { "tenant_id", "school_id", "year_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_certificate_info_tenant_id_staff_id",
                table: "staff_certificate_info",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_course_id_c~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_qtr_marking~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_smstr_marki~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_yr_marking_~",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_staff_id",
                table: "staff_coursesection_schedule",
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
                name: "IX_staff_master_tenant_id_school_id",
                table: "staff_master",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_master_third_language",
                table: "staff_master",
                column: "third_language");

            migrationBuilder.CreateIndex(
                name: "IX_staff_school_info_tenant_id_staff_id",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_state_countryid",
                table: "state",
                column: "countryid");

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_1",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_2",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_3",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "attendance_date" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_attendance_category_i~",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_block_id_period_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_membership_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_staff_id_course_id_co~",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_student_id_course_id_~",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "student_attendance$AK_student_attendance_tenant_id_school_id_student_id_student_at~",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "student_attendance$student_attendance_id_idx",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id_membership_id",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_coursesection_schedule",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_coursesection_schedule_1",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_grade_scale_id",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_section_id",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_calendar_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "calendar_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_qtr_marking_~",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_smstr_markin~",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_yr_marking_p~",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_guid",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_guid" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_qtr_marking_period_id",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_smstr_marking_period~",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_yr_marking_period_id",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_comments_tenant_id_school_id_course_comm~",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "course_comment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_standard_tenant_id_school_id_student_id_~",
                table: "student_final_grade_standard",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });

            migrationBuilder.CreateIndex(
                name: "IX_student_master_first_language_id",
                table: "student_master",
                column: "first_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_master_second_language_id",
                table: "student_master",
                column: "second_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_master_tenant_id_school_id_section_id",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_master_third_language_id",
                table: "student_master",
                column: "third_language_id");

            migrationBuilder.CreateIndex(
                name: "student_master$AK_student_master_tenant_id_school_id_student_guid",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "student_master$IX_student_master",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_report_card_detail_tenant_id_school_id_student_id_sc~",
                table: "student_report_card_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });

            migrationBuilder.CreateIndex(
                name: "IX_student_transcript_detail_tenant_id_school_id_student_id_gra~",
                table: "student_transcript_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "grade_title" });

            migrationBuilder.CreateIndex(
                name: "IX_user_master_lang_id",
                table: "user_master",
                column: "lang_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_master_tenant_id_school_id_membership_id",
                table: "user_master",
                columns: new[] { "tenant_id", "school_id", "membership_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_controller_key_mapping");

            migrationBuilder.DropTable(
                name: "bell_schedule");

            migrationBuilder.DropTable(
                name: "calendar_events");

            migrationBuilder.DropTable(
                name: "city");

            migrationBuilder.DropTable(
                name: "course_block_schedule");

            migrationBuilder.DropTable(
                name: "course_calendar_schedule");

            migrationBuilder.DropTable(
                name: "course_fixed_schedule");

            migrationBuilder.DropTable(
                name: "course_standard");

            migrationBuilder.DropTable(
                name: "course_variable_schedule");

            migrationBuilder.DropTable(
                name: "custom_fields_value");

            migrationBuilder.DropTable(
                name: "dpdown_valuelist");

            migrationBuilder.DropTable(
                name: "effort_grade_library_category_item");

            migrationBuilder.DropTable(
                name: "effort_grade_scale");

            migrationBuilder.DropTable(
                name: "grade");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_gradescale");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_quarter");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_semester");

            migrationBuilder.DropTable(
                name: "gradebook_configuration_year");

            migrationBuilder.DropTable(
                name: "gradebook_grades");

            migrationBuilder.DropTable(
                name: "historical_credit_transfer");

            migrationBuilder.DropTable(
                name: "historical_marking_period");

            migrationBuilder.DropTable(
                name: "honor_rolls");

            migrationBuilder.DropTable(
                name: "login_session");

            migrationBuilder.DropTable(
                name: "notice");

            migrationBuilder.DropTable(
                name: "parent_address");

            migrationBuilder.DropTable(
                name: "parent_associationship");

            migrationBuilder.DropTable(
                name: "programs");

            migrationBuilder.DropTable(
                name: "progress_periods");

            migrationBuilder.DropTable(
                name: "release_number");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "school_detail");

            migrationBuilder.DropTable(
                name: "school_preference");

            migrationBuilder.DropTable(
                name: "school_rollover");

            migrationBuilder.DropTable(
                name: "search_filter");

            migrationBuilder.DropTable(
                name: "staff_certificate_info");

            migrationBuilder.DropTable(
                name: "staff_schedule_view");

            migrationBuilder.DropTable(
                name: "staff_school_info");

            migrationBuilder.DropTable(
                name: "student_attendance_comments");

            migrationBuilder.DropTable(
                name: "student_attendance_history");

            migrationBuilder.DropTable(
                name: "student_comments");

            migrationBuilder.DropTable(
                name: "student_daily_attendance");

            migrationBuilder.DropTable(
                name: "student_documents");

            migrationBuilder.DropTable(
                name: "student_effort_grade_detail");

            migrationBuilder.DropTable(
                name: "student_enrollment");

            migrationBuilder.DropTable(
                name: "student_enrollment_code");

            migrationBuilder.DropTable(
                name: "student_final_grade_comments");

            migrationBuilder.DropTable(
                name: "student_final_grade_standard");

            migrationBuilder.DropTable(
                name: "student_medical_alert");

            migrationBuilder.DropTable(
                name: "student_medical_immunization");

            migrationBuilder.DropTable(
                name: "student_medical_note");

            migrationBuilder.DropTable(
                name: "student_medical_nurse_visit");

            migrationBuilder.DropTable(
                name: "student_medical_provider");

            migrationBuilder.DropTable(
                name: "student_report_card_detail");

            migrationBuilder.DropTable(
                name: "student_schedule_view");

            migrationBuilder.DropTable(
                name: "student_transcript_detail");

            migrationBuilder.DropTable(
                name: "subject");

            migrationBuilder.DropTable(
                name: "user_access_log");

            migrationBuilder.DropTable(
                name: "user_secret_questions");

            migrationBuilder.DropTable(
                name: "api_controller_list");

            migrationBuilder.DropTable(
                name: "api_keys_master");

            migrationBuilder.DropTable(
                name: "state");

            migrationBuilder.DropTable(
                name: "block");

            migrationBuilder.DropTable(
                name: "grade_us_standard");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "custom_fields");

            migrationBuilder.DropTable(
                name: "effort_grade_library_category");

            migrationBuilder.DropTable(
                name: "gradebook_configuration");

            migrationBuilder.DropTable(
                name: "assignment");

            migrationBuilder.DropTable(
                name: "historical_grade");

            migrationBuilder.DropTable(
                name: "parent_info");

            migrationBuilder.DropTable(
                name: "permission_subcategory");

            migrationBuilder.DropTable(
                name: "student_attendance");

            migrationBuilder.DropTable(
                name: "student_effort_grade_master");

            migrationBuilder.DropTable(
                name: "gradelevels");

            migrationBuilder.DropTable(
                name: "course_comment_category");

            migrationBuilder.DropTable(
                name: "student_final_grade");

            migrationBuilder.DropTable(
                name: "student_report_card_master");

            migrationBuilder.DropTable(
                name: "student_transcript_master");

            migrationBuilder.DropTable(
                name: "user_master");

            migrationBuilder.DropTable(
                name: "country");

            migrationBuilder.DropTable(
                name: "fields_category");

            migrationBuilder.DropTable(
                name: "assignment_type");

            migrationBuilder.DropTable(
                name: "permission_category");

            migrationBuilder.DropTable(
                name: "attendance_code");

            migrationBuilder.DropTable(
                name: "block_period");

            migrationBuilder.DropTable(
                name: "staff_coursesection_schedule");

            migrationBuilder.DropTable(
                name: "student_coursesection_schedule");

            migrationBuilder.DropTable(
                name: "grade_age_range");

            migrationBuilder.DropTable(
                name: "grade_educational_stage");

            migrationBuilder.DropTable(
                name: "grade_equivalency");

            migrationBuilder.DropTable(
                name: "membership");

            migrationBuilder.DropTable(
                name: "permission_group");

            migrationBuilder.DropTable(
                name: "staff_master");

            migrationBuilder.DropTable(
                name: "course_section");

            migrationBuilder.DropTable(
                name: "student_master");

            migrationBuilder.DropTable(
                name: "attendance_code_categories");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropTable(
                name: "grade_scale");

            migrationBuilder.DropTable(
                name: "quarters");

            migrationBuilder.DropTable(
                name: "school_calendars");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.DropTable(
                name: "sections");

            migrationBuilder.DropTable(
                name: "semesters");

            migrationBuilder.DropTable(
                name: "school_years");

            migrationBuilder.DropTable(
                name: "school_master");

            migrationBuilder.DropTable(
                name: "plans");
        }
    }
}
