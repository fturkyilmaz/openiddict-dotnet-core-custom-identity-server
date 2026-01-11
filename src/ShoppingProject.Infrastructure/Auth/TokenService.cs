using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Core.SecurityAggregate;
using System.Security.Cryptography;
using ShoppingProject.UseCases.Users.Interfaces;
using OpenIddict.Abstractions;

namespace ShoppingProject.Infrastructure.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;

        private static readonly Dictionary<string, Guid> _refreshTokens = new();

        public TokenService(
            IConfiguration config,
            IOpenIddictTokenManager tokenManager,
            IOpenIddictAuthorizationManager authorizationManager)
        {
            _config = config;
            _tokenManager = tokenManager;
            _authorizationManager = authorizationManager;
        }

        public ValueTask<string> CreateAccessToken(ApplicationUser user, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return ValueTask.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public ValueTask<string> CreateRefreshToken(ApplicationUser user, CancellationToken cancellationToken)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            var tokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenLifetimeDays"] ?? "7"))
            };

            _refreshTokens[refreshToken] = user.Id;

            // TODO: EF Core repository ile DB’ye kaydet
            return ValueTask.FromResult(refreshToken);
        }

        public ValueTask<Guid> ValidateRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out var userId))
            {
                return ValueTask.FromResult(userId);
            }
            return ValueTask.FromResult(Guid.Empty);
        }

        // Todo: OpenIddict token revoke by token id
        public async ValueTask RevokeTokenAsync(string tokenId, CancellationToken cancellationToken)
        {
            var token = await _tokenManager.FindByIdAsync(tokenId, cancellationToken);
            if (token != null)
            {
                await _tokenManager.RevokeAsync(
                    subject: await _tokenManager.GetSubjectAsync(token, cancellationToken),
                    client: await _tokenManager.GetApplicationIdAsync(token, cancellationToken),
                    status: OpenIddictConstants.Statuses.Revoked,
                    type: await _tokenManager.GetTypeAsync(token, cancellationToken),
                    cancellationToken: cancellationToken);
            }
        }

        // Todo: Logout everywhere by user id
        public async ValueTask LogoutEverywhereAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _authorizationManager.RevokeAsync(
                subject: userId.ToString(),
                client: null,
                status: OpenIddictConstants.Statuses.Revoked,
                type: null,
                cancellationToken: cancellationToken);
        }

    }
}
