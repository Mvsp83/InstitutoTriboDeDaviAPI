namespace InstitutoTriboDeDavi.Application.DTO
{
    public class AvisoDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public int PublicoAlvo { get; set; }
        public DateTime DataCriacao { get; set; }
        public string CriadoPor { get; set; }
        public bool Ativo { get; set; }
    }
}
