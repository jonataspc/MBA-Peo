using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.Billing.Domain.Entities;
using System.Reflection;

namespace Peo.Billing.Infra.Data.Contexts;

public class BillingContext : DbContextBase
{
    public DbSet<Payment> Payments { get; set; }

    public BillingContext(DbContextOptions<BillingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.FixPrecisionForDecimalDataTypes()
                   .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                   .RemovePluralizingTableNameConvention();

        base.OnModelCreating(modelBuilder);
    }
} 