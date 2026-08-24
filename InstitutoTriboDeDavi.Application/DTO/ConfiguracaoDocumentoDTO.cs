namespace InstitutoTriboDeDavi.Application.DTO
{
    public class ConfiguracaoDocumentoDTO
    {
        public long Id { get; set; }
        public string TituloCabecalho { get; set; }
        public string LinhaExtra { get; set; }
        public string TextoRodape { get; set; }
        public bool MostrarLogo { get; set; }
        public bool MostrarDataGeracao { get; set; }
        // JSON com os textos-padrão de ofício/recibo/certificado (montado no front).
        public string TextosPadraoJson { get; set; }
    }
}
