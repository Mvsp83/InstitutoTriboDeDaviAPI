namespace InstitutoTriboDeDavi.Application.DTO
{
    // Item do histórico de empréstimo de um bem. O nome do aluno é resolvido no
    // front (que já carrega a lista de alunos), então aqui vai só o AlunoId.
    public class EmprestimoBemDTO
    {
        public long Id { get; set; }
        public long BemPatrimonialId { get; set; }
        // Um destino por alocação: aluno (quimono/faixa) OU polo (tatame).
        public long? AlunoId { get; set; }
        public long? PoloId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public string Observacao { get; set; }
        public string RegistradoPor { get; set; }
    }

    // Payload para registrar uma nova alocação. Informe AlunoId OU PoloId.
    public class EmprestarBemDTO
    {
        public long BemPatrimonialId { get; set; }
        public long? AlunoId { get; set; }
        public long? PoloId { get; set; }
        public string Observacao { get; set; }
    }
}
