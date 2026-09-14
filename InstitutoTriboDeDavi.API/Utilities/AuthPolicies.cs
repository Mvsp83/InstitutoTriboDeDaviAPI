namespace InstitutoTriboDeDavi.API.Utilities
{
    // Nomes das policies de autorização e de rate limiting registradas no Startup
    public static class AuthPolicies
    {
        public const string ProfessorOuSuperior = "ProfessorOuSuperior";
        public const string LoginRateLimit = "login";
        // Envio público da ficha de inscrição (site, sem login).
        public const string InscricaoRateLimit = "inscricao";
        // Beacon público de métricas: corta inflação de contadores por IP.
        public const string MetricaRateLimit = "metrica";
        // Busca pública de rematrícula: chave de baixa entropia (CPF+nascimento),
        // então limite estrito por IP contra enumeração de dados de menores.
        public const string RematriculaRateLimit = "rematricula";
    }
}
