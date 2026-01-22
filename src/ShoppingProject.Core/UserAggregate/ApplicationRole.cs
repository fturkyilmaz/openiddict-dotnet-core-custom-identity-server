using ShoppingProject.Core.Common;
using Ardalis.SharedKernel;

namespace ShoppingProject.Core.UserAggregate;

public class ApplicationRole : BaseEntity<Guid>, IAggregateRoot { 
  public string Name { get; set; } = default!; 
  public string Description { get; set; } = string.Empty; 
  public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
