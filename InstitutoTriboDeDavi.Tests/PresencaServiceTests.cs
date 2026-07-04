using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes do salvamento atômico de presenças:
    // - o lote grava presenças e marca a aula na mesma operação
    // - aula já salva rejeita o lote (trava anti-duplicação)
    public class PresencaServiceTests
    {
        private readonly Mock<IPresencaRepository> _presencaRepositorio = new();
        private readonly Mock<IAulaRepository> _aulaRepositorio = new();
        private readonly PresencaService _service;

        public PresencaServiceTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Presenca, PresencaDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _presencaRepositorio
                .Setup(r => r.CreateBatchComAulaAsync(It.IsAny<IEnumerable<Presenca>>(), It.IsAny<long>()))
                .ReturnsAsync((IEnumerable<Presenca> p, long _) => p.ToList());

            _service = new PresencaService(mapper, _presencaRepositorio.Object, _aulaRepositorio.Object);
        }

        private static PresencaDTO PresencaValida(long alunoId, long aulaId = 10) => new()
        {
            AlunoId = alunoId,
            PoloId = 1,
            AulaId = aulaId,
            Data = DateTime.Today,
            EstaPresente = true
        };

        [Fact]
        public async Task CreateBatch_Valido_SalvaEMarcaAula()
        {
            _aulaRepositorio.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Aula { Id = 10, PresencaSalva = false });

            var lote = new[] { PresencaValida(1), PresencaValida(2) };

            var resultado = await _service.CreateBatch(lote);

            Assert.Equal(2, resultado.Count);
            _presencaRepositorio.Verify(
                r => r.CreateBatchComAulaAsync(It.Is<IEnumerable<Presenca>>(p => p.Count() == 2), 10),
                Times.Once);
        }

        [Fact]
        public async Task CreateBatch_AulaJaSalva_RejeitaLote()
        {
            _aulaRepositorio.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Aula { Id = 10, PresencaSalva = true });

            var lote = new[] { PresencaValida(1) };

            var ex = await Assert.ThrowsAsync<DomainException>(() => _service.CreateBatch(lote));
            Assert.Contains("já foram salvas", ex.Message);

            _presencaRepositorio.Verify(
                r => r.CreateBatchComAulaAsync(It.IsAny<IEnumerable<Presenca>>(), It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateBatch_AulaInexistente_LancaDomainException()
        {
            _aulaRepositorio.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aula?)null);

            var lote = new[] { PresencaValida(1, aulaId: 99) };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateBatch(lote));
        }

        [Fact]
        public async Task CreateBatch_AulasDiferentesNoMesmoLote_LancaDomainException()
        {
            var lote = new[] { PresencaValida(1, aulaId: 10), PresencaValida(2, aulaId: 11) };

            await Assert.ThrowsAsync<DomainException>(() => _service.CreateBatch(lote));
        }

        [Fact]
        public async Task CreateBatch_LoteVazio_LancaDomainException()
        {
            await Assert.ThrowsAsync<DomainException>(
                () => _service.CreateBatch(Array.Empty<PresencaDTO>()));
        }
    }
}
