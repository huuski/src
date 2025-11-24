using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.ValueObjects;

public class NameTests
{
    [Fact]
    public void Create_ValidName_ShouldReturnName()
    {
        // Arrange
        var value = "John Doe";

        // Act
        var name = Name.Create(value);

        // Assert
        name.Value.Should().Be("John Doe");
    }

    [Fact]
    public void Create_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Name.Create(value!));
    }

    [Fact]
    public void Create_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Name.Create(value));
    }

    [Fact]
    public void Create_WhitespaceValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Name.Create(value));
    }

    [Fact]
    public void Create_TooShort_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "A";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Name.Create(value));
    }

    [Fact]
    public void Create_TooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Name.Create(value));
    }

    [Fact]
    public void Create_TrimsWhitespace_ShouldTrimValue()
    {
        // Arrange
        var value = "  John Doe  ";

        // Act
        var name = Name.Create(value);

        // Assert
        name.Value.Should().Be("John Doe");
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var name1 = Name.Create("John Doe");
        var name2 = Name.Create("John Doe");

        // Act & Assert
        name1.Equals(name2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var name1 = Name.Create("John Doe");
        var name2 = Name.Create("Jane Doe");

        // Act & Assert
        name1.Equals(name2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var name = Name.Create("John Doe");

        // Act & Assert
        name.ToString().Should().Be("John Doe");
    }

    [Fact]
    public void ImplicitOperator_ShouldReturnValue()
    {
        // Arrange
        var name = Name.Create("John Doe");

        // Act
        string value = name;

        // Assert
        value.Should().Be("John Doe");
    }
}

