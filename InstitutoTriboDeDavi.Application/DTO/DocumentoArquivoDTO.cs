namespace InstitutoTriboDeDavi.Application.DTO
{
    // Metadados de um arquivo armazenado no Google Drive. Não há entidade/banco:
    // a fonte da verdade é o próprio Drive.
    public class DocumentoArquivoDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string MimeType { get; set; } = string.Empty;
    }
}
