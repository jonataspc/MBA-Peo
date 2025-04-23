using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Peo.Core.Entities.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.Core.Interfaces.Data;
using Peo.Identity.Domain.Entities;
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
            // fix precision for decimal data types
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(12, 2)");
            }

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.RemovePluralizingTableNameConvention();

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