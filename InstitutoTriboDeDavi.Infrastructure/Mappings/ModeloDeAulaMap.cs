using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ModeloDeAulaMap : IEntityTypeConfiguration<ModeloDeAula>
    {
        public void Configure(EntityTypeBuilder<ModeloDeAula> builder)
        {
            builder.ToTable("MODELOS_DE_AULA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Descricao)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasMany(x => x.Blocos)
                .WithOne()
                .HasForeignKey(x => x.ModeloDeAulaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
