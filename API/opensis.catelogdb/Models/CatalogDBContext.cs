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
				
            }
        }
    }
}
