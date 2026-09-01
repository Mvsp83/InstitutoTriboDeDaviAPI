using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("PRODUTO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Descricao).HasMaxLength(2000).IsRequired(false);
            builder.Property(x => x.Preco).HasColumnType("decimal(10,2)");
            builder.Property(x => x.FotoArquivoId).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.FormasPagamento).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Informacoes).HasMaxLength(2000).IsRequired(false);
            builder.Property(x => x.Ativo).IsRequired();
            builder.Property(x => x.DataCriacao).IsRequired();

            // Variações: apagar o produto apaga as variações.
            builder.HasMany(x => x.Variacoes)
                .WithOne()
                .HasForeignKey(v => v.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class VariacaoProdutoMap : IEntityTypeConfiguration<VariacaoProduto>
    {
        public void Configure(EntityTypeBuilder<VariacaoProduto> builder)
        {
            builder.ToTable("PRODUTO_VARIACAO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");
            builder.Property(x => x.ProdutoId).IsRequired();
            builder.Property(x => x.Tamanho).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Cor).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Quantidade).IsRequired();
            builder.HasIndex(x => x.ProdutoId);
        }
    }
}
