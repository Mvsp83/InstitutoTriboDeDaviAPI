using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class BemPatrimonialMap : IEntityTypeConfiguration<BemPatrimonial>
    {
        public void Configure(EntityTypeBuilder<BemPatrimonial> builder)
        {
            builder.ToTable("BEM_PATRIMONIAL");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Categoria).IsRequired();
            builder.Property(x => x.Quantidade).IsRequired();
            builder.Property(x => x.Estado).IsRequired();
            builder.Property(x => x.PoloId).IsRequired(false);
            builder.Property(x => x.DataAquisicao).IsRequired(false);

            builder.Property(x => x.ValorUnitario)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NumeroPatrimonio)
                .HasMaxLength(60)
                .IsRequired(false);

            builder.Property(x => x.Observacoes)
                .HasMaxLength(1000)
                .IsRequired(false);
        }
    }
}
