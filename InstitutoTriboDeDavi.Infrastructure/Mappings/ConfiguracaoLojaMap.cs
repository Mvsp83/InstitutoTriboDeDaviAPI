using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ConfiguracaoLojaMap : IEntityTypeConfiguration<ConfiguracaoLoja>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoLoja> builder)
        {
            builder.ToTable("CONFIGURACAO_LOJA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.CompraWhatsappHabilitada);

            builder.Property(x => x.WhatsappNumero)
                .HasMaxLength(20)
                .IsRequired(false);
        }
    }
}
