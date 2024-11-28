using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Domain.Enums;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Consultas
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
