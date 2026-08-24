using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ConfiguracaoDocumentoMap : IEntityTypeConfiguration<ConfiguracaoDocumento>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoDocumento> builder)
        {
            builder.ToTable("CONFIGURACAO_DOCUMENTO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.TituloCabecalho)
                .HasMaxLength(160)
                .IsRequired(false);

            builder.Property(x => x.LinhaExtra)
                .HasMaxLength(240)
                .IsRequired(false);

            builder.Property(x => x.TextoRodape)
                .HasMaxLength(240)
                .IsRequired(false);

            builder.Property(x => x.MostrarLogo);
            builder.Property(x => x.MostrarDataGeracao);

            builder.Property(x => x.TextosPadraoJson)
                .IsRequired(false); // nvarchar(max)
        }
    }
}
