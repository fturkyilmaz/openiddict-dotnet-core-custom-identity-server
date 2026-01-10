using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public LoginUserHandler(
            IRepository<ApplicationUser> userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async ValueTask<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(
                new UserByEmailSpec(request.Email), cancellationToken);

            if (user is null)
                throw new InvalidOperationException("User not found");

            if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password))
                throw new InvalidOperationException($"Invalid credentials , please try again {request.Email} {request.Password} {user.PasswordHash}");

            return new LoginResultDto(
                user.Id,
                user.UserName,
                user.Email,
                user.Roles.Select(r => r.Role.Name).ToList()
            );
        }
    }
}





