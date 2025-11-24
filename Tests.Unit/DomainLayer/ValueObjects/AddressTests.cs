using DomainLayer.ValueObjects;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_ValidAddress_ShouldReturnAddress()
    {
        // Arrange
        var street = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234-567";

        // Act
        var address = Address.Create(street, city, state, zipCode);

        // Assert
        address.Street.Should().Be(street);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.ZipCode.Should().Be(zipCode);
    }

    [Fact]
    public void Create_AddressWithOptionalFields_ShouldReturnAddress()
    {
        // Arrange
        var street = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234-567";
        var country = "Brasil";
        var complement = "Apto 45";

        // Act
        var address = Address.Create(street, city, state, zipCode, country, complement);

        // Assert
        address.Country.Should().Be(country);
        address.Complement.Should().Be(complement);
    }

    [Theory]
    [InlineData(null, "City", "State", "ZipCode")]
    [InlineData("", "City", "State", "ZipCode")]
    [InlineData("   ", "City", "State", "ZipCode")]
    [InlineData("Street", null, "State", "ZipCode")]
    [InlineData("Street", "", "State", "ZipCode")]
    [InlineData("Street", "City", null, "ZipCode")]
    [InlineData("Street", "City", "", "ZipCode")]
    [InlineData("Street", "City", "State", null)]
    [InlineData("Street", "City", "State", "")]
    public void Create_InvalidRequiredFields_ShouldThrowArgumentException(
        string? street, string? city, string? state, string? zipCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Address.Create(street!, city!, state!, zipCode!));
    }

    [Fact]
    public void Create_TrimsWhitespace_ShouldTrimValues()
    {
        // Arrange
        var street = "  Rua das Flores, 123  ";
        var city = "  São Paulo  ";
        var state = "  SP  ";
        var zipCode = "  01234-567  ";

        // Act
        var address = Address.Create(street, city, state, zipCode);

        // Assert
        address.Street.Should().Be("Rua das Flores, 123");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
        address.ZipCode.Should().Be("01234-567");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedAddress()
    {
        // Arrange
        var address = Address.Create(
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234-567",
            "Brasil",
            "Apto 45"
        );

        // Act
        var result = address.ToString();

        // Assert
        result.Should().Contain("Rua das Flores, 123");
        result.Should().Contain("Apto 45");
        result.Should().Contain("São Paulo");
        result.Should().Contain("SP");
        result.Should().Contain("01234-567");
        result.Should().Contain("Brasil");
    }

    [Fact]
    public void Equals_SameAddress_ShouldReturnTrue()
    {
        // Arrange
        var address1 = Address.Create("Street", "City", "State", "ZipCode", "Country", "Complement");
        var address2 = Address.Create("Street", "City", "State", "ZipCode", "Country", "Complement");

        // Act & Assert
        address1.Equals(address2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentAddress_ShouldReturnFalse()
    {
        // Arrange
        var address1 = Address.Create("Street 1", "City", "State", "ZipCode");
        var address2 = Address.Create("Street 2", "City", "State", "ZipCode");

        // Act & Assert
        address1.Equals(address2).Should().BeFalse();
    }
}

