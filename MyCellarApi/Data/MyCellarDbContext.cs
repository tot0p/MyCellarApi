using Microsoft.EntityFrameworkCore;
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
    }
}
