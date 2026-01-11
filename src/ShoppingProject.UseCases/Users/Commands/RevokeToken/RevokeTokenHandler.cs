using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.RevokeToken;

public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IRevokeTokenService _revokeTokenService;

    public RevokeTokenHandler(IRevokeTokenService revokeTokenService)
    {
        _revokeTokenService = revokeTokenService;
    }

    public async ValueTask<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await _revokeTokenService.RevokeAsync(request.TokenId, cancellationToken);
        return Unit.Value;
    }
}