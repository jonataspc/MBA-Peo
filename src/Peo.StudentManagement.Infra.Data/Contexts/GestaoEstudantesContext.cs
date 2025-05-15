using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.StudentManagement.Domain.Entities;
using System.Reflection;

namespace Peo.StudentManagement.Infra.Data.Contexts
{
    public class GestaoEstudantesContext : DbContextBase
    {
        public DbSet<Estudante> Estudantes { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;
        public DbSet<ProgressoMatricula> ProgressosMatricula { get; set; } = null!;
        public DbSet<Certificado> Certificados { get; set; } = null!;

        public GestaoEstudantesContext(DbContextOptions<GestaoEstudantesContext> options) : base(options)
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