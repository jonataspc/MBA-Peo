using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.ContentManagement.Infra.Data.Configurations
{
    internal class LessonFileEntityConfiguration : EntityBaseConfiguration<LessonFile>
    {
        public override void Configure(EntityTypeBuilder<LessonFile> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Url)
                   .HasMaxLength(1024)
                   .IsRequired();

            builder.HasOne(o => o.Lesson)
                   .WithMany(c => c.Files)
                   .HasForeignKey(o => o.LessonId);
        }
    }
}