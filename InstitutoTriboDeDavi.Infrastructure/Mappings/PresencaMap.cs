using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class PresencaMap : IEntityTypeConfiguration<Presenca>
    {
        public void Configure(EntityTypeBuilder<Presenca> builder)
        {
            builder.ToTable("PRESENCAS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            // Justificativa é opcional: null enquanto a falta não é justificada
            // (é o sinal de "não justificada", junto com JustificadaEm nula).
            builder.Property(x => x.JustificativaResponsavel)
                .IsRequired(false);
        }
    }
}
