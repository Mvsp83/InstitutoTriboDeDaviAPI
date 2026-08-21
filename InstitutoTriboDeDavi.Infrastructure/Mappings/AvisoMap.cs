using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class AvisoMap : IEntityTypeConfiguration<Aviso>
    {
        public void Configure(EntityTypeBuilder<Aviso> builder)
        {
            builder.ToTable("AVISO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.PublicoAlvo).IsRequired();
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.Property(x => x.Ativo).IsRequired();
            builder.Property(x => x.Titulo).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.Mensagem).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.CriadoPor).HasMaxLength(60).IsRequired(false);
        }
    }

    public class AvisoCienteMap : IEntityTypeConfiguration<AvisoCiente>
    {
        public void Configure(EntityTypeBuilder<AvisoCiente> builder)
        {
            builder.ToTable("AVISO_CIENTE");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AvisoId).IsRequired();
            builder.Property(x => x.DataCiente).IsRequired();
            builder.Property(x => x.UsuarioLogin).IsRequired().HasMaxLength(60);
            // Um "ciente" por usuário/aviso.
            builder.HasIndex(x => new { x.AvisoId, x.UsuarioLogin }).IsUnique();
        }
    }
}
