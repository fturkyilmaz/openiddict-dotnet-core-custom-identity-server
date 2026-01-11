namespace ShoppingProject.UseCases.Users.DTOs;

public sealed record LoginResultDto(
    Guid UserId,
    string UserName,
    string Email,
    IReadOnlyCollection<string> Roles
);
