namespace ShoppingProject.UseCases.Users.DTOs;

public record MeDto(Guid Id, string UserName, string Email, bool EmailVerified, bool TwoFactorEnabled);
