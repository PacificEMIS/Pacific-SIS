using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class InitialMigration_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "api_controller_list",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    controller_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    api_title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    controller_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_controller_list_tenant_id", x => new { x.TenantId, x.controller_id });
                });

            migrationBuilder.CreateTable(
                name: "api_keys_master",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    key_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    api_key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    api_title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expires = table.Column<DateTime>(type: "date", nullable: true),
                    revoked = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_keys_master_tenant_id", x => new { x.TenantId, x.school_id, x.key_id });
                });

            migrationBuilder.CreateTable(
                name: "assignment_type",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    course_section_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    weightage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_type_tenant_id", x => new { x.tenant_id, x.school_id, x.assignment_type_id });
                });

            migrationBuilder.CreateTable(
                name: "bell_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    bell_schedule_date = table.Column<DateTime>(type: "date", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bell_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.academic_year, x.bell_schedule_date });
                });

            migrationBuilder.CreateTable(
                name: "calendar_events",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    calendar_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_date = table.Column<DateTime>(type: "date", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    visible_to_membership_id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    event_color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    is_holiday = table.Column<bool>(type: "bit", nullable: true),
                    applicable_to_all_school = table.Column<bool>(type: "bit", nullable: true),
                    system_wide_event = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_events_tenant_id", x => new { x.tenant_id, x.calendar_id, x.event_id });
                });

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    countrycode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    course_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    course_short_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    course_grade_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    course_program = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    course_subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    course_category = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    credit_hours = table.Column<double>(type: "float", nullable: true),
                    standard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    standard_ref_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    course_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_course_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id });
                });

            migrationBuilder.CreateTable(
                name: "course_comment_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_comment_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    course_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    applicable_all_courses = table.Column<bool>(type: "bit", nullable: false),
                    comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValueSql: "(N'')"),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_comment_category_tenant_id", x => new { x.tenant_id, x.school_id, x.course_comment_id });
                });

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_library_category_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_category_id });
                });

            migrationBuilder.CreateTable(
                name: "effort_grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_value = table.Column<int>(type: "int", nullable: true),
                    grade_scale_comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_scale_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_grade_scale_id });
                });

            migrationBuilder.CreateTable(
                name: "grade_age_range",
                columns: table => new
                {
                    age_range_id = table.Column<int>(type: "int", nullable: false),
                    age_range = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_age_range_age_range_id", x => x.age_range_id);
                });

            migrationBuilder.CreateTable(
                name: "grade_educational_stage",
                columns: table => new
                {
                    isced_code = table.Column<int>(type: "int", nullable: false),
                    educational_stage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_educational_stage_isced_code", x => x.isced_code);
                });

            migrationBuilder.CreateTable(
                name: "grade_equivalency",
                columns: table => new
                {
                    equivalency_id = table.Column<int>(type: "int", nullable: false),
                    grade_level_equivalency = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_equivalency_equivalency_id", x => x.equivalency_id);
                });

            migrationBuilder.CreateTable(
                name: "grade_us_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    standard_ref_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    grade_standard_id = table.Column<int>(type: "int", nullable: false),
                    grade_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    subject = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    course = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standard_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_school_specific = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_us_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.standard_ref_no, x.grade_standard_id });
                });

            migrationBuilder.CreateTable(
                name: "historical_grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    hist_grade_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    school_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    equivalency_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id });
                });

            migrationBuilder.CreateTable(
                name: "historical_marking_period",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    grade_post_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "bit", nullable: true),
                    does_exam = table.Column<bool>(type: "bit", nullable: true),
                    does_comments = table.Column<bool>(type: "bit", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_marking_period_tenant_id", x => new { x.tenant_id, x.school_id, x.hist_marking_period_id });
                });

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    lang_id = table.Column<int>(type: "int", nullable: false),
                    lcid = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    locale = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    language_code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_language_lang_id", x => x.lang_id);
                });

            migrationBuilder.CreateTable(
                name: "login_session",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ipaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_expired = table.Column<bool>(type: "bit", nullable: true),
                    login_time = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_session_id", x => new { x.id, x.tenant_id, x.school_id, x.emailaddress });
                });

            migrationBuilder.CreateTable(
                name: "notice",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    notice_id = table.Column<int>(type: "int", nullable: false),
                    target_membership_ids = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valid_from = table.Column<DateTime>(type: "date", nullable: false),
                    valid_to = table.Column<DateTime>(type: "date", nullable: false),
                    isactive = table.Column<bool>(type: "bit", nullable: false),
                    visible_to_all_school = table.Column<bool>(type: "bit", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notice_tenant_id", x => new { x.tenant_id, x.school_id, x.notice_id });
                });

            migrationBuilder.CreateTable(
                name: "parent_associationship",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    associationship = table.Column<bool>(type: "bit", nullable: false),
                    relationship = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    is_custodian = table.Column<bool>(type: "bit", nullable: true),
                    contact_type = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_associationship_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                });

            migrationBuilder.CreateTable(
                name: "parent_info",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    parent_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')"),
                    parent_photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    salutation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    firstname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    middlename = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    lastname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    work_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    mobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    personal_email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    work_email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    user_profile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_portal_user = table.Column<bool>(type: "bit", nullable: false),
                    login_email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    suffix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    bus_No = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    bus_pickup = table.Column<bool>(type: "bit", nullable: true),
                    bus_dropoff = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_info_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id });
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    max_api_checks = table.Column<int>(type: "int", nullable: true),
                    features = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans_tenant_id", x => new { x.tenant_id, x.school_id, x.plan_id });
                });

            migrationBuilder.CreateTable(
                name: "programs",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    program_id = table.Column<int>(type: "int", nullable: false),
                    program_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programs_tenant_id", x => new { x.tenant_id, x.school_id, x.program_id });
                });

            migrationBuilder.CreateTable(
                name: "release_number",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    release_number = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    release_date = table.Column<DateTime>(type: "date", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release_number_tenant_id", x => new { x.tenant_id, x.school_id, x.release_number });
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    isactive = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms_tenant_id", x => new { x.tenant_id, x.school_id, x.room_id });
                });

            migrationBuilder.CreateTable(
                name: "scheduled_job",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<long>(type: "bigint", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    job_title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    job_schedule_date = table.Column<DateTime>(type: "date", nullable: true),
                    api_title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    controller_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    task_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_run_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_run_status = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_job", x => new { x.tenant_id, x.school_id, x.job_id });
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    section_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sections_tenant_id", x => new { x.tenant_id, x.school_id, x.section_id });
                });

            migrationBuilder.CreateTable(
                name: "staff_schedule_view",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    course_short_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    course_section_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    staff_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    staff_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    scheduled = table.Column<bool>(type: "bit", nullable: false),
                    conflict_comment = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_schedule_view_tenant_id", x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id });
                });

            migrationBuilder.CreateTable(
                name: "student_schedule_view",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    course_section_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    student_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    student_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    scheduled = table.Column<bool>(type: "bit", nullable: false),
                    conflict_comment = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_schedule_view_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id });
                });

            migrationBuilder.CreateTable(
                name: "subject",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    subject_id = table.Column<int>(type: "int", nullable: false),
                    subject_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject_tenant_id", x => new { x.tenant_id, x.school_id, x.subject_id });
                });

            migrationBuilder.CreateTable(
                name: "user_access_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    login_attempt_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    profile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    login_failure_count = table.Column<int>(type: "int", nullable: true),
                    login_status = table.Column<bool>(type: "bit", nullable: true),
                    ipaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_access_log_id", x => new { x.id, x.tenant_id, x.school_id, x.login_attempt_date });
                });

            migrationBuilder.CreateTable(
                name: "api_controller_key_mapping",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    key_id = table.Column<int>(type: "int", nullable: false),
                    controller_id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_controller_key_mapping_tenant_id", x => new { x.TenantId, x.school_id, x.key_id, x.controller_id });
                    table.ForeignKey(
                        name: "api_controller_key_mapping$FK_api_controller_list",
                        columns: x => new { x.TenantId, x.controller_id },
                        principalTable: "api_controller_list",
                        principalColumns: new[] { "TenantId", "controller_id" });
                    table.ForeignKey(
                        name: "api_controller_key_mapping$FK_api_keys_master",
                        columns: x => new { x.TenantId, x.school_id, x.key_id },
                        principalTable: "api_keys_master",
                        principalColumns: new[] { "TenantId", "school_id", "key_id" });
                });

            migrationBuilder.CreateTable(
                name: "state",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    countryid = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2(6)", precision: 6, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state", x => x.id);
                    table.ForeignKey(
                        name: "state$FK_state_country",
                        column: x => x.countryid,
                        principalTable: "country",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "effort_grade_library_category_item",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: false),
                    effort_item_id = table.Column<int>(type: "int", nullable: false),
                    effort_item_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_effort_grade_library_category_item_tenant_id", x => new { x.tenant_id, x.school_id, x.effort_category_id, x.effort_item_id });
                    table.ForeignKey(
                        name: "library_category_item$FK_category",
                        columns: x => new { x.tenant_id, x.school_id, x.effort_category_id },
                        principalTable: "effort_grade_library_category",
                        principalColumns: new[] { "tenant_id", "school_id", "effort_category_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    standard_ref_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    grade_standard_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.standard_ref_no, x.grade_standard_id });
                    table.ForeignKey(
                        name: "course_standard$FK_course",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id },
                        principalTable: "course",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id" });
                    table.ForeignKey(
                        name: "course_standard$FK_us_standard",
                        columns: x => new { x.tenant_id, x.school_id, x.standard_ref_no, x.grade_standard_id },
                        principalTable: "grade_us_standard",
                        principalColumns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });
                });

            migrationBuilder.CreateTable(
                name: "historical_credit_transfer",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    hist_grade_id = table.Column<int>(type: "int", nullable: false),
                    hist_marking_period_id = table.Column<int>(type: "int", nullable: false),
                    credit_transfer_id = table.Column<int>(type: "int", nullable: false),
                    course_code = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    course_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    letter_grade = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: true),
                    gp_value = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    calculate_gpa = table.Column<bool>(type: "bit", nullable: true),
                    weighted_gp = table.Column<bool>(type: "bit", nullable: true),
                    grade_scale = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    credit_attempted = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    credit_earned = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_credit_transfer_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id, x.hist_marking_period_id, x.credit_transfer_id });
                    table.ForeignKey(
                        name: "hist_credit_trf$FK_hist_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.hist_grade_id },
                        principalTable: "historical_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" });
                });

            migrationBuilder.CreateTable(
                name: "parent_address",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_address_same = table.Column<bool>(type: "bit", nullable: false),
                    address_line_one = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    address_line_two = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    zip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_address_tenant_id", x => new { x.tenant_id, x.school_id, x.parent_id, x.student_id });
                    table.ForeignKey(
                        name: "parent_address$FK_parent_address_parent_info",
                        columns: x => new { x.tenant_id, x.school_id, x.parent_id },
                        principalTable: "parent_info",
                        principalColumns: new[] { "tenant_id", "school_id", "parent_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    school_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')"),
                    school_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    school_alt_id = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    school_state_id = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    school_district_id = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    school_level = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    school_classification = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    school_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    alternate_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    street_address_1 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    street_address_2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    city = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    county = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    division = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    district = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    zip = table.Column<string>(type: "nchar(20)", fixedLength: true, maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    current_period_ends = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    max_api_checks = table.Column<int>(type: "int", nullable: true),
                    features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    plan_id = table.Column<int>(type: "int", nullable: true),
                    longitude = table.Column<double>(type: "float", nullable: true),
                    latitude = table.Column<double>(type: "float", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_master_tenant_id", x => new { x.tenant_id, x.school_id });
                    table.ForeignKey(
                        name: "school_master$FK_school_master_plans",
                        columns: x => new { x.tenant_id, x.school_id, x.plan_id },
                        principalTable: "plans",
                        principalColumns: new[] { "tenant_id", "school_id", "plan_id" });
                });

            migrationBuilder.CreateTable(
                name: "scheduled_job_history",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<long>(type: "bigint", nullable: false),
                    job_run_id = table.Column<int>(type: "int", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "date", nullable: true),
                    run_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    job_status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_job_history", x => new { x.tenant_id, x.school_id, x.job_id, x.job_run_id });
                    table.ForeignKey(
                        name: "FK_job_history_job",
                        columns: x => new { x.tenant_id, x.school_id, x.job_id },
                        principalTable: "scheduled_job",
                        principalColumns: new[] { "tenant_id", "school_id", "job_id" });
                });

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    stateid = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "attendance_code_categories",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code_categories_tenant_id", x => new { x.tenant_id, x.school_id, x.attendance_category_id });
                    table.ForeignKey(
                        name: "attendance_code_categories$FK_school",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "block",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    block_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    block_sort_order = table.Column<long>(type: "bigint", nullable: true),
                    full_day_minutes = table.Column<int>(type: "int", nullable: true),
                    half_day_minutes = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_tenant_id", x => new { x.tenant_id, x.school_id, x.block_id });
                    table.ForeignKey(
                        name: "block$FK_block_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "block_period",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    block_id = table.Column<int>(type: "int", nullable: false),
                    period_id = table.Column<int>(type: "int", nullable: false),
                    period_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    period_short_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    period_start_time = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: true),
                    period_end_time = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: true),
                    period_sort_order = table.Column<int>(type: "int", nullable: true),
                    calculate_attendance = table.Column<bool>(type: "bit", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_block_period_tenant_id", x => new { x.tenant_id, x.school_id, x.block_id, x.period_id });
                    table.ForeignKey(
                        name: "block_period$FK_block_period_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "dpdown_valuelist",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    lov_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    lov_column_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lov_code = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dpdown_valuelist", x => x.id);
                    table.ForeignKey(
                        name: "dpdown_valuelist$FK_dpdown_valuelist_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "fields_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    is_system_category = table.Column<bool>(type: "bit", nullable: true),
                    search = table.Column<bool>(type: "bit", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    module = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    required = table.Column<bool>(type: "bit", nullable: true),
                    hide = table.Column<bool>(type: "bit", nullable: true),
                    is_system_wide_category = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fields_category_tenant_id", x => new { x.tenant_id, x.school_id, x.category_id });
                    table.ForeignKey(
                        name: "fields_category$FK_custom_category_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "grade_scale",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    grade_scale_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade_scale_comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    calculate_gpa = table.Column<bool>(type: "bit", nullable: true),
                    use_as_standard_grade_scale = table.Column<bool>(type: "bit", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_scale_tenant_id", x => new { x.tenant_id, x.school_id, x.grade_scale_id });
                    table.ForeignKey(
                        name: "grade_scale$FK_grade_scale_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "gradelevels",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    short_name = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    equivalency_id = table.Column<int>(type: "int", nullable: true),
                    age_range_id = table.Column<int>(type: "int", nullable: true),
                    isced_code = table.Column<int>(type: "int", nullable: true),
                    next_grade_id = table.Column<int>(type: "int", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: false),
                    profile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_system = table.Column<bool>(type: "bit", nullable: true),
                    is_superadmin = table.Column<bool>(type: "bit", nullable: false),
                    profile_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_tenant_id", x => new { x.tenant_id, x.school_id, x.membership_id });
                    table.ForeignKey(
                        name: "membership$fk_table_membership_table_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "permission_group",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    is_system = table.Column<bool>(type: "bit", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    icon_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    badgeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    badgeValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_group_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_group_id });
                    table.ForeignKey(
                        name: "permission_group$FK_permission_group_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_calendars",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    calender_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    visible_to_membership_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    default_calender = table.Column<bool>(type: "bit", nullable: true),
                    days = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    session_calendar = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_calendars_tenant_id", x => new { x.tenant_id, x.school_id, x.calender_id });
                    table.ForeignKey(
                        name: "school_calendars$FK_school_calendars_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    affiliation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    associations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    locale = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    lowest_grade_level = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    highest_grade_level = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    date_school_opened = table.Column<DateTime>(type: "date", nullable: true),
                    date_school_closed = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    gender = table.Column<string>(type: "nchar(15)", fixedLength: true, maxLength: 15, nullable: true),
                    internet = table.Column<bool>(type: "bit", nullable: true),
                    electricity = table.Column<bool>(type: "bit", nullable: true),
                    telephone = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: true),
                    fax = table.Column<string>(type: "nchar(20)", fixedLength: true, maxLength: 20, nullable: true),
                    website = table.Column<string>(type: "nchar(150)", fixedLength: true, maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    twitter = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    facebook = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    instagram = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    youtube = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    linkedin = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    name_of_principal = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    name_of_assistant_principal = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    school_logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    running_water = table.Column<bool>(type: "bit", nullable: true),
                    main_source_of_drinking_water = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    currently_available = table.Column<bool>(type: "bit", nullable: true),
                    female_toilet_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_female_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_female_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    female_toilet_accessibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    male_toilet_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_male_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_male_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    male_toilet_accessibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comon_toilet_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_common_toilets = table.Column<short>(type: "smallint", nullable: true),
                    total_common_toilets_usable = table.Column<short>(type: "smallint", nullable: true),
                    common_toilet_accessibility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    handwashing_available = table.Column<bool>(type: "bit", nullable: true),
                    soap_and_water_available = table.Column<bool>(type: "bit", nullable: true),
                    hygene_education = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_detail", x => x.id);
                    table.ForeignKey(
                        name: "school_detail$FK_school_detail_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_preference",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    school_preference_id = table.Column<long>(type: "bigint", nullable: false),
                    school_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    school_alt_id = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    full_day_minutes = table.Column<int>(type: "int", nullable: true),
                    half_day_minutes = table.Column<int>(type: "int", nullable: true),
                    max_login_failure = table.Column<int>(type: "int", nullable: true),
                    max_inactivity_days = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_preference_tenant_id", x => new { x.tenant_id, x.school_id, x.school_preference_id });
                    table.ForeignKey(
                        name: "school_preference$FK_school",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_rollover",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    rollover_id = table.Column<int>(type: "int", nullable: false),
                    reenrollment_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_begin_date = table.Column<DateTime>(type: "date", nullable: true),
                    school_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    rollover_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rollover_status = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_rollover_tenant_id", x => new { x.tenant_id, x.school_id, x.rollover_id });
                    table.ForeignKey(
                        name: "school_rollover$FK_school_rollover_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "school_years",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "bit", nullable: true),
                    does_exam = table.Column<bool>(type: "bit", nullable: true),
                    does_comments = table.Column<bool>(type: "bit", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_years_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "school_years$FK_school_years_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "staff_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')"),
                    staff_photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    salutation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    suffix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    first_given_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_family_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    staff_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    alternate_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    district_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    state_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    preferred_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    previous_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    social_security_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    other_govt_issued_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    race = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ethnicity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dob = table.Column<DateTime>(type: "date", nullable: true),
                    marital_status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    country_of_birth = table.Column<int>(type: "int", nullable: true),
                    nationality = table.Column<int>(type: "int", nullable: true),
                    first_language = table.Column<int>(type: "int", nullable: true),
                    second_language = table.Column<int>(type: "int", nullable: true),
                    third_language = table.Column<int>(type: "int", nullable: true),
                    physical_disability = table.Column<bool>(type: "bit", nullable: true),
                    portal_access = table.Column<bool>(type: "bit", nullable: true),
                    login_email_address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    profile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    job_title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    joining_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    homeroom_teacher = table.Column<bool>(type: "bit", nullable: true),
                    primary_grade_level_taught = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    primary_subject_taught = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    other_grade_level_taught = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    other_subject_taught = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    mobile_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    office_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    personal_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    school_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    twitter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    youtube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    home_address_line_one = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    home_address_line_two = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    home_address_city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_zip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    mailing_address_same_to_home = table.Column<bool>(type: "bit", nullable: true),
                    mailing_address_line_one = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    mailing_address_line_two = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    mailing_address_city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_zip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    emergency_first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    emergency_last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    relationship_to_staff = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    emergency_home_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    emergency_work_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    emergency_mobile_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    emergency_email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    disability_description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    bus_no = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    bus_pickup = table.Column<bool>(type: "bit", nullable: true),
                    bus_dropoff = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "student_enrollment_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    enrollment_code = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    short_name = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment_code_tenant_id", x => new { x.tenant_id, x.school_id, x.enrollment_code });
                    table.ForeignKey(
                        name: "student_enrollment_code$FK_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')"),
                    student_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    alternate_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    district_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    state_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    admission_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    roll_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    salutation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    first_given_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_family_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    suffix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    preferred_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    previous_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    social_security_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    other_govt_issued_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    student_photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    dob = table.Column<DateTime>(type: "date", nullable: true),
                    student_portal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    race = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ethnicity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    marital_status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    country_of_birth = table.Column<int>(type: "int", nullable: true),
                    nationality = table.Column<int>(type: "int", nullable: true),
                    first_language_id = table.Column<int>(type: "int", nullable: true),
                    second_language_id = table.Column<int>(type: "int", nullable: true),
                    third_language_id = table.Column<int>(type: "int", nullable: true),
                    section_id = table.Column<int>(type: "int", nullable: true),
                    estimated_grad_date = table.Column<DateTime>(type: "date", nullable: true),
                    eligibility_504 = table.Column<bool>(type: "bit", nullable: true),
                    economic_disadvantage = table.Column<bool>(type: "bit", nullable: true),
                    free_lunch_eligibility = table.Column<bool>(type: "bit", nullable: true),
                    special_education_indicator = table.Column<bool>(type: "bit", nullable: true),
                    lep_indicator = table.Column<bool>(type: "bit", nullable: true),
                    home_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    mobile_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    personal_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    school_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    twitter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    instagram = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    youtube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    linkedin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    home_address_line_one = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    home_address_line_two = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    home_address_city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    home_address_zip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    bus_no = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    school_bus_pick_up = table.Column<bool>(type: "bit", nullable: true),
                    school_bus_drop_off = table.Column<bool>(type: "bit", nullable: true),
                    mailing_address_same_to_home = table.Column<bool>(type: "bit", nullable: true),
                    mailing_address_line_one = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    mailing_address_line_two = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    mailing_address_city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_state = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    mailing_address_zip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    critical_alert = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    alert_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    primary_care_physician = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    primary_care_physician_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    medical_facility = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    medical_facility_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    insurance_company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    insurance_company_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    policy_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    policy_holder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dentist = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dentist_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vision = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vision_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    associationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    enrollment_type = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "attendance_code",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    attendance_category_id = table.Column<int>(type: "int", nullable: false),
                    attendance_code = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    state_code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    default_code = table.Column<bool>(type: "bit", nullable: true),
                    allow_entry_by = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_code_tenant_id", x => new { x.tenant_id, x.school_id, x.attendance_category_id, x.attendance_code });
                    table.ForeignKey(
                        name: "attendance_code$FK_categories",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id },
                        principalTable: "attendance_code_categories",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_block_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_block_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_block",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id },
                        principalTable: "block",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id" });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_block_schedule$FK_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_calendar_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "date", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_calendar_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_calendar_schedule$FK_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_calendar_schedule$FK_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_fixed_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_fixed_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_fixed_schedule$FK_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_fixed_schedule$FK_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_variable_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    day = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    block_id = table.Column<int>(type: "int", nullable: true),
                    period_id = table.Column<int>(type: "int", nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    take_attendance = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_variable_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.serial });
                    table.ForeignKey(
                        name: "course_variable_schedule$FK_block_periods",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                    table.ForeignKey(
                        name: "course_variable_schedule$FK_rooms",
                        columns: x => new { x.tenant_id, x.school_id, x.room_id },
                        principalTable: "rooms",
                        principalColumns: new[] { "tenant_id", "school_id", "room_id" });
                });

            migrationBuilder.CreateTable(
                name: "custom_fields",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    field_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "(N'')"),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    search = table.Column<bool>(type: "bit", nullable: true),
                    field_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    select_options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    system_field = table.Column<bool>(type: "bit", nullable: true),
                    required = table.Column<bool>(type: "bit", nullable: true),
                    default_selection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    hide = table.Column<bool>(type: "bit", nullable: true),
                    is_system_wide_field = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    breakoff = table.Column<int>(type: "int", nullable: true),
                    weighted_gp_value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    unweighted_gp_value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.grade_scale_id, x.grade_id });
                    table.ForeignKey(
                        name: "grade$FK_grade_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });
                });

            migrationBuilder.CreateTable(
                name: "user_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    passwordhash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    lang_id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    membership_id = table.Column<int>(type: "int", nullable: false),
                    is_tenantadmin = table.Column<bool>(type: "bit", nullable: true),
                    login_attempt_date = table.Column<DateTime>(type: "date", nullable: true),
                    login_failure_count = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    last_used_school_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "permission_category",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    enable_view = table.Column<bool>(type: "bit", nullable: true),
                    enable_add = table.Column<bool>(type: "bit", nullable: true),
                    enable_edit = table.Column<bool>(type: "bit", nullable: true),
                    enable_delete = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_category_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_category_id });
                    table.ForeignKey(
                        name: "permission_category$FK_group",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_group_id },
                        principalTable: "permission_group",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });
                });

            migrationBuilder.CreateTable(
                name: "honor_rolls",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    honor_roll_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    honor_roll = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    breakoff = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_honor_rolls_tenant_id", x => new { x.tenant_id, x.school_id, x.honor_roll_id });
                    table.ForeignKey(
                        name: "honor_rolls$FK_honor_rolls_honor_rolls",
                        columns: x => new { x.tenant_id, x.school_id, x.marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    year_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "bit", nullable: true),
                    does_exam = table.Column<bool>(type: "bit", nullable: true),
                    does_comments = table.Column<bool>(type: "bit", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "assignment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    assignment_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: true),
                    assignment_title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    points = table.Column<int>(type: "int", nullable: true),
                    assignment_date = table.Column<DateTime>(type: "date", nullable: true),
                    due_date = table.Column<DateTime>(type: "date", nullable: true),
                    assignment_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "staff_certificate_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: true),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    certification_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    certification_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    primary_certification = table.Column<bool>(type: "bit", nullable: true),
                    certification_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    certification_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_certificate_info", x => x.id);
                    table.ForeignKey(
                        name: "staff_certificate_info$FK_staff",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                });

            migrationBuilder.CreateTable(
                name: "staff_school_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: true),
                    school_id = table.Column<int>(type: "int", nullable: true),
                    staff_id = table.Column<int>(type: "int", nullable: true),
                    school_attached_id = table.Column<int>(type: "int", nullable: true),
                    school_attached_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    profile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_school_info", x => x.id);
                    table.ForeignKey(
                        name: "staff_school_info$FK_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_attendance_history",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
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
                    modification_timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_history_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.attendance_history_id });
                    table.ForeignKey(
                        name: "student_attendance_history$FK_student",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    comment_id = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.comment_id });
                    table.ForeignKey(
                        name: "student_comments$FK_std_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_daily_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    attendance_date = table.Column<DateTime>(type: "date", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    section_id = table.Column<int>(type: "int", nullable: true),
                    attendance_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    attendance_comment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    attendance_minutes = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_daily_attendance_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.attendance_date });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_grade_scale",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_scale_id },
                        principalTable: "grade_scale",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                    table.ForeignKey(
                        name: "student_daily_attendance$FK_sections",
                        columns: x => new { x.tenant_id, x.school_id, x.section_id },
                        principalTable: "sections",
                        principalColumns: new[] { "tenant_id", "school_id", "section_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_documents",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<int>(type: "int", nullable: false),
                    filename = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    filetype = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    file_uploaded = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    uploaded_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    uploaded_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_documents_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.document_id });
                    table.ForeignKey(
                        name: "student_documents$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_enrollment",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    enrollment_id = table.Column<int>(type: "int", nullable: false),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    student_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false, defaultValueSql: "(N'00000000-0000-0000-0000-000000000000')"),
                    calender_id = table.Column<int>(type: "int", nullable: true),
                    rolling_option = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    school_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_level_title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    enrollment_date = table.Column<DateTime>(type: "date", nullable: true),
                    enrollment_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    exit_date = table.Column<DateTime>(type: "date", nullable: true),
                    exit_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    transferred_school_id = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    school_transferred = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    transferred_grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_enrollment_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.enrollment_id });
                    table.ForeignKey(
                        name: "FK_student_enrollment_student_master_tenant_id_school_id_student_id",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "student_enrollment$FK_gradelevels",
                        columns: x => new { x.tenant_id, x.school_id, x.grade_id },
                        principalTable: "gradelevels",
                        principalColumns: new[] { "tenant_id", "school_id", "grade_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_medical_alert",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    alert_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    alert_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_alert_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_alert$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_medical_immunization",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    immunization_type = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    immunization_date = table.Column<DateTime>(type: "date", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_immunization_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_immunization$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_medical_note",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    note_date = table.Column<DateTime>(type: "date", nullable: true),
                    medical_note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_note_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_note$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_medical_nurse_visit",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    nurse_visit_date = table.Column<DateTime>(type: "date", nullable: true),
                    time_in = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    time_out = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    result = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_nurse_visit_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_nurse_visit$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_medical_provider",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    primary_care_physician = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    primary_care_physician_phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    preferred_medical_facility = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    preferred_medical_facility_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    insurance_company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    insurance_company_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    policy_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    policy_holder_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dentist_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    dentist_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vision_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    vision_provider_phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_medical_provider_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "student_medical_provider$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_report_card_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    school_year = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    marking_period_title = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: false, defaultValueSql: "(N'')"),
                    grade_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    student_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    yod_attendance = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    yod_absence = table.Column<int>(type: "int", nullable: true),
                    report_generation_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    absences = table.Column<int>(type: "int", nullable: true),
                    excused_absences = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_report_card_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.school_year, x.marking_period_title });
                    table.ForeignKey(
                        name: "student_report_card_master$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_transcript_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    grade_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    school_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    school_year = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    credit_attempted = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_grade_credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    student_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    cumulative_gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_credit_attempted = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    total_credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    generated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_transcript_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.grade_title });
                    table.ForeignKey(
                        name: "student_transcript_master$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "custom_fields_value",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    field_id = table.Column<int>(type: "int", nullable: false),
                    target_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    custom_field_title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    custom_field_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    custom_field_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields_value_tenant_id", x => new { x.tenant_id, x.school_id, x.category_id, x.field_id, x.target_id, x.module });
                    table.ForeignKey(
                        name: "custom_fields_value$FK_fields",
                        columns: x => new { x.tenant_id, x.school_id, x.category_id, x.field_id },
                        principalTable: "custom_fields",
                        principalColumns: new[] { "tenant_id", "school_id", "category_id", "field_id" });
                });

            migrationBuilder.CreateTable(
                name: "search_filter",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    module = table.Column<string>(type: "nchar(15)", fixedLength: true, maxLength: 15, nullable: false),
                    filter_id = table.Column<int>(type: "int", nullable: false),
                    filter_name = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    json_list = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<string>(type: "nchar(150)", fixedLength: true, maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nchar(150)", fixedLength: true, maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "user_secret_questions",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    emailaddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    movie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    hero = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    book = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cartoon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_secret_questions_tenant_id", x => new { x.tenant_id, x.school_id, x.emailaddress });
                    table.ForeignKey(
                        name: "user_secret_questions$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.emailaddress },
                        principalTable: "user_master",
                        principalColumns: new[] { "tenant_id", "school_id", "emailaddress" });
                });

            migrationBuilder.CreateTable(
                name: "permission_subcategory",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    permission_subcategory_id = table.Column<int>(type: "int", nullable: false),
                    permission_category_id = table.Column<int>(type: "int", nullable: false),
                    permission_group_id = table.Column<int>(type: "int", nullable: false),
                    permission_subcategory_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    enable_view = table.Column<bool>(type: "bit", nullable: true),
                    enable_add = table.Column<bool>(type: "bit", nullable: true),
                    enable_edit = table.Column<bool>(type: "bit", nullable: true),
                    enable_delete = table.Column<bool>(type: "bit", nullable: true),
                    is_system = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_subcategory_tenant_id", x => new { x.tenant_id, x.school_id, x.permission_subcategory_id });
                    table.ForeignKey(
                        name: "permission_subcategory$FK_category",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_category_id },
                        principalTable: "permission_category",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });
                });

            migrationBuilder.CreateTable(
                name: "quarters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    semester_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "bit", nullable: true),
                    does_exam = table.Column<bool>(type: "bit", nullable: true),
                    does_comments = table.Column<bool>(type: "bit", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "gradebook_grades",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    assignment_type_id = table.Column<int>(type: "int", nullable: false),
                    assignment_id = table.Column<int>(type: "int", nullable: false),
                    allowed_marks = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true),
                    percentage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    letter_grade = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    running_avg = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true),
                    running_avg_grade = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_grades_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.academic_year, x.marking_period_id, x.course_section_id, x.assignment_type_id, x.assignment_id });
                    table.ForeignKey(
                        name: "gradebook_grades$FK_assignment",
                        columns: x => new { x.tenant_id, x.school_id, x.assignment_id },
                        principalTable: "assignment",
                        principalColumns: new[] { "tenant_id", "school_id", "assignment_id" });
                    table.ForeignKey(
                        name: "gradebook_grades$FK_studmast",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_report_card_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    school_year = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    marking_period_title = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    grade_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    course_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    teacher = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    grade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    gpa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    comments = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    teacher_comments = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    overall_teacher_comments = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_report_card_detail", x => x.id);
                    table.ForeignKey(
                        name: "student_report_card_detail$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.school_year, x.marking_period_title },
                        principalTable: "student_report_card_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });
                });

            migrationBuilder.CreateTable(
                name: "student_transcript_detail",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    grade_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValueSql: "(N'')"),
                    course_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    course_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    credit_hours = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    credit_earned = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    gp_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_transcript_detail_id", x => new { x.id, x.tenant_id, x.school_id, x.student_id });
                    table.ForeignKey(
                        name: "student_transcript_detail$FK_transcript",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.grade_title },
                        principalTable: "student_transcript_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "grade_title" });
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    role_permission_id = table.Column<int>(type: "int", nullable: false),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    permission_group_id = table.Column<int>(type: "int", nullable: true),
                    permission_category_id = table.Column<int>(type: "int", nullable: true),
                    permission_subcategory_id = table.Column<int>(type: "int", nullable: true),
                    can_view = table.Column<bool>(type: "bit", nullable: true),
                    can_add = table.Column<bool>(type: "bit", nullable: true),
                    can_edit = table.Column<bool>(type: "bit", nullable: true),
                    can_delete = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission_tenant_id", x => new { x.tenant_id, x.school_id, x.role_permission_id });
                    table.ForeignKey(
                        name: "role_permission$FK_category",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_category_id },
                        principalTable: "permission_category",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_groupId",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_group_id },
                        principalTable: "permission_group",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                    table.ForeignKey(
                        name: "role_permission$FK_subcategory",
                        columns: x => new { x.tenant_id, x.school_id, x.permission_subcategory_id },
                        principalTable: "permission_subcategory",
                        principalColumns: new[] { "tenant_id", "school_id", "permission_subcategory_id" });
                });

            migrationBuilder.CreateTable(
                name: "course_section",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_type = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    course_section_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    attendance_category_id = table.Column<int>(type: "int", nullable: true),
                    credit_hours = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    seats = table.Column<int>(type: "int", nullable: true),
                    allow_student_conflict = table.Column<bool>(type: "bit", nullable: true),
                    allow_teacher_conflict = table.Column<bool>(type: "bit", nullable: true),
                    is_weighted_course = table.Column<bool>(type: "bit", nullable: true),
                    affects_class_rank = table.Column<bool>(type: "bit", nullable: true),
                    affects_honor_roll = table.Column<bool>(type: "bit", nullable: true),
                    online_class_room = table.Column<bool>(type: "bit", nullable: true),
                    online_classroom_url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    online_classroom_password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    use_standards = table.Column<bool>(type: "bit", nullable: true),
                    standard_grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    duration_based_on_period = table.Column<bool>(type: "bit", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    duration_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    duration_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    schedule_type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    meeting_days = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    attendance_taken = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_section_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "course_section$FK_categories",
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
                });

            migrationBuilder.CreateTable(
                name: "progress_periods",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    marking_period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    quarter_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(type: "bit", nullable: true),
                    does_exam = table.Column<bool>(type: "bit", nullable: true),
                    does_comments = table.Column<bool>(type: "bit", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_progress_periods_tenant_id", x => new { x.tenant_id, x.school_id, x.marking_period_id, x.academic_year, x.quarter_id });
                    table.ForeignKey(
                        name: "progress_periods$FK_progress_periods_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.quarter_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_effort_grade_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
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
                    teacher_comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_effort_grade_master_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno });
                    table.ForeignKey(
                        name: "student_effort_grade_calendars",
                        columns: x => new { x.tenant_id, x.school_id, x.calendar_id },
                        principalTable: "school_calendars",
                        principalColumns: new[] { "tenant_id", "school_id", "calender_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_effort_grade_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_final_grade",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_final_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    based_on_standard_grade = table.Column<bool>(type: "bit", nullable: true),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    is_percent = table.Column<bool>(type: "bit", nullable: true),
                    percent_marks = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    grade_obtained = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: true),
                    teacher_comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    creditattempted = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    creditearned = table.Column<decimal>(type: "decimal(8,3)", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno });
                    table.ForeignKey(
                        name: "student_final_grade$FK_master",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "student_final_grade$FK_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    general = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    score_rounding = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    assignment_sorting = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    max_anomalous_grade = table.Column<int>(type: "int", nullable: true),
                    upgraded_assignment_grade_days = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_tenant_id", x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id });
                    table.ForeignKey(
                        name: "grd_conf_course_sec",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                });

            migrationBuilder.CreateTable(
                name: "staff_coursesection_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    staff_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    course_section_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    duration_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    duration_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    meeting_days = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_dropped = table.Column<bool>(type: "bit", nullable: true),
                    effective_drop_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    is_assigned = table.Column<bool>(type: "bit", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_coursesection_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_section",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_staff_master",
                        columns: x => new { x.tenant_id, x.staff_id },
                        principalTable: "staff_master",
                        principalColumns: new[] { "tenant_id", "staff_id" });
                    table.ForeignKey(
                        name: "staff_coursesection_schedule$FK_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_coursesection_schedule",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    student_guid = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    alternate_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    student_internal_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    first_given_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    last_family_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    first_language_id = table.Column<int>(type: "int", nullable: true),
                    grade_id = table.Column<int>(type: "int", nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    course_section_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    calendar_id = table.Column<int>(type: "int", nullable: true),
                    is_dropped = table.Column<bool>(type: "bit", nullable: true),
                    effective_drop_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    effective_start_date = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_coursesection_schedule_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id });
                    table.ForeignKey(
                        name: "sch_mast_std_cs_sch",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" });
                    table.ForeignKey(
                        name: "std_cs_cs_sch",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id },
                        principalTable: "course_section",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "std_mast_cs_sch",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id },
                        principalTable: "student_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_effort_grade_detail",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_effort_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    id = table.Column<long>(type: "bigint", nullable: false),
                    effort_category_id = table.Column<int>(type: "int", nullable: true),
                    effort_item_id = table.Column<int>(type: "int", nullable: true),
                    effort_grade_scale_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_effort_grade_detail_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno, x.id });
                    table.ForeignKey(
                        name: "std_effort_master_detail",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_effort_grade_srlno },
                        principalTable: "student_effort_grade_master",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" });
                });

            migrationBuilder.CreateTable(
                name: "student_final_grade_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_final_grade_srlno = table.Column<long>(type: "bigint", nullable: false),
                    course_comment_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno, x.course_comment_id });
                    table.ForeignKey(
                        name: "std_final_grade_cmnts$FK_final_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno },
                        principalTable: "student_final_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });
                    table.ForeignKey(
                        name: "student_final_grade_comments$FK_comment_category",
                        columns: x => new { x.tenant_id, x.school_id, x.course_comment_id },
                        principalTable: "course_comment_category",
                        principalColumns: new[] { "tenant_id", "school_id", "course_comment_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_final_grade_standard",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
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
                    teacher_comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_final_grade_standard_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.id });
                    table.ForeignKey(
                        name: "std_final_grade_std$FK_final_grade",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_final_grade_srlno },
                        principalTable: "student_final_grade",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_gradescale",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    grade_scale_id = table.Column<int>(type: "int", nullable: false),
                    grade_id = table.Column<int>(type: "int", nullable: false),
                    breakoff_points = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_gradescale", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_conf_gradescale",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_quarter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_quarter", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_configuration_quarter$FK_configuration",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_quarter$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_semester",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    qtr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_semester", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_config_semester$FK_semester",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_semester$FK_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.qtr_marking_period_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "gradebook_configuration_semester$FK_semester",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                });

            migrationBuilder.CreateTable(
                name: "gradebook_configuration_year",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    course_section_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    gradebook_configuration_id = table.Column<int>(type: "int", nullable: false),
                    yr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    smstr_marking_period_id = table.Column<int>(type: "int", nullable: true),
                    grading_percentage = table.Column<int>(type: "int", nullable: true),
                    exam_percentage = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradebook_configuration_year", x => x.id);
                    table.ForeignKey(
                        name: "gradebook_config_year$FK_config",
                        columns: x => new { x.tenant_id, x.school_id, x.course_id, x.course_section_id, x.academic_year, x.gradebook_configuration_id },
                        principalTable: "gradebook_configuration",
                        principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });
                    table.ForeignKey(
                        name: "gradebook_config_year$FK_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.yr_marking_period_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                    table.ForeignKey(
                        name: "gradebook_config_year$FK_sem",
                        columns: x => new { x.tenant_id, x.school_id, x.smstr_marking_period_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_attendance",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
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
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.staff_id, x.course_id, x.course_section_id, x.attendance_date, x.block_id, x.period_id });
                    table.UniqueConstraint("AK_student_attendance_tenant_id_school_id_student_id_student_attendance_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id });
                    table.ForeignKey(
                        name: "student_attd_coursesec_sch",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.course_id, x.course_section_id },
                        principalTable: "student_coursesection_schedule",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "student_attendance$FK_code",
                        columns: x => new { x.tenant_id, x.school_id, x.attendance_category_id, x.attendance_code },
                        principalTable: "attendance_code",
                        principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });
                    table.ForeignKey(
                        name: "student_attendance$FK_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                    table.ForeignKey(
                        name: "student_attendance$FK_staff_cs_sch",
                        columns: x => new { x.tenant_id, x.school_id, x.staff_id, x.course_id, x.course_section_id },
                        principalTable: "staff_coursesection_schedule",
                        principalColumns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });
                    table.ForeignKey(
                        name: "student_attendance$FKperiod",
                        columns: x => new { x.tenant_id, x.school_id, x.block_id, x.period_id },
                        principalTable: "block_period",
                        principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });
                });

            migrationBuilder.CreateTable(
                name: "student_attendance_comments",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", fixedLength: true, maxLength: 36, nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    student_attendance_id = table.Column<long>(type: "bigint", nullable: false),
                    comment_id = table.Column<long>(type: "bigint", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    comment_timestamp = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    membership_id = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    created_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_attendance_comments_tenant_id", x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id, x.comment_id });
                    table.ForeignKey(
                        name: "std_attd_comments$FK_std_atd",
                        columns: x => new { x.tenant_id, x.school_id, x.student_id, x.student_attendance_id },
                        principalTable: "student_attendance",
                        principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" });
                    table.ForeignKey(
                        name: "student_attendance_comments$FK_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
                });

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
                    { 42, "TD", null, null, "Chad", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 84, "GI", null, null, "Gibraltar", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 126, "LT", null, null, "Lithuania", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 168, "PS", null, null, "Palestinian Territory Occupied", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 210, "SZ", null, null, "Swaziland", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "created_by", "created_on", "name", "sort_order", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 5, "9–10", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "grade_age_range",
                columns: new[] { "age_range_id", "age_range", "created_by", "created_on", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 5, null, null, "Short-cycle tertiary education", null, null },
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
                    { 2, null, null, "sq", "sq", "Albanian", null, null }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "created_by", "created_on", "language_code", "lcid", "locale", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 44, null, null, "en", "en-bz", "English - Belize", null, null }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "created_by", "created_on", "language_code", "lcid", "locale", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 86, null, null, "kk", "kk", "Kazakh", null, null }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "created_by", "created_on", "language_code", "lcid", "locale", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                    { 128, null, null, "es", "es-cr", "Spanish - Costa Rica", null, null }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "created_by", "created_on", "language_code", "lcid", "locale", "updated_by", "updated_on" },
                values: new object[,]
                {
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
                name: "IX_api_controller_key_mapping_tenant_id_controller",
                table: "api_controller_key_mapping",
                columns: new[] { "TenantId", "controller_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type",
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
                name: "IX_course_block_schedule_tenant_id_school_id_block",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_room",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_bl",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_ro",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_block",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_room_",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_attendance_c",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_calendar_id",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "calendar_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_markin",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_p",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_standard_tenant_id_school_id_standard_re",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_bl",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_ro",
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
                name: "IX_gradebook_conf_qtr_tenant_id_school",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_schoo",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_grade_conf_sem_t_id_school",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_conf_sem_school",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_scho",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_grade_conf_yr_t_id_school_i",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_grade_conf_yr_tenant_id_school_i",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_i",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.CreateIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment",
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
                name: "historical_marking_period$IX_historical_marking_pe",
                table: "historical_marking_period",
                columns: new[] { "tenant_id", "school_id", "hist_marking_period_id", "academic_year", "title" },
                unique: true,
                filter: "[academic_year] IS NOT NULL AND [title] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_notice",
                table: "notice",
                columns: new[] { "tenant_id", "school_id", "notice_id", "created_on", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_permission_category_tenant_id_school_id_permiss",
                table: "permission_category",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.CreateIndex(
                name: "IX_permission_subcategory_tenant_id_school_id_perm",
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
                name: "IX_role_permission_tenant_id_school_id_membership_",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.CreateIndex(
                name: "role_permission_tenant_id_school_id_permission",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.CreateIndex(
                name: "role_permission_tenant_id_school_id_permission_id",
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
                name: "IX_staff_coursesection_schedule_tenant_id_school_i",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_staff_id",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.CreateIndex(
                name: "staff_coursesection_schedule_tenant_id_1",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "staff_coursesection_schedule_tenant_id_2",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "staff_coursesection_schedule_tenant_id_3",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

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
                name: "IX_student_attendance_tenant_id_school_id_attendan",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_block_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_membersh",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_staff_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_tenant_id_school_id_student_",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });

            migrationBuilder.CreateIndex(
                name: "student_attendance$AK_student_attendance_tenant_id",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "student_attendance$student_attendance_id_idx",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id",
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
                name: "IX_student_daily_attendance_tenant_id_school_id_gr",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_se",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.CreateIndex(
                name: "IX_std_effort_gd_master_tid_school_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_std_effort_grade_master_tid_school_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_std_effort_grade_mster_tid_school_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "calendar_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_grade_id",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "student_guid" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_qtr_mar",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_smstr_m",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_tenant_id_school_id_yr_mark",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_comments_tenant_id_school_i",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "course_comment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_student_final_grade_standard_tenant_id_school_i",
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
                name: "student_master$AK_student_master_tenant_id_school_",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "student_master$IX_student_master",
                table: "student_master",
                columns: new[] { "tenant_id", "school_id", "student_guid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_report_card_detail_tenant_id_school_id_",
                table: "student_report_card_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });

            migrationBuilder.CreateIndex(
                name: "IX_student_transcript_detail_tenant_id_school_id_s",
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
                name: "scheduled_job_history");

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
                name: "scheduled_job");

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
                name: "student_coursesection_schedule");

            migrationBuilder.DropTable(
                name: "attendance_code");

            migrationBuilder.DropTable(
                name: "staff_coursesection_schedule");

            migrationBuilder.DropTable(
                name: "block_period");

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
                name: "student_master");

            migrationBuilder.DropTable(
                name: "course_section");

            migrationBuilder.DropTable(
                name: "staff_master");

            migrationBuilder.DropTable(
                name: "sections");

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
