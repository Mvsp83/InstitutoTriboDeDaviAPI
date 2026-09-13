using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Histórico de empréstimo/comodato de um bem (quimono, faixa) a um aluno.
    // Uma linha por empréstimo; DataDevolucao nula = ainda com o aluno (em aberto).
    public class EmprestimoBem : Base
    {
        public long BemPatrimonialId { get; set; }
        public long AlunoId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;

        public override bool Validate()
        {
            if (BemPatrimonialId <= 0)
                _errors.Add("O bem do empréstimo é obrigatório.");
            if (AlunoId <= 0)
                _errors.Add("O aluno do empréstimo é obrigatório.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
