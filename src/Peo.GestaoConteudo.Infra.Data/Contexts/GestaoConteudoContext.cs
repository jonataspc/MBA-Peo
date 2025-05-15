using Microsoft.EntityFrameworkCore;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using System.Reflection;

namespace Peo.GestaoConteudo.Infra.Data.Contexts
{
    public class GestaoConteudoContext : DbContextBase
    {
        public DbSet<Curso> Cursos { get; set; } = null!;

        public DbSet<Aula> Aulas { get; set; } = null!;

        public GestaoConteudoContext(DbContextOptions<GestaoConteudoContext> options) : base(options)
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