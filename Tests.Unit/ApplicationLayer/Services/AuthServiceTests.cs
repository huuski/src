using ApplicationLayer.DTOs.Auth;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.ValueObjects;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var request = new LoginRequestDto { Email = email, Password = password };

        var user = CreateTestUser(email);
        var tokenResult = new TokenResult
        {
            AccessToken = "access_token",
            RefreshToken = "refresh_token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(password, user.Password.Hash))
            .Returns(true);

        _refreshTokenRepositoryMock
            .Setup(x => x.RevokeAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _tokenServiceMock
            .Setup(x => x.GenerateTokensAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResult);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(tokenResult.AccessToken);
        result.RefreshToken.Should().Be(tokenResult.RefreshToken);
        result.User.Email.Should().Be(email);
        result.User.Name.Should().Be(user.Name.Value);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequestDto 
        { 
            Email = "nonexistent@example.com", 
            Password = "Password123!" 
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var email = "test@example.com";
        var request = new LoginRequestDto { Email = email, Password = "WrongPassword" };

        var user = CreateTestUser(email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(request.Password, user.Password.Hash))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_EmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new LoginRequestDto { Email = string.Empty, Password = "Password123!" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_EmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = string.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var email = "test@example.com";
        var newPassword = "NewPassword123!";
        var request = new ResetPasswordRequestDto 
        { 
            Email = email, 
            NewPassword = newPassword 
        };

        var user = CreateTestUser(email);
        var hashedPassword = "$2a$12$NewHashedPassword";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.HashPassword(newPassword))
            .Returns(hashedPassword);

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);

        // Act
        var result = await _authService.ResetPasswordAsync(request);

        // Assert
        result.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_UserNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new ResetPasswordRequestDto 
        { 
            Email = "nonexistent@example.com", 
            NewPassword = "NewPassword123!" 
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _authService.ResetPasswordAsync(request)
        );
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var refreshToken = "valid_refresh_token";
        var userId = Guid.NewGuid();
        var user = CreateTestUser("test@example.com", userId);
        var tokenResult = new TokenResult
        {
            AccessToken = "new_access_token",
            RefreshToken = "new_refresh_token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var storedRefreshToken = new RefreshToken(
            userId,
            refreshToken,
            DateTime.UtcNow.AddDays(7)
        );

        _tokenServiceMock
            .Setup(x => x.ValidateRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId.ToString());

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedRefreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x => x.GenerateTokensAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResult);

        _refreshTokenRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(tokenResult.AccessToken);
        result.RefreshToken.Should().Be(tokenResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var refreshToken = "invalid_refresh_token";

        _tokenServiceMock
            .Setup(x => x.ValidateRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.RefreshTokenAsync(refreshToken)
        );
    }

    [Fact]
    public async Task RefreshTokenAsync_UserNotFound_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var refreshToken = "valid_refresh_token";
        var userId = Guid.NewGuid();

        var storedRefreshToken = new RefreshToken(
            userId,
            refreshToken,
            DateTime.UtcNow.AddDays(7)
        );

        _tokenServiceMock
            .Setup(x => x.ValidateRefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId.ToString());

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedRefreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.RefreshTokenAsync(refreshToken)
        );
    }

    private User CreateTestUser(string email, Guid? id = null)
    {
        var userId = id ?? Guid.NewGuid();
        var name = Name.Create("Test User");
        var emailValue = Email.Create(email);
        var address = Address.Create("Street", "City", "State", "12345");
        var phoneNumber = PhoneNumber.Create("+5511999999999");
        var password = Password.FromHash("$2a$12$HashedPassword");

        // We need to create a User, but the constructor is protected
        // Let's use reflection or create a helper method
        var user = new User(
            name,
            "12345678900",
            new DateTime(1990, 1, 1),
            emailValue,
            address,
            phoneNumber,
            password
        );

        // Set the ID using reflection if needed
        if (id.HasValue)
        {
            var idProperty = typeof(Entity).GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(user, id.Value);
            }
        }

        return user;
    }
}

