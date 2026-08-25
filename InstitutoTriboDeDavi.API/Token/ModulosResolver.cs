using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.API.Token
{
    // Resolve quais módulos comerciais entram na claim "Modulos" do JWT.
    //
    // Fonte da contratação: configuração "Modulos:Ativos" — o que ESTE deploy
    // (esta instituição) contratou. Sistema é single-tenant hoje; quando virar
    // multi-tenant, basta trocar a fonte por algo por conta, mantendo a claim.
    //
    // graduacao continua per-usuário (admin OU professor com permissão), para
    // preservar o comportamento atual do PermiteGraduacao.
    public static class ModulosResolver
    {
        public static readonly string[] Todos =
            { "core", "graduacao", "captacao", "financeiro", "relacionamento" };

        public static string Resolver(UsuarioDTO usuario, IConfiguration configuration)
        {
            var ativos = configuration.GetSection("Modulos:Ativos").Get<string[]>();
            if (ativos == null || ativos.Length == 0)
                ativos = Todos; // sem config: tudo ativo (não quebra deploy atual)

            var lista = ativos
                .Select(m => m.Trim().ToLowerInvariant())
                .Where(m => Todos.Contains(m))
                .Distinct()
                .ToList();

            if (lista.Contains("graduacao")
                && usuario.Role != UserRole.Administrador
                && !usuario.PermiteGraduacao)
            {
                lista.Remove("graduacao");
            }

            return string.Join(",", lista);
        }
    }
}
