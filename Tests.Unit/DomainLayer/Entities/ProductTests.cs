using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateProduct()
    {
        // Arrange
        var name = "Moisturizing Cream";
        var description = "Hydrating face cream";
        var category = ProductCategory.Cosmetic;
        var amount = 50.00m;
        var image = "https://example.com/image.jpg";

        // Act
        var product = new Product(name, description, category, amount, image);

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Category.Should().Be(category);
        product.Amount.Should().Be(amount);
        product.Image.Should().Be(image);
        product.Id.Should().NotBeEmpty();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithoutImage_ShouldCreateProduct()
    {
        // Arrange
        var name = "Vitamin C";
        var description = "Vitamin C supplement";
        var category = ProductCategory.Supplement;
        var amount = 30.00m;

        // Act
        var product = new Product(name, description, category, amount);

        // Assert
        product.Should().NotBeNull();
        product.Image.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var description = "Description";
        var category = ProductCategory.Cosmetic;
        var amount = 100.00m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Product(null!, description, category, amount)
        );
    }

    [Fact]
    public void Constructor_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var description = "Description";
        var category = ProductCategory.Cosmetic;
        var amount = 100.00m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Product("", description, category, amount)
        );
    }

    [Fact]
    public void Constructor_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Product";
        var description = "Description";
        var category = ProductCategory.Cosmetic;
        var amount = -10.00m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Product(name, description, category, amount)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product(
            "Old Name",
            "Old Description",
            ProductCategory.Cosmetic,
            100.00m
        );

        var newName = "New Name";
        var newDescription = "New Description";
        var newCategory = ProductCategory.Medication;
        var newAmount = 200.00m;
        var newImage = "https://example.com/new-image.jpg";

        // Act
        product.Update(newName, newDescription, newCategory, newAmount, newImage);

        // Assert
        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.Category.Should().Be(newCategory);
        product.Amount.Should().Be(newAmount);
        product.Image.Should().Be(newImage);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateImage_ValidImage_ShouldUpdateImage()
    {
        // Arrange
        var product = new Product(
            "Product",
            "Description",
            ProductCategory.Cosmetic,
            100.00m
        );

        var newImage = "https://example.com/new-image.jpg";

        // Act
        product.UpdateImage(newImage);

        // Assert
        product.Image.Should().Be(newImage);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var product = new Product(
            "Product",
            "Description",
            ProductCategory.Cosmetic,
            100.00m
        );

        // Act
        product.MarkAsDeleted();

        // Assert
        product.IsDeleted.Should().BeTrue();
        product.DeletedAt.Should().NotBeNull();
        product.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

