using System;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Infrastructure.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OtpNet;
using Xunit;

namespace InstitutoTriboDeDavi.Tests
{
    // A4 — 2FA (TOTP) e refresh tokens com revogação. InMemory para os fluxos
    // que tocam o banco; o TotpService é testado direto.
    public class AuthSeguranca2FATests : IDisposable
    {
        private readonly TriboDeDaviContext _context;

        public AuthSeguranca2FATests()
        {
            var options = new DbContextOptionsBuilder<TriboDeDaviContext>()
                .UseInMemoryDatabase($"auth-{Guid.NewGuid()}")
                .Options;
            _context = new TriboDeDaviContext(options);
        }

        // Código que o app autenticador mostraria para este secret, agora.
        private static string CodigoValido(string secret) =>
            new Totp(Base32Encoding.ToBytes(secret)).ComputeTotp();

        // ── TOTP ────────────────────────────────────────────────────────────

        [Fact]
        public void Totp_ValidaCodigoCerto_RejeitaErrado()
        {
            var totp = new TotpService();
            var secret = totp.GerarSecret();

            Assert.True(totp.Validar(secret, CodigoValido(secret)));
            Assert.False(totp.Validar(secret, "000000"));
            Assert.False(totp.Validar(secret, ""));
        }

        // ── Refresh token ───────────────────────────────────────────────────

        [Fact]
        public async Task Refresh_Rotaciona_ERevogaOAntigo()
        {
            var svc = new RefreshTokenService(new RefreshTokenRepository(_context));

            var original = await svc.EmitirAsync(1);
            var rotacao = await svc.RotacionarAsync(original);

            Assert.NotNull(rotacao);
            Assert.Equal(1, rotacao.UsuarioId);
            Assert.NotEqual(original, rotacao.NovoTokenRaw);

            // O token antigo não vale mais (rotação); o novo, sim.
            Assert.Null(await svc.RotacionarAsync(original));
            Assert.NotNull(await svc.RotacionarAsync(rotacao.NovoTokenRaw));
        }

        [Fact]
        public async Task Refresh_RevogarUsuario_InvalidaTodasAsSessoes()
        {
            var svc = new RefreshTokenService(new RefreshTokenRepository(_context));

            var t1 = await svc.EmitirAsync(7);
            var t2 = await svc.EmitirAsync(7);

            var revogadas = await svc.RevogarUsuarioAsync(7);

            Assert.Equal(2, revogadas);
            Assert.Null(await svc.RotacionarAsync(t1));
            Assert.Null(await svc.RotacionarAsync(t2));
        }

        [Fact]
        public async Task Refresh_TokenInexistente_RetornaNull()
        {
            var svc = new RefreshTokenService(new RefreshTokenRepository(_context));
            Assert.Null(await svc.RotacionarAsync("nao-existe"));
            Assert.Null(await svc.RotacionarAsync(""));
        }

        // ── 2FA no fluxo de login ───────────────────────────────────────────

        private UsuarioService NovoUsuarioService()
        {
            var mapper = new MapperConfiguration(
                cfg => cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap(),
                NullLoggerFactory.Instance).CreateMapper();

            return new UsuarioService(
                mapper,
                new UsuarioRepository(_context),
                new PasswordHasher<UsuarioDTO>(),
                new TotpService());
        }

        [Fact]
        public async Task Ativar2FA_ConfirmaEPassaAExigirNoLogin()
        {
            var hasher = new PasswordHasher<UsuarioDTO>();
            _context.Usuarios.Add(new Usuario
            {
                Login = "admin",
                Email = "a@x.com",
                PoloNome = "",
                SenhaHash = hasher.HashPassword(new UsuarioDTO(), "SenhaForte8"),
                Role = UserRole.Administrador,
            });
            _context.SaveChanges();

            var svc = NovoUsuarioService();

            // Antes de ativar: senha certa, login não exige segundo fator.
            var antes = await svc.ValidarUsuarioAsync("admin", "SenhaForte8");
            Assert.NotNull(antes);
            Assert.False(antes.TotpConfirmado);

            // Inicia (secret criado, ainda não confirmado) e confirma.
            var setup = await svc.Iniciar2FAAsync("admin");
            Assert.False(await svc.Status2FAAsync("admin"));
            await svc.Confirmar2FAAsync("admin", CodigoValido(setup.Secret));
            Assert.True(await svc.Status2FAAsync("admin"));

            // Depois: o login sinaliza 2FA e o código do app é aceito.
            var depois = await svc.ValidarUsuarioAsync("admin", "SenhaForte8");
            Assert.True(depois.TotpConfirmado);
            Assert.True(await svc.ValidarCodigo2FAAsync("admin", CodigoValido(setup.Secret)));
            Assert.False(await svc.ValidarCodigo2FAAsync("admin", "000000"));
        }

        [Fact]
        public async Task Confirmar2FA_ComCodigoErrado_Lanca()
        {
            _context.Usuarios.Add(new Usuario
            {
                Login = "u", Email = "u@x.com", PoloNome = "", SenhaHash = "h", Role = UserRole.Professor,
            });
            _context.SaveChanges();

            var svc = NovoUsuarioService();
            await svc.Iniciar2FAAsync("u");

            await Assert.ThrowsAsync<DomainException>(() => svc.Confirmar2FAAsync("u", "000000"));
            Assert.False(await svc.Status2FAAsync("u"));
        }

        public void Dispose() => _context.Dispose();
    }
}
