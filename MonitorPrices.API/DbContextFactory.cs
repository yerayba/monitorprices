using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MonitorPrices.Repository;

namespace MonitorPrices.API
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<MonitorPricedbContext>
    {
        public MonitorPricedbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MonitorPricedbContext>();

            // 🔑 Connection string SOLO para migraciones
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=monitorpricesdb;Username=myuser;Password=mypassword"
            );

            return new MonitorPricedbContext(optionsBuilder.Options);
        }
    }
}