using ApplicationLayer.DTOs.Product;
using ApplicationLayer.Interfaces.Repositories;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Entities;
using DomainLayer.Enums;

namespace ApplicationLayer.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new ArgumentException($"Product with id {id} not found", nameof(id));

        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var product = new Product(
            dto.Name,
            dto.Description,
            dto.Category,
            dto.Amount,
            dto.Image
        );

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new ArgumentException($"Product with id {id} not found", nameof(id));

        product.Update(
            dto.Name,
            dto.Description,
            dto.Category,
            dto.Amount,
            dto.Image
        );

        var updatedProduct = await _productRepository.UpdateAsync(product, cancellationToken);
        return MapToDto(updatedProduct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            throw new ArgumentException($"Product with id {id} not found", nameof(id));

        return await _productRepository.DeleteAsync(id, cancellationToken);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Amount = product.Amount,
            Image = product.Image,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

