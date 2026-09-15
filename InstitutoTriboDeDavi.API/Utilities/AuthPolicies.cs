namespace InstitutoTriboDeDavi.API.Utilities
{
    // Nomes das policies de autorização e de rate limiting registradas no Startup
    public static class AuthPolicies
    {
        // Acesso interno (Administrador/Supervisor/Professor). Usada como Default e
        // Fallback: bloqueia o token do responsável (role "Responsavel" -> que o
        // Authorization resolve como UserRole.Default) em todos os endpoints internos.
        public const string EquipeInterna = "EquipeInterna";
        public const string ProfessorOuSuperior = "ProfessorOuSuperior";
        public const string LoginRateLimit = "login";
        // Envio público da ficha de inscrição (site, sem login).
        public const string InscricaoRateLimit = "inscricao";
        // Beacon público de métricas: corta inflação de contadores por IP.
        public const string MetricaRateLimit = "metrica";
    }
}
