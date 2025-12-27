
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MonitorPrices.Repository;

namespace MonitorPrices.API
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MonitorPricedbContext>
    {
        public MonitorPricedbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("La variable de entorno ConnectionStrings__DefaultConnection no está definida.");

            var optionsBuilder = new DbContextOptionsBuilder<MonitorPricedbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new MonitorPricedbContext(optionsBuilder.Options);
        }
    }
}