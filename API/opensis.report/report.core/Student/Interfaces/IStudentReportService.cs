using opensis.data.Models;
using opensis.data.ViewModels.Student;
using opensis.report.report.data.ViewModels.StudentReport;

namespace opensis.report.report.core.Student.Interfaces
{
    public interface IStudentReportService
    {
        public StudentInfoListForReport GetStudentInfoReport(StudentInfoReport studentInfoReport);
        public StudentAddDropViewModel GetstudentAddDropReport(PageResult pageResult);
        public StudentAdvancedReport StudentAdvancedReport(StudentAdvancedReport reportModel);
        public StudentEnrollmentReport GetStudentEnrollmentReport(PageResult pageResult);
    }
}
