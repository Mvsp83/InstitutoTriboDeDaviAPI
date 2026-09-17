namespace InstitutoTriboDeDavi.Application.DTO
{
    public class RecadoDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Categoria { get; set; }
        public string Anunciante { get; set; }
        public string Contato { get; set; }
        public long? PoloId { get; set; }
        // Foto opcional: id no storage. Vazio = sem foto. O binário é servido
        // pelo endpoint GET Recado/{id}/foto (o cliente checa se está preenchido).
        public string FotoArquivoId { get; set; }
        public DateTime DataCriacao { get; set; }
        // Validade opcional no envio; o servidor aplica um padrão se vier vazio.
        public DateTime? ExpiraEm { get; set; }
        public string CriadoPor { get; set; }
        public bool Ativo { get; set; }
        public bool Aprovado { get; set; }
    }
}
