using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class PaymentMethod : Entity
{
    public string Name { get; private set; } = string.Empty;
    public PaymentMethodType Type { get; private set; }
    public bool Inactive { get; private set; }

    private PaymentMethod()
    {
        // For ORM
    }

    public PaymentMethod(
        string name,
        PaymentMethodType type)
        : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;

        Validate();
    }

    public void Update(
        string name,
        PaymentMethodType type)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;

        Validate();
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (Inactive)
        {
            Inactive = false;
            MarkAsUpdated();
        }
    }

    public void Deactivate()
    {
        if (!Inactive)
        {
            Inactive = true;
            MarkAsUpdated();
        }
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty", nameof(Name));
    }
}

