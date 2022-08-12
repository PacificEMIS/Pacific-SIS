using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace opensis.data.Models
{
    public class CRMContextMySQL : CRMContext
    {
        private readonly DbContextOptions? contextOptions;
        public CRMContextMySQL() { }
        public CRMContextMySQL(DbContextOptions options) : base(options)
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
