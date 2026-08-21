namespace InstitutoTriboDeDavi.Application.DTO
{
    public class BemPatrimonialDTO
    {
        public long Id { get; set; }
        public int Categoria { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public int Estado { get; set; }
        public long? PoloId { get; set; }
        public string NumeroPatrimonio { get; set; }
        public string Observacoes { get; set; }
    }
}
