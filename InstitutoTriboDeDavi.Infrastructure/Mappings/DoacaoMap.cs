using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class DoadorMap : IEntityTypeConfiguration<Doador>
    {
        public void Configure(EntityTypeBuilder<Doador> builder)
        {
            builder.ToTable("DOADOR");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.TipoPessoa).IsRequired();
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(160);
            builder.Property(x => x.Documento).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.Email).HasMaxLength(160).IsRequired(false);
            builder.Property(x => x.Telefone).HasMaxLength(30).IsRequired(false);
            builder.Property(x => x.Endereco).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Cidade).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.Observacoes).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.Ativo).IsRequired();

            builder.HasIndex(x => x.Nome);
        }
    }

    public class DoacaoMap : IEntityTypeConfiguration<Doacao>
    {
        public void Configure(EntityTypeBuilder<Doacao> builder)
        {
            builder.ToTable("DOACAO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.DoadorId).IsRequired(false);
            builder.Property(x => x.Valor).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.Forma).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Finalidade).HasMaxLength(160).IsRequired(false);
            builder.Property(x => x.Observacoes).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.ReciboDocumentoId).IsRequired(false);
            builder.Property(x => x.ReciboNumero).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.RegistradoPor).HasMaxLength(120).IsRequired(false);

            builder.HasIndex(x => x.Data);
            builder.HasIndex(x => x.DoadorId);
        }
    }
}
