using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class BlocoDoPlanoDTO
    {
        public long Id { get; set; }
        public long PlanoDeAulaId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public int DuracaoMinutos { get; set; }
        public string? Descricao { get; set; }
        public List<AtividadeDoBlocoDTO> Atividades { get; set; } = new();
    }
}
