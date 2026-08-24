using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IOcorrenciaAlunoService
    {
        Task<List<OcorrenciaAlunoDTO>> ListarPorAluno(long alunoId);

        // Registra uma advertência (Tipo=0, exige motivo) ou um recado (Tipo=1,
        // status + texto opcional). O polo vem do aluno; o autor, do login.
        Task<OcorrenciaAlunoDTO> Criar(OcorrenciaAlunoDTO dto, string registradoPor);

        Task Excluir(long id);
    }
}
