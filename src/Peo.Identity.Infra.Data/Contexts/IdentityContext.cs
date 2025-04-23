using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Peo.Core.Entities;
using Peo.Core.Entities.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.Core.Interfaces.Data;
using System.Reflection;

namespace Peo.Identity.Infra.Data.Contexts
{
    public class IdentityContext : IdentityDbContext<IdentityUser>, IUnitOfWork
    {
        public DbSet<User> ApplicationUsers { get; set; }

        public IdentityContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.FixPrecionDecimalDataTypes()
                   .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                   .RemovePluralizingTableNameConvention();

            base.OnModelCreating(builder);
        }

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

        public Task<int> CommitAsync()
        {
            return this.SaveChangesAsync();
        }
    }
}