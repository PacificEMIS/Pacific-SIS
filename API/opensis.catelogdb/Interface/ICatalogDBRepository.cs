using opensis.catelogdb.Models;
using System;
using System.Collections.Generic;
using opensis.catalogdb.ViewModels;
using System.Text;

namespace opensis.catelogdb.Interface
{
   public interface ICatalogDBRepository
    {
        public AvailableTenantViewModel CheckIfTenantIsAvailable(AvailableTenantViewModel tenants);
    }
}
