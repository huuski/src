using DomainLayer.Entities;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemorySpotlightCardRepositoryTests
{
    private readonly InMemorySpotlightCardRepository _repository;

    public InMemorySpotlightCardRepositoryTests()
    {
        _repository = new InMemorySpotlightCardRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidSpotlightCard_ShouldCreate()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Test Title",
            "Test Short",
            "Test Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        // Act
        var result = await _repository.CreateAsync(spotlightCard);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(spotlightCard.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSpotlightCard_ShouldReturn()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Test Title",
            "Test Short",
            "Test Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(spotlightCard);

        // Act
        var result = await _repository.GetByIdAsync(spotlightCard.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(spotlightCard.Id);
        result.Title.Should().Be(spotlightCard.Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSpotlightCards()
    {
        // Arrange
        var spotlightCard1 = new SpotlightCard(
            "Title 1",
            "Short 1",
            "Long 1",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        var spotlightCard2 = new SpotlightCard(
            "Title 2",
            "Short 2",
            "Long 2",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );

        await _repository.CreateAsync(spotlightCard1);
        await _repository.CreateAsync(spotlightCard2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingSpotlightCard_ShouldUpdate()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Old Title",
            "Old Short",
            "Old Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(spotlightCard);

        spotlightCard.Update(
            "New Title",
            "New Short",
            "New Long",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(60)
        );

        // Act
        var result = await _repository.UpdateAsync(spotlightCard);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        result.ShortDescription.Should().Be("New Short");
    }

    [Fact]
    public async Task DeleteAsync_ExistingSpotlightCard_ShouldMarkAsDeleted()
    {
        // Arrange
        var spotlightCard = new SpotlightCard(
            "Test Title",
            "Test Short",
            "Test Long",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30)
        );
        await _repository.CreateAsync(spotlightCard);

        // Act
        var result = await _repository.DeleteAsync(spotlightCard.Id);

        // Assert
        result.Should().BeTrue();
        var deletedSpotlightCard = await _repository.GetByIdAsync(spotlightCard.Id);
        deletedSpotlightCard.Should().BeNull();
    }
}

