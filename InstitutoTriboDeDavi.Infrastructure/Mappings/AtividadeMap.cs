using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class AtividadeMap : IEntityTypeConfiguration<Atividade>
    {
        public void Configure(EntityTypeBuilder<Atividade> builder)
        {
            builder.ToTable("ATIVIDADES");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Descricao)
                .IsRequired(false);

            builder.Property(x => x.Tags)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(x => x.Principio)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(x => x.ReferenciaBiblica)
                .IsRequired(false)
                .HasMaxLength(120);

            builder.Property(x => x.VideoUrl)
                .IsRequired(false)
                .HasMaxLength(300);
        }
    }
}
