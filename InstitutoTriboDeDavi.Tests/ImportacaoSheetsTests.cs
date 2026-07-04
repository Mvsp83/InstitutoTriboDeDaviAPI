using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Import;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes do import do Google Sheets — o mapeamento por índice de coluna é o
    // ponto mais frágil do sistema: uma coluna nova no formulário desloca tudo.
    // Estes testes fixam o contrato de colunas atual.
    public class ImportacaoSheetsTests
    {
        private readonly Mock<IAlunoRepository> _alunoRepositorio = new();
        private readonly Mock<IPoloRepository> _poloRepositorio = new();
        private readonly FactoryPlanilhaDB _factory;
        private Aluno? _alunoSalvo;

        public ImportacaoSheetsTests()
        {
            _alunoRepositorio
                .Setup(r => r.CreateAsync(It.IsAny<Aluno>()))
                .Callback<Aluno>(a => _alunoSalvo = a)
                .ReturnsAsync((Aluno a) => a);

            _alunoRepositorio
                .Setup(r => r.UpdateAsync(It.IsAny<Aluno>()))
                .Callback<Aluno>(a => _alunoSalvo = a)
                .ReturnsAsync((Aluno a) => a);

            _factory = new FactoryPlanilhaDB(_alunoRepositorio.Object, _poloRepositorio.Object);
        }

        // Monta uma linha do Sheets com 26 colunas nas posições que o import espera
        private static IList<object> Linha(
            string? nome = "Aluno de Teste",
            string? turma = null,
            string? dataNasc = null,
            string? rg = null,
            string? cpf = null,
            string? peso = null,
            string? faixa = null,
            string? graus = null,
            string? parentesco = null,
            string? rua = null,
            string? numero = null,
            string? complemento = null,
            string? bairro = null,
            string? cidade = null,
            string? celular = null)
        {
            var linha = new object[26];
            linha[3] = turma ?? "";
            linha[4] = nome ?? "";
            linha[5] = dataNasc ?? "";
            linha[6] = rg ?? "";
            linha[7] = cpf ?? "";
            linha[9] = peso ?? "";
            linha[11] = faixa ?? "";
            linha[12] = graus ?? "";
            linha[16] = parentesco ?? "";
            linha[20] = rua ?? "";
            linha[21] = numero ?? "";
            linha[22] = complemento ?? "";
            linha[23] = bairro ?? "";
            linha[24] = cidade ?? "";
            linha[25] = celular ?? "";
            return linha;
        }

        private static IList<object> Cabecalho() => Linha(nome: "NOME");

        [Fact]
        public async Task Import_PulaCabecalho_E_IgnoraLinhaSemNome()
        {
            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: ""), // sem nome → ignorada
            };

            var resultado = await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Equal(1, resultado.Ignorados);
            Assert.Equal(0, resultado.Inseridos);
            _alunoRepositorio.Verify(r => r.CreateAsync(It.IsAny<Aluno>()), Times.Never);
        }

        [Fact]
        public async Task Import_AlunoNovo_MapeiaColunasCorretamente()
        {
            _alunoRepositorio.Setup(r => r.GetByNome(It.IsAny<string>())).ReturnsAsync((Aluno?)null);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(
                    nome: "João da Silva",
                    turma: "Turma 2",
                    rg: "12.345.678-9",
                    cpf: "123.456.789-00",
                    faixa: "Cinza",
                    graus: "2",
                    parentesco: "Mãe",
                    rua: "Rua das Flores",
                    numero: "123",
                    complemento: "Casa",
                    bairro: "Centro",
                    cidade: "Blumenau",
                    celular: "(47) 99999-0000"),
            };

            var resultado = await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 7);

            Assert.Equal(1, resultado.Inseridos);
            Assert.NotNull(_alunoSalvo);
            Assert.Equal("João da Silva", _alunoSalvo!.Nome);
            Assert.Equal(2, _alunoSalvo.Turma);
            Assert.Equal("12.345.678-9", _alunoSalvo.RG);
            Assert.Equal("123.456.789-00", _alunoSalvo.CPF);
            Assert.Equal(Faixa.Cinza2, _alunoSalvo.Faixa);
            Assert.Equal(Parentesco.Mae, _alunoSalvo.Parentesco);
            Assert.Equal("Rua das Flores, 123, Casa", _alunoSalvo.Endereco);
            Assert.Equal("Centro", _alunoSalvo.Bairro);
            Assert.Equal("Blumenau", _alunoSalvo.Cidade);
            Assert.Equal("(47) 99999-0000", _alunoSalvo.Celular);
            Assert.Equal(7, _alunoSalvo.PoloId);
        }

        [Fact]
        public async Task Import_AlunoExistenteComTurma_NaoSobrescreveTurma()
        {
            var existente = new Aluno { Id = 1, Nome = "João da Silva", Turma = 3 };
            _alunoRepositorio.Setup(r => r.GetByNome("João da Silva")).ReturnsAsync(existente);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: "João da Silva", turma: "Turma 1"),
            };

            var resultado = await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Equal(1, resultado.Atualizados);
            Assert.Equal(3, _alunoSalvo!.Turma); // turma existente preservada
        }

        [Fact]
        public async Task Import_AlunoExistentePendente_RecebeTurmaDaPlanilha()
        {
            var existente = new Aluno { Id = 1, Nome = "João da Silva", Turma = 0 };
            _alunoRepositorio.Setup(r => r.GetByNome("João da Silva")).ReturnsAsync(existente);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: "João da Silva", turma: "Turma 2"),
            };

            await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Equal(2, _alunoSalvo!.Turma);
        }

        [Fact]
        public async Task Import_CelulaNAO_ViraNull()
        {
            _alunoRepositorio.Setup(r => r.GetByNome(It.IsAny<string>())).ReturnsAsync((Aluno?)null);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: "Maria", rg: "NÃO", cpf: "não"),
            };

            await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Null(_alunoSalvo!.RG);
            Assert.Null(_alunoSalvo.CPF);
        }

        [Fact]
        public async Task Import_TurmaInvalida_ViraZeroPendente()
        {
            _alunoRepositorio.Setup(r => r.GetByNome(It.IsAny<string>())).ReturnsAsync((Aluno?)null);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: "Maria", turma: "ainda não sei"),
            };

            await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Equal(0, _alunoSalvo!.Turma);
        }

        [Fact]
        public async Task Import_LinhaComErro_RegistraErroEContinuaAsDemais()
        {
            _alunoRepositorio.Setup(r => r.GetByNome("Aluno Problema")).ThrowsAsync(new Exception("falha simulada"));
            _alunoRepositorio.Setup(r => r.GetByNome("Aluno Ok")).ReturnsAsync((Aluno?)null);

            var rows = new List<IList<object>>
            {
                Cabecalho(),
                Linha(nome: "Aluno Problema"),
                Linha(nome: "Aluno Ok"),
            };

            var resultado = await _factory.ImportarAlunosDeSheetsAsync(rows, poloId: 1);

            Assert.Single(resultado.Erros);
            Assert.Equal(1, resultado.Inseridos);
        }
    }
}
