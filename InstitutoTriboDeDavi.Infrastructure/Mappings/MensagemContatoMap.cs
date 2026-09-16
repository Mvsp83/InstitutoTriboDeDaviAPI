using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class MensagemContatoMap : IEntityTypeConfiguration<MensagemContato>
    {
        public void Configure(EntityTypeBuilder<MensagemContato> builder)
        {
            builder.ToTable("MENSAGEM_CONTATO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Nome).IsRequired().HasMaxLength(160);
            builder.Property(x => x.Email).HasMaxLength(160).IsRequired(false);
            builder.Property(x => x.Telefone).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Assunto).HasMaxLength(160).IsRequired(false);
            builder.Property(x => x.Mensagem).IsRequired().HasMaxLength(4000);
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.Property(x => x.Lida).IsRequired();

            builder.HasIndex(x => x.DataCriacao);
        }
    }
}
