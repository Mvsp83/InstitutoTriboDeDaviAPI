using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class RelatorioSalvoService : IRelatorioSalvoService
    {
        private readonly IMapper _mapper;
        private readonly IRelatorioSalvoRepository _relatorioRepository;

        public RelatorioSalvoService(IMapper mapper, IRelatorioSalvoRepository relatorioRepository)
        {
            _mapper = mapper;
            _relatorioRepository = relatorioRepository;
        }

        public async Task<List<RelatorioSalvoDTO>> GetPorUsuario(string usuarioLogin)
        {
            var relatorios = await _relatorioRepository.GetByUsuarioAsync(usuarioLogin);

            return _mapper.Map<List<RelatorioSalvoDTO>>(relatorios);
        }

        public async Task<RelatorioSalvoDTO> Create(RelatorioSalvoDTO relatorioDTO)
        {
            var relatorio = _mapper.Map<RelatorioSalvo>(relatorioDTO);

            relatorio.Validate();

            var relatorioCreated = await _relatorioRepository.CreateAsync(relatorio);

            return _mapper.Map<RelatorioSalvoDTO>(relatorioCreated);
        }

        public async Task Delete(long id, string usuarioLogin)
        {
            var relatorio = await _relatorioRepository.GetByIdAsync(id);

            if (relatorio == null)
            {
                throw new DomainException("Não existe um relatório com o ID informado!");
            }

            // Cada usuário só pode excluir os próprios relatórios
            if (!string.Equals(relatorio.UsuarioLogin, usuarioLogin, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Usuário não tem permissão para excluir este relatório.");
            }

            await _relatorioRepository.DeleteAsync(id);
        }
    }
}
