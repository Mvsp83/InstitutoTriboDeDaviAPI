using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Registro de uma graduação do aluno: a faixa que ele tinha, a que passou a
    // ter e quando. O cadastro do aluno guarda só a faixa atual, então é esta
    // tabela que preserva a trajetória — usada no certificado e no histórico.
    public class Graduacao : Base
    {
        public long AlunoId { get; set; }
        // Polo em que o aluno estava na data (ele pode mudar de polo depois).
        public long PoloId { get; set; }
        public int FaixaAnterior { get; set; }
        public int FaixaNova { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (AlunoId <= 0)
                _errors.Add("A graduação precisa estar vinculada a um aluno.");

            if (Data == default)
                _errors.Add("A data da graduação é obrigatória.");
            else if (Data.Date > DateTime.Today)
                _errors.Add("A data da graduação não pode estar no futuro.");

            if (FaixaAnterior < 0 || FaixaNova < 0)
                _errors.Add("As faixas informadas são inválidas.");

            // Graduar é sempre subir. Um retrocesso quase certamente é engano —
            // e correção se faz excluindo o registro errado, não gravando um
            // registro invertido que sujaria o histórico.
            if (FaixaNova <= FaixaAnterior)
                _errors.Add("A nova faixa precisa ser superior à faixa atual do aluno.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
