using System;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InstitutoTriboDeDavi.Tests
{
    // LGPD: exportação (acesso) e anonimização (eliminação) de um aluno, mais a
    // varredura de candidatos à retenção. Banco InMemory — exercita a lógica do
    // repositório sem depender do schema SQL.
    public class AlunoLgpdTests : IDisposable
    {
        private readonly TriboDeDaviContext _context;
        private readonly AlunoRepository _repo;

        public AlunoLgpdTests()
        {
            var options = new DbContextOptionsBuilder<TriboDeDaviContext>()
                .UseInMemoryDatabase($"lgpd-{Guid.NewGuid()}")
                .Options;
            _context = new TriboDeDaviContext(options);
            _repo = new AlunoRepository(_context);
        }

        private Aluno SemearAlunoCompleto()
        {
            var aluno = new Aluno
            {
                Nome = "João da Silva",
                RG = "12.345.678-9",
                CPF = "123.456.789-00",
                DataNascimento = new DateTime(2015, 5, 10),
                Endereco = "Rua das Flores",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Blumenau",
                Celular = "47999998888",
                Responsavel = "Maria da Silva",
                CPFResponsavel = "987.654.321-00",
                Escola = "EEB Teste",
                PoloId = 1,
                Turma = 2,
            };
            _context.Alunos.Add(aluno);
            _context.SaveChanges();

            _context.Presencas.Add(new Presenca
            {
                AlunoId = aluno.Id,
                NomeAluno = "João da Silva",
                PoloId = 1,
                Data = new DateTime(2026, 3, 1),
                EstaPresente = true,
                Observacoes = string.Empty,
                AulaId = 1,
            });
            _context.Matriculas.Add(new Matricula
            {
                AlunoId = aluno.Id,
                Ano = 2026,
                PoloId = 1,
                Turma = 2,
                DataMatricula = new DateTime(2026, 2, 1),
                Ativa = true,
            });
            _context.Graduacoes.Add(new Graduacao
            {
                AlunoId = aluno.Id,
                PoloId = 1,
                FaixaAnterior = 0,
                FaixaNova = 1,
                Data = new DateTime(2026, 6, 1),
            });
            _context.Inscricoes.Add(new Inscricao
            {
                AlunoId = aluno.Id,
                Ano = 2026,
                PoloId = 1,
                Nome = "João da Silva",
                Cpf = "123.456.789-00",
                NomeResponsavel = "Maria da Silva",
                WhatsApp = "47999998888",
                NomeAssinatura = "Maria da Silva",
                AceitouTermo = true,
                AceitouLgpd = true,
                VersaoTermos = "2026.1",
                DataEnvio = new DateTime(2026, 1, 15),
                Status = (int)StatusInscricao.Aprovada,
            });
            _context.SaveChanges();

            return aluno;
        }

        [Fact]
        public async Task Exportar_ReuneTodosOsDadosDoAluno()
        {
            var aluno = SemearAlunoCompleto();

            var dados = await _repo.ColetarDadosPessoaisAsync(aluno.Id);

            Assert.NotNull(dados);
            Assert.Equal("João da Silva", dados.Aluno.Nome);
            Assert.Single(dados.Presencas);
            Assert.Single(dados.Matriculas);
            Assert.Single(dados.Graduacoes);
            Assert.Single(dados.Inscricoes);
        }

        [Fact]
        public async Task Exportar_IdInexistente_RetornaNull()
        {
            Assert.Null(await _repo.ColetarDadosPessoaisAsync(999));
        }

        [Fact]
        public async Task Anonimizar_ApagaPiiDoAlunoEDaInscricao_MantendoRegistros()
        {
            var aluno = SemearAlunoCompleto();

            var resultado = await _repo.AnonimizarAsync(aluno.Id);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.AnonimizadoEm);

            var salvo = await _context.Alunos.AsNoTracking().FirstAsync(a => a.Id == aluno.Id);
            Assert.Equal($"Aluno anonimizado #{aluno.Id}", salvo.Nome);
            Assert.Null(salvo.CPF);
            Assert.Null(salvo.RG);
            Assert.Null(salvo.Responsavel);
            Assert.Null(salvo.CPFResponsavel);
            // Dados operacionais/estatísticos ficam.
            Assert.Equal(1, salvo.PoloId);
            Assert.Equal(new DateTime(2015, 5, 10), salvo.DataNascimento);

            // O nome copiado na presença também some.
            var presenca = await _context.Presencas.AsNoTracking().FirstAsync(p => p.AlunoId == aluno.Id);
            Assert.DoesNotContain("João", presenca.NomeAluno);

            // Inscrição: PII apagada, mas o consentimento é preservado como prova.
            var inscricao = await _context.Inscricoes.AsNoTracking().FirstAsync(i => i.AlunoId == aluno.Id);
            Assert.Equal(string.Empty, inscricao.Cpf);
            Assert.Equal(string.Empty, inscricao.NomeResponsavel);
            Assert.True(inscricao.AceitouLgpd);
            Assert.Equal("2026.1", inscricao.VersaoTermos);

            // Matrícula e graduação (accountability) permanecem intactas.
            Assert.True(await _context.Matriculas.AnyAsync(m => m.AlunoId == aluno.Id));
            Assert.True(await _context.Graduacoes.AnyAsync(g => g.AlunoId == aluno.Id));
        }

        [Fact]
        public async Task Anonimizar_EhIdempotente()
        {
            var aluno = SemearAlunoCompleto();

            var primeira = await _repo.AnonimizarAsync(aluno.Id);
            var quando = primeira.AnonimizadoEm;

            var segunda = await _repo.AnonimizarAsync(aluno.Id);

            // Não muda a marca de quando foi anonimizado.
            Assert.Equal(quando, segunda.AnonimizadoEm);
        }

        [Fact]
        public async Task Anonimizar_IdInexistente_RetornaNull()
        {
            Assert.Null(await _repo.AnonimizarAsync(999));
        }

        [Fact]
        public async Task Retencao_ListaInativoENaoOAtivoNemOAnonimizado()
        {
            // Inativo: última presença há muito tempo.
            var inativo = new Aluno { Nome = "Inativo", DataNascimento = new DateTime(2012, 1, 1), PoloId = 1, Turma = 1 };
            // Ativo: presença recente.
            var ativo = new Aluno { Nome = "Ativo", DataNascimento = new DateTime(2012, 1, 1), PoloId = 1, Turma = 1 };
            // Já anonimizado: nunca é candidato.
            var anon = new Aluno { Nome = "Anon", DataNascimento = new DateTime(2012, 1, 1), PoloId = 1, Turma = 1, AnonimizadoEm = DateTime.Now };
            _context.Alunos.AddRange(inativo, ativo, anon);
            _context.SaveChanges();

            _context.Presencas.Add(new Presenca { AlunoId = inativo.Id, NomeAluno = "Inativo", PoloId = 1, Data = DateTime.Now.AddMonths(-30), EstaPresente = true, Observacoes = string.Empty, AulaId = 1 });
            _context.Presencas.Add(new Presenca { AlunoId = ativo.Id, NomeAluno = "Ativo", PoloId = 1, Data = DateTime.Now.AddMonths(-1), EstaPresente = true, Observacoes = string.Empty, AulaId = 2 });
            _context.SaveChanges();

            var candidatos = await _repo.ObterCandidatosRetencaoAsync(18);

            Assert.Contains(candidatos, c => c.Aluno.Id == inativo.Id);
            Assert.DoesNotContain(candidatos, c => c.Aluno.Id == ativo.Id);
            Assert.DoesNotContain(candidatos, c => c.Aluno.Id == anon.Id);
        }

        public void Dispose() => _context.Dispose();
    }
}
