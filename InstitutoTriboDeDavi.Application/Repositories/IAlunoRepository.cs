using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IAlunoRepository : IBaseRepository<Aluno>
    {
        Task<Aluno> GetByNome(string nome);
        Task<Aluno> GetByCpf(string cpf);
        Task<List<Aluno>> SearchByNome(string nome);
        Task<int> GetTotalAlunosAsync();
        Task<List<Aluno>> ObterTodosAsync();
        Task<List<Aluno>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas);
        Task<List<Aluno>> ObterPendentesPorPoloAsync(long poloId);
        Task<List<Aluno>> ObterTodosPendentesAsync();

        // ── LGPD ──────────────────────────────────────────────────────────
        // Reúne, num só objeto, tudo que o sistema guarda sobre o aluno.
        Task<DadosPessoaisAluno> ColetarDadosPessoaisAsync(long alunoId);
        // Apaga os identificadores diretos do aluno e dos registros que os
        // copiam (presenças, inscrições), preservando os registros operacionais
        // ligados ao aluno anonimizado. Retorna o aluno atualizado, ou null se
        // o id não existe. Já anonimizado é no-op idempotente.
        Task<Aluno> AnonimizarAsync(long alunoId);
        // Alunos sem atividade (presença/matrícula) há mais de N meses e ainda
        // não anonimizados — candidatos à eliminação por retenção.
        Task<List<CandidatoRetencao>> ObterCandidatosRetencaoAsync(int mesesInativo);

        // Portal do responsável: localiza o aluno pelo código de acesso.
        Task<Aluno> ObterPorCodigoResponsavelAsync(string codigo);

        // Alunos ativos (não anonimizados) de um polo — usado na impressão em
        // lote dos códigos de acesso.
        Task<List<Aluno>> ObterPorPoloAsync(long poloId);

        // ── Foto do aluno ─────────────────────────────────────────────────
        // Define o arquivo da foto e devolve o id do arquivo ANTERIOR (para o
        // serviço remover do storage). Retorna null se o aluno não existe.
        Task<string> DefinirFotoAsync(long alunoId, string fotoArquivoId);

        // Config global (linha única) de onde a foto aparece.
        Task<ConfiguracaoFotoAluno> ObterConfigFotoAsync();
        Task SalvarConfigFotoAsync(ConfiguracaoFotoAluno cfg);
    }

    // Pacote bruto (entidades de domínio) devolvido pela coleta LGPD; o serviço
    // é quem monta o DTO de exportação.
    public record DadosPessoaisAluno(
        Aluno Aluno,
        List<Matricula> Matriculas,
        List<Graduacao> Graduacoes,
        List<Presenca> Presencas,
        List<Inscricao> Inscricoes);

    public record CandidatoRetencao(
        Aluno Aluno,
        DateTime? UltimaPresenca,
        int? UltimoAnoMatricula,
        int MesesInativo);
}
