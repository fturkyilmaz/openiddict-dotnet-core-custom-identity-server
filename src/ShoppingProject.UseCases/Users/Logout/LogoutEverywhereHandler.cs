using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Logout
{
 public class LogoutEverywhereHandler : IRequestHandler<LogoutEverywhereCommand, Unit>
    {
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IRevokeTokenService _revokeTokenService;

    public LogoutEverywhereHandler(
        IRepository<ApplicationUser> userRepository,
        IRevokeTokenService revokeTokenService)
        {
            _userRepository = userRepository;
            _revokeTokenService = revokeTokenService;
        }

    public async ValueTask<Unit> Handle(
        LogoutEverywhereCommand request,
        CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw new UnauthorizedAccessException();

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");


            await _userRepository.UpdateAsync(user, cancellationToken);

            await _revokeTokenService.RevokeByUserIdAsync(userId, cancellationToken);

            return Unit.Value;
        }
    }
}