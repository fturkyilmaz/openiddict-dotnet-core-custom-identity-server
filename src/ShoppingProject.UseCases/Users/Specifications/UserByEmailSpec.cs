using Ardalis.Specification;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UseCases.UserRoles.Specifications
{
    public class UserByEmailSpec : Specification<ApplicationUser>
    {
        public UserByEmailSpec(string email)
        {
            Query.Where(u => u.Email == email)
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role);
        }
    }
}
