using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using InfrastructureLayer.Repositories;
using Moq;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryNegotiationItemRepositoryTests
{
    private readonly Mock<INegotiationRepository> _negotiationRepositoryMock;
    private readonly InMemoryNegotiationItemRepository _repository;

    public InMemoryNegotiationItemRepositoryTests()
    {
        _negotiationRepositoryMock = new Mock<INegotiationRepository>();
        _repository = new InMemoryNegotiationItemRepository(_negotiationRepositoryMock.Object);
        
        // Setup default mock behavior for GetByIdAsync to return a negotiation
        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken ct) => CreateTestNegotiation(id));
    }

    [Fact]
    public async Task CreateAsync_ValidItem_ShouldCreateItem()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var item = new NegotiationItem(
            negotiationId,
            NegotiationItemType.Product,
            2,
            100m,
            180m,
            20m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        // Act
        var result = await _repository.CreateAsync(item);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(item.Id);
    }

    [Fact]
    public async Task CreateAsync_NegotiationNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var item = new NegotiationItem(
            negotiationId,
            NegotiationItemType.Product,
            2,
            100m,
            180m,
            20m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.CreateAsync(item)
        );
    }

    [Fact]
    public async Task GetByIdAsync_ExistingItem_ShouldReturnItem()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var item = new NegotiationItem(
            negotiationId,
            NegotiationItemType.Product,
            2,
            100m,
            180m,
            20m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        await _repository.CreateAsync(item);

        // Act
        var result = await _repository.GetByIdAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Type.Should().Be(item.Type);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentItem_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Arrange
        var negotiationId1 = Guid.NewGuid();
        var negotiationId2 = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId1));

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId2));

        var item1 = new NegotiationItem(negotiationId1, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        var item2 = new NegotiationItem(negotiationId2, NegotiationItemType.Service, 1, 200m, 180m, 20m);

        await _repository.CreateAsync(item1);
        await _repository.CreateAsync(item2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByNegotiationIdAsync_ExistingNegotiationId_ShouldReturnItems()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        var item1 = new NegotiationItem(negotiationId, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        var item2 = new NegotiationItem(negotiationId, NegotiationItemType.Service, 1, 200m, 180m, 20m);

        await _repository.CreateAsync(item1);
        await _repository.CreateAsync(item2);

        // Act
        var result = await _repository.GetByNegotiationIdAsync(negotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(i => i.NegotiationId == negotiationId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByNegotiationIdAsync_NonExistentNegotiationId_ShouldReturnEmpty()
    {
        // Arrange
        var nonExistentNegotiationId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByNegotiationIdAsync(nonExistentNegotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingItem_ShouldUpdateItem()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        var item = new NegotiationItem(negotiationId, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        await _repository.CreateAsync(item);

        item.Update(NegotiationItemType.Service, 3, 150m, 420m, 30m);

        // Act
        var result = await _repository.UpdateAsync(item);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(NegotiationItemType.Service);
        result.Quantity.Should().Be(3);
    }

    [Fact]
    public async Task UpdateAsync_ChangeNegotiationId_ShouldUpdateItemAssociation()
    {
        // Arrange
        var negotiationId1 = Guid.NewGuid();
        var negotiationId2 = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId1));

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId2));

        var item = new NegotiationItem(negotiationId1, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        await _repository.CreateAsync(item);

        // Create new item with different negotiation ID using reflection
        var updatedItem = new NegotiationItem(negotiationId2, NegotiationItemType.Service, 1, 200m, 180m, 20m);
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(updatedItem, item.Id);
        }

        // Act
        var result = await _repository.UpdateAsync(updatedItem);

        // Assert
        result.Should().NotBeNull();
        result.NegotiationId.Should().Be(negotiationId2);

        // Verify item was moved to new negotiation
        var itemsFromOldNegotiation = await _repository.GetByNegotiationIdAsync(negotiationId1);
        itemsFromOldNegotiation.Should().NotContain(i => i.Id == item.Id);

        var itemsFromNewNegotiation = await _repository.GetByNegotiationIdAsync(negotiationId2);
        itemsFromNewNegotiation.Should().Contain(i => i.Id == item.Id);
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_ShouldMarkAsDeleted()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        var item = new NegotiationItem(negotiationId, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        await _repository.CreateAsync(item);

        // Act
        var result = await _repository.DeleteAsync(item.Id);

        // Assert
        result.Should().BeTrue();
        var deletedItem = await _repository.GetByIdAsync(item.Id);
        deletedItem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentItem_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_DeletedItem_ShouldNotAppearInGetAll()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        var item1 = new NegotiationItem(negotiationId, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        var item2 = new NegotiationItem(negotiationId, NegotiationItemType.Service, 1, 200m, 180m, 20m);

        await _repository.CreateAsync(item1);
        await _repository.CreateAsync(item2);

        // Act
        await _repository.DeleteAsync(item1.Id);
        var allItems = await _repository.GetAllAsync();

        // Assert
        allItems.Should().HaveCount(1);
        allItems.First().Id.Should().Be(item2.Id);
    }

    [Fact]
    public async Task DeleteAsync_DeletedItem_ShouldNotAppearInGetByNegotiationId()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestNegotiation(negotiationId));

        var item1 = new NegotiationItem(negotiationId, NegotiationItemType.Product, 2, 100m, 180m, 20m);
        var item2 = new NegotiationItem(negotiationId, NegotiationItemType.Service, 1, 200m, 180m, 20m);

        await _repository.CreateAsync(item1);
        await _repository.CreateAsync(item2);

        // Act
        await _repository.DeleteAsync(item1.Id);
        var items = await _repository.GetByNegotiationIdAsync(negotiationId);

        // Assert
        items.Should().HaveCount(1);
        items.First().Id.Should().Be(item2.Id);
    }

    private Negotiation CreateTestNegotiation(Guid id)
    {
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(negotiation, id);
        }

        return negotiation;
    }
}

