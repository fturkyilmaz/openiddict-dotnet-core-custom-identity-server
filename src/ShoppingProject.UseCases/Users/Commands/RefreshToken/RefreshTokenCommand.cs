using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.RefreshToken;
    
public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResultDto>;