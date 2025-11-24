using DomainLayer.Enums;

namespace ApplicationLayer.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? Image { get; set; }
}

