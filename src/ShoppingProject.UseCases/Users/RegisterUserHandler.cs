using System.Security.Claims;
using MediatR;
using Ardalis.SharedKernel;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UseCases.Users
{
  // Command
  public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;

  // Handler
  public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
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

    public async ValueTask<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
      // Existing user 
      var existing = await _userRepository
          .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

      if (existing is not null)
        throw new InvalidOperationException("User already exists");

      var (salt, hash) = PasswordHasher.Hash(request.Password);

      var user = new ApplicationUser
      {
        Id = Guid.NewGuid(),
        UserName = request.UserName,
        Email = request.Email,
        DisplayName = request.DisplayName,
        PasswordSalt = salt,
        PasswordHash = hash
      };

      //  Hash password
      var (salt, hash) = PasswordHasher.Hash(request.Password);

      // Create user
      var user = new ApplicationUser
      {
        Id = Guid.NewGuid(),
        UserName = request.UserName,
        Email = request.Email,
        DisplayName = request.DisplayName,
        PasswordSalt = salt,
        PasswordHash = hash
      };

      await _userRepository.AddAsync(user, cancellationToken);

      // Generate tokens
      var accessToken = await _tokenService.CreateAccessToken(user, cancellationToken);
      var refreshToken = await _tokenService.CreateRefreshToken(user, cancellationToken);

      // Audit log can be added here for user registration create event

      return user.Id;
    }
  }
}
