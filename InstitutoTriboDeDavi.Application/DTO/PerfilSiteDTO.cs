namespace InstitutoTriboDeDavi.Application.DTO
{
    // Perfil do professor para a seção pública do polo. É o que o próprio
    // usuário edita (nome de exibição, faixa, foto de rosto e o opt-in de
    // aparecer no site). Foto em data URI (mesmo esquema do avatar).
    public class PerfilSiteDTO
    {
        public string? Nome { get; set; }
        public int? Faixa { get; set; }
        public string? FotoSite { get; set; }
        public bool MostrarNoSite { get; set; }
    }
}
