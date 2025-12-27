using Microsoft.EntityFrameworkCore;
using MonitorPrices.Repository.Entities;

namespace MonitorPrices.Repository;

public class MonitorPricedbContext : DbContext
{
    public MonitorPricedbContext(DbContextOptions<MonitorPricedbContext> options) : base(options) { }

    public DbSet<Product> Productos { get; set; }
}



