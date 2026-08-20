using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class DocumentoOficialMap : IEntityTypeConfiguration<DocumentoOficial>
    {
        public void Configure(EntityTypeBuilder<DocumentoOficial> builder)
        {
            builder.ToTable("DOCUMENTO_OFICIAL");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Ano).IsRequired();
            builder.Property(x => x.Numero).IsRequired();
            builder.Property(x => x.DataDocumento).IsRequired();
            builder.Property(x => x.DataAprovacao).IsRequired(false);

            builder.Property(x => x.NumeroFormatado)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Conteudo)
                .IsRequired(false); // JSON → nvarchar(max)

            // Rede de segurança: número oficial único por tipo/ano entre os
            // aprovados (rascunhos têm Numero = 0 e ficam de fora do filtro).
            builder.HasIndex(x => new { x.Tipo, x.Ano, x.Numero })
                .IsUnique()
                .HasFilter("[Status] = 1");
        }
    }
}
