namespace ShoppingProject.UseCases.Users.Commands.TwoFactor; 
public record Enable2FACommand(string UserId) : IRequest<Unit>;