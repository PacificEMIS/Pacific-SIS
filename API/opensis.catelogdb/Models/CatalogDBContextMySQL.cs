using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace opensis.catalogdb.Models
{
    public class CatalogDBContextMySQL : CatalogDBContext
    {
        private readonly DbContextOptions contextOptions;
        public CatalogDBContextMySQL() { }
        public CatalogDBContextMySQL(DbContextOptions options) : base(options)
        {
            this.contextOptions = options;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var tenant = "opensisv2new";
                string connectionString = $"server=localhost;database={tenant};user=root;password=xxxxx";
                //optionsBuilder.UseMySql(connectionString.Replace("{tenant}", "opensisv2new"));
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                /* ********* bob server*********
                 * 
                 string connectionString = "server=xxxx;port=3306;database={tenant};user=xxxx;password=xxxxxxxxxxxxxx";
                 optionsBuilder.UseMySql(connectionString.Replace("{tenant}", "opensisv2"));*/

                //string connectionString = "server=103.230.103.93;port=3306;database={tenant};user=admin;password=xxxxxxxxxxxxxx";
                //optionsBuilder.UseMySql(connectionString.Replace("{tenant}", "opensisv2_test1"));

            }

        }
    }
}
