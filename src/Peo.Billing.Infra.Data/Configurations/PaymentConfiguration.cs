using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Billing.Domain.Entities;
using Peo.Billing.Domain.ValueObjects;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.Billing.Infra.Data.Configurations;

public class PaymentConfiguration : EntityBaseConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.EnrollmentId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Details)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(p => p.PaymentDate)
            .IsRequired(false);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v))
            .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(p => p.TransactionId)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.OwnsOne(p => p.CreditCardData, creditCard =>
        {
            creditCard.Property(c => c.Hash)
                .IsRequired(false)
                .HasMaxLength(2048);
        });

        // Indexes
        builder.HasIndex(p => p.EnrollmentId);
    }
}