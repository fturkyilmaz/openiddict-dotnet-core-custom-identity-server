using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;


namespace ShoppingProject.Infrastructure.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
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
            // Basic refresh token generation logic
            var refreshToken = Guid.NewGuid().ToString("N");
            return ValueTask.FromResult(refreshToken);
        }
    }
}
