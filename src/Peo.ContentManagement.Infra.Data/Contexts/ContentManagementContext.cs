using Microsoft.EntityFrameworkCore;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using System.Reflection;

namespace Peo.ContentManagement.Infra.Data.Contexts
{
    public class ContentManagementContext : DbContextBase
    {
        public DbSet<Curso> Cursos { get; set; } = null!;

        public DbSet<Aula> Aulas { get; set; } = null!;

        public ContentManagementContext(DbContextOptions<ContentManagementContext> options) : base(options)
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
}