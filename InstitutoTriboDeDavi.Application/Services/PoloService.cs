using AutoMapper;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class PoloService : IPoloService
    {
        private readonly IMapper _mapper;
        private readonly IPoloRepository _poloRepository;
        private readonly IInscricaoRepository _inscricaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PoloService(
            IMapper mapper,
            IPoloRepository poloRepository,
            IInscricaoRepository inscricaoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _poloRepository = poloRepository;
            _inscricaoRepository = inscricaoRepository;
            _usuarioRepository = usuarioRepository;
        }
        public async Task<PoloDTO> Create(PoloDTO poloDTO)
        {
            var poloExists = await _poloRepository.GetByNome(poloDTO.Nome);

            if (poloExists != null)
            {
                throw new DomainException("Já existe um registro com o mesmo Nome informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            polo.Validate();

            var poloCreated = await _poloRepository.CreateAsync(polo);

            return _mapper.Map<PoloDTO>(poloCreated);
        }

        public async Task Delete(long id)
        {
            await _poloRepository.DeleteAsync(id);
        }

        public async Task<PoloDTO> Get(long id)
        {
            var polo = await _poloRepository.GetByIdAsync(id);

            return _mapper.Map<PoloDTO>(polo);
        }

        public async Task<List<PoloDTO>> GetAll()
        {
            var allPolos = await _poloRepository.GetAllAsync();
            var dtos = _mapper.Map<List<PoloDTO>>(allPolos);

            // Ocupação: matrículas ativas + inscrições pendentes do ano por polo.
            var ano = DateTime.Now.Year;
            var ativosPorPolo = await _inscricaoRepository.ContarMatriculasAtivasPorPoloAsync(ano);
            var pendentesPorPolo = await _inscricaoRepository.ContarInscricoesPendentesPorPoloAsync(ano);
            foreach (var dto in dtos)
            {
                dto.AlunosAtivos = ativosPorPolo.TryGetValue(dto.Id, out var n) ? n : 0;
                dto.InscricoesPendentes = pendentesPorPolo.TryGetValue(dto.Id, out var p) ? p : 0;
            }

            return dtos;
        }

        public async Task<PoloDTO> GetByNome(string nome)
        {
            var polo = await _poloRepository.GetByNome(nome);

            return _mapper.Map<PoloDTO>(polo);
        }

        public async Task<List<PoloDTO>> SearchByNome(string nome)
        {
            var allPolos = await _poloRepository.SearchByNome(nome);

            return _mapper.Map<List<PoloDTO>>(allPolos);
        }

        public async Task<PoloDTO> Update(PoloDTO poloDTO)
        {
            var poloExists = await _poloRepository.GetByIdAsync(poloDTO.Id);

            if (poloExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var polo = _mapper.Map<Polo>(poloDTO);

            polo.Validate();

            // Update customizado: substitui os horários por turma (a coleção-filho
            // não é tratada pelo UpdateAsync base).
            var poloUpdated = await _poloRepository.AtualizarComHorariosAsync(polo);

            return _mapper.Map<PoloDTO>(poloUpdated);
        }

        public async Task<List<PoloDTO>> ObterPolosAsync(UsuarioDTO usuarioDTO, List<int> turmas)
        {
            var listaTodos = await _poloRepository.ObterTodosAsync();
            var dtos = _mapper.Map<List<PoloDTO>>(listaTodos);

            // Ocupação: matrículas ativas + inscrições pendentes do ano por polo
            // (mesma regra do GetAll) — é a listagem que a tela de polos consome.
            var ano = DateTime.Now.Year;
            var ativosPorPolo = await _inscricaoRepository.ContarMatriculasAtivasPorPoloAsync(ano);
            var pendentesPorPolo = await _inscricaoRepository.ContarInscricoesPendentesPorPoloAsync(ano);
            foreach (var dto in dtos)
            {
                dto.AlunosAtivos = ativosPorPolo.TryGetValue(dto.Id, out var n) ? n : 0;
                dto.InscricoesPendentes = pendentesPorPolo.TryGetValue(dto.Id, out var p) ? p : 0;
            }

            return dtos;
        }

        public async Task<List<PoloPublicoDetalhadoDTO>> ListarPublicos()
        {
            var polos = await _poloRepository.ObterTodosAsync(); // já inclui horários

            // Professores que optaram por aparecer no site e têm foto de rosto.
            var usuarios = await _usuarioRepository.ObterTodosAsync();
            var professoresPorPolo = usuarios
                .Where(u => u.MostrarNoSite
                    && !string.IsNullOrEmpty(u.FotoSite)
                    && u.PoloId != null)
                .GroupBy(u => u.PoloId!.Value)
                .ToDictionary(g => g.Key, g => g
                    .OrderBy(u => u.Nome)
                    .Select(u => new ProfessorPublicoDTO
                    {
                        Nome = u.Nome,
                        Faixa = u.Faixa,
                        Foto = u.FotoSite,
                    })
                    .ToList());

            return polos
                .OrderBy(p => p.Nome)
                .Select(p => new PoloPublicoDetalhadoDTO
                {
                    Nome = p.Nome,
                    Endereco = p.Endereco,
                    Bairro = p.Bairro,
                    Cidade = p.Cidade,
                    Informacoes = p.Informacoes,
                    Horarios = _mapper.Map<List<HorarioTurmaDTO>>(p.Horarios),
                    Professores = professoresPorPolo.TryGetValue(p.Id, out var profs)
                        ? profs
                        : new List<ProfessorPublicoDTO>(),
                })
                .ToList();
        }
    }
}
