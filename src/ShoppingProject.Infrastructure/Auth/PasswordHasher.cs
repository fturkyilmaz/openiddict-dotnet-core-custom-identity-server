using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ShoppingProject.UseCases.Users.Interfaces;

namespace ShoppingProject.Infrastructure.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Salt üret
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            // PBKDF2 ile hashle
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = parts[1];

            string providedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return expectedHash == providedHash;
        }
    }
}
