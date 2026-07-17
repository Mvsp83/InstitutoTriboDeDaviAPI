using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher<UsuarioDTO> _passwordHasher;

        public UsuarioService(IMapper mapper, IUsuarioRepository usuarioRepository, IPasswordHasher<UsuarioDTO> passwordHasher)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioDTO> Create(UsuarioDTO usuarioDTO)
        {
            var usuarioExists = await _usuarioRepository.GetByEmail(usuarioDTO.Email);

            if (usuarioExists != null)
            {
                throw new DomainException("Já existe um registro com o mesmo Email informado!");
            }

            // Política de senha validada sobre o texto, antes do hash
            if (string.IsNullOrWhiteSpace(usuarioDTO.Password) || usuarioDTO.Password.Length < 8 || usuarioDTO.Password.Length > 100)
            {
                throw new DomainException("O Password deve ter entre 8 e 100 caracteres.");
            }

            var usuario = _mapper.Map<Usuario>(usuarioDTO);

            usuario.SenhaHash = _passwordHasher.HashPassword(_mapper.Map<UsuarioDTO>(usuario), usuarioDTO.Password);

            usuario.Validate();

            var usuarioCreated = await _usuarioRepository.CreateAsync(usuario);

            return _mapper.Map<UsuarioDTO>(usuarioCreated);
        }

        public async Task Delete(long id)
        {
            await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<UsuarioDTO> Get(long id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<List<UsuarioDTO>> GetAll()
        {
            var allUsuarios = await _usuarioRepository.GetAllAsync();

            return _mapper.Map<List<UsuarioDTO>>(allUsuarios);
        }

        public async Task<UsuarioDTO> GetByEmail(string email)
        {
            var usuario = await _usuarioRepository.GetByEmail(email);

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<List<UsuarioDTO>> SearchByEmail(string email)
        {
            var allUsuarios = await _usuarioRepository.SearchByEmail(email);

            return _mapper.Map<List<UsuarioDTO>>(allUsuarios);
        }

        public async Task<UsuarioDTO> Update(UsuarioUpdateDTO userDTO)
        {
            var usuarioExists = await _usuarioRepository.GetByIdAsync(userDTO.Id);

            if (usuarioExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            usuarioExists.Login = userDTO.Login;
            usuarioExists.Email = userDTO.Email;

            // Campos opcionais: nulos significam "manter o valor atual"
            if (userDTO.Role.HasValue)
                usuarioExists.Role = userDTO.Role.Value;

            if (userDTO.PoloId.HasValue)
                usuarioExists.PoloId = userDTO.PoloId.Value;

            if (!string.IsNullOrWhiteSpace(userDTO.PoloNome))
                usuarioExists.PoloNome = userDTO.PoloNome;

            // Senha só é alterada se uma nova for enviada; SenhaHash é preservado
            if (!string.IsNullOrWhiteSpace(userDTO.Password))
            {
                if (userDTO.Password.Length < 8 || userDTO.Password.Length > 100)
                    throw new DomainException("O Password deve ter entre 8 e 100 caracteres.");

                usuarioExists.SenhaHash = _passwordHasher.HashPassword(_mapper.Map<UsuarioDTO>(usuarioExists), userDTO.Password);
            }

            usuarioExists.Validate();

            var usuarioUpdated = await _usuarioRepository.UpdateAsync(usuarioExists);

            return _mapper.Map<UsuarioDTO>(usuarioUpdated);
        }

        public async Task<UsuarioDTO> GetByNome(string nome)
        {
            var usuario = await _usuarioRepository.GetByNome(nome);

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<List<UsuarioDTO>> SearchByNome(string nome)
        {
            var allUsuarios = await _usuarioRepository.SearchByNome(nome);

            return _mapper.Map<List<UsuarioDTO>>(allUsuarios);
        }

        public async Task<UsuarioDTO> ValidarUsuarioAsync(string login, string password)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login);

            if (usuario == null)
                return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(_mapper.Map<UsuarioDTO>(usuario), usuario.SenhaHash, password);
            if (verificationResult != PasswordVerificationResult.Success)
                return null; 

            return new UsuarioDTO
            {
                Login = usuario.Login,
                Email = usuario.Email,
                Role = usuario.Role,
                PoloId = usuario.PoloId,
                PoloNome = usuario.PoloNome
            };
        }

        public async Task<List<UsuarioDTO>> ObterUsuariosPorTurmaAsync(UsuarioDTO usuarioDTO, List<int> turmas)
        {
            if (usuarioDTO.Role == UserRole.Administrador)
            {
                var listaTodos = await _usuarioRepository.ObterTodosAsync();

                return _mapper.Map<List<UsuarioDTO>>(listaTodos);
            }

            // Supervisor e Professor enxergam apenas o próprio polo
            if ((usuarioDTO.Role == UserRole.Professor || usuarioDTO.Role == UserRole.Supervisor) && usuarioDTO.PoloId.HasValue)
            {
                var listaPorPolo = await _usuarioRepository.ObterPorPoloTurmaAsync(usuarioDTO.PoloId.Value, turmas);

                return _mapper.Map<List<UsuarioDTO>>(listaPorPolo);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }

        public async Task<bool> ExisteQualquerUsuario()
        {
            return await _usuarioRepository.ExisteQualquerUsuario();
        }
    }
}
