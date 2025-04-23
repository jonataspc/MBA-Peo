using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Infra.Data.Configurations.Base;
using Peo.StudentManagement.Domain.Entities;

namespace Peo.StudentManagement.Infra.Data.Configurations;

public class CertificateConfiguration : EntityBaseConfiguration<Certificate>
{
    public override void Configure(EntityTypeBuilder<Certificate> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.EnrollmentId)
            .IsRequired();

        builder.Property(c => c.Content)
            .IsRequired();

        builder.Property(c => c.IssueDate)
            .IsRequired(false);

        builder.Property(c => c.CertificateNumber)
            .IsRequired(false)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(c => c.EnrollmentId)
            .IsUnique();

        builder.HasIndex(c => c.CertificateNumber)
            .IsUnique();

        // Relationships
        builder.HasOne<Enrollment>()
            .WithOne()
            .HasForeignKey<Certificate>(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}