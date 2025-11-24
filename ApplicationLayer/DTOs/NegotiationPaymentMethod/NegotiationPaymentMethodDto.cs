namespace ApplicationLayer.DTOs.NegotiationPaymentMethod;

public class NegotiationPaymentMethodDto
{
    public Guid Id { get; set; }
    public Guid NegotiationId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

