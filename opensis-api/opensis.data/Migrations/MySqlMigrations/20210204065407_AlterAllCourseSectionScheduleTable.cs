using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterAllCourseSectionScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_block_period_block",
                table: "block_period");

            migrationBuilder.DropForeignKey(
                name: "FK_course_block_schedule_school_periods",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
               name: "FK_course_block_schedule_block",
               table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_calendar_schedule_school_periods",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_fixed_schedule_school_periods",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_variable_schedule_school_periods",
                table: "course_variable_schedule");

            migrationBuilder.DropTable(
                name: "school_periods");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "course_variable_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_period_id",
                table: "course_variable_schedule");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "course_fixed_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_period_id",
                table: "course_fixed_schedule");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "course_calendar_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_period_id",
                table: "course_calendar_schedule");

            migrationBuilder.DropIndex(
                name: "PRIMARY",
                table: "course_block_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id",
                table: "course_block_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_period_id",
                table: "course_block_schedule");

            migrationBuilder.DropColumn(
                name: "marking_period_id",
                table: "course_section");

            migrationBuilder.AddColumn<int>(
                name: "block_id",
                table: "course_variable_schedule",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "qtr_marking_period_id",
                table: "course_section",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "smstr_marking_period_id",
                table: "course_section",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "yr_marking_period_id",
                table: "course_section",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "block_id",
                table: "course_fixed_schedule",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "block_id",
                table: "course_calendar_schedule",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_variable_schedule_1",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "serial" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_fixed_schedule_1",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "serial" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_calendar_schedule_1",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "serial" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_block_schedule_1",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "serial" });

            migrationBuilder.CreateTable(
                name: "school_periods_obsolete",
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_block_id_period~",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

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
                name: "IX_course_fixed_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_block_id_period~",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_block_period_school_master",
                table: "block_period",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_block_schedule_block_periods",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

                migrationBuilder.AddForeignKey(
                name: "FK_course_block_schedule_block",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id" },
                principalTable: "block",
                principalColumns: new[] { "tenant_id", "school_id", "block_id" },
                onDelete: ReferentialAction.Restrict);


            migrationBuilder.AddForeignKey(
                name: "FK_course_calendar_schedule_block_periods",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_fixed_schedule_block_periods",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_quarters",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_semesters",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_section_school_years",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_variable_schedule_block_periods",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_block_period_school_master",
                table: "block_period");

            migrationBuilder.DropForeignKey(
                name: "FK_course_block_schedule_block_periods",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_calendar_schedule_block_periods",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_fixed_schedule_block_periods",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_course_section_quarters",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "FK_course_section_semesters",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "FK_course_section_school_years",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "FK_course_variable_schedule_block_periods",
                table: "course_variable_schedule");

            migrationBuilder.DropTable(
                name: "school_periods_obsolete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course_variable_schedule_1",
                table: "course_variable_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_block_id_period~",
                table: "course_variable_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course_fixed_schedule_1",
                table: "course_fixed_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_fixed_schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course_calendar_schedule_1",
                table: "course_calendar_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_block_id_period~",
                table: "course_calendar_schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course_block_schedule_1",
                table: "course_block_schedule");

            migrationBuilder.DropIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_block_schedule");

            migrationBuilder.DropColumn(
                name: "block_id",
                table: "course_variable_schedule");

            migrationBuilder.DropColumn(
                name: "qtr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropColumn(
                name: "smstr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropColumn(
                name: "yr_marking_period_id",
                table: "course_section");

            migrationBuilder.DropColumn(
                name: "block_id",
                table: "course_fixed_schedule");

            migrationBuilder.DropColumn(
                name: "block_id",
                table: "course_calendar_schedule");

            migrationBuilder.AddColumn<int>(
                name: "marking_period_id",
                table: "course_section",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_variable_schedule_1",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_fixed_schedule_1",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_calendar_schedule_1",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_block_schedule_1",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.CreateTable(
                name: "school_periods",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    period_id = table.Column<int>(type: "int", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4, 0)", nullable: true),
                    attendance = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    block = table.Column<string>(type: "varchar(10) CHARACTER SET utf8mb4", unicode: false, maxLength: 10, nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    ignore_scheduling = table.Column<string>(type: "varchar(10) CHARACTER SET utf8mb4", unicode: false, maxLength: 10, nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    length = table.Column<decimal>(type: "decimal(10, 0)", nullable: true),
                    rollover_id = table.Column<int>(type: "int", nullable: true),
                    short_name = table.Column<string>(type: "varchar(10) CHARACTER SET utf8mb4", unicode: false, maxLength: 10, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: true),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    title = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", unicode: false, maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100) CHARACTER SET utf8mb4", unicode: false, maxLength: 100, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_period_id",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_period_id",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_period_id",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_period_id",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_block_period_block",
                table: "block_period",
                columns: new[] { "tenant_id", "school_id", "block_id" },
                principalTable: "block",
                principalColumns: new[] { "tenant_id", "school_id", "block_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_block_schedule_school_periods",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" },
                principalTable: "school_periods",
                principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_calendar_schedule_school_periods",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" },
                principalTable: "school_periods",
                principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_fixed_schedule_school_periods",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" },
                principalTable: "school_periods",
                principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_course_variable_schedule_school_periods",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "period_id" },
                principalTable: "school_periods",
                principalColumns: new[] { "tenant_id", "school_id", "period_id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
