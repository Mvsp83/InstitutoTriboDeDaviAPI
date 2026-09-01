using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ConfiguracaoDashboardMap : IEntityTypeConfiguration<ConfiguracaoDashboard>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoDashboard> builder)
        {
            builder.ToTable("CONFIGURACAO_DASHBOARD");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.UsuarioLogin)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Layout)
                .IsRequired()
                .HasMaxLength(4000);

            // Uma configuração por usuário.
            builder.HasIndex(x => x.UsuarioLogin).IsUnique();
        }
    }
}
