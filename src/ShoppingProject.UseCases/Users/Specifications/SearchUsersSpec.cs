using Ardalis.Specification;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UseCases.Users.Specifications
{
    public sealed class SearchUsersSpec : Specification<ApplicationUser>
    {
        public SearchUsersSpec(string? search, bool? isDeleted, bool includeClaims)
        {
            if (isDeleted.HasValue)
                Query.Where(u => u.IsDeleted == isDeleted.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                Query.Where(u =>
                    u.UserName.StartsWith(term) ||
                    u.Email.StartsWith(term) ||
                    u.DisplayName.StartsWith(term));

                if (includeClaims)
                    Query.Where(u => u.Claims.Any(c => c.Value.StartsWith(term)));
            }

            Query.OrderBy(u => u.UserName);
        }
    }   
}


