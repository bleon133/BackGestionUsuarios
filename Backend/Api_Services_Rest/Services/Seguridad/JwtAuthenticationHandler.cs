using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api_Services_Rest.Services.Seguridad
{
    /// <summary>
    /// DelegatingHandler que lee la cabecera "Authorization: Bearer &lt;token&gt;", valida el
    /// token y, si es correcto, deja el ClaimsPrincipal en el contexto de la petición para
    /// que funcionen los atributos [Authorize] y [Authorize(Roles = "...")].
    /// Si el token falta o es inválido no se corta la petición aquí: los endpoints
    /// protegidos devolverán 401 y los marcados con [AllowAnonymous] (el login) siguen
    /// funcionando aunque el cliente envíe un token caducado.
    /// </summary>
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        private const string BearerScheme = "Bearer";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authorization = request.Headers.Authorization;

            if (authorization != null
                && string.Equals(authorization.Scheme, BearerScheme, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(authorization.Parameter))
            {
                string error;
                ClaimsPrincipal principal;
                if (JwtService.TryValidateToken(authorization.Parameter, out principal, out error))
                    SetPrincipal(request, principal);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private static void SetPrincipal(HttpRequestMessage request, ClaimsPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;

            request.GetRequestContext().Principal = principal;
        }
    }
}
