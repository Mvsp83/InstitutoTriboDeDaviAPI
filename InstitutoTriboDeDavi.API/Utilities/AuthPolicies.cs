namespace InstitutoTriboDeDavi.API.Utilities
{
    // Nomes das policies de autorização e de rate limiting registradas no Startup
    public static class AuthPolicies
    {
        public const string ProfessorOuSuperior = "ProfessorOuSuperior";
        public const string LoginRateLimit = "login";
    }
}
