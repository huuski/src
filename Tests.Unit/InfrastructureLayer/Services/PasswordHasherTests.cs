using ApplicationLayer.Interfaces.Services;
using FluentAssertions;
using InfrastructureLayer.Services;

namespace Tests.Unit.InfrastructureLayer.Services;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ValidPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "MyPassword123!";

        // Act
        var hash = _passwordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$2a$");
    }

    [Fact]
    public void HashPassword_SamePassword_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "MyPassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt generates different salts
    }

    [Fact]
    public void HashPassword_NullPassword_ShouldThrowArgumentException()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordHasher.HashPassword(password!));
    }

    [Fact]
    public void HashPassword_EmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var password = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordHasher.HashPassword(password));
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "MyPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "MyPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hash = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_NullPassword_ShouldReturnFalse()
    {
        // Arrange
        var hash = _passwordHasher.HashPassword("SomePassword");

        // Act
        var result = _passwordHasher.VerifyPassword(null!, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_NullHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "SomePassword";

        // Act
        var result = _passwordHasher.VerifyPassword(password, null!);

        // Assert
        result.Should().BeFalse();
    }
}

