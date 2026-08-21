using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IInscricaoService
    {
        // Envio público (sem autenticação).
        Task<long> Enviar(InscricaoDTO dto);

        // Fila de revisão. poloId nulo = todos (Administrador).
        Task<List<InscricaoDTO>> Listar(int? status, int? ano, long? poloId);
        Task<InscricaoDTO> Obter(long id);
        Task<int> ContarPendentes(long? poloId);

        // Aprovar cria/atualiza o aluno e a matrícula do ano; o revisor pode
        // corrigir polo e turma nesse momento.
        Task<MatriculaDTO> Aprovar(long id, RevisaoInscricaoDTO revisao, string revisor);
        Task Recusar(long id, string motivo, string revisor);

        Task<List<MatriculaDTO>> ListarMatriculas(int ano, long? poloId);
    }
}
