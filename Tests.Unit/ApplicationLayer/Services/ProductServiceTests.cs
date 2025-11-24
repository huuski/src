using ApplicationLayer.DTOs.Product;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;
using FluentAssertions;
using Moq;

namespace Tests.Unit.ApplicationLayer.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be(product.Name);
        result.Description.Should().Be(product.Description);
        result.Category.Should().Be(product.Category);
        result.Amount.Should().Be(product.Amount);
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _productService.GetByIdAsync(productId)
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateTestProduct(Guid.NewGuid()),
            CreateTestProduct(Guid.NewGuid())
        };

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ShouldCreateProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Moisturizing Cream",
            Description = "Hydrating face cream",
            Category = ProductCategory.Cosmetic,
            Amount = 50.00m,
            Image = "https://example.com/image.jpg"
        };

        var createdProduct = CreateTestProduct(Guid.NewGuid(), dto.Name, dto.Description, dto.Category, dto.Amount, dto.Image);

        _productRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        // Act
        var result = await _productService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.Category.Should().Be(dto.Category);
        result.Amount.Should().Be(dto.Amount);
        _productRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidDto_ShouldUpdateProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = CreateTestProduct(productId);

        var dto = new UpdateProductDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Category = ProductCategory.Medication,
            Amount = 200.00m,
            Image = "https://example.com/updated-image.jpg"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        // Act
        var result = await _productService.UpdateAsync(productId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.Category.Should().Be(dto.Category);
        result.Amount.Should().Be(dto.Amount);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Category = ProductCategory.Cosmetic,
            Amount = 200.00m
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _productService.UpdateAsync(productId, dto)
        );
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ShouldDeleteProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _productService.DeleteAsync(productId)
        );
    }

    private Product CreateTestProduct(
        Guid id,
        string name = "Test Product",
        string description = "Test Description",
        ProductCategory category = ProductCategory.Cosmetic,
        decimal amount = 100.00m,
        string? image = null)
    {
        var product = new Product(
            name,
            description,
            category,
            amount,
            image
        );

        // Set the ID using reflection
        var idProperty = typeof(Entity).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product, id);
        }

        return product;
    }
}

