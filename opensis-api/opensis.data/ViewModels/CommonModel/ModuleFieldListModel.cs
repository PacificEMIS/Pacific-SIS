using Microsoft.AspNetCore.Mvc;
using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.CommonModel
{
    public class ModuleFieldListModel : CommonFields
    {
        public CustomFields [] CustomfieldTitle { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public string Module { get; set; }
    }
}
