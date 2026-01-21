namespace ShoppingProject.UseCases.UserRoles.DTOs;

public sealed record UserRoleDto(
    Guid UserId,
    Guid RoleId,
    string UserName,
    string Email,
    IReadOnlyCollection<string> Roles
);
