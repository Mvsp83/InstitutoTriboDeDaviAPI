using System;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InstitutoTriboDeDavi.Tests
{
    // B3 — Portal do responsável: autenticação por código + nascimento e o
    // painel montado a partir dos dados do aluno.
    public class ResponsavelPortalTests : IDisposable
    {
        private readonly TriboDeDaviContext _context;
        private readonly ResponsavelService _service;

        public ResponsavelPortalTests()
        {
            var options = new DbContextOptionsBuilder<TriboDeDaviContext>()
                .UseInMemoryDatabase($"resp-{Guid.NewGuid()}")
                .Options;
            _context = new TriboDeDaviContext(options);

            _service = new ResponsavelService(
                new AlunoRepository(_context),
                new PresencaRepository(_context),
                new GraduacaoRepository(_context),
                new AvisoRepository(_context),
                new EventoCalendarioRepository(_context),
                new PoloRepository(_context));
        }

        private Aluno Semear()
        {
            _context.Polos.Add(new Polo
            {
                Id = 1, Nome = "Eça de Queiroz", Informacoes = "", Endereco = "",
                Bairro = "", Cidade = "",
            });
            var aluno = new Aluno
            {
                Nome = "João da Silva",
                DataNascimento = new DateTime(2015, 5, 10),
                CodigoResponsavel = "ABCD2345",
                PoloId = 1,
                Turma = 2,
            };
            _context.Alunos.Add(aluno);
            _context.SaveChanges();

            _context.Presencas.AddRange(
                new Presenca { AlunoId = aluno.Id, NomeAluno = "João", PoloId = 1, Data = new DateTime(2026, 3, 1), EstaPresente = true, Observacoes = "", AulaId = 1 },
                new Presenca { AlunoId = aluno.Id, NomeAluno = "João", PoloId = 1, Data = new DateTime(2026, 3, 3), EstaPresente = false, Observacoes = "", AulaId = 2 });
            _context.Graduacoes.Add(new Graduacao { AlunoId = aluno.Id, PoloId = 1, FaixaAnterior = 0, FaixaNova = 1, Data = new DateTime(2026, 6, 1) });
            _context.SaveChanges();
            return aluno;
        }

        [Fact]
        public async Task Autenticar_CodigoENascimentoCertos_RetornaAluno()
        {
            Semear();

            var acesso = await _service.AutenticarAsync("ABCD2345", new DateTime(2015, 5, 10));

            Assert.NotNull(acesso);
            Assert.Equal("João da Silva", acesso.Nome);
            Assert.Equal("Eça de Queiroz", acesso.Polo);
            Assert.Equal(2, acesso.Turma);
        }

        [Fact]
        public async Task Autenticar_NascimentoErrado_RetornaNull()
        {
            Semear();
            Assert.Null(await _service.AutenticarAsync("ABCD2345", new DateTime(2010, 1, 1)));
        }

        [Fact]
        public async Task Autenticar_CodigoInexistente_RetornaNull()
        {
            Semear();
            Assert.Null(await _service.AutenticarAsync("ZZZZZZZZ", new DateTime(2015, 5, 10)));
        }

        [Fact]
        public async Task Painel_ResumeFrequenciaEHistorico()
        {
            var aluno = Semear();

            var painel = await _service.ObterPainelAsync(aluno.Id);

            Assert.NotNull(painel);
            Assert.Equal("João da Silva", painel.Aluno.Nome);
            Assert.Equal(2, painel.Frequencia.TotalAulas);
            Assert.Equal(1, painel.Frequencia.Presencas);
            Assert.Equal(1, painel.Frequencia.Faltas);
            Assert.Equal(50, painel.Frequencia.Percentual);
            Assert.Single(painel.Graduacoes);
            Assert.Equal(2, painel.Presencas.Count);
        }

        public void Dispose() => _context.Dispose();
    }
}
