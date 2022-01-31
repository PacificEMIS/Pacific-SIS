using Microsoft.EntityFrameworkCore;
using opensis.catalogdb.Interface;
using opensis.catalogdb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.catalogdb.Factory
{
    public class CatalogDBContextFactory : ICatalogDBContextFactory
    {
        private readonly string connectionStringTemplate;

        public string TenantName { get; set; }

       

        public CatalogDBContextFactory(string connectionStringTemplate)
        {
            this.connectionStringTemplate = connectionStringTemplate;
          
        }

        public CatalogDBContext Create()
        {
            CatalogDBContext context = null;
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
               
            dbContextOptionsBuilder.UseSqlServer(this.connectionStringTemplate);

            context = new CatalogDBContext(dbContextOptionsBuilder.Options);

            return context;
        }


    }
}
