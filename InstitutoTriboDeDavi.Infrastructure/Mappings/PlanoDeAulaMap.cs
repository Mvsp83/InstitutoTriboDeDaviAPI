using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class PlanoDeAulaMap : IEntityTypeConfiguration<PlanoDeAula>
    {
        public void Configure(EntityTypeBuilder<PlanoDeAula> builder)
        {
            builder.ToTable("PLANOS_DE_AULA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Objetivo)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.HasMany(x => x.Blocos)
                .WithOne()
                .HasForeignKey(x => x.PlanoDeAulaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.PoloId, x.Turma, x.DataPrevista });
        }
    }
}
