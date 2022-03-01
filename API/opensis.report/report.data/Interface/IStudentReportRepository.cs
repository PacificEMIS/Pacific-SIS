using opensis.data.Models;
using opensis.data.ViewModels.Student;
using opensis.report.report.data.ViewModels.StudentReport;

namespace opensis.report.report.data.Interface
{
    public interface IStudentReportRepository
    {
        public StudentInfoListForReport GetStudentInfoReport(StudentInfoReport studentInfoReport);
        public StudentAddDropViewModel GetstudentAddDropReport(PageResult pageResult);
        public StudentAdvancedReport StudentAdvancedReport(StudentAdvancedReport reportModel);
        public StudentEnrollmentReport GetStudentEnrollmentReport(PageResult pageResult);
        public StudentProgressReport GetStudentProgressReport(StudentProgressReport studentProgressReport);
    }
}
