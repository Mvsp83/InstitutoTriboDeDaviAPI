using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Domain.Entities.Consultas
{
    public class Frequencia : Base
    {
        public int AlunoId { get; set; }
        public string Nome { get; set; }
        public Faixa Faixa { get; set; }
        public int TotalAulas { get; set; }
        public int TotalFaltas { get; set; }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
