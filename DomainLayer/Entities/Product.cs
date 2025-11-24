using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ProductCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public string? Image { get; private set; }

    private Product()
    {
        // For ORM
    }

    public Product(
        string name,
        string description,
        ProductCategory category,
        decimal amount,
        string? image = null)
        : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category;
        Amount = amount;
        Image = image;

        Validate();
    }

    public void Update(
        string name,
        string description,
        ProductCategory category,
        decimal amount,
        string? image = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category;
        Amount = amount;
        Image = image;

        Validate();
        MarkAsUpdated();
    }

    public void UpdateImage(string? image)
    {
        Image = image;
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty", nameof(Name));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty", nameof(Description));

        if (Amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(Amount));
    }
}

