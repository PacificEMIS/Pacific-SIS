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

            }

        }
    }
}
