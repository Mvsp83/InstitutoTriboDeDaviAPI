using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class PoloMap : IEntityTypeConfiguration<Polo>
    {
        public void Configure(EntityTypeBuilder<Polo> builder)
        {
            builder.ToTable("POLOS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("Nome")
                .HasColumnType("VARCHAR(120)");

            builder.Property(x => x.LimiteAlunos).IsRequired();

            // Default true no banco: polos existentes continuam aceitando adultos.
            builder.Property(x => x.AceitaAdultos).IsRequired().HasDefaultValue(true);

            // Horários por turma: apagar o polo apaga os horários.
            builder.HasMany(x => x.Horarios)
                .WithOne()
                .HasForeignKey(h => h.PoloId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class HorarioTurmaMap : IEntityTypeConfiguration<HorarioTurma>
    {
        public void Configure(EntityTypeBuilder<HorarioTurma> builder)
        {
            builder.ToTable("POLO_HORARIO_TURMA");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.Turma).IsRequired();
            builder.Property(x => x.DiaSemana).IsRequired();
            builder.Property(x => x.HoraInicio).IsRequired().HasMaxLength(5);
            builder.Property(x => x.HoraFim).HasMaxLength(5).IsRequired(false);
            builder.HasIndex(x => x.PoloId);
        }
    }
}
