using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IMapper mapper, IUsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> Create(UsuarioDTO usuarioDTO)
        {
            var usuarioExists = await _usuarioRepository.GetByEmail(usuarioDTO.Email);

            if (usuarioExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Email informado!");
            }

            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            usuario.DataCadastro = DateTime.Now;

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

        public async Task<List<UsuarioDTO>> SearchEmail(string email)
        {
            var allUsuarios = await _usuarioRepository.SearchEmail(email);

            return _mapper.Map<List<UsuarioDTO>>(allUsuarios);
        }

        public async Task<UsuarioDTO> Update(UsuarioDTO userDTO)
        {
            var usuarioExists = await _usuarioRepository.GetByIdAsync(userDTO.Id);

            if (usuarioExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var usuario = _mapper.Map<Usuario>(userDTO);
            usuario.DataAtualizacao = DateTime.Now;

            var usuarioUpdated = await _usuarioRepository.UpdateAsync(usuario);

            return _mapper.Map<UsuarioDTO>(usuarioUpdated);
        }
    }
}
