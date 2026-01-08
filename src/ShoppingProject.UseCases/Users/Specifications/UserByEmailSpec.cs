using Ardalis.Specification;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UseCases.Users.Specifications
{
    public class UserByEmailSpec : Specification<ApplicationUser>
    {
        public UserByEmailSpec(string email)
        {
            Query.Where(u => u.Email == email);
        }
    }
}
