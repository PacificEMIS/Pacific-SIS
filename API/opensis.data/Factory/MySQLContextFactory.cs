using Microsoft.EntityFrameworkCore;
using opensis.data.Interface;
using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.Factory
{
    public class MySQLContextFactory : IDbContextFactory
    {
        private string connectionStringTemplate;

        public string TenantName { get; set; }

        public MySQLContextFactory(string connectionStringTemplate)
        {
            this.connectionStringTemplate = connectionStringTemplate;
        }

        public CRMContext Create()
        {
            CRMContext context = null;

            if (!string.IsNullOrWhiteSpace(this.TenantName))
            {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder();
                dbContextOptionsBuilder.UseMySql(this.connectionStringTemplate
                                       .Replace("{tenant}", this.TenantName));

                context = new CRMContextMySQL(dbContextOptionsBuilder.Options);
                context.Database.Migrate();
            }

            return context;
        }
    }
}
