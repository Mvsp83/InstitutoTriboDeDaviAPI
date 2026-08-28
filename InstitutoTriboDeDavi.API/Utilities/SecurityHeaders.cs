namespace InstitutoTriboDeDavi.API.Utilities
{
    // Cabeçalhos de segurança em toda resposta. HSTS/HTTPS já são tratados no
    // Startup; aqui vão as defesas que independem de TLS (anti-sniffing,
    // anti-clickjacking, vazamento de referrer). Aplicado cedo no pipeline para
    // valer inclusive nas respostas de erro.
    public static class SecurityHeaders
    {
        public static IApplicationBuilder UseSecurityHeaders(
            this IApplicationBuilder app, bool isDevelopment)
        {
            return app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;

                // Impede o navegador de "adivinhar" o tipo do conteúdo (MIME sniffing).
                headers["X-Content-Type-Options"] = "nosniff";
                // A API não deve ser embutida em iframe (clickjacking).
                headers["X-Frame-Options"] = "DENY";
                // Não vaza a URL da API em navegações para fora.
                headers["Referrer-Policy"] = "no-referrer";
                // Bloqueia políticas cross-domain legadas (Flash/PDF).
                headers["X-Permitted-Cross-Domain-Policies"] = "none";

                // CSP estrita só em produção: em dev quebraria o Swagger UI (que
                // carrega JS/CSS próprios). Como a API só devolve JSON, negar tudo
                // e proibir enquadramento é defesa-em-profundidade — reforça o
                // X-Frame-Options com frame-ancestors, respeitado por navegadores
                // modernos mesmo quando o X-Frame-Options é ignorado.
                if (!isDevelopment)
                    headers["Content-Security-Policy"] =
                        "default-src 'none'; frame-ancestors 'none'";

                await next();
            });
        }
    }
}
