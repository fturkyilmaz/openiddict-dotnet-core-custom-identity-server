using MediatR;
using OpenIddict.Abstractions;

namespace ShoppingProject.UseCases.Auth;

public record RevokeTokenCommand(string TokenId) : IRequest;

public record MeQuery(ClaimsPrincipal User) : IRequest<UserInfoDto>;

public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IOpenIddictTokenManager _tokenManager;

    public RevokeTokenHandler(IOpenIddictTokenManager tokenManager) => _tokenManager = tokenManager;

    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var token = await _tokenManager.FindByIdAsync(request.TokenId);
        if (token is not null)
            await _tokenManager.TryRevokeAsync(token);

        return Unit.Value;
    }
}


public class MeHandler : IRequestHandler<MeQuery, UserInfoDto>
{
    public Task<UserInfoDto> Handle(MeQuery request, CancellationToken ct)
    {
        var userId = request.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        var email = request.User.FindFirst(OpenIddictConstants.Claims.Email)?.Value;
        var name = request.User.FindFirst(OpenIddictConstants.Claims.Name)?.Value;
        var roles = request.User.FindAll(OpenIddictConstants.Claims.Role).Select(r => r.Value);

        return Task.FromResult(new UserInfoDto(userId!, name!, email!, roles));
    }
}

public record UserInfoDto(string Id, string UserName, string Email, IEnumerable<string> Roles);
