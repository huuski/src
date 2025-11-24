namespace DomainLayer.Entities;

public class NegotiationPaymentMethod : Entity
{
    public Guid NegotiationId { get; private set; }
    public Guid PaymentMethodId { get; private set; }
    public decimal Value { get; private set; }

    private NegotiationPaymentMethod()
    {
        // For ORM
    }

    public NegotiationPaymentMethod(
        Guid negotiationId,
        Guid paymentMethodId,
        decimal value)
        : base()
    {
        NegotiationId = negotiationId;
        PaymentMethodId = paymentMethodId;
        Value = value;

        Validate();
    }

    public void Update(decimal value)
    {
        Value = value;

        Validate();
        MarkAsUpdated();
    }

    private void Validate()
    {
        if (NegotiationId == Guid.Empty)
            throw new ArgumentException("NegotiationId cannot be empty", nameof(NegotiationId));

        if (PaymentMethodId == Guid.Empty)
            throw new ArgumentException("PaymentMethodId cannot be empty", nameof(PaymentMethodId));

        if (Value <= 0)
            throw new ArgumentException("Value must be greater than zero", nameof(Value));
    }
}

