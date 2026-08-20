namespace InstitutoTriboDeDavi.Application.DTO
{
    public class DocumentoOficialDTO
    {
        public long Id { get; set; }
        public int Tipo { get; set; }
        public int Status { get; set; }
        public int Ano { get; set; }
        public int Numero { get; set; }
        public string NumeroFormatado { get; set; }
        public DateTime DataDocumento { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime? DataAprovacao { get; set; }
    }
}
