namespace InstitutoTriboDeDavi.API.Utilities
{
    // Nomes das policies de autorização e de rate limiting registradas no Startup
    public static class AuthPolicies
    {
        public const string ProfessorOuSuperior = "ProfessorOuSuperior";
        public const string LoginRateLimit = "login";
        // Envio público da ficha de inscrição (site, sem login).
        public const string InscricaoRateLimit = "inscricao";
    }
}
