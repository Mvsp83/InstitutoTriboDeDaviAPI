using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class GovernancaService : IGovernancaService
    {
        private readonly IMapper _mapper;
        private readonly IGovernancaRepository _repository;

        public GovernancaService(IMapper mapper, IGovernancaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<MembroGovernancaDTO>> ListarPorAno(int ano)
        {
            var membros = await _repository.ListarPorAnoAsync(ano);
            return _mapper.Map<List<MembroGovernancaDTO>>(membros);
        }

        public async Task<List<int>> ListarAnos()
        {
            return await _repository.ListarAnosAsync();
        }

        public async Task Salvar(int ano, List<MembroGovernancaDTO> membros)
        {
            // Slots em branco não viram registro. Cada membro é validado.
            var entidades = (membros ?? new List<MembroGovernancaDTO>())
                .Where(m => !string.IsNullOrWhiteSpace(m.Nome))
                .Select(m =>
                {
                    var e = new MembroGovernanca
                    {
                        Ano = ano,
                        Orgao = m.Orgao,
                        Cargo = (m.Cargo ?? string.Empty).Trim(),
                        Nome = m.Nome.Trim(),
                        Ordem = m.Ordem,
                    };
                    e.Validate();
                    return e;
                })
                .ToList();

            await _repository.SubstituirAnoAsync(ano, entidades);
        }

        public async Task<GovernancaPublicaDTO> ObterPublico(int? ano)
        {
            var anos = await _repository.ListarAnosAsync();
            var alvo = ano ?? (anos.Count > 0 ? anos[0] : 0);

            var membros = alvo > 0
                ? await _repository.ListarPorAnoAsync(alvo)
                : new List<MembroGovernanca>();

            return new GovernancaPublicaDTO
            {
                Ano = alvo,
                Anos = anos,
                Membros = _mapper.Map<List<MembroGovernancaDTO>>(membros),
            };
        }
    }
}
