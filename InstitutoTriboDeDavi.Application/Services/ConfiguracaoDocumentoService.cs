using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ConfiguracaoDocumentoService : IConfiguracaoDocumentoService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguracaoDocumentoRepository _repository;

        public ConfiguracaoDocumentoService(IMapper mapper, IConfiguracaoDocumentoRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<ConfiguracaoDocumentoDTO> Obter()
        {
            var configuracao = await _repository.ObterAsync();

            // Ainda não configurado: devolve o padrão inicial do instituto.
            if (configuracao == null)
            {
                return new ConfiguracaoDocumentoDTO
                {
                    TituloCabecalho = "INSTITUTO TRIBO DE DAVI",
                    LinhaExtra = string.Empty,
                    TextoRodape = "Instituto Tribo de Davi",
                    MostrarLogo = true,
                    MostrarDataGeracao = true
                };
            }

            return _mapper.Map<ConfiguracaoDocumentoDTO>(configuracao);
        }

        public async Task<ConfiguracaoDocumentoDTO> Salvar(ConfiguracaoDocumentoDTO dto)
        {
            var entidade = _mapper.Map<ConfiguracaoDocumento>(dto);
            var salvo = await _repository.SalvarAsync(entidade);
            return _mapper.Map<ConfiguracaoDocumentoDTO>(salvo);
        }
    }
}
