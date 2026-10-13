using System;
using System.Security.Cryptography;

namespace Api_Services_Rest.Services.Seguridad
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2 (SHA-256). Formato almacenado: v1$iteraciones$salt$hash.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "v1";
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash = Derive(password, salt, Iterations);
            return string.Join("$", Prefix, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Verifica la contraseña. needsRehash es true cuando el valor guardado
        /// es una contraseña antigua en texto plano que debe convertirse a hash.
        /// </summary>
        public static bool Verify(string password, string stored, out bool needsRehash)
        {
            needsRehash = false;
            if (password == null || string.IsNullOrEmpty(stored))
                return false;

            if (!stored.StartsWith(Prefix + "$", StringComparison.Ordinal))
            {
                bool match = FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(password),
                    System.Text.Encoding.UTF8.GetBytes(stored));
                needsRehash = match;
                return match;
            }

            string[] parts = stored.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out int iterations))
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                return FixedTimeEquals(Derive(password, salt, iterations, expected.Length), expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int size = HashSize)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                return kdf.GetBytes(size);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
