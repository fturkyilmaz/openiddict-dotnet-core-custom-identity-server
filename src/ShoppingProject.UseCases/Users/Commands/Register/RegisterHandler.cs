using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;

namespace ShoppingProject.UseCases.Users.Commands.Register
{
  // Handler for user registration
  public class RegisterUserHandler : IRequestHandler<RegisterCommand, Guid>
  {
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(
        IRepository<ApplicationUser> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
      _userRepository = userRepository;
      _tokenService = tokenService;
      _passwordHasher = passwordHasher;
    }

    public async ValueTask<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
      // Check if user already exists by email
      var existing = await _userRepository.FirstOrDefaultAsync(new UserByEmailSpec(request.Email), cancellationToken);


      if (existing is not null)
        throw new InvalidOperationException("User already exists");

      // Hash the provided password
      var hashedPassword = _passwordHasher.HashPassword(request.Password);

      // Create new ApplicationUser entity
      var user = new ApplicationUser
      {
        Id = Guid.NewGuid(),
        UserName = request.UserName,
        Email = request.Email,
        DisplayName = request.DisplayName,
        PasswordHash = hashedPassword,
        CreatedAt = DateTime.UtcNow
      };

      // Save user to repository
      await _userRepository.AddAsync(user, cancellationToken);

      // Generate tokens (optional: auto-login after registration)
      var accessToken = await _tokenService.CreateAccessToken(user, cancellationToken);
      var refreshToken = await _tokenService.CreateRefreshToken(user, cancellationToken);

      // Return the new user's Id
      return user.Id;
    }
  }
}
