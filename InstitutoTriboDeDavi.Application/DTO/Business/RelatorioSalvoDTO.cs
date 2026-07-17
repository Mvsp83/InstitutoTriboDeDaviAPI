namespace InstitutoTriboDeDavi.Application.DTO.Business
{
    public class RelatorioSalvoDTO
    {
        public long Id { get; set; }
        public string? UsuarioLogin { get; set; }
        public string Nome { get; set; }
        public string FonteId { get; set; }
        public string Colunas { get; set; }
        public int? Turma { get; set; }
        public long? PoloId { get; set; }
    }
}
