using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class DenunciaRecadoMap : IEntityTypeConfiguration<DenunciaRecado>
    {
        public void Configure(EntityTypeBuilder<DenunciaRecado> builder)
        {
            builder.ToTable("DENUNCIA_RECADO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.RecadoId).IsRequired();
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.Property(x => x.Resolvida).IsRequired();
            builder.Property(x => x.DataResolucao).IsRequired(false);

            builder.Property(x => x.Motivo).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.DenunciadoPor).HasMaxLength(80).IsRequired(false);
            builder.Property(x => x.ResolvidoPor).HasMaxLength(60).IsRequired(false);

            // Fila principal: denúncias pendentes; e busca por recado (na remoção).
            builder.HasIndex(x => x.Resolvida);
            builder.HasIndex(x => x.RecadoId);
        }
    }
}
