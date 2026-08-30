namespace InstitutoTriboDeDavi.Domain.Enums
{
    // Assunto da solicitação interna, para organizar a caixa de entrada.
    public enum CategoriaSolicitacao
    {
        Material = 0,     // quimonos, faixas, tatame, materiais de treino
        Estrutura = 1,    // espaço, manutenção, transporte
        Financeiro = 2,   // reembolsos, verbas, pagamentos
        Pedagogico = 3,   // aulas, graduação, planejamento
        Outro = 4,
    }

    // Ciclo de vida da solicitação.
    public enum StatusSolicitacao
    {
        Aberta = 0,
        EmAndamento = 1,
        Resolvida = 2,
    }
}
