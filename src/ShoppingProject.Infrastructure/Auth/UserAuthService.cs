using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.Infrastructure.Auth;

public interface IUserAuthService
{
    Task<ApplicationUser?> ValidateAsync(string username, string password);
}

public sealed class UserAuthService : IUserAuthService
{
    private readonly AppDbContext _db;

    public UserAuthService(AppDbContext db) => _db = db;

    public async Task<ApplicationUser?> ValidateAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user is null) return null;

        // Replace with real hashing (PBKDF2/BCrypt)
        if (!PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}

public static class PasswordHasher
{
    public static bool Verify(string password, string hash)
    {
        // TODO: Implement secure hashing. This is placeholder.
        return password == hash;
    }
}
