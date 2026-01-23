using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Queries.Search;

public record SearchQuery(
    string? Search,
    int? Status,
    bool IncludeClaims
) : IRequest<List<UserListItemDto>>;
