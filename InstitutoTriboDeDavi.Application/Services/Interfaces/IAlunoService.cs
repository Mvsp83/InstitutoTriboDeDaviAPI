using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDTO> Create(AlunoDTO alunoDTO);
        Task<AlunoDTO> Update(AlunoDTO alunoDTO);
        Task Delete(long id);
        Task<AlunoDTO> Get(long id);
        Task<List<AlunoDTO>> GetAll();
        Task<AlunoDTO> GetByNome(string nome);
        Task<List<AlunoDTO>> SearchByNome(string nome);
        Task<List<AlunoDTO>> ObterAlunosPorTurmaAsync(UsuarioDTO usuario, List<int> turmas);
        Task<List<AlunoPendenteDTO>> ObterAlunosPendentesAsync(UsuarioDTO usuario);
        Task<AlunoDTO> AtribuirTurmaAsync(AtribuirTurmaDTO dto);

        // ── LGPD ──────────────────────────────────────────────────────────
        // Exporta tudo que o sistema guarda sobre o aluno (acesso/portabilidade).
        // Retorna null quando o id não existe.
        Task<DadosPessoaisAlunoDTO> ExportarDadosAsync(long id, string geradoPor);
        // Elimina os dados pessoais do aluno (direito de eliminação), mantendo
        // os registros operacionais anonimizados. Retorna null se o id não existe.
        Task<AlunoDTO> AnonimizarAsync(long id);
        // Lista alunos elegíveis à eliminação por retenção.
        Task<List<CandidatoRetencaoDTO>> ObterCandidatosRetencaoAsync(int mesesInativo);

        // ── Portal do responsável ───────────────────────────────────────────
        // (Re)gera o código de acesso do responsável e o devolve. Null se o
        // aluno não existe.
        Task<string> GerarCodigoResponsavelAsync(long id);
        // Código atual (ou null se não existe/não liberado).
        Task<string?> ObterCodigoResponsavelAsync(long id);
    }
}
