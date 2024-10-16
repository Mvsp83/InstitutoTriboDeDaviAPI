using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Business
{
    public class Presenca : Base
    {
        public long AlunoId { get; set; }
        public Aluno Aluno { get; set; }
        public bool EstaPresente { get; set; }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
