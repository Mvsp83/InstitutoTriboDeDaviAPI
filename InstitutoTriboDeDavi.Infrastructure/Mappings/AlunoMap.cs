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

            // Eliminação de dados pessoais (LGPD). Nulo = cadastro ativo.
            builder.Property(x => x.AnonimizadoEm).IsRequired(false);

            // Código de acesso do responsável ao portal (nulo = não liberado).
            builder.Property(x => x.CodigoResponsavel)
                .IsRequired(false)
                .HasMaxLength(16)
                .HasColumnType("VARCHAR(16)");

            builder.HasIndex(x => x.CodigoResponsavel);

            // Autorização de imagem (LGPD): null = não informado.
            builder.Property(x => x.AutorizaImagem).IsRequired(false);
            builder.Property(x => x.AutorizaImagemEm).IsRequired(false);
            builder.Property(x => x.EhAdulto).IsRequired();
            builder.Property(x => x.FotoArquivoId).HasMaxLength(200).IsRequired(false);
        }
    }
}
