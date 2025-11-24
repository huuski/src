using ApplicationLayer.DTOs.Negotiation;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class NegotiationServiceTests
{
    private readonly Mock<INegotiationRepository> _negotiationRepositoryMock;
    private readonly Mock<INegotiationItemRepository> _negotiationItemRepositoryMock;
    private readonly Mock<INegotiationPaymentMethodRepository> _negotiationPaymentMethodRepositoryMock;
    private readonly INegotiationService _negotiationService;

    public NegotiationServiceTests()
    {
        _negotiationRepositoryMock = new Mock<INegotiationRepository>();
        _negotiationItemRepositoryMock = new Mock<INegotiationItemRepository>();
        _negotiationPaymentMethodRepositoryMock = new Mock<INegotiationPaymentMethodRepository>();
        _negotiationService = new NegotiationService(
            _negotiationRepositoryMock.Object, 
            _negotiationItemRepositoryMock.Object,
            _negotiationPaymentMethodRepositoryMock.Object);
        
        // Setup default mock behavior for GetByNegotiationIdAsync to return empty list
        _negotiationItemRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<NegotiationItem>());

        // Setup default mock behavior for CreateAsync to return the item
        _negotiationItemRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NegotiationItem item, CancellationToken ct) => item);

        // Setup default mock behavior for GetByNegotiationIdAsync to return empty list
        _negotiationPaymentMethodRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<NegotiationPaymentMethod>());
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateNegotiation()
    {
        // Arrange
        var dto = new CreateNegotiationDto
        {
            CustomerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            GrossValue = 1000m,
            NetValue = 900m,
            DiscountValue = 100m
        };

        _negotiationRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Negotiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation n, CancellationToken ct) => n);

        // Act
        var result = await _negotiationService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(dto.CustomerId);
        result.Code.Should().NotBeNullOrEmpty();
        result.Code.Length.Should().Be(10);
    }

    [Fact]
    public async Task CreateAsync_ValidDtoWithItems_ShouldCreateNegotiationAndItems()
    {
        // Arrange
        var dto = new CreateNegotiationDto
        {
            CustomerId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            GrossValue = 1000m,
            NetValue = 900m,
            DiscountValue = 100m,
            Items = new List<NegotiationItemInputDto>
            {
                new NegotiationItemInputDto
                {
                    Type = NegotiationItemType.Product,
                    Quantity = 2,
                    GrossValueUnit = 100m,
                    NetValue = 180m,
                    DiscountValue = 20m
                },
                new NegotiationItemInputDto
                {
                    Type = NegotiationItemType.Service,
                    Quantity = 1,
                    GrossValueUnit = 200m,
                    NetValue = 180m,
                    DiscountValue = 20m
                }
            }
        };

        _negotiationRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Negotiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation n, CancellationToken ct) => n);

        // Act
        var result = await _negotiationService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(dto.CustomerId);
        result.Code.Should().NotBeNullOrEmpty();
        result.Code.Length.Should().Be(10);

        // Verify that items were created
        _negotiationItemRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<NegotiationItem>(), It.IsAny<CancellationToken>()),
            Times.Exactly(dto.Items.Count));
    }

    [Fact]
    public async Task GetByCodeAsync_ValidCode_ShouldReturnNegotiation()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByCodeAsync(negotiation.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        // Act
        var result = await _negotiationService.GetByCodeAsync(negotiation.Code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(negotiation.Code);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnNegotiation()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        // Act
        var result = await _negotiationService.GetByIdAsync(negotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(negotiation.Id);
        result.CustomerId.Should().Be(negotiation.CustomerId);
    }

    [Fact]
    public async Task GetByIdAsync_NegotiationNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationService.GetByIdAsync(negotiationId)
        );
    }

    [Fact]
    public async Task GetByIdWithItemsAsync_ValidId_ShouldReturnNegotiationWithItems()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        var items = new List<NegotiationItem>
        {
            new NegotiationItem(
                negotiationId,
                NegotiationItemType.Product,
                2,
                100m,
                180m,
                20m
            ),
            new NegotiationItem(
                negotiationId,
                NegotiationItemType.Service,
                1,
                200m,
                180m,
                20m
            )
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _negotiationService.GetByIdWithItemsAsync(negotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(negotiation.Id);
        result.Items.Should().HaveCount(2);
        result.Items.First().Type.Should().Be(NegotiationItemType.Product);
        result.Items.Last().Type.Should().Be(NegotiationItemType.Service);
    }

    [Fact]
    public async Task GetByCodeWithItemsAsync_ValidCode_ShouldReturnNegotiationWithItems()
    {
        // Arrange
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        var items = new List<NegotiationItem>
        {
            new NegotiationItem(
                negotiation.Id,
                NegotiationItemType.Product,
                1,
                100m,
                90m,
                10m
            )
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByCodeAsync(negotiation.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(negotiation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _negotiationService.GetByCodeWithItemsAsync(negotiation.Code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(negotiation.Code);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByCodeWithItemsAsync_InvalidCode_ShouldReturnNull()
    {
        // Arrange
        var code = "INVALIDCODE";

        _negotiationRepositoryMock
            .Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act
        var result = await _negotiationService.GetByCodeWithItemsAsync(code);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNegotiations()
    {
        // Arrange
        var negotiations = new List<Negotiation>
        {
            new Negotiation(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 1000m, 900m, 100m),
            new Negotiation(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 2000m, 1800m, 200m)
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiations);

        // Act
        var result = await _negotiationService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ValidCustomerId_ShouldReturnNegotiations()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var negotiations = new List<Negotiation>
        {
            new Negotiation(customerId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 1000m, 900m, 100m),
            new Negotiation(customerId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 2000m, 1800m, 200m)
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiations);

        // Act
        var result = await _negotiationService.GetByCustomerIdAsync(customerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(n => n.CustomerId == customerId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserIdAsync_ValidUserId_ShouldReturnNegotiations()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var negotiations = new List<Negotiation>
        {
            new Negotiation(Guid.NewGuid(), userId, DateTime.UtcNow.AddDays(30), 1000m, 900m, 100m),
            new Negotiation(Guid.NewGuid(), userId, DateTime.UtcNow.AddDays(30), 2000m, 1800m, 200m)
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiations);

        // Act
        var result = await _negotiationService.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(n => n.UserId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdCompleteAsync_ValidId_ShouldReturnNegotiationWithItemsAndPaymentMethods()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        var items = new List<NegotiationItem>
        {
            new NegotiationItem(
                negotiationId,
                NegotiationItemType.Product,
                2,
                100m,
                180m,
                20m
            )
        };

        var paymentMethods = new List<NegotiationPaymentMethod>
        {
            new NegotiationPaymentMethod(
                negotiationId,
                Guid.NewGuid(),
                500m
            )
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationItemRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _negotiationPaymentMethodRepositoryMock
            .Setup(x => x.GetByNegotiationIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _negotiationService.GetByIdCompleteAsync(negotiationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(negotiation.Id);
        result.Items.Should().HaveCount(1);
        result.PaymentMethods.Should().HaveCount(1);
        result.PaymentMethods.First().Value.Should().Be(500m);
    }

    [Fact]
    public async Task GetByIdCompleteAsync_NegotiationNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _negotiationService.GetByIdCompleteAsync(negotiationId)
        );
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateNegotiation()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var existingNegotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        var dto = new UpdateNegotiationDto
        {
            ExpirationDate = DateTime.UtcNow.AddDays(60),
            GrossValue = 2000m,
            NetValue = 1800m,
            DiscountValue = 200m
        };

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNegotiation);

        _negotiationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Negotiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Negotiation n, CancellationToken ct) => n);

        // Act
        var result = await _negotiationService.UpdateAsync(negotiationId, dto);

        // Assert
        result.Should().NotBeNull();
        _negotiationRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Negotiation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteNegotiation()
    {
        // Arrange
        var negotiationId = Guid.NewGuid();
        var negotiation = new Negotiation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(30),
            1000m,
            900m,
            100m
        );

        _negotiationRepositoryMock
            .Setup(x => x.GetByIdAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(negotiation);

        _negotiationRepositoryMock
            .Setup(x => x.DeleteAsync(negotiationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _negotiationService.DeleteAsync(negotiationId);

        // Assert
        result.Should().BeTrue();
        _negotiationRepositoryMock.Verify(x => x.DeleteAsync(negotiationId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

