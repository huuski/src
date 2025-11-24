using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using InfrastructureLayer.Repositories;

namespace Tests.Unit.InfrastructureLayer.Repositories;

public class InMemoryProductRepositoryTests
{
    private readonly InMemoryProductRepository _repository;

    public InMemoryProductRepositoryTests()
    {
        _repository = new InMemoryProductRepository();
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ShouldCreateProduct()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            "Test Description",
            ProductCategory.Cosmetic,
            100.00m
        );

        // Act
        var result = await _repository.CreateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            "Test Description",
            ProductCategory.Cosmetic,
            100.00m
        );
        await _repository.CreateAsync(product);

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var product1 = new Product(
            "Product 1",
            "Description 1",
            ProductCategory.Cosmetic,
            100.00m
        );
        var product2 = new Product(
            "Product 2",
            "Description 2",
            ProductCategory.Medication,
            200.00m
        );

        await _repository.CreateAsync(product1);
        await _repository.CreateAsync(product2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product(
            "Old Name",
            "Old Description",
            ProductCategory.Cosmetic,
            100.00m
        );
        await _repository.CreateAsync(product);

        product.Update(
            "New Name",
            "New Description",
            ProductCategory.Medication,
            200.00m
        );

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_ShouldMarkAsDeleted()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            "Test Description",
            ProductCategory.Cosmetic,
            100.00m
        );
        await _repository.CreateAsync(product);

        // Act
        var result = await _repository.DeleteAsync(product.Id);

        // Assert
        result.Should().BeTrue();
        var deletedProduct = await _repository.GetByIdAsync(product.Id);
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentProduct_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }
}

