using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Core.Interfaces;

namespace ShoppingProject.UseCases.Users.Commands.TwoFactor;

public class Disable2FAHandler : IRequestHandler<Disable2FACommand, Unit>
{
    private readonly IRepository<ApplicationUser> _userRepository;

    public Disable2FAHandler(IRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<Unit> Handle(Disable2FACommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        // Burada user.TwoFactorEnabled = false gibi bir property set edilmeli
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Unit.Value;
    }
}
