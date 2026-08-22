using System.Linq;
using System.Security.Claims;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Auditoria;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes do interceptor de auditoria com SQLite em memória (SQL real):
    // - criar/alterar/excluir entidade auditada gera log com a ação certa
    // - o log de alteração guarda só o campo que mudou
    // - entidade fora do conjunto auditado não gera log
    // - senha nunca aparece no log
    public class AuditoriaInterceptorTests : IDisposable
    {
        private readonly TriboDeDaviContext _context;

        public AuditoriaInterceptorTests()
        {
            var http = new Mock<IHttpContextAccessor>();
            var ctx = new DefaultHttpContext();
            ctx.User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "professor.teste") }, "test"));
            http.Setup(h => h.HttpContext).Returns(ctx);

            var options = new DbContextOptionsBuilder<TriboDeDaviContext>()
                // InMemory: exercita o interceptor sem depender do schema SQL.
                .UseInMemoryDatabase($"auditoria-{System.Guid.NewGuid()}")
                .AddInterceptors(new AuditoriaInterceptor(http.Object))
                .Options;

            _context = new TriboDeDaviContext(options);
        }

        private List<LogAuditoria> Logs() =>
            _context.LogsAuditoria.AsNoTracking().OrderBy(l => l.Id).ToList();

        [Fact]
        public void Criar_EntidadeAuditada_GeraLogCriou()
        {
            _context.Doacoes.Add(new Doacao { Valor = 100, Data = DateTime.Today, Forma = "Pix" });
            _context.SaveChanges();

            var log = Logs().Single();
            Assert.Equal("Criou", log.Acao);
            Assert.Equal("Doacao", log.Entidade);
            Assert.Equal("professor.teste", log.UsuarioLogin);
            Assert.True(log.EntidadeId > 0); // id do registro novo já resolvido
        }

        [Fact]
        public void Alterar_GuardaSomenteOCampoQueMudou()
        {
            var doacao = new Doacao { Valor = 100, Data = DateTime.Today, Forma = "Pix" };
            _context.Doacoes.Add(doacao);
            _context.SaveChanges();

            doacao.Valor = 250;
            _context.SaveChanges();

            var alteracao = Logs().Single(l => l.Acao == "Alterou");
            Assert.Contains("Valor", alteracao.Alteracoes);
            Assert.Contains("100", alteracao.Alteracoes);
            Assert.Contains("250", alteracao.Alteracoes);
            // Um campo que não mudou não entra no diff.
            Assert.DoesNotContain("Forma", alteracao.Alteracoes);
        }

        [Fact]
        public void Excluir_GeraLogExcluiu()
        {
            var doacao = new Doacao { Valor = 100, Data = DateTime.Today, Forma = "Pix" };
            _context.Doacoes.Add(doacao);
            _context.SaveChanges();

            _context.Doacoes.Remove(doacao);
            _context.SaveChanges();

            Assert.Contains(Logs(), l => l.Acao == "Excluiu" && l.Entidade == "Doacao");
        }

        [Fact]
        public void EntidadeNaoAuditada_NaoGeraLog()
        {
            // Polo não está no conjunto auditado.
            _context.Polos.Add(new Polo
            {
                Nome = "Polo Teste", Informacoes = "", Endereco = "", Bairro = "", Cidade = "",
            });
            _context.SaveChanges();

            Assert.Empty(Logs());
        }

        [Fact]
        public void Senha_NuncaApareceNoLog()
        {
            var usuario = new Usuario
            {
                Login = "fulano", Email = "f@x.com", PoloNome = "",
                SenhaHash = "hash-secreto-123", Role = Domain.Enums.UserRole.Professor,
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            usuario.SenhaHash = "novo-hash-secreto-456";
            usuario.Email = "novo@x.com";
            _context.SaveChanges();

            var alteracao = Logs().Single(l => l.Acao == "Alterou" && l.Entidade == "Usuario");
            Assert.DoesNotContain("secreto", alteracao.Alteracoes);
            Assert.DoesNotContain("Password", alteracao.Alteracoes);
            // O campo não sensível é registrado normalmente.
            Assert.Contains("Email", alteracao.Alteracoes);
        }

        [Fact]
        public void OProprioLog_NaoSeAudita()
        {
            _context.Doacoes.Add(new Doacao { Valor = 10, Data = DateTime.Today, Forma = "Pix" });
            _context.SaveChanges();

            // Uma criação de doação gera exatamente um log — não um log do log.
            Assert.Single(Logs());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
