using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class PlanoDeAulaDTO
    {
        public long Id { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public string Titulo { get; set; }
        public string? Objetivo { get; set; }
        public DateTime DataPrevista { get; set; }
        public int DuracaoTotalMinutos { get; set; }
        public StatusPlano Status { get; set; }
        public long? AulaId { get; set; }
        public List<BlocoDoPlanoDTO> Blocos { get; set; } = new();
    }
}
