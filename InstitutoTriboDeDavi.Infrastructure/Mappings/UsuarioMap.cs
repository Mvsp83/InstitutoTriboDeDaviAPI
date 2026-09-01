using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("USUARIOS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Login")
                .HasColumnType("VARCHAR(20)");

            builder.Property(x => x.SenhaHash)
                .IsRequired()
                 .HasColumnName("Password")
                .HasColumnType("VARCHAR(200)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType("VARCHAR(180)");

            // Preset ("preset:7") ou miniatura em data URI. O limite de tamanho
            // real é imposto na aplicação (AtualizarAvatarAsync).
            builder.Property(x => x.Avatar)
                .IsRequired(false)
                .HasColumnName("Avatar")
                .HasColumnType("text");

            // 2FA (TOTP). Secret base32 curto; nulo enquanto o usuário não ativa.
            builder.Property(x => x.TotpSecret)
                .IsRequired(false)
                .HasMaxLength(64)
                .HasColumnType("VARCHAR(64)");

            builder.Property(x => x.TotpConfirmado)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
