using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class DocumentoArquivoMap : IEntityTypeConfiguration<DocumentoArquivo>
    {
        public void Configure(EntityTypeBuilder<DocumentoArquivo> builder)
        {
            builder.ToTable("DOCUMENTO_ARQUIVO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome).HasMaxLength(300).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Conteudo).HasColumnType("varbinary(max)").IsRequired();

            builder.HasIndex(x => x.Categoria);
        }
    }
}
