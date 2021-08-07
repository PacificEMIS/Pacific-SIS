using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.CommonModel
{
    public class ChangePasswordViewModel : CommonFields
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string CurrentPasswordHash { get; set; }
        public string NewPasswordHash { get; set; }
    }
}
