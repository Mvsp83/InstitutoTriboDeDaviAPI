using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Entities.Consultas;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes da biblioteca de atividades:
    // - validação de domínio (nome obrigatório, limites de tamanho)
    // - histórico por turma mapeado para DTO
    public class AtividadeTests
    {
        private readonly Mock<IAtividadeRepository> _atividadeRepositorio = new();
        private readonly AtividadeService _service;

        public AtividadeTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Atividade, AtividadeDTO>().ReverseMap();
                cfg.CreateMap<HistoricoAtividade, HistoricoAtividadeDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _service = new AtividadeService(mapper, _atividadeRepositorio.Object);
        }

        [Fact]
        public void Atividade_Valida_RetornaTrue()
        {
            var atividade = new Atividade
            {
                Nome = "Armlock da guarda fechada",
                Tipo = TipoBloco.Posicoes,
                Tags = "infantil, guarda, finalização"
            };

            Assert.True(atividade.Validate());
        }

        [Fact]
        public void Atividade_SemNome_LancaDomainException()
        {
            var atividade = new Atividade { Nome = "", Tipo = TipoBloco.Lutas };

            Assert.Throws<DomainException>(() => atividade.Validate());
        }

        [Fact]
        public void Atividade_MensagemFinal_ComPrincipioEReferencia_Valida()
        {
            var atividade = new Atividade
            {
                Nome = "Perseverança",
                Tipo = TipoBloco.MensagemFinal,
                Principio = "Perseverar mesmo quando é difícil",
                ReferenciaBiblica = "Tiago 1:12"
            };

            Assert.True(atividade.Validate());
        }

        [Fact]
        public async Task HistoricoTurma_MapeiaParaDTO()
        {
            _atividadeRepositorio
                .Setup(r => r.ObterHistoricoTurmaAsync(1, 2))
                .ReturnsAsync(new List<HistoricoAtividade>
                {
                    new()
                    {
                        AtividadeId = 10,
                        Nome = "Armlock da guarda fechada",
                        Tipo = TipoBloco.Posicoes,
                        UltimaData = new DateTime(2026, 6, 15),
                        Vezes = 3
                    }
                });

            var historico = await _service.ObterHistoricoTurmaAsync(1, 2);

            Assert.Single(historico);
            Assert.Equal(10, historico[0].AtividadeId);
            Assert.Equal(3, historico[0].Vezes);
            Assert.Equal(new DateTime(2026, 6, 15), historico[0].UltimaData);
        }

        [Fact]
        public async Task Update_AtividadeInexistente_LancaDomainException()
        {
            _atividadeRepositorio.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Atividade)null);

            await Assert.ThrowsAsync<DomainException>(
                () => _service.Update(new AtividadeDTO { Id = 99, Nome = "Teste" }));
        }
    }
}
