using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddTableScheduledJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_con",
                table: "api_controller_key_mapping");

            migrationBuilder.DropForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_key",
                table: "api_controller_key_mapping");

            migrationBuilder.DropForeignKey(
                name: "attendance_code$FK_attendance_code_attendance_code_categories",
                table: "attendance_code");

            migrationBuilder.DropForeignKey(
                name: "attendance_code_categories$FK_attendance_code_categories_school_",
                table: "attendance_code_categories");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_block",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_block_periods",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_rooms",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_calendar_schedule$FK_course_calendar_schedule_block_perio",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_calendar_schedule$FK_course_calendar_schedule_rooms",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_fixed_schedule$FK_course_fixed_schedule_block_periods",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_fixed_schedule$FK_course_fixed_schedule_rooms",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_section$FK_course_section_attendance_code_categories",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "course_standard$FK_course_standard_course",
                table: "course_standard");

            migrationBuilder.DropForeignKey(
                name: "course_standard$FK_course_standard_grade_us_standard",
                table: "course_standard");

            migrationBuilder.DropForeignKey(
                name: "course_variable_schedule$FK_course_variable_schedule_block_perio",
                table: "course_variable_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_variable_schedule$FK_course_variable_schedule_rooms",
                table: "course_variable_schedule");

            migrationBuilder.DropForeignKey(
                name: "custom_fields_value$FK_custom_fields_value_custom_fields",
                table: "custom_fields_value");

            migrationBuilder.DropForeignKey(
                name: "effort_grade_library_category_item$FK_effort_category_item_effor",
                table: "effort_grade_library_category_item");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration$FK_gradebook_configuration_course_sectio",
                table: "gradebook_configuration");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_gradescale$FK_gradebook_configuration_gr",
                table: "gradebook_configuration_gradescale");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_quarter$FK_gradebook_configuration",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_quarter$FK_quarters",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_semester$FK_gradebook_configuration",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_semester$FK_quarters",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_semester$FK_semester",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_year$FK_gradebook_configuration",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_year$FK_school_years",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_year$FK_semesters",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_gradebook_grades_assignment",
                table: "gradebook_grades");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_gradebook_grades_student_master",
                table: "gradebook_grades");

            migrationBuilder.DropForeignKey(
                name: "historical_credit_transfer$FK_credit_transfer_grade",
                table: "historical_credit_transfer");

            migrationBuilder.DropForeignKey(
                name: "permission_category$FK_permission_category_permission_group",
                table: "permission_category");

            migrationBuilder.DropForeignKey(
                name: "permission_subcategory$FK_permission_subcategory_permission_cate",
                table: "permission_subcategory");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_role_permission_membership",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_role_permission_permission_category",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_role_permission_permission_groupId",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_role_permission_permission_subcategory",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "school_preference$FK_school_preference_school_master",
                table: "school_preference");

            migrationBuilder.DropForeignKey(
                name: "staff_certificate_info$FK_staff_certificate_info_staff_master",
                table: "staff_certificate_info");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_cou",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_qua",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_sch",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_sem",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_sta",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_school_info$FK_staff_school_info_staff_master",
                table: "staff_school_info");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_student_attendance_attendance_code",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_student_attendance_block_period",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_student_attendance_membership",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_staff_coursesection_schedule",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_student_coursesection_schedule",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance_comments$FK_membership",
                table: "student_attendance_comments");

            migrationBuilder.DropForeignKey(
                name: "student_attendance_comments$FK_student_attendance",
                table: "student_attendance_comments");

            migrationBuilder.DropForeignKey(
                name: "student_attendance_history$FK_student_attendance_history_student",
                table: "student_attendance_history");

            migrationBuilder.DropForeignKey(
                name: "student_comments$FK_student_comments_student_master",
                table: "student_comments");

            migrationBuilder.DropForeignKey(
                name: "student_coursesection_schedule$FK_course_section",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "student_coursesection_schedule$FK_school_master",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "student_coursesection_schedule$FK_student_master",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_grade_scale",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_sections",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_student_mas",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_documents$FK_student_documents_student_master",
                table: "student_documents");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_detail$FK_student_effort_grade_detail_stude",
                table: "student_effort_grade_detail");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_quart",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_schoo",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_master$FK_school_years",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_semes",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_enrollment$FK_student_enrollment_gradelevels",
                table: "student_enrollment");

            migrationBuilder.DropForeignKey(
                name: "student_enrollment_code$FK_student_enrollment_code_school_master",
                table: "student_enrollment_code");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_student_final_grade_quarters",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_student_final_grade_school_years",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_student_final_grade_semesters",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_student_final_grade_student_final_grade",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade_comments$FK_student_final_grade_comments_cou",
                table: "student_final_grade_comments");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade_comments$FK_student_final_grade_comments_stu",
                table: "student_final_grade_comments");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade_standard$FK_student_final_grade_standard_stu",
                table: "student_final_grade_standard");

            migrationBuilder.DropForeignKey(
                name: "student_medical_alert$FK_student_medical_alert_student_master",
                table: "student_medical_alert");

            migrationBuilder.DropForeignKey(
                name: "student_medical_immunization$FK_student_medical_immunization_stu",
                table: "student_medical_immunization");

            migrationBuilder.DropForeignKey(
                name: "student_medical_note$FK_student_medical_note_student_master",
                table: "student_medical_note");

            migrationBuilder.DropForeignKey(
                name: "student_medical_nurse_visit$FK_student_medical_nurse_visit_stude",
                table: "student_medical_nurse_visit");

            migrationBuilder.DropForeignKey(
                name: "student_medical_provider$FK_student_medical_provider_student_mas",
                table: "student_medical_provider");

            migrationBuilder.DropForeignKey(
                name: "student_report_card_detail$FK_student_report_card_detail_student",
                table: "student_report_card_detail");

            migrationBuilder.DropForeignKey(
                name: "student_report_card_master$FK_student_report_card_student_master",
                table: "student_report_card_master");

            migrationBuilder.DropForeignKey(
                name: "student_transcript_detail$FK_student_transcript_detail_student_t",
                table: "student_transcript_detail");

            migrationBuilder.DropForeignKey(
                name: "student_transcript_master$FK_student_transcript_master_student_m",
                table: "student_transcript_master");

            migrationBuilder.DropForeignKey(
                name: "user_secret_questions$FK_user_secret_questions_user_master",
                table: "user_secret_questions");

            migrationBuilder.RenameIndex(
                name: "IX_student_transcript_detail_tenant_id_school_id_student_id_gra~",
                table: "student_transcript_detail",
                newName: "IX_student_transcript_detail_tenant_id_school_id_s");

            migrationBuilder.RenameIndex(
                name: "IX_student_report_card_detail_tenant_id_school_id_student_id_sc~",
                table: "student_report_card_detail",
                newName: "IX_student_report_card_detail_tenant_id_school_id_");

            migrationBuilder.RenameIndex(
                name: "student_master$AK_student_master_tenant_id_school_id_student_gui",
                table: "student_master",
                newName: "student_master$AK_student_master_tenant_id_school_");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_standard_tenant_id_school_id_student_id_~",
                table: "student_final_grade_standard",
                newName: "IX_student_final_grade_standard_tenant_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_comments_tenant_id_school_id_course_comm~",
                table: "student_final_grade_comments",
                newName: "IX_student_final_grade_comments_tenant_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_yr_marking_period_id",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_yr_mark");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_smstr_marking_period~",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_smstr_m");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_qtr_marking_period_id",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_qtr_mar");

            migrationBuilder.RenameIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_guid",
                table: "student_enrollment",
                newName: "IX_student_enrollment_tenant_id_school_id_student_");

            migrationBuilder.RenameIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_yr_marking_p~",
                table: "student_effort_grade_master",
                newName: "IX_std_effort_grade_mster_tid_school_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_smstr_markin~",
                table: "student_effort_grade_master",
                newName: "IX_std_effort_gd_master_tid_school_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_qtr_marking_~",
                table: "student_effort_grade_master",
                newName: "IX_std_effort_grade_master_tid_school_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id_calendar_id",
                table: "student_effort_grade_master",
                newName: "IX_student_effort_grade_master_tenant_id_school_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_section_id",
                table: "student_daily_attendance",
                newName: "IX_student_daily_attendance_tenant_id_school_id_se");

            migrationBuilder.RenameIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_grade_scale_id",
                table: "student_daily_attendance",
                newName: "IX_student_daily_attendance_tenant_id_school_id_gr");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id_membership_id",
                table: "student_attendance_comments",
                newName: "IX_student_attendance_comments_tenant_id_school_id");

            migrationBuilder.RenameIndex(
                name: "student_attendance$AK_student_attendance_tenant_id_school_id_stu",
                table: "student_attendance",
                newName: "student_attendance$AK_student_attendance_tenant_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_student_id_course_id_~",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_student_");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_staff_id_course_id_co~",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_staff_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_membership_id",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_membersh");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_block_id_period_id",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_block_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_attendance_category_i~",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_attendan");

            migrationBuilder.RenameIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_yr_marking_~",
                table: "staff_coursesection_schedule",
                newName: "staff_coursesection_schedule_tenant_id_3");

            migrationBuilder.RenameIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_smstr_marki~",
                table: "staff_coursesection_schedule",
                newName: "staff_coursesection_schedule_tenant_id_2");

            migrationBuilder.RenameIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_qtr_marking~",
                table: "staff_coursesection_schedule",
                newName: "staff_coursesection_schedule_tenant_id_1");

            migrationBuilder.RenameIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_id_course_id_c~",
                table: "staff_coursesection_schedule",
                newName: "IX_staff_coursesection_schedule_tenant_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_subcategory_id",
                table: "role_permission",
                newName: "role_permission_tenant_id_school_id_permission_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_group_id",
                table: "role_permission",
                newName: "role_permission_tenant_id_school_id_permission");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_category_id",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_permission_");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_membership_id",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_membership_");

            migrationBuilder.RenameIndex(
                name: "IX_permission_subcategory_tenant_id_school_id_permission_catego~",
                table: "permission_subcategory",
                newName: "IX_permission_subcategory_tenant_id_school_id_perm");

            migrationBuilder.RenameIndex(
                name: "IX_permission_category_tenant_id_school_id_permission_group_id",
                table: "permission_category",
                newName: "IX_permission_category_tenant_id_school_id_permiss");

            //migrationBuilder.RenameIndex(
            //    name: "IX_honor_rolls_tenant_id_school_id_marking_period_id",
            //    table: "honor_rolls",
            //    newName: "IX_honor_rolls_tenant_id_school_id_marking_period_");

            migrationBuilder.DropForeignKey(
                name: "honor_rolls$FK_honor_rolls_honor_rolls",
                table: "honor_rolls");

            migrationBuilder.DropIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_id",
                table: "honor_rolls");

            migrationBuilder.AddForeignKey(
                name: "honor_rolls$FK_honor_rolls_honor_rolls",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.CreateIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_",
                table: "honor_rolls",
                columns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.RenameIndex(
                name: "historical_marking_period$IX_historical_marking_period",
                table: "historical_marking_period",
                newName: "historical_marking_period$IX_historical_marking_pe");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment_id",
                table: "gradebook_grades",
                newName: "IX_gradebook_grades_tenant_id_school_id_assignment");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_yr_marking_~",
                table: "gradebook_configuration_year",
                newName: "IX_grade_conf_yr_tenant_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_smstr_marki~",
                table: "gradebook_configuration_year",
                newName: "IX_grade_conf_yr_t_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_id_course_id_c~",
                table: "gradebook_configuration_year",
                newName: "IX_gradebook_configuration_year_tenant_id_school_i");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_smstr_m~",
                table: "gradebook_configuration_semester",
                newName: "IX_grade_conf_sem_t_id_school");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_qtr_mar~",
                table: "gradebook_configuration_semester",
                newName: "IX_gradebook_conf_sem_school");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_school_id_course_~",
                table: "gradebook_configuration_semester",
                newName: "IX_gradebook_configuration_semester_tenant_id_scho");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_qtr_mark~",
                table: "gradebook_configuration_quarter",
                newName: "IX_gradebook_conf_qtr_tenant_id_school");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_school_id_course_i~",
                table: "gradebook_configuration_quarter",
                newName: "IX_gradebook_configuration_quarter_tenant_id_schoo");

            migrationBuilder.RenameIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_room_id",
                table: "course_variable_schedule",
                newName: "IX_course_variable_schedule_tenant_id_school_id_ro");

            migrationBuilder.RenameIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_block_id_period~",
                table: "course_variable_schedule",
                newName: "IX_course_variable_schedule_tenant_id_school_id_bl");

            migrationBuilder.RenameIndex(
                name: "IX_course_standard_tenant_id_school_id_standard_ref_no_grade_st~",
                table: "course_standard",
                newName: "IX_course_standard_tenant_id_school_id_standard_re");

            //migrationBuilder.RenameIndex(
            //    name: "IX_course_section_tenant_id_school_id_yr_marking_period_id",
            //    table: "course_section",
            //    newName: "IX_course_section_tenant_id_school_id_yr_marking_p");

            //migrationBuilder.RenameIndex(
            //    name: "IX_course_section_tenant_id_school_id_smstr_marking_period_id",
            //    table: "course_section",
            //    newName: "IX_course_section_tenant_id_school_id_smstr_markin");

            //migrationBuilder.RenameIndex(
            //    name: "IX_course_section_tenant_id_school_id_qtr_marking_period_id",
            //    table: "course_section",
            //    newName: "IX_course_section_tenant_id_school_id_qtr_marking_");

            //migrationBuilder.RenameIndex(
            //    name: "IX_course_section_tenant_id_school_id_grade_scale_id",
            //    table: "course_section",
            //    newName: "IX_course_section_tenant_id_school_id_grade_scale_");

            migrationBuilder.DropForeignKey(
                name: "course_section$FK_course_section_school_years",
                table: "course_section");
            migrationBuilder.DropForeignKey(
                name: "course_section$FK_course_section_semesters",
                table: "course_section");
            migrationBuilder.DropForeignKey(
                name: "course_section$FK_course_section_quarters",
                table: "course_section");
            migrationBuilder.DropForeignKey(
                name: "course_section$FK_course_section_grade_scale",
                table: "course_section");

            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_period_id",
                table: "course_section");
            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_marking_period_id",
                table: "course_section");
            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_period_id",
                table: "course_section");
            migrationBuilder.DropIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_id",
                table: "course_section");

            migrationBuilder.AddForeignKey(
                name: "course_section$FK_course_section_school_years",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
            migrationBuilder.AddForeignKey(
                name: "course_section$FK_course_section_semesters",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
            migrationBuilder.AddForeignKey(
                name: "course_section$FK_course_section_quarters",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });
            migrationBuilder.AddForeignKey(
                name: "course_section$FK_course_section_grade_scale",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                principalTable: "grade_scale",
                principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_p",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" });
            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_markin",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" });
            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" });
            migrationBuilder.CreateIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_attendance_category_id",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_attendance_c");

            migrationBuilder.RenameIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_room_id",
                table: "course_fixed_schedule",
                newName: "IX_course_fixed_schedule_tenant_id_school_id_room_");

            migrationBuilder.RenameIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_fixed_schedule",
                newName: "IX_course_fixed_schedule_tenant_id_school_id_block");

            migrationBuilder.RenameIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_room_id",
                table: "course_calendar_schedule",
                newName: "IX_course_calendar_schedule_tenant_id_school_id_ro");

            migrationBuilder.RenameIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_block_id_period~",
                table: "course_calendar_schedule",
                newName: "IX_course_calendar_schedule_tenant_id_school_id_bl");

            migrationBuilder.RenameIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_room_id",
                table: "course_block_schedule",
                newName: "IX_course_block_schedule_tenant_id_school_id_room");

            migrationBuilder.RenameIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block_id_period_id",
                table: "course_block_schedule",
                newName: "IX_course_block_schedule_tenant_id_school_id_block");

            //migrationBuilder.RenameIndex(
            //    name: "IX_assignment_tenant_id_school_id_assignment_type_id",
            //    table: "assignment",
            //    newName: "IX_assignment_tenant_id_school_id_assignment_type");

            migrationBuilder.DropForeignKey(
                name: "assignment$FK_assignment_assignment_type",
                table: "assignment");

            migrationBuilder.DropIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type_id",
                table: "assignment");

            migrationBuilder.AddForeignKey(
                name: "assignment$FK_assignment_assignment_type",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_type_id" },
                principalTable: "assignment_type",
                principalColumns: new[] { "tenant_id", "school_id", "assignment_type_id" });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type",
                table: "assignment",
                columns: new[] { "tenant_id", "school_id", "assignment_type_id" });

            migrationBuilder.RenameIndex(
                name: "IX_api_controller_key_mapping_tenant_id_controller_id",
                table: "api_controller_key_mapping",
                newName: "IX_api_controller_key_mapping_tenant_id_controller");

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "subject",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "rollover_id",
                table: "semesters",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "rooms",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "honor_rolls",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "grade_scale",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "course_comment_category",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "course",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "academic_year",
                table: "block_period",
                type: "decimal(4,0)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "scheduled_job",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<long>(type: "bigint", nullable: false),
                    academic_year = table.Column<decimal>(type: "decimal(4,0)", nullable: true),
                    job_title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    job_schedule_date = table.Column<DateTime>(type: "date", nullable: true),
                    api_title = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb4_general_ci"),
                    controller_path = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_general_ci"),
                    task_json = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_general_ci"),
                    last_run_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_run_status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    created_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_general_ci"),
                    updated_on = table.Column<DateTime>(type: "datetime(0)", precision: 0, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_job", x => new { x.tenant_id, x.school_id, x.job_id });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "scheduled_job_history",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    school_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<long>(type: "bigint", nullable: false),
                    job_run_id = table.Column<int>(type: "int", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "date", nullable: true),
                    run_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    job_status = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_job_history", x => new { x.tenant_id, x.school_id, x.job_id, x.job_run_id });
                    table.ForeignKey(
                        name: "FK_job_history_job",
                        columns: x => new { x.tenant_id, x.school_id, x.job_id },
                        principalTable: "scheduled_job",
                        principalColumns: new[] { "tenant_id", "school_id", "job_id" });
                })
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_list",
                table: "api_controller_key_mapping",
                columns: new[] { "tenant_id", "controller_id" },
                principalTable: "api_controller_list",
                principalColumns: new[] { "tenant_id", "controller_id" });

            migrationBuilder.AddForeignKey(
                name: "api_controller_key_mapping$FK_api_keys_master",
                table: "api_controller_key_mapping",
                columns: new[] { "tenant_id", "school_id", "key_id" },
                principalTable: "api_keys_master",
                principalColumns: new[] { "tenant_id", "school_id", "key_id" });

            migrationBuilder.AddForeignKey(
                name: "attendance_code$FK_categories",
                table: "attendance_code",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                principalTable: "attendance_code_categories",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.AddForeignKey(
                name: "attendance_code_categories$FK_school",
                table: "attendance_code_categories",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_block",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id" },
                principalTable: "block",
                principalColumns: new[] { "tenant_id", "school_id", "block_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_periods",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_rooms",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_calendar_schedule$FK_periods",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_calendar_schedule$FK_rooms",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_fixed_schedule$FK_periods",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_fixed_schedule$FK_rooms",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_section$FK_categories",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                principalTable: "attendance_code_categories",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.AddForeignKey(
                name: "course_standard$FK_course",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "course_id" },
                principalTable: "course",
                principalColumns: new[] { "tenant_id", "school_id", "course_id" });

            migrationBuilder.AddForeignKey(
                name: "course_standard$FK_us_standard",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" },
                principalTable: "grade_us_standard",
                principalColumns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });

            migrationBuilder.AddForeignKey(
                name: "course_variable_schedule$FK_block_periods",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_variable_schedule$FK_rooms",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "custom_fields_value$FK_fields",
                table: "custom_fields_value",
                columns: new[] { "tenant_id", "school_id", "category_id", "field_id" },
                principalTable: "custom_fields",
                principalColumns: new[] { "tenant_id", "school_id", "category_id", "field_id" });

            migrationBuilder.AddForeignKey(
                name: "library_category_item$FK_category",
                table: "effort_grade_library_category_item",
                columns: new[] { "tenant_id", "school_id", "effort_category_id" },
                principalTable: "effort_grade_library_category",
                principalColumns: new[] { "tenant_id", "school_id", "effort_category_id" });

            migrationBuilder.AddForeignKey(
                name: "grd_conf_course_sec",
                table: "gradebook_configuration",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_conf_gradescale",
                table: "gradebook_configuration_gradescale",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_quarter$FK_configuration",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_quarter$FK_quarters",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_config_semester$FK_semester",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_semester$FK_quarters",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_semester$FK_semester",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_config_year$FK_config",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_config_year$FK_school_years",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_config_year$FK_sem",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_assignment",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" },
                principalTable: "assignment",
                principalColumns: new[] { "tenant_id", "school_id", "assignment_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_studmast",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "hist_credit_trf$FK_hist_grade",
                table: "historical_credit_transfer",
                columns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" },
                principalTable: "historical_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" });

            migrationBuilder.AddForeignKey(
                name: "permission_category$FK_group",
                table: "permission_category",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" },
                principalTable: "permission_group",
                principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.AddForeignKey(
                name: "permission_subcategory$FK_category",
                table: "permission_subcategory",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" },
                principalTable: "permission_category",
                principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_category",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" },
                principalTable: "permission_category",
                principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_groupId",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" },
                principalTable: "permission_group",
                principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_membership",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_subcategory",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_subcategory_id" },
                principalTable: "permission_subcategory",
                principalColumns: new[] { "tenant_id", "school_id", "permission_subcategory_id" });

            migrationBuilder.AddForeignKey(
                name: "school_preference$FK_school",
                table: "school_preference",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_certificate_info$FK_staff",
                table: "staff_certificate_info",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_quarters",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_section",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_semesters",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_master",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_years",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_school_info$FK_master",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attd_coursesec_sch",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" },
                principalTable: "student_coursesection_schedule",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_code",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" },
                principalTable: "attendance_code",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_membership",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_staff_cs_sch",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" },
                principalTable: "staff_coursesection_schedule",
                principalColumns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FKperiod",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "std_attd_comments$FK_std_atd",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                principalTable: "student_attendance",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance_comments$FK_membership",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance_history$FK_student",
                table: "student_attendance_history",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_comments$FK_std_master",
                table: "student_comments",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "sch_mast_std_cs_sch",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "std_cs_cs_sch",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "std_mast_cs_sch",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_grade_scale",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                principalTable: "grade_scale",
                principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_master",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_sections",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "section_id" },
                principalTable: "sections",
                principalColumns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_documents$FK_master",
                table: "student_documents",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "std_effort_master_detail",
                table: "student_effort_grade_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" },
                principalTable: "student_effort_grade_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_calendars",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "calendar_id" },
                principalTable: "school_calendars",
                principalColumns: new[] { "tenant_id", "school_id", "calender_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_quarters",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_semesters",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_years",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_enrollment$FK_gradelevels",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" },
                principalTable: "gradelevels",
                principalColumns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.AddForeignKey(
                name: "student_enrollment_code$FK_master",
                table: "student_enrollment_code",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_master",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_quarters",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_semesters",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_years",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "std_final_grade_cmnts$FK_final_grade",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" },
                principalTable: "student_final_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade_comments$FK_comment_category",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "course_comment_id" },
                principalTable: "course_comment_category",
                principalColumns: new[] { "tenant_id", "school_id", "course_comment_id" });

            migrationBuilder.AddForeignKey(
                name: "std_final_grade_std$FK_final_grade",
                table: "student_final_grade_standard",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" },
                principalTable: "student_final_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_alert$FK_master",
                table: "student_medical_alert",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_immunization$FK_master",
                table: "student_medical_immunization",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_note$FK_master",
                table: "student_medical_note",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_nurse_visit$FK_master",
                table: "student_medical_nurse_visit",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_provider$FK_master",
                table: "student_medical_provider",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_report_card_detail$FK_master",
                table: "student_report_card_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" },
                principalTable: "student_report_card_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });

            migrationBuilder.AddForeignKey(
                name: "student_report_card_master$FK_master",
                table: "student_report_card_master",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_transcript_detail$FK_transcript",
                table: "student_transcript_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "grade_title" },
                principalTable: "student_transcript_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "grade_title" });

            migrationBuilder.AddForeignKey(
                name: "student_transcript_master$FK_master",
                table: "student_transcript_master",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "user_secret_questions$FK_master",
                table: "user_secret_questions",
                columns: new[] { "tenant_id", "school_id", "emailaddress" },
                principalTable: "user_master",
                principalColumns: new[] { "tenant_id", "school_id", "emailaddress" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_list",
                table: "api_controller_key_mapping");

            migrationBuilder.DropForeignKey(
                name: "api_controller_key_mapping$FK_api_keys_master",
                table: "api_controller_key_mapping");

            migrationBuilder.DropForeignKey(
                name: "attendance_code$FK_categories",
                table: "attendance_code");

            migrationBuilder.DropForeignKey(
                name: "attendance_code_categories$FK_school",
                table: "attendance_code_categories");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_block",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_periods",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_block_schedule$FK_rooms",
                table: "course_block_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_calendar_schedule$FK_periods",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_calendar_schedule$FK_rooms",
                table: "course_calendar_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_fixed_schedule$FK_periods",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_fixed_schedule$FK_rooms",
                table: "course_fixed_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_section$FK_categories",
                table: "course_section");

            migrationBuilder.DropForeignKey(
                name: "course_standard$FK_course",
                table: "course_standard");

            migrationBuilder.DropForeignKey(
                name: "course_standard$FK_us_standard",
                table: "course_standard");

            migrationBuilder.DropForeignKey(
                name: "course_variable_schedule$FK_block_periods",
                table: "course_variable_schedule");

            migrationBuilder.DropForeignKey(
                name: "course_variable_schedule$FK_rooms",
                table: "course_variable_schedule");

            migrationBuilder.DropForeignKey(
                name: "custom_fields_value$FK_fields",
                table: "custom_fields_value");

            migrationBuilder.DropForeignKey(
                name: "library_category_item$FK_category",
                table: "effort_grade_library_category_item");

            migrationBuilder.DropForeignKey(
                name: "grd_conf_course_sec",
                table: "gradebook_configuration");

            migrationBuilder.DropForeignKey(
                name: "gradebook_conf_gradescale",
                table: "gradebook_configuration_gradescale");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_quarter$FK_configuration",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_quarter$FK_quarters",
                table: "gradebook_configuration_quarter");

            migrationBuilder.DropForeignKey(
                name: "gradebook_config_semester$FK_semester",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_semester$FK_quarters",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_configuration_semester$FK_semester",
                table: "gradebook_configuration_semester");

            migrationBuilder.DropForeignKey(
                name: "gradebook_config_year$FK_config",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_config_year$FK_school_years",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_config_year$FK_sem",
                table: "gradebook_configuration_year");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_assignment",
                table: "gradebook_grades");

            migrationBuilder.DropForeignKey(
                name: "gradebook_grades$FK_studmast",
                table: "gradebook_grades");

            migrationBuilder.DropForeignKey(
                name: "hist_credit_trf$FK_hist_grade",
                table: "historical_credit_transfer");

            migrationBuilder.DropForeignKey(
                name: "permission_category$FK_group",
                table: "permission_category");

            migrationBuilder.DropForeignKey(
                name: "permission_subcategory$FK_category",
                table: "permission_subcategory");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_category",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_groupId",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_membership",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "role_permission$FK_subcategory",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "school_preference$FK_school",
                table: "school_preference");

            migrationBuilder.DropForeignKey(
                name: "staff_certificate_info$FK_staff",
                table: "staff_certificate_info");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_quarters",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_section",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_semesters",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_staff_master",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_coursesection_schedule$FK_years",
                table: "staff_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "staff_school_info$FK_master",
                table: "staff_school_info");

            migrationBuilder.DropForeignKey(
                name: "student_attd_coursesec_sch",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_code",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_membership",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FK_staff_cs_sch",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_attendance$FKperiod",
                table: "student_attendance");

            migrationBuilder.DropForeignKey(
                name: "std_attd_comments$FK_std_atd",
                table: "student_attendance_comments");

            migrationBuilder.DropForeignKey(
                name: "student_attendance_comments$FK_membership",
                table: "student_attendance_comments");

            migrationBuilder.DropForeignKey(
                name: "student_attendance_history$FK_student",
                table: "student_attendance_history");

            migrationBuilder.DropForeignKey(
                name: "student_comments$FK_std_master",
                table: "student_comments");

            migrationBuilder.DropForeignKey(
                name: "sch_mast_std_cs_sch",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "std_cs_cs_sch",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "std_mast_cs_sch",
                table: "student_coursesection_schedule");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_grade_scale",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_master",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_daily_attendance$FK_sections",
                table: "student_daily_attendance");

            migrationBuilder.DropForeignKey(
                name: "student_documents$FK_master",
                table: "student_documents");

            migrationBuilder.DropForeignKey(
                name: "std_effort_master_detail",
                table: "student_effort_grade_detail");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_calendars",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_quarters",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_semesters",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_effort_grade_years",
                table: "student_effort_grade_master");

            migrationBuilder.DropForeignKey(
                name: "student_enrollment$FK_gradelevels",
                table: "student_enrollment");

            migrationBuilder.DropForeignKey(
                name: "student_enrollment_code$FK_master",
                table: "student_enrollment_code");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_master",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_quarters",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_semesters",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade$FK_years",
                table: "student_final_grade");

            migrationBuilder.DropForeignKey(
                name: "std_final_grade_cmnts$FK_final_grade",
                table: "student_final_grade_comments");

            migrationBuilder.DropForeignKey(
                name: "student_final_grade_comments$FK_comment_category",
                table: "student_final_grade_comments");

            migrationBuilder.DropForeignKey(
                name: "std_final_grade_std$FK_final_grade",
                table: "student_final_grade_standard");

            migrationBuilder.DropForeignKey(
                name: "student_medical_alert$FK_master",
                table: "student_medical_alert");

            migrationBuilder.DropForeignKey(
                name: "student_medical_immunization$FK_master",
                table: "student_medical_immunization");

            migrationBuilder.DropForeignKey(
                name: "student_medical_note$FK_master",
                table: "student_medical_note");

            migrationBuilder.DropForeignKey(
                name: "student_medical_nurse_visit$FK_master",
                table: "student_medical_nurse_visit");

            migrationBuilder.DropForeignKey(
                name: "student_medical_provider$FK_master",
                table: "student_medical_provider");

            migrationBuilder.DropForeignKey(
                name: "student_report_card_detail$FK_master",
                table: "student_report_card_detail");

            migrationBuilder.DropForeignKey(
                name: "student_report_card_master$FK_master",
                table: "student_report_card_master");

            migrationBuilder.DropForeignKey(
                name: "student_transcript_detail$FK_transcript",
                table: "student_transcript_detail");

            migrationBuilder.DropForeignKey(
                name: "student_transcript_master$FK_master",
                table: "student_transcript_master");

            migrationBuilder.DropForeignKey(
                name: "user_secret_questions$FK_master",
                table: "user_secret_questions");

            migrationBuilder.DropTable(
                name: "scheduled_job_history");

            migrationBuilder.DropTable(
                name: "scheduled_job");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "subject");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "honor_rolls");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "grade_scale");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "course_comment_category");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "course");

            migrationBuilder.DropColumn(
                name: "academic_year",
                table: "block_period");

            migrationBuilder.RenameIndex(
                name: "IX_student_transcript_detail_tenant_id_school_id_s",
                table: "student_transcript_detail",
                newName: "IX_student_transcript_detail_tenant_id_school_id_student_id_gra~");

            migrationBuilder.RenameIndex(
                name: "IX_student_report_card_detail_tenant_id_school_id_",
                table: "student_report_card_detail",
                newName: "IX_student_report_card_detail_tenant_id_school_id_student_id_sc~");

            migrationBuilder.RenameIndex(
                name: "student_master$AK_student_master_tenant_id_school_",
                table: "student_master",
                newName: "student_master$AK_student_master_tenant_id_school_id_student_guid");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_standard_tenant_id_school_i",
                table: "student_final_grade_standard",
                newName: "IX_student_final_grade_standard_tenant_id_school_id_student_id_~");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_comments_tenant_id_school_i",
                table: "student_final_grade_comments",
                newName: "IX_student_final_grade_comments_tenant_id_school_id_course_comm~");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_yr_mark",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_yr_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_smstr_m",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_smstr_marking_period~");

            migrationBuilder.RenameIndex(
                name: "IX_student_final_grade_tenant_id_school_id_qtr_mar",
                table: "student_final_grade",
                newName: "IX_student_final_grade_tenant_id_school_id_qtr_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_enrollment_tenant_id_school_id_student_",
                table: "student_enrollment",
                newName: "IX_student_enrollment_tenant_id_school_id_student_guid");

            migrationBuilder.RenameIndex(
                name: "IX_student_effort_grade_master_tenant_id_school_id",
                table: "student_effort_grade_master",
                newName: "IX_student_effort_grade_master_tenant_id_school_id_calendar_id");

            migrationBuilder.RenameIndex(
                name: "IX_std_effort_grade_mster_tid_school_id",
                table: "student_effort_grade_master",
                newName: "IX_student_effort_grade_master_tenant_id_school_id_yr_marking_p~");

            migrationBuilder.RenameIndex(
                name: "IX_std_effort_grade_master_tid_school_id",
                table: "student_effort_grade_master",
                newName: "IX_student_effort_grade_master_tenant_id_school_id_qtr_marking_~");

            migrationBuilder.RenameIndex(
                name: "IX_std_effort_gd_master_tid_school_id",
                table: "student_effort_grade_master",
                newName: "IX_student_effort_grade_master_tenant_id_school_id_smstr_markin~");

            migrationBuilder.RenameIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_se",
                table: "student_daily_attendance",
                newName: "IX_student_daily_attendance_tenant_id_school_id_section_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_daily_attendance_tenant_id_school_id_gr",
                table: "student_daily_attendance",
                newName: "IX_student_daily_attendance_tenant_id_school_id_grade_scale_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_comments_tenant_id_school_id",
                table: "student_attendance_comments",
                newName: "IX_student_attendance_comments_tenant_id_school_id_membership_id");

            migrationBuilder.RenameIndex(
                name: "student_attendance$AK_student_attendance_tenant_id",
                table: "student_attendance",
                newName: "student_attendance$AK_student_attendance_tenant_id_school_id_student_id_student_at~");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_student_",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_student_id_course_id_~");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_staff_id",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_staff_id_course_id_co~");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_membersh",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_membership_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_block_id",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_block_id_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_student_attendance_tenant_id_school_id_attendan",
                table: "student_attendance",
                newName: "IX_student_attendance_tenant_id_school_id_attendance_category_i~");

            migrationBuilder.RenameIndex(
                name: "staff_coursesection_schedule_tenant_id_3",
                table: "staff_coursesection_schedule",
                newName: "IX_staff_coursesection_schedule_tenant_id_school_id_yr_marking_~");

            migrationBuilder.RenameIndex(
                name: "staff_coursesection_schedule_tenant_id_2",
                table: "staff_coursesection_schedule",
                newName: "IX_staff_coursesection_schedule_tenant_id_school_id_smstr_marki~");

            migrationBuilder.RenameIndex(
                name: "staff_coursesection_schedule_tenant_id_1",
                table: "staff_coursesection_schedule",
                newName: "IX_staff_coursesection_schedule_tenant_id_school_id_qtr_marking~");

            migrationBuilder.RenameIndex(
                name: "IX_staff_coursesection_schedule_tenant_id_school_i",
                table: "staff_coursesection_schedule",
                newName: "IX_staff_coursesection_schedule_tenant_id_school_id_course_id_c~");

            migrationBuilder.RenameIndex(
                name: "role_permission_tenant_id_school_id_permission_id",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_permission_subcategory_id");

            migrationBuilder.RenameIndex(
                name: "role_permission_tenant_id_school_id_permission",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_permission_group_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_permission_",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_permission_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permission_tenant_id_school_id_membership_",
                table: "role_permission",
                newName: "IX_role_permission_tenant_id_school_id_membership_id");

            migrationBuilder.RenameIndex(
                name: "IX_permission_subcategory_tenant_id_school_id_perm",
                table: "permission_subcategory",
                newName: "IX_permission_subcategory_tenant_id_school_id_permission_catego~");

            migrationBuilder.RenameIndex(
                name: "IX_permission_category_tenant_id_school_id_permiss",
                table: "permission_category",
                newName: "IX_permission_category_tenant_id_school_id_permission_group_id");

            migrationBuilder.RenameIndex(
                name: "IX_honor_rolls_tenant_id_school_id_marking_period_",
                table: "honor_rolls",
                newName: "IX_honor_rolls_tenant_id_school_id_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "historical_marking_period$IX_historical_marking_pe",
                table: "historical_marking_period",
                newName: "historical_marking_period$IX_historical_marking_period");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_grades_tenant_id_school_id_assignment",
                table: "gradebook_grades",
                newName: "IX_gradebook_grades_tenant_id_school_id_assignment_id");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_year_tenant_id_school_i",
                table: "gradebook_configuration_year",
                newName: "IX_gradebook_configuration_year_tenant_id_school_id_course_id_c~");

            migrationBuilder.RenameIndex(
                name: "IX_grade_conf_yr_tenant_id_school_i",
                table: "gradebook_configuration_year",
                newName: "IX_gradebook_configuration_year_tenant_id_school_id_yr_marking_~");

            migrationBuilder.RenameIndex(
                name: "IX_grade_conf_yr_t_id_school_i",
                table: "gradebook_configuration_year",
                newName: "IX_gradebook_configuration_year_tenant_id_school_id_smstr_marki~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_semester_tenant_id_scho",
                table: "gradebook_configuration_semester",
                newName: "IX_gradebook_configuration_semester_tenant_id_school_id_course_~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_conf_sem_school",
                table: "gradebook_configuration_semester",
                newName: "IX_gradebook_configuration_semester_tenant_id_school_id_qtr_mar~");

            migrationBuilder.RenameIndex(
                name: "IX_grade_conf_sem_t_id_school",
                table: "gradebook_configuration_semester",
                newName: "IX_gradebook_configuration_semester_tenant_id_school_id_smstr_m~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_configuration_quarter_tenant_id_schoo",
                table: "gradebook_configuration_quarter",
                newName: "IX_gradebook_configuration_quarter_tenant_id_school_id_course_i~");

            migrationBuilder.RenameIndex(
                name: "IX_gradebook_conf_qtr_tenant_id_school",
                table: "gradebook_configuration_quarter",
                newName: "IX_gradebook_configuration_quarter_tenant_id_school_id_qtr_mark~");

            migrationBuilder.RenameIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_ro",
                table: "course_variable_schedule",
                newName: "IX_course_variable_schedule_tenant_id_school_id_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_variable_schedule_tenant_id_school_id_bl",
                table: "course_variable_schedule",
                newName: "IX_course_variable_schedule_tenant_id_school_id_block_id_period~");

            migrationBuilder.RenameIndex(
                name: "IX_course_standard_tenant_id_school_id_standard_re",
                table: "course_standard",
                newName: "IX_course_standard_tenant_id_school_id_standard_ref_no_grade_st~");

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_yr_marking_p",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_yr_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_smstr_markin",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_smstr_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_qtr_marking_",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_qtr_marking_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_grade_scale_",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_grade_scale_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_section_tenant_id_school_id_attendance_c",
                table: "course_section",
                newName: "IX_course_section_tenant_id_school_id_attendance_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_room_",
                table: "course_fixed_schedule",
                newName: "IX_course_fixed_schedule_tenant_id_school_id_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_fixed_schedule_tenant_id_school_id_block",
                table: "course_fixed_schedule",
                newName: "IX_course_fixed_schedule_tenant_id_school_id_block_id_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_ro",
                table: "course_calendar_schedule",
                newName: "IX_course_calendar_schedule_tenant_id_school_id_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_calendar_schedule_tenant_id_school_id_bl",
                table: "course_calendar_schedule",
                newName: "IX_course_calendar_schedule_tenant_id_school_id_block_id_period~");

            migrationBuilder.RenameIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_room",
                table: "course_block_schedule",
                newName: "IX_course_block_schedule_tenant_id_school_id_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_course_block_schedule_tenant_id_school_id_block",
                table: "course_block_schedule",
                newName: "IX_course_block_schedule_tenant_id_school_id_block_id_period_id");

            migrationBuilder.RenameIndex(
                name: "IX_assignment_tenant_id_school_id_assignment_type",
                table: "assignment",
                newName: "IX_assignment_tenant_id_school_id_assignment_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_api_controller_key_mapping_tenant_id_controller",
                table: "api_controller_key_mapping",
                newName: "IX_api_controller_key_mapping_tenant_id_controller_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "rollover_id",
                table: "semesters",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_controller_list",
                table: "api_controller_key_mapping",
                columns: new[] { "tenant_id", "controller_id" },
                principalTable: "api_controller_list",
                principalColumns: new[] { "tenant_id", "controller_id" });

            migrationBuilder.AddForeignKey(
                name: "api_controller_key_mapping$FK_api_controller_key_mapping_api_keys_master",
                table: "api_controller_key_mapping",
                columns: new[] { "tenant_id", "school_id", "key_id" },
                principalTable: "api_keys_master",
                principalColumns: new[] { "tenant_id", "school_id", "key_id" });

            migrationBuilder.AddForeignKey(
                name: "attendance_code$FK_attendance_code_attendance_code_categories",
                table: "attendance_code",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                principalTable: "attendance_code_categories",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.AddForeignKey(
                name: "attendance_code_categories$FK_attendance_code_categories_school_master",
                table: "attendance_code_categories",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_block",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id" },
                principalTable: "block",
                principalColumns: new[] { "tenant_id", "school_id", "block_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_block_periods",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_block_schedule$FK_course_block_schedule_rooms",
                table: "course_block_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_calendar_schedule$FK_course_calendar_schedule_block_periods",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_calendar_schedule$FK_course_calendar_schedule_rooms",
                table: "course_calendar_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_fixed_schedule$FK_course_fixed_schedule_block_periods",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_fixed_schedule$FK_course_fixed_schedule_rooms",
                table: "course_fixed_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "course_section$FK_course_section_attendance_code_categories",
                table: "course_section",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id" },
                principalTable: "attendance_code_categories",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id" });

            migrationBuilder.AddForeignKey(
                name: "course_standard$FK_course_standard_course",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "course_id" },
                principalTable: "course",
                principalColumns: new[] { "tenant_id", "school_id", "course_id" });

            migrationBuilder.AddForeignKey(
                name: "course_standard$FK_course_standard_grade_us_standard",
                table: "course_standard",
                columns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" },
                principalTable: "grade_us_standard",
                principalColumns: new[] { "tenant_id", "school_id", "standard_ref_no", "grade_standard_id" });

            migrationBuilder.AddForeignKey(
                name: "course_variable_schedule$FK_course_variable_schedule_block_periods",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "course_variable_schedule$FK_course_variable_schedule_rooms",
                table: "course_variable_schedule",
                columns: new[] { "tenant_id", "school_id", "room_id" },
                principalTable: "rooms",
                principalColumns: new[] { "tenant_id", "school_id", "room_id" });

            migrationBuilder.AddForeignKey(
                name: "custom_fields_value$FK_custom_fields_value_custom_fields",
                table: "custom_fields_value",
                columns: new[] { "tenant_id", "school_id", "category_id", "field_id" },
                principalTable: "custom_fields",
                principalColumns: new[] { "tenant_id", "school_id", "category_id", "field_id" });

            migrationBuilder.AddForeignKey(
                name: "effort_grade_library_category_item$FK_effort_category_item_effort_category",
                table: "effort_grade_library_category_item",
                columns: new[] { "tenant_id", "school_id", "effort_category_id" },
                principalTable: "effort_grade_library_category",
                principalColumns: new[] { "tenant_id", "school_id", "effort_category_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration$FK_gradebook_configuration_course_section",
                table: "gradebook_configuration",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_gradescale$FK_gradebook_configuration_gradescale_gradebook_configuration",
                table: "gradebook_configuration_gradescale",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_quarter$FK_gradebook_configuration_quarter_gradebook_configuration",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_quarter$FK_gradebook_configuration_quarter_quarters",
                table: "gradebook_configuration_quarter",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_semester$FK_gradebook_configuration_semester_gradebook_configuration",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_semester$FK_gradebook_configuration_semester_quarters",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_semester$FK_gradebook_configuration_semester_semester",
                table: "gradebook_configuration_semester",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_year$FK_gradebook_configuration_year_gradebook_configuration",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" },
                principalTable: "gradebook_configuration",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id", "academic_year", "gradebook_configuration_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_year$FK_gradebook_configuration_year_school_years",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_configuration_year$FK_gradebook_configuration_year_semesters",
                table: "gradebook_configuration_year",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_gradebook_grades_assignment",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "assignment_id" },
                principalTable: "assignment",
                principalColumns: new[] { "tenant_id", "school_id", "assignment_id" });

            migrationBuilder.AddForeignKey(
                name: "gradebook_grades$FK_gradebook_grades_student_master",
                table: "gradebook_grades",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "historical_credit_transfer$FK_credit_transfer_grade",
                table: "historical_credit_transfer",
                columns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" },
                principalTable: "historical_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "hist_grade_id" });

            migrationBuilder.AddForeignKey(
                name: "permission_category$FK_permission_category_permission_group",
                table: "permission_category",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" },
                principalTable: "permission_group",
                principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.AddForeignKey(
                name: "permission_subcategory$FK_permission_subcategory_permission_category",
                table: "permission_subcategory",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" },
                principalTable: "permission_category",
                principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_role_permission_membership",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_role_permission_permission_category",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_category_id" },
                principalTable: "permission_category",
                principalColumns: new[] { "tenant_id", "school_id", "permission_category_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_role_permission_permission_groupId",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_group_id" },
                principalTable: "permission_group",
                principalColumns: new[] { "tenant_id", "school_id", "permission_group_id" });

            migrationBuilder.AddForeignKey(
                name: "role_permission$FK_role_permission_permission_subcategory",
                table: "role_permission",
                columns: new[] { "tenant_id", "school_id", "permission_subcategory_id" },
                principalTable: "permission_subcategory",
                principalColumns: new[] { "tenant_id", "school_id", "permission_subcategory_id" });

            migrationBuilder.AddForeignKey(
                name: "school_preference$FK_school_preference_school_master",
                table: "school_preference",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_certificate_info$FK_staff_certificate_info_staff_master",
                table: "staff_certificate_info",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_course_section",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_quarters",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_school_years",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_semesters",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_coursesection_schedule$FK_staff_coursesection_schedule_staff_master",
                table: "staff_coursesection_schedule",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_school_info$FK_staff_school_info_staff_master",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "staff_id" },
                principalTable: "staff_master",
                principalColumns: new[] { "tenant_id", "staff_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_student_attendance_attendance_code",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" },
                principalTable: "attendance_code",
                principalColumns: new[] { "tenant_id", "school_id", "attendance_category_id", "attendance_code" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_student_attendance_block_period",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "block_id", "period_id" },
                principalTable: "block_period",
                principalColumns: new[] { "tenant_id", "school_id", "block_id", "period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_student_attendance_membership",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_student_attendance_staff_coursesection_schedule",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" },
                principalTable: "staff_coursesection_schedule",
                principalColumns: new[] { "tenant_id", "school_id", "staff_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance$FK_student_attendance_student_coursesection_schedule",
                table: "student_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" },
                principalTable: "student_coursesection_schedule",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance_comments$FK_student_attendance_comments_membership",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance_comments$FK_student_attendance_comments_student_attendance",
                table: "student_attendance_comments",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" },
                principalTable: "student_attendance",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_attendance_id" });

            migrationBuilder.AddForeignKey(
                name: "student_attendance_history$FK_student_attendance_history_student_master",
                table: "student_attendance_history",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_comments$FK_student_comments_student_master",
                table: "student_comments",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_coursesection_schedule$FK_student_coursesection_schedule_course_section",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" },
                principalTable: "course_section",
                principalColumns: new[] { "tenant_id", "school_id", "course_id", "course_section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_coursesection_schedule$FK_student_coursesection_schedule_school_master",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "student_coursesection_schedule$FK_student_coursesection_schedule_student_master",
                table: "student_coursesection_schedule",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_grade_scale",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "grade_scale_id" },
                principalTable: "grade_scale",
                principalColumns: new[] { "tenant_id", "school_id", "grade_scale_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_sections",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "section_id" },
                principalTable: "sections",
                principalColumns: new[] { "tenant_id", "school_id", "section_id" });

            migrationBuilder.AddForeignKey(
                name: "student_daily_attendance$FK_student_daily_attendance_student_master",
                table: "student_daily_attendance",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_documents$FK_student_documents_student_master",
                table: "student_documents",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_detail$FK_student_effort_grade_detail_student_effort_grade_master",
                table: "student_effort_grade_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" },
                principalTable: "student_effort_grade_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_effort_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_quarters",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_school_calendars",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "calendar_id" },
                principalTable: "school_calendars",
                principalColumns: new[] { "tenant_id", "school_id", "calender_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_school_years",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_effort_grade_master$FK_student_effort_grade_master_semesters",
                table: "student_effort_grade_master",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_enrollment$FK_student_enrollment_gradelevels",
                table: "student_enrollment",
                columns: new[] { "tenant_id", "school_id", "grade_id" },
                principalTable: "gradelevels",
                principalColumns: new[] { "tenant_id", "school_id", "grade_id" });

            migrationBuilder.AddForeignKey(
                name: "student_enrollment_code$FK_student_enrollment_code_school_master1",
                table: "student_enrollment_code",
                columns: new[] { "tenant_id", "school_id" },
                principalTable: "school_master",
                principalColumns: new[] { "tenant_id", "school_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_student_final_grade_quarters",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "qtr_marking_period_id" },
                principalTable: "quarters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_student_final_grade_school_years",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "yr_marking_period_id" },
                principalTable: "school_years",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_student_final_grade_semesters",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "smstr_marking_period_id" },
                principalTable: "semesters",
                principalColumns: new[] { "tenant_id", "school_id", "marking_period_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade$FK_student_final_grade_student_final_grade",
                table: "student_final_grade",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade_comments$FK_student_final_grade_comments_course_comment_category",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "course_comment_id" },
                principalTable: "course_comment_category",
                principalColumns: new[] { "tenant_id", "school_id", "course_comment_id" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade_comments$FK_student_final_grade_comments_student_final_grade1",
                table: "student_final_grade_comments",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" },
                principalTable: "student_final_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_final_grade_standard$FK_student_final_grade_standard_student_final_grade",
                table: "student_final_grade_standard",
                columns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" },
                principalTable: "student_final_grade",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "student_final_grade_srlno" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_alert$FK_student_medical_alert_student_master",
                table: "student_medical_alert",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_immunization$FK_student_medical_immunization_student_master",
                table: "student_medical_immunization",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_note$FK_student_medical_note_student_master",
                table: "student_medical_note",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_nurse_visit$FK_student_medical_nurse_visit_student_master",
                table: "student_medical_nurse_visit",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_medical_provider$FK_student_medical_provider_student_master",
                table: "student_medical_provider",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_report_card_detail$FK_student_report_card_detail_student_report_card_master",
                table: "student_report_card_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" },
                principalTable: "student_report_card_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "school_year", "marking_period_title" });

            migrationBuilder.AddForeignKey(
                name: "student_report_card_master$FK_student_report_card_student_master",
                table: "student_report_card_master",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "student_transcript_detail$FK_student_transcript_detail_student_transcript_master1",
                table: "student_transcript_detail",
                columns: new[] { "tenant_id", "school_id", "student_id", "grade_title" },
                principalTable: "student_transcript_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id", "grade_title" });

            migrationBuilder.AddForeignKey(
                name: "student_transcript_master$FK_student_transcript_master_student_master",
                table: "student_transcript_master",
                columns: new[] { "tenant_id", "school_id", "student_id" },
                principalTable: "student_master",
                principalColumns: new[] { "tenant_id", "school_id", "student_id" });

            migrationBuilder.AddForeignKey(
                name: "user_secret_questions$FK_user_secret_questions_user_master",
                table: "user_secret_questions",
                columns: new[] { "tenant_id", "school_id", "emailaddress" },
                principalTable: "user_master",
                principalColumns: new[] { "tenant_id", "school_id", "emailaddress" });
        }
    }
}
