using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peo.ContentManagement.Domain.Entities;
using Peo.Core.Infra.Data.Configurations.Base;

namespace Peo.ContentManagement.Infra.Data.Configurations
{
    internal class CursoEntityConfiguration : EntityBaseConfiguration<Curso>
    {
        public override void Configure(EntityTypeBuilder<Curso> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Titulo)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(e => e.Descricao)
                   .HasMaxLength(1024);

            builder.OwnsOne(c => c.ConteudoProgramatico, pc =>
            {
                pc.Property(p => p.Conteudo).HasMaxLength(1024);
            });

            builder.HasOne(o => o.Instrutor)
                   .WithMany()
                   .HasForeignKey(o => o.InstrutorId);
        }
    }
}