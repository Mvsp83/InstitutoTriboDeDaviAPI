using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes de doações e recibos:
    // - o recibo usa a numeração oficial e nasce aprovado
    // - não se emite recibo duas vezes nem para doação anônima
    // - doador com histórico não é excluído
    public class DoacaoServiceTests
    {
        private readonly Mock<IDoacaoRepository> _repo = new();
        private readonly Mock<IDocumentoOficialService> _documentos = new();
        private readonly DoacaoService _service;

        private DocumentoOficialDTO _documentoCriado;
        private Doacao _doacaoSalva;

        public DoacaoServiceTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Doador, DoadorDTO>().ReverseMap();
                cfg.CreateMap<Doacao, DoacaoDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _documentos.Setup(d => d.Create(It.IsAny<DocumentoOficialDTO>()))
                       .ReturnsAsync((DocumentoOficialDTO dto) =>
                       {
                           dto.Id = 77;
                           _documentoCriado = dto;
                           return dto;
                       });
            // A numeração só sai na aprovação, como no fluxo real.
            _documentos.Setup(d => d.Aprovar(77))
                       .ReturnsAsync(new DocumentoOficialDTO
                       {
                           Id = 77, Numero = 3, NumeroFormatado = "2026/0003", Status = 1
                       });

            _repo.Setup(r => r.SalvarDoacaoAsync(It.IsAny<Doacao>()))
                 .ReturnsAsync((Doacao d) => { _doacaoSalva = d; return d; });
            _repo.Setup(r => r.ListarDoadoresAsync()).ReturnsAsync(new List<Doador>());
            _repo.Setup(r => r.ListarDoacoesAsync(It.IsAny<int?>(), It.IsAny<long?>()))
                 .ReturnsAsync(new List<Doacao>());

            _service = new DoacaoService(mapper, _repo.Object, _documentos.Object);
        }

        private static Doador DoadorValido() => new()
        {
            Id = 1, Nome = "Maria Doadora", Documento = "123.456.789-00",
            Endereco = "Rua A, 10", Cidade = "Blumenau", Ativo = true,
        };

        private static Doacao DoacaoValida(long? doadorId = 1, long? reciboId = null) => new()
        {
            Id = 10, DoadorId = doadorId, Valor = 150m,
            Data = DateTime.Today, Forma = "Pix",
            ReciboDocumentoId = reciboId,
            ReciboNumero = reciboId.HasValue ? "2026/0001" : string.Empty,
        };

        [Fact]
        public async Task EmitirRecibo_UsaNumeracaoOficialEVinculaADoacao()
        {
            _repo.Setup(r => r.ObterDoacaoAsync(10)).ReturnsAsync(DoacaoValida());
            _repo.Setup(r => r.ObterDoadorAsync(1)).ReturnsAsync(DoadorValido());

            var r = await _service.EmitirRecibo(10);

            Assert.Equal("2026/0003", r.ReciboNumero);
            Assert.Equal(77, r.ReciboDocumentoId);
            // Tipo próprio: o recibo de doação tem sequência separada.
            Assert.Equal(2, _documentoCriado.Tipo);
            Assert.Contains("Maria Doadora", _documentoCriado.Titulo);
            // O recibo nasce aprovado — a doação já aconteceu.
            _documentos.Verify(d => d.Aprovar(77), Times.Once);
            Assert.Equal("2026/0003", _doacaoSalva.ReciboNumero);
        }

        [Fact]
        public async Task EmitirRecibo_LevaOsDadosDoDoadorNoConteudo()
        {
            _repo.Setup(r => r.ObterDoacaoAsync(10)).ReturnsAsync(DoacaoValida());
            _repo.Setup(r => r.ObterDoadorAsync(1)).ReturnsAsync(DoadorValido());

            await _service.EmitirRecibo(10);

            // O documento precisa do CPF/CNPJ: é o que dá valor fiscal ao recibo.
            Assert.Contains("123.456.789-00", _documentoCriado.Conteudo);
            Assert.Contains("Blumenau", _documentoCriado.Conteudo);
        }

        [Fact]
        public async Task EmitirRecibo_JaEmitido_Recusa()
        {
            _repo.Setup(r => r.ObterDoacaoAsync(10)).ReturnsAsync(DoacaoValida(reciboId: 99));

            await Assert.ThrowsAsync<DomainException>(() => _service.EmitirRecibo(10));
            _documentos.Verify(d => d.Create(It.IsAny<DocumentoOficialDTO>()), Times.Never);
        }

        [Fact]
        public async Task EmitirRecibo_DoacaoAnonima_Recusa()
        {
            _repo.Setup(r => r.ObterDoacaoAsync(10)).ReturnsAsync(DoacaoValida(doadorId: null));

            await Assert.ThrowsAsync<DomainException>(() => _service.EmitirRecibo(10));
            _documentos.Verify(d => d.Create(It.IsAny<DocumentoOficialDTO>()), Times.Never);
        }

        [Fact]
        public async Task EmitirRecibo_DoacaoInexistente_Recusa()
        {
            _repo.Setup(r => r.ObterDoacaoAsync(It.IsAny<long>())).ReturnsAsync((Doacao)null);

            await Assert.ThrowsAsync<DomainException>(() => _service.EmitirRecibo(10));
        }

        [Fact]
        public async Task SalvarDoacao_ValorZero_Recusa()
        {
            var dto = new DoacaoDTO { Valor = 0, Data = DateTime.Today, Forma = "Pix" };

            await Assert.ThrowsAsync<DomainException>(() => _service.SalvarDoacao(dto, "admin"));
        }

        [Fact]
        public async Task SalvarDoacao_DataFutura_Recusa()
        {
            var dto = new DoacaoDTO { Valor = 50, Data = DateTime.Today.AddDays(1), Forma = "Pix" };

            await Assert.ThrowsAsync<DomainException>(() => _service.SalvarDoacao(dto, "admin"));
        }

        [Fact]
        public async Task SalvarDoacao_FormaInvalida_Recusa()
        {
            var dto = new DoacaoDTO { Valor = 50, Data = DateTime.Today, Forma = "Bitcoin" };

            await Assert.ThrowsAsync<DomainException>(() => _service.SalvarDoacao(dto, "admin"));
        }

        [Fact]
        public async Task SalvarDoacao_Anonima_EAceita()
        {
            var dto = new DoacaoDTO { Valor = 50, Data = DateTime.Today, Forma = "Pix", DoadorId = null };

            var r = await _service.SalvarDoacao(dto, "admin");

            Assert.Null(r.DoadorId);
            Assert.Equal("admin", _doacaoSalva.RegistradoPor);
        }

        [Fact]
        public async Task ExcluirDoador_ComHistorico_Recusa()
        {
            _repo.Setup(r => r.ExcluirDoadorAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<DomainException>(() => _service.ExcluirDoador(1));
        }

        [Fact]
        public async Task ListarDoadores_CalculaTotalEUltimaDoacao()
        {
            _repo.Setup(r => r.ListarDoadoresAsync()).ReturnsAsync(new List<Doador> { DoadorValido() });
            _repo.Setup(r => r.ListarDoacoesAsync(null, null)).ReturnsAsync(new List<Doacao>
            {
                new() { Id = 1, DoadorId = 1, Valor = 100m, Data = new DateTime(2026, 1, 10) },
                new() { Id = 2, DoadorId = 1, Valor = 50m,  Data = new DateTime(2026, 5, 20) },
                new() { Id = 3, DoadorId = null, Valor = 30m, Data = new DateTime(2026, 6, 1) },
            });

            var lista = await _service.ListarDoadores();

            Assert.Equal(150m, lista[0].TotalDoado);
            Assert.Equal(2, lista[0].QuantidadeDoacoes);
            Assert.Equal(new DateTime(2026, 5, 20), lista[0].UltimaDoacao);
        }

        [Fact]
        public async Task Resumo_SomaTotalEContaDoadoresDistintos()
        {
            _repo.Setup(r => r.ListarDoacoesAsync(2026, null)).ReturnsAsync(new List<Doacao>
            {
                new() { DoadorId = 1, Valor = 100m, Data = new DateTime(2026, 1, 1) },
                new() { DoadorId = 1, Valor = 200m, Data = new DateTime(2026, 2, 1) },
                new() { DoadorId = 2, Valor = 300m, Data = new DateTime(2026, 3, 1) },
                new() { DoadorId = null, Valor = 100m, Data = new DateTime(2026, 4, 1) },
            });

            var r = await _service.Resumo(2026);

            Assert.Equal(700m, r.Total);
            Assert.Equal(4, r.Quantidade);
            Assert.Equal(2, r.Doadores); // anônima não conta como doador
            Assert.Equal(175m, r.TicketMedio);
        }
    }
}
