using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password
) : IRequest<LoginResultDto>;
