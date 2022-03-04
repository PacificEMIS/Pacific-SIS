using opensis.data.Models;
using opensis.report.report.data.ViewModels.GradeReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.report.report.core.Grade.Interfaces
{
    public interface IGradeReportService
    {
        public HonorRollListForReport GetHonorRollReport(PageResult pageResult);
    }
}
