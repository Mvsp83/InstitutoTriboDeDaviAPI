namespace InstitutoTriboDeDavi.Application.DTO
{
    public class ConfiguracaoLojaDTO
    {
        public long Id { get; set; }
        public bool CompraWhatsappHabilitada { get; set; }
        // Só dígitos, com DDD (ex.: "47999998888").
        public string WhatsappNumero { get; set; }
    }
}
