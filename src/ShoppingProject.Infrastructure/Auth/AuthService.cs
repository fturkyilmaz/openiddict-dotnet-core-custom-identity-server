using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Core.UserAggregate;
using System.Security.Cryptography;

namespace ShoppingProject.Infrastructure.Auth;

public interface IAuthService
{
    Task<ApplicationUser?> ValidateAsync(string username, string password);
}

public sealed class UserAuthService : IAuthService
{
    private readonly AppDbContext _db;

    public UserAuthService(AppDbContext db) => _db = db;

    public async Task<ApplicationUser?> ValidateAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user is null) return null;

        // Geçici: sadece plain text karşılaştırma
        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}

public static class PasswordHasher
{
    public static string Hash(string password, byte[] salt)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32);

        return Convert.ToBase64String(hash);
    }

    public static bool Verify(string password, string hash, byte[] salt)
    {
        var computed = Hash(password, salt);

        // Güvenli karşılaştırma
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(hash),
            Convert.FromBase64String(computed));
    }

    // Geçici fallback (salt yoksa)
    public static bool Verify(string password, string hash) => password == hash;
}
