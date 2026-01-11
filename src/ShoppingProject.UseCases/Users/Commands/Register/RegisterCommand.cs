using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;

namespace ShoppingProject.UseCases.Users.Commands.Register
{
   public record RegisterCommand(string Email, string Password, string UserName, string DisplayName)
      : IRequest<Guid>;
}
