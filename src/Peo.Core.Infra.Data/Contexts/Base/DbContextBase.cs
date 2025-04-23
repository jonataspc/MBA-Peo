using Microsoft.EntityFrameworkCore;
using Peo.Core.Entities.Base;
using Peo.Core.Interfaces.Data;

namespace Peo.Core.Infra.Data.Contexts.Base
{
    //TODO: requerida mesmo?
    public abstract class DbContextBase : DbContext, IUnitOfWork

    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateModifiedAt();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateModifiedAt();
            return base.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await SaveChangesAsync().ConfigureAwait(false);
        }

        private void UpdateModifiedAt()
        {
            var entries = ChangeTracker
                            .Entries()
                            .Where(e => e.Entity is EntityBase && e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                ((EntityBase)entityEntry.Entity).ModifiedAt = DateTime.Now;
            }
        }
    }
}