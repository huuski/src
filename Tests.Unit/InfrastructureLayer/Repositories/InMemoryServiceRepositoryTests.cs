using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryServiceRepositoryTests
{
    private readonly InMemoryServiceRepository _repository;

    public InMemoryServiceRepositoryTests()
    {
        _repository = new InMemoryServiceRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidService_ShouldCreateService()
    {
        // Arrange
        var service = new Service(
            "Test Service",
            "Test Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );

        // Act
        var result = await _repository.CreateAsync(service);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(service.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingService_ShouldReturnService()
    {
        // Arrange
        var service = new Service(
            "Test Service",
            "Test Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );
        await _repository.CreateAsync(service);

        // Act
        var result = await _repository.GetByIdAsync(service.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(service.Id);
        result.Name.Should().Be(service.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentService_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllServices()
    {
        // Arrange
        var service1 = new Service(
            "Service 1",
            "Description 1",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );
        var service2 = new Service(
            "Service 2",
            "Description 2",
            ServiceCategory.Aesthetical,
            200.00m,
            TimeSpan.FromMinutes(60)
        );

        await _repository.CreateAsync(service1);
        await _repository.CreateAsync(service2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingService_ShouldUpdateService()
    {
        // Arrange
        var service = new Service(
            "Old Name",
            "Old Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );
        await _repository.CreateAsync(service);

        service.Update(
            "New Name",
            "New Description",
            ServiceCategory.Aesthetical,
            200.00m,
            TimeSpan.FromMinutes(60)
        );

        // Act
        var result = await _repository.UpdateAsync(service);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task DeleteAsync_ExistingService_ShouldMarkAsDeleted()
    {
        // Arrange
        var service = new Service(
            "Test Service",
            "Test Description",
            ServiceCategory.Medical,
            100.00m,
            TimeSpan.FromMinutes(30)
        );
        await _repository.CreateAsync(service);

        // Act
        var result = await _repository.DeleteAsync(service.Id);

        // Assert
        result.Should().BeTrue();
        var deletedService = await _repository.GetByIdAsync(service.Id);
        deletedService.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentService_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }
}

