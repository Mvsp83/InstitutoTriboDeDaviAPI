using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Infrastructure.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes de regressão dos bugs corrigidos no update/create de usuário:
    // - update apagava o SenhaHash do banco
    // - update sem role/polo promovia o usuário a Administrador (enum = 0) e apagava o polo
    // - senha enviada no update era ignorada (nunca re-hasheada)
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositorio = new();
        private readonly IPasswordHasher<UsuarioDTO> _hasher = new PasswordHasher<UsuarioDTO>();
        private readonly UsuarioService _service;
        private Usuario? _usuarioSalvo;

        public UsuarioServiceTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap();
            }, NullLoggerFactory.Instance).CreateMapper();

            // Captura a entidade passada ao UpdateAsync/CreateAsync para inspeção
            _repositorio
                .Setup(r => r.UpdateAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => _usuarioSalvo = u)
                .ReturnsAsync((Usuario u) => u);

            _repositorio
                .Setup(r => r.CreateAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => _usuarioSalvo = u)
                .ReturnsAsync((Usuario u) => u);

            _service = new UsuarioService(mapper, _repositorio.Object, _hasher, new TotpService());
        }

        private Usuario UsuarioExistente() => new()
        {
            Id = 1,
            Login = "professor1",
            Email = "professor@teste.com",
            SenhaHash = "HASH_ORIGINAL_PRESERVADO",
            Role = UserRole.Professor,
            PoloId = 2,
            PoloNome = "Polo Central"
        };

        private static UsuarioUpdateDTO UpdateBasico() => new()
        {
            Id = 1,
            Login = "professor1",
            Email = "professor@teste.com"
        };

        [Fact]
        public async Task Update_SemPassword_PreservaSenhaHash()
        {
            _repositorio.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(UsuarioExistente());

            await _service.Update(UpdateBasico());

            Assert.NotNull(_usuarioSalvo);
            Assert.Equal("HASH_ORIGINAL_PRESERVADO", _usuarioSalvo!.SenhaHash);
        }

        [Fact]
        public async Task Update_ComPassword_GeraNovoHashVerificavel()
        {
            _repositorio.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(UsuarioExistente());

            var dto = UpdateBasico();
            dto.Password = "novaSenha123";

            await _service.Update(dto);

            Assert.NotNull(_usuarioSalvo);
            Assert.NotEqual("HASH_ORIGINAL_PRESERVADO", _usuarioSalvo!.SenhaHash);

            var verificacao = _hasher.VerifyHashedPassword(new UsuarioDTO(), _usuarioSalvo.SenhaHash, "novaSenha123");
            Assert.Equal(PasswordVerificationResult.Success, verificacao);
        }

        [Fact]
        public async Task Update_SemRoleEPolo_PreservaValoresAtuais()
        {
            _repositorio.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(UsuarioExistente());

            await _service.Update(UpdateBasico());

            Assert.NotNull(_usuarioSalvo);
            Assert.Equal(UserRole.Professor, _usuarioSalvo!.Role);
            Assert.Equal(2, _usuarioSalvo.PoloId);
            Assert.Equal("Polo Central", _usuarioSalvo.PoloNome);
        }

        [Fact]
        public async Task Update_ComRole_AtualizaRole()
        {
            _repositorio.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(UsuarioExistente());

            var dto = UpdateBasico();
            dto.Role = UserRole.Supervisor;

            await _service.Update(dto);

            Assert.Equal(UserRole.Supervisor, _usuarioSalvo!.Role);
        }

        [Fact]
        public async Task Update_IdInexistente_LancaDomainException()
        {
            _repositorio.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Usuario?)null);

            var dto = UpdateBasico();
            dto.Id = 99;

            await Assert.ThrowsAsync<DomainException>(() => _service.Update(dto));
        }

        [Fact]
        public async Task Update_PasswordCurto_LancaDomainException()
        {
            _repositorio.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(UsuarioExistente());

            var dto = UpdateBasico();
            dto.Password = "ab";

            await Assert.ThrowsAsync<DomainException>(() => _service.Update(dto));
        }

        [Fact]
        public async Task Create_EmailJaExistente_LancaDomainException()
        {
            _repositorio.Setup(r => r.GetByEmail("professor@teste.com")).ReturnsAsync(UsuarioExistente());

            var dto = new UsuarioDTO
            {
                Login = "outro",
                Email = "professor@teste.com",
                Password = "senha123"
            };

            await Assert.ThrowsAsync<DomainException>(() => _service.Create(dto));
        }

        [Fact]
        public async Task Create_PasswordCurto_LancaDomainException()
        {
            _repositorio.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((Usuario?)null);

            var dto = new UsuarioDTO
            {
                Login = "usuario1",
                Email = "usuario@teste.com",
                Password = "ab"
            };

            await Assert.ThrowsAsync<DomainException>(() => _service.Create(dto));
        }

        [Fact]
        public async Task Create_Valido_GeraHashVerificavel()
        {
            _repositorio.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((Usuario?)null);

            var dto = new UsuarioDTO
            {
                Login = "usuario1",
                Email = "usuario@teste.com",
                Password = "senha123",
                Role = UserRole.Professor
            };

            await _service.Create(dto);

            Assert.NotNull(_usuarioSalvo);
            var verificacao = _hasher.VerifyHashedPassword(new UsuarioDTO(), _usuarioSalvo!.SenhaHash, "senha123");
            Assert.Equal(PasswordVerificationResult.Success, verificacao);
        }
    }
}
