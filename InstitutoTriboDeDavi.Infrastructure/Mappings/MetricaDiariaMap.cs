using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class MetricaDiariaMap : IEntityTypeConfiguration<MetricaDiaria>
    {
        public void Configure(EntityTypeBuilder<MetricaDiaria> builder)
        {
            builder.ToTable("METRICA_DIARIA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Data)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(x => x.Chave)
                .IsRequired()
                .HasMaxLength(160)
                .HasColumnType("VARCHAR(160)");

            builder.Property(x => x.Valor).IsRequired();

            // Uma linha por (dia, chave) — base do upsert-incremento.
            builder.HasIndex(x => new { x.Data, x.Chave }).IsUnique();
        }
    }
}
