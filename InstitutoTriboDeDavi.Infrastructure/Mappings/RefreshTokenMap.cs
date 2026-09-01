using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("REFRESH_TOKENS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.UsuarioId)
                .IsRequired()
                .HasColumnType("BIGINT");

            // Hash SHA-256 em hex (64 chars). Índice acelera a busca no refresh.
            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnType("VARCHAR(128)");

            builder.HasIndex(x => x.TokenHash);
            builder.HasIndex(x => x.UsuarioId);

            builder.Property(x => x.CriadoEm).IsRequired();
            builder.Property(x => x.ExpiraEm).IsRequired();
            builder.Property(x => x.RevogadoEm).IsRequired(false);

            builder.Ignore(x => x.Ativo);
        }
    }
}
