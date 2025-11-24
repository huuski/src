using ApplicationLayer.DTOs.SpotlightCard;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class SpotlightCardServiceTests
{
    private readonly Mock<ISpotlightCardRepository> _spotlightCardRepositoryMock;
    private readonly ISpotlightCardService _spotlightCardService;

    public SpotlightCardServiceTests()
    {
        _spotlightCardRepositoryMock = new Mock<ISpotlightCardRepository>();
        _spotlightCardService = new SpotlightCardService(_spotlightCardRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnSpotlightCardDto()
    {
        // Arrange
        var spotlightCardId = Guid.NewGuid();
        var spotlightCard = CreateTestSpotlightCard(spotlightCardId);

        _spotlightCardRepositoryMock
            .Setup(x => x.GetByIdAsync(spotlightCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spotlightCard);

        // Act
        var result = await _spotlightCardService.GetByIdAsync(spotlightCardId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(spotlightCardId);
        result.Title.Should().Be(spotlightCard.Title);
        result.ShortDescription.Should().Be(spotlightCard.ShortDescription);
    }

    [Fact]
    public async Task GetByIdAsync_SpotlightCardNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var spotlightCardId = Guid.NewGuid();

        _spotlightCardRepositoryMock
            .Setup(x => x.GetByIdAsync(spotlightCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpotlightCard?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _spotlightCardService.GetByIdAsync(spotlightCardId)
        );
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateSpotlightCard()
    {
        // Arrange
        var dto = new CreateSpotlightCardDto
        {
            Title = "Promoção Especial",
            ShortDescription = "Desconto de 50%",
            LongDescription = "Aproveite nosso desconto especial",
            InitDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            Image = "https://example.com/image.jpg",
            ButtonTitle = "Saiba Mais",
            ButtonLink = "https://example.com/promo"
        };

        _spotlightCardRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpotlightCard sc, CancellationToken ct) => sc);

        // Act
        var result = await _spotlightCardService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.ShortDescription.Should().Be(dto.ShortDescription);
        result.LongDescription.Should().Be(dto.LongDescription);
        _spotlightCardRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateSpotlightCard()
    {
        // Arrange
        var spotlightCardId = Guid.NewGuid();
        var existingSpotlightCard = CreateTestSpotlightCard(spotlightCardId);

        var dto = new UpdateSpotlightCardDto
        {
            Title = "Updated Title",
            ShortDescription = "Updated Short",
            LongDescription = "Updated Long Description",
            InitDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(60),
            Image = "https://example.com/updated-image.jpg"
        };

        _spotlightCardRepositoryMock
            .Setup(x => x.GetByIdAsync(spotlightCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSpotlightCard);

        _spotlightCardRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpotlightCard sc, CancellationToken ct) => sc);

        // Act
        var result = await _spotlightCardService.UpdateAsync(spotlightCardId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.ShortDescription.Should().Be(dto.ShortDescription);
        _spotlightCardRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_ValidId_ShouldActivateSpotlightCard()
    {
        // Arrange
        var spotlightCardId = Guid.NewGuid();
        var spotlightCard = CreateTestSpotlightCard(spotlightCardId);
        spotlightCard.Deactivate();

        _spotlightCardRepositoryMock
            .Setup(x => x.GetByIdAsync(spotlightCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spotlightCard);

        _spotlightCardRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpotlightCard sc, CancellationToken ct) => sc);

        // Act
        var result = await _spotlightCardService.ActivateAsync(spotlightCardId);

        // Assert
        result.Should().NotBeNull();
        result.Inactive.Should().BeFalse();
        _spotlightCardRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_ValidId_ShouldDeactivateSpotlightCard()
    {
        // Arrange
        var spotlightCardId = Guid.NewGuid();
        var spotlightCard = CreateTestSpotlightCard(spotlightCardId);

        _spotlightCardRepositoryMock
            .Setup(x => x.GetByIdAsync(spotlightCardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(spotlightCard);

        _spotlightCardRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpotlightCard sc, CancellationToken ct) => sc);

        // Act
        var result = await _spotlightCardService.DeactivateAsync(spotlightCardId);

        // Assert
        result.Should().NotBeNull();
        result.Inactive.Should().BeTrue();
        _spotlightCardRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SpotlightCard>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private SpotlightCard CreateTestSpotlightCard(
        Guid id,
        string title = "Test Spotlight",
        string shortDescription = "Test Short",
        string longDescription = "Test Long Description",
        DateTime? initDate = null,
        DateTime? endDate = null)
    {
        var spotlightCard = new SpotlightCard(
            title,
            shortDescription,
            longDescription,
            initDate ?? DateTime.UtcNow,
            endDate ?? DateTime.UtcNow.AddDays(30)
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(spotlightCard, id);
        }

        return spotlightCard;
    }
}

