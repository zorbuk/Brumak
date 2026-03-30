using System.Security.Cryptography;

namespace Brumak_Shared.Metrics
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            // Guardamos salt + hash juntos en Base64
            byte[] result = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

            return Convert.ToBase64String(result);
        }

        public static bool Verify(string password, string storedHash)
        {
            byte[] bytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[SaltSize];
            byte[] hash = new byte[HashSize];
            Buffer.BlockCopy(bytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(bytes, SaltSize, hash, 0, HashSize);

            byte[] computed = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return CryptographicOperations.FixedTimeEquals(hash, computed);
        }
    }
}
