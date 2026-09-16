using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Alocação/comodato de uma UNIDADE de um bem a um aluno (quimono, faixa) ou
    // a um polo (tatame). Uma linha por alocação; DataDevolucao nula = em aberto
    // (ainda alocado). Vários registros em aberto por bem, limitados à Quantidade
    // do bem — a disponibilidade é Quantidade menos as alocações em aberto.
    public class EmprestimoBem : Base
    {
        public long BemPatrimonialId { get; set; }
        // Exatamente um destino: aluno (quimono/faixa) OU polo (tatame).
        public long? AlunoId { get; set; }
        public long? PoloId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;

        public override bool Validate()
        {
            if (BemPatrimonialId <= 0)
                _errors.Add("O bem do empréstimo é obrigatório.");

            var temAluno = AlunoId.HasValue && AlunoId.Value > 0;
            var temPolo = PoloId.HasValue && PoloId.Value > 0;
            if (temAluno == temPolo)
                _errors.Add("Informe o aluno OU o polo do empréstimo (apenas um).");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
