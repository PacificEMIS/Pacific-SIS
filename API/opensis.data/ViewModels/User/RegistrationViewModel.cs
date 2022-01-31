using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.User
{
    public class RegistrationViewModel : CommonFields
    {
        public string? SchoolName { get; set; }
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? TenantName { get; set; }
        public string? APIDomain { get; set; }
        public DateTime? SchoolBeginDate { get; set; }
        public DateTime? SchoolEndDate { get; set; }


    }
}
