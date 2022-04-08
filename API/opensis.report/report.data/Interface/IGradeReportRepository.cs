using opensis.data.Models;
using opensis.report.report.data.ViewModels.GradeReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.data.Interface
{
    public interface IGradeReportRepository
    {
        public HonorRollListForReport GetHonorRollReport(PageResult pageResult);
        public StudentFinalGradeViewModel GetStudentFinalGradeReport(StudentFinalGradeViewModel studentFinalGradeViewModel);
    }
}
