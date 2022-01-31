using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using opensis.catalogdb.ViewModels;
using opensis.catelogdb.Interface;
using opensis.catelogdb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.catelogdb.Repository
{
   public class CatalogDBRepository: ICatalogDBRepository
    {
        private CatalogDBContext context;

        public CatalogDBRepository(ICatalogDBContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        public AvailableTenantViewModel CheckIfTenantIsAvailable(AvailableTenantViewModel tenants)
        {
            AvailableTenantViewModel ReturnModel = new AvailableTenantViewModel();
            try
            {

                var tenantDetail = this.context?.AvailableTenants.Where(x => x.TenantName == tenants.tenant.TenantName && x.IsActive).FirstOrDefault();
                if (tenantDetail!=null)
                {
                    ReturnModel.tenant = tenantDetail;
                    ReturnModel.Failure = false;
                    ReturnModel.Message = "";
                }
                else
                {
                    ReturnModel.Failure = true ;
                    ReturnModel.Message = "Tenant not found";
                }
            }
          catch(Exception ex)
            {
                ReturnModel.Failure = true;
                ReturnModel.Message = ex.Message;
            }
            return ReturnModel;
        }
    }
}
