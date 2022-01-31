using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.catelogdb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.catalogdb.Models
{
    public class CatalogDBContext : DbContext
    {
        private DbContextOptions contextOptions;


        public CatalogDBContext() : base()
        {
        }

        public CatalogDBContext(DbContextOptions options) : base(options)
        {
            this.contextOptions = options;
        }

        public virtual DbSet<AvailableTenants> AvailableTenants { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string[] tenants = new string[] { "TenantA" };
                string connectionString = "Server=SABYA\\SQLEXPRESS;Database={tenant};User Id=sa; Password=admin@123;MultipleActiveResultSets=true";
                //string connectionString = "Server=DESKTOP-40434IR\\SQLEXPRESS;Database={tenant};User Id=sa; Password=os4ed;MultipleActiveResultSets=true";
                optionsBuilder.UseSqlServer(connectionString.Replace("{tenant}", "opensisv2_dev"));

                //foreach (string tenant in tenants)
                //{
                //    optionsBuilder.UseSqlServer(connectionString.Replace("{tenant}", "TenantA"));
                //}
            }
        }
    }
}
