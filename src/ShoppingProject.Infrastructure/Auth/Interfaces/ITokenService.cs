namespace ShoppingProject.Infrastructure.Auth.Interfaces
{
    using ShoppingProject.Core.UserAggregate;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ITokenService
    {
        ValueTask<string> CreateAccessToken(ApplicationUser user, CancellationToken cancellationToken);
        ValueTask<string> CreateRefreshToken(ApplicationUser user, CancellationToken cancellationToken);
    }
}
