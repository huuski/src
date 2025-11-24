namespace DomainLayer.Entities;

public class Supply : Entity
{
    public string Name { get; private set; } = string.Empty;
    public int Stock { get; private set; }

    private Supply()
    {
        // For ORM
    }

    public Supply(string name, int stock) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Stock = stock;
        Validate();
    }

    public void Update(string name, int stock)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Stock = stock;
        Validate();
        MarkAsUpdated();
    }

    public void UpdateStock(int stock)
    {
        Stock = stock;
        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty", nameof(Name));

        if (Stock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(Stock));
    }
}

