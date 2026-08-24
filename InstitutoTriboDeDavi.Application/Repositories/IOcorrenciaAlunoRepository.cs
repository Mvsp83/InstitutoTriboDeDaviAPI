using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IOcorrenciaAlunoRepository
    {
        // Ocorrências (advertências + recados) de um aluno, mais recentes primeiro.
        Task<List<OcorrenciaAluno>> ListarPorAlunoAsync(long alunoId);
        Task<OcorrenciaAluno> ObterAsync(long id);
        Task<OcorrenciaAluno> CriarAsync(OcorrenciaAluno ocorrencia);
        Task<bool> ExcluirAsync(long id);
    }
}
