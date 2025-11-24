using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void FromHash_ValidHash_ShouldReturnPassword()
    {
        // Arrange
        var hash = "$2a$12$SomeHashedPassword";

        // Act
        var password = Password.FromHash(hash);

        // Assert
        password.Hash.Should().Be(hash);
    }

    [Fact]
    public void FromHash_NullHash_ShouldThrowArgumentException()
    {
        // Arrange
        string? hash = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Password.FromHash(hash!));
    }

    [Fact]
    public void FromHash_EmptyHash_ShouldThrowArgumentException()
    {
        // Arrange
        var hash = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Password.FromHash(hash));
    }

    [Fact]
    public void Create_PlainPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var plainPassword = "MyPassword123!";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => Password.Create(plainPassword));
    }

    [Fact]
    public void ToString_ShouldReturnRedacted()
    {
        // Arrange
        var hash = "$2a$12$SomeHashedPassword";
        var password = Password.FromHash(hash);

        // Act & Assert
        password.ToString().Should().Be("[REDACTED]");
    }

    [Fact]
    public void Equals_SameHash_ShouldReturnTrue()
    {
        // Arrange
        var hash = "$2a$12$SomeHashedPassword";
        var password1 = Password.FromHash(hash);
        var password2 = Password.FromHash(hash);

        // Act & Assert
        password1.Equals(password2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentHash_ShouldReturnFalse()
    {
        // Arrange
        var password1 = Password.FromHash("$2a$12$Hash1");
        var password2 = Password.FromHash("$2a$12$Hash2");

        // Act & Assert
        password1.Equals(password2).Should().BeFalse();
    }
}

