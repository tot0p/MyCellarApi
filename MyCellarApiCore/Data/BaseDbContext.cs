using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyCellarApiCore.Models;

namespace MyCellarApiCore.Data
{
    public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
    {

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deleteEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Entity is BaseModel);

            var updateEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified && e.Entity is BaseModel);

            foreach (var entity in deleteEntities)
            {
                entity.State = EntityState.Modified;
                entity.CurrentValues["Deleted"] = true;
            }

            foreach (var entity in updateEntities)
            {
                entity.CurrentValues["UpdateAt"] = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
