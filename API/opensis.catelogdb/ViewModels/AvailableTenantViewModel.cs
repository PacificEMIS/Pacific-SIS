using opensis.catelogdb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.catalogdb.ViewModels
{
   public class AvailableTenantViewModel
    {
        public bool Failure { get; set; }
        public string Message { get; set; }
        public AvailableTenants tenant { get; set; }
    }
}
