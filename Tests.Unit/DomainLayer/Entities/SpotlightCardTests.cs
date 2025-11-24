using DomainLayer.Entities;
using FluentAssertions;

namespace Tests.Unit.DomainLayer.Entities;

public class SpotlightCardTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateSpotlightCard()
    {
        // Arrange
        var title = "Promoção Especial";
        var shortDescription = "Desconto de 50%";
        var longDescription = "Aproveite nosso desconto especial de 50% em todos os serviços";
        var initDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(30);
        var image = "https://example.com/image.jpg";
        var buttonTitle = "Saiba Mais";
        var buttonLink = "https://example.com/promo";

        // Act
        var spotlightCard = new SpotlightCard(
            title,
            shortDescription,
            longDescription,
            initDate,
            endDate,
            image,
            buttonTitle,
            buttonLink
        );

        // Assert
        spotlightCard.Should().NotBeNull();
        spotlightCard.Title.Should().Be(title);
        spotlightCard.ShortDescription.Should().Be(shortDescription);
        spotlightCard.LongDescription.Should().Be(longDescription);
        spotlightCard.Image.Should().Be(image);
        spotlightCard.ButtonTitle.Should().Be(buttonTitle);
        spotlightCard.ButtonLink.Should().Be(buttonLink);
        spotlightCard.InitDate.Should().Be(initDate);
        spotlightCard.EndDate.Should().Be(endDate);
        spotlightCard.Inactive.Should().BeFalse();
        spotlightCard.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithoutOptionalFields_ShouldCreateSpotlightCard()
    {
        // Arrange
        var title = "Promoção";
        var shortDescription = "Desconto";
        var longDescription = "Descrição longa";
        var initDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(30);

        // Act
        var spotlightCard = new SpotlightCard(
            title,
            shortDescription,
            longDescription,
            initDate,
            endDate
        );

        // Assert
        spotlightCard.Should().NotBeNull();
        spotlightCard.Image.Should().BeNull();
        spotlightCard.ButtonTitle.Should().BeNull();
        spotlightCard.ButtonLink.Should().BeNull();
    }

    [Fact]
    public void Constructor_NullTitle_ShouldThrowArgumentNullException()
    {
        // Arrange
        var shortDescription = "Short";
        var longDescription = "Long";
        var initDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(30);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SpotlightCard(null!, shortDescription, longDescription, initDate, endDate)
        );
    }

    [Fact]
    public void Constructor_EndDateBeforeInitDate_ShouldThrowArgumentException()
    {
        // Arrange
        var initDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new SpotlightCard("Title", "Short", "Long", initDate, endDate)
        );
    }

    [Fact]
    public void Update_ValidParameters_ShouldUpdateSpotlightCard()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Old Title",
            "Old Short",
            "Old Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        var newTitle = "New Title";
        var newShortDescription = "New Short";
        var newLongDescription = "New Long Description";
        var newInitDate = DateTime.UtcNow.AddDays(1);
        var newEndDate = DateTime.UtcNow.AddDays(60);
        var newImage = "https://example.com/new-image.jpg";

        // Act
        spotlightCard.Update(
            newTitle,
            newShortDescription,
            newLongDescription,
            newInitDate,
            newEndDate,
            newImage
        );

        // Assert
        spotlightCard.Title.Should().Be(newTitle);
        spotlightCard.ShortDescription.Should().Be(newShortDescription);
        spotlightCard.LongDescription.Should().Be(newLongDescription);
        spotlightCard.InitDate.Should().Be(newInitDate);
        spotlightCard.EndDate.Should().Be(newEndDate);
        spotlightCard.Image.Should().Be(newImage);
        spotlightCard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Activate_ShouldSetInactiveToFalse()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        spotlightCard.Deactivate();

        // Act
        spotlightCard.Activate();

        // Assert
        spotlightCard.Inactive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveToTrue()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        spotlightCard.Deactivate();

        // Assert
        spotlightCard.Inactive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenInactive_ShouldReturnFalse()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        spotlightCard.Deactivate();

        // Act & Assert
        spotlightCard.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenBeforeInitDate_ShouldReturnFalse()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30)
        );

        // Act & Assert
        spotlightCard.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenAfterEndDate_ShouldReturnFalse()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow.AddDays(-1)
        );

        // Act & Assert
        spotlightCard.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenActiveAndInDateRange_ShouldReturnTrue()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(30)
        );

        // Act & Assert
        spotlightCard.IsActive.Should().BeTrue();
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetDeletedAt()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Title",
            "Short",
            "Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        spotlightCard.MarkAsDeleted();

        // Assert
        spotlightCard.IsDeleted.Should().BeTrue();
        spotlightCard.DeletedAt.Should().NotBeNull();
        spotlightCard.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

