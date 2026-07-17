namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class ModeloDeAulaDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public int DuracaoTotalMinutos { get; set; }
        public List<BlocoDoModeloDTO> Blocos { get; set; } = new();
    }
}
