using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ShoppingProject.Infrastructure.Common;

public static class PasswordHasher
{
    public static (string Salt, string Hash) Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return (Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool Verify(string password, string saltBase64, string hashBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);
        byte[] expectedHash = Convert.FromBase64String(hashBase64);

        byte[] actualHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}

