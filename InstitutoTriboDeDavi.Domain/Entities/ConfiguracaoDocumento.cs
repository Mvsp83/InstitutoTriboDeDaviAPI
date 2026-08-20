using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Configuração única (singleton) do padrão dos documentos exportados:
    // cabeçalho, rodapé e marca aplicados a todos os PDFs do portal.
    public class ConfiguracaoDocumento : Base
    {
        public string TituloCabecalho { get; set; } = string.Empty;
        public string LinhaExtra { get; set; } = string.Empty;
        public string TextoRodape { get; set; } = string.Empty;
        public bool MostrarLogo { get; set; } = true;
        public bool MostrarDataGeracao { get; set; } = true;

        public override bool Validate() => true;
    }
}
