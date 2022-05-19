using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.ViewModels.StaffSchedule
{
    public class RemoveStaffScheduleViewModel: CommonFields
    {
        
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int StaffId { get; set; }
        public int CourseSectionId { get; set;}
       
    }
}
