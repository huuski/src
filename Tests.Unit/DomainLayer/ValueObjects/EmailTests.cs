using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ValidEmail_ShouldReturnEmail()
    {
        // Arrange
        var value = "test@example.com";

        // Act
        var email = Email.Create(value);

        // Assert
        email.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_ValidEmailWithUppercase_ShouldConvertToLowercase()
    {
        // Arrange
        var value = "TEST@EXAMPLE.COM";

        // Act
        var email = Email.Create(value);

        // Assert
        email.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(value!));
    }

    [Fact]
    public void Create_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(value));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test@.com")]
    [InlineData("test.example.com")]
    public void Create_InvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Fact]
    public void Create_TrimsWhitespace_ShouldTrimValue()
    {
        // Arrange
        var value = "  test@example.com  ";

        // Act
        var email = Email.Create(value);

        // Assert
        email.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
    }
}

