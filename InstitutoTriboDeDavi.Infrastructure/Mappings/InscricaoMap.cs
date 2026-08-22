using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class InscricaoMap : IEntityTypeConfiguration<Inscricao>
    {
        public void Configure(EntityTypeBuilder<Inscricao> builder)
        {
            builder.ToTable("INSCRICAO");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.Ano).IsRequired();
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.Turma).IsRequired(false);
            builder.Property(x => x.TurmaAnterior).IsRequired(false);

            builder.Property(x => x.Nome).IsRequired().HasMaxLength(160);
            builder.Property(x => x.DataNascimento).IsRequired();
            builder.Property(x => x.Rg).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Cpf).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.Peso).HasColumnType("decimal(6,2)").IsRequired(false);
            builder.Property(x => x.Altura).HasColumnType("decimal(4,2)").IsRequired(false);
            builder.Property(x => x.Escola).HasMaxLength(160).IsRequired(false);
            builder.Property(x => x.Serie).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.Periodo).HasMaxLength(20).IsRequired(false);

            builder.Property(x => x.ParentescoOutro).HasMaxLength(60).IsRequired(false);
            builder.Property(x => x.NomeResponsavel).IsRequired().HasMaxLength(160);
            builder.Property(x => x.RgResponsavel).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.CpfResponsavel).HasMaxLength(20).IsRequired(false);

            builder.Property(x => x.Rua).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.Numero).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.Complemento).HasMaxLength(80).IsRequired(false);
            builder.Property(x => x.Bairro).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.Cidade).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.WhatsApp).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Telefone2).HasMaxLength(30).IsRequired(false);

            // JSON dos questionários: nvarchar(max) para não limitar respostas.
            builder.Property(x => x.RespostasSaudeJson).IsRequired(false);
            builder.Property(x => x.RespostasFamiliarJson).IsRequired(false);
            builder.Property(x => x.Medicamentos).HasMaxLength(1000).IsRequired(false);

            builder.Property(x => x.NomeAssinatura).IsRequired().HasMaxLength(160);
            builder.Property(x => x.VersaoTermos).HasMaxLength(20).IsRequired(false);
            builder.Property(x => x.DataEnvio).IsRequired();

            builder.Property(x => x.CodigoResponsavel).HasMaxLength(16).IsRequired(false);

            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.AlunoId).IsRequired(false);
            builder.Property(x => x.DataRevisao).IsRequired(false);
            builder.Property(x => x.RevisadoPor).HasMaxLength(120).IsRequired(false);
            builder.Property(x => x.ObservacaoRevisao).HasMaxLength(1000).IsRequired(false);

            // A fila de revisão filtra por status, ano e polo.
            builder.HasIndex(x => new { x.Status, x.Ano });
            builder.HasIndex(x => x.PoloId);
        }
    }

    public class MatriculaMap : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.ToTable("MATRICULA");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseIdentityColumn()
                .HasColumnType("BIGINT");

            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.Ano).IsRequired();
            builder.Property(x => x.PoloId).IsRequired();
            builder.Property(x => x.Turma).IsRequired();
            builder.Property(x => x.InscricaoId).IsRequired(false);
            builder.Property(x => x.DataMatricula).IsRequired();
            builder.Property(x => x.Ativa).IsRequired();
            builder.Property(x => x.DataEncerramento).IsRequired(false);
            builder.Property(x => x.MotivoEncerramento).HasMaxLength(300).IsRequired(false);

            // Um aluno tem no máximo uma matrícula por ano.
            builder.HasIndex(x => new { x.AlunoId, x.Ano }).IsUnique();
            builder.HasIndex(x => new { x.Ano, x.PoloId });
        }
    }
}
