using MediatR;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.Infrastructure.Auth;

namespace ShoppingProject.UseCases.Users.Register;

public record RegisterUserCommand(string UserName, string Email, string DisplayName, string Password) : IRequest<Guid>;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IRepository<ApplicationUser> _repo;

    public RegisterUserHandler(IRepository<ApplicationUser> repo) => _repo = repo;

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
    {
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

        await _repo.AddAsync(user, ct);
        return user.Id;
    }
}
