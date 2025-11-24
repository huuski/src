using ApplicationLayer.DTOs.Service;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class ServiceServiceTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly IServiceService _serviceService;

    public ServiceServiceTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _serviceService = new ServiceService(_serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnServiceDto()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var service = CreateTestService(serviceId);

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        // Act
        var result = await _serviceService.GetByIdAsync(serviceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(serviceId);
        result.Name.Should().Be(service.Name);
        result.Description.Should().Be(service.Description);
        result.Category.Should().Be(service.Category);
        result.Amount.Should().Be(service.Amount);
        result.Duration.Should().Be(service.Duration);
    }

    [Fact]
    public async Task GetByIdAsync_ServiceNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var serviceId = Guid.NewGuid();

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _serviceService.GetByIdAsync(serviceId)
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllServices()
    {
        // Arrange
        var services = new List<Service>
        {
            CreateTestService(Guid.NewGuid()),
            CreateTestService(Guid.NewGuid())
        };

        _serviceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(services);

        // Act
        var result = await _serviceService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateService()
    {
        // Arrange
        var dto = new CreateServiceDto
        {
            Name = "Facial Treatment",
            Description = "Deep cleansing facial",
            Category = ServiceCategory.Aesthetical,
            Amount = 150.00m,
            Duration = TimeSpan.FromMinutes(60),
            Image = "https://example.com/image.jpg"
        };

        var createdService = CreateTestService(Guid.NewGuid(), dto.Name, dto.Description, dto.Category, dto.Amount, dto.Duration, dto.Image);

        _serviceRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service s, CancellationToken ct) => s);

        // Act
        var result = await _serviceService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.Category.Should().Be(dto.Category);
        result.Amount.Should().Be(dto.Amount);
        result.Duration.Should().Be(dto.Duration);
        _serviceRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateService()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var existingService = CreateTestService(serviceId);

        var dto = new UpdateServiceDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Category = ServiceCategory.Medical,
            Amount = 200.00m,
            Duration = TimeSpan.FromMinutes(90),
            Image = "https://example.com/updated-image.jpg"
        };

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingService);

        _serviceRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service s, CancellationToken ct) => s);

        // Act
        var result = await _serviceService.UpdateAsync(serviceId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.Category.Should().Be(dto.Category);
        result.Amount.Should().Be(dto.Amount);
        _serviceRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ServiceNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var dto = new UpdateServiceDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Category = ServiceCategory.Medical,
            Amount = 200.00m,
            Duration = TimeSpan.FromMinutes(90)
        };

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _serviceService.UpdateAsync(serviceId, dto)
        );
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteService()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var service = CreateTestService(serviceId);

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(service);

        _serviceRepositoryMock
            .Setup(x => x.DeleteAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _serviceService.DeleteAsync(serviceId);

        // Assert
        result.Should().BeTrue();
        _serviceRepositoryMock.Verify(x => x.DeleteAsync(serviceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ServiceNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var serviceId = Guid.NewGuid();

        _serviceRepositoryMock
            .Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Service?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _serviceService.DeleteAsync(serviceId)
        );
    }

    private Service CreateTestService(
        Guid id,
        string name = "Test Service",
        string description = "Test Description",
        ServiceCategory category = ServiceCategory.Medical,
        decimal amount = 100.00m,
        TimeSpan? duration = null,
        string? image = null)
    {
        var service = new Service(
            name,
            description,
            category,
            amount,
            duration ?? TimeSpan.FromMinutes(30),
            image
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(service, id);
        }

        return service;
    }
}

