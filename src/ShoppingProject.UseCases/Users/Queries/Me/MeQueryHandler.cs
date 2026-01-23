using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Queries.Me;
  
public class MeQueryHandler : IRequestHandler<MeQuery, MeDto>
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly ICurrentUserService _currentUser;

    public MeQueryHandler(IRepository<ApplicationUser> userRepository, ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async ValueTask<MeDto> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var guid)) throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        var user = await _userRepository.GetByIdAsync(guid, cancellationToken);
        
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
