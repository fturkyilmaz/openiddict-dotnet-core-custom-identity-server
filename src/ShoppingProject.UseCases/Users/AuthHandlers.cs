using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users
{
    // Query: Me (returns current user info)
    public record MeQuery(string UserId) : IRequest<MeDto>;

    public record MeDto(Guid Id, string UserName, string Email, bool EmailVerified, bool TwoFactorEnabled);

    public class MeQueryHandler : IRequestHandler<MeQuery, MeDto>
    {
        private readonly IRepository<ApplicationUser> _userRepository;

        public MeQueryHandler(IRepository<ApplicationUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async ValueTask<MeDto> Handle(MeQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

            return new MeDto(
                user.Id,
                user.UserName,
                user.Email,
                true,
                true
            );
        }
    }

    // Command: Revoke Token
    public record RevokeTokenCommand(string TokenId) : IRequest<Unit>;

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

        // Command: Refresh Token
    public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResultDto>;

    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResultDto>
    {
        private readonly ITokenService _tokenService;
        private readonly IRepository<ApplicationUser> _userRepository;

        public RefreshTokenHandler(ITokenService tokenService, IRepository<ApplicationUser> userRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async ValueTask<LoginResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Refresh token doğrulamaı
            var userId = await _tokenService.ValidateRefreshToken(request.RefreshToken, cancellationToken);
            if (userId == Guid.Empty)
                throw new InvalidOperationException("Invalid refresh token");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new InvalidOperationException("User not found");

            // Yeni access ve refresh token üret
            var newAccessToken = await _tokenService.CreateAccessToken(user, cancellationToken);
            var newRefreshToken = await _tokenService.CreateRefreshToken(user, cancellationToken);

            return new LoginResultDto(user.Id, user.UserName, user.Email, user.Roles.Select(r => r.Role.Name).ToList());
        }
    }

    public record LogoutEverywhereCommand(string UserId) : IRequest<Unit>;
    public class LogoutEverywhereHandler : IRequestHandler<LogoutEverywhereCommand, Unit> { 
        private readonly ITokenService _tokenService; 
        
        public LogoutEverywhereHandler(ITokenService tokenService) {
            _tokenService = tokenService; 
        } 
        public async ValueTask<Unit> Handle(LogoutEverywhereCommand request, CancellationToken cancellationToken) { 
            await _tokenService.LogoutEverywhereAsync(Guid.Parse(request.UserId), cancellationToken); 
            return Unit.Value; 
        } 
    }

    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest<Unit>; 
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit> { 
        private readonly  IRepository<ApplicationUser> _userRepository; 
        public ChangePasswordHandler( IRepository<ApplicationUser> userRepository) { 
            _userRepository = userRepository; 
        } 
        public async ValueTask<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken) { 
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken); 
            if (user == null ) throw new UnauthorizedAccessException("Kullanıcı bulunamadı"); 
            await _userRepository.UpdateAsync(user, cancellationToken); 
            return Unit.Value; 
        } 
    }

    public record ForgotPasswordCommand(string Email) : IRequest<Unit>; 
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit> { 
        private readonly  IRepository<ApplicationUser> _userRepository; 
        public ForgotPasswordHandler( IRepository<ApplicationUser> userRepository) { _userRepository = userRepository; } 
        public async ValueTask<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) { 
               var user = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

            if (user == null) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
            await _userRepository.UpdateAsync(user, cancellationToken); 
            return Unit.Value; 
        } 
    }

    public record ResetPasswordCommand(string Email, string ResetToken, string NewPassword) : IRequest<Unit>; 
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit> { 
        private readonly  IRepository<ApplicationUser> _userRepository; 
        public ResetPasswordHandler( 
            IRepository<ApplicationUser> userRepository) { 
            _userRepository = userRepository; 
        } 

        public async ValueTask<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) { 
              var user = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

            if (user == null) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
            await _userRepository.UpdateAsync(user, cancellationToken); 
            return Unit.Value; 
        } 
    }

    public record VerifyEmailCommand(string UserId, string VerificationCode) : IRequest<Unit>; 
    public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Unit> { 
        private readonly  IRepository<ApplicationUser> _userRepository; 
        public VerifyEmailHandler( IRepository<ApplicationUser> userRepository) { _userRepository = userRepository; } 
        public async ValueTask<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) { 
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken); 
            if (user == null ) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
            await _userRepository.UpdateAsync(user, cancellationToken); 
            return Unit.Value; 
        } 
    }

    public record ResendVerificationCommand(string Email) : IRequest<Unit>;

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

public record Enable2FACommand(string UserId) : IRequest<Unit>; 
public record Disable2FACommand(string UserId) : IRequest<Unit>; 
public class Enable2FAHandler : IRequestHandler<Enable2FACommand, Unit> { 
    private readonly  IRepository<ApplicationUser> _userRepository; 
    public Enable2FAHandler( IRepository<ApplicationUser> userRepository) { _userRepository = userRepository; } 
    public async ValueTask<Unit> Handle(Enable2FACommand request, CancellationToken cancellationToken) { 
        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken); 
        if (user == null) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
        await _userRepository.UpdateAsync(user, cancellationToken); 
        return Unit.Value; 
    } 
} 
public class Disable2FAHandler : IRequestHandler<Disable2FACommand, Unit> { 
    private readonly  IRepository<ApplicationUser> _userRepository; 
    public Disable2FAHandler( IRepository<ApplicationUser> userRepository) { _userRepository = userRepository; } 
    public async ValueTask<Unit> Handle(Disable2FACommand request, CancellationToken cancellationToken) { 
        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken); 
        if (user == null) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
        await _userRepository.UpdateAsync(user, cancellationToken); 
        return Unit.Value; 
    } 
}

}





