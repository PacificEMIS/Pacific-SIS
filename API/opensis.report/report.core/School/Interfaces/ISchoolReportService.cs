using opensis.report.report.data.ViewModels.SchoolReport;

namespace opensis.report.report.core.School.Interfaces
{
    public interface ISchoolReportService
    {
        public SchoolListForReport GetSchoolReport(SchoolListForReport report);
    }
}
