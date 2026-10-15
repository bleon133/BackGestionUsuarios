using System;
using System.Collections.Concurrent;

namespace Api_Services_Rest.Services.Seguridad
{
    /// <summary>
    /// Control de intentos de login en memoria, con una clave por usuario + IP.
    /// Tras 5 fallos bloquea durante 5 minutos. Al ser en memoria, el contador
    /// se reinicia si la aplicación se recicla o corre en varias instancias: sirve
    /// como freno básico a la fuerza bruta en una sola instancia.
    /// </summary>
    public static class LoginAttemptTracker
    {
        private const int MaxAttempts = 5;
        private static readonly TimeSpan Lockout = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan Retention = TimeSpan.FromMinutes(15);

        private sealed class AttemptInfo
        {
            public int Failures;
            public DateTime? LockedUntil;
            public DateTime LastUpdate;
        }

        private static readonly ConcurrentDictionary<string, AttemptInfo> Attempts =
            new ConcurrentDictionary<string, AttemptInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Construye la clave a partir del nombre de usuario normalizado y la IP de origen.
        /// </summary>
        public static string BuildKey(string username, string ipAddress)
        {
            string user = (username ?? string.Empty).Trim().ToLowerInvariant();
            string ip = string.IsNullOrWhiteSpace(ipAddress) ? "desconocida" : ipAddress.Trim();
            return user + "|" + ip;
        }

        public static bool IsBlocked(string key)
        {
            AttemptInfo info;
            if (string.IsNullOrEmpty(key) || !Attempts.TryGetValue(key, out info))
                return false;

            lock (info)
            {
                return info.LockedUntil.HasValue && info.LockedUntil.Value > DateTime.UtcNow;
            }
        }

        public static void RegisterFailure(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var info = Attempts.GetOrAdd(key, _ => new AttemptInfo());

            lock (info)
            {
                // Si el bloqueo anterior ya caducó, se empieza a contar de nuevo.
                if (info.LockedUntil.HasValue && info.LockedUntil.Value <= DateTime.UtcNow)
                {
                    info.Failures = 0;
                    info.LockedUntil = null;
                }

                if (info.LockedUntil.HasValue)
                    return; // sigue bloqueado

                info.Failures++;
                info.LastUpdate = DateTime.UtcNow;

                if (info.Failures >= MaxAttempts)
                    info.LockedUntil = DateTime.UtcNow.Add(Lockout);
            }

            Prune();
        }

        public static void Reset(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            AttemptInfo removed;
            Attempts.TryRemove(key, out removed);
        }

        /// <summary>
        /// Limpieza sencilla para que el diccionario no crezca sin control.
        /// </summary>
        private static void Prune()
        {
            if (Attempts.Count < 1000)
                return;

            var limit = DateTime.UtcNow.Subtract(Retention);
            foreach (var pair in Attempts)
            {
                AttemptInfo info = pair.Value;
                bool expired;
                lock (info)
                    expired = !info.LockedUntil.HasValue && info.LastUpdate < limit;

                if (expired)
                {
                    AttemptInfo removed;
                    Attempts.TryRemove(pair.Key, out removed);
                }
            }
        }
    }
}
