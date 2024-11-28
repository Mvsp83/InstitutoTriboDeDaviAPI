using InstitutoTriboDeDavi.System.Domain.Enums;

namespace InstitutoTriboDeDavi.System.DTO.Queries
{
    public class FrequenciaDTO
    {
        public int AlunoId { get; set; }
        public string Nome { get; set; }
        public Faixa Faixa { get; set; }
        public int TotalAulas { get; set; }
        public int TotalFaltas { get; set; }
    }
}
