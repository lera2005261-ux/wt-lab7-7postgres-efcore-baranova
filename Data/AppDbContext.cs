using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_baranova.Models;

namespace wt_lab7_7postgres_efcore_baranova.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
}