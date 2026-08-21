using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AvisoService : IAvisoService
    {
        private readonly IMapper _mapper;
        private readonly IAvisoRepository _repository;

        public AvisoService(IMapper mapper, IAvisoRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<AvisoDTO>> GetAll()
        {
            return _mapper.Map<List<AvisoDTO>>(await _repository.ObterTodosAsync());
        }

        public async Task<AvisoDTO> Create(AvisoDTO dto, UsuarioDTO usuario)
        {
            var aviso = _mapper.Map<Aviso>(dto);
            aviso.DataCriacao = DateTime.Now;
            aviso.CriadoPor = usuario.Login;
            aviso.Ativo = true;
            aviso.Validate();
            var criado = await _repository.CreateAsync(aviso);
            return _mapper.Map<AvisoDTO>(criado);
        }

        public async Task Delete(long id) => await _repository.DeleteAsync(id);

        public async Task<List<AvisoDTO>> ObterPendentes(UsuarioDTO usuario)
        {
            var ativos = await _repository.ObterAtivosAsync();
            var cientes = await _repository.ObterCientesDoUsuarioAsync(usuario.Login);

            var pendentes = ativos
                .Where(a => !cientes.Contains(a.Id) && PublicoAlcanca(a.PublicoAlvo, usuario.Role))
                .ToList();

            return _mapper.Map<List<AvisoDTO>>(pendentes);
        }

        public async Task MarcarCiente(long avisoId, UsuarioDTO usuario)
        {
            await _repository.RegistrarCienteAsync(avisoId, usuario.Login);
        }

        private static bool PublicoAlcanca(int publicoAlvo, UserRole role)
        {
            return publicoAlvo == (int)PublicoAviso.Todos
                || (publicoAlvo == (int)PublicoAviso.Professores && role == UserRole.Professor)
                || (publicoAlvo == (int)PublicoAviso.Supervisores && role == UserRole.Supervisor);
        }
    }
}
