using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes do módulo de planejamento de aulas:
    // - validação de domínio (soma dos blocos x duração total, ordem repetida)
    // - criação de plano a partir de modelo (blocos copiados, status Rascunho)
    // - clonagem de plano existente com nova data
    public class PlanoDeAulaTests
    {
        private readonly Mock<IPlanoDeAulaRepository> _planoRepositorio = new();
        private readonly Mock<IModeloDeAulaRepository> _modeloRepositorio = new();
        private readonly PlanoDeAulaService _service;

        public PlanoDeAulaTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlanoDeAula, PlanoDeAulaDTO>().ReverseMap();
                cfg.CreateMap<BlocoDoPlano, BlocoDoPlanoDTO>().ReverseMap();
                cfg.CreateMap<AtividadeDoBloco, AtividadeDoBlocoDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _planoRepositorio
                .Setup(r => r.CreateAsync(It.IsAny<PlanoDeAula>()))
                .ReturnsAsync((PlanoDeAula p) => p);

            _service = new PlanoDeAulaService(mapper, _planoRepositorio.Object, _modeloRepositorio.Object);
        }

        private static PlanoDeAula PlanoValido() => new()
        {
            PoloId = 1,
            Turma = 1,
            Titulo = "Aula infantil - quedas",
            DataPrevista = DateTime.Today.AddDays(1),
            DuracaoTotalMinutos = 65,
            Blocos = new List<BlocoDoPlano>
            {
                new() { Ordem = 1, Nome = "Aquecimento", Tipo = TipoBloco.Aquecimento, DuracaoMinutos = 15 },
                new() { Ordem = 2, Nome = "Posições", Tipo = TipoBloco.Posicoes, DuracaoMinutos = 20 },
                new() { Ordem = 3, Nome = "Lutas", Tipo = TipoBloco.Lutas, DuracaoMinutos = 20 },
                new() { Ordem = 4, Nome = "Mensagem Final", Tipo = TipoBloco.MensagemFinal, DuracaoMinutos = 10 }
            }
        };

        [Fact]
        public void PlanoDeAula_Valido_RetornaTrue()
        {
            Assert.True(PlanoValido().Validate());
        }

        [Fact]
        public void PlanoDeAula_BlocosExcedemDuracaoTotal_LancaDomainException()
        {
            var plano = PlanoValido();
            plano.DuracaoTotalMinutos = 60; // blocos somam 65

            var ex = Assert.Throws<DomainException>(() => plano.Validate());
            Assert.Contains(ex.Errors, e => e.Contains("Duração Total"));
        }

        [Fact]
        public void PlanoDeAula_OrdemRepetida_LancaDomainException()
        {
            var plano = PlanoValido();
            plano.Blocos[1].Ordem = 1; // repete a ordem do primeiro bloco

            var ex = Assert.Throws<DomainException>(() => plano.Validate());
            Assert.Contains(ex.Errors, e => e.Contains("Ordem repetida"));
        }

        [Fact]
        public void PlanoDeAula_SemTitulo_LancaDomainException()
        {
            var plano = PlanoValido();
            plano.Titulo = "";

            Assert.Throws<DomainException>(() => plano.Validate());
        }

        [Fact]
        public void BlocoDoPlano_DuracaoZero_LancaDomainException()
        {
            var plano = PlanoValido();
            plano.Blocos[0].DuracaoMinutos = 0;

            Assert.Throws<DomainException>(() => plano.Validate());
        }

        [Fact]
        public async Task CriarDeModelo_CopiaBlocosEDuracao_StatusRascunho()
        {
            _modeloRepositorio.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new ModeloDeAula
            {
                Id = 5,
                Nome = "Aula padrão 65 min",
                DuracaoTotalMinutos = 65,
                Blocos = new List<BlocoDoModelo>
                {
                    new() { Ordem = 1, Nome = "Aquecimento", Tipo = TipoBloco.Aquecimento, DuracaoMinutos = 15 },
                    new() { Ordem = 2, Nome = "Posições", Tipo = TipoBloco.Posicoes, DuracaoMinutos = 20 },
                    new() { Ordem = 3, Nome = "Lutas", Tipo = TipoBloco.Lutas, DuracaoMinutos = 20 },
                    new() { Ordem = 4, Nome = "Mensagem Final", Tipo = TipoBloco.MensagemFinal, DuracaoMinutos = 10 }
                }
            });

            var dadosBase = new PlanoDeAulaDTO
            {
                PoloId = 1,
                Turma = 2,
                DataPrevista = DateTime.Today.AddDays(3)
            };

            var plano = await _service.CriarDeModelo(5, dadosBase);

            Assert.Equal("Aula padrão 65 min", plano.Titulo);
            Assert.Equal(65, plano.DuracaoTotalMinutos);
            Assert.Equal(StatusPlano.Rascunho, plano.Status);
            Assert.Equal(4, plano.Blocos.Count);
            Assert.Equal(TipoBloco.MensagemFinal, plano.Blocos.Last().Tipo);
            _planoRepositorio.Verify(r => r.CreateAsync(It.IsAny<PlanoDeAula>()), Times.Once);
        }

        [Fact]
        public async Task CriarDeModelo_ModeloInexistente_LancaDomainException()
        {
            _modeloRepositorio.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ModeloDeAula)null);

            await Assert.ThrowsAsync<DomainException>(
                () => _service.CriarDeModelo(99, new PlanoDeAulaDTO { PoloId = 1 }));
        }

        [Fact]
        public async Task Clonar_CopiaBlocosComNovaData_StatusRascunho()
        {
            var original = PlanoValido();
            original.Id = 7;
            original.Status = StatusPlano.Aplicado;
            _planoRepositorio.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(original);

            var novaData = DateTime.Today.AddDays(7);

            var clone = await _service.Clonar(7, novaData);

            Assert.Equal(original.Titulo, clone.Titulo);
            Assert.Equal(novaData, clone.DataPrevista);
            Assert.Equal(StatusPlano.Rascunho, clone.Status);
            Assert.Equal(original.Blocos.Count, clone.Blocos.Count);
        }

        [Fact]
        public async Task Clonar_CopiaAtividadesDosBlocos()
        {
            var original = PlanoValido();
            original.Id = 8;
            original.Blocos[1].Atividades = new List<AtividadeDoBloco>
            {
                new() { Id = 55, BlocoDoPlanoId = 2, AtividadeId = 10 },
                new() { Id = 56, BlocoDoPlanoId = 2, AtividadeId = 11 }
            };
            _planoRepositorio.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(original);

            var clone = await _service.Clonar(8, DateTime.Today.AddDays(7));

            var atividadesClonadas = clone.Blocos.Single(b => b.Ordem == 2).Atividades;
            Assert.Equal(2, atividadesClonadas.Count);
            Assert.Contains(atividadesClonadas, a => a.AtividadeId == 10);
            Assert.Contains(atividadesClonadas, a => a.AtividadeId == 11);
            // Os vínculos clonados são registros novos, não reutilizam os IDs originais
            Assert.All(atividadesClonadas, a => Assert.Equal(0, a.Id));
        }

        [Fact]
        public async Task Clonar_PlanoInexistente_LancaDomainException()
        {
            _planoRepositorio.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((PlanoDeAula)null);

            await Assert.ThrowsAsync<DomainException>(
                () => _service.Clonar(99, DateTime.Today));
        }

        [Fact]
        public void ModeloDeAula_BlocosExcedemDuracaoTotal_LancaDomainException()
        {
            var modelo = new ModeloDeAula
            {
                Nome = "Modelo curto",
                DuracaoTotalMinutos = 30,
                Blocos = new List<BlocoDoModelo>
                {
                    new() { Ordem = 1, Nome = "Aquecimento", Tipo = TipoBloco.Aquecimento, DuracaoMinutos = 20 },
                    new() { Ordem = 2, Nome = "Lutas", Tipo = TipoBloco.Lutas, DuracaoMinutos = 20 }
                }
            };

            Assert.Throws<DomainException>(() => modelo.Validate());
        }
    }
}
