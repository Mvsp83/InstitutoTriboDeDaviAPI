using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Binário de uma foto de treino guardado no banco (padrão atual, igual ao
    // dos documentos via DocumentoBancoService). O FotoTreino referencia este
    // registro pelo Id (string). Trocar por Drive/bucket é só mudar o IFotoStorage.
    public class FotoArquivo : Base
    {
        public byte[] Conteudo { get; set; }
        public string ContentType { get; set; } = "image/jpeg";

        public override bool Validate() => true;
    }
}
