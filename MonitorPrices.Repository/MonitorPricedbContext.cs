using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;

namespace MonitorPrices.Repository;

public class MonitorPricedbContext : DbContext
{
    public MonitorPricedbContext(DbContextOptions<MonitorPricedbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}



