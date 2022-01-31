using Microsoft.EntityFrameworkCore;
using opensis.catalogdb.ViewModels;
using opensis.catelogdb.Interface;
using opensis.catelogdb.Models;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.Factory
{
    public class MySQLContextFactory : IDbContextFactory
    {
        private readonly string connectionStringTemplate;

        public string? TenantName { get; set; } 
        public string? ApiKeyValue { get ; set ; }

        private readonly ICatalogDBRepository _catalogDBRepository;
        public MySQLContextFactory(string connectionStringTemplate, ICatalogDBRepository catalogDBRepository)
        {
            this.connectionStringTemplate = connectionStringTemplate;
            _catalogDBRepository = catalogDBRepository;
        }
        
        public CRMContext Create()
        {
            CRMContext? context = null;

            if (!string.IsNullOrWhiteSpace(this.TenantName))
            {
                var tenant = this.TenantName;
                var dbContextOptionsBuilder = new DbContextOptionsBuilder();
                //dbContextOptionsBuilder.UseMySql(this.connectionStringTemplate
                //                       .Replace("{tenant}", this.TenantName));
                dbContextOptionsBuilder.UseMySql(this.connectionStringTemplate.Replace("{tenant}", tenant), ServerVersion.AutoDetect(this.connectionStringTemplate));

                context = new CRMContextMySQL(dbContextOptionsBuilder.Options);

                var response = this._catalogDBRepository.CheckIfTenantIsAvailable(new AvailableTenantViewModel
                {
                    tenant = new AvailableTenants
                    {
                        TenantName = this.TenantName
                    }
                });
                if (!response.Failure)
                {
                    context.Database.Migrate();
                }
            }

            return context;
        }
    }
}
