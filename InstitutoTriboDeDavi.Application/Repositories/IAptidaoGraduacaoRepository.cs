using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    // Agrega, por aluno, os números usados na aptidão ao exame de graduação:
    // presenças e advertências desde a última graduação. poloId nulo = todos os
    // polos (Administrador); com valor = restrito ao polo do usuário.
    public interface IAptidaoGraduacaoRepository
    {
        Task<List<AptidaoGraduacaoDTO>> ObterAsync(long? poloId);
    }
}
