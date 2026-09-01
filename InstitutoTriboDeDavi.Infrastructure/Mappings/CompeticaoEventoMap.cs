using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class CompeticaoEventoMap : IEntityTypeConfiguration<CompeticaoEvento>
    {
        public void Configure(EntityTypeBuilder<CompeticaoEvento> builder)
        {
            builder.ToTable("COMPETICAO_EVENTO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");
            builder.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.DataFim).IsRequired(false);
            builder.Property(x => x.Local).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Organizador).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.PrazoInscricao).IsRequired(false);
            builder.Property(x => x.Link).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Observacao).HasMaxLength(2000).IsRequired(false);
            builder.Property(x => x.Status).IsRequired();

            builder.HasMany(x => x.Participacoes).WithOne()
                .HasForeignKey(p => p.CompeticaoEventoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ParticipacaoAtletaMap : IEntityTypeConfiguration<ParticipacaoAtleta>
    {
        public void Configure(EntityTypeBuilder<ParticipacaoAtleta> builder)
        {
            builder.ToTable("COMPETICAO_PARTICIPACAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");
            builder.Property(x => x.CompeticaoEventoId).IsRequired();
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.CategoriaPeso).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.Colocacao).IsRequired();
            builder.Property(x => x.Lutas).IsRequired();
            builder.Property(x => x.Vitorias).IsRequired();
            builder.Property(x => x.Finalizacoes).IsRequired();
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);
            builder.HasIndex(x => x.CompeticaoEventoId);
            builder.HasIndex(x => x.AtletaId);
        }
    }
}
