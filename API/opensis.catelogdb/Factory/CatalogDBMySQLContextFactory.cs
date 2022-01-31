using Microsoft.EntityFrameworkCore;
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.catalogdb.Factory
{
    public class CatalogDBMySQLContextFactory : ICatalogDBContextFactory
    {
        private readonly string connectionStringTemplate;

        public string TenantName { get; set; }

        public CatalogDBMySQLContextFactory(string connectionStringTemplate)
        {
            this.connectionStringTemplate = connectionStringTemplate;
        }

        public CatalogDBContext Create()
        {
            CatalogDBContext context = null;

            
                var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            //dbContextOptionsBuilder.UseMySql(this.connectionStringTemplate
            //                       );
            dbContextOptionsBuilder.UseMySql(this.connectionStringTemplate, ServerVersion.AutoDetect(this.connectionStringTemplate));

            context = new CatalogDBContextMySQL(dbContextOptionsBuilder.Options);
           

            return context;
        }
    }
}
