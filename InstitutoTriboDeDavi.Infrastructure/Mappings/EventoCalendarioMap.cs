using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class EventoCalendarioMap : IEntityTypeConfiguration<EventoCalendario>
    {
        public void Configure(EntityTypeBuilder<EventoCalendario> builder)
        {
            builder.ToTable("EVENTO_CALENDARIO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Ano).IsRequired();
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.DataFim).IsRequired(false);
            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.PoloId).IsRequired(false);

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Descricao)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(x => x.Notificar);
            builder.Property(x => x.DiasAntecedencia);
            builder.Property(x => x.NotificacaoEnviada);
            builder.Property(x => x.EmailsNotificacao)
                .HasMaxLength(500)
                .IsRequired(false);
        }
    }
}
