using InstitutoTriboDeDavi.System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.System.Infra.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("USUARIOS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Login")
                .HasColumnType("VARCHAR(20)");

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Password")
                .HasColumnType("VARCHAR(20)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType("VARCHAR(180)");
        }
    }
}
