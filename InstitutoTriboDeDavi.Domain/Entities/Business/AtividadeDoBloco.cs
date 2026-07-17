using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Vínculo N:N entre BlocoDoPlano e Atividade
    public class AtividadeDoBloco : Base
    {
        public long BlocoDoPlanoId { get; set; }
        public long AtividadeId { get; set; }

        public override bool Validate()
        {
            return true;
        }
    }
}
