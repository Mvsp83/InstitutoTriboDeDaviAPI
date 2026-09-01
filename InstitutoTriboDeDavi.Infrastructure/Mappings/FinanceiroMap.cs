using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ContaFinanceiraMap : IEntityTypeConfiguration<ContaFinanceira>
    {
        public void Configure(EntityTypeBuilder<ContaFinanceira> builder)
        {
            builder.ToTable("CONTA_FINANCEIRA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Tipo).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(160);
            builder.Property(x => x.Banco).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.Agencia).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Numero).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Observacoes).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.Ativa).IsRequired();

            builder.Property(x => x.SaldoInicial).HasColumnType("decimal(18,2)");
        }
    }

    public class MovimentacaoFinanceiraMap : IEntityTypeConfiguration<MovimentacaoFinanceira>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoFinanceira> builder)
        {
            builder.ToTable("MOVIMENTACAO_FINANCEIRA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.ContaId).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Descricao).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CategoriaId).IsRequired().HasMaxLength(40);
            builder.Property(x => x.Tipo).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Conciliado).IsRequired();
            builder.Property(x => x.Documento).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.Observacoes).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.TransferenciaId).HasMaxLength(40).IsRequired(false);

            builder.Property(x => x.Valor).HasColumnType("decimal(18,2)");

            // Consultas do portal filtram por conta e ordenam por data.
            builder.HasIndex(x => x.ContaId);
            builder.HasIndex(x => x.Data);
            // Usado para localizar o par de uma transferência.
            builder.HasIndex(x => x.TransferenciaId);
        }
    }
}
