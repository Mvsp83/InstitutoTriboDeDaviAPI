using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class BlocoDoPlanoMap : IEntityTypeConfiguration<BlocoDoPlano>
    {
        public void Configure(EntityTypeBuilder<BlocoDoPlano> builder)
        {
            builder.ToTable("BLOCOS_DO_PLANO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(x => x.Descricao)
                .IsRequired(false);

            builder.HasMany(x => x.Atividades)
                .WithOne()
                .HasForeignKey(x => x.BlocoDoPlanoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
