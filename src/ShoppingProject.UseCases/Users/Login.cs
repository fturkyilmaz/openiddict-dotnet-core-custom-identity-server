using MediatR;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Infrastructure.Auth;

namespace ShoppingProject.UseCases.Users.Login;

public record LoginUserCommand(string UserName, string Password) : IRequest<LoginResultDto>;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
{
    private readonly IReadRepository<ApplicationUser> _repo;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IReadRepository<ApplicationUser> repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = (await _repo.ListAsync(ct)).FirstOrDefault(u => u.UserName == request.UserName);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordSalt, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var tokens = await _tokenService.GenerateTokensAsync(user);
        return tokens;
    }
}

public record LoginResultDto(string AccessToken, string RefreshToken, int ExpiresIn);
