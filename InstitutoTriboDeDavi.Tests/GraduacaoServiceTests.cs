using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes da trilha de graduação:
    // - graduar sobe a faixa do aluno e guarda de onde ele veio
    // - quem já está na faixa (ou acima) é ignorado, sem derrubar o lote
    // - não se grava retrocesso de faixa
    // - excluir devolve o aluno à faixa anterior
    public class GraduacaoServiceTests
    {
        private readonly Mock<IGraduacaoRepository> _graduacoes = new();
        private readonly Mock<IAlunoRepository> _alunos = new();
        private readonly Mock<IPoloRepository> _polos = new();
        private readonly GraduacaoService _service;

        private List<(Graduacao graduacao, Aluno aluno)> _registrados = new();
        private Graduacao _excluida;
        private Aluno _alunoDaExclusao;

        public GraduacaoServiceTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Graduacao, GraduacaoDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _polos.Setup(r => r.GetAllAsync())
                  .ReturnsAsync(new List<Polo> { new() { Id = 1, Nome = "Polo 1" } });

            _graduacoes.Setup(r => r.RegistrarAsync(It.IsAny<List<(Graduacao, Aluno)>>()))
                       .ReturnsAsync((List<(Graduacao, Aluno)> itens) =>
                       {
                           _registrados = itens;
                           return itens.Count;
                       });

            _graduacoes.Setup(r => r.ExcluirAsync(It.IsAny<Graduacao>(), It.IsAny<Aluno>()))
                       .Callback((Graduacao g, Aluno a) => { _excluida = g; _alunoDaExclusao = a; })
                       .Returns(Task.CompletedTask);

            _service = new GraduacaoService(mapper, _graduacoes.Object, _alunos.Object, _polos.Object);
        }

        private static Aluno AlunoCom(long id, int faixa, string nome = "Aluno") =>
            new() { Id = id, Nome = nome, PoloId = 1, Faixa = (Faixa)faixa };

        private static GraduacaoLoteDTO Lote(params (long id, int faixa)[] itens) => new()
        {
            Data = DateTime.Today,
            Observacao = "Exame de faixa",
            Alunos = itens.Select(i => new ItemGraduacaoDTO { AlunoId = i.id, FaixaNova = i.faixa }).ToArray(),
        };

        [Fact]
        public async Task Registrar_SobeAFaixaEGuardaAOrigem()
        {
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno> { AlunoCom(1, 10, "Ana") });

            var r = await _service.Registrar(Lote((1, 11)), "professor");

            Assert.Equal(1, r.Graduados);
            var (g, aluno) = _registrados.Single();
            Assert.Equal(10, g.FaixaAnterior);
            Assert.Equal(11, g.FaixaNova);
            Assert.Equal(1, g.AlunoId);
            Assert.Equal(1, g.PoloId);
            Assert.Equal("professor", g.RegistradoPor);
            Assert.Equal("Ana", aluno.Nome);
        }

        [Fact]
        public async Task Registrar_QuemJaEstaNaFaixa_EIgnoradoSemDerrubarOLote()
        {
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno>
            {
                AlunoCom(1, 10, "Ana"),   // sobe
                AlunoCom(2, 15, "Bruno"), // já está acima do alvo
            });

            var r = await _service.Registrar(Lote((1, 11), (2, 11)), "professor");

            Assert.Equal(1, r.Graduados);
            Assert.Single(r.Ignorados);
            Assert.Contains("Bruno", r.Ignorados[0]);
            Assert.Equal(1, _registrados.Single().graduacao.AlunoId);
        }

        [Fact]
        public async Task Registrar_AlunoInexistente_EIgnorado()
        {
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno> { AlunoCom(1, 10) });

            var r = await _service.Registrar(Lote((1, 11), (99, 11)), "professor");

            Assert.Equal(1, r.Graduados);
            Assert.Contains("99", r.Ignorados[0]);
        }

        [Fact]
        public async Task Registrar_NinguemElegivel_Recusa()
        {
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno> { AlunoCom(1, 20) });

            await Assert.ThrowsAsync<DomainException>(() => _service.Registrar(Lote((1, 10)), "prof"));
            _graduacoes.Verify(r => r.RegistrarAsync(It.IsAny<List<(Graduacao, Aluno)>>()), Times.Never);
        }

        [Fact]
        public async Task Registrar_SemAlunos_Recusa()
        {
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.Registrar(new GraduacaoLoteDTO { Data = DateTime.Today, Alunos = [] }, "prof"));
        }

        [Fact]
        public async Task Registrar_DataFutura_Recusa()
        {
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno> { AlunoCom(1, 10) });
            var lote = Lote((1, 11));
            lote.Data = DateTime.Today.AddDays(1);

            await Assert.ThrowsAsync<DomainException>(() => _service.Registrar(lote, "prof"));
        }

        [Fact]
        public async Task Excluir_DevolveAFaixaAnterior()
        {
            var graduacao = new Graduacao
            {
                Id = 5, AlunoId = 1, PoloId = 1,
                FaixaAnterior = 10, FaixaNova = 11, Data = DateTime.Today,
            };
            _graduacoes.Setup(r => r.ObterAsync(5)).ReturnsAsync(graduacao);
            _alunos.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(AlunoCom(1, 11));

            await _service.Excluir(5);

            Assert.Equal(10, _excluida.FaixaAnterior);
            Assert.Equal(1, _alunoDaExclusao.Id);
        }

        [Fact]
        public async Task Excluir_Inexistente_Recusa()
        {
            _graduacoes.Setup(r => r.ObterAsync(It.IsAny<long>())).ReturnsAsync((Graduacao)null);

            await Assert.ThrowsAsync<DomainException>(() => _service.Excluir(123));
        }

        [Fact]
        public async Task Listar_TrazNomeDoAlunoEDoPolo()
        {
            _graduacoes.Setup(r => r.ListarAsync(It.IsAny<int?>(), It.IsAny<long?>()))
                       .ReturnsAsync(new List<Graduacao>
                       {
                           new() { Id = 1, AlunoId = 1, PoloId = 1, FaixaAnterior = 10, FaixaNova = 11, Data = DateTime.Today }
                       });
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno> { AlunoCom(1, 11, "Ana") });

            var lista = await _service.Listar(null, null);

            Assert.Equal("Ana", lista[0].NomeAluno);
            Assert.Equal("Polo 1", lista[0].PoloNome);
        }
    }
}
