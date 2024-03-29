﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.SqlServerMigrations
{
    public partial class AddPgrsprdMarkingPeriodIdInAllCourseSectionView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView =
                  @"
                    DROP VIEW IF EXISTS [dbo].[all_course_section_view] ;  
                    GO

                    CREATE VIEW [dbo].[all_course_section_view]
                    AS
                    SELECT dbo.course.tenant_id, dbo.course.school_id, dbo.course.course_id, dbo.course.course_title, dbo.course.course_subject, dbo.course.course_program, dbo.course_section.academic_year, dbo.course_section.course_section_id, 
                                             dbo.course_section.course_section_name, dbo.course_section.seats, dbo.course_section.duration_start_date, dbo.course_section.duration_end_date, dbo.course_section.yr_marking_period_id, 
                                             dbo.course_section.qtr_marking_period_id, dbo.course_section.smstr_marking_period_id, dbo.course_section.schedule_type, dbo.course_section.meeting_days AS fixed_days, 
                                             dbo.course_fixed_schedule.room_id AS fixed_room_id, dbo.course_fixed_schedule.period_id AS fixed_period_id, dbo.course_variable_schedule.day AS var_day, dbo.course_variable_schedule.period_id AS var_period_id, 
                                             dbo.course_variable_schedule.room_id AS var_room_id, dbo.course_calendar_schedule.date AS cal_date, dbo.course_calendar_schedule.period_id AS cal_period_id, dbo.course_calendar_schedule.room_id AS cal_room_id, 
                                             dbo.course_block_schedule.period_id AS block_period_id, dbo.course_block_schedule.room_id AS block_room_id, dbo.course_section.is_active, dbo.course.course_grade_level, dbo.course_section.grade_scale_id, 
                                             dbo.course_section.allow_teacher_conflict, dbo.course_section.allow_student_conflict, dbo.course_section.calendar_id, dbo.course_section.attendance_taken, DATENAME(DW, dbo.course_calendar_schedule.date) AS cal_day, 
                                             dbo.course_variable_schedule.take_attendance AS take_attendance_variable, dbo.course_calendar_schedule.take_attendance AS take_attendance_calendar, 
                                             dbo.course_block_schedule.take_attendance AS take_attendeace_block, dbo.course_block_schedule.block_id, dbo.course_section.attendance_category_id, dbo.course_section.prgrsprd_marking_period_id
                    FROM  dbo.course INNER JOIN
                                             dbo.course_section ON dbo.course.tenant_id = dbo.course_section.tenant_id AND dbo.course.school_id = dbo.course_section.school_id AND dbo.course.course_id = dbo.course_section.course_id LEFT OUTER JOIN
                                             dbo.course_fixed_schedule ON dbo.course_section.tenant_id = dbo.course_fixed_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_fixed_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_fixed_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_fixed_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_variable_schedule ON dbo.course_section.tenant_id = dbo.course_variable_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_variable_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_variable_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_variable_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_calendar_schedule ON dbo.course_section.tenant_id = dbo.course_calendar_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_calendar_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_calendar_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_calendar_schedule.course_section_id LEFT OUTER JOIN
                                             dbo.course_block_schedule ON dbo.course_section.tenant_id = dbo.course_block_schedule.tenant_id AND dbo.course_section.school_id = dbo.course_block_schedule.school_id AND 
                                             dbo.course_section.course_id = dbo.course_block_schedule.course_id AND dbo.course_section.course_section_id = dbo.course_block_schedule.course_section_id
                    GO";

            migrationBuilder.Sql(allCourseSectionView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string allCourseSectionView = @"DROP VIEW dbo.all_course_section_view";
            migrationBuilder.Sql(allCourseSectionView);
        }
    }
}
