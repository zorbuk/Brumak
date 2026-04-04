using Brumak_ORM.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Factory
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("db_settings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("AuthDb")
                ?? throw new InvalidOperationException("Connection string 'AuthDb' not found");

            var serverVersion = ServerVersion.AutoDetect(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
