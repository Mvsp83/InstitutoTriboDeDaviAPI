using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InstitutoTriboDeDavi.API.Utilities
{
    // Exige que o token carregue o módulo comercial na claim "Modulos".
    // Complementa [Authorize] (que valida o PAPEL do usuário): aqui validamos o
    // ENTITLEMENT — o que a conta/instituição contratou (ver ModulosResolver).
    //
    // Compat: se o token não traz a claim "Modulos" (só passou a ser emitida
    // agora), não bloqueia — evita derrubar sessões antigas na transição.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RequireModuloAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _modulo;

        public RequireModuloAttribute(string modulo)
        {
            _modulo = (modulo ?? string.Empty).Trim().ToLowerInvariant();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
                return; // sem sessão: deixa o [Authorize] tratar

            var claim = user.FindFirst("Modulos");
            if (claim == null)
                return; // token antigo sem a claim: compat, não bloqueia

            var modulos = claim.Value.Split(
                ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!modulos.Contains(_modulo, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(
                    new { message = $"Módulo '{_modulo}' não contratado." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
