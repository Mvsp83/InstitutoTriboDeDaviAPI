using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ConfiguracaoDashboardService : IConfiguracaoDashboardService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguracaoDashboardRepository _repository;

        public ConfiguracaoDashboardService(IMapper mapper, IConfiguracaoDashboardRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<ConfiguracaoDashboardDTO> Obter(string usuarioLogin)
        {
            var configuracao = await _repository.ObterPorUsuarioAsync(usuarioLogin);

            // Ainda não personalizou: devolve layout vazio (o portal aplica o padrão).
            if (configuracao == null)
            {
                return new ConfiguracaoDashboardDTO
                {
                    UsuarioLogin = usuarioLogin,
                    Layout = string.Empty
                };
            }

            return _mapper.Map<ConfiguracaoDashboardDTO>(configuracao);
        }

        public async Task<ConfiguracaoDashboardDTO> Salvar(string usuarioLogin, ConfiguracaoDashboardDTO dto)
        {
            var entidade = _mapper.Map<ConfiguracaoDashboard>(dto);
            // O dono é sempre o usuário autenticado, ignorando o que vier no body.
            entidade.UsuarioLogin = usuarioLogin;

            entidade.Validate();

            var salvo = await _repository.SalvarAsync(entidade);
            return _mapper.Map<ConfiguracaoDashboardDTO>(salvo);
        }
    }
}
