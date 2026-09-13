using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class EmprestimoBemMap : IEntityTypeConfiguration<EmprestimoBem>
    {
        public void Configure(EntityTypeBuilder<EmprestimoBem> builder)
        {
            builder.ToTable("EMPRESTIMO_BEM");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.BemPatrimonialId).IsRequired();
            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.DataEmprestimo).IsRequired();
            builder.Property(x => x.DataDevolucao).IsRequired(false);

            builder.Property(x => x.Observacao).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.RegistradoPor).HasMaxLength(120).IsRequired(false);

            // Consulta principal: empréstimos de um bem (aberto + histórico).
            builder.HasIndex(x => x.BemPatrimonialId);
        }
    }
}
