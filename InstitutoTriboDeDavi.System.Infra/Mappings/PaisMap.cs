using InstitutoTriboDeDavi.System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.System.Infra.Mappings
{
    public class PaisMap : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.ToTable("PAISES");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("Nome")
                .HasColumnType("VARCHAR(120)");

            builder.Property(x => x.Sigla)
                .IsRequired()
                .HasMaxLength(2)
                .HasColumnName("Sigla")
                .HasColumnType("VARCHAR(2)");
        }
    }
}
