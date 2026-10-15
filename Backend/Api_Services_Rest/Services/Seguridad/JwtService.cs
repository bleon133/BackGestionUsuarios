using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api_Services_Rest.Services.Seguridad
{
    /// <summary>
    /// Implementación manual de JWT firmado con HS256 (HMAC-SHA256), sin paquetes externos.
    /// La configuración se toma de appSettings: Jwt:Secret, Jwt:ExpiresMinutes y Jwt:Issuer.
    /// Nunca se usa un secreto por defecto: si Jwt:Secret falta o es demasiado corto, no se
    /// emiten tokens.
    /// </summary>
    public static class JwtService
    {
        public const int MinSecretLength = 32;
        public const int DefaultExpiresMinutes = 60;
        public const string DefaultIssuer = "GestionUsuarios";

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string GetSecret()
        {
            return ConfigurationManager.AppSettings["Jwt:Secret"];
        }

        public static int GetExpiresMinutes()
        {
            int minutes;
            if (int.TryParse(ConfigurationManager.AppSettings["Jwt:ExpiresMinutes"], out minutes) && minutes > 0)
                return minutes;

            return DefaultExpiresMinutes;
        }

        public static string GetIssuer()
        {
            var issuer = ConfigurationManager.AppSettings["Jwt:Issuer"];
            return string.IsNullOrWhiteSpace(issuer) ? DefaultIssuer : issuer;
        }

        /// <summary>
        /// Indica si hay un secreto utilizable. error describe el problema cuando devuelve false.
        /// </summary>
        public static bool IsConfigured(out string error)
        {
            var secret = GetSecret();

            if (string.IsNullOrWhiteSpace(secret))
            {
                error = "El secreto JWT (Jwt:Secret) no está configurado.";
                return false;
            }

            if (secret.Length < MinSecretLength)
            {
                error = "El secreto JWT (Jwt:Secret) debe tener al menos " + MinSecretLength + " caracteres.";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Genera un token firmado. expiresAtUtc es la fecha de expiración resultante.
        /// </summary>
        public static string GenerateToken(int userId, string username, IEnumerable<string> roles,
            DateTime issuedAtUtc, out DateTime expiresAtUtc)
        {
            string error;
            if (!IsConfigured(out error))
                throw new InvalidOperationException(error);

            expiresAtUtc = issuedAtUtc.AddMinutes(GetExpiresMinutes());

            var header = new JObject
            {
                { "alg", "HS256" },
                { "typ", "JWT" }
            };

            var payload = new JObject
            {
                { "nameid", userId.ToString(CultureInfo.InvariantCulture) },
                { "unique_name", username },
                { "jti", Guid.NewGuid().ToString("N") },
                { "iat", ToUnixSeconds(issuedAtUtc) },
                { "nbf", ToUnixSeconds(issuedAtUtc) },
                { "exp", ToUnixSeconds(expiresAtUtc) }
            };

            var issuer = GetIssuer();
            if (!string.IsNullOrWhiteSpace(issuer))
                payload["iss"] = issuer;

            var roleList = roles == null
                ? new List<string>()
                : roles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();

            if (roleList.Count > 0)
                payload["role"] = new JArray(roleList);

            string headerSegment = Base64UrlEncode(Encoding.UTF8.GetBytes(header.ToString(Formatting.None)));
            string payloadSegment = Base64UrlEncode(Encoding.UTF8.GetBytes(payload.ToString(Formatting.None)));

            string signingInput = headerSegment + "." + payloadSegment;
            string signature = Base64UrlEncode(Sign(signingInput, GetSecret()));

            return signingInput + "." + signature;
        }

        /// <summary>
        /// Valida firma, expiración y emisor (si está configurado). Si el token es válido,
        /// principal contiene las claims del usuario.
        /// </summary>
        public static bool TryValidateToken(string token, out ClaimsPrincipal principal, out string error)
        {
            principal = null;

            if (!IsConfigured(out error))
                return false;

            if (string.IsNullOrWhiteSpace(token))
            {
                error = "Token vacío.";
                return false;
            }

            string[] parts = token.Split('.');
            if (parts.Length != 3 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
            {
                error = "Formato de token inválido.";
                return false;
            }

            string signingInput = parts[0] + "." + parts[1];
            byte[] expectedSignature;
            byte[] providedSignature;
            try
            {
                expectedSignature = Sign(signingInput, GetSecret());
                providedSignature = Base64UrlDecode(parts[2]);
            }
            catch (FormatException)
            {
                error = "Token mal formado.";
                return false;
            }

            if (!FixedTimeEquals(expectedSignature, providedSignature))
            {
                error = "Firma inválida.";
                return false;
            }

            JObject payload;
            try
            {
                payload = JObject.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[1])));
            }
            catch (Exception)
            {
                error = "Contenido del token inválido.";
                return false;
            }

            var expClaim = payload["exp"];
            if (expClaim == null || expClaim.Type == JTokenType.Null)
            {
                error = "El token no tiene fecha de expiración.";
                return false;
            }

            long exp = expClaim.Value<long>();
            long now = ToUnixSeconds(DateTime.UtcNow);
            if (now >= exp)
            {
                error = "El token expiró.";
                return false;
            }

            var nbfClaim = payload["nbf"];
            if (nbfClaim != null && nbfClaim.Type != JTokenType.Null && now < nbfClaim.Value<long>())
            {
                error = "El token todavía no es válido.";
                return false;
            }

            var issuer = GetIssuer();
            if (!string.IsNullOrWhiteSpace(issuer))
            {
                var issClaim = payload["iss"];
                if (issClaim == null || !string.Equals(issClaim.ToString(), issuer, StringComparison.Ordinal))
                {
                    error = "Emisor del token inválido.";
                    return false;
                }
            }

            var claims = new List<Claim>();

            string id = payload.Value<string>("nameid");
            if (!string.IsNullOrEmpty(id))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, id));

            string username = payload.Value<string>("unique_name");
            if (!string.IsNullOrEmpty(username))
                claims.Add(new Claim(ClaimTypes.Name, username));

            var roleClaim = payload["role"];
            if (roleClaim != null && roleClaim.Type != JTokenType.Null)
            {
                if (roleClaim.Type == JTokenType.Array)
                {
                    foreach (var role in roleClaim)
                        claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.ToString()));
                }
            }

            principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
            error = null;
            return true;
        }

        public static long ToUnixSeconds(DateTime utc)
        {
            var utcValue = utc.Kind == DateTimeKind.Utc ? utc : utc.ToUniversalTime();
            return (long)(utcValue - UnixEpoch).TotalSeconds;
        }

        private static byte[] Sign(string input, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string value = input.Replace('-', '+').Replace('_', '/');
            switch (value.Length % 4)
            {
                case 2:
                    value += "==";
                    break;
                case 3:
                    value += "=";
                    break;
            }

            return Convert.FromBase64String(value);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}
