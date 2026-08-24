using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class OcorrenciaAlunoMap : IEntityTypeConfiguration<OcorrenciaAluno>
    {
        public void Configure(EntityTypeBuilder<OcorrenciaAluno> builder)
        {
            builder.ToTable("OCORRENCIA_ALUNO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Texto).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.Data).IsRequired();
            builder.Property(x => x.RegistradoPor).HasMaxLength(120).IsRequired(false);

            builder.HasIndex(x => x.AlunoId);
        }
    }
}
