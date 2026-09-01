using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class SolicitacaoInternaMap : IEntityTypeConfiguration<SolicitacaoInterna>
    {
        public void Configure(EntityTypeBuilder<SolicitacaoInterna> builder)
        {
            builder.ToTable("SOLICITACAO_INTERNA");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Assunto).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Categoria).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.PoloNome).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.CriadoPorLogin).IsRequired().HasMaxLength(60);
            builder.Property(x => x.CriadoPorRole).IsRequired();
            builder.Property(x => x.DestinatarioLogin).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.DataCriacao).IsRequired();
            builder.Property(x => x.DataAtualizacao).IsRequired();
            builder.Property(x => x.Ativo).IsRequired();

            // Consultas típicas: por autor e por destinatário.
            builder.HasIndex(x => x.CriadoPorLogin);
            builder.HasIndex(x => x.DestinatarioLogin);

            // A conversa: apagar a solicitação apaga as mensagens.
            builder.HasMany(x => x.Mensagens)
                .WithOne()
                .HasForeignKey(m => m.SolicitacaoInternaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class MensagemSolicitacaoMap : IEntityTypeConfiguration<MensagemSolicitacao>
    {
        public void Configure(EntityTypeBuilder<MensagemSolicitacao> builder)
        {
            builder.ToTable("SOLICITACAO_MENSAGEM");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.SolicitacaoInternaId).IsRequired();
            builder.Property(x => x.AutorLogin).IsRequired().HasMaxLength(60);
            builder.Property(x => x.AutorRole).IsRequired();
            builder.Property(x => x.Texto).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.DataEnvio).IsRequired();

            builder.HasIndex(x => x.SolicitacaoInternaId);
        }
    }
}
