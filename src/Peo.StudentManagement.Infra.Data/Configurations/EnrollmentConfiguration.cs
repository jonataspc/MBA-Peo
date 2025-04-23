using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.StudentManagement.Domain.Entities;
using Peo.StudentManagement.Domain.ValueObjects;

namespace Peo.StudentManagement.Infra.Data.Configurations;

public class EnrollmentConfiguration : EntityBaseConfiguration<Enrollment>
{
    public override void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.CourseId)
            .IsRequired();

        builder.Property(e => e.EnrollmentDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EnrollmentStatus)Enum.Parse(typeof(EnrollmentStatus), v))
            .HasDefaultValue(EnrollmentStatus.PendingPayment);

        builder.Property(e => e.ProgressPercentage)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();

        // Relationships
        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}