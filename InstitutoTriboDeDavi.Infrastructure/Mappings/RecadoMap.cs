using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class RecadoMap : IEntityTypeConfiguration<Recado>
    {
        public void Configure(EntityTypeBuilder<Recado> builder)
        {
            builder.ToTable("RECADO");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Categoria).IsRequired();
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.Property(x => x.ExpiraEm).IsRequired();
            builder.Property(x => x.Ativo).IsRequired();
            builder.Property(x => x.PoloId).IsRequired(false);

            builder.Property(x => x.Titulo).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Descricao).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.Anunciante).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.Contato).IsRequired().HasMaxLength(120);
            builder.Property(x => x.CriadoPor).HasMaxLength(60).IsRequired(false);

            // Consulta principal do mural: vigentes (ativo + não expirado), recentes.
            builder.HasIndex(x => new { x.Ativo, x.ExpiraEm });
        }
    }
}
