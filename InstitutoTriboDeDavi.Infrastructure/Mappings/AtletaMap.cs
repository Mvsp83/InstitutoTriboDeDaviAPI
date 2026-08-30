using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class AtletaMap : IEntityTypeConfiguration<Atleta>
    {
        public void Configure(EntityTypeBuilder<Atleta> builder)
        {
            builder.ToTable("ATLETA");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.CategoriaPeso).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.Objetivo).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.DataInclusao).IsRequired();
            builder.Property(x => x.Ativo).IsRequired();
            builder.HasIndex(x => x.AlunoId).IsUnique();

            builder.HasMany(x => x.Avaliacoes).WithOne()
                .HasForeignKey(x => x.AtletaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Competicoes).WithOne()
                .HasForeignKey(x => x.AtletaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Anotacoes).WithOne()
                .HasForeignKey(x => x.AtletaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Metas).WithOne()
                .HasForeignKey(x => x.AtletaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Lesoes).WithOne()
                .HasForeignKey(x => x.AtletaId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class LesaoMap : IEntityTypeConfiguration<Lesao>
    {
        public void Configure(EntityTypeBuilder<Lesao> builder)
        {
            builder.ToTable("ATLETA_LESAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Descricao).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Local).HasMaxLength(80).IsRequired(false);
            builder.Property(x => x.Gravidade).IsRequired();
            builder.Property(x => x.DataRetorno).IsRequired(false);
            builder.Property(x => x.Recuperado).IsRequired();
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);
            builder.HasIndex(x => x.AtletaId);
        }
    }

    public class AvaliacaoFisicaMap : IEntityTypeConfiguration<AvaliacaoFisica>
    {
        public void Configure(EntityTypeBuilder<AvaliacaoFisica> builder)
        {
            builder.ToTable("ATLETA_AVALIACAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);
            builder.HasIndex(x => x.AtletaId);

            builder.HasMany(x => x.Indicadores).WithOne()
                .HasForeignKey(x => x.AvaliacaoFisicaId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class IndicadorAvaliacaoMap : IEntityTypeConfiguration<IndicadorAvaliacao>
    {
        public void Configure(EntityTypeBuilder<IndicadorAvaliacao> builder)
        {
            builder.ToTable("ATLETA_AVALIACAO_INDICADOR");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AvaliacaoFisicaId).IsRequired();
            builder.Property(x => x.Nome).HasMaxLength(80).IsRequired();
            builder.Property(x => x.Valor).HasColumnType("decimal(10,2)");
            builder.Property(x => x.Unidade).HasMaxLength(20).IsRequired(false);
            builder.HasIndex(x => x.AvaliacaoFisicaId);
        }
    }

    public class CompeticaoMap : IEntityTypeConfiguration<Competicao>
    {
        public void Configure(EntityTypeBuilder<Competicao> builder)
        {
            builder.ToTable("ATLETA_COMPETICAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Evento).HasMaxLength(150).IsRequired();
            builder.Property(x => x.CategoriaPeso).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);
            builder.HasIndex(x => x.AtletaId);
        }
    }

    public class AnotacaoAtletaMap : IEntityTypeConfiguration<AnotacaoAtleta>
    {
        public void Configure(EntityTypeBuilder<AnotacaoAtleta> builder)
        {
            builder.ToTable("ATLETA_ANOTACAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Texto).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Autor).HasMaxLength(60).IsRequired(false);
            builder.HasIndex(x => x.AtletaId);
        }
    }

    public class MetaAtletaMap : IEntityTypeConfiguration<MetaAtleta>
    {
        public void Configure(EntityTypeBuilder<MetaAtleta> builder)
        {
            builder.ToTable("ATLETA_META");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");
            builder.Property(x => x.AtletaId).IsRequired();
            builder.Property(x => x.Descricao).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Prazo).IsRequired(false);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.DataConclusao).IsRequired(false);
            builder.HasIndex(x => x.AtletaId);
        }
    }
}
