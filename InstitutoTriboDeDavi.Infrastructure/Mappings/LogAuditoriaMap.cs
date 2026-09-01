using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class LogAuditoriaMap : IEntityTypeConfiguration<LogAuditoria>
    {
        public void Configure(EntityTypeBuilder<LogAuditoria> builder)
        {
            builder.ToTable("LOG_AUDITORIA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.UsuarioLogin).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.Acao).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Entidade).HasMaxLength(60).IsRequired();
            builder.Property(x => x.EntidadeId).IsRequired();
            builder.Property(x => x.Resumo).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Alteracoes).IsRequired(false); // JSON -> nvarchar(max)
            builder.Property(x => x.Ip).HasMaxLength(60).IsRequired(false);

            // A tela filtra por data (mais recentes) e por entidade.
            builder.HasIndex(x => x.Data);
            builder.HasIndex(x => new { x.Entidade, x.EntidadeId });
        }
    }
}
