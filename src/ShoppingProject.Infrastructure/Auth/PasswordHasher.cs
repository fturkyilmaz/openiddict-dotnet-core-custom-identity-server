using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ShoppingProject.UseCases.Users.Interfaces;

namespace ShoppingProject.Infrastructure.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;   // 128 bit
        private const int KeySize = 32;    // 256 bit
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            // Salt üret
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // PBKDF2 ile hashle
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: KeySize);

            // salt.hash formatında sakla
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            byte[] providedHash = KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: KeySize);

            // Güvenli karşılaştırma
            return CryptographicOperations.FixedTimeEquals(expectedHash, providedHash);
        }
    }
}
