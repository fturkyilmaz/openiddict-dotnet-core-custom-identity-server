namespace ShoppingProject.UseCases.Users.Commands.TwoFactor;

public record Disable2FACommand(string UserId) : IRequest<Unit>;
