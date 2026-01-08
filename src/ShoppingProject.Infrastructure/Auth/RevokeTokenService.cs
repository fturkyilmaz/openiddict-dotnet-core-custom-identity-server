using OpenIddict.Abstractions;
using ShoppingProject.UseCases.Users.Interfaces;

namespace ShoppingProject.Infrastructure.Auth
{
  public class RevokeTokenService : IRevokeTokenService
  {
    private readonly IOpenIddictTokenManager _tokenManager;

    public RevokeTokenService(IOpenIddictTokenManager tokenManager)
    {
      _tokenManager = tokenManager;
    }

    public async Task RevokeAsync(string tokenId, CancellationToken cancellationToken)
    {
      var token = await _tokenManager.FindByIdAsync(tokenId, cancellationToken);
      if (token is not null)
      {
        await _tokenManager.TryRevokeAsync(token, cancellationToken);
      }
    }
  }
}
