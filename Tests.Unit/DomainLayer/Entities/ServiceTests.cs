using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class ServiceTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateService()
    {
        // Arrange
        var name = "Facial Treatment";
        var description = "Deep cleansing facial treatment";
        var category = ServiceCategory.Aesthetical;
        var amount = 150.00m;
        var duration = TimeSpan.FromMinutes(60);
        var image = "https://example.com/image.jpg";

        // Act
        var service = new Service(name, description, category, amount, duration, image);

        // Assert
        service.Should().NotBeNull();
        service.Name.Should().Be(name);
        service.Description.Should().Be(description);
        service.Category.Should().Be(category);
        service.Amount.Should().Be(amount);
        service.Duration.Should().Be(duration);
        service.Image.Should().Be(image);
        service.Id.Should().NotBeEmpty();
        service.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithoutImage_ShouldCreateService()
    {
        // Arrange
        var name = "Medical Consultation";
        var description = "General medical consultation";
        var category = ServiceCategory.Medical;
        var amount = 200.00m;
        var duration = TimeSpan.FromMinutes(30);

        // Act
        var service = new Service(name, description, category, amount, duration);

        // Assert
        service.Should().NotBeNull();
        service.Image.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var description = "Description";
        var category = ServiceCategory.Medical;
        var amount = 100.00m;
        var duration = TimeSpan.FromMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Service(null!, description, category, amount, duration)
        );
    }

    [Fact]
    public void Constructor_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var description = "Description";
        var category = ServiceCategory.Medical;
        var amount = 100.00m;
        var duration = TimeSpan.FromMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Service("", description, category, amount, duration)
        );
    }

    [Fact]
    public void Constructor_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Service";
        var description = "Description";
        var category = ServiceCategory.Medical;
        var amount = -10.00m;
        var duration = TimeSpan.FromMinutes(30);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Service(name, description, category, amount, duration)
        );
    }

    [Fact]
    public void Constructor_ZeroDuration_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Service";
        var description = "Description";
        var category = ServiceCategory.Medical;
        var amount = 100.00m;
        var duration = TimeSpan.Zero;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Service(name, description, category, amount, duration)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateService()
    {
        // Arrange
        var service = new Service(
            "Old Name",
            "Old Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );

        var newName = "New Name";
        var newDescription = "New Description";
        var newCategory = ServiceCategory.Aesthetical;
        var newAmount = 200.00m;
        var newDuration = TimeSpan.FromMinutes(60);
        var newImage = "https://example.com/new-image.jpg";

        // Act
        service.Update(newName, newDescription, newCategory, newAmount, newDuration, newImage);

        // Assert
        service.Name.Should().Be(newName);
        service.Description.Should().Be(newDescription);
        service.Category.Should().Be(newCategory);
        service.Amount.Should().Be(newAmount);
        service.Duration.Should().Be(newDuration);
        service.Image.Should().Be(newImage);
        service.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateImage_ValidImage_ShouldUpdateImage()
    {
        // Arrange
        var service = new Service(
            "Service",
            "Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );

        var newImage = "https://example.com/new-image.jpg";

        // Act
        service.UpdateImage(newImage);

        // Assert
        service.Image.Should().Be(newImage);
        service.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var service = new Service(
            "Service",
            "Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );

        // Act
        service.MarkAsDeleted();

        // Assert
        service.IsDeleted.Should().BeTrue();
        service.DeletedAt.Should().NotBeNull();
        service.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

