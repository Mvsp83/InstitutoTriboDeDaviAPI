using System;
using System.Collections.Generic;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class PlanoMensalidadeMap : IEntityTypeConfiguration<PlanoMensalidade>
    {
        public void Configure(EntityTypeBuilder<PlanoMensalidade> builder)
        {
            builder.ToTable("PLANO_MENSALIDADE");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Ativo).IsRequired();
            builder.Property(x => x.Descricao).HasMaxLength(1000).IsRequired(false);

            // Lista de dias guardada como CSV ("5,10,15") numa coluna só.
            var conversor = new ValueConverter<List<int>, string>(
                v => string.Join(",", v),
                v => string.IsNullOrEmpty(v)
                    ? new List<int>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            var comparador = new ValueComparer<List<int>>(
                (a, b) => a.SequenceEqual(b),
                v => v.Aggregate(0, (h, i) => HashCode.Combine(h, i)),
                v => v.ToList());

            builder.Property(x => x.OpcoesVencimento)
                .HasConversion(conversor, comparador)
                .HasMaxLength(120)
                .HasColumnName("OpcoesVencimento");
        }
    }

    public class MatriculaFinanceiraMap : IEntityTypeConfiguration<MatriculaFinanceira>
    {
        public void Configure(EntityTypeBuilder<MatriculaFinanceira> builder)
        {
            builder.ToTable("MATRICULA_FINANCEIRA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.PlanoId).IsRequired();
            builder.Property(x => x.DiaVencimento).IsRequired();
            builder.Property(x => x.Inicio).IsRequired().HasMaxLength(7);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
            builder.Property(x => x.DescontoTipo).IsRequired().HasMaxLength(20);
            builder.Property(x => x.DescontoValor).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);

            builder.HasIndex(x => x.AlunoId);
            builder.HasIndex(x => x.PlanoId);
        }
    }

    public class CobrancaMap : IEntityTypeConfiguration<Cobranca>
    {
        public void Configure(EntityTypeBuilder<Cobranca> builder)
        {
            builder.ToTable("COBRANCA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("BIGINT");

            builder.Property(x => x.AlunoId).IsRequired();
            builder.Property(x => x.PlanoId).IsRequired(false);
            builder.Property(x => x.Competencia).IsRequired().HasMaxLength(7);
            builder.Property(x => x.Vencimento).IsRequired();
            builder.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status).IsRequired().HasMaxLength(20);
            builder.Property(x => x.PagamentoData).IsRequired(false);
            builder.Property(x => x.PagamentoValor).HasColumnType("decimal(18,2)").IsRequired(false);
            builder.Property(x => x.PagamentoForma).HasMaxLength(40).IsRequired(false);
            builder.Property(x => x.ContaId).IsRequired(false);
            builder.Property(x => x.MovimentacaoId).IsRequired(false);
            builder.Property(x => x.Observacao).HasMaxLength(1000).IsRequired(false);

            // Consultas do portal filtram por competência e por aluno.
            builder.HasIndex(x => x.Competencia);
            builder.HasIndex(x => new { x.AlunoId, x.Competencia });
        }
    }
}
