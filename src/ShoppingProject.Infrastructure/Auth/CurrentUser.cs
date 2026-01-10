using ShoppingProject.UseCases.Users.Interfaces;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(OpenIddictConstants.Claims.Name)?.Value;

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(OpenIddictConstants.Claims.Email)?.Value;

    public List<string> Roles =>
        _httpContextAccessor.HttpContext?.User?
            .FindAll(OpenIddictConstants.Claims.Role)
            .Select(r => r.Value)
            .ToList() ?? new List<string>();
}
