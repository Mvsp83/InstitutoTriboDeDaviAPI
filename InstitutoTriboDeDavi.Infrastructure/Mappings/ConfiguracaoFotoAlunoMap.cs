using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class ConfiguracaoFotoAlunoMap : IEntityTypeConfiguration<ConfiguracaoFotoAluno>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoFotoAluno> builder)
        {
            builder.ToTable("CONFIG_FOTO_ALUNO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.MostrarNoCadastro).IsRequired();
            builder.Property(x => x.MostrarNaChamada).IsRequired();
            builder.Property(x => x.MostrarNoResponsavel).IsRequired();
            builder.Property(x => x.MostrarNaCarteirinha).IsRequired();
        }
    }
}
