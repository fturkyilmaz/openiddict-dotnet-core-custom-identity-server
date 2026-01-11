using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ShoppingProject.UseCases.Users.Logout;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.UnitTests.UseCases.Users;

public class LogoutEverywhereHandlerTests
{
    private readonly Mock<IRepository<ApplicationUser>> _userRepositoryMock;
    private readonly Mock<IRevokeTokenService> _revokeTokenServiceMock;
    private readonly LogoutEverywhereHandler _handler;

    public LogoutEverywhereHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<ApplicationUser>>();
        _revokeTokenServiceMock = new Mock<IRevokeTokenService>();

        _handler = new LogoutEverywhereHandler(
            _userRepositoryMock.Object,
            _revokeTokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Revoke_All_Tokens_For_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LogoutEverywhereCommand(userId.ToString());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _revokeTokenServiceMock.Verify(
            x => x.RevokeByUserIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser)null);

        var command = new LogoutEverywhereCommand(userId.ToString());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

  [Fact]
    public async Task LogoutEverywhere_Should_Invalidate_AccessToken()
    {
        // 1. Login
        var tokenResponse = await LoginAsync("user@test.com", "Password123!");
        var accessToken = tokenResponse.AccessToken;

        // 2. Token çalışıyor mu?
        var meResponse = await GetMeAsync(accessToken);
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        // 3. Logout everywhere
        await LogoutEverywhereAsync(accessToken);

        // 4. Aynı token artık geçersiz
        var forbiddenResponse = await GetMeAsync(accessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, forbiddenResponse.StatusCode);
    }
}
