using ShoppingProject.UseCases.Users.DTOs;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Queries.Search;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.Core.Interfaces;

public class SearchQueryHandler 
    : IRequestHandler<SearchQuery, List<UserListItemDto>>
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly ICurrentUserService _currentUser;

    public SearchQueryHandler(IRepository<ApplicationUser> userRepository, ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async ValueTask<List<UserListItemDto>> Handle(
        SearchQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.ListAsync(cancellationToken);

        var query = user.AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(u => u.IsDeleted == false);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();

            query = query.Where(u =>
                u.UserName.StartsWith(term) ||
                u.Email.StartsWith(term) ||
                u.DisplayName.StartsWith(term)
            );

            if (request.IncludeClaims)
            {
                query = query.Where(u =>
                    u.Claims.Any(c => c.Value.StartsWith(term))
                );
            }
        }

        return query
            .OrderBy(u => u.UserName)
            .Select(u => new UserListItemDto(
                u.Id,
                u.UserName,
                u.Email,
                u.DisplayName,
                u.IsDeleted ? 0 : 1
            ))
            .ToList();
    }
}
