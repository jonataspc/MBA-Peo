using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Infra.Data.Configurations;

public class EnrollmentProgressConfiguration : EntityBaseConfiguration<EnrollmentProgress>
{
    public override void Configure(EntityTypeBuilder<EnrollmentProgress> builder)
    {
        base.Configure(builder);

        builder.Property(ep => ep.EnrollmentId)
            .IsRequired();

        builder.Property(ep => ep.LessonId)
            .IsRequired();

        builder.Property(ep => ep.StartedAt)
            .IsRequired();

        builder.Property(ep => ep.CompletedAt)
            .IsRequired(false);

        builder.HasIndex(ep => new { ep.EnrollmentId, ep.LessonId })
            .IsUnique();

        builder.HasOne<Enrollment>()
            .WithMany()
            .HasForeignKey(ep => ep.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}