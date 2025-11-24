using DomainLayer.Enums;

namespace DomainLayer.Entities;

public class NegotiationItem : Entity
{
    public Guid NegotiationId { get; private set; }
    public NegotiationItemType Type { get; private set; }
    public int Quantity { get; private set; }
    public decimal GrossValueUnit { get; private set; }
    public decimal GrossValue { get; private set; }
    public decimal NetValue { get; private set; }
    public decimal DiscountValue { get; private set; }

    private NegotiationItem()
    {
        // For ORM
    }

    public NegotiationItem(
        Guid negotiationId,
        NegotiationItemType type,
        int quantity,
        decimal grossValueUnit,
        decimal netValue,
        decimal discountValue)
        : base()
    {
        NegotiationId = negotiationId;
        Type = type;
        Quantity = quantity;
        GrossValueUnit = grossValueUnit;
        NetValue = netValue;
        DiscountValue = discountValue;
        GrossValue = grossValueUnit * quantity;

        Validate();
    }

    public void Update(
        NegotiationItemType type,
        int quantity,
        decimal grossValueUnit,
        decimal netValue,
        decimal discountValue)
    {
        Type = type;
        Quantity = quantity;
        GrossValueUnit = grossValueUnit;
        NetValue = netValue;
        DiscountValue = discountValue;
        GrossValue = grossValueUnit * quantity;

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (NegotiationId == Guid.Empty)
            throw new ArgumentException("NegotiationId cannot be empty", nameof(NegotiationId));

        if (Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(Quantity));

        if (GrossValueUnit < 0)
            throw new ArgumentException("GrossValueUnit cannot be negative", nameof(GrossValueUnit));

        if (NetValue < 0)
            throw new ArgumentException("NetValue cannot be negative", nameof(NetValue));

        if (DiscountValue < 0)
            throw new ArgumentException("DiscountValue cannot be negative", nameof(DiscountValue));

        if (NetValue > GrossValue)
            throw new ArgumentException("NetValue cannot be greater than GrossValue", nameof(NetValue));
    }
}

