namespace InstitutoTriboDeDavi.Application.DTO
{
    // Linha da impressão em lote dos códigos de acesso do responsável.
    public class CodigoResponsavelItemDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Responsavel { get; set; } = string.Empty;
        public long PoloId { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }
}
