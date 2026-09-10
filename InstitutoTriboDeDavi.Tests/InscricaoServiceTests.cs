using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes da inscrição online:
    // - o envio público não aceita campos de controle vindos de fora
    // - a aprovação vira aluno + matrícula do ano
    // - o revisor pode corrigir o polo escolhido errado pela família
    // - rematrícula atualiza o aluno existente em vez de duplicar
    // - inscrição já revisada não é revisada de novo
    public class InscricaoServiceTests
    {
        private readonly Mock<IInscricaoRepository> _inscricoes = new();
        private readonly Mock<IAlunoRepository> _alunos = new();
        private readonly Mock<IPoloRepository> _polos = new();
        private readonly InscricaoService _service;

        // Guarda o que foi enviado ao repositório na aprovação, para inspecionar.
        private Aluno _alunoSalvo;
        private Matricula _matriculaSalva;

        public InscricaoServiceTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Inscricao, InscricaoDTO>().ReverseMap();
                cfg.CreateMap<Matricula, MatriculaDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            _polos.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                  .ReturnsAsync((long id) => new Polo { Id = id, Nome = $"Polo {id}" });
            _polos.Setup(r => r.GetAllAsync())
                  .ReturnsAsync(new List<Polo> { new() { Id = 1, Nome = "Polo 1" }, new() { Id = 2, Nome = "Polo 2" } });

            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno>());

            _inscricoes.Setup(r => r.ContarEnviosRecentesAsync(It.IsAny<string>(), It.IsAny<int>()))
                       .ReturnsAsync(0);
            _inscricoes.Setup(r => r.CriarAsync(It.IsAny<Inscricao>()))
                       .ReturnsAsync((Inscricao i) => { i.Id = 99; return i; });
            _inscricoes.Setup(r => r.AprovarAsync(It.IsAny<Inscricao>(), It.IsAny<Aluno>(), It.IsAny<Matricula>()))
                       .ReturnsAsync((Inscricao _, Aluno a, Matricula m) =>
                       {
                           if (a.Id == 0) a.Id = 500; // id gerado pelo banco
                           m.AlunoId = a.Id;
                           _alunoSalvo = a;
                           _matriculaSalva = m;
                           return (a, m);
                       });

            _service = new InscricaoService(mapper, _inscricoes.Object, _alunos.Object, _polos.Object);
        }

        private static InscricaoDTO FichaValida() => new()
        {
            PoloId = 1,
            Nome = "Maria da Silva",
            DataNascimento = new DateTime(2015, 5, 20),
            Cpf = "123.456.789-00",
            Faixa = 10,
            Escola = "EEB Teste",
            Periodo = "Matutino",
            Parentesco = (int)Parentesco.Mae,
            NomeResponsavel = "Joana da Silva",
            WhatsApp = "47999998888",
            Rua = "Rua A",
            Numero = "10",
            Bairro = "Centro",
            Cidade = "Blumenau",
            AceitouTermo = true,
            AceitouLgpd = true,
            NomeAssinatura = "Joana da Silva",
        };

        private static Inscricao Pendente(long id = 1, long poloId = 1) => new()
        {
            Id = id,
            Ano = DateTime.Today.Year,
            PoloId = poloId,
            Nome = "Maria da Silva",
            DataNascimento = new DateTime(2015, 5, 20),
            Cpf = "123.456.789-00",
            Faixa = 10,
            Escola = "EEB Teste",
            Periodo = "Matutino",
            NomeResponsavel = "Joana da Silva",
            WhatsApp = "47999998888",
            Rua = "Rua A",
            Numero = "10",
            Complemento = "casa 2",
            Bairro = "Centro",
            Cidade = "Blumenau",
            AceitouTermo = true,
            AceitouLgpd = true,
            NomeAssinatura = "Joana da Silva",
            Status = (int)StatusInscricao.Pendente,
        };

        // ── Envio ─────────────────────────────────────────────────────────
        [Fact]
        public async Task Enviar_IgnoraCamposDeControleVindosDeFora()
        {
            var dto = FichaValida();
            // Uma ficha maliciosa tentando entrar já aprovada e ligada a um aluno.
            dto.Status = (int)StatusInscricao.Aprovada;
            dto.AlunoId = 777;
            dto.Ano = 1999;

            await _service.Enviar(dto);

            _inscricoes.Verify(r => r.CriarAsync(It.Is<Inscricao>(i =>
                i.Status == (int)StatusInscricao.Pendente
                && i.AlunoId == null
                && i.Ano == DateTime.Today.Year)), Times.Once);
        }

        [Fact]
        public async Task Enviar_GeraCodigoDeAcessoDoResponsavel()
        {
            var resultado = await _service.Enviar(FichaValida());

            // A família recebe o código no fim, e ele fica gravado na inscrição.
            Assert.False(string.IsNullOrEmpty(resultado.CodigoResponsavel));
            _inscricoes.Verify(r => r.CriarAsync(It.Is<Inscricao>(i =>
                !string.IsNullOrEmpty(i.CodigoResponsavel))), Times.Once);
        }

        [Fact]
        public async Task Aprovar_LevaAutorizacaoDeImagemParaOAluno()
        {
            var ficha = Pendente();
            ficha.AceitouImagem = true;
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.True(_alunoSalvo.AutorizaImagem);
        }

        [Fact]
        public async Task Aprovar_HerdaOCodigoGeradoNaInscricao()
        {
            var ficha = Pendente();
            ficha.CodigoResponsavel = "ABCD2345";
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.Equal("ABCD2345", _alunoSalvo.CodigoResponsavel);
        }

        [Fact]
        public async Task Enviar_SemConsentimentoLgpd_Recusa()
        {
            var dto = FichaValida();
            dto.AceitouLgpd = false;

            await Assert.ThrowsAsync<DomainException>(() => _service.Enviar(dto));
            _inscricoes.Verify(r => r.CriarAsync(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task Enviar_MuitosEnviosDoMesmoContato_Recusa()
        {
            _inscricoes.Setup(r => r.ContarEnviosRecentesAsync(It.IsAny<string>(), It.IsAny<int>()))
                       .ReturnsAsync(3);

            await Assert.ThrowsAsync<DomainException>(() => _service.Enviar(FichaValida()));
            _inscricoes.Verify(r => r.CriarAsync(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task Enviar_PoloInexistente_Recusa()
        {
            _polos.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Polo)null);

            await Assert.ThrowsAsync<DomainException>(() => _service.Enviar(FichaValida()));
        }

        [Fact]
        public async Task Enviar_PoloLotadoContandoAtivasEPendentes_Recusa()
        {
            _polos.Setup(r => r.GetByIdAsync(1))
                  .ReturnsAsync(new Polo { Id = 1, Nome = "Polo 1", LimiteAlunos = 10 });
            // 8 ativas + 2 pendentes = 10 (>= limite) → sem vaga.
            _inscricoes.Setup(r => r.ContarMatriculasAtivasAsync(It.IsAny<int>(), 1)).ReturnsAsync(8);
            _inscricoes.Setup(r => r.ContarInscricoesPendentesAsync(It.IsAny<int>(), 1)).ReturnsAsync(2);

            await Assert.ThrowsAsync<DomainException>(() => _service.Enviar(FichaValida()));
        }

        [Fact]
        public async Task Enviar_ComVagaAindaContandoPendentes_Aceita()
        {
            _polos.Setup(r => r.GetByIdAsync(1))
                  .ReturnsAsync(new Polo { Id = 1, Nome = "Polo 1", LimiteAlunos = 10 });
            // 7 ativas + 2 pendentes = 9 (< limite) → ainda há vaga.
            _inscricoes.Setup(r => r.ContarMatriculasAtivasAsync(It.IsAny<int>(), 1)).ReturnsAsync(7);
            _inscricoes.Setup(r => r.ContarInscricoesPendentesAsync(It.IsAny<int>(), 1)).ReturnsAsync(2);

            var resultado = await _service.Enviar(FichaValida());

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task Enviar_AdultoEmPoloSemTurmaDeAdultos_Recusa()
        {
            _polos.Setup(r => r.GetByIdAsync(1))
                  .ReturnsAsync(new Polo { Id = 1, Nome = "Polo 1", AceitaAdultos = false });
            var dto = FichaValida();
            dto.Publico = 1; // adulto

            await Assert.ThrowsAsync<DomainException>(() => _service.Enviar(dto));
        }

        [Fact]
        public async Task Enviar_AdultoEmPoloComTurmaDeAdultos_Aceita()
        {
            _polos.Setup(r => r.GetByIdAsync(1))
                  .ReturnsAsync(new Polo { Id = 1, Nome = "Polo 1", AceitaAdultos = true });
            var dto = FichaValida();
            dto.Publico = 1; // adulto

            var resultado = await _service.Enviar(dto);

            Assert.NotNull(resultado);
        }

        // ── Aprovação ─────────────────────────────────────────────────────
        [Fact]
        public async Task Aprovar_CriaAlunoEMatriculaDoAno()
        {
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(Pendente());

            var matricula = await _service.Aprovar(
                1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 2 }, "professor.teste");

            Assert.Equal("Maria da Silva", _alunoSalvo.Nome);
            Assert.Equal(1, _alunoSalvo.PoloId);
            Assert.Equal(2, _alunoSalvo.Turma);
            // Endereço vai para campos separados, como veio da ficha.
            Assert.Equal("Rua A", _alunoSalvo.Endereco);
            Assert.Equal("10", _alunoSalvo.Numero);
            Assert.Equal("casa 2", _alunoSalvo.Complemento);

            Assert.Equal(DateTime.Today.Year, _matriculaSalva.Ano);
            Assert.Equal(2, _matriculaSalva.Turma);
            Assert.True(_matriculaSalva.Ativa);
            Assert.Equal(1, _matriculaSalva.InscricaoId);
            Assert.NotNull(matricula);
        }

        [Fact]
        public async Task Aprovar_LevaOsCamposNovosParaOAluno()
        {
            var ficha = Pendente();
            ficha.Altura = 1.42m;
            ficha.Serie = "5o ano";
            ficha.Telefone2 = "4733334444";
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.Equal(1.42, _alunoSalvo.Altura);
            Assert.Equal("5o ano", _alunoSalvo.Serie);
            Assert.Equal("4733334444", _alunoSalvo.Telefone2);
        }

        [Fact]
        public async Task Aprovar_CorrigindoPolo_UsaOPoloDoRevisor()
        {
            // A família marcou o polo 1; o revisor corrige para o 2.
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(Pendente(poloId: 1));

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 2, Turma = 1 }, "admin");

            Assert.Equal(2, _alunoSalvo.PoloId);
            Assert.Equal(2, _matriculaSalva.PoloId);
            // A inscrição também passa a apontar para o destino corrigido.
            _inscricoes.Verify(r => r.AprovarAsync(
                It.Is<Inscricao>(i => i.PoloId == 2 && i.Turma == 1),
                It.IsAny<Aluno>(), It.IsAny<Matricula>()), Times.Once);
        }

        [Fact]
        public async Task Aprovar_AlunoJaExiste_AtualizaEmVezDeDuplicar()
        {
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(Pendente());
            // Mesmo CPF, formatado diferente: ainda é a mesma pessoa.
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno>
            {
                new() { Id = 42, Nome = "Maria da Silva", CPF = "12345678900", DataNascimento = new DateTime(2015, 5, 20) }
            });

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.Equal(42, _alunoSalvo.Id);
            Assert.Equal(42, _matriculaSalva.AlunoId);
        }

        [Fact]
        public async Task Aprovar_SemCpf_CasaPorNomeENascimento()
        {
            var ficha = Pendente();
            ficha.Cpf = "";
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno>
            {
                new() { Id = 7, Nome = "maria da silva", DataNascimento = new DateTime(2015, 5, 20) }
            });

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.Equal(7, _alunoSalvo.Id);
        }

        [Fact]
        public async Task Aprovar_OutraDataDeNascimento_CriaAlunoNovo()
        {
            var ficha = Pendente();
            ficha.Cpf = "";
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);
            // Mesmo nome, nascimento diferente: são pessoas distintas.
            _alunos.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Aluno>
            {
                new() { Id = 7, Nome = "Maria da Silva", DataNascimento = new DateTime(2010, 1, 1) }
            });

            await _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin");

            Assert.Equal(500, _alunoSalvo.Id); // id novo, gerado no "banco"
        }

        [Fact]
        public async Task Aprovar_InscricaoJaRevisada_Recusa()
        {
            var ficha = Pendente();
            ficha.Status = (int)StatusInscricao.Aprovada;
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(ficha);

            await Assert.ThrowsAsync<DomainException>(() =>
                _service.Aprovar(1, new RevisaoInscricaoDTO { PoloId = 1, Turma = 1 }, "admin"));
        }

        // ── Recusa ────────────────────────────────────────────────────────
        [Fact]
        public async Task Recusar_SemMotivo_Recusa()
        {
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(Pendente());

            await Assert.ThrowsAsync<DomainException>(() => _service.Recusar(1, "  ", "admin"));
            _inscricoes.Verify(r => r.AtualizarAsync(It.IsAny<Inscricao>()), Times.Never);
        }

        [Fact]
        public async Task Recusar_ComMotivo_GravaStatusERevisor()
        {
            _inscricoes.Setup(r => r.ObterAsync(1)).ReturnsAsync(Pendente());

            await _service.Recusar(1, "Fora da faixa etária", "admin");

            _inscricoes.Verify(r => r.AtualizarAsync(It.Is<Inscricao>(i =>
                i.Status == (int)StatusInscricao.Recusada
                && i.RevisadoPor == "admin"
                && i.ObservacaoRevisao == "Fora da faixa etária")), Times.Once);
        }
    }
}
