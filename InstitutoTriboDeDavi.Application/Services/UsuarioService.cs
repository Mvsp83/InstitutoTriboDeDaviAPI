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
        private readonly ITotpService _totpService;

        public UsuarioService(IMapper mapper, IUsuarioRepository usuarioRepository, IPasswordHasher<UsuarioDTO> passwordHasher, ITotpService totpService)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _totpService = totpService;
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

            if (userDTO.PermiteGraduacao.HasValue)
                usuarioExists.PermiteGraduacao = userDTO.PermiteGraduacao.Value;

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
                Id = usuario.Id,
                Login = usuario.Login,
                Email = usuario.Email,
                Role = usuario.Role,
                PoloId = usuario.PoloId,
                PoloNome = usuario.PoloNome,
                PermiteGraduacao = usuario.PermiteGraduacao,
                TotpConfirmado = usuario.TotpConfirmado
            };
        }

        // ── 2FA (TOTP) ──────────────────────────────────────────────────────

        public async Task<Setup2FADTO> Iniciar2FAAsync(string login)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login)
                ?? throw new DomainException("Usuário não encontrado.");

            if (usuario.TotpConfirmado)
                throw new DomainException("O 2FA já está ativo. Desative-o antes de gerar um novo.");

            // Gera (ou regenera) um secret ainda não confirmado. Só passa a valer
            // no login depois que o usuário confirmar o primeiro código.
            var secret = _totpService.GerarSecret();
            usuario.TotpSecret = secret;
            usuario.TotpConfirmado = false;
            await _usuarioRepository.UpdateAsync(usuario);

            return new Setup2FADTO
            {
                Secret = secret,
                Uri = _totpService.GerarUri(secret, usuario.Login, "Instituto Tribo de Davi"),
            };
        }

        public async Task Confirmar2FAAsync(string login, string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login)
                ?? throw new DomainException("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(usuario.TotpSecret))
                throw new DomainException("Inicie a configuração do 2FA antes de confirmar.");

            if (!_totpService.Validar(usuario.TotpSecret, codigo))
                throw new DomainException("Código inválido. Confira o app autenticador e tente de novo.");

            usuario.TotpConfirmado = true;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task Desativar2FAAsync(string login, string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login)
                ?? throw new DomainException("Usuário não encontrado.");

            if (!usuario.TotpConfirmado || string.IsNullOrWhiteSpace(usuario.TotpSecret))
                throw new DomainException("O 2FA não está ativo.");

            // Exige um código válido para desligar — evita que uma sessão
            // sequestrada desative a proteção sem o app autenticador.
            if (!_totpService.Validar(usuario.TotpSecret, codigo))
                throw new DomainException("Código inválido.");

            usuario.TotpSecret = null;
            usuario.TotpConfirmado = false;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task<bool> Status2FAAsync(string login)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login);
            return usuario?.TotpConfirmado ?? false;
        }

        public async Task<bool> ValidarCodigo2FAAsync(string login, string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login);
            if (usuario == null || !usuario.TotpConfirmado || string.IsNullOrWhiteSpace(usuario.TotpSecret))
                return false;

            return _totpService.Validar(usuario.TotpSecret, codigo);
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

        // Máximo de presets disponíveis no front (preset:1..preset:12).
        private const int TotalPresets = 12;
        // Limite do data URI (~40 KB de imagem em base64). Mantém o banco leve.
        private const int TamanhoMaximoAvatar = 55_000;
        // Foto do perfil público (rosto do professor). O front comprime para
        // ~50 KB; a folga permite uma foto com um pouco mais de qualidade.
        private const int TamanhoMaximoFotoSite = 120_000;

        public async Task<string?> ObterAvatarAsync(string login)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login);
            return usuario?.Avatar;
        }

        public async Task AtualizarAvatarAsync(string login, string? avatar)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login)
                ?? throw new DomainException("Usuário não encontrado.");

            usuario.Avatar = NormalizarAvatar(avatar);

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task<PerfilSiteDTO> ObterMeuPerfilSiteAsync(string login)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login);
            return new PerfilSiteDTO
            {
                Nome = usuario?.Nome,
                Faixa = usuario?.Faixa,
                FotoSite = usuario?.FotoSite,
                MostrarNoSite = usuario?.MostrarNoSite ?? false,
            };
        }

        public async Task AtualizarMeuPerfilSiteAsync(string login, PerfilSiteDTO dto)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorLoginAsync(login)
                ?? throw new DomainException("Usuário não encontrado.");

            usuario.Nome = string.IsNullOrWhiteSpace(dto.Nome) ? null : dto.Nome.Trim();
            usuario.Faixa = dto.Faixa;
            usuario.FotoSite = NormalizarFotoSite(dto.FotoSite);
            // Só aparece no site se, de fato, tiver foto.
            usuario.MostrarNoSite = dto.MostrarNoSite && usuario.FotoSite != null;

            await _usuarioRepository.UpdateAsync(usuario);
        }

        // Foto do perfil público: só data URI de imagem (png/jpeg/webp) dentro do
        // limite; vazio ou formato não suportado = sem foto. Barra data URI grande
        // (o front comprime, mas a API não pode confiar só no cliente).
        private static string? NormalizarFotoSite(string? foto)
        {
            if (string.IsNullOrWhiteSpace(foto))
                return null;

            foto = foto.Trim();

            var prefixosValidos = new[]
            {
                "data:image/png;base64,",
                "data:image/jpeg;base64,",
                "data:image/webp;base64,"
            };

            if (!prefixosValidos.Any(p => foto.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                return null;

            if (foto.Length > TamanhoMaximoFotoSite)
                throw new DomainException("A foto do perfil é grande demais. Escolha uma foto menor.");

            return foto;
        }

        // Aceita apenas: vazio (limpa), "preset:N" dentro do intervalo, ou um
        // data URI de imagem dentro do limite. URLs externas são recusadas.
        private static string? NormalizarAvatar(string? avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar))
                return null;

            avatar = avatar.Trim();

            if (avatar.StartsWith("preset:", StringComparison.OrdinalIgnoreCase))
            {
                var numero = avatar["preset:".Length..];
                if (int.TryParse(numero, out var n) && n >= 1 && n <= TotalPresets)
                    return $"preset:{n}";

                throw new DomainException("Avatar de preset inválido.");
            }

            var prefixosValidos = new[]
            {
                "data:image/png;base64,",
                "data:image/jpeg;base64,",
                "data:image/webp;base64,"
            };

            if (prefixosValidos.Any(p => avatar.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                if (avatar.Length > TamanhoMaximoAvatar)
                    throw new DomainException("A imagem do avatar é grande demais. Escolha uma foto menor.");

                return avatar;
            }

            throw new DomainException("Formato de avatar não suportado.");
        }
    }
}
