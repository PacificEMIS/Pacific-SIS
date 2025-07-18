using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.School
{
    public class SchoolSeedingDataViewModel
    {

        public List<opensis.data.Models.Membership> MemberShip { get; set; } = null!;
        public List<FieldsCategory> FieldsCategory { get; set; } = null!;
        public List<StudentEnrollmentCode> StudentEnrollmentCode { get; set; } = null!;
        public List<AttendanceCode> AttendanceCode { get; set; } = null!;
        public List<Language> Language { get; set; } = null!;
    }
}
