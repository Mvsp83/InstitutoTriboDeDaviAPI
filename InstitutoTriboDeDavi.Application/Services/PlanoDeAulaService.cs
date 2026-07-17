using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class PlanoDeAulaService : IPlanoDeAulaService
    {
        private readonly IMapper _mapper;
        private readonly IPlanoDeAulaRepository _planoRepository;
        private readonly IModeloDeAulaRepository _modeloRepository;

        public PlanoDeAulaService(IMapper mapper, IPlanoDeAulaRepository planoRepository, IModeloDeAulaRepository modeloRepository)
        {
            _mapper = mapper;
            _planoRepository = planoRepository;
            _modeloRepository = modeloRepository;
        }

        public async Task<PlanoDeAulaDTO> Create(PlanoDeAulaDTO planoDTO)
        {
            var plano = _mapper.Map<PlanoDeAula>(planoDTO);

            plano.Validate();

            var planoCreated = await _planoRepository.CreateAsync(plano);

            return _mapper.Map<PlanoDeAulaDTO>(planoCreated);
        }

        public async Task<PlanoDeAulaDTO> Update(PlanoDeAulaDTO planoDTO)
        {
            var planoExists = await _planoRepository.GetByIdAsync(planoDTO.Id);

            if (planoExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var plano = _mapper.Map<PlanoDeAula>(planoDTO);

            plano.Validate();

            var planoUpdated = await _planoRepository.UpdateAsync(plano);

            return _mapper.Map<PlanoDeAulaDTO>(planoUpdated);
        }

        public async Task<PlanoDeAulaDTO> Get(long id)
        {
            var plano = await _planoRepository.GetByIdAsync(id);

            return _mapper.Map<PlanoDeAulaDTO>(plano);
        }

        public async Task<List<PlanoDeAulaDTO>> GetAll()
        {
            var allPlanos = await _planoRepository.GetAllAsync();

            return _mapper.Map<List<PlanoDeAulaDTO>>(allPlanos);
        }

        public async Task Delete(long id)
        {
            await _planoRepository.DeleteAsync(id);
        }

        public async Task<List<PlanoDeAulaDTO>> ObterPlanosTurmaAsync(UsuarioDTO usuarioDTO, IEnumerable<int> turmas)
        {
            if (usuarioDTO.Role == UserRole.Administrador)
            {
                var listaTodos = await _planoRepository.ObterTodosAsync();

                return _mapper.Map<List<PlanoDeAulaDTO>>(listaTodos);
            }

            // Supervisor e Professor enxergam apenas o próprio polo
            if ((usuarioDTO.Role == UserRole.Professor || usuarioDTO.Role == UserRole.Supervisor) && usuarioDTO.PoloId.HasValue)
            {
                var listaPorPolo = await _planoRepository.ObterPorPoloTurmaAsync(usuarioDTO.PoloId.Value, turmas);

                return _mapper.Map<List<PlanoDeAulaDTO>>(listaPorPolo);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }

        public async Task<PlanoDeAulaDTO> CriarDeModelo(long modeloId, PlanoDeAulaDTO dadosBase)
        {
            var modelo = await _modeloRepository.GetByIdAsync(modeloId);

            if (modelo == null)
            {
                throw new DomainException("Não existe um Modelo de Aula com o ID informado!");
            }

            var plano = new PlanoDeAula
            {
                PoloId = dadosBase.PoloId,
                Turma = dadosBase.Turma,
                Titulo = string.IsNullOrWhiteSpace(dadosBase.Titulo) ? modelo.Nome : dadosBase.Titulo,
                Objetivo = dadosBase.Objetivo,
                DataPrevista = dadosBase.DataPrevista,
                DuracaoTotalMinutos = modelo.DuracaoTotalMinutos,
                Status = StatusPlano.Rascunho,
                Blocos = modelo.Blocos
                    .OrderBy(b => b.Ordem)
                    .Select(b => new BlocoDoPlano
                    {
                        Ordem = b.Ordem,
                        Nome = b.Nome,
                        Tipo = b.Tipo,
                        DuracaoMinutos = b.DuracaoMinutos,
                        Descricao = b.Descricao
                    })
                    .ToList()
            };

            plano.Validate();

            var planoCreated = await _planoRepository.CreateAsync(plano);

            return _mapper.Map<PlanoDeAulaDTO>(planoCreated);
        }

        public async Task<PlanoDeAulaDTO> Clonar(long planoId, DateTime novaDataPrevista)
        {
            var original = await _planoRepository.GetByIdAsync(planoId);

            if (original == null)
            {
                throw new DomainException("Não existe um Plano de Aula com o ID informado!");
            }

            var clone = new PlanoDeAula
            {
                PoloId = original.PoloId,
                Turma = original.Turma,
                Titulo = original.Titulo,
                Objetivo = original.Objetivo,
                DataPrevista = novaDataPrevista,
                DuracaoTotalMinutos = original.DuracaoTotalMinutos,
                Status = StatusPlano.Rascunho,
                Blocos = original.Blocos
                    .OrderBy(b => b.Ordem)
                    .Select(b => new BlocoDoPlano
                    {
                        Ordem = b.Ordem,
                        Nome = b.Nome,
                        Tipo = b.Tipo,
                        DuracaoMinutos = b.DuracaoMinutos,
                        Descricao = b.Descricao,
                        Atividades = b.Atividades
                            .Select(a => new AtividadeDoBloco { AtividadeId = a.AtividadeId })
                            .ToList()
                    })
                    .ToList()
            };

            clone.Validate();

            var planoCreated = await _planoRepository.CreateAsync(clone);

            return _mapper.Map<PlanoDeAulaDTO>(planoCreated);
        }
    }
}
