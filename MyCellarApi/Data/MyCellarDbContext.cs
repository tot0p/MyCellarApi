using Microsoft.EntityFrameworkCore;
using MyCellarApi.Models;
using MyCellarApiCore.Data;

namespace MyCellarApi.Data
{
    public class MyCellarDbContext(DbContextOptions options) : BaseDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("MyCellar");
        }
        public DbSet<WineBottle> WineBottles { get; set; }
        public DbSet<Cellar> Cellars { get; set; }
        public DbSet<StockInformation> StockInformations { get; set; }

    }
}
