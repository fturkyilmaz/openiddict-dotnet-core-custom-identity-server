namespace ShoppingProject.UseCases.Users.DTOs
{
    public record UserListItemDto(
        Guid Id,
        string UserName,
        string Email,
        string DisplayName,
        int Status
    );
}
