using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class CreateStudentFinalGradeListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string studentFinalGradeListView =
                   @"
					DROP VIEW IF EXISTS student_final_grade_list_view;

                    CREATE VIEW student_final_grade_list_view
					AS
					SELECT 
        `s`.`tenant_id` AS `sfg_tenant_id`,
        `s`.`school_id` AS `sfg_school_id`,
        `s`.`student_id` AS `sfg_student_id`,
        `s`.`student_final_grade_srlno` AS `sfg_student_final_grade_srlno`,
        `s`.`academic_year` AS `sfg_academic_year`,
        `s`.`based_on_standard_grade` AS `sfg_based_on_standard_grade`,
        `s`.`calendar_id` AS `sfg_calendar_id`,
        `s`.`course_id` AS `sfg_course_id`,
        `s`.`course_section_id` AS `sfg_course_section_id`,
        `s`.`creditattempted` AS `sfg_creditattempted`,
        `s`.`creditearned` AS `sfg_creditearned`,
        `s`.`grade_id` AS `sfg_grade_id`,
        `s`.`grade_obtained` AS `sfg_grade_obtained`,
        `s`.`grade_scale_id` AS `sfg_grade_scale_id`,
        `s`.`is_custom_marking_period` AS `sfg_is_custom_marking_period`,
        `s`.`is_exam_grade` AS `sfg_is_exam_grade`,
        `s`.`is_percent` AS `sfg_is_percent`,
        `s`.`percent_marks` AS `sfg_percent_marks`,
        `s`.`prgrsprd_marking_period_id` AS `sfg_prgrsprd_marking_period_id`,
        `s`.`qtr_marking_period_id` AS `sfg_qtr_marking_period_id`,
        `s`.`smstr_marking_period_id` AS `sfg_smstr_marking_period_id`,
        `s`.`yr_marking_period_id` AS `sfg_yr_marking_period_id`,
        `s`.`teacher_comment` AS `sfg_teacher_comment`,
        `s0`.`marking_period_id` AS `sy_marking_period_id`,
        `s1`.`marking_period_id` AS `sem_marking_period_id`,
        `s1`.`year_id` AS `sem_year_id`,
        `q`.`marking_period_id` AS `qtr_marking_period_id`,
        `q`.`semester_id` AS `qtr_semester_id`,
        `p`.`marking_period_id` AS `pp_marking_period_id`,
        `p`.`quarter_id` AS `pp_quarter_id`,
        `s2`.`student_id` AS `sfgs_student_id`,
        `s2`.`id` AS `sfgs_id`,
        `s2`.`academic_year` AS `sfgs_academic_year`,
        `s2`.`calendar_id` AS `sfgs_calendar_id`,
        `s2`.`grade_obtained` AS `sfgs_grade_obtained`,
        `s2`.`prgrsprd_marking_period_id` AS `sfgs_prgs_mark_period_id`,
        `s2`.`qtr_marking_period_id` AS `sfgs_qtr_mark_period_id`,
        `s2`.`smstr_marking_period_id` AS `sfgs_smstr_mark_period_id`,
        `s2`.`standard_grade_scale_id` AS `sfgs_standard_grade_scale_id`,
        `s2`.`student_final_grade_srlno` AS `sfgs_student_final_grade_srlno`,
        `s2`.`teacher_comment` AS `sfgs_teacher_comment`,
        `s2`.`yr_marking_period_id` AS `sfgs_yr_mark_period_id`,
        `s0`.`title` AS `school_year_title`,
        `s1`.`title` AS `semesters_title`,
        `q`.`title` AS `quarters_title`,
        `p`.`title` AS `progress_periods_title`
    FROM
        (((((`student_final_grade` `s`
        LEFT JOIN `school_years` `s0` ON (((`s`.`tenant_id` = `s0`.`tenant_id`)
            AND (`s`.`school_id` = `s0`.`school_id`)
            AND (`s`.`yr_marking_period_id` = `s0`.`marking_period_id`))))
        LEFT JOIN `semesters` `s1` ON (((`s`.`tenant_id` = `s1`.`tenant_id`)
            AND (`s`.`school_id` = `s1`.`school_id`)
            AND (`s`.`smstr_marking_period_id` = `s1`.`marking_period_id`))))
        LEFT JOIN `quarters` `q` ON (((`s`.`tenant_id` = `q`.`tenant_id`)
            AND (`s`.`school_id` = `q`.`school_id`)
            AND (`s`.`qtr_marking_period_id` = `q`.`marking_period_id`))))
        LEFT JOIN `progress_periods` `p` ON (((`s`.`tenant_id` = `p`.`tenant_id`)
            AND (`s`.`school_id` = `p`.`school_id`)
            AND (`s`.`prgrsprd_marking_period_id` = `p`.`marking_period_id`))))
        LEFT JOIN `student_final_grade_standard` `s2` ON (((`s`.`tenant_id` = `s2`.`tenant_id`)
            AND (`s`.`school_id` = `s2`.`school_id`)
            AND (`s`.`student_id` = `s2`.`student_id`)
            AND (`s`.`student_final_grade_srlno` = `s2`.`student_final_grade_srlno`))))";
            migrationBuilder.Sql(studentFinalGradeListView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string studentFinalGradeListView =
                  @"
					DROP VIEW IF EXISTS student_final_grade_list_view;

                    CREATE VIEW student_final_grade_list_view
					AS
					SELECT 
        `s`.`tenant_id` AS `sfg_tenant_id`,
        `s`.`school_id` AS `sfg_school_id`,
        `s`.`student_id` AS `sfg_student_id`,
        `s`.`student_final_grade_srlno` AS `sfg_student_final_grade_srlno`,
        `s`.`academic_year` AS `sfg_academic_year`,
        `s`.`based_on_standard_grade` AS `sfg_based_on_standard_grade`,
        `s`.`calendar_id` AS `sfg_calendar_id`,
        `s`.`course_id` AS `sfg_course_id`,
        `s`.`course_section_id` AS `sfg_course_section_id`,
        `s`.`creditattempted` AS `sfg_creditattempted`,
        `s`.`creditearned` AS `sfg_creditearned`,
        `s`.`grade_id` AS `sfg_grade_id`,
        `s`.`grade_obtained` AS `sfg_grade_obtained`,
        `s`.`grade_scale_id` AS `sfg_grade_scale_id`,
        `s`.`is_custom_marking_period` AS `sfg_is_custom_marking_period`,
        `s`.`is_exam_grade` AS `sfg_is_exam_grade`,
        `s`.`is_percent` AS `sfg_is_percent`,
        `s`.`percent_marks` AS `sfg_percent_marks`,
        `s`.`prgrsprd_marking_period_id` AS `sfg_prgrsprd_marking_period_id`,
        `s`.`qtr_marking_period_id` AS `sfg_qtr_marking_period_id`,
        `s`.`smstr_marking_period_id` AS `sfg_smstr_marking_period_id`,
        `s`.`yr_marking_period_id` AS `sfg_yr_marking_period_id`,
        `s`.`teacher_comment` AS `sfg_teacher_comment`,
        `s0`.`marking_period_id` AS `sy_marking_period_id`,
        `s1`.`marking_period_id` AS `sem_marking_period_id`,
        `s1`.`year_id` AS `sem_year_id`,
        `q`.`marking_period_id` AS `qtr_marking_period_id`,
        `q`.`semester_id` AS `qtr_semester_id`,
        `p`.`marking_period_id` AS `pp_marking_period_id`,
        `p`.`quarter_id` AS `pp_quarter_id`,
        `s2`.`student_id` AS `sfgs_student_id`,
        `s2`.`id` AS `sfgs_id`,
        `s2`.`academic_year` AS `sfgs_academic_year`,
        `s2`.`calendar_id` AS `sfgs_calendar_id`,
        `s2`.`grade_obtained` AS `sfgs_grade_obtained`,
        `s2`.`prgrsprd_marking_period_id` AS `sfgs_prgs_mark_period_id`,
        `s2`.`qtr_marking_period_id` AS `sfgs_qtr_mark_period_id`,
        `s2`.`smstr_marking_period_id` AS `sfgs_smstr_mark_period_id`,
        `s2`.`standard_grade_scale_id` AS `sfgs_standard_grade_scale_id`,
        `s2`.`student_final_grade_srlno` AS `sfgs_student_final_grade_srlno`,
        `s2`.`teacher_comment` AS `sfgs_teacher_comment`,
        `s2`.`yr_marking_period_id` AS `sfgs_yr_mark_period_id`,
        `s0`.`title` AS `school_year_title`,
        `s1`.`title` AS `semesters_title`,
        `q`.`title` AS `quarters_title`,
        `p`.`title` AS `progress_periods_title`
    FROM
        (((((`student_final_grade` `s`
        LEFT JOIN `school_years` `s0` ON (((`s`.`tenant_id` = `s0`.`tenant_id`)
            AND (`s`.`school_id` = `s0`.`school_id`)
            AND (`s`.`yr_marking_period_id` = `s0`.`marking_period_id`))))
        LEFT JOIN `semesters` `s1` ON (((`s`.`tenant_id` = `s1`.`tenant_id`)
            AND (`s`.`school_id` = `s1`.`school_id`)
            AND (`s`.`smstr_marking_period_id` = `s1`.`marking_period_id`))))
        LEFT JOIN `quarters` `q` ON (((`s`.`tenant_id` = `q`.`tenant_id`)
            AND (`s`.`school_id` = `q`.`school_id`)
            AND (`s`.`qtr_marking_period_id` = `q`.`marking_period_id`))))
        LEFT JOIN `progress_periods` `p` ON (((`s`.`tenant_id` = `p`.`tenant_id`)
            AND (`s`.`school_id` = `p`.`school_id`)
            AND (`s`.`prgrsprd_marking_period_id` = `p`.`marking_period_id`))))
        LEFT JOIN `student_final_grade_standard` `s2` ON (((`s`.`tenant_id` = `s2`.`tenant_id`)
            AND (`s`.`school_id` = `s2`.`school_id`)
            AND (`s`.`student_id` = `s2`.`student_id`)
            AND (`s`.`student_final_grade_srlno` = `s2`.`student_final_grade_srlno`))))";

            migrationBuilder.Sql(studentFinalGradeListView);
        }
    }
}
