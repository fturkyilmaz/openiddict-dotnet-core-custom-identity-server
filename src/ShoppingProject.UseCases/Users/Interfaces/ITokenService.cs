using System.Threading;
using System.Threading.Tasks;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UseCases.Users.Interfaces
{
  public interface ITokenService
  {
    ValueTask<string> CreateAccessToken(ApplicationUser user, CancellationToken cancellationToken);
    ValueTask<string> CreateRefreshToken(ApplicationUser user, CancellationToken cancellationToken);
    ValueTask<Guid> ValidateRefreshToken(string refreshToken, CancellationToken cancellationToken);
  }
}
