using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class MembroGovernancaMap : IEntityTypeConfiguration<MembroGovernanca>
    {
        public void Configure(EntityTypeBuilder<MembroGovernanca> builder)
        {
            builder.ToTable("MEMBRO_GOVERNANCA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Ano).IsRequired();
            builder.Property(x => x.Orgao).IsRequired();
            builder.Property(x => x.Cargo).IsRequired().HasMaxLength(60);
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Ordem).IsRequired();

            builder.HasIndex(x => x.Ano);
        }
    }
}
