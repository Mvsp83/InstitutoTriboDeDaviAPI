using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Consultas
{
    public class Aniversariante : Base
    {
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool JaComemorado { get; set; }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
