using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+55 11 98765-4321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("11 98765-4321")]
    [InlineData("11987654321")]
    [InlineData("+5511987654321")]
    public void Create_ValidPhoneNumber_ShouldReturnPhoneNumber(string phoneNumber)
    {
        // Act
        var phone = PhoneNumber.Create(phoneNumber);

        // Assert
        phone.Value.Should().Be(phoneNumber.Trim());
    }

    [Fact]
    public void Create_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(value!));
    }

    [Fact]
    public void Create_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(value));
    }

    [Theory]
    [InlineData("12345")] // Too short
    [InlineData("12345678901234567")] // Too long
    [InlineData("abc1234567")] // Contains letters
    public void Create_InvalidPhoneNumber_ShouldThrowArgumentException(string invalidPhone)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(invalidPhone));
    }

    [Fact]
    public void Create_TrimsWhitespace_ShouldTrimValue()
    {
        // Arrange
        var value = "  +55 11 98765-4321  ";

        // Act
        var phone = PhoneNumber.Create(value);

        // Assert
        phone.Value.Should().Be("+55 11 98765-4321");
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+55 11 98765-4321");
        var phone2 = PhoneNumber.Create("+55 11 98765-4321");

        // Act & Assert
        phone1.Equals(phone2).Should().BeTrue();
    }
}

