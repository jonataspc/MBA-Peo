using Microsoft.EntityFrameworkCore;
using Peo.Core.Infra.Data.Contexts.Base;
using Peo.Core.Infra.Data.Extensions;
using Peo.StudentManagement.Domain.Entities;
using System.Reflection;

namespace Peo.StudentManagement.Infra.Data.Contexts
{
    public class StudentManagementContext : DbContextBase
    {
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<EnrollmentProgress> EnrollmentProgresses { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Student> Students { get; set; }

        public StudentManagementContext(DbContextOptions<StudentManagementContext> options) : base(options)
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