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
                // Used only by EF tooling (dotnet ef migrations add).
                // Create a .env file at the repo root with:
                //   OPENSIIS_MIGRATION_CONNSTR=server=localhost;database=kisis;user=...
                var connectionString = ReadDotEnv("OPENSIIS_MIGRATION_CONNSTR")
                    ?? Environment.GetEnvironmentVariable("OPENSIIS_MIGRATION_CONNSTR");
                if (!string.IsNullOrEmpty(connectionString))
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        private static string? ReadDotEnv(string key)
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var envFile = Path.Combine(dir.FullName, ".env");
                if (File.Exists(envFile))
                {
                    foreach (var line in File.ReadAllLines(envFile))
                    {
                        var trimmed = line.Trim();
                        if (trimmed.StartsWith("#") || !trimmed.Contains('=')) continue;
                        var idx = trimmed.IndexOf('=');
                        if (trimmed[..idx].Trim() == key)
                            return trimmed[(idx + 1)..].Trim();
                    }
                }
                dir = dir.Parent;
            }
            return null;
        }
    }
}
