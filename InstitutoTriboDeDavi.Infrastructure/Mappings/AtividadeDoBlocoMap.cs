using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class AtividadeDoBlocoMap : IEntityTypeConfiguration<AtividadeDoBloco>
    {
        public void Configure(EntityTypeBuilder<AtividadeDoBloco> builder)
        {
            builder.ToTable("ATIVIDADES_DO_BLOCO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.HasOne<Atividade>()
                .WithMany()
                .HasForeignKey(x => x.AtividadeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.AtividadeId);
        }
    }
}
