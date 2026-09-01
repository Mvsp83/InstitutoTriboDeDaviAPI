using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class PushSubscriptionMap : IEntityTypeConfiguration<PushSubscription>
    {
        public void Configure(EntityTypeBuilder<PushSubscription> builder)
        {
            builder.ToTable("PUSH_SUBSCRIPTION");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.UsuarioLogin).HasMaxLength(120).IsRequired();
            // Endpoint é uma URL longa (pode passar de 400 chars no FCM); fica sem
            // índice único — a deduplicação é feita em código (a tabela é pequena).
            builder.Property(x => x.Endpoint).IsRequired();
            builder.Property(x => x.P256dh).IsRequired();
            builder.Property(x => x.Auth).IsRequired();

            builder.HasIndex(x => x.UsuarioLogin);
        }
    }
}
