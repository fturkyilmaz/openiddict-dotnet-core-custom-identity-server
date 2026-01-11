using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.ResendVerification;

public class ResendVerificationHandler : IRequestHandler<ResendVerificationCommand, Unit>
{
    private readonly  IRepository<ApplicationUser> _userRepository;
    private readonly IEmailSender _emailService;

    public ResendVerificationHandler( IRepository<ApplicationUser> userRepository, IEmailSender emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async ValueTask<Unit> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
          var user = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

        if (user == null)
            return Unit.Value;

        var code = Guid.NewGuid().ToString("N");
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendEmailAsync(user.Email,"Test", "Email doğrulama", $"Doğrulama kodunuz: {code}");

        return Unit.Value;
    }
}

