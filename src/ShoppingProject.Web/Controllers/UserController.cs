using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Infrastructure.Auth;

namespace ShoppingProject.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Select(u => new { u.Id, u.UserName, u.Email, u.DisplayName })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (salt, hash) = PasswordHasher.Hash(dto.Password);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.UserName });
    }
}

public record RegisterDto(string UserName, string Email, string DisplayName, string Password);
