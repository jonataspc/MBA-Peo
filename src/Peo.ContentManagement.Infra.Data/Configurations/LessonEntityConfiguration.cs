using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.ContentManagement.Infra.Data.Configurations
{
    internal class LessonEntityConfiguration : EntityBaseConfiguration<Lesson>
    {
        public override void Configure(EntityTypeBuilder<Lesson> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(1024);

            builder.Property(e => e.VideoUrl)
                   .HasMaxLength(1024);

            builder.HasOne(o => o.Course)
                   .WithMany(c => c.Lessons)
                   .HasForeignKey(o => o.CourseId);
        }
    }
}