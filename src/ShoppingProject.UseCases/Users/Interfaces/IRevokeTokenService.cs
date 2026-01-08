namespace ShoppingProject.UseCases.Users.Interfaces
{
  public interface IRevokeTokenService
  {
    Task RevokeAsync(string tokenId, CancellationToken cancellationToken);
  }
}
