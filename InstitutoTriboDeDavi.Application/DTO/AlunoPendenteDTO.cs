namespace InstitutoTriboDeDavi.Application.DTO
{
    public class AlunoPendenteDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public double? Peso { get; set; }
        public int Faixa { get; set; }
        public long PoloId { get; set; }
        public string PoloNome { get; set; }
    }
}
