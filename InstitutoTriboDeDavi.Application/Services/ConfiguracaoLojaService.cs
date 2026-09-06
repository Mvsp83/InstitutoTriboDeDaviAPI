using AutoMapper;
using System.Text.RegularExpressions;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ConfiguracaoLojaService : IConfiguracaoLojaService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguracaoLojaRepository _repository;

        public ConfiguracaoLojaService(IMapper mapper, IConfiguracaoLojaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<ConfiguracaoLojaDTO> Obter()
        {
            var configuracao = await _repository.ObterAsync();

            // Ainda não configurada: devolve o padrão (desabilitada, sem número).
            if (configuracao == null)
            {
                return new ConfiguracaoLojaDTO
                {
                    CompraWhatsappHabilitada = false,
                    WhatsappNumero = string.Empty
                };
            }

            return _mapper.Map<ConfiguracaoLojaDTO>(configuracao);
        }

        public async Task<ConfiguracaoLojaDTO> Salvar(ConfiguracaoLojaDTO dto)
        {
            var entidade = _mapper.Map<ConfiguracaoLoja>(dto);
            // Guarda só os dígitos (o front pode mandar com máscara/espaços).
            entidade.WhatsappNumero = Regex.Replace(dto.WhatsappNumero ?? string.Empty, @"\D", "");

            entidade.Validate();

            var salvo = await _repository.SalvarAsync(entidade);
            return _mapper.Map<ConfiguracaoLojaDTO>(salvo);
        }
    }
}
