using ApplicationLayer.DTOs.NegotiationItem;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class NegotiationItemServiceTests
{
    private readonly Mock<INegotiationItemRepository> _negotiationItemRepositoryMock;
    private readonly Mock<INegotiationRepository> _negotiationRepositoryMock;
    private readonly INegotiationItemService _negotiationItemService;

    public NegotiationItemServiceTests()
    {
        _negotiationItemRepositoryMock = new Mock<INegotiationItemRepository>();
        _negotiationRepositoryMock = new Mock<INegotiationRepository>();
        _negotiationItemService = new NegotiationItemService(_negotiationItemRepositoryMock.Object, _negotiationRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnNegotiationItemDto()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var negotiationId = Guid.NewGuid();
        var item = CreateTestNegotiationItem(itemId, negotiationId);

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _negotiationItemService.GetByIdAsync(itemId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(itemId);
        result.NegotiationId.Should().Be(negotiationId);
        result.Type.Should().Be(item.Type);
    }

    [Fact]
    public async Task GetByIdAsync_ItemNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationItemService.GetByIdAsync(itemId)
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNegotiationItems()
    {
        // Arrange
        var items = new List<NegotiationItem>
        {
            CreateTestNegotiationItem(Guid.NewGuid(), Guid.NewGuid()),
            CreateTestNegotiationItem(Guid.NewGuid(), Guid.NewGuid())
        };

        _negotiationItemRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _negotiationItemService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByNegotiationIdAsync_ValidNegotiationId_ShouldReturnItems()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var items = new List<NegotiationItem>
        {
            CreateTestNegotiationItem(Guid.NewGuid(), negotiationId),
            CreateTestNegotiationItem(Guid.NewGuid(), negotiationId)
        };

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _negotiationItemService.GetByNegotiationIdAsync(negotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(i => i.NegotiationId == negotiationId).Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateNegotiationItem()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var negotiation = CreateTestNegotiation(negotiationId);

        var dto = new CreateNegotiationItemDto
        {
            NegotiationId = negotiationId,
            Type = NegotiationItemType.Product,
            Quantity = 2,
            GrossValueUnit = 100m,
            NetValue = 180m,
            DiscountValue = 20m
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationItemRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem item, CancellationToken ct) => item);

        // Act
        var result = await _negotiationItemService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.NegotiationId.Should().Be(negotiationId);
        result.Type.Should().Be(dto.Type);
        result.Quantity.Should().Be(dto.Quantity);
        _negotiationItemRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NegotiationNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var dto = new CreateNegotiationItemDto
        {
            NegotiationId = negotiationId,
            Type = NegotiationItemType.Product,
            Quantity = 2,
            GrossValueUnit = 100m,
            NetValue = 180m,
            DiscountValue = 20m
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationItemService.CreateAsync(dto)
        );

        _negotiationItemRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateNegotiationItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var negotiationId = Guid.NewGuid();
        var existingItem = CreateTestNegotiationItem(itemId, negotiationId);
        var negotiation = CreateTestNegotiation(negotiationId);

        var dto = new UpdateNegotiationItemDto
        {
            Type = NegotiationItemType.Service,
            Quantity = 3,
            GrossValueUnit = 150m,
            NetValue = 420m,
            DiscountValue = 30m
        };

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationItemRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem item, CancellationToken ct) => item);

        // Act
        var result = await _negotiationItemService.UpdateAsync(itemId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(dto.Type);
        result.Quantity.Should().Be(dto.Quantity);
        _negotiationItemRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ItemNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var dto = new UpdateNegotiationItemDto
        {
            Type = NegotiationItemType.Product,
            Quantity = 2,
            GrossValueUnit = 100m,
            NetValue = 180m,
            DiscountValue = 20m
        };

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationItemService.UpdateAsync(itemId, dto)
        );
    }

    [Fact]
    public async Task UpdateAsync_NegotiationNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var negotiationId = Guid.NewGuid();
        var existingItem = CreateTestNegotiationItem(itemId, negotiationId);

        var dto = new UpdateNegotiationItemDto
        {
            Type = NegotiationItemType.Product,
            Quantity = 2,
            GrossValueUnit = 100m,
            NetValue = 180m,
            DiscountValue = 20m
        };

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _negotiationItemService.UpdateAsync(itemId, dto)
        );
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteNegotiationItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var negotiationId = Guid.NewGuid();
        var item = CreateTestNegotiationItem(itemId, negotiationId);

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _negotiationItemRepositoryMock
            .Setup(x => x.DeleteAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _negotiationItemService.DeleteAsync(itemId);

        // Assert
        result.Should().BeTrue();
        _negotiationItemRepositoryMock.Verify(x => x.DeleteAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ItemNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationItemService.DeleteAsync(itemId)
        );
    }

    private NegotiationItem CreateTestNegotiationItem(
        Guid id,
        Guid negotiationId,
        NegotiationItemType type = NegotiationItemType.Product,
        int quantity = 2,
        decimal grossValueUnit = 100m,
        decimal netValue = 180m,
        decimal discountValue = 20m)
    {
        var item = new NegotiationItem(
            negotiationId,
            type,
            quantity,
            grossValueUnit,
            netValue,
            discountValue
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(item, id);
        }

        return item;
    }

    private Negotiation CreateTestNegotiation(
        Guid id,
        Guid customerId = default,
        DateTime? expirationDate = null,
        decimal grossValue = 1000m,
        decimal netValue = 900m,
        decimal discountValue = 100m)
    {
        if (customerId == default)
            customerId = Guid.NewGuid();

        var negotiation = new Negotiation(
            customerId,
            Guid.NewGuid(),
            expirationDate ?? DateTime.UtcNow.AddDays(30),
            grossValue,
            netValue,
            discountValue
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

