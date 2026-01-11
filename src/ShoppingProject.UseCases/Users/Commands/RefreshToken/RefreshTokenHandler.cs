    
using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.RefreshToken;
    
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

