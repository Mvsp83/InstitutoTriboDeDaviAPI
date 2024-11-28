using AutoMapper;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;

namespace InstitutoTriboDeDavi.System.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IMapper _mapper;
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IMapper mapper, IAlunoRepository alunoRepository)
        {
            _mapper = mapper;
            _alunoRepository = alunoRepository;
        }

        public async Task<AlunoDTO> Create(AlunoDTO alunoDTO)
        {
            var alunoExists = await _alunoRepository.GetByNome(alunoDTO.Nome);

            if (alunoExists != null)
            {
                throw new Exception("Já existe um registro com o mesmo Nome informado!");
            }

            var aluno = _mapper.Map<Aluno>(alunoDTO);

            var alunoCreated = await _alunoRepository.CreateAsync(aluno);

            return _mapper.Map<AlunoDTO>(alunoCreated);
        }
        public async Task Delete(long id)
        {
            await _alunoRepository.DeleteAsync(id);
        }

        public async Task<AlunoDTO> Get(long id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);

            return _mapper.Map<AlunoDTO>(aluno);
        }

        public async Task<List<AlunoDTO>> GetAll()
        {
            var allAlunos = await _alunoRepository.GetAllAsync();

            return _mapper.Map<List<AlunoDTO>>(allAlunos);
        }

        public async Task<AlunoDTO> GetByNome(string nome)
        {
            var aluno = await _alunoRepository.GetByNome(nome);

            return _mapper.Map<AlunoDTO>(aluno);
        }

        public async Task<List<AlunoDTO>> SearchByNome(string nome)
        {
            var allAlunos = await _alunoRepository.SearchByNome(nome);

            return _mapper.Map<List<AlunoDTO>>(allAlunos);
        }

        public async Task<AlunoDTO> Update(AlunoDTO alunoDTO)
        {
            var alunoExists = await _alunoRepository.GetByIdAsync(alunoDTO.Id);

            if (alunoExists == null)
            {
                throw new Exception("Não existe um registro com o ID informado!");
            }

            var aluno = _mapper.Map<Aluno>(alunoDTO);

            var alunoUpdated = await _alunoRepository.UpdateAsync(aluno);

            return _mapper.Map<AlunoDTO>(alunoUpdated);
        }

        public async Task<int> GetTotalAlunos()
        {
            return await _alunoRepository.GetTotalAlunosAsync();
        }
    }
}
