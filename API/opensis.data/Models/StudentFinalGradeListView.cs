using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Models
{
    public partial class StudentFinalGradeListView
    {
        public Guid SfgTenantId { get; set; }
        public int SfgSchoolId { get; set; }
        public int SfgStudentId { get; set; }
        public long SfgStudentFinalGradeSrlno { get; set; }
        public decimal? SfgAcademicYear { get; set; }
        public bool? SfgBasedOnStandardGrade { get; set; }
        public int? SfgCalendarId { get; set; }
        public int SfgCourseId { get; set; }
        public int SfgCourseSectionId { get; set; }
        public decimal? SfgCreditattempted { get; set; }
        public decimal? SfgCreditearned { get; set; }
        public int? SfgGradeId { get; set; }
        public string? SfgGradeObtained { get; set; }
        public int? SfgGradeScaleId { get; set; }
        public bool? SfgIsCustomMarkingPeriod { get; set; }
        public bool? SfgIsExamGrade { get; set; }
        public bool? SfgIsPercent { get; set; }
        public decimal? SfgPercentMarks { get; set; }
        public int? SfgPrgrsprdMarkingPeriodId { get; set; }
        public int? SfgQtrMarkingPeriodId { get; set; }
        public int? SfgSmstrMarkingPeriodId { get; set; }
        public int? SfgYrMarkingPeriodId { get; set; }
        public string? SfgTeacherComment { get; set; }
        public int? SyMarkingPeriodId { get; set; }
        public int? SemMarkingPeriodId { get; set; }
        public int? SemYearId { get; set; }
        public int? QtrMarkingPeriodId { get; set; }
        public int? QtrSemesterId { get; set; }
        public int? PpMarkingPeriodId { get; set; }
        public int? PpQuarterId { get; set; }
        public int? SfgsStudentId { get; set; }
        public int? SfgsId { get; set; }
        public decimal? SfgsAcademicYear { get; set; }
        public int? SfgsCalendarId { get; set; }
        public int? SfgsGradeObtained { get; set; }
        public int? SfgsPrgsMarkPeriodId { get; set; }
        public int? SfgsQtrMarkPeriodId { get; set; }
        public int? SfgsSmstrMarkPeriodId { get; set; }
        public int? SfgsStandardGradeScaleId { get; set; }
        public long? SfgsStudentFinalGradeSrlno { get; set; }
        public string? SfgsTeacherComment { get; set; }
        public int? SfgsYrMarkPeriodId { get; set; }
        public string? SchoolYearTitle { get; set; }
        public string? SemestersTitle { get; set; }
        public string? QuartersTitle { get; set; }
        public string? ProgressPeriodsTitle { get; set; }
    }
}
