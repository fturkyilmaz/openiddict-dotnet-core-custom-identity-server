namespace ShoppingProject.UseCases.Users.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    List<string> Roles { get; }
}