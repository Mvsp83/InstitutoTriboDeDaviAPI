using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class AlunoMap : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.ToTable("ALUNOS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("Nome")
                .HasColumnType("VARCHAR(120)");

            // Campos vindos da ficha de inscrição online. Todos opcionais: os
            // cadastros anteriores não os possuem.
            builder.Property(x => x.Altura).IsRequired(false);
            builder.Property(x => x.Numero).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.Complemento).HasMaxLength(80).IsRequired(false);
            builder.Property(x => x.Telefone2).HasMaxLength(30).IsRequired(false);
            builder.Property(x => x.Serie).HasMaxLength(40).IsRequired(false);
        }
    }
}
