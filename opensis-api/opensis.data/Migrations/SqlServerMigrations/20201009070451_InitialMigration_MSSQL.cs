using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class InitialMigration_MSSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(maxLength: 50, nullable: true),
                    countrycode = table.Column<string>(unicode: false, maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gradelevels",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    grade_id = table.Column<int>(nullable: false),
                    short_name = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    next_grade_id = table.Column<int>(nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gradelevels", x => new { x.tenant_id, x.school_id, x.grade_id });
                });

            migrationBuilder.CreateTable(
                name: "language",
                columns: table => new
                {
                    lang_id = table.Column<int>(nullable: false),
                    lcid = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    locale = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    language_code = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_language", x => x.lang_id);
                });

            migrationBuilder.CreateTable(
                name: "notice",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    notice_id = table.Column<int>(nullable: false),
                    target_membership_ids = table.Column<string>(unicode: false, maxLength: 50, nullable: false, comment: "Signifies group of user for whom notice is visible. to be saved as comma separated values. if user's membership_id falls in any of the value, he can see the notice."),
                    title = table.Column<string>(unicode: false, nullable: false),
                    body = table.Column<string>(unicode: false, nullable: false),
                    valid_from = table.Column<DateTime>(type: "date", nullable: false),
                    valid_to = table.Column<DateTime>(type: "date", nullable: false),
                    isactive = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    created_time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_notice", x => new { x.tenant_id, x.school_id, x.notice_id });
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    plan_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    max_api_checks = table.Column<int>(nullable: true),
                    features = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_plans", x => new { x.tenant_id, x.school_id, x.plan_id });
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    room_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    capacity = table.Column<int>(nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    isactive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_rooms", x => new { x.tenant_id, x.school_id, x.room_id });
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    section_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_sections", x => new { x.tenant_id, x.school_id, x.section_id });
                });

            migrationBuilder.CreateTable(
                name: "state",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(maxLength: 50, nullable: true),
                    countryid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state", x => x.id);
                    table.ForeignKey(
                        name: "FK_state_country",
                        column: x => x.countryid,
                        principalTable: "country",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    school_internal_id = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    school_alt_id = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    school_state_id = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    school_district_id = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    school_level = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    school_classification = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    school_name = table.Column<string>(maxLength: 100, nullable: true),
                    alternate_name = table.Column<string>(maxLength: 100, nullable: true),
                    street_address_1 = table.Column<string>(maxLength: 150, nullable: true),
                    street_address_2 = table.Column<string>(maxLength: 150, nullable: true),
                    city = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    county = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    division = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    state = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    district = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    zip = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    country = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    current_period_ends = table.Column<DateTime>(type: "datetime", nullable: true),
                    max_api_checks = table.Column<int>(nullable: true),
                    features = table.Column<string>(unicode: false, nullable: true),
                    plan_id = table.Column<int>(nullable: true),
                    created_by = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    date_created = table.Column<DateTime>(type: "datetime", nullable: true),
                    modified_by = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    date_modifed = table.Column<DateTime>(type: "datetime", nullable: true),
                    longitude = table.Column<double>(nullable: true),
                    latitude = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_school_master", x => new { x.tenant_id, x.school_id });
                    table.ForeignKey(
                        name: "FK_school_master_plans",
                        columns: x => new { x.tenant_id, x.school_id, x.plan_id },
                        principalTable: "plans",
                        principalColumns: new[] { "tenant_id", "school_id", "plan_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(maxLength: 50, nullable: true),
                    stateid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_city", x => x.id);
                    table.ForeignKey(
                        name: "FK_city_state",
                        column: x => x.id,
                        principalTable: "state",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    membership_id = table.Column<int>(nullable: false, comment: "can be considered as profileid of Opensis1"),
                    profile = table.Column<string>(unicode: false, maxLength: 30, nullable: false, comment: "E.g. admin,student,teacher"),
                    title = table.Column<string>(unicode: false, maxLength: 100, nullable: false, comment: "e.g. Administrator,Student,Teacher,Dept. head"),
                    access = table.Column<string>(unicode: false, nullable: true),
                    weekly_update = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_membership_1", x => new { x.tenant_id, x.school_id, x.membership_id });
                    table.ForeignKey(
                        name: "fk_table_membership_table_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_calendars",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    calender_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    default_calender = table.Column<bool>(nullable: true),
                    days = table.Column<string>(unicode: false, maxLength: 7, nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_school_calendars", x => new { x.tenant_id, x.school_id, x.calender_id });
                    table.ForeignKey(
                        name: "FK_school_calendars_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_detail",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: true),
                    affiliation = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    associations = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    locale = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    lowest_grade_level = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    highest_grade_level = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    date_school_opened = table.Column<DateTime>(type: "date", nullable: true),
                    date_school_closed = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<bool>(nullable: true),
                    gender = table.Column<string>(fixedLength: true, maxLength: 6, nullable: true),
                    internet = table.Column<bool>(nullable: true),
                    electricity = table.Column<bool>(nullable: true),
                    telephone = table.Column<string>(fixedLength: true, maxLength: 20, nullable: true),
                    fax = table.Column<string>(fixedLength: true, maxLength: 20, nullable: true),
                    website = table.Column<string>(fixedLength: true, maxLength: 150, nullable: true),
                    email = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    twitter = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    facebook = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    instagram = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    youtube = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    linkedin = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    name_of_principal = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    name_of_assistant_principal = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    school_logo = table.Column<byte[]>(nullable: true),
                    running_water = table.Column<bool>(nullable: true),
                    main_source_of_drinking_water = table.Column<string>(fixedLength: true, maxLength: 100, nullable: true),
                    currently_available = table.Column<bool>(nullable: true),
                    female_toilet_type = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    total_female_toilets = table.Column<short>(nullable: true),
                    total_female_toilets_usable = table.Column<short>(nullable: true),
                    female_toilet_accessibility = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    male_toilet_type = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    total_male_toilets = table.Column<short>(nullable: true),
                    total_male_toilets_usable = table.Column<short>(nullable: true),
                    male_toilet_accessibility = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    comon_toilet_type = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    total_common_toilets = table.Column<short>(nullable: true),
                    total_common_toilets_usable = table.Column<short>(nullable: true),
                    common_toilet_accessibility = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true),
                    handwashing_available = table.Column<bool>(nullable: true),
                    soap_and_water_available = table.Column<bool>(nullable: true),
                    hygene_education = table.Column<string>(fixedLength: true, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_school_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_school_detail_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_periods",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    period_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    length = table.Column<decimal>(type: "decimal(10, 0)", nullable: true),
                    block = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    ignore_scheduling = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    attendance = table.Column<bool>(nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    start_time = table.Column<TimeSpan>(nullable: true),
                    end_time = table.Column<TimeSpan>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_school_periods", x => new { x.tenant_id, x.school_id, x.period_id });
                    table.ForeignKey(
                        name: "FK_school_periods_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "school_years",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10, 0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(nullable: true),
                    does_exam = table.Column<bool>(nullable: true),
                    does_comments = table.Column<bool>(nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_school_years", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "FK_school_years_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_master",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false),
                    emailaddress = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    passwordhash = table.Column<string>(unicode: false, maxLength: 256, nullable: false),
                    lang_id = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    membership_id = table.Column<int>(nullable: false),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_master", x => new { x.tenant_id, x.school_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_user_master_language",
                        column: x => x.lang_id,
                        principalTable: "language",
                        principalColumn: "lang_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_master_membership",
                        columns: x => new { x.tenant_id, x.school_id, x.membership_id },
                        principalTable: "membership",
                        principalColumns: new[] { "tenant_id", "school_id", "membership_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    year_id = table.Column<int>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10, 0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(nullable: true),
                    does_exam = table.Column<bool>(nullable: true),
                    does_comments = table.Column<bool>(nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_semesters", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "FK_semesters_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_semesters_school_years",
                        columns: x => new { x.tenant_id, x.school_id, x.year_id },
                        principalTable: "school_years",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quarters",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    semester_id = table.Column<int>(nullable: true),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    sort_order = table.Column<decimal>(type: "decimal(10, 0)", nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(nullable: true),
                    does_exam = table.Column<bool>(nullable: true),
                    does_comments = table.Column<bool>(nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_quarters", x => new { x.tenant_id, x.school_id, x.marking_period_id });
                    table.ForeignKey(
                        name: "FK_quarters_school_master",
                        columns: x => new { x.tenant_id, x.school_id },
                        principalTable: "school_master",
                        principalColumns: new[] { "tenant_id", "school_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quarters_semesters",
                        columns: x => new { x.tenant_id, x.school_id, x.semester_id },
                        principalTable: "semesters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "progress_periods",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(nullable: false),
                    school_id = table.Column<int>(nullable: false),
                    marking_period_id = table.Column<int>(nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: false),
                    quarter_id = table.Column<int>(nullable: false),
                    title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    short_name = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    post_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    does_grades = table.Column<bool>(nullable: true),
                    does_exam = table.Column<bool>(nullable: true),
                    does_comments = table.Column<bool>(nullable: true),
                    rollover_id = table.Column<int>(nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_progress_periods", x => new { x.tenant_id, x.school_id, x.marking_period_id, x.academic_year, x.quarter_id });
                    table.ForeignKey(
                        name: "FK_progress_periods_quarters",
                        columns: x => new { x.tenant_id, x.school_id, x.quarter_id },
                        principalTable: "quarters",
                        principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "country",
                columns: new[] { "id", "countrycode", "name" },
                values: new object[,]
                {
                    { 1, "AF", "Afghanistan" },
                    { 157, "NZ", "New Zealand" },
                    { 158, "NI", "Nicaragua" },
                    { 159, "NE", "Niger" },
                    { 160, "NG", "Nigeria" },
                    { 161, "NU", "Niue" },
                    { 162, "NF", "Norfolk Island" },
                    { 163, "MP", "Northern Mariana Islands" },
                    { 164, "NO", "Norway" },
                    { 165, "OM", "Oman" },
                    { 166, "PK", "Pakistan" },
                    { 167, "PW", "Palau" },
                    { 168, "PS", "Palestinian Territory Occupied" },
                    { 169, "PA", "Panama" },
                    { 170, "PG", "Papua new Guinea" },
                    { 171, "PY", "Paraguay" },
                    { 172, "PE", "Peru" },
                    { 173, "PH", "Philippines" },
                    { 174, "PN", "Pitcairn Island" },
                    { 175, "PL", "Poland" },
                    { 176, "PT", "Portugal" },
                    { 177, "PR", "Puerto Rico" },
                    { 178, "QA", "Qatar" },
                    { 179, "RE", "Reunion" },
                    { 180, "RO", "Romania" },
                    { 181, "RU", "Russia" },
                    { 182, "RW", "Rwanda" },
                    { 183, "SH", "Saint Helena" },
                    { 156, "NC", "New Caledonia" },
                    { 184, "KN", "Saint Kitts And Nevis" },
                    { 155, "NL", "Netherlands The" },
                    { 153, "NP", "Nepal" },
                    { 126, "LT", "Lithuania" },
                    { 127, "LU", "Luxembourg" },
                    { 128, "MO", "Macau S.A.R." },
                    { 129, "MK", "Macedonia" },
                    { 130, "MG", "Madagascar" },
                    { 131, "MW", "Malawi" },
                    { 132, "MY", "Malaysia" },
                    { 133, "MV", "Maldives" },
                    { 134, "ML", "Mali" },
                    { 135, "MT", "Malta" },
                    { 136, "XM", "Man (Isle of)" },
                    { 137, "MH", "Marshall Islands" },
                    { 138, "MQ", "Martinique" },
                    { 139, "MR", "Mauritania" },
                    { 140, "MU", "Mauritius" },
                    { 141, "YT", "Mayotte" },
                    { 142, "MX", "Mexico" },
                    { 143, "FM", "Micronesia" },
                    { 144, "MD", "Moldova" },
                    { 145, "MC", "Monaco" },
                    { 146, "MN", "Mongolia" },
                    { 147, "MS", "Montserrat" },
                    { 148, "MA", "Morocco" },
                    { 149, "MZ", "Mozambique" },
                    { 150, "MM", "Myanmar" },
                    { 151, "NA", "Namibia" },
                    { 152, "NR", "Nauru" },
                    { 154, "AN", "Netherlands Antilles" },
                    { 125, "LI", "Liechtenstein" },
                    { 185, "LC", "Saint Lucia" },
                    { 187, "VC", "Saint Vincent And The Grenadines" },
                    { 220, "TO", "Tonga" },
                    { 221, "TT", "Trinidad And Tobago" },
                    { 222, "TN", "Tunisia" },
                    { 223, "TR", "Turkey" },
                    { 224, "TM", "Turkmenistan" },
                    { 225, "TC", "Turks And Caicos Islands" },
                    { 226, "TV", "Tuvalu" },
                    { 227, "UG", "Uganda" },
                    { 228, "UA", "Ukraine" },
                    { 229, "AE", "United Arab Emirates" },
                    { 230, "GB", "United Kingdom" },
                    { 231, "US", "United States" },
                    { 232, "UM", "United States Minor Outlying Islands" },
                    { 233, "UY", "Uruguay" },
                    { 234, "UZ", "Uzbekistan" },
                    { 235, "VU", "Vanuatu" },
                    { 236, "VA", "Vatican City State (Holy See)" },
                    { 237, "VE", "Venezuela" },
                    { 238, "VN", "Vietnam" },
                    { 239, "VG", "Virgin Islands (British)" },
                    { 240, "VI", "Virgin Islands (US)" },
                    { 241, "WF", "Wallis And Futuna Islands" },
                    { 242, "EH", "Western Sahara" },
                    { 243, "YE", "Yemen" },
                    { 244, "YU", "Yugoslavia" },
                    { 245, "ZM", "Zambia" },
                    { 246, "ZW", "Zimbabwe" },
                    { 219, "TK", "Tokelau" },
                    { 186, "PM", "Saint Pierre and Miquelon" },
                    { 218, "TG", "Togo" },
                    { 216, "TZ", "Tanzania" },
                    { 188, "WS", "Samoa" },
                    { 189, "SM", "San Marino" },
                    { 190, "ST", "Sao Tome and Principe" },
                    { 191, "SA", "Saudi Arabia" },
                    { 192, "SN", "Senegal" },
                    { 193, "RS", "Serbia" },
                    { 194, "SC", "Seychelles" },
                    { 195, "SL", "Sierra Leone" },
                    { 196, "SG", "Singapore" },
                    { 197, "SK", "Slovakia" },
                    { 198, "SI", "Slovenia" },
                    { 199, "XG", "Smaller Territories of the UK" },
                    { 200, "SB", "Solomon Islands" },
                    { 201, "SO", "Somalia" },
                    { 202, "ZA", "South Africa" },
                    { 203, "GS", "South Georgia" },
                    { 204, "SS", "South Sudan" },
                    { 206, "LK", "Sri Lanka" },
                    { 207, "SD", "Sudan" },
                    { 208, "SR", "Suriname" },
                    { 209, "SJ", "Svalbard And Jan Mayen Islands" },
                    { 210, "SZ", "Swaziland" },
                    { 211, "SE", "Sweden" },
                    { 212, "CH", "Switzerland" },
                    { 213, "SY", "Syria" },
                    { 214, "TW", "Taiwan" },
                    { 215, "TJ", "Tajikistan" },
                    { 217, "TH", "Thailand" },
                    { 124, "LY", "Libya" },
                    { 205, "ES", "Spain" },
                    { 122, "LS", "Lesotho" },
                    { 33, "BG", "Bulgaria" },
                    { 34, "BF", "Burkina Faso" },
                    { 35, "BI", "Burundi" },
                    { 36, "KH", "Cambodia" },
                    { 37, "CM", "Cameroon" },
                    { 38, "CA", "Canada" },
                    { 39, "CV", "Cape Verde" },
                    { 40, "KY", "Cayman Islands" },
                    { 41, "CF", "Central African Republic" },
                    { 42, "TD", "Chad" },
                    { 43, "CL", "Chile" },
                    { 44, "CN", "China" },
                    { 45, "CX", "Christmas Island" },
                    { 46, "CC", "Cocos (Keeling) Islands" },
                    { 47, "CO", "Colombia" },
                    { 48, "KM", "Comoros" },
                    { 49, "CG", "Congo" },
                    { 50, "CD", "Congo The Democratic Republic Of The" },
                    { 51, "CK", "Cook Islands" },
                    { 52, "CR", "Costa Rica" },
                    { 53, "CI", "Cote D'Ivoire (Ivory Coast)" },
                    { 54, "HR", "Croatia (Hrvatska)" },
                    { 55, "CU", "Cuba" },
                    { 56, "CY", "Cyprus" },
                    { 57, "CZ", "Czech Republic" },
                    { 58, "DK", "Denmark" },
                    { 59, "DJ", "Djibouti" },
                    { 32, "BN", "Brunei" },
                    { 60, "DM", "Dominica" },
                    { 31, "IO", "British Indian Ocean Territory" },
                    { 29, "BV", "Bouvet Island" },
                    { 123, "LR", "Liberia" },
                    { 2, "AL", "Albania" },
                    { 3, "DZ", "Algeria" },
                    { 4, "AS", "American Samoa" },
                    { 5, "AD", "Andorra" },
                    { 6, "AO", "Angola" },
                    { 7, "AI", "Anguilla" },
                    { 8, "AQ", "Antarctica" },
                    { 9, "AG", "Antigua And Barbuda" },
                    { 10, "AR", "Argentina" },
                    { 11, "AM", "Armenia" },
                    { 12, "AW", "Aruba" },
                    { 13, "AU", "Australia" },
                    { 14, "AT", "Austria" },
                    { 15, "AZ", "Azerbaijan" },
                    { 16, "BS", "Bahamas The" },
                    { 17, "BH", "Bahrain" },
                    { 18, "BD", "Bangladesh" },
                    { 19, "BB", "Barbados" },
                    { 20, "BY", "Belarus" },
                    { 21, "BE", "Belgium" },
                    { 22, "BZ", "Belize" },
                    { 24, "BM", "Bermuda" },
                    { 25, "BT", "Bhutan" },
                    { 26, "BO", "Bolivia" },
                    { 27, "BA", "Bosnia and Herzegovina" },
                    { 28, "BW", "Botswana" },
                    { 30, "BR", "Brazil" },
                    { 61, "DO", "Dominican Republic" },
                    { 23, "BJ", "Benin" },
                    { 63, "EC", "Ecuador" },
                    { 95, "HT", "Haiti" },
                    { 96, "HM", "Heard and McDonald Islands" },
                    { 97, "HN", "Honduras" },
                    { 98, "HK", "Hong Kong S.A.R." },
                    { 99, "HU", "Hungary" },
                    { 100, "IS", "Iceland" },
                    { 101, "IN", "India" },
                    { 102, "ID", "Indonesia" },
                    { 103, "IR", "Iran" },
                    { 104, "IQ", "Iraq" },
                    { 105, "IE", "Ireland" },
                    { 106, "IL", "Israel" },
                    { 107, "IT", "Italy" },
                    { 108, "JM", "Jamaica" },
                    { 109, "JP", "Japan" },
                    { 111, "JO", "Jordan" },
                    { 112, "KZ", "Kazakhstan" },
                    { 113, "KE", "Kenya" },
                    { 114, "KI", "Kiribati" },
                    { 115, "KP", "Korea North" },
                    { 116, "KR", "Korea South" },
                    { 117, "KW", "Kuwait" },
                    { 118, "KG", "Kyrgyzstan" },
                    { 119, "LA", "Laos" },
                    { 120, "LV", "Latvia" },
                    { 62, "TP", "East Timor" },
                    { 121, "LB", "Lebanon" },
                    { 94, "GY", "Guyana" },
                    { 93, "GW", "Guinea-Bissau" },
                    { 110, "XJ", "Jersey" },
                    { 91, "XU", "Guernsey and Alderney" },
                    { 64, "EG", "Egypt" },
                    { 65, "SV", "El Salvador" },
                    { 92, "GN", "Guinea" },
                    { 66, "GQ", "Equatorial Guinea" },
                    { 67, "ER", "Eritrea" },
                    { 68, "EE", "Estonia" },
                    { 69, "ET", "Ethiopia" },
                    { 70, "XA", "External Territories of Australia" },
                    { 72, "FO", "Faroe Islands" },
                    { 73, "FJ", "Fiji Islands" },
                    { 74, "FI", "Finland" },
                    { 75, "FR", "France" },
                    { 76, "GF", "French Guiana" },
                    { 77, "PF", "French Polynesia" },
                    { 71, "FK", "Falkland Islands" },
                    { 79, "GA", "Gabon" },
                    { 80, "GM", "Gambia The" },
                    { 90, "GT", "Guatemala" },
                    { 88, "GP", "Guadeloupe" },
                    { 81, "GE", "Georgia" },
                    { 82, "DE", "Germany" },
                    { 83, "GH", "Ghana" },
                    { 89, "GU", "Guam" },
                    { 84, "GI", "Gibraltar" },
                    { 78, "TF", "French Southern Territories" },
                    { 85, "GR", "Greece" },
                    { 86, "GL", "Greenland" },
                    { 87, "GD", "Grenada" }
                });

            migrationBuilder.InsertData(
                table: "language",
                columns: new[] { "lang_id", "language_code", "lcid", "locale" },
                values: new object[,]
                {
                    { 109, "rm", "rm", "Raeto-Romance" },
                    { 107, "pt", "pt-pt", "Portuguese - Portugal" },
                    { 110, "ro", "ro-mo", "Romanian - Moldova" },
                    { 106, "pt", "pt-br", "Portuguese - Brazil" },
                    { 105, "pl", "pl", "Polish" },
                    { 108, "pa", "pa", "Punjabi" },
                    { 111, "ro", "ro", "Romanian - Romania" },
                    { 115, "sr", "sr-sp", "Serbian - Cyrillic" },
                    { 113, "ru", "ru-mo", "Russian - Moldova" },
                    { 114, "sa", "sa", "Sanskrit" },
                    { 116, "sr", "sr-sp", "Serbian - Latin" },
                    { 117, "tn", "tn", "Setsuana" },
                    { 120, "sk", "sk", "Slovak" },
                    { 104, "or", "or", "Oriya" },
                    { 119, "Sinhalese", "si", "Sinhala" },
                    { 118, "sd", "sd", "Sindhi" },
                    { 112, "ru", "ru", "Russian" },
                    { 103, "nn", "no-no", "Norwegian - Nynorsk" },
                    { 90, "la", "la", "Latin" },
                    { 101, "ne", "ne", "Nepali" },
                    { 85, "ks", "ks", "Kashmiri" },
                    { 84, "kn", "kn", "Kannada" },
                    { 121, "sl", "sl", "Slovenian" },
                    { 86, "kk", "kk", "Kazakh" },
                    { 87, "km", "km", "Khmer" },
                    { 88, "ko", "ko", "Korean" },
                    { 89, "lo", "lo", "Lao" },
                    { 91, "lv", "lv", "Latvian" },
                    { 92, "lt", "lt", "Lithuanian" },
                    { 93, "ms", "ms-bn", "Malay - Brunei" },
                    { 94, "ms", "ms-my", "Malay - Malaysia" },
                    { 95, "ml", "ml", "Malayalam" },
                    { 96, "mt", "mt", "Maltese" },
                    { 97, "mi", "mi", "Maori" },
                    { 98, "mr", "mr", "Marathi" },
                    { 99, "mn", "mn", "Mongolian" },
                    { 100, "mn", "mn", "Mongolian" },
                    { 102, "nb", "no-no", "Norwegian - Bokml" },
                    { 122, "so", "so", "Somali" },
                    { 151, "bo", "bo", "Tibetan" },
                    { 124, "es", "es-ar", "Spanish - Argentina" },
                    { 146, "tg", "tg", "Tajik" },
                    { 147, "ta", "ta", "Tamil" },
                    { 148, "tt", "tt", "Tatar" },
                    { 149, "te", "te", "Telugu" },
                    { 150, "th", "th", "Thai" },
                    { 152, "ts", "ts", "Tsonga" },
                    { 153, "tr", "tr", "Turkish" },
                    { 145, "sv", "sv-se", "Swedish - Sweden" },
                    { 154, "tk", "tk", "Turkmen" },
                    { 157, "ur", "ur", "Urdu" },
                    { 158, "uz", "uz-uz", "Uzbek - Cyrillic" },
                    { 159, "uz", "uz-uz", "Uzbek - Latin" },
                    { 160, "vi", "vi", "Vietnamese" },
                    { 161, "cy", "cy", "Welsh" },
                    { 83, "ja", "ja", "Japanese" },
                    { 162, "xh", "xh", "Xhosa" },
                    { 155, "uk", "uk", "Ukrainian" },
                    { 144, "sv", "sv-fi", "Swedish - Finland" },
                    { 143, "sw", "sw", "Swahili" },
                    { 142, "es", "es-ve", "Spanish - Venezuela" },
                    { 125, "es", "es-bo", "Spanish - Bolivia" },
                    { 126, "es", "es-cl", "Spanish - Chile" },
                    { 127, "es", "es-co", "Spanish - Colombia" },
                    { 128, "es", "es-cr", "Spanish - Costa Rica" },
                    { 129, "es", "es-do", "Spanish - Dominican Republic" },
                    { 130, "es", "es-ec", "Spanish - Ecuador" },
                    { 131, "es", "es-sv", "Spanish - El Salvador" },
                    { 132, "es", "es-gt", "Spanish - Guatemala" },
                    { 133, "es", "es-hn", "Spanish - Honduras" },
                    { 134, "es", "es-mx", "Spanish - Mexico" },
                    { 135, "es", "es-ni", "Spanish - Nicaragua" },
                    { 136, "es", "es-pa", "Spanish - Panama" },
                    { 137, "es", "es-py", "Spanish - Paraguay" },
                    { 138, "es", "es-pe", "Spanish - Peru" },
                    { 139, "es", "es-pr", "Spanish - Puerto Rico" },
                    { 140, "es", "es-es", "Spanish - Spain (Traditional)" },
                    { 141, "es", "es-uy", "Spanish - Uruguay" },
                    { 123, "sb", "sb", "Sorbian" },
                    { 82, "it", "it-ch", "Italian - Switzerland" },
                    { 21, "as", "as", "Assamese" },
                    { 80, "id", "id", "Indonesian" },
                    { 22, "az", "az-az", "Azeri - Cyrillic" },
                    { 23, "az", "az-az", "Azeri - Latin" },
                    { 24, "eu", "eu", "Basque" },
                    { 25, "be", "be", "Belarusian" },
                    { 26, "bn", "bn", "Bengali - Bangladesh" },
                    { 27, "bn", "bn", "Bengali - India" },
                    { 28, "bs", "bs", "Bosnian" },
                    { 20, "hy", "hy", "Armenian" },
                    { 29, "bg", "bg", "Bulgarian" },
                    { 31, "ca", "ca", "Catalan" },
                    { 32, "zh", "zh-cn", "Chinese - China" },
                    { 33, "zh", "zh-hk", "Chinese - Hong Kong SAR" },
                    { 34, "zh", "zh-mo", "Chinese - Macau SAR" },
                    { 35, "zh", "zh-sg", "Chinese - Singapore" },
                    { 36, "zh", "zh-tw", "Chinese - Taiwan" },
                    { 37, "hr", "hr", "Croatian" },
                    { 30, "my", "my", "Burmese" },
                    { 19, "ar", "ar-ye", "Arabic - Yemen" },
                    { 18, "ar", "ar-ae", "Arabic - United Arab Emirates" },
                    { 17, "ar", "ar-tn", "Arabic - Tunisia" },
                    { 163, "yi", "yi", "Yiddish" },
                    { 1, "af", "af", "Afrikaans" },
                    { 2, "sq", "sq", "Albanian" },
                    { 3, "am", "am", "Amharic" },
                    { 4, "ar", "ar-dz", "Arabic - Algeria" },
                    { 5, "ar", "ar-bh", "Arabic - Bahrain" },
                    { 6, "ar", "ar-eg", "Arabic - Egypt" },
                    { 7, "ar", "ar-iq", "Arabic - Iraq" },
                    { 8, "ar", "ar-jo", "Arabic - Jordan" },
                    { 9, "ar", "ar-kw", "Arabic - Kuwait" },
                    { 10, "ar", "ar-lb", "Arabic - Lebanon" },
                    { 11, "ar", "ar-ly", "Arabic - Libya" },
                    { 12, "ar", "ar-ma", "Arabic - Morocco" },
                    { 13, "ar", "ar-om", "Arabic - Oman" },
                    { 14, "ar", "ar-qa", "Arabic - Qatar" },
                    { 15, "ar", "ar-sa", "Arabic - Saudi Arabia" },
                    { 16, "ar", "ar-sy", "Arabic - Syria" },
                    { 38, "cs", "cs", "Czech" },
                    { 39, "da", "da", "Danish" },
                    { 40, "Dhivehi", "Maldivian", "Divehi" },
                    { 41, "nl", "nl-be", "Dutch - Belgium" },
                    { 63, "fr", "fr-fr", "French - France" },
                    { 64, "fr", "fr-lu", "French - Luxembourg" },
                    { 65, "fr", "fr-ch", "French - Switzerland" },
                    { 66, "gd", "gd-ie", "Gaelic - Ireland" },
                    { 67, "gd", "gd", "Gaelic - Scotland" },
                    { 68, "de", "de-at", "German - Austria" },
                    { 69, "de", "de-de", "German - Germany" },
                    { 70, "de", "de-li", "German - Liechtenstein" },
                    { 71, "de", "de-lu", "German - Luxembourg" },
                    { 72, "de", "de-ch", "German - Switzerland" },
                    { 73, "el", "el", "Greek" },
                    { 74, "gn", "gn", "Guarani - Paraguay" },
                    { 75, "gu", "gu", "Gujarati" },
                    { 76, "he", "he", "Hebrew" },
                    { 77, "hi", "hi", "Hindi" },
                    { 78, "hu", "hu", "Hungarian" },
                    { 79, "is", "is", "Icelandic" },
                    { 62, "fr", "fr-ca", "French - Canada" },
                    { 81, "it", "it-it", "Italian - Italy" },
                    { 61, "fr", "fr-be", "French - Belgium" },
                    { 59, "fa", "fa", "Farsi - Persian" },
                    { 42, "nl", "nl-nl", "Dutch - Netherlands" },
                    { 43, "en", "en-au", "English - Australia" },
                    { 44, "en", "en-bz", "English - Belize" },
                    { 45, "en", "en-ca", "English - Canada" },
                    { 46, "en", "en-cb", "English - Caribbean" },
                    { 47, "en", "en-gb", "English - Great Britain" },
                    { 48, "en", "en-in", "English - India" },
                    { 49, "en", "en-ie", "English - Ireland" },
                    { 50, "en", "en-jm", "English - Jamaica" },
                    { 51, "en", "en-nz", "English - New Zealand" },
                    { 52, "en", "en-ph", "English - Philippines" },
                    { 53, "en", "en-za", "English - Southern Africa" },
                    { 54, "en", "en-tt", "English - Trinidad" },
                    { 55, "en", "en-us", "English - United States" },
                    { 56, "et", "et", "Estonian" },
                    { 57, "mk", "mk", "FYRO Macedonia" },
                    { 58, "fo", "fo", "Faroese" },
                    { 60, "fi", "fi", "Finnish" },
                    { 164, "zu", "zu", "Zulu" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_progress_periods_tenant_id_school_id_quarter_id",
                table: "progress_periods",
                columns: new[] { "tenant_id", "school_id", "quarter_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quarters_tenant_id_school_id_semester_id",
                table: "quarters",
                columns: new[] { "tenant_id", "school_id", "semester_id" });

            migrationBuilder.CreateIndex(
                name: "IX_school_detail_tenant_id_school_id",
                table: "school_detail",
                columns: new[] { "tenant_id", "school_id" });

            migrationBuilder.CreateIndex(
                name: "IX_school_master_tenant_id_school_id_plan_id",
                table: "school_master",
                columns: new[] { "tenant_id", "school_id", "plan_id" });

            migrationBuilder.CreateIndex(
                name: "IX_semesters_tenant_id_school_id_year_id",
                table: "semesters",
                columns: new[] { "tenant_id", "school_id", "year_id" });

            migrationBuilder.CreateIndex(
                name: "IX_state_countryid",
                table: "state",
                column: "countryid");

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
                name: "city");

            migrationBuilder.DropTable(
                name: "gradelevels");

            migrationBuilder.DropTable(
                name: "notice");

            migrationBuilder.DropTable(
                name: "progress_periods");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "school_calendars");

            migrationBuilder.DropTable(
                name: "school_detail");

            migrationBuilder.DropTable(
                name: "school_periods");

            migrationBuilder.DropTable(
                name: "sections");

            migrationBuilder.DropTable(
                name: "user_master");

            migrationBuilder.DropTable(
                name: "state");

            migrationBuilder.DropTable(
                name: "quarters");

            migrationBuilder.DropTable(
                name: "language");

            migrationBuilder.DropTable(
                name: "membership");

            migrationBuilder.DropTable(
                name: "country");

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
