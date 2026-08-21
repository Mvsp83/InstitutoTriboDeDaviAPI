using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IInscricaoRepository
    {
        Task<Inscricao> CriarAsync(Inscricao inscricao);
        Task<Inscricao> ObterAsync(long id);
        // Fila de revisão: status opcional, ano opcional, polo opcional
        // (professor/supervisor enxergam apenas o próprio polo).
        Task<List<Inscricao>> ListarAsync(int? status, int? ano, long? poloId);
        Task<int> ContarPendentesAsync(long? poloId);

        // Quantas inscrições o mesmo WhatsApp enviou no período — trava simples
        // contra envio repetido/automatizado no endpoint público.
        Task<int> ContarEnviosRecentesAsync(string whatsApp, int minutos);

        // Aprovação: cria/atualiza o aluno e a matrícula do ano numa transação.
        Task<(Aluno aluno, Matricula matricula)> AprovarAsync(
            Inscricao inscricao, Aluno aluno, Matricula matricula);

        Task AtualizarAsync(Inscricao inscricao);

        Task<List<Matricula>> ListarMatriculasAsync(int ano, long? poloId);
        Task<Matricula> ObterMatriculaAsync(long alunoId, int ano);
    }
}
