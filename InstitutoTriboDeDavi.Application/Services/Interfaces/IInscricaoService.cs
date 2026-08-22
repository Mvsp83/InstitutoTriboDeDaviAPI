using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Resultado do envio público: o id e o código de acesso que a família
    // recebe no fim da ficha para acompanhar o aluno no portal.
    public record EnvioInscricaoResultado(long Id, string CodigoResponsavel);

    public interface IInscricaoService
    {
        // Envio público (sem autenticação).
        Task<EnvioInscricaoResultado> Enviar(InscricaoDTO dto);

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
