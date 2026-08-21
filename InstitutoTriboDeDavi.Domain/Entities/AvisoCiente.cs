using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Registro de que um usuário deu "ciente" num aviso (para não reaparecer).
    public class AvisoCiente : Base
    {
        public long AvisoId { get; set; }
        public string UsuarioLogin { get; set; } = string.Empty;
        public DateTime DataCiente { get; set; }

        public override bool Validate() => true;
    }
}
