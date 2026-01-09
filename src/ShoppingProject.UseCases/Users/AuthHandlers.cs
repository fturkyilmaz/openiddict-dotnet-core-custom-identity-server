using System.Security.Claims;
using Mediator;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;

namespace ShoppingProject.UseCases.Users
{
    // Query: Me (returns current user info)
    public record MeQuery(ClaimsPrincipal Principal) : IRequest<UserInfoDto>;

    public record UserInfoDto(Guid UserId, string Email, IEnumerable<string> Roles);

    public class MeHandler : IRequestHandler<MeQuery, UserInfoDto>
    {
        public ValueTask<UserInfoDto> Handle(MeQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(request.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var email = request.Principal.FindFirst(ClaimTypes.Email)!.Value;
            var roles = request.Principal.FindAll(ClaimTypes.Role).Select(r => r.Value);

            var dto = new UserInfoDto(userId, email, roles);
            return ValueTask.FromResult(dto);
        }
    }


    // Command: Login
    public record LoginUserCommand(string Email, string Password) : IRequest<LoginResultDto>;

    public record LoginResultDto(Guid UserId, string AccessToken, string RefreshToken);

    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginUserHandler(
            IRepository<ApplicationUser> userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async ValueTask<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

            if (user is null)
                throw new InvalidOperationException("User not found");

            if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password))
                throw new InvalidOperationException($"Invalid credentials , please try again {request.Email} {request.Password} {user.PasswordHash}");

            var accessToken = await _tokenService.CreateAccessToken(user, cancellationToken);
            var refreshToken = await _tokenService.CreateRefreshToken(user, cancellationToken);

            return new LoginResultDto(user.Id, accessToken, refreshToken);
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

            return new LoginResultDto(user.Id, newAccessToken, newRefreshToken);
        }
    }
}





