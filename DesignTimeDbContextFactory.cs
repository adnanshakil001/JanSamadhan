using JanSamadhan.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JanSamadhan
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var cs = config.GetConnectionString("DefaultConnection")
                ?? "Data Source=JanSamadhan.db";
            optionsBuilder.UseSqlite(cs);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}


