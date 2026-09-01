using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class GraduacaoMap : IEntityTypeConfiguration<Graduacao>
    {
        public void Configure(EntityTypeBuilder<Graduacao> builder)
        {
            builder.ToTable("GRADUACAO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.FaixaAnterior).IsRequired();
            builder.Property(x => x.FaixaNova).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Observacao).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.RegistradoPor).HasMaxLength(120).IsRequired(false);

            // O histórico é consultado por aluno e a lista, por data/polo.
            builder.HasIndex(x => x.AlunoId);
            builder.HasIndex(x => new { x.Data, x.PoloId });
        }
    }
}
