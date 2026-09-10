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

        // Retenção/LGPD: inscrições recusadas enviadas antes de `limite` e ainda
        // não anonimizadas — candidatas ao expurgo de dados pessoais.
        Task<List<Inscricao>> ListarRecusadasParaExpurgoAsync(System.DateTime limite);
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

        // AlunoIds já matriculados no ano (opcionalmente por polo) — usado na
        // matrícula em lote para não duplicar.
        Task<List<long>> ObterAlunosMatriculadosAsync(int ano, long? poloId);

        // Cria várias matrículas de uma vez (virada de ano). Retorna a quantidade.
        Task<int> CriarMatriculasAsync(IEnumerable<Matricula> matriculas);

        // Ocupação de vagas: matrículas ATIVAS do ano por polo.
        Task<int> ContarMatriculasAtivasAsync(int ano, long poloId);
        Task<Dictionary<long, int>> ContarMatriculasAtivasPorPoloAsync(int ano);

        // Inscrições PENDENTES do ano por polo — contam como vaga reservada
        // (a lotação considera ativas + pendentes para não aceitar além da conta).
        Task<int> ContarInscricoesPendentesAsync(int ano, long poloId);
        Task<Dictionary<long, int>> ContarInscricoesPendentesPorPoloAsync(int ano);

        // Liga/desliga da matrícula (libera/ocupa vaga no ano).
        Task<Matricula> ObterMatriculaPorIdAsync(long id);
        Task AtualizarMatriculaAsync(Matricula matricula);
    }
}
