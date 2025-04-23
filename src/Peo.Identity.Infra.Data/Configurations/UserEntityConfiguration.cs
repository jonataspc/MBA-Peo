using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.Core.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.Identity.Infra.Data.Configurations
{
    internal class UserEntityConfiguration : EntityBaseConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.FullName)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Email)
                   .HasMaxLength(256)
                   .IsRequired();
        }
    }
}