using System;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                new PoloRepository(_context),
                new OcorrenciaAlunoRepository(_context),
                new BancoFotoStorage(new FotoArquivoRepository(_context)));
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

        private long IdDaFalta() =>
            _context.Presencas.First(p => !p.EstaPresente).Id;

        [Fact]
        public async Task JustificarFalta_FaltaDoAluno_GravaJustificativaEData()
        {
            var aluno = Semear();
            var faltaId = IdDaFalta();

            var item = await _service.JustificarFaltaAsync(aluno.Id, faltaId, "  Estava doente  ");

            Assert.Equal("Estava doente", item.Justificativa); // trim aplicado
            Assert.NotNull(item.JustificadaEm);

            var painel = await _service.ObterPainelAsync(aluno.Id);
            var falta = painel.Presencas.First(p => !p.Presente);
            Assert.Equal("Estava doente", falta.Justificativa);
            Assert.NotNull(falta.JustificadaEm);
        }

        [Fact]
        public async Task JustificarFalta_Presenca_Recusa()
        {
            var aluno = Semear();
            var presencaId = _context.Presencas.First(p => p.EstaPresente).Id;

            await Assert.ThrowsAsync<DomainException>(
                () => _service.JustificarFaltaAsync(aluno.Id, presencaId, "qualquer"));
        }

        [Fact]
        public async Task JustificarFalta_DeOutroAluno_Recusa()
        {
            Semear();
            var faltaId = IdDaFalta();

            // Aluno diferente do dono da falta → mesma resposta de "não encontrada".
            await Assert.ThrowsAsync<DomainException>(
                () => _service.JustificarFaltaAsync(999, faltaId, "qualquer"));
        }

        [Fact]
        public async Task JustificarFalta_MotivoVazio_Recusa()
        {
            var aluno = Semear();
            var faltaId = IdDaFalta();

            await Assert.ThrowsAsync<DomainException>(
                () => _service.JustificarFaltaAsync(aluno.Id, faltaId, "   "));
        }

        public void Dispose() => _context.Dispose();
    }
}
