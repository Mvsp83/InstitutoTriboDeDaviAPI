using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class BlocoDoModeloMap : IEntityTypeConfiguration<BlocoDoModelo>
    {
        public void Configure(EntityTypeBuilder<BlocoDoModelo> builder)
        {
            builder.ToTable("BLOCOS_DO_MODELO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(x => x.Descricao)
                .IsRequired(false);
        }
    }
}
