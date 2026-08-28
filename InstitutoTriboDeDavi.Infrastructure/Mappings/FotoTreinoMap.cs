using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class FotoTreinoMap : IEntityTypeConfiguration<FotoTreino>
    {
        public void Configure(EntityTypeBuilder<FotoTreino> builder)
        {
            builder.ToTable("FOTO_TREINO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.Categoria).IsRequired().HasMaxLength(20);
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.Turma).IsRequired();
            builder.Property(x => x.DataAula).IsRequired().HasColumnType("date");
            builder.Property(x => x.Legenda).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.ArquivoId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ProfessorId).IsRequired();
            builder.Property(x => x.Publicada).IsRequired();
            builder.Property(x => x.CriadoEm).IsRequired();

            // Trava 1 foto por turma por aula — só na categoria "polo" (as
            // coleções do admin podem ter várias fotos por data).
            builder.HasIndex(x => new { x.PoloId, x.Turma, x.DataAula })
                .IsUnique()
                .HasFilter("[Categoria] = 'polo'");
            // O álbum público filtra por publicada e por categoria.
            builder.HasIndex(x => x.Publicada);
            builder.HasIndex(x => x.Categoria);
        }
    }

    public class FotoArquivoMap : IEntityTypeConfiguration<FotoArquivo>
    {
        public void Configure(EntityTypeBuilder<FotoArquivo> builder)
        {
            builder.ToTable("FOTO_ARQUIVO");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.Conteudo).IsRequired();
            builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        }
    }

    public class PoloFotoConfigMap : IEntityTypeConfiguration<PoloFotoConfig>
    {
        public void Configure(EntityTypeBuilder<PoloFotoConfig> builder)
        {
            builder.ToTable("POLO_FOTO_CONFIG");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.RequerAutorizacao).IsRequired();

            builder.HasIndex(x => x.PoloId).IsUnique();
        }
    }
}
