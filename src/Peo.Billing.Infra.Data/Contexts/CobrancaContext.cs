using Microsoft.EntityFrameworkCore;
using Peo.Billing.Domain.Entities;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using System.Reflection;

namespace Peo.Billing.Infra.Data.Contexts;

public class CobrancaContext : DbContextBase
{
    public DbSet<Pagamento> Pagamentos { get; set; }

    public CobrancaContext(DbContextOptions<CobrancaContext> options) : base(options)
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