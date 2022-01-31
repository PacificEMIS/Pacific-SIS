using opensis.report.report.data.ViewModels.SchoolReport;

namespace opensis.report.report.data.Interface
{
    public interface ISchoolReportRepository
    {
        public SchoolListForReport GetSchoolReport(SchoolListForReport report);
    }
}
