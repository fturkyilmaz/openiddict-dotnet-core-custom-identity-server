using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResultDto>;
