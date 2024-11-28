using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Business
{
    public class Presenca : Base
    {
        public long AlunoId { get; set; }
        public string NomeAluno { get; set; }
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public bool EstaPresente { get; set; }
        public string Observacoes { get; set; }
        public long AulaId { get; set; }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
