using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Queries
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
