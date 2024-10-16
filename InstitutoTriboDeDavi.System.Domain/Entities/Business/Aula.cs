using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.System.Domain.Entities.Business
{
    public class Aula : Base
    {
        public DateTime Data { get; set; }
        public long PoloId { get; set; }
        public Polo Polo { get; set; }
        public long UsuarioId { get; set; }
        public Usuario UsuarioLogado { get; set; }
        public List<Presenca> Presencas { get; set; }

        public Aula()
        {
            Presencas = new List<Presenca>();
        }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
