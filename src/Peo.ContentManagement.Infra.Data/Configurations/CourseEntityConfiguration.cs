using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.ContentManagement.Infra.Data.Configurations
{
    internal class CourseEntityConfiguration : EntityBaseConfiguration<Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(1024);

            builder.OwnsOne(c => c.ProgramContent, pc =>
            {
                pc.Property(p => p.Content).HasMaxLength(1024);
            });

            builder.HasOne(o => o.Instructor)
                   .WithMany()
                   .HasForeignKey(o => o.InstructorId);
        }
    }
}