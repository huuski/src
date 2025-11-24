using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class SupplyTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateSupply()
    {
        // Arrange
        var name = "Algodão";
        var stock = 100;

        // Act
        var supply = new Supply(name, stock);

        // Assert
        supply.Should().NotBeNull();
        supply.Name.Should().Be(name);
        supply.Stock.Should().Be(stock);
        supply.Id.Should().NotBeEmpty();
        supply.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ZeroStock_ShouldCreateSupply()
    {
        // Arrange
        var name = "Gaze";
        var stock = 0;

        // Act
        var supply = new Supply(name, stock);

        // Assert
        supply.Should().NotBeNull();
        supply.Stock.Should().Be(0);
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var stock = 50;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Supply(null!, stock)
        );
    }

    [Fact]
    public void Constructor_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var stock = 50;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Supply("", stock)
        );
    }

    [Fact]
    public void Constructor_WhitespaceName_ShouldThrowArgumentException()
    {
        // Arrange
        var stock = 50;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Supply("   ", stock)
        );
    }

    [Fact]
    public void Constructor_NegativeStock_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Algodão";
        var stock = -10;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Supply(name, stock)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateSupply()
    {
        // Arrange
        var supply = new Supply("Old Name", 50);
        var newName = "New Name";
        var newStock = 100;

        // Act
        supply.Update(newName, newStock);

        // Assert
        supply.Name.Should().Be(newName);
        supply.Stock.Should().Be(newStock);
        supply.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var supply = new Supply("Name", 50);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            supply.Update(null!, 100)
        );
    }

    [Fact]
    public void Update_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var supply = new Supply("Name", 50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            supply.Update("", 100)
        );
    }

    [Fact]
    public void Update_NegativeStock_ShouldThrowArgumentException()
    {
        // Arrange
        var supply = new Supply("Name", 50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            supply.Update("New Name", -10)
        );
    }

    [Fact]
    public void UpdateStock_ValidStock_ShouldUpdateStock()
    {
        // Arrange
        var supply = new Supply("Algodão", 50);
        var newStock = 75;

        // Act
        supply.UpdateStock(newStock);

        // Assert
        supply.Stock.Should().Be(newStock);
        supply.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateStock_ZeroStock_ShouldUpdateStock()
    {
        // Arrange
        var supply = new Supply("Algodão", 50);

        // Act
        supply.UpdateStock(0);

        // Assert
        supply.Stock.Should().Be(0);
    }

    [Fact]
    public void UpdateStock_NegativeStock_ShouldThrowArgumentException()
    {
        // Arrange
        var supply = new Supply("Algodão", 50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            supply.UpdateStock(-10)
        );
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var supply = new Supply("Algodão", 50);

        // Act
        supply.MarkAsDeleted();

        // Assert
        supply.IsDeleted.Should().BeTrue();
        supply.DeletedAt.Should().NotBeNull();
        supply.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

