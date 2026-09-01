using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class RelatorioSalvoMap : IEntityTypeConfiguration<RelatorioSalvo>
    {
        public void Configure(EntityTypeBuilder<RelatorioSalvo> builder)
        {
            builder.ToTable("RELATORIOS_SALVOS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.UsuarioLogin)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.FonteId)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(x => x.Colunas)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Turma)
                .IsRequired(false);

            builder.Property(x => x.PoloId)
                .IsRequired(false);

            builder.HasIndex(x => x.UsuarioLogin);
        }
    }
}
